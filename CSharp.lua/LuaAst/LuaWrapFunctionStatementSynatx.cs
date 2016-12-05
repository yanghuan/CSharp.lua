using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public abstract class LuaWrapFunctionStatementSynatx : LuaStatementSyntax {
        private LuaInvocationExpressionSyntax invokeNode_ = new LuaInvocationExpressionSyntax();
        private LuaFunctionExpressSyntax functionNode_;

        public LuaWrapFunctionStatementSynatx(LuaIdentifierNameSyntax name) {
            invokeNode_.ArgumentList.Arguments.Add(new LuaArgumentSyntax(new LuaLiteralExpressionSyntax(name)));
            functionNode_ = new LuaFunctionExpressSyntax();
            invokeNode_.ArgumentList.Arguments.Add(new LuaArgumentSyntax(functionNode_));
        }

        protected void UpdateIdentifiers(LuaIdentifierNameSyntax target, LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax parameter = null) {
            LuaMemberAccessExpressionSyntax memberAccessNode = new LuaMemberAccessExpressionSyntax(target, name, false);
            invokeNode_.Expression = memberAccessNode;
            if(parameter != null) {
                functionNode_.ParameterList.Arguments.Add(new LuaParameterSyntax(parameter));
            }
        }

        public void Add(LuaStatementSyntax statement) {
            functionNode_.Body.Statements.Add(statement);
        }

        internal override void Render(LuaRenderer renderer) {
            LuaExpressionStatementSyntax node = new LuaExpressionStatementSyntax(invokeNode_);
            node.Render(renderer);
        }
    }
}