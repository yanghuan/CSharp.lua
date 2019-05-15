using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3794 - {0}")]
    public class Bridge3794
    {
        [Reflectable]
        public class Test
        {
            static Test() { }
            public Test() { }
        }

        [Test]
        public static void TestCreateInstance()
        {
            Test test = Activator.CreateInstance<Test>();
            Assert.NotNull(test, "Creating instance of class containing static constructor works.");
        }
    }
}