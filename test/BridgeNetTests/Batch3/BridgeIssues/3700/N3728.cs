using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3728 - {0}")]
    public class Bridge3728
    {
        [Test]
        public static void TestGnericParameterValueTuple()
        {
            List<(Guid A, int B)> list = new List<(Guid A, int B)>();
            list.Add((Guid.NewGuid(), 123));

            Assert.AreEqual(123, list.First().B, "Generics with ValueTuple Enumerables works.");
        }
    }
}