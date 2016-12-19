using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using CSharpLua.LuaAst;
using System.Diagnostics.Contracts;

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

        public override LuaSyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node) {
            LuaConstructorAdapterExpressSyntax function = new LuaConstructorAdapterExpressSyntax();
            functions_.Push(function);
            bool isStatic = node.Modifiers.IsStatic();
            function.IsStaticCtor = isStatic;
            var parameterList = (LuaParameterListSyntax)node.ParameterList.Accept(this);
            function.ParameterList.Parameters.Add(new LuaParameterSyntax(LuaIdentifierNameSyntax.This));
            function.ParameterList.Parameters.AddRange(parameterList.Parameters);
            if(node.Initializer != null) {
                var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node.Initializer).Symbol;
                var typeSymbol = (INamedTypeSymbol)symbol.ReceiverType;
                int index = typeSymbol.Constructors.IndexOf(symbol);
                Contract.Assert(index != -1);
                int ctroCounter = index + 1;

                LuaInvocationExpressionSyntax otherCtorInvoke;
                if(node.Initializer.IsKind(SyntaxKind.ThisConstructorInitializer)) {
                    LuaIdentifierNameSyntax thisCtor = new LuaIdentifierNameSyntax(LuaSyntaxNode.SpecailWord(LuaSyntaxNode.Tokens.Ctor + ctroCounter));
                    otherCtorInvoke = new LuaInvocationExpressionSyntax(thisCtor);
                    function.IsInvokeThisCtor = true;
                }
                else {
                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(new LuaIdentifierNameSyntax(typeSymbol.ToString()), LuaIdentifierNameSyntax.Ctor);
                    otherCtorInvoke = new LuaInvocationExpressionSyntax(new LuaTableIndexAccessExpressionSyntax(memberAccess, new LuaIdentifierNameSyntax(ctroCounter.ToString())));
                }

                otherCtorInvoke.ArgumentList.Arguments.Add(new LuaArgumentSyntax(LuaIdentifierNameSyntax.This));
                var argumentList = (LuaArgumentListSyntax)node.Initializer.ArgumentList.Accept(this);
                otherCtorInvoke.ArgumentList.Arguments.AddRange(argumentList.Arguments);
                function.Body.Statements.Add(new LuaExpressionStatementSyntax(otherCtorInvoke));
            }
            LuaBlockSyntax block = (LuaBlockSyntax)node.Body.Accept(this);
            function.Body.Statements.AddRange(block.Statements);
            functions_.Pop();
            if(isStatic) {
                CurType.SetStaticCtor(function);
            }
            else {
                CurType.AddCtor(function);
            }
            return function;
        }
    }
}