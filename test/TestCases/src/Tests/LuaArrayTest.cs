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

  }
}
