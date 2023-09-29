using Bridge.ClientTest;
using Bridge.Test.NUnit;

namespace Batch1.src.Templates
{
    [Category(Constants.MODULE_TEMPLATES)]
    [TestFixture(TestNameFormat = "Property templates - {0}")]
    public class TemplatePropertyTest
    {
        [Test]
        public static void PropertyTemplateGetSetTest()
        {
            var test = new TestType();
            for (var i = 0; i < 10; i++)
            {
                test.FullProp = i;
                Assert.AreEqual(i, test.FullProp);
            }
        }

        [Test]
        public static void PropertyTemplateGetOnlySetOnlyTest()
        {
            var test = new TestType();
            for (var i = 0; i < 10; i++)
            {
                test.SetProp = i;
                Assert.AreEqual(i, test.GetProp);
            }
        }

        [Test]
        public static void PropertyTemplateGetOnlyTest()
        {
            var test = new TestType();
            Assert.AreEqual(4, test.GetOnlyProp);
        }

        [Test]
        public static void StaticPropertyTemplateGetSetTest()
        {
            var a = 5;
            Assert.AreEqual(5, a);
            TestType.StaticProp = 7;
            Assert.AreEqual(7, TestType.StaticProp);
        }

        [Test]
        public static void PropertyTemplateIncrementTest()
        {
            var test = new TestType
            {
                FullProp = 4
            };
            test.FullProp++;
            Assert.AreEqual(5, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateDecrementTest()
        {
            var test = new TestType
            {
                FullProp = 4
            };
            test.FullProp--;
            Assert.AreEqual(3, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateAddTest()
        {
            var test = new TestType
            {
                FullProp = 4
            };
            test.FullProp += 4;
            Assert.AreEqual(8, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateSubtractTest()
        {
            var test = new TestType
            {
                FullProp = 4
            };
            test.FullProp -= 9;
            Assert.AreEqual(-5, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateMultiplyTest()
        {
            var test = new TestType
            {
                FullProp = 4
            };
            test.FullProp *= 5;
            Assert.AreEqual(20, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateDivideTest()
        {
            var test = new TestType
            {
                FullProp = 4
            };
            test.FullProp /= 3;
            Assert.AreEqual(1, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateModuloTest()
        {
            var test = new TestType
            {
                FullProp = 13
            };
            test.FullProp %= 10;
            Assert.AreEqual(3, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateBitwiseAndTest()
        {
            var test = new TestType
            {
                FullProp = 3
            };
            test.FullProp &= 5;
            Assert.AreEqual(1, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateBitwiseOrTest()
        {
            var test = new TestType
            {
                FullProp = 3
            };
            test.FullProp |= 5;
            Assert.AreEqual(7, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateLeftShiftTest()
        {
            var test = new TestType
            {
                FullProp = 5
            };
            test.FullProp <<= 4;
            Assert.AreEqual(80, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateRightShiftTest()
        {
            var test = new TestType
            {
                FullProp = 800
            };
            test.FullProp >>= 4;
            Assert.AreEqual(50, test.FullProp);
        }

        [Test]
        public static void PropertyTemplateConcatenateTest()
        {
            var test = new TestType
            {
                StringProp = "foo"
            };
            test.StringProp += "bar";
            Assert.AreEqual("foobar", test.StringProp);
        }

        [Test]
        public static void PropertyTemplateNullCoallesceTest()
        {
            var test = new TestType();
            test.StringProp ??= "hello world";
            Assert.AreEqual("hello world", test.StringProp);
        }

        [Test]
        public static void PropertyTemplateNullableTest()
        {
            var test = new TestType();
            Assert.False(test.NullableProp.HasValue);
            test.NullableProp = 5;
            Assert.True(test.NullableProp.HasValue);
            Assert.AreEqual(5, test.NullableProp.Value);
        }

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        private class TestType
        {
            /// @CSharpLua.Get = "a"
            /// @CSharpLua.Set = "a = {0}"
            public static extern int StaticProp { get; set; }

            /// @CSharpLua.Get = "{this}.CustomProp1"
            /// @CSharpLua.Set = "{this}.CustomProp1 = {0}"
            public extern int FullProp { get; set; }
            /// @CSharpLua.Get = "{this}.CustomProp2"
            public extern int GetProp { get; set; }
            /// @CSharpLua.Set = "{this}.CustomProp2 = {0}"
            public extern int SetProp { get; set; }
            /// @CSharpLua.Get = "4"
            public extern int GetOnlyProp { get; }

            /// @CSharpLua.Get = "{this}.CustomProp3"
            /// @CSharpLua.Set = "{this}.CustomProp3 = {0}"
            public extern string StringProp { get; set; }

            /// @CSharpLua.Get = "{this}.CustomProp4"
            /// @CSharpLua.Set = "{this}.CustomProp4 = {0}"
            public extern int? NullableProp { get; set; }
        }
    }
}
