using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpLua.LuaAst;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpLua {
    public sealed partial class LuaSyntaxNodeTransfor {
        private Dictionary<ISymbol, string> localReservedNames_ = new Dictionary<ISymbol, string>();
        private int localMappingCounter_;

        private abstract class LuaSyntaxSearcher : CSharpSyntaxWalker {
            private sealed class FoundException : Exception {
            }
            protected void Found() {
                throw new FoundException();
            }

            public bool Find(SyntaxNode root) {
                try {
                    Visit(root);
                }
                catch(FoundException) {
                    return true;
                }
                return false;
            }
        }

        private sealed class LocalVarSearcher : LuaSyntaxSearcher {
            private string name_;
            private SyntaxNode node_;

            public LocalVarSearcher(string name, SyntaxNode node) {
                name_ = name;
                node_ = node;
            }

            public override void VisitParameter(ParameterSyntax node) {
                if(node != node_) {
                    if(node.Identifier.ValueText == name_) {
                        Found();
                    }
                }
            }

            public override void VisitVariableDeclarator(VariableDeclaratorSyntax node) {
                if(node != node_) {
                    if(node.Identifier.ValueText == name_) {
                        Found();
                    } 
                }
            }
        }

        public bool IsLocalVarExists(string name, SyntaxNode parent, SyntaxNode node) {
            LocalVarSearcher searcher = new LocalVarSearcher(name, node);
            return searcher.Find(parent);
        }

        private string GetReservedNewName(string name, int index) {
            switch(index) {
                case 0:
                    return name.FirstLetterToUpper();
                case 1:
                    return name + "_";
                case 2:
                    return "_" + name;
                default:
                    return name + (index - 2);
            }
        }

        private bool CheckReservedWord(ref string name, SyntaxNode parent, SyntaxNode node) {
            if(LuaSyntaxNode.IsReservedWord(name)) {
                int index = 0;
                while(true) {
                    string newName = GetReservedNewName(name, index);
                    bool exists = IsLocalVarExists(newName, parent, node);
                    if(!exists) {
                        AddReservedMapping(newName, node);
                        name = newName;
                        return true;
                    }
                    ++index;
                }
            }
            return false;
        }

        private void AddReservedMapping(string name, SyntaxNode node) {
            ISymbol symbol = semanticModel_.GetDeclaredSymbol(node);
            Contract.Assert(symbol != null);
            localReservedNames_.Add(symbol, name);
        }

        private void CheckParameterName(ref LuaParameterSyntax parameter, MethodDeclarationSyntax parent, ParameterSyntax node) {
            string name = parameter.Identifier.ValueText;
            bool isReserved = CheckReservedWord(ref name, parent, node);
            if(isReserved) {
                parameter = new LuaParameterSyntax(new LuaIdentifierNameSyntax(name));
            }
        }

        private void CheckVariableDeclaratorName(ref LuaIdentifierNameSyntax identifierName, VariableDeclaratorSyntax node) {
            string name = identifierName.ValueText;
            bool isReserved = CheckReservedWord(ref name, node.Parent.Parent.Parent, node);
            if(isReserved) {
                identifierName = new LuaIdentifierNameSyntax(name);
            }
        }

        private void CheckReservedWord(ref string name, ISymbol symbol) {
            if(LuaSyntaxNode.IsReservedWord(name)) {
                name = localReservedNames_[symbol];
            }
        }

        private sealed class ContinueSearcher : LuaSyntaxSearcher {
            public override void VisitContinueStatement(ContinueStatementSyntax node) {
                Found();
            }
        }

        private bool IsContinueExists(SyntaxNode node) {
            ContinueSearcher searcher = new ContinueSearcher();
            return searcher.Find(node);
        }

        private sealed class ReturnStatementSearcher : LuaSyntaxSearcher {
            public override void VisitReturnStatement(ReturnStatementSyntax node) {
                Found();
            }
        }

        public bool IsReturnExists(SyntaxNode node) {
            ReturnStatementSearcher searcher = new ReturnStatementSearcher();
            return searcher.Find(node);
        }
    }
}
