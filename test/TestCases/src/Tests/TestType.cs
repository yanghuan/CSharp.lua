
using System;
using System.Collections;
using System.Collections.Generic;
namespace TestCases {
  class MyTestType1 {
    public void TestMethod1() {

    }
    public void TestMethod2() {

    }
  }

  public class TestType {
    [TestCase]
    public static void Test() {

      var a = new object();
      var b = new object();

      if(a != b) {
        Console.WriteLine("ok");
      }

      var t1 = typeof(MyTestType1);
      var method1 = t1.GetMethod("TestMethod1");
      var method2 = t1.GetMethod("TestMethod2");
      if(method1 == method2) {
        Console.WriteLine("error");
      }
      else
      {
        Console.WriteLine("ok");
      }
    }
  };

}
