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
            LuaFunctionExpressSyntax newNode = new LuaFunctionExpressSyntax();
            CurType.AddMethod(nameNode, newNode);
            return newNode;

            //VariableDeclaratorSyntax
            //LocalDeclarationStatementSyntax 
            //LiteralExpressionSyntax
            //var e = node.Body.Statements[0];
            //MemberAccessExpressionSyntax 
            //InvocationExpressionSyntax
        }
    }
}