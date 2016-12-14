using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using CSharpLua.LuaAst;

namespace CSharpLua {
    public sealed partial class LuaSyntaxNodeTransfor {
        public override LuaSyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node) {
            var type = (LuaExpressionSyntax)node.Type.Accept(this);
            var argumentList = (LuaArgumentListSyntax)node.ArgumentList.Accept(this);
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(type);
            invocationExpression.ArgumentList.Arguments.AddRange(argumentList.Arguments);
            if(node.Initializer == null) {
                return invocationExpression;
            }
            else {
                var thisIdentifier = LuaIdentifierNameSyntax.This;
                LuaFunctionExpressSyntax function = new LuaFunctionExpressSyntax();
                function.ParameterList.Parameters.Add(new LuaParameterSyntax(thisIdentifier));
                foreach(var expression in node.Initializer.Expressions) {
                    if(expression.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
                        AssignmentExpressionSyntax assignment = (AssignmentExpressionSyntax)expression;
                        var identifierName = (LuaIdentifierNameSyntax)assignment.Left.Accept(this);
                        LuaMemberAccessExpressionSyntax left = new LuaMemberAccessExpressionSyntax(thisIdentifier, identifierName);
                        var right = (LuaExpressionSyntax)assignment.Right.Accept(this);
                        function.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(left, right)));
                    }
                    else {
                        LuaInvocationExpressionSyntax add = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.ThisAdd);
                        var argument = (LuaExpressionSyntax)expression.Accept(this);
                        add.ArgumentList.Arguments.Add(new LuaArgumentSyntax(argument));
                        function.Body.Statements.Add(new LuaExpressionStatementSyntax(add));
                    }
                }

                LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.ObjectCreate);
                invocation.ArgumentList.Arguments.Add(new LuaArgumentSyntax(invocationExpression));
                invocation.ArgumentList.Arguments.Add(new LuaArgumentSyntax(function));
                return invocation;
            }
        }

        public override LuaSyntaxNode VisitGenericName(GenericNameSyntax node) {
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            string name = $"{symbol.ContainingNamespace.ToString()}.{symbol.Name}_{node.TypeArgumentList.Arguments.Count}";
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(new LuaIdentifierNameSyntax(name));
            foreach(var typeArgument in node.TypeArgumentList.Arguments) {
                var expression = (LuaExpressionSyntax)typeArgument.Accept(this);
                invocation.ArgumentList.Arguments.Add(new LuaArgumentSyntax(expression));
            }
            return invocation;
        }

        public override LuaSyntaxNode VisitArrayType(ArrayTypeSyntax node) {
            var elementType = (LuaExpressionSyntax)node.ElementType.Accept(this);

            LuaExpressionSyntax typeExpress = null;
            LuaExpressionSyntax size = null;
            foreach(var rank in node.RankSpecifiers.Reverse()) {
                if(rank.Rank > 1) {
                    throw new NotSupportedException();
                }
                size = (LuaExpressionSyntax)rank.Sizes.First().Accept(this);
                LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Array);
                invocation.ArgumentList.Arguments.Add(new LuaArgumentSyntax(typeExpress ?? elementType));
                typeExpress = invocation;
            }

            LuaInvocationExpressionSyntax arrayInvocation = new LuaInvocationExpressionSyntax(typeExpress);
            if(size != null) {
                arrayInvocation.ArgumentList.Arguments.Add(new LuaArgumentSyntax(size));
            }
            return arrayInvocation;
        }

        public override LuaSyntaxNode VisitArrayCreationExpression(ArrayCreationExpressionSyntax node) {
            var arrayInvocation = (LuaInvocationExpressionSyntax)node.Type.Accept(this);
            if(node.Initializer != null) {
                bool hasSize = arrayInvocation.ArgumentList.Arguments.Count > 0;
                if(!hasSize) {
                    var sizeExprssion = new LuaIdentifierNameSyntax(node.Initializer.Expressions.Count.ToString());
                    arrayInvocation.ArgumentList.Arguments.Add(new LuaArgumentSyntax(sizeExprssion));
                }
                foreach(var expression in node.Initializer.Expressions) {
                    var itemExpression = (LuaExpressionSyntax)expression.Accept(this);
                    arrayInvocation.ArgumentList.Arguments.Add(new LuaArgumentSyntax(itemExpression));
                }
            }
            return arrayInvocation;
        }
    }
}