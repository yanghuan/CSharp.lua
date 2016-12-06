using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaTableInitializerExpression : LuaExpressionSyntax {
        public string OpenBraceToken => Tokens.OpenBrace;
        public LuaSyntaxList<LuaTableItemSyntax> Items { get; } = new LuaSyntaxList<LuaTableItemSyntax>();
        public string CloseBraceToken => Tokens.CloseBrace;

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public abstract class LuaTableItemSyntax : LuaSyntaxNode {
    }

    public sealed class LuaSingleTableItemSyntax : LuaTableItemSyntax {
        public LuaExpressionSyntax Expression { get; }

        public LuaSingleTableItemSyntax(LuaExpressionSyntax expression) {
            Expression = expression;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public abstract class LuaTableKeySyntax : LuaSyntaxNode {
    }

    public sealed class LuaTableExpressionKeySyntax : LuaTableKeySyntax {
        public LuaExpressionSyntax Expression { get; }
        public string OpenBracketToken => Tokens.OpenBracket;
        public string CloseBracketToken => Tokens.CloseBracket;

        public LuaTableExpressionKeySyntax(LuaExpressionSyntax expression) {
            Expression = expression;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaTableLiteralKeySyntax : LuaTableKeySyntax {
        public LuaIdentifierNameSyntax Identifier { get; }

        public LuaTableLiteralKeySyntax(LuaIdentifierNameSyntax identifier) {
            Identifier = identifier;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaKeyValueTableItemSyntax : LuaTableItemSyntax {
        public LuaTableKeySyntax Key { get; }
        public string OperatorToken => Tokens.Equals;
        public LuaExpressionSyntax Value { get; }

        public LuaKeyValueTableItemSyntax(LuaTableKeySyntax key, LuaExpressionSyntax value) {
            Key = key;
            Value = value;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
