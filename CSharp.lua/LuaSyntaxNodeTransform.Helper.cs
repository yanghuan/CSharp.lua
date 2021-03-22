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
using System.Globalization;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed partial class LuaSyntaxNodeTransform {
    private sealed class FunctionUpValuesInfo {
      private readonly HashSet<string> strings_ = new HashSet<string>();
      private readonly HashSet<ISymbol> symbols_ = new HashSet<ISymbol>();

      public int Count { get; private set; }

      public void Add(object nameStringOrSymbol) {
        if (nameStringOrSymbol is string name) {
          if (strings_.Add(name)) {
            ++Count;
          }
        } else {
          var symbol = (ISymbol)nameStringOrSymbol;
          if (symbols_.Add(symbol.OriginalDefinition)) {
            Count += symbol.Kind == SymbolKind.Property || symbol.Kind == SymbolKind.Event ? 2 : 1;
          }
        }
      }

      internal bool Contains(object nameStringOrSymbol) {
        if (nameStringOrSymbol is string name) {
          return strings_.Contains(name);
        }
        return symbols_.Contains((ISymbol)nameStringOrSymbol);
      }
    }

    private const int kMaxArrayInitializerCount = 225;
    private static readonly Regex codeTemplateRegex_ = new Regex(@"(,?\s*)\{(\*?[\w|`]+)\}", RegexOptions.Compiled);
    private static readonly Regex unicodeRegex_ = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);
    private readonly Dictionary<ISymbol, LuaIdentifierNameSyntax> localReservedNames_ = new Dictionary<ISymbol, LuaIdentifierNameSyntax>();
    private readonly Dictionary<LuaFunctionExpressionSyntax, FunctionUpValuesInfo> functionUpValues_ = new Dictionary<LuaFunctionExpressionSyntax, FunctionUpValuesInfo>();
    private int localMappingCounter_;
    private readonly Stack<bool> checkeds_ = new Stack<bool>();

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

    private static bool IsLocalVarExists(string name, SyntaxNode root) {
      var searcher = new LocalVarSearcher(name);
      return searcher.Find(root);
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

    private bool CheckLocalVariableName(ref LuaIdentifierNameSyntax identifierName, SyntaxNode node) {
      string name = identifierName.ValueText;
      bool isReserved = CheckLocalBadWord(ref name, node);
      if (isReserved) {
        identifierName = name;
        AddLocalVariableMapping(identifierName, node);
        return true;
      }
      return false;
    }

    private void CheckLocalSymbolName(ISymbol symbol, ref LuaIdentifierNameSyntax name) {
      var newName = localReservedNames_.GetOrDefault(symbol);
      if (newName != null) {
        name = newName;
      }
    }

    private static bool IsStaticLocalMethodEnableAddToType(IMethodSymbol symbol) {
      const int kMaxMemberCountLimit = 150;
      return symbol.ContainingType.GetMembers().Length < kMaxMemberCountLimit;
    }

    private LuaIdentifierNameSyntax GetLocalMethodName(IMethodSymbol symbol, SyntaxNode node) {
      Contract.Assert(symbol.MethodKind == MethodKind.LocalFunction);
      var identifierName = localReservedNames_.GetOrDefault(symbol);
      if (identifierName == null) {
        var lcoalFunctionNode = (LocalFunctionStatementSyntax)symbol.GetDeclaringSyntaxNode();
        string name = symbol.Name;
        if (lcoalFunctionNode.Modifiers.IsStatic() && IsStaticLocalMethodEnableAddToType(symbol)) {
          Utility.IsIdentifierIllegal(ref name);
          name = generator_.GetUniqueNameInType(symbol.ContainingType, name, newName => {
            if (LuaSyntaxNode.IsMethodReservedWord(newName)) {
              return false;
            }
            var body = FindParentMethodBody(node);
            Contract.Assert(body != null);
            return !IsLocalVarExists(newName, body);
          });
        } else {
          if (LuaSyntaxNode.IsReservedWord(name)) {
            CheckLocalBadWord(ref name, node);
          }
        }
        identifierName = name;
        localReservedNames_.Add(symbol, identifierName);
      }
      return identifierName;
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
        ++index;
      }
      throw new InvalidOperationException();
    }

    private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression) {
      return InternalBuildCodeTemplateExpression(codeTemplate, targetExpression, null, null);
    }

    private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, LuaIdentifierNameSyntax targetExpression) {
      return InternalBuildCodeTemplateExpression(codeTemplate, null, null, null, targetExpression);
    }

    private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression, IEnumerable<LuaExpressionSyntax> arguments, IList<ITypeSymbol> typeArguments) {
      return InternalBuildCodeTemplateExpression(codeTemplate, targetExpression, arguments.Select<LuaExpressionSyntax, Func<LuaExpressionSyntax>>(i => () => i), typeArguments);
    }

    private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression, IEnumerable<Func<LuaExpressionSyntax>> arguments, IList<ITypeSymbol> typeArguments) {
      return InternalBuildCodeTemplateExpression(codeTemplate, targetExpression, arguments, typeArguments);
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
      IList<ITypeSymbol> typeArguments,
      LuaIdentifierNameSyntax memberBindingIdentifier = null) {
      LuaCodeTemplateExpressionSyntax codeTemplateExpression = new LuaCodeTemplateExpressionSyntax();

      var matchs = codeTemplateRegex_.Matches(codeTemplate);
      int prevIndex = 0;
      foreach (Match match in matchs) {
        if (match.Index > prevIndex) {
          string prevToken = codeTemplate[prevIndex..match.Index];
          codeTemplateExpression.Expressions.Add(prevToken);
        }
        string comma = match.Groups[1].Value;
        string key = match.Groups[2].Value;
        if (key == "this") {
          AddCodeTemplateExpression(memberBindingIdentifier ?? BuildMemberAccessTargetExpression(targetExpression), comma, codeTemplateExpression);
        } else if (key == "class") {
          var type = semanticModel_.GetTypeInfo(targetExpression).Type;
          var typeName = GetTypeName(type);
          AddCodeTemplateExpression(typeName, comma, codeTemplateExpression);
        } else if (key[0] == '`') {
          if (int.TryParse(key.Substring(1), out int typeIndex)) {
            var typeArgument = typeArguments.GetOrDefault(typeIndex);
            if (typeArgument != null) {
              LuaExpressionSyntax typeName;
              if (typeArgument.TypeKind == TypeKind.Enum && codeTemplate.StartsWith("System.Enum")) {
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
            var argument = arguments?.ElementAtOrDefault(argumentIndex);
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

    private bool IsPropertyFieldOrEventFiled(ISymbol symbol) {
      return generator_.IsPropertyFieldOrEventFiled(symbol);
    }

    private bool IsMoreThanLocalVariables(ISymbol symbol) {
      return generator_.IsMoreThanLocalVariables(symbol);
    }

    private bool IsInternalMember(ISymbol symbol) {
      if (symbol.IsFromCode()) {
        bool isVirtual = symbol.IsOverridable() && !generator_.IsSealed(symbol.ContainingType);
        if (!isVirtual) {
          var typeSymbol = CurTypeSymbol;
          if (typeSymbol.IsContainsInternalSymbol(symbol) && !IsMoreThanLocalVariables(symbol) && !IsMoreThanUpvalues(symbol)) {
            return true;
          }
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
        return arrayType.MemberAccess(LuaIdentifierNameSyntax.New, true).Invocation(elements.Count, new LuaTableExpression(elements) { IsSingleLine = true });
      } else {
        return new LuaInvocationExpressionSyntax(arrayType, elements);
      }
    }

    private LuaExpressionSyntax BuildArray(LuaExpressionSyntax arrayType, LuaExpressionSyntax size) {
      return arrayType.MemberAccess(LuaIdentifierNameSyntax.New, true).Invocation(size);
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
            float v = (float)constantValue;
            return (LuaFloatLiteralExpressionSyntax)v;
          }
          case TypeCode.Double: {
            double v = (double)constantValue;
            return (LuaDoubleLiteralExpressionSyntax)v;
          }
          default: {
            return new LuaIdentifierLiteralExpressionSyntax(constantValue.ToString());
          }
        }
      } else {
        return LuaIdentifierLiteralExpressionSyntax.Nil;
      }
    }

    private LuaExpressionSyntax GetConstLiteralExpression(IFieldSymbol constField) {
      Contract.Assert(constField.HasConstantValue);
      if (constField.Type.SpecialType == SpecialType.System_Char) {
        return new LuaCharacterLiteralExpression((char)constField.ConstantValue);
      } else {
        if (constField.Type.TypeKind == TypeKind.Enum && !generator_.IsConstantEnum(constField.Type)) {
          var typeName = GetTypeName(constField.Type);
          return typeName.MemberAccess(constField.Name);
        }

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
        if (constValue.Value is double d) {
          switch (d) {
            case double.NegativeInfinity:
            case double.PositiveInfinity:
            case double.NaN:
              return null;
          }
        } else if (constValue.Value is float f) {
          switch (f) {
            case float.NegativeInfinity:
            case float.PositiveInfinity:
            case float.NaN:
              return null;
          }
        }

        var literalExpression = GetLiteralExpression(constValue.Value);
        return new LuaConstLiteralExpression(literalExpression, node.ToString());
      }
      return null;
    }

    private string DecodeUnicodeCharacter(string text) {
      if (unicodeRegex_.IsMatch(text)) {
        if (IsLuaClassic) {
          return unicodeRegex_.Replace(text, m => {
            if (short.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var c)) {
              char ch = (char)c;
              switch (ch) {
                case '\a': {
                  return "\\a";
                }
                case '\b': {
                  return "\\b";
                }
                case '\f': {
                  return "\\f";
                }
                case '\n': {
                  return "\\n";
                }
                case '\r': {
                  return "\\r";
                }
                case '\t': {
                  return "\\t";
                }
                case '\v': {
                  return "\\v";
                }
                case '\\': {
                  return "\\";
                }
                case '"': {
                  return "\\\"";
                }
              }
              return ch.ToString();
            }
            return m.Value;
          });
        } else {
          return unicodeRegex_.Replace(text, m => {
            return $@"\u{{{m.Groups[1].Value}}}";
          });
        }
      }
      return text;
    }

    private LuaLiteralExpressionSyntax BuildStringLiteralTokenExpression(SyntaxToken token) {
      if (token.Text[0] == '@') {
        return BuildVerbatimStringExpression(token.ValueText);
      } else {
        return new LuaIdentifierLiteralExpressionSyntax(DecodeUnicodeCharacter(token.Text));
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
      if (symbol.ContainingNamespace.IsRuntimeCompilerServices()) {
        return symbol.Name switch
        {
          "CallerLineNumberAttribute" => CallerAttributeKind.Line,
          "CallerMemberNameAttribute" => CallerAttributeKind.Member,
          "CallerFilePathAttribute" => CallerAttributeKind.FilePath,
          _ => CallerAttributeKind.None
        };
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
          outExpression = usingStaticType.MemberAccess(expression);
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
        switch (expression.Kind()) {
          case SyntaxKind.ObjectCreationExpression:
          case SyntaxKind.ArrayCreationExpression: {
            mayBeNull = false;
            break;
          }
          default: {
            mayBeNull = true;
            break;
          }
        }
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
        var root = methodInfo.Symbol.GetDeclaringSyntaxNode();
        if (IsLocalVarExists(name, root)) {
          return true;
        }
      }
      return false;
    }

    private static bool IsMethodTypeArgument(IMethodSymbol method, ITypeSymbol symbol) {
      if (method.TypeArguments.Length > 0) {
        return method.TypeArguments.Any(i => symbol.IsTypeParameterExists(i));
      } else if (method.MethodKind == MethodKind.LambdaMethod || method.MethodKind == MethodKind.LocalFunction) {
        return IsMethodTypeArgument((IMethodSymbol)method.ContainingSymbol, symbol);
      }
      return false;
    }

    private bool IsCurMethodTypeArgument(ITypeSymbol symbol) {
      var methodInfo = CurMethodInfoOrNull;
      if (methodInfo != null) {
        return IsMethodTypeArgument(methodInfo.Symbol, symbol);
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

    private void CheckNewPrefix(ref string newPrefix, string prefix) {
      var usingDeclare = CurCompilationUnit.UsingDeclares.Find(i => i.Prefix == prefix);
      if (usingDeclare != null) {
        newPrefix = usingDeclare.NewPrefix;
        return;
      }

      string newName = newPrefix;
      const int kMaxNameLength = 25;
      if (newPrefix.Length > kMaxNameLength) {
        string[] names = prefix.Split('.');
        if (names.Length > 2) {
          string head = names.First();
          string tail = names.Last();
          newName = head + tail;
          if (newName.Length > kMaxNameLength) {
            newName = tail;
          }
        }
      }

      int index = 0;
      while (true) {
        string result = Utility.GetNewIdentifierName(newName, index);
        if (!CurCompilationUnit.UsingDeclares.Exists(i => i.NewPrefix == result)) {
          newPrefix = result;
          return;
        }
        ++index;
      }
    }

    private bool IsMoreThanUpvalues(object nameStringOrSymbol) {
      if (IsLuaNewest) {
        return false;
      }

      const int kMaxCountUpvalues = LuaSyntaxNode.kUpvaluesMaxCount - 2;
      var current = CurFunctionOrNull;
      if (current != null) {
        var upvalues = functionUpValues_.GetOrDefault(current);
        if (upvalues != null) {
          if (!upvalues.Contains(nameStringOrSymbol)) {
            if (upvalues.Count >= kMaxCountUpvalues) {
              return true;
            }
            upvalues.Add(nameStringOrSymbol);
          }
        } else {
          upvalues = new FunctionUpValuesInfo();
          functionUpValues_.Add(current, upvalues);
          upvalues.Add(nameStringOrSymbol);
        }
      }
      return false;
    }

    private bool IsImportFunctionUpvaluesMax {
      get {
        const int kMaxCountUpvalues = LuaSyntaxNode.kUpvaluesMaxCount - 1;  // System 
        int usingDeclaresCount = CurCompilationUnit.UsingDeclares.Count(i => i.IsFromCode);
        var genericDeclaresCount = CurCompilationUnit.GenericUsingDeclares.Count(i => i.IsFromCode);
        return usingDeclaresCount + genericDeclaresCount >= kMaxCountUpvalues;
      }
    }

    private bool AddImport(string prefix, string newPrefix, bool isFromCode) {
      if (CurCompilationUnit.UsingDeclares.Exists(i => i.Prefix == prefix && i.IsFromCode == isFromCode)) {
        if (!IsLuaNewest && IsMoreThanUpvalues(newPrefix)) {
          return false;
        }
        return true;
      }

      if (!IsLuaNewest) {
        if (IsImportFunctionUpvaluesMax) {
          return false;
        }

        if (IsMoreThanUpvalues(newPrefix)) {
          return false;
        }
      }

      CurCompilationUnit.UsingDeclares.Add(new UsingDeclare() {
        Prefix = prefix,
        NewPrefix = newPrefix,
        IsFromCode = isFromCode,
      });
      return true;
    }

    internal void ImportTypeName(ref string name, INamedTypeSymbol symbol) {
      if (IsImportTypeNameEnale(symbol)) {
        int pos = name.LastIndexOf('.');
        if (pos != -1) {
          string prefix = name.Substring(0, pos);
          if (prefix != LuaIdentifierNameSyntax.System.ValueText && prefix != LuaIdentifierNameSyntax.Class.ValueText) {
            string newPrefix = prefix.Replace(".", "");
            CheckNewPrefix(ref newPrefix, prefix);
            if (!IsLocalVarExistsInCurMethod(newPrefix)) {
              bool success = AddImport(prefix, newPrefix, symbol.IsFromCode());
              if (success) {
                name = newPrefix + name.Substring(pos);
              }
            }
          }
        }
      }
    }

    internal bool AddGenericImport(LuaInvocationExpressionSyntax invocationExpression, string name, List<string> argumentTypeNames, bool isFromCode) {
      if (CurCompilationUnit.GenericUsingDeclares.Exists(i => i.NewName == name)) {
        if (!IsLuaNewest && IsMoreThanUpvalues(name)) {
          return false;
        }
        return true;
      }

      if (!IsLuaNewest) {
        if (IsImportFunctionUpvaluesMax) {
          return false;
        }

        if (IsMoreThanUpvalues(name)) {
          return false;
        }
      }

      CurCompilationUnit.GenericUsingDeclares.Add(new GenericUsingDeclare() {
        InvocationExpression = invocationExpression,
        ArgumentTypeNames = argumentTypeNames,
        NewName = name,
        IsFromCode = isFromCode
      });
      return true;
    }

    internal void ImportGenericTypeName(ref LuaExpressionSyntax luaExpression, ITypeSymbol symbol) {
      if (!IsNoImportTypeName && !CurTypeSymbol.EQ(symbol) && !IsCurMethodTypeArgument(symbol)) {
        var invocationExpression = (LuaInvocationExpressionSyntax)luaExpression;
        string newName = GetGenericTypeImportName(invocationExpression, out var argumentTypeNames);
        if (!IsLocalVarExistsInCurMethod(newName)) {
          bool success;
          if (!symbol.IsTypeParameterExists()) {
            success = AddGenericImport(invocationExpression, newName, argumentTypeNames, symbol.IsAbsoluteFromCode());
          } else {
            bool hasAdd;
            (success, hasAdd) = CurTypeDeclaration.TypeDeclaration.AddGenericImport(invocationExpression, newName, argumentTypeNames, symbol.IsAbsoluteFromCode());
            if (hasAdd) {
              generator_.AddGenericImportDepend(CurTypeDeclaration.TypeSymbol, symbol.OriginalDefinition as INamedTypeSymbol);
            }
          }
          if (success) {
            luaExpression = newName;
          }
        }
      }
    }

    private static void FillGenericTypeImportName(StringBuilder sb, List<string> argumentTypeNames, LuaInvocationExpressionSyntax invocationExpression) {
      static string CheckLastName(string lastName) {
        return lastName == "Dictionary" ? "Dict" : lastName;
      }
      var identifierName = (LuaIdentifierNameSyntax)invocationExpression.Expression;
      sb.Append(CheckLastName(identifierName.ValueText.LastName()));
      foreach (var argument in invocationExpression.ArgumentList.Arguments) {
        if (argument is LuaIdentifierNameSyntax typeName) {
          string argumentTypeName = typeName.ValueText;
          sb.Append(CheckLastName(argumentTypeName.LastName()));
          argumentTypeNames.Add(argumentTypeName);
        } else {
          FillGenericTypeImportName(sb, argumentTypeNames, (LuaInvocationExpressionSyntax)argument);
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
          if (arguments[0] == LuaIdentifierNameSyntax.This) {
            propertyMethod.ArgumentList.Arguments[0] = expression;
          }
        } else {
          propertyMethod.Update(expression, !isStatic);
        }
        return propertyMethod;
      } else {
        return expression.MemberAccess(name);
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
      int constructorIndex = GetConstructorIndex(symbol);
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
          var argumentExpression = argumentNode.Expression.AcceptExpression(this);
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
            var name = argumentNode.NameEquals.AcceptExpression(this);
            initializers.Add((name, argumentExpression));
          }
        }

        CheckInvocationDefaultArguments(symbol, symbol.Parameters, arguments, argumentNodeInfos, node, false);
        invocation.AddArguments(arguments);

        if (initializers.Count == 0) {
          return invocation;
        } else {
          LuaFunctionExpressionSyntax function = new LuaFunctionExpressionSyntax();
          PushFunction(function);
          var temp = GetTempIdentifier();
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
      var expressions = new List<LuaExpressionSyntax>();
      var attributes = attributeLists.SelectMany(i => i.Attributes);
      foreach (var node in attributes) {
        var expression = node.AcceptExpression(this);
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
        parameter = symbol.Parameters.GetOrDefault(index);
      }
      return parameter;
    }

    private void CheckValueTypeClone(ITypeSymbol typeSymbol, IdentifierNameSyntax node, ref LuaExpressionSyntax expression, bool isPropertyField = false) {
      if (typeSymbol.IsCustomValueType() && !generator_.IsReadOnlyStruct(typeSymbol)) {
        bool need = false;
        if (isPropertyField) {
          need = true;
          SyntaxNode current = node;
          while (true) {
            var parent = current.Parent;
            var kind = parent.Kind();
            if (kind.IsAssignment()) {
              var assignment = (AssignmentExpressionSyntax)parent;
              if (assignment.Left == current) {
                need = false;
              }
              break;
            } else if (kind == SyntaxKind.SimpleMemberAccessExpression) {
              current = parent;
            } else {
              break;
            }
          }
        } else {
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

              if (semanticModel_.GetSymbolInfo(argument.Parent.Parent).Symbol is IMethodSymbol symbol) {
                if (symbol.IsFromAssembly() && !symbol.ContainingType.IsCollectionType()) {
                  break;
                }

                var parameter = GetParameterSymbol(symbol, argument);
                if (parameter != null && parameter.RefKind == RefKind.In) {
                  break;
                }
              }

              need = true;
              break;
            }
            case SyntaxKind.ReturnStatement: {
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
        }

        if (need) {
          if (typeSymbol.IsNullableType()) {
            expression = LuaIdentifierNameSyntax.NullableClone.Invocation(expression);
          } else {
            expression = expression.MemberAccess(LuaIdentifierNameSyntax.Clone, true).Invocation();
          }
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

    private LuaExpressionSyntax BuildInheritTypeName(ITypeSymbol baseType) {
      ++noImportTypeNameCounter_;
      var baseTypeName = GetTypeName(baseType);
      --noImportTypeNameCounter_;
      return baseTypeName;
    }

    private LuaExpressionSyntax GetRecordInerfaceTypeName(INamedTypeSymbol recordType) {
      return BuildInheritTypeName(recordType.Interfaces[0]);
    }

    public override LuaSyntaxNode VisitTypeParameterList(TypeParameterListSyntax node) {
      var parameterList = new LuaParameterListSyntax();
      foreach (var typeParameter in node.Parameters) {
        var typeIdentifier = typeParameter.Accept<LuaIdentifierNameSyntax>(this);
        parameterList.Parameters.Add(typeIdentifier);
      }
      return parameterList;
    }

    private void FillExternalTypeParameters(List<LuaIdentifierNameSyntax> typeParameters, INamedTypeSymbol typeSymbol) {
      var externalType = typeSymbol.ContainingType;
      if (externalType != null) {
        FillExternalTypeParameters(typeParameters, externalType);
        foreach (var typeParameterSymbol in externalType.TypeParameters) {
          LuaIdentifierNameSyntax identifierName = typeParameterSymbol.Name;
          typeParameters.Add(identifierName);
        }
      }
    }

    private void BuildTypeParameters(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax node, LuaTypeDeclarationSyntax typeDeclaration) {
      var typeParameters = new List<LuaIdentifierNameSyntax>();
      FillExternalTypeParameters(typeParameters, typeSymbol);
      if (node.TypeParameterList != null) {
        var parameterList = node.TypeParameterList.Accept<LuaParameterListSyntax>(this);
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
        if (generator_.IsFromLuaModule(symbol)) {
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
      var luaExpression = node.AcceptExpression(this);
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
      if (CurTypeSymbol.EQ(methodSymbol.ContainingType)) {
        return methodName;
      }

      if (CurTypeSymbol.IsContainsInternalSymbol(methodSymbol)) {
        if (CurTypeSymbol.GetMembers(methodSymbol.Name).IsEmpty) {
          return methodName;
        }
      }

      var typeName = GetTypeName(methodSymbol.ContainingType);
      return typeName.MemberAccess(methodName);
    }

    private LuaExpressionSyntax BuildConversionExpression(IMethodSymbol methodSymbol, LuaExpressionSyntax expression) {
      var codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(methodSymbol);
      if (codeTemplate != null) {
        return BuildCodeTemplateExpression(codeTemplate, null, new LuaExpressionSyntax[] { expression }, null);
      }

      var memberAccess = GetOperatorMemberAccessExpression(methodSymbol);
      return new LuaInvocationExpressionSyntax(memberAccess, expression);
    }

    private LuaExpressionSyntax GetUserDefinedOperatorExpression(IMethodSymbol methodSymbol, ExpressionSyntax arguemt) {
      return GetUserDefinedOperatorExpression(methodSymbol, new Func<LuaExpressionSyntax>[] {
        () => VisitExpression(arguemt),
      });
    }

    private LuaExpressionSyntax GetUserDefinedOperatorExpression(IMethodSymbol methodSymbol, ExpressionSyntax left, ExpressionSyntax right) {
      return GetUserDefinedOperatorExpression(methodSymbol, new Func<LuaExpressionSyntax>[] {
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

    private bool IsUserDefinedOperator(ExpressionSyntax node, out IMethodSymbol methodSymbol) {
      methodSymbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node).Symbol;
      if (methodSymbol != null) {
        var typeSymbol = methodSymbol.ContainingType;
        if (typeSymbol != null) {
          if (typeSymbol.TypeKind != TypeKind.Enum
            && typeSymbol.TypeKind != TypeKind.Delegate
            && (typeSymbol.SpecialType == SpecialType.None || typeSymbol.SpecialType == SpecialType.System_DateTime)) {
            return true;
          }
        }
      }
      return false;
    }

    private LuaExpressionSyntax GetUserDefinedOperatorExpression(IMethodSymbol methodSymbol, IEnumerable<Func<LuaExpressionSyntax>> arguments) {
      var codeTemplate = XmlMetaProvider.GetMethodCodeTemplate(methodSymbol);
      if (codeTemplate != null) {
        return InternalBuildCodeTemplateExpression(codeTemplate, null, arguments, null);
      }
      var memberAccess = GetOperatorMemberAccessExpression(methodSymbol);
      return new LuaInvocationExpressionSyntax(memberAccess, arguments.Select(i => i()));
    }

    private LuaExpressionSyntax GetUserDefinedOperatorExpression(ExpressionSyntax node, IEnumerable<Func<LuaExpressionSyntax>> arguments) {
      if (IsUserDefinedOperator(node, out var methodSymbol)) {
        return GetUserDefinedOperatorExpression(methodSymbol, arguments);
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

    private sealed class SymbolAssignmentSearcher : LuaSyntaxSearcher {
      private readonly LuaSyntaxGenerator generator_;
      private readonly ISymbol symbol_;
      private readonly HashSet<IMethodSymbol> methods_ = new HashSet<IMethodSymbol>();

      public SymbolAssignmentSearcher(LuaSyntaxGenerator generator, ISymbol symbol) {
        generator_ = generator;
        symbol_ = symbol;
      }

      public override void VisitAssignmentExpression(AssignmentExpressionSyntax node) {
        var semanticModel = generator_.GetSemanticModel(node.SyntaxTree);
        var symbol = semanticModel.GetSymbolInfo(node.Left).Symbol;
        if (symbol_.EQ(symbol)) {
          Found();
        }

        if (node.Right is AssignmentExpressionSyntax) {
          Visit(node.Right);
        }
      }

      public override void VisitInvocationExpression(InvocationExpressionSyntax node) {
        var semanticModel = generator_.GetSemanticModel(node.SyntaxTree);

        switch (symbol_.Kind) {
          case SymbolKind.Local:
          case SymbolKind.Parameter:
          case SymbolKind.Field: {
            foreach (var argument in node.ArgumentList.Arguments) {
              if (argument.RefKindKeyword.IsOutOrRef()) {
                var symbol = semanticModel.GetSymbolInfo(argument.Expression).Symbol;
                if (symbol.EQ(symbol_)) {
                  Found();
                }
              }
            }
            break;
          }
        }

        if (symbol_.Kind != SymbolKind.Local && symbol_.Kind != SymbolKind.Parameter) {
          var methodSymbol = (IMethodSymbol)semanticModel.GetSymbolInfo(node).Symbol;
          if (methodSymbol != null && !methods_.Contains(methodSymbol)) {
            methods_.Add(methodSymbol);
            var declaringNode = methodSymbol.GetDeclaringSyntaxNode();
            if (declaringNode != null) {
              Visit(declaringNode);
            }
          }
        }
      }
    }

    private bool IsSymbolAssignmentExists(ISymbol symbol, SyntaxNode root) {
      return new SymbolAssignmentSearcher(generator_, symbol).Find(root);
    }

    private bool IsMemberAccessExpressionAssignmentExists(MemberAccessExpressionSyntax memberAccess, SyntaxNode node) {
      bool isExists = false;
      ExpressionSyntax expression = memberAccess.Expression;
      var expressionSymbol = semanticModel_.GetSymbolInfo(expression).Symbol;
      if (expressionSymbol != null) {
        isExists = IsSymbolAssignmentExists(expressionSymbol, node);
        if (!isExists && expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
          isExists = IsMemberAccessExpressionAssignmentExists((MemberAccessExpressionSyntax)expression, node);
        }
      }
      return isExists;
    }

    private bool IsFixedValueSymbol(ISymbol symbol, ExpressionSyntax expression, ForStatementSyntax node) {
      bool isReadOnly;
      if (IsSymbolAssignmentExists(symbol, node)) {
        isReadOnly = false;
      } else if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
        var memberAccess = (MemberAccessExpressionSyntax)expression;
        isReadOnly = !IsMemberAccessExpressionAssignmentExists(memberAccess, node);
      } else {
        isReadOnly = false;
      }
      return isReadOnly;
    }

    private bool IsFixedValueExpression(ExpressionSyntax expression, ForStatementSyntax node, out Optional<object> limitConst) {
      limitConst = semanticModel_.GetConstantValue(expression);
      if (limitConst.HasValue) {
        return true;
      }

      if (expression is BinaryExpressionSyntax binaryExpression) {
        Optional<object> _ = default;
        return IsFixedValueExpression(binaryExpression.Right, node, out _) && IsFixedValueExpression(binaryExpression.Left, node, out _);
      } else {
        bool isReadOnly = false;
        var symbol = semanticModel_.GetSymbolInfo(expression).Symbol;
        if (symbol != null) {
          switch (symbol.Kind) {
            case SymbolKind.Local:
            case SymbolKind.Parameter: {
              isReadOnly = !IsSymbolAssignmentExists(symbol, node);
              break;
            }
            case SymbolKind.Field: {
              var fieldSymbol = (IFieldSymbol)symbol;
              isReadOnly = fieldSymbol.IsReadOnly || IsFixedValueSymbol(symbol, expression, node);
              break;
            }
            case SymbolKind.Property: {
              var propertySymbol = (IPropertySymbol)symbol;
              isReadOnly = (propertySymbol.IsReadOnly && IsPropertyField(propertySymbol)) || IsFixedValueSymbol(symbol, expression, node);
              break;
            }
          }
        }
        return isReadOnly;
      }
    }

    private LuaExpressionSyntax GetFixedValueExpression(ExpressionSyntax expression, ForStatementSyntax node) {
      if (IsFixedValueExpression(expression, node, out var constantValue)) {
        if (constantValue.HasValue) {
          if (constantValue.Value is char c) {
            return c;
          }

          return Convert.ToDouble(constantValue.Value);
        }
        return expression.AcceptExpression(this);
      } else {
        return null;
      }
    }

    private sealed class ClosureVariableSearcher : LuaSyntaxSearcher {
      private readonly ISymbol symbol_;
      private readonly LuaSyntaxGenerator generator_;
      private int closureCounter_;

      public ClosureVariableSearcher(ISymbol symbol, LuaSyntaxGenerator generator) {
        symbol_ = symbol;
        generator_ = generator;
      }

      public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) {
        ++closureCounter_;
        base.VisitParenthesizedLambdaExpression(node);
        --closureCounter_;
      }

      public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) {
        ++closureCounter_;
        base.VisitSimpleLambdaExpression(node);
        --closureCounter_;
      }

      public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) {
        ++closureCounter_;
        base.VisitLocalFunctionStatement(node);
        --closureCounter_;
      }

      public override void VisitIdentifierName(IdentifierNameSyntax node) {
        if (closureCounter_ > 0) {
          var semanticModel = generator_.GetSemanticModel(node.SyntaxTree);
          var symbol = semanticModel.GetSymbolInfo(node).Symbol;
          if (symbol_.EQ(symbol)) {
            Found();
          }
        }
      }
    }

    private bool IsClosureVariableExists(ISymbol symbol, SyntaxNode root) {
      return new ClosureVariableSearcher(symbol, generator_).Find(root);
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

      var limitExpression = GetFixedValueExpression(condition.Right, node);
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

          stepExpression = GetFixedValueExpression(assignment.Right, node);
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

      var variableSybmol = (ILocalSymbol)semanticModel_.GetDeclaredSymbol(variable);
      if (IsClosureVariableExists(variableSybmol, node.Statement)) {
        goto Fail;
      }

      LuaIdentifierNameSyntax identifier = variable.Identifier.ValueText;
      CheckLocalVariableName(ref identifier, variable);

      var startExpression = variable.Initializer.Value.AcceptExpression(this);
      if (hasNoEqual) {
        if (limitExpression is LuaNumberLiteralExpressionSyntax limitLiteral) {
          if (stepExpression is LuaNumberLiteralExpressionSyntax numberLiteral) {
            limitExpression = new LuaIdentifierLiteralExpressionSyntax((limitLiteral.Number - numberLiteral.Number).ToString());
          } else {
            limitExpression = limitExpression.Sub(stepExpression);
          }
        } else {
          if (stepExpression is LuaNumberLiteralExpressionSyntax numberLiteral) {
            if (numberLiteral.Number > 0) {
              limitExpression = limitExpression.Sub(stepExpression);
            } else {
              limitExpression = limitExpression.Plus((-numberLiteral.Number).ToString());
            }
          } else {
            limitExpression = limitExpression.Sub(stepExpression);
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
      return BuildValueTupleCreateExpression(elementTypes.Select(GetDefaultValueExpression));
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

    private LuaExpressionSyntax BuildDeconstructExpression(LuaExpressionSyntax expression, LuaExpressionSyntax methodName) {
      return expression.MemberAccess(methodName, true).Invocation();
    }

    private LuaExpressionSyntax BuildDeconstructExpression(ITypeSymbol typeSymbol, LuaExpressionSyntax expression, SyntaxNode node) {
      if (!typeSymbol.IsTupleType && !typeSymbol.IsSystemTuple()) {
        if (node.Parent is AssignmentExpressionSyntax assignment) {
          var methodSymbol = semanticModel_.GetDeconstructionInfo(assignment).Method;
          if (methodSymbol.IsExtensionMethod) {
            return BuildExtensionMethodInvocation(methodSymbol, expression);
          } else {
            var methodName = GetMemberName(methodSymbol);
            return BuildDeconstructExpression(expression, methodName);
          }
        }
      }
      return BuildDeconstructExpression(expression, LuaIdentifierNameSyntax.Deconstruct);
    }

    private LuaExpressionSyntax BuildDeconstructExpression(ExpressionSyntax node, LuaExpressionSyntax expression) {
      var typeSymbol = semanticModel_.GetTypeInfo(node).Type;
      return BuildDeconstructExpression(typeSymbol, expression, node);
    }

    public int GetConstructorIndex(IMethodSymbol symbool) {
      Contract.Assert(symbool.MethodKind == MethodKind.Constructor);
      if (generator_.IsFromLuaModule(symbool.ContainingType)) {
        var typeSymbol = (INamedTypeSymbol)symbool.ReceiverType;
        if (typeSymbol.InstanceConstructors.Length > 1) {
          var ctors = typeSymbol.InstanceConstructors.ToList();
          int firstCtorIndex;
          if (typeSymbol.IsValueType) {
            Contract.Assert(ctors.Last().IsImplicitlyDeclared);
            firstCtorIndex = ctors.IndexOf(i => i.IsNotNullParameterExists());
            if (firstCtorIndex == -1) {
              firstCtorIndex = ctors.Count - 1;
            } else if (symbool.IsImplicitlyDeclared) {
              return 1;
            }
          } else {
            if (typeSymbol.IsRecordType()) {
              int posIndex = ctors.FindIndex(i => i.Parameters.Length == 1 && i.Parameters[0].Type.EQ(typeSymbol));
              Contract.Assert(posIndex != -1);
              ctors.RemoveAt(posIndex);
              if (ctors.Count <= 1) {
                return 0;
              }
            }
            firstCtorIndex = ctors.IndexOf(i => i.Parameters.IsEmpty);
          }
          if (firstCtorIndex != -1 && firstCtorIndex != 0) {
            var firstCtor = ctors[firstCtorIndex];
            ctors.Remove(firstCtor);
            ctors.Insert(0, firstCtor);
          }
          int index = ctors.IndexOf(symbool);
          Contract.Assert(index != -1);
          int ctroCounter = index + 1;
          return ctroCounter;
        }
      }
      return 0;
    }

    private sealed class YieldStatementSearcher : LuaSyntaxSearcher {
      public override void VisitYieldStatement(YieldStatementSyntax node) {
        Found();
      }
    }

    private bool IsYieldStatementExists(SyntaxNode root) {
      return new YieldStatementSearcher().Find(root);
    }

    private sealed class RecursionCallSearcher : LuaSyntaxSearcher {
      private readonly LuaSyntaxGenerator generator_;
      private readonly IMethodSymbol symbol_;

      public RecursionCallSearcher(LuaSyntaxGenerator generator, IMethodSymbol symbol) {
        generator_ = generator;
        symbol_ = symbol;
      }

      public override void VisitInvocationExpression(InvocationExpressionSyntax node) {
        var semanticModel = generator_.GetSemanticModel(node.SyntaxTree);

        var methodSymbol = (IMethodSymbol)semanticModel.GetSymbolInfo(node).Symbol;
        if (methodSymbol != null) {
          if (methodSymbol.EQ(symbol_)) {
            Found();
          }
        }
      }
    }

    private bool IsRecursionExists(IMethodSymbol symbol, SyntaxNode root) {
      return new RecursionCallSearcher(generator_, symbol).Find(root);
    }

    private bool InliningInvocationExpression(SyntaxNode root, IMethodSymbol symbol, Func<LuaInvocationExpressionSyntax> invocationFn, out LuaExpressionSyntax inlineExpression) {
      if (symbol.IsOverridable()) {
        goto Fail;
      }

      if (symbol.IsAsync) {
        goto Fail;
      }

      SyntaxNode declarationNode;
      ParameterListSyntax parameterList;
      BlockSyntax bodyNode;
      ArrowExpressionClauseSyntax expressionBodyNode;
      if (symbol.MethodKind == MethodKind.PropertyGet) {
        if (symbol.AssociatedSymbol.GetDeclaringSyntaxNode() is not PropertyDeclarationSyntax propertyDeclaration) {
          goto Fail;
        }

        if (propertyDeclaration.AccessorList != null) {
          var accessor = propertyDeclaration.AccessorList.Accessors.First(i => i.IsKind(SyntaxKind.GetAccessorDeclaration));
          declarationNode = accessor;
          bodyNode = accessor.Body;
          expressionBodyNode = accessor.ExpressionBody;
        } else if (propertyDeclaration.ExpressionBody != null) {
          declarationNode = propertyDeclaration.ExpressionBody;
          bodyNode = null;
          expressionBodyNode = propertyDeclaration.ExpressionBody;
        } else {
          goto Fail;
        }
        parameterList = null;
      } else {
        if (symbol.GetDeclaringSyntaxNode() is not MethodDeclarationSyntax methodDeclaration) {
          goto Fail;
        }

        declarationNode = methodDeclaration;
        bodyNode = methodDeclaration.Body;
        expressionBodyNode = methodDeclaration.ExpressionBody;
        parameterList = methodDeclaration.ParameterList;
      }

      if (IsYieldStatementExists(declarationNode)) {
        goto Fail;
      }

      if (IsRecursionExists(symbol, declarationNode)) {
        goto Fail;
      }

      var invocation = invocationFn();
      List<LuaExpressionSyntax> refOrOutParameters = new List<LuaExpressionSyntax>();
      MethodInfo methodInfo = new MethodInfo(symbol, refOrOutParameters) {
        InliningReturnVars = new List<LuaIdentifierNameSyntax>(),
      };
      int prevFunctionTempCount = CurFunction.TempCount;
      int prevBlockTempCount = CurBlock.TempCount;
      if (!symbol.ReturnsVoid) {
        methodInfo.InliningReturnVars.Add(GetTempIdentifier());
      }
      methodInfos_.Push(methodInfo);

      bool isThisMemberAccess = false;
      var block = new LuaBlockStatementSyntax();
      PushBlock(block);
      if (invocation.Expression is LuaMemberAccessExpressionSyntax memberAccess) {
        if (memberAccess.IsObjectColon) {
          var thisLocal = new LuaLocalVariableDeclaratorSyntax(LuaIdentifierNameSyntax.This, memberAccess.Expression);
          block.AddStatement(thisLocal);
          isThisMemberAccess = true;
        }
      }

      if (parameterList != null && parameterList.Parameters.Count > 0) {
        var parameters = new List<LuaIdentifierNameSyntax>();
        foreach (var parameterNode in parameterList.Parameters) {
          var parameter = parameterNode.Accept<LuaIdentifierNameSyntax>(this);
          if (parameter != LuaIdentifierNameSyntax.This) {
            parameters.Add(parameter);
            if (parameterNode.Modifiers.IsOutOrRef()) {
              refOrOutParameters.Add(parameter);
              methodInfo.InliningReturnVars.Add(GetTempIdentifier());
            }
          }
        }
        var parameterValues = invocation.ArgumentList.Arguments.Where(i => i != LuaIdentifierNameSyntax.This);
        block.AddStatement(new LuaLocalVariablesSyntax(parameters, parameterValues));
      }

      var prevSemanticModel_ = semanticModel_;
      semanticModel_ = generator_.GetSemanticModel(declarationNode.SyntaxTree);
      if (bodyNode != null) {
        var body = (LuaBlockSyntax)Visit(bodyNode);
        block.Statements.AddRange(body.Statements);
      } else {
        var assignment = new LuaMultipleAssignmentExpressionSyntax();
        assignment.Lefts.AddRange(methodInfo.InliningReturnVars);
        var expression = expressionBodyNode.AcceptExpression(this);
        if (symbol.ReturnsVoid) {
          block.AddStatement(expression);
        } else {
          assignment.Rights.Add(expression);
        }
        assignment.Rights.AddRange(refOrOutParameters);
        if (assignment.Lefts.Count > 0) {
          if (assignment.Lefts.Count == 1) {
            Contract.Assert(assignment.Rights.Count == 1);
            block.AddStatement(assignment.Lefts.First().Assignment(assignment.Rights.First()));
          } else {
            block.AddStatement(assignment);
          }
        }
      }
      semanticModel_ = prevSemanticModel_;

      PopBlock();
      methodInfos_.Pop();
      generator_.AddInlineSymbol(symbol);

      inlineExpression = CompressionInliningBlock(root, block, methodInfo, isThisMemberAccess);
      if (inlineExpression != null) {
        CurFunction.TempCount = prevFunctionTempCount;
        CurBlock.TempCount = prevBlockTempCount;
        return true;
      }

      CurBlock.AddStatement(new LuaShortCommentStatement($" inline {symbol}"));
      if (methodInfo.InliningReturnVars.Count > 0) {
        CurBlock.AddStatement(new LuaLocalVariablesSyntax(methodInfo.InliningReturnVars));
      }

      if (block.Statements.Count == 1) {
        CurBlock.AddStatement(block.Statements.First());
      } else {
        CurBlock.AddStatement(block);
      }

      if (methodInfo.InliningReturnVars.Count > 0) {
        inlineExpression = new LuaSequenceListExpressionSyntax(methodInfo.InliningReturnVars);
        if (refOrOutParameters.Count == 0 && root.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
          inlineExpression = LuaExpressionSyntax.EmptyExpression;
        }
      } else {
        inlineExpression = LuaExpressionSyntax.EmptyExpression;
      }
      return true;

    Fail:
      inlineExpression = null;
      return false;
    }

    private static bool InliningMemberAccessUpdateTarget(LuaMemberAccessExpressionSyntax memberAccess, LuaExpressionSyntax target) {
      if (memberAccess.Expression == LuaIdentifierNameSyntax.This) {
        memberAccess.UpdateExpression(target);
        return true;
      } else if (memberAccess.Expression is LuaMemberAccessExpressionSyntax accessExpression) {
        return InliningMemberAccessUpdateTarget(accessExpression, target);
      } else if (memberAccess.Expression is LuaPropertyAdapterExpressionSyntax propertyAdapter && propertyAdapter.IsProperty) {
        return InlinePropertyAdapterUpdateTarget(propertyAdapter, target);
      }
      return false;
    }

    private static bool InlinePropertyAdapterUpdateTarget(LuaPropertyAdapterExpressionSyntax propertyAdapter, LuaExpressionSyntax target) {
      if (propertyAdapter.Expression == LuaIdentifierNameSyntax.This) {
        propertyAdapter.Update(target);
        return true;
      } else if (propertyAdapter.Expression is LuaMemberAccessExpressionSyntax proertyExpression) {
        return InliningMemberAccessUpdateTarget(proertyExpression, target);
      }
      return false;
    }

    private LuaExpressionSyntax CompressionInliningBlock(SyntaxNode root, LuaBlockStatementSyntax block, MethodInfo methodInfo, bool isThisMemberAccess) {
      if (methodInfo.InliningReturnVars.Count != 1) {
        return null;
      }

      if (isThisMemberAccess) {
        if (block.Statements.Count != 2) {
          return null;
        }
      } else {
        if (block.Statements.Count != 1) {
          return null;
        }
      }

      if (block.Statements.Last() is not LuaExpressionStatementSyntax expressionStatement) {
        return null;
      }

      if (expressionStatement.Expression is not LuaAssignmentExpressionSyntax assignment) {
        return null;
      }

      if (assignment.Left != methodInfo.InliningReturnVars.First()) {
        return null;
      }

      var expression = assignment.Right;
      if (isThisMemberAccess) {
        if (expression is LuaLiteralExpressionSyntax) {
          expression = expression.Parenthesized();
        } else {
          var thisLocal = (LuaLocalVariableDeclaratorSyntax)block.Statements.First();
          var target = thisLocal.Declarator.Initializer.Value;
          if (expression is LuaMemberAccessExpressionSyntax memberAccess) {
            if (!InliningMemberAccessUpdateTarget(memberAccess, target)) {
              return null;
            }
          } else if (expression is LuaPropertyAdapterExpressionSyntax propertyAdapter && propertyAdapter.IsProperty) {
            if (!InlinePropertyAdapterUpdateTarget(propertyAdapter, target)) {
              return null;
            }
          } else {
            return null;
          }
        }
      } else {
        if (expression is LuaBinaryExpressionSyntax) {
          if (!root.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
            if (!root.Parent.IsKind(SyntaxKind.Argument) && !(root.Parent is AssignmentExpressionSyntax)) {
              expression = expression.Parenthesized();
            }
          }
        }
      }
      if (root.Parent.IsKind(SyntaxKind.ExpressionStatement)) {
        expression = LuaIdentifierNameSyntax.Placeholder.Assignment(expression);
      }
      return expression;
    }

    private bool InliningInvocationExpression(SyntaxNode root, IMethodSymbol symbol, LuaInvocationExpressionSyntax invocation, out LuaExpressionSyntax inlineExpression) {
      return InliningInvocationExpression(root, symbol, () => invocation, out inlineExpression);
    }

    private bool InliningPropertyGetExpression(IdentifierNameSyntax root, IMethodSymbol symbol, out LuaExpressionSyntax inlineExpression) {
      return InliningInvocationExpression(root, symbol, () => {
        LuaExpressionSyntax expression = symbol.Name;
        if (root.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
          var memberAccess = (MemberAccessExpressionSyntax)root.Parent;
          if (memberAccess.Name == root) {
            var target = memberAccess.Expression.AcceptExpression(this);
            expression = target.MemberAccess(symbol.Name, !symbol.IsStatic);
          }
        }
        var invocation = new LuaInvocationExpressionSyntax(expression);
        return invocation;
      }, out inlineExpression);
    }

    private bool IsWantInline(IPropertySymbol symbol) {
      if (symbol.SetMethod != null) {
        return false;
      }

      if (symbol.GetMethod == null) {
        return false;
      }

      if (symbol.IsOverridable()) {
        return false;
      }

      var get = symbol.GetMethod;
      if (get.HasNoInliningAttribute()) {
        return false;
      }

      if (get.HasAggressiveInliningAttribute()) {
        return true;
      }

      if (!generator_.Setting.IsInlineSimpleProperty) {
        return false;
      }

      if (symbol.GetDeclaringSyntaxNode() is not PropertyDeclarationSyntax propertyDeclaration) {
        return false;
      }

      ExpressionSyntax expressionBody;
      if (propertyDeclaration.AccessorList != null) {
        var accessor = propertyDeclaration.AccessorList.Accessors.First(i => i.IsKind(SyntaxKind.GetAccessorDeclaration));
        if (accessor.Body != null) {
          if (accessor.Body.Statements.Count > 1) {
            return false;
          }

          if (accessor.Body.Statements.First() is not ReturnStatementSyntax returnStatement) {
            return false;
          }
          expressionBody = returnStatement.Expression;
        } else {
          expressionBody = accessor.ExpressionBody.Expression;
        }
      } else if (propertyDeclaration.ExpressionBody != null) {
        expressionBody = propertyDeclaration.ExpressionBody.Expression;
      } else {
        return false;
      }

      var kind = expressionBody.Kind();
      switch (kind) {
        case SyntaxKind.IdentifierName: {
          var semanticModel = generator_.GetSemanticModel(expressionBody.SyntaxTree);
          var identifierSymbol = semanticModel.GetSymbolInfo(expressionBody).Symbol;
          if (identifierSymbol != null && identifierSymbol.IsStatic && identifierSymbol.IsPrivate()) {
            return false;
          }
          break;
        }
        case SyntaxKind.SimpleMemberAccessExpression: {
          break;
        }
        default: {
          if (kind >= SyntaxKind.NumericLiteralExpression && kind <= SyntaxKind.DefaultLiteralExpression) {
            break;
          }
          return false;
        }
      }

      return true;
    }

    private void AddRefInvocationVariableMapping(InvocationExpressionSyntax expression, LuaInvocationExpressionSyntax invocation, VariableDeclaratorSyntax node) {
      var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(expression).Symbol;
      var methodDeclaration = (MethodDeclarationSyntax)symbol.GetDeclaringSyntaxNode();
      RefExpressionSyntax refExpression;
      if (methodDeclaration.Body != null) {
        var lastReturn = (ReturnStatementSyntax)methodDeclaration.Body.Statements.Last();
        refExpression = (RefExpressionSyntax)lastReturn.Expression;
      } else {
        refExpression = (RefExpressionSyntax)methodDeclaration.ExpressionBody.Expression;
      }
      var fieldSymbol = (IFieldSymbol)semanticModel_.GetSymbolInfo(refExpression.Expression).Symbol;
      if (fieldSymbol == null) {
        return;
      }

      var name = GetMemberName(fieldSymbol);
      LuaIdentifierNameSyntax target;
      if (invocation.Expression is LuaMemberAccessExpressionSyntax memberAccess) {
        if (memberAccess.Expression is LuaIdentifierNameSyntax targetName) {
          target = targetName;
        } else {
          var temp = GetTempIdentifier();
          CurBlock.AddStatement(new LuaLocalVariableDeclaratorSyntax(temp, memberAccess.Expression));
          memberAccess.UpdateExpression(temp);
          target = temp;
        }
      } else {
        target = LuaIdentifierNameSyntax.This;
      }

      AddLocalVariableMapping(new LuaSymbolNameSyntax(target.MemberAccess(name)), node);
    }
  }
}
