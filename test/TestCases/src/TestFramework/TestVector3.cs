using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILRuntimeTest.TestFramework
{   
    public struct TestStruct1
    {
        public TestStruct1(int v)
        {
            val = v;
        }
        public int val;
        [TestCase]
        public static TestStruct1 test()
        {
            var a = new TestStruct1();
            return a;
        }
    }

    public struct TestVector3
    {
        public float X, Y, Z;

        public static TestVector3 One = new TestVector3(1, 1, 1);

        public TestVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static TestVector3 One2 { get { return new TestVector3(1, 1, 1); } }

        public void Normalize()
        {
            float d = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            X = X / d;
            Y = Y / d;
            Z = Z / d;
        }

        public static TestVector3 operator +(TestVector3 a, TestVector3 b)
        {
            TestVector3 res = new TestVector3();
            res.X = a.X + b.Y;
            res.Y = a.Y + b.Y;
            res.Z = a.Z + b.Z;

            return res;
        }

        public static TestVector3 operator *(TestVector3 a, float b)
        {
            TestVector3 res = new TestVector3();
            res.X = a.X * b;
            res.Y = a.Y * b;
            res.Z = a.Z * b;

            return res;
        }

        public void Test(out TestVector3 a, out float b)
        {
            b = 123f;
            a = this * 123f;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", X, Y, Z);
        }

#if COMPILE_BUG
        [TestCase]
        public static void Test()
        {
            var a = TestVector3.One * 0.33f;
        }
#endif
    }

    public class TestVectorClass
    {
        public TestVector3 vector = new TestVector3(20f, 10f, 1f);
        #if COMPILE_BUG
        public TestVector3 Vector2 { get; set; } = TestVector3.One * 0.33f;
#endif
    }

    public struct TestVectorStruct
    {
        public int A;
        public TestVectorStruct2 B;
        public TestVector3 C;
    }

    public struct TestVectorStruct2
    {
        public TestVectorStruct3 A;
        public TestVector3 Vector;
    }

    public struct TestVectorStruct3
    {
        public float A;
        public float B;
    }
}
