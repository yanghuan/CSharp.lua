using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3786 - {0}")]
    public class Bridge3786
    {
        static void Test(out (int a, int b) tuple)
        {
            tuple = (1, 2);
        }

        static void Case1()
        {
            Test(out var tuple);
            Assert.AreEqual(1, tuple.a, "Tuple's Item1 value is valid.");
            Assert.AreEqual(2, tuple.b, "Tuple's Item2 value is valid.");
        }

        static void Case2()
        {
            (int a, int b) tuple;
            Test(out tuple);
            Assert.AreEqual(1, tuple.a, "Tuple's Item1 value is valid.");
            Assert.AreEqual(2, tuple.b, "Tuple's Item2 value is valid.");
        }

        [Test]
        public static void TestInlineTupleOut()
        {
            Case1();
            Case2();
        }
    }
}