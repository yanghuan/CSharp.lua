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
            if(left == null) {
                throw new ArgumentNullException(nameof(left));
            }
            if(right == null) {
                throw new ArgumentNullException(nameof(right));
            }
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

    public sealed class LuaLineMultipleExpressionSyntax : LuaExpressionSyntax {
        public LuaSyntaxList<LuaExpressionSyntax> Assignments { get; } = new LuaSyntaxList<LuaExpressionSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaBinaryExpressionSyntax : LuaExpressionSyntax {
        public LuaExpressionSyntax Left { get; }
        public string OperatorToken { get; }
        public LuaExpressionSyntax Right { get; }

        public LuaBinaryExpressionSyntax(LuaExpressionSyntax left, string operatorToken, LuaExpressionSyntax right) {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaPrefixUnaryExpressionSyntax : LuaExpressionSyntax {
        public LuaExpressionSyntax Operand { get; }
        public string OperatorToken { get; }

        public LuaPrefixUnaryExpressionSyntax(LuaExpressionSyntax operand, string operatorToken) {
            if(operand == null) {
                throw new ArgumentNullException(nameof(operand));
            }
            Operand = operand;
            OperatorToken = operatorToken;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaParenthesizedExpressionSyntax : LuaExpressionSyntax {
        public LuaExpressionSyntax Expression { get; }
        public string OpenParenToken => Tokens.OpenParentheses;
        public string CloseParenToken => Tokens.CloseParentheses;

        public LuaParenthesizedExpressionSyntax(LuaExpressionSyntax expression) {
            if(expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            Expression = expression;
        }

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaCodeTemplateExpressionSyntax : LuaExpressionSyntax {
        public readonly LuaSyntaxList<LuaExpressionSyntax> Codes = new LuaSyntaxList<LuaExpressionSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaCodeTemplateParamsExpressionSyntax : LuaExpressionSyntax {
        public readonly LuaSyntaxList<LuaExpressionSyntax> Expressions = new LuaSyntaxList<LuaExpressionSyntax>();

        internal override void Render(LuaRenderer renderer) {
            renderer.Render(this);
        }
    }

    public sealed class LuaArrayRankSpecifierSyntax : LuaSyntaxNode {
        public int Rank { get; }
        public readonly List<LuaExpressionSyntax> Sizes = new List<LuaExpressionSyntax>();

        public LuaArrayRankSpecifierSyntax(int rank) {
            Rank = rank;
        }
    }

    public sealed class LuaArrayTypeAdapterExpressionSyntax : LuaExpressionSyntax {
        public LuaInvocationExpressionSyntax InvocationExpression { get; }
        public LuaArrayRankSpecifierSyntax RankSpecifier { get; }

        public LuaArrayTypeAdapterExpressionSyntax(LuaInvocationExpressionSyntax invocationExpression, LuaArrayRankSpecifierSyntax rankSpecifier) {
            if(invocationExpression == null) {
                throw new ArgumentNullException(nameof(invocationExpression));
            }
            if(rankSpecifier == null) {
                throw new ArgumentNullException(nameof(rankSpecifier));
            }
            InvocationExpression = invocationExpression;
            RankSpecifier = rankSpecifier;
        }

        public LuaExpressionSyntax BaseType {
            get {
                return InvocationExpression.ArgumentList.Arguments[0].Expression;
            }
        }

        public bool IsSimapleArray {
            get {
                return RankSpecifier.Rank == 1;
            }
        }

        internal override void Render(LuaRenderer renderer) {
            InvocationExpression.Render(renderer);
        }
    }
}
