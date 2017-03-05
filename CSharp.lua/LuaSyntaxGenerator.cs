/*
Copyright 2016 YANG Huan (sy.yanghuan@gmail.com).
Copyright 2016 Redmoon Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using CSharpLua.LuaAst;

namespace CSharpLua {
    internal sealed class PartialTypeDeclaration : IComparable<PartialTypeDeclaration> {
        public INamedTypeSymbol Symbol;
        public TypeDeclarationSyntax Node;
        public LuaTypeDeclarationSyntax TypeDeclaration;
        public LuaCompilationUnitSyntax CompilationUnit;

        public int CompareTo(PartialTypeDeclaration other) {
            string filePath = CompilationUnit.FilePath;
            string otherFilePath = other.CompilationUnit.FilePath;

            if(filePath.Contains(otherFilePath)) {
                return 1;
            }
            else if(otherFilePath.Contains(filePath)) {
                return -1;
            }
            else {
                return other.Node.Members.Count.CompareTo(Node.Members.Count);
            }
        }
    }

    public sealed class LuaSyntaxGenerator {
        public sealed class SettingInfo {
            public bool HasSemicolon { get; set; }
            private int indent_;
            public string IndentString { get; private set; }
            public bool IsNewest { get; set; }

            public SettingInfo() {
                Indent = 4;
                HasSemicolon = false;
                IsNewest = true;
            }

            public int Indent {
                get {
                    return indent_;
                }
                set {
                    if(value > 0 && indent_ != value) {
                        indent_ = value;
                        IndentString = new string(' ', indent_);
                    }
                }
            }
        }

        private const string kLuaSuffix = ".lua";
        private static readonly Encoding Encoding = new UTF8Encoding(false);

        private CSharpCompilation compilation_;
        public XmlMetaProvider XmlMetaProvider { get; }
        public SettingInfo Setting { get; set; }
        private HashSet<string> exportEnums_ = new HashSet<string>();
        private bool isExportAttributesAll_;
        private HashSet<string> exportAttributes_;
        private List<LuaEnumDeclarationSyntax> enumDeclarations_ = new List<LuaEnumDeclarationSyntax>();
        private Dictionary<INamedTypeSymbol, List<PartialTypeDeclaration>> partialTypes_ = new Dictionary<INamedTypeSymbol, List<PartialTypeDeclaration>>();

        public LuaSyntaxGenerator(IEnumerable<SyntaxTree> syntaxTrees, IEnumerable<MetadataReference> references, IEnumerable<string> metas, SettingInfo setting, string[] attributes) {
            CSharpCompilation compilation = CSharpCompilation.Create("_", syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using(MemoryStream ms = new MemoryStream()) {
                EmitResult result = compilation.Emit(ms);
                if(!result.Success) {
                    var errors = result.Diagnostics.Where(i => i.Severity == DiagnosticSeverity.Error);
                    string message = string.Join("\n", errors);
                    throw new CompilationErrorException(message);
                }
            }
            compilation_ = compilation;
            XmlMetaProvider = new XmlMetaProvider(metas);
            Setting = setting;
            if(attributes != null) {
                if(attributes.Length == 0) {
                    isExportAttributesAll_ = true;
                }
                else {
                    exportAttributes_ = new HashSet<string>(attributes);
                }
            }
            CheckIndirecInterfacePropertyFields();
        }

        private IEnumerable<LuaCompilationUnitSyntax> Create() {
            List<LuaCompilationUnitSyntax> luaCompilationUnits = new List<LuaCompilationUnitSyntax>();
            foreach(SyntaxTree syntaxTree in compilation_.SyntaxTrees) {
                SemanticModel semanticModel = GetSemanticModel(syntaxTree);
                CompilationUnitSyntax compilationUnitSyntax = (CompilationUnitSyntax)syntaxTree.GetRoot();
                LuaSyntaxNodeTransfor transfor = new LuaSyntaxNodeTransfor(this, semanticModel);
                var luaCompilationUnit = (LuaCompilationUnitSyntax)compilationUnitSyntax.Accept(transfor);
                luaCompilationUnits.Add(luaCompilationUnit);
            }
            CheckExportEnums();
            CheckPartialTypes();
            CheckRefactorNames();
            return luaCompilationUnits.Where(i => !i.IsEmpty);
        }

        private void Write(LuaCompilationUnitSyntax luaCompilationUnit, string outFile) {
            using(var writer = new StreamWriter(outFile, false, Encoding)) {
                LuaRenderer rener = new LuaRenderer(this, writer);
                luaCompilationUnit.Render(rener);
            }
        }

        public void Generate(string baseFolder, string outFolder) {
            List<string> modules = new List<string>();
            foreach(var luaCompilationUnit in Create()) {
                string module;
                string outFile = GetOutFilePath(luaCompilationUnit.FilePath, baseFolder, outFolder, out module);
                Write(luaCompilationUnit, outFile);
                modules.Add(module);
            }
            ExportManifestFile(modules, outFolder);
        }

        private string GetOutFilePath(string inFilePath, string folder_, string output_, out string module) {
            string path = inFilePath.Remove(0, folder_.Length).TrimStart(Path.DirectorySeparatorChar, '/');
            string extend = Path.GetExtension(path);
            path = path.Remove(path.Length - extend.Length, extend.Length);
            path = path.Replace('.', '_');
            string outPath = Path.Combine(output_, path + kLuaSuffix);
            string dir = Path.GetDirectoryName(outPath);
            if(!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            module = path.Replace(Path.DirectorySeparatorChar, '.');
            return outPath;
        }

        internal bool IsEnumExport(string enumTypeSymbol) {
            return exportEnums_.Contains(enumTypeSymbol);
        }

        internal void AddExportEnum(string enumTypeSymbol) {
            exportEnums_.Add(enumTypeSymbol);
        }

        internal void AddEnumDeclaration(LuaEnumDeclarationSyntax enumDeclaration) {
            enumDeclarations_.Add(enumDeclaration);
        }

        internal bool IsExportAttribute(INamedTypeSymbol attributeTypeSymbol) {
            if(isExportAttributesAll_) {
                return true;
            }
            else {
                if(exportAttributes_ != null && exportAttributes_.Count > 0) {
                    if(exportAttributes_.Contains(attributeTypeSymbol.ToString())) {
                        return true;
                    }
                }
                if(XmlMetaProvider.IsExportAttribute(attributeTypeSymbol)) {
                    return true;
                }
            }
            return false;
        }

        private void CheckExportEnums() {
            foreach(var enumDeclaration in enumDeclarations_) {
                if(IsEnumExport(enumDeclaration.FullName)) {
                    enumDeclaration.IsExport = true;
                    enumDeclaration.CompilationUnit.AddTypeDeclarationCount();
                }
            }
        }

        internal void AddPartialTypeDeclaration(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax luaNode, LuaCompilationUnitSyntax compilationUnit) {
            var list = partialTypes_.GetOrDefault(typeSymbol);
            if(list == null) {
                list = new List<PartialTypeDeclaration>();
                partialTypes_.Add(typeSymbol, list);
            }
            list.Add(new PartialTypeDeclaration() {
                Symbol = typeSymbol,
                Node = node,
                TypeDeclaration = luaNode,
                CompilationUnit = compilationUnit,
            });
        }

        private void CheckPartialTypes() {
            while(partialTypes_.Count > 0) {
                var types = partialTypes_.Values.ToArray();
                partialTypes_.Clear();
                foreach(var typeDeclarations in types) {
                    PartialTypeDeclaration major = typeDeclarations.Min();
                    LuaSyntaxNodeTransfor transfor = new LuaSyntaxNodeTransfor(this, null);
                    transfor.AcceptPartialType(major, typeDeclarations);
                }
            }
        }

        internal SemanticModel GetSemanticModel(SyntaxTree syntaxTree) {
            return compilation_.GetSemanticModel(syntaxTree);
        }

        internal bool IsBaseType(BaseTypeSyntax type) {
            var syntaxTree = type.SyntaxTree;
            SemanticModel semanticModel = GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetTypeInfo(type.Type).Type;
            Contract.Assert(symbol != null);
            return symbol.TypeKind != TypeKind.Interface;
        }

        internal LuaIdentifierNameSyntax GetMethodName(IMethodSymbol symbol) {
            return GetMemberMethodName(symbol);
        }

        private bool IsTypeEnable(INamedTypeSymbol type) {
            if(type.TypeKind == TypeKind.Enum) {
                return IsEnumExport(type.ToString());
            }
            return true;
        }

        private void AddBaseTypeTo(HashSet<INamedTypeSymbol> parentTypes, INamedTypeSymbol rootType, INamedTypeSymbol baseType) {
            if(baseType.IsFromCode()) {
                if(baseType.IsGenericType) {
                    parentTypes.Add(baseType.OriginalDefinition);
                    foreach(var typeArgument in baseType.TypeArguments) {
                        if(typeArgument.Kind != SymbolKind.TypeParameter) {
                            if(!rootType.IsAssignableFrom(typeArgument)) {
                                INamedTypeSymbol typeArgumentType = (INamedTypeSymbol)typeArgument;
                                AddBaseTypeTo(parentTypes, rootType, typeArgumentType);
                            }
                        }
                    }
                }
                else {
                    parentTypes.Add(baseType);
                }
            }
        }

        private List<INamedTypeSymbol> GetExportTypes() {
            List<INamedTypeSymbol> allTypes = new List<INamedTypeSymbol>();
            if(types_.Count > 0) {
                types_.Sort((x, y) => x.ToString().CompareTo(y.ToString()));

                List<List<INamedTypeSymbol>> typesList = new List<List<INamedTypeSymbol>>();
                typesList.Add(types_);

                while(true) {
                    HashSet<INamedTypeSymbol> parentTypes = new HashSet<INamedTypeSymbol>();
                    var lastTypes = typesList.Last();
                    foreach(var type in lastTypes) {
                        if(type.BaseType != null) {
                            AddBaseTypeTo(parentTypes, type, type.BaseType);
                        }

                        foreach(var interfaceType in type.Interfaces) {
                            AddBaseTypeTo(parentTypes, type, interfaceType);
                        }
                    }

                    if(parentTypes.Count == 0) {
                        break;
                    }

                    typesList.Add(parentTypes.ToList());
                }

                typesList.Reverse();
                var types = typesList.SelectMany(i => i).Distinct().Where(IsTypeEnable);
                allTypes.AddRange(types);
            }
            return allTypes;
        }

        private void ExportManifestFile(List<string> modules, string outFolder) {
            const string kDir = "dir";
            const string kDirInitCode = "dir = (dir and #dir > 0) and (dir .. '.') or \"\"";
            const string kRequire = "require";
            const string kLoadCode = "local load = function(module) return require(dir .. module) end";
            const string kLoad = "load";
            const string kInit = "System.init";
            const string kManifestFile = "manifest.lua";

            if(modules.Count > 0) {
                modules.Sort();
                var types = GetExportTypes();
                if(types.Count > 0) {
                    LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
                    functionExpression.AddParameter(new LuaIdentifierNameSyntax(kDir));
                    functionExpression.AddStatement(new LuaIdentifierNameSyntax(kDirInitCode));

                    LuaIdentifierNameSyntax requireIdentifier = new LuaIdentifierNameSyntax(kRequire);
                    functionExpression.AddStatement(new LuaLocalVariableDeclaratorSyntax(requireIdentifier, requireIdentifier));

                    functionExpression.AddStatement(new LuaIdentifierNameSyntax(kLoadCode));
                    functionExpression.AddStatement(LuaBlankLinesStatement.One);

                    LuaIdentifierNameSyntax loadIdentifier = new LuaIdentifierNameSyntax(kLoad);
                    foreach(string module in modules) {
                        var argument = new LuaStringLiteralExpressionSyntax(new LuaIdentifierNameSyntax(module));
                        var invocation = new LuaInvocationExpressionSyntax(loadIdentifier, argument);
                        functionExpression.AddStatement(invocation);
                    }
                    functionExpression.AddStatement(LuaBlankLinesStatement.One);

                    LuaTableInitializerExpression table = new LuaTableInitializerExpression();
                    foreach(var type in types) {
                        LuaIdentifierNameSyntax typeName = XmlMetaProvider.GetTypeShortName(type);
                        table.Items.Add(new LuaSingleTableItemSyntax(new LuaStringLiteralExpressionSyntax(typeName)));
                    }
                    functionExpression.AddStatement(new LuaInvocationExpressionSyntax(new LuaIdentifierNameSyntax(kInit), table));

                    LuaCompilationUnitSyntax luaCompilationUnit = new LuaCompilationUnitSyntax();
                    luaCompilationUnit.Statements.Add(new LuaReturnStatementSyntax(functionExpression));

                    string outFile = Path.Combine(outFolder, kManifestFile);
                    Write(luaCompilationUnit, outFile);
                }
            }
        }

        #region     // member name refactor

        private Dictionary<ISymbol, LuaSymbolNameSyntax> memberNames_ = new Dictionary<ISymbol, LuaSymbolNameSyntax>();
        private Dictionary<INamedTypeSymbol, HashSet<string>> typeNameUseds_ = new Dictionary<INamedTypeSymbol, HashSet<string>>();
        private HashSet<ISymbol> refactorNames_ = new HashSet<ISymbol>();
        private Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> extends_ = new Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>>();
        private List<INamedTypeSymbol> types_ = new List<INamedTypeSymbol>();

        internal void AddTypeSymbol(INamedTypeSymbol typeSymbol) {
            types_.Add(typeSymbol);
            CheckExtends(typeSymbol);
        }

        private void CheckExtends(INamedTypeSymbol typeSymbol) {
            if(typeSymbol.SpecialType != SpecialType.System_Object) {
                if(typeSymbol.BaseType != null) {
                    var super = typeSymbol.BaseType;
                    TryAddExtend(super, typeSymbol);
                }
            }

            foreach(INamedTypeSymbol super in typeSymbol.AllInterfaces) {
                TryAddExtend(super, typeSymbol);
            }
        }

        private void TryAddExtend(INamedTypeSymbol super, INamedTypeSymbol children) {
            if(super.IsFromCode()) {
                if(super.IsGenericType) {
                    super = super.OriginalDefinition;
                }
                var set = extends_.GetOrDefault(super);
                if(set == null) {
                    set = new HashSet<INamedTypeSymbol>();
                    extends_.Add(super, set);
                }
                set.Add(children);
            }
        }

        internal LuaIdentifierNameSyntax GetMemberMethodName(IMethodSymbol symbol) {
            Utility.CheckOriginalDefinition(ref symbol);
            var name = memberNames_.GetOrDefault(symbol);
            if(name == null) {
                var identifierName = InternalGetMemberMethodName(symbol);
                LuaSymbolNameSyntax symbolName = new LuaSymbolNameSyntax(identifierName);
                memberNames_.Add(symbol, symbolName);
                name = symbolName;
            }
            return name;
        }

        private LuaIdentifierNameSyntax InternalGetMemberMethodName(IMethodSymbol symbol) {
            string name = XmlMetaProvider.GetMethodMapName(symbol);
            if(name != null) {
                return new LuaIdentifierNameSyntax(name);
            }

            if(!symbol.IsFromCode()) {
                return new LuaIdentifierNameSyntax(symbol.Name);
            }

            if(symbol.IsExtensionMethod) {
                return GetExtensionMethodName(symbol);
            }

            if(symbol.IsStatic) {
                if(symbol.ContainingType.IsStatic) {
                    return GetStaticClassMethodName(symbol);
                }
            }

            if(symbol.ContainingType.TypeKind == TypeKind.Interface) {
                return new LuaIdentifierNameSyntax(symbol.Name);
            }

            while(symbol.OverriddenMethod != null) {
                symbol = symbol.OverriddenMethod;
            }

            List<ISymbol> sameNameMembers = GetSameNameMembers(symbol);
            LuaIdentifierNameSyntax symbolExpression = null;
            int index = 0;
            foreach(ISymbol member in sameNameMembers) {
                if(member.Equals(symbol)) {
                    symbolExpression = new LuaIdentifierNameSyntax(GetSymbolName(symbol));
                }
                else {
                    if(!memberNames_.ContainsKey(member)) {
                        LuaIdentifierNameSyntax identifierName = new LuaIdentifierNameSyntax(GetSymbolName(member));
                        memberNames_.Add(member, new LuaSymbolNameSyntax(identifierName));
                    }
                }
                if(index > 0) {
                    if(member.ContainingType.IsFromCode()) {
                        ISymbol refactorSymbol = member;
                        Utility.CheckOriginalDefinition(ref refactorSymbol);
                        refactorNames_.Add(refactorSymbol);
                    }
                }
                ++index;
            }

            if(symbolExpression == null) {
                throw new InvalidOperationException();
            }
            return symbolExpression;
        }

        private string GetSymbolName(ISymbol symbol) {
            if(symbol.Kind == SymbolKind.Method) {
                IMethodSymbol method = (IMethodSymbol)symbol;
                string name = XmlMetaProvider.GetMethodMapName(method);
                if(name != null) {
                    return name;
                }

                if(!method.ExplicitInterfaceImplementations.IsEmpty) {
                    return method.ExplicitInterfaceImplementations[0].Name;
                }
            }
            return symbol.Name;
        }

        private LuaIdentifierNameSyntax GetExtensionMethodName(IMethodSymbol symbol) {
            Contract.Assert(symbol.IsExtensionMethod);
            return GetStaticClassMethodName(symbol);
        }

        private LuaIdentifierNameSyntax GetStaticClassMethodName(IMethodSymbol symbol) {
            Contract.Assert(symbol.ContainingType.IsStatic);
            var sameNameMembers = symbol.ContainingType.GetMembers(symbol.Name);
            LuaIdentifierNameSyntax symbolExpression = null;

            int index = 0;
            foreach(ISymbol member in sameNameMembers) {
                LuaIdentifierNameSyntax identifierName = GetMethodNameFromIndex(symbol, index);
                if(member.Equals(symbol)) {
                    symbolExpression = identifierName;
                }
                else {
                    if(!memberNames_.ContainsKey(member)) {
                        memberNames_.Add(member, new LuaSymbolNameSyntax(identifierName));
                    }
                }
                ++index;
            }

            if(symbolExpression == null) {
                throw new InvalidOperationException();
            }
            return symbolExpression;
        }

        private LuaIdentifierNameSyntax GetMethodNameFromIndex(ISymbol symbol, int index) {
            Contract.Assert(index != -1);
            if(index == 0) {
                return new LuaIdentifierNameSyntax(symbol.Name);
            }
            else {
                while(true) {
                    string newName = symbol.Name + index;
                    if(symbol.ContainingType.GetMembers(newName).IsEmpty) {
                        if(TryAddNewUsedName(symbol.ContainingType, newName)) {
                            return new LuaIdentifierNameSyntax(newName);
                        }
                    }
                    ++index;
                }
            }
        }

        private bool TryAddNewUsedName(INamedTypeSymbol type, string newName) {
            var set = typeNameUseds_.GetOrDefault(type);
            if(set == null) {
                set = new HashSet<string>();
                typeNameUseds_.Add(type, set);
            }
            return set.Add(newName);
        }

        private List<ISymbol> GetSameNameMembers(ISymbol symbol) {
            List<ISymbol> members = new List<ISymbol>();
            string name = GetSymbolName(symbol);
            FillSameNameMembers(symbol.ContainingType, name, members);
            members.Sort(MemberSymbolComparison);
            return members;
        }

        private string MethodSymbolToString(IMethodSymbol symbol) {
            StringBuilder sb = new StringBuilder();
            sb.Append(symbol.Name);
            sb.Append('(');
            bool isFirst = true;
            foreach(var p in symbol.Parameters) {
                if(isFirst) {
                    isFirst = false;
                }
                else {
                    sb.Append(",");
                }
                sb.Append(p.Type.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        private string MemberSymbolToString(ISymbol symbol) {
            if(symbol.Kind == SymbolKind.Method) {
                return MethodSymbolToString((IMethodSymbol)symbol);
            }
            else {
                return symbol.Name;
            }
        }

        private int GetSymbolWright(ISymbol symbol) {
            if(symbol.Kind == SymbolKind.Method) {
                var methodSymbol = (IMethodSymbol)symbol;
                return methodSymbol.Parameters.Length;
            }
            else {
                return 0;
            }
        }

        private int MemberSymbolCommonComparison(ISymbol a, ISymbol b) {
            int weightOfA = GetSymbolWright(a);
            int weightOfB = GetSymbolWright(b);
            if(weightOfA != weightOfB) {
                return weightOfA.CompareTo(weightOfB);
            }
            else {
                string nameOfA = MemberSymbolToString(a);
                string nameOfB = MemberSymbolToString(b);
                return nameOfA.CompareTo(nameOfB);
            }
        }

        private bool MemberSymbolBoolComparison(ISymbol a, ISymbol b, Func<ISymbol, bool> boolFunc, out int v) {
            bool boolOfA = boolFunc(a);
            bool boolOfB = boolFunc(b);

            if(boolOfA) {
                if(boolOfB) {
                    v = MemberSymbolCommonComparison(a, b);
                }
                else {
                    v = -1;
                }
                return true;
            }

            if(b.IsAbstract) {
                v =  1;
                return true;
            }

            v = 0;
            return false;
        }

        private int MemberSymbolComparison(ISymbol a, ISymbol b) {
            bool isFromCodeOfA = a.ContainingType.IsFromCode();
            bool isFromCodeOfB = b.ContainingType.IsFromCode();

            if(!isFromCodeOfA) {
                if(!isFromCodeOfB) {
                    return 0;
                }
                else {
                    return -1;
                }
            }

            if(!isFromCodeOfB) {
                return 1;
            }

            int countOfA = a.InterfaceImplementations().Count();
            int countOfB = b.InterfaceImplementations().Count();
            if(countOfA > 0 || countOfB > 0) {
                if(countOfA != countOfB) {
                    return countOfA > countOfB ? -1 : 1;
                }
                else {
                    return MemberSymbolCommonComparison(a, b);
                }
            }

            int v;
            if(MemberSymbolBoolComparison(a, b, i => i.IsAbstract, out v)) {
                return v;
            }
            if(MemberSymbolBoolComparison(a, b, i => i.IsVirtual, out v)) {
                return v;
            }
            if(MemberSymbolBoolComparison(a, b, i => i.IsOverride, out v)) {
                return v;
            }

            if(a.ContainingType.Equals(b.ContainingType)) {
                string name = a.Name;
                var type = a.ContainingType;
                var members = type.GetMembers(name);
                int indexOfA = members.IndexOf(a);
                Contract.Assert(indexOfA != -1);
                int indexOfB = members.IndexOf(b);
                Contract.Assert(indexOfB != -1);
                Contract.Assert(indexOfA != indexOfB);
                return indexOfA.CompareTo(indexOfB);
            }
            else {
                bool isSubclassOf = a.ContainingType.IsSubclassOf(b.ContainingType);
                return isSubclassOf ? 1 : -1;
            }
        }

        private void FillSameNameMembers(INamedTypeSymbol typeSymbol, string name, List<ISymbol> outList) {
            if(typeSymbol.BaseType != null) {
                FillSameNameMembers(typeSymbol.BaseType, name, outList);
            }

            bool isFromCode = typeSymbol.IsFromCode();
            var members = typeSymbol.GetMembers();
            foreach(ISymbol member in members) {
                if(!isFromCode) {
                    if(member.DeclaredAccessibility == Accessibility.Private || member.DeclaredAccessibility == Accessibility.Internal) {
                        continue;
                    }
                }

                if(member.IsOverride) {
                    continue;
                }

                string memberName = GetSymbolName(member);
                if(memberName == name) {
                    outList.Add(member);
                }
            }
        }

        private void CheckRefactorNames() {
            HashSet<ISymbol> alreadyRefactorSymbols = new HashSet<ISymbol>();
            foreach(ISymbol symbol in refactorNames_) {
                bool hasImplementation = false;
                foreach(ISymbol implementation in symbol.InterfaceImplementations()) {
                    RefactorInterfaceSymbol(implementation, alreadyRefactorSymbols);
                    hasImplementation = true;
                }

                if(!hasImplementation) {
                    RefactorCurTypeSymbol(symbol, alreadyRefactorSymbols);
                }
            }
        }

        private void RefactorCurTypeSymbol(ISymbol symbol, HashSet<ISymbol> alreadyRefactorSymbols) {
            INamedTypeSymbol typeSymbol = symbol.ContainingType;
            var childrens = extends_.GetOrDefault(typeSymbol);
            string newName = GetRefactorName(typeSymbol, childrens, symbol.Name);
            RefactorName(symbol, newName, alreadyRefactorSymbols);
        }

        private void RefactorInterfaceSymbol(ISymbol symbol, HashSet<ISymbol> alreadyRefactorSymbols) {
            if(symbol.IsFromCode()) {
                INamedTypeSymbol typeSymbol = symbol.ContainingType;
                Contract.Assert(typeSymbol.TypeKind == TypeKind.Interface);
                var childrens = extends_[typeSymbol];
                string newName = GetRefactorName(null, childrens, symbol.Name);
                foreach(INamedTypeSymbol children in childrens) {
                    ISymbol childrenSymbol = children.FindImplementationForInterfaceMember(symbol);
                    Contract.Assert(childrenSymbol != null);
                    RefactorName(childrenSymbol, newName, alreadyRefactorSymbols);
                }
            }
        }

        private void RefactorName(ISymbol symbol, string newName, HashSet<ISymbol> alreadyRefactorSymbols) {
            if(!alreadyRefactorSymbols.Contains(symbol)) {
                if(symbol.IsOverridable()) {
                    RefactorChildrensOverridden(symbol, symbol.ContainingType, newName, alreadyRefactorSymbols);
                }
                UpdateName(symbol, newName, alreadyRefactorSymbols);
            }
        }

        private void RefactorChildrensOverridden(ISymbol originalSymbol, INamedTypeSymbol curType, string newName, HashSet<ISymbol> alreadyRefactorSymbols) {
            var childrens = extends_.GetOrDefault(curType);
            if(childrens != null) {
                foreach(INamedTypeSymbol children in childrens) {
                    var curSymbol = children.GetMembers(originalSymbol.Name).FirstOrDefault(i => i.IsOverridden(originalSymbol));
                    if(curSymbol != null) {
                        UpdateName(curSymbol, newName, alreadyRefactorSymbols);
                    }
                    RefactorChildrensOverridden(originalSymbol, children, newName, alreadyRefactorSymbols);
                }
            }
        }

        private void UpdateName(ISymbol symbol, string newName, HashSet<ISymbol> alreadyRefactorSymbols) {
            memberNames_[symbol].Update(newName);
            TryAddNewUsedName(symbol.ContainingType, newName);
            alreadyRefactorSymbols.Add(symbol);
        }

        private string GetRefactorName(INamedTypeSymbol typeSymbol, IEnumerable<INamedTypeSymbol> childrens, string originalName) {
            int index = 1;
            while(true) {
                string newName = originalName + index;
                bool isEnable = true;
                if(typeSymbol != null) {
                    isEnable = CheckNewNameEnable(typeSymbol, newName);
                }

                if(isEnable) {
                    if(childrens != null) {
                        isEnable = childrens.All(i => CheckNewNameEnable(i, newName));
                    }
                }

                if(isEnable) {
                    return newName;
                }
                ++index;
            }
        }

        private bool IsTypeNameUsed(INamedTypeSymbol typeSymbol, string newName) {
            var set = typeNameUseds_.GetOrDefault(typeSymbol);
            return set != null && set.Contains(newName);
        }

        private bool CheckNewNameEnable(INamedTypeSymbol typeSymbol, string newName) {
            if(typeSymbol.GetMembers(newName).IsEmpty) {
                if(IsTypeNameUsed(typeSymbol, newName)) {
                    return false;
                }
            }

            var childrens = extends_.GetOrDefault(typeSymbol);
            if(childrens != null) {
                foreach(INamedTypeSymbol children in childrens) {
                    if(!CheckNewNameEnable(children, newName)) {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
        private Dictionary<IPropertySymbol, bool> isFieldPropertys_ = new Dictionary<IPropertySymbol, bool>();

        private sealed class InterfacePeopertysChecker : CSharpSyntaxWalker {
            private SemanticModel semanticModel_;
            private HashSet<INamedTypeSymbol> classTypes_ = new HashSet<INamedTypeSymbol>();
            private HashSet<IPropertySymbol> mustBePropertys_ = new HashSet<IPropertySymbol>();

            public InterfacePeopertysChecker(CSharpCompilation compilation) {
                foreach(SyntaxTree syntaxTree in compilation.SyntaxTrees) {
                    semanticModel_ = compilation.GetSemanticModel(syntaxTree);
                    Visit(syntaxTree.GetRoot());
                }
                CheckPropertyFields();
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node) {
                var typeSymbol = semanticModel_.GetDeclaredSymbol(node);
                if(!typeSymbol.IsStatic) {
                    classTypes_.Add(typeSymbol);
                }
            }

            public void CheckPropertyFields() {
                foreach(var type in classTypes_) {
                    foreach(var baseInterface in type.AllInterfaces) {
                        foreach(IPropertySymbol property in baseInterface.GetMembers().OfType<IPropertySymbol>()) {
                            var implementationProperty = (IPropertySymbol)type.FindImplementationForInterfaceMember(property);
                            if(implementationProperty.ContainingType != type) {
                                mustBePropertys_.Add(implementationProperty);
                            }
                        }
                    }
                }
            }

            public IEnumerable<IPropertySymbol> Property {
                get {
                    return mustBePropertys_;
                }
            }
        }

        private void CheckIndirecInterfacePropertyFields() {
            var checker = new InterfacePeopertysChecker(compilation_);
            foreach(var property in checker.Property) {
                isFieldPropertys_.Add(property, false);
            }
        }

        public bool IsPropertyField(IPropertySymbol symbol) {
            bool isAutoField;
            if(!isFieldPropertys_.TryGetValue(symbol, out isAutoField)) {
                bool? isMateField = XmlMetaProvider.IsPropertyField(symbol);
                if(isMateField.HasValue) {
                    isAutoField = isMateField.Value;
                }
                else {
                    isAutoField = symbol.IsPropertyField();
                }
                isFieldPropertys_.Add(symbol, isAutoField);
            }
            return isAutoField;
        }
    }
}
