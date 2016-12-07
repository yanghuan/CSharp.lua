using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public sealed class LuaLocalVariablesStatementSyntax : LuaStatementSyntax {
        public string LocalKeyword => Tokens.Local;
        public LuaSyntaxList<LuaIdentifierNameSyntax> Variables { get; } = new LuaSyntaxList<LuaIdentifierNameSyntax>();
        public LuaEqualsValueClauseListSyntax Initializer { get; set; }
        public string SemicolonToken => Tokens.Semicolon;

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaEqualsValueClauseListSyntax : LuaSyntaxNode {
        public string EqualsToken => Tokens.Equals;
        public LuaSyntaxList<LuaExpressionSyntax> Values { get; } = new LuaSyntaxList<LuaExpressionSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }


    public sealed class LuaLocalDeclarationStatementSyntax : LuaStatementSyntax {
        public LuaSyntaxList<LuaVariableDeclaratorSyntax> Variables { get; } = new LuaSyntaxList<LuaVariableDeclaratorSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaVariableDeclaratorSyntax : LuaSyntaxNode {
        public string LocalKeyword => Tokens.Local;
        public LuaIdentifierNameSyntax Identifier { get; }
        public LuaEqualsValueClauseSyntax Initializer { get; set; }
        public string SemicolonToken => Tokens.Semicolon;

        public LuaVariableDeclaratorSyntax(LuaIdentifierNameSyntax identifier) {
            if(identifier == null) {
                throw new ArgumentNullException(nameof(identifier));
            }
            Identifier = identifier;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaEqualsValueClauseSyntax : LuaSyntaxNode {
        public string EqualsToken => Tokens.Equals;
        public LuaExpressionSyntax Value { get; }

        public LuaEqualsValueClauseSyntax(LuaExpressionSyntax value) {
            if(value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            Value = value;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}