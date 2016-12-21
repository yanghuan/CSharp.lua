using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public abstract class LuaTypeDeclarationSyntax : LuaWrapFunctionStatementSynatx {
        private LuaLocalVariablesStatementSyntax local_ = new LuaLocalVariablesStatementSyntax();
        private LuaStatementListSyntax methodList_ = new LuaStatementListSyntax();

        private LuaTableInitializerExpression resultTable_ = new LuaTableInitializerExpression();
        private LuaFunctionExpressSyntax staticInitFunction_;
        private LuaFunctionExpressSyntax staticCtorFunction_;
        private List<string> staticAssignmentNames_ = new List<string>();

        private LuaFunctionExpressSyntax initFunction_;
        private List<LuaConstructorAdapterExpressSyntax> ctors_ = new List<LuaConstructorAdapterExpressSyntax>();

        public LuaTypeDeclarationSyntax() {
            Add(local_);
            Add(methodList_);
        }
        
        public void AddStaticReadOnlyAssignmentName(string name) {
            if(!staticAssignmentNames_.Contains(name)) {
                staticAssignmentNames_.Add(name);
            }
        }

        private void AddResultTable(LuaIdentifierNameSyntax name) {
            LuaKeyValueTableItemSyntax item = new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), name);
            resultTable_.Items.Add(item);
        }

        private void AddResultTable(LuaIdentifierNameSyntax name, LuaExpressionSyntax value) {
            LuaKeyValueTableItemSyntax item = new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), value);
            resultTable_.Items.Add(item);
        }

        public void AddMethod(LuaIdentifierNameSyntax name, LuaFunctionExpressSyntax method, bool isPrivate) {
            local_.Variables.Add(name);
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, method);
            methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
            if(!isPrivate) {
                AddResultTable(name);
            }
        }

        private void AddInitFiled(ref LuaFunctionExpressSyntax initFunction, LuaAssignmentExpressionSyntax assignment) {
            if(initFunction == null) {
                initFunction = new LuaFunctionExpressSyntax();
                initFunction.AddParameter(LuaIdentifierNameSyntax.This);
            }
            initFunction.Body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
        }

        private void AddInitFiled(ref LuaFunctionExpressSyntax initFunction, LuaIdentifierNameSyntax name, LuaExpressionSyntax value) {
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, name);
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(memberAccess, value);
            AddInitFiled(ref initFunction, assignment);
        }

        public void AddField(LuaIdentifierNameSyntax name, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate, bool isReadOnly) {
            if(isStatic) {
                if(isPrivate) {
                    local_.Variables.Add(name);
                    if(value != null) {
                        LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, value);
                        if(isImmutable) {
                            methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                        }
                        else {
                            AddInitFiled(ref staticInitFunction_, assignment);
                        }
                    }
                }
                else {
                    if(isReadOnly) {
                        local_.Variables.Add(name);
                        if(value != null) {
                            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, value);
                            if(isImmutable) {
                                methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                                AddResultTable(name);
                            }
                            else {
                                AddInitFiled(ref staticInitFunction_, assignment);
                                staticAssignmentNames_.Add(name.ValueText);
                            }
                        }
                    }
                    else {
                        if(value != null) {
                            if(isImmutable) {
                                AddResultTable(name, value);
                            }
                            else {
                                AddInitFiled(ref staticInitFunction_, name, value);
                            }
                        }
                    }
                }
            }
            else {
                if(value != null) {
                    if(isImmutable) {
                        AddResultTable(name, value);
                    }
                    else {
                        AddInitFiled(ref initFunction_, name, value);
                    }
                }
            }
        }

        public void AddProperty(string name, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate) {
            LuaIdentifierNameSyntax identifierName = new LuaIdentifierNameSyntax(name);
            LuaIdentifierNameSyntax get = new LuaIdentifierNameSyntax(LuaSyntaxNode.Tokens.Get + name);
            LuaIdentifierNameSyntax set = new LuaIdentifierNameSyntax(LuaSyntaxNode.Tokens.Set + name);
            local_.Variables.Add(get);
            local_.Variables.Add(set);
            LuaMultipleAssignmentExpressionSyntax assignment = new LuaMultipleAssignmentExpressionSyntax();
            assignment.Lefts.Add(get);
            assignment.Lefts.Add(set);
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(LuaIdentifierNameSyntax.Property);
            invocation.AddArgument(new LuaStringLiteralExpressionSyntax(identifierName));
            assignment.Rights.Add(invocation);
            Add(new LuaExpressionStatementSyntax(assignment));

            if(value != null) {
                if(isStatic) {
                    if(isImmutable) {
                        AddResultTable(identifierName, value);
                    }
                    else {
                        AddInitFiled(ref staticInitFunction_, identifierName, value);
                    }
                }
                else {
                    if(isImmutable) {
                        AddResultTable(identifierName, value);
                    }
                    else {
                        AddInitFiled(ref initFunction_, identifierName, value);
                    }
                }
            }

            if(!isPrivate) {
                AddResultTable(get);
                AddResultTable(set);
            }
        }

        public void SetStaticCtor(LuaConstructorAdapterExpressSyntax function) {
            Contract.Assert(staticCtorFunction_ == null);
            staticCtorFunction_ = function;
        }

        public void AddCtor(LuaConstructorAdapterExpressSyntax function) {
            ctors_.Add(function);
        }

        private void AddInitFunction(LuaIdentifierNameSyntax name, LuaFunctionExpressSyntax initFunction, bool isAddItem = true) {
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, initFunction);
            Add(new LuaExpressionStatementSyntax(assignment));
            local_.Variables.Add(name);
            if(isAddItem) {
                AddResultTable(name);
            }
        }

        private void AddStaticAssignmentNames(LuaBlockSyntax body) {
            if(staticAssignmentNames_.Count > 0) {
                LuaMultipleAssignmentExpressionSyntax assignment = new LuaMultipleAssignmentExpressionSyntax();
                foreach(string name in staticAssignmentNames_) {
                    LuaIdentifierNameSyntax identifierName = new LuaIdentifierNameSyntax(name);
                    LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, identifierName);
                    assignment.Lefts.Add(memberAccess);
                    assignment.Rights.Add(identifierName);
                }
                body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
            }
        }

        private void AddStaticCtorFunction() {
            bool hasStaticInit = staticInitFunction_ != null;
            bool hasStaticCtor = staticCtorFunction_ != null;

            if(hasStaticCtor) {
                if(hasStaticInit) {
                    staticCtorFunction_.Body.Statements.InsertRange(0, staticInitFunction_.Body.Statements);
                }
                AddStaticAssignmentNames(staticCtorFunction_.Body);
                AddInitFunction(LuaIdentifierNameSyntax.StaticCtor, staticCtorFunction_);
            }
            else {
                if(hasStaticInit) {
                    AddStaticAssignmentNames(staticInitFunction_.Body);
                    AddInitFunction(LuaIdentifierNameSyntax.StaticCtor, staticInitFunction_);
                }
            }
        }

        private void AddCtorsFunction() {
            bool hasInit = initFunction_ != null;
            bool hasCtors = ctors_.Count > 0;

            if(hasCtors) {
                if(hasInit) {
                    var initIdentifier = LuaIdentifierNameSyntax.Init;
                    AddInitFunction(initIdentifier, initFunction_, false);
                    foreach(var ctor in ctors_) {
                        if(!ctor.IsInvokeThisCtor) {
                            LuaInvocationExpressionSyntax invocationInit = new LuaInvocationExpressionSyntax(initIdentifier);
                            invocationInit.AddArgument(LuaIdentifierNameSyntax.This);
                            ctor.Body.Statements.Insert(0, new LuaExpressionStatementSyntax(invocationInit));
                        }
                    }
                }

                if(ctors_.Count == 1) {
                    AddInitFunction(LuaIdentifierNameSyntax.Ctor, ctors_.First());
                }
                else {
                    LuaTableInitializerExpression ctrosTable = new LuaTableInitializerExpression();
                    int index = 1;
                    foreach(var ctor in ctors_) {
                        string name = SpecailWord(Tokens.Ctor + index);
                        LuaIdentifierNameSyntax nameIdentifier = new LuaIdentifierNameSyntax(name);
                        AddInitFunction(nameIdentifier, ctor, false);
                        ctrosTable.Items.Add(new LuaSingleTableItemSyntax(nameIdentifier));
                        ++index;
                    }
                    AddResultTable(LuaIdentifierNameSyntax.Ctor, ctrosTable);
                }
            }
            else {
                if(hasInit) {
                    AddInitFunction(LuaIdentifierNameSyntax.Ctor, initFunction_);
                }
            }
        }

        public void AddBaseTypes(List<LuaIdentifierNameSyntax> baseTypes) {
            LuaTableInitializerExpression table = new LuaTableInitializerExpression();
            table.Items.AddRange(baseTypes.Select(i => new LuaSingleTableItemSyntax(i)));
            AddResultTable(LuaIdentifierNameSyntax.Inherits, table);
        }

        internal override void Render(LuaRenderer renderer) {
            AddStaticCtorFunction();
            AddCtorsFunction();

            LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(resultTable_);
            Add(returnStatement);
            base.Render(renderer);
        }
    }

    public sealed class LuaClassDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaClassDeclarationSyntax(LuaIdentifierNameSyntax name) {
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Class, LuaIdentifierNameSyntax.Namespace);
        }
    }

    public sealed class LuaStructDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaStructDeclarationSyntax(LuaIdentifierNameSyntax name) {
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Struct, LuaIdentifierNameSyntax.Namespace);
        }
    }

    public sealed class LuaInterfaceDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaInterfaceDeclarationSyntax(LuaIdentifierNameSyntax name) {
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Interface);
        }
    }

    public sealed class LuaEnumDeclarationSyntax : LuaTypeDeclarationSyntax {
        public LuaEnumDeclarationSyntax(LuaIdentifierNameSyntax name) {
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Enum);
        }
    }
}
