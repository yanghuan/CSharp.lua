using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestCases
{
    public class EnumTest
    {
        public enum TestEnum : long
        {
            Enum1,
            Enum2,
            Enum4 = 0x123456789,
        }

        enum TestEnum2 : ulong
        {
            Enum1,
            Enum2,
            Enum3234 = 0x123456789,
        }

        enum TestEnum3 : byte
        {
            Enum1bbb,
            Enum2bbb,
        }

        enum TestEnum4 : ushort
        {
            //零 = 0,
            One = 1,
            //佰 = 100
        }

        enum TestEnumUint : uint
        {
            Zero = 0,
            UOne = 1,
            Max = uint.MaxValue
        }

        enum TestEnumInt:int
        {
            Min  = int.MinValue,
            Max = int.MaxValue
        }

        enum TestEnumSByte : sbyte
        {
            Min = sbyte.MinValue,
            Max = sbyte.MaxValue
        }

        static TestEnum b = TestEnum.Enum2;
        [TestCase]
        public static string Test01()
        {
            TestEnum a = TestEnum.Enum4;
            Console.WriteLine("a=" + a);

            return a.ToString();
        }
        [TestCase]
        public static string Test02()
        {
            TestEnum a = (TestEnum)1;
            Console.WriteLine("a=" + a);
            return a.ToString();
        }
        [TestCase]
        public static bool Test03()
        {
            switch (b)
            {
                case TestEnum.Enum1:
                    return false;
                case TestEnum.Enum2:
                    return true;
                default:
                    return false;
            }
        }
        [TestCase]
        public static string Test04()
        {
            return Test04Sub(TestEnum.Enum4);
        }

        static string Test04Sub(TestEnum a)
        {
            return a.ToString();
        }
        [TestCase]
        public static void Test05()
        {
            TestEnum a = TestEnum.Enum4;
            TestEnum2 b = (TestEnum2)a;
            Console.WriteLine("b=" + b);
            TestEnum3 c = (TestEnum3)EnumTest.b;
            Console.WriteLine("c=" + c);
        }
        [TestCase]
        public static string Test06()
        {
            System.IO.FileMode a = System.IO.FileMode.Create;

            Console.WriteLine("a=" + a);
            return a.ToString();
        }
        [TestCase]
        public static void Test08()
        {
            object o = TestEnum.Enum4;

            Console.WriteLine((TestEnum)o == TestEnum.Enum4);
        }
        [TestCase]
        public static void Test09()
        {
            Dictionary<TestEnum, int> dic = new Dictionary<TestEnum, int>();
            dic[TestEnum.Enum2] = 123;

            int res;
            if (dic.TryGetValue(TestEnum.Enum2, out res))
            {
                Console.WriteLine(res);
            }
        }
        [TestCase]
        public static void Test10()
        {
            object e = TestEnum3.Enum2bbb;
            byte b = (byte)e; //InvalidCastException
            Console.WriteLine(b);
        }
        [TestCase]
        public static void Test11()
        {
            //Enum defined in ILRuntime
            var enumInIL = TestEnum.Enum4;
            var valueDirectly = $"{enumInIL}";
            var valueToString = enumInIL.ToString();

            if (valueDirectly.Equals(valueToString) == false)
            {
                throw new Exception($"Different string value: {valueDirectly} vs. {valueToString}");
            }

            //Enum defined in native code
            var enumInNative = 0x123456789;
            valueDirectly = $"{enumInNative}";
            valueToString = enumInNative.ToString();

            var enumObj = (Object)enumInNative;
            Console.WriteLine(enumObj.GetType().FullName);

            if (valueDirectly.Equals(valueToString) == false)
            {
                throw new Exception($"Different string value: {valueDirectly} vs. {valueToString}");
            }
        }
        [TestCase]
        public static void Test12()
        {
            var arr = Enum.GetValues(typeof(TestEnum));
            foreach (var i in arr)
            {
                Console.WriteLine(i.ToString());
            }
        }
        [TestCase]
        public static void Test13()
        {
            var emmm = TestEnum2.Enum3234;
            Console.WriteLine(emmm.ToString());

            var one = TestEnum4.One;
            Console.WriteLine(one);

            var hundred = TestEnum4.One;
            Console.WriteLine(hundred);
            Console.WriteLine(hundred.ToString());

            var UOne = TestEnumUint.Max;
            Console.WriteLine(UOne);

            Console.WriteLine(TestEnumSByte.Max);
        }
        [TestCase]
        public static void Test14()
        {
            var arr = Enum.GetNames(typeof(TestEnum));
            foreach (var i in arr)
            {
                Console.WriteLine(i);
            }
        }
        class SystemType
        {
            public int value = 10;
        }

        class TestConstructor
        {
            int[] ArrayTest = null;
            TestEnum[] ArrayTest2 = new TestEnum[9];
            TestEnum[] ArrayTest3 = null;
            SystemType ReferenceTest1 = null;
            public void ArrayLengthTest()
            {
                if (ArrayTest == null)
                    ArrayTest = new int[5];
                Console.WriteLine(string.Format("Int array type {0}", ArrayTest.GetType().Name));
                Console.WriteLine(string.Format("Int array length {0}", ArrayTest.Length));
                if (ArrayTest2 == null)
                    ArrayTest2 = new TestEnum[10];
                Console.WriteLine(string.Format("Enum array type {0}", ArrayTest2.GetType().Name));
                Console.WriteLine(string.Format("Enum array length {0}", ArrayTest2.Length));
                if (ReferenceTest1 == null)
                    ReferenceTest1 = new SystemType();
                Console.WriteLine(string.Format("SystemType type {0}", ReferenceTest1.GetType().Name));
                Console.WriteLine(string.Format("SystemType va {0}", ReferenceTest1.value));
                if (ArrayTest3 == null)
                    ArrayTest3 = new TestEnum[15];
                Console.WriteLine(string.Format("Enum array type {0}", ArrayTest3.GetType().Name));
                Console.WriteLine(string.Format("Enum array length {0}", ArrayTest3.Length));
            }
        }
        [TestCase]
        public static void Test15()
        {
            TestConstructor test = new TestConstructor();
            test.ArrayLengthTest();
        }
    }
}
