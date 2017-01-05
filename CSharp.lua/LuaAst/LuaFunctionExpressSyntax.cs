using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public class LuaFunctionExpressionSyntax : LuaExpressionSyntax {
        public LuaParameterListSyntax ParameterList { get; } = new LuaParameterListSyntax();
        public string FunctionKeyword => Tokens.Function;
        public bool HasYield { get; set; }
        public int TempIndex;

        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
            OpenBraceToken = Tokens.Empty,
            CloseBraceToken = Tokens.End,
        };

        public void AddParameter(LuaIdentifierNameSyntax identifier) {
            ParameterList.Parameters.Add(new LuaParameterSyntax(identifier));
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaSimpleLambdaAdapterExpression : LuaExpressionSyntax {
        public LuaFunctionExpressionSyntax FunctionExpress { get; }
        public string EndToken => Tokens.End;

        public LuaSimpleLambdaAdapterExpression(LuaFunctionExpressionSyntax functionExpress) {
            FunctionExpress = functionExpress;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaConstructorAdapterExpressionSyntax : LuaFunctionExpressionSyntax {
        public bool IsStaticCtor { get; set; }
        public bool IsInvokeThisCtor { get; set; }
    }

    public abstract class LuaSpecialAdapterFunctionExpressionSyntax : LuaFunctionExpressionSyntax {
    }

    public sealed class LuaTryBlockAdapterExpressionSyntax : LuaSpecialAdapterFunctionExpressionSyntax {
        public LuaIdentifierNameSyntax CatchTemp { get; set; }
    }

    public sealed class LuaUsingAdapterExpressionSyntax : LuaSpecialAdapterFunctionExpressionSyntax {
    }
}
