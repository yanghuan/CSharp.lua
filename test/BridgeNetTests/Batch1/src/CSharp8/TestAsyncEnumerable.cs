using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp8
{
    [Category(Constants.MODULE_BASIC_CSHARP)]
    [TestFixture(TestNameFormat = "AsyncEnumerable - {0}")]
    public sealed class TestAsyncEnumerable
    {
        public static async System.Collections.Generic.IAsyncEnumerable<int> GenerateSequence(int n)
        {
            for (int i = 0; i < n; i++)
            {
                await Task.Delay(100);
                yield return i;
                await Task.Delay(100);
            }
        }

        //[Test]
        public async static void TestForeach()
        {
            List<int> l = new List<int>();
            await foreach (var number in GenerateSequence(4))
            {
                l.Add(number);
            }
            Assert.AreEqual(l, new int[] { 0, 1, 2, 3, });
        }
    }
}
