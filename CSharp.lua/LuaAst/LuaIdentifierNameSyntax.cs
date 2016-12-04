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

        public readonly static LuaIdentifierNameSyntax System = new LuaIdentifierNameSyntax("System");
        public readonly static LuaIdentifierNameSyntax Namespace = new LuaIdentifierNameSyntax("namespace");
        public readonly static LuaIdentifierNameSyntax Class = new LuaIdentifierNameSyntax("class");

        public LuaIdentifierNameSyntax(IdentifierNameSyntax node) : this(node.Identifier) {
        }

        public LuaIdentifierNameSyntax(SyntaxToken identifier) : this(identifier.ValueText) {
        }

        public LuaIdentifierNameSyntax(string valueText) {
            ValueText = valueText;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
