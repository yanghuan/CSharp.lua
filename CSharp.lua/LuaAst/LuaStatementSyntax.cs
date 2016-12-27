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

        private LuaBreakStatementSyntax() {}

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }

        public static readonly LuaBreakStatementSyntax Statement = new LuaBreakStatementSyntax();
    }

    public sealed class LuaContinueAdapterStatementSyntax : LuaStatementSyntax {
        public LuaExpressionStatementSyntax Assignment { get; }
        public LuaBreakStatementSyntax Break => LuaBreakStatementSyntax.Statement;

        private LuaContinueAdapterStatementSyntax() {
            Assignment = new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(LuaIdentifierNameSyntax.Continue, LuaIdentifierNameSyntax.True));
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }

        public static readonly LuaContinueAdapterStatementSyntax Statement = new LuaContinueAdapterStatementSyntax();
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

    public sealed class LuaGotoStatement : LuaStatementSyntax {
        public LuaIdentifierNameSyntax Identifier { get; }
        public string GotoKeyword => Tokens.Goto;

        public LuaGotoStatement(LuaIdentifierNameSyntax identifier) {
            if(identifier == null) {
                throw new ArgumentNullException(nameof(identifier));
            }
            Identifier = identifier;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaGotoCaseAdapterStatement : LuaStatementSyntax {
        public LuaStatementSyntax Assignment { get; }
        public LuaGotoStatement GotoStatement { get; }

        public LuaGotoCaseAdapterStatement(LuaIdentifierNameSyntax identifier) {
            if(identifier == null) {
                throw new ArgumentNullException(nameof(identifier));
            }

            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(identifier, LuaIdentifierNameSyntax.True);
            Assignment = new LuaExpressionStatementSyntax(assignment);
            GotoStatement = new LuaGotoStatement(identifier);
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaLabeledStatement : LuaStatementSyntax {
        public string PrefixToken => Tokens.Label;
        public string SuffixToken => Tokens.Label;
        public LuaIdentifierNameSyntax Identifier { get; }
        public LuaStatementSyntax Statement { get; }

        public LuaLabeledStatement(LuaIdentifierNameSyntax identifier, LuaStatementSyntax statement = null) {
            if(identifier == null) {
                throw new ArgumentNullException(nameof(identifier));
            }
            Identifier = identifier;
            Statement = statement;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
