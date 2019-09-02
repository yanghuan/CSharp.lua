using System;

namespace CSharpLua {
  /// <summary>
  /// Indicates that the class is a container for members with the <see cref="NativeLuaMemberAttribute"/>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class NativeLuaMemberContainerAttribute : Attribute {
  }
}
