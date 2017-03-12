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
using CSharpLua.LuaAst;

namespace CSharpLua {
    public sealed class LuaRenderer {
        private LuaSyntaxGenerator generator_;
        private TextWriter writer_;
        private bool isNewLine_;
        private int indentLevel_;

        public LuaRenderer(LuaSyntaxGenerator generator, TextWriter writer) {
            generator_ = generator;
            writer_ = writer;
        }

        private LuaSyntaxGenerator.SettingInfo Setting {
            get {
                return generator_.Setting;
            }
        }

        private void AddIndent() {
            ++indentLevel_;
        }

        private void Outdent() {
            if(indentLevel_ == 0) {
                throw new InvalidOperationException();
            }
            --indentLevel_;
        }

        private void WriteNewLine() {
            writer_.Write('\n');
            isNewLine_ = true;
        }

        private void WriteComma() {
            Write(", ");
        }

        private void WriteSpace() {
            Write(" ");
        }

        private void Write(string value) {
            if(isNewLine_) {
                for(int i = 0; i < indentLevel_; i++) {
                    writer_.Write(Setting.IndentString);
                }
                isNewLine_ = false;
            }
            writer_.Write(value);
        }

        private void Write(LuaSyntaxNode.Semicolon semicolonToken) {
            if(Setting.HasSemicolon) {
                Write(semicolonToken.ToString());
            }
        }

        internal void Render(LuaCompilationUnitSyntax node) {
            foreach(var statement in node.Statements) {
                statement.Render(this);
            }
        }

        internal void Render(LuaWrapFunctionStatementSynatx node) {
            node.Statement.Render(this);
        }

        internal void Render(LuaExpressionStatementSyntax node) {
            node.Expression.Render(this);
            Write(node.SemicolonToken);
            WriteNewLine();
        }

        internal void Render(LuaMemberAccessExpressionSyntax node) {
            node.Expression.Render(this);
            Write(node.OperatorToken);
            node.Name.Render(this);
        }

        internal void Render(LuaInvocationExpressionSyntax node) {
            node.Expression.Render(this);
            node.ArgumentList.Render(this);
        }

        internal void Render(LuaIdentifierNameSyntax node) {
            Write(node.ValueText);
        }

        internal void Render(LuaPropertyOrEventIdentifierNameSyntax node) {
            Write(node.PrefixToken);
            node.Name.Render(this);
        }

        private void WriteSeparatedSyntaxList(IEnumerable<LuaSyntaxNode> list) {
            bool isFirst = true;
            foreach(LuaSyntaxNode node in list) {
                if(isFirst) {
                    isFirst = false;
                }
                else {
                    WriteComma();
                }
                node.Render(this);
            }
        }

        private void WriteArgumentList(string openParenToken, IEnumerable<LuaSyntaxNode> list, string closeParenToken) {
            Write(openParenToken);
            WriteSeparatedSyntaxList(list);
            Write(closeParenToken);
        }

        internal void Render(LuaArgumentListSyntax node) {
            WriteArgumentList(node.OpenParenToken, node.Arguments, node.CloseParenToken);
        }

        internal void Render(LuaArgumentSyntax node) {
            node.Expression.Render(this);
        }

        internal void Render(LuaFunctionExpressionSyntax node) {
            Write(node.FunctionKeyword);
            WriteSpace();
            node.ParameterList.Render(this);
            WriteSpace();
            node.Body.Render(this);
        }

        internal void Render(LuaParameterListSyntax node) {
            WriteArgumentList(node.OpenParenToken, node.Parameters, node.CloseParenToken);
        }

        internal void Render(LuaParameterSyntax node) {
            node.Identifier.Render(this);
        }

        internal void Render(LuaBlockSyntax node) {
            Write(node.OpenBraceToken);
            WriteNewLine();
            AddIndent();
            foreach(var statement in node.Statements) {
                statement.Render(this);
            }
            Outdent();
            Write(node.CloseBraceToken);
        }

        internal void Render(LuaBlockStatementSyntax node) {
            Render((LuaBlockSyntax)node);
            WriteNewLine();
        }

        internal void Render(LuaIdentifierLiteralExpressionSyntax node) {
            node.Identifier.Render(this);
        }

        internal void Render(LuaStringLiteralExpressionSyntax node) {
            Write(node.OpenParenToken);
            node.Identifier.Render(this);
            Write(node.CloseParenToken);
        }

        private void WriteEquals(int count) {
            for(int i = 0; i < count; ++i) {
                Write(LuaSyntaxNode.Tokens.Equals);
            }
        }

        internal void Render(LuaVerbatimStringLiteralExpressionSyntax node) {
            Write(node.OpenBracket);
            WriteEquals(node.EqualsCount);
            Write(node.OpenBracket);
            Write(node.Text);
            Write(node.CloseBracket);
            WriteEquals(node.EqualsCount);
            Write(node.CloseBracket);
        }

