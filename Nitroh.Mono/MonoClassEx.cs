using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Nitroh.Mono.Internals;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoClassEx
    {
        private readonly IntPtr _processHandle;

        internal MonoClass MonoClass { get; }

        internal MonoClassEx(MonoClass monoClass, IntPtr processHandle)
        {
            MonoClass = monoClass;
            _processHandle = processHandle;
        }

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) _name = WindowsHelper.ReadString(_processHandle, MonoClass.name);
                return _name;
            }
        }

        private string _namespace;
        public string Namespace
        {
            get
            {
                if(string.IsNullOrEmpty(_namespace)) _namespace = WindowsHelper.ReadString(_processHandle, MonoClass.name_space);
                return _namespace;
            }
        }

        private int _fieldCount;
        public int FieldCount
        {
            get
            {
                if (_fieldCount <= 0) _fieldCount = (int)MonoClass.field_count;
                return _fieldCount;
            }
        }

        private MonoVTable _vTable;
        internal MonoVTable VTable
        {
            get
            {
                if(_vTable.interface_bitmap != 0) return _vTable;
                var monoRti = WindowsHelper.ReadStruct<MonoClassRuntimeInfo>(_processHandle, MonoClass.runtime_info);
                _vTable = WindowsHelper.ReadStruct<MonoVTable>(_processHandle, monoRti.domain_vtables);
                return _vTable;
            }
        }
        
        private List<MonoClassField> _fields;

        public IEnumerable<MonoClassFieldEx> Fields
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_fields == null || _fields.Count == 0)
                {
                    _fields = new List<MonoClassField>();
                    for (var index = 0; index < FieldCount; index++)
                    {
                        var offset = MonoClass.fields + Marshal.SizeOf(typeof(MonoClassField)) * index;
                        _fields.Add(WindowsHelper.ReadStruct<MonoClassField>(_processHandle, offset));
                    }
                }
                return _fields.Select(x => new MonoClassFieldEx(x, _processHandle));
            }
        }
    }
}
