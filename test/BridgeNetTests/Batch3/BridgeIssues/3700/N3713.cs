using Bridge.Test.NUnit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3713 - {0}")]
    public class Bridge3713
    {
        [Test]
        public static void TestValueType()
        {
            int test = 123;
            object o = test;
            ValueType vt = (ValueType)test;

            Assert.AreEqual(123, (int)vt, "A ValueType instance can be cast to int.");
            Assert.AreEqual("123", vt.ToString(), "A ValueType instance's ToString() method works.");
            Assert.True(o is ValueType, "An object containing a ValueType-capable value can be probed as a ValueType.");
        }
    }
}