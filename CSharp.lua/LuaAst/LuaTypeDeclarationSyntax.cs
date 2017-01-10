/*
Copyright 2016 YANG Huan (sy.yanghuan@gmail.com).
Copyright 2016 Redmoon Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua.LuaAst {
    public abstract class LuaTypeDeclarationSyntax : LuaWrapFunctionStatementSynatx {
        private LuaTypeLocalAreaSyntax local_ = new LuaTypeLocalAreaSyntax();
        private LuaStatementListSyntax methodList_ = new LuaStatementListSyntax();

        protected LuaTableInitializerExpression resultTable_ = new LuaTableInitializerExpression();
        private LuaFunctionExpressionSyntax staticInitFunction_;
        private LuaFunctionExpressionSyntax staticCtorFunction_;
        private List<string> staticAssignmentNames_ = new List<string>();

        private LuaFunctionExpressionSyntax initFunction_;
        private List<LuaConstructorAdapterExpressionSyntax> ctors_ = new List<LuaConstructorAdapterExpressionSyntax>();
        private List<LuaIdentifierNameSyntax> typeIdentifiers_ = new List<LuaIdentifierNameSyntax>();
        public bool IsPartialMark { get; set; }

        public LuaTypeDeclarationSyntax() {
        }
        
        public void AddStaticReadOnlyAssignmentName(string name) {
            if(!staticAssignmentNames_.Contains(name)) {
                staticAssignmentNames_.Add(name);
            }
        }

        public void AddTypeIdentifier(LuaIdentifierNameSyntax identifier) {
            typeIdentifiers_.Add(identifier);
        }

        private void AddResultTable(LuaIdentifierNameSyntax name) {
            LuaKeyValueTableItemSyntax item = new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), name);
            resultTable_.Items.Add(item);
        }

        private void AddResultTable(LuaIdentifierNameSyntax name, LuaExpressionSyntax value) {
            LuaKeyValueTableItemSyntax item = new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), value);
            resultTable_.Items.Add(item);
        }

        public void AddMethod(LuaIdentifierNameSyntax name, LuaFunctionExpressionSyntax method, bool isPrivate) {
            local_.Variables.Add(name);
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, method);
            methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
            if(!isPrivate) {
                AddResultTable(name);
            }
        }

        private void AddInitFiled(ref LuaFunctionExpressionSyntax initFunction, LuaAssignmentExpressionSyntax assignment) {
            if(initFunction == null) {
                initFunction = new LuaFunctionExpressionSyntax();
                initFunction.AddParameter(LuaIdentifierNameSyntax.This);
            }
            initFunction.Body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
        }

        private void AddInitFiled(ref LuaFunctionExpressionSyntax initFunction, LuaIdentifierNameSyntax name, LuaExpressionSyntax value) {
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

        private void AddPropertyOrEvent(bool isProperty, string name, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate) {
            string getToken, setToken;
            LuaIdentifierNameSyntax initMethodIdentifier;
            if(isProperty) {
                getToken = LuaSyntaxNode.Tokens.Get;
                setToken = LuaSyntaxNode.Tokens.Set;
                initMethodIdentifier = LuaIdentifierNameSyntax.Property;
            }
            else {
                getToken = LuaSyntaxNode.Tokens.Add;
                setToken = LuaSyntaxNode.Tokens.Remove;
                initMethodIdentifier = LuaIdentifierNameSyntax.Event;
            }

            LuaIdentifierNameSyntax identifierName = new LuaIdentifierNameSyntax(name);
            LuaIdentifierNameSyntax get = new LuaIdentifierNameSyntax(getToken + name);
            LuaIdentifierNameSyntax set = new LuaIdentifierNameSyntax(setToken + name);
            local_.Variables.Add(get);
            local_.Variables.Add(set);
            LuaMultipleAssignmentExpressionSyntax assignment = new LuaMultipleAssignmentExpressionSyntax();
            assignment.Lefts.Add(get);
            assignment.Lefts.Add(set);
            LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(initMethodIdentifier);
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

        public void AddProperty(string name, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate) {
            AddPropertyOrEvent(true, name, value, isImmutable, isStatic, isPrivate);
        }

        public void AddEvent(string name, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate) {
            AddPropertyOrEvent(false, name, value, isImmutable, isStatic, isPrivate);
        }

        public void SetStaticCtor(LuaConstructorAdapterExpressionSyntax function) {
            Contract.Assert(staticCtorFunction_ == null);
            staticCtorFunction_ = function;
        }

        public void AddCtor(LuaConstructorAdapterExpressionSyntax function) {
            ctors_.Add(function);
        }

        private void AddInitFunction(LuaIdentifierNameSyntax name, LuaFunctionExpressionSyntax initFunction, bool isAddItem = true) {
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
                            LuaInvocationExpressionSyntax invocationInit = new LuaInvocationExpressionSyntax(initIdentifier, LuaIdentifierNameSyntax.This);
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

        public void AddBaseTypes(IEnumerable<LuaExpressionSyntax> baseTypes) {
            LuaTableInitializerExpression table = new LuaTableInitializerExpression();
            table.Items.AddRange(baseTypes.Select(i => new LuaSingleTableItemSyntax(i)));
            AddResultTable(LuaIdentifierNameSyntax.Inherits, table);
        }

        internal override void Render(LuaRenderer renderer) {
            if(IsPartialMark) {
                return;
            }

            Add(local_);
            AddStaticCtorFunction();
            AddCtorsFunction();
            Add(methodList_);

            LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax(resultTable_);
            Add(returnStatement);

            if(typeIdentifiers_.Count > 0) {
                LuaFunctionExpressionSyntax wrapFunction = new LuaFunctionExpressionSyntax();
                foreach(var type in typeIdentifiers_) {
                    wrapFunction.AddParameter(type);
                }
                wrapFunction.Body.Statements.AddRange(statements_);
                statements_.Clear();
                statements_.Add(new LuaReturnStatementSyntax(wrapFunction));
            }
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
        public string FullName { get; }

        public LuaEnumDeclarationSyntax(string fullName, LuaIdentifierNameSyntax name) {
            FullName = fullName;
            UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Enum);
        }

        public void Add(LuaKeyValueTableItemSyntax statement) {
            resultTable_.Items.Add(statement);
        }

        internal override void Render(LuaRenderer renderer) {
            if(renderer.IsEnumExport(FullName)) {
                base.Render(renderer);
            }
        }
    }
}
