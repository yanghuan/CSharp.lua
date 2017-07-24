/*
Copyright 2016 YANG Huan (sy.yanghuan@gmail.com).
Copyright 2016 Redmoon Inc.

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
  public sealed class LuaForInStatementSyntax : LuaStatementSyntax {
    public LuaExpressionSyntax Expression { get; }
    public string ForKeyword => Tokens.For;
    public LuaIdentifierNameSyntax Identifier { get; }
    public string InKeyword => Tokens.In;
    public LuaExpressionSyntax Placeholder => LuaIdentifierNameSyntax.Placeholder;

    public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
      OpenBraceToken = Tokens.Do,
      CloseBraceToken = Tokens.End,
    };

    public LuaForInStatementSyntax(LuaIdentifierNameSyntax identifier, LuaExpressionSyntax expression) {
      if (identifier == null) {
        throw new ArgumentNullException(nameof(identifier));
      }
      if (expression == null) {
        throw new ArgumentNullException(nameof(expression));
      }
      Identifier = identifier;
      Expression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Each, expression);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaNumericalForStatementSyntax : LuaStatementSyntax {
    public string ForKeyword => Tokens.For;
    public LuaIdentifierNameSyntax Identifier { get; }
    public string EqualsToken => Tokens.Equals;
    public LuaExpressionSyntax StartExpression { get; }
    public LuaExpressionSyntax LimitExpression { get; }
    public LuaExpressionSyntax StepExpression { get; }

    public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
      OpenBraceToken = Tokens.Do,
      CloseBraceToken = Tokens.End,
    };

    public LuaNumericalForStatementSyntax(
      LuaIdentifierNameSyntax identifier,
      LuaExpressionSyntax startExpression,
      LuaExpressionSyntax limitExpression,
      LuaExpressionSyntax stepExpression) {
      if (identifier == null) {
        throw new ArgumentNullException(nameof(identifier));
      }
      if (startExpression == null) {
        throw new ArgumentNullException(nameof(startExpression));
      }
      if (limitExpression == null) {
        throw new ArgumentNullException(nameof(limitExpression));
      }
      Identifier = identifier;
      StartExpression = startExpression;
      LimitExpression = limitExpression;
      StepExpression = stepExpression;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaWhileStatementSyntax : LuaStatementSyntax {
    public LuaExpressionSyntax Condition { get; }
    public string WhileKeyword => LuaSyntaxNode.Tokens.While;

    public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
      OpenBraceToken = Tokens.Do,
      CloseBraceToken = Tokens.End,
    };

    public LuaWhileStatementSyntax(LuaExpressionSyntax condition) {
      if (condition == null) {
        throw new ArgumentNullException(nameof(condition));
      }
      Condition = condition;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaRepeatStatementSyntax : LuaStatementSyntax {
    public LuaExpressionSyntax Condition { get; }
    public string RepeatKeyword => Tokens.Repeat;
    public string UntilKeyword => Tokens.Until;
    public LuaBlockSyntax Body { get; } = new LuaBlockSyntax();

    public LuaRepeatStatementSyntax(LuaExpressionSyntax condition) {
      if (condition == null) {
        throw new ArgumentNullException(nameof(condition));
      }
      Condition = condition;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }
}
