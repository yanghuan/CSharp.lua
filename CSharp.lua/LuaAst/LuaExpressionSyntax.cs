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
  public abstract class LuaExpressionSyntax : LuaSyntaxNode {
    private sealed class EmptyLuaExpressionSyntax : LuaExpressionSyntax {
      internal override void Render(LuaRenderer renderer) {
      }
    }

    public static implicit operator LuaExpressionSyntax(string valueText) {
      LuaIdentifierNameSyntax identifierName = valueText;
      return identifierName;
    }

    public static implicit operator LuaExpressionSyntax(double number) {
      LuaNumberLiteralExpressionSyntax numberLiteral = number;
      return numberLiteral;
    }

    public static readonly LuaExpressionSyntax EmptyExpression = new EmptyLuaExpressionSyntax();

    public LuaBinaryExpressionSyntax And(LuaExpressionSyntax right) {
      return new LuaBinaryExpressionSyntax(this, Tokens.And, right);
    }

    public LuaBinaryExpressionSyntax Or(LuaExpressionSyntax right) {
      return new LuaBinaryExpressionSyntax(this, Tokens.Or, right);
    }
  }

  public sealed class LuaAssignmentExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax Left { get; }
    public string OperatorToken => Tokens.Equals;
    public LuaExpressionSyntax Right { get; }

    public LuaAssignmentExpressionSyntax(LuaExpressionSyntax left, LuaExpressionSyntax right) {
      Left = left ?? throw new ArgumentNullException(nameof(left));
      Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaMultipleAssignmentExpressionSyntax : LuaExpressionSyntax {
    public LuaSyntaxList<LuaExpressionSyntax> Lefts { get; } = new LuaSyntaxList<LuaExpressionSyntax>();
    public string OperatorToken => Tokens.Equals;
    public LuaSyntaxList<LuaExpressionSyntax> Rights { get; } = new LuaSyntaxList<LuaExpressionSyntax>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLineMultipleExpressionSyntax : LuaExpressionSyntax {
    public LuaSyntaxList<LuaExpressionSyntax> Assignments { get; } = new LuaSyntaxList<LuaExpressionSyntax>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaBinaryExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax Left { get; }
    public string OperatorToken { get; }
    public LuaExpressionSyntax Right { get; }

    public LuaBinaryExpressionSyntax(LuaExpressionSyntax left, string operatorToken, LuaExpressionSyntax right) {
      Left = left ?? throw new ArgumentNullException(nameof(left));
      OperatorToken = operatorToken;
      Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaPrefixUnaryExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax Operand { get; }
    public string OperatorToken { get; }

    public LuaPrefixUnaryExpressionSyntax(LuaExpressionSyntax operand, string operatorToken) {
      Operand = operand ?? throw new ArgumentNullException(nameof(operand));
      OperatorToken = operatorToken;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaParenthesizedExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax Expression { get; }
    public string OpenParenToken => Tokens.OpenParentheses;
    public string CloseParenToken => Tokens.CloseParentheses;

    public LuaParenthesizedExpressionSyntax(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaCodeTemplateExpressionSyntax : LuaExpressionSyntax {
    public readonly LuaSyntaxList<LuaExpressionSyntax> Expressions = new LuaSyntaxList<LuaExpressionSyntax>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaArrayRankSpecifierSyntax : LuaSyntaxNode {
    public int Rank { get; }
    public readonly List<LuaExpressionSyntax> Sizes = new List<LuaExpressionSyntax>();

    public LuaArrayRankSpecifierSyntax(int rank) {
      Rank = rank;
    }
  }

  public sealed class LuaArrayTypeAdapterExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax TypeExpression { get; }
    public LuaArrayRankSpecifierSyntax RankSpecifier { get; }

    public LuaArrayTypeAdapterExpressionSyntax(LuaExpressionSyntax typeExpression, LuaArrayRankSpecifierSyntax rankSpecifier) {
      TypeExpression = typeExpression ?? throw new ArgumentNullException(nameof(typeExpression));
      RankSpecifier = rankSpecifier ?? throw new ArgumentNullException(nameof(rankSpecifier));
    }

    public bool IsSimapleArray {
      get {
        return RankSpecifier.Rank == 1;
      }
    }

    internal override void Render(LuaRenderer renderer) {
      TypeExpression.Render(renderer);
    }
  }

  public sealed class LuaInternalMethodExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax Expression { get; }

    public LuaInternalMethodExpressionSyntax(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    internal override void Render(LuaRenderer renderer) {
      Expression.Render(renderer);
    }
  }

  public sealed class LuaSequenceListExpressionSyntax : LuaExpressionSyntax {
    public readonly LuaSyntaxList<LuaExpressionSyntax> Expressions = new LuaSyntaxList<LuaExpressionSyntax>();

    public LuaSequenceListExpressionSyntax() {
    }

    public LuaSequenceListExpressionSyntax(IEnumerable<LuaExpressionSyntax> expressions) {
      Expressions.AddRange(expressions);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }
}
