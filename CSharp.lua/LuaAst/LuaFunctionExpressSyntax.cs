using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaFunctionExpressSyntax : LuaExpressionSyntax {
        public LuaParameterListSyntax ParameterList { get; } = new LuaParameterListSyntax();
        public string FunctionKeyword => Tokens.Function;
        public bool HasYield { get; set; }

        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
            OpenBraceToken = Tokens.Empty,
            CloseBraceToken = Tokens.End,
        };

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
