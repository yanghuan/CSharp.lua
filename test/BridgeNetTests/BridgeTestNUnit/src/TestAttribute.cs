using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Test.NUnit {
  /// <summary>
  /// The Test attribute is one way of marking a method inside a TestFixture class as a test.
  /// Test methods may have parameters. Multiple sets of arguments cause the creation of multiple tests.
  /// A method marked by <c>[Test]</c>:
  ///   - Must be a public method.
  ///   - Must not be abstract.
  ///   - A parameterized test method must match the parameters provided.
  ///   - May be instance or static.
  ///   - May appear one or more times on a test method.
  /// </summary>
  [System.AttributeUsageAttribute(System.AttributeTargets.Method, AllowMultiple = true)]
  public sealed class TestAttribute : Attribute {
    /// <summary>
		/// Specify how many assertions are expected to run within a test
		/// </summary>
    public int ExpectedCount { get; set; }
    /// <summary>
		/// Provides a name for the test.
		/// If not specified, a name is generated based on the method name and the arguments provided
		/// <see cref="P:Bridge.Test.NUnit.TestFixtureAttribute.TestNameFormat">[TestFixture] TestNameFormat for details.</see>
		/// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Inline data to be used when invoking that method if a test method has parameters
    /// </summary>
    public object[] MethodParameters { get; }

    /// <summary>
    /// Constructor with inline data
    /// </summary>
    /// <param name="methodParameters">Inline data to be used when invoking that method if a test method has parameters</param>
    public TestAttribute(params object[] methodParameters) {
      MethodParameters = methodParameters;
    }
  }
}
