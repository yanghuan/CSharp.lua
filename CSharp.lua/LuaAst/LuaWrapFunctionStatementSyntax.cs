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

using System.Collections.Generic;

namespace CSharpLua.LuaAst {
  public abstract class LuaWrapFunctionStatementSyntax : LuaStatementSyntax {
    public LuaExpressionStatementSyntax Statement { get; private set; }
    private readonly LuaFunctionExpressionSyntax function_ = new();

    protected void UpdateIdentifiers(LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax target, LuaIdentifierNameSyntax memberName, LuaIdentifierNameSyntax parameter = null) {
      var invoke = target.MemberAccess(memberName).Invocation();
      invoke.AddArgument(new LuaStringLiteralExpressionSyntax(name));
      invoke.AddArgument(function_);
      if (parameter != null) {
        function_.AddParameter(parameter);
      }
      Statement = new LuaExpressionStatementSyntax(invoke);
    }

    public LuaBlockSyntax Body {
      get {
        return function_.Body;
      }
    }

    public void AddGlobalParameter() {
      if (function_.ParameterList.Count == 1) {
        function_.ParameterList.Parameters.Add(LuaIdentifierNameSyntax.Global);
      }
    }

    public void AddStatement(LuaStatementSyntax statement) {
      Body.Statements.Add(statement);
    }

    public void AddStatements(IEnumerable<LuaStatementSyntax> statements) {
      Body.Statements.AddRange(statements);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaNamespaceDeclarationSyntax : LuaWrapFunctionStatementSyntax {
    public LuaNamespaceDeclarationSyntax(LuaIdentifierNameSyntax name, bool isContained = false) {
      UpdateIdentifiers(name, isContained ? LuaIdentifierNameSyntax.Namespace : LuaIdentifierNameSyntax.System, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Namespace);
    }
  }
}
