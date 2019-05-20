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
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
  public sealed class LuaLocalVariablesStatementSyntax : LuaVariableDeclarationSyntax {
    public string LocalKeyword => Tokens.Local;
    public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Variables = new LuaSyntaxList<LuaIdentifierNameSyntax>();
    public LuaEqualsValueClauseListSyntax Initializer { get; set; }

    public LuaLocalVariablesStatementSyntax() {
    }

    public LuaLocalVariablesStatementSyntax(IEnumerable<LuaIdentifierNameSyntax> variables, IEnumerable<LuaExpressionSyntax> valuse = null) {
      Variables.AddRange(variables);
      if (valuse != null) {
        Initializer = new LuaEqualsValueClauseListSyntax(valuse);
      }
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaEqualsValueClauseListSyntax : LuaSyntaxNode {
    public string EqualsToken => Tokens.Equals;
    public readonly LuaSyntaxList<LuaExpressionSyntax> Values = new LuaSyntaxList<LuaExpressionSyntax>();

    public LuaEqualsValueClauseListSyntax() {

    }

    public LuaEqualsValueClauseListSyntax(IEnumerable<LuaExpressionSyntax> values) {
      Values.AddRange(values);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalDeclarationStatementSyntax : LuaStatementSyntax {
    public LuaVariableDeclarationSyntax Declaration { get; }

    public LuaLocalDeclarationStatementSyntax(LuaVariableDeclarationSyntax declaration) {
      Declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public abstract class LuaVariableDeclarationSyntax : LuaStatementSyntax {
  }

  public sealed class LuaVariableListDeclarationSyntax : LuaVariableDeclarationSyntax {
    public readonly LuaSyntaxList<LuaVariableDeclaratorSyntax> Variables = new LuaSyntaxList<LuaVariableDeclaratorSyntax>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalVariableDeclaratorSyntax : LuaStatementSyntax {
    public LuaVariableDeclaratorSyntax Declarator { get; }

    public LuaLocalVariableDeclaratorSyntax(LuaVariableDeclaratorSyntax declarator) {
      Declarator = declarator ?? throw new ArgumentNullException(nameof(declarator));
    }

    public LuaLocalVariableDeclaratorSyntax(LuaIdentifierNameSyntax identifier, LuaExpressionSyntax expression = null) {
      Declarator = new LuaVariableDeclaratorSyntax(identifier, expression);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaVariableDeclaratorSyntax : LuaStatementSyntax {
    public string LocalKeyword => Tokens.Local;
    public LuaIdentifierNameSyntax Identifier { get; }
    public LuaEqualsValueClauseSyntax Initializer { get; set; }

    public LuaVariableDeclaratorSyntax(LuaIdentifierNameSyntax identifier, LuaExpressionSyntax expression = null) {
      Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
      if (expression != null) {
        Initializer = new LuaEqualsValueClauseSyntax(expression);
      }
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaEqualsValueClauseSyntax : LuaSyntaxNode {
    public string EqualsToken => Tokens.Equals;
    public LuaExpressionSyntax Value { get; }

    public LuaEqualsValueClauseSyntax(LuaExpressionSyntax value) {
      Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalAreaSyntax : LuaStatementSyntax {
    public string LocalKeyword => Tokens.Local;
    public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Variables = new LuaSyntaxList<LuaIdentifierNameSyntax>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalFunctionSyntx : LuaStatementSyntax {
    public readonly LuaStatementListSyntax Comments = new LuaStatementListSyntax();
    public string LocalKeyword => Tokens.Local;
    public LuaIdentifierNameSyntax IdentifierName { get; }
    public LuaFunctionExpressionSyntax FunctionExpression { get; }

    public LuaLocalFunctionSyntx(LuaIdentifierNameSyntax identifierName, LuaFunctionExpressionSyntax functionExpression, LuaDocumentStatement documentation = null) {
      IdentifierName = identifierName ?? throw new ArgumentNullException(nameof(identifierName));
      FunctionExpression = functionExpression ?? throw new ArgumentNullException(nameof(functionExpression));
      if (documentation != null) {
        Comments.Statements.Add(documentation);
      }
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalTupleVariableExpression : LuaExpressionSyntax {
    public string LocalKeyword => Tokens.Local;
    public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Variables = new LuaSyntaxList<LuaIdentifierNameSyntax>();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }

    public LuaLocalTupleVariableExpression() {
    }

    public LuaLocalTupleVariableExpression(IEnumerable<LuaIdentifierNameSyntax> variables) {
      Variables.AddRange(variables);
    }
  }
}
