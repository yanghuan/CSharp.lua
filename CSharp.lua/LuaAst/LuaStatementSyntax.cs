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

        public LuaExpressionStatementSyntax(LuaExpressionSyntax expression) {
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
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
        public LuaSyntaxList<LuaExpressionSyntax> Expressions { get; } = new LuaSyntaxList<LuaExpressionSyntax>();
        public string ReturnKeyword => Tokens.Return;

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
