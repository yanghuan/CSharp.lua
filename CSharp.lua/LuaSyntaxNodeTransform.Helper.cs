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
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed partial class LuaSyntaxNodeTransform {
    private const int kMaxArrayInitializerCount = 225;
    private static readonly Regex codeTemplateRegex_ = new Regex(@"(,?\s*)\{(\*?[\w|^]+)\}", RegexOptions.Compiled);
    private Dictionary<ISymbol, LuaIdentifierNameSyntax> localReservedNames_ = new Dictionary<ISymbol, LuaIdentifierNameSyntax>();
    private int localMappingCounter_;
    private Stack<bool> checkeds_ = new Stack<bool>();

    private abstract class LuaSyntaxSearcher : CSharpSyntaxWalker {
      private sealed class FoundException : Exception {
      }

      protected void Found() {
        throw new FoundException();
      }

      public bool Find(SyntaxNode root) {
        try {
          Visit(root);
        } catch (FoundException) {
          return true;
        }
        return false;
      }
    }

    private sealed class LocalVarSearcher : LuaSyntaxSearcher {
      private readonly string name_;

      public LocalVarSearcher(string name) {
        name_ = name;
      }

      public override void VisitParameter(ParameterSyntax node) {
        if (node.Identifier.ValueText == name_) {
          Found();
        }
      }

      public override void VisitVariableDeclarator(VariableDeclaratorSyntax node) {
        if (node.Identifier.ValueText == name_) {
          Found();
        }
      }
    }

    private bool IsLocalVarExists(string name, SyntaxNode root) {
      LocalVarSearcher searcher = new LocalVarSearcher(name);
      return searcher.Find(root);
    }

    private SyntaxNode FindFromCur(SyntaxNode node, Func<SyntaxNode, bool> macth) {
      var cur = node;
      while (cur != null) {
        if (macth(cur)) {
          return cur;
        }
        cur = cur.Parent;
      }
      return null;
    }

    private SyntaxNode FindParent(SyntaxNode node, Func<SyntaxNode, bool> macth) {
      var parent = node.Parent;
      while (true) {
        if (macth(parent)) {
          return parent;
        }
        parent = parent.Parent;
      }
    }

    private SyntaxNode FindParent(SyntaxNode node, SyntaxKind kind) {
      return FindParent(node, i => i.IsKind(kind));
    }

    private T FindParent<T>(SyntaxNode node) where T : CSharpSyntaxNode {
      return (T)FindParent(node, i => i is T);
    }

    private (SyntaxNode node, BlockSyntax body) FindParentMethodDeclaration(SyntaxNode node) {
      BlockSyntax body = null;
      var parnet = FindParent(node, i => {
        switch (i.Kind()) {
          case SyntaxKind.ConstructorDeclaration:
          case SyntaxKind.MethodDeclaration:
          case SyntaxKind.OperatorDeclaration:
          case SyntaxKind.ConversionOperatorDeclaration: {
              var methodDeclaration = (BaseMethodDeclarationSyntax)i;
              body = methodDeclaration.Body;
              return true;
            }
          case SyntaxKind.LocalFunctionStatement: {
              var localFunction = (LocalFunctionStatementSyntax)i;
              body = localFunction.Body;
              return true;
            }
          case SyntaxKind.GetAccessorDeclaration:
          case SyntaxKind.SetAccessorDeclaration: {
              var accessorDeclaration = (AccessorDeclarationSyntax)i;
              body = accessorDeclaration.Body;
              return true;
            }
        }
        return false;
      });
      return (parnet, body);
    }

    private BlockSyntax FindParentMethodBody(SyntaxNode node) {
     return FindParentMethodDeclaration(node).body;
    }
 
    private string GetUniqueIdentifier(string name, SyntaxNode node, int index = 0) {
      var (root, _) = FindParentMethodDeclaration(node);
      while (true) {
        string newName = Utility.GetNewIdentifierName(name, index);
        bool exists = IsLocalVarExists(newName, root);
        if (!exists) {
          return newName;
        }
        ++index;
      }
    }

    private bool CheckLocalBadWord(ref string name, SyntaxNode node) {
      if (LuaSyntaxNode.IsReservedWord(name)) {
        name = GetUniqueIdentifier(name, node, 1);
        return true;
      } else if (Utility.IsIdentifierIllegal(ref name)) {
        name = GetUniqueIdentifier(name, node, 0);
        return true;
      }
      return false;
    }

    private void AddLocalVariableMapping(LuaIdentifierNameSyntax name, SyntaxNode node) {
      ISymbol symbol = semanticModel_.GetDeclaredSymbol(node);
      Contract.Assert(symbol != null);
      localReservedNames_.Add(symbol, name);
    }

    private void CheckLocalVariableName(ref LuaIdentifierNameSyntax identifierName, SyntaxNode node) {
      string name = identifierName.ValueText;
      bool isReserved = CheckLocalBadWord(ref name, node);
      if (isReserved) {
        identifierName = name;
        AddLocalVariableMapping(identifierName, node);
      }
    }

    private void CheckLocalSymbolName(ISymbol symbol, ref LuaIdentifierNameSyntax name) {
      var newName = localReservedNames_.GetOrDefault(symbol);
      if (newName != null) {
        name = newName;
      }
    }

    private sealed class ContinueSearcher : LuaSyntaxSearcher {
      public override void VisitContinueStatement(ContinueStatementSyntax node) {
        Found();
      }
    }

    private bool IsContinueExists(SyntaxNode node) {
      ContinueSearcher searcher = new ContinueSearcher();
      return searcher.Find(node);
    }

    private sealed class ReturnStatementSearcher : LuaSyntaxSearcher {
      public override void VisitReturnStatement(ReturnStatementSyntax node) {
        Found();
      }
    }

    private bool IsReturnExists(SyntaxNode node) {
      ReturnStatementSearcher searcher = new ReturnStatementSearcher();
      return searcher.Find(node);
    }

    private int GetCaseLabelIndex(GotoStatementSyntax node) {
      var switchStatement = (SwitchStatementSyntax)FindParent(node, SyntaxKind.SwitchStatement);
      int index = 0;
      foreach (var section in switchStatement.Sections) {
        bool isFound = section.Labels.Any(i => {
          if (i.IsKind(SyntaxKind.CaseSwitchLabel)) {
            var label = (CaseSwitchLabelSyntax)i;
            if (label.Value.ToString() == node.Expression.ToString()) {
              return true;
            }
          }
          return false;
        });
        if (isFound) {
          return index;
        }
      }
      throw new InvalidOperationException();
    }

    private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression) {
      return InternalBuildCodeTemplateExpression(codeTemplate, targetExpression, null, null);
    }

    private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression, IEnumerable<LuaExpressionSyntax> arguments, IList<ITypeSymbol> typeArguments) {
      return InternalBuildCodeTemplateExpression(codeTemplate, targetExpression, arguments.Select<LuaExpressionSyntax, Func<LuaExpressionSyntax>>(i => () => i), typeArguments);
    }

    private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression, IEnumerable<ExpressionSyntax> arguments, IList<ITypeSymbol> typeArguments) {
      return InternalBuildCodeTemplateExpression(codeTemplate, targetExpression, arguments.Select<ExpressionSyntax, Func<LuaExpressionSyntax>>(i => () => VisitExpression(i)), typeArguments);
    }

    private void AddCodeTemplateExpression(LuaExpressionSyntax expression, string comma, LuaCodeTemplateExpressionSyntax codeTemplateExpression) {
      if (!string.IsNullOrEmpty(comma)) {
        codeTemplateExpression.Expressions.Add(comma);
      }
      codeTemplateExpression.Expressions.Add(expression);
    }

    private LuaExpressionSyntax InternalBuildCodeTemplateExpression(
      string codeTemplate, 
      ExpressionSyntax targetExpression, 
      IEnumerable<Func<LuaExpressionSyntax>> arguments, 
      IList<ITypeSymbol> typeArguments) {
      LuaCodeTemplateExpressionSyntax codeTemplateExpression = new LuaCodeTemplateExpressionSyntax();

      var matchs = codeTemplateRegex_.Matches(codeTemplate);
      int prevIndex = 0;
      foreach (Match match in matchs) {
        if (match.Index > prevIndex) {
          string prevToken = codeTemplate.Substring(prevIndex, match.Index - prevIndex);
          codeTemplateExpression.Expressions.Add(prevToken);
        }
        string comma = match.Groups[1].Value;
        string key = match.Groups[2].Value;
        if (key == "this") {
          AddCodeTemplateExpression(BuildMemberAccessTargetExpression(targetExpression), comma, codeTemplateExpression);
        } else if (key == "class") {
          var type = semanticModel_.GetTypeInfo(targetExpression).Type;
          LuaExpressionSyntax typeName;
          if (type.TypeKind == TypeKind.Enum) {
            typeName = GetTypeShortName(type);
            AddExportEnum(type);
          } else {
            typeName = GetTypeName(type);
          }
          AddCodeTemplateExpression(typeName, comma, codeTemplateExpression);
        } else if (key[0] == '^') {
          if (int.TryParse(key.Substring(1), out int typeIndex)) {
            var typeArgument = typeArguments.GetOrDefault(typeIndex);
            if (typeArgument != null) {
              LuaExpressionSyntax typeName;
              if (typeArgument.TypeKind == TypeKind.Enum && codeTemplate.StartsWith("System.Enum.TryParse")) {
                typeName = GetTypeShortName(typeArgument);
                AddExportEnum(typeArgument);
              } else {
                typeName = GetTypeName(typeArgument);
              }
              AddCodeTemplateExpression(typeName, comma, codeTemplateExpression);
            }
          }
        } else if (key[0] == '*') {
          if (int.TryParse(key.Substring(1), out int paramsIndex)) {
            LuaSequenceListExpressionSyntax sequenceList = new LuaSequenceListExpressionSyntax();
            foreach (var argument in arguments.Skip(paramsIndex)) {
              var argumentExpression = argument();
              sequenceList.Expressions.Add(argumentExpression);
            }
            if (sequenceList.Expressions.Count > 0) {
              AddCodeTemplateExpression(sequenceList, comma, codeTemplateExpression);
            }
          }
        } else {
          if (int.TryParse(key, out int argumentIndex)) {
            var argument = arguments.ElementAtOrDefault(argumentIndex);
            if (argument != null) {
              var argumentExpression = argument();
              AddCodeTemplateExpression(argumentExpression, comma, codeTemplateExpression);
            }
          }
        }
        prevIndex = match.Index + match.Length;
      }

      if (prevIndex < codeTemplate.Length) {
        string last = codeTemplate.Substring(prevIndex);
        codeTemplateExpression.Expressions.Add(last);
      }

      return codeTemplateExpression;
    }

    private void AddExportEnum(ITypeSymbol enumType) {
      generator_.AddExportEnum(enumType);
    }

    private bool IsPropertyField(IPropertySymbol symbol) {
      return generator_.IsPropertyField(symbol);
    }

    private bool IsEventFiled(IEventSymbol symbol) {
      return generator_.IsEventFiled(symbol);
    }

    private bool IsInternalMember(SyntaxNode node, ISymbol symbol) {
      bool isVirtual = symbol.IsOverridable() && !generator_.IsSealed(symbol.ContainingType);
      if (!isVirtual) {
        var typeSymbol = CurTypeSymbol;
        if (typeSymbol.IsContainsInternalSymbol(symbol)) {
          return true;
        }
      }
      return false;
    }

    private LuaExpressionSyntax BuildArray(ITypeSymbol elementType, params LuaExpressionSyntax[] elements) {
      var baseType = GetTypeName(elementType);
      var arrayType = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Array, baseType);
      return BuildArray(arrayType, elements);
    }

    private LuaExpressionSyntax BuildArray(LuaExpressionSyntax arrayType, IList<LuaExpressionSyntax> elements) {
      if (elements.Count > kMaxArrayInitializerCount) {
        return new LuaInvocationExpressionSyntax(
          new LuaMemberAccessExpressionSyntax(arrayType, LuaIdentifierNameSyntax.New, true), 
          elements.Count,
          new LuaTableExpression(elements) { IsSingleLine = true }
          );
      } else {
        return new LuaInvocationExpressionSyntax(arrayType, elements);
      }
    }

    private LuaExpressionSyntax BuildArray(LuaExpressionSyntax arrayType, LuaExpressionSyntax size) {
      return new LuaInvocationExpressionSyntax(new LuaMemberAccessExpressionSyntax(arrayType, LuaIdentifierNameSyntax.New, true), size);
    }

    private LuaExpressionSyntax BuildMultiArray(LuaExpressionSyntax arrayType, LuaExpressionSyntax rank, List<LuaExpressionSyntax> elements = null) {
      var invocation = new LuaInvocationExpressionSyntax(arrayType, rank);
      if (elements != null) {
        invocation.AddArgument(new LuaTableExpression(elements) { IsSingleLine = true });
      }
      return invocation;
    }

    private LuaLiteralExpressionSyntax GetLiteralExpression(object constantValue) {
      if (constantValue != null) {
        var code = Type.GetTypeCode(constantValue.GetType());
        switch (code) {
          case TypeCode.Char: {
              return new LuaCharacterLiteralExpression((char)constantValue);
            }
          case TypeCode.String: {
              return BuildStringLiteralExpression((string)constantValue);
            }
          case TypeCode.Boolean: {
              bool v = (bool)constantValue;
              return v ? LuaIdentifierLiteralExpressionSyntax.True : LuaIdentifierLiteralExpressionSyntax.False;
            }
          case TypeCode.Single: {
            return (float)constantValue;
          }
          case TypeCode.Double: {
            return (double)constantValue;
          }
          default: {
              return new LuaIdentifierLiteralExpressionSyntax(constantValue.ToString());
            }
        }
      } else {
        return LuaIdentifierLiteralExpressionSyntax.Nil;
      }
    }

    private LuaLiteralExpressionSyntax GetConstLiteralExpression(IFieldSymbol constField) {
      Contract.Assert(constField.HasConstantValue);
      if (constField.Type.SpecialType == SpecialType.System_Char) {
        return new LuaCharacterLiteralExpression((char)constField.ConstantValue);
      } else {
        var literalExpression = GetLiteralExpression(constField.ConstantValue);
        string identifierToken = constField.ContainingType.Name + '.' + constField.Name;
        return new LuaConstLiteralExpression(literalExpression, identifierToken);
      }
    }

    private LuaLiteralExpressionSyntax GetConstLiteralExpression(ILocalSymbol constLocal) {
      Contract.Assert(constLocal.HasConstantValue);
      if (constLocal.Type.SpecialType == SpecialType.System_Char) {
        return new LuaCharacterLiteralExpression((char)constLocal.ConstantValue);
      } else {
        var literalExpression = GetLiteralExpression(constLocal.ConstantValue);
        string identifierToken = constLocal.Name;
        return new LuaConstLiteralExpression(literalExpression, identifierToken);
      }
    }

    private LuaLiteralExpressionSyntax GetConstExpression(ExpressionSyntax node) {
      var constValue = semanticModel_.GetConstantValue(node);
      if (constValue.HasValue) {
        var literalExpression = GetLiteralExpression(constValue.Value);
        return new LuaConstLiteralExpression(literalExpression, node.ToString());
      }
      return null;
    }

    private LuaLiteralExpressionSyntax BuildStringLiteralTokenExpression(SyntaxToken token) {
      if (token.Text[0] == '@') {
        return BuildVerbatimStringExpression(token.ValueText);
      } else {
        return new LuaIdentifierLiteralExpressionSyntax(token.Text);
      }
    }

    private LuaIdentifierLiteralExpressionSyntax BuildStringLiteralExpression(string value) {
      string text = SyntaxFactory.Literal(value).Text;
      return new LuaIdentifierLiteralExpressionSyntax(text);
    }

    private LuaVerbatimStringLiteralExpressionSyntax BuildVerbatimStringExpression(string value) {
      return new LuaVerbatimStringLiteralExpressionSyntax(value);
    }

    private enum CallerAttributeKind {
      None,
      Line,
      Member,
      FilePath,
    }

    private CallerAttributeKind GetCallerAttributeKind(INamedTypeSymbol symbol) {
      switch (symbol.Name) {
        case "CallerLineNumberAttribute": {
            if (symbol.ContainingNamespace.IsRuntimeCompilerServices()) {
              return CallerAttributeKind.Line;
            }
            break;
          }
        case "CallerMemberNameAttribute": {
            if (symbol.ContainingNamespace.IsRuntimeCompilerServices()) {
              return CallerAttributeKind.Member;
            }
            break;
          }
        case "CallerFilePathAttribute": {
            if (symbol.ContainingNamespace.IsRuntimeCompilerServices()) {
              return CallerAttributeKind.FilePath;
            }
            break;
          }
      }
      return CallerAttributeKind.None;
    }

    private CallerAttributeKind GetCallerAttributeKind(IParameterSymbol parameter) {
      foreach (var attribute in parameter.GetAttributes()) {
        var callerKind = GetCallerAttributeKind(attribute.AttributeClass);
        if (callerKind != CallerAttributeKind.None) {
          return callerKind;
        }
      }
      return CallerAttributeKind.None;
    }

    private LuaExpressionSyntax CheckCallerAttribute(IParameterSymbol parameter, SyntaxNode node) {
      var kind = GetCallerAttributeKind(parameter);
      switch (kind) {
        case CallerAttributeKind.Line: {
            var lineSpan = node.SyntaxTree.GetLineSpan(node.Span);
            return lineSpan.StartLinePosition.Line + 1;
          }
        case CallerAttributeKind.Member: {
            var parentMethod = (MethodDeclarationSyntax)FindParent(node, SyntaxKind.MethodDeclaration);
            return new LuaStringLiteralExpressionSyntax(parentMethod.Identifier.ValueText);
          }
        case CallerAttributeKind.FilePath: {
            return BuildStringLiteralExpression(generator_.RemoveBaseFolder(node.SyntaxTree.FilePath));
          }
        default:
          return null;
      }
    }

    private bool CheckUsingStaticNameSyntax(ISymbol symbol, NameSyntax node, LuaExpressionSyntax expression, out LuaMemberAccessExpressionSyntax outExpression) {
      bool isUsingStaticName = false;
      if (node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
        var memberAccess = (MemberAccessExpressionSyntax)node.Parent;
        if (memberAccess.Expression == node) {
          isUsingStaticName = true;
        }
      } else {
        isUsingStaticName = true;
      }

      if (isUsingStaticName) {
        if (!CurTypeSymbol.IsContainsInternalSymbol(symbol)) {          
          var usingStaticType = GetTypeName(symbol.ContainingType);
          outExpression = new LuaMemberAccessExpressionSyntax(usingStaticType, expression);
          return true;
        }
      }

      outExpression = null;
      return false;
    }

    private bool MayBeFalse(ExpressionSyntax expression, ITypeSymbol type) {
      bool mayBeFalse = false;
      if (type.IsValueType) {
        if (type.SpecialType == SpecialType.System_Boolean) {
          var constValue = semanticModel_.GetConstantValue(expression);
          if (constValue.HasValue && (bool)constValue.Value) {
            mayBeFalse = false;
          } else {
            mayBeFalse = true;
          }
        }
      }
      return mayBeFalse;
    }

    private bool MayBeNull(ExpressionSyntax expression, ITypeSymbol type) {
      if (expression.IsKind(SyntaxKind.ObjectInitializerExpression)) {
        return false;
      }

      Contract.Assert(type != null);
      bool mayBeNull;
      if (type.IsValueType) {
        mayBeNull = false;
      } else if (type.IsStringType()) {
        var constValue = semanticModel_.GetConstantValue(expression);
        if (constValue.HasValue) {
          mayBeNull = false;
        } else {
          if (expression.IsKind(SyntaxKind.InvocationExpression)) {
            var invocation = (InvocationExpressionSyntax)expression;
            if (invocation.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
              var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
              if (memberAccess.Name.Identifier.ValueText == LuaIdentifierNameSyntax.ToStr.ValueText) {
                var typeInfo = semanticModel_.GetTypeInfo(memberAccess.Expression).Type;
                if (typeInfo.SpecialType > SpecialType.System_Object) {
                  return false;
                }
              }
            }
          } else if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
            var memberAccess = (MemberAccessExpressionSyntax)expression;
            var typeInfo = semanticModel_.GetTypeInfo(memberAccess.Expression).Type;
            if (typeInfo.SpecialType > SpecialType.System_Object) {
              return false;
            }
          }
          mayBeNull = true;
        }
      } else {
        mayBeNull = true;
      }
      return mayBeNull;
    }

    private bool MayBeNullOrFalse(ExpressionSyntax conditionalWhenTrue) {
      if (conditionalWhenTrue.IsKind(SyntaxKind.NullLiteralExpression)) {
        return true;
      }
      var type = semanticModel_.GetTypeInfo(conditionalWhenTrue).Type;
      return MayBeNull(conditionalWhenTrue, type) || MayBeFalse(conditionalWhenTrue, type);
    }

    private bool IsLocalVarExistsInCurMethod(string name) {
      var methodInfo = CurMethodInfoOrNull;
      if (methodInfo != null) {
        var syntaxReference = methodInfo.Symbol.DeclaringSyntaxReferences.First();
        var root = syntaxReference.GetSyntax();
        if (IsLocalVarExists(name, root)) {
          return true;
        }
      }
      return false;
    }

    private bool IsCurMethodTypeArgument(ITypeSymbol symbol) {
      var method = CurMethodInfoOrNull;
      if (method != null && method.Symbol.TypeArguments.Length > 0) {
        bool isMethodTypeArgument = method.Symbol.TypeArguments.Any(i => symbol.IsTypeParameterExists(i));
        if (isMethodTypeArgument) {
          return true;
        }
      }
      return false;
    }

    private bool IsImportTypeNameEnale(INamedTypeSymbol symbol) {
      if (symbol.IsGenericType) {
        if (symbol.IsTypeParameterExists() && !IsCurMethodTypeArgument(symbol)) {
          return false;
        }
        return true;
      }
      return true;
    }

    internal void ImportTypeName(ref string name, INamedTypeSymbol symbol) {
      if (IsImportTypeNameEnale(symbol)) {
        int pos = name.LastIndexOf('.');
        if (pos != -1) {
          string prefix = name.Substring(0, pos);
          if (prefix != LuaIdentifierNameSyntax.System.ValueText && prefix != LuaIdentifierNameSyntax.Class.ValueText) {
            string newPrefix = prefix.Replace(".", "");
            if (!IsLocalVarExistsInCurMethod(newPrefix)) {
              name = newPrefix + name.Substring(pos);
              CurCompilationUnit.AddImport(prefix, newPrefix, symbol.IsFromCode());
            }      
          }
        }
      }
    }

    internal void ImportGenericTypeName(ref LuaExpressionSyntax luaExpression, ITypeSymbol symbol) {
      if (!IsNoImportTypeName && !CurTypeSymbol.Equals(symbol) && !IsCurMethodTypeArgument(symbol)) {
        var invocationExpression = (LuaInvocationExpressionSyntax)luaExpression;
        string newName = GetGenericTypeImportName(invocationExpression, out var argumentTypeNames);
        if (!IsLocalVarExistsInCurMethod(newName)) {
          if (!symbol.IsTypeParameterExists()) {
            CurCompilationUnit.AddImport(invocationExpression, newName, argumentTypeNames, symbol.IsAbsoluteFromCode());
          } else {
            CurTypeDeclaration.TypeDeclaration.AddImport(invocationExpression, newName, argumentTypeNames, symbol.IsAbsoluteFromCode());
          }
          luaExpression = newName;
        }
      }
    }

    private static void FillGenericTypeImportName(StringBuilder sb, List<string> argumentTypeNames, LuaInvocationExpressionSyntax invocationExpression) {
      string CheckLastName(string lastName) {
        return lastName == "Dictionary" ? "Dict" : lastName;
      }
      var identifierName =(LuaIdentifierNameSyntax)invocationExpression.Expression;
      sb.Append(CheckLastName(identifierName.ValueText.LastName()));
      foreach (var argument in invocationExpression.ArgumentList.Arguments) {
        if (argument.Expression is LuaIdentifierNameSyntax typeName) {
          string argumentTypeName = typeName.ValueText;
          sb.Append(CheckLastName(argumentTypeName.LastName()));
          argumentTypeNames.Add(argumentTypeName);
        } else {
          FillGenericTypeImportName(sb, argumentTypeNames,(LuaInvocationExpressionSyntax)argument.Expression);
        }
      }
    }

    private static string GetGenericTypeImportName(LuaInvocationExpressionSyntax invocationExpression, out List<string> argumentTypeNames) {
      StringBuilder sb = new StringBuilder();
      argumentTypeNames = new List<string>();
      FillGenericTypeImportName(sb, argumentTypeNames, invocationExpression);
      return sb.ToString();
    }

    private LuaIdentifierNameSyntax GetTypeShortName(ISymbol symbol) {
      return generator_.GetTypeShortName(symbol, this);
    }

    private LuaExpressionSyntax GetTypeName(ISymbol symbol) {
      return generator_.GetTypeName(symbol, this);
    }

    private LuaExpressionSyntax GetTypeNameWithoutImport(ISymbol symbol) {
      ++noImportTypeNameCounter_;
      var name = GetTypeName(symbol);
      --noImportTypeNameCounter_;
      return name;
    }

    private LuaExpressionSyntax GetTypeNameOfMetadata(ISymbol symbol) {
      ++metadataTypeNameCounter_;
      var name = GetTypeNameWithoutImport(symbol);
      --metadataTypeNameCounter_;
      return name;
    }

    private LuaExpressionSyntax BuildFieldOrPropertyMemberAccessExpression(LuaExpressionSyntax expression, LuaExpressionSyntax name, bool isStatic) {
      if (name is LuaPropertyAdapterExpressionSyntax propertyMethod) {
        var arguments = propertyMethod.ArgumentList.Arguments;
        if (arguments.Count == 1) {
          if (arguments[0].Expression == LuaIdentifierNameSyntax.This) {
            propertyMethod.ArgumentList.Arguments[0] = new LuaArgumentSyntax(expression);
          }
        } else {
          propertyMethod.Update(expression, !isStatic);
        }
        return propertyMethod;
      } else {
        return new LuaMemberAccessExpressionSyntax(expression, name);
      }
    }

    public override LuaSyntaxNode VisitAttributeList(AttributeListSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitAttributeArgument(AttributeArgumentSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitNameColon(NameColonSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitAttributeArgumentList(AttributeArgumentListSyntax node) {
      throw new InvalidOperationException();
    }

    public override LuaSyntaxNode VisitNameEquals(NameEqualsSyntax node) {
      return node.Name.Accept(this);
    }

    private LuaInvocationExpressionSyntax BuildObjectCreationInvocation(IMethodSymbol symbol, LuaExpressionSyntax expression) {
      int constructorIndex = symbol.GetConstructorIndex();
      if (constructorIndex > 1) {
        return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.SystemNew, expression, constructorIndex.ToString());
      }
      return new LuaInvocationExpressionSyntax(expression);
    }

    public override LuaSyntaxNode VisitAttribute(AttributeSyntax node) {
      var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node.Name).Symbol;
      INamedTypeSymbol typeSymbol = symbol.ContainingType;
      if (!generator_.IsExportAttribute(typeSymbol)) {
        return null;
      }

      var expression = GetTypeNameWithoutImport(typeSymbol);
      LuaInvocationExpressionSyntax invocation = BuildObjectCreationInvocation(symbol, expression);

      if (node.ArgumentList != null) {
        List<LuaExpressionSyntax> arguments = new List<LuaExpressionSyntax>();
        var initializers = new List<(LuaExpressionSyntax Name, LuaExpressionSyntax Expression)>();
        var argumentNodeInfos = new List<(NameColonSyntax, ExpressionSyntax)>();

        foreach (var argumentNode in node.ArgumentList.Arguments) {
          var argumentExpression = (LuaExpressionSyntax)argumentNode.Expression.Accept(this);
          if (argumentNode.NameEquals == null) {
            if (argumentNode.NameColon != null) {
              string name = argumentNode.NameColon.Name.Identifier.ValueText;
              int index = symbol.Parameters.IndexOf(i => i.Name == name);
              Contract.Assert(index != -1);
              arguments.AddAt(index, argumentExpression);
            } else {
              arguments.Add(argumentExpression);
            }
          } else {
            var name = (LuaExpressionSyntax)argumentNode.NameEquals.Accept(this);
            initializers.Add((name, argumentExpression));
          }
        }

        CheckInvocationDeafultArguments(symbol, symbol.Parameters, arguments, argumentNodeInfos, node, false);
        invocation.AddArguments(arguments);

        if (initializers.Count == 0) {
          return invocation;
        } else {
          LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
          PushFunction(function);
          var temp = GetTempIdentifier(node);
          function.AddParameter(temp);

          foreach (var initializer in initializers) {
            var memberAccess = BuildFieldOrPropertyMemberAccessExpression(temp, initializer.Name, false);
            var assignmentExpression = BuildLuaSimpleAssignmentExpression(memberAccess, initializer.Expression);
            function.AddStatement(assignmentExpression);
          }

          PopFunction();
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Apply, invocation, function);
        }
      } else {
        return invocation;
      }
    }

    private List<LuaExpressionSyntax> BuildAttributes(SyntaxList<AttributeListSyntax> attributeLists) {
      List<LuaExpressionSyntax> expressions = new List<LuaExpressionSyntax>();
      var attributes = attributeLists.SelectMany(i => i.Attributes);
      foreach (var node in attributes) {
        var expression = (LuaExpressionSyntax)node.Accept(this);
        if (expression != null) {
          expressions.Add(expression);
        }
      }
      return expressions;
    }

    private static IParameterSymbol GetParameterSymbol(IMethodSymbol symbol, ArgumentSyntax argument) {
      IParameterSymbol parameter;
      if (argument.NameColon != null) {
        parameter = symbol.Parameters.First(i => i.Name == argument.NameColon.Name.Identifier.ValueText);
      } else {
        var argumentList = (ArgumentListSyntax)argument.Parent;
        int index = argumentList.Arguments.IndexOf(argument);
        parameter = symbol.Parameters[index];
      }
      return parameter;
    }

    private void CheckValueTypeClone(ITypeSymbol typeSymbol, IdentifierNameSyntax node, ref LuaExpressionSyntax expression) {
      if (typeSymbol.IsCustomValueType() && !typeSymbol.IsNullableType() && !generator_.IsReadOnlyStruct(typeSymbol)) {
        bool need = false;
        switch (node.Parent.Kind()) {
          case SyntaxKind.Argument: {
              var argument = (ArgumentSyntax)node.Parent;
              switch (argument.RefKindKeyword.Kind()) {
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword: {
                    return;
                  }
              }

              var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(argument.Parent.Parent).Symbol;
              if (symbol != null) {
                if (symbol.IsFromAssembly() && !symbol.ContainingType.IsCollectionType()) {
                  break;
                }

                var parameter = GetParameterSymbol(symbol, argument);
                if (parameter.RefKind == RefKind.In) {
                  break;
                }
              }

              need = true;
              break;
            }
          case SyntaxKind.ReturnStatement:  {
              need = true;
              break;
            }
          case SyntaxKind.SimpleAssignmentExpression: {
              var assignment = (AssignmentExpressionSyntax)node.Parent;
              if (assignment.Right == node) {
                var symbol = semanticModel_.GetSymbolInfo(assignment.Left).Symbol;
                if (symbol != null && symbol.IsFromAssembly()) {
                  break;
                }

                if (assignment.Left.Kind().IsTupleDeclaration()) {
                  break;
                }

                need = true;
              }
              break;
            }
          case SyntaxKind.EqualsValueClause: {
              var equalsValueClause = (EqualsValueClauseSyntax)node.Parent; 
              if (equalsValueClause.Value == node) {
                need = true;
              }
              break;
            }
          case SyntaxKind.SimpleMemberAccessExpression: {
              var memberAccess = (MemberAccessExpressionSyntax)node.Parent;
              if (memberAccess.Name == node) {
                switch (memberAccess.Parent.Kind()) {
                  case SyntaxKind.EqualsValueClause: {
                      need = true;
                      break;
                    }

                  case SyntaxKind.SimpleAssignmentExpression: {
                      var assignment = (AssignmentExpressionSyntax)memberAccess.Parent;
                      if (assignment.Right == memberAccess) {
                        need = true;
                      }
                      break;
                    }
                }
              }
              break;
            }
        }

        if (need) {
          var invocation = new LuaInvocationExpressionSyntax(new LuaMemberAccessExpressionSyntax(expression, LuaIdentifierNameSyntax.Clone, true));
          expression = invocation;
        }
      }
    }

    private LuaDocumentStatement BuildDocumentationComment(CSharpSyntaxNode node) {
      foreach (var trivia in node.GetLeadingTrivia()) {
        if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)) {
          string triviaText = trivia.ToString();
          if (!string.IsNullOrWhiteSpace(triviaText)) {
            return new LuaDocumentStatement(triviaText);
          }
        }
      }
      return null;
    }

    private LuaExpressionSyntax BuildInheritTypeName(BaseTypeSyntax baseType) {
      ++noImportTypeNameCounter_;
      var baseTypeName = (LuaExpressionSyntax)baseType.Accept(this);
      --noImportTypeNameCounter_;
      return baseTypeName;
    }

    public override LuaSyntaxNode VisitTypeParameterList(TypeParameterListSyntax node) {
      LuaParameterListSyntax parameterList = new LuaParameterListSyntax();
      foreach (var typeParameter in node.Parameters) {
        var typeIdentifier = (LuaIdentifierNameSyntax)typeParameter.Accept(this);
        parameterList.Parameters.Add(new LuaParameterSyntax(typeIdentifier));
      }
      return parameterList;
    }

    private void FillExternalTypeParameters(List<LuaParameterSyntax> typeParameters, INamedTypeSymbol typeSymbol) {
      var externalType = typeSymbol.ContainingType;
      if (externalType != null) {
        FillExternalTypeParameters(typeParameters, externalType);
        foreach (var typeParameterSymbol in externalType.TypeParameters) {
          LuaIdentifierNameSyntax identifierName = typeParameterSymbol.Name;
          typeParameters.Add(new LuaParameterSyntax(identifierName));
        }
      }
    }

    private void BuildTypeParameters(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
      List<LuaParameterSyntax> typeParameters = new List<LuaParameterSyntax>();
      FillExternalTypeParameters(typeParameters, typeSymbol);
      if (node.TypeParameterList != null) {
        var parameterList = (LuaParameterListSyntax)node.TypeParameterList.Accept(this);
        typeParameters.AddRange(parameterList.Parameters);
      }
      typeDeclaration.AddTypeParameters(typeParameters);
    }

    private LuaIdentifierNameSyntax GetMemberName(ISymbol symbol) {
      return generator_.GetMemberName(symbol);
    }

    private LuaIdentifierNameSyntax AddInnerName(ISymbol symbol) {
      return generator_.AddInnerName(symbol);
    }

    private void TryRemoveNilArgumentsAtTail(ISymbol symbol, List<LuaExpressionSyntax> arguments) {
      if (arguments.Count > 0) {
        if (symbol.IsFromCode() || symbol.ContainingType.GetMembers(symbol.Name).Length == 1) {
          arguments.RemoveNilAtTail();
        }
      }
    }

    private void PushChecked(bool isChecked) {
      checkeds_.Push(isChecked);
    }

    private void PopChecked() {
      checkeds_.Pop();
    }

    private bool IsCurChecked {
      get {
        if (checkeds_.Count > 0) {
          return checkeds_.Peek();
        }
        return generator_.IsCheckedOverflow;
      }
    }

    private LuaExpressionSyntax VisitExpression(ExpressionSyntax node) {
      var luaExpression = (LuaExpressionSyntax)node.Accept(this);
      CheckConversion(node, ref luaExpression);
      return luaExpression;
    }

    private void CheckConversion(ExpressionSyntax node, ref LuaExpressionSyntax expression) {
      var conversion = semanticModel_.GetConversion(node);
      if (conversion.IsUserDefined && conversion.IsImplicit) {
        expression = BuildConversionExpression(conversion.MethodSymbol, expression);
      }
    }

    private LuaExpressionSyntax GetOperatorMemberAccessExpression(IMethodSymbol methodSymbol) {
      var methodName = GetMemberName(methodSymbol);
      if (CurTypeSymbol.Equals(methodSymbol.ContainingType)) {
        return methodName;
      }

      if (CurTypeSymbol.IsContainsInternalSymbol(methodSymbol)) {
        if (CurTypeSymbol.GetMembers(methodSymbol.Name).IsEmpty) {
          return methodName;
        }
      }

      var typeName = GetTypeName(methodSymbol.ContainingType);
      return new LuaMemberAccessExpressionSyntax(typeName, methodName);
    }

    private LuaExpressionSyntax BuildConversionExpression(IMethodSymbol methodSymbol, LuaExpressionSyntax expression) {
      var codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(methodSymbol);
      if (codeTemplate != null) {
        return BuildCodeTemplateExpression(codeTemplate, null, new LuaExpressionSyntax[] { expression }, null);
      }

      var memberAccess = GetOperatorMemberAccessExpression(methodSymbol);
      return new LuaInvocationExpressionSyntax(memberAccess, expression);
    }

    private LuaExpressionSyntax GetUserDefinedOperatorExpression(ExpressionSyntax node, ExpressionSyntax arguemt) {
      return GetUserDefinedOperatorExpression(node, new Func<LuaExpressionSyntax>[] {
        () => VisitExpression(arguemt),
      });
    }

    private LuaExpressionSyntax GetUserDefinedOperatorExpression(ExpressionSyntax node, ExpressionSyntax left, ExpressionSyntax right) {
      return GetUserDefinedOperatorExpression(node, new Func<LuaExpressionSyntax>[] {
        () => VisitExpression(left),
        () => VisitExpression(right)
      });
    }

    private LuaExpressionSyntax GetUserDefinedOperatorExpression(ExpressionSyntax node, LuaExpressionSyntax left, LuaExpressionSyntax right) {
      return GetUserDefinedOperatorExpression(node, new Func<LuaExpressionSyntax>[] {
        () => left,
        () => right
      });
    }

    private LuaExpressionSyntax GetUserDefinedOperatorExpression(ExpressionSyntax node, IEnumerable<Func<LuaExpressionSyntax>> arguments) {
      var methodSymbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
      if (methodSymbol != null) {
        var typeSymbol = methodSymbol.ContainingType;
        if (typeSymbol != null) {
          if (typeSymbol.TypeKind != TypeKind.Enum 
            && typeSymbol.TypeKind != TypeKind.Delegate 
            && typeSymbol.SpecialType == SpecialType.None 
            && !typeSymbol.IsTimeSpanType()) {
            var codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(methodSymbol);
            if (codeTemplate != null) {
              return InternalBuildCodeTemplateExpression(codeTemplate, null, arguments, null);
            }
            var memberAccess = GetOperatorMemberAccessExpression(methodSymbol);
            return new LuaInvocationExpressionSyntax(memberAccess, arguments.Select(i => i()));
          }
        }
      }
      return null;
    }

    private bool IsNumericalForVariableMatch(ExpressionSyntax node, SyntaxToken identifier) {
      if (node.IsKind(SyntaxKind.IdentifierName)) {
        var identifierName = (IdentifierNameSyntax)node;
        return identifierName.Identifier.ValueText == identifier.ValueText;
      }
      return false;
    }

    private bool IsNumericalForLess(SyntaxKind kind, out bool isLess) {
      switch (kind) {
        case SyntaxKind.NotEqualsExpression:
        case SyntaxKind.LessThanExpression:
          isLess = true;
          return true;
        case SyntaxKind.LessThanOrEqualExpression:
          isLess = false;
          return true;
        default:
          isLess = false;
          return false;
      }
    }

    private bool IsNumericalForGreater(SyntaxKind kind, out bool isGreater) {
      switch (kind) {
        case SyntaxKind.NotEqualsExpression:
        case SyntaxKind.GreaterThanExpression:
          isGreater = true;
          return true;
        case SyntaxKind.GreaterThanOrEqualExpression:
          isGreater = false;
          return true;
        default:
          isGreater = false;
          return false;
      }
    }

    private bool IsFixedValueExpression(ExpressionSyntax expression, out Optional<object> limitConst) {
      limitConst = semanticModel_.GetConstantValue(expression);
      if (limitConst.HasValue) {
        return true;
      }

      if (expression is BinaryExpressionSyntax binaryExpression) {
        Optional<object> _ = default;
        return IsFixedValueExpression(binaryExpression.Right, out _) && IsFixedValueExpression(binaryExpression.Left, out _);
      } else {
        bool isReadOnly = false;
        var symbol = semanticModel_.GetSymbolInfo(expression).Symbol;
        if (symbol != null) {
          if (symbol.Kind == SymbolKind.Field) {
            isReadOnly = ((IFieldSymbol)symbol).IsReadOnly;
          } else if (symbol.Kind == SymbolKind.Property) {
            var propertySymbol = (IPropertySymbol)symbol;
            isReadOnly = propertySymbol.IsReadOnly && IsPropertyField(propertySymbol);
          }
        }
        return isReadOnly;
      }
    }

    private LuaExpressionSyntax GetFixedValueExpression(ExpressionSyntax expression) {
      if (IsFixedValueExpression(expression, out var constantValue)) {
        if (constantValue.HasValue) {
          return Convert.ToDouble(constantValue.Value);
        }
        return (LuaExpressionSyntax)expression.Accept(this);
      } else {
        return null;
      }
    }

    private LuaNumericalForStatementSyntax GetNumericalForStatement(ForStatementSyntax node) {
      if (node.Declaration == null || node.Declaration.Variables.Count > 1) {
        goto Fail;
      }

      if (node.Condition == null) {
        goto Fail;
      }

      if (node.Incrementors.Count != 1) {
        goto Fail;
      }

      var variable = node.Declaration.Variables.First();
      if (variable.Initializer == null) {
        goto Fail;
      }

      var conditionKind = node.Condition.Kind();
      if (conditionKind < SyntaxKind.NotEqualsExpression || conditionKind > SyntaxKind.GreaterThanOrEqualExpression) {
        goto Fail;
      }

      var condition = (BinaryExpressionSyntax)node.Condition;
      if (!IsNumericalForVariableMatch(condition.Left, variable.Identifier)) {
        goto Fail;
      }

      var limitExpression = GetFixedValueExpression(condition.Right);
      if (limitExpression == null) {
        goto Fail;
      }

      LuaExpressionSyntax stepExpression;
      bool hasNoEqual;
      var incrementor = node.Incrementors.First();
      switch (incrementor.Kind()) {
        case SyntaxKind.PreIncrementExpression:
        case SyntaxKind.PreDecrementExpression:
        case SyntaxKind.PostIncrementExpression:
        case SyntaxKind.PostDecrementExpression: {
          ExpressionSyntax operand;
          if (incrementor is PrefixUnaryExpressionSyntax prefixUnaryExpression) {
            operand = prefixUnaryExpression.Operand;
          } else {
            operand = ((PostfixUnaryExpressionSyntax)incrementor).Operand;
          }
          if (!IsNumericalForVariableMatch(operand, variable.Identifier)) {
            goto Fail;
          }
          if (incrementor.IsKind(SyntaxKind.PreIncrementExpression) || incrementor.IsKind(SyntaxKind.PostIncrementExpression)) {
            if (!IsNumericalForLess(conditionKind, out hasNoEqual)) {
              goto Fail;
            }
            stepExpression = 1;
          } else {
            if (!IsNumericalForGreater(conditionKind, out hasNoEqual)) {
              goto Fail;
            }
            stepExpression = -1;
          }
          break;
        }
        case SyntaxKind.AddAssignmentExpression:
        case SyntaxKind.SubtractAssignmentExpression: {
          var assignment = (AssignmentExpressionSyntax)incrementor;
          if (!IsNumericalForVariableMatch(assignment.Left, variable.Identifier)) {
            goto Fail;
          }

          stepExpression = GetFixedValueExpression(assignment.Right);
          if (stepExpression == null) {
            goto Fail;
          }

          if (incrementor.IsKind(SyntaxKind.AddAssignmentExpression)) {
            if (!IsNumericalForLess(conditionKind, out hasNoEqual)) {
              goto Fail;
            }
          } else {
            if (!IsNumericalForGreater(conditionKind, out hasNoEqual)) {
              goto Fail;
            }
            if (stepExpression is LuaNumberLiteralExpressionSyntax numberLiteral) {
              stepExpression = -numberLiteral.Number;
            }
          }
          break;
        }
        default:
          goto Fail;
      }

      LuaIdentifierNameSyntax identifier = variable.Identifier.ValueText;
      CheckLocalVariableName(ref identifier, variable);

      var startExpression = (LuaExpressionSyntax)variable.Initializer.Value.Accept(this);
      if (hasNoEqual) {
        if (limitExpression is LuaNumberLiteralExpressionSyntax limitLiteral) {
          if (stepExpression is LuaNumberLiteralExpressionSyntax numberLiteral) {
            limitExpression = new LuaIdentifierLiteralExpressionSyntax((limitLiteral.Number - numberLiteral.Number).ToString());
          } else {
            limitExpression = new LuaBinaryExpressionSyntax(limitExpression, LuaSyntaxNode.Tokens.Sub, stepExpression);
          }
        } else {
          if (stepExpression is LuaNumberLiteralExpressionSyntax numberLiteral) {
            if (numberLiteral.Number > 0) {
              limitExpression = new LuaBinaryExpressionSyntax(limitExpression, LuaSyntaxNode.Tokens.Sub, stepExpression);
            } else {
              limitExpression = new LuaBinaryExpressionSyntax(limitExpression, LuaSyntaxNode.Tokens.Plus, (-numberLiteral.Number).ToString());
            }
          } else {
            limitExpression = new LuaBinaryExpressionSyntax(limitExpression, LuaSyntaxNode.Tokens.Sub, stepExpression);
          }
        }
      }

      if (stepExpression is LuaNumberLiteralExpressionSyntax stepNumber && stepNumber.Number == 1) {
        stepExpression = null;
      }

      var numericalForStatement = new LuaNumericalForStatementSyntax(identifier, startExpression, limitExpression, stepExpression);
      VisitLoopBody(node.Statement, numericalForStatement.Body);
      return numericalForStatement;

    Fail:
      return null;
    }

    private LuaExpressionSyntax GetValueTupleDefaultExpression(ITypeSymbol typeSymbol) {
      var elementTypes = typeSymbol.GetTupleElementTypes();
      return BuildValueTupleCreateExpression(elementTypes.Select(i => GetDefaultValueExpression(i)));
    }

    private LuaExpressionSyntax GetDefaultValueExpression(ITypeSymbol typeSymbol) {
      if (typeSymbol.IsReferenceType) {
        return LuaIdentifierLiteralExpressionSyntax.Nil;
      }

      if (typeSymbol.IsValueType) {
        if (typeSymbol.IsNullableType()) {
          return LuaIdentifierLiteralExpressionSyntax.Nil;
        }

        if (typeSymbol.IsTupleType) {
          return GetValueTupleDefaultExpression(typeSymbol);
        }

        var predefinedValueType = GetPredefinedValueTypeDefaultValue(typeSymbol);
        if (predefinedValueType != null) {
          return predefinedValueType;
        }
      }

      var typeName = GetTypeName(typeSymbol);
      return BuildDefaultValue(typeName);
    }

    private LuaExpressionSyntax BuildDeconstructExpression(ITypeSymbol typeSymbol, LuaExpressionSyntax expression, SyntaxNode node) {
      const string kDeconstructName = "Deconstruct";
      LuaInvocationExpressionSyntax BuildInvocation() {
        return new LuaInvocationExpressionSyntax(new LuaMemberAccessExpressionSyntax(expression, kDeconstructName, true));
      }

      if (typeSymbol.IsTupleType) {
        var invocationExpression = BuildInvocation();
        invocationExpression.AddArgument(typeSymbol.GetTupleElementCount());
        return invocationExpression;
      } else if (typeSymbol.IsSystemTuple()) {
        var nameTypeSymbol = (INamedTypeSymbol)typeSymbol;
        var invocationExpression = BuildInvocation();
        invocationExpression.AddArgument(nameTypeSymbol.TypeArguments.Length);
        return invocationExpression;
      } else {
        var methods = typeSymbol.GetMembers(kDeconstructName);
        if (methods.IsEmpty) {
          throw new CompilationErrorException(node, "current version Roslyn not public api get extension Deconstruct method symbol");
        }
        return BuildInvocation();
      }
    }

    private LuaExpressionSyntax BuildDeconstructExpression(ExpressionSyntax node, LuaExpressionSyntax expression) {
      var typeSymbol = semanticModel_.GetTypeInfo(node).Type;
      return BuildDeconstructExpression(typeSymbol, expression, node);
    }
  }
}
