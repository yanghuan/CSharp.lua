using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaNamespaceDeclarationSyntax : LuaWrapFunctionStatementSynatx {
        public LuaNamespaceDeclarationSyntax(LuaIdentifierNameSyntax name) : base(name) {
            UpdateIdentifiers(LuaIdentifierNameSyntax.System, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Namespace);
        }
    }
}
