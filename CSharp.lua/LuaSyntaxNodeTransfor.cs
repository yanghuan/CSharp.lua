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
        private SemanticModel semanticModel_;
        private Stack<LuaCompilationUnitSyntax> compilationUnits_ = new Stack<LuaCompilationUnitSyntax>();
        private Stack<LuaNamespaceDeclarationSyntax> namespaces_ = new Stack<LuaNamespaceDeclarationSyntax>();
        private Stack<LuaTypeDeclarationSyntax> typeDeclarations_ = new Stack<LuaTypeDeclarationSyntax>();
        private Stack<LuaFunctionExpressSyntax> functions_ = new Stack<LuaFunctionExpressSyntax>();
        private Stack<LuaBlockSyntax> blocks_ = new Stack<LuaBlockSyntax>();

        private static readonly Dictionary<string, string> operatorTokenMapps_ = new Dictionary<string, string>() {
            ["!="] = "~=",
            ["!"] = LuaSyntaxNode.Tokens.Not,
            ["&&"] = LuaSyntaxNode.Tokens.And,
            ["||"] = LuaSyntaxNode.Tokens.Or,
            ["??"] = LuaSyntaxNode.Tokens.Or,
        };

        public LuaSyntaxNodeTransfor(SemanticModel semanticModel) {
            semanticModel_ = semanticModel;
        }

        private static string GetOperatorToken(string operatorToken) {
            return operatorTokenMapps_.GetOrDefault(operatorToken, operatorToken);
        }

        private static string GetPredefinedTypeName(string name) {
            switch(name) {
                case "byte":
                case "sbyte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                    return "System.Int";
                case "float":
                case "double":
                    return "System.Double";
                case "bool":
                    return "System.Boolean";
                case "string":
                    return "System.String";
                case "object":
                    return "System.Object";
                default:
                    return name;
            }
        }

        private LuaCompilationUnitSyntax CurCompilationUnit {
            get {
                return compilationUnits_.Peek();
            }
        }

        private LuaNamespaceDeclarationSyntax CurNamespace {
            get {
                return namespaces_.Peek();
            }
        }

        private LuaTypeDeclarationSyntax CurType {
            get {
                return typeDeclarations_.Peek();
            }
        }

        private LuaFunctionExpressSyntax CurFunction {
            get {
                return functions_.Peek();
            }
        }

        private LuaBlockSyntax CurBlock {
            get {
                return blocks_.Peek();
            }
        }

        public override LuaSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node) {
            LuaCompilationUnitSyntax newNode = new LuaCompilationUnitSyntax() { FilePath = node.SyntaxTree.FilePath };
            compilationUnits_.Push(newNode);
            foreach(var member in node.Members) {
                LuaStatementSyntax memberNode = (LuaStatementSyntax)member.Accept(this);
                var typeDeclaration = memberNode as LuaTypeDeclarationSyntax;
                if(typeDeclaration != null) {
                    newNode.AddTypeDeclaration(typeDeclaration);
                }
                else {
                    newNode.Statements.Add(memberNode);
                }
            }
            compilationUnits_.Pop();
            return newNode;
        }

        public override LuaSyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
            LuaIdentifierNameSyntax nameNode = new LuaIdentifierNameSyntax(((IdentifierNameSyntax)node.Name).Identifier.ValueText);
            LuaNamespaceDeclarationSyntax newNode = new LuaNamespaceDeclarationSyntax(nameNode);
            namespaces_.Push(newNode);
            foreach(var member in node.Members) {
                var memberNode = (LuaTypeDeclarationSyntax)member.Accept(this);
                newNode.Add(memberNode);
            }
            namespaces_.Pop();
            return newNode;
        }

        public override LuaSyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaClassDeclarationSyntax classDeclaration = new LuaClassDeclarationSyntax(name);
            typeDeclarations_.Push(classDeclaration);
            if(node.BaseList != null) {
                List<LuaIdentifierNameSyntax> baseTypes = new List<LuaIdentifierNameSyntax>();
                foreach(var baseType in node.BaseList.Types) {
                    var baseTypeName = (LuaIdentifierNameSyntax)baseType.Accept(this);
                    baseTypes.Add(baseTypeName);
                }
                classDeclaration.AddBaseTypes(baseTypes);
            }
            foreach(var member in node.Members) {
                member.Accept(this);
            }
            typeDeclarations_.Pop();
            return classDeclaration;
        }

        public override LuaSyntaxNode VisitStructDeclaration(StructDeclarationSyntax node) {
            LuaIdentifierNameSyntax nameNode = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaStructDeclarationSyntax newNode = new LuaStructDeclarationSyntax(nameNode);
            typeDeclarations_.Push(newNode);
            foreach(var member in node.Members) {
                member.Accept(this);
            }
            typeDeclarations_.Pop();
            return newNode;
        }

        public override LuaSyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) {
            LuaIdentifierNameSyntax nameNode = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaInterfaceDeclarationSyntax newNode = new LuaInterfaceDeclarationSyntax(nameNode);
            typeDeclarations_.Push(newNode);
            foreach(var member in node.Members) {
                member.Accept(this);
            }
            typeDeclarations_.Pop();
            return newNode;
        }

        public override LuaSyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node) {
            LuaIdentifierNameSyntax nameNode = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaEnumDeclarationSyntax newNode = new LuaEnumDeclarationSyntax(nameNode);
            typeDeclarations_.Push(newNode);
            foreach(var member in node.Members) {
                member.Accept(this);
            }
            typeDeclarations_.Pop();
            return newNode;
        }

        private void VisitYield(MethodDeclarationSyntax node, LuaFunctionExpressSyntax function) {
            Contract.Assert(function.HasYield);

            var nameSyntax = (SimpleNameSyntax)node.ReturnType;
            string name = LuaSyntaxNode.Tokens.Yield + nameSyntax.Identifier.ValueText;
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.System, new LuaIdentifierNameSyntax(name));
            LuaInvocationExpressionSyntax invokeExpression = new LuaInvocationExpressionSyntax(memberAccess);
            LuaFunctionExpressSyntax wrapFunction = new LuaFunctionExpressSyntax();

            var parameters = function.ParameterList.Parameters;
            wrapFunction.ParameterList.Parameters.AddRange(parameters);
            wrapFunction.Body.Statements.AddRange(function.Body.Statements);
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
            function.Body.Statements.Add(returnStatement);
        }

        public override LuaSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaFunctionExpressSyntax function = new LuaFunctionExpressSyntax();
            functions_.Push(function);
            if(!node.Modifiers.IsStatic()) {
                function.AddParameter(LuaIdentifierNameSyntax.This);
            }
            var parameterList = (LuaParameterListSyntax)node.ParameterList.Accept(this);
            function.ParameterList.Parameters.AddRange(parameterList.Parameters);
            LuaBlockSyntax block = (LuaBlockSyntax)node.Body.Accept(this);
            function.Body.Statements.AddRange(block.Statements);
            if(function.HasYield) {
                VisitYield(node, function);
            }
            functions_.Pop();
            CurType.AddMethod(name, function, node.Modifiers.IsPrivate());
            return function;
        }

        private static string GetPredefinedTypeDefaultValue(string name) {
            switch(name) {
                case "byte":
                case "sbyte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                    return 0.ToString();
                case "float":
                case "double":
                    return 0.0.ToString();
                case "bool":
                    return false.ToString();
                default:
                    return null;
            }
        }

        private LuaInvocationExpressionSyntax BuildDefaultValueExpression(TypeSyntax type) {
            var identifierName = (LuaIdentifierNameSyntax)type.Accept(this);
            return new LuaInvocationExpressionSyntax(new LuaMemberAccessExpressionSyntax(identifierName, LuaIdentifierNameSyntax.Default));
        }

        private void VisitBaseFieldDeclarationSyntax(BaseFieldDeclarationSyntax node) {
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
                        string valueText = GetPredefinedTypeDefaultValue(typeSymbol.ToString());
                        if(valueText != null) {
                            valueExpression = new LuaIdentifierNameSyntax(valueText);
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
            bool isStatic = node.Modifiers.IsStatic();
            bool isPrivate = node.Modifiers.IsPrivate();
            bool hasGet = false;
            bool hasSet = false;
            if(node.AccessorList != null) {
                foreach(var accessor in node.AccessorList.Accessors) {
                    if(accessor.Body != null) {
                        var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                        LuaFunctionExpressSyntax functionExpress = new LuaFunctionExpressSyntax();
                        if(!isStatic) {
                            functionExpress.AddParameter(LuaIdentifierNameSyntax.This);
                        }
                        functionExpress.Body.Statements.AddRange(block.Statements);
                        LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, node.Identifier.ValueText);
                        CurType.AddMethod(name, functionExpress, isPrivate);
                        if(accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) {
                            Contract.Assert(!hasGet);
                            hasGet = true;
                        }
                        else {
                            Contract.Assert(!hasSet);
                            functionExpress.AddParameter(LuaIdentifierNameSyntax.Value);
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
                LuaFunctionExpressSyntax functionExpress = new LuaFunctionExpressSyntax();
                if(!isStatic) {
                    functionExpress.AddParameter(LuaIdentifierNameSyntax.This);
                }
                LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(expression);
                functionExpress.Body.Statements.Add(returnStatement);
                CurType.AddMethod(name, functionExpress, isPrivate);
                hasGet = true;
            }

            if(!hasGet && !hasSet) {
                if(!node.Parent.IsKind(SyntaxKind.InterfaceDeclaration)) {
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
            bool isStatic = node.Modifiers.IsStatic();
            bool isPrivate = node.Modifiers.IsPrivate();
            foreach(var accessor in node.AccessorList.Accessors) {
                var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                LuaFunctionExpressSyntax functionExpress = new LuaFunctionExpressSyntax();
                if(!isStatic) {
                    functionExpress.AddParameter(LuaIdentifierNameSyntax.This);
                }
                functionExpress.AddParameter(LuaIdentifierNameSyntax.Value);
                functionExpress.Body.Statements.AddRange(block.Statements);
                LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(false, node.Identifier.ValueText);
                CurType.AddMethod(name, functionExpress, isPrivate);
                if(accessor.IsKind(SyntaxKind.RemoveAccessorDeclaration)) {
                    name.IsGetOrAdd = false;
                }
            }

            return base.VisitEventDeclaration(node);
        }

        public override LuaSyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) {
            VisitBaseFieldDeclarationSyntax(node);
            return base.VisitEventFieldDeclaration(node);
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
            LuaBlockSyntax block = new LuaBlockSyntax();
            blocks_.Push(block);

            var comments = node.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.SingleLineCommentTrivia) || i.IsKind(SyntaxKind.MultiLineCommentTrivia));
            List<BlockCommonNode> commonNodes = new List<BlockCommonNode>();
            commonNodes.AddRange(comments.Select(i => new BlockCommonNode(i)));
            bool hasComments = commonNodes.Count > 0;
            commonNodes.AddRange(node.Statements.Select(i => new BlockCommonNode(i)));
            if(hasComments) {
                commonNodes.Sort();
            }

            int lastLine = -1;
            foreach(var common in commonNodes) {
                common.Visit(this, block, ref lastLine);
            }

            blocks_.Pop();
            SyntaxKind kind = node.Parent.Kind();
            if(kind == SyntaxKind.Block || kind == SyntaxKind.SwitchSection) {
                return new LuaBlockBlockSyntax(block);
            }
            else {
                return block;
            }
        }

        public override LuaSyntaxNode VisitReturnStatement(ReturnStatementSyntax node) {
            if(node.Expression != null) {
                var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                return new LuaReturnStatementSyntax(expression);
            }
            return new LuaReturnStatementSyntax();
        }

        public override LuaSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
            LuaExpressionSyntax expressionNode = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaExpressionStatementSyntax(expressionNode);
        }

        private LuaExpressionSyntax BuildLuaAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right) {
            var propertyAdapter = left as LuaPropertyAdapterExpressionSyntax;
            if(propertyAdapter != null) {
                propertyAdapter.IsGet = false;
                propertyAdapter.InvocationExpression.AddArgument(right);
                return propertyAdapter;
            }
            else {
                return new LuaAssignmentExpressionSyntax(left, right);
            }
        }

        public override LuaSyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) {
            if(node.Right.Kind() != SyntaxKind.SimpleAssignmentExpression ) {
                var left = (LuaExpressionSyntax)node.Left.Accept(this);
                var right = (LuaExpressionSyntax)node.Right.Accept(this);
                return BuildLuaAssignmentExpression(left, right);
            }
            else {
                List<LuaExpressionSyntax> assignments = new List<LuaExpressionSyntax>();
                var leftExpression = node.Left;
                var rightExpression = node.Right;

                while(true) {
                    var left = (LuaExpressionSyntax)leftExpression.Accept(this);
                    var assignmentRight = rightExpression as AssignmentExpressionSyntax;
                    if(assignmentRight == null) {
                        var right = (LuaExpressionSyntax)rightExpression.Accept(this);
                        assignments.Add(BuildLuaAssignmentExpression(left, right));
                        break;
                    }
                    else {
                        var right = (LuaExpressionSyntax)assignmentRight.Left.Accept(this);
                        assignments.Add(BuildLuaAssignmentExpression(left, right));
                        leftExpression = assignmentRight.Left;
                        rightExpression = assignmentRight.Right;
                    }
                }

                assignments.Reverse();
                LuaLineMultipleExpressionSyntax multipleAssignment = new LuaLineMultipleExpressionSyntax();
                multipleAssignment.Assignments.AddRange(assignments);
                return multipleAssignment;
            }
        }

        private LuaSyntaxNode BuildInvokeRefOrOut(InvocationExpressionSyntax node, LuaInvocationExpressionSyntax invocatione, List<LuaArgumentSyntax> refOrOutArguments) {
            if(node.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
                LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
                IMethodSymbol symbol = (IMethodSymbol)symbolInfo.Symbol;
                if(!symbol.ReturnsVoid) {
                    var temp = LuaIdentifierNameSyntax.Temp1;
                    CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(temp)));
                    multipleAssignment.Lefts.Add(temp);
                }
                multipleAssignment.Lefts.AddRange(refOrOutArguments.Select(i => i.Expression));
                multipleAssignment.Rights.Add(invocatione);
                return multipleAssignment;
            }
            else {
                var temp = LuaIdentifierNameSyntax.Temp1;
                LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                multipleAssignment.Lefts.Add(temp);
                multipleAssignment.Lefts.AddRange(refOrOutArguments.Select(i => i.Expression));
                multipleAssignment.Rights.Add(invocatione);

                CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(new LuaVariableDeclaratorSyntax(temp)));
                CurBlock.Statements.Add(new LuaExpressionStatementSyntax(multipleAssignment));
                return temp;
            }
        }

        public override LuaSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node) {
            List<LuaArgumentSyntax> arguments = new List<LuaArgumentSyntax>();
            List<LuaArgumentSyntax> refOrOutArguments = new List<LuaArgumentSyntax>();

            foreach(var argument in node.ArgumentList.Arguments) {
                var luaArgument = (LuaArgumentSyntax)argument.Accept(this);
                arguments.Add(luaArgument);
                if(argument.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) || argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword)) {
                    refOrOutArguments.Add(luaArgument);
                }
            }

            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(expression);
            invocation.ArgumentList.Arguments.AddRange(arguments);

            if(refOrOutArguments.Count > 0) {
                return BuildInvokeRefOrOut(node, invocation, refOrOutArguments);
            }
            return invocation;
        }

        public override LuaSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            if(symbol.Kind != SymbolKind.Property) {
                var identifier = (LuaIdentifierNameSyntax)node.Name.Accept(this);
                return new LuaMemberAccessExpressionSyntax(expression, identifier, !symbol.IsStatic && symbol.Kind == SymbolKind.Method);
            }
            else {
                var propertyIdentifier = (LuaExpressionSyntax)node.Name.Accept(this);
                var propertyAdapter = propertyIdentifier as LuaPropertyAdapterExpressionSyntax;
                if(propertyAdapter != null) {
                    var memberAccessExpression = new LuaMemberAccessExpressionSyntax(expression, propertyAdapter.InvocationExpression.Expression, !symbol.IsStatic);
                    propertyAdapter.Update(memberAccessExpression);
                    return propertyAdapter;
                }
                else {
                    return new LuaMemberAccessExpressionSyntax(expression, propertyIdentifier);
                }
            }
        }

        private string BuildStaticFieldName(ISymbol symbol, bool isReadOnly, IdentifierNameSyntax node) {
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
                }
                else {
                    var constructor = CurFunction as LuaConstructorAdapterExpressSyntax;
                    if(constructor != null && constructor.IsStaticCtor) {
                        name = LuaSyntaxNode.Tokens.This + '.' + symbol.Name;
                    }
                    else {
                        if(IsInternalNode(node)) {
                            name = symbol.ToString();
                        }
                        else {
                            name = symbol.Name;
                        }
                    }
                }
            }
            return name;
        }

        private bool IsInternalNode(IdentifierNameSyntax node) {
            bool isInternal = false;
            MemberAccessExpressionSyntax parent = null;
            if(node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                parent = (MemberAccessExpressionSyntax)node.Parent;
            }
            if(parent != null) {
                if(parent.Expression == node) {
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
                isField = propertySymbol.IsPropertyField();
                isReadOnly = propertySymbol.IsReadOnly;
            }
            else {
                var eventSymbol = (IEventSymbol)symbol;
                isField = eventSymbol.IsEventFiled();
                isReadOnly = false;
            }

            if(symbol.IsStatic) {
                if(isField) {
                    name = BuildStaticFieldName(symbol, isReadOnly, node);
                }
                else {
                    return new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
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
                    if(IsInternalNode(node)) {
                        if(symbol.IsOverridable() && !symbol.ContainingType.IsSealed) {
                            LuaPropertyOrEventIdentifierNameSyntax identifierName = new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name);
                            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, identifierName, true);
                            return new LuaPropertyAdapterExpressionSyntax(memberAccess, identifierName);
                        }
                        else {
                            var propertyAdapter = new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
                            propertyAdapter.InvocationExpression.AddArgument(LuaIdentifierNameSyntax.This);
                            return propertyAdapter;
                        }
                    }
                    else {
                        return new LuaPropertyAdapterExpressionSyntax(new LuaPropertyOrEventIdentifierNameSyntax(isProperty, symbol.Name));
                    }
                }
            }
            return new LuaIdentifierNameSyntax(name);
        }

        public override LuaSyntaxNode VisitIdentifierName(IdentifierNameSyntax node) {
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            string name;
            switch(symbol.Kind) {
                case SymbolKind.Local:
                case SymbolKind.Parameter: {
                        name = symbol.Name;
                        break;
                    }
                case SymbolKind.NamedType: {
                        name = symbol.ToString();
                        break;
                    }
                case SymbolKind.Field: {
                        if(symbol.IsStatic) {
                            var fieldSymbol = (IFieldSymbol)symbol;
                            name = BuildStaticFieldName(symbol, fieldSymbol.IsReadOnly, node);
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
                        if(symbol.IsStatic) {
                            name = symbol.Name;
                        }
                        else {
                            if(IsInternalNode(node)) {
                                if(symbol.IsOverridable() && !symbol.ContainingType.IsSealed) {
                                    LuaIdentifierNameSyntax identifierName = new LuaIdentifierNameSyntax(symbol.Name);
                                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, identifierName, true);
                                    return memberAccess;
                                }
                                else {
                                    name = symbol.Name;
                                }
                            }
                            else {
                                name = symbol.Name;
                            }
                        }
                        break;
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

        public override LuaSyntaxNode VisitArgumentList(ArgumentListSyntax node) {
            LuaArgumentListSyntax argumentList = new LuaArgumentListSyntax();
            foreach(var argument in node.Arguments) {
                var newNode = (LuaArgumentSyntax)argument.Accept(this);
                argumentList.Arguments.Add(newNode);
            }
            return argumentList;
        }

        public override LuaSyntaxNode VisitArgument(ArgumentSyntax node) {
            LuaExpressionSyntax expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaArgumentSyntax argument = new LuaArgumentSyntax(expression);
            return argument;
        }

        public override LuaSyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node) {
            if(node.IsKind(SyntaxKind.CharacterLiteralExpression)) {
                return new LuaCharacterLiteralExpression((char)node.Token.Value);
            }
            else {
                return new LuaLiteralExpressionSyntax(node.Token.Text);
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
            string typeName = GetPredefinedTypeName(node.Keyword.ValueText);
            return new LuaIdentifierNameSyntax(typeName);
        }

        private void WriteStatementOrBlock(StatementSyntax statement, LuaBlockSyntax luablock) {
            if(statement.IsKind(SyntaxKind.Block)) {
                var blockNode = (LuaBlockSyntax)statement.Accept(this);
                luablock.Statements.AddRange(blockNode.Statements);
            }
            else {
                var statementNode = (LuaStatementSyntax)statement.Accept(this);
                luablock.Statements.Add(statementNode);
            }
        }

        #region if else switch

        public override LuaSyntaxNode VisitIfStatement(IfStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
            WriteStatementOrBlock(node.Statement, ifStatement.Body);
            ifStatement.Else = (LuaElseClauseSyntax)node.Else?.Accept(this);
            return ifStatement;
        }

        public override LuaSyntaxNode VisitElseClause(ElseClauseSyntax node) {
            LuaStatementSyntax statement;
            if(node.Statement.IsKind(SyntaxKind.IfStatement)) {
                statement = (LuaStatementSyntax)node.Statement.Accept(this);
            }
            else {
                LuaBlockSyntax block = new LuaBlockSyntax();
                WriteStatementOrBlock(node.Statement, block);
                statement = block;
            }
            LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax(statement);
            return elseClause;
        }

        public override LuaSyntaxNode VisitSwitchStatement(SwitchStatementSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaSwitchAdapterStatementSyntax switchStatement = new LuaSwitchAdapterStatementSyntax(expression, node.Sections.Select(i => (LuaStatementSyntax)i.Accept(this)));
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
            var left = LuaIdentifierNameSyntax.Temp1;
            var right = (LuaExpressionSyntax)node.Value.Accept(this);
            LuaBinaryExpressionSyntax BinaryExpression = new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.EqualsEquals, right);
            return BinaryExpression;
        }

        #endregion

        public override LuaSyntaxNode VisitBreakStatement(BreakStatementSyntax node) {     
            var parent = node.Parent;
            do {
                SyntaxKind kind = parent.Kind();
                if(kind == SyntaxKind.SwitchSection) {
                    return LuaStatementSyntax.Empty;
                }
                else if(kind >= SyntaxKind.WhileStatement && kind <= SyntaxKind.ForEachStatement) {
                    return new LuaBreakStatementSyntax();
                }
                parent = parent.Parent;
            } while(parent != null);
            throw new InvalidOperationException();
        }

        public override LuaSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
            var left = (LuaExpressionSyntax)node.Left.Accept(this);
            var right = (LuaExpressionSyntax)node.Right.Accept(this);
            string operatorToken = GetOperatorToken(node.OperatorToken.ValueText);
            return new LuaBinaryExpressionSyntax(left, operatorToken, right);
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
                if(node.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
                    return assignment;
                }
                else {
                    CurBlock.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                    return assignment.Left;
                }
            }
            else {
                var operand = (LuaExpressionSyntax)node.Operand.Accept(this);
                string operatorToken = GetOperatorToken(node.OperatorToken.ValueText);
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
            if(node.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
                return assignment;
            }
            else {
                var temp = LuaIdentifierNameSyntax.Temp1;
                LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(temp);
                variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(assignment.Left);
                CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(variableDeclarator));
                CurBlock.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                return temp;
            }
        }

        public override LuaSyntaxNode VisitThrowStatement(ThrowStatementSyntax node) {
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Throw);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            invocationExpression.AddArgument(expression);
            return new LuaExpressionStatementSyntax(invocationExpression);
        }

        public override LuaSyntaxNode VisitForEachStatement(ForEachStatementSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaForInStatementSyntax forInStatement = new LuaForInStatementSyntax(identifier, expression);
            WriteStatementOrBlock(node.Statement, forInStatement.Body);
            return forInStatement;
        }

        public override LuaSyntaxNode VisitWhileStatement(WhileStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            LuaWhileStatementSyntax whileStatement = new LuaWhileStatementSyntax(condition);
            WriteStatementOrBlock(node.Statement, whileStatement.Body);
            return whileStatement;
        }

        public override LuaSyntaxNode VisitForStatement(ForStatementSyntax node) {
            LuaBlockSyntax body = new LuaBlockSyntax();
            blocks_.Push(body);

            if(node.Declaration != null) {
                body.Statements.Add((LuaVariableDeclarationSyntax)node.Declaration.Accept(this));
            }
            var initializers = node.Initializers.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
            body.Statements.AddRange(initializers);

            LuaExpressionSyntax condition = node.Condition != null ? (LuaExpressionSyntax)node.Condition.Accept(this) : LuaIdentifierNameSyntax.True;
            LuaWhileStatementSyntax whileStatement = new LuaWhileStatementSyntax(condition);
            blocks_.Push(whileStatement.Body);
            WriteStatementOrBlock(node.Statement, whileStatement.Body);
            var incrementors = node.Incrementors.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
            whileStatement.Body.Statements.AddRange(incrementors);
            blocks_.Pop();
            body.Statements.Add(whileStatement);
            blocks_.Pop();

            return new LuaBlockBlockSyntax(body);
        }

        public override LuaSyntaxNode VisitDoStatement(DoStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            var newCondition = new LuaPrefixUnaryExpressionSyntax(new LuaParenthesizedExpressionSyntax(condition), LuaSyntaxNode.Keyword.Not);
            LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(newCondition);
            WriteStatementOrBlock(node.Statement, repeatStatement.Body);
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
    }
}