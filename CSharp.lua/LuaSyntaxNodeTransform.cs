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
    private int noImportTypeNameCounter_;
    public bool IsNoImportTypeName => noImportTypeNameCounter_ > 0;
    public bool IsNoneGenericTypeCounter => generator_.IsNoneGenericTypeCounter;
    private int metadataTypeNameCounter_;
    public bool IsMetadataTypeName => metadataTypeNameCounter_ > 0;

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

    private INamedTypeSymbol CurTypeSymbol {
      get {
        return typeDeclarations_.Peek().TypeSymbol;
      }
    }

    internal TypeDeclarationInfo CurTypeDeclaration {
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
        if (statement is LuaTypeDeclarationSyntax typeDeclaration) {
          var ns = new LuaNamespaceDeclarationSyntax(LuaIdentifierNameSyntax.Empty);
          ns.AddStatement(typeDeclaration);
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
      string name = generator_.GetNamespaceDefineName(symbol, node);
      LuaNamespaceDeclarationSyntax namespaceDeclaration = new LuaNamespaceDeclarationSyntax(name, isContained);
      var statements = VisitTriviaAndNode(node, node.Members);
      namespaceDeclaration.AddStatements(statements);
      return namespaceDeclaration;
    }

    private void BuildTypeMembers(LuaTypeDeclarationSyntax typeDeclaration, TypeDeclarationSyntax node) {
      if (!node.IsKind(SyntaxKind.InterfaceDeclaration)) {
        foreach (var nestedTypeDeclaration in node.Members.Where(i => i.Kind().IsTypeDeclaration())) {
          var luaNestedTypeDeclaration = (LuaTypeDeclarationSyntax)nestedTypeDeclaration.Accept(this);
          typeDeclaration.AddNestedTypeDeclaration(luaNestedTypeDeclaration);
        }

        foreach (var member in node.Members.Where(i => !i.Kind().IsTypeDeclaration())) {
          member.Accept(this);
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
          bool isLazy = argumentType.Kind != SymbolKind.TypeParameter && argumentType.IsFromCode();
          var typeName = isLazy ?  GetTypeNameWithoutImport(argumentType) : GetTypeName(argumentType);
          return new LuaSpeaicalGenericType() {
            Name = LuaIdentifierNameSyntax.GenericT,
            Value = typeName,
            IsLazy = isLazy
          };
        }
      }
      return null;
    }

    private List<LuaIdentifierNameSyntax> GetBaseCopyFields(BaseTypeSyntax baseType) {
      if (baseType != null) {
        var baseTypeSymbol = semanticModel_.GetTypeInfo(baseType.Type).Type;
        if (baseTypeSymbol.TypeKind == TypeKind.Class && baseTypeSymbol.SpecialType != SpecialType.System_Object) {
          if (baseTypeSymbol.IsMemberExists("Finalize", true)) {
            return new List<LuaIdentifierNameSyntax>() { LuaIdentifierNameSyntax.__GC };
          }
        }
      }
      return null;
    }

    private bool IsBaseTypeNotSystemObject(LuaExpressionSyntax baseTypeExpression) {
      return !(baseTypeExpression is LuaIdentifierNameSyntax name && name.ValueText == LuaIdentifierNameSyntax.Object.ValueText);
    }

    private void BuildTypeDeclaration(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
      typeDeclarations_.Push(new TypeDeclarationInfo(typeSymbol, typeDeclaration));

      var comments = BuildDocumentationComment(node);
      typeDeclaration.AddDocument(comments);

      var attributes = BuildAttributes(node.AttributeLists);
      BuildTypeParameters(typeSymbol, node, typeDeclaration);
      if (node.BaseList != null) {
        bool hasExtendSelf = false;
        var baseTypes = new List<LuaExpressionSyntax>();
        foreach (var baseType in node.BaseList.Types) {
          var baseTypeName = BuildInheritTypeName(baseType);
          if (IsBaseTypeNotSystemObject(baseTypeName)) {
            baseTypes.Add(baseTypeName);
            CheckBaseTypeGenericKind(ref hasExtendSelf, typeSymbol, baseType);
          }
        }

        if (baseTypes.Count > 0) {
          var genericArgument = CheckSpeaicalGenericArgument(typeSymbol);
          var baseCopyFields = GetBaseCopyFields(node.BaseList.Types.FirstOrDefault());
          typeDeclaration.AddBaseTypes(baseTypes, genericArgument, baseCopyFields);
          if (hasExtendSelf && !generator_.IsExplicitStaticCtorExists(typeSymbol)) {
            typeDeclaration.IsForceStaticCtor = true;
          }
        }
      }

      BuildTypeMembers(typeDeclaration, node);
      CheckTypeDeclaration(typeSymbol, typeDeclaration, attributes);

      typeDeclarations_.Pop();
      CurCompilationUnit.AddTypeDeclarationCount();
    }

    private void CheckTypeDeclaration(INamedTypeSymbol typeSymbol, LuaTypeDeclarationSyntax typeDeclaration, List<LuaExpressionSyntax> attributes) {
      if (typeDeclaration.IsNoneCtros) {
        var baseTypeSymbol = typeSymbol.BaseType;
        if (baseTypeSymbol != null) {
          bool isNeedCallBase;
          if (typeDeclaration.IsInitStatementExists) {
            isNeedCallBase = generator_.IsBaseExplicitCtorExists(baseTypeSymbol);
          } else {
            isNeedCallBase = generator_.HasStaticCtor(baseTypeSymbol);
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

      if (typeDeclaration.IsIgnoreExport) {
        generator_.AddIgnoreExportType(typeSymbol);
      }

      if (typeSymbol.TypeKind == TypeKind.Class) {
        if (IsCurTypeExportMetadataAll || attributes.Count > 0 || typeDeclaration.IsExportMetadata) {
          var data = new LuaTableExpression() { IsSingleLine = true };
          data.Add(typeSymbol.GetMetaDataAttributeFlags());
          data.AddRange(attributes);
          typeDeclaration.AddClassMetaData(data);
        }
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
          if (IsBaseTypeNotSystemObject(baseTypeName)) {
            baseTypeExpressions.Add(baseTypeName);
            CheckBaseTypeGenericKind(ref hasExtendSelf, major.Symbol, baseType);
          }
        }

        if (baseTypeExpressions.Count > 0) {
          var genericArgument = CheckSpeaicalGenericArgument(major.Symbol);
          var baseCopyFields = GetBaseCopyFields(baseTypes.FirstOrDefault());
          major.TypeDeclaration.AddBaseTypes(baseTypeExpressions, genericArgument, baseCopyFields);
          if (hasExtendSelf && !generator_.IsExplicitStaticCtorExists(major.Symbol)) {
            major.TypeDeclaration.IsForceStaticCtor = true;
          }
        }
      }

      foreach (var typeDeclaration in typeDeclarations) {
        semanticModel_ = generator_.GetSemanticModel(typeDeclaration.Node.SyntaxTree);
        BuildTypeMembers(major.TypeDeclaration, typeDeclaration.Node);
      }

      CheckTypeDeclaration(major.Symbol, major.TypeDeclaration, attributes);
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
      generator_.AddEnumDeclaration(typeSymbol, enumDeclaration);
      return enumDeclaration;
    }

    public override LuaSyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node) {
      return LuaStatementSyntax.Empty;
    }

    private void VisitYield(TypeSyntax returnType, LuaFunctionExpressionSyntax function) {
      Contract.Assert(function.HasYield);

      var retrurnTypeSymbol = semanticModel_.GetTypeInfo(returnType).Type;
      string name = LuaSyntaxNode.Tokens.Yield + retrurnTypeSymbol.Name;
      var memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.System, name);
      var invokeExpression = new LuaInvocationExpressionSyntax(memberAccess);
      var wrapFunction = new LuaFunctionExpressionSyntax();

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

      var returnStatement = new LuaReturnStatementSyntax(invokeExpression);
      function.Body.Statements.Clear();
      function.AddStatement(returnStatement);
    }

    private void VisitAsync(bool returnsVoid, TypeSyntax returnType, LuaFunctionExpressionSyntax function) {
      var memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.System, LuaIdentifierNameSyntax.Async);
      var invokeExpression = new LuaInvocationExpressionSyntax(memberAccess);
      var wrapFunction = new LuaFunctionExpressionSyntax();

      var parameters = function.ParameterList.Parameters;
      wrapFunction.AddParameter(LuaIdentifierNameSyntax.Async);
      wrapFunction.ParameterList.Parameters.AddRange(parameters);
      wrapFunction.AddStatements(function.Body.Statements);
      invokeExpression.AddArgument(wrapFunction);
      invokeExpression.AddArgument(returnsVoid ? LuaIdentifierNameSyntax.True : LuaIdentifierNameSyntax.Nil);
      invokeExpression.ArgumentList.Arguments.AddRange(parameters.Select(i => new LuaArgumentSyntax(i.Identifier)));

      function.Body.Statements.Clear();
      if (returnsVoid) {
        function.AddStatement(invokeExpression);
      } else {
        function.AddStatement(new LuaReturnStatementSyntax(invokeExpression));
      }
    }

    private sealed class MethodDeclarationResult {
      public IMethodSymbol Symbol;
      public LuaFunctionExpressionSyntax Function;
      public bool IsPrivate;
      public LuaIdentifierNameSyntax Name;
      public LuaDocumentStatement Document;
      public List<LuaExpressionSyntax> Attributes;
      public bool IsIgnore => Document != null && Document.HasIgnoreAttribute;
      public bool IsMetadata => (Document != null && Document.HasMetadataAttribute);
    }

    private void AddMethodMetaData(MethodDeclarationResult result) {
      var table = new LuaTableExpression() { IsSingleLine = true };
      table.Add(new LuaStringLiteralExpressionSyntax(result.Symbol.Name));
      table.Add(result.Symbol.GetMetaDataAttributeFlags());
      table.Add(result.Name);

      var parameters = result.Symbol.Parameters.Select(i => GetTypeNameOfMetadata(i.Type)).ToList();
      if (!result.Symbol.ReturnsVoid) {
        parameters.Add(GetTypeNameOfMetadata(result.Symbol.ReturnType));
      }
      if (result.Symbol.IsGenericMethod) {
        var function = new LuaFunctionExpressionSyntax();
        function.AddParameters(result.Symbol.TypeParameters.Select(i => new LuaParameterSyntax(i.Name)));
        function.AddStatement(new LuaMultipleReturnStatementSyntax(parameters));
        table.Add(function);
      } else {
        table.AddRange(parameters);
      }

      table.AddRange(result.Attributes);
      CurType.AddMethodMetaData(table);
    }

    private MethodDeclarationResult BuildMethodDeclaration(
      CSharpSyntaxNode node,
      SyntaxList<AttributeListSyntax> attributeLists,
      ParameterListSyntax parameterList,
      TypeParameterListSyntax typeParameterList,
      BlockSyntax body,
      ArrowExpressionClauseSyntax expressionBody,
      TypeSyntax returnType) {
      IMethodSymbol symbol = (IMethodSymbol)semanticModel_.GetDeclaredSymbol(node);
      List<LuaExpressionSyntax> refOrOutParameters = new List<LuaExpressionSyntax>();
      MethodInfo methodInfo = new MethodInfo(symbol, refOrOutParameters);
      methodInfos_.Push(methodInfo);

      LuaIdentifierNameSyntax methodName;
      if (symbol.MethodKind == MethodKind.LocalFunction) {
        methodName = symbol.Name;
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
        if (isPrivate) {
          if (generator_.IsForcePublicSymbol(symbol) || generator_.IsMonoBehaviourSpeicalMethod(symbol)) {
            isPrivate = false;
          }
        }
      } else if (symbol.IsMainEntryPoint()) {
        isPrivate = false;
        bool success = generator_.SetMainEntryPoint(symbol);
        if (!success) {
          throw new CompilationErrorException(node, "has more than one entry point");
        }
      } else if (isPrivate && generator_.IsForcePublicSymbol(symbol)) {
        isPrivate = false;
      }

      var attributes = BuildAttributes(attributeLists);
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
        if (symbol.ReturnsVoid) {
          function.AddStatement(expression);
        } else {
          function.AddStatement(new LuaReturnStatementSyntax(expression));
        }
      }

      if (function.HasYield) {
        VisitYield(returnType, function);
      } else if (symbol.IsAsync) {
        VisitAsync(symbol.ReturnsVoid, returnType, function);
      } else {
        if (symbol.ReturnsVoid && refOrOutParameters.Count > 0) {
          function.AddStatement(new LuaMultipleReturnStatementSyntax(refOrOutParameters));
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
        Attributes = attributes,
      };
    }

    private bool IsCurTypeExportMetadataAll {
      get {
        return generator_.Setting.IsExportMetadata || CurType.IsExportMetadataAll;
      }
    }

    private bool IsCurTypeSerializable {
      get {
        return IsCurTypeExportMetadataAll || CurTypeSymbol.IsSerializable;
      }
    }

    public override LuaSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
      if (!node.Modifiers.IsAbstract() && !node.Modifiers.IsExtern()) {
        var result = BuildMethodDeclaration(node, node.AttributeLists, node.ParameterList, node.TypeParameterList, node.Body, node.ExpressionBody, node.ReturnType);
        if (!result.IsIgnore) {
          CurType.AddMethod(result.Name, result.Function, result.IsPrivate, result.Document);
          if (IsCurTypeExportMetadataAll || result.Attributes.Count > 0 || result.IsMetadata) {
            AddMethodMetaData(result);
          }
        }
        return result.Function;
      }
      return base.VisitMethodDeclaration(node);
    }

    private static LuaExpressionSyntax GetPredefinedValueTypeDefaultValue(ITypeSymbol typeSymbol) {
      switch (typeSymbol.SpecialType) {
        case SpecialType.None: {
          if (typeSymbol.TypeKind == TypeKind.Enum) {
            return LuaNumberLiteralExpressionSyntax.Zero;
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
          return LuaNumberLiteralExpressionSyntax.Zero;
        }
        case SpecialType.System_Single:
        case SpecialType.System_Double: {
          return LuaNumberLiteralExpressionSyntax.ZeroFloat;
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
      return name;
    }

    private static LuaInvocationExpressionSyntax BuildDefaultValue(LuaExpressionSyntax typeExpression) {
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemDefault, typeExpression);
    }

    private void AddFieldMetaData(IFieldSymbol symbol, LuaIdentifierNameSyntax fieldName, List<LuaExpressionSyntax> attributes) {
      var data = new LuaTableExpression() { IsSingleLine = true };
      data.Add(new LuaStringLiteralExpressionSyntax(symbol.Name));
      data.Add(symbol.GetMetaDataAttributeFlags());
      data.Add(GetTypeNameOfMetadata(symbol.Type));
      if (generator_.IsNeedRefactorName(symbol)) {
        data.Add(new LuaStringLiteralExpressionSyntax(fieldName));
      }
      data.AddRange(attributes);
      CurType.AddFieldMetaData(data);
    }

    private void VisitBaseFieldDeclarationSyntax(BaseFieldDeclarationSyntax node) {
      if (!node.Modifiers.IsConst()) {
        bool isStatic = node.Modifiers.IsStatic();
        bool isPrivate = node.Modifiers.IsPrivate();
        bool isReadOnly = node.Modifiers.IsReadOnly();

        var attributes = BuildAttributes(node.AttributeLists);
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
              LuaExpressionSyntax valueExpression = GetFieldValueExpression(type, typeSymbol, variable.Initializer?.Value, out bool valueIsLiteral, out var statements);
              LuaExpressionSyntax typeExpression = null;
              if (isStatic) {
                typeExpression = GetTypeName(eventSymbol.ContainingType);
              }
              var (add, remove) = CurType.AddEvent(eventName, innerName, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, typeExpression, statements);
              if (attributes.Count > 0 || variableSymbol.HasMetadataAttribute()) {
                AddPropertyOrEventMetaData(variableSymbol, new PropertyMethodResult(add), new PropertyMethodResult(remove), null, attributes);
              }
              continue;
            }
          } else {
            if (!isStatic && isPrivate) {
              var fieldSymbol = (IFieldSymbol)variableSymbol;
              if (fieldSymbol.IsProtobufNetSpecialField(out string name)) {
                AddField(name, typeSymbol, type, variable.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, attributes);
                continue;
              }
            }
          }
          if (isPrivate && generator_.IsForcePublicSymbol(variableSymbol)) {
            isPrivate = false;
          }
          var fieldName = GetMemberName(variableSymbol);
          AddField(fieldName, typeSymbol, type, variable.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, attributes);
          if (IsCurTypeSerializable || attributes.Count > 0 || variableSymbol.HasMetadataAttribute()) {
            if (variableSymbol.Kind == SymbolKind.Field) {
              AddFieldMetaData((IFieldSymbol)variableSymbol, fieldName, attributes);
            } else {
              AddPropertyOrEventMetaData(variableSymbol, null, null, fieldName, attributes);
            }
          }
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
              if (isPrivate && generator_.IsForcePublicSymbol(variableSymbol)) {
                isPrivate = false;
              }
              var attributes = BuildAttributes(node.AttributeLists);
              LuaIdentifierNameSyntax fieldName = GetMemberName(variableSymbol);
              AddField(fieldName, typeSymbol, type, variable.Initializer.Value, true, true, isPrivate, true, attributes);
            }
          }
        }
      }
    }

    public override LuaSyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) {
      VisitBaseFieldDeclarationSyntax(node);
      return base.VisitFieldDeclaration(node);
    }

    private LuaExpressionSyntax GetFieldValueExpression(TypeSyntax type, ITypeSymbol typeSymbol, ExpressionSyntax expression, out bool valueIsLiteral, out List<LuaStatementSyntax> statements) {
      LuaExpressionSyntax valueExpression = null;
      valueIsLiteral = false;
      statements = null;

      if (expression != null && !expression.IsKind(SyntaxKind.NullLiteralExpression)) {
        var function = new LuaFunctionExpressionSyntax();
        PushFunction(function);
        blocks_.Push(function.Body);
        valueExpression = VisitExpression(expression);
        blocks_.Pop();
        PopFunction();
        if (function.Body.Statements.Count > 0) {
          statements = function.Body.Statements;
        }
        valueIsLiteral = valueExpression is LuaLiteralExpressionSyntax;
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

    private void AddField(LuaIdentifierNameSyntax name, ITypeSymbol typeSymbol, TypeSyntax type, ExpressionSyntax expression, bool isImmutable, bool isStatic, bool isPrivate, bool isReadOnly, List<LuaExpressionSyntax> attributes) {
      var valueExpression = GetFieldValueExpression(type, typeSymbol, expression, out bool valueIsLiteral, out var statements);
      CurType.AddField(name, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, isReadOnly, statements);
    }

    private sealed class PropertyMethodResult {
      public LuaPropertyOrEventIdentifierNameSyntax Name { get; }
      public List<LuaExpressionSyntax> Attributes { get; }

      public PropertyMethodResult(LuaPropertyOrEventIdentifierNameSyntax name, List<LuaExpressionSyntax> attributes = null) {
        Name = name;
        Attributes = attributes;
      }
    }

    private void AddPropertyOrEventMetaData(ISymbol symol, PropertyMethodResult get, PropertyMethodResult set, LuaIdentifierNameSyntax name, List<LuaExpressionSyntax> attributes) {
      bool isProperty = symol.Kind == SymbolKind.Property;
      PropertyMethodKind kind;
      if (get == null && set == null) {
        kind = PropertyMethodKind.Field;
      } else if (get != null) {
        kind = PropertyMethodKind.GetOnly;
      } else if (set != null) {
        kind = PropertyMethodKind.SetOnly;
      } else {
        kind = PropertyMethodKind.Both;
      }
      var data = new LuaTableExpression() { IsSingleLine = true };
      data.Add(new LuaStringLiteralExpressionSyntax(symol.Name));
      data.Add(symol.GetMetaDataAttributeFlags(kind));
      var type = isProperty ? ((IPropertySymbol)symol).Type : ((IEventSymbol)symol).Type;
      data.Add(GetTypeNameOfMetadata(type));
      if (kind == PropertyMethodKind.Field) {
        if (generator_.IsNeedRefactorName(symol)) {
          data.Add(new LuaStringLiteralExpressionSyntax(name));
        }
      } else {
        if (get != null) {
          if (get.Attributes.IsNullOrEmpty()) {
            data.Add(get.Name);
          } else {
            var getTable = new LuaTableExpression();
            getTable.Add(get.Name);
            getTable.AddRange(get.Attributes);
            data.Add(getTable);
          }
        }
        if (set != null) {
          if (set.Attributes.IsNullOrEmpty()) {
            data.Add(set.Name);
          } else {
            var setTable = new LuaTableExpression();
            setTable.Add(set.Name);
            setTable.AddRange(set.Attributes);
            data.Add(setTable);
          }
        }
      }
      data.AddRange(attributes);
      if (isProperty) {
        CurType.AddPropertyMetaData(data);
      } else {
        CurType.AddEventMetaData(data);
      }
    }

    public override LuaSyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
      if (!node.Modifiers.IsAbstract()) {
        var symbol = semanticModel_.GetDeclaredSymbol(node);
        if (symbol.IsProtobufNetSpecialProperty()) {
          return null;
        }

        bool isStatic = symbol.IsStatic;
        bool isPrivate = symbol.IsPrivate();
        bool hasGet = false;
        bool hasSet = false;
        if (isPrivate && generator_.IsForcePublicSymbol(symbol)) {
          isPrivate = false;
        }

        var propertyName = GetMemberName(symbol);
        var attributes = BuildAttributes(node.AttributeLists);
        PropertyMethodResult getMethod = null;
        PropertyMethodResult setMethod = null;
        if (node.AccessorList != null) {
          foreach (var accessor in node.AccessorList.Accessors) {
            if (accessor.Body != null || accessor.ExpressionBody != null) {
              bool isGet = accessor.IsKind(SyntaxKind.GetAccessorDeclaration);
              var functionExpression = new LuaFunctionExpressionSyntax();
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
              var name = new LuaPropertyOrEventIdentifierNameSyntax(true, propertyName);
              CurType.AddMethod(name, functionExpression, isPrivate);

              var methodAttributes = BuildAttributes(accessor.AttributeLists);
              if (isGet) {
                Contract.Assert(!hasGet);
                hasGet = true;
                getMethod = new PropertyMethodResult(name, methodAttributes);
              } else {
                Contract.Assert(!hasSet);
                functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
                name.IsGetOrAdd = false;
                hasSet = true;
                setMethod = new PropertyMethodResult(name, methodAttributes);
              }
            }
          }
        } else {
          Contract.Assert(!hasGet);
          var name = new LuaPropertyOrEventIdentifierNameSyntax(true, propertyName);
          var functionExpression = new LuaFunctionExpressionSyntax();
          PushFunction(functionExpression);
          blocks_.Push(functionExpression.Body);
          var expression = (LuaExpressionSyntax)node.ExpressionBody.Accept(this);
          blocks_.Pop();
          PopFunction();

          if (!isStatic) {
            functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
          }
          var returnStatement = new LuaReturnStatementSyntax(expression);
          functionExpression.AddStatement(returnStatement);
          CurType.AddMethod(name, functionExpression, isPrivate);
          hasGet = true;
          getMethod = new PropertyMethodResult(name);
        }

        if (!hasGet && !hasSet) {
          ITypeSymbol typeSymbol = symbol.Type;
          bool isImmutable = typeSymbol.IsImmutable();
          bool isField = IsPropertyField(semanticModel_.GetDeclaredSymbol(node));
          if (isField) {
            bool isReadOnly = IsReadOnlyProperty(node);
            AddField(propertyName, typeSymbol, node.Type, node.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, attributes);
          } else {
            LuaIdentifierNameSyntax innerName = AddInnerName(symbol);
            LuaExpressionSyntax valueExpression = GetFieldValueExpression(node.Type, typeSymbol, node.Initializer?.Value, out bool valueIsLiteral, out var statements);
            LuaExpressionSyntax typeExpression = null;
            if (isStatic) {
              typeExpression = GetTypeName(symbol.ContainingType);
            }
            var (getName, setName) =  CurType.AddProperty(propertyName, innerName, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, typeExpression, statements);
            getMethod = new PropertyMethodResult(getName);
            setMethod = new PropertyMethodResult(setName);
          }
        }

        if (IsCurTypeSerializable || attributes.Count > 0 || symbol.HasMetadataAttribute()) {
          AddPropertyOrEventMetaData(symbol, getMethod, setMethod, propertyName, attributes);
        }
      }

      return base.VisitPropertyDeclaration(node);
    }

    private bool IsReadOnlyProperty(PropertyDeclarationSyntax node) {
      return node.AccessorList.Accessors.Count == 1 && node.AccessorList.Accessors[0].Body == null;
    }

    public override LuaSyntaxNode VisitEventDeclaration(EventDeclarationSyntax node) {
      if (!node.Modifiers.IsAbstract()) {
        var attributes = BuildAttributes(node.AttributeLists);
        var symbol = semanticModel_.GetDeclaredSymbol(node);
        bool isStatic = symbol.IsStatic;
        bool isPrivate = symbol.IsPrivate();
        LuaIdentifierNameSyntax eventName = GetMemberName(symbol);
        PropertyMethodResult addMethod = null;
        PropertyMethodResult removeMethod = null;
        foreach (var accessor in node.AccessorList.Accessors) {
          var methodAttributes = BuildAttributes(accessor.AttributeLists);
          LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
          if (!isStatic) {
            functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
          }
          functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
          PushFunction(functionExpression);
          var block = (LuaBlockSyntax)accessor.Body.Accept(this);
          PopFunction();
          functionExpression.AddStatements(block.Statements);
          var name = new LuaPropertyOrEventIdentifierNameSyntax(false, eventName);
          CurType.AddMethod(name, functionExpression, isPrivate);
          if (accessor.IsKind(SyntaxKind.RemoveAccessorDeclaration)) {
            name.IsGetOrAdd = false;
            removeMethod = new PropertyMethodResult(name, methodAttributes);
          } else {
            addMethod = new PropertyMethodResult(name, methodAttributes);
          }
        }
        
        if (attributes.Count > 0 || symbol.HasMetadataAttribute()) {
          AddPropertyOrEventMetaData(symbol, addMethod, removeMethod, null, attributes);
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
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
      var value = new LuaIdentifierLiteralExpressionSyntax(symbol.ConstantValue.ToString());
      return new LuaKeyValueTableItemSyntax(identifier, value);
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
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
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
            throw e.With(SyntaxNode);
          } catch (BugErrorException) {
            throw;
          } catch (Exception e) {
            if (e.InnerException is CompilationErrorException ex) {
              throw ex.With(SyntaxNode);
            }
            throw new BugErrorException(SyntaxNode, e);
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
              commentContent = commentContent.ReplaceNewline();
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
              string code = commentContent.Substring(begin, end - begin);
              statement = (LuaIdentifierNameSyntax)code.Trim();
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
          var expression = VisitExpression(node.Expression);
          returnStatement.Expressions.Add(expression);
        }
        result = returnStatement;
      } else {
        var curMethodInfo = CurMethodInfoOrNull;
        if (curMethodInfo != null && curMethodInfo.RefOrOutParameters.Count > 0) {
          LuaMultipleReturnStatementSyntax returnStatement = new LuaMultipleReturnStatementSyntax();
          if (node.Expression != null) {
            var expression = VisitExpression(node.Expression);
            returnStatement.Expressions.Add(expression);
          }
          returnStatement.Expressions.AddRange(curMethodInfo.RefOrOutParameters);
          result = returnStatement;
        } else {
          var expression = node.Expression != null ? VisitExpression(node.Expression) : null;
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

    private LuaExpressionSyntax BuildCommonAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, string operatorToken, ExpressionSyntax rightNode, ExpressionSyntax parnet) {
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        LuaExpressionSyntax expression = null;
        var geter = propertyAdapter.GetCloneOfGet();
        if (parnet != null) {
          expression = GetUserDefinedOperatorExpression(parnet, geter, right);
        }
        if (expression == null) {
          expression = new LuaBinaryExpressionSyntax(geter, operatorToken, right);
        }
        propertyAdapter.ArgumentList.AddArgument(expression);
        return propertyAdapter;
      } else {
        LuaExpressionSyntax expression = null;
        if (parnet != null) {
          expression = GetUserDefinedOperatorExpression(parnet, left, right);
        }
        if (expression == null) {
          bool isRightParenthesized = rightNode is BinaryExpressionSyntax || rightNode.IsKind(SyntaxKind.ConditionalExpression);
          if (isRightParenthesized) {
            right = new LuaParenthesizedExpressionSyntax(right);
          }
          expression = new LuaBinaryExpressionSyntax(left, operatorToken, right);
        }
        return new LuaAssignmentExpressionSyntax(left, expression);
      }
    }

    private LuaExpressionSyntax BuildCommonAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, string operatorToken, ExpressionSyntax parnet) {
      var left = (LuaExpressionSyntax)leftNode.Accept(this);
      var right = VisitExpression(rightNode);
      return BuildCommonAssignmentExpression(left, right, operatorToken, rightNode, parnet);
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
        return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.IntegerDiv, null);
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

    private LuaExpressionSyntax BuildLuaAssignmentExpression(ExpressionSyntax parnet, ExpressionSyntax leftNode, ExpressionSyntax rightNode, SyntaxKind kind) {
      switch (kind) {
        case SyntaxKind.SimpleAssignmentExpression: {
          var left = (LuaExpressionSyntax)leftNode.Accept(this);
          var right = VisitExpression(rightNode);
          if (leftNode.Kind().IsTupleDeclaration()) {
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
            return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Concatenation, rightNode, null);
          } else {
            var left = (LuaExpressionSyntax)leftNode.Accept(this);
            var right = (LuaExpressionSyntax)rightNode.Accept(this);

            if (leftType.IsDelegateType()) {
              return BuildDelegateAssignmentExpression(left, right, true);
            } else {
              return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Plus, rightNode, parnet);
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
            return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Sub, rightNode, parnet);
          }
        }
        case SyntaxKind.MultiplyAssignmentExpression: {
          return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Multiply, parnet);
        }
        case SyntaxKind.DivideAssignmentExpression: {
          var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
          var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
          if (leftType.IsIntegerType() && rightType.IsIntegerType()) {
            var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.IntegerDivOfNull : LuaIdentifierNameSyntax.IntegerDiv;
            return BuildIntegerDivAssignmentExpression(leftNode, rightNode, methodName);
          } else {
            return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Div, parnet);
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
          return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Mod, parnet);
        }
        case SyntaxKind.AndAssignmentExpression: {
          return BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.And, LuaSyntaxNode.Tokens.BitAnd, LuaIdentifierNameSyntax.BitAnd, LuaIdentifierNameSyntax.BitAndOfNull, parnet);
        }
        case SyntaxKind.ExclusiveOrAssignmentExpression: {
          return BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.NotEquals, LuaSyntaxNode.Tokens.BitXor, LuaIdentifierNameSyntax.BitXor, LuaIdentifierNameSyntax.BitXorOfNull, parnet);
        }
        case SyntaxKind.OrAssignmentExpression: {
          return BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Or, LuaSyntaxNode.Tokens.BitOr, LuaIdentifierNameSyntax.BitOr, LuaIdentifierNameSyntax.BitOrOfNull, parnet);
        }
        case SyntaxKind.LeftShiftAssignmentExpression: {
          if (IsLuaNewest) {
            return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.LeftShift, parnet);
          } else {
            var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
            var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
            var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? LuaIdentifierNameSyntax.ShiftLeftOfNull : LuaIdentifierNameSyntax.ShiftLeft;
            return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, methodName);
          }
        }
        case SyntaxKind.RightShiftAssignmentExpression: {
          if (IsLuaNewest) {
            return BuildCommonAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.RightShift, parnet);
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

    private LuaExpressionSyntax BuildBitAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, string boolOperatorToken, string otherOperatorToken, LuaIdentifierNameSyntax methodName, LuaIdentifierNameSyntax methodNameOfNull, ExpressionSyntax parnet) {
      var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
      if (leftType.SpecialType == SpecialType.System_Boolean) {
        return BuildCommonAssignmentExpression(leftNode, rightNode, boolOperatorToken, null);
      } else if (!IsLuaNewest) {
        var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
        if (leftType.IsNullableType() || rightType.IsNullableType()) {
          methodName = methodNameOfNull;
        }
        return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, methodName);
      } else {
        string operatorToken = GetOperatorToken(otherOperatorToken);
        return BuildCommonAssignmentExpression(leftNode, rightNode, operatorToken, parnet);
      }
    }

    private LuaExpressionSyntax InternalVisitAssignmentExpression(AssignmentExpressionSyntax node) {
      List<LuaExpressionSyntax> assignments = new List<LuaExpressionSyntax>();

      while (true) {
        var leftExpression = node.Left;
        var rightExpression = node.Right;
        var kind = node.Kind();

        if (rightExpression is AssignmentExpressionSyntax assignmentRight) {
          assignments.Add(BuildLuaAssignmentExpression(node, leftExpression, assignmentRight.Left, kind));
          node = assignmentRight;
        } else {
          assignments.Add(BuildLuaAssignmentExpression(node, leftExpression, rightExpression, kind));
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

    private bool IsInlineAssignment(AssignmentExpressionSyntax node) {
      bool isInlineAssignment = false;
      SyntaxKind kind = node.Parent.Kind();
      switch (kind) {
        case SyntaxKind.ExpressionStatement:
        case SyntaxKind.ForStatement:
          break;
        case SyntaxKind.ArrowExpressionClause: {
          var symbol = semanticModel_.GetDeclaredSymbol(node.Parent.Parent);
          switch (symbol.Kind) {
            case SymbolKind.Method:
              var method = (IMethodSymbol)symbol;
              if (!method.ReturnsVoid) {
                isInlineAssignment = true;
              }
              break;
            case SymbolKind.Property: {
              var property = (IPropertySymbol)symbol;
              if (!property.GetMethod.ReturnsVoid) {
                isInlineAssignment = true;
              }
              break;
            }
          }
          break;
        }
        case SyntaxKind.SimpleLambdaExpression:
        case SyntaxKind.ParenthesizedLambdaExpression: {
          var method = CurMethodInfoOrNull.Symbol;
          if (!method.ReturnsVoid) {
            isInlineAssignment = true;
          }
          break;
        }
        default:
          isInlineAssignment = true;
          break;
      }
      return isInlineAssignment;
    }

    public override LuaSyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) {
      var assignment = InternalVisitAssignmentExpression(node);
      if (IsInlineAssignment(node)) {
        CurBlock.Statements.Add(assignment);
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

    private LuaExpressionSyntax BuildInvokeRefOrOut(CSharpSyntaxNode node, LuaExpressionSyntax invocation, IEnumerable<LuaExpressionSyntax> refOrOutArguments) {
      var locals = new LuaLocalVariablesStatementSyntax();
      LuaMultipleAssignmentExpressionSyntax multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
      LuaStatementListSyntax propertyStatements = new LuaStatementListSyntax();

      void FillRefOrOutArguments() {
        foreach (var refOrOutArgument in refOrOutArguments) {
          // fn(out arr[0])
          if (refOrOutArgument is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
            var propertyTemp = GetTempIdentifier(node);
            locals.Variables.Add(propertyTemp);
            multipleAssignment.Lefts.Add(propertyTemp);

            var setPropertyAdapter = propertyAdapter.GetClone();
            setPropertyAdapter.IsGetOrAdd = false;
            setPropertyAdapter.ArgumentList.AddArgument(propertyTemp);
            propertyStatements.Statements.Add(setPropertyAdapter);
          } else {
            multipleAssignment.Lefts.Add(refOrOutArgument);
          }
        }
      }

      switch (node.Parent.Kind()) {
        case SyntaxKind.ExpressionStatement:
        case SyntaxKind.ConstructorDeclaration: {
          var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
          if (!symbol.ReturnsVoid || node.IsKind(SyntaxKind.ObjectCreationExpression)) {
            var temp = GetTempIdentifier(node);
            locals.Variables.Add(temp);
            multipleAssignment.Lefts.Add(temp);
          }
          FillRefOrOutArguments();
          multipleAssignment.Rights.Add(invocation);

          if (locals.Variables.Count > 0) {
            CurBlock.Statements.Add(locals);
          }
          if (propertyStatements.Statements.Count > 0) {
            CurBlock.Statements.Add(multipleAssignment);
            CurBlock.Statements.Add(propertyStatements);
            return LuaExpressionSyntax.EmptyExpression;
          } else {
            return multipleAssignment;
          }
        }
        default: {
          var temp = GetTempIdentifier(node);
          locals.Variables.Add(temp);
          multipleAssignment.Lefts.Add(temp);
          FillRefOrOutArguments();
          multipleAssignment.Rights.Add(invocation);
          CurBlock.Statements.Add(locals);
          CurBlock.Statements.Add(new LuaExpressionStatementSyntax(multipleAssignment));
          if (propertyStatements.Statements.Count > 0) {
            CurBlock.Statements.Add(propertyStatements);
          }
          return temp;
        }
      }
    }

    private LuaExpressionSyntax CheckCodeTemplateInvocationExpression(IMethodSymbol symbol, InvocationExpressionSyntax node) {
      if (node.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
        string codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(symbol);
        if (codeTemplate != null) {
          var argumentExpressions = new List<Func<LuaExpressionSyntax>>();
          var memberAccessExpression = (MemberAccessExpressionSyntax)node.Expression;
          if (symbol.IsExtensionMethod) {
            if (symbol.ReducedFrom != null) {
              argumentExpressions.Add(() => (LuaExpressionSyntax)memberAccessExpression.Expression.Accept(this));
            }
            if (symbol.ContainingType.IsSystemLinqEnumerable()) {
              CurCompilationUnit.ImportLinq();
            }
          }
          argumentExpressions.AddRange(node.ArgumentList.Arguments.Select(i => {
            Func<LuaExpressionSyntax> func = () => VisitExpression(i.Expression);
            return func;
          }));
          if (symbol.Parameters.Length > node.ArgumentList.Arguments.Count) {
            argumentExpressions.AddRange(symbol.Parameters.Skip(node.ArgumentList.Arguments.Count).Where(i => !i.IsParams).Select(i => {
              Func<LuaExpressionSyntax> func = () => GetDeafultParameterValue(i, node, true);
              return func;
            }));
          }
          var invocationExpression = InternalBuildCodeTemplateExpression(codeTemplate, memberAccessExpression.Expression, argumentExpressions, symbol.TypeArguments);
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
          if (symbol.MethodKind == MethodKind.DelegateInvoke) {
            foreach (var typeArgument in symbol.ContainingType.TypeArguments) {
              if (typeArgument.TypeKind == TypeKind.TypeParameter) {
                LuaExpressionSyntax typeName = GetTypeName(typeArgument);
                arguments.Add(typeName);
              }
            }
          } else {
            foreach (var typeArgument in symbol.TypeArguments) {
              LuaExpressionSyntax typeName = GetTypeName(typeArgument);
              arguments.Add(typeName);
            }
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

    private LuaInvocationExpressionSyntax CheckInvocationExpression(IMethodSymbol symbol, InvocationExpressionSyntax node, LuaExpressionSyntax expression) {
      LuaInvocationExpressionSyntax invocation;
      if (symbol != null && symbol.IsExtensionMethod) {
        if (expression is LuaMemberAccessExpressionSyntax memberAccess) {
          if (memberAccess.Name is LuaInternalMethodExpressionSyntax) {
            invocation = new LuaInvocationExpressionSyntax(memberAccess.Name);
            invocation.AddArgument(memberAccess.Expression);
          } else if (symbol.ReducedFrom != null) {
            invocation = BuildExtensionMethodInvocation(symbol.ReducedFrom, memberAccess.Expression, node);
          } else {
            invocation = new LuaInvocationExpressionSyntax(expression);
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

    private void CheckInvocationDeafultArguments(
      ISymbol symbol,
      ImmutableArray<IParameterSymbol> parameters,
      List<LuaExpressionSyntax> arguments,
      List<(NameColonSyntax Name, ExpressionSyntax Expression)> argumentNodeInfos,
      SyntaxNode node,
      bool isCheckCallerAttribute) {
      if (parameters.Length > arguments.Count) {
        var optionalParameters = parameters.Skip(arguments.Count);
        foreach (IParameterSymbol parameter in optionalParameters) {
          if (parameter.IsParams) {
            var arrayType = (IArrayTypeSymbol)parameter.Type;
            LuaExpressionSyntax emptyArray = BuildArray(arrayType.ElementType);
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
            if (paramsArgument.Name != null) {
              string name = paramsArgument.Name.Name.Identifier.ValueText;
              if (name != last.Name) {
                paramsArgument = argumentNodeInfos.First(i => i.Name != null && i.Name.Name.Identifier.ValueText == last.Name);
              }
            }
            var paramsType = semanticModel_.GetTypeInfo(paramsArgument.Expression).Type;
            if (paramsType.TypeKind != TypeKind.Array) {
              var arrayTypeSymbol = (IArrayTypeSymbol)last.Type;
              var array = BuildArray(arrayTypeSymbol.ElementType, arguments.Last());
              arguments[arguments.Count - 1] = array;
            }
          } else {
            int otherParameterCount = parameters.Length - 1;
            var arrayTypeSymbol = (IArrayTypeSymbol)last.Type;
            var paramsArguments = arguments.Skip(otherParameterCount).ToArray();
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
      var argumentNodeInfos = node.Arguments.Select(i => (i.NameColon, i.Expression)).ToList();
      CheckInvocationDeafultArguments(symbol, parameters, arguments, argumentNodeInfos, node.Parent, true);
    }

    private void CheckPrevIsInvokeStatement(ExpressionSyntax node) {
      SyntaxNode current = node;
      while (true) {
        var parent = current.Parent;
        if (parent == null) {
          return;
        }

        switch (parent.Kind()) {
          case SyntaxKind.Argument:
          case SyntaxKind.LocalDeclarationStatement:
          case SyntaxKind.CastExpression: {
            return;
          }

          default: {
            if (parent is AssignmentExpressionSyntax assignment && assignment.Right == current) {
              return;
            }
            break;
          }
        }

        if (parent.IsKind(SyntaxKind.ExpressionStatement)) {
          break;
        }
        current = parent;
      }

      var curBlock = CurBlockOrNull;
      if (curBlock != null) {
        for (int i = curBlock.Statements.Count - 1; i >= 0; --i) {
          var statement = curBlock.Statements[i];
          if (!(statement is LuaBlankLinesStatement) && !(statement is LuaCommentStatement)) {
            bool hasInsertColon = false;
            if (statement is LuaExpressionStatementSyntax expressionStatement) {
              if (expressionStatement.Expression is LuaInvocationExpressionSyntax) {
                hasInsertColon = true;
              } else if (expressionStatement.Expression is LuaAssignmentExpressionSyntax assignmentExpression) {
                if (assignmentExpression.Right is LuaInvocationExpressionSyntax) {
                  hasInsertColon = true;
                }
              }
            } else if (statement is LuaLocalDeclarationStatementSyntax declarationStatement) {
              if (declarationStatement.Declaration is LuaVariableListDeclarationSyntax variableList) {
                var last = variableList.Variables.Last();
                if (last.Initializer != null && last.Initializer.Value is LuaInvocationExpressionSyntax) {
                  hasInsertColon = true;
                }
              }
            }
            if (hasInsertColon) {
              curBlock.Statements.Add(LuaStatementSyntax.Colon);
            }
            break;
          }
        }
      }
    }

    private LuaExpressionSyntax BuildMemberAccessTargetExpression(ExpressionSyntax targetExpression) {
      var expression = (LuaExpressionSyntax)targetExpression.Accept(this);
      SyntaxKind kind = targetExpression.Kind();
      if ((kind >= SyntaxKind.NumericLiteralExpression && kind <= SyntaxKind.NullLiteralExpression) || (expression is LuaLiteralExpressionSyntax)) {
        CheckPrevIsInvokeStatement(targetExpression);
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
          return new LuaTableIndexAccessExpressionSyntax(targetExpression, elementIndex);
        }

        string codeTemplate = XmlMetaProvider.GetFieldCodeTemplate(fieldSymbol);
        if (codeTemplate != null) {
          return BuildCodeTemplateExpression(codeTemplate, node.Expression);
        }

        if (fieldSymbol.HasConstantValue) {
          return GetConstLiteralExpression(fieldSymbol);
        }

        if (XmlMetaProvider.IsFieldForceProperty(fieldSymbol)) {
          var expression = (LuaExpressionSyntax)node.Expression.Accept(this);
          var propertyIdentifierName = new LuaPropertyOrEventIdentifierNameSyntax(true, GetMemberName(symbol));
          return new LuaPropertyAdapterExpressionSyntax(expression, propertyIdentifierName, !fieldSymbol.IsStatic);
        }
      } else if (symbol.Kind == SymbolKind.Property) {
        var propertySymbol = (IPropertySymbol)symbol;
        bool isGet = node.IsGetExpressionNode();
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

      if (symbol.IsStatic && node.Expression.IsKind(SyntaxKind.IdentifierName) && CurTypeSymbol.IsContainsInternalSymbol(symbol)) {
        bool isOnlyName = false;
        switch (symbol.Kind) {
          case SymbolKind.Method: {
            isOnlyName = true;
            break;
          }
          case SymbolKind.Property:
          case SymbolKind.Event: {
            if (!generator_.IsPropertyFieldOrEventFiled(symbol)) {
              isOnlyName = true;
            }
            break;
          }
          case SymbolKind.Field: {
            if (symbol.IsPrivate()) {
              isOnlyName = true;
            }
            break;
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
          return BuildDelegateNameExpression((IMethodSymbol)symbol, expression, name, node);
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
        LuaIdentifierNameSyntax name = node.Name.Identifier.ValueText;
        return new LuaMemberAccessExpressionSyntax(expression, name, node.Parent.IsKind(SyntaxKind.InvocationExpression));
      }

      if (symbol.Kind == SymbolKind.NamedType) {
        var expressionSymbol = semanticModel_.GetSymbolInfo(node.Expression).Symbol;
        if (expressionSymbol.Kind == SymbolKind.Namespace) {
          return node.Name.Accept(this);
        }
      }

      return InternalVisitMemberAccessExpression(symbol, node);
    }

    private LuaExpressionSyntax BuildStaticFieldName(ISymbol symbol, bool isReadOnly, IdentifierNameSyntax node) {
      Contract.Assert(symbol.IsStatic);
      LuaIdentifierNameSyntax name = GetMemberName(symbol);
      bool isPrivate = symbol.IsPrivate();
      if (isPrivate && generator_.IsForcePublicSymbol(symbol)) {
        isPrivate = false;
      }
      if (!isPrivate) {
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
      } else {
        if (!CurTypeSymbol.IsContainsInternalSymbol(symbol)) {
          if (symbol.IsPrivate()) {
            generator_.AddForcePublicSymbol(symbol);
          }
          var typeName = GetTypeName(symbol.ContainingType);
          return new LuaMemberAccessExpressionSyntax(typeName, name);
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

    private bool IsEventAddOrRemoveIdentifierName(IdentifierNameSyntax node) {
      SyntaxNode current = node;
      while (true) {
        var parent = current.Parent;
        if (parent == null) {
          break;
        }

        var kind = parent.Kind();
        if (kind == SyntaxKind.AddAssignmentExpression || kind == SyntaxKind.SubtractAssignmentExpression) {
          var assignment = (AssignmentExpressionSyntax)parent;
          return assignment.Left == current;
        } else if (kind == SyntaxKind.SimpleMemberAccessExpression) {
          var memberAccessExpression = (MemberAccessExpressionSyntax)parent;
          if (memberAccessExpression.Name != current) {
            break;
          }
        } else {
          break;
        }

        current = parent;
      }

      return false;
    }

    private LuaExpressionSyntax VisitPropertyOrEventIdentifierName(IdentifierNameSyntax node, ISymbol symbol, bool isProperty) {
      bool isField, isReadOnly;
      if (isProperty) {
        var propertySymbol = (IPropertySymbol)symbol;
        isField = IsPropertyField(propertySymbol);
        isReadOnly = propertySymbol.IsReadOnly;
      } else {
        var eventSymbol = (IEventSymbol)symbol;
        isField = IsEventFiled(eventSymbol);
        isReadOnly = false;
        if (!isField) {
          if (!IsEventAddOrRemoveIdentifierName(node)) {
            isField = true;
          }
        }
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
            if (symbol.IsPrivate()) {
              generator_.AddForcePublicSymbol(symbol);
            }
            return newExpression;
          } else if (symbol.IsPrivate() && !CurTypeSymbol.IsContainsInternalSymbol(symbol)) {
            generator_.AddForcePublicSymbol(symbol);
            var usingStaticType = GetTypeName(symbol.ContainingType);
            return new LuaMemberAccessExpressionSyntax(usingStaticType, identifierName);
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
              if (symbol.IsPrivate() && !CurTypeSymbol.IsContainsInternalSymbol(symbol)) {
                generator_.AddForcePublicSymbol(symbol);
              }
              return new LuaPropertyAdapterExpressionSyntax(identifierName);
            }
          }
        }
      }
    }

    private static bool IsParentDelegateName(NameSyntax node) {
      return !node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) && !node.Parent.IsKind(SyntaxKind.InvocationExpression);
    }

    private sealed class GenericPlaceholder {
      public ITypeSymbol Symbol { get; }
      private int index_;
      public int TypeParameterIndex => index_ + 1;
      public bool IsTypeParameter => index_ != -1;
      public bool IsSwaped { get; private set; }

      public GenericPlaceholder(ITypeSymbol symbol, int index) {
        Symbol = symbol;
        index_ = index;
      }

      public GenericPlaceholder(ITypeSymbol symbol) : this(symbol, -1) {
      }

      public LuaExpressionSyntax Build(LuaSyntaxNodeTransform transform) {
        return IsTypeParameter ? TypeParameterIndex.ToString() : transform.GetTypeName(Symbol);
      }

      public static void Swap(List<GenericPlaceholder> placeholders, int x, int y) {
        var itemX = placeholders[x];
        var itemY = placeholders[y];
        placeholders[x] = itemY;
        placeholders[y] = itemX;

        itemX.IsSwaped = true;
        itemY.IsSwaped = true;
      }

      public static bool IsEnable(List<GenericPlaceholder> placeholders) {
        int index = 0;
        foreach (var placeholder in placeholders) {
          if (!placeholder.IsTypeParameter) {
            return true;
          }

          if (placeholder.index_ != index) {
            return true;
          }

          ++index;
        }

        return false;
      }
    }

    private sealed class TypeParameterPlaceholder {
      public ITypeSymbol Symbol;
      public int ParameterIndex;
    }

    private IMethodSymbol GetDelegateTargetMethodSymbol(CSharpSyntaxNode node) {
      var parent = node.Parent;
      switch (parent.Kind()) {
        case SyntaxKind.Argument: {
          var argument = (ArgumentSyntax)parent;
          var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(argument.Parent.Parent).Symbol;
          var parameter = GetParameterSymbol(symbol.OriginalDefinition, argument);
          var type = (INamedTypeSymbol)parameter.Type;
          return type.DelegateInvokeMethod;
        }
        case SyntaxKind.EqualsValueClause: {
          var variableDeclaration = (VariableDeclarationSyntax)parent.Parent.Parent;
          var type = (INamedTypeSymbol)semanticModel_.GetTypeInfo(variableDeclaration.Type).Type;
          return type.DelegateInvokeMethod;
        }
        case SyntaxKind.SimpleAssignmentExpression:
        case SyntaxKind.AddAssignmentExpression:
        case SyntaxKind.SubtractAssignmentExpression: {
          var assignment = (AssignmentExpressionSyntax)parent;
          var type = (INamedTypeSymbol)semanticModel_.GetTypeInfo(assignment.Left).Type;
          return type.DelegateInvokeMethod;
        }
        default:
          throw new InvalidProgramException();
      }
    }

    private LuaExpressionSyntax BuildDelegateNameExpression(IMethodSymbol symbol, LuaExpressionSyntax target, LuaExpressionSyntax name, CSharpSyntaxNode node) {
      LuaExpressionSyntax nameExpression;
      if (symbol.IsStatic) {
        nameExpression = name;
      } else {
        nameExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateMake, target, name);
      }

      if (symbol.IsGenericMethod) {
        var originalDefinition = symbol.OriginalDefinition;
        if (originalDefinition != symbol) {
          var targetMethodSymbol = GetDelegateTargetMethodSymbol(node);
          var targetTypeParameters = new List<TypeParameterPlaceholder>();
          foreach (var typeArgument in targetMethodSymbol.ContainingType.TypeArguments) {
            if (typeArgument.TypeKind == TypeKind.TypeParameter) {
              int parameterIndex = targetMethodSymbol.Parameters.IndexOf(i => i.Type.IsTypeParameterExists(typeArgument));
              Contract.Assert(parameterIndex != -1);
              targetTypeParameters.Add(new TypeParameterPlaceholder() {
                Symbol = typeArgument,
                ParameterIndex = parameterIndex,
              });
            }
          }

          int j = 0;
          var originalTypeParameters = new List<TypeParameterPlaceholder>();
          foreach (var originalTypeArgument in originalDefinition.TypeArguments) {
            Contract.Assert(originalTypeArgument.TypeKind == TypeKind.TypeParameter);
            int parameterIndex = originalDefinition.Parameters.IndexOf(i => i.Type.IsTypeParameterExists(originalTypeArgument));
            if (parameterIndex != -1) {
              originalTypeParameters.Add(new TypeParameterPlaceholder() {
                Symbol = originalTypeArgument,
                ParameterIndex = parameterIndex,
              });
            } else {
              var typeArgument = symbol.TypeArguments[j];
              Contract.Assert(typeArgument.TypeKind != TypeKind.TypeParameter);
              originalTypeParameters.Add(new TypeParameterPlaceholder() {
                Symbol = typeArgument,
                ParameterIndex = -1,
              });
            }
            ++j;
          }

          var placeholders = new List<GenericPlaceholder>();
          foreach (var originalTypeParameter in originalTypeParameters) {
            int parameterIndex = originalTypeParameter.ParameterIndex;
            if (parameterIndex != -1) {
              int index = targetTypeParameters.FindIndex(i => i.ParameterIndex == parameterIndex);
              if (index != -1) {
                placeholders.Add(new GenericPlaceholder(originalTypeParameter.Symbol, index));
              } else {
                var parameter = targetMethodSymbol.Parameters[parameterIndex];
                placeholders.Add(new GenericPlaceholder(parameter.Type));
              }
            } else {
              placeholders.Add(new GenericPlaceholder(originalTypeParameter.Symbol));
            }
          }

          if (GenericPlaceholder.IsEnable(placeholders)) {
            if (placeholders.TrueForAll(i => !i.IsTypeParameter)) {
              if (placeholders.Count <= 3) {
                string bindMethodName = LuaIdentifierNameSyntax.DelegateBind.ValueText + placeholders.Count;
                var invocationExpression = new LuaInvocationExpressionSyntax(bindMethodName, nameExpression);
                invocationExpression.AddArguments(placeholders.Select(i => i.Build(this)));
                return invocationExpression;
              }
            } else if (symbol.Parameters.Length == 2) {
              if (placeholders.Count == 2) {
                if (placeholders[0].TypeParameterIndex == 2 && placeholders[1].TypeParameterIndex == 1) {
                  string bindMethodName = LuaIdentifierNameSyntax.DelegateBind.ValueText + "2_1";
                  return new LuaInvocationExpressionSyntax(bindMethodName, nameExpression);
                }
              } else if (placeholders.Count == 1) {
                if (placeholders.First().TypeParameterIndex == 2) {
                  string bindMethodName = LuaIdentifierNameSyntax.DelegateBind.ValueText + "0_2";
                  return new LuaInvocationExpressionSyntax(bindMethodName, nameExpression);
                }
              }
            }

            var invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind, nameExpression, symbol.Parameters.Length.ToString());
            invocation.AddArguments(placeholders.Select(i => i.Build(this)));
            nameExpression = invocation;
          }
        }
      }

      return nameExpression;
    }

    private LuaExpressionSyntax BuildDelegateNameExpression(IMethodSymbol symbol, LuaExpressionSyntax name, CSharpSyntaxNode node) {
      return BuildDelegateNameExpression(symbol, LuaIdentifierNameSyntax.This, name, node);
    }

    private LuaExpressionSyntax GetMethodNameExpression(IMethodSymbol symbol, NameSyntax node) {
      LuaIdentifierNameSyntax methodName = GetMemberName(symbol);
      if (symbol.IsStatic) {
        if (CheckUsingStaticNameSyntax(symbol, node, methodName, out var outExpression)) {
          if (symbol.IsPrivate()) {
            generator_.AddForcePublicSymbol(symbol);
          }
          return outExpression;
        }
        if (IsInternalMember(node, symbol)) {
          if (IsParentDelegateName(node)) {
            return BuildDelegateNameExpression(symbol, methodName, node);
          }
          return new LuaInternalMethodExpressionSyntax(methodName);
        }
        return methodName;
      } else {
        if (IsInternalMember(node, symbol)) {
          if (IsParentDelegateName(node)) {
            return BuildDelegateNameExpression(symbol, methodName, node);
          }

          return new LuaInternalMethodExpressionSyntax(methodName);
        } else {
          if (IsInternalNode(node)) {
            if (IsParentDelegateName(node)) {
              return BuildDelegateNameExpression(symbol, new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, methodName), node);
            }

            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, methodName, true);
            return memberAccess;
          } else if (symbol.IsPrivate() && !CurTypeSymbol.IsContainsInternalSymbol(symbol)) {
            generator_.AddForcePublicSymbol(symbol);
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
          if (symbol.IsProtobufNetSpecialField(out string name)) {
            return new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, name);
          }
          return new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, GetMemberName(symbol));
        } else {
          return GetMemberName(symbol);
        }
      }
    }

    public override LuaSyntaxNode VisitIdentifierName(IdentifierNameSyntax node) {
      LuaIdentifierNameSyntax GetSampleName(ISymbol nodeSymbol) {
        LuaIdentifierNameSyntax nameIdentifier = nodeSymbol.Name;
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
          identifier = symbol.Name;
          break;
        }
        case SymbolKind.NamedType: {
          identifier = GetTypeName(symbol);
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
          identifier = VisitPropertyOrEventIdentifierName(node, symbol, true);
          break;
        }
        case SymbolKind.Event: {
          identifier = VisitPropertyOrEventIdentifierName(node, symbol, false);
          break;
        }
        default: {
          throw new NotSupportedException();
        }
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
      } else {
        CheckConversion(node.Expression, ref expression);
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

    private LuaExpressionSyntax InternalVisitLiteralExpression(LiteralExpressionSyntax node) {
      switch (node.Kind()) {
        case SyntaxKind.NumericLiteralExpression: {
          bool hasTransform = false;
          string value = node.Token.Text;
          value = value.Replace("_", "");
          if (value.StartsWith("0b") || value.StartsWith("0B")) {
            value = node.Token.ValueText;
            hasTransform = true;
          } else {
            int len = value.Length;
            int removeCount = 0;
            switch (value[len - 1]) {
              case 'f': 
              case 'F':
              case 'd':
              case 'D': {
                if (!value.StartsWith("0x") && !value.StartsWith("0X")) {
                  removeCount = 1;
                }
                break;
              }
              case 'L':
              case 'l': {
                removeCount = 1;
                if (len > 2) {
                  if (value[len - 2] == 'U' || value[len - 2] == 'u') {
                    removeCount = 2;
                  }
                }
                break;
              }
            }
            if (removeCount > 0) {
              value = value.Remove(len - removeCount);
            }
          }
          if (hasTransform) {
            return new LuaConstLiteralExpression(value, node.Token.Text);
          } else {
            return new LuaIdentifierLiteralExpressionSyntax(value);
          }
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
        case SyntaxKind.DefaultLiteralExpression: {
          var type = semanticModel_.GetTypeInfo(node).Type;
          return GetDefaultValueExpression(type);
        }
        default: {
          return new LuaIdentifierLiteralExpressionSyntax(node.Token.ValueText);
        }
      }
    }

    public override LuaSyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node) {
      return InternalVisitLiteralExpression(node);
    }

    public override LuaSyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) {
      var declaration = (LuaVariableDeclarationSyntax)node.Declaration.Accept(this);
      return new LuaLocalDeclarationStatementSyntax(declaration);
    }

    private bool IsValueTypeVariableDeclarationWithoutAssignment(ITypeSymbol typeSymbol, VariableDeclaratorSyntax variable) {
      var body = FindParentMethodBody(variable);
      if (body != null) {
        int index = body.Statements.IndexOf((StatementSyntax)variable.Parent.Parent);
        foreach (var i in body.Statements.Skip(index + 1)) {
          if (i.IsKind(SyntaxKind.ExpressionStatement)) {
            var expressionStatement = (ExpressionStatementSyntax)i;
            if (expressionStatement.Expression.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
              var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;
              if (assignment.Left.IsKind(SyntaxKind.IdentifierName)) {
                var identifierName = (IdentifierNameSyntax)assignment.Left;
                if (identifierName.Identifier.ValueText == variable.Identifier.ValueText) {
                  return false;
                }
              } else if (assignment.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
                var memberAccessExpression = (MemberAccessExpressionSyntax)assignment.Left;
                if (memberAccessExpression.Expression.IsKind(SyntaxKind.IdentifierName)) {
                  var identifierName = (IdentifierNameSyntax)memberAccessExpression.Expression;
                  if (identifierName.Identifier.ValueText == variable.Identifier.ValueText) {
                    return true;
                  }
                }
              }
            }
          }
        }
      }
      return false;
    }

    public override LuaSyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node) {
      ITypeSymbol typeSymbol = null;
      var variableListDeclaration = new LuaVariableListDeclarationSyntax();
      foreach (var variable in node.Variables) {
        if (variable.Initializer != null && variable.Initializer.Value.IsKind(SyntaxKind.RefExpression)) {
          var refExpression = (LuaExpressionSyntax)variable.Initializer.Value.Accept(this);
          AddLocalVariableMapping(new LuaSymbolNameSyntax(refExpression), variable);
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
            if (variableDeclarator.Initializer == null) {
              if (typeSymbol == null) {
                typeSymbol = semanticModel_.GetTypeInfo(node.Type).Type;
              }
              if (typeSymbol.IsCustomValueType() && IsValueTypeVariableDeclarationWithoutAssignment(typeSymbol, variable)) {
                var typeExpression = GetTypeName(typeSymbol);
                variableDeclarator.Initializer = new LuaEqualsValueClauseSyntax(BuildDefaultValue(typeExpression));
              }
            }
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
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
      CheckLocalVariableName(ref identifier, node);
      LuaVariableDeclaratorSyntax variableDeclarator = new LuaVariableDeclaratorSyntax(identifier);
      if (node.Initializer != null) {
        variableDeclarator.Initializer = (LuaEqualsValueClauseSyntax)node.Initializer.Accept(this);
      }
      return variableDeclarator;
    }

    public override LuaSyntaxNode VisitEqualsValueClause(EqualsValueClauseSyntax node) {
      var expression = VisitExpression(node.Value);
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
      var condition = VisitExpression(node.Condition);
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
        var condition = VisitExpression(ifStatement.Condition);
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

    private void FillSwitchSectionStatements(LuaBlockSyntax block, SwitchSectionSyntax node) {
      if (node.Statements.Count == 1 && node.Statements.First().IsKind(SyntaxKind.Block)) {
        var luaBlock = (LuaBlockSyntax)node.Statements.First().Accept(this);
        block.Statements.AddRange(luaBlock.Statements);
      } else {
        blocks_.Push(block);
        foreach (var statement in node.Statements) {
          var luaStatement = (LuaStatementSyntax)statement.Accept(this);
          block.Statements.Add(luaStatement);
        }
        blocks_.Pop();
      }
    }

    public override LuaSyntaxNode VisitSwitchSection(SwitchSectionSyntax node) {
      bool isDefault = node.Labels.Any(i => i.Kind() == SyntaxKind.DefaultSwitchLabel);
      if (isDefault) {
        LuaBlockSyntax block = new LuaBlockSyntax();
        FillSwitchSectionStatements(block, node);
        return block;
      } else {
        var expressions = node.Labels.Select(i => (LuaExpressionSyntax)i.Accept(this));
        var condition = expressions.Aggregate((x, y) => new LuaBinaryExpressionSyntax(x, LuaSyntaxNode.Tokens.Or, y));
        LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
        FillSwitchSectionStatements(ifStatement.Body, node);
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
        var switchStatement = (SwitchStatementSyntax)FindParent(node, SyntaxKind.SwitchStatement);
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

    private bool IsParnetTryStatement(SyntaxNode node) {
      bool isTry = false;
      FindParent(node, i => {
        var kind = i.Kind();
        if (kind <= SyntaxKind.ForEachStatement && kind >= SyntaxKind.WhileStatement) {
          return true;
        }
        if (kind == SyntaxKind.SwitchStatement) {
          return true;
        }
        if (kind <= SyntaxKind.FinallyClause && kind >= SyntaxKind.TryStatement) {
          isTry = true;
          return true;
        }
        return false;
      });
      return isTry;
    }

    public override LuaSyntaxNode VisitBreakStatement(BreakStatementSyntax node) {
      if (IsParnetTryStatement(node)) {
        return new LuaReturnStatementSyntax();
      }
      return LuaBreakStatementSyntax.Statement;
    }

    private LuaExpressionSyntax BuildEnumToStringExpression(ITypeSymbol typeInfo, LuaExpressionSyntax original) {
      AddExportEnum(typeInfo);
      LuaIdentifierNameSyntax typeName = GetTypeShortName(typeInfo);
      LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(original, LuaIdentifierNameSyntax.ToEnumString, true);
      return new LuaInvocationExpressionSyntax(memberAccess, typeName);
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
          return BuildEnumToStringExpression(typeInfo, original);
        }
      } else if (typeInfo.IsValueType) {
        LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(original, LuaIdentifierNameSyntax.ToStr, true);
        return new LuaInvocationExpressionSyntax(memberAccess);
      } else {
        return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemToString, original);
      }
    }

    private LuaExpressionSyntax BuildStringConcatExpression(BinaryExpressionSyntax node) {
      return BuildStringConcatExpression(node.Left, node.Right);
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
      var left = VisitExpression(node.Left);
      var right = VisitExpression(node.Right);
      return new LuaBinaryExpressionSyntax(left, operatorToken, right);
    }

    private LuaExpressionSyntax BuildBitExpression(BinaryExpressionSyntax node, string boolOperatorToken, LuaIdentifierNameSyntax otherName, LuaIdentifierNameSyntax nameOfNull) {
      var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
      if (leftType.SpecialType == SpecialType.System_Boolean) {
        return BuildBinaryExpression(node, boolOperatorToken);
      } else if (leftType.IsIntegerType()) {
        if (!IsLuaNewest) {
          var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
          var methodName = leftType.IsNullableType() || rightType.IsNullableType() ? nameOfNull : otherName;
          return BuildBinaryInvokeExpression(node, methodName);
        } else {
          string operatorToken = GetOperatorToken(node.OperatorToken);
          return BuildBinaryExpression(node, operatorToken);
        }
      }
      return null;
    }

    private LuaExpressionSyntax BuildLogicOrBinaryExpression(BinaryExpressionSyntax node) {
      var left = VisitExpression(node.Left);
      LuaBlockSyntax rightBody = new LuaBlockSyntax();
      blocks_.Push(rightBody);
      var right = VisitExpression(node.Right);
      blocks_.Pop();
      if (rightBody.Statements.Count == 0) {
        return new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.Or, right);
      } else {
        var temp = GetTempIdentifier(node);
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp));
        LuaIfStatementSyntax leftIfStatement = new LuaIfStatementSyntax(left);
        CurBlock.Statements.Add(leftIfStatement);
        leftIfStatement.Body.AddStatement(new LuaAssignmentExpressionSyntax(temp, LuaIdentifierNameSyntax.True));
        leftIfStatement.Else = new LuaElseClauseSyntax();
        leftIfStatement.Else.Body.Statements.AddRange(rightBody.Statements);
        LuaIfStatementSyntax rightIfStatement = new LuaIfStatementSyntax(right);
        leftIfStatement.Else.Body.AddStatement(rightIfStatement);
        rightIfStatement.Body.AddStatement(new LuaAssignmentExpressionSyntax(temp, LuaIdentifierNameSyntax.True));
        return temp;
      }
    }

    private LuaExpressionSyntax BuildLogicAndBinaryExpression(BinaryExpressionSyntax node) {
      var left = VisitExpression(node.Left);
      LuaBlockSyntax rightBody = new LuaBlockSyntax();
      blocks_.Push(rightBody);
      var right = VisitExpression(node.Right);
      blocks_.Pop();
      if (rightBody.Statements.Count == 0) {
        return new LuaBinaryExpressionSyntax(left, LuaSyntaxNode.Tokens.And, right);
      } else {
        var temp = GetTempIdentifier(node);
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp));
        LuaIfStatementSyntax leftIfStatement = new LuaIfStatementSyntax(left);
        CurBlock.Statements.Add(leftIfStatement);
        leftIfStatement.Body.Statements.AddRange(rightBody.Statements);
        LuaIfStatementSyntax rightIfStatement = new LuaIfStatementSyntax(right);
        leftIfStatement.Body.AddStatement(rightIfStatement);
        rightIfStatement.Body.AddStatement(new LuaAssignmentExpressionSyntax(temp, LuaIdentifierNameSyntax.True));
        return temp;
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
          var expression = BuildBitExpression(node, LuaSyntaxNode.Tokens.Or, LuaIdentifierNameSyntax.BitOr, LuaIdentifierNameSyntax.BitOrOfNull);
          if (expression != null) {
            return expression;
          }
          break;
        }
        case SyntaxKind.BitwiseAndExpression: {
          var expression = BuildBitExpression(node, LuaSyntaxNode.Tokens.And, LuaIdentifierNameSyntax.BitAnd, LuaIdentifierNameSyntax.BitAndOfNull);
          if (expression != null) {
            return expression;
          }
          break;
        }
        case SyntaxKind.ExclusiveOrExpression: {
          var expression = BuildBitExpression(node, LuaSyntaxNode.Tokens.NotEquals, LuaIdentifierNameSyntax.BitXor, LuaIdentifierNameSyntax.BitXorOfNull);
          if (expression != null) {
            return expression;
          }
          break;
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
        case SyntaxKind.LogicalOrExpression: {
          return BuildLogicOrBinaryExpression(node);
        }
        case SyntaxKind.LogicalAndExpression: {
          return BuildLogicAndBinaryExpression(node);
        }
      }

      var operatorExpression = GetUserDefinedOperatorExpression(node, node.Left, node.Right);
      if (operatorExpression != null) {
        return operatorExpression;
      }

      string operatorToken = GetOperatorToken(node.OperatorToken);
      return BuildBinaryExpression(node, operatorToken);
    }

    private bool IsSingleLineUnary(ExpressionSyntax node) {
      switch (node.Parent.Kind()) {
        case SyntaxKind.ExpressionStatement:
        case SyntaxKind.ForStatement: {
          return true;
        }
        case SyntaxKind.SimpleLambdaExpression:
        case SyntaxKind.ParenthesizedLambdaExpression: {
          var method = CurMethodInfoOrNull.Symbol;
          if (method.ReturnsVoid) {
            return true;
          }
          break;
        }
      }

      return false;
    }

    private void ChecktIncrementExpression(ExpressionSyntax operand, ref LuaExpressionSyntax expression, bool isAddOrAssignment) {
      var symbol = semanticModel_.GetTypeInfo(operand).Type;
      if (!symbol.IsNumberType()) {
        var op_Implicits = symbol.GetMembers("op_Implicit").OfType<IMethodSymbol>();
        var methodSymbol = op_Implicits.FirstOrDefault(i => isAddOrAssignment ? i.ReturnType.IsIntegerType() : i.ReturnType.Equals(symbol));
        if (methodSymbol != null) {
          expression = BuildConversionExpression(methodSymbol, expression);
        }
      }
    }

    private LuaSyntaxNode BuildPrefixUnaryExpression(bool isSingleLine, string operatorToken, LuaExpressionSyntax operand, PrefixUnaryExpressionSyntax node, bool isLocalVar = false) {
      var left = operand;
      ChecktIncrementExpression(node.Operand, ref left, true);
      LuaExpressionSyntax binary = new LuaBinaryExpressionSyntax(left, operatorToken, LuaIdentifierNameSyntax.One);
      ChecktIncrementExpression(node.Operand, ref binary, false);
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

    private LuaSyntaxNode BuildPropertyPrefixUnaryExpression(bool isSingleLine, string operatorToken, LuaPropertyAdapterExpressionSyntax get, LuaPropertyAdapterExpressionSyntax set, PrefixUnaryExpressionSyntax node) {
      set.IsGetOrAdd = false;
      LuaExpressionSyntax left = get;
      ChecktIncrementExpression(node.Operand, ref left, true);
      LuaExpressionSyntax binary = new LuaBinaryExpressionSyntax(left, operatorToken, LuaIdentifierNameSyntax.One);
      ChecktIncrementExpression(node.Operand, ref binary, false);
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
      var operatorExpression = GetUserDefinedOperatorExpression(node, node.Operand);
      if (operatorExpression != null) {
        return operatorExpression;
      }

      var operand = VisitExpression(node.Operand);
      SyntaxKind kind = node.Kind();
      switch (kind) {
        case SyntaxKind.PreIncrementExpression:
        case SyntaxKind.PreDecrementExpression: {
          bool isSingleLine = IsSingleLineUnary(node);
          string operatorToken = kind == SyntaxKind.PreIncrementExpression ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub;
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
          var identifier = new LuaPropertyOrEventIdentifierNameSyntax(true, LuaIdentifierNameSyntax.Empty);
          return new LuaPropertyAdapterExpressionSyntax(operand, identifier, true);
        }
        case SyntaxKind.BitwiseNotExpression when !IsLuaNewest: {
          var type = semanticModel_.GetTypeInfo(node.Operand).Type;
          return new LuaInvocationExpressionSyntax(type.IsNullableType() ? LuaIdentifierNameSyntax.BitNotOfNull : LuaIdentifierNameSyntax.BitNot, operand);
        }
        case SyntaxKind.UnaryPlusExpression: {
          return operand;
        }
        default: {
          string operatorToken = GetOperatorToken(node.OperatorToken);
          LuaPrefixUnaryExpressionSyntax unaryExpression = new LuaPrefixUnaryExpressionSyntax(operand, operatorToken);
          return unaryExpression;
        }
      }
    }

    private LuaSyntaxNode BuildPostfixUnaryExpression(bool isSingleLine, string operatorToken, LuaExpressionSyntax operand, PostfixUnaryExpressionSyntax node) {
      if (isSingleLine) {
        var left = operand;
        ChecktIncrementExpression(node.Operand, ref left, true);
        LuaExpressionSyntax binary = new LuaBinaryExpressionSyntax(left, operatorToken, LuaIdentifierNameSyntax.One);
        ChecktIncrementExpression(node.Operand, ref binary, false);
        return new LuaAssignmentExpressionSyntax(operand, binary);
      } else {
        var temp = GetTempIdentifier(node);
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, operand));
        LuaExpressionSyntax left = temp;
        ChecktIncrementExpression(node.Operand, ref left, true);
        LuaExpressionSyntax binary = new LuaBinaryExpressionSyntax(left, operatorToken, LuaIdentifierNameSyntax.One);
        ChecktIncrementExpression(node.Operand, ref binary, false);
        CurBlock.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(operand, binary)));
        return temp;
      }
    }

    private LuaSyntaxNode BuildPropertyPostfixUnaryExpression(bool isSingleLine, string operatorToken, LuaPropertyAdapterExpressionSyntax get, LuaPropertyAdapterExpressionSyntax set, PostfixUnaryExpressionSyntax node) {
      set.IsGetOrAdd = false;
      if (isSingleLine) {
        LuaExpressionSyntax left = get;
        ChecktIncrementExpression(node.Operand, ref left, true);
        LuaExpressionSyntax binary = new LuaBinaryExpressionSyntax(left, operatorToken, LuaIdentifierNameSyntax.One);
        ChecktIncrementExpression(node.Operand, ref binary, false);
        set.ArgumentList.AddArgument(binary);
        return set;
      } else {
        var temp = GetTempIdentifier(node);
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, get));
        LuaExpressionSyntax left = temp;
        ChecktIncrementExpression(node.Operand, ref left, true);
        LuaExpressionSyntax binary = new LuaBinaryExpressionSyntax(left, operatorToken, LuaIdentifierNameSyntax.One);
        ChecktIncrementExpression(node.Operand, ref binary, false);
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
      return new LuaContinueAdapterStatementSyntax(IsParnetTryStatement(node));
    }

    private void VisitLoopBody(StatementSyntax bodyStatement, LuaBlockSyntax block) {
      bool hasContinue = IsContinueExists(bodyStatement);
      if (hasContinue) {
        // http://lua-users.org/wiki/ContinueProposal
        var continueIdentifier = LuaIdentifierNameSyntax.Continue;
        block.Statements.Add(new LuaLocalVariableDeclaratorSyntax(continueIdentifier));
        LuaRepeatStatementSyntax repeatStatement = new LuaRepeatStatementSyntax(LuaIdentifierNameSyntax.One);
        WriteStatementOrBlock(bodyStatement, repeatStatement.Body);
        var lastStatement = repeatStatement.Body.Statements.Last();
        if (lastStatement is LuaBaseReturnStatementSyntax) {
          LuaBlockStatementSyntax returnBlock = new LuaBlockStatementSyntax();
          returnBlock.Statements.Add(lastStatement);
          repeatStatement.Body.Statements[repeatStatement.Body.Statements.Count - 1] = returnBlock;
        }
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
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
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
      var left = (LuaLocalTupleVariableExpression)node.Variable.Accept(this);
      var sourceType = semanticModel_.GetTypeInfo(node.Expression).Type;
      var elementType = sourceType.GetIEnumerableElementType();
      var right = BuildDeconstructExpression(elementType, temp, node.Expression);
      forInStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(left, right)));
      VisitLoopBody(node.Statement, forInStatement.Body);
      return forInStatement;
    }

    private LuaWhileStatementSyntax BuildWhileStatement(ExpressionSyntax nodeCondition, StatementSyntax nodeStatement) {
      LuaBlockSyntax conditionBody = new LuaBlockSyntax();
      blocks_.Push(conditionBody);
      var condition = nodeCondition != null ? VisitExpression(nodeCondition) : LuaIdentifierNameSyntax.True;
      blocks_.Pop();

      LuaWhileStatementSyntax whileStatement;
      if (conditionBody.Statements.Count == 0) {
        whileStatement = new LuaWhileStatementSyntax(condition);
      } else {
        whileStatement = new LuaWhileStatementSyntax(LuaIdentifierNameSyntax.True);
        if (condition is LuaBinaryExpressionSyntax) {
          condition = new LuaParenthesizedExpressionSyntax(condition);
        }
        LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(new LuaPrefixUnaryExpressionSyntax(condition, LuaSyntaxNode.Tokens.Not));
        ifStatement.Body.AddStatement(LuaBreakStatementSyntax.Statement);
        whileStatement.Body.Statements.AddRange(conditionBody.Statements);
        whileStatement.Body.Statements.Add(ifStatement);
      }
      VisitLoopBody(nodeStatement, whileStatement.Body);
      return whileStatement;
    }

    public override LuaSyntaxNode VisitWhileStatement(WhileStatementSyntax node) {
      return BuildWhileStatement(node.Condition, node.Statement);
    }

    public override LuaSyntaxNode VisitForStatement(ForStatementSyntax node) {
      var numericalForStatement = GetNumericalForStatement(node);
      if (numericalForStatement != null) {
        return numericalForStatement;
      }

      LuaBlockSyntax forBlock = new LuaBlockStatementSyntax();
      blocks_.Push(forBlock);

      if (node.Declaration != null) {
        forBlock.Statements.Add((LuaVariableDeclarationSyntax)node.Declaration.Accept(this));
      }
      var initializers = node.Initializers.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
      forBlock.Statements.AddRange(initializers);

      var whileStatement = BuildWhileStatement(node.Condition, node.Statement);
      blocks_.Push(whileStatement.Body);
      var incrementors = node.Incrementors.Select(i => new LuaExpressionStatementSyntax((LuaExpressionSyntax)i.Accept(this)));
      whileStatement.Body.Statements.AddRange(incrementors);
      blocks_.Pop();

      forBlock.Statements.Add(whileStatement);
      blocks_.Pop();

      return forBlock;
    }

    public override LuaSyntaxNode VisitDoStatement(DoStatementSyntax node) {
      LuaBlockSyntax body = new LuaBlockSyntax();
      blocks_.Push(body);
      VisitLoopBody(node.Statement, body);
      var condition = VisitExpression(node.Condition);
      if (condition is LuaBinaryExpressionSyntax) {
        condition = new LuaParenthesizedExpressionSyntax(condition);
      }
      var newCondition = new LuaPrefixUnaryExpressionSyntax(condition, LuaSyntaxNode.Tokens.Not);
      blocks_.Pop();
      return new LuaRepeatStatementSyntax(newCondition, body);
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
      if (expression is LuaIdentifierNameSyntax || expression is LuaMemberAccessExpressionSyntax) {
        return expression;
      }

      CheckPrevIsInvokeStatement(node);
      return new LuaParenthesizedExpressionSyntax(expression);
    }

    /// <summary>
    /// http://lua-users.org/wiki/TernaryOperator
    /// </summary>
    public override LuaSyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node) {
      bool mayBeNullOrFalse = MayBeNullOrFalse(node.WhenTrue);
      if (mayBeNullOrFalse) {
        var temp = GetTempIdentifier(node);
        var condition = VisitExpression(node.Condition);
        LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
        blocks_.Push(ifStatement.Body);
        var whenTrue = VisitExpression(node.WhenTrue);
        blocks_.Pop();
        ifStatement.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(temp, whenTrue)));

        LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
        blocks_.Push(elseClause.Body);
        var whenFalse = VisitExpression(node.WhenFalse);
        blocks_.Pop();
        elseClause.Body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(temp, whenFalse)));

        ifStatement.Else = elseClause;
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp));
        CurBlock.Statements.Add(ifStatement);
        return temp;
      } else {
        LuaExpressionSyntax Accept(ExpressionSyntax expressionNode) {
          var expression = VisitExpression(expressionNode);
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
          labelIdentifier = uniqueName;
          switchStatement.CaseLabels.Add(caseIndex, labelIdentifier);
        }
        return new LuaGotoCaseAdapterStatement(labelIdentifier);
      } else if (node.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword)) {
        const string kDefaultLabel = "defaultLabel";
        var switchStatement = switchs_.Peek();
        if (switchStatement.DefaultLabel == null) {
          string identifier = GetUniqueIdentifier(kDefaultLabel, node);
          switchStatement.DefaultLabel = identifier;
        }
        return new LuaGotoCaseAdapterStatement(switchStatement.DefaultLabel);
      } else {
        var identifier = (LuaIdentifierNameSyntax)node.Expression.Accept(this);
        return new LuaGotoStatement(identifier);
      }
    }

    public override LuaSyntaxNode VisitLabeledStatement(LabeledStatementSyntax node) {
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
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
          if (originalType.IsNullableType()) {
            var explicitMethod = (IMethodSymbol)originalType.GetMembers("op_Explicit").First();
            return BuildConversionExpression(explicitMethod, expression);
          }

          if (targetType.IsNullableType()) {
            return expression;
          }

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
      var methodName = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.System, name);
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
