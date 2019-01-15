using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TestCases
{
    class DelegateInnerTest {



    static bool testValue;

	    [TestCase]
	    public static void TestRun00() {
	      testValue = false;
	      // 初始化1
	      int a = 1;
	      // 1 + 2 = 3
	      a += testValue ? 1 : 2;
	      if (a != 3) {
	        throw new Exception("error result:" + a.ToString());
	      }
	      // 3 + 1 = 4
	      a += !testValue ? 1 : 2;
	      if (a != 4) {
	        throw new Exception("error result:" + a.ToString());
	      }
	      Console.WriteLine("--------TestRunOK{0}", a);
	    }

        [TestCase]
        public static void TestRun01()
        {
            //var dc = new DelegateContainer();
            //dc.OnAction += () => { Console.WriteLine("----------OnAction"); };
            //dc.OnIntAction += OnIntAction;
            //dc.OnBoolFuncInt += i =>
            //{
            //    Console.WriteLine("--------OnBoolFuncInt{0}", i);
            //    return i > 0;
            //};
            //dc.OnIntFuncInt += OnFuncNameXiaQi;

            //dc.OnAction();
            //dc.OnIntAction(123);
            //Console.WriteLine("result:{0}", dc.OnBoolFuncInt(1234));
            //Console.WriteLine("result 1:{0}", dc.OnIntFuncInt(12345));

            List<int> list = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            var result = from num in list where num < 8 select num;
            Console.WriteLine("result:{0}", result.Count());
        }
      [TestCase]
      public static void TestRun02() {
        Dictionary<int, int> dict = null;
        int outValue;
        if (dict != null && dict.TryGetValue(1, out outValue)) {

        }

      }
    private static int OnFuncNameXiaQi(int arg)
        {
            Console.WriteLine("--------OnFuncNameXiaQi{0}", arg);
            return arg;
        }

        private static void OnIntAction(int obj)
        {
            Console.WriteLine("-------OnIntAction");
        }


    }

    class DelegateContainer
    {
        public Action<int> OnIntAction;
        public Action OnAction;
        public Func<int, int> OnIntFuncInt;
        public Func<int, bool> OnBoolFuncInt;
    }
}
