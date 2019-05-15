using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Test.NUnit {
  /// <summary>
  /// This attribute is used inside a <c>TestFixture</c> to provide a common set of functions that are performed after each test method.
  /// <c>TearDown</c> methods may be either static or instance methods and you may define more than one of them in a fixture.
  /// The <c>TearDown</c> method is guaranteed to run. It will not run if a <c>SetUp</c> method fails or throws an exception.
  /// </summary>
  public sealed class TearDownAttribute : Attribute {
  }
}
