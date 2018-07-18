/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

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
  public sealed class LuaSpeaicalGenericType {
    public LuaIdentifierNameSyntax Name;
    public LuaExpressionSyntax Value;
    public bool IsLazy;
  }

  public abstract class LuaTypeDeclarationSyntax : LuaWrapFunctionStatementSynatx {
    public bool IsPartialMark { get; set; }
    public bool IsClassUsed { get; set; }
    private LuaLocalAreaSyntax local_ = new LuaLocalAreaSyntax();
    private LuaStatementListSyntax methodList_ = new LuaStatementListSyntax();
    protected LuaTableInitializerExpression resultTable_ = new LuaTableInitializerExpression();

    private List<LuaStatementSyntax> staticInitStatements_ = new List<LuaStatementSyntax>();
    private List<LuaStatementSyntax> staticcCtorStatements_ = new List<LuaStatementSyntax>();
    private List<LuaIdentifierNameSyntax> staticAssignmentNames_ = new List<LuaIdentifierNameSyntax>();

    private List<LuaStatementSyntax> initStatements_ = new List<LuaStatementSyntax>();
    private List<LuaConstructorAdapterExpressionSyntax> ctors_ = new List<LuaConstructorAdapterExpressionSyntax>();

    private List<LuaParameterSyntax> typeParameters_ = new List<LuaParameterSyntax>();
    private LuaTableInitializerExpression attributes_ = new LuaTableInitializerExpression();
    private LuaDocumentStatement document_ = new LuaDocumentStatement();
    public bool IsIgnoreExport => document_.HasIgnoreAttribute;

    public LuaTypeDeclarationSyntax() {
    }

    internal void AddStaticReadOnlyAssignmentName(LuaIdentifierNameSyntax name) {
      if (!staticAssignmentNames_.Contains(name)) {
        staticAssignmentNames_.Add(name);
      }
    }

    internal void AddDocument(LuaDocumentStatement document) {
      if (document != null) {
        document_.Add(document);
      }
    }

    internal void AddClassAttributes(List<LuaExpressionSyntax> attributes) {
      AddFieldAttributes(LuaIdentifierNameSyntax.Class, attributes);
    }

    internal void AddMethodAttributes(LuaIdentifierNameSyntax name, List<LuaExpressionSyntax> attributes) {
      AddFieldAttributes(name, attributes);
    }

    internal void AddFieldAttributes(LuaIdentifierNameSyntax name, List<LuaExpressionSyntax> attributes) {
      if (attributes.Count > 0) {
        LuaTableInitializerExpression table = new LuaTableInitializerExpression();
        table.Items.AddRange(attributes.Select(i => new LuaSingleTableItemSyntax(i)));
        LuaTableLiteralKeySyntax key = new LuaTableLiteralKeySyntax(name);
        LuaKeyValueTableItemSyntax item = new LuaKeyValueTableItemSyntax(key, table);
        attributes_.Items.Add(item);
      }
    }

    internal void AddTypeParameters(IEnumerable<LuaParameterSyntax> typeParameters) {
      typeParameters_.AddRange(typeParameters);
    }

    internal void AddBaseTypes(IEnumerable<LuaExpressionSyntax> baseTypes, LuaSpeaicalGenericType genericArgument = null) {
      bool hasLazyGenericArgument = false;
      if (genericArgument != null) {
        if (genericArgument.IsLazy) {
          hasLazyGenericArgument = true;
        } else {
          AddResultTable(genericArgument.Name, genericArgument.Value);
        }
      }

      LuaTableInitializerExpression table = new LuaTableInitializerExpression();
      table.Items.AddRange(baseTypes.Select(i => new LuaSingleTableItemSyntax(i)));
      LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
      functionExpression.AddParameter(LuaIdentifierNameSyntax.Global);
      if (hasLazyGenericArgument) {
        functionExpression.AddParameter(LuaIdentifierNameSyntax.This);
        var assignment = new LuaAssignmentExpressionSyntax(new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, genericArgument.Name), genericArgument.Value);
        functionExpression.AddStatement(assignment);
      }
      functionExpression.AddStatement(new LuaReturnStatementSyntax(table));
      AddResultTable(LuaIdentifierNameSyntax.Inherits, functionExpression);
    }

    private void AddResultTable(LuaIdentifierNameSyntax name) {
      LuaKeyValueTableItemSyntax item = new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), name);
      resultTable_.Items.Add(item);
    }

    private void AddResultTable(LuaIdentifierNameSyntax name, LuaExpressionSyntax value) {
      LuaKeyValueTableItemSyntax item = new LuaKeyValueTableItemSyntax(new LuaTableLiteralKeySyntax(name), value);
      resultTable_.Items.Add(item);
    }

    public void AddMethod(LuaIdentifierNameSyntax name, LuaFunctionExpressionSyntax method, bool isPrivate, LuaDocumentStatement document = null) {
      bool isIgnore = document != null && document.HasIgnoreAttribute;
      if (isIgnore) {
        return;
      }

      local_.Variables.Add(name);
      LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, method);
      if (document != null && !document.IsEmpty) {
        methodList_.Statements.Add(document);
      }
      methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
      if (!isPrivate) {
        AddResultTable(name);
      }
    }

    private void AddInitFiled(LuaIdentifierNameSyntax name, LuaExpressionSyntax value) {
      LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, name);
      LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(memberAccess, value);
      initStatements_.Add(new LuaExpressionStatementSyntax(assignment));
    }

    public void AddField(LuaIdentifierNameSyntax name, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate, bool isReadOnly) {
      if (isStatic) {
        if (isPrivate) {
          local_.Variables.Add(name);
          if (value != null) {
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, value);
            if (isImmutable) {
              methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
            } else {
              staticInitStatements_.Add(new LuaExpressionStatementSyntax(assignment));
            }
          }
        } else {
          if (isReadOnly) {
            local_.Variables.Add(name);
            if (value != null) {
              if (isImmutable) {
                LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, value);
                methodList_.Statements.Add(new LuaExpressionStatementSyntax(assignment));
                AddResultTable(name);
              } else {
                LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(name, value);
                staticInitStatements_.Add(new LuaExpressionStatementSyntax(assignment));
                staticAssignmentNames_.Add(name);
              }
            }
          } else {
            if (value != null) {
              if (isImmutable) {
                AddResultTable(name, value);
              } else {
                LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, name), value);
                staticInitStatements_.Add(new LuaExpressionStatementSyntax(assignment));
              }
            }
          }
        }
      } else {
        if (value != null) {
          if (isImmutable) {
            AddResultTable(name, value);
          } else {
            AddInitFiled(name, value);
          }
        }
      }
    }

    private void AddPropertyOrEvent(bool isProperty, LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax innerName, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate, LuaExpressionSyntax typeExpression) {
      LuaPropertyOrEventIdentifierNameSyntax get, set;
      if (isProperty) {
        get = new LuaPropertyOrEventIdentifierNameSyntax(true, true, name);
        set = new LuaPropertyOrEventIdentifierNameSyntax(true, false, name);
      } else {
        get = new LuaPropertyOrEventIdentifierNameSyntax(false, true, name);
        set = new LuaPropertyOrEventIdentifierNameSyntax(false, false, name);
      }

      local_.Variables.Add(get);
      local_.Variables.Add(set);
      if (!isStatic) {
        LuaIdentifierNameSyntax initMethodIdentifier = isProperty ? LuaIdentifierNameSyntax.Property : LuaIdentifierNameSyntax.Event;
        LuaMultipleAssignmentExpressionSyntax assignment = new LuaMultipleAssignmentExpressionSyntax();
        assignment.Lefts.Add(get);
        assignment.Lefts.Add(set);
        LuaInvocationExpressionSyntax invocation = new LuaInvocationExpressionSyntax(initMethodIdentifier);
        invocation.AddArgument(new LuaStringLiteralExpressionSyntax(innerName));
        assignment.Rights.Add(invocation);
        methodList_.Statements.Add(assignment.ToStatement());
      } else {
        var memberAccess = new LuaMemberAccessExpressionSyntax(typeExpression, innerName);
        var getFunction = new LuaFunctionExpressionSyntax();
        var setFunction = new LuaFunctionExpressionSyntax();
        if (isProperty) {
          getFunction.AddStatement(new LuaReturnStatementSyntax(memberAccess));
          setFunction.AddParameter(LuaIdentifierNameSyntax.Value);
          var assignment = new LuaAssignmentExpressionSyntax(memberAccess, LuaIdentifierNameSyntax.Value);
          setFunction.AddStatement(assignment.ToStatement());
        } else {
          getFunction.AddParameter(LuaIdentifierNameSyntax.Value);
          var getAssignment = new LuaAssignmentExpressionSyntax(memberAccess, new LuaBinaryExpressionSyntax(memberAccess, Tokens.Plus, LuaIdentifierNameSyntax.Value));
          getFunction.AddStatement(getAssignment.ToStatement());
          setFunction.AddParameter(LuaIdentifierNameSyntax.Value);
          var setAssignment = new LuaAssignmentExpressionSyntax(memberAccess, new LuaBinaryExpressionSyntax(memberAccess, Tokens.Sub, LuaIdentifierNameSyntax.Value));
          setFunction.AddStatement(setAssignment.ToStatement());
        }
        methodList_.Statements.Add(new LuaAssignmentExpressionSyntax(get, getFunction).ToStatement());
        methodList_.Statements.Add(new LuaAssignmentExpressionSyntax(set, setFunction).ToStatement());
      }

      if (value != null) {
        if (isStatic) {
          if (isImmutable) {
            AddResultTable(innerName, value);
          } else {
            LuaAssignmentExpressionSyntax assignment = new LuaAssignmentExpressionSyntax(new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, innerName), value);
            staticInitStatements_.Add(new LuaExpressionStatementSyntax(assignment));
          }
        } else {
          if (isImmutable) {
            AddResultTable(innerName, value);
          } else {
            AddInitFiled(innerName, value);
          }
        }
      }

      if (!isPrivate) {
        AddResultTable(get);
        AddResultTable(set);
      }
    }

    public void AddProperty(LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax innerName, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate, LuaExpressionSyntax typeExpression) {
      AddPropertyOrEvent(true, name, innerName, value, isImmutable, isStatic, isPrivate, typeExpression);
    }

    public void AddEvent(LuaIdentifierNameSyntax name, LuaIdentifierNameSyntax innerName, LuaExpressionSyntax value, bool isImmutable, bool isStatic, bool isPrivate, LuaExpressionSyntax typeExpression) {
      AddPropertyOrEvent(false, name, innerName, value, isImmutable, isStatic, isPrivate, typeExpression);
    }

    public void SetStaticCtor(LuaConstructorAdapterExpressionSyntax function) {
      Contract.Assert(staticcCtorStatements_.Count == 0);
      staticcCtorStatements_.AddRange(function.Body.Statements);
    }

    public void SetStaticCtorEmpty() {
      Contract.Assert(staticcCtorStatements_.Count == 0);
      staticcCtorStatements_.Add(Empty);
    }

    public bool IsNoneCtros {
      get {
        return ctors_.Count == 0;
      }
    }

    public bool IsInitStatementExists {
      get {
        return initStatements_.Count > 0;
      }
    }

    public void AddCtor(LuaConstructorAdapterExpressionSyntax function, bool isZeroParameters) {
      if (isZeroParameters) {
        ctors_.Insert(0, function);
      } else {
        ctors_.Add(function);
      }
    }

    private void AddInitFunction(LuaBlockSyntax body, LuaIdentifierNameSyntax name, LuaFunctionExpressionSyntax initFunction, bool isAddItem = true) {
      local_.Variables.Add(name);
      body.Statements.Add(new LuaExpressionStatementSyntax(new LuaAssignmentExpressionSyntax(name, initFunction)));
      if (isAddItem) {
        AddResultTable(name);
      }
    }

    private void AddStaticAssignmentNames(LuaBlockSyntax body) {
      if (staticAssignmentNames_.Count > 0) {
        LuaMultipleAssignmentExpressionSyntax assignment = new LuaMultipleAssignmentExpressionSyntax();
        foreach (var identifierName in staticAssignmentNames_) {
          LuaMemberAccessExpressionSyntax memberAccess = new LuaMemberAccessExpressionSyntax(LuaIdentifierNameSyntax.This, identifierName);
          assignment.Lefts.Add(memberAccess);
          assignment.Rights.Add(identifierName);
        }
        body.Statements.Add(new LuaExpressionStatementSyntax(assignment));
      }
    }

    private void CheckStaticCtorFunction(LuaBlockSyntax body) {
      List<LuaStatementSyntax> statements = new List<LuaStatementSyntax>();
      statements.AddRange(staticInitStatements_);
      statements.AddRange(staticcCtorStatements_);
      if (statements.Count > 0) {
        LuaFunctionExpressionSyntax staticCtor = new LuaFunctionExpressionSyntax();
        staticCtor.AddParameter(LuaIdentifierNameSyntax.This);
        staticCtor.Body.Statements.AddRange(statements);
        AddStaticAssignmentNames(staticCtor.Body);
        AddInitFunction(body, LuaIdentifierNameSyntax.StaticCtor, staticCtor);
      }
    }

    private LuaFunctionExpressionSyntax GetInitFunction() {
      LuaFunctionExpressionSyntax initFuntion = new LuaFunctionExpressionSyntax();
      initFuntion.AddParameter(LuaIdentifierNameSyntax.This);
      initFuntion.Body.Statements.AddRange(initStatements_);
      return initFuntion;
    }

    private void CheckCtorsFunction(LuaBlockSyntax body) {
      bool hasInit = initStatements_.Count > 0;
      bool hasCtors = ctors_.Count > 0;

      if (hasCtors) {
        if (hasInit) {
          var initIdentifier = LuaIdentifierNameSyntax.Init;
          AddInitFunction(body, initIdentifier, GetInitFunction(), false);
          foreach (var ctor in ctors_) {
            if (!ctor.IsInvokeThisCtor) {
              LuaInvocationExpressionSyntax invocationInit = new LuaInvocationExpressionSyntax(initIdentifier, LuaIdentifierNameSyntax.This);
              ctor.Body.Statements.Insert(0, new LuaExpressionStatementSyntax(invocationInit));
            }
          }
        }

        if (ctors_.Count == 1) {
          AddInitFunction(body, LuaIdentifierNameSyntax.Ctor, ctors_.First());
        } else {
          LuaTableInitializerExpression ctrosTable = new LuaTableInitializerExpression();
          int index = 1;
          foreach (var ctor in ctors_) {
            string name = SpecailWord(Tokens.Ctor + index);
            LuaIdentifierNameSyntax nameIdentifier = new LuaIdentifierNameSyntax(name);
            AddInitFunction(body, nameIdentifier, ctor, false);
            ctrosTable.Items.Add(new LuaSingleTableItemSyntax(nameIdentifier));
            ++index;
          }
          AddResultTable(LuaIdentifierNameSyntax.Ctor, ctrosTable);
        }
      } else {
        if (hasInit) {
          AddInitFunction(body, LuaIdentifierNameSyntax.Ctor, GetInitFunction());
        }
      }
    }

    private void CheckAttributes() {
      if (attributes_.Items.Count > 0) {
        LuaFunctionExpressionSyntax functionExpression = new LuaFunctionExpressionSyntax();
        functionExpression.AddParameter(LuaIdentifierNameSyntax.Global);
        functionExpression.AddStatement(new LuaReturnStatementSyntax(attributes_));
        AddResultTable(LuaIdentifierNameSyntax.Attributes, functionExpression);
      }
    }

    private void AddAllStatementsTo(LuaBlockSyntax body) {
      body.Statements.Add(local_);
      CheckStaticCtorFunction(body);
      CheckCtorsFunction(body);
      body.Statements.Add(methodList_);
      CheckAttributes();
      if (IsClassUsed) {
        body.Statements.Add(new LuaAssignmentExpressionSyntax(LuaIdentifierNameSyntax.Class, resultTable_).ToStatement());
        body.Statements.Add(new LuaReturnStatementSyntax(LuaIdentifierNameSyntax.Class));
      } else {
        body.Statements.Add(new LuaReturnStatementSyntax(resultTable_));
      }
    }

    internal override void Render(LuaRenderer renderer) {
      if (IsPartialMark) {
        return;
      }

      if (IsIgnoreExport) {
        return;
      }

      if (IsClassUsed) {
        local_.Variables.Add(LuaIdentifierNameSyntax.Class);
      }

      document_.Render(renderer);
      if (typeParameters_.Count > 0) {
        LuaFunctionExpressionSyntax wrapFunction = new LuaFunctionExpressionSyntax();
        foreach (var type in typeParameters_) {
          wrapFunction.AddParameter(type);
        }
        AddAllStatementsTo(wrapFunction.Body);
        Body.Statements.Add(new LuaReturnStatementSyntax(wrapFunction));
      } else {
        AddAllStatementsTo(Body);
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
    public LuaCompilationUnitSyntax CompilationUnit { get; }
    public bool IsExport { get; set; }

    public LuaEnumDeclarationSyntax(string fullName, LuaIdentifierNameSyntax name, LuaCompilationUnitSyntax compilationUnit) {
      FullName = fullName;
      CompilationUnit = compilationUnit;
      UpdateIdentifiers(name, LuaIdentifierNameSyntax.Namespace, LuaIdentifierNameSyntax.Enum);
    }

    public void Add(LuaKeyValueTableItemSyntax statement) {
      resultTable_.Items.Add(statement);
    }

    internal override void Render(LuaRenderer renderer) {
      if (IsExport) {
        base.Render(renderer);
      }
    }
  }
}
