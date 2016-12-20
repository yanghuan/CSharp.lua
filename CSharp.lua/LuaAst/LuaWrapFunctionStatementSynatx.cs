using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public abstract class LuaWrapFunctionStatementSynatx : LuaStatementSyntax {
        public LuaExpressionStatementSyntax Statement { get; private set; }
        private LuaFunctionExpressSyntax functionNode_ = new LuaFunctionExpressSyntax();

        protected void UpdateIdentifiers(LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax target, LuaIdentifierNameSyntax memberName, LuaIdentifierNameSyntax parameter = null) {
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(target, memberName);
            LuaInvocationExpressionSyntax invoke = new LuaInvocationExpressionSyntax(memberAccess);
            invoke.ArgumentList.Arguments.Add(new LuaArgumentSyntax(new LuaStringLiteralExpressionSyntax(name)));
            invoke.ArgumentList.Arguments.Add(new LuaArgumentSyntax(functionNode_));
            if(parameter != null) {
                functionNode_.AddParameter(parameter);
            }
            Statement = new LuaExpressionStatementSyntax(invoke);
        }

        public void Add(LuaStatementSyntax statement) {
            functionNode_.Body.Statements.Add(statement);
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}