using System.Runtime.InteropServices;
using pointer = System.UInt32;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

// Source: https://github.com/Unity-Technologies/mono/blob/unity-staging/mono/utils/mono-internal-hash.h

namespace Nitroh.Mono.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoInternalHashTable
    {
        internal pointer hash_func;
        internal pointer key_extract;
        internal pointer next_value;
        internal int size;
        internal int num_entries;
        internal pointer table;
    }
}