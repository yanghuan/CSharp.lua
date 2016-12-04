using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaExpressionStatementSyntax : LuaStatementSyntax {
        public LuaExpressionSyntax Expression { get; }
        public string SemicolonToken => Tokens.Semicolon;

        public LuaExpressionStatementSyntax(LuaExpressionSyntax expression) {
            Expression = expression;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
