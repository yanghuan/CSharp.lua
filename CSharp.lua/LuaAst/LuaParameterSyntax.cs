using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaParameterListSyntax : LuaSyntaxNode {
        public string OpenParenToken => Tokens.OpenParentheses;
        public string CloseParenToken => Tokens.CloseParentheses;
        public LuaSyntaxList<LuaParameterSyntax> Parameters { get; } = new LuaSyntaxList<LuaParameterSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaParameterSyntax : LuaSyntaxNode {
        public LuaIdentifierNameSyntax Identifier { get; }

        public LuaParameterSyntax(LuaIdentifierNameSyntax identifier) {
            Identifier = identifier;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
