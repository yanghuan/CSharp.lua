using Bridge.Test.NUnit;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    public class SubClass : BaseClass
    {
        public SubClass() : base(1)
        {

        }
    }

    public class SubClass0 : BaseClass
    {
        public SubClass0() : base(0)
        {

        }
    }

    public class SubClass0m : BaseClass
    {
        public SubClass0m() : base(0m)
        {

        }
    }

    public class BaseClass
    {
        public BaseClass(decimal value)
        {
            this.Value = value;
        }

        public decimal Value { get; set; }
    }

    [TestFixture(TestNameFormat = "#3714 - {0}")]
    public class Bridge3714
    {
        [Test]
        public static void TestBaseCtorArgumentConversion()
        {
            var test = new SubClass();
            var v = test.Value + test.Value;

            Assert.True(v == 2m, "Implicit conversion on base constructor call works when passing 1 to decimal.");

            var test0 = new SubClass0();
            var v0 = test0.Value + test0.Value;

            Assert.True(v0 == 0, "Implicit conversion on base constructor call works when passing 0 to decimal.");

            var test0m = new SubClass0m();
            var v0m = test0.Value + test0.Value;

            Assert.True(v0m == 0, "Base constructor call works when passing 0m to decimal (explicit conversion).");
        }
    }
}