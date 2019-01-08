using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCases {

  public class WhileAndOutTest {

    class Test {
      int value = 10;
      public bool TryPeek(out int outValue) {
        outValue = --value;
        return outValue > 0;
      }
    }

    [TestCase]
    public static void WhileAndOutTest1() {
      var instance = new Test();

      int outValue;
      while (instance.TryPeek(out outValue)) {
        Console.WriteLine("Peeked:{0}", outValue);
      }
    }
  }
}
