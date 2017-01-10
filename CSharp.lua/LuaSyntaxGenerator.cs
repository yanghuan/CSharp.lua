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
using System.Collections.Immutable;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using CSharpLua.LuaAst;
using System.Diagnostics.Contracts;

namespace CSharpLua {
    internal sealed class PartialTypeDeclaration : IComparable<PartialTypeDeclaration> {
        public INamedTypeSymbol Symbol;
        public TypeDeclarationSyntax Node;
        public LuaTypeDeclarationSyntax LuaNode;
        public LuaCompilationUnitSyntax CompilationUnit;

        public int CompareTo(PartialTypeDeclaration other) {
            return CompilationUnit.FilePath.Length.CompareTo(other.CompilationUnit.FilePath.Length);
        }
    }

    public sealed class LuaSyntaxGenerator {
        public sealed class SettingInfo {
            public bool HasSemicolon { get; }
            private int indent_;
            public string IndentString { get; private set; }
            public bool IsNewest { get; }

            public SettingInfo() {
                Indent = 4;
                HasSemicolon = true;
                IsNewest = true;
            }

            public int Indent {
                get {
                    return indent_;
                }
                set {
                    if(indent_ != value) {
                        indent_ = value;
                        IndentString = new string(' ', indent_);
                    }
                }
            }
        }

        private CSharpCompilation compilation_;
        public XmlMetaProvider XmlMetaProvider { get; }
        public SettingInfo Setting { get; }
        private HashSet<string> exportEnums_ = new HashSet<string>();
        private Dictionary<INamedTypeSymbol, List<PartialTypeDeclaration>> partialTypes_ = new Dictionary<INamedTypeSymbol, List<PartialTypeDeclaration>>();

        public LuaSyntaxGenerator(IEnumerable<string> metas, CSharpCompilation compilation) {
            XmlMetaProvider = new XmlMetaProvider(metas);
            Setting = new SettingInfo();
            compilation_ = compilation;
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
            CheckPartialTypes();
            return luaCompilationUnits.Where(i => !i.IsEmpty);
        }

        public void Generate(Func<LuaCompilationUnitSyntax, TextWriter> writerFunctor) {
            foreach(var luaCompilationUnit in Create()) {
                using(var writer = writerFunctor(luaCompilationUnit)) {
                    LuaRenderer rener = new LuaRenderer(this, writer);
                    luaCompilationUnit.Render(rener);
                }
            }
        }

        internal bool IsEnumExport(string enumName) {
            return exportEnums_.Contains(enumName);
        }

        internal void AddExportEnum(string enumName) {
            exportEnums_.Add(enumName);
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
                LuaNode = luaNode,
                CompilationUnit = compilationUnit,
            });
        }

        private void CheckPartialTypes() {
            foreach(var typeDeclarations in partialTypes_.Values) {
                PartialTypeDeclaration major = typeDeclarations.Min();
                LuaSyntaxNodeTransfor transfor = new LuaSyntaxNodeTransfor(this, null);
                transfor.AcceptPartialType(major, typeDeclarations);
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
    }
}
