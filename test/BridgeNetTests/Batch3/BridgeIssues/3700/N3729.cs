using Bridge.Test.NUnit;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    /// <summary>
    /// Tests whether a scenario on multiple inheritance and generics works.
    /// </summary>
    [TestFixture(TestNameFormat = "#3729 - {0}")]
    public class Bridge3729
    {
        public interface Interface1
        {
            [Rules(AutoProperty = AutoPropertyRule.Plain)]
            string Name { get; set; }
        }

        public interface Interface2 : Interface1
        {

        }

        public interface Interface3 : Interface1
        {

        }

        public class Class1<T> : Interface1
        {
            [Rules(AutoProperty = AutoPropertyRule.Plain)]
            public string Name { get; set; }
        }

        public class Class2 : Class1<string>, Interface1, Interface2, Interface3
        {

        }

        /// <summary>
        /// Instantiate the classes, and also cast them into their interfaces
        /// and check whether the 'AutoProperty.Plain' property works.
        /// </summary>
        [Test]
        public static void TestAliases()
        {
            var c2 = new Class2();
            Class1<string> c1 = c2;
            Interface2 i2 = c2;
            Interface3 i3 = c2;
            Interface1 i1 = c2;

            c1.Name = "test";

            Assert.AreEqual("test", c2.Name, "Generic class, multiple-inheritance works.");
            Assert.AreEqual("test", i1.Name, "Base-level interface works.");
            Assert.AreEqual("test", i2.Name, "Inherit-driven interface 1 works.");
            Assert.AreEqual("test", i3.Name, "Inherit-driven interface 2 works.");
        }
    }
}