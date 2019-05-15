using Bridge.Test.NUnit;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3757 - {0}")]
    public class Bridge3757
    {
        [Test]
        public static void TestDateTimeDateComponent()
        {
            var a = new System.DateTime(2001, 2, 3, 4, 5, 6).Date;
            var b = new System.DateTime(2001, 2, 3, 4, 5, 7);
            var c = new System.DateTime(2001, 2, 3, 4, 5, 8).Date;

            //gist of issue: throws exception due to wrong DateTime.js $clearTime
            Assert.True(!b.Equals(a), "DateTime(2001, 2, 3, 4, 5, 7).Equals(DateTime(2001, 2, 3, 4, 5, 6).Date) returns false.");
            Assert.AreNotEqual(b, a, "DateTime(2001, 2, 3, 4, 5, 7) and DateTime(2001, 2, 3, 4, 5, 6).Date are different.");

            //next two are safety guards that fix doesn't introduce new bugs
            Assert.True(!a.Equals(b), "DateTime(2001, 2, 3, 4, 5, 6).Date.Equals(DateTime(2001, 2, 3, 4, 5, 7)) returns false.");
            Assert.AreNotEqual(a, b, "DateTime(2001, 2, 3, 4, 5, 6).Date and DateTime(2001, 2, 3, 4, 5, 7) are different.");

            Assert.True(a.Equals(c), "DateTime(2001, 2, 3, 4, 5, 6).Date.Equals(DateTime(2001, 2, 3, 4, 5, 8).Date) return true.");
            Assert.AreEqual(a, c, "DateTime(2001, 2, 3, 4, 5, 6).Date DateTime(2001, 2, 3, 4, 5, 8).Date are equal.");
        }
    }
}
