using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Bridge.Test.NUnit;

namespace Tests {
  class Program {
    private static readonly HashSet<string> ignores_ = new HashSet<string>() {
      "Reflection",
    };

    private static bool IsTestClass(Type type, out CategoryAttribute category, out TestFixtureAttribute testFixture) {
      category = (CategoryAttribute)type.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault();
      testFixture = (TestFixtureAttribute)type.GetCustomAttributes(typeof(TestFixtureAttribute), true).FirstOrDefault();
      return category != null && testFixture != null && !ignores_.Contains(category.Name);
    }

    static void Main(string[] args) {
      var types = Assembly.GetExecutingAssembly().GetExportedTypes();
      foreach (var t in types) {
        if (IsTestClass(t, out var category, out var testFixture)) {
          TestClass(t, category.Name, testFixture.TestNameFormat);
        }
      }
    }

    private static void TestClass(Type type, string category, string testFixture) {
      Console.WriteLine("{0} for {1}", category, type);
      var methods = type.GetMethods();
      object instance = Activator.CreateInstance(type);
      foreach (var method in methods) {
        if (method.IsDefined(typeof(TestAttribute), true)) {
          Console.WriteLine(testFixture ?? "Test - {0}", method.Name);
          method.Invoke(instance, null);
        }
      }
    }
  }
}
