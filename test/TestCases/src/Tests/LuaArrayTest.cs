using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCases {
  public class LuaArrayTest {
    public static void LuaArrayTest01() {
      var arr = new LuaArray<int>();
      arr.Add(123);
      var indexValue = arr[123];
      arr[456] = indexValue;
    }
    public static void LuaObjectTest01() {
      var obj = new LuaObject();
      LuaObject obj0 = new LuaObject();
      LuaObject obj1 = LuaArrayExternions.CreateLuaObject();
      LuaObject obj2 = LuaArrayExternions.CreateLuaObjectWithMetaXml();
    }

    public static void LuaObjectTest02() {
      // 不带MetaXml调用隐式转换函数
      int obj2 = LuaArrayExternions.CreateLuaObject();
      // 带MetaXml的调用隐式转换函数
      int obj3 = LuaArrayExternions.CreateLuaObjectWithMetaXml();
    }

  }
}
