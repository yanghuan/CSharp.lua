using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaClassDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaClassDeclarationSyntax(LuaIdentifierNameSyntax name) : base(name) {
            UpdateIdentifiers(LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Class);
        }
    }
}
