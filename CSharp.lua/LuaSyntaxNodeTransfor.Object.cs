/*
Copyright 2016 YANG Huan (sy.yanghuan@gmail.com).
Copyright 2016 Redmoon Inc.

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
            var expression = (LuaExpressionSyntax)node.Type.Accept(this);

            int constructorIndex = 0;
            var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
            if(symbol != null) {
                constructorIndex = GetConstructorIndex(symbol);
                if(constructorIndex > 0) {
                    expression = new LuaMemberAccessExpressionSyntax(expression, LuaIdentifierNameSyntax.New, true);
                }
            }

            var argumentList = (LuaArgumentListSyntax)node.ArgumentList.Accept(this);
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(expression);
            if(constructorIndex > 0) {
                invocationExpression.AddArgument(new LuaIdentifierNameSyntax(constructorIndex));
            }
            invocationExpression.ArgumentList.Arguments.AddRange(argumentList.Arguments);
            if(node.Initializer == null) {
                return invocationExpression;
            }
            else {
                var functionExpression = BuildObjectInitializerExpression(node.Initializer);
                return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Create, invocationExpression, functionExpression);
            }
        }

        private LuaFunctionExpressionSyntax BuildObjectInitializerExpression(InitializerExpressionSyntax node) {
            LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
            PushFunction(function);
            var temp = GetTempIdentifier(node);
            function.AddParameter(temp);
            foreach(var expression in node.Expressions) {
                if(expression.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
                    AssignmentExpressionSyntax assignment = (AssignmentExpressionSyntax)expression;
                    var left = assignment.Left.Accept(this);
                    var right = (LuaExpressionSyntax)assignment.Right.Accept(this);

                    if(assignment.Left.IsKind(SyntaxKind.ImplicitElementAccess)) {
                        var argumentList = (LuaArgumentListSyntax)left;
                        LuaIdentifierNameSyntax methodName = new LuaIdentifierNameSyntax(LuaSyntaxNode.Tokens.Set);
                        LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(temp, methodName, true);
                        LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(memberAccess);
                        invocation.ArgumentList.Arguments.AddRange(argumentList.Arguments);
                        invocation.AddArgument(right);
                        function.AddStatement(invocation);
                    }
                    else {
                        LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(temp, (LuaExpressionSyntax)left);
                        function.AddStatement(new LuaAssignmentExpressionSyntax(memberAccess, right));
                    }
                }
                else {
                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(temp, LuaIdentifierNameSyntax.Add, true);
                    var value = (LuaExpressionSyntax)expression.Accept(this);
                    function.AddStatement(new LuaInvocationExpressionSyntax(memberAccess, value));
                }
            }

            PopFunction();
            return function;
        }

        public override LuaSyntaxNode VisitInitializerExpression(InitializerExpressionSyntax node) {
            Contract.Assert(node.IsKind(SyntaxKind.ArrayInitializerExpression));
            var symbol = (IArrayTypeSymbol)semanticModel_.GetTypeInfo(node).ConvertedType;
            if(node.Expressions.Count > 0) {
                LuaExpressionSyntax arrayType = GetTypeName(symbol, node);
                LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(arrayType);
                foreach(var expression in node.Expressions) {
                    var element = (LuaExpressionSyntax)expression.Accept(this);
                    invocation.AddArgument(element);
                }
                return invocation;
            }
            else {
                LuaExpressionSyntax baseType = GetTypeName(symbol.ElementType, node);
                return BuildEmptyArray(baseType);
            }
        }

        public override LuaSyntaxNode VisitBracketedArgumentList(BracketedArgumentListSyntax node) {
            return BuildArgumentList(node.Arguments);
        }

        public override LuaSyntaxNode VisitImplicitElementAccess(ImplicitElementAccessSyntax node) {
            return node.ArgumentList.Accept(this);
        }

        public override LuaSyntaxNode VisitGenericName(GenericNameSyntax node) {
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            if(symbol.Kind == SymbolKind.Method) {
                return GetMethodNameExpression((IMethodSymbol)symbol, node);
            }
            else {
                return GetTypeName(symbol, node);
            }
        }

        public override LuaSyntaxNode VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node) {
            return null;
        }

        public override LuaSyntaxNode VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node) {
            LuaArrayRankSpecifierSyntax rankSpecifier = new LuaArrayRankSpecifierSyntax(node.Rank);
            foreach(var size in node.Sizes) {
                var expression = (LuaExpressionSyntax)size.Accept(this);
                rankSpecifier.Sizes.Add(expression);
            }
            return rankSpecifier;
        }

        public override LuaSyntaxNode VisitArrayType(ArrayTypeSyntax node) {
            var elementType = (LuaExpressionSyntax)node.ElementType.Accept(this);

            LuaInvocationExpressionSyntax typeExpress = null;
            foreach(var rank in node.RankSpecifiers.Reverse()) {
                var arrayTypeName = rank.Rank == 1 ? LuaIdentifierNameSyntax.Array : LuaIdentifierNameSyntax.MultiArray;
                typeExpress = new LuaInvocationExpressionSyntax(arrayTypeName, typeExpress ?? elementType);
            }

            var arrayRankSpecifier = (LuaArrayRankSpecifierSyntax)node.RankSpecifiers[0].Accept(this);
            LuaArrayTypeAdapterExpressionSyntax arrayTypeAdapter = new LuaArrayTypeAdapterExpressionSyntax(typeExpress, arrayRankSpecifier);
            return arrayTypeAdapter;
        }

        private void FillMultiArrayInitializer(InitializerExpressionSyntax initializer, LuaTableInitializerExpression rankSpecifier, LuaInvocationExpressionSyntax invocation, bool isFirst) {
            if(isFirst) {
                rankSpecifier.Items.Add(new LuaSingleTableItemSyntax(new LuaIdentifierNameSyntax(initializer.Expressions.Count)));
            }

            int index = 0;
            foreach(var expression in initializer.Expressions) {
                if(expression.IsKind(SyntaxKind.ArrayInitializerExpression)) {
                    FillMultiArrayInitializer((InitializerExpressionSyntax)expression, rankSpecifier, invocation, index == 0);
                }
                else {
                    var item = (LuaExpressionSyntax)expression.Accept(this);
                    invocation.AddArgument(item);
                }
                ++index;
            }
        }

        public override LuaSyntaxNode VisitArrayCreationExpression(ArrayCreationExpressionSyntax node) {
            var arrayType = (LuaArrayTypeAdapterExpressionSyntax)node.Type.Accept(this);
            if(node.Initializer != null && node.Initializer.Expressions.Count > 0) {
                if(arrayType.IsSimapleArray) {
                    return new LuaInvocationExpressionSyntax(arrayType, node.Initializer.Expressions.Select(i => (LuaExpressionSyntax)i.Accept(this)));
                }
                else {
                    LuaTableInitializerExpression rankSpecifier = new LuaTableInitializerExpression();
                    LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(arrayType, rankSpecifier);
                    FillMultiArrayInitializer(node.Initializer, rankSpecifier, invocationExpression, true);
                    return invocationExpression;
                }
            }
            else {
                if(arrayType.IsSimapleArray) {
                    var size = arrayType.RankSpecifier.Sizes[0];
                    if(size == null) {
                        return BuildEmptyArray(arrayType.BaseType);
                    }

                    var constSize = size as LuaLiteralExpressionSyntax;
                    if(constSize != null && constSize.Text == 0.ToString()) {
                        return BuildEmptyArray(arrayType.BaseType);
                    }

                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(arrayType, LuaIdentifierNameSyntax.New, true);
                    return new LuaInvocationExpressionSyntax(memberAccess, size);
                }
                else {
                    LuaTableInitializerExpression rankSpecifier = new LuaTableInitializerExpression();
                    foreach(var size in arrayType.RankSpecifier.Sizes) {
                        if(size != null) {
                            rankSpecifier.Items.Add(new LuaSingleTableItemSyntax(size));
                        }
                        else {
                            rankSpecifier.Items.Add(new LuaSingleTableItemSyntax(new LuaIdentifierNameSyntax(0)));
                        }
                    }
                    return new LuaInvocationExpressionSyntax(arrayType, rankSpecifier);
                }
            }
        }

        public override LuaSyntaxNode VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node) {
            var symbol = semanticModel_.GetTypeInfo(node.Initializer.Expressions.First()).Type;
            LuaExpressionSyntax elementTypeExpression = GetTypeName(symbol, node);
            LuaInvocationExpressionSyntax arrayTypeExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Array, elementTypeExpression);
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(arrayTypeExpression);
            foreach(var expression in node.Initializer.Expressions) {
                var element = (LuaExpressionSyntax)expression.Accept(this);
                invocation.AddArgument(element);
            }
            return invocation;
        }

        public override LuaSyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node) {
            LuaConstructorAdapterExpressionSyntax function = new LuaConstructorAdapterExpressionSyntax();
            PushFunction(function);
            bool isStatic = node.Modifiers.IsStatic();
            function.IsStaticCtor = isStatic;
            var parameterList = (LuaParameterListSyntax)node.ParameterList.Accept(this);
            function.AddParameter(LuaIdentifierNameSyntax.This);
            function.ParameterList.Parameters.AddRange(parameterList.Parameters);
            if(node.Initializer != null) {
                var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node.Initializer).Symbol;
                int ctroCounter = GetConstructorIndex(symbol);
                LuaInvocationExpressionSyntax otherCtorInvoke;
                if(node.Initializer.IsKind(SyntaxKind.ThisConstructorInitializer)) {
                    Contract.Assert(ctroCounter != 0);
                    LuaIdentifierNameSyntax thisCtor = new LuaIdentifierNameSyntax(LuaSyntaxNode.SpecailWord(LuaSyntaxNode.Tokens.Ctor + ctroCounter));
                    otherCtorInvoke = new LuaInvocationExpressionSyntax(thisCtor);
                    function.IsInvokeThisCtor = true;
                }
                else {
                    var typeName = GetTypeName(symbol.ReceiverType, node);
                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(typeName, LuaIdentifierNameSyntax.Ctor);
                    if(ctroCounter > 0) {
                        otherCtorInvoke = new LuaInvocationExpressionSyntax(new LuaTableIndexAccessExpressionSyntax(memberAccess, new LuaIdentifierNameSyntax(ctroCounter)));
                    }
                    else {
                        otherCtorInvoke = new LuaInvocationExpressionSyntax(memberAccess);
                    }
                }

                otherCtorInvoke.AddArgument(LuaIdentifierNameSyntax.This);
                var argumentList = (LuaArgumentListSyntax)node.Initializer.ArgumentList.Accept(this);
                otherCtorInvoke.ArgumentList.Arguments.AddRange(argumentList.Arguments);
                function.AddStatement(otherCtorInvoke);
            }
            LuaBlockSyntax block = (LuaBlockSyntax)node.Body.Accept(this);
            function.AddStatements(block.Statements);
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

        private LuaExpressionSyntax VisitLambdaExpression(IEnumerable<ParameterSyntax> parameters, CSharpSyntaxNode body) {
            LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
            PushFunction(function);

            foreach(var parameter in parameters) {
                var luaParameter = (LuaParameterSyntax)parameter.Accept(this);
                function.ParameterList.Parameters.Add(luaParameter);
            }

            LuaExpressionSyntax resultExpression = function;
            if(body.IsKind(SyntaxKind.Block)) {
                var block = (LuaBlockSyntax)body.Accept(this);
                function.AddStatements(block.Statements);
            }
            else {
                var type = (INamedTypeSymbol)semanticModel_.GetTypeInfo(body.Parent).ConvertedType;
                var delegateInvokeMethod = type.DelegateInvokeMethod;

                blocks_.Push(function.Body);
                var expression = (LuaExpressionSyntax)body.Accept(this);
                blocks_.Pop();
                if(delegateInvokeMethod.ReturnsVoid) {
                    function.AddStatement(expression);
                }
                else {
                    function.AddStatement(new LuaReturnStatementSyntax(expression));
                }
                if(function.Body.Statements.Count == 1) {
                    resultExpression = new LuaSimpleLambdaAdapterExpression(function);
                }
            }

            PopFunction();
            return resultExpression;
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
            var typeName = (LuaIdentifierNameSyntax)node.Type.Accept(this);
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.TypeOf, typeName);
        }

        public override LuaSyntaxNode VisitThrowStatement(ThrowStatementSyntax node) {
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Throw);
            if(node.Expression != null) {
                var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                invocationExpression.AddArgument(expression);
            }
            else {
                var curTryFunction = (LuaTryAdapterExpressionSyntax)CurFunction;
                Contract.Assert(curTryFunction.CatchTemp != null);
                invocationExpression.AddArgument(curTryFunction.CatchTemp);
            }
            return new LuaExpressionStatementSyntax(invocationExpression);
        }

        public override LuaSyntaxNode VisitCatchFilterClause(CatchFilterClauseSyntax node) {
            return node.FilterExpression.Accept(this);    
        }

        public override LuaSyntaxNode VisitCatchClause(CatchClauseSyntax node) {
            throw new InvalidOperationException();
        }

        public override LuaSyntaxNode VisitCatchDeclaration(CatchDeclarationSyntax node) {
            return new LuaVariableDeclaratorSyntax(new LuaIdentifierNameSyntax(node.Identifier.ValueText));
        }

        private LuaTryAdapterExpressionSyntax VisitTryCatchesExpress(SyntaxList<CatchClauseSyntax> catches) {
            LuaTryAdapterExpressionSyntax functionExpress = new LuaTryAdapterExpressionSyntax();
            PushFunction(functionExpress);
            var temp = GetTempIdentifier(catches.First());
            functionExpress.CatchTemp = temp;
            functionExpress.AddParameter(temp);

            LuaIfStatementSyntax ifStatement = null;
            bool hasCatchRoot = false;
            foreach(var catchNode in catches) {
                bool isRootExceptionDeclaration = false;
                LuaExpressionSyntax ifCondition = null;
                if(catchNode.Filter != null) {
                    ifCondition = (LuaExpressionSyntax)catchNode.Filter.Accept(this);
                }
                if(catchNode.Declaration != null) {
                    var typeName = (LuaIdentifierNameSyntax)catchNode.Declaration.Type.Accept(this);
                    if(typeName.ValueText != "System.Exception") {
                        var mathcTypeInvocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Is, temp, typeName);
                        if(ifCondition != null) {
                            ifCondition = new LuaBinaryExpressionSyntax(ifCondition, LuaSyntaxNode.Tokens.And, mathcTypeInvocation);
                        }
                        else {
                            ifCondition = mathcTypeInvocation;
                        }
                    }
                    else {
                        if(!catchNode.Declaration.Identifier.IsKind(SyntaxKind.None)) {
                            isRootExceptionDeclaration = true;
                        }
                        hasCatchRoot = true;
                    }
                }
                else {
                    hasCatchRoot = true;
                }

                var block = (LuaBlockSyntax)catchNode.Block.Accept(this);
                if(ifCondition != null) {
                    LuaBlockSyntax body;
                    if(ifStatement == null) {
                        ifStatement = new LuaIfStatementSyntax(ifCondition);
                        body = ifStatement.Body;
                    }
                    else {
                        LuaElseIfStatementSyntax elseIfStatement = new LuaElseIfStatementSyntax(ifCondition);
                        body = elseIfStatement.Body;
                        ifStatement.ElseIfStatements.Add(elseIfStatement);
                    }
                    if(catchNode.Declaration != null && !catchNode.Declaration.Identifier.IsKind(SyntaxKind.None)) {
                        var variableDeclarator = (LuaVariableDeclaratorSyntax)catchNode.Declaration.Accept(this);
                        variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(temp);
                        body.Statements.Add(new LuaLocalVariableDeclaratorSyntax(variableDeclarator));
                    }
                    body.Statements.AddRange(block.Statements);
                }
                else {
                    if(isRootExceptionDeclaration) {
                        var variableDeclarator = (LuaVariableDeclaratorSyntax)catchNode.Declaration.Accept(this);
                        variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(temp);
                        block.Statements.Insert(0, new LuaLocalVariableDeclaratorSyntax(variableDeclarator));
                    }

                    if(ifStatement != null) {
                        LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
                        elseClause.Body.Statements.AddRange(block.Statements);
                        ifStatement.Else = elseClause;
                    }
                    else {
                        functionExpress.AddStatements(block.Statements);
                    }
                    break;
                }
            }

            if(ifStatement != null) {
                if(!hasCatchRoot) {
                    Contract.Assert(ifStatement.Else == null);
                    LuaMultipleReturnStatementSyntax rethrowStatement = new LuaMultipleReturnStatementSyntax();
                    rethrowStatement.Expressions.Add(LuaIdentifierNameSyntax.One);
                    rethrowStatement.Expressions.Add(temp);
                    LuaBlockSyntax block = new LuaBlockSyntax();
                    block.Statements.Add(rethrowStatement);
                    LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
                    elseClause.Body.Statements.AddRange(block.Statements);
                    ifStatement.Else = elseClause;
                }
                functionExpress.AddStatement(ifStatement);
            }

            PopFunction();
            return functionExpress;
        }

        private LuaStatementSyntax BuildCheckReturnInvocationExpression(LuaInvocationExpressionSyntax invocationExpression, SyntaxNode node) {
            if(IsReturnExists(node)) {
                var temp1 = GetTempIdentifier(node);
                var temp2 = GetTempIdentifier(node);
                LuaLocalVariablesStatementSyntax localVariables = new LuaLocalVariablesStatementSyntax();
                localVariables.Variables.Add(temp1);
                localVariables.Variables.Add(temp2);
                LuaEqualsValueClauseListSyntax initializer = new LuaEqualsValueClauseListSyntax();
                initializer.Values.Add(invocationExpression);
                localVariables.Initializer = initializer;

                LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(temp1);
                if(CurFunction is LuaCheckReturnFunctionExpressionSyntax) {
                    LuaMultipleReturnStatementSyntax returnStatement = new LuaMultipleReturnStatementSyntax();
                    returnStatement.Expressions.Add(LuaIdentifierNameSyntax.True);
                    returnStatement.Expressions.Add(temp2);
                    ifStatement.Body.Statements.Add(returnStatement);
                }
                else {
                    ifStatement.Body.Statements.Add(new LuaReturnStatementSyntax(temp2));
                }

                LuaStatementListSyntax statements = new LuaStatementListSyntax();
                statements.Statements.Add(localVariables);
                statements.Statements.Add(ifStatement);
                return statements;
            }
            else {
                return new LuaExpressionStatementSyntax(invocationExpression);
            }
        }

        public override LuaSyntaxNode VisitFinallyClause(FinallyClauseSyntax node) {
            LuaFunctionExpressionSyntax functionExpress = new LuaFunctionExpressionSyntax();
            PushFunction(functionExpress);
            var finallyBlock = (LuaBlockSyntax)node.Block.Accept(this);
            PopFunction();
            functionExpress.AddStatements(finallyBlock.Statements);
            return functionExpress;
        }

        public override LuaSyntaxNode VisitTryStatement(TryStatementSyntax node) {
            LuaInvocationExpressionSyntax tryInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Try);

            LuaTryAdapterExpressionSyntax tryBlockFunctionExpress = new LuaTryAdapterExpressionSyntax();
            PushFunction(tryBlockFunctionExpress);
            var block = (LuaBlockSyntax)node.Block.Accept(this);
            PopFunction();
            tryBlockFunctionExpress.AddStatements(block.Statements);
            tryInvocationExpression.AddArgument(tryBlockFunctionExpress);

            if(node.Catches.Count > 0) {
                var catchesExpress = VisitTryCatchesExpress(node.Catches);
                tryInvocationExpression.AddArgument(catchesExpress);
            }
            else {
                tryInvocationExpression.AddArgument(LuaIdentifierNameSyntax.Nil);
            }

            if(node.Finally != null) {
                var finallyfunctionExpress = (LuaFunctionExpressionSyntax)node.Finally.Accept(this);
                tryInvocationExpression.AddArgument(finallyfunctionExpress);
            }

            return BuildCheckReturnInvocationExpression(tryInvocationExpression, node);
        }

        public override LuaSyntaxNode VisitUsingStatement(UsingStatementSyntax node) {
            List<LuaIdentifierNameSyntax> variableIdentifiers = new List<LuaIdentifierNameSyntax>();
            List<LuaExpressionSyntax> variableExpressions = new List<LuaExpressionSyntax>();
            if(node.Declaration != null) {
                var variableList = (LuaVariableListDeclarationSyntax)node.Declaration.Accept(this);
                foreach(var variable in variableList.Variables) {
                    variableIdentifiers.Add(variable.Identifier);
                    variableExpressions.Add(variable.Initializer.Value);
                }
            }
            else {
                var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                variableExpressions.Add(expression);
            }

            LuaUsingAdapterExpressionSyntax usingAdapterExpress = new LuaUsingAdapterExpressionSyntax();
            usingAdapterExpress.ParameterList.Parameters.AddRange(variableIdentifiers.Select(i => new LuaParameterSyntax(i)));
            PushFunction(usingAdapterExpress);
            WriteStatementOrBlock(node.Statement, usingAdapterExpress.Body);
            PopFunction();

            if(variableExpressions.Count == 1) {
                LuaInvocationExpressionSyntax usingInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Using);
                usingInvocationExpression.AddArgument(variableExpressions.First());
                usingInvocationExpression.AddArgument(usingAdapterExpress);
                return BuildCheckReturnInvocationExpression(usingInvocationExpression, node);
            }
            else {
                LuaInvocationExpressionSyntax usingInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.UsingX);
                usingInvocationExpression.AddArgument(usingAdapterExpress);
                usingInvocationExpression.ArgumentList.Arguments.AddRange(variableExpressions.Select(i => new LuaArgumentSyntax(i)));
                return BuildCheckReturnInvocationExpression(usingInvocationExpression, node);
            }
        }

        public override LuaSyntaxNode VisitThisExpression(ThisExpressionSyntax node) {
            return LuaIdentifierNameSyntax.This;
        }

        private bool IsBaseEnable<T>(MemberAccessExpressionSyntax parent, T symbol, Func<T, ISymbol> overriddenFunc) where T : ISymbol {
            if(symbol.IsOverridable()) {
                var curTypeSymbol = GetTypeDeclarationSymbol(parent);
                if(curTypeSymbol.IsSealed) {
                    bool exists = curTypeSymbol.GetMembers().OfType<T>().Any(i => {
                        var overriddenSymbol = overriddenFunc(i);
                        return overriddenSymbol != null && overriddenSymbol.Equals(symbol);
                    });
                    if(!exists) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override LuaSyntaxNode VisitBaseExpression(BaseExpressionSyntax node) {
            var parent = (MemberAccessExpressionSyntax)node.Parent;
            var symbol = semanticModel_.GetSymbolInfo(parent).Symbol;

            bool hasBase = false;
            switch(symbol.Kind) {
                case SymbolKind.Method: {
                        var methodSymbol = (IMethodSymbol)symbol;
                        if(IsBaseEnable(parent, methodSymbol, i => i.OverriddenMethod)) {
                            hasBase = true;
                        }
                        break;
                    }
                case SymbolKind.Property: {
                        var propertySymbol = (IPropertySymbol)symbol;
                        if(!IsPropertyField(propertySymbol)) {
                            if(IsBaseEnable(parent, propertySymbol, i => i.OverriddenProperty)) {
                                hasBase = true;
                            }
                        }
                        break;
                    }
                case SymbolKind.Event: {
                        var eventSymbol = (IEventSymbol)symbol;
                        if(!eventSymbol.IsEventFiled()) {
                            if(IsBaseEnable(parent, eventSymbol, i => i.OverriddenEvent)) {
                                hasBase = true;
                            }
                        }
                        break;
                    }
            }

            if(hasBase) {
                return GetTypeName(symbol.ContainingType);
            }
            else {
                return LuaIdentifierNameSyntax.This;
            }
        }

        private Stack<LuaIdentifierNameSyntax> conditionalTemps_ = new Stack<LuaIdentifierNameSyntax>();

        public override LuaSyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node) {
            var temp = GetTempIdentifier(node.Expression);
            conditionalTemps_.Push(temp);

            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, expression));

            LuaBinaryExpressionSyntax condition = new LuaBinaryExpressionSyntax(temp, LuaSyntaxNode.Tokens.NotEquals, LuaIdentifierNameSyntax.Nil);
            LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
            CurBlock.Statements.Add(ifStatement);

            blocks_.Push(ifStatement.Body);
            var whenNotNull = (LuaExpressionSyntax)node.WhenNotNull.Accept(this);
            blocks_.Pop();
            conditionalTemps_.Pop();

            if(node.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
                ifStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(whenNotNull));
                return LuaExpressionSyntax.EmptyExpression;
            }
            else {
                LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(temp, whenNotNull);
                ifStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                return temp;
            }
        }

        public override LuaSyntaxNode VisitMemberBindingExpression(MemberBindingExpressionSyntax node) {
            ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
            if(node.Name.Identifier.ValueText == "Invoke") {
                if(symbol.ContainingType.IsDelegateType()) {
                    return conditionalTemps_.Peek();
                }
            }

            var nameExpression = (LuaExpressionSyntax)node.Name.Accept(this);
            return new LuaMemberAccessExpressionSyntax(conditionalTemps_.Peek(), nameExpression, symbol.Kind == SymbolKind.Method);
        }

        public override LuaSyntaxNode VisitElementBindingExpression(ElementBindingExpressionSyntax node) {
            var argumentList = (LuaArgumentListSyntax)node.ArgumentList.Accept(this);
            var memberAccess = new LuaMemberAccessExpressionSyntax(conditionalTemps_.Peek(), new LuaIdentifierNameSyntax(LuaSyntaxNode.Tokens.Get), true);
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(memberAccess);
            invocation.ArgumentList.Arguments.AddRange(argumentList.Arguments);
            return invocation;
        }

        public override LuaSyntaxNode VisitDefaultExpression(DefaultExpressionSyntax node) {
            return BuildDefaultValueExpression(node.Type);
        }

        public override LuaSyntaxNode VisitElementAccessExpression(ElementAccessExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            var argumentList = (LuaArgumentListSyntax)node.ArgumentList.Accept(this);
            LuaPropertyOrEventIdentifierNameSyntax identifierName = new LuaPropertyOrEventIdentifierNameSyntax(true, string.Empty);
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(expression, identifierName, true);
            LuaPropertyAdapterExpressionSyntax propertyAdapter = new LuaPropertyAdapterExpressionSyntax(memberAccess, identifierName);
            propertyAdapter.InvocationExpression.ArgumentList.Arguments.AddRange(argumentList.Arguments);
            return propertyAdapter;
        }

        public override LuaSyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node) {
            int index = 0;
            StringBuilder sb = new StringBuilder();
            List<LuaExpressionSyntax> expressions = new List<LuaExpressionSyntax>();
            foreach(var content in node.Contents) {
                if(content.IsKind(SyntaxKind.InterpolatedStringText)) {
                    var stringText = (InterpolatedStringTextSyntax)content;
                    sb.Append(stringText.TextToken.ValueText);
                }
                else {
                    var expression = (LuaExpressionSyntax)content.Accept(this);
                    expressions.Add(expression);
                    sb.Append('{');
                    sb.Append(index);
                    sb.Append('}');
                    ++index;
                }
            }

            LuaLiteralExpressionSyntax format;
            if(node.StringStartToken.ValueText.Contains('@')) {
                format = BuildVerbatimStringExpression(sb.ToString());
            }
            else {
                format = BuildStringLiteralExpression(sb.ToString());
            }
            LuaMemberAccessExpressionSyntax memberAccessExpression = new LuaMemberAccessExpressionSyntax(new LuaParenthesizedExpressionSyntax(format), LuaIdentifierNameSyntax.Format, true);
            return new LuaInvocationExpressionSyntax(memberAccessExpression, expressions);
        }

        public override LuaSyntaxNode VisitInterpolation(InterpolationSyntax node) {
            return node.Expression.Accept(this);
        }

        public override LuaSyntaxNode VisitInterpolatedStringText(InterpolatedStringTextSyntax node) {
            throw new InvalidOperationException();
        }

        public override LuaSyntaxNode VisitAliasQualifiedName(AliasQualifiedNameSyntax node) {
            return node.Name.Accept(this);
        }
    }
}