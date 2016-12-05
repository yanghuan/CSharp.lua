using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public abstract class LuaStatementSyntax : LuaSyntaxNode {
    }

    public sealed class LuaStatementListSyntax : LuaStatementSyntax {
        public List<LuaStatementSyntax> Statements { get; } = new List<LuaStatementSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
