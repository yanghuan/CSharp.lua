using Bridge.Test.NUnit;
using System;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    public static class Bridge3759_Exts
    {
        public static string Arr(this string self, params object[] args)
        {
            return string.Format(self, args);
        }

        public static string Inst(this string self, int arg)
        {
            return string.Format(self, arg);
        }

        public static string Obj(this string self, Object arg)
        {
            return string.Format(self, arg);
        }
    }

    [TestFixture(TestNameFormat = "#3759 - {0}")]
    public class Bridge3759
    {
        [Bridge.Rules(Boxing = Bridge.BoxingRule.Managed)]
        public static string ExtArrWithBoxingEnabled(DateTime i)
        {
            return "{0:0000}".Arr(i.Month);
        }

        [Bridge.Rules(Boxing = Bridge.BoxingRule.Managed)]
        public static string ExtInstWithBoxingEnabled(DateTime i)
        {
            return "{0:0000}".Inst(i.Month);
        }

        [Bridge.Rules(Boxing = Bridge.BoxingRule.Managed)]
        public static string ExtObjWithBoxingEnabled(DateTime i)
        {
            return "{0:0000}".Obj(i.Month);
        }

        [Bridge.Rules(Boxing = Bridge.BoxingRule.Managed)]
        public static string RegularCallWithBoxingEnabled(DateTime i)
        {
            return string.Format("{0:0000}", i.Month);
        }

        [Test]
        public static void TestBoxing()
        {
            var bug = 0;

            var x = new System.DateTime(2001, 2, 3);
            try
            {
                if (ExtArrWithBoxingEnabled(x) != "0002")
                {
                    bug++;
                    Assert.Fail("[1] didn't get expected value");
                }
            }
            catch (Exception)
            {
                bug++;
                Assert.Fail("[2] call failed");
            }

            try
            {
                if (ExtInstWithBoxingEnabled(x) != "0002")
                {
                    bug++;
                    Assert.Fail("[3] didn't get expected value");
                }
            }
            catch (Exception)
            {
                bug++;
                Assert.Fail("[4] call failed");
            }

            try
            {
                if (ExtObjWithBoxingEnabled(x) != "0002")
                {
                    bug++;
                    Assert.Fail("[5] didn't get expected value");
                }
            }
            catch (Exception)
            {
                bug++;
                Assert.Fail("[6] call failed");
            }

            try
            {
                if (RegularCallWithBoxingEnabled(x) != "0002")
                {
                    bug++;
                    Assert.Fail("[7] didn't get expected value");
                }
            }
            catch (Exception)
            {
                bug++;
                Assert.Fail("[8] call failed");
            }

            Assert.AreEqual(0, bug, "All boxed methods could be called and returned the expected value.");
        }
    }
}