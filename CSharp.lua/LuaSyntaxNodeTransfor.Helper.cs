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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpLua.LuaAst;

namespace CSharpLua {
    public sealed partial class LuaSyntaxNodeTransfor {
        private static readonly Regex codeTemplateRegex_ = new Regex(@"(,?\s*)\{(\*?[\w|^]+)\}", RegexOptions.Compiled);
        private Dictionary<ISymbol, string> localReservedNames_ = new Dictionary<ISymbol, string>();
        private int localMappingCounter_;

        private abstract class LuaSyntaxSearcher : CSharpSyntaxWalker {
            private sealed class FoundException : Exception {
            }
            protected void Found() {
                throw new FoundException();
            }

            public bool Find(SyntaxNode root) {
                try {
                    Visit(root);
                }
                catch(FoundException) {
                    return true;
                }
                return false;
            }
        }

        private sealed class LocalVarSearcher : LuaSyntaxSearcher {
            private string name_;

            public LocalVarSearcher(string name) {
                name_ = name;
            }

            public override void VisitParameter(ParameterSyntax node) {
                if(node.Identifier.ValueText == name_) {
                    Found();
                }
            }

            public override void VisitVariableDeclarator(VariableDeclaratorSyntax node) {
                if(node.Identifier.ValueText == name_) {
                    Found();
                }
            }
        }

        private bool IsLocalVarExists(string name, SyntaxNode root) {
            LocalVarSearcher searcher = new LocalVarSearcher(name);
            return searcher.Find(root);
        }

        private string GetNewIdentifierName(string name, int index) {
            switch(index) {
                case 0:
                    return name;
                case 1:
                    return name + "_";
                case 2:
                    return "_" + name;
                default:
                    return name + (index - 2);
            }
        }

        private SyntaxNode FindFromCur(SyntaxNode node, Func<SyntaxNode, bool> macth) {
            var cur = node;
            while(cur != null) {
                if(macth(cur)) {
                    return cur;
                }
                cur = cur.Parent;
            }
            return null;
        }

        private SyntaxNode FindParent(SyntaxNode node, Func<SyntaxNode, bool> macth) {
            var parent = node.Parent;
            while(true) {
                if(macth(parent)) {
                    return parent;
                }
                parent = parent.Parent;
            }
        }

        private SyntaxNode FindParent(SyntaxNode node, SyntaxKind kind) {
            return FindParent(node, i => i.IsKind(kind));
        }

        private T FindParent<T>(SyntaxNode node) where T : CSharpSyntaxNode {
            return (T)FindParent(node, i => i is T);
        }

        private string GetUniqueIdentifier(string name, SyntaxNode node, int index = 0) {
            var root = FindParent<BaseMethodDeclarationSyntax>(node);
            while(true) {
                string newName = GetNewIdentifierName(name, index);
                bool exists = IsLocalVarExists(newName, root);
                if(!exists) {
                    return newName;
                }
                ++index;
            }
        }

        private bool CheckReservedWord(ref string name, SyntaxNode node) {
            if(LuaSyntaxNode.IsReservedWord(name)) {
                name = GetUniqueIdentifier(name, node, 1);
                AddReservedMapping(name, node);
                return true;
            }
            return false;
        }

        private void AddReservedMapping(string name, SyntaxNode node) {
            ISymbol symbol = semanticModel_.GetDeclaredSymbol(node);
            Contract.Assert(symbol != null);
            localReservedNames_.Add(symbol, name);
        }

        private void CheckParameterName(ref LuaParameterSyntax parameter, ParameterSyntax node) {
            string name = parameter.Identifier.ValueText;
            bool isReserved = CheckReservedWord(ref name, node);
            if(isReserved) {
                parameter = new LuaParameterSyntax(new LuaIdentifierNameSyntax(name));
            }
        }

        private void CheckVariableDeclaratorName(ref LuaIdentifierNameSyntax identifierName, SyntaxNode node) {
            string name = identifierName.ValueText;
            bool isReserved = CheckReservedWord(ref name, node);
            if(isReserved) {
                identifierName = new LuaIdentifierNameSyntax(name);
            }
        }

        private void CheckReservedWord(ref string name, ISymbol symbol) {
            if(LuaSyntaxNode.IsReservedWord(name)) {
                name = localReservedNames_[symbol];
            }
        }

