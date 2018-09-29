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

namespace CSharpLua.LuaAst {
  public class LuaIdentifierNameSyntax : LuaExpressionSyntax {
    public string ValueText { get; }

    public static readonly LuaIdentifierNameSyntax Empty = new LuaIdentifierNameSyntax("");
    public static readonly LuaIdentifierNameSyntax Placeholder = new LuaIdentifierNameSyntax("_");
    public static readonly LuaIdentifierNameSyntax One = new LuaIdentifierNameSyntax(1);
    public static readonly LuaIdentifierNameSyntax System = new LuaIdentifierNameSyntax("System");
    public static readonly LuaIdentifierNameSyntax Namespace = new LuaIdentifierNameSyntax("namespace");
    public static readonly LuaIdentifierNameSyntax Class = new LuaIdentifierNameSyntax("class");
    public static readonly LuaIdentifierNameSyntax Struct = new LuaIdentifierNameSyntax("struct");
    public static readonly LuaIdentifierNameSyntax Interface = new LuaIdentifierNameSyntax("interface");
    public static readonly LuaIdentifierNameSyntax Enum = new LuaIdentifierNameSyntax("enum");
    public static readonly LuaIdentifierNameSyntax Value = new LuaIdentifierNameSyntax("value");
    public static readonly LuaIdentifierNameSyntax This = new LuaIdentifierNameSyntax("this");
    public static readonly LuaIdentifierNameSyntax True = new LuaIdentifierNameSyntax("true");
    public static readonly LuaIdentifierNameSyntax False = new LuaIdentifierNameSyntax("false");
    public static readonly LuaIdentifierNameSyntax Throw = new LuaIdentifierNameSyntax("System.throw");
    public static readonly LuaIdentifierNameSyntax Each = new LuaIdentifierNameSyntax("System.each");
    public static readonly LuaIdentifierNameSyntax YieldReturn = new LuaIdentifierNameSyntax("System.yieldReturn");
    public static readonly LuaIdentifierNameSyntax Object = new LuaIdentifierNameSyntax("System.Object");
    public static readonly LuaIdentifierNameSyntax Array = new LuaIdentifierNameSyntax("System.Array");
    public static readonly LuaIdentifierNameSyntax ArrayEmpty = new LuaIdentifierNameSyntax("System.Array.Empty");
    public static readonly LuaIdentifierNameSyntax MultiArray = new LuaIdentifierNameSyntax("System.MultiArray");
    public static readonly LuaIdentifierNameSyntax Create = new LuaIdentifierNameSyntax("System.create");
    public static readonly LuaIdentifierNameSyntax Add = new LuaIdentifierNameSyntax("Add");
    public static readonly LuaIdentifierNameSyntax StaticCtor = new LuaIdentifierNameSyntax("__staticCtor__");
    public static readonly LuaIdentifierNameSyntax Init = new LuaIdentifierNameSyntax("__init__");
    public static readonly LuaIdentifierNameSyntax Ctor = new LuaIdentifierNameSyntax("__ctor__");
    public static readonly LuaIdentifierNameSyntax Inherits = new LuaIdentifierNameSyntax("__inherits__");
    public static readonly LuaIdentifierNameSyntax Default = new LuaIdentifierNameSyntax("__default__");
    public static readonly LuaIdentifierNameSyntax SystemDefault = new LuaIdentifierNameSyntax("System.default");
    public static readonly LuaIdentifierNameSyntax Property = new LuaIdentifierNameSyntax("System.property");
    public static readonly LuaIdentifierNameSyntax Event = new LuaIdentifierNameSyntax("System.event");
    public static readonly LuaIdentifierNameSyntax Nil = new LuaIdentifierNameSyntax("nil");
    public static readonly LuaIdentifierNameSyntax TypeOf = new LuaIdentifierNameSyntax("System.typeof");
    public static readonly LuaIdentifierNameSyntax Continue = new LuaIdentifierNameSyntax("continue");
    public static readonly LuaIdentifierNameSyntax StringChar = new LuaIdentifierNameSyntax("string.char");
    public static readonly LuaIdentifierNameSyntax ToStr = new LuaIdentifierNameSyntax("ToString");
    public static readonly LuaIdentifierNameSyntax SystemToString = new LuaIdentifierNameSyntax("System.toString");
    public static readonly LuaIdentifierNameSyntax ToEnumString = new LuaIdentifierNameSyntax("ToEnumString");
    public static readonly LuaIdentifierNameSyntax DelegateBind = new LuaIdentifierNameSyntax("System.bind");
    public static readonly LuaIdentifierNameSyntax IntegerDiv = new LuaIdentifierNameSyntax("System.div");
    public static readonly LuaIdentifierNameSyntax IntegerDivOfNull = new LuaIdentifierNameSyntax("System.divOfNull");
    public static readonly LuaIdentifierNameSyntax IntegerMod = new LuaIdentifierNameSyntax("System.mod");
    public static readonly LuaIdentifierNameSyntax IntegerModOfNull = new LuaIdentifierNameSyntax("System.modOfNull");
    public static readonly LuaIdentifierNameSyntax BitNot = new LuaIdentifierNameSyntax("System.bnot");
    public static readonly LuaIdentifierNameSyntax BitNotOfNull = new LuaIdentifierNameSyntax("System.bnotOfNull");
    public static readonly LuaIdentifierNameSyntax BitAnd = new LuaIdentifierNameSyntax("System.band");
    public static readonly LuaIdentifierNameSyntax BitAndOfNull = new LuaIdentifierNameSyntax("System.bandOfNull");
    public static readonly LuaIdentifierNameSyntax BitOr = new LuaIdentifierNameSyntax("System.bor");
    public static readonly LuaIdentifierNameSyntax BitOrOfNull = new LuaIdentifierNameSyntax("System.borOfNull");
    public static readonly LuaIdentifierNameSyntax BitXor = new LuaIdentifierNameSyntax("System.xor");
    public static readonly LuaIdentifierNameSyntax BitXorOfNull = new LuaIdentifierNameSyntax("System.xorOfNull");
    public static readonly LuaIdentifierNameSyntax ShiftRight = new LuaIdentifierNameSyntax("System.sr");
    public static readonly LuaIdentifierNameSyntax ShiftRightOfNull = new LuaIdentifierNameSyntax("System.srOfNull");
    public static readonly LuaIdentifierNameSyntax ShiftLeft = new LuaIdentifierNameSyntax("System.sl");
    public static readonly LuaIdentifierNameSyntax ShiftLeftOfNull = new LuaIdentifierNameSyntax("System.slOfNull");
    public static readonly LuaIdentifierNameSyntax Try = new LuaIdentifierNameSyntax("System.try");
    public static readonly LuaIdentifierNameSyntax Is = new LuaIdentifierNameSyntax("System.is");
    public static readonly LuaIdentifierNameSyntax As = new LuaIdentifierNameSyntax("System.as");
    public static readonly LuaIdentifierNameSyntax Cast = new LuaIdentifierNameSyntax("System.cast");
    public static readonly LuaIdentifierNameSyntax Using = new LuaIdentifierNameSyntax("System.using");
    public static readonly LuaIdentifierNameSyntax UsingX = new LuaIdentifierNameSyntax("System.usingX");
    public static readonly LuaIdentifierNameSyntax Linq = new LuaIdentifierNameSyntax("Linq");
    public static readonly LuaIdentifierNameSyntax SystemLinqEnumerable = new LuaIdentifierNameSyntax("System.Linq.Enumerable");
    public static readonly LuaIdentifierNameSyntax New = new LuaIdentifierNameSyntax("new");
    public static readonly LuaIdentifierNameSyntax Format = new LuaIdentifierNameSyntax("Format");
    public static readonly LuaIdentifierNameSyntax Delegate = new LuaIdentifierNameSyntax("System.Delegate");
    public static readonly LuaIdentifierNameSyntax Int = new LuaIdentifierNameSyntax("System.Int");
    public static readonly LuaIdentifierNameSyntax UsingDeclare = new LuaIdentifierNameSyntax("System.usingDeclare");
    public static readonly LuaIdentifierNameSyntax Global = new LuaIdentifierNameSyntax("global");
    public static readonly LuaIdentifierNameSyntax Attributes = new LuaIdentifierNameSyntax("__attributes__");
    public static readonly LuaIdentifierNameSyntax setmetatable = new LuaIdentifierNameSyntax("setmetatable");
    public static readonly LuaIdentifierNameSyntax getmetatable = new LuaIdentifierNameSyntax("getmetatable");
    public static readonly LuaIdentifierNameSyntax Clone = new LuaIdentifierNameSyntax("__clone__");
    public static readonly LuaIdentifierNameSyntax ValueType = new LuaIdentifierNameSyntax("System.ValueType");
    public static readonly LuaIdentifierNameSyntax DateTime = new LuaIdentifierNameSyntax("System.DateTime");
    public static readonly LuaIdentifierNameSyntax TimeSpan = new LuaIdentifierNameSyntax("System.TimeSpan");
    public static readonly LuaIdentifierNameSyntax AnonymousTypeCreate = new LuaIdentifierNameSyntax("System.anonymousType");
    public static readonly LuaIdentifierNameSyntax AnonymousType = new LuaIdentifierNameSyntax("System.AnonymousType");
    public static readonly LuaIdentifierNameSyntax SystemNew = new LuaIdentifierNameSyntax("System.new");
    public static readonly LuaIdentifierNameSyntax StackAlloc = new LuaIdentifierNameSyntax("System.stackalloc");
    public static readonly LuaIdentifierNameSyntax GenericT = new LuaIdentifierNameSyntax("__genericT__");
    public static readonly LuaIdentifierNameSyntax Base = new LuaIdentifierNameSyntax("base");
    public static readonly LuaIdentifierNameSyntax TupleType = new LuaIdentifierNameSyntax("System.Tuple");
    public static readonly LuaIdentifierNameSyntax ValueTupleType = new LuaIdentifierNameSyntax("System.ValueTuple");
    public static readonly LuaIdentifierNameSyntax ValueTupleTypeCreate = new LuaIdentifierNameSyntax("System.valueTuple");
    public static readonly LuaIdentifierNameSyntax __GC = new LuaIdentifierNameSyntax("__gc");
    public static readonly LuaIdentifierNameSyntax Await = new LuaIdentifierNameSyntax("await");
    public static readonly LuaIdentifierNameSyntax Async = new LuaIdentifierNameSyntax("async");

