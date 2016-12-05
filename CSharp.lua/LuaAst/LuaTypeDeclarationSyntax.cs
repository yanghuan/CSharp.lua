using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public abstract class LuaTypeDeclarationSyntax : LuaWrapFunctionStatementSynatx {
        public LuaLocalDeclarationStatementSyntax Local { get; } = new LuaLocalDeclarationStatementSyntax();
        public LuaStatementListSyntax MethodList { get; } = new LuaStatementListSyntax();

        public LuaTypeDeclarationSyntax(LuaIdentifierNameSyntax name) : base(name) {
            Add(Local);
            Add(MethodList);
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }

        public void AddMethod(LuaIdentifierNameSyntax name, LuaFunctionExpressSyntax method) {
            Local.Declaration.Variables.Add(name);
        }
    }

    public sealed class LuaClassDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaClassDeclarationSyntax(LuaIdentifierNameSyntax name) : base(name) {
            UpdateIdentifiers(LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Class, LuaIdentifierNameSyntax.Namespace);
        }
    }

    public sealed class LuaStructDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaStructDeclarationSyntax(LuaIdentifierNameSyntax name) : base(name) {
            UpdateIdentifiers(LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Struct, LuaIdentifierNameSyntax.Namespace);
        }
    }

    public sealed class LuaInterfaceDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaInterfaceDeclarationSyntax(LuaIdentifierNameSyntax name) : base(name) {
            UpdateIdentifiers(LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Interface);
        }
    }

    public sealed class LuaEnumDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaEnumDeclarationSyntax(LuaIdentifierNameSyntax name) : base(name) {
            UpdateIdentifiers(LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Enum);
        }
    }
}
