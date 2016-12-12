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
        public readonly static LuaIdentifierNameSyntax System = new LuaIdentifierNameSyntax("System");
        public readonly static LuaIdentifierNameSyntax Namespace = new LuaIdentifierNameSyntax("namespace");
        public readonly static LuaIdentifierNameSyntax Class = new LuaIdentifierNameSyntax("class");
        public readonly static LuaIdentifierNameSyntax Struct = new LuaIdentifierNameSyntax("struct");
        public readonly static LuaIdentifierNameSyntax Interface = new LuaIdentifierNameSyntax("interface");
        public readonly static LuaIdentifierNameSyntax Enum = new LuaIdentifierNameSyntax("enum");
        public readonly static LuaIdentifierNameSyntax Temp1 = new LuaIdentifierNameSyntax("default");
        public readonly static LuaIdentifierNameSyntax True = new LuaIdentifierNameSyntax("true");
        public readonly static LuaIdentifierNameSyntax Throw = new LuaIdentifierNameSyntax("System.throw");
        public readonly static LuaIdentifierNameSyntax Each = new LuaIdentifierNameSyntax("System.each");
        public readonly static LuaIdentifierNameSyntax YieldReturn = new LuaIdentifierNameSyntax("System.yieldReturn");
        public readonly static LuaIdentifierNameSyntax Object = new LuaIdentifierNameSyntax("System.Object");
        public readonly static LuaIdentifierNameSyntax One = new LuaIdentifierNameSyntax("1");

        public LuaIdentifierNameSyntax(string valueText) {
            ValueText = valueText;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
