/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
  public class LuaFunctionExpressionSyntax : LuaExpressionSyntax {
    public readonly LuaParameterListSyntax ParameterList = new LuaParameterListSyntax();
    public string FunctionKeyword => Tokens.Function;
    public int TempCount;

    public readonly LuaBlockSyntax Body = new LuaBlockSyntax() {
      OpenToken = Tokens.Empty,
      CloseToken = Tokens.End,
    };

    public void AddParameter(LuaParameterSyntax parameter) {
      ParameterList.Parameters.Add(parameter);
    }

    public void AddParameters(IEnumerable<LuaParameterSyntax> parameters) {
      ParameterList.Parameters.AddRange(parameters);
    }

    public void AddParameter(LuaIdentifierNameSyntax identifier) {
      AddParameter(new LuaParameterSyntax(identifier));
    }

    public void AddStatement(LuaStatementSyntax statement) {
      Body.Statements.Add(statement);
    }

    public void AddStatement(LuaExpressionSyntax expression) {
      AddStatement(new LuaExpressionStatementSyntax(expression));
    }

    public void AddStatements(IEnumerable<LuaStatementSyntax> statements) {
      Body.Statements.AddRange(statements);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaConstructorAdapterExpressionSyntax : LuaFunctionExpressionSyntax {
    public bool IsInvokeThisCtor { get; set; }
    public bool IsStatic { get; set; }
  }

  public abstract class LuaCheckReturnFunctionExpressionSyntax : LuaFunctionExpressionSyntax {
  }

  public sealed class LuaTryAdapterExpressionSyntax : LuaCheckReturnFunctionExpressionSyntax {
    public LuaIdentifierNameSyntax CatchTemp { get; set; }
  }

  public sealed class LuaUsingAdapterExpressionSyntax : LuaCheckReturnFunctionExpressionSyntax {
  }
}
