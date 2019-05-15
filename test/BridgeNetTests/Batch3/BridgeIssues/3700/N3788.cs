using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3788 - {0}")]
    public class Bridge3788
    {
        private static void DoInt5((int, int) val)
        {
            Assert.AreEqual(0, val.Item1, "Int tuple's Item1 value in DoInt5() is correct.");
            Assert.AreEqual(0, val.Item2, "Int tuple's Item2 value in DoInt5() is correct.");
        }

        public static void DoInt(int x)
        {
            Assert.AreEqual(0, x, "Int value ind DoInt() is correct.");
        }

        public static void DoInt2<T>(int x)
        {
            Assert.AreEqual(0, x, "Int value in DoInt2<T>() is correct.");
        }

        public static class Test<T>
        {
            public delegate void DoDel(T val);
            public delegate void DoDel1<T2>(T val);

            public static void DoFunc(DoDel doIt)
            {
                doIt(default(T));
            }

            public static void DoFunc2(Action<T> doIt)
            {
                doIt(default(T));
            }

            public static void DoFunc3(DoDel1<T> doIt)
            {
                doIt(default(T));
            }

            internal static void DoFunc4(Action<int> doIt)
            {
                doIt(0);
            }
        }

        [Test]
        public static void TestGenericTypeResolver()
        {
            Test<int>.DoFunc2(DoInt);
            Test<int>.DoFunc(DoInt);
            Test<(int, int)>.DoFunc(DoInt5);
            Test<int>.DoFunc3(DoInt2<int>);
            Test<(int, int)>.DoFunc4(DoInt2<(int, int)>);
        }
    }
}