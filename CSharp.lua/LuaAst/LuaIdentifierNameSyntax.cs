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
        public readonly static LuaIdentifierNameSyntax Temp1 = new LuaIdentifierNameSyntax("default");
        public readonly static LuaIdentifierNameSyntax Value = new LuaIdentifierNameSyntax("value");
        public readonly static LuaIdentifierNameSyntax This = new LuaIdentifierNameSyntax("this");
        public readonly static LuaIdentifierNameSyntax True = new LuaIdentifierNameSyntax("true");
        public readonly static LuaIdentifierNameSyntax Throw = new LuaIdentifierNameSyntax("System.throw");
        public readonly static LuaIdentifierNameSyntax Each = new LuaIdentifierNameSyntax("System.each");
        public readonly static LuaIdentifierNameSyntax YieldReturn = new LuaIdentifierNameSyntax("System.yieldReturn");
        public readonly static LuaIdentifierNameSyntax Object = new LuaIdentifierNameSyntax("System.Object");
        public readonly static LuaIdentifierNameSyntax Array = new LuaIdentifierNameSyntax("System.Array");
        public readonly static LuaIdentifierNameSyntax Create = new LuaIdentifierNameSyntax("System.create");
        public readonly static LuaIdentifierNameSyntax ThisAdd = new LuaIdentifierNameSyntax("this:Add");
        public readonly static LuaIdentifierNameSyntax Default = new LuaIdentifierNameSyntax("__default__");
        public readonly static LuaIdentifierNameSyntax StaticCtor = new LuaIdentifierNameSyntax("__staticCtor__");
        public readonly static LuaIdentifierNameSyntax Init = new LuaIdentifierNameSyntax("__init__");
        public readonly static LuaIdentifierNameSyntax Ctor = new LuaIdentifierNameSyntax("__ctor__");
        public readonly static LuaIdentifierNameSyntax Inherits = new LuaIdentifierNameSyntax("__inherits__");
        public readonly static LuaIdentifierNameSyntax Property = new LuaIdentifierNameSyntax("System.property");
        public readonly static LuaIdentifierNameSyntax Event = new LuaIdentifierNameSyntax("System.event");
        public readonly static LuaIdentifierNameSyntax Nil = new LuaIdentifierNameSyntax("nil");

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
