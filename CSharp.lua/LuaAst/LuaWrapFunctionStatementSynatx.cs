using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public abstract class LuaWrapFunctionStatementSynatx : LuaStatementSyntax {
        public LuaExpressionStatementSyntax Statement { get; private set; }
        private LuaFunctionExpressSyntax function_ = new LuaFunctionExpressSyntax();
        protected List<LuaStatementSyntax> statements_ = new List<LuaStatementSyntax>();

        protected void UpdateIdentifiers(LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax target, LuaIdentifierNameSyntax memberName, LuaIdentifierNameSyntax parameter = null) {
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(target, memberName);
            LuaInvocationExpressionSyntax invoke = new LuaInvocationExpressionSyntax(memberAccess);
            invoke.AddArgument(new LuaStringLiteralExpressionSyntax(name));
            invoke.AddArgument(function_);
            if(parameter != null) {
                function_.AddParameter(parameter);
            }
            Statement = new LuaExpressionStatementSyntax(invoke);
        }

        public void Add(LuaStatementSyntax statement) {
            if(statement == null) {
                throw new ArgumentNullException(nameof(statement));
            }
            statements_.Add(statement);
        }

        internal override void Render(LuaRenderer renderer) {
            function_.Body.Statements.AddRange(statements_);
            renderer.Render(this);
        }
    }
}