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

        private sealed class LocalVarSearcher : CSharpSyntaxWalker {
            private string name_;
            private SyntaxNode node_;
            public bool IsExists { get; private set; }

            public LocalVarSearcher(string name, SyntaxNode node) {
                name_ = name;
                node_ = node;
            }

            public override void VisitParameter(ParameterSyntax node) {
                if(node != node_) {
                    if(node.Identifier.ValueText == name_) {
                        IsExists = true;
                    }
                }
            }

            public override void VisitVariableDeclarator(VariableDeclaratorSyntax node) {
                if(node != node_) {
                    if(node.Identifier.ValueText == name_) {
                        IsExists = true;
                    } 
                }
            }

            public static bool Exists(string name, SyntaxNode parent, SyntaxNode node) {
                LocalVarSearcher searcher = new LocalVarSearcher(name, node);
                searcher.Visit(parent);
                return searcher.IsExists;
            }
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
                    bool exists = LocalVarSearcher.Exists(newName, parent, node);
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
    }
}
