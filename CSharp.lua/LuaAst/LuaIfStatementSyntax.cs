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
        public readonly LuaBlockSyntax Body = new LuaBlockSyntax();

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
        public readonly LuaRepeatStatementSyntax RepeatStatement = new LuaRepeatStatementSyntax(LuaIdentifierNameSyntax.One);
        public LuaIdentifierNameSyntax Temp { get; }
        private LuaBlockSyntax defaultBock_;
        private LuaLocalVariablesStatementSyntax caseLabelVariables_ = new LuaLocalVariablesStatementSyntax();
        public LuaIdentifierNameSyntax DefaultLabel { get; set; }
        public readonly Dictionary<int, LuaIdentifierNameSyntax> CaseLabels = new Dictionary<int, LuaIdentifierNameSyntax>();
        private LuaIfStatementSyntax headIfStatement_;

        public LuaSwitchAdapterStatementSyntax(LuaIdentifierNameSyntax temp) {
            Temp = temp;
        }

        public void Fill(LuaExpressionSyntax expression, IEnumerable<LuaStatementSyntax> sections) {
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            if(sections == null) {
                throw new ArgumentNullException(nameof(sections));
            }

            var body = RepeatStatement.Body;
            body.Statements.Add(caseLabelVariables_);
            LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(Temp);
            variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(expression);
            body.Statements.Add(new LuaLocalVariableDeclaratorSyntax(variableDeclarator));

            LuaIfStatementSyntax ifHeadStatement = null;
            LuaIfStatementSyntax ifTailStatement = null;
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
                    Contract.Assert(defaultBock_ == null);
                    defaultBock_ = (LuaBlockSyntax)section;
                }
            }

            if(ifHeadStatement != null) {
                body.Statements.Add(ifHeadStatement);
                if(defaultBock_ != null) {
                    ifTailStatement.Else = new LuaElseClauseSyntax(defaultBock_);
                }
                headIfStatement_ = ifHeadStatement;
            }
            else {
                if(defaultBock_ != null) {
                    body.Statements.AddRange(defaultBock_.Statements);
                }
            }
        }

        private void CheckHasDefaultLabel() {
            if(DefaultLabel != null) {
                Contract.Assert(defaultBock_ != null);
                caseLabelVariables_.Variables.Add(DefaultLabel);
                LuaLabeledStatement labeledStatement = new LuaLabeledStatement(DefaultLabel);
                RepeatStatement.Body.Statements.Add(labeledStatement);
                LuaIfStatementSyntax IfStatement = new LuaIfStatementSyntax(DefaultLabel);
                IfStatement.Body.Statements.AddRange(defaultBock_.Statements);
                RepeatStatement.Body.Statements.Add(IfStatement);
            }
        }

        private LuaIfStatementSyntax FindMatchIfStatement(int index) {
            LuaIfStatementSyntax head = headIfStatement_;
            int counter = 0;
            while(true) {
                if(counter == index) {
                    return head;
                }
                head = (LuaIfStatementSyntax)head.Else.Statement;
                ++counter;
            }
        }

        private void CheckHasCaseLabel() {
            if(CaseLabels.Count > 0) {
                Contract.Assert(headIfStatement_ != null);
                caseLabelVariables_.Variables.AddRange(CaseLabels.Values);
                foreach(var pair in CaseLabels) {
                    LuaIfStatementSyntax caseLabelStatement = FindMatchIfStatement(pair.Key);
                    LuaIdentifierNameSyntax labelIdentifier = pair.Value;
                    RepeatStatement.Body.Statements.Add(new LuaLabeledStatement(labelIdentifier));
                    LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(labelIdentifier);
                    ifStatement.Body.Statements.AddRange(caseLabelStatement.Body.Statements);
                    RepeatStatement.Body.Statements.Add(ifStatement);
                }
            }
        }                                                                                                                                                                                               

        internal override void Render(LuaRenderer renderer) {
            CheckHasCaseLabel();
            CheckHasDefaultLabel();
            renderer.Render(this);
        }
    }
}