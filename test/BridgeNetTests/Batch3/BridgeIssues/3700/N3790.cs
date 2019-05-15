using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3790 - {0}")]
    public class Bridge3790
    {
        public class MyClass
        {
            public MyClass()
            {
                this.Test(null);
            }

            private void Test(Action<TimeSpan?> callback)
            {
            }
        }

        [Test]
        public static void TestArgumentDelegate()
        {
            MyClass mc = new MyClass();

            Assert.NotNull(mc, "Instantiation of class involving nullable generics specification works.");
        }
    }
}