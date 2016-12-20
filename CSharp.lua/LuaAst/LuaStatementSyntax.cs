using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public abstract class LuaStatementSyntax : LuaSyntaxNode {
        public string SemicolonToken => Tokens.Semicolon;

        private sealed class EmptyLuaStatementSyntax : LuaStatementSyntax {
            internal override void Render(LuaRenderer renderer) {
            }
        }
        public readonly static LuaStatementSyntax Empty = new EmptyLuaStatementSyntax();
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
        public LuaExpressionSyntax Expression { get; }
        public string ReturnKeyword => Tokens.Return;

        public LuaReturnStatementSyntax(LuaExpressionSyntax expression) {
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            Expression = expression;
        }

        public LuaReturnStatementSyntax() {
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaMultipleReturnStatementSyntax : LuaStatementSyntax {
        public LuaSyntaxList<LuaExpressionSyntax> Expressions { get; } = new LuaSyntaxList<LuaExpressionSyntax>();
        public string ReturnKeyword => Tokens.Return;

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaBreakStatementSyntax : LuaStatementSyntax {
        public string BreakKeyword => Tokens.Break;

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaBlankLinesStatement : LuaStatementSyntax {
        public int Count { get; }

        public LuaBlankLinesStatement(int count) {
            Count = count;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaShortCommentStatement : LuaStatementSyntax {
        public string SingleCommentToken => Tokens.ShortComment;
        public string Comment { get; }

        public LuaShortCommentStatement(string comment) {
            Comment = comment;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaLongCommentStatement : LuaStatementSyntax {
        public string OpenCommentToken => Tokens.OpenLongComment;
        public string Comment { get; }
        public string CloseCommentToken => Tokens.CloseLongComment;

        public LuaLongCommentStatement(string comment) {
            Comment = comment;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
