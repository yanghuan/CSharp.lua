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
