using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public abstract class LuaExpressionSyntax : LuaSyntaxNode {
    }

    public sealed class LuaAssignmentExpressionSyntax : LuaExpressionSyntax {
        public LuaExpressionSyntax Left { get; }
        public string OperatorToken => Tokens.Equals;
        public LuaExpressionSyntax Right { get; }

        public LuaAssignmentExpressionSyntax(LuaExpressionSyntax left, LuaExpressionSyntax right) {
            Left = left;
            Right = right;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaMultipleAssignmentExpressionSyntax : LuaExpressionSyntax {
        public LuaSyntaxList<LuaExpressionSyntax> Lefts { get; } = new LuaSyntaxList<LuaExpressionSyntax>();
        public string OperatorToken => Tokens.Equals;
        public LuaSyntaxList<LuaExpressionSyntax> Rights { get; } = new LuaSyntaxList<LuaExpressionSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaLineMultipleAssignmentExpressionSyntax : LuaExpressionSyntax {
        public LuaSyntaxList<LuaAssignmentExpressionSyntax> Assignments { get; } = new LuaSyntaxList<LuaAssignmentExpressionSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }
}