        private int GetConstructorIndex(IMethodSymbol constructorSymbol) {
            if(constructorSymbol.IsFromCode()) {
                var typeSymbol = (INamedTypeSymbol)constructorSymbol.ReceiverType;
                var ctors = typeSymbol.Constructors.Where(i => !i.IsStatic).ToList();
                if(ctors.Count > 1) {
                    int firstCtorIndex = ctors.IndexOf(i => i.Parameters.IsEmpty);
                    if(firstCtorIndex != -1) {
                        var firstCtor = ctors[firstCtorIndex];
                        ctors.Remove(firstCtor);
                        ctors.Insert(0, firstCtor);
                    }
                    int index = ctors.IndexOf(constructorSymbol);
                    Contract.Assert(index != -1);
                    int ctroCounter = index + 1;
                    return ctroCounter;
                }
            }
            return 0;
        }

        private sealed class ContinueSearcher : LuaSyntaxSearcher {
            public override void VisitContinueStatement(ContinueStatementSyntax node) {
                Found();
            }
        }

        private bool IsContinueExists(SyntaxNode node) {
            ContinueSearcher searcher = new ContinueSearcher();
            return searcher.Find(node);
        }

        private sealed class ReturnStatementSearcher : LuaSyntaxSearcher {
            public override void VisitReturnStatement(ReturnStatementSyntax node) {
                Found();
            }
        }

        private bool IsReturnExists(SyntaxNode node) {
            ReturnStatementSearcher searcher = new ReturnStatementSearcher();
            return searcher.Find(node);
        }

        private int GetCaseLabelIndex(GotoStatementSyntax node) {
            var switchStatement = (SwitchStatementSyntax)FindParent(node, SyntaxKind.SwitchStatement);
            int index = 0;
            foreach(var section in switchStatement.Sections) {
                bool isFound = section.Labels.Any(i => {
                    if(i.IsKind(SyntaxKind.CaseSwitchLabel)) {
                        var label = (CaseSwitchLabelSyntax)i;
                        if(label.Value.ToString() == node.Expression.ToString()) {
                            return true;
                        }
                    }
                    return false;
                });
                if(isFound) {
                    return index;
                }
            }
            throw new InvalidOperationException();
        }

        private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression) {
            return BuildCodeTemplateExpression(codeTemplate, targetExpression, Array.Empty<ExpressionSyntax>(), ImmutableArray<ITypeSymbol>.Empty);
        }

        private void AddCodeTemplateExpression(LuaExpressionSyntax expression, string comma, LuaCodeTemplateExpressionSyntax codeTemplateExpression) {
            if(!string.IsNullOrEmpty(comma)) {
                codeTemplateExpression.Expressions.Add(new LuaIdentifierNameSyntax(comma));
            }
            codeTemplateExpression.Expressions.Add(expression);
        }

