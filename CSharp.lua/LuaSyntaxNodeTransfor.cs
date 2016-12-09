using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using CSharpLua.LuaAst;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace CSharpLua {
    public sealed class LuaSyntaxNodeTransfor : CSharpSyntaxVisitor<LuaSyntaxNode> {
        private SemanticModel semanticModel_;
        private Stack<LuaCompilationUnitSyntax> compilationUnits_ = new Stack<LuaCompilationUnitSyntax>();
        private Stack<LuaNamespaceDeclarationSyntax> namespaces_ = new Stack<LuaNamespaceDeclarationSyntax>();
        private Stack<LuaTypeDeclarationSyntax> typeDeclarations_ = new Stack<LuaTypeDeclarationSyntax>();
        private Stack<LuaFunctionExpressSyntax> functions_ = new Stack<LuaFunctionExpressSyntax>();
        private Stack<LuaBlockSyntax> blocks_ = new Stack<LuaBlockSyntax>();

        private static readonly ImmutableDictionary<string, string> operatorTokenMapps_ = new Dictionary<string, string>() {
            ["!="] = "~=",
            ["!"] = LuaSyntaxNode.Tokens.Not,
            ["&&"] = LuaSyntaxNode.Tokens.And,
            ["||"] = LuaSyntaxNode.Tokens.Or,
        }.ToImmutableDictionary();

        public LuaSyntaxNodeTransfor(SemanticModel semanticModel) {
            semanticModel_ = semanticModel;
        }

        private static string GetOperatorToken(string operatorToken) {
            return operatorTokenMapps_.GetOrDefault(operatorToken, operatorToken);
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
            LuaIdentifierNameSyntax nameNode = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaClassDeclarationSyntax newNode = new LuaClassDeclarationSyntax(nameNode);
            typeDeclarations_.Push(newNode);
            foreach(var member in node.Members) {
                member.Accept(this);
            }
            typeDeclarations_.Pop();
            return newNode;
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

        public override LuaSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
            LuaIdentifierNameSyntax nameNode = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaFunctionExpressSyntax functionNode = new LuaFunctionExpressSyntax();
            functions_.Push(functionNode);
            var parameterList = (LuaParameterListSyntax)node.ParameterList.Accept(this);
            functionNode.ParameterList.Parameters.AddRange(parameterList.Parameters);
            LuaBlockSyntax blockNode = (LuaBlockSyntax)node.Body.Accept(this);
            functionNode.Body.Statements.AddRange(blockNode.Statements);
            CurType.AddMethod(nameNode, functionNode);
            functions_.Pop();
            return functionNode;
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

        public override LuaSyntaxNode VisitBlock(BlockSyntax node) {
            LuaBlockSyntax blockNode = new LuaBlockSyntax();
            blocks_.Push(blockNode);
            foreach(var statement in node.Statements) {
                LuaStatementSyntax statementNode = (LuaStatementSyntax)statement.Accept(this);
                blockNode.Statements.Add(statementNode);
            }
            blocks_.Pop();
            return blockNode;
        }

        public override LuaSyntaxNode VisitReturnStatement(ReturnStatementSyntax node) {
            LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax();
            if(node.Expression != null) {
                var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                returnStatement.Expressions.Add(expression);
            }
            return returnStatement;
        }

        public override LuaSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
            LuaExpressionSyntax expressionNode = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaExpressionStatementSyntax(expressionNode);
        }

        public override LuaSyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) {
            if(node.Right.Kind() != SyntaxKind.SimpleAssignmentExpression ) {
                var left = (LuaExpressionSyntax)node.Left.Accept(this);
                var right = (LuaExpressionSyntax)node.Right.Accept(this);
                return new LuaAssignmentExpressionSyntax(left, right);
            }
            else {
                List<LuaAssignmentExpressionSyntax> assignments = new List<LuaAssignmentExpressionSyntax>();
                var leftExpression = node.Left;
                var rightExpression = node.Right;

                while(true) {
                    var left = (LuaExpressionSyntax)leftExpression.Accept(this);
                    var assignmentRight = rightExpression as AssignmentExpressionSyntax;
                    if(assignmentRight == null) {
                        var right = (LuaExpressionSyntax)rightExpression.Accept(this);
                        assignments.Add(new LuaAssignmentExpressionSyntax(left, right));
                        break;
                    }
                    else {
                        var right = (LuaExpressionSyntax)assignmentRight.Left.Accept(this);
                        assignments.Add(new LuaAssignmentExpressionSyntax(left, right));
                        leftExpression = assignmentRight.Left;
                        rightExpression = assignmentRight.Right;
                    }
                }

                assignments.Reverse();
                LuaLineMultipleAssignmentExpressionSyntax multipleAssignment = new LuaLineMultipleAssignmentExpressionSyntax();
                multipleAssignment.Assignments.AddRange(assignments);
                return multipleAssignment;
            }
        }

        public override LuaSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node) {
            List<LuaArgumentSyntax> arguments = new List<LuaArgumentSyntax>();
            List<LuaArgumentSyntax> refOrOutArguments = new List<LuaArgumentSyntax>();

            foreach(var argument in node.ArgumentList.Arguments) {
                var luaArgument = (LuaArgumentSyntax)argument.Accept(this);
                arguments.Add(luaArgument);
                string refOrOutKeyword = argument.RefOrOutKeyword.ValueText;
                if(refOrOutKeyword == LuaSyntaxNode.Keyword.Ref || refOrOutKeyword == LuaSyntaxNode.Keyword.Out) {
                    refOrOutArguments.Add(luaArgument);
                }
            }

            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            LuaInvocationExpressionSyntax invocationNode = new LuaInvocationExpressionSyntax(expression);
            invocationNode.ArgumentList.Arguments.AddRange(arguments);

            if(refOrOutArguments.Count > 0) {
                SyntaxKind kind = node.Parent.Kind();
                if(kind == SyntaxKind.ExpressionStatement) {
                    LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                    SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
                    IMethodSymbol symbol = (IMethodSymbol)symbolInfo.Symbol;
                    if(!symbol.ReturnsVoid) {
                        var temp = CurFunction.GetTemp1();
                        multipleAssignment.Lefts.Add(temp);
                    }
                    multipleAssignment.Lefts.AddRange(refOrOutArguments.Select(i => i.Expression));
                    multipleAssignment.Rights.Add(invocationNode);
                    return multipleAssignment;
                }
                else {
                    var temp = CurFunction.GetTemp1();
                    LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                    multipleAssignment.Lefts.Add(temp);
                    multipleAssignment.Lefts.AddRange(refOrOutArguments.Select(i => i.Expression));
                    multipleAssignment.Rights.Add(invocationNode);

                    CurBlock.Statements.Add(new LuaExpressionStatementSyntax(multipleAssignment));
                    return temp;
                }
            }
            return invocationNode;
        }

        public override LuaSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node.Name);
            ISymbol symbol = symbolInfo.Symbol;
            LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(symbol.Name);
            return new LuaMemberAccessExpressionSyntax(expression, name, !symbol.IsStatic);
        }

        public override LuaSyntaxNode VisitIdentifierName(IdentifierNameSyntax node) {
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            string text;
            if(symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Parameter || symbol.Kind == SymbolKind.Method) {
                text = symbol.Name;
            }
            else {
                text = symbol.ContainingNamespace.Name + '.' + symbol.Name;
            }
            return new LuaIdentifierNameSyntax(text);
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
            return new LuaLiteralExpressionSyntax(node.Token.Text);
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
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            return new LuaIdentifierNameSyntax(symbol.ContainingNamespace.Name + '.' + symbol.Name);
        }


        private void WriteStatementOrBlock(StatementSyntax statement, LuaBlockSyntax luablock) {
            if(statement.Kind() == SyntaxKind.Block) {
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
            if(node.Else != null) {
                var elseCause = (LuaElseClauseSyntax)node.Else.Accept(this);
                ifStatement.Else = elseCause;
            }
            return ifStatement;
        }

        public override LuaSyntaxNode VisitElseClause(ElseClauseSyntax node) {
            var statement = (LuaStatementSyntax)node.Statement.Accept(this);
            if(node.Statement.Kind() != SyntaxKind.IfStatement) {
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
            return new LuaBreakStatementSyntax();
        }

        public override LuaSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
            var left = (LuaExpressionSyntax)node.Left.Accept(this);
            var right = (LuaExpressionSyntax)node.Right.Accept(this);
            string operatorToken = GetOperatorToken(node.OperatorToken.ValueText);
            return new LuaBinaryExpressionSyntax(left, operatorToken, right);
        }

        public override LuaSyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node) {
            var operand = (LuaExpressionSyntax)node.Operand.Accept(this);
            string operatorToken = GetOperatorToken(node.OperatorToken.ValueText);
            LuaPrefixUnaryExpressionSyntax unaryExpression = new LuaPrefixUnaryExpressionSyntax(operand, operatorToken);
            return unaryExpression;
        }

        public override LuaSyntaxNode VisitThrowStatement(ThrowStatementSyntax node) {
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Throw);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            invocationExpression.ArgumentList.Arguments.Add(new LuaArgumentSyntax(expression));
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
            var declaration = (LuaVariableDeclarationSyntax)node.Declaration?.Accept(this);
            var initializers = node.Initializers.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
            var condition = (LuaExpressionSyntax)node.Condition?.Accept(this);
            var incrementors = node.Incrementors.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
            LuaBlockSyntax statementBlock = new LuaBlockSyntax();
            WriteStatementOrBlock(node.Statement, statementBlock);
            LuaForAdapterStatementSyntax forStatement = new LuaForAdapterStatementSyntax(declaration, initializers, condition, statementBlock, incrementors);
            return forStatement;
        }

        public override LuaSyntaxNode VisitDoStatement(DoStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(new LuaPrefixUnaryExpressionSyntax(condition, LuaSyntaxNode.Keyword.Not));
            WriteStatementOrBlock(node.Statement, repeatStatement.Body);
            return repeatStatement;
        }
    }
}