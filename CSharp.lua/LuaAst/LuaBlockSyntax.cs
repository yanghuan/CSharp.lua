using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public sealed class LuaBlockSyntax : LuaStatementSyntax {
        public string OpenBraceToken { get; set; }
        public string CloseBraceToken { get; set; }
        public LuaSyntaxList<LuaStatementSyntax> Statements { get; } = new LuaSyntaxList<LuaStatementSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaBlockBlockSyntax : LuaStatementSyntax {
        public LuaBlockSyntax Body { get; } = new LuaBlockSyntax() {
            OpenBraceToken = Tokens.Do,
            CloseBraceToken = Tokens.End,
        };

        public LuaBlockBlockSyntax(LuaBlockSyntax block) {
            if(block == null) {
                throw new ArgumentNullException(nameof(block));
            }
            Body.Statements.AddRange(block.Statements);
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
