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

namespace CSharpLua.LuaAst {
  public sealed class LuaIfStatementSyntax : LuaStatementSyntax {
    public string IfKeyword => Keyword.If;
    public LuaExpressionSyntax Condition { get; }
    public string OpenParenToken => Keyword.Then;
    public readonly LuaBlockSyntax Body = new();
    public readonly LuaSyntaxList<LuaElseIfStatementSyntax> ElseIfStatements = new();
    public LuaElseClauseSyntax Else { get; set; }
    public string CloseParenToken => Keyword.End;

    public LuaIfStatementSyntax(LuaExpressionSyntax condition) {
      Condition = condition ?? throw new ArgumentNullException(nameof(condition));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaElseIfStatementSyntax : LuaStatementSyntax {
    public string ElseIfKeyword => Keyword.ElseIf;
    public LuaExpressionSyntax Condition { get; }
    public string OpenParenToken => Keyword.Then;
    public readonly LuaBlockSyntax Body = new();

    public LuaElseIfStatementSyntax(LuaExpressionSyntax condition) {
      Condition = condition ?? throw new ArgumentNullException(nameof(condition)); ;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaElseClauseSyntax : LuaSyntaxNode {
    public string ElseKeyword => Keyword.Else;
    public readonly LuaBlockSyntax Body = new();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaSwitchAdapterStatementSyntax : LuaStatementSyntax {
    public readonly LuaRepeatStatementSyntax RepeatStatement = new(LuaIdentifierNameSyntax.One);
    public LuaIdentifierNameSyntax Temp { get; }
    private LuaBlockSyntax defaultBlock_;
    private readonly LuaLocalVariablesSyntax caseLabelVariables_ = new();
    public LuaIdentifierNameSyntax DefaultLabel { get; set; }
    public readonly Dictionary<int, LuaIdentifierNameSyntax> CaseLabels = new();
    private LuaIfStatementSyntax headIfStatement_;

    public LuaSwitchAdapterStatementSyntax(LuaIdentifierNameSyntax temp) {
      Temp = temp;
    }

    public void Fill(LuaExpressionSyntax expression, IEnumerable<LuaStatementSyntax> sections) {
      if (expression == null) {
        throw new ArgumentNullException(nameof(expression));
      }
      if (sections == null) {
        throw new ArgumentNullException(nameof(sections));
      }

      var body = RepeatStatement.Body;
      body.Statements.Add(caseLabelVariables_);
      body.Statements.Add(new LuaLocalVariableDeclaratorSyntax(Temp, expression));

      LuaIfStatementSyntax ifStatement = null;
      foreach (var section in sections) {
        if (section is LuaIfStatementSyntax statement) {
          if (ifStatement == null) {
            ifStatement = statement;
          } else {
            LuaElseIfStatementSyntax elseIfStatement = new LuaElseIfStatementSyntax(statement.Condition);
            elseIfStatement.Body.Statements.AddRange(statement.Body.Statements);
            ifStatement.ElseIfStatements.Add(elseIfStatement);
          }
        } else {
          Contract.Assert(defaultBlock_ == null);
          defaultBlock_ = (LuaBlockSyntax)section;
        }
      }

      if (ifStatement != null) {
        body.Statements.Add(ifStatement);
        if (defaultBlock_ != null) {
          LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
          elseClause.Body.Statements.AddRange(defaultBlock_.Statements);
          ifStatement.Else = elseClause;
        }
        headIfStatement_ = ifStatement;
      } else {
        if (defaultBlock_ != null) {
          body.Statements.AddRange(defaultBlock_.Statements);
        }
      }
    }

    private void CheckHasDefaultLabel() {
      if (DefaultLabel != null) {
        Contract.Assert(defaultBlock_ != null);
        caseLabelVariables_.Variables.Add(DefaultLabel);
        LuaLabeledStatement labeledStatement = new LuaLabeledStatement(DefaultLabel);
        RepeatStatement.Body.Statements.Add(labeledStatement);
        LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(DefaultLabel);
        ifStatement.Body.Statements.AddRange(defaultBlock_.Statements);
        RepeatStatement.Body.Statements.Add(ifStatement);
      }
    }

    private LuaBlockSyntax FindMatchIfStatement(int index) {
      if (index == 0) {
        return headIfStatement_.Body;
      }

      return headIfStatement_.ElseIfStatements[index - 1].Body;
    }

    private void CheckHasCaseLabel() {
      if (CaseLabels.Count > 0) {
        Contract.Assert(headIfStatement_ != null);
        caseLabelVariables_.Variables.AddRange(CaseLabels.Values);
        foreach (var (index, labelIdentifier) in CaseLabels) {
          var caseLabelStatement = FindMatchIfStatement(index);
          RepeatStatement.Body.Statements.Add(new LuaLabeledStatement(labelIdentifier));
          LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(labelIdentifier);
          ifStatement.Body.Statements.AddRange(caseLabelStatement.Statements);
          RepeatStatement.Body.Statements.Add(ifStatement);
        }
      }
    }

    internal override void Render(LuaRenderer renderer) {
      CheckHasCaseLabel();
      CheckHasDefaultLabel();
      renderer.Render(this);
    }
  }

}
