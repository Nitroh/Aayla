using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Nitroh.Mono.Internals;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoClassEx
    {
        private readonly PortableExecutable _executable;

        internal MonoClass MonoClass { get; }

        internal MonoClassEx(MonoClass monoClass, PortableExecutable executable)
        {
            _executable = executable;
            MonoClass = monoClass;
        }

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) _name = _executable.ReadString(MonoClass.name, false);
                return _name;
            }
        }

        private string _namespace;
        public string Namespace
        {
            get
            {
                if(string.IsNullOrEmpty(_namespace)) _namespace = _executable.ReadString(MonoClass.name_space, false);
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
                var monoRti = _executable.ReadStruct<MonoClassRuntimeInfo>(MonoClass.runtime_info, false);
                _vTable = _executable.ReadStruct<MonoVTable>(monoRti.domain_vtables, false);
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
                        _fields.Add(_executable.ReadStruct<MonoClassField>(offset, false));
                    }
                }
                return _fields.Select(x => new MonoClassFieldEx(x, _executable));
            }
        }
    }
}
