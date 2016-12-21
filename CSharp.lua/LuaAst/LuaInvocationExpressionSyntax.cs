using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaInvocationExpressionSyntax : LuaExpressionSyntax {
        public LuaArgumentListSyntax ArgumentList { get; } = new LuaArgumentListSyntax();
        public LuaExpressionSyntax Expression { get; }

        public LuaInvocationExpressionSyntax(LuaExpressionSyntax expression) {
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            Expression = expression;
        }

        public void AddArgument(LuaExpressionSyntax argument) {
            ArgumentList.Arguments.Add(new LuaArgumentSyntax(argument));
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
