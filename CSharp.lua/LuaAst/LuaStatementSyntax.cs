using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public abstract class LuaStatementSyntax : LuaSyntaxNode {
    }

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

    public sealed class LuaStatementListSyntax : LuaStatementSyntax {
        public List<LuaStatementSyntax> Statements { get; } = new List<LuaStatementSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaReturnStatementSyntax : LuaStatementSyntax {
        public LuaExpressionSyntax Expression { get; }
        public string ReturnKeyword => Tokens.Return;
        public string SemicolonToken => Tokens.Semicolon;

        public LuaReturnStatementSyntax(LuaExpressionSyntax expression = null) {
            Expression = expression;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
