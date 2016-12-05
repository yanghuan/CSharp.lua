using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public sealed class LuaLiteralExpressionSyntax : LuaExpressionSyntax {
        public string OpenParenToken => Tokens.Quote;
        public string CloseParenToken => Tokens.Quote;
        public LuaIdentifierNameSyntax Identifier { get; }

        public LuaLiteralExpressionSyntax(string text) {
            Identifier = new LuaIdentifierNameSyntax(text);
        }

        public LuaLiteralExpressionSyntax(LuaIdentifierNameSyntax identifier) {
            Identifier = identifier;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}