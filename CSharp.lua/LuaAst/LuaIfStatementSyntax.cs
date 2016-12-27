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
        public LuaIdentifierNameSyntax Temp { get; }
        private LuaBlockSyntax defaultBock_;
        private LuaLocalVariablesStatementSyntax caseLabelVariables_ = new LuaLocalVariablesStatementSyntax();
        private LuaIdentifierNameSyntax defaultLabel_;
        private Dictionary<string, LuaIdentifierNameSyntax> caseLabels_ = new Dictionary<string, LuaIdentifierNameSyntax>();
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

        public void AddDefaultLabel(LuaIdentifierNameSyntax label) {
            Contract.Assert(defaultLabel_ == null);
            defaultLabel_ = label;
        }

        public void AddCaseLabel(LuaIdentifierNameSyntax label, string valueText) {
            caseLabels_[valueText] = label;
        }

        private void CheckHasDefaultLabel() {
            if(defaultLabel_ != null) {
                Contract.Assert(defaultBock_ != null);
                caseLabelVariables_.Variables.Add(defaultLabel_);
                LuaLabeledStatement labeledStatement = new LuaLabeledStatement(defaultLabel_);
                RepeatStatement.Body.Statements.Add(labeledStatement);
                LuaIfStatementSyntax IfStatement = new LuaIfStatementSyntax(defaultLabel_);
                IfStatement.Body.Statements.AddRange(defaultBock_.Statements);
                RepeatStatement.Body.Statements.Add(IfStatement);
            }
        }

        private bool IsConditionMatch(LuaBinaryExpressionSyntax condition, string keyText) {
            if(condition.Right is LuaLiteralExpressionSyntax) {
                var literalExpression = (LuaLiteralExpressionSyntax)condition.Right;
                return literalExpression.Text == keyText;
            }
            else {
                var left = (LuaBinaryExpressionSyntax)condition.Left;
                if(IsConditionMatch(left, keyText)) {
                    return true;
                }

                var right = (LuaBinaryExpressionSyntax)condition.Right;
                if(IsConditionMatch(right, keyText)) {
                    return true;
                }

                return false;
            }
        }

        private LuaIfStatementSyntax FindMatchIfStatement(string keyText) {
            LuaIfStatementSyntax head = headIfStatement_;
            while(head != null) {
                var condition = (LuaBinaryExpressionSyntax)head.Condition;
                if(IsConditionMatch(condition, keyText)) {
                    return head;
                }
                head = head.Else.Statement as LuaIfStatementSyntax;
            }
            return null;
        }

        private void CheckHasCaseLabel() {
            if(caseLabels_.Count > 0) {
                Contract.Assert(headIfStatement_ != null);
                caseLabelVariables_.Variables.AddRange(caseLabels_.Values);
                var groups = caseLabels_.GroupBy(pair => FindMatchIfStatement(pair.Key)).ToDictionary(i => i.Key, i => i.ToArray());
                foreach(var group in groups) {
                    LuaExpressionSyntax condition = null;
                    foreach(var pair in group.Value) {
                        LuaIdentifierNameSyntax labelIdentifier = pair.Value;
                        LuaLabeledStatement labeledStatement = new LuaLabeledStatement(labelIdentifier);
                        RepeatStatement.Body.Statements.Add(labeledStatement);
                        if(condition != null) {
                            condition = new LuaBinaryExpressionSyntax(condition, LuaSyntaxNode.Tokens.Or, labelIdentifier);
                        }
                        else {
                            condition = labelIdentifier;
                        }
                    }
                    LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
                    ifStatement.Body.Statements.AddRange(group.Key.Body.Statements);
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