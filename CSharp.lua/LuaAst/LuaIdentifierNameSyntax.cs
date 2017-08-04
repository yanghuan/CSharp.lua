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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace CSharpLua.LuaAst {
  public class LuaIdentifierNameSyntax : LuaExpressionSyntax {
    public string ValueText { get; }

    public readonly static LuaIdentifierNameSyntax Empty = new LuaIdentifierNameSyntax("");
    public readonly static LuaIdentifierNameSyntax Placeholder = new LuaIdentifierNameSyntax("_");
    public readonly static LuaIdentifierNameSyntax One = new LuaIdentifierNameSyntax(1);
    public readonly static LuaIdentifierNameSyntax System = new LuaIdentifierNameSyntax("System");
    public readonly static LuaIdentifierNameSyntax Namespace = new LuaIdentifierNameSyntax("namespace");
    public readonly static LuaIdentifierNameSyntax Class = new LuaIdentifierNameSyntax("class");
    public readonly static LuaIdentifierNameSyntax Struct = new LuaIdentifierNameSyntax("struct");
    public readonly static LuaIdentifierNameSyntax Interface = new LuaIdentifierNameSyntax("interface");
    public readonly static LuaIdentifierNameSyntax Enum = new LuaIdentifierNameSyntax("enum");
    public readonly static LuaIdentifierNameSyntax Value = new LuaIdentifierNameSyntax("value");
    public readonly static LuaIdentifierNameSyntax This = new LuaIdentifierNameSyntax("this");
    public readonly static LuaIdentifierNameSyntax True = new LuaIdentifierNameSyntax("true");
    public readonly static LuaIdentifierNameSyntax False = new LuaIdentifierNameSyntax("false");
    public readonly static LuaIdentifierNameSyntax Throw = new LuaIdentifierNameSyntax("System.throw");
    public readonly static LuaIdentifierNameSyntax Each = new LuaIdentifierNameSyntax("System.each");
    public readonly static LuaIdentifierNameSyntax YieldReturn = new LuaIdentifierNameSyntax("System.yieldReturn");
    public readonly static LuaIdentifierNameSyntax Object = new LuaIdentifierNameSyntax("System.Object");
    public readonly static LuaIdentifierNameSyntax Array = new LuaIdentifierNameSyntax("System.Array");
    public readonly static LuaIdentifierNameSyntax ArrayEmpty = new LuaIdentifierNameSyntax("System.Array.Empty");
    public readonly static LuaIdentifierNameSyntax MultiArray = new LuaIdentifierNameSyntax("System.MultiArray");
    public readonly static LuaIdentifierNameSyntax Create = new LuaIdentifierNameSyntax("System.create");
    public readonly static LuaIdentifierNameSyntax Add = new LuaIdentifierNameSyntax("Add");
    public readonly static LuaIdentifierNameSyntax StaticCtor = new LuaIdentifierNameSyntax("__staticCtor__");
    public readonly static LuaIdentifierNameSyntax Init = new LuaIdentifierNameSyntax("__init__");
    public readonly static LuaIdentifierNameSyntax Ctor = new LuaIdentifierNameSyntax("__ctor__");
    public readonly static LuaIdentifierNameSyntax Inherits = new LuaIdentifierNameSyntax("__inherits__");
    public readonly static LuaIdentifierNameSyntax Default = new LuaIdentifierNameSyntax("__default__");
    public readonly static LuaIdentifierNameSyntax SystemDefault = new LuaIdentifierNameSyntax("System.default");
    public readonly static LuaIdentifierNameSyntax Property = new LuaIdentifierNameSyntax("System.property");
    public readonly static LuaIdentifierNameSyntax Event = new LuaIdentifierNameSyntax("System.event");
    public readonly static LuaIdentifierNameSyntax Nil = new LuaIdentifierNameSyntax("nil");
    public readonly static LuaIdentifierNameSyntax TypeOf = new LuaIdentifierNameSyntax("System.typeof");
    public readonly static LuaIdentifierNameSyntax Continue = new LuaIdentifierNameSyntax("continue");
    public readonly static LuaIdentifierNameSyntax StringChar = new LuaIdentifierNameSyntax("string.char");
    public readonly static LuaIdentifierNameSyntax ToStr = new LuaIdentifierNameSyntax("ToString");
    public readonly static LuaIdentifierNameSyntax SystemToString = new LuaIdentifierNameSyntax("System.toString");
    public readonly static LuaIdentifierNameSyntax ToEnumString = new LuaIdentifierNameSyntax("ToEnumString");
    public readonly static LuaIdentifierNameSyntax DelegateCombine = new LuaIdentifierNameSyntax("System.combine");
    public readonly static LuaIdentifierNameSyntax DelegateRemove = new LuaIdentifierNameSyntax("System.remove");
    public readonly static LuaIdentifierNameSyntax DelegateBind = new LuaIdentifierNameSyntax("System.bind");
    public readonly static LuaIdentifierNameSyntax IntegerDiv = new LuaIdentifierNameSyntax("System.div");
    public readonly static LuaIdentifierNameSyntax IntegerDivOfNull = new LuaIdentifierNameSyntax("System.divOfNull");
    public readonly static LuaIdentifierNameSyntax IntegerMod = new LuaIdentifierNameSyntax("System.mod");
    public readonly static LuaIdentifierNameSyntax IntegerModOfNull = new LuaIdentifierNameSyntax("System.modOfNull");
    public readonly static LuaIdentifierNameSyntax BitNot = new LuaIdentifierNameSyntax("System.bnot");
    public readonly static LuaIdentifierNameSyntax BitNotOfNull = new LuaIdentifierNameSyntax("System.bnotOfNull");
    public readonly static LuaIdentifierNameSyntax BitAnd = new LuaIdentifierNameSyntax("System.band");
    public readonly static LuaIdentifierNameSyntax BitAndOfNull = new LuaIdentifierNameSyntax("System.bandOfNull");
    public readonly static LuaIdentifierNameSyntax BitOr = new LuaIdentifierNameSyntax("System.bor");
    public readonly static LuaIdentifierNameSyntax BitOrOfNull = new LuaIdentifierNameSyntax("System.borOfNull");
    public readonly static LuaIdentifierNameSyntax BitXor = new LuaIdentifierNameSyntax("System.xor");
    public readonly static LuaIdentifierNameSyntax BitXorOfNull = new LuaIdentifierNameSyntax("System.xorOfNull");
    public readonly static LuaIdentifierNameSyntax ShiftRight = new LuaIdentifierNameSyntax("System.sr");
    public readonly static LuaIdentifierNameSyntax ShiftRightOfNull = new LuaIdentifierNameSyntax("System.srOfNull");
    public readonly static LuaIdentifierNameSyntax ShiftLeft = new LuaIdentifierNameSyntax("System.sl");
    public readonly static LuaIdentifierNameSyntax ShiftLeftOfNull = new LuaIdentifierNameSyntax("System.slOfNull");
    public readonly static LuaIdentifierNameSyntax Try = new LuaIdentifierNameSyntax("System.try");
    public readonly static LuaIdentifierNameSyntax Is = new LuaIdentifierNameSyntax("System.is");
    public readonly static LuaIdentifierNameSyntax As = new LuaIdentifierNameSyntax("System.as");
    public readonly static LuaIdentifierNameSyntax Cast = new LuaIdentifierNameSyntax("System.cast");
    public readonly static LuaIdentifierNameSyntax Using = new LuaIdentifierNameSyntax("System.using");
    public readonly static LuaIdentifierNameSyntax UsingX = new LuaIdentifierNameSyntax("System.usingX");
    public readonly static LuaIdentifierNameSyntax Linq = new LuaIdentifierNameSyntax("Linq");
    public readonly static LuaIdentifierNameSyntax SystemLinqEnumerable = new LuaIdentifierNameSyntax("System.Linq.Enumerable");
    public readonly static LuaIdentifierNameSyntax New = new LuaIdentifierNameSyntax("new");
    public readonly static LuaIdentifierNameSyntax Format = new LuaIdentifierNameSyntax("Format");
    public readonly static LuaIdentifierNameSyntax Delegate = new LuaIdentifierNameSyntax("System.Delegate");
    public readonly static LuaIdentifierNameSyntax Int = new LuaIdentifierNameSyntax("System.Int");
    public readonly static LuaIdentifierNameSyntax UsingDeclare = new LuaIdentifierNameSyntax("System.usingDeclare");
    public readonly static LuaIdentifierNameSyntax Global = new LuaIdentifierNameSyntax("global");
    public readonly static LuaIdentifierNameSyntax Attributes = new LuaIdentifierNameSyntax("__attributes__");
    public readonly static LuaIdentifierNameSyntax Trunc = new LuaIdentifierNameSyntax("System.trunc");
    public readonly static LuaIdentifierNameSyntax setmetatable = new LuaIdentifierNameSyntax("setmetatable");
    public readonly static LuaIdentifierNameSyntax getmetatable = new LuaIdentifierNameSyntax("getmetatable");
    public readonly static LuaIdentifierNameSyntax Clone = new LuaIdentifierNameSyntax("__clone__");
    public readonly static LuaIdentifierNameSyntax EqualsObj = new LuaIdentifierNameSyntax("EqualsObj");
    public readonly static LuaIdentifierNameSyntax Obj = new LuaIdentifierNameSyntax("obj");
    public readonly static LuaIdentifierNameSyntax EqualsStatic = new LuaIdentifierNameSyntax("equalsStatic");
    public readonly static LuaIdentifierNameSyntax SystemObjectEqualsStatic = new LuaIdentifierNameSyntax("System.Object.EqualsStatic");
    public readonly static LuaIdentifierNameSyntax DateTime = new LuaIdentifierNameSyntax("System.DateTime");
    public readonly static LuaIdentifierNameSyntax TimeSpan = new LuaIdentifierNameSyntax("System.TimeSpan");
    public readonly static LuaIdentifierNameSyntax AnonymousType = new LuaIdentifierNameSyntax("System.anonymousType");
    public readonly static LuaIdentifierNameSyntax SystemNew = new LuaIdentifierNameSyntax("System.new");
    public readonly static LuaIdentifierNameSyntax StackAlloc = new LuaIdentifierNameSyntax("System.stackalloc");
    public readonly static LuaIdentifierNameSyntax GenericT = new LuaIdentifierNameSyntax("__genericT__");
    public readonly static LuaIdentifierNameSyntax Base = new LuaIdentifierNameSyntax("__base__");

    #region QueryExpression
    public readonly static LuaIdentifierNameSyntax LinqWhere = new LuaIdentifierNameSyntax("Linq.Where");
    public readonly static LuaIdentifierNameSyntax LinqSelect = new LuaIdentifierNameSyntax("Linq.Select");
    public readonly static LuaIdentifierNameSyntax LinqOrderBy = new LuaIdentifierNameSyntax("Linq.OrderBy");
    public readonly static LuaIdentifierNameSyntax LinqOrderByDescending = new LuaIdentifierNameSyntax("Linq.OrderByDescending");
    public readonly static LuaIdentifierNameSyntax LinqThenBy = new LuaIdentifierNameSyntax("Linq.ThenBy");
    public readonly static LuaIdentifierNameSyntax LinqThenByDescending = new LuaIdentifierNameSyntax("Linq.ThenByDescending");
    public readonly static LuaIdentifierNameSyntax LinqGroupBy = new LuaIdentifierNameSyntax("Linq.GroupBy");
    #endregion

    public LuaIdentifierNameSyntax(string valueText) {
      ValueText = valueText;
    }

    public LuaIdentifierNameSyntax(int v) : this(v.ToString()) {
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaPropertyOrEventIdentifierNameSyntax : LuaIdentifierNameSyntax {
    public bool IsGetOrAdd { get; set; }
    public bool IsProperty { get; }
    public LuaIdentifierNameSyntax Name { get; }

    public LuaPropertyOrEventIdentifierNameSyntax(bool isProperty, LuaIdentifierNameSyntax name) : this(isProperty, true, name) {
    }

    public LuaPropertyOrEventIdentifierNameSyntax(bool isProperty, bool isGetOrAdd, LuaIdentifierNameSyntax name) : base(string.Empty) {
      IsProperty = isProperty;
      IsGetOrAdd = isGetOrAdd;
      Name = name;
    }

    public string PrefixToken {
      get {
        if (IsProperty) {
          return IsGetOrAdd ? Tokens.Get : Tokens.Set;
        }
        else {
          return IsGetOrAdd ? Tokens.Add : Tokens.Remove;
        }
      }
    }

    public LuaPropertyOrEventIdentifierNameSyntax GetClone() {
      LuaPropertyOrEventIdentifierNameSyntax clone = new LuaPropertyOrEventIdentifierNameSyntax(IsProperty, Name);
      clone.IsGetOrAdd = IsGetOrAdd;
      return clone;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaSymbolNameSyntax : LuaIdentifierNameSyntax {
    public LuaIdentifierNameSyntax IdentifierName { get; private set; }

    public LuaSymbolNameSyntax(LuaIdentifierNameSyntax identifierName) : base("") {
      IdentifierName = identifierName;
    }

    public void Update(string newName) {
      if (IdentifierName.ValueText != newName) {
        IdentifierName = new LuaIdentifierNameSyntax(newName);
      }
    }

    internal override void Render(LuaRenderer renderer) {
      IdentifierName.Render(renderer);
    }
  }

  public sealed class LuaRefNameSyntax : LuaIdentifierNameSyntax {
    public LuaExpressionSyntax NameExpression { get; }

    public LuaRefNameSyntax(LuaExpressionSyntax nameExpression) : base("") {
      NameExpression = nameExpression ?? throw new ArgumentNullException(nameof(nameExpression));
    }

    internal override void Render(LuaRenderer renderer) {
      NameExpression.Render(renderer);
    }
  }
}
