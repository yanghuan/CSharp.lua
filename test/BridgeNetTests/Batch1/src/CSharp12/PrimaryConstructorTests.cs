using System;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp12;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "Primary Constructors - {0}")]
public sealed class PrimaryConstructorTests {
    private class Person(string name, int age) {
        public string Name => name;
        public int Age => age;

        public string Greet() => $"Hello, {name}, age {age}";
    }

    private class Base(int a) {
        public int A => a;
    }

    private class Derived(int a, int b) : Base(a) {
        public int B => b;
        public int Sum() => A + b;
    }

    private struct Point(int x, int y) {
        public int X => x;
        public int Y => y;
        public int DistanceSq() => x * x + y * y;
    }

    [Test]
    public static void TestClassPrimaryConstructor() {
        var p = new Person("Alice", 30);
        Assert.AreEqual("Alice", p.Name);
        Assert.AreEqual(30, p.Age);
        Assert.AreEqual("Hello, Alice, age 30", p.Greet());
    }

    [Test]
    public static void TestInheritanceWithPrimaryConstructor() {
        var d = new Derived(10, 20);
        Assert.AreEqual(10, d.A);
        Assert.AreEqual(20, d.B);
        Assert.AreEqual(30, d.Sum());
    }

    [Test]
    public static void TestStructPrimaryConstructor() {
        var pt = new Point(3, 4);
        Assert.AreEqual(3, pt.X);
        Assert.AreEqual(4, pt.Y);
        Assert.AreEqual(25, pt.DistanceSq());
    }
}
