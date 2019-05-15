using Bridge.Test.NUnit;
using static Bridge.ClientTest.Batch3.BridgeIssues.Bridge3467_Test1;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    /// <summary>
    /// This tests whether a sealed nested class set up works on Bridge.
    /// The way the tests are written allows it to break the code no matter
    /// how minification is set in bridge.json -- as long as the issue is still
    /// present.
    /// </summary>
    sealed class Bridge3467_Test1 : Bridge3467_ITest<NestedClass>
    {
        public sealed class NestedClass { }
    }

    interface Bridge3467_ITest<T> { }

    [Category(Constants.MODULE_ISSUES)]
    [TestFixture(TestNameFormat = "#3467 - {0}")]
    public class Bridge3467
    {
        /// <summary>
        /// Just check if a new instance of the class is not null. Actually
        /// what counts here is just if the code will result in runnable js.
        /// </summary>
        [Test]
        public static void TestGenericUsingStatic()
        {
            Assert.NotNull(new Bridge3467_Test1(), "Instantiating nested, sealed classes works.");
        }
    }
}