using System;
using System.Collections.Generic;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp8;

[Category(Constants.MODULE_BASIC_CSHARP)]
[TestFixture(TestNameFormat = "Default Interface Methods - {0}")]
public sealed class TestDefaultInterfaceMethod {
    interface IA {
        int M() { return 0; }
    }

    class A1 : IA {
    }

    [Test]
    public static void Test1() {
       IA a = new A1();
       Assert.AreEqual(a.M(), 0); 
    }   
}