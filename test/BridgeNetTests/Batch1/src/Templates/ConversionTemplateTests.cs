using Bridge.ClientTest;
using Bridge.Test.NUnit;

namespace Batch1.src.Templates
{
    [Category(Constants.MODULE_TEMPLATES)]
    [TestFixture(TestNameFormat = "Conversion templates - {0}")]
    public class ConversionTemplateTests
    {
        [Test]
        public void TestImplicitCast()
        {
            var type1 = new Type1();
            Type2 type2 = type1;

            // Type didn't actually change due to the template
            Assert.AreEqual($"{nameof(ConversionTemplateTests)}+{nameof(Type1)}", type2.GetType().Name);
        }

        [Test]
        public void TestExplicitCast()
        {
            var type2 = new Type2();
            var type1 = (Type1)type2;

            // Type didn't actually change due to the template
            Assert.AreEqual($"{nameof(ConversionTemplateTests)}+{nameof(Type2)}", type1.GetType().Name);
        }

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        private class Type1
        {
            /// @CSharpLua.Template = "{0}"
            public static extern implicit operator Type2(Type1 t);
        }

        private class Type2
        {
            /// @CSharpLua.Template = "{0}"
            public static extern explicit operator Type1(Type2 t);
        }
    }
}
