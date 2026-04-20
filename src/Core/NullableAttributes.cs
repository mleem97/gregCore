// This file provides the necessary attributes for the compiler to support
// nullable reference types and other modern C# features in environments
// where the core library might not have them (like IL2CPP mixing).

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    internal sealed class NullableAttribute : Attribute
    {
        public readonly byte[] NullableFlags;
        public NullableAttribute(byte flag) => NullableFlags = new[] { flag };
        public NullableAttribute(byte[] flags) => NullableFlags = flags;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class NullableContextAttribute : Attribute
    {
        public readonly byte Flag;
        public NullableContextAttribute(byte flag) => Flag = flag;
    }
}
