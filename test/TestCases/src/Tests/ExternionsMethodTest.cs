using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCases {
  public class ExternionsMethodTest {
    [TestCase]
    public static void ExternionsMethodTest1() {
      var instance = new ExternionClass();
      instance.TestMethod(123);
      ExternionClassLinq.TestMethod(instance, 456);
    }


    public static void ExternionsMethodInDllWithTemplateCall() {
      var arr = new LuaObject();
      arr.DOLocalMove(123);
    }
  }

  class ExternionClass {

  }
  static class ExternionClassLinq {
    public static void TestMethod(this ExternionClass self, int value) {

    }
  }
}
