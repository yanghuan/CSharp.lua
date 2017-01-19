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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        public CompilationErrorException(string message) : base(message) {
        }
    }

    public static class Utility {
        public static Dictionary<string, string[]> GetCommondLines(string[] args) {
            Dictionary<string, string[]> cmds = new Dictionary<string, string[]>();

            string key = "";
            List<string> values = new List<string>();

            foreach(string arg in args) {
                string i = arg.Trim();
                if(i.StartsWith("-")) {
                    if(!string.IsNullOrEmpty(key)) {
                        cmds.Add(key, values.ToArray());
                        key = "";
                        values.Clear();
                    }
                    key = i;
                }
                else {
                    values.Add(i);
                }
            }

            if(!string.IsNullOrEmpty(key)) {
                cmds.Add(key, values.ToArray());
            }
            return cmds;
        }

        public static T First<T>(this IList<T> list) {
            return list[0];
        }

        public static T Last<T>(this IList<T> list) {
            return list[list.Count - 1];
        }

        public static T GetOrDefault<T>(this IList<T> list, int index, T v = default(T)) {
            return index >= 0 && index < list.Count ? list[index] : v;
        }

        public static T GetOrDefault<K, T>(this IDictionary<K, T> dict, K key, T t = default(T)) {
            T v;
            if(dict.TryGetValue(key, out v)) {
                return v;
            }
            return t;
        }

        public static string GetArgument(this Dictionary<string, string[]> args, string name, bool isOption = false) {
            string[] values = args.GetOrDefault(name);
            if(values == null || values.Length == 0) {
                if(isOption) {
                    return null;
                }
                throw new CmdArgumentException(name + " is not found");
            }
            return values[0];
        }

        public static string GetCurrentDirectory(string path) {
            const string CurrentDirectorySign1 = "~/";
            const string CurrentDirectorySign2 = "~\\";

            if(path.StartsWith(CurrentDirectorySign1)) {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(CurrentDirectorySign1.Length));
            }
            else if(path.StartsWith(CurrentDirectorySign2)) {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(CurrentDirectorySign2.Length));
            }

            return Path.Combine(Environment.CurrentDirectory, path);
        }

        public static string[] Split(string s, bool isPath = true) {
            HashSet<string> list = new HashSet<string>();
            if(!string.IsNullOrEmpty(s)) {
                string[] array = s.Split(';');
                foreach(string i in array) {
                    list.Add(isPath ? GetCurrentDirectory(i) : i);
                }
            }
            return list.ToArray();
        }

        public static bool IsPrivate(this SyntaxTokenList modifiers) {
            return modifiers.Any(i => i.IsKind(SyntaxKind.PrivateKeyword));
        }

        public static bool IsStatic(this SyntaxTokenList modifiers) {
            return modifiers.Any(i => i.IsKind(SyntaxKind.StaticKeyword));
        }

        public static bool IsAbstract(this SyntaxTokenList modifiers) {
            return modifiers.Any(i => i.IsKind(SyntaxKind.AbstractKeyword));
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

        public static bool IsStringType(this ITypeSymbol type) {
            return type.SpecialType == SpecialType.System_String;
        }

        public static bool IsDelegateType(this ITypeSymbol type) {
            return type.TypeKind == TypeKind.Delegate;
        }

        public static bool IsIntegerType(this ITypeSymbol type) {
            return type.SpecialType >= SpecialType.System_SByte && type.SpecialType <= SpecialType.System_UInt64;
        }

        public static bool IsImmutable(this ITypeSymbol type) {
            bool isImmutable = (type.IsValueType && type.IsDefinition) || type.IsStringType() || type.IsDelegateType();
            return isImmutable;
        }

        public static bool IsInterfaceImplementation<T>(this T symbol) where T : ISymbol {
            if(!symbol.IsStatic) {
                var type = symbol.ContainingType;
                if(type != null) {
                    var interfaceSymbols = type.AllInterfaces.SelectMany(i => i.GetMembers().OfType<T>());
                    return interfaceSymbols.Any(i => symbol.Equals(type.FindImplementationForInterfaceMember(i)));
                }
            }
            return false;
        }

        public static IEnumerable<T> InterfaceImplementations<T>(this T symbol) where T : ISymbol {
            if(!symbol.IsStatic) {
                var type = symbol.ContainingType;
                if(type != null) {
                    var interfaceSymbols = type.AllInterfaces.SelectMany(i => i.GetMembers().OfType<T>());
                    return interfaceSymbols.Where(i => symbol.Equals(type.FindImplementationForInterfaceMember(i)));
                }
            }
            return null;
        }

        public static bool IsFromCode(this ISymbol symbol) {
            return !symbol.DeclaringSyntaxReferences.IsEmpty;
        }

        public static bool IsOverridable(this ISymbol symbol) {
            return !symbol.IsStatic && (symbol.IsAbstract || symbol.IsVirtual || symbol.IsOverride);
        }

        public static ISymbol OverriddenSymbol(this ISymbol symbol) {
            switch(symbol.Kind) {
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
            while(true) {
                ISymbol overriddenSymbol = symbol.OverriddenSymbol();
                if(overriddenSymbol != null) {
                    if(overriddenSymbol.Equals(superSymbol)) {
                        return true;
                    }
                }
                else {
                    break;
                }
            }
            return false;
        }

        public static bool IsPropertyField(this IPropertySymbol symbol) {
            if(symbol.IsOverridable()) {
                return false;
            }

            var syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
            if(syntaxReference != null) {
                var node = (PropertyDeclarationSyntax)syntaxReference.GetSyntax();
                bool hasGet = false;
                bool hasSet = false;
                if(node.AccessorList != null) {
                    foreach(var accessor in node.AccessorList.Accessors) {
                        if(accessor.Body != null) {
                            if(accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) {
                                Contract.Assert(!hasGet);
                                hasGet = true;
                            }
                            else {
                                Contract.Assert(!hasSet);
                                hasSet = true;
                            }
                        }
                    }
                }
                else {
                    Contract.Assert(!hasGet);
                    hasGet = true;
                }
                bool isAuto = !hasGet && !hasSet;
                if(isAuto) {
                    if(symbol.IsInterfaceImplementation()) {
                        isAuto = false;
                    }
                }
                return isAuto;
            }
            return false;
        }

        public static bool IsEventFiled(this IEventSymbol symbol) {
            if(symbol.IsOverridable()) {
                return false;
            }

            var syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
            if(syntaxReference != null) {
                bool isField = syntaxReference.GetSyntax().IsKind(SyntaxKind.VariableDeclarator);
                if(isField) {
                    if(symbol.IsInterfaceImplementation()) {
                        isField = false;
                    }
                }
                return isField;
            }
            return false;
        }

        public static bool IsAssignment(this SyntaxKind kind) {
            return kind >= SyntaxKind.SimpleAssignmentExpression && kind <= SyntaxKind.RightShiftAssignmentExpression;
        }

        private static INamedTypeSymbol systemLinqEnumerableType_;

        public static bool IsSystemLinqEnumerable(this INamedTypeSymbol symbol) {
            if(systemLinqEnumerableType_ != null) {
                return symbol == systemLinqEnumerableType_;
            }
            else {
                bool success = symbol.ToString() == LuaIdentifierNameSyntax.SystemLinqEnumerable.ValueText;
                if(success) {
                    systemLinqEnumerableType_ = symbol;
                }
                return success;
            }
        }

        public static string GetLocationString(this SyntaxNode node) {
            var location = node.SyntaxTree.GetLocation(node.Span);
            var methodInfo = location.GetType().GetMethod("GetDebuggerDisplay", BindingFlags.Instance | BindingFlags.NonPublic);
            return (string)methodInfo.Invoke(location, null);
        }

        public static bool IsSubclassOf(this ITypeSymbol child, ITypeSymbol parent) {
            ITypeSymbol p = child;
            if(p == parent) {
                return false;
            }

            while(p != null) {
                if(p == parent) {
                    return true;
                }
                p = p.BaseType;
            }
            return false;
        }

        private static bool IsImplementInterface(this ITypeSymbol implementType, ITypeSymbol interfaceType) {
            ITypeSymbol t = implementType;
            while(t != null) {
                var interfaces = implementType.AllInterfaces;
                foreach(var i in interfaces) {
                    if(i == interfaceType || i.IsImplementInterface(interfaceType)) {
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

        private static bool IsNumberTypeAssignableFrom(this ITypeSymbol left, ITypeSymbol right) {
            if(left.SpecialType.IsBaseNumberType() && right.SpecialType.IsBaseNumberType()) {
                SpecialType begin;
                switch(left.SpecialType) {
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

        public static bool IsAssignableFrom(this ITypeSymbol left, ITypeSymbol right) {
            if(left == right) {
                return true;
            }

            if(left.IsNumberTypeAssignableFrom(right)) {
                return true;
            }

            if(right.IsSubclassOf(left)) {
                return true;
            }

            if(left.TypeKind == TypeKind.Interface) {
                return right.IsImplementInterface(left);
            }

            return false;
        }

        public static void CheckOriginalDefinition(ref IMethodSymbol symbol) {
            if(symbol.IsExtensionMethod) {
                if(symbol.ReducedFrom != null) {
                    symbol = symbol.ReducedFrom;
                }
            }
            else {
                if(symbol.OriginalDefinition != symbol) {
                    symbol = symbol.OriginalDefinition;
                }
            }
        }
    }
}
