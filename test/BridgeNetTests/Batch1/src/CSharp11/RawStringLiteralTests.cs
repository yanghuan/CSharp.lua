using System;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp11;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "Raw String Literals - {0}")]
public sealed class RawStringLiteralTests {
    [Test]
    public static void TestSingleLineRawString() {
        string s = """hello "world" """;
        Assert.AreEqual("hello \"world\" ", s);
    }

    [Test]
    public static void TestMultiLineRawString() {
        string s = """
            line 1
            line 2
            """;
        Assert.AreEqual("line 1\nline 2", s.Replace("\r\n", "\n"));
    }

    [Test]
    public static void TestInterpolatedRawString() {
        string name = "Lua";
        int ver = 54;
        string s = $"""
            Name: {name}
            Ver: {ver}
            """;
        Assert.AreEqual("Name: Lua\nVer: 54", s.Replace("\r\n", "\n"));
    }

    [Test]
    public static void TestUtf8StringLiteral() {
        ReadOnlySpan<byte> bytes = "abc"u8;
        Assert.AreEqual(3, bytes.Length);
        Assert.AreEqual((byte)97, bytes[0]);
        Assert.AreEqual((byte)98, bytes[1]);
        Assert.AreEqual((byte)99, bytes[2]);
        Assert.False(bytes.IsEmpty);

        ReadOnlySpan<byte> empty = ""u8;
        Assert.AreEqual(0, empty.Length);
        Assert.True(empty.IsEmpty);

        var slice = bytes.Slice(1, 2);
        Assert.AreEqual(2, slice.Length);
        Assert.AreEqual((byte)98, slice[0]);
        Assert.AreEqual((byte)99, slice[1]);

        byte[] arr = bytes.ToArray();
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual((byte)97, arr[0]);
        Assert.AreEqual((byte)98, arr[1]);
        Assert.AreEqual((byte)99, arr[2]);

        ReadOnlySpan<byte> multiByte = "你好"u8;
        Assert.AreEqual(6, multiByte.Length);
        Assert.AreEqual((byte)0xE4, multiByte[0]);
        Assert.AreEqual((byte)0xBD, multiByte[1]);
        Assert.AreEqual((byte)0xA0, multiByte[2]);
        Assert.AreEqual((byte)0xE5, multiByte[3]);
        Assert.AreEqual((byte)0xA5, multiByte[4]);
        Assert.AreEqual((byte)0xBD, multiByte[5]);

        ReadOnlySpan<byte> raw = """hello "world" """u8;
        Assert.AreEqual(14, raw.Length);
        Assert.AreEqual((byte)'h', raw[0]);

        int sum = 0;
        foreach (byte b in bytes) {
            sum += b;
        }
        Assert.AreEqual(97 + 98 + 99, sum);
    }
}
