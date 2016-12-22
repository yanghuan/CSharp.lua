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

        public static string[] SplitPaths(string path) {
            HashSet<string> list = new HashSet<string>();
            if(!string.IsNullOrEmpty(path)) {
                string[] paths = path.Split(';');
                foreach(string file in paths) {
                    list.Add(Utility.GetCurrentDirectory(path));
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

        public static bool IsReadOnly(this SyntaxTokenList modifiers) {
            return modifiers.Any(i => i.IsKind(SyntaxKind.ReadOnlyKeyword));
        }

        private static ITypeSymbol stringType_;
        public static bool IsStringType(this ITypeSymbol type) {
            if(stringType_ != null) {
                return type == stringType_;
            }
            else {
                bool success = type.IsReferenceType && type.IsDefinition && type.ToString() == "string";
                if(success) {
                    stringType_ = type;
                }
                return success;
            }
        }

        private static ITypeSymbol multicastDelegateType_;
        public static bool IsDelegateType(this ITypeSymbol type) {
            if(multicastDelegateType_ != null) {
                return multicastDelegateType_ == type.BaseType;
            }
            else {
                bool success = type.IsReferenceType && type.BaseType.ToString() == "System.MulticastDelegate";
                if(success) {
                    multicastDelegateType_ = type.BaseType;
                }
                return success;
            }
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

        public static bool IsOverridable(this ISymbol symbol) {
            return !symbol.IsStatic && (symbol.IsAbstract || symbol.IsVirtual || symbol.IsOverride);
        }

        public static bool IsPropertyField(this IPropertySymbol symbol) {
            if(!symbol.IsOverridable()) {
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
            if(!symbol.IsOverridable()) {
                return false;
            }

            var syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
            if(syntaxReference != null) {
                bool isField = syntaxReference.GetSyntax() is EventFieldDeclarationSyntax;
                if(isField) {
                    if(symbol.IsInterfaceImplementation()) {
                        isField = false;
                    }
                }
                return isField;
            }
            return false;
        }

        public static string GetLocationString(this SyntaxNode node) {
            var location = node.SyntaxTree.GetLocation(node.Span);
            var methodInfo = location.GetType().GetMethod("GetDebuggerDisplay", BindingFlags.Instance | BindingFlags.NonPublic);
            return (string)methodInfo.Invoke(location, null);
        }
    }
}
