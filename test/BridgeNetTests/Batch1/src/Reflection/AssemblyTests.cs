using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bridge.ClientTest.Batch1.Reflection
{
    [Category(Constants.MODULE_REFLECTION)]
    [TestFixture(TestNameFormat = "Reflection - Assembly {0}")]
    public class AssemblyTests
    {
        [NonScriptable]
        [External]
        public class A1Attribute : Attribute
        {
            public int X
            {
                get; private set;
            }

            public int P
            {
                get; set;
            }

            public A1Attribute()
            {
            }

            public A1Attribute(int x)
            {
                this.X = x;
            }
        }

        public class A2Attribute : Attribute
        {
            public int X
            {
                get; private set;
            }

            public int P
            {
                get; set;
            }

            public A2Attribute()
            {
            }

            public A2Attribute(int x)
            {
                this.X = x;
            }
        }

        public class A3Attribute : Attribute
        {
            public int X
            {
                get; private set;
            }

            public int P
            {
                get; set;
            }

            public A3Attribute()
            {
            }

            public A3Attribute(int x)
            {
                this.X = x;
            }
        }

        private class C
        {
        }

        private class G<T1, T2>
        {
        }

        private string CompilerVersion
        {
            get
            {
                //return AssemblyVersionMarker.GetVersion(AssemblyVersionMarker.VersionType.Compiler);
                return "";
            }
        }

        private string MscorlibName
        {
            get
            {
                return "mscorlib";
            }
        }

        private string MscorlibWithVersion
        {
            get
            {
                //return MscorlibName + ", Version=" + CompilerVersion;
                return MscorlibName;
            }
        }

        private string AssemblyName
        {
            get
            {
                return "Bridge.ClientTest";
            }
        }

        private string AssemblyWithVersion
        {
            get
            {
                //return AssemblyName + ", Version=" + AssemblyVersionMarker.GetVersion(AssemblyVersionMarker.VersionType.CurrentAssembly);
                return AssemblyName;
            }
        }

        [Test]
        public void GetExecutingAssemblyWorks()
        {
            Assert.AreEqual(AssemblyWithVersion, Assembly.GetExecutingAssembly().FullName);
        }

        [Test]
        public void GetAssemblyForTypeWorks()
        {
            Assert.AreEqual(MscorlibWithVersion, Assembly.GetAssembly(typeof(int)).FullName);
            Assert.AreEqual(AssemblyWithVersion, Assembly.GetAssembly(typeof(AssemblyTests)).FullName);
        }

        [Test]
        public void FullNameWorks()
        {
            Assert.AreEqual(MscorlibWithVersion, typeof(int).Assembly.FullName);
            Assert.AreEqual(AssemblyWithVersion, typeof(AssemblyTests).Assembly.FullName);
        }

        [Test]
        public void ToStringWorks()
        {
            Assert.AreEqual(MscorlibWithVersion, typeof(int).Assembly.ToString());
            Assert.AreEqual(AssemblyWithVersion, typeof(AssemblyTests).Assembly.ToString());
        }

        [Test]
        public void GetTypesWorks()
        {
            var types = new List<string>();
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
                types.Add(t.FullName);
            Assert.True(types.Contains(typeof(AssemblyTests).FullName));
        }

        [Test]
        public void GetTypeWorks()
        {
            Assert.True(Assembly.GetExecutingAssembly().GetType(typeof(AssemblyTests).FullName) == typeof(AssemblyTests));
            Assert.True(Assembly.GetExecutingAssembly().GetType(typeof(Dictionary<,>).FullName) == null);
            Assert.True(typeof(int).Assembly.GetType(typeof(Dictionary<,>).FullName) == typeof(Dictionary<,>));
        }

        [Test]
        public void GetTypeWorksWithGenerics()
        {
            Assert.True(Assembly.GetExecutingAssembly().GetType(typeof(G<,>).FullName) == typeof(G<,>));
            Assert.True(Assembly.GetExecutingAssembly().GetType(typeof(G<int, string>).FullName) == typeof(G<int, string>));
        }

        [Test]
        public void AssemblyOfBuiltInTypes()
        {
            Assert.AreEqual(MscorlibWithVersion, typeof(DateTime).Assembly.FullName);
            Assert.AreEqual(MscorlibWithVersion, typeof(double).Assembly.FullName);
            Assert.AreEqual(MscorlibWithVersion, typeof(bool).Assembly.FullName);
            Assert.AreEqual(MscorlibWithVersion, typeof(string).Assembly.FullName);
            Assert.AreEqual(MscorlibWithVersion, typeof(Delegate).Assembly.FullName);
            Assert.AreEqual(MscorlibWithVersion, typeof(int[]).Assembly.FullName);
        }

        [Test]
        public void CreateInstanceWorks()
        {
            Assert.True(typeof(C).Assembly.CreateInstance(typeof(C).FullName) is C, "#1");
            Assert.AreEqual(0, typeof(int).Assembly.CreateInstance(typeof(int).FullName), "#2");
            Assert.True(typeof(C).Assembly.CreateInstance("NonExistentType") == null, "#3");
        }

        [Test]
        public void GetCustomAttributesWorks()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var a in new[] { asm.GetCustomAttributes(true), asm.GetCustomAttributes(false) })
            {
                var a2 = a.Where(x => x is AssemblyAttributes.A2Attribute).ToArray();
                Assert.AreEqual(1, a2.Length);
                Assert.True(((AssemblyAttributes.A2Attribute)a2[0]).X == 64);
                Assert.True(((AssemblyAttributes.A2Attribute)a2[0]).P == 23);

                var a3 = a.Where(x => x is AssemblyAttributes.A3Attribute).ToArray();
                Assert.AreEqual(1, a3.Length);
                Assert.True(((AssemblyAttributes.A3Attribute)a3[0]).X == 15);
                Assert.True(((AssemblyAttributes.A3Attribute)a3[0]).P == 45);
            }

            foreach (var a in new[] { asm.GetCustomAttributes(typeof(AssemblyAttributes.A2Attribute), true), asm.GetCustomAttributes(typeof(AssemblyAttributes.A2Attribute), false) })
            {
                Assert.AreEqual(1, a.Length);
                Assert.True(((AssemblyAttributes.A2Attribute)a[0]).X == 64);
                Assert.True(((AssemblyAttributes.A2Attribute)a[0]).P == 23);
            }
        }

        [Test]
        public void LoadCanReturnReferenceToLoadedAssembly()
        {
            Assert.True(Assembly.Load(AssemblyName) == Assembly.GetExecutingAssembly(), AssemblyName);
            Assert.True(Assembly.Load(MscorlibName) == typeof(int).Assembly, MscorlibName);

            Assert.True(Assembly.Load(AssemblyWithVersion) == Assembly.GetExecutingAssembly(), AssemblyWithVersion);
            Assert.True(Assembly.Load(MscorlibWithVersion) == typeof(int).Assembly, MscorlibWithVersion);
        }

        [Test]
        public void UriBelongsSystem()
        {
            Assert.AreEqual(typeof(System.Uri).Assembly.FullName, "System");
        }
    }
}
