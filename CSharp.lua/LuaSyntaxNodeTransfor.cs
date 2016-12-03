using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CSharpLua.LuaAst;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpLua {
    public sealed class LuaSyntaxNodeTransfor : CSharpSyntaxVisitor<LuaSyntaxNode> {
        private Stack<LuaCompilationUnitSyntax> compilationUnits_ = new Stack<LuaCompilationUnitSyntax>();

        private LuaCompilationUnitSyntax CurCompilationUnit {
            get {
                return compilationUnits_.Peek();
            }
        }

        public override LuaSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node) {
            LuaCompilationUnitSyntax newNode = new LuaCompilationUnitSyntax(node);
            compilationUnits_.Push(newNode);
            foreach(var member in node.Members) {
                member.Accept(this);
            }
            compilationUnits_.Pop();
            return newNode;
        }

        public override LuaSyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
            return null;
        }
    }
}