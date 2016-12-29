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

        public bool IsObjectColon {
            get {
                return OperatorToken == Tokens.ObjectColon;
            }
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
        private LuaPropertyOrEventIdentifierNameSyntax identifier_;
        public LuaInvocationExpressionSyntax InvocationExpression { get; set; }

        public LuaPropertyAdapterExpressionSyntax(LuaPropertyOrEventIdentifierNameSyntax identifier) {
            identifier_ = identifier;
            InvocationExpression = new LuaInvocationExpressionSyntax(identifier);
        }

        public LuaPropertyAdapterExpressionSyntax(LuaMemberAccessExpressionSyntax memberAccess, LuaPropertyOrEventIdentifierNameSyntax identifier) {
            identifier_ = identifier;
            InvocationExpression = new LuaInvocationExpressionSyntax(memberAccess);
        }

        public void Update(LuaMemberAccessExpressionSyntax memberAccessExpression) {
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(memberAccessExpression);
            invocationExpression.ArgumentList.Arguments.AddRange(InvocationExpression.ArgumentList.Arguments);
            InvocationExpression = invocationExpression;
        }

        public bool IsGetOrAdd {
            set {
                 identifier_.IsGetOrAdd = value;
            }
        }

        public bool IsProperty {
            get {
                return identifier_.IsProperty;
            }
        }

        private bool isAutoGet_;

        public LuaExpressionSyntax GetCloneOfGet() {
            IsGetOrAdd = false;
            isAutoGet_ = true;
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(InvocationExpression.Expression);
            invocationExpression.ArgumentList.Arguments.AddRange(InvocationExpression.ArgumentList.Arguments);
            return invocationExpression;
        }

        internal override void Render(LuaRenderer renderer) {
            InvocationExpression.Expression.Render(renderer);
            if(isAutoGet_) {
                IsGetOrAdd = true;
            }
            InvocationExpression.ArgumentList.Render(renderer);
        }
    }
}
