using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bridge.ClientTest;
using Bridge.Test.NUnit;

namespace Batch1.src.BasicCSharp
{
    [Category(Constants.MODULE_BASIC_CSHARP)]
    [TestFixture(TestNameFormat = "From issues - {0}")]
    public sealed class TestFromIssue
    {
        [Test]
        public static void TestOf351()
        {
            var a = new ExtendedClass();
            Assert.AreEqual(0, a.Property.Value);
        }
    }

    public class BaseClass<T>
    {
        public Test<T> Property { get; set; } = new Test<T>();
    }

    public class ExtendedClass : BaseClass<int> { }

    public class Test<T>
    {
        public T Value { get; set; }
    }
}
