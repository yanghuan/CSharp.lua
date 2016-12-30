using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
                LuaFunctionExpressSyntax function = new LuaFunctionExpressSyntax();
                function.AddParameter(LuaIdentifierNameSyntax.This);
                foreach(var expression in node.Initializer.Expressions) {
                    if(expression.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
                        AssignmentExpressionSyntax assignment = (AssignmentExpressionSyntax)expression;
                        var identifierName = (LuaIdentifierNameSyntax)assignment.Left.Accept(this);
                        var right = (LuaExpressionSyntax)assignment.Right.Accept(this);
                        function.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(identifierName, right)));
                    }
                    else {
                        LuaInvocationExpressionSyntax add = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.ThisAdd);
                        var argument = (LuaExpressionSyntax)expression.Accept(this);
                        add.AddArgument(argument);
                        function.Body.Statements.Add(new LuaExpressionStatementSyntax(add));
                    }
                }

                LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Create);
                invocation.AddArgument(invocationExpression);
                invocation.AddArgument(function);
                return invocation;
            }
        }

        public override LuaSyntaxNode VisitGenericName(GenericNameSyntax node) {
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            if(symbol.Kind == SymbolKind.Method) {
                return GetMethodNameExpression((IMethodSymbol)symbol, node);
            }
            else {
                string name = xmlMetaProvider_.GetTypeMapName(symbol);
                LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(new LuaIdentifierNameSyntax(name));
                foreach(var typeArgument in node.TypeArgumentList.Arguments) {
                    var expression = (LuaExpressionSyntax)typeArgument.Accept(this);
                    invocation.AddArgument(expression);
                }
                return invocation;
            }
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
                invocation.AddArgument(typeExpress ?? elementType);
                typeExpress = invocation;
            }

            LuaInvocationExpressionSyntax arrayInvocation = new LuaInvocationExpressionSyntax(typeExpress);
            if(size != null) {
                arrayInvocation.AddArgument(size);
            }
            return arrayInvocation;
        }

        public override LuaSyntaxNode VisitArrayCreationExpression(ArrayCreationExpressionSyntax node) {
            var arrayInvocation = (LuaInvocationExpressionSyntax)node.Type.Accept(this);
            if(node.Initializer != null) {
                bool hasSize = arrayInvocation.ArgumentList.Arguments.Count > 0;
                if(!hasSize) {
                    var sizeExprssion = new LuaIdentifierNameSyntax(node.Initializer.Expressions.Count.ToString());
                    arrayInvocation.AddArgument(sizeExprssion);
                }
                foreach(var expression in node.Initializer.Expressions) {
                    var itemExpression = (LuaExpressionSyntax)expression.Accept(this);
                    arrayInvocation.AddArgument(itemExpression);
                }
            }
            return arrayInvocation;
        }

        public override LuaSyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node) {
            LuaConstructorAdapterExpressSyntax function = new LuaConstructorAdapterExpressSyntax();
            PushFunction(function);
            bool isStatic = node.Modifiers.IsStatic();
            function.IsStaticCtor = isStatic;
            var parameterList = (LuaParameterListSyntax)node.ParameterList.Accept(this);
            function.AddParameter(LuaIdentifierNameSyntax.This);
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

                otherCtorInvoke.AddArgument(LuaIdentifierNameSyntax.This);
                var argumentList = (LuaArgumentListSyntax)node.Initializer.ArgumentList.Accept(this);
                otherCtorInvoke.ArgumentList.Arguments.AddRange(argumentList.Arguments);
                function.Body.Statements.Add(new LuaExpressionStatementSyntax(otherCtorInvoke));
            }
            LuaBlockSyntax block = (LuaBlockSyntax)node.Body.Accept(this);
            function.Body.Statements.AddRange(block.Statements);
            PopFunction();
            if(isStatic) {
                CurType.SetStaticCtor(function);
            }
            else {
                CurType.AddCtor(function);
            }
            return function;
        }

        public override LuaSyntaxNode VisitSimpleBaseType(SimpleBaseTypeSyntax node) {
            return node.Type.Accept(this);
        }

        private LuaFunctionExpressSyntax VisitLambdaExpression(IEnumerable<ParameterSyntax> parameters, CSharpSyntaxNode body) {
            LuaFunctionExpressSyntax function = new LuaFunctionExpressSyntax();
            PushFunction(function);

            foreach(var parameter in parameters) {
                var luaParameter = (LuaParameterSyntax)parameter.Accept(this);
                function.ParameterList.Parameters.Add(luaParameter);
            }

            if(body.IsKind(SyntaxKind.Block)) {
                var block = (LuaBlockSyntax)body.Accept(this);
                function.Body.Statements.AddRange(block.Statements);
            }
            else {
                blocks_.Push(function.Body);
                var expression = (LuaExpressionSyntax)body.Accept(this);
                blocks_.Pop();
                function.Body.Statements.Add(new LuaExpressionStatementSyntax(expression));
            }

            PopFunction();
            return function;
        }

        public override LuaSyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) {
            return VisitLambdaExpression(new ParameterSyntax[] { node.Parameter }, node.Body);
        }

        public override LuaSyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) {
            return VisitLambdaExpression(node.ParameterList.Parameters, node.Body);
        }

        public override LuaSyntaxNode VisitTypeParameter(TypeParameterSyntax node) {
            return new LuaIdentifierNameSyntax(node.Identifier.ValueText);
        }

        public override LuaSyntaxNode VisitTypeOfExpression(TypeOfExpressionSyntax node) {
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.TypeOf);
            var typeName = (LuaIdentifierNameSyntax)node.Type.Accept(this);
            invocation.AddArgument(typeName);
            return invocation;
        }

        private LuaFunctionExpressSyntax VisitTryCatchesExpress(SyntaxList<CatchClauseSyntax> catches) {
            LuaFunctionExpressSyntax functionExpress = new LuaFunctionExpressSyntax();
            var temp = GetTempIdentifier(catches.First());
            functionExpress.AddParameter(temp);
            foreach(var catchNode in catches) {

            }
            return functionExpress;
        }

        public override LuaSyntaxNode VisitTryStatement(TryStatementSyntax node) {
            LuaInvocationExpressionSyntax tryInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Try);

            var block = (LuaBlockSyntax)node.Block.Accept(this);
            LuaFunctionExpressSyntax tryBlockFunctionExpress = new LuaFunctionExpressSyntax();
            tryBlockFunctionExpress.Body.Statements.AddRange(block.Statements);
            tryInvocationExpression.AddArgument(tryBlockFunctionExpress);

            if(node.Catches.Count > 0) {
                var catchesExpress = VisitTryCatchesExpress(node.Catches);
                tryInvocationExpression.AddArgument(catchesExpress);
            }
            else {
                tryInvocationExpression.AddArgument(LuaIdentifierNameSyntax.Nil);
            }

            if(node.Finally != null) {
                var finallyBlock = (LuaBlockSyntax)node.Finally.Block.Accept(this);
                LuaFunctionExpressSyntax finallyBlockFunctionExpress = new LuaFunctionExpressSyntax();
                finallyBlockFunctionExpress.Body.Statements.AddRange(finallyBlock.Statements);
                tryInvocationExpression.AddArgument(finallyBlockFunctionExpress);
            }

            return new LuaExpressionStatementSyntax(tryInvocationExpression);
        }
    }
}