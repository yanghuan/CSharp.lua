using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace CSharpLua.LuaAst {
    public class LuaIdentifierNameSyntax : LuaExpressionSyntax {
        public string ValueText { get; }

        public readonly static LuaIdentifierNameSyntax Empty = new LuaIdentifierNameSyntax("");
        public readonly static LuaIdentifierNameSyntax Placeholder = new LuaIdentifierNameSyntax("_");
        public readonly static LuaIdentifierNameSyntax One = new LuaIdentifierNameSyntax("1");
        public readonly static LuaIdentifierNameSyntax System = new LuaIdentifierNameSyntax("System");
        public readonly static LuaIdentifierNameSyntax Namespace = new LuaIdentifierNameSyntax("namespace");
        public readonly static LuaIdentifierNameSyntax Class = new LuaIdentifierNameSyntax("class");
        public readonly static LuaIdentifierNameSyntax Struct = new LuaIdentifierNameSyntax("struct");
        public readonly static LuaIdentifierNameSyntax Interface = new LuaIdentifierNameSyntax("interface");
        public readonly static LuaIdentifierNameSyntax Enum = new LuaIdentifierNameSyntax("enum");
        public readonly static LuaIdentifierNameSyntax Value = new LuaIdentifierNameSyntax("value");
        public readonly static LuaIdentifierNameSyntax This = new LuaIdentifierNameSyntax("this");
        public readonly static LuaIdentifierNameSyntax True = new LuaIdentifierNameSyntax("true");
        public readonly static LuaIdentifierNameSyntax Throw = new LuaIdentifierNameSyntax("System.throw");
        public readonly static LuaIdentifierNameSyntax Each = new LuaIdentifierNameSyntax("System.each");
        public readonly static LuaIdentifierNameSyntax YieldReturn = new LuaIdentifierNameSyntax("System.yieldReturn");
        public readonly static LuaIdentifierNameSyntax Object = new LuaIdentifierNameSyntax("System.Object");
        public readonly static LuaIdentifierNameSyntax Array = new LuaIdentifierNameSyntax("System.Array");
        public readonly static LuaIdentifierNameSyntax ArrayEmpty = new LuaIdentifierNameSyntax("System.Array.Empty");
        public readonly static LuaIdentifierNameSyntax Create = new LuaIdentifierNameSyntax("System.create");
        public readonly static LuaIdentifierNameSyntax ThisAdd = new LuaIdentifierNameSyntax("this:Add");
        public readonly static LuaIdentifierNameSyntax StaticCtor = new LuaIdentifierNameSyntax("__staticCtor__");
        public readonly static LuaIdentifierNameSyntax Init = new LuaIdentifierNameSyntax("__init__");
        public readonly static LuaIdentifierNameSyntax Ctor = new LuaIdentifierNameSyntax("__ctor__");
        public readonly static LuaIdentifierNameSyntax Inherits = new LuaIdentifierNameSyntax("__inherits__");
        public readonly static LuaIdentifierNameSyntax Default = new LuaIdentifierNameSyntax("__default__");
        public readonly static LuaIdentifierNameSyntax Property = new LuaIdentifierNameSyntax("System.property");
        public readonly static LuaIdentifierNameSyntax Event = new LuaIdentifierNameSyntax("System.event");
        public readonly static LuaIdentifierNameSyntax Nil = new LuaIdentifierNameSyntax("nil");
        public readonly static LuaIdentifierNameSyntax TypeOf = new LuaIdentifierNameSyntax("System.typeof");
        public readonly static LuaIdentifierNameSyntax Continue = new LuaIdentifierNameSyntax("continue");
        public readonly static LuaIdentifierNameSyntax StringChar = new LuaIdentifierNameSyntax("string.char");
        public readonly static LuaIdentifierNameSyntax ToStr = new LuaIdentifierNameSyntax("ToString");
        public readonly static LuaIdentifierNameSyntax ToEnumString = new LuaIdentifierNameSyntax("ToEnumString");
        public readonly static LuaIdentifierNameSyntax StringConcat = new LuaIdentifierNameSyntax("System.strconcat");
        public readonly static LuaIdentifierNameSyntax DelegateCombine = new LuaIdentifierNameSyntax("System.combine");
        public readonly static LuaIdentifierNameSyntax DelegateRemove = new LuaIdentifierNameSyntax("System.remove");
        public readonly static LuaIdentifierNameSyntax DelegateBind = new LuaIdentifierNameSyntax("System.bind");
        public readonly static LuaIdentifierNameSyntax IntegerDiv = new LuaIdentifierNameSyntax("System.div");
        public readonly static LuaIdentifierNameSyntax IntegerMod = new LuaIdentifierNameSyntax("System.mod");
        public readonly static LuaIdentifierNameSyntax BitAnd = new LuaIdentifierNameSyntax("System.band");
        public readonly static LuaIdentifierNameSyntax BitOr = new LuaIdentifierNameSyntax("System.bor");
        public readonly static LuaIdentifierNameSyntax BitXor = new LuaIdentifierNameSyntax("System.xor");
        public readonly static LuaIdentifierNameSyntax ShiftRight = new LuaIdentifierNameSyntax("System.sr");
        public readonly static LuaIdentifierNameSyntax ShiftLeft = new LuaIdentifierNameSyntax("System.sl");
        public readonly static LuaIdentifierNameSyntax Try = new LuaIdentifierNameSyntax("System.try");
        public readonly static LuaIdentifierNameSyntax Is = new LuaIdentifierNameSyntax("System.is");
        public readonly static LuaIdentifierNameSyntax As = new LuaIdentifierNameSyntax("System.as");
        public readonly static LuaIdentifierNameSyntax Cast = new LuaIdentifierNameSyntax("System.cast");
        public readonly static LuaIdentifierNameSyntax Using = new LuaIdentifierNameSyntax("System.using");
        public readonly static LuaIdentifierNameSyntax Linq = new LuaIdentifierNameSyntax("Linq");
        public readonly static LuaIdentifierNameSyntax SystemLinqEnumerable = new LuaIdentifierNameSyntax("System.Linq.Enumerable");
        public readonly static LuaIdentifierNameSyntax New = new LuaIdentifierNameSyntax("new");
        public readonly static LuaIdentifierNameSyntax Access = new LuaIdentifierNameSyntax("System.access");
        public readonly static LuaIdentifierNameSyntax Format = new LuaIdentifierNameSyntax("Format");
        public readonly static LuaIdentifierNameSyntax Delegate = new LuaIdentifierNameSyntax("System.Delegate");
        public readonly static LuaIdentifierNameSyntax Int = new LuaIdentifierNameSyntax("System.Int");

        public LuaIdentifierNameSyntax(string valueText) {
            ValueText = valueText;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaPropertyOrEventIdentifierNameSyntax : LuaIdentifierNameSyntax {
        public bool IsGetOrAdd { get; set; } = true;
        public bool IsProperty { get; }

        public LuaPropertyOrEventIdentifierNameSyntax(bool isProperty, string valueText) : base(valueText) {
            IsProperty = isProperty;
        }

        public string PrefixToken {
            get {
                if(IsProperty) {
                    return IsGetOrAdd ? Tokens.Get : Tokens.Set;
                }
                else {
                    return IsGetOrAdd ? Tokens.Add : Tokens.Remove;
                }
            }
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaInternalMethodIdentifierNameSyntax : LuaIdentifierNameSyntax {
        public LuaInternalMethodIdentifierNameSyntax(string valueText) : base(valueText) {
        }
    }
}
