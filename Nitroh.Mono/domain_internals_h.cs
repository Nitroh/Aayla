using System.Runtime.InteropServices;
using pointer = System.UInt32;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

// Source: https://github.com/mono/mono/blob/master/mono/metadata/domain-internals.h

namespace Nitroh.Mono
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
        //TODO: finish this
    }
}
