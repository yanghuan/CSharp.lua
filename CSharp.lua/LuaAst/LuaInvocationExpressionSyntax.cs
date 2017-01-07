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

        public LuaInvocationExpressionSyntax(LuaExpressionSyntax expression, LuaExpressionSyntax argument) : this(expression) {
            AddArgument(argument);
        }

        public LuaInvocationExpressionSyntax(LuaExpressionSyntax expression, LuaExpressionSyntax argument1, LuaExpressionSyntax argument2) : this(expression) {
            AddArgument(argument1);
            AddArgument(argument2);
        }

        public LuaInvocationExpressionSyntax(LuaExpressionSyntax expression, LuaExpressionSyntax argument1, LuaExpressionSyntax argument2, LuaExpressionSyntax argument3) : this(expression) {
            AddArgument(argument1);
            AddArgument(argument2);
            AddArgument(argument3);
        }

        public LuaInvocationExpressionSyntax(LuaExpressionSyntax expression, IEnumerable<LuaExpressionSyntax> arguments) : this(expression) {
            ArgumentList.Arguments.AddRange(arguments.Select(i => new LuaArgumentSyntax(i)));
        }

        public LuaInvocationExpressionSyntax(LuaExpressionSyntax expression, params LuaExpressionSyntax[] arguments) : this(expression) {
            ArgumentList.Arguments.AddRange(arguments.Select(i => new LuaArgumentSyntax(i)));
        }

        public void AddArgument(LuaExpressionSyntax argument) {
            ArgumentList.Arguments.Add(new LuaArgumentSyntax(argument));
        }

   
        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
