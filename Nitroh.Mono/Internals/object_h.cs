using System.Runtime.InteropServices;
using pointer = System.UInt32;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

// Source: https://github.com/Unity-Technologies/mono/blob/unity-staging/mono/metadata/object.h

namespace Nitroh.Mono.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoArray
    {
        internal pointer vtable; //From MonoObject
        internal pointer synchronisation; //From MonoObject
        internal pointer bounds;
        internal uint max_length;
        //internal pointer vector; //Array data starts here
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoString
    {
        internal pointer vtable; //From MonoObject
        internal pointer synchronisation; //From MonoObject
        internal int length;
        //internal pointer chars; //Array of chars starts here
    }
}