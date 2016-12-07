using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public sealed class LuaIfStatementSyntax : LuaStatementSyntax {
        public string CloseParenToken => Keyword.End;
        public LuaExpressionSyntax Condition { get; }
        public LuaElseClauseSyntax Else { get; set; }
        public string IfKeyword => Keyword.If;
        public string OpenParenToken => Keyword.Then;

        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
            OpenBraceToken = Tokens.Empty,
            CloseBraceToken = Tokens.Empty,
        };

        public LuaIfStatementSyntax(LuaExpressionSyntax condition) {
            if(condition == null) {
                throw new ArgumentNullException(nameof(condition));
            }
            Condition = condition;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaElseClauseSyntax : LuaSyntaxNode {
        public string ElseKeyword => Keyword.Else;

        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
            OpenBraceToken = Tokens.Empty,
            CloseBraceToken = Tokens.Empty,
        };

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}