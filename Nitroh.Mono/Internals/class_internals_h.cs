using System.Runtime.InteropServices;
using pointer = System.UInt32;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

// Source: https://github.com/Unity-Technologies/mono/blob/unity-staging/mono/metadata/class-internals.h

namespace Nitroh.Mono.Internals
{
	[StructLayout(LayoutKind.Sequential)]
    internal struct MonoClass
	{
	    internal pointer element_class;
	    internal pointer cast_class;
	    internal pointer supertypes;
	    internal ushort idepth;
	    internal byte rank;
	    internal int instance_size;
        internal uint bitfields1; //TODO: bitfields
        internal byte min_align;
	    internal uint bitfields2; //TODO: bitfields
	    internal byte exception_type;
	    internal pointer parent;
	    internal pointer nested_in;
	    internal pointer image;
		internal pointer name;
	    internal pointer name_space;
	    internal uint type_token;
	    internal int vtable_size;
	    internal ushort interface_count;
	    internal ushort interface_id;
	    internal ushort max_interface_id;
	    internal ushort interface_offsets_count;
	    internal pointer interfaces_packed;
	    internal pointer interface_offsets_packed;
	    internal pointer interface_bitmap;
	    internal pointer interfaces;
	    internal int sizes;
	    internal uint flags;
	    internal uint field_first;
	    internal uint field_count;
	    internal uint method_first;
	    internal uint method_count;
	    internal pointer marshal_info;
	    internal pointer fields;
	    internal pointer methods;
	    internal MonoType this_arg;
	    internal MonoType byval_arg;
	    internal pointer generic_class;
	    internal pointer generic_container;
	    internal pointer refelction_info;
	    internal pointer gc_descr;
	    internal pointer runtime_info;
	    internal pointer next_class_cache;
	    internal pointer vtable;
	    internal pointer ext;
	    internal pointer user_data;
	}

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoClassField
    {
        internal pointer type;
        internal pointer name;
        internal pointer parent;
        internal int offset;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoClassRuntimeInfo
    {
        internal ushort max_domain;
        internal pointer domain_vtables;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoVTable
    {
        internal pointer klass;
        internal pointer gc_descr;
        internal pointer domain;
        internal pointer data;
        internal pointer type;
        internal pointer interface_bitmap;
        internal ushort max_interface_id;
        internal byte rank;
        internal byte bitfields; //TODO: bitfields
        internal uint imt_collisions_bitmap;
        internal pointer runtime_generic_context;
        internal pointer vtable; //TODO: correct?
    }
}