using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public abstract class LuaLiteralExpressionSyntax : LuaExpressionSyntax {
        public abstract string Text { get; }
    }

    public class LuaIdentifierLiteralExpressionSyntax : LuaLiteralExpressionSyntax {
        public LuaIdentifierNameSyntax Identifier { get; }

        public LuaIdentifierLiteralExpressionSyntax(string text) : this(new LuaIdentifierNameSyntax(text)) {
        }

        public LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax identifier) {
            Identifier = identifier;
        }

        public override string Text {
            get {
                return Identifier.ValueText;
            }
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaStringLiteralExpressionSyntax : LuaLiteralExpressionSyntax {
        public string OpenParenToken => Tokens.Quote;
        public LuaIdentifierNameSyntax Identifier { get; }
        public string CloseParenToken => Tokens.Quote;

        public LuaStringLiteralExpressionSyntax(LuaIdentifierNameSyntax identifier) {
            Identifier = identifier;
        }

        public override string Text {
            get {
                return Identifier.ValueText;
            }
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }

        public static readonly LuaStringLiteralExpressionSyntax Empty = new LuaStringLiteralExpressionSyntax(LuaIdentifierNameSyntax.Empty); 
    }

    public sealed class LuaCharacterStringLiteralExpressionSyntax : LuaLiteralExpressionSyntax {
        public string OpenParenToken => Tokens.SingleQuote;
        public char Character { get; }
        public string CloseParenToken => Tokens.SingleQuote;

        public LuaCharacterStringLiteralExpressionSyntax(char character) {
            Character = character;
        }

        public override string Text {
            get {
                return Character.ToString();
            }
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaCharacterLiteralExpression : LuaLiteralExpressionSyntax {
        public char Character { get; }
        public string OpenCommentToken => Tokens.OpenLongComment;
        public string CloseCommentToken => Tokens.CloseLongComment;

        public LuaCharacterLiteralExpression(char character) {
            Character = character;
        }

        public string Comment {
            get {
                string character = Character != '\0' ? Character.ToString() : "\\0";
                return $"{OpenCommentToken} '{character}' {CloseCommentToken}";
            }
        }

        public override string Text {
            get {
                return Character.ToString();
            }
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}