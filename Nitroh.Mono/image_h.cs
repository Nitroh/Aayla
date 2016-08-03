using System.Runtime.InteropServices;
using pointer = System.UInt32;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

// Source: //https://github.com/mono/mono/blob/master/mono/metadata/image.h

namespace Nitroh.Mono
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoAssemblyName
    {
        internal pointer name;
        internal pointer culture;
        internal pointer hash_value;
        internal pointer public_key;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        internal byte[] public_key_tokens;
        internal uint hash_alg;
        internal uint hash_len;
        internal uint flags;
        internal ushort major;
        internal ushort minor;
        internal ushort build;
        internal ushort revision;
    }
}