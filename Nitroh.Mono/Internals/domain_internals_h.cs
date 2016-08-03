using System.Runtime.InteropServices;
using pointer = System.UInt32;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

// Source: https://github.com/mono/mono/blob/master/mono/metadata/domain-internals.h

namespace Nitroh.Mono.Internals
{
    //See winnt.h
    [StructLayout(LayoutKind.Sequential)]
    internal struct CriticalSection
    {
        internal pointer DebugInfo;
        internal pointer LockCount;
        internal pointer RecursionCount;
        internal pointer OwningThread;
        internal pointer LockSemaphore;
        internal pointer SpinCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoDomain
    {
        internal CriticalSection lock_;
        internal pointer mp;
        internal pointer code_mp;
        internal pointer setup;
        internal pointer domain;
        internal pointer default_context;
        internal pointer out_of_memory_ex;
        internal pointer null_reference_ex;
        internal pointer stack_overflow_ex;
        internal pointer divide_by_zero_ex;
        internal pointer typeof_void;
        internal pointer env;
        internal pointer ldstr_table;
        internal pointer type_hash;
        internal pointer refobject_hash;
        internal pointer static_data_array;
        internal pointer type_init_exception_hash;
        internal pointer delegate_hash_table;
        internal uint state;
        internal int domain_id;
        internal int shadow_serial;
        internal char inet_family_hint;
        internal pointer domain_assemblies;
        internal pointer entry_assembly;
        internal pointer friendly_name;
        internal pointer class_vtable_hash;
        internal pointer proxy_vtable_hash;
        internal MonoInternalHashTable jit_code_hash;
        internal CriticalSection jit_code_hash_lock;
        internal int num_jit_info_tables;
        internal pointer jit_info_table;
        internal pointer jit_info_free_queue;
        internal pointer search_path;
        internal pointer private_bin_path;
        internal pointer create_proxy_for_type_method;
        internal pointer private_invoke_method;
        internal pointer special_static_fields;
        internal pointer finalizable_objects_hash;
        internal pointer track_resurrection_objects_hash;
        internal pointer track_resurrection_handles_hash;
        internal CriticalSection finalizable_objects_hash_lock;
        internal CriticalSection assemblies_lock;
        internal pointer method_rgctx_hash;
        internal pointer generic_virtual_cases;
        internal pointer thunk_free_lists;
        internal pointer class_custom_attributes;
        internal pointer runtime_info;
        internal int threadpool_jobs;
        internal pointer cleanup_semaphore;
        internal pointer finalize_runtime_invoke;
        internal pointer capture_context_runtime_invoke;
        internal pointer capture_context_method;
        internal pointer socket_assembly;
        internal pointer sockaddr_class;
        internal pointer sockaddr_data_field;
        internal pointer static_data_class_array;
    }
}
