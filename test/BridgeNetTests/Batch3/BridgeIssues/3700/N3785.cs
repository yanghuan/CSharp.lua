using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3785 - {0}")]
    public class Bridge3785
    {
        public static class Ext
        {
            public static T Boo<T>(T x)
            {
                return x;
            }
        }

        public class Model
        {
            public DateTimeOffset? CreatedDate { get; set; }
        }

        [Test]
        public static void TestNullableExtension()
        {
            int? y = 0;
            Nullable<int> y1 = null;
            Assert.AreEqual(0, Ext.Boo(y), "Ext.Boo(y), where y == 0, results in 0.");
            Assert.Null(Ext.Boo(y1), "Ext.Boo(y1), where y1 == null, results in null.");

            var a = new List<Model>();
            Assert.AreEqual(0, a.OrderByDescending(i => i.CreatedDate).Count(), "Linq Order() works in empty list.");
        }
    }
}