using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public sealed class LuaLocalDeclarationStatementSyntax : LuaStatementSyntax {
        public LuaVariableDeclarationSyntax Declaration { get; } = new LuaVariableDeclarationSyntax();
        public string SemicolonToken => Tokens.Semicolon;

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaVariableDeclarationSyntax : LuaSyntaxNode {
        public LuaSyntaxList<LuaIdentifierNameSyntax> Variables { get; } = new LuaSyntaxList<LuaIdentifierNameSyntax>();
        public EqualsValueClauseListSyntax Initializer { get; }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class EqualsValueClauseListSyntax : LuaSyntaxNode {
        public string EqualsToken => Tokens.Equals;
        public LuaSyntaxList<LuaExpressionSyntax> Values { get; } = new LuaSyntaxList<LuaExpressionSyntax>();
    }
}