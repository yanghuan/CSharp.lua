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
  public sealed class LuaTableExpression : LuaExpressionSyntax {
    public string OpenBraceToken => Tokens.OpenBrace;
    public readonly LuaSyntaxList<LuaTableItemSyntax> Items = new LuaSyntaxList<LuaTableItemSyntax>();
    public string CloseBraceToken => Tokens.CloseBrace;
    public bool IsSingleLine { get; set; }

    public LuaTableExpression() {
    }

    public LuaTableExpression(IEnumerable<LuaExpressionSyntax> expressions) {
      Items.AddRange(expressions.Select(i => new LuaSingleTableItemSyntax(i)));
    }

    internal void Add(LuaIdentifierNameSyntax key, LuaExpressionSyntax value) {
      Items.Add(new LuaKeyValueTableItemSyntax(key, value));
    }

    internal void Add(LuaExpressionSyntax value) {
      Items.Add(new LuaSingleTableItemSyntax(value));
    }

    internal void AddRange(IEnumerable<LuaExpressionSyntax> values) {
      Items.AddRange(values.Select(i => new LuaSingleTableItemSyntax(i)));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }

    public static readonly LuaTableExpression Empty = new LuaTableExpression();

    public LuaExpressionSyntax GetSingleExpression(int index) {
      var item = (LuaSingleTableItemSyntax)Items[index];
      return item.Expression;
    }
  }

  public abstract class LuaTableItemSyntax : LuaSyntaxNode {
  }

  public sealed class LuaSingleTableItemSyntax : LuaTableItemSyntax {
    public LuaExpressionSyntax Expression { get; }

    public LuaSingleTableItemSyntax(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public abstract class LuaTableKeySyntax : LuaSyntaxNode {
  }

  public sealed class LuaTableExpressionKeySyntax : LuaTableKeySyntax {
    public LuaExpressionSyntax Expression { get; }
    public string OpenBracketToken => Tokens.OpenBracket;
    public string CloseBracketToken => Tokens.CloseBracket;

    public LuaTableExpressionKeySyntax(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaTableLiteralKeySyntax : LuaTableKeySyntax {
    public LuaIdentifierNameSyntax Identifier { get; }

    public LuaTableLiteralKeySyntax(LuaIdentifierNameSyntax identifier) {
      Identifier = identifier;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaKeyValueTableItemSyntax : LuaTableItemSyntax {
    public LuaTableKeySyntax Key { get; }
    public string OperatorToken => Tokens.Equals;
    public LuaExpressionSyntax Value { get; }

    public LuaKeyValueTableItemSyntax(LuaTableKeySyntax key, LuaExpressionSyntax value) {
      Key = key;
      Value = value;
    }

    public LuaKeyValueTableItemSyntax(LuaIdentifierNameSyntax key, LuaExpressionSyntax value) : this(new LuaTableLiteralKeySyntax(key), value) {
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaTableIndexAccessExpressionSyntax : LuaExpressionSyntax {
    public LuaExpressionSyntax Expression { get; }
    public LuaExpressionSyntax Index { get; }
    public string OpenBracketToken => Tokens.OpenBracket;
    public string CloseBracketToken => Tokens.CloseBracket;

    public LuaTableIndexAccessExpressionSyntax(LuaExpressionSyntax expression, LuaExpressionSyntax index) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
      Index = index ?? throw new ArgumentNullException(nameof(index));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }
}
