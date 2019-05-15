using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Batch3.BridgeIssues
{
    [TestFixture(TestNameFormat = "#3796 - {0}")]
    public class Bridge3796
    {
        [Test]
        public static void TestInt64Cast()
        {
            double double1 = 0L;
            var result = double1 / double1;
            Assert.True(Double.IsNaN(result), "x/x result in double operation is NaN.");

            // We cannot guarantee a NaN cast to a type is exactly a given number,
            // so we just check whether it is a number within the range the type comports.
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/conversions
            // Look for "conversion from float or double to an integral type", "unchecked context".
            Assert.True((sbyte)(result) >= sbyte.MinValue && (sbyte)(result) <= sbyte.MaxValue,
                "NaN cast into sbyte value is within sbyte range.");
            Assert.True((byte)(result) >= byte.MinValue && (byte)(result) <= byte.MaxValue,
                "NaN cast into byte value is within short byte.");
            Assert.True((short)(result) >= short.MinValue && (short)(result) <= short.MaxValue,
                "NaN cast into short value is within short range.");
            Assert.True((ushort)(result) >= ushort.MinValue && (ushort)(result) <= ushort.MaxValue,
                "NaN cast into ushort value is within ushort range.");
            Assert.True((int)(result) >= int.MinValue && (int)(result) <= int.MaxValue,
                "NaN cast into int value is within int range.");
            Assert.True((uint)(result) >= uint.MinValue && (uint)(result) <= uint.MaxValue,
                "NaN cast into uint value is within uint range.");
            Assert.True((long)(result) >= long.MinValue && (long)(result) <= long.MaxValue,
                "NaN cast into long value is within long range.");
            Assert.True((ulong)(result) >= ulong.MinValue && (ulong)(result) <= ulong.MaxValue,
                "NaN cast into ulong value is within ulong range.");
        }
    }
}