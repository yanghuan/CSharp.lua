using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

public class TestCase : System.Attribute
{

}

namespace ILRuntimeTest.TestBase
{
    public class StaticTestUnit
    {
        public static void Run()
        {
            var types = Assembly.GetExecutingAssembly().GetExportedTypes().OrderBy(v=>v.FullName);
            foreach(var type in types)
            {
                var members = type.GetMethods().OrderBy(v=>v.Name);
                bool started = false;

                foreach (var member in members)
                {
                    if (member.IsDefined(typeof(TestCase), false))
                    {
                        if(!started)
                        {
                            started = true;
                            Console.WriteLine("TestClass Start : {0}", type.Name);
                        }
                        Console.WriteLine("TestCase : {0}", member.Name);
                        member.Invoke(null, new object[] { });
                    }
                }

                if (started)
                {
                    Console.WriteLine("TestClass Finish : {0}", type.Name);
                }
            }
        }
    }
}
