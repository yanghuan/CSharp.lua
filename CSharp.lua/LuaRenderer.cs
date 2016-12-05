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

        private void WriteFunction() {
            Write("function ");
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
        }

        internal void Render(LuaMemberAccessExpressionSyntax node) {
            node.Expression.Render(this);
            Write(node.OperatorToken);
            node.Name.Render(this);
        }

        internal void Render(LuaInvocationExpressionSyntax node) {
            node.Expression.Render(this);
            node.ArgumentList.Render(this);
            WriteNewLine();
        }

        internal void Render(LuaIdentifierNameSyntax node) {
            Write(node.ValueText);
        }

        private void WriteArgumentList(string openParenToken, IEnumerable<LuaSyntaxNode> arguments, string closeParenToken) {
            Write(openParenToken);
            bool isFirst = true;
            foreach(var argument in arguments) {
                if(isFirst) {
                    isFirst = false;
                }
                else {
                    WriteComma();
                }
                argument.Render(this);
            }
            Write(closeParenToken);
        }

        internal void Render(LuaArgumentListSyntax node) {
            WriteArgumentList(node.OpenParenToken, node.Arguments, node.CloseParenToken);
        }

        internal void Render(LuaArgumentSyntax node) {
            node.Expression.Render(this);
        }

        internal void Render(LuaFunctionExpressSyntax node) {
            WriteFunction();
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

        internal void Render(LuaTypeDeclarationSyntax node) {
            node.Local.Render(this);
            node.MethodList.Render(this);
        }

        internal void Render(LuaStatementListSyntax node) {
            foreach(var statement in node.Statements) {
                statement.Render(this);
            }
        }

        internal void Render(LuaLocalDeclarationStatementSyntax node) {
            node.Declaration.Render(this);
            Write(node.SemicolonToken);
        }

        internal void Render(LuaVariableDeclarationSyntax node) {
            WriteArgumentList(string.Empty, node.Variables, string.Empty);
            if(node != null) {
                node.Render(this);
            }
        }
    }
}
