using System;

namespace CSharpLua
{
    /// <summary>
    /// Indicates that the member is natively available in lua, and therefore its namespace and class prefix should be omitted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
    public class NativeLuaMemberAttribute : Attribute
    {
    }
}
