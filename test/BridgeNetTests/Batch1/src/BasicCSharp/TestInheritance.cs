using Bridge.Test.NUnit;
using System;

namespace Bridge.ClientTest.BasicCSharp
{
    [Category(Constants.MODULE_BASIC_CSHARP)]
    [TestFixture(TestNameFormat = "Static overloads - {0}")]
    public class TestInheritance
    {
        private class A
        {
            public int X
            {
                get;
                set;
            }

            public A(int x)
            {
                this.X = x;
            }

            public int HandleNumber(int i)
            {
                return i;
            }

            public string HandleString(string s)
            {
                return s;
            }
        }

        private class B : A
        {
            public int Y
            {
                get;
                set;
            }

            public B(int x, int y)
                : base(x)
            {
                this.Y = y;
            }

            public new int HandleNumber(int i)
            {
                return i * 100;
            }
        }

        [Test(ExpectedCount = 4)]
        public static void TestA()
        {
            var a = new A(10);

            Assert.True(a != null, "Instance of A created");
            Assert.AreEqual(10, a.X, "a.X = 10");
            Assert.AreEqual(100, a.HandleNumber(100), "a.HandleNumber(100) = 100");
            Assert.AreEqual("Hundred", a.HandleString("Hundred"), "a.HandleString('Hundred') = 'Hundred'");
        }

        [Test(ExpectedCount = 5)]
        public static void TestB()
        {
            var b = new B(10, 20);

            Assert.True(b != null, "Instance of B created");
            Assert.AreEqual(10, b.X, "b.X = 10");
            Assert.AreEqual(20, b.Y, "b.Y = 20");
            Assert.AreEqual(100, b.HandleNumber(1), "b.HandleNumber(1) = 100");
            Assert.AreEqual("Hundred", b.HandleString("Hundred"), "b.HandleString('Hundred') = 'Hundred'");
        }

        [Test(ExpectedCount = 4)]
        public static void TestAB()
        {
            A b = new B(10, 20);

            Assert.True(b != null, "Instance of B created as A type");
            Assert.AreEqual(10, b.X, "b.X = 10");
            Assert.AreEqual(10, b.HandleNumber(10), "b.HandleNumber(10) = 10");
            Assert.AreEqual("Hundred", b.HandleString("Hundred"), "b.HandleString('Hundred') = 'Hundred'");
        }

        private class Counter
        {
            private int count;

            private int Total { get; set; }

            private event Action Changed;

            public int Count
            {
                get
                {
                    return count;
                }
            }

            public int BaseTotal
            {
                get
                {
                    return Total;
                }
            }

            public void Add()
            {
                count++;
                Total += 10;
            }

            public void ListenBase(Action action)
            {
                Changed += action;
            }

            public void RaiseBase()
            {
                Changed?.Invoke();
            }
        }

        private class OwnCounter : Counter
        {
            private int count = 100;

            public int Total { get; set; } = 7;

            public event Action Changed;

            public int Own
            {
                get
                {
                    return count;
                }
            }

            public void RaiseOwn()
            {
                Changed?.Invoke();
            }
        }

        private class MiddleCounter : Counter
        {
        }

        private class DeeperCounter : MiddleCounter
        {
            private int count = 300;

            public int Own
            {
                get
                {
                    return count;
                }
            }
        }

        [Test]
        public static void TestPrivateBaseFieldsKeepTheirValues()
        {
            var c = new OwnCounter();
            c.Add();
            Assert.AreEqual(1, c.Count, "base count");
            Assert.AreEqual(100, c.Own, "derived count");
            Assert.AreEqual(10, c.BaseTotal, "base Total");
            Assert.AreEqual(7, c.Total, "derived Total");

            int baseCalls = 0, ownCalls = 0;
            c.ListenBase(() => baseCalls++);
            c.Changed += () => ownCalls++;
            c.RaiseBase();
            Assert.AreEqual(1, baseCalls, "base Changed raised");
            Assert.AreEqual(0, ownCalls, "derived Changed not raised");
            c.RaiseOwn();
            Assert.AreEqual(1, baseCalls, "base Changed not raised again");
            Assert.AreEqual(1, ownCalls, "derived Changed raised");
        }

        [Test]
        public static void TestPrivateFieldOfABaseTwoLevelsUp()
        {
            var c = new DeeperCounter();
            c.Add();
            Assert.AreEqual(1, c.Count, "base count");
            Assert.AreEqual(300, c.Own, "derived count");
        }
    }
}