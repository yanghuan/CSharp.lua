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
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed partial class LuaSyntaxNodeTransform : CSharpSyntaxVisitor<LuaSyntaxNode> {
    private const int kStringConstInlineCount = 15;

    private sealed class MethodInfo {
      public IMethodSymbol Symbol { get; }
      public IList<LuaExpressionSyntax> RefOrOutParameters { get; }

      public MethodInfo(IMethodSymbol symbol) {
        Symbol = symbol;
        RefOrOutParameters = Array.Empty<LuaExpressionSyntax>();
      }

      public MethodInfo(IMethodSymbol symbol, IList<LuaExpressionSyntax> refOrOutParameters) {
        Symbol = symbol;
        RefOrOutParameters = refOrOutParameters;
      }
    }

    internal sealed class TypeDeclarationInfo {
      public INamedTypeSymbol TypeSymbol { get; }
      public LuaTypeDeclarationSyntax TypeDeclaration { get; }

      public TypeDeclarationInfo(INamedTypeSymbol typeSymbol, LuaTypeDeclarationSyntax luaTypeDeclarationSyntax) {
        TypeSymbol = typeSymbol;
        TypeDeclaration = luaTypeDeclarationSyntax;
      }

      public bool CheckTypeName(INamedTypeSymbol getNameTypeSymbol, out LuaIdentifierNameSyntax name) {
        if (getNameTypeSymbol.Equals(TypeSymbol)) {
          TypeDeclaration.IsClassUsed = true;
          name = LuaIdentifierNameSyntax.Class;
          return true;
        }
        name = null;
        return false;
      }
    }

    private LuaSyntaxGenerator generator_;
    private SemanticModel semanticModel_;

    private Stack<LuaCompilationUnitSyntax> compilationUnits_ = new Stack<LuaCompilationUnitSyntax>();
    private Stack<TypeDeclarationInfo> typeDeclarations_ = new Stack<TypeDeclarationInfo>();
    private Stack<LuaFunctionExpressionSyntax> functions_ = new Stack<LuaFunctionExpressionSyntax>();
    private Stack<MethodInfo> methodInfos_ = new Stack<MethodInfo>();
    private Stack<LuaBlockSyntax> blocks_ = new Stack<LuaBlockSyntax>();
    private Stack<LuaIfStatementSyntax> ifStatements_ = new Stack<LuaIfStatementSyntax>();
    private Stack<LuaSwitchAdapterStatementSyntax> switchs_ = new Stack<LuaSwitchAdapterStatementSyntax>();
    private int inheritNameNodeCounter_;
    public bool IsGetInheritTypeName => inheritNameNodeCounter_ > 0;

    private static readonly Dictionary<string, string> operatorTokenMapps_ = new Dictionary<string, string>() {
      ["!="] = LuaSyntaxNode.Tokens.NotEquals,
      ["!"] = LuaSyntaxNode.Tokens.Not,
      ["&&"] = LuaSyntaxNode.Tokens.And,
      ["||"] = LuaSyntaxNode.Tokens.Or,
      ["??"] = LuaSyntaxNode.Tokens.Or,
      ["^"] = LuaSyntaxNode.Tokens.BitXor,
    };

    public LuaSyntaxNodeTransform(LuaSyntaxGenerator generator, SemanticModel semanticModel) {
      generator_ = generator;
      semanticModel_ = semanticModel;
    }

    private XmlMetaProvider XmlMetaProvider {
      get {
        return generator_.XmlMetaProvider;
      }
    }

    private static string GetOperatorToken(SyntaxToken operatorToken) {
      string token = operatorToken.ValueText;
      return operatorTokenMapps_.GetOrDefault(token, token);
    }

    private static string GetOperatorToken(string token) {
      return operatorTokenMapps_.GetOrDefault(token, token);
    }

    private bool IsLuaNewest {
      get {
        return generator_.Setting.IsNewest;
      }
    }

    private LuaCompilationUnitSyntax CurCompilationUnit {
      get {
        return compilationUnits_.Peek();
      }
    }

    private LuaTypeDeclarationSyntax CurType {
      get {
        return typeDeclarations_.Peek().TypeDeclaration;
      }
    }

    private INamedTypeSymbol CurTypeSymbol  {
     get {
        return typeDeclarations_.Peek().TypeSymbol;
      }
    }

    internal TypeDeclarationInfo CurTypeDeclaration{
      get {
        return typeDeclarations_.Count > 0 ? typeDeclarations_.Peek() : null;
      }
    }

    private LuaFunctionExpressionSyntax CurFunction {
      get {
        return functions_.Peek();
      }
    }

    private LuaFunctionExpressionSyntax CurFunctionOrNull {
      get {
        return functions_.Count > 0 ? functions_.Peek() : null;
      }
    }

    private MethodInfo CurMethodInfoOrNull {
      get {
        return methodInfos_.Count > 0 ? methodInfos_.Peek() : null;
      }
    }

    private void PushFunction(LuaFunctionExpressionSyntax function) {
      functions_.Push(function);
      ++localMappingCounter_;
    }

    private void PopFunction() {
      functions_.Pop();
      --localMappingCounter_;
      if (localMappingCounter_ == 0) {
        localReservedNames_.Clear();
      }
    }

    private LuaBlockSyntax CurBlock {
      get {
        return blocks_.Peek();
      }
    }

    private LuaBlockSyntax CurBlockOrNull {
      get {
        return blocks_.Count > 0 ? blocks_.Peek() : null;
      }
    }

    public override LuaSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node) {
      LuaCompilationUnitSyntax compilationUnit = new LuaCompilationUnitSyntax(node.SyntaxTree.FilePath);
      compilationUnits_.Push(compilationUnit);

      var statements = VisitTriviaAndNode(node, node.Members, false);
      foreach (var statement in statements) {
        if (statement is LuaTypeDeclarationSyntax typeeDeclaration) {
          var ns = new LuaNamespaceDeclarationSyntax(LuaIdentifierNameSyntax.Empty);
          ns.AddStatement(typeeDeclaration);
          compilationUnit.AddStatement(ns);
        } else {
          compilationUnit.AddStatement(statement);
        }
      }

      compilationUnits_.Pop();
      return compilationUnit;
    }

    public override LuaSyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
      var symbol = semanticModel_.GetDeclaredSymbol(node);
      bool isContained = node.Parent.IsKind(SyntaxKind.NamespaceDeclaration);
      LuaIdentifierNameSyntax name = new LuaIdentifierNameSyntax(isContained ? symbol.Name : symbol.ToString());
      LuaNamespaceDeclarationSyntax namespaceDeclaration = new LuaNamespaceDeclarationSyntax(name, isContained);
      var statements = VisitTriviaAndNode(node, node.Members);
      namespaceDeclaration.AddStatements(statements);
      return namespaceDeclaration;
    }

    private void BuildTypeMembers(LuaTypeDeclarationSyntax typeDeclaration, TypeDeclarationSyntax node) {
      if (!node.IsKind(SyntaxKind.InterfaceDeclaration)) {
        foreach (var member in node.Members) {
          var newMember = member.Accept(this);
          SyntaxKind kind = member.Kind();
          if (kind >= SyntaxKind.ClassDeclaration && kind <= SyntaxKind.EnumDeclaration) {
            typeDeclaration.AddStatement((LuaTypeDeclarationSyntax)newMember);
          }
        }
      }
    }

    private void CheckBaseTypeGenericKind(ref bool hasExtendSelf, INamedTypeSymbol typeSymbol, BaseTypeSyntax baseType) {
      if (!hasExtendSelf) {
        if (baseType.IsKind(SyntaxKind.SimpleBaseType)) {
          var baseNode = (SimpleBaseTypeSyntax)baseType;
          if (baseNode.Type.IsKind(SyntaxKind.GenericName)) {
            var baseGenericNameNode = (GenericNameSyntax)baseNode.Type;
            var baseTypeSymbol = (INamedTypeSymbol)semanticModel_.GetTypeInfo(baseGenericNameNode).Type;
            hasExtendSelf = Utility.IsExtendSelf(typeSymbol, baseTypeSymbol);
          }
        }
      }
    }

    private LuaSpeaicalGenericType CheckSpeaicalGenericArgument(INamedTypeSymbol typeSymbol) {
      var interfaceType = typeSymbol.Interfaces.FirstOrDefault(i => i.IsGenericIEnumerableType());
      if (interfaceType != null) {
        bool isBaseImplementation = typeSymbol.BaseType != null && typeSymbol.BaseType.AllInterfaces.Any(i => i.IsGenericIEnumerableType());
        if (!isBaseImplementation) {
          var argumentType = interfaceType.TypeArguments.First();
          var typeName = GetTypeName(argumentType);
          return new LuaSpeaicalGenericType() {
            Name = LuaIdentifierNameSyntax.GenericT,
            Value = typeName,
            IsLazy = argumentType.Kind != SymbolKind.TypeParameter && argumentType.IsFromCode(),
          };
        }
      }
      return null;
    }

    private void BuildTypeDeclaration(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
      typeDeclarations_.Push(new TypeDeclarationInfo(typeSymbol, typeDeclaration));

      var comments = BuildDocumentationComment(node);
      typeDeclaration.AddDocument(comments);

      var attributes = BuildAttributes(node.AttributeLists);
      typeDeclaration.AddClassAttributes(attributes);

      BuildTypeParameters(typeSymbol, node, typeDeclaration);
      if (node.BaseList != null) {
        bool hasExtendSelf = false;
        List<LuaExpressionSyntax> baseTypes = new List<LuaExpressionSyntax>();
        foreach (var baseType in node.BaseList.Types) {
          var baseTypeName = BuildInheritTypeName(baseType);
          baseTypes.Add(baseTypeName);
          CheckBaseTypeGenericKind(ref hasExtendSelf, typeSymbol, baseType);
        }
        var genericArgument = CheckSpeaicalGenericArgument(typeSymbol);
        typeDeclaration.AddBaseTypes(baseTypes, genericArgument);
        if (hasExtendSelf && !typeSymbol.HasStaticCtor()) {
          typeDeclaration.SetStaticCtorEmpty();
        }
      }

      BuildTypeMembers(typeDeclaration, node);
      CheckTypeDeclaration(typeSymbol, typeDeclaration);

      typeDeclarations_.Pop();
      CurCompilationUnit.AddTypeDeclarationCount();
    }

    private void CheckTypeDeclaration(INamedTypeSymbol typeSymbol, LuaTypeDeclarationSyntax typeDeclaration) {
      if (typeDeclaration.IsNoneCtros) {
        var bseTypeSymbol = typeSymbol.BaseType;
        if (bseTypeSymbol != null) {
          bool isNeedCallBase;
          if (typeDeclaration.IsInitStatementExists) {
            isNeedCallBase = !bseTypeSymbol.Constructors.IsEmpty;
          } else {
            isNeedCallBase = generator_.HasStaticCtor(bseTypeSymbol) || bseTypeSymbol.Constructors.Count() > 1;
          }
          if (isNeedCallBase) {
            var baseCtorInvoke = BuildCallBaseConstructor(typeSymbol);
            if (baseCtorInvoke != null) {
              LuaConstructorAdapterExpressionSyntax function = new LuaConstructorAdapterExpressionSyntax();
              function.AddParameter(LuaIdentifierNameSyntax.This);
              function.AddStatement(baseCtorInvoke);
              typeDeclaration.AddCtor(function, false);
            }
          }
        }
      } else if (typeSymbol.IsValueType) {
        LuaConstructorAdapterExpressionSyntax function = new LuaConstructorAdapterExpressionSyntax();
        function.AddParameter(LuaIdentifierNameSyntax.This);
        typeDeclaration.AddCtor(function, true);
      }

      if (typeSymbol.IsValueType) {
        TryAddStructDefaultMethod(typeSymbol, typeDeclaration);
      }

      if (typeDeclaration.IsIgnoreExport) {
        generator_.AddIgnoreExportType(typeSymbol);
      }
    }

    private INamedTypeSymbol VisitTypeDeclaration(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
      if (node.Modifiers.IsPartial()) {
        if (typeSymbol.DeclaringSyntaxReferences.Length > 1) {
          generator_.AddPartialTypeDeclaration(typeSymbol, node, typeDeclaration, CurCompilationUnit);
          typeDeclaration.IsPartialMark = true;
        } else {
          BuildTypeDeclaration(typeSymbol, node, typeDeclaration);
        }
      } else {
        BuildTypeDeclaration(typeSymbol, node, typeDeclaration);
      }
      return typeSymbol;
    }

    internal void AcceptPartialType(PartialTypeDeclaration major, List<PartialTypeDeclaration> typeDeclarations) {
      compilationUnits_.Push(major.CompilationUnit);
      typeDeclarations_.Push(new TypeDeclarationInfo(major.Symbol, major.TypeDeclaration));

      List<LuaExpressionSyntax> attributes = new List<LuaExpressionSyntax>();
      foreach (var typeDeclaration in typeDeclarations) {
        semanticModel_ = generator_.GetSemanticModel(typeDeclaration.Node.SyntaxTree);
        var document = BuildDocumentationComment(typeDeclaration.Node);
        major.TypeDeclaration.AddDocument(document);

        var expressions = BuildAttributes(typeDeclaration.Node.AttributeLists);
        attributes.AddRange(expressions);
      }
      major.TypeDeclaration.AddClassAttributes(attributes);

      BuildTypeParameters(major.Symbol, major.Node, major.TypeDeclaration);
      List<BaseTypeSyntax> baseTypes = new List<BaseTypeSyntax>();
      HashSet<ITypeSymbol> baseSymbols = new HashSet<ITypeSymbol>();
      foreach (var typeDeclaration in typeDeclarations) {
        if (typeDeclaration.Node.BaseList != null) {
          foreach (var baseTypeSyntax in typeDeclaration.Node.BaseList.Types) {
            var semanticModel = generator_.GetSemanticModel(baseTypeSyntax.SyntaxTree);
            var baseTypeSymbol = semanticModel.GetTypeInfo(baseTypeSyntax.Type).Type;
            if (baseSymbols.Add(baseTypeSymbol)) {
              baseTypes.Add(baseTypeSyntax);
            }
          }
        }
      }

      if (baseTypes.Count > 0) {
        if (baseTypes.Count > 1) {
          var baseTypeIndex = baseTypes.FindIndex(generator_.IsBaseType);
          if (baseTypeIndex > 0) {
            var baseType = baseTypes[baseTypeIndex];
            baseTypes.RemoveAt(baseTypeIndex);
            baseTypes.Insert(0, baseType);
          }
        }

        bool hasExtendSelf = false;
        List<LuaExpressionSyntax> baseTypeExpressions = new List<LuaExpressionSyntax>();
        foreach (var baseType in baseTypes) {
          semanticModel_ = generator_.GetSemanticModel(baseType.SyntaxTree);
          var baseTypeName = BuildInheritTypeName(baseType);
          baseTypeExpressions.Add(baseTypeName);
          CheckBaseTypeGenericKind(ref hasExtendSelf, major.Symbol, baseType);
        }
        var genericArgument = CheckSpeaicalGenericArgument(major.Symbol);
        major.TypeDeclaration.AddBaseTypes(baseTypeExpressions, genericArgument);
        if (hasExtendSelf && !major.Symbol.HasStaticCtor()) {
          major.TypeDeclaration.SetStaticCtorEmpty();
        }
      }

      foreach (var typeDeclaration in typeDeclarations) {
        semanticModel_ = generator_.GetSemanticModel(typeDeclaration.Node.SyntaxTree);
        BuildTypeMembers(major.TypeDeclaration, typeDeclaration.Node);
      }

      CheckTypeDeclaration(major.Symbol, major.TypeDeclaration);
      typeDeclarations_.Pop();
      compilationUnits_.Pop();

      major.TypeDeclaration.IsPartialMark = false;
      major.CompilationUnit.AddTypeDeclarationCount();
    }

    private void GetTypeDeclarationName(BaseTypeDeclarationSyntax typeDeclaration, out LuaIdentifierNameSyntax name, out INamedTypeSymbol typeSymbol) {
      typeSymbol = semanticModel_.GetDeclaredSymbol(typeDeclaration);
      name = generator_.GetTypeDeclarationName(typeSymbol);
    }

    public override LuaSyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) {
      GetTypeDeclarationName(node, out var name, out var typeSymbol);
      LuaClassDeclarationSyntax classDeclaration = new LuaClassDeclarationSyntax(name);
      VisitTypeDeclaration(typeSymbol, node, classDeclaration);
      return classDeclaration;
    }

    public override LuaSyntaxNode VisitStructDeclaration(StructDeclarationSyntax node) {
      GetTypeDeclarationName(node, out var name, out var typeSymbol);
      LuaStructDeclarationSyntax structDeclaration = new LuaStructDeclarationSyntax(name);
      VisitTypeDeclaration(typeSymbol, node, structDeclaration);
      return structDeclaration;
    }

    public override LuaSyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) {
      GetTypeDeclarationName(node, out var name, out var typeSymbol);
      LuaInterfaceDeclarationSyntax interfaceDeclaration = new LuaInterfaceDeclarationSyntax(name);
      var symbol = VisitTypeDeclaration(typeSymbol, node, interfaceDeclaration);
      return interfaceDeclaration;
    }

    public override LuaSyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node) {
      GetTypeDeclarationName(node, out var name, out var typeSymbol);
      LuaEnumDeclarationSyntax enumDeclaration = new LuaEnumDeclarationSyntax(typeSymbol.ToString(), name, CurCompilationUnit);
      typeDeclarations_.Push(new TypeDeclarationInfo(typeSymbol, enumDeclaration));
      var document = BuildDocumentationComment(node);
      enumDeclaration.AddDocument(document);
      foreach (var member in node.Members) {
        var statement = (LuaKeyValueTableItemSyntax)member.Accept(this);
        enumDeclaration.Add(statement);
      }
      typeDeclarations_.Pop();
      generator_.AddEnumDeclaration(enumDeclaration);
      return enumDeclaration;
    }

    public override LuaSyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node) {
      return LuaStatementSyntax.Empty;
    }

    private void VisitYield(TypeSyntax returnType, LuaFunctionExpressionSyntax function) {
      Contract.Assert(function.HasYield);

      var retrurnTypeSymbol = semanticModel_.GetTypeInfo(returnType).Type;
      string name = LuaSyntaxNode.Tokens.Yield + retrurnTypeSymbol.Name;
      LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.System, new LuaIdentifierNameSyntax(name));
      LuaInvocationExpressionSyntax invokeExpression = new LuaInvocationExpressionSyntax(memberAccess);
      LuaFunctionExpressionSyntax wrapFunction = new LuaFunctionExpressionSyntax();

      var parameters = function.ParameterList.Parameters;
      wrapFunction.ParameterList.Parameters.AddRange(parameters);
      wrapFunction.AddStatements(function.Body.Statements);
      invokeExpression.AddArgument(wrapFunction);
      if (returnType.IsKind(SyntaxKind.GenericName)) {
        var genericNameSyntax = (GenericNameSyntax)returnType;
        var typeName = genericNameSyntax.TypeArgumentList.Arguments.First();
        var expression = (LuaExpressionSyntax)typeName.Accept(this);
        invokeExpression.AddArgument(expression);
      } else {
        invokeExpression.AddArgument(LuaIdentifierNameSyntax.Object);
      }
      invokeExpression.ArgumentList.Arguments.AddRange(parameters.Select(i => new LuaArgumentSyntax(i.Identifier)));

      LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(invokeExpression);
      function.Body.Statements.Clear();
      function.AddStatement(returnStatement);
    }

    private sealed class MethodDeclarationResult {
      public IMethodSymbol Symbol;
      public LuaFunctionExpressionSyntax Function;
      public bool IsPrivate;
      public LuaIdentifierNameSyntax Name;
      public LuaDocumentStatement Document;
    }

    private MethodDeclarationResult BuildMethodDeclaration(CSharpSyntaxNode node, SyntaxList<AttributeListSyntax> attributeLists, ParameterListSyntax parameterList, TypeParameterListSyntax typeParameterList, BlockSyntax body, ArrowExpressionClauseSyntax expressionBody, TypeSyntax returnType) {
      IMethodSymbol symbol = (IMethodSymbol)semanticModel_.GetDeclaredSymbol(node);
      List<LuaExpressionSyntax> refOrOutParameters = new List<LuaExpressionSyntax>();
      MethodInfo methodInfo = new MethodInfo(symbol, refOrOutParameters);
      methodInfos_.Push(methodInfo);

      LuaIdentifierNameSyntax methodName;
      if (symbol.MethodKind == MethodKind.LocalFunction) {
        methodName = new LuaIdentifierNameSyntax(symbol.Name);
        CheckLocalVariableName(ref methodName, node);
      } else {
        methodName = GetMemberName(symbol);
      }
      LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
      PushFunction(function);

      var document = BuildDocumentationComment(node);
      bool isPrivate = symbol.IsPrivate() && symbol.ExplicitInterfaceImplementations.IsEmpty;
      if (!symbol.IsStatic) {
        function.AddParameter(LuaIdentifierNameSyntax.This);
        if (isPrivate &&  generator_.IsMonoBehaviourSpeicalMethod(symbol)) {
          isPrivate = false;
        }
      } else if (symbol.IsMainEntryPoint()) {
        isPrivate = false;
        bool success = generator_.SetMainEntryPoint(symbol);
        if (!success) {
          throw new CompilationErrorException(node, "has more than one entry point");
        }
      }

      if (!symbol.IsPrivate()) {
        var attributes = BuildAttributes(attributeLists);
        CurType.AddMethodAttributes(methodName, attributes);
      }

      foreach (var parameterNode in parameterList.Parameters) {
        var parameter = (LuaParameterSyntax)parameterNode.Accept(this);
        function.AddParameter(parameter);
        if (parameterNode.Modifiers.IsOutOrRef()) {
          refOrOutParameters.Add(parameter.Identifier);
        }
      }

      if (typeParameterList != null) {
        var typeParameters = (LuaParameterListSyntax)typeParameterList.Accept(this);
        function.AddParameters(typeParameters.Parameters);
      }

      if (body != null) {
        LuaBlockSyntax block = (LuaBlockSyntax)body.Accept(this);
        function.AddStatements(block.Statements);
      } else {
        blocks_.Push(function.Body);
        var expression = (LuaExpressionSyntax)expressionBody.Accept(this);
        blocks_.Pop();
        function.AddStatement(new LuaReturnStatementSyntax(expression));
      }

      if (function.HasYield) {
        VisitYield(returnType, function);
      } else {
        if (symbol.ReturnsVoid && refOrOutParameters.Count > 0) {
          var returnStatement = new LuaMultipleReturnStatementSyntax();
          returnStatement.Expressions.AddRange(refOrOutParameters);
          function.AddStatement(returnStatement);
        }
      }

      PopFunction();
      methodInfos_.Pop();
      return new MethodDeclarationResult {
        Symbol = symbol,
        Name = methodName,
        Function = function,
        IsPrivate = isPrivate,
        Document = document,
      };
    }

    public override LuaSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
      if (!node.Modifiers.IsAbstract()) {
        var result = BuildMethodDeclaration(node, node.AttributeLists, node.ParameterList, node.TypeParameterList, node.Body, node.ExpressionBody, node.ReturnType);
        CurType.AddMethod(result.Name, result.Function, result.IsPrivate, result.Document);
        return result.Function;
      }
      return base.VisitMethodDeclaration(node);
    }

    private static LuaExpressionSyntax GetPredefinedValueTypeDefaultValue(ITypeSymbol typeSymbol) {
      switch (typeSymbol.SpecialType) {
        case SpecialType.None: {
            if (typeSymbol.TypeKind == TypeKind.Enum) {
              return LuaIdentifierLiteralExpressionSyntax.Zero;
            } else if (typeSymbol.IsTimeSpanType()) {
              return BuildDefaultValue(LuaIdentifierNameSyntax.TimeSpan);
            }
            return null;
          }
        case SpecialType.System_Boolean: {
            return new LuaIdentifierLiteralExpressionSyntax(LuaIdentifierNameSyntax.False);
          }
        case SpecialType.System_Char: {
            return new LuaCharacterLiteralExpression(default(char));
          }
        case SpecialType.System_SByte:
        case SpecialType.System_Byte:
        case SpecialType.System_Int16:
        case SpecialType.System_UInt16:
        case SpecialType.System_Int32:
        case SpecialType.System_UInt32:
        case SpecialType.System_Int64:
        case SpecialType.System_UInt64: {
            return LuaIdentifierLiteralExpressionSyntax.Zero;
          }
        case SpecialType.System_Single:
        case SpecialType.System_Double: {
            return LuaIdentifierLiteralExpressionSyntax.ZeroFloat;
          }
        case SpecialType.System_DateTime: {
            return BuildDefaultValue(LuaIdentifierNameSyntax.DateTime);
          }
        default:
          return null;
      }
    }

    private LuaIdentifierNameSyntax GetTempIdentifier(SyntaxNode node) {
      int index = CurFunction.TempIndex++;
      string name = LuaSyntaxNode.TempIdentifiers.GetOrDefault(index);
      if (name == null) {
        throw new CompilationErrorException(node, $"Your code is startling,{LuaSyntaxNode.TempIdentifiers.Length} temporary variables is not enough");
      }
      return new LuaIdentifierNameSyntax(name);
    }

    private static LuaInvocationExpressionSyntax BuildDefaultValue(LuaExpressionSyntax typeExpression) {
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemDefault, typeExpression);
    }

    private void VisitBaseFieldDeclarationSyntax(BaseFieldDeclarationSyntax node) {
      if (!node.Modifiers.IsConst()) {
        bool isStatic = node.Modifiers.IsStatic();
        bool isPrivate = node.Modifiers.IsPrivate();
        bool isReadOnly = node.Modifiers.IsReadOnly();

        var type = node.Declaration.Type;
        ITypeSymbol typeSymbol = (ITypeSymbol)semanticModel_.GetSymbolInfo(type).Symbol;
        bool isImmutable = typeSymbol.IsImmutable();
        foreach (var variable in node.Declaration.Variables) {
          var variableSymbol = semanticModel_.GetDeclaredSymbol(variable);
          if (node.IsKind(SyntaxKind.EventFieldDeclaration)) {
            var eventSymbol = (IEventSymbol)variableSymbol;
            if (!IsEventFiled(eventSymbol)) {
              var eventName = GetMemberName(eventSymbol);
              var innerName = AddInnerName(eventSymbol);
              LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, variable.Initializer?.Value, out bool valueIsLiteral);
              LuaExpressionSyntax typeExpression = null;
              if (isStatic) {
                typeExpression = GetTypeName(eventSymbol.ContainingType);
              }
              CurType.AddEvent(eventName, innerName, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, typeExpression);
              continue;
            }
          } else {
            if (!isStatic && isPrivate) {
              string name = variable.Identifier.ValueText;
              bool success = CheckFieldNameOfProtobufnet(ref name, variableSymbol.ContainingType);
              if (success) {
                AddField(new LuaIdentifierNameSyntax(name), typeSymbol, type, variable.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, node.AttributeLists);
                continue;
              }
            }
          }
          LuaIdentifierNameSyntax fieldName = GetMemberName(variableSymbol);
          AddField(fieldName, typeSymbol, type, variable.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, node.AttributeLists);
        }
      } else {
        bool isPrivate = node.Modifiers.IsPrivate();
        var type = node.Declaration.Type;
        ITypeSymbol typeSymbol = (ITypeSymbol)semanticModel_.GetSymbolInfo(type).Symbol;
        if (typeSymbol.SpecialType == SpecialType.System_String) {
          foreach (var variable in node.Declaration.Variables) {
            var constValue = semanticModel_.GetConstantValue(variable.Initializer.Value);
            Contract.Assert(constValue.HasValue);
            string v = (string)constValue.Value;
            if (v.Length > kStringConstInlineCount) {
              var variableSymbol = semanticModel_.GetDeclaredSymbol(variable);
              LuaIdentifierNameSyntax fieldName = GetMemberName(variableSymbol);
              AddField(fieldName, typeSymbol, type, variable.Initializer.Value, true, true, isPrivate, true, node.AttributeLists);
            }
          }
        }
      }
    }

    public override LuaSyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) {
      VisitBaseFieldDeclarationSyntax(node);
      return base.VisitFieldDeclaration(node);
    }

    private LuaExpressionSyntax GetFieldValueExpression(TypeSyntax type, ITypeSymbol typeSymbol, ExpressionSyntax expression, out bool valueIsLiteral) {
      LuaExpressionSyntax valueExpression = null;
      valueIsLiteral = false;
      if (expression != null && !expression.IsKind(SyntaxKind.NullLiteralExpression)) {
        valueExpression = (LuaExpressionSyntax)expression.Accept(this);
        valueIsLiteral = expression is LiteralExpressionSyntax;
      }

      if (valueExpression == null) {
        if (typeSymbol.IsValueType && !typeSymbol.IsNullableType()) {
          LuaExpressionSyntax defalutValue = GetPredefinedValueTypeDefaultValue(typeSymbol);
          if (defalutValue != null) {
            valueExpression = defalutValue;
            valueIsLiteral = true;
          } else {
            valueExpression = GetDefaultValueExpression(typeSymbol);
          }
        }
      }
      return valueExpression;
    }

    private void AddField(LuaIdentifierNameSyntax name, ITypeSymbol typeSymbol, TypeSyntax type, ExpressionSyntax expression, bool isImmutable, bool isStatic, bool isPrivate, bool isReadOnly, SyntaxList<AttributeListSyntax> attributeLists) {
      if (!(isStatic && isPrivate)) {
        var attributes = BuildAttributes(attributeLists);
        CurType.AddFieldAttributes(name, attributes);
      }
      LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, expression, out bool valueIsLiteral);
      CurType.AddField(name, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, isReadOnly);
    }

    public override LuaSyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
      if (!node.Modifiers.IsAbstract()) {
        var symbol = semanticModel_.GetDeclaredSymbol(node);
        bool isStatic = symbol.IsStatic;
        bool isPrivate = symbol.IsPrivate();
        bool hasGet = false;
        bool hasSet = false;
        LuaIdentifierNameSyntax propertyName = GetMemberName(symbol);
        if (node.AccessorList != null) {
          foreach (var accessor in node.AccessorList.Accessors) {
            if (accessor.Body != null || accessor.ExpressionBody != null) {
              bool isGet = accessor.IsKind(SyntaxKind.GetAccessorDeclaration);
              LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
              if (!isStatic) {
                functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
              }
              PushFunction(functionExpression);
              if (accessor.Body != null) {
                var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                functionExpression.AddStatements(block.Statements);
              } else {
                blocks_.Push(functionExpression.Body);
                var bodyExpression = (LuaExpressionSyntax)accessor.ExpressionBody.Accept(this);
                blocks_.Pop();
                if (isGet) {
                  functionExpression.AddStatement(new LuaReturnStatementSyntax(bodyExpression));
                } else {
                  functionExpression.AddStatement(bodyExpression);
                }
              }
              PopFunction();
              LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, propertyName);
              CurType.AddMethod(name, functionExpression, isPrivate);
              if (isGet) {
                Contract.Assert(!hasGet);
                hasGet = true;
              } else {
                Contract.Assert(!hasSet);
                functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
                name.IsGetOrAdd = false;
                hasSet = true;
              }

              if (!isPrivate) {
                var attributes = BuildAttributes(accessor.AttributeLists);
                CurType.AddMethodAttributes(name, attributes);
              }
            }
          }
        } else {
          Contract.Assert(!hasGet);
          LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, propertyName);

          LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
          PushFunction(functionExpression);
          blocks_.Push(functionExpression.Body);
          var expression = (LuaExpressionSyntax)node.ExpressionBody.Accept(this);
          blocks_.Pop();
          PopFunction();

          if (!isStatic) {
            functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
          }
          LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(expression);
          functionExpression.AddStatement(returnStatement);
          CurType.AddMethod(name, functionExpression, isPrivate);
          hasGet = true;
        }

        if (!hasGet && !hasSet) {
          ITypeSymbol typeSymbol = symbol.Type;
          bool isImmutable = typeSymbol.IsImmutable();
          bool isField = IsPropertyField(semanticModel_.GetDeclaredSymbol(node));
          if (isField) {
            bool isReadOnly = IsReadOnlyProperty(node);
            AddField(propertyName, typeSymbol, node.Type, node.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, node.AttributeLists);
          } else {
            if (!isPrivate) {
              var attributes = BuildAttributes(node.AttributeLists);
              CurType.AddFieldAttributes(propertyName, attributes);
            }
            LuaIdentifierNameSyntax innerName = AddInnerName(symbol);
            LuaExpressionSyntax valueExpression = GetFieldValueExpression(node.Type, typeSymbol, node.Initializer?.Value, out bool valueIsLiteral);
            LuaExpressionSyntax typeExpression = null;
            if (isStatic) {
              typeExpression = GetTypeName(symbol.ContainingType);
            }
            CurType.AddProperty(propertyName, innerName, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, typeExpression);
          }
        } else {
          if (!isPrivate) {
            var attributes = BuildAttributes(node.AttributeLists);
            CurType.AddFieldAttributes(propertyName, attributes);
          }
        }
      }
      return base.VisitPropertyDeclaration(node);
    }

    private bool IsReadOnlyProperty(PropertyDeclarationSyntax node) {
      return node.AccessorList.Accessors.Count == 1 && node.AccessorList.Accessors[0].Body == null;
    }

    public override LuaSyntaxNode VisitEventDeclaration(EventDeclarationSyntax node) {
      if (!node.Modifiers.IsAbstract()) {
        var symbol = semanticModel_.GetDeclaredSymbol(node);
        bool isStatic = symbol.IsStatic;
        bool isPrivate = symbol.IsPrivate();
        LuaIdentifierNameSyntax eventName = GetMemberName(symbol);
        foreach (var accessor in node.AccessorList.Accessors) {
          LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
          if (!isStatic) {
            functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
          }
          functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
          PushFunction(functionExpression);
          var block = (LuaBlockSyntax)accessor.Body.Accept(this);
          PopFunction();
          functionExpression.AddStatements(block.Statements);
          LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(false, eventName);
          CurType.AddMethod(name, functionExpression, isPrivate);
          if (accessor.IsKind(SyntaxKind.RemoveAccessorDeclaration)) {
            name.IsGetOrAdd = false;
          }

          if (!isPrivate) {
            var attributes = BuildAttributes(accessor.AttributeLists);
            CurType.AddMethodAttributes(name, attributes);
          }
        }
      }
      return base.VisitEventDeclaration(node);
    }

    public override LuaSyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) {
      VisitBaseFieldDeclarationSyntax(node);
      return base.VisitEventFieldDeclaration(node);
    }

    public override LuaSyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node) {
      IFieldSymbol symbol = semanticModel_.GetDeclaredSymbol(node);
      Contract.Assert(symbol.HasConstantValue);
      LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
      var value = new LuaIdentifierLiteralExpressionSyntax(symbol.ConstantValue.ToString());
      return new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(identifier), value);
    }

    public override LuaSyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node) {
      if (!node.Modifiers.IsAbstract()) {
        var symbol = semanticModel_.GetDeclaredSymbol(node);
        bool isPrivate = symbol.IsPrivate();
        LuaIdentifierNameSyntax indexName = GetMemberName(symbol);
        var parameterList = (LuaParameterListSyntax)node.ParameterList.Accept(this);

        void Fill(Action<LuaFunctionExpressionSyntax, LuaPropertyOrEventIdentifierNameSyntax> action) {
          LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
          function.AddParameter(LuaIdentifierNameSyntax.This);
          function.ParameterList.Parameters.AddRange(parameterList.Parameters);
          LuaPropertyOrEventIdentifierNameSyntax name = new LuaPropertyOrEventIdentifierNameSyntax(true, indexName);
          PushFunction(function);
          action(function, name);
          PopFunction();
          CurType.AddMethod(name, function, isPrivate);
        }

        if (node.AccessorList != null) {
          foreach (var accessor in node.AccessorList.Accessors) {
            Fill((function, name) => {
              bool isGet = accessor.IsKind(SyntaxKind.GetAccessorDeclaration);
              if (accessor.Body != null) {
                var block = (LuaBlockSyntax)accessor.Body.Accept(this);
                function.AddStatements(block.Statements);
              } else {
                var bodyExpression = (LuaExpressionSyntax)accessor.ExpressionBody.Accept(this);
                if (isGet) {
                  function.AddStatement(new LuaReturnStatementSyntax(bodyExpression));
                } else {
                  function.AddStatement(bodyExpression);
                }
              }
              if (!isGet) {
                function.AddParameter(LuaIdentifierNameSyntax.Value);
                name.IsGetOrAdd = false;
              }
            });
          }
        } else {
          Fill((function, name) => {
            var bodyExpression = (LuaExpressionSyntax)node.ExpressionBody.Accept(this);
            function.AddStatement(bodyExpression);
          });
        }
      }
      return base.VisitIndexerDeclaration(node);
    }

    public override LuaSyntaxNode VisitBracketedParameterList(BracketedParameterListSyntax node) {
      return BuildParameterList(node.Parameters);
    }

    public override LuaSyntaxNode VisitParameterList(ParameterListSyntax node) {
      return BuildParameterList(node.Parameters);
    }

    private LuaSyntaxNode BuildParameterList(SeparatedSyntaxList<ParameterSyntax> parameters) {
      LuaParameterListSyntax parameterList = new LuaParameterListSyntax();
      foreach (var parameter in parameters) {
        var newNode = (LuaParameterSyntax)parameter.Accept(this);
        parameterList.Parameters.Add(newNode);
      }
      return parameterList;
    }

    public override LuaSyntaxNode VisitParameter(ParameterSyntax node) {
      LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
      CheckLocalVariableName(ref identifier, node);
      return new LuaParameterSyntax(identifier);
    }

    private sealed class BlockCommonNode : IComparable<BlockCommonNode> {
      public SyntaxTrivia SyntaxTrivia { get; }
      public CSharpSyntaxNode SyntaxNode { get; }
      public FileLinePositionSpan LineSpan { get; }

      public BlockCommonNode(SyntaxTrivia syntaxTrivia) {
        SyntaxTrivia = syntaxTrivia;
        LineSpan = syntaxTrivia.SyntaxTree.GetLineSpan(syntaxTrivia.Span);
      }

      public BlockCommonNode(CSharpSyntaxNode statement) {
        SyntaxNode = statement;
        LineSpan = statement.SyntaxTree.GetLineSpan(statement.Span);
      }

      public int CompareTo(BlockCommonNode other) {
        return LineSpan.StartLinePosition.CompareTo(other.LineSpan.StartLinePosition);
      }

      public bool Contains(BlockCommonNode other) {
        var otherLineSpan = other.LineSpan;
        return otherLineSpan.StartLinePosition > LineSpan.StartLinePosition
            && otherLineSpan.EndLinePosition < LineSpan.EndLinePosition;
      }

      public LuaBlankLinesStatement CheckBlankLine(ref int lastLine) {
        LuaBlankLinesStatement statement = null;
        if (lastLine != -1) {
          if (SyntaxTrivia != null && SyntaxTrivia.Kind() == SyntaxKind.DisabledTextTrivia) {
            ++lastLine;
          }
          int count = LineSpan.StartLinePosition.Line - lastLine - 1;
          if (count > 0) {
            statement = new LuaBlankLinesStatement(count);
          }
        }
        lastLine = LineSpan.EndLinePosition.Line;
        return statement;
      }

      public LuaSyntaxNode Visit(LuaSyntaxNodeTransform transfor) {
        const int kCommentCharCount = 2;
        if (SyntaxNode != null) {
          try {
            var node = SyntaxNode.Accept(transfor);
            if (node == null) {
              throw new InvalidOperationException();
            }
            return node;
          } catch (CompilationErrorException e) {
            if (e.SyntaxNode == null) {
              throw new CompilationErrorException(SyntaxNode, e.Message);
            }
            throw e;
          } catch (Exception e) {
            throw new Exception($"Compiler has a bug, thanks to commit issue at https://github.com/yanghuan/CSharp.lua/issue, {SyntaxNode.GetLocationString()}", e);
          }
        } else {
          string content = SyntaxTrivia.ToString();
          switch (SyntaxTrivia.Kind()) {
            case SyntaxKind.SingleLineCommentTrivia: {
                string commentContent = content.Substring(kCommentCharCount);
                return new LuaShortCommentStatement(commentContent);
              }
            case SyntaxKind.MultiLineCommentTrivia: {
                string commentContent = content.Substring(kCommentCharCount, content.Length - kCommentCharCount - kCommentCharCount);
                if (CheckInsertLuaCodeTemplate(commentContent, out var codeStatement)) {
                  return codeStatement;
                }
                return new LuaLongCommentStatement(commentContent);
              }
            case SyntaxKind.SingleLineDocumentationCommentTrivia:
            case SyntaxKind.DisabledTextTrivia: {
                return LuaStatementSyntax.Empty;
              }
            case SyntaxKind.RegionDirectiveTrivia:
            case SyntaxKind.EndRegionDirectiveTrivia: {
                return new LuaShortCommentStatement(content);
              }
            default:
              throw new InvalidOperationException();
          }
        }
      }

      private bool CheckInsertLuaCodeTemplate(string commentContent, out LuaStatementSyntax statement) {
        statement = null;

        char openBracket = LuaSyntaxNode.Tokens.OpenBracket[0];
        int index = commentContent.IndexOf(openBracket);
        if (index != -1) {
          char equals = LuaSyntaxNode.Tokens.Equals[0];
          int count = 0;
          ++index;
          while (commentContent[index] == equals) {
            ++index;
            ++count;
          }
          if (commentContent[index] == openBracket) {
            string closeToken = LuaSyntaxNode.Tokens.CloseBracket + new string(equals, count) + LuaSyntaxNode.Tokens.CloseBracket;
            int begin = index + 1;
            int end = commentContent.IndexOf(closeToken, begin);
            if (end != -1) {
              int start = begin + closeToken.Length;
              string code = commentContent.Substring(start, end - start);
              statement = new LuaIdentifierNameSyntax(code.Trim()).ToStatement();
              return true;
            }
          }
        }

        return false;
      }
    }

    private IEnumerable<LuaStatementSyntax> VisitTriviaAndNode(SyntaxNode rootNode, IEnumerable<CSharpSyntaxNode> nodes, bool isCheckBlank = true) {
      var syntaxTrivias = rootNode.DescendantTrivia().Where(i => i.IsExportSyntaxTrivia(rootNode));
      var syntaxTriviaNodes = syntaxTrivias.Select(i => new BlockCommonNode(i));

      List<BlockCommonNode> list = nodes.Select(i => new BlockCommonNode(i)).ToList();
      bool hasComments = false;
      foreach (var comment in syntaxTriviaNodes) {
        bool isContains = list.Any(i => i.Contains(comment));
        if (!isContains) {
          list.Add(comment);
          hasComments = true;
        }
      }
      if (hasComments) {
        list.Sort();
      }

      int lastLine = -1;
      foreach (var common in list) {
        if (isCheckBlank) {
          var black = common.CheckBlankLine(ref lastLine);
          if (black != null) {
            yield return black;
          }
        }
        yield return (LuaStatementSyntax)common.Visit(this);
      }
    }

    public override LuaSyntaxNode VisitBlock(BlockSyntax node) {
      LuaBlockStatementSyntax block = new LuaBlockStatementSyntax();
      blocks_.Push(block);

      var statements = VisitTriviaAndNode(node, node.Statements);
      block.Statements.AddRange(statements);

      blocks_.Pop();
      return block;
    }

    public override LuaSyntaxNode VisitReturnStatement(ReturnStatementSyntax node) {
      LuaStatementSyntax result;
      if (CurFunction is LuaCheckReturnFunctionExpressionSyntax) {
        LuaMultipleReturnStatementSyntax returnStatement = new LuaMultipleReturnStatementSyntax();
        returnStatement.Expressions.Add(LuaIdentifierNameSyntax.True);
        if (node.Expression != null) {
          var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
          returnStatement.Expressions.Add(expression);
        }
        result = returnStatement;
      } else {
        var curMethodInfo = CurMethodInfoOrNull;
        if (curMethodInfo != null && curMethodInfo.RefOrOutParameters.Count > 0) {
          LuaMultipleReturnStatementSyntax returnStatement = new LuaMultipleReturnStatementSyntax();
          if (node.Expression != null) {
            var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
            returnStatement.Expressions.Add(expression);
          }
          returnStatement.Expressions.AddRange(curMethodInfo.RefOrOutParameters);
          result = returnStatement;
        } else {
          var expression = (LuaExpressionSyntax)node.Expression?.Accept(this);
          result = new LuaReturnStatementSyntax(expression);
        }
      }

      if (node.Parent.IsKind(SyntaxKind.Block) && node.Parent.Parent is MemberDeclarationSyntax) {
        var block = (BlockSyntax)node.Parent;
        if (block.Statements.Last() != node) {
          LuaBlockStatementSyntax blockStatement = new LuaBlockStatementSyntax();
          blockStatement.Statements.Add(result);
          result = blockStatement;
        }
      }
      return result;
    }

    public override LuaSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
      LuaExpressionSyntax expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      if (expression != LuaExpressionSyntax.EmptyExpression) {
        if (expression is LuaLiteralExpressionSyntax) {
          return new LuaShortCommentExpressionStatement(expression);
        }
        return new LuaExpressionStatementSyntax(expression);
      } else {
        return LuaStatementSyntax.Empty;
      }
    }

    private LuaExpressionSyntax BuildCommonAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, string operatorToken, bool isRightParenthesized) {
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        propertyAdapter.ArgumentList.AddArgument(new LuaBinaryExpressionSyntax(propertyAdapter.GetCloneOfGet(), operatorToken, right));
        return propertyAdapter;
      } else {
        if (isRightParenthesized) {
          right = new LuaParenthesizedExpressionSyntax(right);
        }
        return new LuaAssignmentExpressionSyntax(left, new LuaBinaryExpressionSyntax(left, operatorToken, right));
      }
    }

    private LuaExpressionSyntax BuildCommonAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, string operatorToken) {
      var left = (LuaExpressionSyntax)leftNode.Accept(this);
      var right = (LuaExpressionSyntax)rightNode.Accept(this);
      return BuildCommonAssignmentExpression(left, right, operatorToken, rightNode is BinaryExpressionSyntax);
    }

    private LuaExpressionSyntax BuildDelegateAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, bool isPlus) {
      var operatorToken = isPlus ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub;
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        if (propertyAdapter.IsProperty) {
          propertyAdapter.ArgumentList.AddArgument(new LuaBinaryExpressionSyntax(propertyAdapter.GetCloneOfGet(), operatorToken, right));
          return propertyAdapter;
        } else {
          propertyAdapter.IsGetOrAdd = isPlus;
          propertyAdapter.ArgumentList.AddArgument(right);
          return propertyAdapter;
        }
      } else {
        return new LuaAssignmentExpressionSyntax(left, new LuaBinaryExpressionSyntax(left, operatorToken, right));
      }
    }

    private LuaExpressionSyntax BuildBinaryInvokeAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, LuaIdentifierNameSyntax methodName) {
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(methodName, propertyAdapter.GetCloneOfGet(), right);
        propertyAdapter.ArgumentList.AddArgument(invocation);
        return propertyAdapter;
      } else {
        LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(methodName, left, right);
        return new LuaAssignmentExpressionSyntax(left, invocation);
      }
    }

    private LuaExpressionSyntax BuildBinaryInvokeAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, LuaIdentifierNameSyntax methodName) {
      var left = (LuaExpressionSyntax)leftNode.Accept(this);
      var right = (LuaExpressionSyntax)rightNode.Accept(this);
      return BuildBinaryInvokeAssignmentExpression(left, right, methodName);
    }

    private LuaExpressionSyntax BuildIntegerDivAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, LuaIdentifierNameSyntax methodName) {
      if (IsLuaNewest) {
        return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.IntegerDiv);
      } else {
        return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, methodName);
      }
    }

    private LuaExpressionSyntax BuildLuaSimpleAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right) {
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        propertyAdapter.IsGetOrAdd = false;
        propertyAdapter.ArgumentList.AddArgument(right);
        return propertyAdapter;
      } else {
        return new LuaAssignmentExpressionSyntax(left, right);
      }
    }

    private LuaExpressionSyntax BuildLuaAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, SyntaxKind kind) {
      switch (kind) {
        case SyntaxKind.SimpleAssignmentExpression: {
            var left = (LuaExpressionSyntax)leftNode.Accept(this);
            var right = (LuaExpressionSyntax)rightNode.Accept(this);
            if (leftNode.IsKind(SyntaxKind.DeclarationExpression) || leftNode.IsKind(SyntaxKind.TupleExpression)) {
              if (!rightNode.IsKind(SyntaxKind.TupleExpression)) {
                right = BuildDeconstructExpression(rightNode, right);
              }
            }
            return BuildLuaSimpleAssignmentExpression(left, right);
          }
        case SyntaxKind.AddAssignmentExpression: {
            var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
            if (leftType.IsStringType()) {
              var left = (LuaExpressionSyntax)leftNode.Accept(this);
              var right = WrapStringConcatExpression(rightNode);
              return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Concatenation, rightNode is BinaryExpressionSyntax);
            } else {
              var left = (LuaExpressionSyntax)leftNode.Accept(this);
              var right = (LuaExpressionSyntax)rightNode.Accept(this);

              if (leftType.IsDelegateType()) {
                return BuildDelegateAssignmentExpression(left, right, true);
              } else {
                return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Plus, rightNode is BinaryExpressionSyntax);
              }
            }
          }
        case SyntaxKind.SubtractAssignmentExpression: {
            var left = (LuaExpressionSyntax)leftNode.Accept(this);
            var right = (LuaExpressionSyntax)rightNode.Accept(this);

            var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
            if (leftType.IsDelegateType()) {
              return BuildDelegateAssignmentExpression(left, right, false);
            } else {
              return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Sub, rightNode is BinaryExpressionSyntax);
            }
          }
        case SyntaxKind.MultiplyAssignmentExpression: {
            return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Multiply);
          }
        case SyntaxKind.DivideAssignmentExpression: {
            var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
            var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
            if (leftType.IsIntegerType() && rightType.IsIntegerType()) {
              var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.IntegerDivOfNull : LuaIdentifierNameSyntax.IntegerDiv;
              return BuildIntegerDivAssignmentExpression(leftNode, rightNode, methodName);
            } else {
              return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Div);
            }
          }
        case SyntaxKind.ModuloAssignmentExpression: {
            if (!IsLuaNewest) {
              var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
              var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
              if (leftType.IsIntegerType() && rightType.IsIntegerType()) {
                var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.IntegerModOfNull : LuaIdentifierNameSyntax.IntegerMod;
                return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, methodName);
              }
            }
            return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Mod);
          }
        case SyntaxKind.AndAssignmentExpression: {
            return BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.And, LuaSyntaxNode.Tokens.BitAnd, LuaIdentifierNameSyntax.BitAnd, LuaIdentifierNameSyntax.BitAndOfNull);
          }
        case SyntaxKind.ExclusiveOrAssignmentExpression: {
            return BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.NotEquals, LuaSyntaxNode.Tokens.BitXor, LuaIdentifierNameSyntax.BitXor, LuaIdentifierNameSyntax.BitXorOfNull);
          }
        case SyntaxKind.OrAssignmentExpression: {
            return BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Or, LuaSyntaxNode.Tokens.BitOr, LuaIdentifierNameSyntax.BitOr, LuaIdentifierNameSyntax.BitOrOfNull);
          }
        case SyntaxKind.LeftShiftAssignmentExpression: {
            if (IsLuaNewest) {
              return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.LeftShift);
            } else {
              var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
              var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
              var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.ShiftLeftOfNull : LuaIdentifierNameSyntax.ShiftLeft;
              return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, methodName);
            }
          }
        case SyntaxKind.RightShiftAssignmentExpression: {
            if (IsLuaNewest) {
              return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.RightShift);
            } else {
              var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
              var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
              var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.ShiftRightOfNull : LuaIdentifierNameSyntax.ShiftRight;
              return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, methodName);
            }
          }
        default:
          throw new NotImplementedException();
      }
    }

    private LuaExpressionSyntax BuildBitAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, string boolOperatorToken, string otherOperatorToken, LuaIdentifierNameSyntax methodName, LuaIdentifierNameSyntax methodNameOfNull) {
      var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
      if (leftType.SpecialType == SpecialType.System_Boolean) {
        return BuildCommonAssignmentExpression(leftNode, rightNode, boolOperatorToken);
      } else if (!IsLuaNewest) {
        var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
        if (leftType.IsNullableType() || rightType.IsNullableType()) {
          methodName = methodNameOfNull;
        }
        return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, methodName);
      } else {
        string operatorToken = GetOperatorToken(otherOperatorToken);
        return BuildCommonAssignmentExpression(leftNode, rightNode, operatorToken);
      }
    }

    private LuaExpressionSyntax InternalVisitAssignmentExpression(AssignmentExpressionSyntax node) {
      List<LuaExpressionSyntax> assignments = new List<LuaExpressionSyntax>();

      while (true) {
        var leftExpression = node.Left;
        var rightExpression = node.Right;
        var kind = node.Kind();

        if (rightExpression is AssignmentExpressionSyntax assignmentRight) {
          assignments.Add(BuildLuaAssignmentExpression(leftExpression, assignmentRight.Left, kind));
          node = assignmentRight;
        } else {
          assignments.Add(BuildLuaAssignmentExpression(leftExpression, rightExpression, kind));
          break;
        }
      }

      if (assignments.Count == 1) {
        return assignments.First();
      } else {
        assignments.Reverse();
        LuaLineMultipleExpressionSyntax multipleAssignment = new LuaLineMultipleExpressionSyntax();
        multipleAssignment.Assignments.AddRange(assignments);
        return multipleAssignment;
      }
    }

    private static bool IsInlineAssignment(AssignmentExpressionSyntax node) {
      bool isInlineAssignment = false;
      SyntaxKind kind = node.Parent.Kind();
      if (kind == SyntaxKind.ParenthesizedExpression) {
        isInlineAssignment = true;
      } else {
        switch (kind) {
          case SyntaxKind.ExpressionStatement:
          case SyntaxKind.ArrowExpressionClause:
            break;
          default:
            isInlineAssignment = true;
            break;
        }
      }
      return isInlineAssignment;
    }

    public override LuaSyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) {
      var assignment = InternalVisitAssignmentExpression(node);
      if (IsInlineAssignment(node)) {
        CurBlock.Statements.Add(assignment.ToStatement());
        if (assignment is LuaLineMultipleExpressionSyntax lineMultipleExpression) {
          assignment = lineMultipleExpression.Assignments.Last();
        }
        if (assignment is LuaAssignmentExpressionSyntax assignmentExpression) {
          assignment = assignmentExpression.Left;
        } else {
          assignment = (LuaExpressionSyntax)node.Left.Accept(this);
        }
      }
      return assignment;
    }

    private LuaExpressionSyntax BuildInvokeRefOrOut(InvocationExpressionSyntax node, LuaExpressionSyntax invocation, IEnumerable<LuaExpressionSyntax> refOrOutArguments) {
      if (node.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
        LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
        SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
        IMethodSymbol symbol = (IMethodSymbol)symbolInfo.Symbol;
        if (!symbol.ReturnsVoid) {
          var temp = GetTempIdentifier(node);
          CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp));
          multipleAssignment.Lefts.Add(temp);
        }
        multipleAssignment.Lefts.AddRange(refOrOutArguments);
        multipleAssignment.Rights.Add(invocation);
        return multipleAssignment;
      } else {
        var temp = GetTempIdentifier(node);
        LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
        multipleAssignment.Lefts.Add(temp);
        multipleAssignment.Lefts.AddRange(refOrOutArguments);
        multipleAssignment.Rights.Add(invocation);

        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp));
        CurBlock.Statements.Add(new LuaExpressionStatementSyntax(multipleAssignment));
        return temp;
      }
    }

    private LuaExpressionSyntax CheckCodeTemplateInvocationExpression(IMethodSymbol symbol, InvocationExpressionSyntax node) {
      if (node.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
        string codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(symbol);
        if (codeTemplate != null) {
          List<ExpressionSyntax> argumentExpressions = new List<ExpressionSyntax>();
          var memberAccessExpression = (MemberAccessExpressionSyntax)node.Expression;
          if (symbol.IsExtensionMethod) {
            argumentExpressions.Add(memberAccessExpression.Expression);
            if (symbol.ContainingType.IsSystemLinqEnumerable()) {
              CurCompilationUnit.ImportLinq();
            }
          }
          argumentExpressions.AddRange(node.ArgumentList.Arguments.Select(i => i.Expression));
          var invocationExpression = BuildCodeTemplateExpression(codeTemplate, memberAccessExpression.Expression, argumentExpressions, symbol.TypeArguments);
          var refOrOuts = node.ArgumentList.Arguments.Where(i => i.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) || i.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword));
          if (refOrOuts.Any()) {
            return BuildInvokeRefOrOut(node, invocationExpression, refOrOuts.Select(i => ((LuaArgumentSyntax)i.Accept(this)).Expression));
          } else {
            return invocationExpression;
          }
        }
      }
      return null;
    }

    private List<LuaExpressionSyntax> BuildInvocationArguments(IMethodSymbol symbol, InvocationExpressionSyntax node, out List<LuaExpressionSyntax> refOrOutArguments) {
      refOrOutArguments = new List<LuaExpressionSyntax>();
      List<LuaExpressionSyntax> arguments;
      if (symbol != null) {
        arguments = BuildArgumentList(symbol, symbol.Parameters, node.ArgumentList, refOrOutArguments);
        bool ignoreGeneric = generator_.XmlMetaProvider.IsMethodIgnoreGeneric(symbol);
        if (!ignoreGeneric) {
          foreach (var typeArgument in symbol.TypeArguments) {
            LuaExpressionSyntax typeName = GetTypeName(typeArgument);
            arguments.Add(typeName);
          }
        }
        TryRemoveNilArgumentsAtTail(symbol, arguments);
      } else {
        arguments = new List<LuaExpressionSyntax>();
        foreach (var argument in node.ArgumentList.Arguments) {
          if (argument.NameColon != null) {
            throw new CompilationErrorException(argument, "named argument is not support at dynamic");
          }
          FillInvocationArgument(arguments, argument, ImmutableArray<IParameterSymbol>.Empty, refOrOutArguments);
        }
      }
      return arguments;
    }

    private LuaInvocationExpressionSyntax CheckInvocationExpression(IMethodSymbol symbol, InvocationExpressionSyntax node,  LuaExpressionSyntax expression) {
      LuaInvocationExpressionSyntax invocation;
      if (symbol != null && symbol.IsExtensionMethod) {
        if (expression is LuaMemberAccessExpressionSyntax memberAccess) {
          if (memberAccess.Name is LuaInternalMethodExpressionSyntax) {
            invocation = new LuaInvocationExpressionSyntax(memberAccess.Name);
            invocation.AddArgument(memberAccess.Expression);
          } else {
            invocation = BuildExtensionMethodInvocation(symbol.ReducedFrom, memberAccess.Expression, node);
          }
        } else {
          invocation = new LuaInvocationExpressionSyntax(expression);
        }
      } else {
        if (expression is LuaMemberAccessExpressionSyntax memberAccess) {
          if (memberAccess.Name is LuaInternalMethodExpressionSyntax) {
            invocation = new LuaInvocationExpressionSyntax(memberAccess.Name);
            invocation.AddArgument(memberAccess.Expression);
          } else {
            invocation = new LuaInvocationExpressionSyntax(memberAccess);
          }
        } else {
          invocation = new LuaInvocationExpressionSyntax(expression);
          if (expression is LuaInternalMethodExpressionSyntax) {
            if (!symbol.IsStatic) {
              invocation.AddArgument(LuaIdentifierNameSyntax.This);
            }
          }
        }
      }
      return invocation;
    }

    public override LuaSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node) {
      var constExpression = GetConstExpression(node);
      if (constExpression != null) {
        return constExpression;
      }

      var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
      if (symbol != null) {
        var codeTemplateExpression = CheckCodeTemplateInvocationExpression(symbol, node);
        if (codeTemplateExpression != null) {
          return codeTemplateExpression;
        }
      }

      var arguments = BuildInvocationArguments(symbol, node, out var refOrOutArguments);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      var invocation = CheckInvocationExpression(symbol, node, expression);
      invocation.AddArguments(arguments);
      if (refOrOutArguments.Count > 0) {
        return BuildInvokeRefOrOut(node, invocation, refOrOutArguments);
      } else {
        return invocation;
      }
    }

    private LuaInvocationExpressionSyntax BuildExtensionMethodInvocation(IMethodSymbol reducedFrom, LuaExpressionSyntax expression, InvocationExpressionSyntax node) {
      LuaExpressionSyntax typeName = GetTypeName(reducedFrom.ContainingType);
      LuaIdentifierNameSyntax methodName = GetMemberName(reducedFrom);
      LuaMemberAccessExpressionSyntax typeMemberAccess = new LuaMemberAccessExpressionSyntax(typeName, methodName);
      LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(typeMemberAccess);
      invocation.AddArgument(expression);
      return invocation;
    }

    private LuaExpressionSyntax GetDeafultParameterValue(IParameterSymbol parameter, SyntaxNode node, bool isCheckCallerAttribute) {
      Contract.Assert(parameter.HasExplicitDefaultValue);
      LuaExpressionSyntax defaultValue = isCheckCallerAttribute ? CheckCallerAttribute(parameter, node) : null;
      if (defaultValue == null) {
        if (parameter.ExplicitDefaultValue == null && parameter.Type.IsValueType) {
          defaultValue = GetDefaultValueExpression(parameter.Type);
        } else {
          defaultValue = GetLiteralExpression(parameter.ExplicitDefaultValue);
        }
      }
      Contract.Assert(defaultValue != null);
      return defaultValue;
    }

    private void CheckInvocationDeafultArguments(ISymbol symbol, ImmutableArray<IParameterSymbol> parameters, List<LuaExpressionSyntax> arguments, List<Tuple<NameColonSyntax, ExpressionSyntax>> argumentNodeInfos, SyntaxNode node, bool isCheckCallerAttribute) {
      if (parameters.Length > arguments.Count) {
        var optionalParameters = parameters.Skip(arguments.Count);
        foreach (IParameterSymbol parameter in optionalParameters) {
          if (parameter.IsParams) {
            var arrayType = (IArrayTypeSymbol)parameter.Type;
            LuaExpressionSyntax baseType = GetTypeName(arrayType.ElementType);
            LuaExpressionSyntax emptyArray = BuildEmptyArray(baseType);
            arguments.Add(emptyArray);
          } else {
            LuaExpressionSyntax defaultValue = GetDeafultParameterValue(parameter, node, isCheckCallerAttribute);
            arguments.Add(defaultValue);
          }
        }
      } else if (!parameters.IsEmpty) {
        IParameterSymbol last = parameters.Last();
        if (last.IsParams && symbol.IsFromCode()) {
          if (parameters.Length == arguments.Count) {
            var paramsArgument = argumentNodeInfos.Last();
            if (paramsArgument.Item1 != null) {
              string name = paramsArgument.Item1.Name.Identifier.ValueText;
              if (name != last.Name) {
                paramsArgument = argumentNodeInfos.First(i => i.Item1 != null && i.Item1.Name.Identifier.ValueText == last.Name);
              }
            }
            var paramsType = semanticModel_.GetTypeInfo(paramsArgument.Item2).Type;
            if (paramsType.TypeKind != TypeKind.Array) {
              var arrayTypeSymbol = (IArrayTypeSymbol)last.Type;
              var array = BuildArray(arrayTypeSymbol.ElementType, arguments.Last());
              arguments[arguments.Count - 1] = array;
            }
          } else {
            int otherParameterCount = parameters.Length - 1;
            var arrayTypeSymbol = (IArrayTypeSymbol)last.Type;
            var paramsArguments = arguments.Skip(otherParameterCount);
            var array = BuildArray(arrayTypeSymbol.ElementType, paramsArguments);
            arguments.RemoveRange(otherParameterCount, arguments.Count - otherParameterCount);
            arguments.Add(array);
          }
        }
      }

      for (int i = 0; i < arguments.Count; ++i) {
        if (arguments[i] == null) {
          LuaExpressionSyntax defaultValue = GetDeafultParameterValue(parameters[i], node, isCheckCallerAttribute);
          arguments[i] = defaultValue;
        }
      }
    }

    private void CheckInvocationDeafultArguments(ISymbol symbol, ImmutableArray<IParameterSymbol> parameters, List<LuaExpressionSyntax> arguments, BaseArgumentListSyntax node) {
      var argumentNodeInfos = node.Arguments.Select(i => Tuple.Create(i.NameColon, i.Expression)).ToList();
      CheckInvocationDeafultArguments(symbol, parameters, arguments, argumentNodeInfos, node.Parent, true);
    }

    private void CheckPrevIsInvokeStatement() {
      var curBlock = CurBlockOrNull;
      if (curBlock != null) {
        for (int i = curBlock.Statements.Count - 1; i >= 0; --i) {
          var statement = curBlock.Statements[i];
          if (!(statement is LuaBlankLinesStatement)) {
            if (statement is LuaExpressionStatementSyntax expressionStatement) {
              if (expressionStatement.Expression is LuaInvocationExpressionSyntax) {
                curBlock.Statements.Add(LuaStatementSyntax.Colon);
              }
            }
            break;
          }
        }
      }
    }

    private LuaExpressionSyntax BuildMemberAccessTargetExpression(ExpressionSyntax targetExpression) {
      var expression = (LuaExpressionSyntax)targetExpression.Accept(this);
      SyntaxKind kind = targetExpression.Kind();
      if ((kind >= SyntaxKind.NumericLiteralExpression && kind <= SyntaxKind.NullLiteralExpression)
          || (expression is LuaLiteralExpressionSyntax)) {
        CheckPrevIsInvokeStatement();
        expression = new LuaParenthesizedExpressionSyntax(expression);
      }
      return expression;
    }

    private LuaExpressionSyntax BuildMemberAccessExpression(ISymbol symbol, ExpressionSyntax node) {
      bool isExtensionMethod = symbol.Kind == SymbolKind.Method && ((IMethodSymbol)symbol).IsExtensionMethod;
      if (isExtensionMethod) {
        return (LuaExpressionSyntax)node.Accept(this);
      } else {
        return BuildMemberAccessTargetExpression(node);
      }
    }

    private LuaExpressionSyntax CheckMemberAccessCodeTemplate(ISymbol symbol, MemberAccessExpressionSyntax node) {
      if (symbol.Kind == SymbolKind.Field) {
        IFieldSymbol fieldSymbol = (IFieldSymbol)symbol;
        if (fieldSymbol.ContainingType.IsTupleType) {
          int elementIndex = fieldSymbol.GetTupleElementIndex();
          var targetExpression = (LuaExpressionSyntax)node.Expression.Accept(this);
          return new LuaTableIndexAccessExpressionSyntax(targetExpression, new LuaIdentifierNameSyntax(elementIndex));
        }

        string codeTemplate = XmlMetaProvider.GetFieldCodeTemplate(fieldSymbol);
        if (codeTemplate != null) {
          return BuildCodeTemplateExpression(codeTemplate, node.Expression);
        }

        if (fieldSymbol.HasConstantValue) {
          return GetConstLiteralExpression(fieldSymbol);
        }
      } else if (symbol.Kind == SymbolKind.Property) {
        IPropertySymbol propertySymbol = (IPropertySymbol)symbol;
        bool isGet = !node.Parent.Kind().IsAssignment();
        string codeTemplate = XmlMetaProvider.GetProertyCodeTemplate(propertySymbol, isGet);
        if (codeTemplate != null) {
          var result = BuildCodeTemplateExpression(codeTemplate, node.Expression);
          if (codeTemplate[0] == '#' && node.Parent.Parent.IsKind(SyntaxKind.InvocationExpression)) {
            result = new LuaParenthesizedExpressionSyntax(result);
          }
          return result;
        }
      }
      return null;
    }

    private LuaExpressionSyntax InternalVisitMemberAccessExpression(ISymbol symbol, MemberAccessExpressionSyntax node) {
      var codeTemplateExpression = CheckMemberAccessCodeTemplate(symbol, node);
      if (codeTemplateExpression != null) {
        return codeTemplateExpression;
      }

      if (node.Expression.IsKind(SyntaxKind.ThisExpression)) {
        return (LuaExpressionSyntax)node.Name.Accept(this);
      }

      if (node.Expression.IsKind(SyntaxKind.BaseExpression)) {
        var baseExpression = (LuaExpressionSyntax)node.Expression.Accept(this);
        var nameExpression = (LuaExpressionSyntax)node.Name.Accept(this);
        if (symbol.Kind == SymbolKind.Property || symbol.Kind == SymbolKind.Event) {
          if (nameExpression is LuaPropertyAdapterExpressionSyntax propertyMethod) {
            if (baseExpression != LuaIdentifierNameSyntax.This) {
              propertyMethod.ArgumentList.AddArgument(LuaIdentifierNameSyntax.This);
            }
            propertyMethod.Update(baseExpression, true);
            return propertyMethod;
          } else {
            return new LuaMemberAccessExpressionSyntax(baseExpression, nameExpression);
          }
        } else {
          if (baseExpression == LuaIdentifierNameSyntax.This) {
            return new LuaMemberAccessExpressionSyntax(baseExpression, nameExpression, symbol.Kind == SymbolKind.Method);
          } else {
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(baseExpression, nameExpression);
            return new LuaInternalMethodExpressionSyntax(memberAccess);
          }
        }
      }

      if (symbol.IsStatic && node.Expression.IsKind(SyntaxKind.IdentifierName) && CurTypeSymbol == symbol.ContainingSymbol) {
        bool isOnlyName = false;
        if (symbol.Kind == SymbolKind.Method) {
          isOnlyName = true;
        } else if (symbol.Kind == SymbolKind.Property || symbol.Kind == SymbolKind.Event) {
          if (!generator_.IsPropertyFieldOrEventFiled(symbol)) {
            isOnlyName = true;
          }
        }
        if (isOnlyName) {
          return (LuaExpressionSyntax)node.Name.Accept(this);
        }
      }

      var expression = BuildMemberAccessExpression(symbol, node.Expression);
      var name = (LuaExpressionSyntax)node.Name.Accept(this);
      if (symbol.Kind == SymbolKind.Property || symbol.Kind == SymbolKind.Event) {
        return BuildFieldOrPropertyMemberAccessExpression(expression, name, symbol.IsStatic);
      }

      if (symbol.Kind == SymbolKind.Method) {
        if (!node.Parent.IsKind(SyntaxKind.InvocationExpression)) {
          if (!IsInternalMember(node.Name, symbol)) {
            name = new LuaMemberAccessExpressionSyntax(expression, name);
          }
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind, expression, name);
        } else if (IsDelegateInvoke(symbol, node.Name)) {
          return expression;
        }
      }

      return new LuaMemberAccessExpressionSyntax(expression, name, !symbol.IsStatic && symbol.Kind == SymbolKind.Method);
    }

    private static bool IsDelegateInvoke(ISymbol symbol, SimpleNameSyntax name) {
      return symbol.ContainingType.IsDelegateType() && name.Identifier.ValueText == "Invoke";
    }

    public override LuaSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node) {
      ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
      if (symbol == null) {  // dynamic
        var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
        var name = new LuaIdentifierNameSyntax(node.Name.Identifier.ValueText);
        return new LuaMemberAccessExpressionSyntax(expression, name, node.Parent.IsKind(SyntaxKind.InvocationExpression));
      }

      if (symbol.Kind == SymbolKind.NamedType) {
        var expressionSymbol = semanticModel_.GetSymbolInfo(node.Expression).Symbol;
        if (expressionSymbol.Kind == SymbolKind.Namespace) {
          return node.Name.Accept(this); 
        }
      }

      var luaExpression = InternalVisitMemberAccessExpression(symbol, node);
      CheckConversion(node, ref luaExpression);
      return luaExpression;
    }

    private LuaExpressionSyntax BuildStaticFieldName(ISymbol symbol, bool isReadOnly, IdentifierNameSyntax node) {
      Contract.Assert(symbol.IsStatic);
      LuaIdentifierNameSyntax name = GetMemberName(symbol);
      if (!symbol.IsPrivate()) {
        if (isReadOnly) {
          if (node.Parent.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
            AssignmentExpressionSyntax assignmentExpression = (AssignmentExpressionSyntax)node.Parent;
            if (assignmentExpression.Left == node) {
              CurType.AddStaticReadOnlyAssignmentName(name);
            }
          }
          if (CheckUsingStaticNameSyntax(symbol, node, name, out var newExpression)) {
            return newExpression;
          }
        } else {
          if (IsInternalNode(node)) {
            if (CurFunctionOrNull is LuaConstructorAdapterExpressionSyntax ctor && ctor.IsStatic) {
              return new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, name);
            } else {
              var typeName = GetTypeName(symbol.ContainingType);
              return new LuaMemberAccessExpressionSyntax(typeName, name);
            }
          } else {
            if (CheckUsingStaticNameSyntax(symbol, node, name, out var newExpression)) {
              return newExpression;
            }
          }
        }
      }
      return name;
    }

    private bool IsInternalNode(NameSyntax node) {
      var parentNode = node.Parent;
      switch (parentNode.Kind()) {
        case SyntaxKind.SimpleMemberAccessExpression: {
            MemberAccessExpressionSyntax parent = (MemberAccessExpressionSyntax)parentNode;
            if (parent.Expression.IsKind(SyntaxKind.ThisExpression)) {
              return true;
            } else if (parent.Expression == node) {
              return true;
            }
            return false;
          }
        case SyntaxKind.MemberBindingExpression:
        case SyntaxKind.NameEquals: {
            return false;
          }
        case SyntaxKind.SimpleAssignmentExpression: {
            AssignmentExpressionSyntax parent = (AssignmentExpressionSyntax)parentNode;
            if (parent.Right != node) {
              if (parent.Parent.IsKind(SyntaxKind.ObjectInitializerExpression)) {
                return false;
              }
            }
            break;
          }
      }
      return true;
    }

    private LuaExpressionSyntax VisitFieldOrEventIdentifierName(IdentifierNameSyntax node, ISymbol symbol, bool isProperty) {
      bool isField, isReadOnly;
      if (isProperty) {
        var propertySymbol = (IPropertySymbol)symbol;
        isField = IsPropertyField(propertySymbol);
        isReadOnly = propertySymbol.IsReadOnly;
      } else {
        var eventSymbol = (IEventSymbol)symbol;
        isField = IsEventFiled(eventSymbol);
        isReadOnly = false;
      }

      if (isField) {
        if (symbol.IsStatic) {
          return BuildStaticFieldName(symbol, isReadOnly, node);
        } else {
          LuaIdentifierNameSyntax fieldName = GetMemberName(symbol);
          if (IsInternalNode(node)) {
            return new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, fieldName);
          } else {
            return fieldName;
          }
        }
      } else {
        var name = GetMemberName(symbol);
        var identifierName = new LuaPropertyOrEventIdentifierNameSyntax(isProperty, name);
        if (symbol.IsStatic) {
          var identifierExpression = new LuaPropertyAdapterExpressionSyntax(identifierName);
          if (CheckUsingStaticNameSyntax(symbol, node, identifierExpression, out var newExpression)) {
            return newExpression;
          }
          return identifierExpression;
        } else {
          if (IsInternalMember(node, symbol)) {
            var propertyAdapter = new LuaPropertyAdapterExpressionSyntax(identifierName);
            propertyAdapter.ArgumentList.AddArgument(LuaIdentifierNameSyntax.This);
            return propertyAdapter;
          } else {
            if (IsInternalNode(node)) {
              return new LuaPropertyAdapterExpressionSyntax(LuaIdentifierNameSyntax.This, identifierName, true);
            } else {
              return new LuaPropertyAdapterExpressionSyntax(identifierName);
            }
          }
        }
      }
    }

    private LuaExpressionSyntax GetMethodNameExpression(IMethodSymbol symbol, NameSyntax node) {
      LuaIdentifierNameSyntax methodName = GetMemberName(symbol);
      if (symbol.IsStatic) {
        if (CheckUsingStaticNameSyntax(symbol, node, methodName, out var outExpression)) {
          return outExpression;
        }
        if (IsInternalMember(node, symbol)) {
          return new LuaInternalMethodExpressionSyntax(methodName);
        }
        return methodName;
      } else {
        if (IsInternalMember(node, symbol)) {
          if (!node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) && !node.Parent.IsKind(SyntaxKind.InvocationExpression)) {
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind, LuaIdentifierNameSyntax.This, methodName);
          }
          return new LuaInternalMethodExpressionSyntax(methodName);
        } else {
          if (IsInternalNode(node)) {
            if (!node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) && !node.Parent.IsKind(SyntaxKind.InvocationExpression)) {
              return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind, LuaIdentifierNameSyntax.This, new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, methodName));
            }

            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, methodName, true);
            return memberAccess;
          }
        }
      }
      return methodName;
    }

    private LuaExpressionSyntax GetFieldNameExpression(IFieldSymbol symbol, IdentifierNameSyntax node) {
      if (symbol.IsStatic) {
        if (symbol.HasConstantValue) {
          if (symbol.Type.SpecialType == SpecialType.System_String) {
            if (((string)symbol.ConstantValue).Length <= kStringConstInlineCount) {
              return GetConstLiteralExpression(symbol);
            }
          } else {
            return GetConstLiteralExpression(symbol);
          }
        }
        return BuildStaticFieldName(symbol, symbol.IsReadOnly, node);
      } else {
        if (IsInternalNode(node)) {
          if (symbol.IsPrivate() && symbol.IsFromCode()) {
            string symbolName = symbol.Name;
            bool success = CheckFieldNameOfProtobufnet(ref symbolName, symbol.ContainingType);
            if (success) {
              return new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, new LuaIdentifierNameSyntax(symbolName));
            }
          }
          return new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, GetMemberName(symbol));
        } else {
          return GetMemberName(symbol);
        }
      }
    }

    public override LuaSyntaxNode VisitIdentifierName(IdentifierNameSyntax node) {
      LuaIdentifierNameSyntax GetSampleName(ISymbol nodeSymbol) {
        var nameIdentifier = new LuaIdentifierNameSyntax(nodeSymbol.Name);
        CheckLocalSymbolName(nodeSymbol, ref nameIdentifier);
        return nameIdentifier;
      }

      SymbolInfo symbolInfo = semanticModel_.GetSymbolInfo(node);
      ISymbol symbol = symbolInfo.Symbol;
      Contract.Assert(symbol != null);
      LuaExpressionSyntax identifier;
      switch (symbol.Kind) {
        case SymbolKind.Local: {
            var localSymbol = (ILocalSymbol)symbol;
            if (localSymbol.IsConst) {
              if (localSymbol.Type.SpecialType == SpecialType.System_String) {
                if (((string)localSymbol.ConstantValue).Length <= kStringConstInlineCount) {
                  return GetConstLiteralExpression(localSymbol);
                }
              } else {
                return GetConstLiteralExpression(localSymbol);
              }
            }

            identifier = GetSampleName(symbol);
            CheckValueTypeClone(localSymbol.Type, node, ref identifier);
            break;
          }
        case SymbolKind.Parameter: {
            var parameterSymbol = (IParameterSymbol)symbol;
            identifier = GetSampleName(symbol);
            CheckValueTypeClone(parameterSymbol.Type, node, ref identifier);
            break;
          }
        case SymbolKind.RangeVariable: {
            identifier = GetRangeIdentifierName(node);
            break;
          }
        case SymbolKind.TypeParameter:
        case SymbolKind.Label: {
            identifier = new LuaIdentifierNameSyntax(symbol.Name);
            break;
          }
        case SymbolKind.NamedType: {
            identifier = GetTypeName(symbol);
            break;
          }
        case SymbolKind.Namespace: {
            identifier = new LuaIdentifierNameSyntax(symbol.ToString());
            break;
          }
        case SymbolKind.Field: {
            var fieldSymbol = (IFieldSymbol)symbol;
            identifier = GetFieldNameExpression(fieldSymbol, node);
            CheckValueTypeClone(fieldSymbol.Type, node, ref identifier);
            break;
          }
        case SymbolKind.Method: {
            var methodSymbol = (IMethodSymbol)symbol;
            if (methodSymbol.MethodKind == MethodKind.LocalFunction) {
              identifier = GetSampleName(symbol);
            } else {
              identifier = GetMethodNameExpression(methodSymbol, node);
            }
            break;
          }
        case SymbolKind.Property: {
            identifier = VisitFieldOrEventIdentifierName(node, symbol, true);
            break;
          }
        case SymbolKind.Event: {
            identifier = VisitFieldOrEventIdentifierName(node, symbol, false);
            break;
          }
        default: {
            throw new NotSupportedException();
          }
      }
      if (!node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
        CheckConversion(node, ref identifier);
      }
      return identifier;
    }

    public override LuaSyntaxNode VisitQualifiedName(QualifiedNameSyntax node) {
      return node.Right.Accept(this);
    }

    private void FillInvocationArgument(List<LuaExpressionSyntax> arguments, ArgumentSyntax node, ImmutableArray<IParameterSymbol> parameters, List<LuaExpressionSyntax> refOrOutArguments) {
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      Contract.Assert(expression != null);
      if (node.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword)) {
        refOrOutArguments.Add(expression);
      } else if (node.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword)) {
        refOrOutArguments.Add(expression);
        expression = LuaIdentifierNameSyntax.Nil;
      }
      if (node.NameColon != null) {
        string name = node.NameColon.Name.Identifier.ValueText;
        int index = parameters.IndexOf(i => i.Name == name);
        if (index == -1) {
          throw new InvalidOperationException();
        }
        arguments.AddAt(index, expression);
      } else {
        arguments.Add(expression);
      }
    }

    private List<LuaExpressionSyntax> BuildArgumentList(ISymbol symbol, ImmutableArray<IParameterSymbol> parameters, BaseArgumentListSyntax node, List<LuaExpressionSyntax> refOrOutArguments = null) {
      Contract.Assert(node != null);
      List<LuaExpressionSyntax> arguments = new List<LuaExpressionSyntax>();
      foreach (var argument in node.Arguments) {
        FillInvocationArgument(arguments, argument, parameters, refOrOutArguments);
      }
      CheckInvocationDeafultArguments(symbol, parameters, arguments, node);
      return arguments;
    }

    private LuaArgumentListSyntax BuildArgumentList(SeparatedSyntaxList<ArgumentSyntax> arguments) {
      LuaArgumentListSyntax argumentList = new LuaArgumentListSyntax();
      foreach (var argument in arguments) {
        var newNode = (LuaArgumentSyntax)argument.Accept(this);
        argumentList.Arguments.Add(newNode);
      }
      return argumentList;
    }

    public override LuaSyntaxNode VisitArgumentList(ArgumentListSyntax node) {
      return BuildArgumentList(node.Arguments);
    }

    public override LuaSyntaxNode VisitArgument(ArgumentSyntax node) {
      Contract.Assert(node.NameColon == null);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      return new LuaArgumentSyntax(expression);
    }

    public override LuaSyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node) {
      switch (node.Kind()) {
        case SyntaxKind.NumericLiteralExpression: {
            string value = node.Token.ValueText;
            if (node.Token.Value is float || node.Token.Value is double) {
              if (!value.Contains('.')) {
                value += ".0";
              }
            }
            return new LuaIdentifierLiteralExpressionSyntax(value);
          }
        case SyntaxKind.StringLiteralExpression: {
            return BuildStringLiteralTokenExpression(node.Token);
          }
        case SyntaxKind.CharacterLiteralExpression: {
            return new LuaCharacterLiteralExpression((char)node.Token.Value);
          }
        case SyntaxKind.NullLiteralExpression: {
            return LuaIdentifierLiteralExpressionSyntax.Nil;
          }
        default: {
            return new LuaIdentifierLiteralExpressionSyntax(node.Token.ValueText);
          }
      }
    }

    public override LuaSyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) {
      var declaration = (LuaVariableDeclarationSyntax)node.Declaration.Accept(this);
      return new LuaLocalDeclarationStatementSyntax(declaration);
    }

    public override LuaSyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node) {
      LuaVariableListDeclarationSyntax variableListDeclaration = new LuaVariableListDeclarationSyntax();
      foreach (VariableDeclaratorSyntax variable in node.Variables) {
        if (variable.Initializer != null && variable.Initializer.Value.IsKind(SyntaxKind.RefExpression)) {
          var refExpression = (LuaExpressionSyntax)variable.Initializer.Value.Accept(this);
          AddLocalVariableMapping(new LuaExpressionNameSyntax(refExpression), variable);
        } else {
          bool isConst = false;
          if (node.Parent is LocalDeclarationStatementSyntax parent && parent.IsConst) {
            isConst = true;
            if (variable.Initializer.Value is LiteralExpressionSyntax value) {
              var token = value.Token;
              if (token.Value is string str) {
                if (str.Length > kStringConstInlineCount) {
                  isConst = false;
                }
              }
            }
          }
          if (!isConst) {
            var variableDeclarator = (LuaVariableDeclaratorSyntax)variable.Accept(this);
            variableListDeclaration.Variables.Add(variableDeclarator);
          }
        }
      }
      bool isMultiNil = variableListDeclaration.Variables.Count > 0 && variableListDeclaration.Variables.All(i => i.Initializer == null);
      if (isMultiNil) {
        LuaLocalVariablesStatementSyntax declarationStatement = new LuaLocalVariablesStatementSyntax();
        foreach (var variable in variableListDeclaration.Variables) {
          declarationStatement.Variables.Add(variable.Identifier);
        }
        return declarationStatement;
      } else {
        return variableListDeclaration;
      }
    }

    public override LuaSyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node) {
      LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
      CheckLocalVariableName(ref identifier, node);
      LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(identifier);
      if (node.Initializer != null) {
        variableDeclarator.Initializer = (LuaEqualsValueClauseSyntax)node.Initializer.Accept(this);
      }
      return variableDeclarator;
    }

    public override LuaSyntaxNode VisitEqualsValueClause(EqualsValueClauseSyntax node) {
      var expression = (LuaExpressionSyntax)node.Value.Accept(this);
      return new LuaEqualsValueClauseSyntax(expression);
    }

    public override LuaSyntaxNode VisitPredefinedType(PredefinedTypeSyntax node) {
      ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
      return GetTypeShortName(symbol);
    }

    private void WriteStatementOrBlock(StatementSyntax statement, LuaBlockSyntax block) {
      if (statement.IsKind(SyntaxKind.Block)) {
        var blockNode = (LuaBlockSyntax)statement.Accept(this);
        block.Statements.AddRange(blockNode.Statements);
      } else {
        blocks_.Push(block);
        var statementNode = (LuaStatementSyntax)statement.Accept(this);
        block.Statements.Add(statementNode);
        blocks_.Pop();
      }
    }

    #region if else switch

    public override LuaSyntaxNode VisitIfStatement(IfStatementSyntax node) {
      var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
      LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
      WriteStatementOrBlock(node.Statement, ifStatement.Body);
      ifStatements_.Push(ifStatement);
      node.Else?.Accept(this);
      ifStatements_.Pop();
      return ifStatement;
    }

    public override LuaSyntaxNode VisitElseClause(ElseClauseSyntax node) {
      if (node.Statement.IsKind(SyntaxKind.IfStatement)) {
        var ifStatement = (IfStatementSyntax)node.Statement;
        var condition = (LuaExpressionSyntax)ifStatement.Condition.Accept(this);
        LuaElseIfStatementSyntax elseIfStatement = new LuaElseIfStatementSyntax(condition);
        WriteStatementOrBlock(ifStatement.Statement, elseIfStatement.Body);
        ifStatements_.Peek().ElseIfStatements.Add(elseIfStatement);
        ifStatement.Else?.Accept(this);
        return elseIfStatement;
      } else {
        LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
        WriteStatementOrBlock(node.Statement, elseClause.Body);
        ifStatements_.Peek().Else = elseClause;
        return elseClause;
      }
    }

    public override LuaSyntaxNode VisitSwitchStatement(SwitchStatementSyntax node) {
      var temp = GetTempIdentifier(node);
      LuaSwitchAdapterStatementSyntax switchStatement = new LuaSwitchAdapterStatementSyntax(temp);
      switchs_.Push(switchStatement);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      switchStatement.Fill(expression, node.Sections.Select(i => (LuaStatementSyntax)i.Accept(this)));
      switchs_.Pop();
      return switchStatement;
    }

    public override LuaSyntaxNode VisitSwitchSection(SwitchSectionSyntax node) {
      bool isDefault = node.Labels.Any(i => i.Kind() == SyntaxKind.DefaultSwitchLabel);
      if (isDefault) {
        LuaBlockSyntax block = new LuaBlockSyntax();
        foreach (var statement in node.Statements) {
          var luaStatement = (LuaStatementSyntax)statement.Accept(this);
          block.Statements.Add(luaStatement);
        }
        return block;
      } else {
        var expressions = node.Labels.Select(i => (LuaExpressionSyntax)i.Accept(this));
        var condition = expressions.Aggregate((x, y) => new LuaBinaryExpressionSyntax(x, LuaSyntaxNode.Tokens.Or, y));
        LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
        foreach (var statement in node.Statements) {
          var luaStatement = (LuaStatementSyntax)statement.Accept(this);
          ifStatement.Body.Statements.Add(luaStatement);
        }
        return ifStatement;
      }
    }

    public override LuaSyntaxNode VisitCaseSwitchLabel(CaseSwitchLabelSyntax node) {
      var left = switchs_.Peek().Temp;
      var right = (LuaExpressionSyntax)node.Value.Accept(this);
      LuaBinaryExpressionSyntax BinaryExpression = new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.EqualsEquals, right);
      return BinaryExpression;
    }

    private LuaExpressionSyntax BuildSwitchLabelWhenClause(LuaExpressionSyntax expression, WhenClauseSyntax whenClause) {
      if (whenClause != null) {
        var whenExpression = (LuaExpressionSyntax)whenClause.Accept(this);
        return new LuaBinaryExpressionSyntax(expression, LuaSyntaxNode.Tokens.And, whenExpression);
      } else {
        return expression;
      }
    }

    public override LuaSyntaxNode VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node) {
      var left = switchs_.Peek().Temp;
      if (node.Pattern is DeclarationPatternSyntax declarationPattern) {
        AddLocalVariableMapping(left, declarationPattern.Designation);
        var switchStatement = FindParent<SwitchStatementSyntax>(node);
        var leftType = semanticModel_.GetTypeInfo(switchStatement.Expression).Type;
        var rightType = semanticModel_.GetTypeInfo(declarationPattern.Type).Type;
        if (leftType.IsSubclassOf(rightType)) {
          return node.WhenClause != null ? node.WhenClause.Accept(this) : LuaIdentifierLiteralExpressionSyntax.True;
        } else {
          var type = (LuaExpressionSyntax)declarationPattern.Type.Accept(this);
          var isInvoke = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Is, left, type);
          return BuildSwitchLabelWhenClause(isInvoke, node.WhenClause);
        }
      } else {
        var patternExpression = (LuaExpressionSyntax)node.Pattern.Accept(this);
        var expression = new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.EqualsEquals, patternExpression);
        return BuildSwitchLabelWhenClause(expression, node.WhenClause);
      }
    }

    public override LuaSyntaxNode VisitWhenClause(WhenClauseSyntax node) {
      return node.Condition.Accept(this);
    }

    public override LuaSyntaxNode VisitConstantPattern(ConstantPatternSyntax node) {
      return node.Expression.Accept(this);
    }

    #endregion

    public override LuaSyntaxNode VisitBreakStatement(BreakStatementSyntax node) {
      return LuaBreakStatementSyntax.Statement;
    }

    private LuaExpressionSyntax WrapStringConcatExpression(ExpressionSyntax expression) {
      ITypeSymbol typeInfo = semanticModel_.GetTypeInfo(expression).Type;
      var original = (LuaExpressionSyntax)expression.Accept(this);
      if (typeInfo.IsStringType()) {
        return original;
      } else if (typeInfo.SpecialType == SpecialType.System_Char) {
        var constValue = semanticModel_.GetConstantValue(expression);
        if (constValue.HasValue) {
          string text = SyntaxFactory.Literal((char)constValue.Value).Text;
          return new LuaIdentifierLiteralExpressionSyntax(text);
        } else {
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.StringChar, original);
        }
      } else if (typeInfo.SpecialType >= SpecialType.System_Boolean && typeInfo.SpecialType <= SpecialType.System_Double) {
        return original;
      } else if (typeInfo.TypeKind == TypeKind.Enum) {
        if (original is LuaLiteralExpressionSyntax) {
          var symbol = semanticModel_.GetSymbolInfo(expression).Symbol;
          return new LuaConstLiteralExpression(symbol.Name, typeInfo.ToString());
        } else {
          AddExportEnum(typeInfo);
          LuaIdentifierNameSyntax typeName = GetTypeShortName(typeInfo);
          LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(original, LuaIdentifierNameSyntax.ToEnumString, true);
          return new LuaInvocationExpressionSyntax(memberAccess, typeName);
        }
      } else if (typeInfo.IsValueType) {
        LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(original, LuaIdentifierNameSyntax.ToStr, true);
        return new LuaInvocationExpressionSyntax(memberAccess);
      } else {
        return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemToString, original);
      }
    }

    private LuaExpressionSyntax BuildStringConcatExpression(BinaryExpressionSyntax node) {
      var expression = BuildStringConcatExpression(node.Left, node.Right);
      if (node.Parent.IsKind(SyntaxKind.AddExpression)) {
        expression = new LuaParenthesizedExpressionSyntax(expression);
      }
      return expression;
    }

    private LuaExpressionSyntax BuildStringConcatExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode) {
      var left = WrapStringConcatExpression(leftNode);
      var right = WrapStringConcatExpression(rightNode);
      return new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.Concatenation, right);
    }

    private LuaExpressionSyntax BuildBinaryInvokeExpression(BinaryExpressionSyntax node, LuaIdentifierNameSyntax name) {
      var left = (LuaExpressionSyntax)node.Left.Accept(this);
      var right = (LuaExpressionSyntax)node.Right.Accept(this);
      return new LuaInvocationExpressionSyntax(name, left, right);
    }

    private LuaExpressionSyntax BuildIntegerDivExpression(ITypeSymbol leftType, ITypeSymbol rightType, BinaryExpressionSyntax node) {
      if (IsLuaNewest) {
        return BuildBinaryExpression(node, LuaSyntaxNode.Tokens.IntegerDiv);
      } else {
        var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.IntegerDivOfNull : LuaIdentifierNameSyntax.IntegerDiv;
        return BuildBinaryInvokeExpression(node, methodName);
      }
    }

    private LuaBinaryExpressionSyntax BuildBinaryExpression(BinaryExpressionSyntax node, string operatorToken) {
      var left = (LuaExpressionSyntax)node.Left.Accept(this);
      var right = (LuaExpressionSyntax)node.Right.Accept(this);
      return new LuaBinaryExpressionSyntax(left, operatorToken, right);
    }

    private LuaExpressionSyntax BuildBitExpression(BinaryExpressionSyntax node, string boolOperatorToken, LuaIdentifierNameSyntax otherName, LuaIdentifierNameSyntax nameOfNull) {
      var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
      if (leftType.SpecialType == SpecialType.System_Boolean) {
        return BuildBinaryExpression(node, boolOperatorToken);
      } else if (!IsLuaNewest) {
        var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
        var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? nameOfNull : otherName;
        return BuildBinaryInvokeExpression(node, methodName);
      } else {
        string operatorToken = GetOperatorToken(node.OperatorToken);
        return BuildBinaryExpression(node, operatorToken);
      }
    }

    public override LuaSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
      var constExpression = GetConstExpression(node);
      if (constExpression != null) {
        return constExpression;
      }

      switch (node.Kind()) {
        case SyntaxKind.AddExpression: {
            if (semanticModel_.GetSymbolInfo(node).Symbol is IMethodSymbol methodSymbol) {
              if (methodSymbol.ContainingType.IsStringType()) {
                return BuildStringConcatExpression(node);
              }
            }
            break;
          }
        case SyntaxKind.DivideExpression: {
            var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
            var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
            if (leftType.IsIntegerType() && rightType.IsIntegerType()) {
              return BuildIntegerDivExpression(leftType, rightType, node);
            }
            break;
          }
        case SyntaxKind.ModuloExpression: {
            if (!IsLuaNewest) {
              var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
              var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
              if (leftType.IsIntegerType() && rightType.IsIntegerType()) {
                var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.IntegerModOfNull : LuaIdentifierNameSyntax.IntegerMod;
                return BuildBinaryInvokeExpression(node, methodName);
              }
            }
            break;
          }
        case SyntaxKind.LeftShiftExpression: {
            if (!IsLuaNewest) {
              var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
              var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
              var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.ShiftLeftOfNull : LuaIdentifierNameSyntax.ShiftLeft;
              return BuildBinaryInvokeExpression(node, methodName);
            }
            break;
          }
        case SyntaxKind.RightShiftExpression: {
            if (!IsLuaNewest) {
              var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
              var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
              var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.ShiftRightOfNull : LuaIdentifierNameSyntax.ShiftRight;
              return BuildBinaryInvokeExpression(node, methodName);
            }
            break;
          }
        case SyntaxKind.BitwiseOrExpression: {
            return BuildBitExpression(node, LuaSyntaxNode.Tokens.Or, LuaIdentifierNameSyntax.BitOr, LuaIdentifierNameSyntax.BitOrOfNull);
          }
        case SyntaxKind.BitwiseAndExpression: {
            return BuildBitExpression(node, LuaSyntaxNode.Tokens.And, LuaIdentifierNameSyntax.BitAnd, LuaIdentifierNameSyntax.BitAndOfNull);
          }
        case SyntaxKind.ExclusiveOrExpression: {
            return BuildBitExpression(node, LuaSyntaxNode.Tokens.NotEquals, LuaIdentifierNameSyntax.BitXor, LuaIdentifierNameSyntax.BitXorOfNull);
          }
        case SyntaxKind.IsExpression: {
            var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
            var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
            if (leftType.IsSubclassOf(rightType)) {
              return LuaIdentifierLiteralExpressionSyntax.True;
            }

            return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.Is);
          }
        case SyntaxKind.AsExpression: {
            var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
            var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
            if (leftType.IsSubclassOf(rightType)) {
              return node.Left.Accept(this);
            }

            return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.As);
          }
      }

      var operatorExpression = GerUserDefinedOperatorExpression(node);
      if (operatorExpression != null) {
        return operatorExpression;
      }

      string operatorToken = GetOperatorToken(node.OperatorToken);
      return BuildBinaryExpression(node, operatorToken);
    }

    private bool IsSingleLineUnary(ExpressionSyntax node) {
      return node.Parent.IsKind(SyntaxKind.ExpressionStatement) || node.Parent.IsKind(SyntaxKind.ForStatement);
    }

    private LuaSyntaxNode BuildPrefixUnaryExpression(bool isSingleLine, string operatorToken, LuaExpressionSyntax operand, SyntaxNode node, bool isLocalVar = false) {
      LuaBinaryExpressionSyntax binary = new LuaBinaryExpressionSyntax(operand, operatorToken, LuaIdentifierNameSyntax.One);
      if (isSingleLine) {
        return new LuaAssignmentExpressionSyntax(operand, binary);
      } else {
        if (isLocalVar) {
          CurBlock.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(operand, binary)));
          return operand;
        } else {
          var temp = GetTempIdentifier(node);
          CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, binary));
          CurBlock.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(operand, temp)));
          return temp;
        }
      }
    }

    private LuaSyntaxNode BuildPropertyPrefixUnaryExpression(bool isSingleLine, string operatorToken, LuaPropertyAdapterExpressionSyntax get, LuaPropertyAdapterExpressionSyntax set, SyntaxNode node) {
      set.IsGetOrAdd = false;
      var binary = new LuaBinaryExpressionSyntax(get, operatorToken, LuaIdentifierNameSyntax.One);
      if (isSingleLine) {
        set.ArgumentList.AddArgument(binary);
        return set;
      } else {
        var temp = GetTempIdentifier(node);
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, binary));
        set.ArgumentList.AddArgument(temp);
        CurBlock.Statements.Add(new LuaExpressionStatementSyntax(set));
        return temp;
      }
    }

    private LuaMemberAccessExpressionSyntax GetTempUnaryExpression(LuaMemberAccessExpressionSyntax memberAccess, out LuaLocalVariableDeclaratorSyntax localTemp, SyntaxNode node) {
      var temp = GetTempIdentifier(node);
      localTemp = new LuaLocalVariableDeclaratorSyntax(temp, memberAccess.Expression);
      return new LuaMemberAccessExpressionSyntax(temp, memberAccess.Name, memberAccess.IsObjectColon);
    }

    private LuaPropertyAdapterExpressionSyntax GetTempPropertyUnaryExpression(LuaPropertyAdapterExpressionSyntax propertyAdapter, out LuaLocalVariableDeclaratorSyntax localTemp, SyntaxNode node) {
      var temp = GetTempIdentifier(node);
      localTemp = new LuaLocalVariableDeclaratorSyntax(temp, propertyAdapter.Expression);
      return new LuaPropertyAdapterExpressionSyntax(temp, propertyAdapter.Name, propertyAdapter.IsObjectColon);
    }

    public override LuaSyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node) {
      SyntaxKind kind = node.Kind();
      switch (kind) {
        case SyntaxKind.PreIncrementExpression:
        case SyntaxKind.PreDecrementExpression: {
            bool isSingleLine = IsSingleLineUnary(node);
            string operatorToken = kind == SyntaxKind.PreIncrementExpression ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub;
            var operand = (LuaExpressionSyntax)node.Operand.Accept(this);

            if (operand is LuaMemberAccessExpressionSyntax memberAccess) {
              if (memberAccess.Expression != LuaIdentifierNameSyntax.This) {
                memberAccess = GetTempUnaryExpression(memberAccess, out var localTemp, node);
                CurBlock.Statements.Add(localTemp);
              }
              return BuildPrefixUnaryExpression(isSingleLine, operatorToken, memberAccess, node);
            } else if (operand is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
              if (propertyAdapter.Expression != null) {
                var getAdapter = GetTempPropertyUnaryExpression(propertyAdapter, out var localTemp, node);
                CurBlock.Statements.Add(localTemp);
                return BuildPropertyPrefixUnaryExpression(isSingleLine, operatorToken, getAdapter, getAdapter.GetClone(), node);
              } else {
                return BuildPropertyPrefixUnaryExpression(isSingleLine, operatorToken, propertyAdapter, propertyAdapter.GetClone(), node);
              }
            } else {
              bool isLocalVar = false;
              if (!isSingleLine) {
                SymbolKind symbolKind = semanticModel_.GetSymbolInfo(node.Operand).Symbol.Kind;
                if (symbolKind == SymbolKind.Parameter || symbolKind == SymbolKind.Local) {
                  isLocalVar = true;
                }
              }
              return BuildPrefixUnaryExpression(isSingleLine, operatorToken, operand, node, isLocalVar);
            }
          }
        case SyntaxKind.PointerIndirectionExpression: {
            var operand = (LuaExpressionSyntax)node.Operand.Accept(this);
            var identifier = new LuaPropertyOrEventIdentifierNameSyntax(true, LuaIdentifierNameSyntax.Empty);
            return new LuaPropertyAdapterExpressionSyntax(operand, identifier, true);
          }
        case SyntaxKind.BitwiseNotExpression when !IsLuaNewest: {
            var type = semanticModel_.GetTypeInfo(node.Operand).Type;
            var operand = (LuaExpressionSyntax)node.Operand.Accept(this);
            return new LuaInvocationExpressionSyntax(type.IsNullableType() ? LuaIdentifierNameSyntax.BitNotOfNull : LuaIdentifierNameSyntax.BitNot, operand);
          }
        default: {
            var operand = (LuaExpressionSyntax)node.Operand.Accept(this);
            string operatorToken = GetOperatorToken(node.OperatorToken);
            LuaPrefixUnaryExpressionSyntax unaryExpression = new LuaPrefixUnaryExpressionSyntax(operand, operatorToken);
            return unaryExpression;
          }
      }
    }

    private LuaSyntaxNode BuildPostfixUnaryExpression(bool isSingleLine, string operatorToken, LuaExpressionSyntax operand, SyntaxNode node) {
      if (isSingleLine) {
        LuaBinaryExpressionSyntax binary = new LuaBinaryExpressionSyntax(operand, operatorToken, LuaIdentifierNameSyntax.One);
        return new LuaAssignmentExpressionSyntax(operand, binary);
      } else {
        var temp = GetTempIdentifier(node);
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, operand));
        LuaBinaryExpressionSyntax binary = new LuaBinaryExpressionSyntax(temp, operatorToken, LuaIdentifierNameSyntax.One);
        CurBlock.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(operand, binary)));
        return temp;
      }
    }

    private LuaSyntaxNode BuildPropertyPostfixUnaryExpression(bool isSingleLine, string operatorToken, LuaPropertyAdapterExpressionSyntax get, LuaPropertyAdapterExpressionSyntax set, SyntaxNode node) {
      set.IsGetOrAdd = false;
      if (isSingleLine) {
        var binary = new LuaBinaryExpressionSyntax(get, operatorToken, LuaIdentifierNameSyntax.One);
        set.ArgumentList.AddArgument(binary);
        return set;
      } else {
        var temp = GetTempIdentifier(node);
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, get));
        LuaBinaryExpressionSyntax binary = new LuaBinaryExpressionSyntax(temp, operatorToken, LuaIdentifierNameSyntax.One);
        set.ArgumentList.AddArgument(binary);
        CurBlock.Statements.Add(new LuaExpressionStatementSyntax(set));
        return temp;
      }
    }

    public override LuaSyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node) {
      SyntaxKind kind = node.Kind();
      if (kind != SyntaxKind.PostIncrementExpression && kind != SyntaxKind.PostDecrementExpression) {
        throw new NotSupportedException();
      }

      bool isSingleLine = IsSingleLineUnary(node);
      string operatorToken = kind == SyntaxKind.PostIncrementExpression ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub;
      var operand = (LuaExpressionSyntax)node.Operand.Accept(this);

      if (operand is LuaMemberAccessExpressionSyntax memberAccess) {
        if (memberAccess.Expression != LuaIdentifierNameSyntax.This) {
          memberAccess = GetTempUnaryExpression(memberAccess, out var localTemp, node);
          CurBlock.Statements.Add(localTemp);
        }
        return BuildPostfixUnaryExpression(isSingleLine, operatorToken, memberAccess, node);
      } else if (operand is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        if (propertyAdapter.Expression != null) {
          var getAdapter = GetTempPropertyUnaryExpression(propertyAdapter, out var localTemp, node);
          CurBlock.Statements.Add(localTemp);
          return BuildPropertyPostfixUnaryExpression(isSingleLine, operatorToken, getAdapter, getAdapter.GetClone(), node);
        } else {
          return BuildPropertyPostfixUnaryExpression(isSingleLine, operatorToken, propertyAdapter, propertyAdapter.GetClone(), node);
        }
      } else {
        return BuildPostfixUnaryExpression(isSingleLine, operatorToken, operand, node);
      }
    }

    public override LuaSyntaxNode VisitContinueStatement(ContinueStatementSyntax node) {
      return LuaContinueAdapterStatementSyntax.Statement;
    }

    private void VisitLoopBody(StatementSyntax bodyStatement, LuaBlockSyntax block) {
      bool hasContinue = IsContinueExists(bodyStatement);
      if (hasContinue) {
        // http://lua-users.org/wiki/ContinueProposal
        var continueIdentifier = LuaIdentifierNameSyntax.Continue;
        block.Statements.Add(new LuaLocalVariableDeclaratorSyntax(continueIdentifier));
        LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(LuaIdentifierNameSyntax.One);
        WriteStatementOrBlock(bodyStatement, repeatStatement.Body);
        LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(continueIdentifier, LuaIdentifierNameSyntax.True);
        repeatStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
        block.Statements.Add(repeatStatement);
        LuaIfStatementSyntax IfStatement = new LuaIfStatementSyntax(new LuaPrefixUnaryExpressionSyntax(continueIdentifier, LuaSyntaxNode.Tokens.Not));
        IfStatement.Body.Statements.Add(LuaBreakStatementSyntax.Statement);
        block.Statements.Add(IfStatement);
      } else {
        WriteStatementOrBlock(bodyStatement, block);
      }
    }

    private void CheckForeachCast(LuaIdentifierNameSyntax identifier, ForEachStatementSyntax node, LuaForInStatementSyntax forInStatement) {
      var sourceType = semanticModel_.GetTypeInfo(node.Expression).Type;
      var targetType = semanticModel_.GetTypeInfo(node.Type).Type;
      bool hasCast = false;
      var elementType = sourceType.GetIEnumerableElementType();
      if (elementType != null) {
        if (!elementType.Equals(targetType) && !elementType.IsSubclassOf(targetType)) {
          hasCast = true;
        }
      } else {
        if (targetType.SpecialType != SpecialType.System_Object) {
          hasCast = true;
        }
      }
      if (hasCast) {
        var cast = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Cast, GetTypeName(targetType), identifier);
        var assignment = new LuaAssignmentExpressionSyntax(identifier, cast);
        forInStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
      }
    }

    public override LuaSyntaxNode VisitForEachStatement(ForEachStatementSyntax node) {
      LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
      CheckLocalVariableName(ref identifier, node);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      LuaForInStatementSyntax forInStatement = new LuaForInStatementSyntax(identifier, expression);
      CheckForeachCast(identifier, node, forInStatement);
      VisitLoopBody(node.Statement, forInStatement.Body);
      return forInStatement;
    }

    public override LuaSyntaxNode VisitForEachVariableStatement(ForEachVariableStatementSyntax node) {
      var temp = GetTempIdentifier(node);
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      LuaForInStatementSyntax forInStatement = new LuaForInStatementSyntax(temp, expression);
      var left = (LuatLocalTupleVariableExpression)node.Variable.Accept(this);
      var elementType = semanticModel_.GetTypeInfo(node.Expression).Type;
      var right = BuildDeconstructExpression(elementType, temp, node.Expression);
      forInStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(left, right)));
      VisitLoopBody(node.Statement, forInStatement.Body);
      return forInStatement;
    }

    public override LuaSyntaxNode VisitWhileStatement(WhileStatementSyntax node) {
      var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
      LuaWhileStatementSyntax whileStatement = new LuaWhileStatementSyntax(condition);
      VisitLoopBody(node.Statement, whileStatement.Body);
      return whileStatement;
    }

    public override LuaSyntaxNode VisitForStatement(ForStatementSyntax node) {
      var numericalForStatement = GetNumericalForStatement(node);
      if (numericalForStatement != null) {
        return numericalForStatement;
      }

      LuaBlockSyntax block = new LuaBlockStatementSyntax();
      blocks_.Push(block);

      if (node.Declaration != null) {
        block.Statements.Add((LuaVariableDeclarationSyntax)node.Declaration.Accept(this));
      }
      var initializers = node.Initializers.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
      block.Statements.AddRange(initializers);

      LuaExpressionSyntax condition = node.Condition != null ? (LuaExpressionSyntax)node.Condition.Accept(this) : LuaIdentifierNameSyntax.True;
      LuaWhileStatementSyntax whileStatement = new LuaWhileStatementSyntax(condition);
      blocks_.Push(whileStatement.Body);
      VisitLoopBody(node.Statement, whileStatement.Body);
      var incrementors = node.Incrementors.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
      whileStatement.Body.Statements.AddRange(incrementors);
      blocks_.Pop();
      block.Statements.Add(whileStatement);
      blocks_.Pop();

      return block;
    }

    public override LuaSyntaxNode VisitDoStatement(DoStatementSyntax node) {
      var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
      var newCondition = new LuaPrefixUnaryExpressionSyntax(new LuaParenthesizedExpressionSyntax(condition), LuaSyntaxNode.Tokens.Not);
      LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(newCondition);
      VisitLoopBody(node.Statement, repeatStatement.Body);
      return repeatStatement;
    }

    public override LuaSyntaxNode VisitYieldStatement(YieldStatementSyntax node) {
      CurFunction.HasYield = true;
      if (node.IsKind(SyntaxKind.YieldBreakStatement)) {
        return new LuaReturnStatementSyntax();
      } else {
        var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
        LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.YieldReturn);
        invocationExpression.AddArgument(expression);
        return new LuaExpressionStatementSyntax(invocationExpression);
      }
    }

    public override LuaSyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node) {
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
      CheckPrevIsInvokeStatement();
      return new LuaParenthesizedExpressionSyntax(expression);
    }

    /// <summary>
    /// http://lua-users.org/wiki/TernaryOperator
    /// </summary>
    public override LuaSyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node) {
      bool mayBeNullOrFalse = MayBeNullOrFalse(node.WhenTrue);
      if (mayBeNullOrFalse) {
        var temp = GetTempIdentifier(node);
        var condition = (LuaExpressionSyntax)node.Condition.Accept(this);
        LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
        blocks_.Push(ifStatement.Body);
        var whenTrue = (LuaExpressionSyntax)node.WhenTrue.Accept(this);
        blocks_.Pop();
        ifStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(temp, whenTrue)));

        LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
        blocks_.Push(elseClause.Body);
        var whenFalse = (LuaExpressionSyntax)node.WhenFalse.Accept(this);
        blocks_.Pop();
        elseClause.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(temp, whenFalse)));

        ifStatement.Else = elseClause;
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp));
        CurBlock.Statements.Add(ifStatement);
        return temp;
      } else {
        LuaExpressionSyntax Accept(ExpressionSyntax expressionNode) {
          var expression = (LuaExpressionSyntax)expressionNode.Accept(this);
          return expressionNode.IsKind(SyntaxKind.LogicalAndExpression) || expressionNode.IsKind(SyntaxKind.LogicalOrExpression) ? new LuaParenthesizedExpressionSyntax(expression) : expression;
        }

        var condition = Accept(node.Condition);
        var whenTrue = Accept(node.WhenTrue);
        var whenFalse = Accept(node.WhenFalse);
        return new LuaBinaryExpressionSyntax(new LuaBinaryExpressionSyntax(condition, LuaSyntaxNode.Tokens.And, whenTrue), LuaSyntaxNode.Tokens.Or, whenFalse);
      }
    }

    public override LuaSyntaxNode VisitGotoStatement(GotoStatementSyntax node) {
      if (node.CaseOrDefaultKeyword.IsKind(SyntaxKind.CaseKeyword)) {
        const string kCaseLabel = "caseLabel";
        var switchStatement = switchs_.Peek();
        int caseIndex = GetCaseLabelIndex(node);
        var labelIdentifier = switchStatement.CaseLabels.GetOrDefault(caseIndex);
        if (labelIdentifier == null) {
          string uniqueName = GetUniqueIdentifier(kCaseLabel + caseIndex, node);
          labelIdentifier = new LuaIdentifierNameSyntax(uniqueName);
          switchStatement.CaseLabels.Add(caseIndex, labelIdentifier);
        }
        return new LuaGotoCaseAdapterStatement(labelIdentifier);
      } else if (node.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword)) {
        const string kDefaultLabel = "defaultLabel";
        var switchStatement = switchs_.Peek();
        if (switchStatement.DefaultLabel == null) {
          string identifier = GetUniqueIdentifier(kDefaultLabel, node);
          switchStatement.DefaultLabel = new LuaIdentifierNameSyntax(identifier);
        }
        return new LuaGotoCaseAdapterStatement(switchStatement.DefaultLabel);
      } else {
        var identifier = (LuaIdentifierNameSyntax)node.Expression.Accept(this);
        return new LuaGotoStatement(identifier);
      }
    }

    public override LuaSyntaxNode VisitLabeledStatement(LabeledStatementSyntax node) {
      LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(node.Identifier.ValueText);
      var statement = (LuaStatementSyntax)node.Statement.Accept(this);
      return new LuaLabeledStatement(identifier, statement);
    }

    public override LuaSyntaxNode VisitEmptyStatement(EmptyStatementSyntax node) {
      return LuaStatementSyntax.Empty;
    }

    public override LuaSyntaxNode VisitCastExpression(CastExpressionSyntax node) {
      var constExpression = GetConstExpression(node);
      if (constExpression != null) {
        return constExpression;
      }

      var originalType = semanticModel_.GetTypeInfo(node.Expression).Type;
      var targetType = semanticModel_.GetTypeInfo(node.Type).Type;
      var expression = (LuaExpressionSyntax)node.Expression.Accept(this);

      if (targetType.IsAssignableFrom(originalType)) {
        return expression;
      }

      if (targetType.TypeKind == TypeKind.Enum) {
        if (originalType.TypeKind == TypeKind.Enum || originalType.IsIntegerType()) {
          return expression;
        }
      }

      if (targetType.IsIntegerType()) {
        if (originalType.TypeKind == TypeKind.Enum) {
          return expression;
        }

        if (originalType.IsIntegerType()) {
          return GetCastToNumberExpression(expression, targetType, false);
        }

        if (originalType.SpecialType == SpecialType.System_Double || originalType.SpecialType == SpecialType.System_Single) {
          return GetCastToNumberExpression(expression, targetType, true);
        }
      } else if (targetType.SpecialType == SpecialType.System_Single && originalType.SpecialType == SpecialType.System_Double) {
        return GetCastToNumberExpression(expression, targetType, true);
      }

      if (originalType.IsAssignableFrom(targetType)) {
        return BuildCastExpression(node.Type, expression);
      }

      var explicitSymbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
      if (explicitSymbol != null) {
        return BuildConversionExpression(explicitSymbol, expression);
      }

      return BuildCastExpression(node.Type, expression);
    }

    private LuaExpressionSyntax BuildCastExpression(TypeSyntax type, LuaExpressionSyntax expression) {
      var typeExpression = (LuaExpressionSyntax)type.Accept(this);
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Cast, typeExpression, expression);
    }

    private LuaExpressionSyntax GetCastToNumberExpression(LuaExpressionSyntax expression, ITypeSymbol targetType, bool isFromFloat) {
      string name = (isFromFloat ? "To" : "to") + targetType.Name;
      var methodName = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.System, new LuaIdentifierNameSyntax(name));
      return new LuaInvocationExpressionSyntax(methodName, expression);
    }

    public override LuaSyntaxNode VisitCheckedStatement(CheckedStatementSyntax node) {
      bool isChecked = node.Keyword.Kind() == SyntaxKind.CheckedKeyword;
      PushChecked(isChecked);
      LuaStatementListSyntax statements = new LuaStatementListSyntax();
      statements.Statements.Add(new LuaShortCommentStatement(" " + node.Keyword.ValueText));
      var block = (LuaStatementSyntax)node.Block.Accept(this);
      statements.Statements.Add(block);
      PopChecked();
      return statements;
    }

    public override LuaSyntaxNode VisitCheckedExpression(CheckedExpressionSyntax node) {
      bool isChecked = node.Keyword.Kind() == SyntaxKind.CheckedKeyword;
      PushChecked(isChecked);
      var expression = node.Expression.Accept(this);
      PopChecked();
      return expression;
    }
  }
}