        internal void Render(LuaConstLiteralExpression node) {
            node.Value.Render(this);
            WriteSpace();
            Write(node.OpenComment);
            Write(node.IdentifierToken);
            Write(node.CloseComment);
        }

        internal void Render(LuaStatementListSyntax node) {
            foreach(var statement in node.Statements) {
                statement.Render(this);
            }
        }

        internal void Render(LuaLocalVariablesStatementSyntax node) {
            if(node.Variables.Count > 0) {
                Write(node.LocalKeyword);
                WriteSpace();
                WriteSeparatedSyntaxList(node.Variables);
                node.Initializer?.Render(this);
                Write(node.SemicolonToken);
                WriteNewLine();
            }
        }

        internal void Render(LuaEqualsValueClauseListSyntax node) {
            WriteSpace();
            Write(node.EqualsToken);
            WriteSpace();
            WriteSeparatedSyntaxList(node.Values);
        }

        internal void Render(LuaAssignmentExpressionSyntax node) {
            node.Left.Render(this);
            WriteSpace();
            Write(node.OperatorToken);
            WriteSpace();
            node.Right.Render(this);
        }

        internal void Render(LuaMultipleAssignmentExpressionSyntax node) {
            Contract.Assert(node.Lefts.Count > 0 && node.Rights.Count > 0);
            WriteSeparatedSyntaxList(node.Lefts);
            WriteSpace();
            Write(node.OperatorToken);
            WriteSpace();
            WriteSeparatedSyntaxList(node.Rights);
        }

        internal void Render(LuaLineMultipleExpressionSyntax node) {
            bool isFirst = true;
            foreach(var assignment in node.Assignments) {
                if(isFirst) {
                    isFirst = false;
                }
                else {
                    Write(LuaSyntaxNode.Tokens.Semicolon);
                    WriteSpace();
                }
                assignment.Render(this);
            }
        }

        internal void Render(LuaMultipleReturnStatementSyntax node) {
            Write(node.ReturnKeyword);
            if(node.Expressions.Count > 0) {
                WriteSpace();
                WriteSeparatedSyntaxList(node.Expressions);
            }
            Write(node.SemicolonToken);
            WriteNewLine();
        }

        internal void Render(LuaReturnStatementSyntax node) {
            Write(node.ReturnKeyword);
            if(node.Expression != null) {
                WriteSpace();
                node.Expression.Render(this);
            }
            Write(node.SemicolonToken);
            WriteNewLine();
        }

        internal void Render(LuaTableInitializerExpression node) {
            Write(node.OpenBraceToken);
            if(node.Items.Count > 0) {
                WriteNewLine();
                AddIndent();
                bool isFirst = true;
                foreach(LuaSyntaxNode itemNode in node.Items) {
                    if(isFirst) {
                        isFirst = false;
                    }
                    else {
                        WriteComma();
                        WriteNewLine();
                    }
                    itemNode.Render(this);
                }
                Outdent();
                WriteNewLine();
            }
            Write(node.CloseBraceToken);
        }

        internal void Render(LuaSingleTableItemSyntax node) {
            node.Expression.Render(this);
        }

        internal void Render(LuaKeyValueTableItemSyntax node) {
            node.Key.Render(this);
            WriteSpace();
            Write(node.OperatorToken);
            WriteSpace();
            node.Value.Render(this);
        }

        internal void Render(LuaTableExpressionKeySyntax node) {
            Write(node.OpenBracketToken);
            node.Expression.Render(this);
            Write(node.CloseBracketToken);
        }

        internal void Render(LuaTableLiteralKeySyntax node) {
            node.Identifier.Render(this);
        }

        internal void Render(LuaTableIndexAccessExpressionSyntax node) {
            node.Expression.Render(this);
            Write(node.OpenBracketToken);
            node.Index.Render(this);
            Write(node.CloseBracketToken);
        }

        internal void Render(LuaEqualsValueClauseSyntax node) {
            WriteSpace();
            Write(node.EqualsToken);
            WriteSpace();
            node.Value.Render(this);
        }

        internal void Render(LuaLocalDeclarationStatementSyntax node) {
            node.Declaration.Render(this);
        }

        internal void Render(LuaVariableListDeclarationSyntax node) {
            if(node.Variables.Count > 0) {
                bool isFirst = true;
                foreach(var variable in node.Variables) {
                    if(isFirst) {
                        isFirst = false;
                    }
                    else {
                        WriteSpace();
                    }
                    variable.Render(this);
                }
                WriteNewLine();
            }
        }

        internal void Render(LuaVariableDeclaratorSyntax node) {
            Write(node.LocalKeyword);
            WriteSpace();
            node.Identifier.Render(this);
            node.Initializer?.Render(this);
            Write(node.SemicolonToken);
        }

        internal void Render(LuaLocalVariableDeclaratorSyntax node) {
            node.Declarator.Render(this);
            WriteNewLine();
        }

