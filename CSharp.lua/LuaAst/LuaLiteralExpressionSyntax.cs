using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public class LuaLiteralExpressionSyntax : LuaExpressionSyntax {
        public LuaIdentifierNameSyntax Identifier { get; }

        public LuaLiteralExpressionSyntax(string text) : this(new LuaIdentifierNameSyntax(text)) {
        }

        public LuaLiteralExpressionSyntax(LuaIdentifierNameSyntax identifier) {
            Identifier = identifier;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaStringLiteralExpressionSyntax : LuaLiteralExpressionSyntax {
        public string OpenParenToken => Tokens.Quote;
        public string CloseParenToken => Tokens.Quote;

        public LuaStringLiteralExpressionSyntax(LuaIdentifierNameSyntax identifier) : base(identifier) {
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaCharacterLiteralExpression : LuaExpressionSyntax {
        public char Character { get; }
        public string OpenCommentToken => Tokens.OpenLongComment;
        public string CloseCommentToken => Tokens.CloseLongComment;

        public LuaCharacterLiteralExpression(char character) {
            Character = character;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}