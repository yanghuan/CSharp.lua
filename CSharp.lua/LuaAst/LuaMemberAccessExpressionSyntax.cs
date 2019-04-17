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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
  public sealed class LuaMemberAccessExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax Expression { get; private set; }
    public LuaExpressionSyntax Name { get; }
    public string OperatorToken { get; }

    public LuaMemberAccessExpressionSyntax(LuaExpressionSyntax expression, LuaExpressionSyntax name, bool isObjectColon = false) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
      Name = name ?? throw new ArgumentNullException(nameof(name));
      OperatorToken = isObjectColon ? Tokens.ObjectColon : Tokens.Dot;
    }

    public bool IsObjectColon {
      get {
        return OperatorToken == Tokens.ObjectColon;
      }
    }

    public void UpdateExpression(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaPropertyAdapterExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax Expression { get; private set; }
    public string OperatorToken { get; private set; }
    public LuaPropertyOrEventIdentifierNameSyntax Name { get; }
    public readonly LuaArgumentListSyntax ArgumentList = new LuaArgumentListSyntax();

    public LuaPropertyAdapterExpressionSyntax(LuaPropertyOrEventIdentifierNameSyntax name) {
      Name = name;
    }

    public LuaPropertyAdapterExpressionSyntax(LuaExpressionSyntax expression, LuaPropertyOrEventIdentifierNameSyntax name, bool isObjectColon) {
      Update(expression, isObjectColon);
      Name = name;
    }

    public void Update(LuaExpressionSyntax expression, bool isObjectColon) {
      Contract.Assert(Expression == null);
      Expression = expression;
      OperatorToken = isObjectColon ? Tokens.ObjectColon : Tokens.Dot;
    }

    public void Update(LuaExpressionSyntax expression) {
      Expression = expression;
    }

    public bool IsGetOrAdd {
      set {
        Name.IsGetOrAdd = value;
      }
      get {
        return Name.IsGetOrAdd;
      }
    }

    public bool IsProperty {
      get {
        return Name.IsProperty;
      }
    }

    public bool IsObjectColon {
      get {
        return OperatorToken == Tokens.ObjectColon;
      }
    }

    public LuaPropertyAdapterExpressionSyntax GetClone() {
      LuaPropertyAdapterExpressionSyntax clone = new LuaPropertyAdapterExpressionSyntax(Name.GetClone()) {
        Expression = Expression,
        OperatorToken = OperatorToken,
      };
      clone.ArgumentList.Arguments.AddRange(ArgumentList.Arguments);
      return clone;
    }

    public LuaPropertyAdapterExpressionSyntax GetCloneOfGet() {
      LuaPropertyAdapterExpressionSyntax clone = GetClone();
      clone.IsGetOrAdd = true;
      IsGetOrAdd = false;
      return clone;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }
}
