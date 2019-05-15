using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3791 - {0}")]
    public class Bridge3791
    {
        [Flags]
        public enum Mask : long
        {
            L00 = 1L << 00,
            L01 = 1L << 01,
            L02 = 1L << 02,
            L03 = 1L << 03,
            L04 = 1L << 04,
            L05 = 1L << 05,
            L06 = 1L << 06,
            L07 = 1L << 07,
            L08 = 1L << 08,
            //...
            L63 = 1L << 63
        }

        [Test]
        public static void TestEnumBitwise()
        {
            Mask mask1 = (Mask)0L;
            Mask mask2 = (Mask)0L;

            mask1 = mask1 | Mask.L02; // I am OK!
            mask2 |= Mask.L02; // But I am not!

            bool test1 = mask1 != 0;
            bool test2 = mask2 != 0;

            Assert.True(test1, "x = x | y syntax works.");
            Assert.True(test2, "x |= y syntax works.");

            mask1 = mask1 & Mask.L02; // I am OK!
            mask2 &= Mask.L02; // But I am not!

            test1 = mask1 != 0;
            test2 = mask2 != 0;

            Assert.True(test1, "x = x & y syntax works.");
            Assert.True(test2, "x &= y syntax works.");

        }
    }
}