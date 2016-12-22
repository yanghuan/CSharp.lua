using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

    public sealed class LuaSwitchAdapterStatementSyntax : LuaStatementSyntax {
        public LuaRepeatStatementSyntax RepeatStatement = new LuaRepeatStatementSyntax(LuaIdentifierNameSyntax.One);

        public LuaSwitchAdapterStatementSyntax(LuaExpressionSyntax expression, IEnumerable<LuaStatementSyntax> sections) {
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            if(sections == null) {
                throw new ArgumentNullException(nameof(sections));
            }

            var body = RepeatStatement.Body;
            var temp = LuaIdentifierNameSyntax.Temp1;
            LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(temp);
            variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(expression);
            body.Statements.Add(new LuaLocalVariableDeclaratorSyntax(variableDeclarator));

            LuaIfStatementSyntax ifHeadStatement = null;
            LuaIfStatementSyntax ifTailStatement = null;
            LuaBlockSyntax defaultBock = null;
            foreach(var section in sections) {
                LuaIfStatementSyntax statement = section as LuaIfStatementSyntax;
                if(statement != null) {
                    if(ifTailStatement != null) {
                        ifTailStatement.Else = new LuaElseClauseSyntax(statement);
                    }
                    else {
                        ifHeadStatement = statement;
                    }
                    ifTailStatement = statement;
                }
                else {
                    Contract.Assert(defaultBock == null);
                    defaultBock = (LuaBlockSyntax)section;
                }
            }

            if(ifHeadStatement != null) {
                body.Statements.Add(ifHeadStatement);
                if(defaultBock != null) {
                    ifTailStatement.Else = new LuaElseClauseSyntax(defaultBock);
                }
            }
            else {
                if(defaultBock != null) {
                    body.Statements.AddRange(defaultBock.Statements);
                }
            }
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}