using System;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp11;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "Generic Attributes - {0}")]
public sealed class GenericAttributeTests {
    [AttributeUsage(AttributeTargets.All)]
    private class GenericTypeAttribute<T> : Attribute {
        public Type TargetType => typeof(T);
        public string Extra { get; set; }

        public GenericTypeAttribute(string extra = null) {
            Extra = extra;
        }
    }

    [GenericType<string>("hello")]
    private class TargetClass1 {
    }

    [GenericType<int>]
    private class TargetClass2 {
    }

    [Test]
    public static void TestGenericAttributeTypeArg() {
        var attrs = typeof(TargetClass1).GetCustomAttributes(typeof(GenericTypeAttribute<string>), false);
        Assert.AreEqual(1, attrs.Length);
        var attr1 = (GenericTypeAttribute<string>)attrs[0];
        Assert.AreEqual(typeof(string), attr1.TargetType);
        Assert.AreEqual("hello", attr1.Extra);

        var attrs2 = typeof(TargetClass2).GetCustomAttributes(typeof(GenericTypeAttribute<int>), false);
        Assert.AreEqual(1, attrs2.Length);
        var attr2 = (GenericTypeAttribute<int>)attrs2[0];
        Assert.AreEqual(typeof(int), attr2.TargetType);
        Assert.Null(attr2.Extra);
    }
}
