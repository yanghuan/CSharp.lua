using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CSharpLua.LuaAst;

namespace CSharpLua {
    public sealed class LuaRenderer {
        public bool IsWriteSemicolon { get; set; }

        private TextWriter writer_;
        private int indent_;
        private string indentStr_;
        private bool isNewLine_;
        private int indentLevel_;

        public LuaRenderer(TextWriter writer) {
            writer_ = writer;
            Indent = 4;
        }

        public int Indent {
            get {
                return indent_;
            }
            set {
                if(indent_ != value) {
                    indent_ = value;
                    indentStr_ = new string(' ', indent_);
                }
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
                    writer_.Write(indentStr_);
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
            WriteArgumentList(node.OpenParenToken, node.Arguments, node.CloseParenToken);
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
            Write(node.OpenParenToken);
            node.Identifier.Render(this);
            Write(node.CloseParenToken);
        }

        internal void Render(LuaStatementListSyntax node) {
            foreach(var statement in node.Statements) {
                statement.Render(this);
            }
        }

        internal void Render(LuaLocalDeclarationStatementSyntax node) {
            Write(node.LocalKeyword);
            WriteSpace();
            node.Declaration.Render(this);
            Write(node.SemicolonToken);
            WriteNewLine();
        }

        internal void Render(LuaVariableDeclarationSyntax node) {
            WriteSeparatedSyntaxList(node.Variables);
            if(node.Initializer != null) {
                node.Initializer.Render(this);
            }
        }

        internal void Render(LuaAssignmentExpressionSyntax node) {
            node.Left.Render(this);
            Write(node.OperatorToken);
            node.Right.Render(this);
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
            WriteSeparatedSyntaxList(node.Items);
            Write(node.CloseBraceToken);
        }
    }
}