        internal void Render(LuaTypeLocalAreaSyntax node) {
            const int kPerLineCount = 8;
            if(node.Variables.Count > 0) {
                Write(node.LocalKeyword);
                WriteSpace();
                int count = 0;
                foreach(var item in node.Variables) {
                    if(count > 0) {
                        WriteComma();
                        if(count % kPerLineCount == 0) {
                            WriteNewLine();
                        }
                    }
                    item.Render(this);
                    ++count;
                }
                Write(node.SemicolonToken);
                WriteNewLine();
            }
        }

        internal void Render(LuaBinaryExpressionSyntax node) {
            node.Left.Render(this);
            WriteSpace();
            Write(node.OperatorToken);
            WriteSpace();
            node.Right.Render(this);
        }

        internal void Render(LuaIfStatementSyntax node) {
            Write(node.IfKeyword);
            WriteSpace();
            node.Condition.Render(this);
            WriteSpace();
            Write(node.OpenParenToken);
            node.Body.Render(this);
            foreach(var elseIfNode in node.ElseIfStatements) {
                elseIfNode.Render(this);
            }
            node.Else?.Render(this);
            Write(node.CloseParenToken);
            WriteNewLine();
        }

        internal void Render(LuaElseIfStatementSyntax node) {
            Write(node.ElseIfKeyword);
            WriteSpace();
            node.Condition.Render(this);
            WriteSpace();
            Write(node.OpenParenToken);
            node.Body.Render(this);
        }

        internal void Render(LuaElseClauseSyntax node) {
            Write(node.ElseKeyword);
            node.Body.Render(this);
        }

        internal void Render(LuaPrefixUnaryExpressionSyntax node) {
            Write(node.OperatorToken);
            WriteSpace();
            node.Operand.Render(this);
        }

        internal void Render(LuaForInStatementSyntax node) {
            Write(node.ForKeyword);
            WriteSpace();
            node.Placeholder.Render(this);
            WriteComma();
            node.Identifier.Render(this);
            WriteSpace();
            Write(node.InKeyword);
            WriteSpace();
            node.Expression.Render(this);
            WriteSpace();
            node.Body.Render(this);
            WriteNewLine();
        }

        internal void Render(LuaWhileStatementSyntax node) {
            Write(node.WhileKeyword);
            WriteSpace();
            node.Condition.Render(this);
            WriteSpace();
            node.Body.Render(this);
            WriteNewLine();
        }

        internal void Render(LuaRepeatStatementSyntax node) {
            Write(node.RepeatKeyword);
            node.Body.Render(this);
            Write(node.UntilKeyword);
            WriteSpace();
            node.Condition.Render(this);
            Write(node.SemicolonToken);
            WriteNewLine();
        }

        internal void Render(LuaSwitchAdapterStatementSyntax node) {
            node.RepeatStatement.Render(this);
        }

        internal void Render(LuaBreakStatementSyntax node) {
            Write(node.BreakKeyword);
            Write(node.SemicolonToken);
            WriteNewLine();
        }

        internal void Render(LuaContinueAdapterStatementSyntax node) {
            node.Assignment.Render(this);
            node.Break.Render(this);
        }

        internal void Render(LuaBlankLinesStatement node) {
            for(int i = 0; i < node.Count; ++i) {
                WriteNewLine();
            }
        }

        internal void Render(LuaShortCommentStatement node) {
            Write(node.SingleCommentToken);
            Write(node.Comment);
            WriteNewLine();
        }

        internal void Render(LuaLongCommentStatement node) {
            Write(node.OpenCommentToken);
            Write(node.Comment);
            Write(node.CloseCommentToken);
            WriteNewLine();
        }

        internal void Render(LuaParenthesizedExpressionSyntax node) {
            Write(node.OpenParenToken);
            node.Expression.Render(this);
            Write(node.CloseParenToken);
        }

        internal void Render(LuaGotoStatement node) {
            Write(node.GotoKeyword);
            WriteSpace();
            node.Identifier.Render(this);
            Write(node.SemicolonToken);
            WriteNewLine();
        }

        internal void Render(LuaLabeledStatement node) {
            Write(node.PrefixToken);
            node.Identifier.Render(this);
            Write(node.SuffixToken);
            Write(node.SemicolonToken);
            WriteNewLine();
            node.Statement?.Render(this);
        }

        internal void Render(LuaGotoCaseAdapterStatement node) {
            node.Assignment.Render(this);
            node.GotoStatement.Render(this);
        }

        internal void Render(LuaCodeTemplateExpressionSyntax node) {
            foreach(var code in node.Expressions) {
                code.Render(this);
            }
        }

        internal void Render(LuaPropertyAdapterExpressionSyntax node) {
            if(node.Expression != null) {
                node.Expression.Render(this);
                Write(node.OperatorToken);
            }
            node.Name.Render(this);
            node.ArgumentList.Render(this);
        }
    }
}
