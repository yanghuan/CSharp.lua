using System;
using System.Collections.Generic;
using Bridge.Test.NUnit;

using Point = (int X, int Y);
using IntList = System.Collections.Generic.List<int>;
using StringMap = System.Collections.Generic.Dictionary<string, int>;

namespace Bridge.ClientTest.CSharp12;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "Using Alias - {0}")]
public sealed class UsingAliasTests {
    [Test]
    public static void TestTupleAlias() {
        Point pt = (10, 20);
        Assert.AreEqual(10, pt.X);
        Assert.AreEqual(20, pt.Y);
    }

    [Test]
    public static void TestGenericCollectionAlias() {
        IntList list = new IntList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);

        StringMap map = new StringMap();
        map["alpha"] = 100;
        map["beta"] = 200;
        Assert.AreEqual(100, map["alpha"]);
        Assert.AreEqual(200, map["beta"]);
    }
}
