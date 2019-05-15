using Bridge.Test.NUnit;
using System.Collections;
using System.Collections.Generic;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3672 - {0}")]
    public class Bridge3672
    {
        [Test]
        public static void TestStringComparer()
        {
            var str1 = "ab";
            var str2 = "Cd";
            var strC = Comparer<string>.Default;
            Assert.AreEqual(-1, strC.Compare(str1, str2), "string.Compare(a, b) works.");
            Assert.AreEqual(-1, str1.CompareTo(str2), "string.CompareTo(a) works.");
        }
    }
}