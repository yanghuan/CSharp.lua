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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed partial class LuaSyntaxNodeTransfor {
    private sealed class QueryRangeVariable {
      public SyntaxToken Identifier;
      public LuaIdentifierNameSyntax Name;
      public bool HasPlaceholder;

      public LuaIdentifierNameSyntax GetTrueName() {
        return HasPlaceholder ? LuaIdentifierNameSyntax.Placeholder : Name;
      }

      public LuaExpressionSyntax GetIdentifierName() {
        if (HasPlaceholder) {
          return new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.Placeholder, Name);
        }
        return Name;
      }
    }

    private List<QueryRangeVariable> queryRangeVariables_ = new List<QueryRangeVariable>();

    private QueryRangeVariable AddRangeVariable(SyntaxToken identifier) {
      string name = identifier.ValueText;
      CheckLocalReservedWord(ref name, identifier.Parent);
      var queryRangeVariable = new QueryRangeVariable {
        Identifier = identifier,
        Name = new LuaIdentifierNameSyntax(name),
      };
      queryRangeVariables_.Add(queryRangeVariable);
      return queryRangeVariable;
    }

    private LuaExpressionSyntax GetRangeVariableIdentifierName(IdentifierNameSyntax name) {
      var info = queryRangeVariables_.Find(i => i.Identifier.ValueText == name.Identifier.ValueText);
      Contract.Assert(info != null);
      return info.GetIdentifierName();
    }

    public override LuaSyntaxNode VisitQueryExpression(QueryExpressionSyntax node) {
      CurCompilationUnit.ImportLinq();

      var rangeVariable = AddRangeVariable(node.FromClause.Identifier);
      var collection = (LuaExpressionSyntax)node.FromClause.Accept(this);
      var queryExpression = BuildQueryBody(collection, node.Body, rangeVariable);
      queryRangeVariables_.Clear();
      return queryExpression;
    }

    public override LuaSyntaxNode VisitFromClause(FromClauseSyntax node) {
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      if (node.Type != null) {
        var typeName = (LuaExpressionSyntax)node.Type.Accept(this);
        expression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqCast, expression, typeName);
      }
      return expression;
    }

    public override LuaSyntaxNode VisitWhereClause(WhereClauseSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitQueryBody(QueryBodySyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitSelectClause(SelectClauseSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitQueryContinuation(QueryContinuationSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitLetClause(LetClauseSyntax node) {
      throw new InvalidOperationException();
    }

    private LuaExpressionSyntax BuildQueryWhere(LuaExpressionSyntax collection, WhereClauseSyntax node, QueryRangeVariable rangeVariable) {
      var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
      if (condition == LuaIdentifierLiteralExpressionSyntax.True) {
        return collection;
      }

      var whereFunction = new LuaFunctionExpressionSyntax();
      whereFunction.AddParameter(rangeVariable.GetTrueName());
      whereFunction.AddStatement(new LuaReturnStatementSyntax(condition));
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqWhere, collection, whereFunction);
    }

    private LuaExpressionSyntax BuildOrdering(LuaIdentifierNameSyntax methodName, LuaExpressionSyntax collection, OrderingSyntax node, QueryRangeVariable rangeVariable) {
      var type = semanticModel_.GetTypeInfo(node.Expression).Type;
      var typeName = GetTypeName(type);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      var keySelector = new LuaFunctionExpressionSyntax();
      keySelector.AddParameter(rangeVariable.GetTrueName());
      keySelector.AddStatement(new LuaReturnStatementSyntax(expression));
      return new LuaInvocationExpressionSyntax(methodName, collection, keySelector, LuaIdentifierNameSyntax.Nil, typeName);
    }

    private LuaExpressionSyntax BuildQueryOrderBy(LuaExpressionSyntax collection, OrderByClauseSyntax node, QueryRangeVariable rangeVariable) {
      foreach (var ordering in node.Orderings) {
        bool isDescending = ordering.AscendingOrDescendingKeyword.IsKind(SyntaxKind.DescendingKeyword);
        if (ordering == node.Orderings.First()) {
          var methodName = isDescending ? LuaIdentifierNameSyntax.LinqOrderByDescending : LuaIdentifierNameSyntax.LinqOrderBy;
          collection = BuildOrdering(methodName, collection, ordering, rangeVariable);
        }
        else {
          var methodName = isDescending ? LuaIdentifierNameSyntax.LinqThenByDescending : LuaIdentifierNameSyntax.LinqThenBy;
          collection = BuildOrdering(methodName, collection, ordering, rangeVariable);
        }
      }
      return collection;
    }

    private LuaExpressionSyntax BuildQuerySelect(LuaExpressionSyntax collection, SelectClauseSyntax node, QueryRangeVariable rangeVariable) {
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      if (node.Expression.IsKind(SyntaxKind.IdentifierName)) {
        var identifierName = expression as LuaIdentifierNameSyntax;
        if (identifierName != null && identifierName.ValueText == rangeVariable.Identifier.ValueText) {
          return collection;
        }
      }

      var selectFunction = new LuaFunctionExpressionSyntax();
      selectFunction.AddParameter(rangeVariable.GetTrueName());
      selectFunction.AddStatement(new LuaReturnStatementSyntax(expression));
      var type = semanticModel_.GetTypeInfo(node.Expression).Type;
      var typeExpression = GetTypeName(type);
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqSelect, collection, selectFunction, typeExpression);
    }

    private LuaExpressionSyntax BuildGroupClause(LuaExpressionSyntax collection, GroupClauseSyntax node, QueryRangeVariable rangeVariable) {
      var keyType = semanticModel_.GetTypeInfo(node.ByExpression).Type;
      var keyTypeName = GetTypeName(keyType);
      var byExpression = (LuaExpressionSyntax)node.ByExpression.Accept(this);
      var keySelector = new LuaFunctionExpressionSyntax();
      keySelector.AddParameter(rangeVariable.GetTrueName());
      keySelector.AddStatement(new LuaReturnStatementSyntax(byExpression));

      var groupExpression = (LuaExpressionSyntax)node.GroupExpression.Accept(this);
      if (node.GroupExpression.IsKind(SyntaxKind.IdentifierName)) {
        var groupIdentifierName = groupExpression as LuaIdentifierNameSyntax;
        if (groupIdentifierName != null && groupIdentifierName.ValueText == rangeVariable.Identifier.ValueText) {
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqGroupBy, collection, keySelector, keyTypeName);
        }
      }

      var elementType = semanticModel_.GetTypeInfo(node.GroupExpression).Type;
      var elementTypeName = GetTypeName(elementType);
      var elementSelector = new LuaFunctionExpressionSyntax();
      elementSelector.AddParameter(rangeVariable.GetTrueName());
      elementSelector.AddStatement(new LuaReturnStatementSyntax(groupExpression));
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqGroupBy, collection, keySelector, elementSelector, keyTypeName, elementTypeName);
    }

    private LuaInvocationExpressionSyntax CreateQueryAnonymousType(LuaIdentifierNameSyntax key1, LuaExpressionSyntax value1, LuaIdentifierNameSyntax key2, LuaExpressionSyntax value2) {
      LuaTableInitializerExpression table = new LuaTableInitializerExpression();
      table.Items.Add(new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(key1), value1));
      table.Items.Add(new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(key2), value2));
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.AnonymousTypeCreate, table);
    }

    private LuaExpressionSyntax BuildFromClause(LuaExpressionSyntax collection, FromClauseSyntax node, QueryRangeVariable rangeVariable, out bool isOver) {
      var collectionSelector = new LuaFunctionExpressionSyntax();
      collectionSelector.AddParameter(rangeVariable.Name);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      collectionSelector.AddStatement(new LuaReturnStatementSyntax(expression));

      var rangeVariable2 = AddRangeVariable(node.Identifier);
      var resultSelector = new LuaFunctionExpressionSyntax();
      resultSelector.AddParameter(rangeVariable.Name);
      resultSelector.AddParameter(rangeVariable2.Name);
      LuaExpressionSyntax resultSelectorExpression;
      LuaExpressionSyntax resultSelectorType;
      var parentNode = (QueryBodySyntax)node.Parent;
      if (parentNode.Clauses.Count == 1 && parentNode.SelectOrGroup.IsKind(SyntaxKind.SelectClause)) {
        var selectClause = (SelectClauseSyntax)parentNode.SelectOrGroup;
        resultSelectorExpression = (LuaExpressionSyntax)selectClause.Expression.Accept(this);
        var type = semanticModel_.GetTypeInfo(selectClause.Expression).Type;
        resultSelectorType = GetTypeName(type);
        isOver = true;
      }
      else {
        rangeVariable.HasPlaceholder = true;
        rangeVariable2.HasPlaceholder = true;
        resultSelectorExpression = CreateQueryAnonymousType(rangeVariable.Name, rangeVariable.Name, rangeVariable2.Name, rangeVariable2.Name);
        resultSelectorType = LuaIdentifierNameSyntax.AnonymousType;
        isOver = false;
      }
      resultSelector.AddStatement(new LuaReturnStatementSyntax(resultSelectorExpression));
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqSelectMany, collection, collectionSelector, resultSelector, resultSelectorType);
    }

    private LuaExpressionSyntax BuildLetClause(LuaExpressionSyntax collection, LetClauseSyntax node, QueryRangeVariable rangeVariable) {
      var letExpression = (LuaExpressionSyntax)node.Expression.Accept(this);
      var letRangeVariable = AddRangeVariable(node.Identifier);
      letRangeVariable.HasPlaceholder = true;
      rangeVariable.HasPlaceholder = true;
      var anonymousType = CreateQueryAnonymousType(rangeVariable.Name, rangeVariable.Name, letRangeVariable.Name, letExpression);

      var selectFunction = new LuaFunctionExpressionSyntax();
      selectFunction.AddParameter(rangeVariable.GetTrueName());
      selectFunction.AddStatement(new LuaReturnStatementSyntax(anonymousType));

      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqSelect, collection, selectFunction, LuaIdentifierNameSyntax.AnonymousType);
    }

    private LuaExpressionSyntax BuildQueryBody(LuaExpressionSyntax collection, QueryBodySyntax node, QueryRangeVariable rangeVariable) {
      foreach (var clause in node.Clauses) {
        switch (clause.Kind()) {
          case SyntaxKind.FromClause: {
              bool isOver;
              collection = BuildFromClause(collection, (FromClauseSyntax)clause, rangeVariable, out isOver);
              if (isOver) {
                if (node.Continuation != null) {
                  collection = BuildQueryContinuation(collection, node.Continuation);
                }
                return collection;
              }
              break;
            }
          case SyntaxKind.LetClause: {
              collection = BuildLetClause(collection, (LetClauseSyntax)clause, rangeVariable);
              break;
            }
          case SyntaxKind.WhereClause: {
              collection = BuildQueryWhere(collection, (WhereClauseSyntax)clause, rangeVariable);
              break;
            }
          case SyntaxKind.OrderByClause: {
              collection = BuildQueryOrderBy(collection, (OrderByClauseSyntax)clause, rangeVariable);
              break;
            }
          default: {
              throw new NotSupportedException();
            }
        }
      }

      if (node.SelectOrGroup.IsKind(SyntaxKind.SelectClause)) {
        var selectClause = (SelectClauseSyntax)node.SelectOrGroup;
        collection = BuildQuerySelect(collection, selectClause, rangeVariable);
      }
      else {
        var groupClause = (GroupClauseSyntax)node.SelectOrGroup;
        collection = BuildGroupClause(collection, groupClause, rangeVariable);
      }

      if (node.Continuation != null) {
        collection = BuildQueryContinuation(collection, node.Continuation);
      }
      return collection;
    }

    private LuaExpressionSyntax BuildQueryContinuation(LuaExpressionSyntax collection, QueryContinuationSyntax node) {
      var rangeVariable = AddRangeVariable(node.Identifier);
      return BuildQueryBody(collection, node.Body, rangeVariable);
    }
  }
}
