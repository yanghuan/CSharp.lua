using System;
using System.Collections.Generic;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp12;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "Collection Expressions - {0}")]
public sealed class CollectionExpressionTests {
    [Test]
    public static void TestArrayCreation() {
        int[] empty = [];
        Assert.AreEqual(0, empty.Length);

        int[] numbers = [1, 2, 3];
        Assert.AreEqual(3, numbers.Length);
        Assert.AreEqual(1, numbers[0]);
        Assert.AreEqual(2, numbers[1]);
        Assert.AreEqual(3, numbers[2]);
    }

    [Test]
    public static void TestListCreation() {
        List<string> list = ["alpha", "beta", "gamma"];
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("alpha", list[0]);
        Assert.AreEqual("beta", list[1]);
        Assert.AreEqual("gamma", list[2]);
    }

    [Test]
    public static void TestSpanCreation() {
        Span<int> span = [10, 20, 30];
        Assert.AreEqual(3, span.Length);
        Assert.AreEqual(10, span[0]);
        Assert.AreEqual(20, span[1]);
        Assert.AreEqual(30, span[2]);

        ReadOnlySpan<int> roSpan = [100, 200];
        Assert.AreEqual(2, roSpan.Length);
        Assert.AreEqual(100, roSpan[0]);
        Assert.AreEqual(200, roSpan[1]);
    }

    [Test]
    public static void TestSpreadElement() {
        int[] prefix = [1, 2];
        int[] all = [..prefix, 3, 4];
        Assert.AreEqual(4, all.Length);
        Assert.AreEqual(1, all[0]);
        Assert.AreEqual(2, all[1]);
        Assert.AreEqual(3, all[2]);
        Assert.AreEqual(4, all[3]);
    }
}
