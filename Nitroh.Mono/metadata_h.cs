using System.Runtime.InteropServices;
using pointer = System.UInt32;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

// Source: https://github.com/Unity-Technologies/mono/blob/unity-staging/mono/metadata/metadata.h

namespace Nitroh.Mono
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoType
    {
        internal pointer data;
        internal uint bitfields; //TODO: bitfields
        internal uint modifiers;
    }
}