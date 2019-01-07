using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCases {


  public class BaseClassCtorCall1 {

    [TestCase]
    public static void TestCtor() {
      new GenericTable();

    }

    class BaseTable {
      public List<int> myList;
      // 有默认构造函数
      public BaseTable() {
         myList = new List<int>();
      }
    }
    class GenericTable : BaseTable {
      public GenericTable() {
        if (myList == null) {
          throw new Exception("not call base ctor");
        }
      }

    }
  }

  public class BaseClassCtorCall2 {

    [TestCase]
    public static void TestCtor() {
      new GenericTable();

    }

    class BaseTable {
      public List<int> myList = new List<int>();
      // 空的默认构造函数
      public BaseTable() {
      }
    }
    class GenericTable : BaseTable {
      public GenericTable() {
        if (myList == null) {
          throw new Exception("not call base ctor");
        }
      }

    }
  }



  public class BaseClassCtorCall3 {

    [TestCase]
    public static void TestCtor() {
      new GenericTable();

    }

    class BaseTable {
      // 这里应该被初始化
      public List<int> myList = new List<int>();
      // 隐式构造函数
    }
    class GenericTable : BaseTable {
      public GenericTable() {
        if (myList == null) {
          throw new Exception("not call base ctor");
        }
      }
    }

  }

}
