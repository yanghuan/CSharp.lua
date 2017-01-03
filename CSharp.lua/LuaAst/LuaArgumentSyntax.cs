using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaArgumentSyntax : LuaSyntaxNode {
        public LuaExpressionSyntax Expression { get; }

        public LuaArgumentSyntax(LuaExpressionSyntax expression) {
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            Expression = expression;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaArgumentListSyntax : LuaSyntaxNode {
        public string OpenParenToken => Tokens.OpenParentheses;
        public string CloseParenToken => Tokens.CloseParentheses;
        public readonly LuaSyntaxList<LuaArgumentSyntax> Arguments = new LuaSyntaxList<LuaArgumentSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
