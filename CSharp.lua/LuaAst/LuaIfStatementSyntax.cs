using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public sealed class LuaIfStatementSyntax : LuaStatementSyntax {
        public string CloseParenToken => Tokens.End;
        public LuaExpressionSyntax Condition { get; }
        public LuaElseClauseSyntax Else { get; set; }
        public string IfKeyword => Tokens.If;
        public string OpenParenToken => Tokens.Then;

        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax();

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
        public string ElseKeyword => Tokens.Else;
        public LuaStatementSyntax Statement { get; }

        public LuaElseClauseSyntax(LuaStatementSyntax statement) {
            Statement = statement;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}