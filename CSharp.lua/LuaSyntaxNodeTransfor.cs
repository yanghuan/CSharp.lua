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
            foreach(var statement in node.Statements) {
                LuaStatementSyntax statementNode = (LuaStatementSyntax)statement.Accept(this);
                blockNode.Statements.Add(statementNode);
            }
            return blockNode;
        }

        public override LuaSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
            LuaExpressionSyntax expressionNode = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaExpressionStatementSyntax(expressionNode);
        }

        public override LuaSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            var argumentList = (LuaArgumentListSyntax)node.ArgumentList.Accept(this);
            LuaInvocationExpressionSyntax invocationNode = new LuaInvocationExpressionSyntax(expression);
            invocationNode.ArgumentList.Arguments.AddRange(argumentList.Arguments);
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
            return new LuaIdentifierNameSyntax(symbol.ContainingNamespace.Name + '.' + symbol.Name);
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
            LuaLocalVariablesStatementSyntax variablesStatement = new LuaLocalVariablesStatementSyntax();
            foreach(VariableDeclaratorSyntax variable in node.Declaration.Variables) {
               var variableDeclarator = (LuaVariableDeclaratorSyntax)variable.Accept(this);
                variablesStatement.Variables.Add(variableDeclarator);
            }

            bool isMultiNil = variablesStatement.Variables.Count > 0 && variablesStatement.Variables.All(i => i.Value == null);
            if(isMultiNil) {
                LuaLocalDeclarationStatementSyntax declarationStatement = new LuaLocalDeclarationStatementSyntax();
                foreach(var variable in variablesStatement.Variables) {
                    declarationStatement.Variables.Add(variable.Identifier);
                }
                return declarationStatement;
            }
            return variablesStatement;
        }

        public override LuaSyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            LuaExpressionSyntax expression = null;
            if(node.Initializer != null) {
                expression = (LuaExpressionSyntax)node.Initializer.Value.Accept(this);
            }
            return new LuaVariableDeclaratorSyntax(identifier, expression);
        }
    }
}