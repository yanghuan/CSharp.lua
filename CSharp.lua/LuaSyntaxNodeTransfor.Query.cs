using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using CSharpLua.LuaAst;

namespace CSharpLua {
    public sealed partial class LuaSyntaxNodeTransfor {
        public override LuaSyntaxNode VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node) {
            var name = (LuaIdentifierNameSyntax)node.NameEquals.Accept(this);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), expression);
        }

        public override LuaSyntaxNode VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node) {
            LuaTableInitializerExpression table = new LuaTableInitializerExpression();
            foreach(var initializer in node.Initializers) {
                var item = (LuaKeyValueTableItemSyntax)initializer.Accept(this);
                table.Items.Add(item);
            }
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.AnonymousType, table);
        }

        public override LuaSyntaxNode VisitQueryExpression(QueryExpressionSyntax node) {
            CurCompilationUnit.ImportLinq();
            LuaIdentifierNameSyntax rangeVariable = new LuaIdentifierNameSyntax(node.FromClause.Identifier.ValueText);
            CheckVariableDeclaratorName(ref rangeVariable, node);
            var fromClauseExpression = (LuaExpressionSyntax)node.FromClause.Expression.Accept(this);
            return BuildQueryBody(node.Body, fromClauseExpression, rangeVariable);
        }

        public override LuaSyntaxNode VisitFromClause(FromClauseSyntax node) {
            throw new InvalidOperationException();
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

        private LuaExpressionSyntax BuildQueryWhere(LuaExpressionSyntax collection, WhereClauseSyntax node, LuaIdentifierNameSyntax rangeVariable) {
            var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
            var whereFunction = new LuaFunctionExpressionSyntax();
            whereFunction.AddParameter(rangeVariable);
            whereFunction.AddStatement(new LuaReturnStatementSyntax(condition));
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqWhere, collection, whereFunction);
        }

        private LuaExpressionSyntax BuildOrdering(LuaIdentifierNameSyntax methodName, LuaExpressionSyntax collection, OrderingSyntax node, LuaIdentifierNameSyntax rangeVariable) {
            var type = semanticModel_.GetTypeInfo(node.Expression).Type;
            var typeName = GetTypeName(type);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            var keySelector = new LuaFunctionExpressionSyntax();
            keySelector.AddParameter(rangeVariable);
            keySelector.AddStatement(new LuaReturnStatementSyntax(expression));
            return new LuaInvocationExpressionSyntax(methodName, collection, keySelector, LuaIdentifierNameSyntax.Nil, typeName);
        }

        private LuaExpressionSyntax BuildQueryOrderBy(LuaExpressionSyntax collection, OrderByClauseSyntax node, LuaIdentifierNameSyntax rangeVariable) {
            foreach(var ordering in node.Orderings) {
                bool isDescending = ordering.AscendingOrDescendingKeyword.IsKind(SyntaxKind.DescendingKeyword);
                if(ordering == node.Orderings.First()) {
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

        private LuaExpressionSyntax BuildQuerySelect(LuaExpressionSyntax collection, SelectClauseSyntax node, LuaIdentifierNameSyntax rangeVariable) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            if(node.Expression.IsKind(SyntaxKind.IdentifierName)) {
                var identifierName = expression as LuaIdentifierNameSyntax;
                if(identifierName != null && identifierName.ValueText == rangeVariable.ValueText) {
                    return collection;
                }
            }
            var type = semanticModel_.GetTypeInfo(node.Expression).Type;
            var typeExpression = GetTypeName(type);
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqSelect, collection, expression, typeExpression);
        }

        private LuaExpressionSyntax BuildGroupClause(LuaExpressionSyntax collection, GroupClauseSyntax node, LuaIdentifierNameSyntax rangeVariable) {
            var type = semanticModel_.GetTypeInfo(node.ByExpression).Type;
            var typeName = GetTypeName(type);
            var byExpression = (LuaExpressionSyntax)node.ByExpression.Accept(this);
            var keySelector = new LuaFunctionExpressionSyntax();
            keySelector.AddParameter(rangeVariable);
            keySelector.AddStatement(new LuaReturnStatementSyntax(byExpression));
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqGroupBy, collection, keySelector, typeName);
        }

        public LuaSyntaxNode BuildQueryBody(QueryBodySyntax node, LuaExpressionSyntax fromClauseExpression, LuaIdentifierNameSyntax rangeVariable) {
            LuaExpressionSyntax collection = fromClauseExpression;
            foreach(var clause in node.Clauses) {
                switch(clause.Kind()) {
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
            if(node.SelectOrGroup.IsKind(SyntaxKind.SelectClause)) {
                var selectClause = (SelectClauseSyntax)node.SelectOrGroup;
                collection = BuildQuerySelect(collection, selectClause, rangeVariable);
            }
            else {
                var groupClause = (GroupClauseSyntax)node.SelectOrGroup;
                collection = BuildGroupClause(collection, groupClause, rangeVariable);
            }
            return collection;
        }
    }
}
