using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public abstract class LuaLiteralExpressionSyntax : LuaExpressionSyntax {
        public abstract string Text { get; }
    }

    public sealed class LuaIdentifierLiteralExpressionSyntax : LuaLiteralExpressionSyntax {
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

    public sealed class LuaVerbatimStringLiteralExpressionSyntax : LuaLiteralExpressionSyntax {
        public override string Text { get; }
        public int EqualsCount { get; }
        public string OpenBracket => Tokens.OpenBracket;
        public string CloseBracket => Tokens.CloseBracket;

        public LuaVerbatimStringLiteralExpressionSyntax(string text, int equalsCount) {
            Text = text;
            EqualsCount = equalsCount;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public class LuaConstLiteralExpression : LuaLiteralExpressionSyntax {
        public LuaLiteralExpressionSyntax Value { get; }
        public string OpenComment => Tokens.OpenLongComment;
        public string IdentifierToken { get; }
        public string CloseComment => Tokens.CloseDoubleBrace;

        public LuaConstLiteralExpression(string value, string identifierToken) : this(new LuaIdentifierLiteralExpressionSyntax(value), identifierToken) {
        }

        public LuaConstLiteralExpression(LuaLiteralExpressionSyntax value, string identifierToken) {
            Value = value;
            IdentifierToken = identifierToken;
        }

        public override string Text {
            get {
                return Value.Text;
            }
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaCharacterLiteralExpression : LuaConstLiteralExpression {
        public LuaCharacterLiteralExpression(char character) : base(((int)character).ToString(), GetIdentifierToken(character)) {
        }

        private static string GetIdentifierToken(char character) {
            return SyntaxFactory.Literal(character).Text;
        }
    }
}