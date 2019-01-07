using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests
{
    public class GenericClass<T1, T2>
    {
        public GenericClass(T1 arg1, T2 arg2)
        {
            Console.Write(arg1);
        }

        public GenericClass(T1 arg1)
        {
            Console.Write(arg1);
        }

        public GenericClass(string arg1)
        {
            Console.Write(arg1);
        }

        public GenericClass(int arg1)
        {
            Console.Write(arg1);
        }
        public GenericClass(int ar, T2 arg1, T2 arg2)
        {
            Console.Write(arg1);
        }
        public GenericClass(T2 arg1, T1 arg2)
        {
            Console.Write(arg1);
        }
        public GenericClass(T2 arg1, T2 arg2)
        {
            Console.Write(arg1);
        }
        public GenericClass(string ar, T2 arg1, T2 arg2)
        {
            Console.Write(arg1);
        }
        public GenericClass(bool arg1)
        {
            Console.Write(arg1);
        }
        public GenericClass(object arg1)
        {
            Console.Write(arg1);
        }
        public static T1 TestStaticGenericMethod(T1 arg1, T2 arg2)
        {
            Console.Write(arg1);
            Console.Write(arg2);
            return arg1;
        }

        public T1 TestGenericMethod(T1 arg1, T2 arg2)
        {
            Console.Write(arg1);
            Console.Write(arg2);
            return arg1;
        }

        public T1 TestGenericMethod(T1 arg1)
        {
            Console.Write(arg1);
            return arg1;
        }

        public string TestGenericMethod(string arg1)
        {
            Console.Write(arg1);
            return arg1;
        }

        public string TestGenericMethod(int arg1)
        {
            Console.Write(arg1);
            return "";
        }
        public string TestGenericMethod(T1 arg1, T1 arg2)
        {
            Console.Write(arg1);
            return "";
        }
        public string TestGenericMethod(int ar, T2 arg1, T2 arg2)
        {
            Console.Write(arg1);
            return "";
        }
        public string TestGenericMethod(T2 arg1, T1 arg2)
        {
            Console.Write(arg1);
            return "";
        }
        public string TestGenericMethod(T2 arg1, T2 arg2)
        {
            Console.Write(arg1);
            return "";
        }
        public string TestGenericMethod(string ar , T2 arg1, T2 arg2)
        {
            Console.Write(arg1);
            return "";
        }
        public void TestGenericMethod(bool arg1)
        {
            Console.Write(arg1);
        }
        public void TestGenericMethod(object arg1)
        {
            Console.Write(arg1);
        }
        public GenericClass<T1, T2> TestReturnGeneric2<T1, T2>(T1 arg1, T2 arg2)
        {
            Console.Write(arg1);
            Console.Write(arg2);
            var obj = new GenericClass<T1, T2>(arg1, arg2);
            obj.TestGenericMethod(arg1, arg2);
            return obj;
        }
        public T1 TestReturnGeneric2<T1>(T1 arg1)
        {
            Console.Write(arg1);
            return arg1;
        }
        public static GenericClass<T1, T2> TestReturnGeneric2<T3, T4>(T1 arg1, T2 arg2, T2 arg3)
        {
            Console.Write(arg1);
            Console.Write(arg2);
            var obj = new GenericClass<T1, T2>(arg1, arg2);
            obj.TestGenericMethod(arg1, arg2);
            return obj;
        }
        public static  T1 TestReturnGeneric2<T1>(T1 arg1, T1 arg2)
        {
            Console.Write(arg1);
            return arg1;
        }

    }

    public class GenericMethod
    {
 
        public static T1 TestStaticGenericMethod<T1, T2>(T1 arg1, T2 arg2)
        {
            Console.Write(arg1);
            Console.Write(arg2);
            return arg1;
        }

        public T1 TestGenericMethod<T1,T2>(T1 arg1 , T2 arg2)
        {
            Console.Write(arg1);
            Console.Write(arg2);
            return arg1;
        }

        public GenericClass<string, int> TestReturnGeneric1(string arg1, int arg2)
        {
            var cls2 = new GenericClass<TestCases.ClassA, GenericMethod>(new TestCases.ClassA(), new GenericMethod());

            Console.Write(arg1);
            Console.Write(arg2);
            return new GenericClass<string, int>(arg1);
        }
        public GenericClass<T1, T2> TestReturnGeneric2<T1,T2>(T1 arg1, T2 arg2)
        {
            Console.Write(arg1);
            Console.Write(arg2);
            var obj =  new GenericClass<T1, T2>(arg1 , arg2);
            obj.TestGenericMethod(arg1, arg2);
            return obj;
        }
    }


}
