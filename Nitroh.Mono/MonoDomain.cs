using System.Runtime.InteropServices;

namespace Nitroh.Mono
{
    /// <summary>
    /// Source is https://github.com/mono/mono/blob/master/mono/metadata/domain-internals.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct _MonoDomain
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] lock_;
        public uint mp;
        public uint code_mp;
        public uint setup;
        public uint domain;
        public uint default_context;
        public uint out_of_memory_ex;
        public uint null_reference_ex;
        public uint stack_overflow_ex;
        public uint typeof_void;
        public uint ephemeron_tombstone;
        public uint empty_types;
        public uint env;
        public uint ldstr_table;
        public uint type_hash;
        public uint refobject_hash;
        public uint static_data_array;
        public uint type_init_exception_hash;
        public uint delegate_hash_table;
        public uint state;
        public uint domain_id;
        public uint shadow_serial;
        public uint inet_family_hint;
        public uint domain_assemblies;
    }
}