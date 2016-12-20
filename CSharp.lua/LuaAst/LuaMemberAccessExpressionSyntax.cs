using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaMemberAccessExpressionSyntax : LuaExpressionSyntax {
        public LuaExpressionSyntax Expression { get; }
        public LuaExpressionSyntax Name { get; }
        public string OperatorToken { get; }

        public LuaMemberAccessExpressionSyntax(LuaExpressionSyntax expression, LuaExpressionSyntax name, bool isObjectColon = false) {
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            if(name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            Expression = expression;
            Name = name;
            OperatorToken = isObjectColon ? Tokens.ObjectColon : Tokens.Dot;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaWrapExpressionSyntax : LuaExpressionSyntax {
        public LuaExpressionSyntax Expression { get; set; }

        public LuaWrapExpressionSyntax() {

        }
    }

    public sealed class LuaPropertyAdapterExpressionSyntax : LuaExpressionSyntax {
        private LuaPropertyIdentifierNameSyntax identifier_;
        public LuaInvocationExpressionSyntax InvocationExpression { get; set; }

        public LuaPropertyAdapterExpressionSyntax(LuaPropertyIdentifierNameSyntax identifier) {
            identifier_ = identifier;
            InvocationExpression = new LuaInvocationExpressionSyntax(identifier);
        }

        public void Update(LuaMemberAccessExpressionSyntax memberAccessExpression) {
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(memberAccessExpression);
            invocationExpression.ArgumentList.Arguments.AddRange(InvocationExpression.ArgumentList.Arguments);
            InvocationExpression = invocationExpression;
        }

        public bool IsGet {
            set {
                 identifier_.IsGet = value;
            }
        }

        internal override void Render(LuaRenderer renderer) {
            InvocationExpression.Render(renderer);
        }
    }
}
