using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public class LuaSyntaxNode {
        internal virtual void Render(LuaRenderer renderer) {
            throw new NotSupportedException($"{this.GetType().Name} is not override");
        }

        public sealed class Semicolon {
            public override string ToString() {
                return ";";
            }
        }

        public class Tokens : Keyword {
            public static string Empty => string.Empty;
            public static readonly Semicolon Semicolon = new Semicolon();
            public const string OpenParentheses = "(";
            public const string CloseParentheses = ")";
            public const string OpenBrace = "{";
            public const string CloseBrace = "}";
            public const string OpenBracket = "[";
            public const string CloseBracket = "]";
            public const string ObjectColon = ":";
            public const string Dot = ".";
            public const string Quote = "\"";
            public const string SingleQuote = "'";
            public new const string Equals = "=";
            public const string EqualsEquals = "==";
            public const string NotEquals = "~=";
            public const string Yield = "yield";
            public const string Plus = "+";
            public const string Sub = "-";
            public const string Div = "//";
            public const string ShortComment = "--";
            public const string OpenLongComment = "--[[";
            public const string CloseLongComment = "--]]";
            public const string OpenDoubleBrace = "[[";
            public const string CloseDoubleBrace = "]]";
            public const string Ctor = "ctor";
            public const string This = "this";
            public const string Get = "get";
            public const string Set = "set";
            public const string Add = "add";
            public const string Remove = "remove";
            public const string Label = "::";
            public const string Concatenation = "..";
        }

        public class Keyword {
            public const string And = "and";
            public const string Break = "break";
            public const string Do = "do";
            public const string Else = "else";
            public const string ElseIf = "elseif";
            public const string End = "end";

            public const string False = "false";
            public const string For = "for";
            public const string Function = "function";
            public const string Goto = "goto";
            public const string If = "if";
            public const string In = "in";

            public const string Local = "local";
            public const string Nil = "nil";
            public const string Not = "not";
            public const string Or = "or";
            public const string Repeat = "repeat";
            public const string Return = "return";

            public const string Then = "then";
            public const string True = "true";
            public const string Until = "until";
            public const string While = "while";
        }

        public static string SpecailWord(string s) {
            return "__" + s + "__";
        }

        public static string[] TempIdentifiers = {
            "default", "extern", "ref", "out", "internal",
            "void",  "case", "new", "object", "using",
            "fixed", "override", "abstract", "checked", "virtual",
        };

        public readonly static HashSet<string> ReservedWords = new HashSet<string>() {
            //  lua reserved words
            "and", "elseif", "end", "function", "local", "nil", "not", "or", "repeat", "then", "until",
            
            // compiler reserved words
            "System", "Linq",
        };

        public readonly static HashSet<string> SpecialReservedWords = new HashSet<string>() {
            // lua metatable methods
            "__add", "__sub", "__mul", "__div", "__mod", "__pow", "__umm", "__idiv",
            "__band", "__bor", "__bxor", "__bnot", "__shl", "__shr", "__concat", "__len",
            "__eq", "__lt", "__le", "__index", "__newindex", "__call",

            // adapter special methods 
            "__id__", "__name__", "__kind__", "__base__", "__ctor__", "__inherits__", "__interfaces__", "__default__",
        };

        public static bool IsReservedWord(string identifier) {
            return ReservedWords.Contains(identifier);
        }
    }

    public sealed class LuaSyntaxList<T> : List<T> where T : LuaSyntaxNode {
        public new void Add(T node) {
            if(node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            base.Add(node);
        }

        public new void AddRange(IEnumerable<T> collection) {
            foreach(var item in collection) {
                if(item == null) {
                    throw new ArgumentNullException(nameof(item));
                }
                base.Add(item);
            }
        }
    }
}