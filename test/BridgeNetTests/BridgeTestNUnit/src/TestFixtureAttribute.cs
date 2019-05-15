using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Test.NUnit {
  /// <summary>
  /// This is the attribute that marks a class that contains tests and, optionally, setup or teardown methods.
  /// There are a few restrictions on a class that is used as a test fixture.
  ///   - It must be a publicly exported type.
  ///   - It must not be abstract.
  ///   - It must have a default constructor.
  /// </summary>
  [System.AttributeUsageAttribute(System.AttributeTargets.Class, AllowMultiple = false)]
  public sealed class TestFixtureAttribute : Attribute {
    /// <summary>
    /// Test name that will be applyied for methods marked by [Test] that do not have Name specified.
    /// There is only one parameter that can be formatted - {0} that is a test method name
    /// </summary>
    public string TestNameFormat { get; set; }
  }
}
