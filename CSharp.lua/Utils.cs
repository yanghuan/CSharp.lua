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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed class CmdArgumentException : Exception {
    public CmdArgumentException(string message) : base(message) {
    }
  }

  public sealed class CompilationErrorException : Exception {
    public SyntaxNode SyntaxNode { get; }

    public CompilationErrorException(string message) : base(message) {
    }

    public CompilationErrorException(SyntaxNode node, string message)
      : base($"{node.GetLocationString()}: {message}, please refactor your code.") {
      SyntaxNode = node;
    }

    public CompilationErrorException With(SyntaxNode node) {
      if (SyntaxNode == null) {
        return new CompilationErrorException(node, Message);
      }
      return this;
    }
  }

  public sealed class BugErrorException : Exception {
    public BugErrorException(SyntaxNode node, Exception e)
      : base($"{node.GetLocationString()}: Compiler has a bug, thanks to commit issue at https://github.com/yanghuan/CSharp.lua/issue", e) {
    }
  }

  public sealed class ConcurrentList<T> : ConcurrentBag<T> {
  }

  public sealed class ConcurrentHashSet<T> : IEnumerable<T> {
    private readonly ConcurrentDictionary<T, bool> dict_ = new ConcurrentDictionary<T, bool>();

    public bool Add(T v) {
      return dict_.TryAdd(v, true);
    }

    public bool Contains(T v) {
      return dict_.ContainsKey(v);
    }

    public IEnumerator<T> GetEnumerator() {
      return dict_.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public void UnionWith(IEnumerable<T> collection) {
      foreach (var v in collection) {
        Add(v);
      }
    }
  }

  public enum PropertyMethodKind {
    Field = 0,
    Both = 1,
    GetOnly = 2,
    SetOnly = 3,
  }

  public static class Utility {
    public static T First<T>(this IList<T> list) {
      return list[0];
    }

    public static T Last<T>(this IList<T> list) {
      return list[list.Count - 1];
    }

    public static bool IsNullOrEmpty<T>(this IList<T> list) {
      return list == null || list.Count == 0;
    }

    public static T GetOrDefault<T>(this IList<T> list, int index, T v = default) {
      return index >= 0 && index < list.Count ? list[index] : v;
    }

    public static void RemoveRange<T>(this List<T> list, int index) {
      list.RemoveRange(index, list.Count - index);
    }

    public static T GetOrDefault<K, T>(this IDictionary<K, T> dict, K key, T t = default) {
      if (dict.TryGetValue(key, out T v)) {
        return v;
      }
      return t;
    }

    public static bool TryAdd<K, V>(this Dictionary<K, HashSet<V>> dict, K key, V value) {
      var set = dict.GetOrDefault(key);
      if (set == null) {
        set = new HashSet<V>();
        dict.Add(key, set);
      }
      return set.Add(value);
    }

    public static T GetOrAdd<K, T>(this IDictionary<K, T> dict, K key, Func<K, T> valueFunc) {
      if (!dict.TryGetValue(key, out var v)) {
        v = valueFunc(key);
        dict.Add(key, v);
      }
      return v;
    }

    public static bool Contains<K, V>(this Dictionary<K, HashSet<V>> dict, K key, V value) {
      var set = dict.GetOrDefault(key);
      return set != null && set.Contains(value);
    }

    public static void AddAt<T>(this IList<T> list, int index, T v) {
      if (index < list.Count) {
        list[index] = v;
      } else {
        int count = index - list.Count;
        for (int i = 0; i < count; ++i) {
          list.Add(default);
        }
        list.Add(v);
      }
    }

    public static int IndexOf<T>(this IEnumerable<T> source, T value) {
      var comparer = EqualityComparer<T>.Default;
      return source.IndexOf(i => comparer.Equals(i, value));
    }

    public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> match) {
      int index = 0;
      foreach (var item in source) {
        if (match(item)) {
          return index;
        }
        ++index;
      }
      return -1;
    }

    public static int FindIndex(this string s, int startIndex, Predicate<char> match) {
      for (int i = startIndex; i < s.Length; ++i) {
        if (match(s[i])) {
          return i;
        }
      }
      return -1;
    }

    public static T[] ArrayOf<T>(this T t) {
      return new T[] { t };
    }

    public static T[] ArrayOf<T>(this T t, T a) {
      return new T[] { t, a };
    }

    public static T[] ArrayOf<T>(this T t, params T[] args) {
      T[] array = new T[args.Length + 1];
      array[0] = t;
      Array.Copy(args, 0, array, 1, args.Length);
      return array;
    }

    public static Dictionary<string, string[]> GetCommandLines(string[] args) {
      Dictionary<string, string[]> cmds = new Dictionary<string, string[]>();

      string key = "";
      List<string> values = new List<string>();

      foreach (string arg in args) {
        string i = arg.Trim();
        if (i.StartsWith("-")) {
          if (!string.IsNullOrEmpty(key)) {
            cmds.Add(key, values.ToArray());
            key = "";
            values.Clear();
          }
          key = i;
        } else {
          values.Add(i);
        }
      }

      if (!string.IsNullOrEmpty(key)) {
        cmds.Add(key, values.ToArray());
      }
      return cmds;
    }

    public static string GetArgument(this Dictionary<string, string[]> args, string name, bool isOption = false) {
      string[] values = args.GetOrDefault(name);
      if (values == null || values.Length == 0) {
        if (isOption) {
          return null;
        }
        throw new CmdArgumentException(name + " is not found");
      }
      return values[0];
    }

    public static string GetCurrentDirectory(string path) {
      const string CurrentDirectorySign1 = "~/";
      const string CurrentDirectorySign2 = "~\\";

      if (path.StartsWith(CurrentDirectorySign1)) {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(CurrentDirectorySign1.Length));
      } else if (path.StartsWith(CurrentDirectorySign2)) {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(CurrentDirectorySign2.Length));
      }

      return Path.Combine(Environment.CurrentDirectory, path);
    }

    public static string[] Split(string s, bool isPath = true) {
      HashSet<string> list = new HashSet<string>();
      if (!string.IsNullOrEmpty(s)) {
        string[] array = s.Split(';', ',');
        foreach (string i in array) {
          list.Add(isPath ? GetCurrentDirectory(i) : i);
        }
      }
      return list.ToArray();
    }

    public static string ReplaceNewline(this string s) {
      return s.Replace("\r\n", "\n").Replace('\r', '\n');
    }

    public static string LastName(this string s) {
      int pos = s.LastIndexOf('.');
      if (pos != -1) {
        return s.Substring(pos + 1);
      }
      return s;
    }

    public static string TrimEnd(this string s, string v) {
      if (s.EndsWith(v)) {
        return s.Remove(s.Length - v.Length);
      }
      return s;
    }

    public static LuaStringLiteralExpressionSyntax ToStringLiteral(this string s) {
      return new LuaStringLiteralExpressionSyntax(s);
    }

    public static bool IsPrivate(this ISymbol symbol) {
      return symbol.DeclaredAccessibility == Accessibility.Private;
    }

    public static bool IsPublic(this ISymbol symbol) {
      return symbol.DeclaredAccessibility == Accessibility.Public;
    }

    public static bool IsPrivate(this SyntaxTokenList modifiers) {
      foreach (var modifier in modifiers) {
        switch (modifier.Kind()) {
          case SyntaxKind.PrivateKeyword: {
            return true;
          }
          case SyntaxKind.PublicKeyword:
          case SyntaxKind.InternalKeyword:
          case SyntaxKind.ProtectedKeyword: {
            return false;
          }
        }
      }
      return true;
    }

    public static bool IsStatic(this SyntaxTokenList modifiers) {
      return modifiers.Any(i => i.IsKind(SyntaxKind.StaticKeyword));
    }

    public static bool IsAbstract(this SyntaxTokenList modifiers) {
      return modifiers.Any(i => i.IsKind(SyntaxKind.AbstractKeyword));
    }

    public static bool IsExtern(this SyntaxTokenList modifiers) {
      return modifiers.Any(i => i.IsKind(SyntaxKind.ExternKeyword));
    }

    public static bool IsReadOnly(this SyntaxTokenList modifiers) {
      return modifiers.Any(i => i.IsKind(SyntaxKind.ReadOnlyKeyword));
    }

    public static bool IsConst(this SyntaxTokenList modifiers) {
      return modifiers.Any(i => i.IsKind(SyntaxKind.ConstKeyword));
    }

    public static bool IsParams(this SyntaxTokenList modifiers) {
      return modifiers.Any(i => i.IsKind(SyntaxKind.ParamsKeyword));
    }

    public static bool IsPartial(this SyntaxTokenList modifiers) {
      return modifiers.Any(i => i.IsKind(SyntaxKind.PartialKeyword));
    }

    public static bool IsOutOrRef(this SyntaxTokenList modifiers) {
      return modifiers.Any(i => i.IsKind(SyntaxKind.OutKeyword) || i.IsKind(SyntaxKind.RefKeyword));
    }

    public static bool IsOutOrRef(this SyntaxToken token) {
      return token.IsKind(SyntaxKind.OutKeyword) || token.IsKind(SyntaxKind.RefKeyword);
    }

    public static bool EQ(this ISymbol a, ISymbol b) {
      return SymbolEqualityComparer.Default.Equals(a, b);
    }

    public static bool IsBasicType(this ITypeSymbol type) {
      return type.SpecialType >= SpecialType.System_Enum && type.SpecialType <= SpecialType.System_Double;
    }

    public static bool IsStringType(this ITypeSymbol type) {
      return type.SpecialType == SpecialType.System_String;
    }

    public static bool IsDelegateType(this ITypeSymbol type) {
      return type.TypeKind == TypeKind.Delegate;
    }

    public static bool IsIntegerType(this ITypeSymbol type, bool withNullable = true) {
      if (withNullable && type.IsNullableType()) {
        type = ((INamedTypeSymbol)type).TypeArguments.First();
      }
      return type.SpecialType >= SpecialType.System_SByte && type.SpecialType <= SpecialType.System_UInt64;
    }

    public static bool IsCastIntegerType(this ITypeSymbol type) {
      return type.SpecialType >= SpecialType.System_Char && type.SpecialType <= SpecialType.System_UInt64;
    }

    public static bool IsNumberType(this ITypeSymbol type, bool withNullable = true) {
      if (withNullable && type.IsNullableType()) {
        type = ((INamedTypeSymbol)type).TypeArguments.First();
      }
      return type.SpecialType >= SpecialType.System_SByte && type.SpecialType <= SpecialType.System_Double;
    }

    public static bool IsDoubleOrFloatType(this ITypeSymbol type, bool withNullable = true) {
      if (withNullable && type.IsNullableType()) {
        type = ((INamedTypeSymbol)type).TypeArguments.First();
      }
      return type.SpecialType == SpecialType.System_Double || type.SpecialType == SpecialType.System_Single;
    }


    public static bool IsBoolType(this ITypeSymbol type, bool withNullable = true) {
      if (withNullable && type.IsNullableType()) {
        type = ((INamedTypeSymbol)type).TypeArguments.First();
      }
      return type.SpecialType == SpecialType.System_Boolean;
    }

    public static bool IsNullableType(this ITypeSymbol type) {
      return type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
    }

    public static bool IsNullableType(this ITypeSymbol type, out ITypeSymbol elemetType) {
      elemetType = type.NullableElemetType();
      return elemetType != null;
    }

    public static ITypeSymbol NullableElemetType(this ITypeSymbol type) {
      return type.IsNullableType() ? ((INamedTypeSymbol)type).TypeArguments.First() : null;
    }

    public static bool IsEnumType(this ITypeSymbol type, out ITypeSymbol symbol, out bool isNullable) {
      if (type.TypeKind == TypeKind.Enum) {
        symbol = type;
        isNullable = false;
        return true;
      } else {
        var nullableElemetType = type.NullableElemetType();
        if (nullableElemetType != null && nullableElemetType.TypeKind == TypeKind.Enum) {
          symbol = nullableElemetType;
          isNullable = true;
          return true;
        }
      }

      symbol = null;
      isNullable = false;
      return false;
    }

    public static bool IsImmutable(this ITypeSymbol type) {
      bool isImmutable = (type.IsValueType && type.IsDefinition) || type.IsStringType() || type.IsDelegateType();
      return isImmutable;
    }

    public static bool IsSystemTuple(this ITypeSymbol type) {
      return type.Name == "Tuple" && type.ContainingNamespace.Name == "System";
    }

    public static bool IsSystemIndex(this ITypeSymbol type) {
      return type.Name == "Index" && type.ContainingNamespace.Name == "System";
    }

    private static bool IsSystemIComparableT(this INamedTypeSymbol type) {
      return type.Name == "IComparable"   && type.ContainingNamespace.Name == "System"  && type.IsGenericType;
    }

    private static bool IsSystemIEquatableT(this INamedTypeSymbol type) {
      return type.Name == "IEquatable" && type.ContainingNamespace.Name == "System"  && type.IsGenericType;
    }

    private static bool IsSystemIFormattable(this INamedTypeSymbol type) {
      return type.Name == "IFormattable"  && type.ContainingNamespace.Name == "System";
    }

    public static bool IsBasicTypInterface(this INamedTypeSymbol type) {
      return type.TypeKind == TypeKind.Interface && (type.IsSystemIComparableT() || type.IsSystemIEquatableT() || type.IsSystemIFormattable());
    }

    public static bool IsRecordType(this INamedTypeSymbol type) {
      var methods = type.GetMembers("<Clone>$");
      return methods.Length == 1;
    }

    public static bool IsSystemTask(this ITypeSymbol symbol) {
      return (symbol.Name == "Task" || symbol.Name == "ValueTask")
        && symbol.ContainingNamespace.Name == "Tasks"
        && symbol.ContainingNamespace.ContainingNamespace.Name == "Threading"
        && symbol.ContainingNamespace.ContainingNamespace.ContainingNamespace.Name == "System";
    }

    public static bool IsInterfaceImplementation<T>(this T symbol) where T : ISymbol {
      if (!symbol.IsStatic) {
        var type = symbol.ContainingType;
        if (type != null) {
          var interfaceSymbols = type.AllInterfaces.SelectMany(i => i.GetMembers().OfType<T>());
          return interfaceSymbols.Any(i => symbol.Equals(type.FindImplementationForInterfaceMember(i)));
        }
      }
      return false;
    }

    public static IEnumerable<T> InterfaceImplementations<T>(this T symbol) where T : ISymbol {
      if (!symbol.IsStatic) {
        var type = symbol.ContainingType;
        if (type != null) {
          var interfaceSymbols = type.AllInterfaces.SelectMany(i => i.GetMembers().OfType<T>());
          return interfaceSymbols.Where(i => symbol.Equals(type.FindImplementationForInterfaceMember(i)));
        }
      }
      return Array.Empty<T>();
    }

    public static bool IsFromCode(this ISymbol symbol) {
      return !symbol.DeclaringSyntaxReferences.IsEmpty;
    }

    public static bool IsFromAssembly(this ISymbol symbol) {
      return !symbol.IsFromCode();
    }

    public static bool IsOverridable(this ISymbol symbol) {
      return !symbol.IsStatic && (symbol.IsAbstract || symbol.IsVirtual || symbol.IsOverride);
    }

    public static ISymbol OverriddenSymbol(this ISymbol symbol) {
      switch (symbol.Kind) {
        case SymbolKind.Method: {
          IMethodSymbol methodSymbol = (IMethodSymbol)symbol;
          return methodSymbol.OverriddenMethod;
        }
        case SymbolKind.Property: {
          IPropertySymbol propertySymbol = (IPropertySymbol)symbol;
          return propertySymbol.OverriddenProperty;
        }
        case SymbolKind.Event: {
          IEventSymbol eventSymbol = (IEventSymbol)symbol;
          return eventSymbol.OverriddenEvent;
        }
      }
      return null;
    }

    public static bool IsOverridden(this ISymbol symbol, ISymbol superSymbol) {
      while (true) {
        ISymbol overriddenSymbol = symbol.OverriddenSymbol();
        if (overriddenSymbol != null) {
          CheckOriginalDefinition(ref overriddenSymbol);
          if (overriddenSymbol.EQ(superSymbol)) {
            return true;
          }
          symbol = overriddenSymbol;
        } else {
          return false;
        }
      }
    }

    public static bool HasMetadataAttribute(this ISymbol symbol) {
      var node = symbol.GetDeclaringSyntaxNode();
      if (node != null) {
        return node.HasCSharpLuaAttribute(LuaDocumentStatement.AttributeFlags.Metadata);
      }
      return false;
    }

    public static bool HasParamsAttribute(this ISymbol symbol) {
      var node = symbol.GetDeclaringSyntaxNode();
      if (node != null) {
        return node.HasCSharpLuaAttribute(LuaDocumentStatement.AttributeFlags.Params);
      }
      return false;
    }

    public static bool HasCSharpLuaAttribute(this SyntaxNode node, LuaDocumentStatement.AttributeFlags attribute) {
      return node.HasCSharpLuaAttribute(attribute, out _);
    }

    public static bool HasCSharpLuaAttribute(this SyntaxNode node, LuaDocumentStatement.AttributeFlags attribute, out string text) {
      text = null;
      var documentTrivia = node.GetLeadingTrivia().FirstOrDefault(i => i.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
      if (documentTrivia != null) {
        string document = documentTrivia.ToString();
        if (document.Contains(LuaDocumentStatement.ToString(attribute))) {
          text = document;
          return true;
        }
      }
      return false;
    }

    public static string GetCodeTemplateFromAttribute(this ISymbol symbol) {
      var node = symbol.GetDeclaringSyntaxNode();
      if (node != null) {
        if (symbol.Kind == SymbolKind.Field) {
          node = node.Parent.Parent;
        }
        if (node.HasCSharpLuaAttribute(LuaAst.LuaDocumentStatement.AttributeFlags.Template, out string text)) {
          return GetCodeTemplateFromAttributeText(text);
        }
      }
      return null;
    }

    private static readonly Regex codeTemplateAttributeRegex_ = new Regex(@"@CSharpLua.Template\s*=\s*(.+)\s*", RegexOptions.Compiled);

    private static string GetCodeTemplateFromAttributeText(string document) {
      var matchs = codeTemplateAttributeRegex_.Matches(document);
      if (matchs.Count > 0) {
        string text = matchs[0].Groups[1].Value;
        return text.Trim().Trim('"');
      }
      return null;
    }

    public static bool IsAssignment(this SyntaxKind kind) {
      return kind >= SyntaxKind.SimpleAssignmentExpression && kind <= SyntaxKind.RightShiftAssignmentExpression;
    }

    public static bool IsTupleDeclaration(this SyntaxKind kind) {
      return kind == SyntaxKind.DeclarationExpression || kind == SyntaxKind.TupleExpression;
    }

    public static bool IsTypeDeclaration(this SyntaxKind kind) {
      return kind >= SyntaxKind.ClassDeclaration && kind <= SyntaxKind.EnumDeclaration;
    }

    public static bool IsLiteralExpression(this SyntaxKind kind) {
      return kind >= SyntaxKind.NumericLiteralExpression && kind <= SyntaxKind.DefaultLiteralExpression;
    }

    public static bool IsSystemLinqEnumerable(this INamedTypeSymbol symbol) {
      return symbol.Name == "Enumerable" && symbol.ContainingNamespace.Name == "Linq" && symbol.ContainingNamespace.ContainingNamespace.Name == "System";
    }

    public static string GetLocationString(this SyntaxNode node) {
      var location = node.SyntaxTree.GetLocation(node.Span);
      var methodInfo = location.GetType().GetMethod("GetDebuggerDisplay", BindingFlags.Instance | BindingFlags.NonPublic);
      return (string)methodInfo.Invoke(location, null);
    }

    public static bool IsGetExpressionNode(this ExpressionSyntax node) {
      bool isGet;
      if (!node.Parent.Kind().IsAssignment()) {
        isGet = true;
      } else {
        var assignment = (AssignmentExpressionSyntax)node.Parent;
        isGet = assignment.Right == node;
      }
      return isGet;
    }

    public static bool IsSubclassOf(this ITypeSymbol child, ITypeSymbol parent) {
      if (parent.SpecialType == SpecialType.System_Object) {
        return true;
      }

      ITypeSymbol p = child;
      if (p.EQ(parent)) {
        return false;
      }

      while (p != null) {
        if (p.EQ(parent)) {
          return true;
        }
        p = p.BaseType;
      }
      return false;
    }

    private static bool IsImplementInterface(this ITypeSymbol implementType, ITypeSymbol interfaceType) {
      Contract.Assert(interfaceType.TypeKind == TypeKind.Interface);

      ITypeSymbol t = implementType;
      while (t != null) {
        var interfaces = implementType.AllInterfaces;
        foreach (var i in interfaces) {
          if (i.EQ(interfaceType) || i.IsImplementInterface(interfaceType)) {
            return true;
          }
        }
        t = t.BaseType;
      }
      return false;
    }

    private static bool IsBaseNumberType(this SpecialType specialType) {
      return specialType >= SpecialType.System_Char && specialType <= SpecialType.System_Double;
    }

    public static bool IsNumberTypeAssignableFrom(this ITypeSymbol left, ITypeSymbol right) {
      if (left.SpecialType.IsBaseNumberType() && right.SpecialType.IsBaseNumberType()) {
        if (left.EQ(right)) {
          return true;
        }

        SpecialType begin;
        switch (right.SpecialType) {
          case SpecialType.System_Char:
          case SpecialType.System_SByte:
          case SpecialType.System_Byte: {
            begin = SpecialType.System_Int16;
            break;
          }
          case SpecialType.System_Int16:
          case SpecialType.System_UInt16: {
            begin = SpecialType.System_Int32;
            break;
          }
          case SpecialType.System_Int32:
          case SpecialType.System_UInt32: {
            begin = SpecialType.System_Int64;
            break;
          }
          case SpecialType.System_Int64:
          case SpecialType.System_UInt64: {
            begin = SpecialType.System_Decimal;
            break;
          }
          default: {
            begin = SpecialType.System_Double;
            break;
          }
        }

        SpecialType end = SpecialType.System_Double;
        return left.SpecialType >= begin && left.SpecialType <= end;
      }
      return false;
    }

    public static bool Is(this ITypeSymbol left, ITypeSymbol right) {
      if (left.EQ(right)) {
        return true;
      }

      if (left.IsSubclassOf(right)) {
        return true;
      }

      if (right.TypeKind == TypeKind.Interface) {
        return left.IsImplementInterface(right);
      }

      return false;
    }

    private static void CheckSymbolDefinition<T>(ref T symbol) where T : class, ISymbol {
      var originalDefinition = (T)symbol.OriginalDefinition;
      if (originalDefinition != symbol) {
        symbol = originalDefinition;
      }
    }

    public static void CheckMethodDefinition(ref IMethodSymbol symbol) {
      if (symbol.IsExtensionMethod) {
        if (symbol.ReducedFrom != null && !symbol.ReducedFrom.EQ(symbol)) {
          symbol = symbol.ReducedFrom;
        } else {
          CheckSymbolDefinition(ref symbol);
        }
      } else {
        CheckSymbolDefinition(ref symbol);
      }
    }

    public static void CheckOriginalDefinition(ref ISymbol symbol) {
      if (symbol.Kind == SymbolKind.Method) {
        IMethodSymbol methodSymbol = (IMethodSymbol)symbol;
        CheckMethodDefinition(ref methodSymbol);
        if (!methodSymbol.EQ(symbol)) {
          symbol = methodSymbol;
        }
      } else {
        CheckSymbolDefinition(ref symbol);
      }
    }

    public static bool IsMainEntryPoint(this IMethodSymbol symbol) {
      if (symbol.IsStatic && symbol.MethodKind == MethodKind.Ordinary && symbol.TypeArguments.IsEmpty && symbol.ContainingType.TypeArguments.IsEmpty && symbol.Name == "Main") {
        if (symbol.ReturnsVoid || symbol.ReturnType.SpecialType == SpecialType.System_Int32) {
          if (symbol.Parameters.IsEmpty) {
            return true;
          } else if (symbol.Parameters.Length == 1) {
            var parameterType = symbol.Parameters[0].Type;
            if (parameterType.TypeKind == TypeKind.Array) {
              var arrayType = (IArrayTypeSymbol)parameterType;
              if (arrayType.ElementType.SpecialType == SpecialType.System_String) {
                return true;
              }
            }
          }
        }
      }
      return false;
    }

    public static bool IsExtendSelf(INamedTypeSymbol typeSymbol, INamedTypeSymbol baseTypeSymbol) {
      if (baseTypeSymbol.IsGenericType) {
        foreach (var baseTypeArgument in baseTypeSymbol.TypeArguments) {
          if (baseTypeSymbol.Kind != SymbolKind.TypeParameter) {
            if (!baseTypeArgument.EQ(typeSymbol) && baseTypeArgument.Is(typeSymbol)) {
              return true;
            }
          }
        }
      }
      return false;
    }

    public static bool IsTimeSpanType(this ITypeSymbol typeSymbol) {
      return typeSymbol.IsValueType && typeSymbol.ContainingNamespace.Name == "System" && typeSymbol.Name == "TimeSpan";
    }

    public static bool IsKeyValuePairType(this ITypeSymbol typeSymbol) {
      return typeSymbol.IsValueType && typeSymbol.Name == "KeyValuePair" && typeSymbol.ContainingNamespace.Name == "Generic";
    }

    public static bool IsGenericIEnumerableType(this ITypeSymbol typeSymbol) {
      return typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
    }

    public static bool IsGenericIAsyncEnumerableType(this ITypeSymbol typeSymbol) {
      return typeSymbol.Name == "IAsyncEnumerable" && typeSymbol.IsCollectionType();
    }

    public static bool IsCustomValueType(this ITypeSymbol typeSymbol) {
      return typeSymbol.IsValueType
        && typeSymbol.TypeKind != TypeKind.Enum
        && typeSymbol.TypeKind != TypeKind.Pointer
        && (typeSymbol.SpecialType == SpecialType.None && !typeSymbol.IsTimeSpanType());
    }

    public static bool IsMaybeValueType(this ITypeSymbol typeSymbol) {
      if (typeSymbol.IsValueType) {
        return true;
      }

      if (typeSymbol.IsReferenceType) {
        return false;
      }

      return typeSymbol.TypeKind == TypeKind.TypeParameter;
    }

    public static bool IsExplicitInterfaceImplementation(this ISymbol symbol) {
      switch (symbol.Kind) {
        case SymbolKind.Property: {
          IPropertySymbol property = (IPropertySymbol)symbol;
          if (property.GetMethod != null) {
            if (property.GetMethod.MethodKind == MethodKind.ExplicitInterfaceImplementation) {
              return true;
            }
            if (property.SetMethod != null) {
              if (property.SetMethod.MethodKind == MethodKind.ExplicitInterfaceImplementation) {
                return true;
              }
            }
          }
          break;
        }
        case SymbolKind.Method: {
          IMethodSymbol method = (IMethodSymbol)symbol;
          if (method.MethodKind == MethodKind.ExplicitInterfaceImplementation) {
            return true;
          }
          break;
        }
      }
      return false;
    }

    public static bool IsExportSyntaxTrivia(this SyntaxTrivia syntaxTrivia, SyntaxNode rootNode) {
      SyntaxKind kind = syntaxTrivia.Kind();
      switch (kind) {
        case SyntaxKind.SingleLineCommentTrivia:
        case SyntaxKind.MultiLineCommentTrivia:
        case SyntaxKind.SingleLineDocumentationCommentTrivia:
        case SyntaxKind.DisabledTextTrivia:
        case SyntaxKind.RegionDirectiveTrivia:
        case SyntaxKind.EndRegionDirectiveTrivia: {
          var span = rootNode.IsKind(SyntaxKind.CompilationUnit) ? rootNode.FullSpan : rootNode.Span;
          return span.Contains(syntaxTrivia.Span);
        }
        default:
          return false;
      }
    }

    public static ITypeSymbol GetIEnumerableElementType(this ITypeSymbol symbol) {
      var interfaceType = symbol.IsGenericIEnumerableType() ? (INamedTypeSymbol)symbol : symbol.AllInterfaces.FirstOrDefault(i => i.IsGenericIEnumerableType());
      return interfaceType?.TypeArguments.First();
    }

    public static ITypeSymbol GetIAsyncEnumerableElementType(this ITypeSymbol symbol) {
      var interfaceType = symbol.IsGenericIAsyncEnumerableType() ? (INamedTypeSymbol)symbol : symbol.AllInterfaces.FirstOrDefault(i => i.IsGenericIAsyncEnumerableType());
      return interfaceType?.TypeArguments.First();
    }

    public static IEnumerable<ITypeSymbol> GetTupleElementTypes(this ITypeSymbol typeSymbol) {
      var nameSymbol = (INamedTypeSymbol)typeSymbol;
      return nameSymbol.TupleElements.Select(i => i.Type);
    }

    public static int GetTupleElementIndex(this IFieldSymbol fieldSymbol) {
      Contract.Assert(fieldSymbol.ContainingType.IsTupleType);
      var fields = fieldSymbol.ContainingType.GetMembers().OfType<IFieldSymbol>().Where(i => i.CorrespondingTupleField.EQ(i));
      int index = fields.IndexOf(fieldSymbol.CorrespondingTupleField);
      Contract.Assert(index != -1);
      return index + 1;
    }

    public static bool IsStringConstNotInline(this IFieldSymbol field) {
      if (field.IsConst && field.Type.SpecialType == SpecialType.System_String && field.ConstantValue != null) {
        if (((string)field.ConstantValue).Length > LuaSyntaxNodeTransform.kStringConstInlineCount) {
          return true;
        }
      }
      return false;
    }

    public static bool IsLocalVariableField(this IFieldSymbol field) {
      if (!field.IsImplicitlyDeclared) {
        if (!field.IsConst) {
          return field.IsStatic && (field.IsPrivate() || field.IsReadOnly);
        }
        if (field.IsStringConstNotInline()) {
          return true;
        }
      }
      return false;
    }

    public static bool IsIndexerProperty(this ISymbol symbol) {
      if (symbol.Kind == SymbolKind.Property) {
        var propertySymbol = (IPropertySymbol)symbol;
        return propertySymbol.IsIndexer;
      }
      return false;
    }

    private static readonly Regex identifierRegex_ = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    public static bool IsIdentifierIllegal(ref string identifierName) {
      if (!identifierRegex_.IsMatch(identifierName)) {
        identifierName = EncodeToIdentifier(identifierName);
        return true;
      }
      return false;
    }

    private static string ToBase63(int number) {
      const string kAlphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
      int basis = kAlphabet.Length;
      int n = number;
      StringBuilder sb = new StringBuilder();
      while (n > 0) {
        char ch = kAlphabet[n % basis];
        sb.Append(ch);
        n /= basis;
      }
      return sb.ToString();
    }

    private static string EncodeToIdentifier(string name) {
      StringBuilder sb = new StringBuilder();
      foreach (char c in name) {
        if (c < 127) {
          sb.Append(c);
        } else {
          string base63 = ToBase63(c);
          sb.Append(base63);
        }
      }
      if (char.IsNumber(sb[0])) {
        sb.Insert(0, '_');
      }
      return sb.ToString();
    }

    private static void FillNamespaceName(StringBuilder sb, INamedTypeSymbol typeSymbol, Func<INamespaceSymbol, string, string> funcOfNamespace) {
      string namespaceName;
      var namespaceSymbol = typeSymbol.ContainingNamespace;
      if (namespaceSymbol.IsGlobalNamespace) {
        namespaceName = string.Empty;
      } else {
        namespaceName = namespaceSymbol.ToString();
        string newName = funcOfNamespace?.Invoke(namespaceSymbol, namespaceName);
        if (newName != null) {
          namespaceName = newName;
        }
      }
      if (namespaceName.Length > 0) {
        sb.Append(namespaceName);
        sb.Append('.');
      }
    }

    private static void FillExternalTypeName(
      StringBuilder sb,
      INamedTypeSymbol typeSymbol,
      Func<INamespaceSymbol, string, string> funcOfNamespace,
      Func<INamedTypeSymbol, string> funcOfTypeName,
      LuaSyntaxNodeTransform transfor = null) {
      var externalType = typeSymbol.ContainingType;
      if (externalType != null) {
        if (transfor != null && transfor.IsNoneGenericTypeCounter && !externalType.IsGenericType && !typeSymbol.IsGenericType) {
          var curTypeDeclaration = transfor.CurTypeDeclaration;
          if (curTypeDeclaration != null && curTypeDeclaration.CheckTypeName(externalType, out var classIdentifier)) {
            sb.Append(classIdentifier.ValueText);
            sb.Append('.');
            return;
          }
        }

        FillExternalTypeName(sb, externalType, funcOfNamespace, funcOfTypeName);
        string typeName = funcOfTypeName?.Invoke(typeSymbol) ?? externalType.Name;
        sb.Append(typeName);
        int typeParametersCount = externalType.TypeParameters.Length;
        if (typeParametersCount > 0) {
          sb.Append('_');
          sb.Append(typeParametersCount);
        }
        sb.Append('.');
      } else {
        FillNamespaceName(sb, typeSymbol, funcOfNamespace);
      }
    }

    public static string GetTypeShortName(
      this INamedTypeSymbol typeSymbol,
      Func<INamespaceSymbol, string, string> funcOfNamespace = null,
      Func<INamedTypeSymbol, string> funcOfTypeName = null,
      LuaSyntaxNodeTransform transfor = null) {
      StringBuilder sb = new StringBuilder();
      FillExternalTypeName(sb, typeSymbol, funcOfNamespace, funcOfTypeName, transfor);
      string typeName = funcOfTypeName?.Invoke(typeSymbol);
      if (typeName != null) {
        sb.Append(typeName);
      } else {
        typeName = typeSymbol.Name;
        sb.Append(typeName);
        int typeParametersCount = typeSymbol.TypeParameters.Length;
        if (typeParametersCount > 0) {
          sb.Append('_');
          sb.Append(typeParametersCount);
        }
      }
      return sb.ToString();
    }

    public static string GetNewIdentifierName(string name, int index) {
      return index switch
      {
        0 => name,
        1 => name + "_",
        2 => "_" + name,
        _ => name + (index - 2),
      };
    }

    private static IEnumerable<INamespaceSymbol> InternalGetAllNamespaces(INamespaceSymbol symbol) {
      do {
        yield return symbol;
        symbol = symbol.ContainingNamespace;
      } while (!symbol.IsGlobalNamespace);
    }

    public static IEnumerable<INamespaceSymbol> GetAllNamespaces(this INamespaceSymbol symbol) {
      if (symbol.IsGlobalNamespace) {
        return Array.Empty<INamespaceSymbol>();
      }
      return InternalGetAllNamespaces(symbol).Reverse();
    }

    public static bool IsCollectionType(this ITypeSymbol symbol) {
      INamespaceSymbol containingNamespace = symbol.ContainingNamespace;
      while (!containingNamespace.IsGlobalNamespace) {
        if (containingNamespace.Name == "Collections" && containingNamespace.ContainingNamespace.Name == "System") {
          return true;
        }
        containingNamespace = containingNamespace.ContainingNamespace;
      }
      return false;
    }

    public static bool IsTypeParameterExists(this ITypeSymbol symbol, ITypeSymbol matchType = null) {
      switch (symbol.Kind) {
        case SymbolKind.ArrayType: {
          var arrayType = (IArrayTypeSymbol)symbol;
          if (arrayType.ElementType.IsTypeParameterExists(matchType)) {
            return true;
          }
          break;
        }
        case SymbolKind.NamedType: {
          var nameTypeSymbol = (INamedTypeSymbol)symbol;
          foreach (var typeArgument in nameTypeSymbol.TypeArguments) {
            if (typeArgument.IsTypeParameterExists(matchType)) {
              return true;
            }
          }
          if (symbol.ContainingType != null) {
            if (symbol.ContainingType.IsTypeParameterExists(matchType)) {
              return true;
            }
          }
          break;
        }
        case SymbolKind.TypeParameter: {
          return matchType == null || symbol.EQ(matchType);
        }
        case SymbolKind.PointerType: {
          var pointType = (IPointerTypeSymbol)symbol;
          if (pointType.PointedAtType.IsTypeParameterExists(matchType)) {
            return true;
          }
          break;
        }
      }

      return false;
    }

    public static bool IsAbsoluteFromCode(this ITypeSymbol symbol) {
      if (symbol.IsFromCode()) {
        return true;
      }

      switch (symbol.Kind) {
        case SymbolKind.ArrayType: {
          var arrayType = (IArrayTypeSymbol)symbol;
          if (arrayType.ElementType.IsAbsoluteFromCode()) {
            return true;
          }
          break;
        }
        case SymbolKind.NamedType: {
          var nameTypeSymbol = (INamedTypeSymbol)symbol;
          foreach (var typeArgument in nameTypeSymbol.TypeArguments) {
            if (typeArgument.IsAbsoluteFromCode()) {
              return true;
            }
          }
          break;
        }
      }

      return false;
    }

    public static bool IsMemberExists(this ITypeSymbol symbol, string memberName, bool inherit = false) {
      if (inherit) {
        while (true) {
          if (!symbol.GetMembers(memberName).IsEmpty) {
            return true;
          }

          symbol = symbol.BaseType;
          if (symbol == null || symbol.SpecialType == SpecialType.System_Object) {
            return false;
          }
        }
      } else {
        return !symbol.GetMembers(memberName).IsEmpty;
      }
    }

    public static bool IsContainsInternalSymbol(this INamedTypeSymbol type, ISymbol symbol) {
      if (type.EQ(symbol.ContainingType)) {
        return true;
      }

      var containingType = type.ContainingType;
      if (containingType != null && !containingType.IsGenericType) {
        return containingType.IsContainsInternalSymbol(symbol);
      }

      return false;
    }

    public static bool IsRuntimeCompilerServices(this INamespaceSymbol symbol) {
      return symbol.Name == "CompilerServices" && symbol.ContainingNamespace.Name == "Runtime" && symbol.ContainingNamespace.ContainingNamespace.Name == "System";
    }

    public static bool HasCompilerGeneratedAttribute(this ImmutableArray<AttributeData> attrs) {
      return attrs.Any(i => i.AttributeClass.IsCompilerGeneratedAttribute());
    }

    public static bool IsCompilerGeneratedAttribute(this INamedTypeSymbol symbol) {
      return symbol.Name == "CompilerGeneratedAttribute" && symbol.ContainingNamespace.IsRuntimeCompilerServices();
    }

    private static bool IsSystemReflection(this INamespaceSymbol symbol) {
      return symbol.Name == "Reflection" && symbol.ContainingNamespace.Name == "System";
    }

    public static bool IsAssemblyAttribute(this INamedTypeSymbol symbol) {
      return symbol.Name.StartsWith("Assembly") && symbol.ContainingNamespace.IsSystemReflection();
    }

    public static bool HasAggressiveInliningAttribute(this ISymbol symbol) {
      return symbol.GetAttributes().Any(i => i.IsMethodImplOptions(MethodImplOptions.AggressiveInlining));
    }

    public static bool HasNoInliningAttribute(this ISymbol symbol) {
      return symbol.GetAttributes().Any(i => i.IsMethodImplOptions(MethodImplOptions.NoInlining));
    }

    private static bool IsMethodImplOptions(this AttributeData attributeData, MethodImplOptions option) {
      if (attributeData.AttributeClass.IsMethodImplAttribute()) {
        foreach (var constructorArgument in attributeData.ConstructorArguments) {
          if (constructorArgument.Value is int v) {
            var options = (MethodImplOptions)v;
            return options.HasFlag(option);
          }
        }
      }
      return false;
    }

    private static bool IsMethodImplAttribute(this INamedTypeSymbol symbol) {
      return symbol.Name == "MethodImplAttribute" && symbol.ContainingNamespace.IsRuntimeCompilerServices();
    }

    private static bool IsSystemDiagnostics(this INamespaceSymbol symbol) {
      return symbol.Name == "Diagnostics" && symbol.ContainingNamespace.Name == "System";
    }

    public static bool IsConditionalAttribute(this INamedTypeSymbol symbol) {
      return symbol.Name == "ConditionalAttribute" && symbol.ContainingNamespace.IsSystemDiagnostics();
    }

    public static bool IsSystemObjectOrValueType(this INamedTypeSymbol symbol) {
      return symbol.SpecialType == SpecialType.System_Object || symbol.SpecialType == SpecialType.System_ValueType;
    }

    public static bool IsCombineImplicitlyCtor(this INamedTypeSymbol symbol) {
      return symbol.IsValueType && symbol.InstanceConstructors.Any(i => !i.IsImplicitlyDeclared && i.IsNotNullParameterExists());
    }

    private static int FindNotNullParameterIndex(this IMethodSymbol symbol) {
      return symbol.Parameters.IndexOf(i => i.Type.IsValueType && i.RefKind != RefKind.Out);
    }

    public static bool IsNotNullParameterExists(this IMethodSymbol symbol) {
      return symbol.OriginalDefinition.FindNotNullParameterIndex() != -1;
    }

    public static bool IsCombineImplicitlyCtorMethod(this IMethodSymbol symbol, out int notNullParameterIndex) {
      notNullParameterIndex = -1;

      var type = (INamedTypeSymbol)symbol.ReceiverType;
      if (type.IsValueType) {
        foreach (var ctor in type.InstanceConstructors) {
          if (!ctor.IsImplicitlyDeclared) {
            int index = ctor.FindNotNullParameterIndex();
            if (index != -1) {
              notNullParameterIndex = index;
              return symbol.EQ(ctor);
            }
          }
        }
      }
      return false;
    }

    public static bool IsEmptyPartialMethod(this IMethodSymbol symbol) {
      if (symbol.ReturnsVoid && symbol.IsPrivate()) {
        var p = symbol.GetType().GetProperty("DeclarationModifiers", BindingFlags.NonPublic | BindingFlags.Instance);
        if (p != null) {
          var declarationModifiers = p.GetValue(symbol);
          if (declarationModifiers.ToString().Contains("Partial")) {
            return symbol.PartialImplementationPart == null || symbol.PartialDefinitionPart != null;
          }
        }
      }

      return false;
    }

    public static bool IsInterfaceDefaultMethod(this IMethodSymbol symbol) {
      return symbol.ContainingType.TypeKind == TypeKind.Interface && !symbol.IsStatic && !symbol.IsAbstract;
    }

    public static string GetMetaDataAttributeFlags(this ISymbol symbol, PropertyMethodKind propertyMethodKind = 0) {
      const int kParametersMaxCount = 256;

      int accessibility = (int)symbol.DeclaredAccessibility;
      int isStatic = symbol.IsStatic ? 1 : 0;
      int flags = accessibility | (isStatic << 3);
      switch (symbol.Kind) {
        case SymbolKind.Method: {
          var methodSymbol = (IMethodSymbol)symbol;
          if (!methodSymbol.ReturnsVoid) {
            flags |= 1 << 7;
          }
          int parameterCount = methodSymbol.Parameters.Length;
          if (parameterCount > 0) {
            Contract.Assert(parameterCount < kParametersMaxCount);
            flags |= parameterCount << 8;
          }
          if (methodSymbol.IsGenericMethod) {
            int typeCount = methodSymbol.TypeParameters.Length;
            Contract.Assert(typeCount < kParametersMaxCount);
            flags |= typeCount << 16;
          }
          break;
        }
        case SymbolKind.NamedType: {
          var nameType = (INamedTypeSymbol)symbol;
          if (nameType.IsGenericType) {
            int typeCount = nameType.TypeParameters.Length;
            Contract.Assert(typeCount < kParametersMaxCount);
            flags |= typeCount << 8;
          }
          break;
        }
        case SymbolKind.Property:
        case SymbolKind.Event: {
          if (propertyMethodKind > 0) {
            flags |= (int)propertyMethodKind << 8;
          }
          break;
        }
      }
      return $"0x{flags:X}";
    }

    public static SyntaxNode GetDeclaringSyntaxNode(this ISymbol symbol) {
      return symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
    }

    public static bool IsNil(this LuaExpressionSyntax expression) {
      return expression == null || expression == LuaIdentifierNameSyntax.Nil || expression == LuaIdentifierLiteralExpressionSyntax.Nil;
    }

    public static void RemoveNilAtTail(this List<LuaExpressionSyntax> expressions) {
      int pos = expressions.FindLastIndex(i => !i.IsNil());
      int nilStartIndex = pos + 1;
      int nilArgumentCount = expressions.Count - nilStartIndex;
      if (nilArgumentCount > 0) {
        expressions.RemoveRange(nilStartIndex, nilArgumentCount);
      }
    }

    public static T Accept<T>(this CSharpSyntaxNode node, LuaSyntaxNodeTransform transform) where T : LuaSyntaxNode {
      return (T)node.Accept(transform);
    }

    public static LuaExpressionSyntax AcceptExpression(this CSharpSyntaxNode node, LuaSyntaxNodeTransform transform) {
      return node.Accept<LuaExpressionSyntax>(transform);
    }

#region hard code for protobuf-net

    public static bool IsProtobufNetDeclaration(this INamedTypeSymbol type) {
      foreach (var attr in type.GetAttributes()) {
        var attribute = attr.AttributeClass;
        if (attribute.Name == "ProtoContractAttribute" && attribute.ContainingNamespace.Name == "ProtoBuf") {
          return true;
        }
      }
      return false;
    }

    public static bool IsProtobufNetSpecialField(this IFieldSymbol symbol, out string name) {
      name = null;
      if (!symbol.IsStatic && symbol.IsPrivate() && symbol.IsFromCode()) {
        if (symbol.Name.StartsWith("_")) {
          var containingType = symbol.ContainingType;
          if (containingType.IsProtobufNetDeclaration()) {
            name = symbol.Name.TrimStart('_');
            return true;
          }
        }
      }
      return false;
    }

    public static bool IsProtobufNetSpecialProperty(this IPropertySymbol symbol) {
      if (!symbol.IsStatic && symbol.IsPublic() && symbol.IsFromCode()) {
        var containingType = symbol.ContainingType;
        if (containingType.IsProtobufNetDeclaration()) {
          string fieldName = "_" + symbol.Name;
          var field = symbol.ContainingType.GetMembers(fieldName).OfType<IFieldSymbol>().FirstOrDefault();
          if (field != null && !field.IsStatic && field.IsPrivate()) {
            return true;
          }
        }
      }
      return false;
    }

#endregion
  }
}
