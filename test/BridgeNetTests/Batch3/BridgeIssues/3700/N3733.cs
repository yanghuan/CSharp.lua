using Bridge.Test.NUnit;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3733 - {0}")]
    public class Bridge3733
    {
        private const int A = 10;
        private const int B = 20;

        private const int C = A > B ? A : B;
        private static readonly int[] Test = new int[C * 10];

        [Test]
        public static void TestConstants()
        {
            Assert.AreEqual(10, A, "A is 10");
            Assert.AreEqual(20, B, "B is 20");
            Assert.False(A > B, "A is not greater than B");
            Assert.AreEqual(B, C, "C received value from A");
        }
    }
}