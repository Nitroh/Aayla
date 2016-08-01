using System.Runtime.InteropServices;

namespace Nitroh.Mono
{
    /// <summary>
    /// Source is https://github.com/mono/mono/blob/master/mono/metadata/metadata-internals.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoAssemblyName
    {
        internal uint name;
        internal uint culture;
        internal uint hash_value;
        internal uint public_key;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] public_key_token;
        internal uint hash_alg;
        internal uint hash_len;
        internal uint flags;
        internal ushort major;
        internal ushort minor;
        internal ushort build;
        internal ushort revision;
        internal ushort arch;
    }

    /// <summary>
    /// Source is https://github.com/mono/mono/blob/master/mono/metadata/metadata-internals.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoAssembly
    {
        internal int ref_count;
        internal uint basedir;
        internal MonoAssemblyName aname;
        internal uint image;
    }
}