using System;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp14;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "Field Keyword - {0}")]
public sealed class FieldKeywordTests {
    private class Item {
        public string Title {
            get => field;
            set => field = value;
        } = "Default";

        public string Tag {
            get => field;
            set => field = value.ToUpper();
        }

        public int Score {
            get => field;
            set {
                if (value >= 0) {
                    field = value;
                }
            }
        }
    }

    [Test]
    public static void TestDefaultValue() {
        var item = new Item();
        Assert.AreEqual("Default", item.Title);
        item.Title = "Custom";
        Assert.AreEqual("Custom", item.Title);
    }

    [Test]
    public static void TestSetterTransform() {
        var item = new Item();
        item.Tag = "lua";
        Assert.AreEqual("LUA", item.Tag);
    }

    [Test]
    public static void TestSetterValidation() {
        var item = new Item();
        item.Score = 50;
        Assert.AreEqual(50, item.Score);
        item.Score = -10; // should be ignored
        Assert.AreEqual(50, item.Score);
    }
}
