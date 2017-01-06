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

            var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
            int index = GetConstructorIndex(symbol);
            if(index > 0) {
                expression = new LuaMemberAccessExpressionSyntax(expression, LuaIdentifierNameSyntax.New, true);
            }

            var argumentList = (LuaArgumentListSyntax)node.ArgumentList.Accept(this);
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(expression);
            if(index > 0) {
                invocationExpression.AddArgument(new LuaIdentifierNameSyntax(index.ToString()));
            }
            invocationExpression.ArgumentList.Arguments.AddRange(argumentList.Arguments);
            if(node.Initializer == null) {
                return invocationExpression;
            }
            else {
                var functionExpression = (LuaFunctionExpressionSyntax)node.Initializer.Accept(this);
                return BuildBinaryInvokeExpression(invocationExpression, functionExpression, LuaIdentifierNameSyntax.Create);
            }
        }

        public override LuaSyntaxNode VisitInitializerExpression(InitializerExpressionSyntax node) {
            LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
            function.AddParameter(LuaIdentifierNameSyntax.This);
            foreach(var expression in node.Expressions) {
                if(expression.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
                    AssignmentExpressionSyntax assignment = (AssignmentExpressionSyntax)expression;
                    var identifierName = (LuaIdentifierNameSyntax)assignment.Left.Accept(this);
                    Contract.Assert(identifierName != null);
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
            return function;
        }

        public override LuaSyntaxNode VisitGenericName(GenericNameSyntax node) {
            SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
            ISymbol symbol = symbolInfo.Symbol;
            if(symbol.Kind == SymbolKind.Method) {
                return GetMethodNameExpression((IMethodSymbol)symbol, node);
            }
            else {
                string name = XmlMetaProvider.GetTypeMapName(symbol);
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
                    string typeName = XmlMetaProvider.GetTypeMapName(symbol.ReceiverType);
                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(new LuaIdentifierNameSyntax(typeName), LuaIdentifierNameSyntax.Ctor);
                    if(ctroCounter > 0) {
                        otherCtorInvoke = new LuaInvocationExpressionSyntax(new LuaTableIndexAccessExpressionSyntax(memberAccess, new LuaIdentifierNameSyntax(ctroCounter.ToString())));
                    }
                    else {
                        otherCtorInvoke = new LuaInvocationExpressionSyntax(memberAccess);
                    }
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
                function.Body.Statements.AddRange(block.Statements);
            }
            else {
                blocks_.Push(function.Body);
                var expression = (LuaExpressionSyntax)body.Accept(this);
                blocks_.Pop();
                function.Body.Statements.Add(new LuaReturnStatementSyntax(expression));
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
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.TypeOf);
            var typeName = (LuaIdentifierNameSyntax)node.Type.Accept(this);
            invocation.AddArgument(typeName);
            return invocation;
        }

        public override LuaSyntaxNode VisitThrowStatement(ThrowStatementSyntax node) {
            LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Throw);
            if(node.Expression != null) {
                var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
                invocationExpression.AddArgument(expression);
            }
            else {
                var curTryFunction = (LuaTryBlockAdapterExpressionSyntax)CurFunction;
                Contract.Assert(curTryFunction.CatchTemp != null);
                invocationExpression.AddArgument(curTryFunction.CatchTemp);
            }
            return new LuaExpressionStatementSyntax(invocationExpression);
        }

        public override LuaSyntaxNode VisitCatchFilterClause(CatchFilterClauseSyntax node) {
            return node.FilterExpression.Accept(this);    
        }

        private LuaTryBlockAdapterExpressionSyntax VisitTryCatchesExpress(SyntaxList<CatchClauseSyntax> catches) {
            LuaTryBlockAdapterExpressionSyntax functionExpress = new LuaTryBlockAdapterExpressionSyntax();
            PushFunction(functionExpress);
            var temp = GetTempIdentifier(catches.First());
            functionExpress.CatchTemp = temp;
            functionExpress.AddParameter(temp);

            LuaIfStatementSyntax ifHeadStatement = null;
            LuaIfStatementSyntax ifTailStatement = null;
            bool hasCatchRoot = false;
            foreach(var catchNode in catches) {
                LuaExpressionSyntax ifCondition = null;
                if(catchNode.Filter != null) {
                    ifCondition = (LuaExpressionSyntax)catchNode.Filter.Accept(this);
                }
                if(catchNode.Declaration != null) {
                    var typeName = (LuaIdentifierNameSyntax)catchNode.Declaration.Type.Accept(this);
                    if(typeName.ValueText != "System.Exception") {
                        var mathcTypeInvocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Is);
                        mathcTypeInvocation.AddArgument(temp);
                        mathcTypeInvocation.AddArgument(typeName);
                        if(ifCondition != null) {
                            ifCondition = new LuaBinaryExpressionSyntax(ifCondition, LuaSyntaxNode.Tokens.And, mathcTypeInvocation);
                        }
                        else {
                            ifCondition = mathcTypeInvocation;
                        }
                    }
                    else {
                        hasCatchRoot = true;
                    }
                }
                else {
                    hasCatchRoot = true;
                }

                var block = (LuaBlockSyntax)catchNode.Block.Accept(this);
                if(ifCondition != null) {
                    LuaIfStatementSyntax statement = new LuaIfStatementSyntax(ifCondition);
                    if(catchNode.Declaration != null && !catchNode.Declaration.Identifier.IsKind(SyntaxKind.None)) {
                        var variableDeclarator = new LuaVariableDeclaratorSyntax(new LuaIdentifierNameSyntax(catchNode.Declaration.Identifier.ValueText));
                        variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(temp);
                        statement.Body.Statements.Add(new LuaLocalVariableDeclaratorSyntax(variableDeclarator));
                    }
                    statement.Body.Statements.AddRange(block.Statements);
                    if(ifTailStatement != null) {
                        ifTailStatement.Else = new LuaElseClauseSyntax(statement);
                    }
                    else {
                        ifHeadStatement = statement;
                    }
                    ifTailStatement = statement;
                }
                else {
                    if(ifTailStatement != null) {
                        ifTailStatement.Else = new LuaElseClauseSyntax(block);
                    }
                    else {
                        functionExpress.Body.Statements.AddRange(block.Statements);
                    }
                    break;
                }
            }

            if(ifHeadStatement != null) {
                if(!hasCatchRoot) {
                    Contract.Assert(ifTailStatement.Else == null);
                    LuaMultipleReturnStatementSyntax rethrowStatement = new LuaMultipleReturnStatementSyntax();
                    rethrowStatement.Expressions.Add(LuaIdentifierNameSyntax.One);
                    rethrowStatement.Expressions.Add(temp);
                    LuaBlockSyntax block = new LuaBlockSyntax();
                    block.Statements.Add(rethrowStatement);
                    ifTailStatement.Else = new LuaElseClauseSyntax(block);
                }
                functionExpress.Body.Statements.Add(ifHeadStatement);
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
                if(CurFunction is LuaSpecialAdapterFunctionExpressionSyntax) {
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
            functionExpress.Body.Statements.AddRange(finallyBlock.Statements);
            return functionExpress;
        }

        public override LuaSyntaxNode VisitTryStatement(TryStatementSyntax node) {
            LuaInvocationExpressionSyntax tryInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Try);

            LuaTryBlockAdapterExpressionSyntax tryBlockFunctionExpress = new LuaTryBlockAdapterExpressionSyntax();
            PushFunction(tryBlockFunctionExpress);
            var block = (LuaBlockSyntax)node.Block.Accept(this);
            PopFunction();
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
                var finallyfunctionExpress = (LuaFunctionExpressionSyntax)node.Finally.Accept(this);
                tryInvocationExpression.AddArgument(finallyfunctionExpress);
            }

            return BuildCheckReturnInvocationExpression(tryInvocationExpression, node);
        }

        public override LuaSyntaxNode VisitUsingStatement(UsingStatementSyntax node) {
            LuaInvocationExpressionSyntax usingInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Using);

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

            usingInvocationExpression.AddArgument(usingAdapterExpress);
            usingInvocationExpression.ArgumentList.Arguments.AddRange(variableExpressions.Select(i => new LuaArgumentSyntax(i)));
            return BuildCheckReturnInvocationExpression(usingInvocationExpression, node);
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
                string typeName = XmlMetaProvider.GetTypeMapName(symbol.ContainingType);
                return new LuaIdentifierNameSyntax(typeName);
            }
            else {
                return LuaIdentifierNameSyntax.This;
            }
        }

        public override LuaSyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            var whenNotNull = (LuaExpressionSyntax)node.WhenNotNull.Accept(this);
            return BuildBinaryInvokeExpression(expression, whenNotNull, LuaIdentifierNameSyntax.Access);
        }

        public override LuaSyntaxNode VisitMemberBindingExpression(MemberBindingExpressionSyntax node) {
            var temp = GetTempIdentifier(node.Name);
            var nameExpression = (LuaExpressionSyntax)node.Name.Accept(this);
            LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
            functionExpression.AddParameter(temp);
            functionExpression.Body.Statements.Add(new LuaReturnStatementSyntax(nameExpression));
            return new LuaSimpleLambdaAdapterExpression(functionExpression);
        }
    }
}