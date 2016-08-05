using System;
using Nitroh.Mono.Internals;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoClassFieldEx
    {
        private readonly IntPtr _processHandle;

        internal MonoClassField ClassField { get; }

        internal MonoClassFieldEx(MonoClassField classField, IntPtr processHandle)
        {
            ClassField = classField;
            _processHandle = processHandle;
        }

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) _name = WindowsHelper.ReadString(_processHandle, ClassField.name);
                return _name;
            }
        }

        private MonoType _type;
        internal MonoType Type
        {
            get
            {
                if(ClassField.type == 0) return new MonoType();
                if (_type.data != 0) return _type;
                var monoType = WindowsHelper.ReadStruct<MonoType>(_processHandle, ClassField.type);
                _type = monoType;
                return _type;
            }
        }

        public int Offset => ClassField.offset;

        private MonoClassEx _parent;

        public MonoClassEx Parent
        {
            get
            {
                if (ClassField.parent == 0) return null;
                if (_parent != null) return _parent;
                var monoClass = WindowsHelper.ReadStruct<MonoClass>(_processHandle, ClassField.parent);
                _parent = new MonoClassEx(monoClass, _processHandle);
                return _parent;
            }
        }

        public MonoObject TryGetFieldAsInstance()
        {
            try
            {
                var pointer = WindowsHelper.ReadUInt(_processHandle, Parent.VTable.interface_bitmap + Offset);
                return pointer == 0 ? null : new MonoObject(pointer, _processHandle);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}