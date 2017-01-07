using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaForInStatementSyntax : LuaStatementSyntax {
        public LuaExpressionSyntax Expression { get; }
        public string ForKeyword => Tokens.For;
        public LuaIdentifierNameSyntax Identifier { get; }
        public string InKeyword => Tokens.In;
        public LuaExpressionSyntax Placeholder => LuaIdentifierNameSyntax.Placeholder;

        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
            OpenBraceToken = Tokens.Do,
            CloseBraceToken = Tokens.End,
        };

        public LuaForInStatementSyntax(LuaIdentifierNameSyntax identifier, LuaExpressionSyntax expression) {
            if(identifier == null) {
                throw new ArgumentNullException(nameof(identifier));
            }
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            Identifier = identifier;
            Expression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Each, expression);
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaWhileStatementSyntax : LuaStatementSyntax {
        public LuaExpressionSyntax Condition { get; }
        public string WhileKeyword => LuaSyntaxNode.Tokens.While;

        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
            OpenBraceToken = Tokens.Do,
            CloseBraceToken = Tokens.End,
        };

        public LuaWhileStatementSyntax(LuaExpressionSyntax condition) {
            if(condition == null) {
                throw new ArgumentNullException(nameof(condition));
            }
            Condition = condition;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaRepeatStatementSyntax : LuaStatementSyntax {
        public LuaExpressionSyntax Condition { get; }
        public string RepeatKeyword => Tokens.Repeat;
        public string UntilKeyword => Tokens.Until;
        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax();

        public LuaRepeatStatementSyntax(LuaExpressionSyntax condition) {
            if(condition == null) {
                throw new ArgumentNullException(nameof(condition));
            }
            Condition = condition;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
