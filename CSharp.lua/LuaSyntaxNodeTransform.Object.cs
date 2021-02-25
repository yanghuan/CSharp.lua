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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed partial class LuaSyntaxNodeTransform {
    private readonly Stack<LuaIdentifierNameSyntax> conditionalTemps_ = new Stack<LuaIdentifierNameSyntax>();

    private LuaExpressionSyntax GetObjectCreationExpression(IMethodSymbol symbol, BaseObjectCreationExpressionSyntax node) {
      LuaExpressionSyntax creationExpression;
      var expression = GetTypeName(symbol.ContainingType);
      var invokeExpression = BuildObjectCreationInvocation(symbol, expression);
      if (node.ArgumentList != null) {
        var refOrOutArguments = new List<RefOrOutArgument>();
        var arguments = BuildArgumentList(symbol, symbol.Parameters, node.ArgumentList, refOrOutArguments);
        TryRemoveNilArgumentsAtTail(symbol, arguments);
        invokeExpression.AddArguments(arguments);
        if (refOrOutArguments.Count > 0) {
          creationExpression = BuildInvokeRefOrOut(node, invokeExpression, refOrOutArguments);
        } else {
          creationExpression = invokeExpression;
        }
      } else {
        creationExpression = invokeExpression;
      }
      return creationExpression;
    }

    private LuaExpressionSyntax GetObjectCreationInitializer(LuaExpressionSyntax creationExpression, BaseObjectCreationExpressionSyntax node) {
      if (node.Initializer == null) {
        return creationExpression;
      } else {
        int prevTempCount = CurFunction.TempCount;
        var temp = GetTempIdentifier();
        CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp, creationExpression));
        FillObjectInitializerExpression(temp, node.Initializer);
        ReleaseTempIdentifiers(prevTempCount);
        return !node.Parent.IsKind(SyntaxKind.ExpressionStatement) ? temp : LuaExpressionSyntax.EmptyExpression;
      }
    }

    public override LuaSyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node) {
      var constExpression = GetConstExpression(node);
      if (constExpression != null) {
        return constExpression;
      }

      var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
      LuaExpressionSyntax creationExpression;
      if (symbol != null) {
        string codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(symbol);
        if (codeTemplate != null) {
          creationExpression = BuildCodeTemplateExpression(codeTemplate, null, FillCodeTemplateInvocationArguments(symbol, node.ArgumentList, null), symbol.TypeArguments);
        } else if (node.Type.IsKind(SyntaxKind.NullableType)) {
          Contract.Assert(node.ArgumentList.Arguments.Count == 1);
          var argument = node.ArgumentList.Arguments.First();
          return argument.Expression.Accept(this);
        } else if (symbol.ContainingType.IsTupleType) {
          var expressions = node.ArgumentList.Arguments.Select(i => i.Expression.AcceptExpression(this));
          creationExpression = BuildValueTupleCreateExpression(expressions);
        } else {
          creationExpression = GetObjectCreationExpression(symbol, node);
        }
      } else {
        var type = semanticModel_.GetSymbolInfo(node.Type).Symbol;
        if (type != null && type.Kind == SymbolKind.NamedType) {
          var nameType = (INamedTypeSymbol)type;
          if (nameType.IsDelegateType()) {
            Contract.Assert(node.ArgumentList.Arguments.Count == 1);
            var argument = node.ArgumentList.Arguments.First();
            return argument.Expression.Accept(this);
          }
        }

        Contract.Assert(!node.ArgumentList.Arguments.Any());
        var expression = node.Type.AcceptExpression(this);
        creationExpression = new LuaInvocationExpressionSyntax(expression);
      }

      return GetObjectCreationInitializer(creationExpression, node);
    }

    private void FillObjectInitializerExpression(LuaIdentifierNameSyntax temp, InitializerExpressionSyntax node) {
      foreach (var expression in node.Expressions) {
        if (expression.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
          var assignment = (AssignmentExpressionSyntax)expression;
          var left = assignment.Left.Accept(this);
          var right = assignment.Right.AcceptExpression(this);
          if (assignment.Left.IsKind(SyntaxKind.ImplicitElementAccess)) {
            var argumentList = (LuaArgumentListSyntax)left;
            LuaIdentifierNameSyntax methodName = LuaSyntaxNode.Tokens.Set;
            var invocation = temp.MemberAccess(methodName, true).Invocation();
            invocation.ArgumentList.Arguments.AddRange(argumentList.Arguments);
            invocation.AddArgument(right);
            CurBlock.AddStatement(invocation);
          } else {
            var memberAccess = BuildFieldOrPropertyMemberAccessExpression(temp, (LuaExpressionSyntax)left, false);
            var assignmentExpression = BuildLuaSimpleAssignmentExpression(memberAccess, right);
            CurBlock.AddStatement(assignmentExpression);
          }
        } else {
          var symbol = semanticModel_.GetCollectionInitializerSymbolInfo(expression).Symbol;
          var name = GetMemberName(symbol);
          var invocation = temp.MemberAccess(name, true).Invocation();
          var block = new LuaBlockSyntax();
          PushBlock(block);
          if (expression.IsKind(SyntaxKind.ComplexElementInitializerExpression)) {
            var initializer = (InitializerExpressionSyntax)expression;
            foreach (var expressionNode in initializer.Expressions) {
              var argument = expressionNode.AcceptExpression(this);
              invocation.AddArgument(argument);
            }
          } else {
            var value = expression.AcceptExpression(this);
            invocation.AddArgument(value);
          }
          PopBlock();
          CurBlock.Statements.AddRange(block.Statements);
          CurBlock.AddStatement(invocation);
        }
      }
    }

    public override LuaSyntaxNode VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node) {
      LuaIdentifierNameSyntax name;
      var expression = node.Expression.AcceptExpression(this);
      if (node.NameEquals != null) {
        name = node.NameEquals.Accept<LuaIdentifierNameSyntax>(this);
      } else {
        name = (LuaIdentifierNameSyntax)expression;
      }
      return new LuaKeyValueTableItemSyntax(name, expression);
    }

    public override LuaSyntaxNode VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node) {
      var table = new LuaTableExpression();
      foreach (var initializer in node.Initializers) {
        var item = initializer.Accept<LuaKeyValueTableItemSyntax>(this);
        table.Items.Add(item);
      }
      return LuaIdentifierNameSyntax.AnonymousType.Invocation(table);
    }

    public override LuaSyntaxNode VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node) {
      var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
      var creationExpression = GetObjectCreationExpression(symbol, node);
      return GetObjectCreationInitializer(creationExpression, node);
    }

    public override LuaSyntaxNode VisitInitializerExpression(InitializerExpressionSyntax node) {
      Contract.Assert(node.IsKind(SyntaxKind.ArrayInitializerExpression));
      var arrayType = (IArrayTypeSymbol)semanticModel_.GetTypeInfo(node).ConvertedType;
      return BuildArrayTypeFromInitializer(arrayType, node);
    }

    public override LuaSyntaxNode VisitBracketedArgumentList(BracketedArgumentListSyntax node) {
      return BuildArgumentList(node.Arguments);
    }

    public override LuaSyntaxNode VisitImplicitElementAccess(ImplicitElementAccessSyntax node) {
      return node.ArgumentList.Accept(this);
    }

    public override LuaSyntaxNode VisitGenericName(GenericNameSyntax node) {
      ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
      if (symbol.Kind == SymbolKind.Method) {
        return GetMethodNameExpression((IMethodSymbol)symbol, node);
      } else {
        return GetTypeName(symbol);
      }
    }

    public override LuaSyntaxNode VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node) {
      return LuaNumberLiteralExpressionSyntax.Zero;
    }

    public override LuaSyntaxNode VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node) {
      var rankSpecifier = new LuaArrayRankSpecifierSyntax(node.Rank);
      foreach (var size in node.Sizes) {
        var expression = size.AcceptExpression(this);
        rankSpecifier.Sizes.Add(expression);
      }
      return rankSpecifier;
    }

    public override LuaSyntaxNode VisitArrayType(ArrayTypeSyntax node) {
      var arrayType = semanticModel_.GetTypeInfo(node).Type;
      var typeExpress = GetTypeName(arrayType);
      var arrayRankSpecifier = node.RankSpecifiers[0].Accept<LuaArrayRankSpecifierSyntax>(this);
      var arrayTypeAdapter = new LuaArrayTypeAdapterExpressionSyntax(typeExpress, arrayRankSpecifier);
      return arrayTypeAdapter;
    }

    private void FillMultiArrayInitializer(InitializerExpressionSyntax initializer, LuaTableExpression rankSpecifier, List<LuaExpressionSyntax> expressions, bool isFirst) {
      if (isFirst) {
        rankSpecifier.Add(initializer.Expressions.Count);
      }

      int index = 0;
      foreach (var expression in initializer.Expressions) {
        if (expression.IsKind(SyntaxKind.ArrayInitializerExpression)) {
          FillMultiArrayInitializer((InitializerExpressionSyntax)expression, rankSpecifier, expressions, isFirst && index == 0);
        } else {
          var item = expression.AcceptExpression(this);
          expressions.Add(item);
        }
        ++index;
      }
    }

    private LuaExpressionSyntax BuildArrayCreationExpression(LuaArrayTypeAdapterExpressionSyntax arrayType, InitializerExpressionSyntax initializer) {
      if (initializer != null && initializer.Expressions.Count > 0) {
        if (arrayType.IsSimapleArray) {
          var initializerExpressions = initializer.Expressions.Select(i => i.AcceptExpression(this)).ToList();
          return BuildArray(arrayType, initializerExpressions);
        } else {
          var rank = new LuaTableExpression() { IsSingleLine = true };
          var expressions = new List<LuaExpressionSyntax>();
          FillMultiArrayInitializer(initializer, rank, expressions, true);
          return BuildMultiArray(arrayType, rank, expressions);
        }
      } else {
        if (arrayType.IsSimapleArray) {
          var size = arrayType.RankSpecifier.Sizes[0];
          return BuildArray(arrayType, size);
        } else {
          var rank = new LuaTableExpression() { IsSingleLine = true };
          foreach (var size in arrayType.RankSpecifier.Sizes) {
            rank.Add(size);
          }
          return BuildMultiArray(arrayType, rank);
        }
      }
    }

    public override LuaSyntaxNode VisitArrayCreationExpression(ArrayCreationExpressionSyntax node) {
      var arrayType = node.Type.Accept<LuaArrayTypeAdapterExpressionSyntax>(this);
      return BuildArrayCreationExpression(arrayType, node.Initializer);
    }

    private LuaExpressionSyntax BuildArrayTypeFromInitializer(IArrayTypeSymbol arrayType, InitializerExpressionSyntax initializer) {
      var typeExpress = GetTypeName(arrayType);
      var arrayExpression = new LuaArrayTypeAdapterExpressionSyntax(typeExpress, new LuaArrayRankSpecifierSyntax(arrayType.Rank));
      return BuildArrayCreationExpression(arrayExpression, initializer);
    }

    public override LuaSyntaxNode VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node) {
      var arrayType = (IArrayTypeSymbol)semanticModel_.GetTypeInfo(node).Type;
      return BuildArrayTypeFromInitializer(arrayType, node.Initializer);
    }

    private LuaInvocationExpressionSyntax BuildBaseFromThis() {
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemBase, LuaIdentifierNameSyntax.This);
    }

    private LuaInvocationExpressionSyntax BuildCallBaseConstructor(INamedTypeSymbol type, ITypeSymbol baseType, int ctroCounter) {
      var typeName = !generator_.IsSealed(type) ? GetTypeName(baseType) : BuildBaseFromThis();
      var memberAccess = typeName.MemberAccess(LuaIdentifierNameSyntax.Ctor);
      LuaInvocationExpressionSyntax otherCtorInvoke;
      if (ctroCounter > 0) {
        otherCtorInvoke = new LuaInvocationExpressionSyntax(new LuaTableIndexAccessExpressionSyntax(memberAccess, ctroCounter));
      } else {
        otherCtorInvoke = new LuaInvocationExpressionSyntax(memberAccess);
      }
      return otherCtorInvoke;
    }

    private LuaInvocationExpressionSyntax BuildCallBaseConstructor(INamedTypeSymbol typeSymbol) {
      return BuildCallBaseConstructor(typeSymbol, out int _);
    }

    private LuaInvocationExpressionSyntax BuildCallBaseConstructor(INamedTypeSymbol typeSymbol, out int ctroCounter) {
      ctroCounter = 0;
      var baseType = typeSymbol.BaseType;
      if (baseType != null && !baseType.IsSystemObjectOrValueType()) {
        if (baseType.IsFromCode()) {
          if (baseType.InstanceConstructors.Length > 1) {
            ctroCounter = 1;
          }
        }
        var otherCtorInvoke = BuildCallBaseConstructor(typeSymbol, baseType, ctroCounter);
        otherCtorInvoke.AddArgument(LuaIdentifierNameSyntax.This);
        return otherCtorInvoke;
      }
      return null;
    }

    public override LuaSyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node) {
      if (node.Modifiers.IsExtern()) {
        return null;
      }

      IMethodSymbol symbol = semanticModel_.GetDeclaredSymbol(node);
      methodInfos_.Push(new MethodInfo(symbol));

      var document = BuildDocumentationComment(node);
      var attributes = BuildAttributes(node.AttributeLists);
      if (document != null) {
        document.UnIgnore();
      }

      var refOrOutParameters = new List<LuaExpressionSyntax>();
      var function = new LuaConstructorAdapterExpressionSyntax();
      PushFunction(function);
      bool isStatic = node.Modifiers.IsStatic();
      function.IsStatic = isStatic;
      function.AddParameter(LuaIdentifierNameSyntax.This);
      foreach (var parameterNode in node.ParameterList.Parameters) {
        var parameter = parameterNode.Accept<LuaIdentifierNameSyntax>(this);
        function.AddParameter(parameter);
        if (parameterNode.Modifiers.IsOutOrRef()) {
          refOrOutParameters.Add(parameter);
        }
      }

      if (node.Initializer != null) {
        var initializerSymbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node.Initializer).Symbol;
        int ctroIndex = GetConstructorIndex(initializerSymbol);
        LuaInvocationExpressionSyntax otherCtorInvoke = null;
        if (node.Initializer.IsKind(SyntaxKind.ThisConstructorInitializer)) {
          Contract.Assert(ctroIndex != 0);
          if (!symbol.IsCombineImplicitlyCtorMethod(out _)) {
            LuaIdentifierNameSyntax thisCtor = LuaSyntaxNode.GetCtorNameString(ctroIndex);
            otherCtorInvoke = new LuaInvocationExpressionSyntax(thisCtor);
            function.IsInvokeThisCtor = true;
          }
        } else {
          otherCtorInvoke = BuildCallBaseConstructor(symbol.ContainingType, initializerSymbol.ReceiverType, ctroIndex);
        }

        if (otherCtorInvoke != null) {
          otherCtorInvoke.AddArgument(LuaIdentifierNameSyntax.This);
          var refOrOutArguments = new List<RefOrOutArgument>();
          var arguments = BuildArgumentList(initializerSymbol, initializerSymbol.Parameters, node.Initializer.ArgumentList, refOrOutArguments);
          TryRemoveNilArgumentsAtTail(initializerSymbol, arguments);
          otherCtorInvoke.AddArguments(arguments);
          if (refOrOutArguments.Count == 0) {
            function.AddStatement(otherCtorInvoke);
          } else {
            var newExpression = BuildInvokeRefOrOut(node.Initializer, otherCtorInvoke, refOrOutArguments);
            function.AddStatement(newExpression);
          }
        }
      } else if (!isStatic && generator_.IsBaseExplicitCtorExists(symbol.ContainingType.BaseType)) {
        var baseCtorInvoke = BuildCallBaseConstructor(symbol.ContainingType, out _);
        Contract.Assert(baseCtorInvoke != null);
        function.AddStatement(baseCtorInvoke);
      }

      bool isCombineImplicitlyCtorMethod = false;
      if (symbol.IsCombineImplicitlyCtorMethod(out int notNullParameterIndex)) {
        var parameter = function.ParameterList.Parameters[notNullParameterIndex + 1];
        var ifStatement = new LuaIfStatementSyntax(parameter.EqualsEquals(LuaIdentifierNameSyntax.Nil));
        ifStatement.Body.AddStatement(new LuaReturnStatementSyntax());
        function.AddStatement(ifStatement);
        isCombineImplicitlyCtorMethod = true;
      }

      if (node.Body != null) {
        var block = node.Body.Accept<LuaBlockSyntax>(this);
        function.AddStatements(block.Statements);
      } else {
        var bodyExpression = node.ExpressionBody.AcceptExpression(this);
        function.AddStatement(bodyExpression);
      }

      if (refOrOutParameters.Count > 0) {
        var returnStatement = new LuaReturnStatementSyntax();
        returnStatement.Expressions.AddRange(refOrOutParameters);
        function.AddStatement(returnStatement);
      }

      PopFunction();

      if (isStatic) {
        CurType.SetStaticCtor(function, document);
      } else {
        CurType.AddCtor(function, node.ParameterList.Parameters.Count == 0 || isCombineImplicitlyCtorMethod, document);
        bool isExportMetadata = IsCurTypeExportMetadataAll || attributes.Count > 0 || symbol.HasMetadataAttribute();
        if (isExportMetadata) {
          int ctorIndex = GetConstructorIndex(symbol);
          LuaIdentifierNameSyntax name;
          if (ctorIndex == 0) {
            name = LuaIdentifierNameSyntax.Nil;
          } else if (isCombineImplicitlyCtorMethod && symbol.ContainingType.InstanceConstructors.Length == 2) {
            name = LuaIdentifierNameSyntax.Ctor;
          } else {
            name = LuaSyntaxNode.GetCtorNameString(ctorIndex);
          }
          AddMethodMetaData(new MethodDeclarationResult() {
            Symbol = symbol,
            Name = name,
            Function = function,
            Document = document,
            Attributes = attributes,
          });
        }
      }

      methodInfos_.Pop();
      return function;
    }

    public override LuaSyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node) {
      if (node.Body != null || node.ExpressionBody != null) {
        IMethodSymbol ctorSymbol = semanticModel_.GetDeclaredSymbol(node);
        methodInfos_.Push(new MethodInfo(ctorSymbol));

        var function = new LuaFunctionExpressionSyntax();
        PushFunction(function);

        function.AddParameter(LuaIdentifierNameSyntax.This);
        if (node.Body != null) {
          var block = node.Body.Accept<LuaBlockSyntax>(this);
          function.Body.Statements.AddRange(block.Statements);
        } else {
          var bodyExpression = node.ExpressionBody.AcceptExpression(this);
          function.AddStatement(bodyExpression);
        }
        CurType.AddMethod(LuaIdentifierNameSyntax.__GC, function, false);

        PopFunction();
        methodInfos_.Pop();
        return function;
      }

      return null;
    }

    public override LuaSyntaxNode VisitSimpleBaseType(SimpleBaseTypeSyntax node) {
      return node.Type.Accept(this);
    }

    private LuaExpressionSyntax VisitLambdaExpression(IEnumerable<ParameterSyntax> parameters, CSharpSyntaxNode body) {
      var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(body.Parent).Symbol;
      methodInfos_.Push(new MethodInfo(symbol));

      var function = new LuaFunctionExpressionSyntax();
      PushFunction(function);

      if (parameters != null) {
        foreach (var parameter in parameters) {
          var luaParameter = parameter.Accept<LuaIdentifierNameSyntax>(this);
          function.ParameterList.Parameters.Add(luaParameter);
        }
      }

      LuaExpressionSyntax resultExpression = function;
      if (body.IsKind(SyntaxKind.Block)) {
        var block = body.Accept<LuaBlockSyntax>(this);
        function.AddStatements(block.Statements);
      } else {
        var type = (INamedTypeSymbol)semanticModel_.GetTypeInfo(body.Parent).ConvertedType;
        var delegateInvokeMethod = type.DelegateInvokeMethod;
        var expression = body.AcceptExpression(this);
        if (delegateInvokeMethod.ReturnsVoid) {
          if (expression != LuaExpressionSyntax.EmptyExpression) {
            function.AddStatement(expression);
          }
        } else {
          function.AddStatement(new LuaReturnStatementSyntax(expression));
        }
      }

      if (symbol.IsAsync) {
        VisitAsync(symbol.ReturnsVoid, function);
      }

      PopFunction();
      methodInfos_.Pop();

      return resultExpression;
    }

    public override LuaSyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) {
      return VisitLambdaExpression(new ParameterSyntax[] { node.Parameter }, node.Body);
    }

    public override LuaSyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) {
      return VisitLambdaExpression(node.ParameterList.Parameters, node.Body);
    }

    public override LuaSyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) {
      return VisitLambdaExpression(node.ParameterList?.Parameters, node.Body);
    }

    public override LuaSyntaxNode VisitTypeParameter(TypeParameterSyntax node) {
      return (LuaIdentifierNameSyntax)node.Identifier.ValueText;
    }

    public override LuaSyntaxNode VisitTypeOfExpression(TypeOfExpressionSyntax node) {
      var type = semanticModel_.GetTypeInfo(node.Type).Type;
      if (type != null && type.TypeKind == TypeKind.Enum) {
        AddExportEnum(type);
        var typeNameExpression = GetTypeShortName(type);
        return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.TypeOf, typeNameExpression);
      }

      var typeName = node.Type.AcceptExpression(this);
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.TypeOf, typeName);
    }

    public override LuaSyntaxNode VisitThrowStatement(ThrowStatementSyntax node) {
      var invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Throw);
      if (node.Expression != null) {
        var expression = node.Expression.AcceptExpression(this);
        invocationExpression.AddArgument(expression);
      } else {
        var curTryFunction = (LuaTryAdapterExpressionSyntax)CurFunction;
        Contract.Assert(curTryFunction.CatchTemp != null);
        invocationExpression.AddArgument(curTryFunction.CatchTemp);
      }
      return new LuaExpressionStatementSyntax(invocationExpression);
    }

    public override LuaSyntaxNode VisitThrowExpression(ThrowExpressionSyntax node) {
      var invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Throw);
      var expression = node.Expression.AcceptExpression(this);
      invocationExpression.AddArgument(expression);
      return invocationExpression;
    }

    public override LuaSyntaxNode VisitCatchFilterClause(CatchFilterClauseSyntax node) {
      var function = new LuaFunctionExpressionSyntax();
      PushFunction(function);
      var expression = node.FilterExpression.AcceptExpression(this);
      function.AddStatement(new LuaReturnStatementSyntax(expression));
      PopFunction();
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.CatchFilter, function);
    }

    public override LuaSyntaxNode VisitCatchClause(CatchClauseSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitCatchDeclaration(CatchDeclarationSyntax node) {
      return new LuaVariableDeclaratorSyntax(node.Identifier.ValueText);
    }

    private LuaTryAdapterExpressionSyntax VisitTryCatchesExpress(SyntaxList<CatchClauseSyntax> catches) {
      LuaTryAdapterExpressionSyntax functionExpress = new LuaTryAdapterExpressionSyntax();
      PushFunction(functionExpress);
      var temp = GetTempIdentifier();
      functionExpress.CatchTemp = temp;
      functionExpress.AddParameter(temp);

      LuaIfStatementSyntax ifStatement = null;
      LuaInvocationExpressionSyntax filter = null;
      bool hasCatchRoot = false;
      foreach (var catchNode in catches) {
        bool isRootExceptionDeclaration = false;
        LuaExpressionSyntax ifCondition = null;
        if (catchNode.Filter != null) {
          filter = catchNode.Filter.Accept<LuaInvocationExpressionSyntax>(this);
          ifCondition = filter;
        }

        if (catchNode.Declaration != null) {
          var typeName = catchNode.Declaration.Type.Accept<LuaIdentifierNameSyntax>(this);
          var typeSymbol = semanticModel_.GetTypeInfo(catchNode.Declaration.Type).Type;
          if (!typeSymbol.EQ(generator_.SystemExceptionTypeSymbol)) {
            var mathcTypeInvocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Is, temp, typeName);
            if (ifCondition != null) {
              ifCondition = ifCondition.And(mathcTypeInvocation);
            } else {
              ifCondition = mathcTypeInvocation;
            }
          } else {
            if (!catchNode.Declaration.Identifier.IsKind(SyntaxKind.None)) {
              isRootExceptionDeclaration = true;
            }
            hasCatchRoot = true;
          }
        } else {
          hasCatchRoot = true;
        }

        var block = catchNode.Block.Accept<LuaBlockSyntax>(this);
        if (ifCondition != null) {
          LuaBlockSyntax body;
          if (ifStatement == null) {
            ifStatement = new LuaIfStatementSyntax(ifCondition);
            body = ifStatement.Body;
          } else {
            var elseIfStatement = new LuaElseIfStatementSyntax(ifCondition);
            body = elseIfStatement.Body;
            ifStatement.ElseIfStatements.Add(elseIfStatement);
          }
          if (catchNode.Declaration != null && !catchNode.Declaration.Identifier.IsKind(SyntaxKind.None)) {
            var variableDeclarator = catchNode.Declaration.Accept<LuaVariableDeclaratorSyntax>(this);
            variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(temp);
            body.Statements.Add(new LuaLocalVariableDeclaratorSyntax(variableDeclarator));
            if (filter != null) {
              var when = (LuaFunctionExpressionSyntax)filter.Arguments.First();
              when.AddParameter(variableDeclarator.Identifier);
              filter.AddArgument(temp);
            }
          }
          body.Statements.AddRange(block.Statements);
        } else {
          if (isRootExceptionDeclaration) {
            var variableDeclarator = catchNode.Declaration.Accept<LuaVariableDeclaratorSyntax>(this);
            variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(temp);
            block.Statements.Insert(0, new LuaLocalVariableDeclaratorSyntax(variableDeclarator));
          }

          if (ifStatement != null) {
            var elseClause = new LuaElseClauseSyntax();
            elseClause.Body.Statements.AddRange(block.Statements);
            ifStatement.Else = elseClause;
          } else {
            functionExpress.AddStatements(block.Statements);
          }
          break;
        }
      }

      if (ifStatement != null) {
        if (!hasCatchRoot) {
          Contract.Assert(ifStatement.Else == null);
          var rethrowStatement = new LuaReturnStatementSyntax();
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

    private LuaStatementSyntax BuildCheckLoopControlInvocationExpression(LuaInvocationExpressionSyntax invocationExpression, LuaCheckLoopControlExpressionSyntax check) {
      return BuildCheckLoopControlInvocationExpression(invocationExpression, check.HasReturn, check.HasBreak, check.HasContinue);
    }

    private LuaStatementSyntax BuildCheckLoopControlInvocationExpression(LuaInvocationExpressionSyntax invocationExpression, IEnumerable<LuaCheckLoopControlExpressionSyntax> checks) {
      return BuildCheckLoopControlInvocationExpression(invocationExpression, checks.Any(i => i.HasReturn), checks.Any(i => i.HasBreak), checks.Any(i => i.HasContinue));
    }

    private LuaStatementSyntax BuildCheckLoopControlInvocationExpression(LuaInvocationExpressionSyntax invocationExpression, bool hasReturn, bool hasBreak, bool hasContinue) {
      if (!hasReturn && !hasBreak && !hasContinue) {
        return invocationExpression;
      }

      var curMethodInfo = CurMethodInfoOrNull;
      bool isReturnValueExists = hasReturn && curMethodInfo != null && !curMethodInfo.Symbol.ReturnsVoid;
      var status = GetTempIdentifier();
      var returnValue = isReturnValueExists ? GetTempIdentifier() : null;
      var localVariables = new LuaLocalVariablesSyntax();
      localVariables.Variables.Add(status);
      if (isReturnValueExists) {
        localVariables.Variables.Add(returnValue);
      }
      localVariables.Initializer = new LuaEqualsValueClauseListSyntax(invocationExpression);

      var statements = new LuaStatementListSyntax();
      statements.Statements.Add(localVariables);
      LuaIfStatementSyntax ifStatement = null;

      if (hasReturn) {
        ifStatement = new LuaIfStatementSyntax(status);
        var statement = InternalVisitReturnStatement(returnValue);
        ifStatement.Body.Statements.Add(statement);
      }

      if (hasBreak || hasContinue) {
        var condition = status.EqualsEquals(LuaIdentifierLiteralExpressionSyntax.False);
        if (ifStatement != null) {
          var elseIfStatement = new LuaElseIfStatementSyntax(condition);
          elseIfStatement.Body.AddStatement(LuaBreakStatementSyntax.Instance);
          ifStatement.ElseIfStatements.Add(elseIfStatement);
        } else {
          ifStatement = new LuaIfStatementSyntax(condition);
          ifStatement.Body.Statements.Add(LuaBreakStatementSyntax.Instance);
        }
      }

      statements.Statements.Add(ifStatement);
      return statements;
    }

    public override LuaSyntaxNode VisitFinallyClause(FinallyClauseSyntax node) {
      var functionExpress = new LuaFunctionExpressionSyntax();
      PushFunction(functionExpress);
      var finallyBlock = node.Block.Accept<LuaBlockSyntax>(this);
      PopFunction();
      functionExpress.AddStatements(finallyBlock.Statements);
      return functionExpress;
    }

    public override LuaSyntaxNode VisitTryStatement(TryStatementSyntax node) {
      var tryInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Try);

      var tryExpress = new LuaTryAdapterExpressionSyntax();
      PushFunction(tryExpress);
      var block = node.Block.Accept<LuaBlockSyntax>(this);
      PopFunction();
      tryExpress.AddStatements(block.Statements);
      tryInvocationExpression.AddArgument(tryExpress);

      if (node.Catches.Count > 0) {
        var catchesExpress = VisitTryCatchesExpress(node.Catches);
        tryInvocationExpression.AddArgument(catchesExpress);
      } else {
        tryInvocationExpression.AddArgument(LuaIdentifierNameSyntax.Nil);
      }

      var tryExpresses = tryInvocationExpression.ArgumentList.Arguments.OfType<LuaTryAdapterExpressionSyntax>();
      if (node.Finally != null) {
        var finallyfunctionExpress = node.Finally.Accept<LuaFunctionExpressionSyntax>(this);
        tryInvocationExpression.AddArgument(finallyfunctionExpress);
      }

      return BuildCheckLoopControlInvocationExpression(tryInvocationExpression, tryExpresses);
    }

    private LuaStatementSyntax BuildUsingStatement(SyntaxNode node, List<LuaIdentifierNameSyntax> variableIdentifiers, List<LuaExpressionSyntax> variableExpressions, Action<LuaBlockSyntax> writeStatements) {
      var usingAdapterExpress = new LuaUsingAdapterExpressionSyntax();
      usingAdapterExpress.ParameterList.Parameters.AddRange(variableIdentifiers);
      PushFunction(usingAdapterExpress);
      var block = new LuaBlockSyntax();
      writeStatements(block);
      usingAdapterExpress.AddStatements(block.Statements);
      PopFunction();

      if (variableExpressions.Count == 1) {
        var usingInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Using);
        usingInvocationExpression.AddArgument(variableExpressions.First());
        usingInvocationExpression.AddArgument(usingAdapterExpress);
        return BuildCheckLoopControlInvocationExpression(usingInvocationExpression, usingAdapterExpress);
      } else {
        var usingInvocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.UsingX);
        usingInvocationExpression.AddArgument(usingAdapterExpress);
        usingInvocationExpression.ArgumentList.Arguments.AddRange(variableExpressions);
        return BuildCheckLoopControlInvocationExpression(usingInvocationExpression, usingAdapterExpress);
      }
    }

    public override LuaSyntaxNode VisitUsingStatement(UsingStatementSyntax node) {
      var variableIdentifiers = new List<LuaIdentifierNameSyntax>();
      var variableExpressions = new List<LuaExpressionSyntax>();
      if (node.Declaration != null) {
        var variableList = node.Declaration.Accept<LuaVariableListDeclarationSyntax>(this);
        foreach (var variable in variableList.Variables) {
          variableIdentifiers.Add(variable.Identifier);
          variableExpressions.Add(variable.Initializer.Value);
        }
      } else {
        var expression = node.Expression.AcceptExpression(this);
        variableExpressions.Add(expression);
      }

      return BuildUsingStatement(node, variableIdentifiers, variableExpressions, body => WriteStatementOrBlock(node.Statement, body));
    }

    private void ApplyUsingDeclarations(LuaBlockSyntax block, List<int> indexs, BlockSyntax node) {
      int postion = indexs.Count - 1;
      while (postion >= 1 && indexs[postion] == indexs[postion - 1] + 1) {
        --postion;
      }

      var variableIdentifiers = new List<LuaIdentifierNameSyntax>();
      var variableExpressions = new List<LuaExpressionSyntax>();
      for (int i = postion; i < indexs.Count; ++i) {
        int index = indexs[i];
        var localDeclaration = (LuaLocalDeclarationStatementSyntax)block.Statements[index];
        var variableList = (LuaVariableListDeclarationSyntax)localDeclaration.Declaration;
        foreach (var variable in variableList.Variables) {
          variableIdentifiers.Add(variable.Identifier);
          variableExpressions.Add(variable.Initializer.Value);
        }
      }

      int lastIndex = indexs.Last();
      var statements = block.Statements.Skip(lastIndex + 1);
      var usingStatement = BuildUsingStatement(node, variableIdentifiers, variableExpressions, body => body.Statements.AddRange(statements));
      block.Statements.RemoveRange(indexs[postion]);
      block.AddStatement(usingStatement);
      indexs.RemoveRange(postion);

      if (indexs.Count > 0) {
        ApplyUsingDeclarations(block, indexs, node);
      }
    }

    public override LuaSyntaxNode VisitThisExpression(ThisExpressionSyntax node) {
      return LuaIdentifierNameSyntax.This;
    }

    private enum BaseVisitType {
      UseThis,
      UseName,
      UseBase,
    }

    private BaseVisitType CheckBaseVisitType<T>(T symbol, Func<T, ISymbol> overriddenFunc) where T : ISymbol {
      if (symbol.IsOverridable()) {
        var curTypeSymbol = CurTypeSymbol;
        if (generator_.IsSealed(curTypeSymbol)) {
          bool exists = curTypeSymbol.GetMembers().OfType<T>().Any(i => {
            var overriddenSymbol = overriddenFunc(i);
            return overriddenSymbol != null && overriddenSymbol.OriginalDefinition.EQ(symbol.OriginalDefinition);
          });
          return exists ? BaseVisitType.UseBase : BaseVisitType.UseThis;
        } else {
          return BaseVisitType.UseName;
        }
      } else {
        return BaseVisitType.UseThis;
      }
    }

    public override LuaSyntaxNode VisitBaseExpression(BaseExpressionSyntax node) {
      var symbol = semanticModel_.GetSymbolInfo(node.Parent).Symbol;
      BaseVisitType useType = BaseVisitType.UseThis;
      switch (symbol.Kind) {
        case SymbolKind.Method: {
          symbol.IsOverridable();
          var methodSymbol = (IMethodSymbol)symbol;
          useType = CheckBaseVisitType(methodSymbol, i => i.OverriddenMethod);
          break;
        }
        case SymbolKind.Property: {
          var propertySymbol = (IPropertySymbol)symbol;
          if (!IsPropertyField(propertySymbol)) {
            useType = CheckBaseVisitType(propertySymbol, i => i.OverriddenProperty);
          }
          break;
        }
        case SymbolKind.Event: {
          var eventSymbol = (IEventSymbol)symbol;
          if (!IsEventFiled(eventSymbol)) {
            useType = CheckBaseVisitType(eventSymbol, i => i.OverriddenEvent);
          }
          break;
        }
      }

      switch (useType) {
        case BaseVisitType.UseThis:
          return LuaIdentifierNameSyntax.This;

        case BaseVisitType.UseName:
          return GetTypeName(symbol.ContainingType);

        case BaseVisitType.UseBase:
          return BuildBaseFromThis();

        default:
          Contract.Assert(false);
          throw new InvalidOperationException();
      }
    }

    private bool IsReturnVoidConditionalAccessExpression(ConditionalAccessExpressionSyntax node) {
      switch (node.Parent.Kind()) {
        case SyntaxKind.ExpressionStatement: {
          return true;
        }
        case SyntaxKind.ArrowExpressionClause:
        case SyntaxKind.ParenthesizedLambdaExpression:
        case SyntaxKind.SimpleLambdaExpression: {
          if (CurMethodInfoOrNull.Symbol.ReturnsVoid) {
            return true;
          }
          break;
        }
      }
      return false;
    }

    public override LuaSyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node) {
      bool isEmpty = functions_.Count == 0;
      if (isEmpty) {
        LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
        PushFunction(function);
      }

      bool isRoot = !node.Parent.IsKind(SyntaxKind.ConditionalAccessExpression);
      if (isRoot) {
        conditionalTemps_.Push(GetTempIdentifier());
      }

      var temp = conditionalTemps_.Peek();
      var expression = node.Expression.AcceptExpression(this);
      if (isRoot) {
        CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp, expression));
      } else {
        CurBlock.AddStatement(temp.Assignment(expression));
      }

      var ifStatement = new LuaIfStatementSyntax(temp.NotEquals(LuaIdentifierNameSyntax.Nil));
      CurBlock.Statements.Add(ifStatement);

      PushBlock(ifStatement.Body);
      var whenNotNull = node.WhenNotNull.AcceptExpression(this);
      PopBlock();

      if (isRoot) {
        conditionalTemps_.Pop();
        AddReleaseTempIdentifier(temp);
      }

      if (IsReturnVoidConditionalAccessExpression(node)) {
        if (isEmpty) {
          throw new InvalidOperationException();
        }
        if (!node.WhenNotNull.IsKind(SyntaxKind.ConditionalAccessExpression)) {
          ifStatement.Body.AddStatement(whenNotNull);
        }
        return LuaExpressionSyntax.EmptyExpression;
      } else {
        if (!node.WhenNotNull.IsKind(SyntaxKind.ConditionalAccessExpression)) {
          ifStatement.Body.AddStatement(temp.Assignment(whenNotNull));
        }
        if (isEmpty) {
          var function = CurFunction;
          function.AddStatement(new LuaReturnStatementSyntax(temp));
          PopFunction();
          return function.Parenthesized().Invocation();
        } else {
          return temp;
        }
      }
    }

    public override LuaSyntaxNode VisitMemberBindingExpression(MemberBindingExpressionSyntax node) {
      ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
      if (IsDelegateInvoke(symbol, node.Name)) {
        return conditionalTemps_.Peek();
      }
      var nameExpression = node.Name.AcceptExpression(this);
      bool isObjectColon = symbol.Kind == SymbolKind.Method || (symbol.Kind == SymbolKind.Property && !IsPropertyFieldOrEventFiled(symbol));
      return conditionalTemps_.Peek().MemberAccess(nameExpression, isObjectColon);
    }

    public override LuaSyntaxNode VisitElementBindingExpression(ElementBindingExpressionSyntax node) {
      var argumentList = node.ArgumentList.Accept<LuaArgumentListSyntax>(this);
      var invocation = conditionalTemps_.Peek().MemberAccess(LuaSyntaxNode.Tokens.Get, true).Invocation();
      invocation.ArgumentList.Arguments.AddRange(argumentList.Arguments);
      return invocation;
    }

    public override LuaSyntaxNode VisitDefaultExpression(DefaultExpressionSyntax node) {
      var constExpression = GetConstExpression(node);
      if (constExpression != null) {
        return constExpression;
      }

      var type = semanticModel_.GetTypeInfo(node.Type).Type;
      return GetDefaultValueExpression(type);
    }

    private LuaExpressionSyntax BuildCountExpressionForIndex(ExpressionSyntax targetExpression, LuaIdentifierNameSyntax target) {
      var typeSymbol = semanticModel_.GetTypeInfo(targetExpression).Type;
      if (typeSymbol.Kind == SymbolKind.ArrayType) {
        return new LuaCodeTemplateExpressionSyntax("#", target);
      }

      var propertySymbol = typeSymbol.GetMembers("Count").Concat(typeSymbol.GetMembers("Length")).OfType<IPropertySymbol>().Where(i => {
        return !i.IsWriteOnly && i.Type.SpecialType == SpecialType.System_Int32;
      }).First();
      string codeTemplate = XmlMetaProvider.GetProertyCodeTemplate(propertySymbol, true);
      if (codeTemplate != null) {
        return InternalBuildCodeTemplateExpression(codeTemplate, null, null, null, target);
      }

      var name = GetMemberName(propertySymbol);
      if (IsPropertyField(propertySymbol)) {
        return target.MemberAccess(name);
      }

      return new LuaPropertyAdapterExpressionSyntax(target, new LuaPropertyOrEventIdentifierNameSyntax(true, name), true);
    }

    private void UpdateIndexArgumentExpression(ExpressionSyntax targetExpression, LuaPropertyAdapterExpressionSyntax propertyAdapter, bool isIndex) {
      if (propertyAdapter.Expression is not LuaIdentifierNameSyntax target) {
        target = GetTempIdentifier();
        CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(target, propertyAdapter.Expression));
        propertyAdapter.Update(target);
      }
      var argumentExpression = propertyAdapter.ArgumentList.Arguments[0];
      var lengthExpression = BuildCountExpressionForIndex(targetExpression, target);
      if (isIndex) {
        var code = new LuaCodeTemplateExpressionSyntax();
        code.Expressions.Add(lengthExpression);
        code.Expressions.Add(argumentExpression is LuaPrefixUnaryExpressionSyntax ? " " : " + ");
        code.Expressions.Add(argumentExpression);
        argumentExpression = code;
      } else {
        argumentExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.IndexGetOffset, argumentExpression, lengthExpression);
      }
      propertyAdapter.ArgumentList.Arguments[0] = argumentExpression;
    }

    private LuaExpressionSyntax InternalVisitElementAccessExpression(IPropertySymbol symbol, ElementAccessExpressionSyntax node) {
      if (symbol != null) {
        bool isGet = node.IsGetExpressionNode();
        string codeTemplate = XmlMetaProvider.GetProertyCodeTemplate(symbol, isGet);
        if (codeTemplate != null) {
          var arguments = BuildArgumentList(symbol, symbol.Parameters, node.ArgumentList);
          return BuildCodeTemplateExpression(codeTemplate, node.Expression, arguments, null);
        }
      }

      var expression = BuildMemberAccessTargetExpression(node.Expression);
      var baseName = symbol == null ? LuaIdentifierNameSyntax.Empty : GetMemberName(symbol);
      var identifierName = new LuaPropertyOrEventIdentifierNameSyntax(true, baseName);
      var propertyAdapter = new LuaPropertyAdapterExpressionSyntax(expression, identifierName, true);
      if (symbol != null) {
        var arguments = BuildArgumentList(symbol, symbol.Parameters, node.ArgumentList);
        propertyAdapter.ArgumentList.AddArguments(arguments);
      } else {
        var argumentList = node.ArgumentList.Accept<LuaArgumentListSyntax>(this);
        propertyAdapter.ArgumentList.Arguments.AddRange(argumentList.Arguments);
        if (node.ArgumentList.Arguments.Count == 1) {
          var arg = node.ArgumentList.Arguments.First().Expression;
          if (arg.IsKind(SyntaxKind.IndexExpression)) {
            UpdateIndexArgumentExpression(node.Expression, propertyAdapter, true);
          } else {
            var typeSymbol = semanticModel_.GetTypeInfo(arg).Type;
            if (typeSymbol.IsSystemIndex()) {
              UpdateIndexArgumentExpression(node.Expression, propertyAdapter, false);
            } else {
              var parent = node.Parent;
              if (parent.IsKind(SyntaxKind.Argument)) {
                var argument = (ArgumentSyntax)parent;
                if (argument.RefKindKeyword.IsKind(SyntaxKind.RefKeyword)) {
                  var first = propertyAdapter.ArgumentList.Arguments[0];
                  if (!(first is LuaIdentifierNameSyntax)) {
                    var temp = GetTempIdentifier();
                    CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp, first));
                    propertyAdapter.ArgumentList.Arguments[0] = temp;
                  }
                }
              }
            }
          }
        }
      }
      return propertyAdapter;
    }

    public override LuaSyntaxNode VisitElementAccessExpression(ElementAccessExpressionSyntax node) {
      var symbol = semanticModel_.GetSymbolInfo(node).Symbol;
      if (symbol != null && symbol.Kind == SymbolKind.Method) {
        var typeSymbol = semanticModel_.GetTypeInfo(node.Expression).Type;
        IPropertySymbol propertySymbol;
        if (typeSymbol.TypeKind == TypeKind.Array) {
          propertySymbol = null;
        } else {
          propertySymbol = (IPropertySymbol)typeSymbol.GetMembers().First(i => i.IsIndexerProperty());
        }
        return InternalVisitElementAccessExpression(propertySymbol, node);
      } else {
        return InternalVisitElementAccessExpression((IPropertySymbol)symbol, node);
      }
    }

    private LuaExpressionSyntax VisitForamtInterpolatedStringExpression(InterpolatedStringExpressionSyntax node) {
      var sb = new StringBuilder();
      var expressions = new List<LuaExpressionSyntax>();
      foreach (var content in node.Contents) {
        if (content.IsKind(SyntaxKind.InterpolatedStringText)) {
          var identifier = content.Accept<LuaIdentifierNameSyntax>(this);
          sb.Append(identifier.ValueText);
        } else {
          var interpolation = (InterpolationSyntax)content;
          var expression = WrapStringConcatExpression(interpolation.Expression);
          expressions.Add(expression);
          sb.Append("%s");
        }
      }
      LuaLiteralExpressionSyntax format = BuildVerbatimStringExpression(sb.ToString());
      if (expressions.Count == 0) {
        return format;
      }

      var memberAccessExpression = format.Parenthesized().MemberAccess("format", true);
      return new LuaInvocationExpressionSyntax(memberAccessExpression, expressions);
    }

    private LuaExpressionSyntax WrapInterpolatedString(object obj) {
      if (obj is LuaIdentifierNameSyntax s) {
        return new LuaStringLiteralExpressionSyntax(s);
      } else if (obj is ExpressionSyntax e) {
        return WrapStringConcatExpression(e);
      }
      return (LuaBinaryExpressionSyntax)obj;
    }

    private LuaBinaryExpressionSyntax ConcatInterpolatedString(object left, object right) {
      return WrapInterpolatedString(left).Binary(LuaSyntaxNode.Tokens.Concatenation, WrapInterpolatedString(right));
    }

    private LuaExpressionSyntax VisitConcatInterpolatedStringExpression(InterpolatedStringExpressionSyntax node) {
      int index = 0;
      List<object> expressions = new List<object>();
      foreach (var content in node.Contents) {
        if (content.IsKind(SyntaxKind.InterpolatedStringText)) {
          expressions.Add(content.Accept(this));
        } else {
          var interpolation = (InterpolationSyntax)content;
          expressions.Add(interpolation.Expression);
          ++index;
        }
      }

      if (expressions.Count == 1) {
        LuaIdentifierNameSyntax empty = "";
        expressions.Add(empty);
      }

      var resultExpression = (LuaExpressionSyntax)expressions.Aggregate(ConcatInterpolatedString);
      if (node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
        resultExpression = resultExpression.Parenthesized();
      }
      return resultExpression;
    }

    public override LuaSyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node) {
      if (node.StringStartToken.ValueText.Contains('@')) {
        return VisitForamtInterpolatedStringExpression(node);
      }
      return VisitConcatInterpolatedStringExpression(node);
    }

    public override LuaSyntaxNode VisitInterpolation(InterpolationSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitInterpolatedStringText(InterpolatedStringTextSyntax node) {
      string text = node.TextToken.Text.Replace("{{", "{").Replace("}}", "}");
      return (LuaIdentifierNameSyntax)text;
    }

    public override LuaSyntaxNode VisitAliasQualifiedName(AliasQualifiedNameSyntax node) {
      return node.Name.Accept(this);
    }

    private void BuildOperatorMethodDeclaration(BaseMethodDeclarationSyntax node) {
      var symbol = semanticModel_.GetDeclaredSymbol(node);
      methodInfos_.Push(new MethodInfo(symbol));
      bool isPrivate = symbol.IsPrivate();

      var name = GetMemberName(symbol);
      var parameterList = node.ParameterList.Accept<LuaParameterListSyntax>(this);
      var function = new LuaFunctionExpressionSyntax();
      function.ParameterList.Parameters.AddRange(parameterList.Parameters);
      PushFunction(function);

      var comments = BuildDocumentationComment(node);
      if (node.Body != null) {
        var block = node.Body.Accept<LuaBlockSyntax>(this);
        function.AddStatements(block.Statements);
      } else {
        var expression = node.ExpressionBody.AcceptExpression(this);
        if (symbol.ReturnsVoid) {
          function.AddStatement(expression);
        } else {
          function.AddStatement(new LuaReturnStatementSyntax(expression));
        }
      }

      CurType.AddMethod(name, function, isPrivate, comments, IsMoreThanLocalVariables(symbol));
      PopFunction();
      methodInfos_.Pop();
    }

    public override LuaSyntaxNode VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node) {
      BuildOperatorMethodDeclaration(node);
      return base.VisitConversionOperatorDeclaration(node);
    }

    public override LuaSyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node) {
      if (IsExportMethodDeclaration(node)) {
        BuildOperatorMethodDeclaration(node);
      } 
      return base.VisitOperatorDeclaration(node);
    }

    public override LuaSyntaxNode VisitSizeOfExpression(SizeOfExpressionSyntax node) {
      var constExpression = GetConstExpression(node);
      Contract.Assert(constExpression != null);
      return constExpression;
    }

    public override LuaSyntaxNode VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node) {
      var arrayType = node.Type.Accept<LuaArrayTypeAdapterExpressionSyntax>(this);
      var array = BuildArrayCreationExpression(arrayType, node.Initializer);
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.StackAlloc, array);
    }

    public override LuaSyntaxNode VisitUnsafeStatement(UnsafeStatementSyntax node) {
      var statements = new LuaStatementListSyntax();
      statements.Statements.Add(new LuaShortCommentStatement(" " + node.UnsafeKeyword));
      var block = node.Block.Accept<LuaStatementSyntax>(this);
      statements.Statements.Add(block);
      return statements;
    }

    public override LuaSyntaxNode VisitFixedStatement(FixedStatementSyntax node) {
      var statements = new LuaStatementListSyntax();
      statements.Statements.Add(new LuaShortCommentStatement(" " + node.FixedKeyword));
      var block = new LuaBlockStatementSyntax();
      var declaration = node.Declaration.Accept<LuaStatementSyntax>(this);
      block.Statements.Add(declaration);
      WriteStatementOrBlock(node.Statement, block);
      statements.Statements.Add(block);
      return statements;
    }

    public override LuaSyntaxNode VisitLockStatement(LockStatementSyntax node) {
      LuaStatementListSyntax statements = new LuaStatementListSyntax();
      statements.Statements.Add(new LuaShortCommentStatement($" {node.LockKeyword}({node.Expression})"));
      LuaBlockStatementSyntax block = new LuaBlockStatementSyntax();
      WriteStatementOrBlock(node.Statement, block);
      statements.Statements.Add(block);
      return statements;
    }

    public override LuaSyntaxNode VisitArrowExpressionClause(ArrowExpressionClauseSyntax node) {
      return VisitExpression(node.Expression);
    }

    public override LuaSyntaxNode VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) {
      var result = BuildMethodDeclaration(node, default, node.ParameterList, node.TypeParameterList, node.Body, node.ExpressionBody);
      if (node.Modifiers.IsStatic() && IsStaticLocalMethodEnableAddToType(result.Symbol)) {
        CurTypeDeclaration.TypeDeclaration.AddMethod(result.Name, result.Function, true, result.Document);
        return LuaStatementSyntax.Empty;
      } else {
        var body = FindParentMethodBody(node);
        bool isOnlyOne = body == null || body.Statements.OfType<LocalFunctionStatementSyntax>().Count() == 1;
        if (isOnlyOne) {
          return new LuaLocalFunctionSyntx(result.Name, result.Function, result.Document);
        } else {
          CurBlock.AddHeadVariable(result.Name);
          LuaStatementSyntax localVar = result.Name.Assignment(result.Function);
          if (result.Document != null && !result.Document.IsEmpty) {
            var statementList = new LuaStatementListSyntax();
            statementList.Statements.Add(result.Document);
            statementList.Statements.Add(localVar);
            return statementList;
          } else {
            return localVar;
          }
        }
      }
    }

    public override LuaSyntaxNode VisitDeclarationExpression(DeclarationExpressionSyntax node) {
      return node.Designation.Accept(this);
    }

    public override LuaSyntaxNode VisitDiscardDesignation(DiscardDesignationSyntax node) {
      return (LuaIdentifierNameSyntax)node.UnderscoreToken.ValueText;
    }

    public override LuaSyntaxNode VisitSingleVariableDesignation(SingleVariableDesignationSyntax node) {
      LuaIdentifierNameSyntax name = node.Identifier.ValueText;
      CheckLocalVariableName(ref name, node);
      return name;
    }

    private LuaExpressionSyntax BuildIsPatternExpression(ExpressionSyntax leftTypeExpression, ExpressionSyntax rightTypeExpression, LuaExpressionSyntax leftName) {
      var leftType = semanticModel_.GetTypeInfo(leftTypeExpression).Type;
      var rightType = semanticModel_.GetTypeInfo(rightTypeExpression).Type;
      if (leftType.Is(rightType)) {
        if (leftType.IsValueType) {
          return LuaIdentifierLiteralExpressionSyntax.True;
        } else {
          return leftName.NotEquals(LuaIdentifierNameSyntax.Nil);
        }
      } else {
        var type = rightTypeExpression.AcceptExpression(this);
        return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Is, leftName, type);
      }
    }

    private LuaExpressionSyntax BuildIsConstantExpression(LuaExpressionSyntax left, CSharpSyntaxNode right) {
      var constValue = semanticModel_.GetConstantValue(right);
      return BuildIsConstantExpression(left, right, constValue);
    }

    private LuaExpressionSyntax BuildIsConstantExpression(LuaExpressionSyntax leftExpression, CSharpSyntaxNode right, Optional<object> constValue) {
      const string kIsNaNMethodName = "System.Double.IsNaN";

      Contract.Assert(constValue.HasValue);
      if (constValue.Value is double.NaN) {
        return new LuaInvocationExpressionSyntax(kIsNaNMethodName, leftExpression);
      }

      var rightExpression = right.AcceptExpression(this);
      return leftExpression.EqualsEquals(rightExpression);
    }

    private LuaExpressionSyntax BuildPatternExpression(LuaExpressionSyntax targetExpression, PatternSyntax pattern, ExpressionSyntax targetNode) {
      switch (pattern.Kind()) {
        case SyntaxKind.VarPattern: {
          var varPattern = (VarPatternSyntax)pattern;
          if (!varPattern.IsKind(SyntaxKind.DiscardPattern)) {
            var name = varPattern.Designation.Accept<LuaIdentifierNameSyntax>(this);
            CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(name, targetExpression));
          }
          return LuaIdentifierLiteralExpressionSyntax.True;
        }
        case SyntaxKind.ConstantPattern: {
          var constantPattern = (ConstantPatternSyntax)pattern;
          return BuildIsConstantExpression(targetExpression, constantPattern.Expression);
        }
        case SyntaxKind.DeclarationPattern: {
          var declarationPattern = (DeclarationPatternSyntax)pattern;
          var name = declarationPattern.Designation.Accept<LuaIdentifierNameSyntax>(this);
          CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(name, targetExpression));
          return BuildIsPatternExpression(targetNode, declarationPattern.Type, name);
        }
        case SyntaxKind.NotPattern: {
          var notPattern = (UnaryPatternSyntax)pattern;
          switch (notPattern.Pattern.Kind()) {
            case SyntaxKind.TypePattern: {
              var typePattern = (TypePatternSyntax)notPattern.Pattern;
              var expression = BuildIsPatternExpression(targetNode, typePattern.Type, targetExpression);
              return expression.Not();
            }
            case SyntaxKind.DeclarationPattern: {
              var declarationPattern = (DeclarationPatternSyntax)notPattern.Pattern;
              var name = declarationPattern.Designation.Accept<LuaIdentifierNameSyntax>(this);
              CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(name, targetExpression));
              var expression = BuildIsPatternExpression(targetNode, declarationPattern.Type, name);
              return expression.Not();
            }
            default: {
              var expression = notPattern.Pattern.AcceptExpression(this);
              return targetExpression.NotEquals(expression);
            }
          }
        }
        case SyntaxKind.AndPattern:
        case SyntaxKind.OrPattern: {
          var binaryPattern = (BinaryPatternSyntax)pattern;
          var name = GetIdentifierNameFromExpression(targetExpression);
          var left = BuildPatternExpression(name, binaryPattern.Left, targetNode);
          var right = BuildPatternExpression(name, binaryPattern.Right, targetNode);
          return pattern.IsKind(SyntaxKind.AndPattern) ? left.And(right) : left.Or(right);
        }
        case SyntaxKind.RelationalPattern: {
          var relationalPattern = (RelationalPatternSyntax)pattern;
          var right = relationalPattern.Expression.AcceptExpression(this);
          string token = relationalPattern.OperatorToken.ValueText;
          return targetExpression.Binary(token, right);
        }
        default: {
          var recursivePattern = (RecursivePatternSyntax)pattern;
          LuaIdentifierNameSyntax name;
          if (recursivePattern.Designation != null) {
            name = recursivePattern.Designation.Accept<LuaIdentifierNameSyntax>(this);
            CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(name, targetExpression));
          } else {
            name = GetIdentifierNameFromExpression(targetExpression);
          }
          return BuildRecursivePatternExpression(recursivePattern, name, null, targetNode);
        }
      }
    }

    public override LuaSyntaxNode VisitIsPatternExpression(IsPatternExpressionSyntax node) {
      var expression = node.Expression.AcceptExpression(this);
      return BuildPatternExpression(expression, node.Pattern, node.Expression);
    }

    private LuaIdentifierNameSyntax GetIdentifierNameFromExpression(LuaExpressionSyntax expression) {
      LuaIdentifierNameSyntax name;
      if (expression is LuaIdentifierNameSyntax identifierName) {
        name = identifierName;
      } else {
        name = GetTempIdentifier();
        CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(name, expression));
      }
      return name;
    }

    public override LuaSyntaxNode VisitDeclarationPattern(DeclarationPatternSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitRefExpression(RefExpressionSyntax node) {
      return node.Expression.Accept(this);
    }

    public override LuaSyntaxNode VisitTupleType(TupleTypeSyntax node) {
      return LuaIdentifierNameSyntax.ValueTuple;
    }

    private LuaExpressionSyntax BuildValueTupleCreateExpression(IEnumerable<LuaExpressionSyntax> expressions) {
      return LuaIdentifierNameSyntax.ValueTuple.Invocation(expressions);
    }

    public override LuaSyntaxNode VisitTupleExpression(TupleExpressionSyntax node) {
      var expressions = node.Arguments.Select(i => i.Expression.AcceptExpression(this));
      switch (node.Parent.Kind()) {
        case SyntaxKind.SimpleAssignmentExpression: {
          var assigment = (AssignmentExpressionSyntax)node.Parent;
          if (assigment.Left == node) {
            if (node.Arguments.Any(i => i.Expression.IsKind(SyntaxKind.DeclarationExpression))) {
              return new LuaLocalTupleVariableExpression(expressions.Cast<LuaIdentifierNameSyntax>());
            }
            return new LuaSequenceListExpressionSyntax(expressions);
          }

          if (assigment.Right.IsKind(SyntaxKind.TupleExpression) && assigment.Left.IsKind(SyntaxKind.TupleExpression)) {
            return new LuaSequenceListExpressionSyntax(expressions);
          }
          break;
        }
        case SyntaxKind.ForEachVariableStatement: {
          return new LuaLocalTupleVariableExpression(expressions.Cast<LuaIdentifierNameSyntax>());
        }
      }
      return BuildValueTupleCreateExpression(expressions);
    }

    public override LuaSyntaxNode VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node) {
      var expression = new LuaLocalTupleVariableExpression();
      expression.Variables.AddRange(node.Variables.Select(i => i.Accept<LuaIdentifierNameSyntax>(this)));
      return expression;
    }

    public override LuaSyntaxNode VisitAwaitExpression(AwaitExpressionSyntax node) {
      var type = semanticModel_.GetTypeInfo(node.Expression).Type;
      var methodName = type.IsSystemTask() ? LuaIdentifierNameSyntax.Await : LuaIdentifierNameSyntax.AwaitAnything;
      var expression = node.Expression.AcceptExpression(this);
      return LuaIdentifierNameSyntax.Async.MemberAccess(methodName, true).Invocation(expression);
    }

    public override LuaSyntaxNode VisitRangeExpression(RangeExpressionSyntax node) {
      var left = node.LeftOperand.AcceptExpression(this);
      var right = node.RightOperand.AcceptExpression(this);
      return LuaIdentifierNameSyntax.Range.Invocation(left, right);
    }

    public override LuaSyntaxNode VisitWithExpression(WithExpressionSyntax node) {
      var expression = node.Expression.AcceptExpression(this);
      var temp = GetTempIdentifier();
      CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp, expression.MemberAccess(LuaIdentifierNameSyntax.Clone, true).Invocation()));
      FillObjectInitializerExpression(temp, node.Initializer);
      return temp;
    }
  }
}
