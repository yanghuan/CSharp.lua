using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public abstract class LuaWrapFunctionStatementSynatx : LuaStatementSyntax {
        private LuaInvocationExpressionSyntax invokeNode_;
        private LuaFunctionExpressSyntax functionNode_ = new LuaFunctionExpressSyntax();

        protected void UpdateIdentifiers(LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax target, LuaIdentifierNameSyntax memberName, LuaIdentifierNameSyntax parameter = null) {
            LuaMemberAccessExpressionSyntax memberAccessNode = new LuaMemberAccessExpressionSyntax(target, memberName);
            invokeNode_ = new LuaInvocationExpressionSyntax(memberAccessNode);
            invokeNode_.ArgumentList.Arguments.Add(new LuaArgumentSyntax(new LuaStringLiteralExpressionSyntax(name)));
            invokeNode_.ArgumentList.Arguments.Add(new LuaArgumentSyntax(functionNode_));
            if(parameter != null) {
                functionNode_.ParameterList.Parameters.Add(new LuaParameterSyntax(parameter));
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