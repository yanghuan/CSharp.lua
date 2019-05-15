using Bridge.Test.NUnit;
using System;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3738 - {0}")]
    public class Bridge37383
    {
        public class TestC
        {
            public string Value { get; } = "String Value";
        }

        public class TestB
        {
            public Object InstanceC { get; } = new TestC();
        }

        public class TestA
        {
            public TestB InstanceB { get; } = new TestB();
        }

        [Test]
        public static void TestConditionalAccess()
        {
            // Create a new instance of class A
            TestA instanceA = new TestA();

            // ** Work-around example **
            TestC innerClassWorkaround = (TestC)(instanceA.InstanceB?.InstanceC);
            Assert.AreEqual("String Value", innerClassWorkaround.Value, "Parens grouping association workaround works.");

            // Maybe "nicer" workaround?
            TestC innerClass2 = instanceA.InstanceB?.InstanceC as TestC;
            Assert.AreEqual("String Value", innerClass2.Value, "'as' casting works.");

            // ** Bug example **
            // Bridge.NET: Exception thrown: Unable to cast TestB to TestC.
            TestC innerClass = (TestC)instanceA.InstanceB?.InstanceC;
            Assert.AreEqual("String Value", innerClass.Value, "No grouping association works.");
        }
    }
}