using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using CSharpLua.LuaAst;

namespace CSharpLua {
    public sealed class LuaRenderer {
        public static class Setting {
            public static bool HasSemicolon { get; set; }
            private static int indent_;
            public static string IndentString;

            static Setting() {
                Indent = 4;
                HasSemicolon = true;
            }

            public static int Indent {
                get {
                    return indent_;
                }
                set {
                    if(indent_ != value) {
                        indent_ = value;
                        IndentString = new string(' ', indent_);
                    }
                }
            }
        }

        private TextWriter writer_;
        private bool isNewLine_;
        private int indentLevel_;

        public LuaRenderer(TextWriter writer) {
            writer_ = writer;
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

        internal void Render(LuaCompilationUnitSyntax node) {
            foreach(var statement in node.Statements) {
                statement.Render(this);
            }
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

        internal void Render(LuaFunctionExpressSyntax node) {
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

        internal void Render(LuaLiteralExpressionSyntax node) {
            node.Identifier.Render(this);
        }

        internal void Render(LuaStringLiteralExpressionSyntax node) {
            Write(node.OpenParenToken);
            node.Identifier.Render(this);
            Write(node.CloseParenToken);
        }

        internal void Render(LuaCharacterLiteralExpression node) {
            Write(((int)node.Character).ToString());
            WriteSpace();
            Write($"{node.OpenCommentToken} '{node.Character}' {node.CloseCommentToken}");
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
            Write(node.EqualsToken);
            WriteSeparatedSyntaxList(node.Values);
        }

        internal void Render(LuaAssignmentExpressionSyntax node) {
            node.Left.Render(this);
            Write(node.OperatorToken);
            node.Right.Render(this);
        }

        internal void Render(LuaMultipleAssignmentExpressionSyntax node) {
            Contract.Assert(node.Lefts.Count > 0 && node.Rights.Count > 0);
            WriteSeparatedSyntaxList(node.Lefts);
            Write(node.OperatorToken);
            WriteSeparatedSyntaxList(node.Rights);
        }

        internal void Render(LuaLineMultipleAssignmentExpressionSyntax node) {
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

        internal void Render(LuaReturnStatementSyntax node) {
            Write(node.ReturnKeyword);
            if(node.Expressions.Count > 0) {
                WriteSpace();
                WriteSeparatedSyntaxList(node.Expressions);
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
            Write(node.OperatorToken);
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

        internal void Render(LuaEqualsValueClauseSyntax node) {
            Write(node.EqualsToken);
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
            bool isClose = true;
            if(node.Else != null) {
                node.Else.Render(this);
                if(node.Else.Statement is LuaIfStatementSyntax) {
                    isClose = false;
                }
            }
            if(isClose) {
                Write(node.CloseParenToken);
                WriteNewLine();
            }
        }

        internal void Render(LuaElseClauseSyntax node) {
            Write(node.ElseKeyword);
            node.Statement.Render(this);
        }

        internal void Render(LuaPrefixUnaryExpressionSyntax node) {
            Write(node.OperatorToken);
            WriteSpace();
            node.Operand.Render(this);
        }

        internal void Render(LuaForInStatementSyntax node) {
            Write(node.ForKeyword);
            WriteSpace();
            LuaIdentifierNameSyntax.Placeholder.Render(this);
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

        /*
        internal void Render(LuaForAdapterStatementSyntax node) {
            node.Body.Render(this);
            WriteNewLine();
        }*/

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
            node.Body.Render(this);
            WriteNewLine();
        }

        internal void Render(LuaBreakStatementSyntax node) {
            Write(node.BreakKeyword);
            Write(node.SemicolonToken);
            WriteNewLine();
        }

        internal void Render(LuaBlockBlockSyntax node) {
            node.Body.Render(this);
            WriteNewLine();
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
    }
}
