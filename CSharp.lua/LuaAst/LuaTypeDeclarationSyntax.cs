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
        private LuaFunctionExpressSyntax initFunction_;
        private List<LuaFunctionExpressSyntax> ctors_ = new List<LuaFunctionExpressSyntax>();

        public LuaTypeDeclarationSyntax() {
            Add(local_);
            Add(methodList_);
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

        private void AddInitFiled(ref LuaFunctionExpressSyntax initFunction, LuaIdentifierNameSyntax name, LuaExpressionSyntax value) {
            var thisIdentifier = LuaIdentifierNameSyntax.This;
            if(initFunction == null) {
                initFunction = new LuaFunctionExpressSyntax();
                initFunction.ParameterList.Parameters.Add(new LuaParameterSyntax(thisIdentifier));
            }
            LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(thisIdentifier, name);
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(memberAccess, value);
            initFunction.Body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
        }

        public void AddField(LuaIdentifierNameSyntax name, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate, bool isReadOnly) {
            if(isStatic) {
                if(isPrivate) {
                    local_.Variables.Add(name);
                    if(isImmutable) {
                        LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, value);
                        methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                    }
                    else {
                        AddInitFiled(ref staticInitFunction_, name, value);
                    }
                }
                else {
                    //TODO 字段在静态构造函数中赋值操作
                    if(isReadOnly) {
                        local_.Variables.Add(name);
                        if(isImmutable) {
                            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, value);
                            methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                            AddResultTable(name);
                        }
                        else {
                            AddInitFiled(ref staticInitFunction_, name, value);
                        }
                    }
                    else {
                        if(isImmutable) {
                            AddResultTable(name, value);
                        }
                        else {
                            AddInitFiled(ref staticInitFunction_, name, value);
                        }
                    }
                }
            }
            else {
                if(isImmutable) {
                    AddResultTable(name, value);
                }
                else {
                    AddInitFiled(ref initFunction_, name, value);
                }
            }
        }

        public void SetStaticCtor(LuaFunctionExpressSyntax function) {
            Contract.Assert(staticCtorFunction_ == null);
            staticCtorFunction_ = function;
        }

        public void AddCtor(LuaFunctionExpressSyntax function) {
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

        private void AddStaticCtorFunction() {
            bool hasStaticInit = staticInitFunction_ != null;
            bool hasStaticCtor = staticCtorFunction_ != null;

            if(hasStaticCtor) {
                if(hasStaticInit) {
                    staticCtorFunction_.Body.Statements.InsertRange(0, staticInitFunction_.Body.Statements);
                }
                AddInitFunction(LuaIdentifierNameSyntax.StaticCtor, staticCtorFunction_);
            }
            else {
                if(hasStaticInit) {
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
                        LuaInvocationExpressionSyntax invocationInit = new LuaInvocationExpressionSyntax(initIdentifier);
                        invocationInit.ArgumentList.Arguments.Add(new LuaArgumentSyntax(LuaIdentifierNameSyntax.This));
                        ctor.Body.Statements.Insert(0, new LuaExpressionStatementSyntax(invocationInit));
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

        internal override void Render(LuaRenderer renderer) {
            AddStaticCtorFunction();
            AddCtorsFunction();

            LuaReturnStatementSyntax returnStatement = new LuaReturnStatementSyntax();
            returnStatement.Expressions.Add(resultTable_);
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
