using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3775 - {0}")]
    public class Bridge3775
    {
        public class ReverseComparer<T> : IComparer<T>
        {
            private IComparer<T> _comparer;
            public ReverseComparer(IComparer<T> comparer)
            {
                _comparer = comparer;
            }
            public int Compare(T x, T y) => _comparer.Compare(y, x);
        }

        [Test]
        public static void TestComparerScope()
        {
            var comparer = new ReverseComparer<int>(Comparer<int>.Default);
            var list = new List<int>() { 0, 1, 2 };
            try
            {
                Assert.AreEqual(1, list.BinarySearch(1, comparer), "1 found in list at index 1.");
            }
            catch (Exception)
            {
                Assert.Fail("Exception thrown in BinarySearch");
            }
        }
    }
}