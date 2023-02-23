using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bridge.ClientTest;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.BasicCSharp
{
    [Category(Constants.MODULE_BASIC_CSHARP)]
    [TestFixture(TestNameFormat = "From issues - {0}")]
    public sealed class TestFromIssue
    {
        [Test]
        public static void TestOf351()
        {
            var a = new ExtendedClass();
            Assert.AreEqual(0, a.Property.Value);
        }

        [Test]
        public static void TestCallOutPrivate()
        {
            new Test<int>.A().RunTest();
        }

        [Test]
        public static void TestOf398()
        {
            var arr = new A[] 
            {
                new A(i => 
                {
                    if (i > 0) {
                        // 注释测试
                    }
                }),
                new A(i => 
                {
                }),
                new A(i => 
                {
                }),
            };
        }

        [Test]
        public static void TestOf408() {
            var tList = typeof(List<>);
            var tListArgs = new[]{ typeof(int) };
            // tNew incorrect
            var tNew = tList.MakeGenericType(tListArgs);
            var tNewArgs = tNew.GetGenericArguments();
            // fail, given 'System.Type'
            Assert.AreEqual("System.Int32" , tNewArgs[0].FullName);
        }

        [Test]
        public static void TestOf410() {
            var type = typeof(IEnumerable<IEnumerable<int>>);
            var genericType = type.GetGenericArguments();
            Assert.AreEqual("System.Collections.Generic.IEnumerable`1[System.Int32]", genericType[0].FullName);

            var type1 = typeof(IEnumerable<IEnumerable<IEnumerable<int>>>);
            var genericType1 = type1.GetGenericArguments();
            Assert.AreEqual("System.Collections.Generic.IEnumerable`1[System.Collections.Generic.IEnumerable`1[System.Int32]]", genericType1[0].FullName);
        }

        [Test]
        public static void TestOf417() {
            IEnumerable<string> a = new List<string>();
            IEnumerable<string> b = new List<string>();
            int result = (true && a is List<string> a1 && b is List<string> b1) 
                ? a1.Count + b1.Count + 1 : 0;
            Assert.AreEqual(result, 1);
        }

        [Test]
        public static void TestOf418() {
            static void OnKickTeamMemberClick() {
            }
            bool isLeader = true;
            Action kickAction = isLeader ? OnKickTeamMemberClick : null;
            Assert.AreEqual(kickAction, OnKickTeamMemberClick);
        }

        [Test]
        public static void TestOf419() {
            static string GetStr() {
                return "";
            }
            var d = new Dictionary<int, BattleModelSlotPrototype>();
            var b = new BattleModelSlotPrototype() {
                a = GetStr() ?? "",
                b = GetStr() ?? "",
                c = GetStr() ?? "",
            };
            d.Add(1, b);
            Assert.AreEqual(d.Count, 1);
        }

        private class BattleModelSlotPrototype {
            public string a;
            public string b;
            public string c;
        }

        private class A
        {
            public A(Action<int> f)
            {

            }
        }
    }

    public class BaseClass<T>
    {
        public Test<T> Property { get; set; } = new Test<T>();
    }

    public class ExtendedClass : BaseClass<int> { 

    }

    public class Test<T>
    {
        public class A
        {
            public void RunTest()
            {
                Assert.AreEqual(100, a_);
                Assert.AreEqual(100, b_);
                Assert.AreEqual(d_, default(DateTime));
                f();
            }
        }

        public T Value { get; set; }
        private static int a_ = 100;
        private static int b_ { get { return a_; } }
        private static DateTime d_;
        private static void f() {}
    }
}
