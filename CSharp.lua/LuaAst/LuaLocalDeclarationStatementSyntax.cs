using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public sealed class LuaLocalVariablesStatementSyntax : LuaVariableDeclarationSyntax {
        public string LocalKeyword => Tokens.Local;
        public LuaSyntaxList<LuaIdentifierNameSyntax> Variables { get; } = new LuaSyntaxList<LuaIdentifierNameSyntax>();
        public LuaEqualsValueClauseListSyntax Initializer { get; set; }

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
        public LuaVariableDeclarationSyntax Declaration { get; }

        public LuaLocalDeclarationStatementSyntax(LuaVariableDeclarationSyntax declaration) {
            Declaration = declaration;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public abstract class LuaVariableDeclarationSyntax : LuaStatementSyntax {
    }

    public sealed class LuaVariableListDeclarationSyntax : LuaVariableDeclarationSyntax {
        public LuaSyntaxList<LuaVariableDeclaratorSyntax> Variables { get; } = new LuaSyntaxList<LuaVariableDeclaratorSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaLocalVariableDeclaratorSyntax : LuaStatementSyntax {
        public LuaVariableDeclaratorSyntax Declarator { get; }

        public LuaLocalVariableDeclaratorSyntax(LuaVariableDeclaratorSyntax declarator) {
            if(declarator == null) {
                throw new ArgumentNullException(nameof(declarator));
            }
            Declarator = declarator;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaVariableDeclaratorSyntax : LuaStatementSyntax {
        public string LocalKeyword => Tokens.Local;
        public LuaIdentifierNameSyntax Identifier { get; }
        public LuaEqualsValueClauseSyntax Initializer { get; set; }

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

    public sealed class LuaTypeLocalAreaSyntax : LuaStatementSyntax {
        public string LocalKeyword => Tokens.Local;
        public LuaSyntaxList<LuaIdentifierNameSyntax> Variables { get; } = new LuaSyntaxList<LuaIdentifierNameSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}