using Bridge.Test.NUnit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    /// <summary>
    /// This extends the tests for issue 3728, ensuring the same scenario works
    /// with other linq extensions as well.
    /// </summary>
    [TestFixture(TestNameFormat = "#3754 - {0}")]
    public class Bridge3754
    {
        public class Test : IEnumerable<(string Name, string Value)>
        {
            private readonly Dictionary<string, string> data = new Dictionary<string, string> { { "a1", "b1" }, { "a2", "b2" } };

            public IEnumerator<(string Name, string Value)> GetEnumerator()
            {
                foreach (KeyValuePair<string, string> pair in this.data)
                {
                    yield return (pair.Key, pair.Value);
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// Tests with an assortment of linq queries to ensure this scenario
        /// works for the extensions, in general.
        /// </summary>
        [Test]
        public static void TestGnericParameterValueTuple()
        {
            Test test = new Test();

            Assert.AreEqual("a1", test.First().Name, "Linq.First() wokrs.");
            Assert.NotNull(test.Select(item => (object)item.Value), "Linq.Select() works.");
            Assert.NotNull(test.Select(item => (object)item.Value).First(), "Linq.Select().First() works.");
            Assert.True(test.Any(item => item.Name == "a2"), "Linq.Any(key) works.");
            Assert.True(test.Any(item => item.Value == "b1"), "Linq.Any(value) works.");
            Assert.True(test.All(item => item.Name.Length == 2), "Linq.All() works.");
            Assert.AreEqual("a2", test.Skip(1).First().Name, "Linq.Skip() works.");
            Assert.AreEqual("a1", test.ToArray()[0].Name, "Linq.ToArray() works.");
            Assert.AreEqual("a2", test.Where(item => item.Name != "a1").First().Name, "Linq.Where() works.");
        }
    }
}