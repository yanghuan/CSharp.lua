using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
    public class LuaSyntaxNode {
        internal virtual void Render(LuaRenderer renderer) {
        }

        public static class Tokens {
            public static string Empty => string.Empty;
            public const string End = "end";
            public const string Semicolon = ";";
            public const string OpenParentheses = "(";
            public const string CloseParentheses = ")";
            public const string ObjectColon = ":";
            public const string Dot = ".";
            public const string Quote = "\"";
            public new const string Equals = "=";
        }
    }

    public sealed class LuaSyntaxList<T> : List<T> where T : LuaSyntaxNode {
        public new void Add(T node) {
            if(node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            base.Add(node);
        }
    }
}