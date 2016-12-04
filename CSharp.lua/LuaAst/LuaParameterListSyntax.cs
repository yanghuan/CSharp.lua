using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaParameterListSyntax : LuaSyntaxNode {
        public string OpenParenToken => Tokens.OpenParentheses;
        public string CloseParenToken => Tokens.CloseParentheses;
        public List<LuaParameterSyntax> Arguments { get; } = new List<LuaParameterSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
