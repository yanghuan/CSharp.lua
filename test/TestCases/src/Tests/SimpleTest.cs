using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCases
{
    class TestStatic
    {
        public static Dictionary<string, int> staticTest = new Dictionary<string, int>();
    }
    public class SimpleTest
    {
        
        static int b;
        int a;
        int c = 2;
        static SimpleTest()
        {
            b = 1;
        }

        public SimpleTest()
            : this(1)
        {
            Action a = () =>
            {
                c = 2;
            };
            a();
        }
        public SimpleTest(int a)
        {
            Action act = () =>
            {
                this.a = a;
            };
            act();
        }

        public void Test(int a, int c)
        {
            this.a = a;
            this.c = c;
        }
        [TestCase]
        public static long bar()
        {
            long b = 0;
            for (int i = 0; i < 10000; i++)
            {
                b += i;
            }

            return b;
        }
        [TestCase]
        public static int foo2()
        {
            int b = 0;
            for (int i = 0; i < 500000; i++)
            {
                b += i;
            }

            return b;
        }
        [TestCase]
        public static int foo3()
        {
            int b = 3;
            int a = 4;
            int c = 5;
            return b + a - c;
        }
        [TestCase]
        public static void StaticTest()
        {
            TestStatic.staticTest.Add("1", 1);
            Console.WriteLine("Value for 1:" + TestStatic.staticTest["1"]);
        }
        [TestCase]
        public static void InstanceTest()
        {
            SimpleTest t = new SimpleTest();
            int a = t.c;
            SimpleTest t2 = new SimpleTest(12);            
            Console.WriteLine(t2.a.ToString());
            t2.Test(5, 4);
            Console.WriteLine(t2.a.ToString());
            Console.WriteLine(t2.c.ToString());
            int b = t2.c;
            if (a == b)
            {
                Console.WriteLine("true");
            }
            if (t != t2)
            {
                Console.Write("true 2");
            }
        }
        [TestCase]
        public static void MultiDimensionalArrayTest()
        {
            int[,] arr = new int[3, 4];
            arr[1, 2] = 3;

            MultiDimensionalArrayTestSub(arr);
        }

        static void MultiDimensionalArrayTestSub(int[,] arr)
        {
            Console.WriteLine("arr = " + arr[1, 2]);
        }

        public class FileCode<T>
        {
            public static readonly bool TypeSupportsPacking = default(T) != null;
        }
        [TestCase]
        public static void GenericDefaultTest()
        {
            var ins = new FileCode<uint>();
            Console.WriteLine(FileCode<uint>.TypeSupportsPacking);
            Console.WriteLine(FileCode<string>.TypeSupportsPacking);
        }
        [TestCase]
        public static void EqualsTest()
        {
            act1 = foo4;
            Action act2 = foo4;
            Console.WriteLine(new Test4().Equals2(act2));//true
            Console.WriteLine(new Test4().Equals(act2));//false
        }
        [TestCase]
        public static void NullableTest()
        {
            int? val = 123;
            if (val.HasValue)
                Console.WriteLine(val.Value.ToString());
        }

        static void foo4() { }
        static Action act1 = null;
        class Test4
        {
            public bool Equals2(object obj)
            {
                return act1.Equals(obj);
            }

            public override bool Equals(object obj)
            {
                return act1.Equals(obj);
            }
        }

    class PropValue<T> {
      public T value;
      public static implicit operator T(PropValue<T> v) {
        return v.value;
      }
    }


    [TestCase]
    public static void ImplicitTest01() {
      var value = new PropValue<bool>() { value = false };
      if(value) {
        throw new Exception("the correct value is false");
      }
    }


    private static float _lastAngle;
    [TestCase]
    public static void ImplicitTest02() {
      for (float tempAngle = _lastAngle, j = 0; j < 10; j++) {
      }
    }


    static int delayCollision;
    [TestCase]
    public static void ImplicitTest03() {
      var value = new PropValue<bool>() { value = false };
      if (value && --delayCollision <= 0) {
        throw new Exception("the correct value is false");
      }
    }

    static bool GetOutValue(out int value) {
      value = 123;
      return true;
    }
    [TestCase]
    public static void ImplicitTest04() {
      var value = new PropValue<bool>() { value = false };
      int outValue;
      if (value && GetOutValue(out outValue)) {
        throw new Exception("the correct value is false");
      }
    }


    static bool GetRefValue(ref int value) {
      value = 123;
      return true;
    }
    [TestCase]
    public static void ImplicitTest05() {
      var value = new PropValue<bool>() { value = false };
      int outValue = 0;
      if (value && GetRefValue(ref outValue)) {
        throw new Exception("the correct value is false");
      }
    }

    [TestCase]
    public static void ImplicitTest06() {
      var value = new PropValue<bool>() { value = false };
      int outValue = 0;
      if (value && --delayCollision <= 0 && GetOutValue(out outValue) && GetRefValue(ref outValue)) {
        throw new Exception("the correct value is false");
      }
    }

    [TestCase]
    public static void ImplicitTest08() {
      var value = new PropValue<bool>() { value = false };
      if (!value)
		{
        
      }
	  else
		{
		  throw new Exception("the correct value is false");
		}
    }
    
    [TestCase]
    public static void ImplicitTest09() {
      var value = new PropValue<bool>() { value = false };
      if (value)
		{
		  throw new Exception("the correct value is false");
      }
    }
	
    
    [TestCase]
    public static void ImplicitTest10() {
      var value = new PropValue<bool>() { value = false };
	  var falseValue = value ? true : false;
      if (falseValue)
		{
		  throw new Exception("the correct value is false");
      }
    }
    
    [TestCase]
    public static void ImplicitTest11() {
      var value = new PropValue<bool>() { value = false };
	  var trueValue = !value ? true : false;
      if (!trueValue)
		{
		  throw new Exception("the correct value is true");
      }
    }

	public class Node
	{
		private Node parent;

		public Node Root
		{
			get
			{
				return parent == null && GetType() == typeof(Node) ? (Node)this : parent.Root;
			}
		}

		private Node owner;
		public Node Owner
		{
			get
			{
				return Root.owner;
			}
		}

	}
	

	public class TestNode
	{
		Node node1;
		Node node2;
		public void Test()
		{
			node1 = node2.Owner;
		}

	}
  }



}
