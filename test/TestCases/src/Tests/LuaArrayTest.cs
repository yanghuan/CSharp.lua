using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCases {
  public class LuaArrayTest {
    [TestCase]
    public static void ExternionsMethodTest1() {
      var arr = new CSLua.LuaArray<object>();
      arr.Add(123);
      var indexValue = arr[123];
      arr[456] = indexValue;
    }
  }
}
