using Bridge.Test.NUnit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    /// <summary>
    /// This tests whether CreateInstance throws the expected method, yet does
    /// not lose its functionality to normal cases.
    /// </summary>
    [TestFixture(TestNameFormat = "#3680 - {0}")]
    public class Bridge3680
    {
        [Reflectable]
        public interface ITest
        {
            string A { get; set; }
        }

        [Reflectable]
        public class Test
        {
            private Test() { }
            string A { get; set; }
        }

        /// <summary>
        /// Explores the potential issues for class, interface, and also checks
        /// some situations where it should work.
        /// </summary>
        [Test]
        public static void TestActivator()
        {
            Assert.Throws<MissingMethodException>(() => Activator.CreateInstance(typeof(Test)),
                "Expected exception thrown for class.");
            Assert.Throws<MissingMethodException>(() => Activator.CreateInstance(typeof(ITest)),
                "Expected exception thrown for interface.");

            var test = Activator.CreateInstance(typeof(Test), true);

            Assert.NotNull(test, "CreateIntance() results in a non-null result.");
            Assert.True(test is Test, "CreateInstance() results in an instance of the expected class.");
        }
    }
}