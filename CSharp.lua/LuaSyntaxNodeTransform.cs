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
    public const int kStringConstInlineCount = 27;

    private sealed class MethodInfo {
      public IMethodSymbol Symbol { get; }
      public IList<LuaExpressionSyntax> RefOrOutParameters { get; }
      public List<LuaIdentifierNameSyntax> InliningReturnVars { get; set; }
      public bool IsInlining => InliningReturnVars != null;
      public bool HasYield;

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
        if (getNameTypeSymbol.EQ(TypeSymbol)) {
          TypeDeclaration.IsClassUsed = true;
          name = LuaIdentifierNameSyntax.Class;
          return true;
        }
        name = null;
        return false;
      }
    }

    private readonly LuaSyntaxGenerator generator_;
    private SemanticModel semanticModel_;

    private readonly Stack<LuaCompilationUnitSyntax> compilationUnits_ = new Stack<LuaCompilationUnitSyntax>();
    private readonly Stack<TypeDeclarationInfo> typeDeclarations_ = new Stack<TypeDeclarationInfo>();
    private readonly Stack<LuaFunctionExpressionSyntax> functions_ = new Stack<LuaFunctionExpressionSyntax>();
    private readonly Stack<MethodInfo> methodInfos_ = new Stack<MethodInfo>();
    private readonly Stack<LuaBlockSyntax> blocks_ = new Stack<LuaBlockSyntax>();
    private readonly Stack<LuaIfStatementSyntax> ifStatements_ = new Stack<LuaIfStatementSyntax>();
    private readonly Stack<LuaSwitchAdapterStatementSyntax> switchs_ = new Stack<LuaSwitchAdapterStatementSyntax>();

    private int releaseTempIdentifierCount_;
    private int noImportTypeNameCounter_;
    public bool IsNoImportTypeName => noImportTypeNameCounter_ > 0;
    private int genericTypeCounter_;
    public bool IsNoneGenericTypeCounter => genericTypeCounter_ == 0;
    public void AddGenericTypeCounter() => ++genericTypeCounter_;
    public void SubGenericTypeCounter() => --genericTypeCounter_;
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
      return GetOperatorToken(token);
    }

    private static string GetOperatorToken(string token) {
      return operatorTokenMapps_.GetOrDefault(token, token);
    }

    private bool IsLuaClassic => generator_.Setting.IsClassic;
    private bool IsLuaNewest => !IsLuaClassic;
    private bool IsPreventDebug => generator_.Setting.IsPreventDebugObject;

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

      PushBlock(function.Body);
    }

    private void PopFunction() {
      PopBlock();

      var fucntion = functions_.Pop();
      --localMappingCounter_;
      if (localMappingCounter_ == 0) {
        localReservedNames_.Clear();
      }
      functionUpValues_.Remove(fucntion);
      Contract.Assert(fucntion.TempCount == 0);
    }

    public void PushBlock(LuaBlockSyntax block) {
      blocks_.Push(block);
    }

    public void PopBlock() {
      var block = blocks_.Pop();
      if (block.TempCount > 0) {
        Contract.Assert(CurFunction.TempCount >= block.TempCount);
        CurFunction.TempCount -= block.TempCount;
        releaseTempIdentifierCount_ = 0;
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

    private LuaIdentifierNameSyntax GetTempIdentifier() {
      int index = CurFunction.TempCount++;
      string name = LuaSyntaxNode.TempIdentifiers.GetOrDefault(index);
      if (name == null) {
        throw new CompilationErrorException($"Your code is startling,{LuaSyntaxNode.TempIdentifiers.Length} temporary variables is not enough");
      }
      ++CurBlock.TempCount;
      return name;
    }

    private void ReleaseTempIdentifiers(int prevTempCount) {
      int count = CurFunction.TempCount - prevTempCount;
      PopTempCount(count);
    }

    private void PopTempCount(int count) {
      Contract.Assert(CurBlock.TempCount >= count && CurFunction.TempCount >= count);
      CurBlock.TempCount -= count;
      CurFunction.TempCount -= count;
    }

    private void AddReleaseTempIdentifier(LuaIdentifierNameSyntax tempName) {
      ++releaseTempIdentifierCount_;
    }

    private void ReleaseTempIdentifiers() {
      if (releaseTempIdentifierCount_ > 0) {
        PopTempCount(releaseTempIdentifierCount_);
        releaseTempIdentifierCount_ = 0;
      }
    }

    public LuaCompilationUnitSyntax VisitCompilationUnit(CompilationUnitSyntax node, bool isSingleFile = false) {
      LuaCompilationUnitSyntax compilationUnit = new LuaCompilationUnitSyntax(node.SyntaxTree.FilePath, !isSingleFile);
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

      var attributes = BuildAttributes(node.AttributeLists);
      generator_.WithAssemblyAttributes(attributes);

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
      foreach (var nestedTypeDeclaration in node.Members.Where(i => i.Kind().IsTypeDeclaration())) {
        var luaNestedTypeDeclaration = nestedTypeDeclaration.Accept<LuaTypeDeclarationSyntax>(this);
        typeDeclaration.AddNestedTypeDeclaration(luaNestedTypeDeclaration);
      }

      foreach (var member in node.Members.Where(i => !i.Kind().IsTypeDeclaration())) {
        member.Accept(this);
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
      var interfaceType = typeSymbol.AllInterfaces.FirstOrDefault(i => i.IsGenericIEnumerableType());
      if (interfaceType != null) {
        bool isBaseImplementation = typeSymbol.BaseType != null && typeSymbol.BaseType.AllInterfaces.Any(i => i.IsGenericIEnumerableType());
        if (!isBaseImplementation) {
          var argumentType = interfaceType.TypeArguments.First();
          bool isLazy = argumentType.Kind != SymbolKind.TypeParameter && argumentType.IsFromCode();
          var typeName = isLazy ? GetTypeNameWithoutImport(argumentType) : GetTypeName(argumentType);
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
        var fields = new List<LuaIdentifierNameSyntax>();
        var semanticModel = generator_.GetSemanticModel(baseType.SyntaxTree);
        var baseTypeSymbol = semanticModel.GetTypeInfo(baseType.Type).Type;
        if (baseTypeSymbol.TypeKind == TypeKind.Class && baseTypeSymbol.SpecialType != SpecialType.System_Object) {
          if (baseTypeSymbol.IsMemberExists("Finalize", true)) {
            fields.Add(LuaIdentifierNameSyntax.__GC);
          }
          if (baseTypeSymbol.Is(generator_.SystemExceptionTypeSymbol)) {
            fields.Add(LuaIdentifierNameSyntax.__ToString);
          }
        }
        return fields;
      }
      return null;
    }

    private bool IsBaseTypeIgnore(ITypeSymbol symbol) {
      if (symbol.SpecialType == SpecialType.System_Object) {
        return true;
      }

      if (symbol.ContainingNamespace.IsRuntimeCompilerServices()) {
        return true;
      }

      return false;
    }

    private void BuildBaseTypes(INamedTypeSymbol typeSymbol, LuaTypeDeclarationSyntax typeDeclaration, IEnumerable<BaseTypeSyntax> types, bool isPartial) {
      bool hasExtendSelf = false;
      var baseTypes = new List<LuaExpressionSyntax>();
      foreach (var baseType in types) {
        if (isPartial) {
          semanticModel_ = generator_.GetSemanticModel(baseType.SyntaxTree);
        }
        var baseTypeSymbol = semanticModel_.GetTypeInfo(baseType.Type).Type;
        if (!IsBaseTypeIgnore(baseTypeSymbol)) {
          var baseTypeName = BuildInheritTypeName(baseTypeSymbol);
          baseTypes.Add(baseTypeName);
          CheckBaseTypeGenericKind(ref hasExtendSelf, typeSymbol, baseType);
        }
      }

      if (baseTypes.Count > 0) {
        if (typeSymbol.IsRecordType()) {
          baseTypes.Add(GetRecordInerfaceTypeName(typeSymbol));
        }
        var genericArgument = CheckSpeaicalGenericArgument(typeSymbol);
        var baseCopyFields = GetBaseCopyFields(types.FirstOrDefault());
        typeDeclaration.AddBaseTypes(baseTypes, genericArgument, baseCopyFields);
        if (hasExtendSelf && !generator_.IsExplicitStaticCtorExists(typeSymbol)) {
          typeDeclaration.IsForceStaticCtor = true;
        }
      }
    }

    private void BuildTypeDeclaration(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
      typeDeclarations_.Push(new TypeDeclarationInfo(typeSymbol, typeDeclaration));

      var comments = BuildDocumentationComment(node);
      typeDeclaration.AddDocument(comments);

      var attributes = BuildAttributes(node.AttributeLists);
      BuildTypeParameters(typeSymbol, node, typeDeclaration);
      if (node.BaseList != null) {
        BuildBaseTypes(typeSymbol, typeDeclaration, node.BaseList.Types, false);
      }

      CheckRecordParameterCtor(typeSymbol, node, typeDeclaration);
      BuildTypeMembers(typeDeclaration, node);
      CheckTypeDeclaration(typeSymbol, typeDeclaration, attributes, node);
  
      typeDeclarations_.Pop();
      CurCompilationUnit.AddTypeDeclarationCount();
    }

    private void CheckTypeDeclaration(INamedTypeSymbol typeSymbol, LuaTypeDeclarationSyntax typeDeclaration, List<LuaExpressionSyntax> attributes, BaseTypeDeclarationSyntax node) {
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
              var function = new LuaConstructorAdapterExpressionSyntax();
              function.AddParameter(LuaIdentifierNameSyntax.This);
              function.AddStatement(baseCtorInvoke);
              typeDeclaration.AddCtor(function, false);
            }
          }
        }
      } else if (typeSymbol.IsValueType && !typeSymbol.IsCombineImplicitlyCtor()) {
        var function = new LuaConstructorAdapterExpressionSyntax();
        function.AddParameter(LuaIdentifierNameSyntax.This);
        typeDeclaration.AddCtor(function, true);
      }

      if (typeSymbol.IsRecordType()) {
        if (typeSymbol.BaseType != null && typeSymbol.BaseType.SpecialType == SpecialType.System_Object) {
          typeDeclaration.AddBaseTypes(LuaIdentifierNameSyntax.RecordType.ArrayOf(GetRecordInerfaceTypeName(typeSymbol)), null, null);
        }
        BuildRecordMembers(typeSymbol, typeDeclaration);
      }

      if (typeDeclaration.IsIgnoreExport) {
        generator_.AddIgnoreExportType(typeSymbol);
      }

      if (IsCurTypeExportMetadataAll || attributes.Count > 0 || typeDeclaration.IsExportMetadata) {
        var data = new LuaTableExpression() { IsSingleLine = true };
        data.Add(typeSymbol.GetMetaDataAttributeFlags());
        data.AddRange(typeDeclaration.TypeParameterExpressions);
        data.AddRange(attributes);
        typeDeclaration.AddClassMetaData(data);
      }
    }

    private void CheckRecordParameterCtor(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
      if (typeSymbol.IsRecordType()) {
        var recordDeclaration = (RecordDeclarationSyntax)node;
        if (recordDeclaration.ParameterList != null) {
          BuildRecordParameterCtor(typeSymbol, typeDeclaration, recordDeclaration);
        }
      }
    }

    private void BuildRecordParameterCtor(INamedTypeSymbol typeSymbol, LuaTypeDeclarationSyntax typeDeclaration, RecordDeclarationSyntax recordDeclaration) {
      var parameterList = recordDeclaration.ParameterList.Accept<LuaParameterListSyntax>(this);
      var function = new LuaConstructorAdapterExpressionSyntax();
      function.AddParameter(LuaIdentifierNameSyntax.This);
      function.AddParameters(parameterList.Parameters);
      function.AddStatements(parameterList.Parameters.Select(i => LuaIdentifierNameSyntax.This.MemberAccess(i).Assignment(i).ToStatementSyntax()));
      typeDeclaration.AddCtor(function, false);
      var ctor = typeSymbol.InstanceConstructors.First();
      int index = 0;
      foreach (var p in ctor.Parameters) {
        var expression = GetFieldValueExpression(p.Type, null, out bool isLiteral, out _);
        if (expression != null) {
          typeDeclaration.AddField(parameterList.Parameters[index], expression, p.Type.IsImmutable() && isLiteral, false, false, true, null, false);
        }
        ++index;
      }
    }

    private void BuildRecordMembers(INamedTypeSymbol typeSymbol, LuaTypeDeclarationSyntax typeDeclaration) {
      var propertys = typeSymbol.GetMembers().OfType<IPropertySymbol>().Skip(1);
      var exprssions = new List<LuaExpressionSyntax>() { typeSymbol.Name.ToStringLiteral() };
      exprssions.AddRange(propertys.Select(i => GetMemberName(i).ToStringLiteral()));
      var function = new LuaFunctionExpressionSyntax();
      function.AddStatement(new LuaReturnStatementSyntax(exprssions));
      typeDeclaration.AddMethod(LuaIdentifierNameSyntax.RecordMembers, function, false);
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

    internal void AcceptPartialType(PartialTypeDeclaration major, IEnumerable<PartialTypeDeclaration> typeDeclarations) {
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
        BuildBaseTypes(major.Symbol, major.TypeDeclaration, baseTypes, true);
      }

      foreach (var typeDeclaration in typeDeclarations) {
        semanticModel_ = generator_.GetSemanticModel(typeDeclaration.Node.SyntaxTree);
        BuildTypeMembers(major.TypeDeclaration, typeDeclaration.Node);
      }

      CheckTypeDeclaration(major.Symbol, major.TypeDeclaration, attributes, major.Node);
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
      var interfaceDeclaration = new LuaInterfaceDeclarationSyntax(name);
      VisitTypeDeclaration(typeSymbol, node, interfaceDeclaration);
      return interfaceDeclaration;
    }

    private void VisitEnumDeclaration(INamedTypeSymbol typeSymbol, EnumDeclarationSyntax node, LuaEnumDeclarationSyntax enumDeclaration) {
      typeDeclarations_.Push(new TypeDeclarationInfo(typeSymbol, enumDeclaration));
      var document = BuildDocumentationComment(node);
      enumDeclaration.AddDocument(document);
      var attributes = BuildAttributes(node.AttributeLists);
      foreach (var member in node.Members) {
        var statement = member.Accept<LuaKeyValueTableItemSyntax>(this);
        enumDeclaration.Add(statement);
      }
      CheckTypeDeclaration(typeSymbol, enumDeclaration, attributes, node);
      typeDeclarations_.Pop();
      generator_.AddEnumDeclaration(typeSymbol, enumDeclaration);
    }

    public override LuaSyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node) {
      GetTypeDeclarationName(node, out var name, out var typeSymbol);
      LuaEnumDeclarationSyntax enumDeclaration = new LuaEnumDeclarationSyntax(typeSymbol.ToString(), name, CurCompilationUnit);
      VisitEnumDeclaration(typeSymbol, node, enumDeclaration);
      return enumDeclaration;
    }

    public override LuaSyntaxNode VisitRecordDeclaration(RecordDeclarationSyntax node) {
      GetTypeDeclarationName(node, out var name, out var typeSymbol);
      LuaClassDeclarationSyntax classDeclaration = new LuaClassDeclarationSyntax(name);
      VisitTypeDeclaration(typeSymbol, node, classDeclaration);
      return classDeclaration;
    }

    public override LuaSyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node) {
      return LuaStatementSyntax.Empty;
    }

    private void VisitYield(IMethodSymbol symbol, LuaFunctionExpressionSyntax function) {
      var retrurnTypeSymbol = (INamedTypeSymbol)symbol.ReturnType;
      string name = LuaSyntaxNode.Tokens.Yield + retrurnTypeSymbol.Name;
      var invokeExpression = LuaIdentifierNameSyntax.System.MemberAccess(name).Invocation();
      var wrapFunction = new LuaFunctionExpressionSyntax();
      if (symbol.IsAsync) {
        wrapFunction.AddParameter(LuaIdentifierNameSyntax.Async);
      }

      var parameters = function.ParameterList.Parameters;
      wrapFunction.ParameterList.Parameters.AddRange(parameters);
      wrapFunction.AddStatements(function.Body.Statements);
      invokeExpression.AddArgument(wrapFunction);
      if (retrurnTypeSymbol.IsGenericType) {
        var typeName = retrurnTypeSymbol.TypeArguments.First();
        var expression = GetTypeName(typeName);
        invokeExpression.AddArgument(expression);
      } else {
        invokeExpression.AddArgument(LuaIdentifierNameSyntax.Object);
      }

      invokeExpression.ArgumentList.Arguments.AddRange(parameters);
      var returnStatement = new LuaReturnStatementSyntax(invokeExpression);
      function.Body.Statements.Clear();
      function.AddStatement(returnStatement);
    }

    private void VisitAsync(bool returnsVoid, LuaFunctionExpressionSyntax function) {
      var invokeExpression = LuaIdentifierNameSyntax.System.MemberAccess(LuaIdentifierNameSyntax.Async).Invocation();
      var wrapFunction = new LuaFunctionExpressionSyntax();

      var parameters = function.ParameterList.Parameters;
      wrapFunction.AddParameter(LuaIdentifierNameSyntax.Async);
      wrapFunction.ParameterList.Parameters.AddRange(parameters);
      wrapFunction.AddStatements(function.Body.Statements);
      invokeExpression.AddArgument(wrapFunction);
      invokeExpression.AddArgument(returnsVoid ? LuaIdentifierNameSyntax.True : LuaIdentifierNameSyntax.Nil);
      invokeExpression.ArgumentList.Arguments.AddRange(parameters);

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

    private void AddMethodMetaData(MethodDeclarationResult result, bool isMoreThanLocalVariables = false) {
      var table = new LuaTableExpression() { IsSingleLine = true };
      table.Add(new LuaStringLiteralExpressionSyntax(result.Symbol.Name));
      table.Add(result.Symbol.GetMetaDataAttributeFlags());
      if (isMoreThanLocalVariables) {
        table.Add(LuaIdentifierNameSyntax.MorenManyLocalVarTempTable.MemberAccess(result.Name));
      } else {
        table.Add(result.Name);
      }
      var parameters = result.Symbol.Parameters.Select(i => GetTypeNameOfMetadata(i.Type)).ToList();
      if (!result.Symbol.ReturnsVoid) {
        parameters.Add(GetTypeNameOfMetadata(result.Symbol.ReturnType));
      }
      if (result.Symbol.IsGenericMethod) {
        var function = new LuaFunctionExpressionSyntax();
        function.AddParameters(result.Symbol.TypeParameters.Select(i => (LuaIdentifierNameSyntax)i.Name));
        function.AddStatement(new LuaReturnStatementSyntax(parameters));
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
      ArrowExpressionClauseSyntax expressionBody) {
      IMethodSymbol symbol = (IMethodSymbol)semanticModel_.GetDeclaredSymbol(node);
      var refOrOutParameters = new List<LuaExpressionSyntax>();
      MethodInfo methodInfo = new MethodInfo(symbol, refOrOutParameters);
      methodInfos_.Push(methodInfo);

      LuaIdentifierNameSyntax methodName;
      if (symbol.MethodKind == MethodKind.LocalFunction) {
        methodName = GetLocalMethodName(symbol, node);
      } else {
        methodName = GetMemberName(symbol);
      }
      LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
      PushFunction(function);

      var document = BuildDocumentationComment(node);
      bool isPrivate = symbol.IsPrivate() && symbol.ExplicitInterfaceImplementations.IsEmpty;
      if (!symbol.IsStatic && symbol.MethodKind != MethodKind.LocalFunction) {
        function.AddParameter(LuaIdentifierNameSyntax.This);
        if (isPrivate) {
          if (generator_.IsForcePublicSymbol(symbol) || generator_.IsMonoBehaviourSpecialMethod(symbol)) {
            isPrivate = false;
          }
        }
      } else if (symbol.IsMainEntryPoint()) {
        isPrivate = false;
        generator_.SetMainEntryPoint(symbol, node);
      } else if (isPrivate && generator_.IsForcePublicSymbol(symbol)) {
        isPrivate = false;
      }

      var attributes = BuildAttributes(attributeLists);
      foreach (var parameterNode in parameterList.Parameters) {
        var parameter = parameterNode.Accept<LuaIdentifierNameSyntax>(this);
        function.AddParameter(parameter);
        if (parameterNode.Modifiers.IsOutOrRef()) {
          refOrOutParameters.Add(parameter);
        } else if (parameterNode.Modifiers.IsParams() && symbol.HasParamsAttribute()) {
          function.ParameterList.Parameters[^1] = LuaSyntaxNode.Tokens.Params;
        }
      }

      if (typeParameterList != null) {
        var typeParameters = typeParameterList.Accept<LuaParameterListSyntax>(this);
        function.AddParameters(typeParameters.Parameters);
      }

      if (body != null) {
        var block = body.Accept<LuaBlockSyntax>(this);
        function.AddStatements(block.Statements);
      } else {
        var expression = expressionBody.AcceptExpression(this);
        if (symbol.ReturnsVoid) {
          function.AddStatement(expression);
        } else {
          var returnStatement = new LuaReturnStatementSyntax(expression);
          returnStatement.Expressions.AddRange(refOrOutParameters);
          function.AddStatement(returnStatement);
        }
      }

      if (methodInfo.HasYield) {
        VisitYield(symbol, function);
      } else if (symbol.IsAsync) {
        VisitAsync(symbol.ReturnsVoid, function);
      } else {
        if (symbol.ReturnsVoid && refOrOutParameters.Count > 0) {
          function.AddStatement(new LuaReturnStatementSyntax(refOrOutParameters));
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

    private static bool IsExportMethodDeclaration(BaseMethodDeclarationSyntax node) {
      return (node.Body != null || node.ExpressionBody != null) && !node.HasCSharpLuaAttribute(LuaDocumentStatement.AttributeFlags.Ignore);
    }

    public override LuaSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
      if (IsExportMethodDeclaration(node)) {
        var result = BuildMethodDeclaration(node, node.AttributeLists, node.ParameterList, node.TypeParameterList, node.Body, node.ExpressionBody);
        bool isMoreThanLocalVariables = IsMoreThanLocalVariables(result.Symbol);
        CurType.AddMethod(result.Name, result.Function, result.IsPrivate, result.Document, isMoreThanLocalVariables, result.Symbol.IsInterfaceDefaultMethod());
        if (IsCurTypeExportMetadataAll || result.Attributes.Count > 0 || result.IsMetadata) {
          AddMethodMetaData(result, isMoreThanLocalVariables);
        }
        return result.Function;
      }
      return base.VisitMethodDeclaration(node);
    }

    private LuaExpressionSyntax BuildEnumNoConstantDefaultValue(ITypeSymbol typeSymbol) {
      var typeName = GetTypeName(typeSymbol);
      var field = typeSymbol.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(i => i.ConstantValue.Equals(0));
      if (field != null) {
        return typeName.MemberAccess(field.Name);
      }
      return typeName.Invocation(LuaNumberLiteralExpressionSyntax.Zero);
    }

    private LuaExpressionSyntax GetPredefinedValueTypeDefaultValue(ITypeSymbol typeSymbol) {
      switch (typeSymbol.SpecialType) {
        case SpecialType.None: {
          if (typeSymbol.TypeKind == TypeKind.Enum) {
            if (!generator_.IsConstantEnum(typeSymbol)) {
              return BuildEnumNoConstantDefaultValue(typeSymbol);
            }
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
          return new LuaCharacterLiteralExpression(default);
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
          if (variableSymbol.IsAbstract) {
            continue;
          }

          if (node.IsKind(SyntaxKind.EventFieldDeclaration)) {
            var eventSymbol = (IEventSymbol)variableSymbol;
            if (!IsEventFiled(eventSymbol)) {
              var eventName = GetMemberName(eventSymbol);
              var innerName = AddInnerName(eventSymbol);
              LuaExpressionSyntax valueExpression = GetFieldValueExpression(typeSymbol, variable.Initializer?.Value, out bool valueIsLiteral, out var statements);
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
                AddField(name, typeSymbol, variable.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, attributes);
                continue;
              }
            }
          }
          if (isPrivate && generator_.IsForcePublicSymbol(variableSymbol)) {
            isPrivate = false;
          }
          var fieldName = GetMemberName(variableSymbol);
          AddField(fieldName, typeSymbol, variable.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, attributes);
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
            if (v != null) {
              if (v.Length > kStringConstInlineCount) {
                var variableSymbol = semanticModel_.GetDeclaredSymbol(variable);
                if (isPrivate && generator_.IsForcePublicSymbol(variableSymbol)) {
                  isPrivate = false;
                }
                var attributes = BuildAttributes(node.AttributeLists);
                var fieldName = GetMemberName(variableSymbol);
                bool isMoreThanLocalVariables = IsMoreThanLocalVariables(variableSymbol);
                AddField(fieldName, typeSymbol, variable.Initializer.Value, true, true, isPrivate, true, attributes, isMoreThanLocalVariables);
              }
            }
          }
        }
      }
    }

    public override LuaSyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) {
      VisitBaseFieldDeclarationSyntax(node);
      return base.VisitFieldDeclaration(node);
    }

    private LuaExpressionSyntax GetFieldValueExpression(ITypeSymbol typeSymbol, ExpressionSyntax expression, out bool valueIsLiteral, out List<LuaStatementSyntax> statements) {
      LuaExpressionSyntax valueExpression = null;
      valueIsLiteral = false;
      statements = null;

      if (expression != null && !expression.IsKind(SyntaxKind.NullLiteralExpression)) {
        var function = new LuaFunctionExpressionSyntax();
        PushFunction(function);
        valueExpression = VisitExpression(expression);
        PopFunction();
        if (function.Body.Statements.Count > 0) {
          statements = function.Body.Statements;
        }
        LuaExpressionSyntax v = valueExpression;
        if (valueExpression is LuaPrefixUnaryExpressionSyntax prefixUnaryExpression) {
          v = prefixUnaryExpression.Operand;
        }
        valueIsLiteral = v is LuaLiteralExpressionSyntax;
      }

      if (valueExpression == null) {
        if (typeSymbol.IsMaybeValueType() && !typeSymbol.IsNullableType()) {
          var defalutValue = GetPredefinedValueTypeDefaultValue(typeSymbol);
          if (defalutValue != null) {
            valueExpression = defalutValue;
            valueIsLiteral = defalutValue is LuaLiteralExpressionSyntax;
          } else {
            valueExpression = GetDefaultValueExpression(typeSymbol);
          }
        }
      }
      return valueExpression;
    }

    private void AddField(LuaIdentifierNameSyntax name, ITypeSymbol typeSymbol, ExpressionSyntax expression, bool isImmutable, bool isStatic, bool isPrivate, bool isReadOnly, List<LuaExpressionSyntax> attributes, bool isMoreThanLocalVariables = false) {
      var valueExpression = GetFieldValueExpression(typeSymbol, expression, out bool valueIsLiteral, out var statements);
      CurType.AddField(name, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, isReadOnly, statements, isMoreThanLocalVariables);
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
      if (get != null) {
        if (set != null) {
          kind = PropertyMethodKind.Both;
        } else {
          kind = PropertyMethodKind.GetOnly;
        }
      } else {
        if (set != null) {
          kind = PropertyMethodKind.SetOnly;
        } else {
          kind = PropertyMethodKind.Field;
        }
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
      var symbol = semanticModel_.GetDeclaredSymbol(node);
      if (!symbol.IsAbstract) {
        if (symbol.IsProtobufNetSpecialProperty()) {
          return null;
        }
        bool isStatic = symbol.IsStatic;
        bool isPrivate = symbol.IsPrivate() && symbol.ExplicitInterfaceImplementations.IsEmpty;
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
              var accessorSymbol = semanticModel_.GetDeclaredSymbol(accessor);
              var methodInfo = new MethodInfo(accessorSymbol);
              methodInfos_.Push(methodInfo);
              bool isGet = accessor.IsKind(SyntaxKind.GetAccessorDeclaration);
              var functionExpression = new LuaFunctionExpressionSyntax();
              if (!isStatic) {
                functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
              }
              PushFunction(functionExpression);
              if (accessor.Body != null) {
                var block = accessor.Body.Accept<LuaBlockSyntax>(this);
                functionExpression.AddStatements(block.Statements);
              } else {
                var bodyExpression = accessor.ExpressionBody.AcceptExpression(this);
                if (isGet) {
                  functionExpression.AddStatement(new LuaReturnStatementSyntax(bodyExpression));
                } else {
                  functionExpression.AddStatement(bodyExpression);
                }
              }
              if (methodInfo.HasYield) {
                VisitYield(accessorSymbol, functionExpression);
              }
              PopFunction();
              methodInfos_.Pop();

              var name = new LuaPropertyOrEventIdentifierNameSyntax(true, propertyName);
              CurType.AddMethod(name, functionExpression, isPrivate, null, IsMoreThanLocalVariables(accessorSymbol));

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
          methodInfos_.Push(new MethodInfo(symbol.GetMethod));
          var name = new LuaPropertyOrEventIdentifierNameSyntax(true, propertyName);
          var functionExpression = new LuaFunctionExpressionSyntax();
          PushFunction(functionExpression);
          var expression = node.ExpressionBody.AcceptExpression(this);
          PopFunction();
          methodInfos_.Pop();

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
            AddField(propertyName, typeSymbol, node.Initializer?.Value, isImmutable, isStatic, isPrivate, isReadOnly, attributes);
          } else {
            var innerName = AddInnerName(symbol);
            var valueExpression = GetFieldValueExpression(typeSymbol, node.Initializer?.Value, out bool valueIsLiteral, out var statements);
            LuaExpressionSyntax typeExpression = null;
            if (isStatic) {
              typeExpression = GetTypeName(symbol.ContainingType);
            }
            bool isGetOnly = symbol.SetMethod == null;
            var (getName, setName) = CurType.AddProperty(propertyName, innerName, valueExpression, isImmutable && valueIsLiteral, isStatic, isPrivate, typeExpression, statements, isGetOnly);
            getMethod = new PropertyMethodResult(getName);
            setMethod = isGetOnly ? null : new PropertyMethodResult(setName);
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
      var symbol = semanticModel_.GetDeclaredSymbol(node);
      if (!symbol.IsAbstract) {
        var attributes = BuildAttributes(node.AttributeLists);
        bool isStatic = symbol.IsStatic;
        bool isPrivate = symbol.IsPrivate() && symbol.ExplicitInterfaceImplementations.IsEmpty;
        var eventName = GetMemberName(symbol);
        PropertyMethodResult addMethod = null;
        PropertyMethodResult removeMethod = null;
        foreach (var accessor in node.AccessorList.Accessors) {
          var methodAttributes = BuildAttributes(accessor.AttributeLists);
          var functionExpression = new LuaFunctionExpressionSyntax();
          if (!isStatic) {
            functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
          }
          functionExpression.AddParameter(LuaIdentifierNameSyntax.Value);
          PushFunction(functionExpression);
          if (accessor.Body != null) {
            var block = accessor.Body.Accept<LuaBlockSyntax>(this);
            functionExpression.AddStatements(block.Statements);
          } else {
            var bodyExpression = accessor.ExpressionBody.AcceptExpression(this);
            functionExpression.AddStatement(bodyExpression);
          }
          PopFunction();
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
      var attributes = BuildAttributes(node.AttributeLists);
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
      AddFieldMetaData(symbol, identifier, attributes);
      var value = new LuaIdentifierLiteralExpressionSyntax(symbol.ConstantValue.ToString());
      return new LuaKeyValueTableItemSyntax(identifier, value);
    }

    public override LuaSyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node) {
      var symbol = semanticModel_.GetDeclaredSymbol(node);
      if (!symbol.IsAbstract) {
        bool isPrivate = symbol.IsPrivate();
        var indexName = GetMemberName(symbol);
        var parameterList = node.ParameterList.Accept<LuaParameterListSyntax>(this);

        void Fill(Action<LuaFunctionExpressionSyntax, LuaPropertyOrEventIdentifierNameSyntax> action) {
          var function = new LuaFunctionExpressionSyntax();
          function.AddParameter(LuaIdentifierNameSyntax.This);
          function.ParameterList.Parameters.AddRange(parameterList.Parameters);
          var name = new LuaPropertyOrEventIdentifierNameSyntax(true, indexName);
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
                var block = accessor.Body.Accept<LuaBlockSyntax>(this);
                function.AddStatements(block.Statements);
              } else {
                var bodyExpression = accessor.ExpressionBody.AcceptExpression(this);
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
            var bodyExpression = node.ExpressionBody.AcceptExpression(this);
            function.AddStatement(new LuaReturnStatementSyntax(bodyExpression));
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
      var parameterList = new LuaParameterListSyntax();
      foreach (var parameter in parameters) {
        var newNode = parameter.Accept<LuaIdentifierNameSyntax>(this);
        parameterList.Parameters.Add(newNode);
      }
      return parameterList;
    }

    public override LuaSyntaxNode VisitParameter(ParameterSyntax node) {
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
      CheckLocalVariableName(ref identifier, node);
      return identifier;
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
              string commentContent = content[kCommentCharCount..^kCommentCharCount];
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
              string codeString = commentContent[begin..end];
              string[] lines = codeString.Split('\n');
              var codeLines = new LuaStatementListSyntax();
              int indent = -1;
              foreach (string line in lines) {
                if (!string.IsNullOrWhiteSpace(line)) {
                  if (indent == -1) {
                    indent = line.IndexOf(i => !char.IsWhiteSpace(i));
                  }
                  int space = line.IndexOf(i => !char.IsWhiteSpace(i));
                  string code = space >= indent && indent != -1 ? line.Substring(indent) : line;
                  codeLines.Statements.Add((LuaIdentifierNameSyntax)code);
                }
              }
              statement = codeLines;
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
      PushBlock(block);

      var statements = VisitTriviaAndNode(node, node.Statements);
      block.Statements.AddRange(statements);

      var indexs = block.UsingDeclarations;
      if (indexs != null) {
        block.UsingDeclarations = null;
        ApplyUsingDeclarations(block, indexs, node);
      }

      PopBlock();
      return block;
    }

    private LuaReturnStatementSyntax BuildLoopControlReturnStatement(LuaExpressionSyntax expression) {
      var returnStatement = new LuaReturnStatementSyntax(LuaIdentifierLiteralExpressionSyntax.True);
      if (expression != null) {
        returnStatement.Expressions.Add(expression);
      }
      return returnStatement;
    }

    private LuaStatementSyntax InternalVisitReturnStatement(LuaExpressionSyntax expression) {
      if (CurFunction is LuaCheckLoopControlExpressionSyntax check) {
        check.HasReturn = true;
        return BuildLoopControlReturnStatement(expression);
      }

      if (CurBlock.HasUsingDeclarations) {
        return BuildLoopControlReturnStatement(expression);
      }

      LuaStatementSyntax result;
      var curMethodInfo = CurMethodInfoOrNull;
      if (curMethodInfo != null && curMethodInfo.RefOrOutParameters.Count > 0) {
        if (curMethodInfo.IsInlining) {
          var multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
          multipleAssignment.Lefts.AddRange(curMethodInfo.InliningReturnVars);
          if (expression != null) {
            multipleAssignment.Rights.Add(expression);
          }
          multipleAssignment.Rights.AddRange(curMethodInfo.RefOrOutParameters);
          result = multipleAssignment;
        } else {
          var returnStatement = new LuaReturnStatementSyntax();
          if (expression != null) {
            returnStatement.Expressions.Add(expression);
          }
          returnStatement.Expressions.AddRange(curMethodInfo.RefOrOutParameters);
          result = returnStatement;
        }
      } else {
        if (curMethodInfo != null && curMethodInfo.IsInlining) {
          Contract.Assert(curMethodInfo.InliningReturnVars.Count == 1);
          result = curMethodInfo.InliningReturnVars.First().Assignment(expression);
        } else {
          result = new LuaReturnStatementSyntax(expression);
        }
      }
      return result;
    }

    public override LuaSyntaxNode VisitReturnStatement(ReturnStatementSyntax node) {
      var expression = node.Expression != null ? VisitExpression(node.Expression) : null;
      var result = InternalVisitReturnStatement(expression);
      if (node.Parent.IsKind(SyntaxKind.Block) && node.Parent.Parent is MemberDeclarationSyntax) {
        var block = (BlockSyntax)node.Parent;
        if (block.Statements.Last() != node) {
          var blockStatement = new LuaBlockStatementSyntax();
          blockStatement.Statements.Add(result);
          result = blockStatement;
        } else if (expression == null) {
          result = LuaStatementSyntax.Empty;
        }
      }
      return result;
    }

    public override LuaSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
      var expression = node.Expression.AcceptExpression(this);
      ReleaseTempIdentifiers();
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
      bool isPreventDebugConcatenation = IsPreventDebug && operatorToken == LuaSyntaxNode.Tokens.Concatenation;
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        LuaExpressionSyntax expression = null;
        var geter = propertyAdapter.GetCloneOfGet();
        LuaExpressionSyntax leftExpression = geter;
        if (isPreventDebugConcatenation) {
          leftExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemToString, leftExpression);
        }
        if (parnet != null) {
          expression = GetUserDefinedOperatorExpression(parnet, leftExpression, right);
        }
        if (expression == null) {
          expression = leftExpression.Binary(operatorToken, right);
        }
        propertyAdapter.ArgumentList.AddArgument(expression);
        return propertyAdapter;
      } else {
        LuaExpressionSyntax expression = null;
        LuaExpressionSyntax leftExpression = left;
        if (isPreventDebugConcatenation) {
          leftExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemToString, leftExpression);
        }
        if (parnet != null) {
          expression = GetUserDefinedOperatorExpression(parnet, leftExpression, right);
        }
        if (expression == null) {
          bool isRightParenthesized = rightNode is BinaryExpressionSyntax || rightNode.IsKind(SyntaxKind.ConditionalExpression);
          if (isRightParenthesized) {
            right = right.Parenthesized();
          }
          expression = leftExpression.Binary(operatorToken, right);
        }
        return left.Assignment(expression);
      }
    }

    private LuaExpressionSyntax BuildCommonAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, string operatorToken, ExpressionSyntax parnet) {
      var left = VisitExpression(leftNode);
      var right = VisitExpression(rightNode);
      return BuildCommonAssignmentExpression(left, right, operatorToken, rightNode, parnet);
    }

    private LuaExpressionSyntax BuildDelegateBinaryExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, bool isPlus) {
      if (IsPreventDebug) {
        var methodName = isPlus ? LuaIdentifierNameSyntax.DelegateCombine : LuaIdentifierNameSyntax.DelegateRemove;
        return new LuaInvocationExpressionSyntax(methodName, left, right);
      } else {
        var operatorToken = isPlus ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub;
        return left.Binary(operatorToken, right);
      }
    }

    private LuaExpressionSyntax BuildDelegateAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, bool isPlus) {
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        if (propertyAdapter.IsProperty) {
          propertyAdapter.ArgumentList.AddArgument(BuildDelegateBinaryExpression(propertyAdapter.GetCloneOfGet(), right, isPlus));
          return propertyAdapter;
        } else {
          propertyAdapter.IsGetOrAdd = isPlus;
          propertyAdapter.ArgumentList.AddArgument(right);
          return propertyAdapter;
        }
      } else {
        return left.Assignment(BuildDelegateBinaryExpression(left, right, isPlus));
      }
    }

    private LuaExpressionSyntax BuildBinaryInvokeAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, LuaExpressionSyntax methodName) {
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        var invocation = new LuaInvocationExpressionSyntax(methodName, propertyAdapter.GetCloneOfGet(), right);
        propertyAdapter.ArgumentList.AddArgument(invocation);
        return propertyAdapter;
      } else {
        var invocation = new LuaInvocationExpressionSyntax(methodName, left, right);
        return left.Assignment(invocation);
      }
    }

    private LuaExpressionSyntax BuildBinaryInvokeAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, LuaExpressionSyntax methodName) {
      var left = leftNode.AcceptExpression(this);
      var right = rightNode.AcceptExpression(this);
      return BuildBinaryInvokeAssignmentExpression(left, right, methodName);
    }

    private LuaExpressionSyntax BuildLuaSimpleAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right) {
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        propertyAdapter.IsGetOrAdd = false;
        propertyAdapter.ArgumentList.AddArgument(right);
        return propertyAdapter;
      } else {
        return left.Assignment(right);
      }
    }

    private List<LuaExpressionSyntax> BuildNumberNullableIdentifiers(ref LuaExpressionSyntax left, ref LuaExpressionSyntax right, bool isLeftNullbale, bool isRightNullable) {
      var identifiers = new List<LuaExpressionSyntax>();
      if (isLeftNullbale) {
        if (isRightNullable) {
          left = BuildNullableExpressionIdentifier(left, identifiers);
          right = BuildNullableExpressionIdentifier(right, identifiers);
        } else {
          left = BuildNullableExpressionIdentifier(left, identifiers);
        }
      } else {
        right = BuildNullableExpressionIdentifier(right, identifiers);
      }
      return identifiers;
    }

    private static void TransformIdentifiersForCompareExpression(List<LuaExpressionSyntax> identifiers) {
      for (int i = 0; i < identifiers.Count; ++i) {
        var identifier = identifiers[i];
        identifiers[i] = identifier.NotEquals(LuaIdentifierNameSyntax.Nil);
      }
    }

    private static void CheckNumberNullableCompareExpression(string operatorToken, List<LuaExpressionSyntax> identifiers) {
      switch (operatorToken) {
        case ">":
        case ">=":
        case "<":
        case "<=": {
          TransformIdentifiersForCompareExpression(identifiers);
          break;
        }
      }
    }

    private LuaExpressionSyntax BuildNumberNullableExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, string operatorToken, bool isLeftNullbale, bool isRightNullable, LuaIdentifierNameSyntax method) {
      if (left.IsNil() || right.IsNil()) {
        return LuaIdentifierLiteralExpressionSyntax.Nil;
      }

      var identifiers = BuildNumberNullableIdentifiers(ref left, ref right, isLeftNullbale, isRightNullable);
      LuaExpressionSyntax expression;
      if (method != null) {
        expression = new LuaInvocationExpressionSyntax(method, left, right);
      } else {
        expression = left.Binary(operatorToken, right);
        CheckNumberNullableCompareExpression(operatorToken, identifiers);
      }
      return identifiers.Aggregate((x, y) => x.And(y)).And(expression);
    }

    private LuaExpressionSyntax BuildNumberNullableAssignment(LuaExpressionSyntax left, LuaExpressionSyntax right, string operatorToken, bool isLeftNullbale, bool isRightNullable, LuaIdentifierNameSyntax method) {
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        Contract.Assert(propertyAdapter.IsProperty);
        propertyAdapter.ArgumentList.AddArgument(BuildNumberNullableExpression(propertyAdapter.GetCloneOfGet(), right, operatorToken, isLeftNullbale, isRightNullable, method));
        return propertyAdapter;
      } else {
        return left.Assignment(BuildNumberNullableExpression(left, right, operatorToken, isLeftNullbale, isRightNullable, method));
      }
    }

    private bool IsNullableType(ExpressionSyntax leftNode, ExpressionSyntax rightNode, out bool isLeftNullable, out bool isRightNullable) {
      var leftType = semanticModel_.GetTypeInfo(leftNode).Type;
      var rightType = semanticModel_.GetTypeInfo(rightNode).Type;
      isLeftNullable = leftType != null && leftType.IsNullableType();
      isRightNullable = rightType != null && rightType.IsNullableType();
      return isLeftNullable || isRightNullable;
    }

    private bool IsNullableType(ExpressionSyntax leftNode, ExpressionSyntax rightNode) => IsNullableType(leftNode, rightNode, out _, out _);

    private bool IsNullableAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right, string operatorToken, out LuaExpressionSyntax result, LuaIdentifierNameSyntax method = null) {
      if (IsNullableType(left, right, out bool isLeftNullbale, out bool isRightNullable)) {
        var leftExpression = left.AcceptExpression(this);
        var rightExpression = right.AcceptExpression(this);
        result = BuildNumberNullableAssignment(leftExpression, rightExpression, operatorToken, isLeftNullbale, isRightNullable, method);
        return true;
      }
      result = null;
      return false;
    }

    private bool IsNumberNullableAssignmentExpression(INamedTypeSymbol containingType, ExpressionSyntax left, ExpressionSyntax right, string operatorToken, out LuaExpressionSyntax result, LuaIdentifierNameSyntax method = null) {
      if (containingType.IsNumberType(false)) {
        if (IsNullableAssignmentExpression(left, right, operatorToken, out result, method)) {
          return true;
        }
      }
      result = null;
      return false;
    }

    private LuaExpressionSyntax BuildNumberAssignmentExpression(ExpressionSyntax node, ExpressionSyntax leftNode, ExpressionSyntax rightNode, SyntaxKind kind) {
      if (semanticModel_.GetSymbolInfo(node).Symbol is IMethodSymbol symbol) {
        var containingType = symbol.ContainingType;
        if (containingType != null) {
          switch (kind) {
            case SyntaxKind.AddAssignmentExpression:
            case SyntaxKind.SubtractAssignmentExpression: {
              if (containingType.IsStringType()) {
                var left = leftNode.AcceptExpression(this);
                var right = WrapStringConcatExpression(rightNode);
                return BuildCommonAssignmentExpression(left, right, LuaSyntaxNode.Tokens.Concatenation, rightNode, null);
              }

              bool isPlus = kind == SyntaxKind.AddAssignmentExpression;
              if (containingType.IsDelegateType() || (symbol.MethodKind == MethodKind.EventAdd || symbol.MethodKind == MethodKind.EventRemove)) {
                var left = leftNode.AcceptExpression(this);
                var right = rightNode.AcceptExpression(this);
                return BuildDelegateAssignmentExpression(left, right, isPlus);
              }

              if (IsPreventDebug) {
                if (IsNumberNullableAssignmentExpression(containingType, leftNode, rightNode, isPlus ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub, out var result)) {
                  return result;
                }
              }
              break;
            }
            case SyntaxKind.MultiplyAssignmentExpression: {
              if (IsNumberNullableAssignmentExpression(containingType, leftNode, rightNode, LuaSyntaxNode.Tokens.Multiply, out var result)) {
                return result;
              }
              break;
            }
            case SyntaxKind.DivideAssignmentExpression: {
              if (IsPreventDebug && containingType.IsDoubleOrFloatType(false)) {
                if (IsNullableAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Div, out var result)) {
                  return result;
                }
              }

              if (containingType.IsIntegerType(false)) {
                if (IsNullableType(leftNode, rightNode)) {
                  if (IsLuaClassic || IsPreventDebug) {
                    bool success = IsNullableAssignmentExpression(leftNode, rightNode, null, out var result, LuaIdentifierNameSyntax.IntegerDiv);
                    Contract.Assert(success);
                    return result;
                  }
                }
                return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, LuaIdentifierNameSyntax.IntegerDiv);
              }
              break;
            }
            case SyntaxKind.ModuloAssignmentExpression: {
              if (containingType.IsNumberType(false)) {
                if (IsLuaClassic) {
                  var method = containingType.IsIntegerType(false) ? LuaIdentifierNameSyntax.Mod : LuaIdentifierNameSyntax.ModFloat;
                  if (IsNullableAssignmentExpression(leftNode, rightNode, null, out var result, method)) {
                    return result;
                  }

                  return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, method);
                } else {
                  if (IsPreventDebug && IsNullableAssignmentExpression(leftNode, rightNode, null, out var result, LuaIdentifierNameSyntax.Mod)) {
                    return result;
                  }

                  return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, LuaIdentifierNameSyntax.Mod);
                }
              }
              break;
            }
            case SyntaxKind.LeftShiftAssignmentExpression:
            case SyntaxKind.RightShiftAssignmentExpression: {
              if (containingType.IsIntegerType(false)) {
                bool isLeftShift = kind == SyntaxKind.LeftShiftAssignmentExpression;
                if (IsLuaClassic) {
                  var method = isLeftShift ? LuaIdentifierNameSyntax.ShiftLeft : LuaIdentifierNameSyntax.ShiftRight;
                  if (IsNullableAssignmentExpression(leftNode, rightNode, null, out var result, method)) {
                    return result;
                  }

                  return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, method);
                } else if (IsPreventDebug && IsNullableAssignmentExpression(leftNode, rightNode, isLeftShift ? LuaSyntaxNode.Tokens.ShiftLeft : LuaSyntaxNode.Tokens.ShiftRight, out var result)) {
                  return result;
                }
              }
              break;
            }
          }
        }
      }
      return null;
    }

    private LuaExpressionSyntax BuildLuaAssignmentExpression(AssignmentExpressionSyntax node, ExpressionSyntax leftNode, ExpressionSyntax rightNode, SyntaxKind kind) {
      LuaExpressionSyntax resultExpression = null;
      switch (kind) {
        case SyntaxKind.SimpleAssignmentExpression: {
          var left = leftNode.AcceptExpression(this);
          var right = VisitExpression(rightNode);
          if (leftNode.Kind().IsTupleDeclaration()) {
            if (!rightNode.IsKind(SyntaxKind.TupleExpression)) {
              right = BuildDeconstructExpression(rightNode, right);
            }
          } else if (leftNode.IsKind(SyntaxKind.IdentifierName) && left == LuaIdentifierNameSyntax.Placeholder) {
            var local = new LuaLocalVariableDeclaratorSyntax(LuaIdentifierNameSyntax.Placeholder, right);
            CurBlock.AddStatement(local);
            return LuaExpressionSyntax.EmptyExpression;
          } else if (leftNode.IsKind(SyntaxKind.ThisExpression)) {
            return left.MemberAccess(LuaIdentifierNameSyntax.CopyThis, true).Invocation(right);
          }
          return BuildLuaSimpleAssignmentExpression(left, right);
        }
        case SyntaxKind.AddAssignmentExpression:
        case SyntaxKind.SubtractAssignmentExpression:
        case SyntaxKind.MultiplyAssignmentExpression:
        case SyntaxKind.DivideAssignmentExpression:
        case SyntaxKind.ModuloAssignmentExpression:
        case SyntaxKind.LeftShiftAssignmentExpression:
        case SyntaxKind.RightShiftAssignmentExpression:
          resultExpression = BuildNumberAssignmentExpression(node, leftNode, rightNode, kind);
          break;

        case SyntaxKind.AndAssignmentExpression: {
          resultExpression = BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.And, LuaIdentifierNameSyntax.BitAnd, node);
          break;
        }
        case SyntaxKind.ExclusiveOrAssignmentExpression: {
          resultExpression = BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.NotEquals, LuaIdentifierNameSyntax.BitXor, node);
          break;
        }
        case SyntaxKind.OrAssignmentExpression: {
          resultExpression = BuildBitAssignmentExpression(leftNode, rightNode, LuaSyntaxNode.Tokens.Or, LuaIdentifierNameSyntax.BitOr, node);
          break;
        }
        case SyntaxKind.CoalesceAssignmentExpression: {
          var left = leftNode.AcceptExpression(this);
          var right = VisitExpression(rightNode);
          var ifStatement = new LuaIfStatementSyntax(left.EqualsEquals(LuaIdentifierNameSyntax.Nil));
          ifStatement.Body.AddStatement(left.Assignment(right));
          CurBlock.AddStatement(ifStatement);
          return LuaExpressionSyntax.EmptyExpression;
        }
      }

      if (resultExpression != null) {
        return resultExpression;
      }

      string operatorToken = GetOperatorToken(node.OperatorToken.ValueText[0 ..^1]);
      return BuildCommonAssignmentExpression(leftNode, rightNode, operatorToken, node);
    }

    private LuaIdentifierNameSyntax BuildBoolXorOfNullAssignmentExpression(LuaExpressionSyntax left, LuaExpressionSyntax right, bool isLeftNullable, bool isRightNullable) {
      var temp = GetTempIdentifier();
      CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp));
      var identifiers = BuildNumberNullableIdentifiers(ref left, ref right, isLeftNullable, isRightNullable);
      LuaExpressionSyntax condition;
      if (identifiers.Count == 1) {
        condition = identifiers.First().NotEquals(LuaIdentifierNameSyntax.Nil);
      } else {
        condition = left.NotEquals(LuaIdentifierNameSyntax.Nil).And(right.NotEquals(LuaIdentifierNameSyntax.Nil));
      }
      var ifStatement = new LuaIfStatementSyntax(condition);
      ifStatement.Body.AddStatement(temp.Assignment(left.NotEquals(right)));
      CurBlock.AddStatement(ifStatement);
      return temp;
    }

    private LuaExpressionSyntax BuildBoolXorOfNullAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, bool isLeftNullable, bool isRightNullable) {
      var left = VisitExpression(leftNode);
      var right = VisitExpression(rightNode);
      LuaExpressionSyntax result;
      if (right.IsNil()) {
        result = new LuaConstLiteralExpression(LuaIdentifierLiteralExpressionSyntax.Nil, rightNode.ToString());
      } else if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        result = BuildBoolXorOfNullAssignmentExpression(propertyAdapter.GetCloneOfGet(), right, isLeftNullable, isRightNullable);
      } else {
        result = BuildBoolXorOfNullAssignmentExpression(left, right, isLeftNullable, isRightNullable);
      }
      return BuildLuaSimpleAssignmentExpression(left, result);
    }

    private LuaExpressionSyntax BuildLeftNullableBoolLogicAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, string boolOperatorToken) {
      var left = VisitExpression(leftNode);
      var right = VisitExpression(rightNode);
      var temp = GetTempIdentifier();
      LuaExpressionSyntax identifier;
      CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp));
      if (left is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        var geter = propertyAdapter.GetCloneOfGet();
        identifier = BuildNullableExpressionIdentifier(geter, new List<LuaExpressionSyntax>());
      } else {
        identifier = BuildNullableExpressionIdentifier(left, new List<LuaExpressionSyntax>());
      }
      var ifStatement = new LuaIfStatementSyntax(identifier.EqualsEquals(LuaIdentifierNameSyntax.Nil));
      ifStatement.Body.AddStatement(temp.Assignment(right.Binary(boolOperatorToken, identifier)));
      ifStatement.Else = new LuaElseClauseSyntax();
      ifStatement.Else.Body.AddStatement(temp.Assignment(identifier.Binary(boolOperatorToken, right)));
      CurBlock.AddStatement(ifStatement);
      return BuildLuaSimpleAssignmentExpression(left, temp);
    }

    private LuaExpressionSyntax BuildBitAssignmentExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode, string boolOperatorToken, LuaIdentifierNameSyntax methodName, ExpressionSyntax parnet) {
      if (semanticModel_.GetSymbolInfo(parnet).Symbol is IMethodSymbol symbol) {
        var containingType = symbol.ContainingType;
        if (containingType != null) {
          if (containingType.IsBoolType(false)) {
            switch (parnet.Kind()) {
              case SyntaxKind.ExclusiveOrAssignmentExpression: {
                if (IsNullableType(leftNode, rightNode, out bool isLeftNullable, out bool isRightNullable)) {
                  return BuildBoolXorOfNullAssignmentExpression(leftNode, rightNode, isLeftNullable, isRightNullable);
                }
                break;
              }
              case SyntaxKind.AndAssignmentExpression:
              case SyntaxKind.OrAssignmentExpression: {
                if (IsNullableType(leftNode, rightNode, out bool isLeftNullable, out _) && isLeftNullable) {
                  return BuildLeftNullableBoolLogicAssignmentExpression(leftNode, rightNode, boolOperatorToken);
                }
                break;
              }
            }
            return BuildCommonAssignmentExpression(leftNode, rightNode, boolOperatorToken, null);
          }

          if (containingType.IsIntegerType(false) || containingType.TypeKind == TypeKind.Enum) {
            if (IsLuaClassic) {
              if (IsNullableAssignmentExpression(leftNode, rightNode, null, out var result, methodName)) {
                return result;
              }

              return BuildBinaryInvokeAssignmentExpression(leftNode, rightNode, methodName);
            } else if (IsPreventDebug && IsNullableAssignmentExpression(leftNode, rightNode, null, out var result, methodName)) {
              return result;
            }
          }
        }
      }
      return null;
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
          assignment = node.Left.AcceptExpression(this);
        }
      }
      return assignment;
    }

    private sealed class RefOrOutArgument {
      public LuaExpressionSyntax Expression { get; }
      public bool IsDeclaration { get; }
      public bool IsSpecial { get; }

      public RefOrOutArgument(LuaExpressionSyntax expression) {
        Expression = expression;
      }

      public RefOrOutArgument(LuaExpressionSyntax expression, ArgumentSyntax argument) {
        Expression = expression;
        IsSpecial = IsInSpecialBinaryExpression(argument);
        IsDeclaration = (argument.Expression.IsKind(SyntaxKind.DeclarationExpression) && !IsSpecial) || expression == LuaIdentifierNameSyntax.Placeholder;
      }

      private static bool IsInSpecialBinaryExpression(ArgumentSyntax argument) {
        if (argument.Expression.IsKind(SyntaxKind.DeclarationExpression)) {
          var invocationExpression = (InvocationExpressionSyntax)argument.Parent.Parent;
          var parent = invocationExpression.Parent;
          if (parent.IsKind(SyntaxKind.LogicalAndExpression) || parent.IsKind(SyntaxKind.LogicalOrExpression)) {
            var binaryExpression = (BinaryExpressionSyntax)parent;
            if (binaryExpression.Right == invocationExpression) {
              return true;
            }
          }
        }
        return false;
      }
    }

    private LuaExpressionSyntax BuildInvokeRefOrOut(CSharpSyntaxNode node, LuaExpressionSyntax invocation, IEnumerable<RefOrOutArgument> refOrOutArguments) {
      var locals = new LuaLocalVariablesSyntax();
      var multipleAssignment = new LuaMultipleAssignmentExpressionSyntax();
      var propertyStatements = new LuaStatementListSyntax();

      void FillRefOrOutArguments() {
        foreach (var refOrOutArgument in refOrOutArguments) {
          // fn(out arr[0])
          if (refOrOutArgument.Expression is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
            var propertyTemp = GetTempIdentifier();
            locals.Variables.Add(propertyTemp);
            multipleAssignment.Lefts.Add(propertyTemp);

            var setPropertyAdapter = propertyAdapter.GetClone();
            setPropertyAdapter.IsGetOrAdd = false;
            setPropertyAdapter.ArgumentList.AddArgument(propertyTemp);
            propertyStatements.Statements.Add(setPropertyAdapter);
          } else {
            if (refOrOutArgument.IsDeclaration) {
              locals.Variables.Add((LuaIdentifierNameSyntax)refOrOutArgument.Expression);
            } else if (refOrOutArgument.IsSpecial) {
              CurFunction.Body.AddHeadVariable((LuaIdentifierNameSyntax)refOrOutArgument.Expression);
            }
            multipleAssignment.Lefts.Add(refOrOutArgument.Expression);
          }
        }
      }

      switch (node.Parent.Kind()) {
        case SyntaxKind.ExpressionStatement:
        case SyntaxKind.ConstructorDeclaration: {
          var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
          if (!symbol.ReturnsVoid || node.IsKind(SyntaxKind.ObjectCreationExpression)) {
            var temp = node.Parent.IsKind(SyntaxKind.ExpressionStatement) ? LuaIdentifierNameSyntax.Placeholder : GetTempIdentifier();
            locals.Variables.Add(temp);
            multipleAssignment.Lefts.Add(temp);
          }
          FillRefOrOutArguments();
          multipleAssignment.Rights.Add(invocation);

          if (locals.Variables.Count > 0) {
            CurBlock.Statements.Add(locals);
            if (locals.Variables.SequenceEqual(multipleAssignment.Lefts)) {
              locals.Initializer = new LuaEqualsValueClauseListSyntax(multipleAssignment.Rights);
              if (propertyStatements.Statements.Count > 0) {
                CurBlock.Statements.Add(propertyStatements);
              }
              return LuaExpressionSyntax.EmptyExpression;
            }
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
          var temp = GetTempIdentifier();
          locals.Variables.Add(temp);
          multipleAssignment.Lefts.Add(temp);
          FillRefOrOutArguments();
          multipleAssignment.Rights.Add(invocation);
          CurBlock.Statements.Add(locals);
          if (locals.Variables.SequenceEqual(multipleAssignment.Lefts)) {
            locals.Initializer = new LuaEqualsValueClauseListSyntax(multipleAssignment.Rights);
          } else {
            CurBlock.Statements.Add(multipleAssignment);
          }
          if (propertyStatements.Statements.Count > 0) {
            CurBlock.Statements.Add(propertyStatements);
          }
          return temp;
        }
      }
    }

    private bool IsEnumToStringInvocationExpression(IMethodSymbol symbol, InvocationExpressionSyntax node, out LuaExpressionSyntax result) {
      result = null;
      if (symbol.Name == "ToString") {
        if (symbol.ContainingType.SpecialType == SpecialType.System_Enum) {
          var memberAccessExpression = (MemberAccessExpressionSyntax)node.Expression;
          var target = memberAccessExpression.Expression.AcceptExpression(this);
          var enumTypeSymbol = semanticModel_.GetTypeInfo(memberAccessExpression.Expression).Type;
          result = BuildEnumToStringExpression(enumTypeSymbol, false, target, memberAccessExpression.Expression);
          return true;
        } else if (symbol.ContainingType.IsEnumType(out var enumTypeSymbol, out bool isNullable)) {
          var memberAccessExpression = (MemberAccessExpressionSyntax)node.Expression;
          var target = memberAccessExpression.Expression.AcceptExpression(this);
          result = BuildEnumToStringExpression(enumTypeSymbol, isNullable, target, memberAccessExpression.Expression);
          return true;
        }
      }
      return false;
    }

    private List<Func<LuaExpressionSyntax>> FillCodeTemplateInvocationArguments(IMethodSymbol symbol, ArgumentListSyntax argumentList, List<Func<LuaExpressionSyntax>> argumentExpressions) {
      argumentExpressions ??= new();
      foreach (var argument in argumentList.Arguments) {
        if (argument.NameColon != null) {
          string name = argument.NameColon.Name.Identifier.ValueText;
          int index = symbol.Parameters.IndexOf(i => i.Name == name);
          if (index == -1) {
            throw new InvalidOperationException();
          }
          argumentExpressions.AddAt(index, () => VisitExpression(argument.Expression));
        } else {
          argumentExpressions.Add(() => VisitExpression(argument.Expression));
        }
      }

      for (int i = 0; i < argumentExpressions.Count; ++i) {
        if (argumentExpressions[i] == null) {
          argumentExpressions[i] = () => GetDefaultParameterValue(symbol.Parameters[i], argumentList.Parent, true);
        }
      }

      if (symbol.Parameters.Length > argumentList.Arguments.Count) {
        argumentExpressions.AddRange(symbol.Parameters.Skip(argumentList.Arguments.Count).Where(i => !i.IsParams).Select(i => {
          Func<LuaExpressionSyntax> func = () => GetDefaultParameterValue(i, argumentList.Parent, true);
          return func;
        }));
      }

      return argumentExpressions;
    }

    private LuaExpressionSyntax CheckCodeTemplateInvocationExpression(IMethodSymbol symbol, InvocationExpressionSyntax node) {
      var kind = node.Expression.Kind();
      if (kind == SyntaxKind.SimpleMemberAccessExpression || kind == SyntaxKind.MemberBindingExpression || kind == SyntaxKind.IdentifierName) {
        if (IsEnumToStringInvocationExpression(symbol, node, out var result)) {
          return result;
        }

        string codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(symbol);
        if (codeTemplate != null) {
          var argumentExpressions = new List<Func<LuaExpressionSyntax>>();
          var memberAccessExpression = node.Expression as MemberAccessExpressionSyntax;
          if (symbol.IsExtensionMethod) {
            if (symbol.ReducedFrom != null) {
              if (memberAccessExpression != null) {
                argumentExpressions.Add(() => memberAccessExpression.Expression.AcceptExpression(this));
              } else {
                Contract.Assert(kind == SyntaxKind.MemberBindingExpression);
                argumentExpressions.Add(() => conditionalTemps_.Peek());
              }
            }
            if (symbol.ContainingType.IsSystemLinqEnumerable()) {
              CurCompilationUnit.ImportLinq();
            }
          }

          FillCodeTemplateInvocationArguments(symbol, node.ArgumentList, argumentExpressions);
          var invocationExpression = InternalBuildCodeTemplateExpression(
            codeTemplate,
            memberAccessExpression?.Expression,
            argumentExpressions,
            symbol.TypeArguments,
            kind == SyntaxKind.MemberBindingExpression ? conditionalTemps_.Peek() : null);

          var refOrOuts = node.ArgumentList.Arguments.Where(i => i.RefKindKeyword.IsOutOrRef());
          if (refOrOuts.Any()) {
            var refOrOutArguments = refOrOuts.Select(i => {
              var argument = i.AcceptExpression(this);
              return new RefOrOutArgument(argument, i);
            });
            return BuildInvokeRefOrOut(node, invocationExpression, refOrOutArguments);
          } else {
            return invocationExpression;
          }
        }
      }

      return null;
    }

    private List<LuaExpressionSyntax> BuildInvocationArguments(IMethodSymbol symbol, InvocationExpressionSyntax node, out List<RefOrOutArgument> refOrOutArguments) {
      refOrOutArguments = new List<RefOrOutArgument>();
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

    private LuaInvocationExpressionSyntax CheckInvocationExpression(IMethodSymbol symbol, LuaExpressionSyntax expression) {
      LuaInvocationExpressionSyntax invocation;
      if (symbol != null && symbol.IsExtensionMethod) {
        if (expression is LuaMemberAccessExpressionSyntax memberAccess) {
          if (memberAccess.Name is LuaInternalMethodExpressionSyntax) {
            invocation = new LuaInvocationExpressionSyntax(memberAccess.Name);
            invocation.AddArgument(memberAccess.Expression);
          } else if (symbol.ReducedFrom != null) {
            invocation = BuildExtensionMethodInvocation(symbol.ReducedFrom, memberAccess.Expression);
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
            if (IsPreventDebug && symbol != null && !symbol.IsStatic) {
              var containingType = symbol.ContainingType;
              if (containingType.IsBasicType()) {
                var typeName = GetTypeName(containingType);
                invocation = typeName.MemberAccess(memberAccess.Name).Invocation(memberAccess.Expression);
              } else if (containingType.SpecialType == SpecialType.System_Object || containingType.IsBasicTypInterface()) {
                var methodMemberAccess = LuaIdentifierNameSyntax.System.MemberAccess(new LuaCodeTemplateExpressionSyntax(symbol.ContainingType.Name, memberAccess.Name));
                invocation = methodMemberAccess.Invocation(memberAccess.Expression);
              }
            }
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
        if (symbol.ReturnsVoid) {
          if (generator_.IsConditionalAttributeIgnore(symbol) || symbol.IsEmptyPartialMethod()) {
            return LuaExpressionSyntax.EmptyExpression;
          }
        }

        var codeTemplateExpression = CheckCodeTemplateInvocationExpression(symbol, node);
        if (codeTemplateExpression != null) {
          return codeTemplateExpression;
        }
      }

      var arguments = BuildInvocationArguments(symbol, node, out var refOrOutArguments);
      var expression = node.Expression.AcceptExpression(this);
      var invocation = CheckInvocationExpression(symbol, expression);
      invocation.AddArguments(arguments);
      LuaExpressionSyntax resultExpression = invocation;
      if (symbol != null && symbol.HasAggressiveInliningAttribute()) {
        if (InliningInvocationExpression(node, symbol, invocation, out var inlineExpression)) {
          resultExpression = inlineExpression;
        }
      }
      if (refOrOutArguments.Count > 0) {
        resultExpression = BuildInvokeRefOrOut(node, resultExpression, refOrOutArguments);
      }
      return resultExpression;
    }

    private LuaInvocationExpressionSyntax BuildExtensionMethodInvocation(IMethodSymbol reducedFrom, LuaExpressionSyntax expression) {
      var typeName = GetTypeName(reducedFrom.ContainingType);
      var methodName = GetMemberName(reducedFrom);
      return typeName.MemberAccess(methodName).Invocation(expression);
    }

    private LuaExpressionSyntax GetDefaultParameterValue(IParameterSymbol parameter, SyntaxNode node, bool isCheckCallerAttribute) {
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

    private void CheckInvocationDefaultArguments(
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
            var elementType = GetTypeName(arrayType.ElementType);
            arguments.Add(LuaIdentifierNameSyntax.EmptyArray.Invocation(elementType));
          } else {
            LuaExpressionSyntax defaultValue = GetDefaultParameterValue(parameter, node, isCheckCallerAttribute);
            arguments.Add(defaultValue);
          }
        }
      } else if (!parameters.IsEmpty) {
        IParameterSymbol last = parameters.Last();
        if (last.IsParams && generator_.IsFromLuaModule(symbol) && !symbol.HasParamsAttribute()) {
          if (parameters.Length == arguments.Count) {
            var paramsArgument = argumentNodeInfos.Last();
            if (paramsArgument.Name != null) {
              string name = paramsArgument.Name.Name.Identifier.ValueText;
              if (name != last.Name) {
                paramsArgument = argumentNodeInfos.First(i => i.Name != null && i.Name.Name.Identifier.ValueText == last.Name);
              }
            }
            var paramsType = semanticModel_.GetTypeInfo(paramsArgument.Expression).Type;
            bool isLastParamsArrayType = paramsType != null && paramsType.TypeKind == TypeKind.Array;
            if (!isLastParamsArrayType) {
              var arrayTypeSymbol = (IArrayTypeSymbol)last.Type;
              var array = BuildArray(arrayTypeSymbol.ElementType, arguments.Last());
              arguments[^1] = array;
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
          LuaExpressionSyntax defaultValue = GetDefaultParameterValue(parameters[i], node, isCheckCallerAttribute);
          arguments[i] = defaultValue;
        }
      }
    }

    private void CheckInvocationDefaultArguments(ISymbol symbol, ImmutableArray<IParameterSymbol> parameters, List<LuaExpressionSyntax> arguments, BaseArgumentListSyntax node) {
      var argumentNodeInfos = node.Arguments.Select(i => (i.NameColon, i.Expression)).ToList();
      CheckInvocationDefaultArguments(symbol, parameters, arguments, argumentNodeInfos, node.Parent, true);
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
        var statement = curBlock.Statements.FindLast(i => !(i is LuaBlankLinesStatement) && !(i is LuaCommentStatement));
        if (statement != null) {
          statement.ForceSemicolon = true;
        }
      }
    }

    private LuaExpressionSyntax BuildMemberAccessTargetExpression(ExpressionSyntax targetExpression) {
      var expression = targetExpression.AcceptExpression(this);
      SyntaxKind kind = targetExpression.Kind();
      if ((kind >= SyntaxKind.NumericLiteralExpression && kind <= SyntaxKind.NullLiteralExpression) || (expression is LuaLiteralExpressionSyntax)) {
        CheckPrevIsInvokeStatement(targetExpression);
        expression = expression.Parenthesized();
      }
      return expression;
    }

    private LuaExpressionSyntax BuildMemberAccessExpression(ISymbol symbol, ExpressionSyntax node) {
      bool isExtensionMethod = symbol.Kind == SymbolKind.Method && ((IMethodSymbol)symbol).IsExtensionMethod;
      if (isExtensionMethod) {
        return node.AcceptExpression(this);
      } else {
        return BuildMemberAccessTargetExpression(node);
      }
    }

    private LuaExpressionSyntax CheckMemberAccessCodeTemplate(ISymbol symbol, MemberAccessExpressionSyntax node) {
      if (symbol.Kind == SymbolKind.Field) {
        IFieldSymbol fieldSymbol = (IFieldSymbol)symbol;
        if (fieldSymbol.ContainingType.IsTupleType) {
          int elementIndex = fieldSymbol.GetTupleElementIndex();
          var targetExpression = node.Expression.AcceptExpression(this);
          return new LuaTableIndexAccessExpressionSyntax(targetExpression, elementIndex);
        }

        string codeTemplate = XmlMetaProvider.GetFieldCodeTemplate(fieldSymbol);
        if (codeTemplate != null) {
          return BuildCodeTemplateExpression(codeTemplate, node.Expression);
        }

        if (fieldSymbol.HasConstantValue) {
          if (!fieldSymbol.IsStringConstNotInline()) {
            return GetConstLiteralExpression(fieldSymbol);
          }
        }

        if (XmlMetaProvider.IsFieldForceProperty(fieldSymbol)) {
          var expression = node.Expression.AcceptExpression(this);
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
            result = result.Parenthesized();
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
        var nameExpression = node.Name.AcceptExpression(this);
        if (symbol.Kind == SymbolKind.Method) {
          if (IsDelegateExpression((IMethodSymbol)symbol, node, nameExpression, LuaIdentifierNameSyntax.This, out var delegateExpression)) {
            return delegateExpression;
          }
        }
        return nameExpression;
      }

      if (node.Expression.IsKind(SyntaxKind.BaseExpression)) {
        var baseExpression = node.Expression.AcceptExpression(this);
        var nameExpression = node.Name.AcceptExpression(this);
        if (symbol.Kind == SymbolKind.Property || symbol.Kind == SymbolKind.Event) {
          if (nameExpression is LuaPropertyAdapterExpressionSyntax propertyMethod) {
            if (baseExpression != LuaIdentifierNameSyntax.This) {
              propertyMethod.ArgumentList.AddArgument(LuaIdentifierNameSyntax.This);
            }
            propertyMethod.Update(baseExpression, false);
            return propertyMethod;
          } else {
            return baseExpression.MemberAccess(nameExpression);
          }
        } else {
          if (baseExpression == LuaIdentifierNameSyntax.This) {
            return baseExpression.MemberAccess(nameExpression, symbol.Kind == SymbolKind.Method);
          } else {
            return new LuaInternalMethodExpressionSyntax(baseExpression.MemberAccess(nameExpression));
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
          return node.Name.AcceptExpression(this);
        }
      }

      var name = node.Name.AcceptExpression(this);
      if (generator_.IsInlineSymbol(symbol)) {
        return name;
      }

      var expression = BuildMemberAccessExpression(symbol, node.Expression);
      if (symbol.Kind == SymbolKind.Property || symbol.Kind == SymbolKind.Event) {
        return BuildFieldOrPropertyMemberAccessExpression(expression, name, symbol.IsStatic);
      }

      if (symbol.Kind == SymbolKind.Method) {
        if (IsDelegateExpression((IMethodSymbol)symbol, node, name, expression, out var delegateExpression)) {
          return delegateExpression;
        }
      }

      return expression.MemberAccess(name, !symbol.IsStatic && symbol.Kind == SymbolKind.Method);
    }

    private bool IsDelegateExpression(IMethodSymbol symbol, MemberAccessExpressionSyntax node, LuaExpressionSyntax name, LuaExpressionSyntax expression, out LuaExpressionSyntax delegateExpression) {
      if (!node.Parent.IsKind(SyntaxKind.InvocationExpression)) {
        if (!IsInternalMember(symbol)) {
          name = expression.MemberAccess(name);
        }
        delegateExpression = BuildDelegateNameExpression(symbol, expression, name, node);
        return true;
      } else if (IsDelegateInvoke(symbol, node.Name)) {
        delegateExpression = expression;
        return true;
      }
      delegateExpression = null;
      return false;
    }

    private static bool IsDelegateInvoke(ISymbol symbol, SimpleNameSyntax name) {
      return symbol.ContainingType.IsDelegateType() && name.Identifier.ValueText == "Invoke";
    }

    public override LuaSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node) {
      ISymbol symbol = semanticModel_.GetSymbolInfo(node).Symbol;
      if (symbol == null) {  // dynamic
        var expressSymol = semanticModel_.GetSymbolInfo(node.Expression).Symbol;
        var expression = node.Expression.AcceptExpression(this);
        bool isObjectColon = node.Parent.IsKind(SyntaxKind.InvocationExpression) && (expressSymol == null || expressSymol.Kind != SymbolKind.NamedType);
        LuaIdentifierNameSyntax name = node.Name.Identifier.ValueText;
        return expression.MemberAccess(name, isObjectColon);
      }

      if (symbol.Kind == SymbolKind.NamedType) {
        var expressionSymbol = semanticModel_.GetSymbolInfo(node.Expression).Symbol;
        if (expressionSymbol.Kind == SymbolKind.Namespace || expressionSymbol.Kind == SymbolKind.NamedType) {
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
              return LuaIdentifierNameSyntax.This.MemberAccess(name);
            } else {
              var typeName = GetTypeName(symbol.ContainingType);
              return typeName.MemberAccess(name);
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
          return typeName.MemberAccess(name);
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
        case SyntaxKind.NameColon:
        case SyntaxKind.NameEquals: {
          return false;
        }
        case SyntaxKind.SimpleAssignmentExpression: {
          AssignmentExpressionSyntax parent = (AssignmentExpressionSyntax)parentNode;
          if (parent.Right != node) {
            switch (parent.Parent.Kind()) {
              case SyntaxKind.ObjectInitializerExpression:
              case SyntaxKind.WithInitializerExpression:
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

    private LuaExpressionSyntax VisitPropertyOrEventIdentifierName(IdentifierNameSyntax node, ISymbol symbol, bool isProperty, out bool isField) {
      bool isReadOnly;
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
            return LuaIdentifierNameSyntax.This.MemberAccess(fieldName);
          } else {
            return fieldName;
          }
        }
      } else {
        if (isProperty) {
          var propertySymbol = (IPropertySymbol)symbol;
          if (IsWantInline(propertySymbol)) {
            if (InliningPropertyGetExpression(node, propertySymbol.GetMethod, out var inlineExpression)) {
              return inlineExpression;
            }
          }
        }

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
            return usingStaticType.MemberAccess(identifierName);
          }
          return identifierExpression;
        } else {
          if (IsInternalMember(symbol)) {
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
      var kind = node.Parent.Kind();
      switch (kind) {
        case SyntaxKind.SimpleMemberAccessExpression:
        case SyntaxKind.InvocationExpression:
        case SyntaxKind.MemberBindingExpression:
          return false;
      }
      return true;
    }

    private sealed class GenericPlaceholder {
      public ITypeSymbol Symbol { get; }
      private readonly int index_;
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
        case SyntaxKind.ReturnStatement: {
          var type = (INamedTypeSymbol)CurMethodInfoOrNull.Symbol.ReturnType;
          return type.DelegateInvokeMethod;
        }
        default:
          throw new InvalidProgramException();
      }
    }

    private void CheckDelegateBind(IMethodSymbol symbol, CSharpSyntaxNode node, ref LuaExpressionSyntax nameExpression) {
      const int kReturnParameterIndex = int.MaxValue;
      if (symbol.IsGenericMethod) {
        var originalDefinition = symbol.OriginalDefinition;
        if (!originalDefinition.EQ(symbol)) {
          var targetMethodSymbol = GetDelegateTargetMethodSymbol(node);
          var targetTypeParameters = new List<TypeParameterPlaceholder>();
          foreach (var typeArgument in targetMethodSymbol.ContainingType.TypeArguments) {
            if (typeArgument.TypeKind == TypeKind.TypeParameter) {
              int parameterIndex = targetMethodSymbol.Parameters.IndexOf(i => i.Type.IsTypeParameterExists(typeArgument));
              if (parameterIndex == -1) {
                Contract.Assert(targetMethodSymbol.ReturnType != null && targetMethodSymbol.ReturnType.IsTypeParameterExists(typeArgument));
                parameterIndex = kReturnParameterIndex;
              }
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
              if (originalDefinition.ReturnType != null && originalDefinition.ReturnType.IsTypeParameterExists(originalTypeArgument)) {
                originalTypeParameters.Add(new TypeParameterPlaceholder() {
                  Symbol = originalTypeArgument,
                  ParameterIndex = kReturnParameterIndex,
                });
              } else {
                var typeArgument = symbol.TypeArguments[j];
                Contract.Assert(typeArgument.TypeKind != TypeKind.TypeParameter);
                originalTypeParameters.Add(new TypeParameterPlaceholder() {
                  Symbol = typeArgument,
                  ParameterIndex = -1,
                });
              }
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
                ITypeSymbol parameterType;
                if (parameterIndex == kReturnParameterIndex) {
                  Contract.Assert(targetMethodSymbol.ReturnType != null);
                  parameterType = targetMethodSymbol.ReturnType;
                } else {
                  var parameter = targetMethodSymbol.Parameters[parameterIndex];
                  parameterType = parameter.Type;
                }
                placeholders.Add(new GenericPlaceholder(parameterType));
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
                nameExpression = invocationExpression;
                return;
              }
            } else if (symbol.Parameters.Length == 2) {
              if (placeholders.Count == 2) {
                if (placeholders[0].TypeParameterIndex == 2 && placeholders[1].TypeParameterIndex == 1) {
                  string bindMethodName = LuaIdentifierNameSyntax.DelegateBind.ValueText + "2_1";
                  nameExpression = new LuaInvocationExpressionSyntax(bindMethodName, nameExpression);
                  return;
                }
              } else if (placeholders.Count == 1) {
                if (placeholders.First().TypeParameterIndex == 2) {
                  string bindMethodName = LuaIdentifierNameSyntax.DelegateBind.ValueText + "0_2";
                  nameExpression = new LuaInvocationExpressionSyntax(bindMethodName, nameExpression);
                  return;
                }
              }
            }

            var invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateBind, nameExpression, symbol.Parameters.Length.ToString());
            invocation.AddArguments(placeholders.Select(i => i.Build(this)));
            nameExpression = invocation;
          }
        }
      }
    }

    private LuaExpressionSyntax BuildDelegateNameExpression(IMethodSymbol symbol, LuaExpressionSyntax target, LuaExpressionSyntax name, CSharpSyntaxNode node) {
      LuaExpressionSyntax nameExpression;
      if (symbol.IsStatic) {
        nameExpression = name;
      } else {
        nameExpression = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.DelegateMake, target, name);
      }
      CheckDelegateBind(symbol, node, ref nameExpression);
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
        if (IsInternalMember(symbol)) {
          if (IsParentDelegateName(node)) {
            return BuildDelegateNameExpression(symbol, methodName, node);
          }
          return new LuaInternalMethodExpressionSyntax(methodName);
        }
        if (CurTypeSymbol.IsContainsInternalSymbol(symbol) && IsMoreThanLocalVariables(symbol)) {
          return LuaIdentifierNameSyntax.MorenManyLocalVarTempTable.MemberAccess(methodName);
        }
        return methodName;
      } else {
        if (IsInternalMember(symbol)) {
          if (IsParentDelegateName(node)) {
            return BuildDelegateNameExpression(symbol, methodName, node);
          }

          return new LuaInternalMethodExpressionSyntax(methodName);
        } else {
          if (IsInternalNode(node)) {
            if (IsParentDelegateName(node)) {
              return BuildDelegateNameExpression(symbol, LuaIdentifierNameSyntax.This.MemberAccess(methodName), node);
            }
            return LuaIdentifierNameSyntax.This.MemberAccess(methodName, true);
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
            return LuaIdentifierNameSyntax.This.MemberAccess(name);
          }
          return LuaIdentifierNameSyntax.This.MemberAccess(GetMemberName(symbol));
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
      if (symbol == null) {  // dynamic
        return (LuaIdentifierNameSyntax)node.Identifier.ValueText;
      }

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
          var codeTemplate = fieldSymbol.GetCodeTemplateFromAttribute();
          if (codeTemplate != null) {
            identifier = BuildCodeTemplateExpression(codeTemplate, fieldSymbol.IsStatic ? null : LuaIdentifierNameSyntax.This);
            break;
          }

          identifier = GetFieldNameExpression(fieldSymbol, node);
          CheckValueTypeClone(fieldSymbol.Type, node, ref identifier);
          break;
        }
        case SymbolKind.Method: {
          var methodSymbol = (IMethodSymbol)symbol;
          if (methodSymbol.MethodKind == MethodKind.LocalFunction) {
            identifier = GetLocalMethodName(methodSymbol, node);
          } else {
            identifier = GetMethodNameExpression(methodSymbol, node);
          }
          break;
        }
        case SymbolKind.Property: {
          var propertyField = (IPropertySymbol)symbol;
          identifier = VisitPropertyOrEventIdentifierName(node, propertyField, true, out bool isField);
          if (isField) {
            CheckValueTypeClone(propertyField.Type, node, ref identifier, true);
          }
          break;
        }
        case SymbolKind.Event: {
          identifier = VisitPropertyOrEventIdentifierName(node, symbol, false, out _);
          break;
        }
        case SymbolKind.Discard: {
          identifier = LuaIdentifierNameSyntax.Placeholder;
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

    private void FillInvocationArgument(List<LuaExpressionSyntax> arguments, ArgumentSyntax node, ImmutableArray<IParameterSymbol> parameters, List<RefOrOutArgument> refOrOutArguments) {
      var expression = node.Expression.AcceptExpression(this);
      Contract.Assert(expression != null);
      if (node.RefKindKeyword.IsKind(SyntaxKind.RefKeyword)) {
        refOrOutArguments.Add(new RefOrOutArgument(expression));
      } else if (node.RefKindKeyword.IsKind(SyntaxKind.OutKeyword)) {
        refOrOutArguments.Add(new RefOrOutArgument(expression, node));
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

    private List<LuaExpressionSyntax> BuildArgumentList(ISymbol symbol, ImmutableArray<IParameterSymbol> parameters, BaseArgumentListSyntax node, List<RefOrOutArgument> refOrOutArguments = null) {
      Contract.Assert(node != null);
      List<LuaExpressionSyntax> arguments = new List<LuaExpressionSyntax>();
      foreach (var argument in node.Arguments) {
        FillInvocationArgument(arguments, argument, parameters, refOrOutArguments);
      }
      CheckInvocationDefaultArguments(symbol, parameters, arguments, node);
      return arguments;
    }

    private LuaArgumentListSyntax BuildArgumentList(SeparatedSyntaxList<ArgumentSyntax> arguments) {
      var argumentList = new LuaArgumentListSyntax();
      foreach (var argument in arguments) {
        var newNode = argument.AcceptExpression(this);
        argumentList.Arguments.Add(newNode);
      }
      return argumentList;
    }

    public override LuaSyntaxNode VisitArgumentList(ArgumentListSyntax node) {
      return BuildArgumentList(node.Arguments);
    }

    public override LuaSyntaxNode VisitArgument(ArgumentSyntax node) {
      Contract.Assert(node.NameColon == null);
      return node.Expression.AcceptExpression(this);
    }

    private bool IsExtremelyZero(LiteralExpressionSyntax node, string value) {
      object v = semanticModel_.GetConstantValue(node).Value;
      var isFloatZero = (v is float f && f == 0) || (v is double d && d == 0);
      return isFloatZero && value.Length > 5;
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
                  if (IsExtremelyZero(node, value)) {
                    value = LuaNumberLiteralExpressionSyntax.ZeroFloat.Text;
                    hasTransform = true;
                  } else {
                    removeCount = 1;
                  }
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
              case 'u':
              case 'U':
              case 'm':
              case 'M': {
                removeCount = 1;
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
      var declaration = node.Declaration.Accept<LuaVariableDeclarationSyntax>(this);
      if (node.UsingKeyword.IsKeyword()) {
        var block = CurBlock;
        if (block.UsingDeclarations == null) {
          block.UsingDeclarations = new List<int>();
        }
        block.UsingDeclarations.Add(block.Statements.Count);
      }
      return new LuaLocalDeclarationStatementSyntax(declaration);
    }

    private bool IsValueTypeVariableDeclarationWithoutAssignment(VariableDeclaratorSyntax variable) {
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
        bool isRefIgnore = false;
        if (variable.Initializer != null && variable.Initializer.Value.IsKind(SyntaxKind.RefExpression)) {
          var value = (RefExpressionSyntax)variable.Initializer.Value;
          var refExpression = value.AcceptExpression(this);
          if (value.Expression.IsKind(SyntaxKind.InvocationExpression)) {
            var invocationExpression = (LuaInvocationExpressionSyntax)refExpression;
            AddRefInvocationVariableMapping((InvocationExpressionSyntax)value.Expression, invocationExpression, variable);
          } else {
            AddLocalVariableMapping(new LuaSymbolNameSyntax(refExpression), variable);
            isRefIgnore = true;
          }
        }

        if (!isRefIgnore) {
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
            var variableDeclarator = variable.Accept<LuaVariableDeclaratorSyntax>(this);
            if (variableDeclarator.Initializer == null) {
              if (typeSymbol == null) {
                typeSymbol = semanticModel_.GetTypeInfo(node.Type).Type;
              }
              if (typeSymbol.IsCustomValueType() && IsValueTypeVariableDeclarationWithoutAssignment(variable)) {
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
        LuaLocalVariablesSyntax declarationStatement = new LuaLocalVariablesSyntax();
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
      var variableDeclarator = new LuaVariableDeclaratorSyntax(identifier);
      if (node.Initializer != null) {
        variableDeclarator.Initializer = node.Initializer.Accept<LuaEqualsValueClauseSyntax>(this);
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
        var blockNode = statement.Accept<LuaBlockSyntax>(this);
        block.Statements.AddRange(blockNode.Statements);
      } else {
        PushBlock(block);
        var statementNode = statement.Accept<LuaStatementSyntax>(this);
        block.Statements.Add(statementNode);
        PopBlock();
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

        LuaBlockSyntax conditionBody = new LuaBlockSyntax();
        PushBlock(conditionBody);
        var condition = VisitExpression(ifStatement.Condition);
        PopBlock();

        if (conditionBody.Statements.Count == 0) {
          var elseIfStatement = new LuaElseIfStatementSyntax(condition);
          WriteStatementOrBlock(ifStatement.Statement, elseIfStatement.Body);
          ifStatements_.Peek().ElseIfStatements.Add(elseIfStatement);
          ifStatement.Else?.Accept(this);
          return elseIfStatement;
        } else {
          var elseClause = new LuaElseClauseSyntax();
          elseClause.Body.Statements.AddRange(conditionBody.Statements);
          var elseIfStatement = new LuaIfStatementSyntax(condition);
          WriteStatementOrBlock(ifStatement.Statement, elseIfStatement.Body);
          elseClause.Body.AddStatement(elseIfStatement);
          ifStatements_.Peek().Else = elseClause;
          ifStatements_.Push(elseIfStatement);
          ifStatement.Else?.Accept(this);
          ifStatements_.Pop();
          return elseClause;
        }
      } else {
        LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
        WriteStatementOrBlock(node.Statement, elseClause.Body);
        ifStatements_.Peek().Else = elseClause;
        return elseClause;
      }
    }

    public override LuaSyntaxNode VisitSwitchStatement(SwitchStatementSyntax node) {
      var temp = GetTempIdentifier();
      var switchStatement = new LuaSwitchAdapterStatementSyntax(temp);
      switchs_.Push(switchStatement);
      var expression = node.Expression.AcceptExpression(this);
      switchStatement.Fill(expression, node.Sections.Select(i => i.Accept<LuaStatementSyntax>(this)));
      switchs_.Pop();
      return switchStatement;
    }

    private void FillSwitchSectionStatements(LuaBlockSyntax block, SwitchSectionSyntax node) {
      if (node.Statements.Count == 1 && node.Statements.First().IsKind(SyntaxKind.Block)) {
        var luaBlock = node.Statements.First().Accept<LuaBlockSyntax>(this);
        block.Statements.AddRange(luaBlock.Statements);
      } else {
        PushBlock(block);
        var statements = VisitTriviaAndNode(node, node.Statements);
        block.AddStatements(statements);
        PopBlock();
      }
    }

    public override LuaSyntaxNode VisitSwitchSection(SwitchSectionSyntax node) {
      bool isDefault = node.Labels.Any(i => i.Kind() == SyntaxKind.DefaultSwitchLabel);
      if (isDefault) {
        var block = new LuaBlockSyntax();
        FillSwitchSectionStatements(block, node);
        return block;
      } else {
        var expressions = node.Labels.Select(i => i.AcceptExpression(this));
        var condition = expressions.Aggregate((x, y) => x.Or(y));
        var ifStatement = new LuaIfStatementSyntax(condition);
        FillSwitchSectionStatements(ifStatement.Body, node);
        return ifStatement;
      }
    }

    public override LuaSyntaxNode VisitCaseSwitchLabel(CaseSwitchLabelSyntax node) {
      var left = switchs_.Peek().Temp;
      var right = node.Value.AcceptExpression(this);
      return left.EqualsEquals(right);
    }

    private LuaExpressionSyntax BuildSwitchLabelWhenClause(LuaExpressionSyntax expression, WhenClauseSyntax whenClause) {
      if (whenClause != null) {
        var whenExpression = whenClause.AcceptExpression(this);
        return expression != null ? expression.And(whenExpression) : whenExpression;
      } else {
        return expression;
      }
    }

    private LuaExpressionSyntax BuildDeclarationPattern(DeclarationPatternSyntax declarationPattern, LuaIdentifierNameSyntax left, ExpressionSyntax expressionType, WhenClauseSyntax whenClause) {
      if (!declarationPattern.Designation.IsKind(SyntaxKind.DiscardDesignation)) {
        AddLocalVariableMapping(left, declarationPattern.Designation);
      }
      var isExpression = BuildIsPatternExpression(expressionType, declarationPattern.Type, left);
      if (isExpression == LuaIdentifierLiteralExpressionSyntax.True) {
        return whenClause != null ? whenClause.AcceptExpression(this) : LuaIdentifierLiteralExpressionSyntax.True;
      } else {
        return BuildSwitchLabelWhenClause(isExpression, whenClause);
      }
    }

    public override LuaSyntaxNode VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node) {
      var left = switchs_.Peek().Temp;
      switch (node.Pattern.Kind()) {
        case SyntaxKind.DeclarationPattern: {
          var switchStatement = (SwitchStatementSyntax)FindParent(node, SyntaxKind.SwitchStatement);
          var declarationPattern = (DeclarationPatternSyntax)node.Pattern;
          return BuildDeclarationPattern(declarationPattern, left, switchStatement.Expression, node.WhenClause);
        }
        case SyntaxKind.VarPattern: {
          var varPattern = (VarPatternSyntax)node.Pattern;
          AddLocalVariableMapping(left, varPattern.Designation);
          return BuildSwitchLabelWhenClause(null, node.WhenClause);
        }
        default: {
          var patternExpression = node.Pattern.AcceptExpression(this);
          var expression = left.EqualsEquals(patternExpression);
          return BuildSwitchLabelWhenClause(expression, node.WhenClause);
        }
      }
    }

    public override LuaSyntaxNode VisitWhenClause(WhenClauseSyntax node) {
      return node.Condition.AcceptExpression(this);
    }

    public override LuaSyntaxNode VisitConstantPattern(ConstantPatternSyntax node) {
      return node.Expression.AcceptExpression(this);
    }

    private void FillSwitchPatternSyntax(ref LuaIfStatementSyntax ifStatement, LuaExpressionSyntax condition, WhenClauseSyntax whenClause, LuaIdentifierNameSyntax assignmentLeft, ExpressionSyntax assignmentRight) {
      if (condition == null && whenClause == null) {
        condition = LuaIdentifierLiteralExpressionSyntax.True;
      } else {
        condition = BuildSwitchLabelWhenClause(condition, whenClause);
      }
      if (ifStatement == null) {
        ifStatement = new LuaIfStatementSyntax(condition);
        PushBlock(ifStatement.Body);
        var rightExpression = assignmentRight.AcceptExpression(this);
        PopBlock();
        ifStatement.Body.AddStatement(assignmentLeft.Assignment(rightExpression));
      } else {
        var elseIfStatement = new LuaElseIfStatementSyntax(condition);
        PushBlock(elseIfStatement.Body);
        var rightExpression = assignmentRight.AcceptExpression(this);
        PopBlock();
        elseIfStatement.Body.AddStatement(assignmentLeft.Assignment(rightExpression));
        ifStatement.ElseIfStatements.Add(elseIfStatement);
      }
    }

    private void CheckSwitchDeconstruct(ref LuaLocalVariablesSyntax deconstruct, LuaIdentifierNameSyntax identifier, ExpressionSyntax type, int count) {
      if (deconstruct == null) {
        var typeSymbol = semanticModel_.GetTypeInfo(type).Type;
        var deconstructInvocation = BuildDeconstructExpression(typeSymbol, identifier, type);
        deconstruct = new LuaLocalVariablesSyntax();
        for (int i = 0; i < count; ++i) {
          deconstruct.Variables.Add(GetTempIdentifier());
        }
        deconstruct.Initializer = new LuaEqualsValueClauseListSyntax();
        deconstruct.Initializer.Values.Add(deconstructInvocation);
      }
    }

    public LuaExpressionSyntax BuildPropertyPatternNameExpression(LuaIdentifierNameSyntax governingIdentifier, IdentifierNameSyntax nameIdentifier) {
      var symbol = semanticModel_.GetSymbolInfo(nameIdentifier).Symbol;
      if (symbol.Kind == SymbolKind.Field) {
        var name = GetMemberName(symbol);
        return governingIdentifier.MemberAccess(name);
      } else {
        var propertySymbol = (IPropertySymbol)symbol;
        var codeTemplate = XmlMetaProvider.GetProertyCodeTemplate(propertySymbol, true);
        if (codeTemplate != null) {
          return InternalBuildCodeTemplateExpression(codeTemplate, null, null, null, governingIdentifier);
        }

        var name = nameIdentifier.AcceptExpression(this);
        return governingIdentifier.MemberAccess(name, name is LuaPropertyAdapterExpressionSyntax);
      }
    }

    private LuaExpressionSyntax BuildRecursivePatternExpression(RecursivePatternSyntax recursivePattern, LuaIdentifierNameSyntax governingIdentifier, LuaLocalVariablesSyntax deconstruct, ExpressionSyntax governingExpression) {
      var subpatterns = recursivePattern.PropertyPatternClause?.Subpatterns ?? recursivePattern.PositionalPatternClause.Subpatterns;
      var subpatternExpressions = new List<LuaExpressionSyntax>();
      int subpatternIndex = 0;
      foreach (var subpattern in subpatterns) {
        var expression = subpattern.Pattern.AcceptExpression(this);
        if (subpattern.NameColon != null) {
          LuaExpressionSyntax left;
          if (governingIdentifier != null) {
            left = BuildPropertyPatternNameExpression(governingIdentifier, subpattern.NameColon.Name);
          } else {
            var fieldSymbol = (IFieldSymbol)semanticModel_.GetSymbolInfo(subpattern.NameColon.Name).Symbol;
            Contract.Assert(fieldSymbol.ContainingType.IsTupleType);
            int elementIndex = fieldSymbol.GetTupleElementIndex();
            left = deconstruct.Variables[subpatternIndex];
          }
          subpatternExpressions.Add(left.EqualsEquals(expression));
        } else if (!subpattern.Pattern.IsKind(SyntaxKind.DiscardPattern)) {
          CheckSwitchDeconstruct(ref deconstruct, governingIdentifier, recursivePattern.Type ?? governingExpression, subpatterns.Count);
          var variable = deconstruct.Variables[subpatternIndex];
          subpatternExpressions.Add(variable.EqualsEquals(expression));
        }
      }
      ++subpatternIndex;
      LuaExpressionSyntax condition;
      if (subpatternExpressions.Count > 0) {
        condition = subpatternExpressions.Aggregate((x, y) => x.And(y));
      } else {
        condition = governingIdentifier.NotEquals(LuaIdentifierNameSyntax.Nil);
      }
      if (recursivePattern.Type != null) {
        var isExpression = BuildIsPatternExpression(governingExpression, recursivePattern.Type, governingIdentifier);
        if (isExpression != LuaIdentifierLiteralExpressionSyntax.True) {
          condition = isExpression.And(condition);
        }
      }
      return condition;
    }

    public override LuaSyntaxNode VisitSwitchExpression(SwitchExpressionSyntax node) {
      const string kNewSwitchExpressionException = "System.SwitchExpressionException()";

      var result = GetTempIdentifier();
      CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(result));

      LuaIdentifierNameSyntax governingIdentifier;
      LuaLocalVariablesSyntax deconstruct = null;
      var governingExpression = node.GoverningExpression.AcceptExpression(this);
      if (node.GoverningExpression.IsKind(SyntaxKind.TupleExpression)) {
        governingIdentifier = null;
        var invocation = (LuaInvocationExpressionSyntax)governingExpression;
        deconstruct = new LuaLocalVariablesSyntax();
        for (int i = 0; i < invocation.ArgumentList.Arguments.Count; ++i) {
          deconstruct.Variables.Add(GetTempIdentifier());
        }
        deconstruct.Initializer = new LuaEqualsValueClauseListSyntax();
        deconstruct.Initializer.Values.AddRange(invocation.ArgumentList.Arguments);
      } else {
        governingIdentifier = GetTempIdentifier();
        CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(governingIdentifier, governingExpression));
      }

      LuaIfStatementSyntax ifStatement = null;
      foreach (var arm in node.Arms) {
        switch (arm.Pattern.Kind()) {
          case SyntaxKind.ConstantPattern: {
            var patternExpression = arm.Pattern.AcceptExpression(this);
            var condition = governingIdentifier.EqualsEquals(patternExpression);
            FillSwitchPatternSyntax(ref ifStatement, condition, arm.WhenClause, result, arm.Expression);
            break;
          }
          case SyntaxKind.RecursivePattern: {
            var recursivePattern = (RecursivePatternSyntax)arm.Pattern;
            if (recursivePattern.Designation != null) {
              AddLocalVariableMapping(governingIdentifier, recursivePattern.Designation);
            }
            var condition = BuildRecursivePatternExpression(recursivePattern, governingIdentifier, deconstruct, node.GoverningExpression);
            FillSwitchPatternSyntax(ref ifStatement, condition, arm.WhenClause, result, arm.Expression);
            break;
          }
          case SyntaxKind.DeclarationPattern: {
            var declarationPattern = (DeclarationPatternSyntax)arm.Pattern;
            var condition = BuildDeclarationPattern(declarationPattern, governingIdentifier, node.GoverningExpression, arm.WhenClause);
            FillSwitchPatternSyntax(ref ifStatement, condition, null, result, arm.Expression);
            break;
          }
          case SyntaxKind.VarPattern: {
            var varPatternSyntax = (VarPatternSyntax)arm.Pattern;
            var parenthesizedVariable = (ParenthesizedVariableDesignationSyntax)varPatternSyntax.Designation;
            int variableIndex = 0;
            foreach (var variable in parenthesizedVariable.Variables) {
              if (!variable.IsKind(SyntaxKind.DiscardDesignation)) {
                CheckSwitchDeconstruct(ref deconstruct, governingIdentifier, node.GoverningExpression, parenthesizedVariable.Variables.Count);
                var variableName = deconstruct.Variables[variableIndex];
                AddLocalVariableMapping(variableName, variable);
              }
              ++variableIndex;
            }
            FillSwitchPatternSyntax(ref ifStatement, null, arm.WhenClause, result, arm.Expression);
            break;
          }
          case SyntaxKind.DiscardPattern: {
            var elseClause = new LuaElseClauseSyntax();
            PushBlock(elseClause.Body);
            var rightExpression = arm.Expression.AcceptExpression(this);
            PopBlock();
            elseClause.Body.AddStatement(result.Assignment(rightExpression));
            ifStatement.Else = elseClause;
            break;
          }
          default: {
            throw new NotImplementedException();
          }
        }
      }

      if (ifStatement.Else == null) {
        var elseClause = new LuaElseClauseSyntax();
        elseClause.Body.AddStatement(LuaIdentifierNameSyntax.Throw.Invocation(kNewSwitchExpressionException));
        ifStatement.Else = elseClause;
      }

      if (deconstruct != null) {
        CurBlock.AddStatement(deconstruct);
      }

      CurBlock.AddStatement(ifStatement);
      return result;
    }

#endregion

    private bool IsParnetTryStatement(SyntaxNode node) {
      bool isTry = false;
      FindParent(node, i => {
        var kind = i.Kind();
        switch (kind) {
          case SyntaxKind.WhileStatement:
          case SyntaxKind.DoStatement:
          case SyntaxKind.ForStatement:
          case SyntaxKind.ForEachStatement:
          case SyntaxKind.SwitchStatement:
            return true;

          case SyntaxKind.TryStatement:
          case SyntaxKind.UsingStatement:
            isTry = true;
            return true;
        }
        return false;
      });
      return isTry;
    }

    private bool CheckBreakLastBlockStatement(BreakStatementSyntax node) {
      if (IsLuaClassic) {
        switch (node.Parent.Kind()) {
          case SyntaxKind.Block: {
            var block = (BlockSyntax)node.Parent;
            return block.Statements.Last() != node;
          }
          case SyntaxKind.SwitchSection: {
            var switchSection = (SwitchSectionSyntax)node.Parent;
            return switchSection.Statements.Last() != node;
          }
        }
      }
      return false;
    }

    public override LuaSyntaxNode VisitBreakStatement(BreakStatementSyntax node) {
      if (IsParnetTryStatement(node)) {
        var check = (LuaCheckLoopControlExpressionSyntax)CurFunction;
        check.HasBreak = true;
        return new LuaReturnStatementSyntax(LuaIdentifierLiteralExpressionSyntax.False);
      }

      if (CheckBreakLastBlockStatement(node)) {
        var blockStatement = new LuaBlockStatementSyntax();
        blockStatement.Statements.Add(LuaBreakStatementSyntax.Instance);
        return blockStatement;
      }

      return LuaBreakStatementSyntax.Instance;
    }

    private LuaExpressionSyntax BuildEnumToStringExpression(ITypeSymbol typeInfo, bool isNullable, LuaExpressionSyntax original, ExpressionSyntax node) {
      if (original is LuaLiteralExpressionSyntax) {
        var symbol = semanticModel_.GetSymbolInfo(node).Symbol;
        return new LuaConstLiteralExpression(new LuaStringLiteralExpressionSyntax(symbol.Name), typeInfo.ToString());
      }

      AddExportEnum(typeInfo);
      var typeName = GetTypeShortName(typeInfo);
      if (IsPreventDebug || isNullable) {
        return LuaIdentifierNameSyntax.System.MemberAccess(LuaIdentifierNameSyntax.EnumToString).Invocation(original, typeName);
      } else {
        return original.MemberAccess(LuaIdentifierNameSyntax.EnumToString, true).Invocation(typeName);
      }
    }

    private LuaExpressionSyntax WrapStringConcatExpression(ExpressionSyntax expression) {
      ITypeSymbol typeInfo = semanticModel_.GetTypeInfo(expression).Type;
      var original = expression.AcceptExpression(this);
      if (typeInfo.IsStringType()) {
        if (IsPreventDebug && !expression.IsKind(SyntaxKind.AddExpression) && !expression.IsKind(SyntaxKind.StringLiteralExpression)) {
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemToString, original);
        }
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
        if (IsPreventDebug && typeInfo.SpecialType == SpecialType.System_Boolean) {
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemToString, original);
        }
        return original;
      } else if (typeInfo.IsEnumType(out var enumTypeSymbol, out bool isNullable)) {
        return BuildEnumToStringExpression(enumTypeSymbol, isNullable, original, expression);
      } else if (typeInfo.IsValueType) {
        return original.MemberAccess(LuaIdentifierNameSyntax.ToStr, true).Invocation();
      } else {
        return LuaIdentifierNameSyntax.SystemToString.Invocation(original);
      }
    }

    private LuaExpressionSyntax BuildStringConcatExpression(BinaryExpressionSyntax node) {
      return BuildStringConcatExpression(node.Left, node.Right);
    }

    private LuaExpressionSyntax BuildStringConcatExpression(ExpressionSyntax leftNode, ExpressionSyntax rightNode) {
      var left = WrapStringConcatExpression(leftNode);
      var right = WrapStringConcatExpression(rightNode);
      return left.Binary(LuaSyntaxNode.Tokens.Concatenation, right);
    }

    private LuaExpressionSyntax BuildBinaryInvokeExpression(BinaryExpressionSyntax node, LuaExpressionSyntax name) {
      var left = node.Left.AcceptExpression(this);
      var right = node.Right.AcceptExpression(this);
      return new LuaInvocationExpressionSyntax(name, left, right);
    }

    private LuaBinaryExpressionSyntax BuildBinaryExpression(BinaryExpressionSyntax node, string operatorToken) {
      var left = VisitExpression(node.Left);
      var right = VisitExpression(node.Right);
      return left.Binary(operatorToken, right);
    }

    private bool IsNullableType(BinaryExpressionSyntax node, out bool isLeftNullable, out bool isRightNullable) {
      var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
      var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
      isLeftNullable = leftType != null && leftType.IsNullableType();
      isRightNullable = rightType != null && rightType.IsNullableType();
      return isLeftNullable || isRightNullable;
    }

    private bool IsNullableType(BinaryExpressionSyntax node) => IsNullableType(node, out _, out _);

    private LuaExpressionSyntax BuildBoolXorOfNullExpression(BinaryExpressionSyntax node, bool isLeftNullable, bool isRightNullable) {
      var left = VisitExpression(node.Left);
      var right = VisitExpression(node.Right);
      if (left.IsNil() || right.IsNil()) {
        return new LuaConstLiteralExpression(LuaIdentifierLiteralExpressionSyntax.Nil, node.ToString());
      }

      var temp = GetTempIdentifier();
      CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp));
      var identifiers = BuildNumberNullableIdentifiers(ref left, ref right, isLeftNullable, isRightNullable);
      LuaExpressionSyntax condition;
      if (identifiers.Count == 1) {
        condition = identifiers.First().NotEquals(LuaIdentifierNameSyntax.Nil);
      } else {
        condition = left.NotEquals(LuaIdentifierNameSyntax.Nil).And(right.NotEquals(LuaIdentifierNameSyntax.Nil));
      }

      var ifStatement = new LuaIfStatementSyntax(condition);
      ifStatement.Body.AddStatement(temp.Assignment(left.NotEquals(right)));
      CurBlock.AddStatement(ifStatement);
      return temp;
    }

    private LuaExpressionSyntax BuildLeftNullableBoolLogicExpression(BinaryExpressionSyntax node, string boolOperatorToken) {
      var left = VisitExpression(node.Left);
      var right = VisitExpression(node.Right);
      var temp = GetTempIdentifier();
      CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp));
      var identifier = BuildNullableExpressionIdentifier(left, new List<LuaExpressionSyntax>());
      var ifStatement = new LuaIfStatementSyntax(identifier.EqualsEquals(LuaIdentifierNameSyntax.Nil));
      ifStatement.Body.AddStatement(temp.Assignment(right.Binary(boolOperatorToken, identifier)));
      ifStatement.Else = new LuaElseClauseSyntax();
      ifStatement.Else.Body.AddStatement(temp.Assignment(identifier.Binary(boolOperatorToken, right)));
      CurBlock.AddStatement(ifStatement);
      return temp;
    }

    private LuaExpressionSyntax BuildBitExpression(BinaryExpressionSyntax node, string boolOperatorToken, LuaIdentifierNameSyntax bitMethodName) {
      if (semanticModel_.GetSymbolInfo(node).Symbol is IMethodSymbol methodSymbol) {
        var containingType = methodSymbol.ContainingType;
        if (containingType != null) {
          if (containingType.IsBoolType(false)) {
            switch (node.Kind()) {
              case SyntaxKind.ExclusiveOrExpression: {
                if (IsNullableType(node, out bool isLeftNullable, out bool isRightNullable)) {
                  return BuildBoolXorOfNullExpression(node, isLeftNullable, isRightNullable);
                }
                break;
              }
              case SyntaxKind.BitwiseOrExpression:
              case SyntaxKind.BitwiseAndExpression: {
                if (IsNullableType(node, out bool isLeftNullable, out _) && isLeftNullable) {
                  return BuildLeftNullableBoolLogicExpression(node, boolOperatorToken);
                }
                break;
              }
            }
            return BuildBinaryExpression(node, boolOperatorToken);
          }

          if (containingType.IsIntegerType(false) || containingType.TypeKind == TypeKind.Enum) {
            if (IsLuaClassic) {
              if (IsNullableBinaryExpression(node, null, out var result, bitMethodName)) {
                return result;
              }

              return BuildBinaryInvokeExpression(node, bitMethodName);
            } else if (IsPreventDebug && IsNullableBinaryExpression(node, null, out var result, bitMethodName)) {
              return result;
            }
          }
        }
      }
      return null;
    }

    private LuaExpressionSyntax BuildLogicOrBinaryExpression(BinaryExpressionSyntax node) {
      var left = VisitExpression(node.Left);
      LuaBlockSyntax rightBody = new LuaBlockSyntax();
      PushBlock(rightBody);
      var right = VisitExpression(node.Right);
      if (rightBody.Statements.Count == 0) {
        PopBlock();
        return left.Or(right);
      } else {
        var temp = GetTempIdentifier();
        PopBlock();
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp));
        LuaIfStatementSyntax leftIfStatement = new LuaIfStatementSyntax(left);
        CurBlock.Statements.Add(leftIfStatement);
        leftIfStatement.Body.AddStatement(temp.Assignment(LuaIdentifierNameSyntax.True));
        leftIfStatement.Else = new LuaElseClauseSyntax();
        leftIfStatement.Else.Body.Statements.AddRange(rightBody.Statements);
        LuaIfStatementSyntax rightIfStatement = new LuaIfStatementSyntax(right);
        leftIfStatement.Else.Body.AddStatement(rightIfStatement);
        rightIfStatement.Body.AddStatement(temp.Assignment(LuaIdentifierNameSyntax.True));
        return temp;
      }
    }

    private LuaExpressionSyntax BuildLogicAndBinaryExpression(BinaryExpressionSyntax node) {
      var left = VisitExpression(node.Left);
      LuaBlockSyntax rightBody = new LuaBlockSyntax();
      PushBlock(rightBody);
      var right = VisitExpression(node.Right);
      if (rightBody.Statements.Count == 0) {
        PopBlock();
        return left.And(right);
      } else {
        var temp = GetTempIdentifier();
        PopBlock();
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp));
        LuaIfStatementSyntax leftIfStatement = new LuaIfStatementSyntax(left);
        CurBlock.Statements.Add(leftIfStatement);
        leftIfStatement.Body.Statements.AddRange(rightBody.Statements);
        LuaIfStatementSyntax rightIfStatement = new LuaIfStatementSyntax(right);
        leftIfStatement.Body.AddStatement(rightIfStatement);
        rightIfStatement.Body.AddStatement(temp.Assignment(LuaIdentifierNameSyntax.True));
        return temp;
      }
    }

    private bool IsNullableBinaryExpression(BinaryExpressionSyntax node, string operatorToken, out LuaExpressionSyntax result, LuaIdentifierNameSyntax method = null) {
      if (IsNullableType(node, out bool isLeftNullable, out bool isRightNullable)) {
        var left = node.Left.AcceptExpression(this);
        var right = node.Right.AcceptExpression(this);
        result = BuildNumberNullableExpression(left, right, operatorToken, isLeftNullable, isRightNullable, method);
        return true;
      }
      result = null;
      return false;
    }

    private bool IsNumberNullableBinaryExpression(INamedTypeSymbol containingType, BinaryExpressionSyntax node, string operatorToken, out LuaExpressionSyntax result, LuaIdentifierNameSyntax method = null) {
      if (containingType.IsNumberType(false)) {
        if (IsNullableBinaryExpression(node, operatorToken, out result, method)) {
          return true;
        }
      }
      result = null;
      return false;
    }

    private LuaExpressionSyntax BuildNumberBinaryExpression(BinaryExpressionSyntax node, SyntaxKind kind) {
      if (semanticModel_.GetSymbolInfo(node).Symbol is IMethodSymbol methodSymbol) {
        var containingType = methodSymbol.ContainingType;
        if (containingType != null) {
          if (kind == SyntaxKind.AddExpression && containingType.IsStringType()) {
            return BuildStringConcatExpression(node);
          }

          if (IsPreventDebug) {
            switch (kind) {
              case SyntaxKind.AddExpression:
              case SyntaxKind.SubtractExpression: {
                bool isPlus = node.IsKind(SyntaxKind.AddExpression);
                if (containingType.IsDelegateType()) {
                  return BuildBinaryInvokeExpression(node, isPlus ? LuaIdentifierNameSyntax.DelegateCombine : LuaIdentifierNameSyntax.DelegateRemove);
                }

                if (IsNumberNullableBinaryExpression(containingType, node, isPlus ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub, out var result)) {
                  return result;
                }
                break;
              }
              case SyntaxKind.MultiplyExpression: {
                if (IsNumberNullableBinaryExpression(containingType, node, LuaSyntaxNode.Tokens.Multiply, out var result)) {
                  return result;
                }
                break;
              }
              case SyntaxKind.LessThanExpression:
              case SyntaxKind.LessThanOrEqualExpression:
              case SyntaxKind.GreaterThanExpression:
              case SyntaxKind.GreaterThanOrEqualExpression: {
                var operatorToken = GetOperatorToken(node.OperatorToken);
                if (IsNumberNullableBinaryExpression(containingType, node, operatorToken, out var result)) {
                  return result.Or(LuaIdentifierNameSyntax.False);
                }
                break;
              }
            }
          }

          switch (kind) {
            case SyntaxKind.DivideExpression: {
              if (IsPreventDebug && containingType.IsDoubleOrFloatType(false)) {
                if (IsNullableBinaryExpression(node, LuaSyntaxNode.Tokens.Div, out var result)) {
                  return result;
                }
              }

              if (containingType.IsIntegerType(false)) {
                if (IsNullableType(node)) {
                  if (IsLuaClassic || IsPreventDebug) {
                    bool success = IsNullableBinaryExpression(node, null, out var result, LuaIdentifierNameSyntax.IntegerDiv);
                    Contract.Assert(success);
                    return result;
                  }
                }

                return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.IntegerDiv);
              }
              break;
            }
            case SyntaxKind.ModuloExpression: {
              if (containingType.IsNumberType(false)) {
                if (IsLuaClassic) {
                  var method = containingType.IsIntegerType(false) ? LuaIdentifierNameSyntax.Mod : LuaIdentifierNameSyntax.ModFloat;
                  if (IsNullableBinaryExpression(node, null, out var result, method)) {
                    return result;
                  }

                  return BuildBinaryInvokeExpression(node, method);
                } else {
                  if (IsPreventDebug && IsNullableBinaryExpression(node, null, out var result, LuaIdentifierNameSyntax.Mod)) {
                    return result;
                  }

                  return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.Mod);
                }
              }
              break;
            }
            case SyntaxKind.LeftShiftExpression:
            case SyntaxKind.RightShiftExpression: {
              if (containingType.IsIntegerType(false)) {
                bool isLeftShift = kind == SyntaxKind.LeftShiftExpression;
                string operatorToken = isLeftShift ? LuaSyntaxNode.Tokens.ShiftLeft : LuaSyntaxNode.Tokens.ShiftRight;
                if (IsLuaClassic) {
                  var method = isLeftShift ? LuaIdentifierNameSyntax.ShiftLeft : LuaIdentifierNameSyntax.ShiftRight;
                  if (IsNullableBinaryExpression(node, operatorToken, out var result, method)) {
                    return result;
                  }

                  return BuildBinaryInvokeExpression(node, method);
                } else if (IsPreventDebug && IsNullableBinaryExpression(node, operatorToken, out var result)) {
                  return result;
                }
              }
              break;
            }
          }
        }
      }

      return null;
    }

    public override LuaSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
      LuaExpressionSyntax resultExpression = GetConstExpression(node);
      if (resultExpression != null) {
        return resultExpression;
      }

      var kind = node.Kind();
      switch (kind) {
        case SyntaxKind.AddExpression:
        case SyntaxKind.SubtractExpression:
        case SyntaxKind.MultiplyExpression:
        case SyntaxKind.DivideExpression:
        case SyntaxKind.ModuloExpression:
        case SyntaxKind.LeftShiftExpression:
        case SyntaxKind.RightShiftExpression:
        case SyntaxKind.LessThanExpression:
        case SyntaxKind.LessThanOrEqualExpression:
        case SyntaxKind.GreaterThanExpression:
        case SyntaxKind.GreaterThanOrEqualExpression:
          resultExpression = BuildNumberBinaryExpression(node, kind);
          break;

        case SyntaxKind.BitwiseOrExpression: {
          resultExpression = BuildBitExpression(node, LuaSyntaxNode.Tokens.Or, LuaIdentifierNameSyntax.BitOr);
          break;
        }
        case SyntaxKind.BitwiseAndExpression: {
          resultExpression = BuildBitExpression(node, LuaSyntaxNode.Tokens.And, LuaIdentifierNameSyntax.BitAnd);
          break;
        }
        case SyntaxKind.ExclusiveOrExpression: {
          resultExpression = BuildBitExpression(node, LuaSyntaxNode.Tokens.NotEquals, LuaIdentifierNameSyntax.BitXor);
          break;
        }
        case SyntaxKind.IsExpression: {
          var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
          if (rightType.IsNullableType(out var nullElementType)) {
            var left = node.Left.AcceptExpression(this);
            var right = GetTypeName(nullElementType);
            return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Is, left, right);
          }

          var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
          if (leftType.Is(rightType)) {
            if (leftType.IsValueType) {
              return LuaIdentifierLiteralExpressionSyntax.True;
            } else {
              return node.Left.AcceptExpression(this).NotEquals(LuaIdentifierNameSyntax.Nil);
            }
          }

          var constValue = semanticModel_.GetConstantValue(node.Right);
          if (constValue.HasValue) {
            var leftExpression = node.Left.AcceptExpression(this);
            return BuildIsConstantExpression(leftExpression, node.Right, constValue);
          }

          return BuildBinaryInvokeExpression(node, LuaIdentifierNameSyntax.Is);
        }
        case SyntaxKind.AsExpression: {
          if (node.Left.IsKind(SyntaxKind.NullLiteralExpression)) {
            return LuaIdentifierLiteralExpressionSyntax.Nil;
          }

          var leftType = semanticModel_.GetTypeInfo(node.Left).Type;
          var rightType = semanticModel_.GetTypeInfo(node.Right).Type;
          if (leftType.Is(rightType)) {
            return node.Left.Accept(this);
          }

          var left = node.Left.AcceptExpression(this);
          var right = node.Right.AcceptExpression(this);
          if (rightType.IsNullableType()) {
            right = ((LuaInvocationExpressionSyntax)right).Arguments.First();
          }
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.As, left, right);
        }
        case SyntaxKind.LogicalOrExpression: {
          return BuildLogicOrBinaryExpression(node);
        }
        case SyntaxKind.LogicalAndExpression: {
          return BuildLogicAndBinaryExpression(node);
        }
        case SyntaxKind.CoalesceExpression: {
          var left = node.Left.AcceptExpression(this);
          var temp = GetTempIdentifier();
          var block = new LuaBlockSyntax();
          PushBlock(block);
          var right = node.Right.AcceptExpression(this);
          PopBlock();
          if (block.Statements.Count == 0) {
            var typeSymbol = semanticModel_.GetTypeInfo(node.Left).Type;
            bool isBool = typeSymbol != null && typeSymbol.IsBoolType();
            if (!isBool) {
              AddReleaseTempIdentifier(temp);
              return left.Binary(GetOperatorToken(node.OperatorToken), right);
            }
          }

          CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp, left));
          var ifStatement = new LuaIfStatementSyntax(temp.EqualsEquals(LuaIdentifierNameSyntax.Nil));
          ifStatement.Body.AddStatements(block.Statements);
          ifStatement.Body.AddStatement(temp.Assignment(right));
          CurBlock.AddStatement(ifStatement);
          return temp;
        }
      }

      if (resultExpression != null) {
        return resultExpression;
      }

      if (IsUserDefinedOperator(node, out var methodSymbol)) {
        if (IsNullableTypeUserDefinedOperator(node, methodSymbol, out var result)) {
          return result;
        }
        return GetUserDefinedOperatorExpression(methodSymbol, node.Left, node.Right);
      }

      string operatorToken = GetOperatorToken(node.OperatorToken);
      return BuildBinaryExpression(node, operatorToken);
    }

    private bool IsNullableTypeUserDefinedOperator(BinaryExpressionSyntax node, IMethodSymbol methodSymbol, out LuaExpressionSyntax result) {
      if (IsNullableType(node, out bool isLeftNullable, out bool isRightNullable)) {
        var arguments = new List<Func<LuaExpressionSyntax>>();
        var identifiers = new List<LuaExpressionSyntax>();
        if (isLeftNullable) {
          if (isRightNullable) {
            arguments.Add(() => BuildNullableExpressionIdentifier(node.Left, identifiers));
            arguments.Add(() => BuildNullableExpressionIdentifier(node.Right, identifiers));
          } else {
            arguments.Add(() => BuildNullableExpressionIdentifier(node.Left, identifiers));
            arguments.Add(() => VisitExpression(node.Right));
          }
        } else {
          arguments.Add(() => VisitExpression(node.Left));
          arguments.Add(() => BuildNullableExpressionIdentifier(node.Right, identifiers));
        }
        var operatorExpression = GetUserDefinedOperatorExpression(methodSymbol, arguments);
        switch (node.Kind()) {
          case SyntaxKind.EqualsExpression:
          case SyntaxKind.NotEqualsExpression: {
            var prevIdentifiers = identifiers.ToList();
            TransformIdentifiersForCompareExpression(identifiers);
            var temp = GetTempIdentifier();
            CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp));
            var ifStatement = new LuaIfStatementSyntax(identifiers.Aggregate((x, y) => x.And(y)));
            ifStatement.Body.AddStatement(temp.Assignment(operatorExpression));
            ifStatement.Else = new LuaElseClauseSyntax();
            LuaExpressionSyntax right;
            if (node.IsKind(SyntaxKind.EqualsExpression)) {
              if (identifiers.Count == 1) {
                right = LuaIdentifierLiteralExpressionSyntax.False;
              } else {
                Contract.Assert(identifiers.Count == 2);
                right = prevIdentifiers.First().EqualsEquals(prevIdentifiers.Last());
              }
            } else {
              if (identifiers.Count == 1) {
                right = LuaIdentifierLiteralExpressionSyntax.True;
              } else {
                Contract.Assert(identifiers.Count == 2);
                right = prevIdentifiers.First().NotEquals(prevIdentifiers.Last());
              }
            }
            ifStatement.Else.Body.AddStatement(temp.Assignment(right));
            CurBlock.AddStatement(ifStatement);
            result = temp;
            return true;
          }
          case SyntaxKind.LessThanExpression:
          case SyntaxKind.LessThanOrEqualExpression:
          case SyntaxKind.GreaterThanExpression:
          case SyntaxKind.GreaterThanOrEqualExpression: {
            TransformIdentifiersForCompareExpression(identifiers);
            break;
          }
        }
        result = identifiers.Aggregate((x, y) => x.And(y)).And(operatorExpression);
        return true;
      }

      result = null;
      return false;
    }

    private LuaExpressionSyntax BuildNullableExpressionIdentifier(ExpressionSyntax node, List<LuaExpressionSyntax> identifiers) {
      var expression = node.AcceptExpression(this);
      return BuildNullableExpressionIdentifier(expression, identifiers);
    }

    private LuaIdentifierNameSyntax BuildNullableExpressionIdentifier(LuaExpressionSyntax expression, List<LuaExpressionSyntax> identifiers) {
      if (expression is LuaIdentifierNameSyntax identifierName) {
        identifiers.Add(identifierName);
        return identifierName;
      } else {
        var temp = GetTempIdentifier();
        CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp, expression));
        identifiers.Add(temp);
        return temp;
      }
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
        var methodSymbol = op_Implicits.FirstOrDefault(i => isAddOrAssignment ? i.ReturnType.IsIntegerType() : i.ReturnType.EQ(symbol));
        if (methodSymbol != null) {
          expression = BuildConversionExpression(methodSymbol, expression);
        }
      }
    }

    private LuaSyntaxNode BuildPrefixUnaryExpression(bool isSingleLine, string operatorToken, LuaExpressionSyntax operand, PrefixUnaryExpressionSyntax node, bool isLocalVar = false) {
      var left = operand;
      ChecktIncrementExpression(node.Operand, ref left, true);
      LuaExpressionSyntax binary = left.Binary(operatorToken, LuaIdentifierNameSyntax.One);
      ChecktIncrementExpression(node.Operand, ref binary, false);
      if (isSingleLine) {
        return operand.Assignment(binary);
      } else {
        if (isLocalVar) {
          CurBlock.Statements.Add(operand.Assignment(binary));
          return operand;
        } else {
          var temp = GetTempIdentifier();
          CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, binary));
          CurBlock.Statements.Add(operand.Assignment(temp));
          return temp;
        }
      }
    }

    private LuaSyntaxNode BuildPropertyPrefixUnaryExpression(bool isSingleLine, string operatorToken, LuaPropertyAdapterExpressionSyntax get, LuaPropertyAdapterExpressionSyntax set, PrefixUnaryExpressionSyntax node) {
      set.IsGetOrAdd = false;
      LuaExpressionSyntax left = get;
      ChecktIncrementExpression(node.Operand, ref left, true);
      LuaExpressionSyntax binary = left.Binary(operatorToken, LuaIdentifierNameSyntax.One);
      ChecktIncrementExpression(node.Operand, ref binary, false);
      if (isSingleLine) {
        set.ArgumentList.AddArgument(binary);
        return set;
      } else {
        var temp = GetTempIdentifier();
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, binary));
        set.ArgumentList.AddArgument(temp);
        CurBlock.Statements.Add(set);
        return temp;
      }
    }

    private LuaMemberAccessExpressionSyntax GetTempUnaryExpression(LuaMemberAccessExpressionSyntax memberAccess, out LuaLocalVariableDeclaratorSyntax localTemp) {
      var temp = GetTempIdentifier();
      localTemp = new LuaLocalVariableDeclaratorSyntax(temp, memberAccess.Expression);
      return temp.MemberAccess(memberAccess.Name, memberAccess.IsObjectColon);
    }

    private LuaPropertyAdapterExpressionSyntax GetTempPropertyUnaryExpression(LuaPropertyAdapterExpressionSyntax propertyAdapter, out LuaLocalVariableDeclaratorSyntax localTemp) {
      var temp = GetTempIdentifier();
      localTemp = new LuaLocalVariableDeclaratorSyntax(temp, propertyAdapter.Expression);
      return new LuaPropertyAdapterExpressionSyntax(temp, propertyAdapter.Name, propertyAdapter.IsObjectColon);
    }

    private bool IsNullablePrefixUnaryExpression(PrefixUnaryExpressionSyntax node, LuaExpressionSyntax operand, string operatorToken, out LuaExpressionSyntax result, LuaIdentifierNameSyntax method = null) {
      var type = semanticModel_.GetTypeInfo(node.Operand).Type;
      if (type.IsNullableType()) {
        var identifier = BuildNullableExpressionIdentifier(operand, new List<LuaExpressionSyntax>());
        if (operatorToken != null) {
          result = identifier.And(new LuaPrefixUnaryExpressionSyntax(identifier, operatorToken));
        } else {
          result = identifier.And(new LuaInvocationExpressionSyntax(method, identifier));
        }
        return true;
      }
      result = null;
      return false;
    }

    public override LuaSyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node) {
      SyntaxKind kind = node.Kind();
      if (kind == SyntaxKind.IndexExpression) {
        var expression = VisitExpression(node.Operand);
        var v = semanticModel_.GetConstantValue(node.Operand);
        if (v.HasValue && (int)v.Value > 0) {
          return new LuaPrefixUnaryExpressionSyntax(expression, LuaSyntaxNode.Tokens.Sub);
        }
        return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Index, expression, LuaIdentifierNameSyntax.True);
      }

      if (IsUserDefinedOperator(node, out var methodSymbol)) {
        var type = semanticModel_.GetTypeInfo(node.Operand).Type;
        if (type != null && type.IsNullableType()) {
          var arguments = new List<Func<LuaExpressionSyntax>>();
          var identifiers = new List<LuaExpressionSyntax>();
          arguments.Add(() => BuildNullableExpressionIdentifier(node.Operand, identifiers));
          var operatorExpression = GetUserDefinedOperatorExpression(methodSymbol, arguments);
          return identifiers.First().And(operatorExpression);
        }
        return GetUserDefinedOperatorExpression(methodSymbol, node.Operand);
      }

      var operand = VisitExpression(node.Operand);
      switch (kind) {
        case SyntaxKind.PreIncrementExpression:
        case SyntaxKind.PreDecrementExpression: {
          bool isSingleLine = IsSingleLineUnary(node);
          string operatorToken = kind == SyntaxKind.PreIncrementExpression ? LuaSyntaxNode.Tokens.Plus : LuaSyntaxNode.Tokens.Sub;
          if (operand is LuaMemberAccessExpressionSyntax memberAccess) {
            if (memberAccess.Expression != LuaIdentifierNameSyntax.This) {
              memberAccess = GetTempUnaryExpression(memberAccess, out var localTemp);
              CurBlock.Statements.Add(localTemp);
            }
            return BuildPrefixUnaryExpression(isSingleLine, operatorToken, memberAccess, node);
          } else if (operand is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
            if (propertyAdapter.Expression != null) {
              var getAdapter = GetTempPropertyUnaryExpression(propertyAdapter, out var localTemp);
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
        case SyntaxKind.BitwiseNotExpression: {
          if (IsLuaClassic) {
            if (IsNullablePrefixUnaryExpression(node, operand, null, out var result, LuaIdentifierNameSyntax.BitNot)) {
              return result;
            }
            return LuaIdentifierNameSyntax.BitNot.Invocation(operand);
          } else if (IsPreventDebug && IsNullablePrefixUnaryExpression(node, operand, LuaSyntaxNode.Tokens.BitNot, out var result)) {
            return result;
          }
          break;
        }
        case SyntaxKind.UnaryPlusExpression: {
          return operand;
        }
        case SyntaxKind.UnaryMinusExpression: {
          if (operand is LuaLiteralExpressionSyntax e && e.Text == "0") {
            return operand;
          }
          if (IsPreventDebug && IsNullablePrefixUnaryExpression(node, operand, LuaSyntaxNode.Tokens.Sub, out var result)) {
            return result;
          }
          break;
        }
        case SyntaxKind.LogicalNotExpression: {
          var symbol = semanticModel_.GetTypeInfo(node.Operand).Type;
          if (symbol != null && symbol.IsNullableType() && symbol.IsBoolType(true)) {
            var temp = GetTempIdentifier();
            CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp));
            var identifier = BuildNullableExpressionIdentifier(operand, new List<LuaExpressionSyntax>());
            var ifStatement = new LuaIfStatementSyntax(identifier.NotEquals(LuaIdentifierNameSyntax.Nil));
            ifStatement.Body.AddStatement(temp.Assignment(new LuaPrefixUnaryExpressionSyntax(identifier, LuaSyntaxNode.Tokens.Not)));
            CurBlock.AddStatement(ifStatement);
            return temp;
          }
          break;
        }
      }

      var unaryExpression = new LuaPrefixUnaryExpressionSyntax(operand, GetOperatorToken(node.OperatorToken));
      return unaryExpression;
    }

    private LuaSyntaxNode BuildPostfixUnaryExpression(bool isSingleLine, string operatorToken, LuaExpressionSyntax operand, PostfixUnaryExpressionSyntax node) {
      if (isSingleLine) {
        var left = operand;
        ChecktIncrementExpression(node.Operand, ref left, true);
        LuaExpressionSyntax binary = left.Binary(operatorToken, LuaIdentifierNameSyntax.One);
        ChecktIncrementExpression(node.Operand, ref binary, false);
        return operand.Assignment(binary);
      } else {
        var temp = GetTempIdentifier();
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, operand));
        LuaExpressionSyntax left = temp;
        ChecktIncrementExpression(node.Operand, ref left, true);
        LuaExpressionSyntax binary = left.Binary(operatorToken, LuaIdentifierNameSyntax.One);
        ChecktIncrementExpression(node.Operand, ref binary, false);
        CurBlock.Statements.Add(operand.Assignment(binary));
        return temp;
      }
    }

    private LuaSyntaxNode BuildPropertyPostfixUnaryExpression(bool isSingleLine, string operatorToken, LuaPropertyAdapterExpressionSyntax get, LuaPropertyAdapterExpressionSyntax set, PostfixUnaryExpressionSyntax node) {
      set.IsGetOrAdd = false;
      if (isSingleLine) {
        LuaExpressionSyntax left = get;
        ChecktIncrementExpression(node.Operand, ref left, true);
        LuaExpressionSyntax binary = left.Binary(operatorToken, LuaIdentifierNameSyntax.One);
        ChecktIncrementExpression(node.Operand, ref binary, false);
        set.ArgumentList.AddArgument(binary);
        return set;
      } else {
        var temp = GetTempIdentifier();
        CurBlock.Statements.Add(new LuaLocalVariableDeclaratorSyntax(temp, get));
        LuaExpressionSyntax left = temp;
        ChecktIncrementExpression(node.Operand, ref left, true);
        LuaExpressionSyntax binary = left.Binary(operatorToken, LuaIdentifierNameSyntax.One);
        ChecktIncrementExpression(node.Operand, ref binary, false);
        set.ArgumentList.AddArgument(binary);
        CurBlock.AddStatement(set);
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
      var operand = node.Operand.AcceptExpression(this);

      if (operand is LuaMemberAccessExpressionSyntax memberAccess) {
        if (memberAccess.Expression != LuaIdentifierNameSyntax.This) {
          memberAccess = GetTempUnaryExpression(memberAccess, out var localTemp);
          CurBlock.Statements.Add(localTemp);
        }
        return BuildPostfixUnaryExpression(isSingleLine, operatorToken, memberAccess, node);
      } else if (operand is LuaPropertyAdapterExpressionSyntax propertyAdapter) {
        if (propertyAdapter.Expression != null) {
          var getAdapter = GetTempPropertyUnaryExpression(propertyAdapter, out var localTemp);
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
      bool isWithinTry = IsParnetTryStatement(node);
      if (isWithinTry) {
        var check = (LuaCheckLoopControlExpressionSyntax)CurFunction;
        check.HasContinue = true;
      }
      return new LuaContinueAdapterStatementSyntax(isWithinTry);
    }

    private static bool IsLastBreakStatement(LuaStatementSyntax lastStatement) {
      if (lastStatement == LuaBreakStatementSyntax.Instance) {
        return true;
      }

      if (lastStatement is LuaContinueAdapterStatementSyntax) {
        return true;
      }

      if (lastStatement is LuaLabeledStatement labeledStatement && IsLastBreakStatement(labeledStatement.Statement)) {
        return true;
      }

      return false;
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
        bool isLastFinal = lastStatement is LuaBaseReturnStatementSyntax || IsLastBreakStatement(lastStatement);
        if (!isLastFinal) {
          repeatStatement.Body.Statements.Add(continueIdentifier.Assignment(LuaIdentifierNameSyntax.True));
        }
        block.Statements.Add(repeatStatement);
        LuaIfStatementSyntax IfStatement = new LuaIfStatementSyntax(new LuaPrefixUnaryExpressionSyntax(continueIdentifier, LuaSyntaxNode.Tokens.Not));
        IfStatement.Body.Statements.Add(LuaBreakStatementSyntax.Instance);
        block.Statements.Add(IfStatement);
      } else {
        WriteStatementOrBlock(bodyStatement, block);
      }
    }

    private void CheckForeachCast(LuaIdentifierNameSyntax identifier, ForEachStatementSyntax node, LuaForInStatementSyntax forInStatement, bool isAsync) {
      var sourceType = semanticModel_.GetTypeInfo(node.Expression).Type;
      var targetType = semanticModel_.GetTypeInfo(node.Type).Type;
      bool hasCast = false;
      var elementType = !isAsync ? sourceType.GetIEnumerableElementType() : sourceType.GetIAsyncEnumerableElementType();
      if (elementType != null) {
        if (!elementType.EQ(targetType) && !elementType.Is(targetType)) {
          hasCast = true;
        }
      } else {
        if (targetType.SpecialType != SpecialType.System_Object) {
          hasCast = true;
        }
      }
      if (hasCast) {
        var cast = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Cast, GetTypeName(targetType), identifier);
        forInStatement.Body.AddStatement(identifier.Assignment(cast));
      }
    }

    public override LuaSyntaxNode VisitForEachStatement(ForEachStatementSyntax node) {
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
      CheckLocalVariableName(ref identifier, node);
      var expression = node.Expression.AcceptExpression(this);
      bool isAsync = node.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword);
      var forInStatement = new LuaForInStatementSyntax(identifier, expression, isAsync);
      CheckForeachCast(identifier, node, forInStatement, isAsync);
      VisitLoopBody(node.Statement, forInStatement.Body);
      return forInStatement;
    }

    public override LuaSyntaxNode VisitForEachVariableStatement(ForEachVariableStatementSyntax node) {
      var temp = GetTempIdentifier();
      var expression = node.Expression.AcceptExpression(this);
      var forInStatement = new LuaForInStatementSyntax(temp, expression);
      var left = node.Variable.AcceptExpression(this);
      var sourceType = semanticModel_.GetTypeInfo(node.Expression).Type;
      var elementType = sourceType.GetIEnumerableElementType();
      var right = BuildDeconstructExpression(elementType, temp, node.Expression);
      forInStatement.Body.AddStatement(left.Assignment(right));
      VisitLoopBody(node.Statement, forInStatement.Body);
      return forInStatement;
    }

    private LuaWhileStatementSyntax BuildWhileStatement(ExpressionSyntax nodeCondition, StatementSyntax nodeStatement) {
      LuaBlockSyntax conditionBody = new LuaBlockSyntax();
      PushBlock(conditionBody);
      var condition = nodeCondition != null ? VisitExpression(nodeCondition) : LuaIdentifierNameSyntax.True;
      PopBlock();

      LuaWhileStatementSyntax whileStatement;
      if (conditionBody.Statements.Count == 0) {
        whileStatement = new LuaWhileStatementSyntax(condition);
      } else {
        whileStatement = new LuaWhileStatementSyntax(LuaIdentifierNameSyntax.True);
        if (condition is LuaBinaryExpressionSyntax) {
          condition = condition.Parenthesized();
        }
        LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(new LuaPrefixUnaryExpressionSyntax(condition, LuaSyntaxNode.Tokens.Not));
        ifStatement.Body.AddStatement(LuaBreakStatementSyntax.Instance);
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
      PushBlock(forBlock);

      if (node.Declaration != null) {
        forBlock.AddStatement(node.Declaration.Accept<LuaVariableDeclarationSyntax>(this));
      }
      var initializers = node.Initializers.Select(i => i.AcceptExpression(this));
      forBlock.AddStatements(initializers);

      var whileStatement = BuildWhileStatement(node.Condition, node.Statement);
      PushBlock(whileStatement.Body);
      var incrementors = node.Incrementors.Select(i => i.AcceptExpression(this));
      whileStatement.Body.AddStatements(incrementors);
      PopBlock();

      forBlock.Statements.Add(whileStatement);
      PopBlock();

      return forBlock;
    }

    public override LuaSyntaxNode VisitDoStatement(DoStatementSyntax node) {
      LuaBlockSyntax body = new LuaBlockSyntax();
      PushBlock(body);
      VisitLoopBody(node.Statement, body);
      var condition = VisitExpression(node.Condition);
      if (condition is LuaBinaryExpressionSyntax) {
        condition = condition.Parenthesized();
      }
      var newCondition = new LuaPrefixUnaryExpressionSyntax(condition, LuaSyntaxNode.Tokens.Not);
      PopBlock();
      return new LuaRepeatStatementSyntax(newCondition, body);
    }

    public override LuaSyntaxNode VisitYieldStatement(YieldStatementSyntax node) {
      var curMehod = CurMethodInfoOrNull;
      curMehod.HasYield = true;
      if (node.IsKind(SyntaxKind.YieldBreakStatement)) {
        return new LuaReturnStatementSyntax();
      } else {
        string yieldToken = node.YieldKeyword.ValueText;
        var expression = node.Expression.AcceptExpression(this);
        LuaExpressionSyntax targetMethod;
        if (curMehod.Symbol.IsAsync) {
          targetMethod = LuaIdentifierNameSyntax.Async.MemberAccess(yieldToken, true);
        } else {
          targetMethod = LuaIdentifierNameSyntax.System.MemberAccess(yieldToken);
        }
        return new LuaExpressionStatementSyntax(targetMethod.Invocation(expression));
      }
    }

    public override LuaSyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node) {
      var expression = node.Expression.AcceptExpression(this);
      if (expression is LuaIdentifierNameSyntax || expression is LuaMemberAccessExpressionSyntax) {
        return expression;
      }

      CheckPrevIsInvokeStatement(node);
      return expression.Parenthesized();
    }

    /// <summary>
    /// http://lua-users.org/wiki/TernaryOperator
    /// </summary>
    public override LuaSyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node) {
      bool mayBeNullOrFalse = MayBeNullOrFalse(node.WhenTrue);
      if (mayBeNullOrFalse) {
        var temp = GetTempIdentifier();
        var condition = VisitExpression(node.Condition);
        LuaIfStatementSyntax ifStatement = new LuaIfStatementSyntax(condition);
        PushBlock(ifStatement.Body);
        var whenTrue = VisitExpression(node.WhenTrue);
        PopBlock();
        ifStatement.Body.AddStatement(temp.Assignment(whenTrue));

        LuaElseClauseSyntax elseClause = new LuaElseClauseSyntax();
        PushBlock(elseClause.Body);
        var whenFalse = VisitExpression(node.WhenFalse);
        PopBlock();
        elseClause.Body.AddStatement(temp.Assignment(whenFalse));

        ifStatement.Else = elseClause;
        CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp));
        CurBlock.AddStatement(ifStatement);
        return temp;
      } else {
        LuaExpressionSyntax Accept(ExpressionSyntax expressionNode) {
          var expression = VisitExpression(expressionNode);
          return expression is LuaBinaryExpressionSyntax ? expression.Parenthesized() : expression;
        }

        var condition = Accept(node.Condition);
        var whenTrue = Accept(node.WhenTrue);
        var whenFalse = Accept(node.WhenFalse);
        return condition.And(whenTrue).Or(whenFalse);
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
        var identifier = node.Expression.Accept<LuaIdentifierNameSyntax>(this);
        return new LuaGotoStatement(identifier);
      }
    }

    public override LuaSyntaxNode VisitLabeledStatement(LabeledStatementSyntax node) {
      LuaIdentifierNameSyntax identifier = node.Identifier.ValueText;
      var statement = node.Statement.Accept<LuaStatementSyntax>(this);
      return new LuaLabeledStatement(identifier, statement);
    }

    public override LuaSyntaxNode VisitEmptyStatement(EmptyStatementSyntax node) {
      return LuaStatementSyntax.Empty;
    }

    private LuaExpressionSyntax BuildEnumCastExpression(LuaExpressionSyntax expression, ITypeSymbol originalType, ITypeSymbol targetType) {
      if (targetType.TypeKind == TypeKind.Enum) {
        LuaExpressionSyntax result = null;
        var targetEnumUnderlyingType = ((INamedTypeSymbol)targetType).EnumUnderlyingType;
        if (originalType.TypeKind == TypeKind.Enum || originalType.IsCastIntegerType()) {
          var originalIntegerType = originalType.TypeKind == TypeKind.Enum ? ((INamedTypeSymbol)originalType).EnumUnderlyingType : originalType;
          if (targetEnumUnderlyingType.IsNumberTypeAssignableFrom(originalIntegerType)) {
            result = expression;
          } else {
            result = GetCastToNumberExpression(expression, targetEnumUnderlyingType, false);
          }
        } else if (originalType.IsDoubleOrFloatType(false)) {
          result = GetCastToNumberExpression(expression, targetEnumUnderlyingType, true);
        }
        if (result != null) {
          if (!generator_.IsConstantEnum(targetType) && !originalType.EQ(targetType)) {
            result = GetTypeName(targetType).Invocation(expression);
          }
          return result;
        }
      } else if (originalType.TypeKind == TypeKind.Enum) {
        var originalEnumUnderlyingType = ((INamedTypeSymbol)originalType).EnumUnderlyingType;
        if (targetType.IsCastIntegerType()) {
          if (!generator_.IsConstantEnum(originalType)) {
            return GetEnumNoConstantToNumberExpression(expression, targetType);
          }

          if (targetType.IsNumberTypeAssignableFrom(originalEnumUnderlyingType)) {
            return expression;
          }
          return GetCastToNumberExpression(expression, targetType, false);
        } else if (targetType.IsDoubleOrFloatType(false)) {
          return expression;
        }
      }

      return null;
    }

    private LuaExpressionSyntax BuildNumberCastExpression(LuaExpressionSyntax expression, ITypeSymbol originalType, ITypeSymbol targetType) {
      if (targetType.IsCastIntegerType()) {
        if (originalType.IsCastIntegerType()) {
          if (targetType.IsNumberTypeAssignableFrom(originalType)) {
            return expression;
          }
          return GetCastToNumberExpression(expression, targetType, false);
        } else if (originalType.IsDoubleOrFloatType(false)) {
          return GetCastToNumberExpression(expression, targetType, true);
        }
      } else if (originalType.IsCastIntegerType()) {
        if (targetType.IsDoubleOrFloatType(false)) {
          return expression;
        }
      } else if (targetType.SpecialType == SpecialType.System_Single && originalType.SpecialType == SpecialType.System_Double) {
        return GetCastToNumberExpression(expression, targetType, true);
      }
      return null;
    }

    private LuaExpressionSyntax BuildEnumAndNumberCastExpression(LuaExpressionSyntax expression, ITypeSymbol originalType, ITypeSymbol targetType) {
      return BuildEnumCastExpression(expression, originalType, targetType) ?? BuildNumberCastExpression(expression, originalType, targetType);
    }

    private LuaExpressionSyntax BuildNullableCastExpression(LuaExpressionSyntax expression, ITypeSymbol originalType, ITypeSymbol targetType) {
      var targetNullableElemetType = targetType.NullableElemetType();
      var originalNullableElemetType = originalType.NullableElemetType();
      if (targetNullableElemetType != null) {
        if (originalNullableElemetType != null) {
          bool isIdentifier = false;
          LuaIdentifierNameSyntax identifier;
          if (expression is LuaIdentifierNameSyntax identifierName) {
            identifier = identifierName;
            isIdentifier = true;
          } else {
            identifier = GetTempIdentifier();
          }
          var castExpression = BuildEnumAndNumberCastExpression(identifier, originalNullableElemetType, targetNullableElemetType);
          if (castExpression != null) {
            if (castExpression == identifier) {
              return expression;
            }
            if (!isIdentifier) {
              CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(identifier, expression));
            }
            return identifier.And(castExpression);
          }
        } else {
          return BuildEnumAndNumberCastExpression(expression, originalType, targetNullableElemetType);
        }
      } else if (originalNullableElemetType != null) {
        var explicitMethod = (IMethodSymbol)originalType.GetMembers("op_Explicit").First();
        expression = BuildConversionExpression(explicitMethod, expression);
        return BuildEnumAndNumberCastExpression(expression, originalNullableElemetType, targetType);
      }
      return null;
    }

    public override LuaSyntaxNode VisitCastExpression(CastExpressionSyntax node) {
      var constExpression = GetConstExpression(node);
      if (constExpression != null) {
        return constExpression;
      }

      var targetType = semanticModel_.GetTypeInfo(node.Type).Type;
      var expression = node.Expression.AcceptExpression(this);
      if (targetType.SpecialType == SpecialType.System_Object || targetType.Kind == SymbolKind.DynamicType) {
        return expression;
      }

      var originalType = semanticModel_.GetTypeInfo(node.Expression).Type;
      if (originalType == null) {
        Contract.Assert(targetType.IsDelegateType());
        return expression;
      }

      if (originalType.Is(targetType)) {
        return expression;
      }

      var result = BuildEnumAndNumberCastExpression(expression, originalType, targetType);
      if (result != null) {
        return result;
      }

      result = BuildNullableCastExpression(expression, originalType, targetType);
      if (result != null) {
        return result;
      }

      var explicitSymbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
      if (explicitSymbol != null) {
        return BuildConversionExpression(explicitSymbol, expression);
      }

      return BuildCastExpression(targetType, expression);
    }

    private LuaExpressionSyntax BuildCastExpression(ITypeSymbol type, LuaExpressionSyntax expression) {
      var typeExpression = GetTypeName(type.IsNullableType() ? type.NullableElemetType() : type);
      var invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Cast, typeExpression, expression);
      if (type.IsNullableType()) {
        invocation.AddArgument(LuaIdentifierNameSyntax.True);
      }
      return invocation;
    }

    private LuaExpressionSyntax GetEnumNoConstantToNumberExpression(LuaExpressionSyntax expression, ITypeSymbol targetType) {
      string methodName = "System.Convert.To" + targetType.Name;
      return new LuaInvocationExpressionSyntax(methodName, expression);
    }

    private LuaExpressionSyntax GetCastToNumberExpression(LuaExpressionSyntax expression, ITypeSymbol targetType, bool isFromFloat) {
      if (expression is LuaParenthesizedExpressionSyntax parenthesizedExpression) {
        expression = parenthesizedExpression.Expression;
      }
      string name = (isFromFloat ? "To" : "to") + (targetType.SpecialType == SpecialType.System_Char ? "UInt16" : targetType.Name);
      var invocation = LuaIdentifierNameSyntax.System.MemberAccess(name).Invocation(expression);
      if (IsCurChecked) {
        invocation.AddArgument(LuaIdentifierNameSyntax.True);
      }
      return invocation;
    }

    public override LuaSyntaxNode VisitCheckedStatement(CheckedStatementSyntax node) {
      bool isChecked = node.Keyword.Kind() == SyntaxKind.CheckedKeyword;
      PushChecked(isChecked);
      var statements = new LuaStatementListSyntax();
      statements.Statements.Add(new LuaShortCommentStatement(" " + node.Keyword.ValueText));
      var block = node.Block.Accept<LuaStatementSyntax>(this);
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

    public override LuaSyntaxNode VisitNullableType(NullableTypeSyntax node) {
      var elementType = node.ElementType.AcceptExpression(this);
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.NullableType, elementType);
    }
  }
}
