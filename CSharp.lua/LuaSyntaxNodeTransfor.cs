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
        private Stack<LuaClassDeclarationSyntax> classDeclarations_ = new Stack<LuaClassDeclarationSyntax>();

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

        private LuaClassDeclarationSyntax CurClass {
            get {
                return classDeclarations_.Peek();
            }
        }

        public override LuaSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node) {
            LuaCompilationUnitSyntax newNode = new LuaCompilationUnitSyntax() { FilePath = node.SyntaxTree.FilePath };
            compilationUnits_.Push(newNode);
            foreach(var member in node.Members) {
                var memberNode = (LuaTypeDeclarationSyntax)member.Accept(this);
                newNode.Statements.Add(memberNode);
            }
            compilationUnits_.Pop();
            return newNode;
        }

        public override LuaSyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
            LuaIdentifierNameSyntax nameNode = new LuaIdentifierNameSyntax((IdentifierNameSyntax)node.Name);
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
            LuaIdentifierNameSyntax nameNode = new LuaIdentifierNameSyntax(node.Identifier);
            LuaClassDeclarationSyntax newNode = new LuaClassDeclarationSyntax(nameNode);
            classDeclarations_.Push(newNode);
            foreach(var member in node.Members) {
                member.Accept(this);
            }
            classDeclarations_.Pop();
            return newNode;
        }

        public override LuaSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
            var e = node.Body.Statements[0];
            //MemberAccessExpressionSyntax 
            InvocationExpressionSyntax

            return base.VisitMethodDeclaration(node);
        }
    }
}