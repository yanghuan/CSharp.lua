using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpLua.LuaAst {
    public sealed class LuaCompilationUnitSyntax : LuaSyntaxNode {
        public string FilePath { get; set; }
        public LuaSyntaxList<LuaStatementSyntax> Statements { get; } = new LuaSyntaxList<LuaStatementSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }

        public void AddTypeDeclaration(LuaTypeDeclarationSyntax memberNode) {
            LuaNamespaceDeclarationSyntax namespaceNode = new LuaNamespaceDeclarationSyntax(LuaIdentifierNameSyntax.Empty);
            namespaceNode.Add(memberNode);
            Statements.Add(namespaceNode);
        }
    }
}