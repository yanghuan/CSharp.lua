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
  public abstract class LuaStatementSyntax : LuaSyntaxNode {
    public Semicolon SemicolonToken => Tokens.Semicolon;
    public bool ForceSemicolon { get; set; }

    private sealed class EmptyLuaStatementSyntax : LuaStatementSyntax {
      internal override void Render(LuaRenderer renderer) {
      }
    }

    public static implicit operator LuaStatementSyntax(LuaExpressionSyntax expression) {
      return new LuaExpressionStatementSyntax(expression);
    }

    public static readonly LuaStatementSyntax Empty = new EmptyLuaStatementSyntax();
  }

  public sealed class LuaExpressionStatementSyntax : LuaStatementSyntax {
    public LuaExpressionSyntax Expression { get; }

    public LuaExpressionStatementSyntax(LuaExpressionSyntax expression) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }

    public static implicit operator LuaExpressionStatementSyntax(LuaExpressionSyntax expression) {
      return new LuaExpressionStatementSyntax(expression);
    }
  }

  public sealed class LuaStatementListSyntax : LuaStatementSyntax {
    public readonly LuaSyntaxList<LuaStatementSyntax> Statements = new LuaSyntaxList<LuaStatementSyntax>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }
  
  public abstract class LuaBaseReturnStatementSyntax : LuaStatementSyntax {
    public string ReturnKeyword => Tokens.Return;
  }

  public sealed class LuaReturnStatementSyntax : LuaBaseReturnStatementSyntax {
    public readonly LuaSyntaxList<LuaExpressionSyntax> Expressions = new LuaSyntaxList<LuaExpressionSyntax>();

    public LuaReturnStatementSyntax() {
    }

    public LuaReturnStatementSyntax(LuaExpressionSyntax expression) {
      if (expression != null) {
        Expressions.Add(expression);
      }
    }

    public LuaReturnStatementSyntax(IEnumerable<LuaExpressionSyntax> expressions) {
      Expressions.AddRange(expressions);
    }

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

    public static readonly LuaBreakStatementSyntax Instance = new LuaBreakStatementSyntax();
  }

  public sealed class LuaContinueAdapterStatementSyntax : LuaStatementSyntax {
    public LuaExpressionStatementSyntax Assignment { get; }
    public LuaStatementSyntax Statement { get; }

    public LuaContinueAdapterStatementSyntax(bool isWithinTry) {
      Assignment = LuaIdentifierNameSyntax.Continue.Assignment(LuaIdentifierNameSyntax.True);
      if (isWithinTry) {
        Statement = new LuaReturnStatementSyntax(LuaIdentifierLiteralExpressionSyntax.False);
      } else {
        Statement = LuaBreakStatementSyntax.Instance;
      }
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
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

  public abstract class LuaCommentStatement : LuaStatementSyntax {
  }

  public sealed class LuaShortCommentStatement : LuaCommentStatement {
    public string SingleCommentToken => Tokens.ShortComment;
    public string Comment { get; }

    public LuaShortCommentStatement(string comment) {
      Comment = comment;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public class LuaShortCommentExpressionStatement : LuaCommentStatement {
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
      Assignment = identifier.Assignment(LuaIdentifierNameSyntax.True);
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

  public sealed class LuaDocumentStatement : LuaStatementSyntax {
    private const string kAttributePrefix = "@CSharpLua.";

    [Flags]
    public enum AttributeFlags {
      None = 0,
      Ignore = 1 << 0,
      NoField = 1 << 1,
      Metadata = 1 << 2,
      MetadataAll = 1 << 3,
      Template = 1 << 4,
      Params = 1 << 5,
    }

    public readonly List<LuaStatementSyntax> Statements = new List<LuaStatementSyntax>();
    public bool IsEmpty => Statements.Count == 0;
    private AttributeFlags attr_;
    public bool HasIgnoreAttribute => HasAttribute(AttributeFlags.Ignore);
    public bool HasMetadataAttribute => HasAttribute(AttributeFlags.Metadata);
    public bool HasMetadataAllAttribute => HasAttribute(AttributeFlags.MetadataAll);

    public LuaDocumentStatement() {
    }

    public LuaDocumentStatement(string triviaText) {
      var items = triviaText.Replace("///", string.Empty)
        .Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
        .Select(i => i.Trim()).ToList();

      var document = new LuaSummaryDocumentStatement();
      foreach (var item in items) {
        if (IsAttribute(item, out AttributeFlags arrt)) {
          attr_ |= arrt;
        } else {
          document.Texts.Add(item);
        }
      }
      if (!IsEmptyDocument(document)) {
        Statements.Add(document);
      }
    }

    private bool IsEmptyDocument(LuaSummaryDocumentStatement document) {
      if (document.Texts.Count == 0) {
        return true;
      }

      if (attr_ != AttributeFlags.None && document.Texts.Count == 2) {
        return document.Texts[0] == "<summary>" && document.Texts[1] == "</summary>";
      }

      return false;
    }

    private static bool IsAttribute(string text, out AttributeFlags attr) {
      attr = AttributeFlags.None;
      int index = text.IndexOf(kAttributePrefix);
      if (index != -1) {
        string prefix = text.Substring(index + kAttributePrefix.Length);
        if (Enum.TryParse(prefix, out attr)) {
          return true;
        } else if (prefix.Contains(AttributeFlags.Template.ToString())) {
          attr = AttributeFlags.Template;
          return true;
        } else {
          throw new CompilationErrorException($"{prefix} is not define attribute");
        }
      }
      return false;
    }

    public void Add(LuaDocumentStatement document) {
      Statements.AddRange(document.Statements);
      attr_ |= document.attr_;
    }

    public bool HasAttribute(AttributeFlags type) {
      return attr_.HasFlag(type);
    }

    private void UnAttribute(AttributeFlags type) {
      attr_ &= ~type;
    }

    public void UnIgnore() {
      UnAttribute(AttributeFlags.Ignore);
    }

    public static string ToString(AttributeFlags attribute) {
      return kAttributePrefix + attribute.ToString();
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaSummaryDocumentStatement : LuaStatementSyntax {
    public readonly List<string> Texts = new List<string>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLineDocumentStatement : LuaStatementSyntax {
    public string Text { get; }

    public LuaLineDocumentStatement(string text) {
      Text = text;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

}
