using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaFunctionExpressSyntax : LuaExpressionSyntax {
        public LuaParameterListSyntax ParameterList { get; } = new LuaParameterListSyntax();
        public string FunctionKeyword => Tokens.Function;

        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
            OpenBraceToken = Tokens.Empty,
            CloseBraceToken = Tokens.End,
        };

        private LuaLocalVariablesStatementSyntax tempStatement_ = new LuaLocalVariablesStatementSyntax();

        public LuaFunctionExpressSyntax() {
            Body.Statements.Add(tempStatement_);
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }

        public LuaIdentifierNameSyntax GetTemp1() {
            var temp1 = LuaIdentifierNameSyntax.Temp1;
            if(tempStatement_.Variables.Count == 0) {
                tempStatement_.Variables.Add(temp1);
            }
            return temp1;
        }
    }
}
