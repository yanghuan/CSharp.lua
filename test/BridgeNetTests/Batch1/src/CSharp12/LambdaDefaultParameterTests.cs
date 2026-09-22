using System;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp12;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "Lambda Default Parameters - {0}")]
public sealed class LambdaDefaultParameterTests {
    [Test]
    public static void TestExpressionBodyWithDefault() {
        var add = (int x, int y = 10) => x + y;
        Assert.AreEqual(15, add(5));
        Assert.AreEqual(7, add(5, 2));
    }

    [Test]
    public static void TestBlockBodyWithDefaults() {
        var greet = (string name = "World", string prefix = "Hello") => {
            return prefix + ", " + name + "!";
        };
        Assert.AreEqual("Hello, World!", greet());
        Assert.AreEqual("Hello, Alice!", greet("Alice"));
        Assert.AreEqual("Hi, Bob!", greet("Bob", "Hi"));
    }

    [Test]
    public static void TestMultipleTypesAndNilCheck() {
        var format = (int count, bool uppercase = false, string tag = "item") => {
            string s = count + " " + tag;
            return uppercase ? s.ToUpper() : s;
        };
        Assert.AreEqual("3 item", format(3));
        Assert.AreEqual("3 ITEM", format(3, true));
        Assert.AreEqual("5 cats", format(5, false, "cats"));
    }
}
