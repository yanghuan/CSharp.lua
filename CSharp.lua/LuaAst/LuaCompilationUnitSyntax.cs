using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpLua.LuaAst {
    public sealed class LuaCompilationUnitSyntax : LuaSyntaxNode {
        private CompilationUnitSyntax original_;

        public LuaCompilationUnitSyntax(CompilationUnitSyntax compilationUnitSyntax) {
            original_ = compilationUnitSyntax;
        }
    }
}