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
            var localVariable = (LuaLocalVariableDeclaratorSyntax)node.FromClause.Accept(this);
            CurBlock.Statements.Add(localVariable);
            return BuildQueryBody(node.Body, localVariable.Declarator.Identifier);
        }

        public override LuaSyntaxNode VisitFromClause(FromClauseSyntax node) {
            LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
            CheckVariableDeclaratorName(ref identifier, node);
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            return new LuaLocalVariableDeclaratorSyntax(identifier, expression);
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

        private LuaExpressionSyntax BuildQueryOrderBy(LuaExpressionSyntax collection, OrderByClauseSyntax node, LuaIdentifierNameSyntax rangeVariable) {
            return null;
        }

        private LuaExpressionSyntax BuildQuerySelect(LuaExpressionSyntax collection, SelectClauseSyntax select, LuaIdentifierNameSyntax rangeVariable) {
            var expression = (LuaExpressionSyntax)select.Expression.Accept(this);
            if(select.Expression.IsKind(SyntaxKind.IdentifierName)) {
                var identifierName = expression as LuaIdentifierNameSyntax;
                if(identifierName != null && identifierName.ValueText == rangeVariable.ValueText) {
                    return collection;
                }
            }
            var type = semanticModel_.GetTypeInfo(select.Expression).Type;
            var typeExpression = GetTypeName(type);
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.LinqSelect, collection, expression, typeExpression);
        }

        public LuaSyntaxNode BuildQueryBody(QueryBodySyntax node, LuaIdentifierNameSyntax rangeVariable) {
            LuaExpressionSyntax collection = rangeVariable;
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
                }
            }
            if(node.SelectOrGroup.IsKind(SyntaxKind.SelectClause)) {
                var selectClause = (SelectClauseSyntax)node.SelectOrGroup;
                collection = BuildQuerySelect(collection, selectClause, rangeVariable);
            }
            else {
                throw new NotSupportedException();
            }
            return collection;
        }
    }
}
