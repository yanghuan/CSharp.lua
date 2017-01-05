using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpLua.LuaAst;

namespace CSharpLua {
    public sealed partial class LuaSyntaxNodeTransfor {
        private static readonly Regex codeTemplateRegex_ = new Regex(@"(,?\s*)\{(\*?[\w|^]+)\}", RegexOptions.Compiled);
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

            public LocalVarSearcher(string name) {
                name_ = name;
            }

            public override void VisitParameter(ParameterSyntax node) {
                if(node.Identifier.ValueText == name_) {
                    Found();
                }
            }

            public override void VisitVariableDeclarator(VariableDeclaratorSyntax node) {
                if(node.Identifier.ValueText == name_) {
                    Found();
                }
            }
        }

        private bool IsLocalVarExists(string name, MethodDeclarationSyntax root) {
            LocalVarSearcher searcher = new LocalVarSearcher(name);
            return searcher.Find(root);
        }

        private string GetNewIdentifierName(string name, int index) {
            switch(index) {
                case 0:
                    return name;
                case 1:
                    return name + "_";
                case 2:
                    return "_" + name;
                default:
                    return name + (index - 2);
            }
        }

        private SyntaxNode FindParent(SyntaxNode node, Func<SyntaxNode, bool> macth) {
            var parent = node.Parent;
            while(true) {
                if(macth(parent)) {
                    return parent;
                }
                parent = parent.Parent;
            }
        }

        private SyntaxNode FindParent(SyntaxNode node, SyntaxKind kind) {
            return FindParent(node, i => i.IsKind(kind));
        }

        private string GetUniqueIdentifier(string name, SyntaxNode node, int index = 0) {
            var root = (MethodDeclarationSyntax)FindParent(node, SyntaxKind.MethodDeclaration);
            while(true) {
                string newName = GetNewIdentifierName(name, index);
                bool exists = IsLocalVarExists(newName, root);
                if(!exists) {
                    return newName;
                }
                ++index;
            }
        }

        private bool CheckReservedWord(ref string name, SyntaxNode node) {
            if(LuaSyntaxNode.IsReservedWord(name)) {
                name = GetUniqueIdentifier(name, node, 1);
                AddReservedMapping(name, node);
                return true;
            }
            return false;
        }

        private void AddReservedMapping(string name, SyntaxNode node) {
            ISymbol symbol = semanticModel_.GetDeclaredSymbol(node);
            Contract.Assert(symbol != null);
            localReservedNames_.Add(symbol, name);
        }

        private void CheckParameterName(ref LuaParameterSyntax parameter, ParameterSyntax node) {
            string name = parameter.Identifier.ValueText;
            bool isReserved = CheckReservedWord(ref name, node);
            if(isReserved) {
                parameter = new LuaParameterSyntax(new LuaIdentifierNameSyntax(name));
            }
        }

        private void CheckVariableDeclaratorName(ref LuaIdentifierNameSyntax identifierName, VariableDeclaratorSyntax node) {
            string name = identifierName.ValueText;
            bool isReserved = CheckReservedWord(ref name, node);
            if(isReserved) {
                identifierName = new LuaIdentifierNameSyntax(name);
            }
        }

        private void CheckReservedWord(ref string name, ISymbol symbol) {
            if(LuaSyntaxNode.IsReservedWord(name)) {
                name = localReservedNames_[symbol];
            }
        }

        private int GetConstructorIndex(IMethodSymbol constructorSymbol) {
            if(constructorSymbol.IsFromCode()) {
                var typeSymbol = (INamedTypeSymbol)constructorSymbol.ReceiverType;
                if(typeSymbol.Constructors.Length > 1) {
                    int index = typeSymbol.Constructors.IndexOf(constructorSymbol);
                    Contract.Assert(index != -1);
                    int ctroCounter = index + 1;
                    return ctroCounter;
                }
            }
            return 0;
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

        private bool IsReturnExists(SyntaxNode node) {
            ReturnStatementSearcher searcher = new ReturnStatementSearcher();
            return searcher.Find(node);
        }

        private int GetCaseLabelIndex(GotoStatementSyntax node) {
            var switchStatement = (SwitchStatementSyntax)FindParent(node, SyntaxKind.SwitchStatement);
            int index = 0;
            foreach(var section in switchStatement.Sections) {
               bool isFound = section.Labels.Any(i => {
                    if(i.IsKind(SyntaxKind.CaseSwitchLabel)) {
                        var label = (CaseSwitchLabelSyntax)i;
                        if(label.Value.ToString() == node.Expression.ToString()) {
                            return true;
                        }
                    }
                    return false;
                });
                if(isFound) {
                    return index;
                }
            }
            throw new InvalidOperationException();
        }

        private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression) {
            return BuildCodeTemplateExpression(codeTemplate, targetExpression, Array.Empty<ExpressionSyntax>(), ImmutableArray<ITypeSymbol>.Empty);
        }

