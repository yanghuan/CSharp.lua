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

namespace CSharpLua.LuaAst {
  public sealed class LuaArgumentListSyntax : LuaSyntaxNode {
    public string OpenParenToken => Tokens.OpenParentheses;
    public string CloseParenToken => Tokens.CloseParentheses;
    public readonly LuaSyntaxList<LuaExpressionSyntax> Arguments = new();
    public bool IsCallSingleTable { get; set; }

    public void AddArgument(LuaExpressionSyntax argument) {
      Arguments.Add(argument);
    }

    public void AddArguments(IEnumerable<LuaExpressionSyntax> arguments) {
      foreach (var argument in arguments) {
        AddArgument(argument);
      }
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaInvocationExpressionSyntax : LuaExpressionSyntax {
    public readonly LuaArgumentListSyntax ArgumentList = new();
    public LuaExpressionSyntax Expression { get; }
    public List<LuaExpressionSyntax> Arguments => ArgumentList.Arguments;

    public LuaInvocationExpressionSyntax(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
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
      AddArguments(arguments);
    }

    public LuaInvocationExpressionSyntax(LuaExpressionSyntax expression, params LuaExpressionSyntax[] arguments) : this(expression) {
      AddArguments(arguments);
    }

    public void AddArgument(LuaExpressionSyntax argument) {
      ArgumentList.AddArgument(argument);
    }

    public void AddArguments(IEnumerable<LuaExpressionSyntax> arguments) {
      ArgumentList.AddArguments(arguments);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }
}
