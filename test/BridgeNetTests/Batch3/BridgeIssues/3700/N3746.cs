using Bridge.Test.NUnit;
using System;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    /// <summary>
    /// Ensures array versions of class instances returns the correct
    /// assembly's FullName value.
    /// </summary>
    [TestFixture(TestNameFormat = "#3746 - {0}")]
    public class Bridge3746
    {
        public class MyClass
        {
        }

        /// <summary>
        /// Checks type of Object, local class and an array of that local
        /// class, for the expected assembly name.
        /// </summary>
        [Test]
        public static void TestAssemblyName()
        {
            Assert.AreEqual("mscorlib", typeof(Object).Assembly.FullName,
                "Object reads 'mscorlib'.");
            Assert.AreEqual("Bridge.ClientTest.Batch3", typeof(MyClass).Assembly.FullName,
                "Local class reads after the current assembly name.");
            Assert.AreEqual("Bridge.ClientTest.Batch3", typeof(MyClass[]).Assembly.FullName,
                "Array of the local class above reas as the current assembly name.");
        }
    }
}