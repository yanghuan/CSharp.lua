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
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpLua.LuaAst {
  public class LuaSyntaxNode {
    internal virtual void Render(LuaRenderer renderer) {
      throw new NotSupportedException($"{this.GetType().Name} is not override");
    }

    public sealed class Semicolon {
      public const string kSemicolon = ";";

      public override string ToString() {
        return kSemicolon;
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
      public const string Multiply = "*";
      public const string IntegerDiv = "//";
      public const string Div = "/";
      public const string Mod = "%";
      public const string BitAnd = "&";
      public const string BitOr = "|";
      public const string BitXor = "~";
      public const string BitNot = "~";
      public const string ShiftLeft = "<<";
      public const string ShiftRight = ">>";
      public const string ShortComment = "--";
      public const string OpenLongComment = "--[[";
      public const string CloseLongComment = "--]]";
      public const string OpenDoubleBrace = "[[";
      public const string CloseDoubleBrace = "]]";
      public const string OpenSummary = "<summary>";
      public const string CloseSummary = "</summary>";
      public const string Ctor = "ctor";
      public const string This = "this";
      public const string Get = "get";
      public const string Set = "set";
      public const string Add = "add";
      public const string Remove = "remove";
      public const string Index = "index";
      public const string Label = "::";
      public const string Concatenation = "..";
      public const string Params = "...";
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

    public const int kUpvaluesMaxCount = 60;
    public const int kLocalVariablesMaxCount = 200;

    private static string SpecailWord(string s) {
      return "__" + s + "__";
    }

    public static string GetCtorNameString(int ctorIndex) {
      Contract.Assert(ctorIndex > 0);
      return SpecailWord(Tokens.Ctor + ctorIndex);
    }

    public static readonly string[] TempIdentifiers = {
      "default", "extern", "ref", "out", "try",
      "case", "void", "byte", "char", "uint",
      "lock",  "using", "fixed", "const", "object",
      "internal", "virtual",
    };

    private static readonly HashSet<string> ReservedWords = new HashSet<string>() {            
      // compiler reserved words
      "System", "Linq",
    };

    static LuaSyntaxNode() {
      // lua reserved words
      foreach (var field in typeof(Keyword).GetFields()) {
        ReservedWords.Add(field.GetRawConstantValue().ToString());
      }
    }

    private static readonly HashSet<string> SpecialMethodReservedWords = new HashSet<string>() {
      // lua metatable methods
      "__add", "__sub", "__mul", "__div", "__mod", "__pow", "__umm", "__idiv",
      "__band", "__bor", "__bxor", "__bnot", "__shl", "__shr", "__concat", "__len",
      "__eq", "__lt", "__le", "__index", "__newindex", "__call", "__gc",

      // adapter special methods 
      "__name__", "__ctor__", "__metadata__", "__clone__",
    };

    public static bool IsReservedWord(string identifier) {
      return ReservedWords.Contains(identifier);
    }

    public static bool IsMethodReservedWord(string identifier) {
      return IsReservedWord(identifier) || SpecialMethodReservedWords.Contains(identifier);
    }
  }

  public sealed class LuaSyntaxList<T> : List<T> where T : LuaSyntaxNode {
    public new void Add(T node) {
      if (node == null) {
        throw new ArgumentNullException(nameof(node));
      }
      base.Add(node);
    }

    public new void AddRange(IEnumerable<T> collection) {
      foreach (var item in collection) {
        if (item == null) {
          throw new ArgumentNullException(nameof(item));
        }
        base.Add(item);
      }
    }
  }
}
