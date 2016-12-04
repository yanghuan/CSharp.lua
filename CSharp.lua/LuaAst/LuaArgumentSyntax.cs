using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaArgumentSyntax : LuaSyntaxNode {
        public LuaExpressionSyntax Expression { get; }

        public LuaArgumentSyntax(LuaExpressionSyntax expression) {
            Expression = expression;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