        private void AddCodeTemplateExpression(LuaExpressionSyntax expression, string comma, LuaCodeTemplateExpressionSyntax codeTemplateExpression) {
            if(!string.IsNullOrEmpty(comma)) {
                codeTemplateExpression.Codes.Add(new LuaIdentifierNameSyntax(comma));
            }
            codeTemplateExpression.Codes.Add(expression);
        }

        private LuaExpressionSyntax BuildCodeTemplateExpression(string codeTemplate, ExpressionSyntax targetExpression, IList<ExpressionSyntax> arguments, ImmutableArray<ITypeSymbol> typeArguments) {
            LuaCodeTemplateExpressionSyntax codeTemplateExpression = new LuaCodeTemplateExpressionSyntax();

            var matchs = codeTemplateRegex_.Matches(codeTemplate);
            int prevIndex = 0;
            foreach(Match match in matchs) {
                if(match.Index > prevIndex) {
                    string prevToken = codeTemplate.Substring(prevIndex, match.Index - prevIndex);
                    codeTemplateExpression.Codes.Add(new LuaIdentifierNameSyntax(prevToken));
                }
                string comma = match.Groups[1].Value;
                string key = match.Groups[2].Value;
                if(key == "this") {
                    AddCodeTemplateExpression(BuildMemberAccessTargetExpression(targetExpression), comma, codeTemplateExpression);
                }
                else if(key == "class") {
                    var type = semanticModel_.GetTypeInfo(targetExpression).Type;
                    string typeName = XmlMetaProvider.GetTypeMapName(type);
                    AddCodeTemplateExpression(new LuaIdentifierNameSyntax(typeName), comma, codeTemplateExpression);
                }
                else if(key[0] == '^') {
                    int typeIndex;
                    if(int.TryParse(key.Substring(1), out typeIndex)) {
                        var typeArgument = typeArguments.GetOrDefault(typeIndex);
                        if(typeArgument != null) {
                            string typeName = XmlMetaProvider.GetTypeMapName(typeArgument);
                            AddCodeTemplateExpression(new LuaIdentifierNameSyntax(typeName), comma, codeTemplateExpression);
                        }
                    }
                }
                else if(key[0] == '*') {
                    int paramsIndex;
                    if(int.TryParse(key.Substring(1), out paramsIndex)) {
                        LuaCodeTemplateParamsExpressionSyntax paramsExpression = new LuaCodeTemplateParamsExpressionSyntax();
                        foreach(var argument in arguments.Skip(paramsIndex)) {
                            var argumentExpression = (LuaExpressionSyntax)argument.Accept(this);
                            paramsExpression.Expressions.Add(argumentExpression);
                        }
                        if(paramsExpression.Expressions.Count > 0) {
                            AddCodeTemplateExpression(paramsExpression, comma, codeTemplateExpression);
                        }
                    }
                }
                else {
                    int argumentIndex;
                    if(int.TryParse(key, out argumentIndex)) {
                        var argument = arguments.GetOrDefault(argumentIndex);
                        if(argument != null) {
                            var argumentExpression = (LuaExpressionSyntax)argument.Accept(this);
                            AddCodeTemplateExpression(argumentExpression, comma, codeTemplateExpression);
                        }
                    }
                }
                prevIndex = match.Index + match.Length;
            }

            if(prevIndex < codeTemplate.Length) {
                string last = codeTemplate.Substring(prevIndex);
                codeTemplateExpression.Codes.Add(new LuaIdentifierNameSyntax(last));
            }

            return codeTemplateExpression;
        }

        private bool IsPropertyField(IPropertySymbol symbol) {
            return symbol.IsPropertyField() || XmlMetaProvider.IsPropertyField(symbol);
        }

        private INamedTypeSymbol GetTypeDeclarationSymbol(SyntaxNode node) {
            var typeDeclaration = (TypeDeclarationSyntax)FindParent(node, i => i.IsKind(SyntaxKind.ClassDeclaration) || i.IsKind(SyntaxKind.StructDeclaration));
            return semanticModel_.GetDeclaredSymbol(typeDeclaration);
        }

        private bool IsInternalMember(SyntaxNode node, ISymbol symbol) {
            bool isVirtual = symbol.IsOverridable() && !symbol.ContainingType.IsSealed;
            if(!isVirtual) {
                var typeSymbol = GetTypeDeclarationSymbol(node);
                if(typeSymbol == symbol.ContainingType) {
                    return true;
                }
            }
            return false;
        }
    }
}
