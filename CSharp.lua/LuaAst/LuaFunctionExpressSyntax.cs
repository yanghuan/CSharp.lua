using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public class LuaFunctionExpressSyntax : LuaExpressionSyntax {
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

        public void AddParameter(LuaIdentifierNameSyntax identifier) {
            ParameterList.Parameters.Add(new LuaParameterSyntax(identifier));
        }
    }

    public sealed class LuaConstructorAdapterExpressSyntax : LuaFunctionExpressSyntax {
        public bool IsStaticCtor { get; set; }
        public bool IsInvokeThisCtor { get; set; }
    }
}
