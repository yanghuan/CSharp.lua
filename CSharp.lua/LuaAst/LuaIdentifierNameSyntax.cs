/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License";
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

    public static readonly LuaIdentifierNameSyntax Empty = "";
    public static readonly LuaIdentifierNameSyntax Placeholder = "_";
    public static readonly LuaIdentifierNameSyntax One = 1.ToString();
    public static readonly LuaIdentifierNameSyntax System = "System";
    public static readonly LuaIdentifierNameSyntax Namespace = "namespace";
    public static readonly LuaIdentifierNameSyntax Class = "class";
    public static readonly LuaIdentifierNameSyntax Struct = "struct";
    public static readonly LuaIdentifierNameSyntax Interface = "interface";
    public static readonly LuaIdentifierNameSyntax Enum = "enum";
    public static readonly LuaIdentifierNameSyntax Value = "value";
    public static readonly LuaIdentifierNameSyntax This = "this";
    public static readonly LuaIdentifierNameSyntax True = "true";
    public static readonly LuaIdentifierNameSyntax False = "false";
    public static readonly LuaIdentifierNameSyntax Throw = "System.throw";
    public static readonly LuaIdentifierNameSyntax Each = "System.each";
    public static readonly LuaIdentifierNameSyntax YieldReturn = "System.yieldReturn";
    public static readonly LuaIdentifierNameSyntax Object = "System.Object";
    public static readonly LuaIdentifierNameSyntax Array = "System.Array";
    public static readonly LuaIdentifierNameSyntax MultiArray = "System.MultiArray";
    public static readonly LuaIdentifierNameSyntax Apply = "System.apply";
    public static readonly LuaIdentifierNameSyntax StaticCtor = "static";
    public static readonly LuaIdentifierNameSyntax Init = "internal";
    public static readonly LuaIdentifierNameSyntax Ctor = "__ctor__";
    public static readonly LuaIdentifierNameSyntax Inherits = "__inherits__";
    public static readonly LuaIdentifierNameSyntax Default = "default";
    public static readonly LuaIdentifierNameSyntax SystemDefault = "System.default";
    public static readonly LuaIdentifierNameSyntax Property = "System.property";
    public static readonly LuaIdentifierNameSyntax Event = "System.event";
    public static readonly LuaIdentifierNameSyntax SystemVoid = "System.Void";
    public static readonly LuaIdentifierNameSyntax Nil = "nil";
    public static readonly LuaIdentifierNameSyntax TypeOf = "System.typeof";
    public static readonly LuaIdentifierNameSyntax Continue = "continue";
    public static readonly LuaIdentifierNameSyntax StringChar = "string.char";
    public static readonly LuaIdentifierNameSyntax ToStr = "ToString";
    public static readonly LuaIdentifierNameSyntax SystemToString = "System.toString";
    public static readonly LuaIdentifierNameSyntax ToEnumString = "ToEnumString";
    public static readonly LuaIdentifierNameSyntax DelegateMake = "System.fn";
    public static readonly LuaIdentifierNameSyntax DelegateBind = "System.bind";
    public static readonly LuaIdentifierNameSyntax DelegateCombine = "System.Delegate.Combine";
    public static readonly LuaIdentifierNameSyntax DelegateRemove = "System.Delegate.Remove";
    public static readonly LuaIdentifierNameSyntax IntegerDiv = "System.div";
    public static readonly LuaIdentifierNameSyntax IntegerDivOfNull = "System.divOfNull";
    public static readonly LuaIdentifierNameSyntax IntegerMod = "System.mod";
    public static readonly LuaIdentifierNameSyntax BitNot = "System.bnot";
    public static readonly LuaIdentifierNameSyntax BitNotOfNull = "System.bnotOfNull";
    public static readonly LuaIdentifierNameSyntax BitAnd = "System.band";
    public static readonly LuaIdentifierNameSyntax BitAndOfNull = "System.bandOfNull";
    public static readonly LuaIdentifierNameSyntax BitOr = "System.bor";
    public static readonly LuaIdentifierNameSyntax BitOrOfNull = "System.borOfNull";
    public static readonly LuaIdentifierNameSyntax BitXor = "System.xor";
    public static readonly LuaIdentifierNameSyntax BitXorOfNull = "System.xorOfNull";
    public static readonly LuaIdentifierNameSyntax ShiftRight = "System.sr";
    public static readonly LuaIdentifierNameSyntax ShiftRightOfNull = "System.srOfNull";
    public static readonly LuaIdentifierNameSyntax ShiftLeft = "System.sl";
    public static readonly LuaIdentifierNameSyntax ShiftLeftOfNull = "System.slOfNull";
    public static readonly LuaIdentifierNameSyntax BoolOrOfNull = "System.orOfNull";
    public static readonly LuaIdentifierNameSyntax BoolAndOfNull = "System.andOfNull";
    public static readonly LuaIdentifierNameSyntax BoolXorOfNull = "System.xorOfBoolNull";
    public static readonly LuaIdentifierNameSyntax Try = "System.try";
    public static readonly LuaIdentifierNameSyntax CatchFilter = "System.when";
    public static readonly LuaIdentifierNameSyntax Is = "System.is";
    public static readonly LuaIdentifierNameSyntax As = "System.as";
    public static readonly LuaIdentifierNameSyntax Cast = "System.cast";
    public static readonly LuaIdentifierNameSyntax Using = "System.using";
    public static readonly LuaIdentifierNameSyntax UsingX = "System.usingX";
    public static readonly LuaIdentifierNameSyntax Linq = "Linq";
    public static readonly LuaIdentifierNameSyntax SystemLinqEnumerable = "System.Linq.Enumerable";
    public static readonly LuaIdentifierNameSyntax Format = "Format";
    public static readonly LuaIdentifierNameSyntax Delegate = "System.Delegate";
    public static readonly LuaIdentifierNameSyntax Import = "System.import";
    public static readonly LuaIdentifierNameSyntax Global = "out";
    public static readonly LuaIdentifierNameSyntax Metadata = "__metadata__";
    public static readonly LuaIdentifierNameSyntax Attributes = "attributes";
    public static readonly LuaIdentifierNameSyntax Fields = "fields";
    public static readonly LuaIdentifierNameSyntax Properties = "properties";
    public static readonly LuaIdentifierNameSyntax Events = "events";
    public static readonly LuaIdentifierNameSyntax Methods = "methods";
    public static readonly LuaIdentifierNameSyntax setmetatable = "setmetatable";
    public static readonly LuaIdentifierNameSyntax Clone = "__clone__";
    public static readonly LuaIdentifierNameSyntax NullableClone = "System.Nullable.clone";
    public static readonly LuaIdentifierNameSyntax CopyThis = "__copy__";
    public static readonly LuaIdentifierNameSyntax ValueType = "System.ValueType";
    public static readonly LuaIdentifierNameSyntax DateTime = "System.DateTime";
    public static readonly LuaIdentifierNameSyntax TimeSpan = "System.TimeSpan";
    public static readonly LuaIdentifierNameSyntax AnonymousTypeCreate = "System.anonymousType";
    public static readonly LuaIdentifierNameSyntax AnonymousType = "System.AnonymousType";
    public static readonly LuaIdentifierNameSyntax New = "new";
    public static readonly LuaIdentifierNameSyntax SystemNew = "System.new";
    public static readonly LuaIdentifierNameSyntax StackAlloc = "System.stackalloc";
    public static readonly LuaIdentifierNameSyntax GenericT = "__genericT__";
    public static readonly LuaIdentifierNameSyntax Base = "base";
    public static readonly LuaIdentifierNameSyntax SystemBase = "System.base";
    public static readonly LuaIdentifierNameSyntax TupleType = "System.Tuple";
    public static readonly LuaIdentifierNameSyntax ValueTupleType = "System.ValueTuple";
    public static readonly LuaIdentifierNameSyntax ValueTupleTypeCreate = "System.valueTuple";
    public static readonly LuaIdentifierNameSyntax Deconstruct = "Deconstruct";
    public static readonly LuaIdentifierNameSyntax KeyValuePair = "System.KeyValuePair";
    public static readonly LuaIdentifierNameSyntax NullableType = "System.Nullable";
    public static readonly LuaIdentifierNameSyntax __GC = "__gc";
    public static readonly LuaIdentifierNameSyntax __ToString = "__tostring";
    public static readonly LuaIdentifierNameSyntax Await = "await";
    public static readonly LuaIdentifierNameSyntax Async = "async";
    public static readonly LuaIdentifierNameSyntax MorenManyLocalVarTempTable = "const";

    #region QueryExpression
    public static readonly LuaIdentifierNameSyntax LinqCast = "Linq.Cast";
    public static readonly LuaIdentifierNameSyntax LinqWhere = "Linq.Where";
    public static readonly LuaIdentifierNameSyntax LinqSelect = "Linq.Select";
    public static readonly LuaIdentifierNameSyntax LinqSelectMany = "Linq.SelectMany";
    public static readonly LuaIdentifierNameSyntax LinqOrderBy = "Linq.OrderBy";
    public static readonly LuaIdentifierNameSyntax LinqOrderByDescending = "Linq.OrderByDescending";
    public static readonly LuaIdentifierNameSyntax LinqThenBy = "Linq.ThenBy";
    public static readonly LuaIdentifierNameSyntax LinqThenByDescending = "Linq.ThenByDescending";
    public static readonly LuaIdentifierNameSyntax LinqGroupBy = "Linq.GroupBy";
    public static readonly LuaIdentifierNameSyntax LinqJoin = "Linq.Join";
    public static readonly LuaIdentifierNameSyntax LinqGroupJoin = "Linq.GroupJoin";
    #endregion

    protected LuaIdentifierNameSyntax(string valueText) {
      ValueText = valueText;
    }

    public static implicit operator LuaIdentifierNameSyntax(string valueText) {
      return new LuaIdentifierNameSyntax(valueText);
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

  public sealed class LuaSymbolNameSyntax : LuaIdentifierNameSyntax {
    public LuaExpressionSyntax NameExpression { get; private set; }

    public LuaSymbolNameSyntax(LuaExpressionSyntax identifierName) : base("") {
      NameExpression = identifierName;
    }

    public void Update(string newName) {
      NameExpression = newName;
    }

    internal override void Render(LuaRenderer renderer) {
      NameExpression.Render(renderer);
    }
  }
}
