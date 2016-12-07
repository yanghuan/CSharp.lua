using System;
using System.Collections.Generic;
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
        private Stack<LuaBlockSyntax> blocks_ = new Stack<LuaBlockSyntax>();

        public LuaSyntaxNodeTransfor(SemanticModel semanticModel) {
            semanticModel_ = semanticModel;
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
            var parameterList = (LuaParameterListSyntax)node.ParameterList.Accept(this);
            functionNode.ParameterList.Parameters.AddRange(parameterList.Parameters);
            LuaBlockSyntax blockNode = (LuaBlockSyntax)node.Body.Accept(this);
            functionNode.Body.Statements.AddRange(blockNode.Statements);
            CurType.AddMethod(nameNode, functionNode);
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
                        multipleAssignment.Lefts.Add(LuaIdentifierNameSyntax.Placeholder);
                    }
                    multipleAssignment.Lefts.AddRange(refOrOutArguments.Select(i => i.Expression));
                    multipleAssignment.Rights.Add(invocationNode);
                    return multipleAssignment;
                }
                else {
                    LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
                    multipleAssignment.Lefts.Add(LuaIdentifierNameSyntax.Placeholder);
                    multipleAssignment.Lefts.AddRange(refOrOutArguments.Select(i => i.Expression));
                    multipleAssignment.Rights.Add(invocationNode);

                    CurBlock.Statements.Add(new LuaExpressionStatementSyntax(multipleAssignment));
                    return LuaIdentifierNameSyntax.Placeholder;
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
            if(symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Parameter) {
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
            LuaLocalDeclarationStatementSyntax variablesStatement = new LuaLocalDeclarationStatementSyntax();
            foreach(VariableDeclaratorSyntax variable in node.Declaration.Variables) {
               var variableDeclarator = (LuaVariableDeclaratorSyntax)variable.Accept(this);
                variablesStatement.Variables.Add(variableDeclarator);
            }

            bool isMultiNil = variablesStatement.Variables.Count > 0 && variablesStatement.Variables.All(i => i.Initializer == null);
            if(isMultiNil) {
                LuaLocalVariablesStatementSyntax declarationStatement = new LuaLocalVariablesStatementSyntax();
                foreach(var variable in variablesStatement.Variables) {
                    declarationStatement.Variables.Add(variable.Identifier);
                }
                return declarationStatement;
            }
            return variablesStatement;
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


        #region if else

        public override LuaSyntaxNode VisitIfStatement(IfStatementSyntax node) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
            if(node.Statement.Kind() == SyntaxKind.Block) {
                var block = (LuaBlockSyntax)node.Statement.Accept(this);
                ifStatement.Body.Statements.AddRange(block.Statements);
            }
            else {
                var statement = (LuaStatementSyntax)node.Statement.Accept(this);
                ifStatement.Body.Statements.Add(statement);
            }
            if(node.Else != null) {
                var elseCause = (LuaElseClauseSyntax)node.Else.Accept(this);
                ifStatement.Else = elseCause;
            }
            return ifStatement;
        }

        #endregion

        public override LuaSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
            var left = (LuaExpressionSyntax)node.Left.Accept(this);
            var right = (LuaExpressionSyntax)node.Right.Accept(this);
            return new LuaBinaryExpressionSyntax(left, node.OperatorToken.ValueText, right);
        }

        public override LuaSyntaxNode VisitElseClause(ElseClauseSyntax node) {
            LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
            if(node.Statement.Kind() == SyntaxKind.Block) {
                var block = (LuaBlockSyntax)node.Statement.Accept(this);
                elseClause.Body.Statements.AddRange(block.Statements);
            }
            else {
                var statement = (LuaStatementSyntax)node.Statement.Accept(this);
                elseClause.Body.Statements.Add(statement);
            }
            return elseClause;
        }
    }
}