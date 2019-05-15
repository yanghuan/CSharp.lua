using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Test.NUnit {
  /// <summary>
  /// This attribute is used inside a <c>TestFixture</c> to provide a common set of functions that are performed just before each test method is called.
  /// SetUp methods may be either static or instance methods and you may define more than one of them in a fixture.
  /// If a SetUp method fails or throws an exception, the test is not executed and a failure or error is reported.
  /// </summary>
  public sealed class SetUpAttribute : Attribute {
  }
}
