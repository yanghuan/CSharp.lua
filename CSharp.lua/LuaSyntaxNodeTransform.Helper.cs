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
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed partial class LuaSyntaxNodeTransform {
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

    private string GetUniqueIdentifier(string name, SyntaxNode node, int index = 0) {
      var root = FindParent<BaseMethodDeclarationSyntax>(node);
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
        identifierName = new LuaIdentifierNameSyntax(name);
        AddLocalVariableMapping(identifierName, node);
      }
    }

    private void CheckLocalSymbolName(ISymbol symbol, ref LuaIdentifierNameSyntax name) {
      var newName = localReservedNames_.GetOrDefault(symbol);
      if (newName != null) {
        name = newName;
      }
    }

    private int GetConstructorIndex(IMethodSymbol constructorSymbol) {
      if (constructorSymbol.ContainingType.IsFromCode()) {
        var typeSymbol = (INamedTypeSymbol)constructorSymbol.ReceiverType;
        var ctors = typeSymbol.Constructors.Where(i => !i.IsStatic).ToList();
        if (ctors.Count > 1) {
          int firstCtorIndex = ctors.IndexOf(i => i.Parameters.IsEmpty);
          if (firstCtorIndex != -1 && firstCtorIndex != 0) {
            var firstCtor = ctors[firstCtorIndex];
            ctors.Remove(firstCtor);
            ctors.Insert(0, firstCtor);
          }
          int index = ctors.IndexOf(constructorSymbol);
          Contract.Assert(index != -1);
          int ctroCounter = index + 1;
          return ctroCounter;
        }
      }
      return 0;
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
      return InternalBuildCodeTemplateExpression(codeTemplate, targetExpression, arguments.Select<ExpressionSyntax, Func<LuaExpressionSyntax>>(i => () => (LuaExpressionSyntax)i.Accept(this)), typeArguments);
    }

    private void AddCodeTemplateExpression(LuaExpressionSyntax expression, string comma, LuaCodeTemplateExpressionSyntax codeTemplateExpression) {
      if (!string.IsNullOrEmpty(comma)) {
        codeTemplateExpression.Expressions.Add(new LuaIdentifierNameSyntax(comma));
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
          codeTemplateExpression.Expressions.Add(new LuaIdentifierNameSyntax(prevToken));
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
        codeTemplateExpression.Expressions.Add(new LuaIdentifierNameSyntax(last));
      }

      return codeTemplateExpression;
    }

    private void AddExportEnum(ITypeSymbol enumType) {
      Contract.Assert(enumType.TypeKind == TypeKind.Enum);
      generator_.AddExportEnum(enumType.ToString());
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
        if (typeSymbol.Equals(symbol.ContainingType)) {
          return true;
        }
      }
      return false;
    }

    private LuaInvocationExpressionSyntax BuildEmptyArray(LuaExpressionSyntax baseType) {
      return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.ArrayEmpty, baseType);
    }

    private LuaInvocationExpressionSyntax BuildArray(ITypeSymbol elementType, params LuaExpressionSyntax[] elements) {
      IEnumerable<LuaExpressionSyntax> expressions = elements;
      return BuildArray(elementType, expressions);
    }

    private LuaInvocationExpressionSyntax BuildArray(ITypeSymbol elementType, IEnumerable<LuaExpressionSyntax> elements) {
      LuaExpressionSyntax baseType = GetTypeName(elementType);
      var arrayType = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Array, baseType);
      return new LuaInvocationExpressionSyntax(arrayType, elements);
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

    private CallerAttributeKind GetCallerAttributeKind(INamedTypeSymbol typeSymbol) {
      switch (typeSymbol.ToString()) {
        case "System.Runtime.CompilerServices.CallerLineNumberAttribute":
          return CallerAttributeKind.Line;
        case "System.Runtime.CompilerServices.CallerMemberNameAttribute":
          return CallerAttributeKind.Member;
        case "System.Runtime.CompilerServices.CallerFilePathAttribute":
          return CallerAttributeKind.FilePath;
        default:
          return CallerAttributeKind.None;
      }
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
            return new LuaIdentifierNameSyntax(lineSpan.StartLinePosition.Line + 1);
          }
        case CallerAttributeKind.Member: {
            var parentMethod = (MethodDeclarationSyntax)FindParent(node, SyntaxKind.MethodDeclaration);
            return new LuaStringLiteralExpressionSyntax(new LuaIdentifierNameSyntax(parentMethod.Identifier.ValueText));
          }
        case CallerAttributeKind.FilePath: {
            return BuildStringLiteralExpression(generator_.RemoveBaseFolder(node.SyntaxTree.FilePath));
          }
        default:
          return null;
      }
    }

    private bool CheckUsingStaticNameSyntax(ISymbol symbol, NameSyntax node, LuaExpressionSyntax expression, out LuaMemberAccessExpressionSyntax outExpression) {
      if (!node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)) {
        if (symbol.ContainingType != CurTypeSymbol) {           //using static
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

    internal void ImportTypeName(ref string name, ISymbol symbol) {
      if (!IsGetInheritTypeName) {
        int pos = name.LastIndexOf('.');
        if (pos != -1) {
          string prefix = name.Substring(0, pos);
          if (prefix != LuaIdentifierNameSyntax.System.ValueText && prefix != LuaIdentifierNameSyntax.Class.ValueText) {
            string newPrefix = prefix.Replace(".", "");
            var methodInfo = CurMethodInfoOrNull;
            if (methodInfo != null) {
              var syntaxReference = methodInfo.Symbol.DeclaringSyntaxReferences.First();
              var root = syntaxReference.GetSyntax();
              if (IsLocalVarExists(newPrefix, root)) {
                return;
              }
            }
            name = newPrefix + name.Substring(pos);
            CurCompilationUnit.AddImport(prefix, newPrefix, symbol.IsFromCode());
          }
        }
      }
    }

    private LuaIdentifierNameSyntax GetTypeShortName(ISymbol symbol) {
      return generator_.GetTypeShortName(symbol, this);
    }

    private LuaExpressionSyntax GetTypeName(ISymbol symbol) {
      return generator_.GetTypeName(symbol, this);
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
      int constructorIndex = GetConstructorIndex(symbol);
      if (constructorIndex > 0) {
        expression = new LuaMemberAccessExpressionSyntax(expression, LuaIdentifierNameSyntax.New, true);
      }
      LuaInvocationExpressionSyntax invocationExpression = new LuaInvocationExpressionSyntax(expression);
      if (constructorIndex > 0) {
        invocationExpression.AddArgument(new LuaIdentifierNameSyntax(constructorIndex));
      }
      return invocationExpression;
    }

    public override LuaSyntaxNode VisitAttribute(AttributeSyntax node) {
      var symbol = (IMethodSymbol)semanticModel_.GetSymbolInfo(node.Name).Symbol;
      INamedTypeSymbol typeSymbol = symbol.ContainingType;
      if (!generator_.IsExportAttribute(typeSymbol)) {
        return null;
      }

      INamedTypeSymbol typeDeclarationSymbol = CurTypeSymbol;
      generator_.AddTypeDeclarationAttribute(typeDeclarationSymbol, typeSymbol);

      ++inheritNameNodeCounter_;
      var expression = GetTypeName(typeSymbol);
      --inheritNameNodeCounter_;
      LuaInvocationExpressionSyntax invocation = BuildObjectCreationInvocation(symbol, expression);

      if (node.ArgumentList != null) {
        List<LuaExpressionSyntax> arguments = new List<LuaExpressionSyntax>();
        List<Tuple<LuaExpressionSyntax, LuaExpressionSyntax>> initializers = new List<Tuple<LuaExpressionSyntax, LuaExpressionSyntax>>();
        List<Tuple<NameColonSyntax, ExpressionSyntax>> argumentNodeInfos = new List<Tuple<NameColonSyntax, ExpressionSyntax>>();

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
            initializers.Add(Tuple.Create(name, argumentExpression));
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
            var memberAccess = BuildFieldOrPropertyMemberAccessExpression(temp, initializer.Item1, false);
            var assignmentExpression = BuildLuaSimpleAssignmentExpression(memberAccess, initializer.Item2);
            function.AddStatement(assignmentExpression);
          }

          PopFunction();
          return new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Create, invocation, function);
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

    private void TryAddStructDefaultMethod(INamedTypeSymbol symbol, LuaTypeDeclarationSyntax declaration) {
      Contract.Assert(symbol.IsValueType);
      if (declaration.IsInitStatementExists) {
        LuaIdentifierNameSyntax className = new LuaIdentifierNameSyntax(symbol.Name);
        var thisIdentifier = LuaIdentifierNameSyntax.This;
        LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
        functionExpression.AddParameter(className);
        var invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.setmetatable, LuaTableInitializerExpression.Empty, className);
        LuaLocalVariableDeclaratorSyntax local = new LuaLocalVariableDeclaratorSyntax(thisIdentifier, invocation);
        functionExpression.AddStatement(local);
        functionExpression.AddStatement(new LuaExpressionStatementSyntax(new LuaInvocationExpressionSyntax(declaration.IsNoneCtros ? LuaIdentifierNameSyntax.Ctor : LuaIdentifierNameSyntax.Init, thisIdentifier)));
        functionExpression.AddStatement(new LuaReturnStatementSyntax(thisIdentifier));
        declaration.AddMethod(LuaIdentifierNameSyntax.Default, functionExpression, false);
      }
    }

    private void CheckValueTypeClone(ITypeSymbol typeSymbol, IdentifierNameSyntax node, ref LuaExpressionSyntax expression) {
      if (typeSymbol.IsValueType && typeSymbol.TypeKind != TypeKind.Enum && (typeSymbol.SpecialType == SpecialType.None && !typeSymbol.IsTimeSpanType())) {
        bool need = false;
        switch (node.Parent.Kind()) {
          case SyntaxKind.Argument: {
              var symbol = semanticModel_.GetSymbolInfo(node.Parent.Parent.Parent).Symbol;
              if (symbol != null && symbol.IsFromAssembly() && !symbol.ContainingType.IsCollectionType()) {
                break;
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
      ++inheritNameNodeCounter_;
      var baseTypeName = (LuaExpressionSyntax)baseType.Accept(this);
      --inheritNameNodeCounter_;
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
          var identifierName = new LuaIdentifierNameSyntax(typeParameterSymbol.Name);
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

    public bool CheckFieldNameOfProtobufnet(ref string fieldName, ITypeSymbol containingType) {
      if (!containingType.Interfaces.IsEmpty) {
        if (containingType.Interfaces.First().ToString() == "ProtoBuf.IExtensible") {
          fieldName = fieldName.TrimStart('_');
          return true;
        }
      }
      return false;
    }

    private LuaIdentifierNameSyntax GetMemberName(ISymbol symbol) {
      return generator_.GetMemberName(symbol);
    }

    private LuaIdentifierNameSyntax AddInnerName(ISymbol symbol) {
      return generator_.AddInnerName(symbol);
    }

    private void RemoveNilArgumentsAtTail(List<LuaExpressionSyntax> arguments) {
      int i;
      for (i = arguments.Count - 1; i >= 0; --i) {
        if (!arguments[i].IsNil()) {
          break;
        }
      }
      int nilStartIndex = i + 1;
      int nilArgumentCount = arguments.Count - nilStartIndex;
      if (nilArgumentCount > 0) {
        arguments.RemoveRange(nilStartIndex, nilArgumentCount);
      }
    }

    private void TryRemoveNilArgumentsAtTail(ISymbol symbol, List<LuaExpressionSyntax> arguments) {
      if (arguments.Count > 0) {
        if (symbol.IsFromCode() || symbol.ContainingType.GetMembers(symbol.Name).Length == 1) {
          RemoveNilArgumentsAtTail(arguments);
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

    private void CheckConversion(ExpressionSyntax node, ref LuaExpressionSyntax expression) {
      var conversion = semanticModel_.GetConversion(node);
      if (conversion.IsUserDefined && conversion.IsImplicit) {
        expression = BuildConversionExpression(conversion.MethodSymbol, expression);
      }
    }

    private LuaMemberAccessExpressionSyntax GetOperatorMemberAccessExpression(IMethodSymbol methodSymbol) {
      var typeName = GetTypeName(methodSymbol.ContainingType);
      var methodName = GetMemberName(methodSymbol);
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

    private LuaExpressionSyntax GerUserDefinedOperatorExpression(BinaryExpressionSyntax node) {
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
              return BuildCodeTemplateExpression(codeTemplate, null, new ExpressionSyntax[] { node.Left, node.Right }, null);
            }

            var left = (LuaExpressionSyntax)node.Left.Accept(this);
            var right = (LuaExpressionSyntax)node.Right.Accept(this);
            var memberAccess = GetOperatorMemberAccessExpression(methodSymbol);
            return new LuaInvocationExpressionSyntax(memberAccess, left, right);
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

      var limitConst = semanticModel_.GetConstantValue(condition.Right);
      if (limitConst.HasValue) {
        if (!(limitConst.Value is int)) {
          goto Fail;
        }
      } else {
        bool isReadOnly = false;
        var symbol = semanticModel_.GetSymbolInfo(condition.Right).Symbol;
        if (symbol != null) {
          if (symbol.Kind == SymbolKind.Field) {
            isReadOnly = ((IFieldSymbol)symbol).IsReadOnly;
          } else if (symbol.Kind == SymbolKind.Property) {
            var propertySymbol = (IPropertySymbol)symbol;
            isReadOnly = propertySymbol.IsReadOnly && IsPropertyField(propertySymbol);
          }
        }
        if (!isReadOnly) {
          goto Fail;
        }
      }

      bool hasNoEqual;
      bool isPlus;
      var incrementor = node.Incrementors.First();
      switch (incrementor.Kind()) {
        case SyntaxKind.PreIncrementExpression:
        case SyntaxKind.PreDecrementExpression:
          var prefixUnaryExpression = (PrefixUnaryExpressionSyntax)incrementor;
          if (!IsNumericalForVariableMatch(prefixUnaryExpression.Operand, variable.Identifier)) {
            goto Fail;
          }
          if (incrementor.IsKind(SyntaxKind.PreIncrementExpression)) {
            if (!IsNumericalForLess(conditionKind, out hasNoEqual)) {
              goto Fail;
            }
            isPlus = true;
          } else {
            if (!IsNumericalForGreater(conditionKind, out hasNoEqual)) {
              goto Fail;
            }
            isPlus = false;
          }
          break;
        case SyntaxKind.PostIncrementExpression:
        case SyntaxKind.PostDecrementExpression:
          var postfixUnaryExpression = (PostfixUnaryExpressionSyntax)incrementor;
          if (!IsNumericalForVariableMatch(postfixUnaryExpression.Operand, variable.Identifier)) {
            goto Fail;
          }
          if (incrementor.IsKind(SyntaxKind.PostIncrementExpression)) {
            if (!IsNumericalForLess(conditionKind, out hasNoEqual)) {
              goto Fail;
            }
            isPlus = true;
          } else {
            if (!IsNumericalForGreater(conditionKind, out hasNoEqual)) {
              goto Fail;
            }
            isPlus = false;
          }
          break;
        default:
          goto Fail;
      }

      LuaIdentifierNameSyntax identifier = new LuaIdentifierNameSyntax(variable.Identifier.ValueText);
      CheckLocalVariableName(ref identifier, variable);

      var startExpression = (LuaExpressionSyntax)variable.Initializer.Value.Accept(this);
      LuaExpressionSyntax limitExpression;
      LuaExpressionSyntax stepExpression = null;
      if (hasNoEqual) {
        if (limitConst.Value != null) {
          int limit = (int)limitConst.Value;
          if (isPlus) {
            --limit;
          } else {
            ++limit;
            stepExpression = new LuaPrefixUnaryExpressionSyntax(LuaIdentifierNameSyntax.One, LuaSyntaxNode.Tokens.Sub);
          }
          limitExpression = new LuaIdentifierLiteralExpressionSyntax(limit.ToString());
        } else {
          limitExpression = (LuaExpressionSyntax)condition.Right.Accept(this);
          if (isPlus) {
            limitExpression = new LuaBinaryExpressionSyntax(limitExpression, LuaSyntaxNode.Tokens.Sub, LuaIdentifierNameSyntax.One);
          } else {
            limitExpression = new LuaBinaryExpressionSyntax(limitExpression, LuaSyntaxNode.Tokens.Plus, LuaIdentifierNameSyntax.One);
            stepExpression = new LuaPrefixUnaryExpressionSyntax(LuaIdentifierNameSyntax.One, LuaSyntaxNode.Tokens.Sub);
          }
        }
      } else {
        limitExpression = (LuaExpressionSyntax)condition.Right.Accept(this);
        if (!isPlus) {
          stepExpression = new LuaPrefixUnaryExpressionSyntax(LuaIdentifierNameSyntax.One, LuaSyntaxNode.Tokens.Sub);
        }
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
        return new LuaInvocationExpressionSyntax(new LuaMemberAccessExpressionSyntax(expression, new LuaIdentifierNameSyntax(kDeconstructName), true));
      }

      if (typeSymbol.IsTupleType) {
        var invocationExpression = BuildInvocation();
        invocationExpression.AddArgument(new LuaIdentifierNameSyntax(typeSymbol.GetTupleElementCount()));
        return invocationExpression;
      } else if (typeSymbol.IsSystemTuple()) {
        var nameTypeSymbol = (INamedTypeSymbol)typeSymbol;
        var invocationExpression = BuildInvocation();
        invocationExpression.AddArgument(new LuaIdentifierNameSyntax(nameTypeSymbol.TypeArguments.Length));
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
