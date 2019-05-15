using Bridge.Test.NUnit;
using System;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    /// <summary>
    /// This consists of checking whether InvariantCultureIgnoreCase IndexOf()
    /// works on strings.
    /// </summary>
    [TestFixture(TestNameFormat = "#3679 - {0}")]
    public class Bridge3679
    {
        /// <summary>
        /// Simply part from a simple 'hello world' example to see whether
        /// we can use the method from String class:
        /// String.IndexOf(StringComparison.InvariantCultureIgnoreCase)
        /// </summary>
        [Test]
        public static void TestStringIndexOf()
        {
            var msg = "Hello, World!";

            int i = msg.IndexOf("world", 3, StringComparison.InvariantCultureIgnoreCase);

            Assert.AreEqual(7, i, "IndexOf, InvariantCulture, IgnoringCase: works.");
        }
    }
}