using System;

namespace Bridge
{
    /// <summary>
    /// Applies to interface if it's implementation is done outside Bridge type system (class implementation doesn't provide aliases for interface members implementations)
    /// </summary>
    [NonScriptable]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Interface)]
    public sealed class ExternalInterfaceAttribute : Attribute
    {
        public ExternalInterfaceAttribute() { }
        public ExternalInterfaceAttribute(bool nativeImplementation) { }

        public bool IsVirtual
        {
            get; set;
        }
    }
}
