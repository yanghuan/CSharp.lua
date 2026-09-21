using System;
using System.Collections.Generic;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp11;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "List Patterns - {0}")]
public sealed class ListPatternTests {
    [Test]
    public static void TestExactMatch() {
        int[] arr = new[] { 1, 2, 3 };
        Assert.True(arr is [1, 2, 3]);
        Assert.False(arr is [1, 2]);
        Assert.False(arr is [1, 2, 4]);
    }

    [Test]
    public static void TestSlicePattern() {
        int[] arr = new[] { 1, 2, 3, 4, 5 };
        Assert.True(arr is [1, 2, ..]);
        Assert.True(arr is [1, .., 5]);
        Assert.True(arr is [.., 5]);
        Assert.False(arr is [2, ..]);
    }

    [Test]
    public static void TestDiscardsAndVariables() {
        int[] arr = new[] { 10, 20, 30 };
        Assert.True(arr is [_, 20, _]);

        if (arr is [var first, .., var last]) {
            Assert.AreEqual(10, first);
            Assert.AreEqual(30, last);
        } else {
            Assert.Fail("Pattern should have matched");
        }
    }

    private static string Describe(int[] values) => values switch {
        [] => "empty",
        [var x] => $"single {x}",
        [var x, var y] => $"pair {x},{y}",
        [var first, .., var last] => $"many {first}..{last}"
    };

    [Test]
    public static void TestSwitchExpression() {
        Assert.AreEqual("empty", Describe(Array.Empty<int>()));
        Assert.AreEqual("single 42", Describe(new[] { 42 }));
        Assert.AreEqual("pair 1,2", Describe(new[] { 1, 2 }));
        Assert.AreEqual("many 1..10", Describe(new[] { 1, 2, 3, 10 }));
    }
}
