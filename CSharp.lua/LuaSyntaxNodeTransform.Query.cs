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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed partial class LuaSyntaxNodeTransform {
    private const string kQueryPlaceholderConflictName = "as";

    private interface IQueryRangeVariable {
      LuaIdentifierNameSyntax Name { get; }
      void AddPackCount();
    }

    private sealed class QueryIdentifier : IQueryRangeVariable {
      public SyntaxToken Identifier { get; }
      private readonly LuaIdentifierNameSyntax name_;
      private int packCount_;

      public QueryIdentifier(SyntaxToken identifier, LuaIdentifierNameSyntax name) {
        Identifier = identifier;
        name_ = name;
      }

      public void AddPackCount() {
        ++packCount_;
      }

      public bool HasPack => packCount_ > 0;

      public LuaIdentifierNameSyntax Name {
        get {
          if (HasPack) {
            throw new InvalidOperationException();
          }
          return name_;
        }
      }

      public LuaExpressionSyntax GetIdentifierName() {
        LuaExpressionSyntax name = name_;
        for (int i = 0; i < packCount_; ++i) {
          name = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.Placeholder, name);
        }
        return name;
      }
    }

    private sealed class QueryPackVariable : IQueryRangeVariable {
      private IQueryRangeVariable x1_;
      private IQueryRangeVariable x2_;

      public QueryPackVariable(IQueryRangeVariable x1, IQueryRangeVariable x2) {
        x1_ = x1;
        x2_ = x2;
        AddPackCount();
      }

      public LuaIdentifierNameSyntax Name => LuaIdentifierNameSyntax.Placeholder;

      public void AddPackCount() {
        x1_.AddPackCount();
        x2_.AddPackCount();
      }
    }

    private List<QueryIdentifier> queryIdentifiers_ = new List<QueryIdentifier>();

    private QueryIdentifier AddRangeIdentifier(SyntaxToken identifier) {
      string name = identifier.ValueText;
      if (name == LuaIdentifierNameSyntax.Placeholder.ValueText) {
        if (queryIdentifiers_.Exists(i => i.HasPack)) {
          name = kQueryPlaceholderConflictName;
        }
      } else {
        CheckLocalBadWord(ref name, identifier.Parent);
      }
      var queryIdentifier = new QueryIdentifier(identifier, name);
      queryIdentifiers_.Add(queryIdentifier);
      return queryIdentifier;
    }

    private LuaExpressionSyntax GetRangeIdentifierName(IdentifierNameSyntax name) {
      var info = queryIdentifiers_.Find(i => i.Identifier.ValueText == name.Identifier.ValueText);
      Contract.Assert(info != null);
      return info.GetIdentifierName();
    }

    public override LuaSyntaxNode VisitQueryExpression(QueryExpressionSyntax node) {
      CurCompilationUnit.ImportLinq();

      var rangeVariable = AddRangeIdentifier(node.FromClause.Identifier);
      var collection = (LuaExpressionSyntax)node.FromClause.Accept(this);
      var queryExpression = BuildQueryBody(collection, node.Body, rangeVariable);
      queryIdentifiers_.Clear();
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

    public override LuaSyntaxNode VisitJoinClause(JoinClauseSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitJoinIntoClause(JoinIntoClauseSyntax node) {
      throw new InvalidOperationException();
    }

    private LuaExpressionSyntax BuildQueryWhere(LuaExpressionSyntax collection, WhereClauseSyntax node, IQueryRangeVariable rangeVariable) {
      var whereFunction = new LuaFunctionExpressionSyntax();
      PushFunction(whereFunction);
      var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
      if (condition == LuaIdentifierLiteralExpressionSyntax.True) {
        PopFunction();
        return collection;
      }
      whereFunction.AddParameter(rangeVariable.Name);
      whereFunction.AddStatement(new LuaReturnStatementSyntax(condition));
      PopFunction();
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqWhere, collection, whereFunction);
    }

    private LuaExpressionSyntax BuildOrdering(LuaIdentifierNameSyntax methodName, LuaExpressionSyntax collection, OrderingSyntax node, IQueryRangeVariable rangeVariable) {
      var type = semanticModel_.GetTypeInfo(node.Expression).Type;
      var typeName = GetTypeName(type);
      var keySelector = new LuaFunctionExpressionSyntax();
      PushFunction(keySelector);
      keySelector.AddParameter(rangeVariable.Name);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      keySelector.AddStatement(new LuaReturnStatementSyntax(expression));
      PopFunction();
      return new LuaInvocationExpressionSyntax(methodName, collection, keySelector, LuaIdentifierNameSyntax.Nil, typeName);
    }

    private LuaExpressionSyntax BuildQueryOrderBy(LuaExpressionSyntax collection, OrderByClauseSyntax node, IQueryRangeVariable rangeVariable) {
      foreach (var ordering in node.Orderings) {
        bool isDescending = ordering.AscendingOrDescendingKeyword.IsKind(SyntaxKind.DescendingKeyword);
        if (ordering == node.Orderings.First()) {
          var methodName = isDescending ? LuaIdentifierNameSyntax.LinqOrderByDescending : LuaIdentifierNameSyntax.LinqOrderBy;
          collection = BuildOrdering(methodName, collection, ordering, rangeVariable);
        } else {
          var methodName = isDescending ? LuaIdentifierNameSyntax.LinqThenByDescending : LuaIdentifierNameSyntax.LinqThenBy;
          collection = BuildOrdering(methodName, collection, ordering, rangeVariable);
        }
      }
      return collection;
    }

    private LuaExpressionSyntax BuildQuerySelect(LuaExpressionSyntax collection, SelectClauseSyntax node, IQueryRangeVariable rangeVariable) {
      var selectFunction = new LuaFunctionExpressionSyntax();
      PushFunction(selectFunction);
      selectFunction.AddParameter(rangeVariable.Name);

      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      if (node.Expression.IsKind(SyntaxKind.IdentifierName)) {
        if (expression is LuaIdentifierNameSyntax identifierName && identifierName.ValueText == rangeVariable.Name.ValueText) {
          PopFunction();
          return collection;
        }
      }

      selectFunction.AddStatement(new LuaReturnStatementSyntax(expression));
      PopFunction();
      var type = semanticModel_.GetTypeInfo(node.Expression).Type;
      var typeExpression = GetTypeName(type);
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqSelect, collection, selectFunction, typeExpression);
    }

    private LuaExpressionSyntax BuildGroupClause(LuaExpressionSyntax collection, GroupClauseSyntax node, IQueryRangeVariable rangeVariable) {
      var keyType = semanticModel_.GetTypeInfo(node.ByExpression).Type;
      var keyTypeName = GetTypeName(keyType);
      var keySelector = new LuaFunctionExpressionSyntax();
      PushFunction(keySelector);
      keySelector.AddParameter(rangeVariable.Name);
      var byExpression = (LuaExpressionSyntax)node.ByExpression.Accept(this);
      keySelector.AddStatement(new LuaReturnStatementSyntax(byExpression));
      PopFunction();

      var elementSelector = new LuaFunctionExpressionSyntax();
      PushFunction(elementSelector);
      elementSelector.AddParameter(rangeVariable.Name);
      var groupExpression = (LuaExpressionSyntax)node.GroupExpression.Accept(this);
      if (node.GroupExpression.IsKind(SyntaxKind.IdentifierName)) {
        if (groupExpression is LuaIdentifierNameSyntax groupIdentifierName && groupIdentifierName.ValueText == rangeVariable.Name.ValueText) {
          PopFunction();
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqGroupBy, collection, keySelector, keyTypeName);
        }
      }
      elementSelector.AddStatement(new LuaReturnStatementSyntax(groupExpression));
      PopFunction();

      var elementType = semanticModel_.GetTypeInfo(node.GroupExpression).Type;
      var elementTypeName = GetTypeName(elementType);
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqGroupBy, collection, keySelector, elementSelector, keyTypeName, elementTypeName);
    }

    private LuaExpressionSyntax CreateQueryAnonymousType(LuaIdentifierNameSyntax key1, LuaExpressionSyntax value1, LuaIdentifierNameSyntax key2, LuaExpressionSyntax value2) {
      LuaTableExpression table = new LuaTableExpression();
      table.Add(key1, value1);
      table.Add(key2, value2);
      return table;
    }

    private bool IsSpecialQueryNode(QueryBodySyntax node) {
      return node.Clauses.Count == 1 && node.SelectOrGroup.IsKind(SyntaxKind.SelectClause);
    }

    private LuaExpressionSyntax BuildFromClause(LuaExpressionSyntax collection, FromClauseSyntax node, ref IQueryRangeVariable rangeVariable, out bool isOver) {
      var collectionSelector = new LuaFunctionExpressionSyntax();
      PushFunction(collectionSelector);
      collectionSelector.AddParameter(rangeVariable.Name);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      collectionSelector.AddStatement(new LuaReturnStatementSyntax(expression));
      PopFunction();

      var rangeVariable2 = AddRangeIdentifier(node.Identifier);
      var resultSelector = new LuaFunctionExpressionSyntax();
      PushFunction(resultSelector);
      resultSelector.AddParameter(rangeVariable.Name);
      resultSelector.AddParameter(rangeVariable2.Name);
      LuaExpressionSyntax resultSelectorExpression;
      LuaExpressionSyntax resultSelectorType;
      var parentNode = (QueryBodySyntax)node.Parent;
      if (IsSpecialQueryNode(parentNode)) {
        var selectClause = (SelectClauseSyntax)parentNode.SelectOrGroup;
        resultSelectorExpression = (LuaExpressionSyntax)selectClause.Expression.Accept(this);
        var type = semanticModel_.GetTypeInfo(selectClause.Expression).Type;
        resultSelectorType = GetTypeName(type);
        isOver = true;
      } else {
        resultSelectorExpression = CreateQueryAnonymousType(rangeVariable.Name, rangeVariable.Name, rangeVariable2.Name, rangeVariable2.Name);
        resultSelectorType = LuaIdentifierNameSyntax.AnonymousType;
        rangeVariable = new QueryPackVariable(rangeVariable, rangeVariable2);
        isOver = false;
      }
      resultSelector.AddStatement(new LuaReturnStatementSyntax(resultSelectorExpression));
      PopFunction();
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqSelectMany, collection, collectionSelector, resultSelector, resultSelectorType);
    }

    private LuaExpressionSyntax BuildLetClause(LuaExpressionSyntax collection, LetClauseSyntax node, ref IQueryRangeVariable rangeVariable) {
      var selectFunction = new LuaFunctionExpressionSyntax();
      PushFunction(selectFunction);
      selectFunction.AddParameter(rangeVariable.Name);

      var letExpression = (LuaExpressionSyntax)node.Expression.Accept(this);
      var letRangeVariable = AddRangeIdentifier(node.Identifier);
      var anonymousType = CreateQueryAnonymousType(rangeVariable.Name, rangeVariable.Name, letRangeVariable.Name, letExpression);
      selectFunction.AddStatement(new LuaReturnStatementSyntax(anonymousType));
      PopFunction();

      rangeVariable = new QueryPackVariable(rangeVariable, letRangeVariable);
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqSelect, collection, selectFunction, LuaIdentifierNameSyntax.AnonymousType);
    }

    private bool BuildQueryJoin(JoinClauseSyntax node, out LuaExpressionSyntax resultSelectorExpression, out LuaExpressionSyntax resultSelectorType, ref IQueryRangeVariable rangeVariable, QueryIdentifier queryIdentifier) {
      var parentNode = (QueryBodySyntax)node.Parent;
      if (IsSpecialQueryNode(parentNode)) {
        var selectClause = (SelectClauseSyntax)parentNode.SelectOrGroup;
        resultSelectorExpression = (LuaExpressionSyntax)selectClause.Expression.Accept(this);
        var typeSymbol = semanticModel_.GetTypeInfo(selectClause.Expression).Type;
        resultSelectorType = GetTypeName(typeSymbol);
        return true;
      } else {
        resultSelectorExpression = CreateQueryAnonymousType(rangeVariable.Name, rangeVariable.Name, queryIdentifier.Name, queryIdentifier.Name);
        resultSelectorType = LuaIdentifierNameSyntax.AnonymousType;
        rangeVariable = new QueryPackVariable(rangeVariable, queryIdentifier);
        return false;
      }
    }

    private LuaExpressionSyntax BuildJoinClause(LuaExpressionSyntax collection, JoinClauseSyntax node, ref IQueryRangeVariable rangeVariable, out bool isOver) {
      isOver = false;

      var rangeVariable2 = AddRangeIdentifier(node.Identifier);
      var inner = (LuaExpressionSyntax)node.InExpression.Accept(this);
      var outerKeySelector = new LuaFunctionExpressionSyntax();
      PushFunction(outerKeySelector);
      outerKeySelector.AddParameter(rangeVariable.Name);
      var left = (LuaExpressionSyntax)node.LeftExpression.Accept(this);
      outerKeySelector.AddStatement(new LuaReturnStatementSyntax(left));
      PopFunction();

      var keyTypeSymbol = semanticModel_.GetTypeInfo(node.LeftExpression).Type;
      var keyType = GetTypeName(keyTypeSymbol);

      var innerKeySelector = new LuaFunctionExpressionSyntax();
      PushFunction(innerKeySelector);
      innerKeySelector.AddParameter(rangeVariable2.Name);
      var right = (LuaExpressionSyntax)node.RightExpression.Accept(this);
      innerKeySelector.AddStatement(new LuaReturnStatementSyntax(right));
      PopFunction();

      LuaFunctionExpressionSyntax resultSelector = new LuaFunctionExpressionSyntax();
      PushFunction(resultSelector);
      LuaExpressionSyntax resultSelectorExpression;
      LuaExpressionSyntax resultSelectorType;
      LuaIdentifierNameSyntax methodName;

      var parentNode = (QueryBodySyntax)node.Parent;
      if (node.Into == null) {
        methodName = LuaIdentifierNameSyntax.LinqJoin;
        resultSelector.AddParameter(rangeVariable.Name);
        resultSelector.AddParameter(rangeVariable2.Name);
        isOver = BuildQueryJoin(node, out resultSelectorExpression, out resultSelectorType, ref rangeVariable, rangeVariable2);
      } else {
        methodName = LuaIdentifierNameSyntax.LinqGroupJoin;
        var rangeVariableOfInto = AddRangeIdentifier(node.Into.Identifier);
        resultSelector.AddParameter(rangeVariable.Name);
        resultSelector.AddParameter(rangeVariableOfInto.Name);
        isOver = BuildQueryJoin(node, out resultSelectorExpression, out resultSelectorType, ref rangeVariable, rangeVariableOfInto);
      }
      resultSelector.AddStatement(new LuaReturnStatementSyntax(resultSelectorExpression));
      PopFunction();
      return new LuaInvocationExpressionSyntax(methodName, collection, inner, outerKeySelector, innerKeySelector, resultSelector, LuaIdentifierLiteralExpressionSyntax.Nil, keyType, resultSelectorType);
    }

    private LuaExpressionSyntax BuildQueryBody(LuaExpressionSyntax collection, QueryBodySyntax node, IQueryRangeVariable rangeVariable) {
      foreach (var clause in node.Clauses) {
        switch (clause.Kind()) {
          case SyntaxKind.FromClause: {
              collection = BuildFromClause(collection, (FromClauseSyntax)clause, ref rangeVariable, out bool isOver);
              if (isOver) {
                goto Continuation;
              }
              break;
            }
          case SyntaxKind.LetClause: {
              collection = BuildLetClause(collection, (LetClauseSyntax)clause, ref rangeVariable);
              break;
            }
          case SyntaxKind.JoinClause: {
              collection = BuildJoinClause(collection, (JoinClauseSyntax)clause, ref rangeVariable, out bool isOver);
              if (isOver) {
                goto Continuation;
              }
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
      } else {
        var groupClause = (GroupClauseSyntax)node.SelectOrGroup;
        collection = BuildGroupClause(collection, groupClause, rangeVariable);
      }

    Continuation:
      if (node.Continuation != null) {
        collection = BuildQueryContinuation(collection, node.Continuation);
      }
      return collection;
    }

    private LuaExpressionSyntax BuildQueryContinuation(LuaExpressionSyntax collection, QueryContinuationSyntax node) {
      var rangeVariable = AddRangeIdentifier(node.Identifier);
      return BuildQueryBody(collection, node.Body, rangeVariable);
    }
  }
}
