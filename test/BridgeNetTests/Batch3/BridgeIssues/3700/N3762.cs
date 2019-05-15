using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    public class Bridge3762_A
    {
        public string P { get; set; }

        public Bridge3762_A()
        {
            P = ""; //dotnetfiddle doesn't support property initializer
        }

        public override string ToString()
        {
            return P;
        }
    }

    public class Bridge3762_B
    {
        public static string Fail<T>(T v, Func<T, string> transform)
        {
            return transform(v);
        }

        public static string Fail2<T>(Func<T, string> transform)
        {
            return transform(default(T));
        }

        public static string Fine<T>(T v)
        {
            return v.ToString();
        }

        public static Bridge3762_A Helper(Bridge3762_A aaa)
        {
            return aaa;
        }
    }

    [TestFixture(TestNameFormat = "#3762 - {0}")]
    public class Bridge3762
    {
        [Test]
        public static void TestModelResolving()
        {
            Bridge.DoTest();
        }

        public class Bridge
        {
            public static void DoTest()
            {
                // used to fail in 17.4.0.
                Assert.AreEqual(
                    "123456",
                    Bridge3762_B.Fail(new Bridge3762_A { P = "123" }, x => x.P + "456"),
                    "Works: B.Fail(new A {P = \"123\"}, x => x.P+\"456\")");
                Assert.AreEqual(
                    "678",
                    Bridge3762_B.Fail2((Bridge3762_A x) => x + "678"),
                    "Works: B.Fail2((A x) => x+\"678\")");

                // workaround for 17.4.0.
                Assert.AreEqual(
                    "123456",
                    Bridge3762_B.Fail<Bridge3762_A>(new Bridge3762_A { P = "123" }, x => x.P + "456"),
                    "Workaround works: B.Fail<A>(new A {P = \"123\"}, x => x.P+\"456\")");
                Assert.AreEqual(
                    "678",
                    Bridge3762_B.Fail2<Bridge3762_A>((Bridge3762_A x) => x + "678"),
                    "Workaround works: B.Fail2<A>((A x) => x+\"678\")");

                Assert.AreEqual(
                    "13",
                    Bridge3762_B.Fine(new Bridge3762_A { P = "13" }),
                    "Works: B.Fine(new A {P = \"13\"})");

                // Again, used to fail in 17.4.0.
                var lst = new List<Bridge3762_A> { new Bridge3762_A { P = "22" } };
                var lstFail = lst.Select(z => {
                    return Bridge3762_B.Helper(z);
                }).ToList();

                Assert.AreEqual(1, lstFail.Count,
                    "Non-empty list: (new List<A> {new A {P = \"22\"} }).Select(z => {return B.Helper(z);}).ToList()");
                Assert.AreEqual("22", lstFail[0].P,
                    "Produces expected value: (new List<A> {new A {P = \"22\"} }).Select(z => {return B.Helper(z);}).ToList()");

                // And workaround with the same functionality
                var lstWorkaround = lst.Select<Bridge3762_A, Bridge3762_A>(z => {
                    return Bridge3762_B.Helper(z);
                }).ToList();

                Assert.AreEqual(1, lstWorkaround.Count,
                    "Workaround non-empty list: lst.Select<A,A>(z => {return B.Helper(z);}).ToList()");
                Assert.AreEqual("22", lstFail[0].P,
                    "Workaround produces expected value: lst.Select<A,A>(z => {return B.Helper(z);}).ToList()");
            }
        }
    }
}