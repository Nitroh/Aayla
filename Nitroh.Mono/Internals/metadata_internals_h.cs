using System.Runtime.InteropServices;
using pointer = System.UInt32;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

// Source: https://github.com/Unity-Technologies/mono/blob/unity-staging/mono/metadata/metadata-internals.h

namespace Nitroh.Mono.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoAssembly
    {
        internal int ref_count; //0 {4}
        internal pointer basedir; //4 {4}
        internal MonoAssemblyName aname; //8 {56}
        internal pointer image; //64 {4}
        internal pointer friend_assembly_names;
        internal ushort friend_assembly_names_inited;
        internal ushort in_gac;
        internal ushort dynamic;
        internal ushort corlib_internal;
        internal int ref_only;
        //internal uint bitfields; //TODO: actual bitfields?
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoStreamHeader
    {
        internal pointer data;
        internal uint size;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoImage
    {
        internal int ref_count;
        internal pointer raw_data_handle;
        internal pointer raw_data;
        internal uint raw_data_len;
        internal ushort bitfields; //TODO: bitfields
        internal pointer name;
        internal pointer assembly_name;
        internal pointer module_name;
        internal pointer version;
        internal short md_version_major;
        internal short md_version_minor;
        internal pointer guid;
        internal pointer image_info;
        internal pointer mempool;
        internal pointer raw_metadata;
        internal MonoStreamHeader heap_strings;
        internal MonoStreamHeader heap_us;
        internal MonoStreamHeader heap_blob;
        internal MonoStreamHeader heap_guid;
        internal MonoStreamHeader heap_tables;
        internal pointer tables_base;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 45)]
        internal MonoTableInfo[] tables;
        internal pointer references;
        internal pointer modules;
        internal uint module_count;
        internal pointer modules_loaded;
        internal pointer files;
        internal pointer aot_module;
        internal pointer assembly;
        internal pointer method_cache;
        internal MonoInternalHashTable class_cache;
        internal pointer methodref_cache;
        internal pointer field_cache;
        internal pointer typespec_cache;
        internal pointer memberref_signatures;
        internal pointer helper_signatures;
        internal pointer method_signatures;
        internal pointer name_cache;
        internal pointer array_cache;
        internal pointer ptr_cache;
        internal pointer szarray_cache;
        internal CriticalSection szarray_cache_lock;
        internal pointer delegate_begin_invoke_cache;
        internal pointer delegate_end_invoke_cache;
        internal pointer delegate_invoke_cache;
        internal pointer runtime_invoke_cache;
        internal pointer delegate_abstract_invoke_cache;
        internal pointer runtime_invoke_direct_cache;
        internal pointer runtime_invoke_vcall_cache;
        internal pointer managed_wrapper_cache;
        internal pointer native_wrapper_cache;
        internal pointer native_wrapper_aot_cache;
        internal pointer remoting_invoke_cache;
        internal pointer synchronized_cache;
        internal pointer unbox_wrapper_cache;
        internal pointer cominterop_invoke_cache;
        internal pointer cominterop_wrapper_cache;
        internal pointer thunk_invoke_cache;
        internal pointer ldfld_wrapper_cache;
        internal pointer ldflda_wrapper_cache;
        internal pointer stfld_wrapper_cache;
        internal pointer isinst_cache;
        internal pointer castclass_cache;
        internal pointer proxy_isinst_cache;
        internal pointer rgctx_template_hash;
        internal pointer generic_class_cache;
        internal pointer property_hash;
        internal pointer reflection_info;
        internal pointer user_info;
        internal pointer dll_map;
        internal pointer interface_bitset;
        internal pointer reflection_info_unregister_classes;
        internal CriticalSection lock_;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoTableInfo
    {
        internal pointer base_;
        internal uint rowbitfields; //TODO: bitfields
        internal uint size_bitfield;
    }
}