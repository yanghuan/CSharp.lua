/*
Copyright 2016 YANG Huan (sy.yanghuan@gmail.com).
Copyright 2016 Redmoon Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public sealed class LuaLocalVariablesStatementSyntax : LuaVariableDeclarationSyntax {
        public string LocalKeyword => Tokens.Local;
        public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Variables = new LuaSyntaxList<LuaIdentifierNameSyntax>();
        public LuaEqualsValueClauseListSyntax Initializer { get; set; }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaEqualsValueClauseListSyntax : LuaSyntaxNode {
        public string EqualsToken => Tokens.Equals;
        public readonly LuaSyntaxList<LuaExpressionSyntax> Values = new LuaSyntaxList<LuaExpressionSyntax>();

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
        public readonly LuaSyntaxList<LuaVariableDeclaratorSyntax> Variables = new LuaSyntaxList<LuaVariableDeclaratorSyntax>();

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
        public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Variables = new LuaSyntaxList<LuaIdentifierNameSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}