        private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression, IEnumerable<ExpressionSyntax> arguments, ImmutableArray<ITypeSymbol> typeArguments) {
            LuaCodeTemplateExpressionSyntax codeTemplateExpression = new LuaCodeTemplateExpressionSyntax();

            var matchs = codeTemplateRegex_.Matches(codeTemplate);
            int prevIndex = 0;
            foreach(Match match in matchs) {
                if(match.Index > prevIndex) {
                    string prevToken = codeTemplate.Substring(prevIndex, match.Index - prevIndex);
                    codeTemplateExpression.Expressions.Add(new LuaIdentifierNameSyntax(prevToken));
                }
                string comma = match.Groups[1].Value;
                string key = match.Groups[2].Value;
                if(key == "this") {
                    AddCodeTemplateExpression(BuildMemberAccessTargetExpression(targetExpression), comma, codeTemplateExpression);
                }
                else if(key == "class") {
                    var type = semanticModel_.GetTypeInfo(targetExpression).Type;
                    var typeName = GetTypeName(type);
                    AddCodeTemplateExpression(typeName, comma, codeTemplateExpression);
                }
                else if(key[0] == '^') {
                    int typeIndex;
                    if(int.TryParse(key.Substring(1), out typeIndex)) {
                        var typeArgument = typeArguments.GetOrDefault(typeIndex);
                        if(typeArgument != null) {
                            var typeName = GetTypeName(typeArgument);
                            AddCodeTemplateExpression(typeName, comma, codeTemplateExpression);
                        }
                    }
                }
                else if(key[0] == '*') {
                    int paramsIndex;
                    if(int.TryParse(key.Substring(1), out paramsIndex)) {
                        LuaCodeTemplateExpressionSyntax paramsExpression = new LuaCodeTemplateExpressionSyntax();
                        foreach(var argument in arguments.Skip(paramsIndex)) {
                            var argumentExpression = (LuaExpressionSyntax)argument.Accept(this);
                            paramsExpression.Expressions.Add(argumentExpression);
                        }
                        if(paramsExpression.Expressions.Count > 0) {
                            AddCodeTemplateExpression(paramsExpression, comma, codeTemplateExpression);
                        }
                    }
                }
                else {
                    int argumentIndex;
                    if(int.TryParse(key, out argumentIndex)) {
                        var argument = arguments.ElementAtOrDefault(argumentIndex);
                        if(argument != null) {
                            var argumentExpression = (LuaExpressionSyntax)argument.Accept(this);
                            AddCodeTemplateExpression(argumentExpression, comma, codeTemplateExpression);
                        }
                    }
                }
                prevIndex = match.Index + match.Length;
            }

            if(prevIndex < codeTemplate.Length) {
                string last = codeTemplate.Substring(prevIndex);
                codeTemplateExpression.Expressions.Add(new LuaIdentifierNameSyntax(last));
            }

            return codeTemplateExpression;
        }

        private bool IsPropertyField(IPropertySymbol symbol) {
            return generator_.IsPropertyField(symbol);
        }

        private bool IsEventFiled(IEventSymbol symbol) {
            return generator_.IsEventFiled(symbol);
        }

        private INamedTypeSymbol GetTypeDeclarationSymbol(SyntaxNode node) {
            var typeDeclaration = (TypeDeclarationSyntax)FindParent(node, i => i.IsKind(SyntaxKind.ClassDeclaration) || i.IsKind(SyntaxKind.StructDeclaration));
            return semanticModel_.GetDeclaredSymbol(typeDeclaration);
        }

        private bool IsInternalMember(SyntaxNode node, ISymbol symbol) {
            bool isVirtual = symbol.IsOverridable() && !symbol.ContainingType.IsSealed;
            if(!isVirtual) {
                var typeSymbol = GetTypeDeclarationSymbol(node);
                if(typeSymbol.Equals(symbol.ContainingType)) {
                    return true;
                }
            }
            return false;
        }

        private LuaInvocationExpressionSyntax BuildEmptyArray(LuaExpressionSyntax baseType) {
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.ArrayEmpty, baseType);
        }

        private LuaInvocationExpressionSyntax BuildArray(ITypeSymbol elementType, params LuaExpressionSyntax[] elements) {
            IEnumerable<LuaExpressionSyntax> expressions = elements;
            return BuildArray(elementType, expressions);
        }

        private LuaInvocationExpressionSyntax BuildArray(ITypeSymbol elementType, IEnumerable<LuaExpressionSyntax> elements) {
            LuaExpressionSyntax baseType = GetTypeName(elementType);
            var arrayType = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Array, baseType);
            return new LuaInvocationExpressionSyntax(arrayType, elements);
        }

        private LuaLiteralExpressionSyntax GetConstLiteralExpression(object constantValue) {
            if(constantValue != null) {
                var code = Type.GetTypeCode(constantValue.GetType());
                switch(code) {
                    case TypeCode.Char: {
                            return new LuaCharacterLiteralExpression((char)constantValue);
                        }
                    case TypeCode.String: {
                            return BuildStringLiteralExpression((string)constantValue);
                        }
                    case TypeCode.Boolean: {
                            bool v = (bool)constantValue;
                            return new LuaIdentifierLiteralExpressionSyntax(v ? LuaIdentifierNameSyntax.True : LuaIdentifierNameSyntax.False);
                        }
                    default: {
                            return new LuaIdentifierLiteralExpressionSyntax(constantValue.ToString());
                        }
                }
            }
            else {
                return LuaIdentifierLiteralExpressionSyntax.Nil;
            }
        }

        private LuaLiteralExpressionSyntax GetConstLiteralExpression(IFieldSymbol constField) {
            Contract.Assert(constField.HasConstantValue);
            if(constField.Type.SpecialType == SpecialType.System_Char) {
                return new LuaCharacterLiteralExpression((char)constField.ConstantValue);
            }
            else {
                var constExpression = GetConstLiteralExpression(constField.ConstantValue);
                string identifierToken = constField.ContainingType.Name + '.' + constField.Name;
                return new LuaConstLiteralExpression(constExpression, identifierToken);
            }
        }

        private LuaLiteralExpressionSyntax BuildStringLiteralTokenExpression(SyntaxToken token) {
            if(token.Text[0] == '@') {
                return BuildVerbatimStringExpression(token.ValueText);
            }
            else {
                return new LuaIdentifierLiteralExpressionSyntax(token.Text);
            }
        }

        private LuaIdentifierLiteralExpressionSyntax BuildStringLiteralExpression(string value) {
            string text = SyntaxFactory.Literal(value).Text;
            return new LuaIdentifierLiteralExpressionSyntax(text);
        }

        private LuaVerbatimStringLiteralExpressionSyntax BuildVerbatimStringExpression(string value) {
            const string kCloseBracket = LuaSyntaxNode.Tokens.CloseBracket;
            char equals = LuaSyntaxNode.Tokens.Equals[0];
            int count = 0;
            while(true) {
                string closeToken = kCloseBracket + new string(equals, count) + kCloseBracket;
                if(!value.Contains(closeToken)) {
                    break;
                }
                ++count;
            }
            if(value[0] == '\n') {
                value = '\n' + value;
            }
            return new LuaVerbatimStringLiteralExpressionSyntax(value, count);
        }

        private enum CallerAttributeKind {
            None,
            Line,
            Member,
            FilePath,
        }

        private CallerAttributeKind GetCallerAttributeKind(INamedTypeSymbol typeSymbol) {
            switch(typeSymbol.ToString()) {
                case "System.Runtime.CompilerServices.CallerLineNumberAttribute":
                    return CallerAttributeKind.Line;
                case "System.Runtime.CompilerServices.CallerMemberNameAttribute":
                    return CallerAttributeKind.Member;
                case "System.Runtime.CompilerServices.CallerFilePathAttribute":
                    return CallerAttributeKind.FilePath;
                default:
                    return CallerAttributeKind.None;
            }
        }

        private CallerAttributeKind GetCallerAttributeKind(IParameterSymbol parameter) {
            foreach(var attribute in parameter.GetAttributes()) {
                var callerKind = GetCallerAttributeKind(attribute.AttributeClass);
                if(callerKind != CallerAttributeKind.None) {
                    return callerKind;
                }
            }
            return CallerAttributeKind.None;
        }

        private LuaExpressionSyntax CheckCallerAttribute(IParameterSymbol parameter, SyntaxNode node) {
            var kind = GetCallerAttributeKind(parameter);
            switch(kind) {
                case CallerAttributeKind.Line: {
                        var lineSpan = node.SyntaxTree.GetLineSpan(node.Span);
                        return new LuaIdentifierNameSyntax(lineSpan.StartLinePosition.Line + 1);
                    }
                case CallerAttributeKind.Member: {
                        var parentMethod = (MethodDeclarationSyntax)FindParent(node, SyntaxKind.MethodDeclaration);
                        return new LuaStringLiteralExpressionSyntax(new LuaIdentifierNameSyntax(parentMethod.Identifier.ValueText));
                    }
                case CallerAttributeKind.FilePath: {
                        return BuildStringLiteralExpression(node.SyntaxTree.FilePath);
                    }
                default:
                    return null;
            }
        }

        private LuaExpressionSyntax CheckUsingStaticNameSyntax(ISymbol symbol, NameSyntax node) {
            if(!node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                if(symbol.ContainingType != GetTypeDeclarationSymbol(node)) {           //using static
                    var luadTypeExpression = GetTypeName(symbol.ContainingType);
                    return luadTypeExpression;
                }
            }
            return null;
        }

        private bool MayBeFalse(ExpressionSyntax expression, ITypeSymbol type) {
            bool mayBeFalse = false;
            if(type.IsValueType) {
                if(type.SpecialType == SpecialType.System_Boolean) {
                    var constValue = semanticModel_.GetConstantValue(expression);
                    if(constValue.HasValue && (bool)constValue.Value) {
                        mayBeFalse = false;
                    }
                    else {
                        mayBeFalse = true;
                    }
                }
            }
            return mayBeFalse;
        }

        private bool MayBeNull(ExpressionSyntax expression, ITypeSymbol type) {
            if(expression.IsKind(SyntaxKind.ObjectInitializerExpression)) {
                return false;
            }

            Contract.Assert(type != null);
            bool mayBeNull;
            if(type.IsValueType) {
                mayBeNull = false;
            }
            else if(type.IsStringType()) {
                var constValue = semanticModel_.GetConstantValue(expression);
                if(constValue.HasValue) {
                    mayBeNull = false;
                }
                else {
                    if(expression.IsKind(SyntaxKind.InvocationExpression)) {
                        var invocation = (InvocationExpressionSyntax)expression;
                        if(invocation.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
                            if(memberAccess.Name.Identifier.ValueText == LuaIdentifierNameSyntax.ToStr.ValueText) {
                                var typeInfo = semanticModel_.GetTypeInfo(memberAccess.Expression).Type;
                                if(typeInfo.SpecialType > SpecialType.System_Object) {
                                    return false;
                                }
                            }
                        }
                    }
                    else if(expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;
                        var typeInfo = semanticModel_.GetTypeInfo(memberAccess.Expression).Type;
                        if(typeInfo.SpecialType > SpecialType.System_Object) {
                            return false;
                        }
                    }
                    mayBeNull = true;
                }
            }
            else {
                mayBeNull = true;
            }
            return mayBeNull;
        }

        private bool MayBeNullOrFalse(ExpressionSyntax conditionalWhenTrue) {
            if(conditionalWhenTrue.IsKind(SyntaxKind.NullLiteralExpression)) {
                return true;
            }
            var type = semanticModel_.GetTypeInfo(conditionalWhenTrue).Type;
            return MayBeNull(conditionalWhenTrue, type) || MayBeFalse(conditionalWhenTrue, type);
        }

        internal void ImportTypeName(ref string name, ISymbol symbol) {
            if(baseNameNodeCounter_ == 0) {
                int pos = name.LastIndexOf('.');
                if(pos != -1) {
                    string prefix = name.Substring(0, pos);
                    if(prefix != LuaIdentifierNameSyntax.System.ValueText) {
                        string newPrefix = prefix.Replace(".", "");
                        var methodInfo = CurMethodInfoOrNull;
                        if(methodInfo != null) {
                            var syntaxReference = methodInfo.Symbol.DeclaringSyntaxReferences.First();
                            var root = syntaxReference.GetSyntax();
                            if(IsLocalVarExists(newPrefix, root)) {
                                return;
                            }
                        }
                        name = newPrefix + name.Substring(pos);
                        CurCompilationUnit.AddImport(prefix, newPrefix, symbol.IsFromCode());
                    }
                }
            }
        }

        private LuaIdentifierNameSyntax GetTypeShortName(ISymbol symbol) {
            return XmlMetaProvider.GetTypeShortName(symbol, this);
        }

        private LuaExpressionSyntax GetTypeName(ISymbol symbol) {
            return XmlMetaProvider.GetTypeName(symbol, this);
        }

        private LuaExpressionSyntax BuildFieldOrPropertyMemberAccessExpression(LuaExpressionSyntax expression, LuaExpressionSyntax name, bool isStatic) {
            var propertyMethod = name as LuaPropertyAdapterExpressionSyntax;
            if(propertyMethod != null) {
                var arguments = propertyMethod.ArgumentList.Arguments;
                if(arguments.Count == 1) {
                    if(arguments[0].Expression == LuaIdentifierNameSyntax.This) {
                        propertyMethod.ArgumentList.Arguments[0] = new LuaArgumentSyntax(expression);
                    }
                }
                else {
                    propertyMethod.Update(expression, !isStatic);
                }
                return propertyMethod;
            }
            else {
                return new LuaMemberAccessExpressionSyntax(expression, name);
            }
        }

        public override LuaSyntaxNode VisitAttributeList(AttributeListSyntax node) {
            throw new InvalidOperationException();
        }

        public override LuaSyntaxNode VisitAttributeArgument(AttributeArgumentSyntax node) {
            throw new InvalidOperationException();
        }

        public override LuaSyntaxNode VisitNameColon(NameColonSyntax node) {
            throw new InvalidOperationException();
        }

        public override LuaSyntaxNode VisitAttributeArgumentList(AttributeArgumentListSyntax node) {
            throw new InvalidOperationException();
        }

        public override LuaSyntaxNode VisitNameEquals(NameEqualsSyntax node) {
            return node.Name.Accept(this);
        }

        private LuaInvocationExpressionSyntax BuildObjectCreationInvocation(IMethodSymbol symbol, LuaExpressionSyntax expression) {
            int constructorIndex = GetConstructorIndex(symbol);
            if(constructorIndex > 0) {
                expression = new LuaMemberAccessExpressionSyntax(expression, LuaIdentifierNameSyntax.New, true);
            }
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(expression);
            if(constructorIndex > 0) {
                invocationExpression.AddArgument(new LuaIdentifierNameSyntax(constructorIndex));
            }
            return invocationExpression;
        }

        public override LuaSyntaxNode VisitAttribute(AttributeSyntax node) {
            var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node.Name).Symbol;
            INamedTypeSymbol typeSymbol = symbol.ContainingType;
            if(!generator_.IsExportAttribute(typeSymbol)) {
                return null;
            }

            INamedTypeSymbol typeDeclarationSymbol = GetTypeDeclarationSymbol(node);
            generator_.AddTypeDeclarationAttribute(typeDeclarationSymbol, typeSymbol);

            ++baseNameNodeCounter_;
            var expression = GetTypeName(typeSymbol);
            --baseNameNodeCounter_;
            LuaInvocationExpressionSyntax invocation = BuildObjectCreationInvocation(symbol, new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.Global, expression));

            if(node.ArgumentList != null) {
                List<LuaExpressionSyntax> arguments = new List<LuaExpressionSyntax>();
                List<Tuple<LuaExpressionSyntax, LuaExpressionSyntax>> initializers = new List<Tuple<LuaExpressionSyntax, LuaExpressionSyntax>>();
                List<Tuple<NameColonSyntax, ExpressionSyntax>> argumentNodeInfos = new List<Tuple<NameColonSyntax, ExpressionSyntax>>();

                foreach(var argumentNode in node.ArgumentList.Arguments) {
                    var argumentExpression = (LuaExpressionSyntax)argumentNode.Expression.Accept(this);
                    CheckValueTypeClone(argumentNode.Expression, ref argumentExpression);
                    if(argumentNode.NameEquals == null) {
                        if(argumentNode.NameColon != null) {
                            string name = argumentNode.NameColon.Name.Identifier.ValueText;
                            int index = symbol.Parameters.IndexOf(i => i.Name == name);
                            Contract.Assert(index != -1);
                            arguments.AddAt(index, argumentExpression);
                        }
                        else {
                            arguments.Add(argumentExpression);
                        }
                    }
                    else {
                        var name = (LuaExpressionSyntax)argumentNode.NameEquals.Accept(this);
                        initializers.Add(Tuple.Create(name, argumentExpression));
                    }
                }

                CheckInvocationDeafultArguments(symbol, symbol.Parameters, arguments, argumentNodeInfos, node, false);
                invocation.AddArguments(arguments);

                if(initializers.Count == 0) {
                    return invocation;
                }
                else {
                    LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
                    PushFunction(function);
                    var temp = GetTempIdentifier(node);
                    function.AddParameter(temp);

                    foreach(var initializer in initializers) {
                        var memberAccess = BuildFieldOrPropertyMemberAccessExpression(temp, initializer.Item1, false);
                        var assignmentExpression = BuildLuaSimpleAssignmentExpression(memberAccess, initializer.Item2);
                        function.AddStatement(assignmentExpression);
                    }

                    PopFunction();
                    return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Create, invocation, function);
                }
            }
            else {
                return invocation;
            }
        }

        private List<LuaExpressionSyntax> BuildAttributes(SyntaxList<AttributeListSyntax> attributeLists) {
            List<LuaExpressionSyntax> expressions = new List<LuaExpressionSyntax>();
            var attributes = attributeLists.SelectMany(i => i.Attributes);
            foreach(var node in attributes) {
                var expression = (LuaExpressionSyntax)node.Accept(this);
                if(expression != null) {
                    expressions.Add(expression);
                }
            }
            return expressions;
        }

        private void AddStructCloneMethodItem(LuaTableInitializerExpression table, LuaIdentifierNameSyntax name, ITypeSymbol typeSymbol) {
            LuaExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, name);
            CheckValueTypeClone(typeSymbol, ref memberAccess);
            table.Items.Add(new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), memberAccess));
        }

        private LuaExpressionSyntax AddStructDefaultMethod(INamedTypeSymbol symbol, LuaStructDeclarationSyntax declaration) {
            LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
            LuaExpressionSyntax typeName = GetTypeName(symbol);
            functionExpression.AddStatement(new LuaReturnStatementSyntax(new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.setmetatable, LuaTableInitializerExpression.Empty, typeName)));
            declaration.AddMethod(LuaIdentifierNameSyntax.Default, functionExpression, false);
            return typeName;
        }

        private List<LuaIdentifierNameSyntax> AddStructCloneMethod(INamedTypeSymbol symbol, LuaStructDeclarationSyntax declaration, LuaExpressionSyntax typeName) {
            List<LuaIdentifierNameSyntax> filelds = new List<LuaIdentifierNameSyntax>();
            LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
            functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
            LuaTableInitializerExpression cloneTable = new LuaTableInitializerExpression();
            foreach(var member in symbol.GetMembers()) {
                if(!member.IsStatic && member.Kind != SymbolKind.Method) {
                    switch(member.Kind) {
                        case SymbolKind.Field: {
                                IFieldSymbol memberSymbol = (IFieldSymbol)member;
                                LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(member.Name);
                                AddStructCloneMethodItem(cloneTable, name, memberSymbol.Type);
                                filelds.Add(name);
                                break;
                            }
                        case SymbolKind.Property: {
                                IPropertySymbol memberSymbol = (IPropertySymbol)member;
                                if(IsPropertyField(memberSymbol)) {
                                    LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(member.Name);
                                    AddStructCloneMethodItem(cloneTable, name, memberSymbol.Type);
                                    filelds.Add(name);
                                }
                                break;
                            }
                        case SymbolKind.Event: {
                                IEventSymbol memberSymbol = (IEventSymbol)member;
                                if(IsEventFiled(memberSymbol)) {
                                    LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(member.Name);
                                    AddStructCloneMethodItem(cloneTable, name, null);
                                    filelds.Add(name);
                                }
                                break;
                            }
                    }
                }
            }
            functionExpression.AddStatement(new LuaReturnStatementSyntax(new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.setmetatable, cloneTable, typeName)));
            declaration.AddMethod(LuaIdentifierNameSyntax.Clone, functionExpression, false);
            return filelds;
        }

        private void AddStructEqualsObjMethod(INamedTypeSymbol symbol, LuaStructDeclarationSyntax declaration, LuaExpressionSyntax typeName, List<LuaIdentifierNameSyntax> fields) {
            var thisIdentifier = LuaIdentifierNameSyntax.This;
            LuaIdentifierNameSyntax obj = LuaIdentifierNameSyntax.Obj;
            LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
            functionExpression.AddParameter(thisIdentifier);
            functionExpression.AddParameter(obj);

            var left = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.getmetatable, obj);
            LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.NotEquals, typeName));
            ifStatement.Body.Statements.Add(new LuaReturnStatementSyntax(LuaIdentifierNameSyntax.False));
            functionExpression.AddStatement(ifStatement);

            if(fields.Count > 0) {
                var equalsStatic = LuaIdentifierNameSyntax.EqualsStatic;
                LuaLocalVariableDeclaratorSyntax variableDeclarator = new LuaLocalVariableDeclaratorSyntax(equalsStatic, LuaIdentifierNameSyntax.SystemObjectEqualsStatic);
                functionExpression.AddStatement(variableDeclarator);
                LuaExpressionSyntax expression = null;
                foreach(LuaIdentifierNameSyntax field in fields) {
                    LuaMemberAccessExpressionSyntax argument1 = new LuaMemberAccessExpressionSyntax(thisIdentifier, field);
                    LuaMemberAccessExpressionSyntax argument2 = new LuaMemberAccessExpressionSyntax(obj, field);
                    LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(equalsStatic, argument1, argument2);
                    if(expression == null) {
                        expression = invocation;
                    }
                    else {
                        expression = new LuaBinaryExpressionSyntax(expression, LuaSyntaxNode.Tokens.And, invocation);
                    }
                }
                Contract.Assert(expression != null);
                functionExpression.AddStatement(new LuaReturnStatementSyntax(expression));
            }
            declaration.AddMethod(LuaIdentifierNameSyntax.EqualsObj, functionExpression, false);
        }

        private void BuildStructMethods(INamedTypeSymbol symbol, LuaStructDeclarationSyntax declaration) {
            LuaExpressionSyntax typeName = AddStructDefaultMethod(symbol, declaration);
            var fileds = AddStructCloneMethod(symbol, declaration, typeName);
            AddStructEqualsObjMethod(symbol, declaration, typeName, fileds);
        }

        private void CheckValueTypeClone(ExpressionSyntax node, ref LuaExpressionSyntax expression) {
            ITypeSymbol typeSymbol = semanticModel_.GetTypeInfo(node).Type;
            CheckValueTypeClone(typeSymbol, ref expression);
        }

        private void CheckValueTypeClone(ITypeSymbol typeSymbol, ref LuaExpressionSyntax expression) {
            if(typeSymbol != null) {
                if(typeSymbol.IsValueType && typeSymbol.TypeKind != TypeKind.Enum && typeSymbol.IsFromCode()) {
                    expression = new LuaInvocationExpressionSyntax(new LuaMemberAccessExpressionSyntax(expression, LuaIdentifierNameSyntax.Default, true));
                }
            }
        }

        private List<LuaStatementSyntax> BuildDocumentationComment(CSharpSyntaxNode node) {
            List<LuaStatementSyntax> comments = new List<LuaStatementSyntax>();
            foreach(var trivia in node.GetLeadingTrivia()) {
                if(trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)) {
                    string triviaText = trivia.ToString();
                    if(!string.IsNullOrWhiteSpace(triviaText)) {
                        string shortComment = LuaSyntaxNode.Tokens.ShortComment;
                        string comment = shortComment + triviaText.TrimEnd(Environment.NewLine).Replace(Environment.NewLine, "\n").Replace("///", shortComment);
                        var statement = new LuaExpressionStatementSyntax(new LuaIdentifierNameSyntax(comment));
                        comments.Add(statement);
                    }
                }
            }
            return comments;
        }

        private LuaExpressionSyntax BuildBaseTypeName(BaseTypeSyntax baseType) {
            ++baseNameNodeCounter_;
            var baseTypeName = (LuaExpressionSyntax)baseType.Accept(this);
            --baseNameNodeCounter_;
            return baseTypeName;
        }

        public override LuaSyntaxNode VisitTypeParameterList(TypeParameterListSyntax node) {
            LuaParameterListSyntax parameterList = new LuaParameterListSyntax();
            foreach(var typeParameter in node.Parameters) {
                var typeIdentifier = (LuaIdentifierNameSyntax)typeParameter.Accept(this);
                parameterList.Parameters.Add(new LuaParameterSyntax(typeIdentifier));
            }
            return parameterList;
        }

        private void FillExternalTypeParameters(List<LuaParameterSyntax> typeParameters, INamedTypeSymbol typeSymbol) {
            var externalType = typeSymbol.ContainingType;
            if(externalType != null) {
                FillExternalTypeParameters(typeParameters, externalType);
                foreach(var typeParameterSymbol in externalType.TypeParameters) {
                    var identifierName = new LuaIdentifierNameSyntax(typeParameterSymbol.Name);
                    typeParameters.Add(new LuaParameterSyntax(identifierName));
                }
            }
        }

        private List<LuaParameterSyntax> BuildTypeParameters(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node) {
            List<LuaParameterSyntax> typeParameters = new List<LuaParameterSyntax>();
            FillExternalTypeParameters(typeParameters, typeSymbol);
            if(node.TypeParameterList != null) {
                var parameterList = (LuaParameterListSyntax)node.TypeParameterList.Accept(this);
                typeParameters.AddRange(parameterList.Parameters);
            }
            return typeParameters;
        }

        public bool CheckFieldNameOfProtobufnet(ref string fieldName, ITypeSymbol containingType) {
            if(!containingType.Interfaces.IsEmpty) {
                if(containingType.Interfaces.First().ToString() == "ProtoBuf.IExtensible") {
                    fieldName = fieldName.TrimStart('_');
                    return true;
                }
            }
            return false;
        }

        private LuaIdentifierNameSyntax GetMemberName(ISymbol symbol) {
            return generator_.GetMemberName(symbol);
        }

        private void RemoveNilArgumentsAtTail(List<LuaExpressionSyntax> arguments) {
            int i;
            for(i = arguments.Count - 1; i >= 0; --i) {
                if(!IsNilLuaExpression(arguments[i])) {
                    break;
                }
            }
            int nilStartIndex = i + 1;
            int nilArgumentCount = arguments.Count - nilStartIndex;
            if(nilArgumentCount > 0) {
                arguments.RemoveRange(nilStartIndex, nilArgumentCount);
            }
        }

        private bool IsNilLuaExpression(LuaExpressionSyntax expression) {
            return expression == LuaIdentifierNameSyntax.Nil || expression == LuaIdentifierLiteralExpressionSyntax.Nil;
        }

        private void TryRemoveNilArgumentsAtTail(ISymbol symbol, List<LuaExpressionSyntax> arguments) {
            if(symbol.IsFromCode() || symbol.ContainingType.GetMembers(symbol.Name).Length == 1) {
                RemoveNilArgumentsAtTail(arguments);
            }
        }
    }
}
