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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using CSharpLua.LuaAst;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace CSharpLua {
    public sealed partial class LuaSyntaxNodeTransfor : CSharpSyntaxVisitor<LuaSyntaxNode> {
        private LuaSyntaxGenerator generator_;
        private SemanticModel semanticModel_;

        private Stack<LuaCompilationUnitSyntax> compilationUnits_ = new Stack<LuaCompilationUnitSyntax>();
        private Stack<LuaTypeDeclarationSyntax> typeDeclarations_ = new Stack<LuaTypeDeclarationSyntax>();
        private Stack<LuaFunctionExpressionSyntax> functions_ = new Stack<LuaFunctionExpressionSyntax>();
        private Stack<LuaBlockSyntax> blocks_ = new Stack<LuaBlockSyntax>();
        private Stack<LuaIfStatementSyntax> ifStatements_ = new Stack<LuaIfStatementSyntax>();
        private Stack<LuaSwitchAdapterStatementSyntax> switchs_ = new Stack<LuaSwitchAdapterStatementSyntax>();
  
        private static readonly Dictionary<string, string> operatorTokenMapps_ = new Dictionary<string, string>() {
            ["!="] = LuaSyntaxNode.Tokens.NotEquals,
            ["!"] = LuaSyntaxNode.Tokens.Not,
            ["&&"] = LuaSyntaxNode.Tokens.And,
            ["||"] = LuaSyntaxNode.Tokens.Or,
            ["??"] = LuaSyntaxNode.Tokens.Or,
            ["^"] = "~"
        };

        public LuaSyntaxNodeTransfor(LuaSyntaxGenerator generator, SemanticModel semanticModel) {
            generator_ = generator;
            semanticModel_ = semanticModel;
        }

        private XmlMetaProvider XmlMetaProvider {
            get {
                return generator_.XmlMetaProvider;
            }
        }

        private static string GetOperatorToken(SyntaxToken operatorToken) {
            string token = operatorToken.ValueText;
            return operatorTokenMapps_.GetOrDefault(token, token);
        }

        private bool IsLuaNewest {
            get {
                return generator_.Setting.IsNewest;
            }
        }

        private LuaCompilationUnitSyntax CurCompilationUnit {
            get {
                return compilationUnits_.Peek();
            }
        }

        private LuaTypeDeclarationSyntax CurType {
            get {
                return typeDeclarations_.Peek();
            }
        }

        private LuaFunctionExpressionSyntax CurFunction {
            get {
                return functions_.Peek();
            }
        }

        private void PushFunction(LuaFunctionExpressionSyntax function) {
            functions_.Push(function);
            ++localMappingCounter_;
        }

        private void PopFunction() {
            functions_.Pop();
            --localMappingCounter_;
            if(localMappingCounter_ == 0) {
                localReservedNames_.Clear();
            }
        }

        private LuaBlockSyntax CurBlock {
            get {
                return blocks_.Peek();
            }
        }

        public override LuaSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node) {
            LuaCompilationUnitSyntax compilationUnit = new LuaCompilationUnitSyntax() { FilePath = node.SyntaxTree.FilePath };
            compilationUnits_.Push(compilationUnit);
            foreach(var member in node.Members) {
                LuaStatementSyntax memberNode = (LuaStatementSyntax)member.Accept(this);
                var typeDeclaration = memberNode as LuaTypeDeclarationSyntax;
                if(typeDeclaration != null) {
                    compilationUnit.AddTypeDeclaration(typeDeclaration);
                }
                else {
                    compilationUnit.Statements.Add(memberNode);
                }
            }
            compilationUnits_.Pop();
            return compilationUnit;
        }

        public override LuaSyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = (LuaIdentifierNameSyntax)node.Name.Accept(this);
            LuaNamespaceDeclarationSyntax namespaceDeclaration = new LuaNamespaceDeclarationSyntax(name);
            foreach(var member in node.Members) {
                var memberNode = (LuaWrapFunctionStatementSynatx)member.Accept(this);
                namespaceDeclaration.AddMemberDeclaration(memberNode);
            }
            return namespaceDeclaration;
        }

        private void BuildTypeMembers(LuaTypeDeclarationSyntax typeDeclaration, TypeDeclarationSyntax node) {
            if(!node.IsKind(SyntaxKind.InterfaceDeclaration)) {
                foreach(var member in node.Members) {
                    var newMember = member.Accept(this);
                    SyntaxKind kind = member.Kind();
                    if(kind >= SyntaxKind.ClassDeclaration && kind <= SyntaxKind.EnumDeclaration) {
                        typeDeclaration.AddMemberDeclaration((LuaWrapFunctionStatementSynatx)newMember);
                    }
                }
            }
        }

        private void BuildTypeDeclaration(TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
            typeDeclarations_.Push(typeDeclaration);
            if(node.TypeParameterList != null) {
                foreach(var typeParameter in node.TypeParameterList.Parameters) {
                    var typeIdentifier = (LuaIdentifierNameSyntax)typeParameter.Accept(this);
                    typeDeclaration.AddTypeIdentifier(typeIdentifier);
                }
            }
            if(node.BaseList != null) {
                List<LuaExpressionSyntax> baseTypes = new List<LuaExpressionSyntax>();
                foreach(var baseType in node.BaseList.Types) {
                    var baseTypeName = (LuaExpressionSyntax)baseType.Accept(this);
                    baseTypes.Add(baseTypeName);
                }
                typeDeclaration.AddBaseTypes(baseTypes);
            }
            BuildTypeMembers(typeDeclaration, node);
            typeDeclarations_.Pop();
            CurCompilationUnit.AddTypeDeclarationCount();
        }

        private void VisitTypeDeclaration(TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
            INamedTypeSymbol typeSymbol = semanticModel_.GetDeclaredSymbol(node);
            if(node.Modifiers.IsPartial()) {
                if(!typeSymbol.DeclaringSyntaxReferences.IsEmpty) {
                    generator_.AddPartialTypeDeclaration(typeSymbol, node, typeDeclaration, CurCompilationUnit);
                    typeDeclaration.IsPartialMark = true;
                }
                else {
                    BuildTypeDeclaration(node, typeDeclaration);
                }
            }
            else {
                BuildTypeDeclaration(node, typeDeclaration);
            }
            generator_.AddTypeSymbol(typeSymbol);
        }

        internal void AcceptPartialType(PartialTypeDeclaration major, List<PartialTypeDeclaration> typeDeclarations) {
            major.LuaNode.IsPartialMark = false;
            major.CompilationUnit.AddTypeDeclarationCount();

            compilationUnits_.Push(major.CompilationUnit);
            typeDeclarations_.Push(major.LuaNode);

            if(major.Node.TypeParameterList != null) {
                foreach(var typeParameter in major.Node.TypeParameterList.Parameters) {
                    var typeIdentifier = (LuaIdentifierNameSyntax)typeParameter.Accept(this);
                    major.LuaNode.AddTypeIdentifier(typeIdentifier);
                }
            }

            var baseTypes = typeDeclarations.SelectMany(i => i.Node.BaseList != null ? (IEnumerable<BaseTypeSyntax>)i.Node.BaseList.Types : Array.Empty<BaseTypeSyntax>()).ToList();
            if(baseTypes.Count > 0) {
                if(baseTypes.Count > 1) {
                    var baseTypeIndex = baseTypes.FindIndex(generator_.IsBaseType);
                    if(baseTypeIndex > 0) {
                        var baseType = baseTypes[baseTypeIndex];
                        baseTypes.RemoveAt(baseTypeIndex);
                        baseTypes.Insert(0, baseType);
                    }
                }

                List<LuaExpressionSyntax> baseTypeExpressions = new List<LuaExpressionSyntax>();
                foreach(var baseType in baseTypes) {
                    semanticModel_ = generator_.GetSemanticModel(baseType.SyntaxTree);
                    var baseTypeName = (LuaExpressionSyntax)baseType.Accept(this);
                    baseTypeExpressions.Add(baseTypeName);
                }
                major.LuaNode.AddBaseTypes(baseTypeExpressions);
            }

            foreach(var typeDeclaration in typeDeclarations) {
                semanticModel_ = generator_.GetSemanticModel(typeDeclaration.Node.SyntaxTree);
                BuildTypeMembers(typeDeclaration.LuaNode, typeDeclaration.Node);
            }

            typeDeclarations_.Pop();
            compilationUnits_.Pop();
        }

        public override LuaSyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaClassDeclarationSyntax classDeclaration = new LuaClassDeclarationSyntax(name);
            VisitTypeDeclaration(node, classDeclaration);
            return classDeclaration;
        }

        public override LuaSyntaxNode VisitStructDeclaration(StructDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaStructDeclarationSyntax structDeclaration = new LuaStructDeclarationSyntax(name);
            VisitTypeDeclaration(node, structDeclaration);
            return structDeclaration;
        }

        public override LuaSyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaInterfaceDeclarationSyntax interfaceDeclaration = new LuaInterfaceDeclarationSyntax(name);
            VisitTypeDeclaration(node, interfaceDeclaration);
            return interfaceDeclaration;
        }

        public override LuaSyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node) {
            INamedTypeSymbol symbol = semanticModel_.GetDeclaredSymbol(node);
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaEnumDeclarationSyntax enumDeclaration = new LuaEnumDeclarationSyntax(symbol.ToString(), name, CurCompilationUnit);
            typeDeclarations_.Push(enumDeclaration);
            foreach(var member in node.Members) {
                var statement = (LuaKeyValueTableItemSyntax)member.Accept(this);
                enumDeclaration.Add(statement);
            }
            typeDeclarations_.Pop();
            generator_.AddTypeSymbol(symbol);
            generator_.AddEnumDeclaration(enumDeclaration);
            return enumDeclaration;
        }

        private void VisitYield(MethodDeclarationSyntax node, LuaFunctionExpressionSyntax function) {
            Contract.Assert(function.HasYield);

            var nameSyntax = (SimpleNameSyntax)node.ReturnType;
            string name = LuaSyntaxNode.Tokens.Yield + nameSyntax.Identifier.ValueText;
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.System, new LuaIdentifierNameSyntax(name));
            LuaInvocationExpressionSyntax invokeExpression = new LuaInvocationExpressionSyntax(memberAccess);
            LuaFunctionExpressionSyntax wrapFunction = new LuaFunctionExpressionSyntax();

            var parameters = function.ParameterList.Parameters;
            wrapFunction.ParameterList.Parameters.AddRange(parameters);
            wrapFunction.AddStatements(function.Body.Statements);
            invokeExpression.AddArgument(wrapFunction);
            if(node.ReturnType.IsKind(SyntaxKind.GenericName)) {
                var genericNameSyntax = (GenericNameSyntax)nameSyntax;
                var typeName = genericNameSyntax.TypeArgumentList.Arguments.First();
                var expression = (LuaExpressionSyntax)typeName.Accept(this);
                invokeExpression.AddArgument(expression);
            }
            else {
                invokeExpression.AddArgument(LuaIdentifierNameSyntax.Object);
            }
            invokeExpression.ArgumentList.Arguments.AddRange(parameters.Select(i => new LuaArgumentSyntax(i.Identifier)));

            LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(invokeExpression);
            function.Body.Statements.Clear();
            function.AddStatement(returnStatement);
        }

        public override LuaSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
            if(!node.Modifiers.IsAbstract()) {
                IMethodSymbol symbol = semanticModel_.GetDeclaredSymbol(node);
                LuaIdentifierNameSyntax methodName = generator_.GetMethodName(symbol);
                LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
                PushFunction(function);
                if(!node.Modifiers.IsStatic()) {
                    function.AddParameter(LuaIdentifierNameSyntax.This);
                }

                foreach(var parameter in node.ParameterList.Parameters) {
                    var luaParameter = (LuaParameterSyntax)parameter.Accept(this);
                    CheckParameterName(ref luaParameter, parameter);
                    function.ParameterList.Parameters.Add(luaParameter);
                    if(parameter.Default != null) {
                        if(!parameter.Default.Value.IsKind(SyntaxKind.NullLiteralExpression)) {
                            var attributes = parameter.AttributeLists.SelectMany(i => i.Attributes);
                            bool hasCaller = attributes.Any(IsCallerAttribute);
                            if(!hasCaller) {
                                var expression = (LuaExpressionSyntax)parameter.Default.Value.Accept(this);
                                var intiStatement = new LuaMethodParameterDefaultValueStatementSyntax(luaParameter.Identifier, expression);
                                function.AddStatement(intiStatement);
                            }
                        }
                    }
                    else {
                        if(parameter.Modifiers.IsParams()) {
                            var typeName = (LuaIdentifierNameSyntax)((ArrayTypeSyntax)parameter.Type).ElementType.Accept(this);
                            var emptyArray = BuildEmptyArray(typeName);
                            var intiStatement = new LuaMethodParameterDefaultValueStatementSyntax(luaParameter.Identifier, emptyArray);
                            function.AddStatement(intiStatement);
                        }
                    }
                }

                if(node.TypeParameterList != null) {
                    foreach(var typeParameter in node.TypeParameterList.Parameters) {
                        var typeName = (LuaIdentifierNameSyntax)typeParameter.Accept(this);
                        function.AddParameter(typeName);
                    }
                }

                LuaBlockSyntax block = (LuaBlockSyntax)node.Body.Accept(this);
                function.AddStatements(block.Statements);
                if(function.HasYield) {
                    VisitYield(node, function);
                }
                PopFunction();
                CurType.AddMethod(methodName, function, node.Modifiers.IsPrivate());
                return function;
            }

            return base.VisitMethodDeclaration(node);
        }

        private static LuaExpressionSyntax GetPredefinedTypeDefaultValue(ITypeSymbol typeSymbol) {
            switch(typeSymbol.SpecialType) {
                case SpecialType.System_Boolean: {
                        return new LuaIdentifierNameSyntax(default(bool).ToString());
                    }
                case SpecialType.System_Char: {
                        return new LuaCharacterLiteralExpression(default(char));
                    }
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64: {
                        return new LuaIdentifierNameSyntax(0);
                    }
                case SpecialType.System_Single:
                case SpecialType.System_Double: {
                        return new LuaIdentifierNameSyntax(0.0.ToString());
                    }
                default:
                    return null;
            }
        }

        private LuaIdentifierNameSyntax GetTempIdentifier(SyntaxNode node) {
            int index = CurFunction.TempIndex++;
            string name = LuaSyntaxNode.TempIdentifiers.GetOrDefault(index);
            if(name == null) {
                throw new CompilationErrorException($"{node.GetLocationString()} : Your code is startling, {LuaSyntaxNode.TempIdentifiers.Length} "
                    + "temporary variables is not enough, please refactor your code.");
            }
            return new LuaIdentifierNameSyntax(name);
        }

        private LuaInvocationExpressionSyntax BuildDefaultValueExpression(TypeSyntax type) {
            var identifier = (LuaExpressionSyntax)type.Accept(this);
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemDefault, identifier);
        }

        private void VisitBaseFieldDeclarationSyntax(BaseFieldDeclarationSyntax node) {
            if(!node.Modifiers.IsConst()) {
                bool isStatic = node.Modifiers.IsStatic();
                bool isPrivate = node.Modifiers.IsPrivate();
                bool isReadOnly = node.Modifiers.IsReadOnly();
                var type = node.Declaration.Type;
                ITypeSymbol typeSymbol = (ITypeSymbol)semanticModel_.GetSymbolInfo(type).Symbol;
                bool isImmutable = typeSymbol.IsImmutable();
                foreach(var variable in node.Declaration.Variables) {
                    if(node.IsKind(SyntaxKind.EventFieldDeclaration)) {
                        var eventSymbol = (IEventSymbol)semanticModel_.GetDeclaredSymbol(variable);
                        if(eventSymbol.IsOverridable() || eventSymbol.IsInterfaceImplementation()) {
                            bool valueIsLiteral;
                            LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, variable.Initializer?.Value, out valueIsLiteral);
                            CurType.AddEvent(variable.Identifier.ValueText, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate);
                            continue;
                        }
                    }
                    AddField(type, typeSymbol, variable.Identifier, variable.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly);
                }
            }
            else {
                bool isPrivate = node.Modifiers.IsPrivate();
                var type = node.Declaration.Type;
                ITypeSymbol typeSymbol = (ITypeSymbol)semanticModel_.GetSymbolInfo(type).Symbol;
                if(typeSymbol.SpecialType == SpecialType.System_String) {
                    foreach(var variable in node.Declaration.Variables) {
                        var value = (LiteralExpressionSyntax)variable.Initializer.Value;
                        if(value.Token.ValueText.Length > LuaSyntaxNode.StringConstInlineCount) {
                            AddField(type, typeSymbol, variable.Identifier, value, true, true, isPrivate, true);
                        }
                    }
                }
            }
        }

        public override LuaSyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) {
            VisitBaseFieldDeclarationSyntax(node);
            return base.VisitFieldDeclaration(node);
        }

        private LuaExpressionSyntax GetFieldValueExpression(TypeSyntax type, ITypeSymbol typeSymbol, ExpressionSyntax expression, out bool valueIsLiteral) {
            LuaExpressionSyntax valueExpression = null;
            valueIsLiteral = false;
            if(expression != null) {
                valueExpression = (LuaExpressionSyntax)expression.Accept(this);
                valueIsLiteral = expression is LiteralExpressionSyntax;
            }
            if(valueExpression == null) {
                if(typeSymbol.IsValueType) {
                    if(typeSymbol.IsDefinition) {
                        LuaExpressionSyntax defalutValue = GetPredefinedTypeDefaultValue(typeSymbol);
                        if(defalutValue != null) {
                            valueExpression = defalutValue;
                        }
                        else {
                            valueExpression = BuildDefaultValueExpression(type);
                        }
                        valueIsLiteral = true;
                    }
                    else {
                        valueExpression = BuildDefaultValueExpression(type);
                    }
                }
            }
            return valueExpression;
        }

        private void AddField(TypeSyntax type, ITypeSymbol typeSymbol, SyntaxToken identifier, ExpressionSyntax expression, bool isImmutable, bool isStatic, bool isPrivate, bool isReadOnly) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(identifier.ValueText);
            bool valueIsLiteral;
            LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, expression, out valueIsLiteral);
            CurType.AddField(name, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, isReadOnly);
        }

        public override LuaSyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
            if(!node.Modifiers.IsAbstract()) {
                bool isStatic = node.Modifiers.IsStatic();
                bool isPrivate = node.Modifiers.IsPrivate();
                bool hasGet = false;
                bool hasSet = false;
                if(node.AccessorList != null) {
                    foreach(var accessor in node.AccessorList.Accessors) {
                        if(accessor.Body != null) {
                            LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
                            if(!isStatic) {
                                functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
                            }
                            PushFunction(functionExpression);
                            var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                            PopFunction();
                            functionExpression.AddStatements(block.Statements);
                            LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, node.Identifier.ValueText);
                            CurType.AddMethod(name, functionExpression, isPrivate);
                            if(accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) {
                                Contract.Assert(!hasGet);
                                hasGet = true;
                            }
                            else {
                                Contract.Assert(!hasSet);
                                functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
                                name.IsGetOrAdd = false;
                                hasSet = true;
                            }
                        }
                    }
                }
                else {
                    Contract.Assert(!hasGet);
                    LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, node.Identifier.ValueText);
                    var expression = (LuaExpressionSyntax)node.ExpressionBody.Expression.Accept(this);
                    LuaFunctionExpressionSyntax functionExpress = new LuaFunctionExpressionSyntax();
                    if(!isStatic) {
                        functionExpress.AddParameter(LuaIdentifierNameSyntax.This);
                    }
                    LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(expression);
                    functionExpress.AddStatement(returnStatement);
                    CurType.AddMethod(name, functionExpress, isPrivate);
                    hasGet = true;
                }

                if(!hasGet && !hasSet) {
                    var type = node.Type;
                    ITypeSymbol typeSymbol = (ITypeSymbol)semanticModel_.GetSymbolInfo(type).Symbol;
                    bool isImmutable = typeSymbol.IsImmutable();
                    if(isStatic) {
                        bool isReadOnly = node.AccessorList.Accessors.Count == 1 && node.AccessorList.Accessors[0].Body == null;
                        AddField(type, typeSymbol, node.Identifier, node.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly);
                    }
                    else {
                        bool isAuto = semanticModel_.GetDeclaredSymbol(node).IsPropertyField();
                        if(isAuto) {
                            bool isReadOnly = node.AccessorList.Accessors.Count == 1 && node.AccessorList.Accessors[0].Body == null;
                            AddField(type, typeSymbol, node.Identifier, node.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly);
                        }
                        else {
                            bool valueIsLiteral;
                            LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, node.Initializer?.Value, out valueIsLiteral);
                            CurType.AddProperty(node.Identifier.ValueText, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate);
                        }
                    }
                }
            }
            return base.VisitPropertyDeclaration(node);
        }

        public override LuaSyntaxNode VisitEventDeclaration(EventDeclarationSyntax node) {
            if(!node.Modifiers.IsAbstract()) {
                bool isStatic = node.Modifiers.IsStatic();
                bool isPrivate = node.Modifiers.IsPrivate();
                foreach(var accessor in node.AccessorList.Accessors) {
                    var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                    LuaFunctionExpressionSyntax functionExpress = new LuaFunctionExpressionSyntax();
                    if(!isStatic) {
                        functionExpress.AddParameter(LuaIdentifierNameSyntax.This);
                    }
                    functionExpress.AddParameter(LuaIdentifierNameSyntax.Value);
                    functionExpress.AddStatements(block.Statements);
                    LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(false, node.Identifier.ValueText);
                    CurType.AddMethod(name, functionExpress, isPrivate);
                    if(accessor.IsKind(SyntaxKind.RemoveAccessorDeclaration)) {
                        name.IsGetOrAdd = false;
                    }
                }
            }
            return base.VisitEventDeclaration(node);
        }

        public override LuaSyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) {
            VisitBaseFieldDeclarationSyntax(node);
            return base.VisitEventFieldDeclaration(node);
        }

        public override LuaSyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node) {
            IFieldSymbol symbol = semanticModel_.GetDeclaredSymbol(node);
            Contract.Assert(symbol.HasConstantValue);
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            var value = new LuaIdentifierLiteralExpressionSyntax(symbol.ConstantValue.ToString());
            return new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(identifier), value);
        }

        public override LuaSyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node) {
            bool isStatic = node.Modifiers.IsStatic();
            bool isPrivate = node.Modifiers.IsPrivate();
            bool hasGet = false;
            bool hasSet = false;
            foreach(var accessor in node.AccessorList.Accessors) {
                LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
                if(!isStatic) {
                    functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
                }
                PushFunction(functionExpression);
                var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                PopFunction();
                functionExpression.AddStatements(block.Statements);
                LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, string.Empty);
                CurType.AddMethod(name, functionExpression, isPrivate);
                if(accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) {
                    Contract.Assert(!hasGet);
                    hasGet = true;
                }
                else {
                    Contract.Assert(!hasSet);
                    functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
                    name.IsGetOrAdd = false;
                    hasSet = true;
                }
            }

            return base.VisitIndexerDeclaration(node);
        }

        public override LuaSyntaxNode VisitParameterList(ParameterListSyntax node) {
            LuaParameterListSyntax parameterList = new LuaParameterListSyntax();
            foreach(var parameter in node.Parameters) {
                var newNode = (LuaParameterSyntax)parameter.Accept(this);
                parameterList.Parameters.Add(newNode);
            }
            return parameterList;
        }

        public override LuaSyntaxNode VisitParameter(ParameterSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            return new LuaParameterSyntax(identifier);
        }

        private sealed class BlockCommonNode : IComparable<BlockCommonNode> {
            const int kCommentCharCount = 2;
            public SyntaxTrivia Comment { get; }
            public StatementSyntax Statement { get; }
            public FileLinePositionSpan LineSpan { get; }

            public BlockCommonNode(SyntaxTrivia comment) {
                Comment = comment;
                LineSpan = comment.SyntaxTree.GetLineSpan(comment.Span);
            }

            public BlockCommonNode(StatementSyntax statement) {
                Statement = statement;
                LineSpan = statement.SyntaxTree.GetLineSpan(statement.Span);
            }

            public int CompareTo(BlockCommonNode other) {
                return LineSpan.StartLinePosition.CompareTo(other.LineSpan.StartLinePosition);
            }

            public bool Contains(BlockCommonNode other) {
                var otherLineSpan = other.LineSpan;
                return otherLineSpan.StartLinePosition > LineSpan.StartLinePosition
                    && otherLineSpan.EndLinePosition < LineSpan.EndLinePosition;
            }

            public void Visit(LuaSyntaxNodeTransfor transfor, LuaBlockSyntax block, ref int lastLine) {
                if(lastLine != -1) {
                    int count = LineSpan.StartLinePosition.Line - lastLine - 1;
                    if(count > 0) {
                        block.Statements.Add(new LuaBlankLinesStatement(count));
                    }
                }

                if(Statement != null) {
                    LuaStatementSyntax statementNode = (LuaStatementSyntax)Statement.Accept(transfor);
                    block.Statements.Add(statementNode);
                }
                else {
                    string content = Comment.ToString();
                    if(Comment.IsKind(SyntaxKind.SingleLineCommentTrivia)) {
                        string commentContent = content.Substring(kCommentCharCount);
                        LuaShortCommentStatement singleComment = new LuaShortCommentStatement(commentContent);
                        block.Statements.Add(singleComment);
                    }
                    else {
                        string commentContent = content.Substring(kCommentCharCount, content.Length - kCommentCharCount - kCommentCharCount);
                        LuaLongCommentStatement longComment = new LuaLongCommentStatement(commentContent);
                        block.Statements.Add(longComment);
                    }
                }

                lastLine = LineSpan.EndLinePosition.Line;
            }
        }

        public override LuaSyntaxNode VisitBlock(BlockSyntax node) {
            LuaBlockStatementSyntax block = new LuaBlockStatementSyntax();
            blocks_.Push(block);

            var comments = node.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.SingleLineCommentTrivia) || i.IsKind(SyntaxKind.MultiLineCommentTrivia));
            var commentNodes = comments.Select(i => new BlockCommonNode(i));

            List<BlockCommonNode> nodes = node.Statements.Select(i => new BlockCommonNode(i)).ToList();
            bool hasComments = false;
            foreach(var comment in commentNodes) {
                bool isContains = nodes.Any(i => i.Contains(comment));
                if(!isContains) {
                    nodes.Add(comment);
                    hasComments = true;
                }
            }
            if(hasComments) {
                nodes.Sort();
            }

            int lastLine = -1;
            foreach(var common in nodes) {
                common.Visit(this, block, ref lastLine);
            }

            blocks_.Pop();
            return block;
        }

        public override LuaSyntaxNode VisitReturnStatement(ReturnStatementSyntax node) {
            if(CurFunction is LuaCheckReturnFunctionExpressionSyntax) {
                LuaMultipleReturnStatementSyntax returnStatement = new LuaMultipleReturnStatementSyntax();
                returnStatement.Expressions.Add(LuaIdentifierNameSyntax.True);
                if(node.Expression != null) {
                    var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                    returnStatement.Expressions.Add(expression);
                }
                return returnStatement;
            }
            else {
                if(node.Expression != null) {
                    var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                    return new LuaReturnStatementSyntax(expression);
                }
                return new LuaReturnStatementSyntax();
            }
        }

        public override LuaSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
            LuaExpressionSyntax expressionNode = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaExpressionStatementSyntax(expressionNode);
        }

        private LuaExpressionSyntax BuildCommonAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, string operatorToken) {
            var propertyAdapter = left as LuaPropertyAdapterExpressionSyntax;
            if(propertyAdapter != null) {
                propertyAdapter.InvocationExpression.AddArgument(new LuaBinaryExpressionSyntax(propertyAdapter.GetCloneOfGet(), operatorToken, right));
                return propertyAdapter;
            }
            else {
                return new LuaAssignmentExpressionSyntax(left, new LuaBinaryExpressionSyntax(left, operatorToken, right));
            }
        }

        private LuaExpressionSyntax BuildDelegateAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, bool isPlus) {
            if(right is LuaInternalMethodExpressionSyntax) {
                right = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind, LuaIdentifierNameSyntax.This, right);
            }
            else if(right is LuaMemberAccessExpressionSyntax) {
                LuaMemberAccessExpressionSyntax memberAccess = (LuaMemberAccessExpressionSyntax)right;
                if(memberAccess.IsObjectColon) {
                    right = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind, memberAccess.Expression, memberAccess.Name);
                }
            }

            var methodName = isPlus ? LuaIdentifierNameSyntax.DelegateCombine : LuaIdentifierNameSyntax.DelegateRemove;
            var propertyAdapter = left as LuaPropertyAdapterExpressionSyntax;
            if(propertyAdapter != null) {
                if(propertyAdapter.IsProperty) {
                    propertyAdapter.InvocationExpression.AddArgument(new LuaInvocationExpressionSyntax(methodName, propertyAdapter.GetCloneOfGet(), right));
                    return propertyAdapter;
                }
                else {
                    propertyAdapter.IsGetOrAdd = isPlus;
                    propertyAdapter.InvocationExpression.AddArgument(right);
                    return propertyAdapter;
                }
            }
            else {
                return new LuaAssignmentExpressionSyntax(left, new LuaInvocationExpressionSyntax(methodName, left, right));
            }
        }

        private LuaExpressionSyntax BuildLuaAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, SyntaxKind kind) {
            switch(kind) {
                case SyntaxKind.SimpleAssignmentExpression: {
                        var left = (LuaExpressionSyntax)leftNode.Accept(this);
                        var right = (LuaExpressionSyntax)rightNode.Accept(this);

                        var propertyAdapter = left as LuaPropertyAdapterExpressionSyntax;
                        if(propertyAdapter != null) {
                            propertyAdapter.IsGetOrAdd = false;
                            propertyAdapter.InvocationExpression.AddArgument(right);
                            return propertyAdapter;
                        }
                        else {
                            return new LuaAssignmentExpressionSyntax(left, right);
                        }
                    }
                case SyntaxKind.AddAssignmentExpression: {
                        var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
                        if(leftType.IsStringType()) {
                            var left = (LuaExpressionSyntax)leftNode.Accept(this);
                            var right = WrapStringConcatExpression(rightNode);
                            return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Concatenation);
                        }
                        else {
                            var left = (LuaExpressionSyntax)leftNode.Accept(this);
                            var right = (LuaExpressionSyntax)rightNode.Accept(this);

                            if(leftType.IsDelegateType()) {
                                return BuildDelegateAssignmentExpression(left, right, true);
                            }
                            else {
                                return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Plus);
                            }
                        }
                    }
                case SyntaxKind.SubtractAssignmentExpression: {
                        var left = (LuaExpressionSyntax)leftNode.Accept(this);
                        var right = (LuaExpressionSyntax)rightNode.Accept(this);

                        var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
                        if(leftType.IsDelegateType()) {
                            return BuildDelegateAssignmentExpression(left, right, false);
                        }
                        else {
                            return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Sub);
                        }
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public override LuaSyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) {
            List<LuaExpressionSyntax> assignments = new List<LuaExpressionSyntax>();

            while(true) {
                var leftExpression = node.Left;
                var rightExpression = node.Right;
                var kind = node.Kind();

                var assignmentRight = rightExpression as AssignmentExpressionSyntax;
                if(assignmentRight == null) {
                    assignments.Add(BuildLuaAssignmentExpression(leftExpression, rightExpression, kind));
                    break;
                }
                else {
                    assignments.Add(BuildLuaAssignmentExpression(leftExpression, assignmentRight.Left, kind));
                    node = assignmentRight;
                }
            }

            if(assignments.Count == 1) {
                return assignments.First();
            }
            else {
                assignments.Reverse();
                LuaLineMultipleExpressionSyntax multipleAssignment = new LuaLineMultipleExpressionSyntax();
                multipleAssignment.Assignments.AddRange(assignments);
                return multipleAssignment;
            }
        }

        private LuaExpressionSyntax BuildInvokeRefOrOut(InvocationExpressionSyntax node, LuaExpressionSyntax invocation, IEnumerable<LuaExpressionSyntax> refOrOutArguments) {
            if(node.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
                LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
                IMethodSymbol symbol = (IMethodSymbol)symbolInfo.Symbol;
                if(!symbol.ReturnsVoid) {
                    var temp = GetTempIdentifier(node);
                    CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(temp)));
                    multipleAssignment.Lefts.Add(temp);
                }
                multipleAssignment.Lefts.AddRange(refOrOutArguments);
                multipleAssignment.Rights.Add(invocation);
                return multipleAssignment;
            }
            else {
                var temp = GetTempIdentifier(node);
                LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                multipleAssignment.Lefts.Add(temp);
                multipleAssignment.Lefts.AddRange(refOrOutArguments);
                multipleAssignment.Rights.Add(invocation);

                CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(temp)));
                CurBlock.Statements.Add(new LuaExpressionStatementSyntax(multipleAssignment));
                return temp;
            }
        }

        private LuaExpressionSyntax CheckCodeTemplateInvocationExpression(IMethodSymbol symbol, InvocationExpressionSyntax node) {
            var constValue = semanticModel_.GetConstantValue(node);
            if(constValue.HasValue) {
                return GetConstLiteralExpression(constValue.Value);
            }

            if(node.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                string codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(symbol);
                if(codeTemplate != null) {
                    List<ExpressionSyntax> argumentExpressions = new List<ExpressionSyntax>();
                    var memberAccessExpression = (MemberAccessExpressionSyntax)node.Expression;
                    if(symbol.IsExtensionMethod) {
                        argumentExpressions.Add(memberAccessExpression.Expression);
                        if(symbol.ContainingType.IsSystemLinqEnumerable()) {
                            CurCompilationUnit.ImportLinq();
                        }
                    }
                    argumentExpressions.AddRange(node.ArgumentList.Arguments.Select(i => i.Expression));
                    var invocationExpression = BuildCodeTemplateExpression(codeTemplate, memberAccessExpression.Expression, argumentExpressions, symbol.TypeArguments);
                    var refOrOuts = node.ArgumentList.Arguments.Where(i => i.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) || i.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword));
                    if(refOrOuts.Any()) {
                        return BuildInvokeRefOrOut(node, invocationExpression, refOrOuts.Select(i => ((LuaArgumentSyntax)i.Accept(this)).Expression));
                    }
                    else {
                        return invocationExpression;
                    }
                }
            }
            return null;
        }

        public override LuaSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node) {
            var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
            var codeTemplateExpression = CheckCodeTemplateInvocationExpression(symbol, node);
            if(codeTemplateExpression != null) {
                return codeTemplateExpression;
            }

            List<LuaArgumentSyntax> arguments = new List<LuaArgumentSyntax>();
            List<LuaExpressionSyntax> refOrOutArguments = new List<LuaExpressionSyntax>();

            foreach(var argument in node.ArgumentList.Arguments) {
                var luaArgument = (LuaArgumentSyntax)argument.Accept(this);
                arguments.Add(luaArgument);
                if(argument.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) || argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword)) {
                    refOrOutArguments.Add(luaArgument.Expression);
                }
            }

            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaInvocationExpressionSyntax invocation;
            if(!symbol.IsExtensionMethod) {
                LuaMemberAccessExpressionSyntax memberAccess = expression as LuaMemberAccessExpressionSyntax;
                if(memberAccess != null) {
                    if(memberAccess.Name is LuaInternalMethodExpressionSyntax) {
                        invocation = new LuaInvocationExpressionSyntax(memberAccess.Name);
                        invocation.AddArgument(memberAccess.Expression);
                    }
                    else {
                        invocation = new LuaInvocationExpressionSyntax(memberAccess);
                    }
                }
                else {
                    invocation = new LuaInvocationExpressionSyntax(expression);
                    invocation.AddArgument(LuaIdentifierNameSyntax.This);
                }
            }
            else {
                LuaMemberAccessExpressionSyntax memberAccess = expression as LuaMemberAccessExpressionSyntax;
                if(memberAccess != null) {
                    if(memberAccess.Name is LuaInternalMethodExpressionSyntax) {
                        invocation = new LuaInvocationExpressionSyntax(memberAccess.Name);
                        invocation.AddArgument(memberAccess.Expression);
                    }
                    else {
                        invocation = BuildExtensionMethodInvocation(symbol.ReducedFrom, memberAccess.Expression);
                    }
                }
                else {
                    invocation = new LuaInvocationExpressionSyntax(expression);
                }
            }

            invocation.ArgumentList.Arguments.AddRange(arguments);
            CheckInvocationCallerAttribute(symbol, node, invocation);
            AddInvocationTypeArguments(symbol, node, invocation);

            if(refOrOutArguments.Count > 0) {
                return BuildInvokeRefOrOut(node, invocation, refOrOutArguments);
            }
            return invocation;
        }

        private LuaInvocationExpressionSyntax BuildExtensionMethodInvocation(IMethodSymbol reducedFrom, LuaExpressionSyntax expression) {
            LuaExpressionSyntax typeName = XmlMetaProvider.GetTypeName(reducedFrom.ContainingType);
            LuaIdentifierNameSyntax methodName = generator_.GetMethodName(reducedFrom);
            LuaMemberAccessExpressionSyntax typeMemberAccess = new LuaMemberAccessExpressionSyntax(typeName, methodName);
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(typeMemberAccess);
            invocation.AddArgument(expression);
            return invocation;
        }

        private void AddInvocationTypeArguments(IMethodSymbol symbol, InvocationExpressionSyntax node, LuaInvocationExpressionSyntax invocation) {
            if(symbol.TypeArguments.Length > 0) {
                int optionalCount = symbol.Parameters.Length - node.ArgumentList.Arguments.Count;
                while(optionalCount > 0) {
                    invocation.AddArgument(LuaIdentifierNameSyntax.Nil);
                    --optionalCount;
                }
                foreach(var typeArgument in symbol.TypeArguments) {
                    var typeName = XmlMetaProvider.GetTypeName(typeArgument);
                    invocation.AddArgument(typeName);
                }
            }
        }

        private void CheckInvocationCallerAttribute(IMethodSymbol symbol, InvocationExpressionSyntax node, LuaInvocationExpressionSyntax invocation) {
            int argumentCount = node.ArgumentList.Arguments.Count;
            if(symbol.Parameters.Length > argumentCount) {
                var optionalParameters = symbol.Parameters.Skip(argumentCount);
                int prevCallerIndex = -1;
                int index = 0;
                foreach(var parameter in optionalParameters) {
                    var callerExpression = CheckCallerAttribute(parameter, node);
                    if(callerExpression != null) {
                        int placeholderCount = index - prevCallerIndex - 1;
                        if(placeholderCount > 0) {
                            for(int i = 0; i < placeholderCount; ++i) {
                                invocation.AddArgument(LuaIdentifierNameSyntax.Nil);
                            }
                        }
                        invocation.AddArgument(callerExpression);
                        prevCallerIndex = index;
                    }
                    ++index;
                }
            }
        }

        private LuaExpressionSyntax BuildMemberAccessTargetExpression(ExpressionSyntax targetExpression) {
            var expression = (LuaExpressionSyntax)targetExpression.Accept(this);
            SyntaxKind kind = targetExpression.Kind();
            if((kind >= SyntaxKind.NumericLiteralExpression && kind <= SyntaxKind.NullLiteralExpression) 
                || (expression is LuaLiteralExpressionSyntax)) {
                expression = new LuaParenthesizedExpressionSyntax(expression);
            }
            return expression;
        }

        private LuaExpressionSyntax CheckMemberAccessCodeTemplate(ISymbol symbol, MemberAccessExpressionSyntax node) {
            if(symbol.Kind == SymbolKind.Field) {
                IFieldSymbol fieldSymbol = (IFieldSymbol)symbol;
                string codeTemplate = XmlMetaProvider.GetFieldCodeTemplate(fieldSymbol);
                if(codeTemplate != null) {
                    return BuildCodeTemplateExpression(codeTemplate, node.Expression);
                }

                if(fieldSymbol.HasConstantValue) {
                    return GetConstLiteralExpression(fieldSymbol);
                }
            }
            else if(symbol.Kind == SymbolKind.Property) {
                IPropertySymbol propertySymbol = (IPropertySymbol)symbol;
                bool isGet = !node.Parent.Kind().IsAssignment();
                string codeTemplate = XmlMetaProvider.GetProertyCodeTemplate(propertySymbol, isGet);
                if(codeTemplate != null) {
                    return BuildCodeTemplateExpression(codeTemplate, node.Expression);
                }
            }
            return null;
        }

        public override LuaSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node) {
            ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
            var codeTemplateExpression = CheckMemberAccessCodeTemplate(symbol, node);
            if(codeTemplateExpression != null) {
                return codeTemplateExpression;
            }

            if(symbol.Kind == SymbolKind.Property || symbol.Kind == SymbolKind.Event) {
                if(node.Expression.IsKind(SyntaxKind.ThisExpression)) {
                    var propertyIdentifier = (LuaExpressionSyntax)node.Name.Accept(this);
                    var propertyAdapter = propertyIdentifier as LuaPropertyAdapterExpressionSyntax;
                    if(propertyAdapter != null) {
                        return propertyAdapter;
                    }
                    else {
                        return new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, propertyIdentifier);
                    }
                }

                if(node.Expression.IsKind(SyntaxKind.BaseExpression)) {
                    var expression = (LuaIdentifierNameSyntax)node.Expression.Accept(this);
                    var nameIdentifier = (LuaExpressionSyntax)node.Name.Accept(this);
                    var propertyMethod = nameIdentifier as LuaPropertyAdapterExpressionSyntax;
                    if(propertyMethod != null) {
                        if(expression != LuaIdentifierNameSyntax.This) {
                            propertyMethod.InvocationExpression.AddArgument(LuaIdentifierNameSyntax.This);
                        }

                        var memberAccessExpression = new LuaMemberAccessExpressionSyntax(expression, propertyMethod.InvocationExpression.Expression, true);
                        propertyMethod.Update(memberAccessExpression);
                        return propertyMethod;
                    }
                    else {
                        return new LuaMemberAccessExpressionSyntax(expression, nameIdentifier);
                    }
                }
                else {
                    if(symbol.IsStatic) {
                        if(node.Expression.IsKind(SyntaxKind.IdentifierName)) {
                            var identifierName = (IdentifierNameSyntax)node.Expression;
                            if(GetTypeDeclarationSymbol(node) == symbol.ContainingSymbol) {
                                return node.Name.Accept(this);
                            }
                        }
                    }

                    var expression = BuildMemberAccessTargetExpression(node.Expression);
                    var nameExpression = (LuaExpressionSyntax)node.Name.Accept(this);
                    var propertyMethod = nameExpression as LuaPropertyAdapterExpressionSyntax;
                    if(propertyMethod != null) {
                        var arguments = propertyMethod.InvocationExpression.ArgumentList.Arguments;
                        if(arguments.Count == 1) {
                            if(arguments[0].Expression == LuaIdentifierNameSyntax.This) {
                                propertyMethod.InvocationExpression.ArgumentList.Arguments[0] = new LuaArgumentSyntax(expression);
                            }
                        }
                        else {
                            var memberAccessExpression = new LuaMemberAccessExpressionSyntax(expression, propertyMethod.InvocationExpression.Expression, !symbol.IsStatic);
                            propertyMethod.Update(memberAccessExpression);
                        }
                        return propertyMethod;
                    }
                    else {
                        return new LuaMemberAccessExpressionSyntax(expression, nameExpression);
                    }
                }
            }
            else {
                if(node.Expression.IsKind(SyntaxKind.ThisExpression)) {
                    return node.Name.Accept(this);
                }

                if(node.Expression.IsKind(SyntaxKind.BaseExpression)) {
                    var baseExpression = (LuaExpressionSyntax)node.Expression.Accept(this);
                    var identifier = (LuaIdentifierNameSyntax)node.Name.Accept(this);
                    if(baseExpression == LuaIdentifierNameSyntax.This) {
                        return new LuaMemberAccessExpressionSyntax(baseExpression, identifier, symbol.Kind == SymbolKind.Method);
                    }
                    else {
                        LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(baseExpression, identifier);
                        return new LuaInternalMethodExpressionSyntax(memberAccess);
                    }
                }
                else {
                    if(symbol.IsStatic) {
                        if(node.Expression.IsKind(SyntaxKind.IdentifierName)) {
                            var identifierName = (IdentifierNameSyntax)node.Expression;
                            if(GetTypeDeclarationSymbol(node) == symbol.ContainingSymbol) {
                                return node.Name.Accept(this);
                            }
                        }
                    }

                    var expression = BuildMemberAccessTargetExpression(node.Expression);
                    var identifier = (LuaExpressionSyntax)node.Name.Accept(this);
                    return new LuaMemberAccessExpressionSyntax(expression, identifier, !symbol.IsStatic && symbol.Kind == SymbolKind.Method);
                }
            }
        }

        private LuaExpressionSyntax BuildStaticFieldName(ISymbol symbol, bool isReadOnly, IdentifierNameSyntax node) {
            Contract.Assert(symbol.IsStatic);
            string name;
            if(symbol.DeclaredAccessibility == Accessibility.Private) {
                name = symbol.Name;
            }
            else {
                if(isReadOnly) {
                    name = symbol.Name;
                    if(node.Parent.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
                        AssignmentExpressionSyntax assignmentExpression = (AssignmentExpressionSyntax)node.Parent;
                        if(assignmentExpression.Left == node) {
                            CurType.AddStaticReadOnlyAssignmentName(name);
                        }
                    }
                    var usingStaticType = CheckUsingStaticNameSyntax(symbol, node);
                    if(usingStaticType != null) {
                        return new LuaMemberAccessExpressionSyntax(usingStaticType, new LuaIdentifierNameSyntax(name));
                    }
                }
                else {
                    var constructor = CurFunction as LuaConstructorAdapterExpressionSyntax;
                    if(constructor != null && constructor.IsStaticCtor) {
                        name = LuaSyntaxNode.Tokens.This + '.' + symbol.Name;
                    }
                    else {
                        if(IsInternalNode(node)) {
                            name = symbol.ToString();
                        }
                        else {
                            name = symbol.Name;
                            var usingStaticType = CheckUsingStaticNameSyntax(symbol, node);
                            if(usingStaticType != null) {
                                return new LuaMemberAccessExpressionSyntax(usingStaticType, new LuaIdentifierNameSyntax(name));
                            }
                        }
                    }
                }
            }
            return new LuaIdentifierNameSyntax(name);
        }

        private bool IsInternalNode(NameSyntax node) {
            bool isInternal = false;
            MemberAccessExpressionSyntax parent = null;
            if(node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                parent = (MemberAccessExpressionSyntax)node.Parent;
            }
            if(parent != null) {
                if(parent.Expression.IsKind(SyntaxKind.ThisExpression)) {
                    isInternal = true;
                }
                else if(parent.Expression == node) {
                    isInternal = true;
                }
            }
            else {
                isInternal = true;
            }
            return isInternal;
        }

        private LuaExpressionSyntax VisitFieldOrEventIdentifierName(IdentifierNameSyntax node, ISymbol symbol, bool isProperty) {
            string name;
            bool isField, isReadOnly;
            if(isProperty) {
                var propertySymbol = (IPropertySymbol)symbol;
                isField =  IsPropertyField(propertySymbol);
                isReadOnly = propertySymbol.IsReadOnly;
            }
            else {
                var eventSymbol = (IEventSymbol)symbol;
                isField = eventSymbol.IsEventFiled();
                isReadOnly = false;
            }

            if(symbol.IsStatic) {
                if(isField) {
                    return BuildStaticFieldName(symbol, isReadOnly, node);
                }
                else {
                    var identifierExpression = new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
                    var usingStaticType = CheckUsingStaticNameSyntax(symbol, node);
                    if(usingStaticType != null) {
                        return new LuaMemberAccessExpressionSyntax(usingStaticType, identifierExpression);
                    }
                    return identifierExpression;
                }
            }
            else {
                if(isField) {
                    if(IsInternalNode(node)) {
                        name = LuaSyntaxNode.Tokens.This + '.' + symbol.Name;
                    }
                    else {
                        name = symbol.Name;
                    }
                }
                else {
                    if(IsInternalMember(node, symbol)) {
                        var propertyAdapter = new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
                        propertyAdapter.InvocationExpression.AddArgument(LuaIdentifierNameSyntax.This);
                        return propertyAdapter;
                    }
                    else {
                        if(IsInternalNode(node)) {
                            LuaPropertyOrEventIdentifierNameSyntax identifierName = new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name);
                            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, identifierName, true);
                            return new LuaPropertyAdapterExpressionSyntax(memberAccess, identifierName);
                        }
                        else {
                            return new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
                        }
                    }
                }
            }
            return new LuaIdentifierNameSyntax(name);
        }

        private LuaExpressionSyntax GetMethodNameExpression(IMethodSymbol symbol, NameSyntax node) {
            LuaIdentifierNameSyntax methodName = generator_.GetMethodName(symbol);
            if(symbol.IsStatic) {
                var usingStaticType = CheckUsingStaticNameSyntax(symbol, node);
                if(usingStaticType != null) {
                    return new LuaMemberAccessExpressionSyntax(usingStaticType, methodName);
                }
                if(IsInternalMember(node, symbol)) {
                    return new LuaInternalMethodExpressionSyntax(methodName);
                }
                return methodName;
            }
            else {
                if(IsInternalMember(node, symbol)) {
                    return new LuaInternalMethodExpressionSyntax(methodName);
                }
                else {
                    if(IsInternalNode(node)) {
                        LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, methodName, true);
                        return memberAccess;
                    }
                }
            }
            return methodName;
        }

        public override LuaSyntaxNode VisitIdentifierName(IdentifierNameSyntax node) {
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            Contract.Assert(symbol != null);
            string name;
            switch(symbol.Kind) {
                case SymbolKind.Local:
                case SymbolKind.Parameter: {
                        name = symbol.Name;
                        CheckReservedWord(ref name, symbol);
                        break;
                    }
                case SymbolKind.TypeParameter:
                case SymbolKind.Label: {
                        name = symbol.Name;
                        break;
                    }
                case SymbolKind.NamedType: {
                        if(node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                            var parent = (MemberAccessExpressionSyntax)node.Parent;
                            if(parent.Name == node) {
                                name = symbol.Name;
                                break;
                            }
                        }
                        return XmlMetaProvider.GetTypeName(symbol);
                    }
                case SymbolKind.Namespace: {
                        name = symbol.ToString();
                        break;
                    }
                case SymbolKind.Field: {
                        if(symbol.IsStatic) {
                            var fieldSymbol = (IFieldSymbol)symbol;
                            if(fieldSymbol.HasConstantValue) {
                                if(fieldSymbol.Type.SpecialType == SpecialType.System_String) {
                                    if(((string)fieldSymbol.ConstantValue).Length < LuaSyntaxNode.StringConstInlineCount) {
                                        return GetConstLiteralExpression(fieldSymbol);
                                    }
                                }
                                else {
                                    return GetConstLiteralExpression(fieldSymbol);
                                }
                            }
                            return BuildStaticFieldName(symbol, fieldSymbol.IsReadOnly, node);
                        }
                        else {
                            if(IsInternalNode(node)) {
                                name = LuaSyntaxNode.Tokens.This + '.' + symbol.Name;
                            }
                            else {
                                name = symbol.Name;
                            }
                        }
                        break;
                    }
                case SymbolKind.Method: {
                        return GetMethodNameExpression((IMethodSymbol)symbol, node);
                    }
                case SymbolKind.Property: {
                        return VisitFieldOrEventIdentifierName(node, symbol, true);
                    }
                case SymbolKind.Event: {
                        return VisitFieldOrEventIdentifierName(node, symbol, false);
                    }
                default: {
                        throw new NotSupportedException();
                    }
            }
            return new LuaIdentifierNameSyntax(name);
        }

        public override LuaSyntaxNode VisitQualifiedName(QualifiedNameSyntax node) {
            return new LuaIdentifierNameSyntax(node.ToString());
        }

        private LuaArgumentListSyntax BuildArgumentList(SeparatedSyntaxList<ArgumentSyntax> arguments) {
            LuaArgumentListSyntax argumentList = new LuaArgumentListSyntax();
            foreach(var argument in arguments) {
                var newNode = (LuaArgumentSyntax)argument.Accept(this);
                argumentList.Arguments.Add(newNode);
            }
            return argumentList;
        }

        public override LuaSyntaxNode VisitArgumentList(ArgumentListSyntax node) {
            return BuildArgumentList(node.Arguments);
        }

        public override LuaSyntaxNode VisitArgument(ArgumentSyntax node) {
            LuaExpressionSyntax expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaArgumentSyntax argument = new LuaArgumentSyntax(expression);
            return argument;
        }

        public override LuaSyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node) {
            switch(node.Kind()) {
                case SyntaxKind.CharacterLiteralExpression: {
                        return new LuaCharacterLiteralExpression((char)node.Token.Value);
                    }
                case SyntaxKind.NullLiteralExpression: {
                        return new LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax.Nil);
                    }
                case SyntaxKind.StringLiteralExpression: {
                        return BuildStringLiteralTokenExpression(node.Token);
                    }
                default: {
                        return new LuaIdentifierLiteralExpressionSyntax(node.Token.ValueText);
                    }
            }
        }

        public override LuaSyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) {
            var declaration = (LuaVariableDeclarationSyntax)node.Declaration.Accept(this);
            return new LuaLocalDeclarationStatementSyntax(declaration);
        }

        public override LuaSyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node) {
            LuaVariableListDeclarationSyntax variableListDeclaration = new LuaVariableListDeclarationSyntax();
            foreach(VariableDeclaratorSyntax variable in node.Variables) {
                var variableDeclarator = (LuaVariableDeclaratorSyntax)variable.Accept(this);
                variableListDeclaration.Variables.Add(variableDeclarator);
            }
            bool isMultiNil = variableListDeclaration.Variables.Count > 0 && variableListDeclaration.Variables.All(i => i.Initializer == null);
            if(isMultiNil) {
                LuaLocalVariablesStatementSyntax declarationStatement = new LuaLocalVariablesStatementSyntax();
                foreach(var variable in variableListDeclaration.Variables) {
                    declarationStatement.Variables.Add(variable.Identifier);
                }
                return declarationStatement;
            }
            else {
                return variableListDeclaration;
            }
        }

        public override LuaSyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            CheckVariableDeclaratorName(ref identifier, node);
            LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(identifier);
            if(node.Initializer != null) {
                variableDeclarator.Initializer = (LuaEqualsValueClauseSyntax)node.Initializer.Accept(this);
            }
            return variableDeclarator;
        }

        public override LuaSyntaxNode VisitEqualsValueClause(EqualsValueClauseSyntax node) {
            var expression = (LuaExpressionSyntax)node.Value.Accept(this);
            return new LuaEqualsValueClauseSyntax(expression);
        }

        public override LuaSyntaxNode VisitPredefinedType(PredefinedTypeSyntax node) {
            ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
            return GetTypeShortName(symbol);
        }

        private void WriteStatementOrBlock(StatementSyntax statement, LuaBlockSyntax block) {
            if(statement.IsKind(SyntaxKind.Block)) {
                var blockNode = (LuaBlockSyntax)statement.Accept(this);
                block.Statements.AddRange(blockNode.Statements);
            }
            else {
                blocks_.Push(block);
                var statementNode = (LuaStatementSyntax)statement.Accept(this);
                block.Statements.Add(statementNode);
                blocks_.Pop();
            }
        }

        #region if else switch

        public override LuaSyntaxNode VisitIfStatement(IfStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
            WriteStatementOrBlock(node.Statement, ifStatement.Body);
            ifStatements_.Push(ifStatement);
            node.Else?.Accept(this);
            ifStatements_.Pop();
            return ifStatement;
        }

        public override LuaSyntaxNode VisitElseClause(ElseClauseSyntax node) {
            if(node.Statement.IsKind(SyntaxKind.IfStatement)) {
                var ifStatement = (IfStatementSyntax)node.Statement;
                var condition = (LuaExpressionSyntax)ifStatement.Condition.Accept(this);
                LuaElseIfStatementSyntax elseIfStatement = new LuaElseIfStatementSyntax(condition);
                WriteStatementOrBlock(ifStatement.Statement, elseIfStatement.Body);
                ifStatements_.Peek().ElseIfStatements.Add(elseIfStatement);
                ifStatement.Else?.Accept(this);
                return elseIfStatement;
            }
            else {
                LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
                WriteStatementOrBlock(node.Statement, elseClause.Body);
                ifStatements_.Peek().Else = elseClause;
                return elseClause;
            }
        }

        public override LuaSyntaxNode VisitSwitchStatement(SwitchStatementSyntax node) {
            var temp = GetTempIdentifier(node);
            LuaSwitchAdapterStatementSyntax switchStatement = new LuaSwitchAdapterStatementSyntax(temp);
            switchs_.Push(switchStatement);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            switchStatement.Fill(expression, node.Sections.Select(i => (LuaStatementSyntax)i.Accept(this)));
            switchs_.Pop();
            return switchStatement;
        }

        public override LuaSyntaxNode VisitSwitchSection(SwitchSectionSyntax node) {
            bool isDefault = node.Labels.Any(i => i.Kind() == SyntaxKind.DefaultSwitchLabel);
            if(isDefault) {
                LuaBlockSyntax block = new LuaBlockSyntax();
                foreach(var statement in node.Statements) {
                    var luaStatement = (LuaStatementSyntax)statement.Accept(this);
                    block.Statements.Add(luaStatement);
                }
                return block;
            }
            else {
                var expressions = node.Labels.Select(i => (LuaExpressionSyntax)i.Accept(this));
                var condition = expressions.Aggregate((x, y) => new LuaBinaryExpressionSyntax(x, LuaSyntaxNode.Tokens.Or, y));
                LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
                foreach(var statement in node.Statements) {
                    var luaStatement = (LuaStatementSyntax)statement.Accept(this);
                    ifStatement.Body.Statements.Add(luaStatement);
                }
                return ifStatement;
            }
        }

        public override LuaSyntaxNode VisitCaseSwitchLabel(CaseSwitchLabelSyntax node) {
            var left = switchs_.Peek().Temp;
            var right = (LuaLiteralExpressionSyntax)node.Value.Accept(this);
            LuaBinaryExpressionSyntax BinaryExpression = new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.EqualsEquals, right);
            return BinaryExpression;
        }

        #endregion

        public override LuaSyntaxNode VisitBreakStatement(BreakStatementSyntax node) {
            return LuaBreakStatementSyntax.Statement;
        }

        private LuaExpressionSyntax WrapStringConcatExpression(ExpressionSyntax expression) {
            ITypeSymbol typeInfo = semanticModel_.GetTypeInfo(expression).Type;
            var original = (LuaExpressionSyntax)expression.Accept(this);
            if(typeInfo.IsStringType()) {
                if(expression is BinaryExpressionSyntax) {
                    return original;
                }

                var constValue = semanticModel_.GetConstantValue(expression);
                if(constValue.HasValue) {
                    return original;
                }
                else {
                    bool mayBeNull = MayBeNull(expression, typeInfo);
                    if(mayBeNull) {
                        LuaBinaryExpressionSyntax binaryExpression = new LuaBinaryExpressionSyntax(original, LuaSyntaxNode.Tokens.Or, LuaStringLiteralExpressionSyntax.Empty);
                        return new LuaParenthesizedExpressionSyntax(binaryExpression);
                    }
                    else {
                        return original;
                    }
                }
            }
            else if(typeInfo.SpecialType == SpecialType.System_Char) {
                var constValue = semanticModel_.GetConstantValue(expression);
                if(constValue.HasValue) {
                    string text = SyntaxFactory.Literal((char)constValue.Value).Text;
                    return new LuaIdentifierLiteralExpressionSyntax(text);
                }
                else {
                    return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.StringChar, original);
                }
            }
            else if(typeInfo.SpecialType >= SpecialType.System_Boolean && typeInfo.SpecialType <= SpecialType.System_Double) {
                return original;
            }
            else if(typeInfo.TypeKind == TypeKind.Enum) {
                if(original is LuaLiteralExpressionSyntax) {
                    var symbol = semanticModel_.GetSymbolInfo(expression).Symbol;
                    return new LuaConstLiteralExpression(symbol.Name, typeInfo.ToString());
                }
                else {
                    generator_.AddExportEnum(typeInfo.ToString());
                    LuaIdentifierNameSyntax typeName = GetTypeShortName(typeInfo);
                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(original, LuaIdentifierNameSyntax.ToEnumString, true);
                    return new LuaInvocationExpressionSyntax(memberAccess, typeName);
                }
            }
            else if(typeInfo.IsValueType) {
                LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(original, LuaIdentifierNameSyntax.ToStr, true);
                return new LuaInvocationExpressionSyntax(memberAccess);
            }
            else {
                LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(original, LuaIdentifierNameSyntax.ToStr, true);
                var andExpression = new LuaBinaryExpressionSyntax(original, LuaSyntaxNode.Tokens.And, new LuaInvocationExpressionSyntax(memberAccess));
                var orExpression = new LuaBinaryExpressionSyntax(andExpression, LuaSyntaxNode.Tokens.Or, LuaStringLiteralExpressionSyntax.Empty);
                return new LuaParenthesizedExpressionSyntax(orExpression);
            }
        }

        private LuaBinaryExpressionSyntax BuildStringConcatExpression(BinaryExpressionSyntax node) {
            return BuildStringConcatExpression(node.Left, node.Right);
        }

        private LuaBinaryExpressionSyntax BuildStringConcatExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode) {
            var left = WrapStringConcatExpression(leftNode);
            var right = WrapStringConcatExpression(rightNode);
            return new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.Concatenation, right);
        }

        private LuaExpressionSyntax BuildBinaryInvokeExpression(BinaryExpressionSyntax node, LuaIdentifierNameSyntax name) {
            var left = (LuaExpressionSyntax)node.Left.Accept(this);
            var right = (LuaExpressionSyntax)node.Right.Accept(this);
            return new LuaInvocationExpressionSyntax(name, left, right);
        }

        private LuaExpressionSyntax BuildIntegerDivExpression(BinaryExpressionSyntax node) {
            if(IsLuaNewest) {
                return BuildBinaryExpression(node, LuaSyntaxNode.Tokens.Div);
            }
            else {
                return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.IntegerDiv);
            }
        }

        private LuaBinaryExpressionSyntax BuildBinaryExpression(BinaryExpressionSyntax node, string operatorToken) {
            var left = (LuaExpressionSyntax)node.Left.Accept(this);
            var right = (LuaExpressionSyntax)node.Right.Accept(this);
            return new LuaBinaryExpressionSyntax(left, operatorToken, right);
        }

        private LuaExpressionSyntax BuildBitExpression(BinaryExpressionSyntax node, string boolOperatorToken, LuaIdentifierNameSyntax otherName) {
            var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
            if(leftType.SpecialType == SpecialType.System_Boolean) {
                return BuildBinaryExpression(node, boolOperatorToken);
            }
            else if(!IsLuaNewest) {
                return BuildBinaryInvokeExpression(node, otherName);
            }
            else {
                string operatorToken = GetOperatorToken(node.OperatorToken);
                return BuildBinaryExpression(node, operatorToken);
            }
        }

        public override LuaSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
            switch(node.Kind()) {
                case SyntaxKind.AddExpression: {
                        var methodSymbol = semanticModel_.GetSymbolInfo(node).Symbol as IMethodSymbol;
                        if(methodSymbol != null) {
                            if(methodSymbol.ContainingType.IsStringType()) {
                                return BuildStringConcatExpression(node);
                            }
                            else if(methodSymbol.ContainingType.IsDelegateType()) {
                                return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.DelegateCombine);
                            }
                        }
                        break;
                    }
                case SyntaxKind.SubtractExpression: {
                        var methodSymbol = semanticModel_.GetSymbolInfo(node).Symbol as IMethodSymbol;
                        if(methodSymbol != null && methodSymbol.ContainingType.IsDelegateType()) {
                            return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.DelegateRemove);
                        }
                        break;
                    }
                case SyntaxKind.DivideExpression: {
                        var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
                        var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
                        if(leftType.IsIntegerType() && rightType.IsIntegerType()) {
                            return BuildIntegerDivExpression(node);
                        }
                        break;
                    }
                case SyntaxKind.ModuloExpression: {
                        if(!IsLuaNewest) {
                            var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
                            var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
                            if(leftType.IsIntegerType() && rightType.IsIntegerType()) {
                                return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.IntegerMod);
                            }
                        }
                        break;
                    }
                case SyntaxKind.LeftShiftExpression: {
                        if(!IsLuaNewest) {
                            return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.ShiftLeft);
                        }
                        break;
                    }
                case SyntaxKind.RightShiftExpression: {
                        if(!IsLuaNewest) {
                            return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.ShiftRight);
                        }
                        break;
                    }
                case SyntaxKind.BitwiseOrExpression: {
                        return BuildBitExpression(node, LuaSyntaxNode.Tokens.Or, LuaIdentifierNameSyntax.BitOr);
                    }
                case SyntaxKind.BitwiseAndExpression: {
                        return BuildBitExpression(node, LuaSyntaxNode.Tokens.And, LuaIdentifierNameSyntax.BitAnd);
                    }
                case SyntaxKind.ExclusiveOrExpression: {
                        return BuildBitExpression(node, LuaSyntaxNode.Tokens.NotEquals, LuaIdentifierNameSyntax.BitXor);
                    }
                case SyntaxKind.IsExpression: {
                        return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.Is);
                    }
                case SyntaxKind.AsExpression: {
                        return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.As);
                    }
            }
            string operatorToken = GetOperatorToken(node.OperatorToken);
            return BuildBinaryExpression(node, operatorToken);
        }

        private LuaAssignmentExpressionSyntax GetLuaAssignmentExpressionSyntax(ExpressionSyntax operand, bool isPlus) {
            var expression = (LuaExpressionSyntax)operand.Accept(this);
            string operatorToken = isPlus ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub;
            LuaBinaryExpressionSyntax binary = new LuaBinaryExpressionSyntax(expression, operatorToken, LuaIdentifierNameSyntax.One);
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(expression, binary);
            return assignment;
        }

        public override LuaSyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node) {
            SyntaxKind kind = node.Kind();
            if(kind == SyntaxKind.PreIncrementExpression || kind == SyntaxKind.PreDecrementExpression) {
                LuaAssignmentExpressionSyntax assignment = GetLuaAssignmentExpressionSyntax(node.Operand, kind == SyntaxKind.PreIncrementExpression);
                if(node.Parent.IsKind(SyntaxKind.ExpressionStatement) || node.Parent.IsKind(SyntaxKind.ForStatement)) {
                    return assignment;
                }
                else {
                    CurBlock.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                    return assignment.Left;
                }
            }
            else {
                var operand = (LuaExpressionSyntax)node.Operand.Accept(this);
                string operatorToken = GetOperatorToken(node.OperatorToken);
                LuaPrefixUnaryExpressionSyntax unaryExpression = new LuaPrefixUnaryExpressionSyntax(operand, operatorToken);
                return unaryExpression;
            }
        }

        public override LuaSyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node) {
            SyntaxKind kind = node.Kind();
            if(kind != SyntaxKind.PostIncrementExpression && kind != SyntaxKind.PostDecrementExpression) {
                throw new NotSupportedException();
            }
            LuaAssignmentExpressionSyntax assignment = GetLuaAssignmentExpressionSyntax(node.Operand, kind == SyntaxKind.PostIncrementExpression);
            if(node.Parent.IsKind(SyntaxKind.ExpressionStatement) || node.Parent.IsKind(SyntaxKind.ForStatement)) {
                return assignment;
            }
            else {
                var temp = GetTempIdentifier(node);
                LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(temp);
                variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(assignment.Left);
                CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(variableDeclarator));
                CurBlock.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                return temp;
            }
        }

        public override LuaSyntaxNode VisitContinueStatement(ContinueStatementSyntax node) {
            return LuaContinueAdapterStatementSyntax.Statement;      
        }

        private void VisitLoopBody(StatementSyntax bodyStatement, LuaBlockSyntax block) {
            bool hasContinue = IsContinueExists(bodyStatement);
            if(hasContinue) {
                // http://lua-users.org/wiki/ContinueProposal
                var continueIdentifier = LuaIdentifierNameSyntax.Continue;
                block.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(continueIdentifier)));
                LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(LuaIdentifierNameSyntax.One);
                WriteStatementOrBlock(bodyStatement, repeatStatement.Body);
                LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(continueIdentifier, LuaIdentifierNameSyntax.True);
                repeatStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                block.Statements.Add(repeatStatement);
                LuaIfStatementSyntax IfStatement = new LuaIfStatementSyntax(new LuaPrefixUnaryExpressionSyntax(continueIdentifier, LuaSyntaxNode.Tokens.Not));
                IfStatement.Body.Statements.Add(LuaBreakStatementSyntax.Statement);
                block.Statements.Add(IfStatement);
            }
            else {
                WriteStatementOrBlock(bodyStatement, block);
            }
        }

        public override LuaSyntaxNode VisitForEachStatement(ForEachStatementSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaForInStatementSyntax forInStatement = new LuaForInStatementSyntax(identifier, expression);
            VisitLoopBody(node.Statement, forInStatement.Body);
            return forInStatement;
        }

        public override LuaSyntaxNode VisitWhileStatement(WhileStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            LuaWhileStatementSyntax whileStatement = new LuaWhileStatementSyntax(condition);
            VisitLoopBody(node.Statement, whileStatement.Body);
            return whileStatement;
        }

        public override LuaSyntaxNode VisitForStatement(ForStatementSyntax node) {
            LuaBlockSyntax block = new LuaBlockStatementSyntax();
            blocks_.Push(block);

            if(node.Declaration != null) {
                block.Statements.Add((LuaVariableDeclarationSyntax)node.Declaration.Accept(this));
            }
            var initializers = node.Initializers.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
            block.Statements.AddRange(initializers);

            LuaExpressionSyntax condition = node.Condition != null ? (LuaExpressionSyntax)node.Condition.Accept(this) : LuaIdentifierNameSyntax.True;
            LuaWhileStatementSyntax whileStatement = new LuaWhileStatementSyntax(condition);
            blocks_.Push(whileStatement.Body);
            VisitLoopBody(node.Statement, whileStatement.Body);
            var incrementors = node.Incrementors.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
            whileStatement.Body.Statements.AddRange(incrementors);
            blocks_.Pop();
            block.Statements.Add(whileStatement);
            blocks_.Pop();

            return block;
        }

        public override LuaSyntaxNode VisitDoStatement(DoStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            var newCondition = new LuaPrefixUnaryExpressionSyntax(new LuaParenthesizedExpressionSyntax(condition), LuaSyntaxNode.Tokens.Not);
            LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(newCondition);
            VisitLoopBody(node.Statement, repeatStatement.Body);
            return repeatStatement;
        }

        public override LuaSyntaxNode VisitYieldStatement(YieldStatementSyntax node) {
            CurFunction.HasYield = true;
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            if(node.IsKind(SyntaxKind.YieldBreakStatement)) {
                LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(expression);
                return returnStatement;
            }
            else {
                LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.YieldReturn);
                invocationExpression.AddArgument(expression);
                return new LuaExpressionStatementSyntax(invocationExpression);
            }
        }

        public override LuaSyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaParenthesizedExpressionSyntax(expression);
        }

        /// <summary>
        /// http://lua-users.org/wiki/TernaryOperator
        /// </summary>
        public override LuaSyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node) {
            bool mayBeNullOrFalse = MayBeNullOrFalse(node.WhenTrue);
            if(mayBeNullOrFalse) {
                var temp = GetTempIdentifier(node);
                var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
                LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
                blocks_.Push(ifStatement.Body);
                var whenTrue = (LuaExpressionSyntax)node.WhenTrue.Accept(this);
                blocks_.Pop();
                ifStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(temp, whenTrue)));

                LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
                blocks_.Push(elseClause.Body);
                var whenFalse = (LuaExpressionSyntax)node.WhenFalse.Accept(this);
                blocks_.Pop();
                elseClause.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(temp, whenFalse)));

                ifStatement.Else = elseClause;
                CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(temp)));
                CurBlock.Statements.Add(ifStatement);
                return temp;
            }
            else {
                var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
                var whenTrue = (LuaExpressionSyntax)node.WhenTrue.Accept(this);
                var whenFalse = (LuaExpressionSyntax)node.WhenFalse.Accept(this);
                return new LuaBinaryExpressionSyntax(new LuaBinaryExpressionSyntax(condition, LuaSyntaxNode.Tokens.And, whenTrue), LuaSyntaxNode.Tokens.Or, whenFalse);
            }
        }

        public override LuaSyntaxNode VisitGotoStatement(GotoStatementSyntax node) {
            if(node.CaseOrDefaultKeyword.IsKind(SyntaxKind.CaseKeyword)) {
                const string kCaseLabel = "caseLabel";
                var switchStatement = switchs_.Peek();
                int caseIndex = GetCaseLabelIndex(node);
                var labelIdentifier = switchStatement.CaseLabels.GetOrDefault(caseIndex);
                if(labelIdentifier == null) {
                    string uniqueName = GetUniqueIdentifier(kCaseLabel + caseIndex, node);
                    labelIdentifier = new LuaIdentifierNameSyntax(uniqueName);
                    switchStatement.CaseLabels.Add(caseIndex, labelIdentifier);
                }
                return new LuaGotoCaseAdapterStatement(labelIdentifier);
            }
            else if(node.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword)) {
                const string kDefaultLabel = "defaultLabel";
                var switchStatement = switchs_.Peek();
                if(switchStatement.DefaultLabel == null) {
                    string identifier = GetUniqueIdentifier(kDefaultLabel, node);
                    switchStatement.DefaultLabel = new LuaIdentifierNameSyntax(identifier);
                }
                return new LuaGotoCaseAdapterStatement(switchStatement.DefaultLabel);
            }
            else {
                var identifier = (LuaIdentifierNameSyntax)node.Expression.Accept(this);
                return new LuaGotoStatement(identifier);
            }
        }

        public override LuaSyntaxNode VisitLabeledStatement(LabeledStatementSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            var statement = (LuaStatementSyntax)node.Statement.Accept(this);
            return new LuaLabeledStatement(identifier, statement);
        }

        public override LuaSyntaxNode VisitEmptyStatement(EmptyStatementSyntax node) {
            return LuaStatementSyntax.Empty;
        }

        public override LuaSyntaxNode VisitCastExpression(CastExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);

            var originalType = semanticModel_.GetTypeInfo(node.Expression).Type;
            var targetType = semanticModel_.GetTypeInfo(node.Type).Type;
            if(targetType.IsAssignableFrom(originalType)) {
                return expression;
            }

            var typeExpression = (LuaExpressionSyntax)node.Type.Accept(this);
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Cast, typeExpression, expression);
        }

        public override LuaSyntaxNode VisitCheckedStatement(CheckedStatementSyntax node) {
            LuaStatementListSyntax statements = new LuaStatementListSyntax();
            statements.Statements.Add(new LuaShortCommentStatement(" " + node.Keyword.ValueText));
            var block = (LuaStatementSyntax)node.Block.Accept(this);
            statements.Statements.Add(block);
            return statements;
        }

        public override LuaSyntaxNode VisitCheckedExpression(CheckedExpressionSyntax node) {
            //TODO 未处理
            return node.Expression.Accept(this);
        }
    }
}