    #region QueryExpression
    public static readonly LuaIdentifierNameSyntax LinqCast = new LuaIdentifierNameSyntax("Linq.Cast");
    public static readonly LuaIdentifierNameSyntax LinqWhere = new LuaIdentifierNameSyntax("Linq.Where");
    public static readonly LuaIdentifierNameSyntax LinqSelect = new LuaIdentifierNameSyntax("Linq.Select");
    public static readonly LuaIdentifierNameSyntax LinqSelectMany = new LuaIdentifierNameSyntax("Linq.SelectMany");
    public static readonly LuaIdentifierNameSyntax LinqOrderBy = new LuaIdentifierNameSyntax("Linq.OrderBy");
    public static readonly LuaIdentifierNameSyntax LinqOrderByDescending = new LuaIdentifierNameSyntax("Linq.OrderByDescending");
    public static readonly LuaIdentifierNameSyntax LinqThenBy = new LuaIdentifierNameSyntax("Linq.ThenBy");
    public static readonly LuaIdentifierNameSyntax LinqThenByDescending = new LuaIdentifierNameSyntax("Linq.ThenByDescending");
    public static readonly LuaIdentifierNameSyntax LinqGroupBy = new LuaIdentifierNameSyntax("Linq.GroupBy");
    public static readonly LuaIdentifierNameSyntax LinqJoin = new LuaIdentifierNameSyntax("Linq.Join");
    public static readonly LuaIdentifierNameSyntax LinqGroupJoin = new LuaIdentifierNameSyntax("Linq.GroupJoin");
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
        } else {
          return IsGetOrAdd ? Tokens.Add : Tokens.Remove;
        }
      }
    }

    public LuaPropertyOrEventIdentifierNameSyntax GetClone() {
      return new LuaPropertyOrEventIdentifierNameSyntax(IsProperty, Name) { IsGetOrAdd = IsGetOrAdd };
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaExpressionNameSyntax : LuaIdentifierNameSyntax {
    public LuaExpressionSyntax Expression { get; }

    public LuaExpressionNameSyntax(LuaExpressionSyntax expression) : base("") {
      Expression = expression;
    }

    internal override void Render(LuaRenderer renderer) {
      Expression.Render(renderer);
    }
  }

  public sealed class LuaSymbolNameSyntax : LuaIdentifierNameSyntax {
    public LuaIdentifierNameSyntax NameExpression { get; private set; }

    public LuaSymbolNameSyntax(LuaIdentifierNameSyntax identifierName) : base("") {
      NameExpression = identifierName;
    }

    public void Update(string newName) {
      NameExpression = new LuaIdentifierNameSyntax(newName);
    }

    internal override void Render(LuaRenderer renderer) {
      NameExpression.Render(renderer);
    }
  }
}
