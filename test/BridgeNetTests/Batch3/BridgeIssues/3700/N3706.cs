using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3706 - {0}")]
    public class Bridge3706
    {
        public static void FooWorking(KeyValuePair<String, Action<string>>[] actions)
        {
            actions[0].Value("KeyValuePair[]");
        }

        public static void FooFails(IList<KeyValuePair<String, Action<string>>> actions)
        {
            actions[0].Value("IList-KeyValuePair");
        }

        public static void Test(string what)
        {
            Assert.True(true, what + " action is invoked");
        }

        [Test(ExpectedCount = 2)]
        public static void TestIListIndexer()
        {
            var l = new KeyValuePair<String, Action<string>>[]
            {
            new KeyValuePair<String, Action<string>>("Bar", Test),
            };
            FooWorking(l);
            FooFails(l);
        }
    }
}