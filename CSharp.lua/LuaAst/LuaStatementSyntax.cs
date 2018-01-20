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
  public abstract class LuaStatementSyntax : LuaSyntaxNode {
    public Semicolon SemicolonToken => Tokens.Semicolon;

    private sealed class EmptyLuaStatementSyntax : LuaStatementSyntax {
      internal override void Render(LuaRenderer renderer) {
      }
    }
    public readonly static LuaStatementSyntax Empty = new EmptyLuaStatementSyntax();
  }

  public sealed class LuaExpressionStatementSyntax : LuaStatementSyntax {
    public LuaExpressionSyntax Expression { get; }

    public LuaExpressionStatementSyntax(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaStatementListSyntax : LuaStatementSyntax {
    public readonly LuaSyntaxList<LuaStatementSyntax> Statements = new LuaSyntaxList<LuaStatementSyntax>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaReturnStatementSyntax : LuaStatementSyntax {
    public LuaExpressionSyntax Expression { get; }
    public string ReturnKeyword => Tokens.Return;

    public LuaReturnStatementSyntax(LuaExpressionSyntax expression = null) {
      Expression = expression;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaMultipleReturnStatementSyntax : LuaStatementSyntax {
    public LuaSyntaxList<LuaExpressionSyntax> Expressions { get; } = new LuaSyntaxList<LuaExpressionSyntax>();
    public string ReturnKeyword => Tokens.Return;

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaBreakStatementSyntax : LuaStatementSyntax {
    public string BreakKeyword => Tokens.Break;

    private LuaBreakStatementSyntax() { }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }

    public static readonly LuaBreakStatementSyntax Statement = new LuaBreakStatementSyntax();
  }

  public sealed class LuaContinueAdapterStatementSyntax : LuaStatementSyntax {
    public LuaExpressionStatementSyntax Assignment { get; }
    public LuaBreakStatementSyntax Break => LuaBreakStatementSyntax.Statement;

    private LuaContinueAdapterStatementSyntax() {
      Assignment = new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(LuaIdentifierNameSyntax.Continue, LuaIdentifierNameSyntax.True));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }

    public static readonly LuaContinueAdapterStatementSyntax Statement = new LuaContinueAdapterStatementSyntax();
  }

  public sealed class LuaBlankLinesStatement : LuaStatementSyntax {
    public int Count { get; }

    public LuaBlankLinesStatement(int count) {
      Count = count;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }

    public static readonly LuaBlankLinesStatement One = new LuaBlankLinesStatement(1);
  }

  public sealed class LuaShortCommentStatement : LuaStatementSyntax {
    public string SingleCommentToken => Tokens.ShortComment;
    public string Comment { get; }

    public LuaShortCommentStatement(string comment) {
      Comment = comment;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public class LuaShortCommentExpressionStatement : LuaStatementSyntax {
    public string SingleCommentToken => Tokens.ShortComment;
    public LuaExpressionSyntax Expression { get; }

    public LuaShortCommentExpressionStatement(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLongCommentStatement : LuaShortCommentExpressionStatement {
    public LuaLongCommentStatement(string comment) : base(new LuaVerbatimStringLiteralExpressionSyntax(comment, false)) {
    }
  }

  public sealed class LuaGotoStatement : LuaStatementSyntax {
    public LuaIdentifierNameSyntax Identifier { get; }
    public string GotoKeyword => Tokens.Goto;

    public LuaGotoStatement(LuaIdentifierNameSyntax identifier) {
      Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaGotoCaseAdapterStatement : LuaStatementSyntax {
    public LuaStatementSyntax Assignment { get; }
    public LuaGotoStatement GotoStatement { get; }

    public LuaGotoCaseAdapterStatement(LuaIdentifierNameSyntax identifier) {
      if (identifier == null) {
        throw new ArgumentNullException(nameof(identifier));
      }

      LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(identifier, LuaIdentifierNameSyntax.True);
      Assignment = new LuaExpressionStatementSyntax(assignment);
      GotoStatement = new LuaGotoStatement(identifier);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLabeledStatement : LuaStatementSyntax {
    public string PrefixToken => Tokens.Label;
    public string SuffixToken => Tokens.Label;
    public LuaIdentifierNameSyntax Identifier { get; }
    public LuaStatementSyntax Statement { get; }

    public LuaLabeledStatement(LuaIdentifierNameSyntax identifier, LuaStatementSyntax statement = null) {
      Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
      Statement = statement;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }
}
