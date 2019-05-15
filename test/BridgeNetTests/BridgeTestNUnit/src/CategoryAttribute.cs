using System;

namespace Bridge.Test.NUnit {
  /// <summary>
  /// Either individual tests or fixtures may be identified as belonging to a particular category.
  /// <c>[Category]</c> attached to method has priority over <c>[Category]</c> attached test class (fixture)
  /// </summary>
  [System.AttributeUsageAttribute(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false)]
  public sealed class CategoryAttribute : Attribute {
    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Category name
    /// </summary>
    public CategoryAttribute(string name) {
      Name = name;
    }
  }
}
