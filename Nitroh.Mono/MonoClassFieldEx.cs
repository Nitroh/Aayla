using System;
using Nitroh.Mono.Internals;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoClassFieldEx
    {
        private readonly PortableExecutable _executable;

        internal MonoClassField ClassField { get; }

        internal MonoClassFieldEx(MonoClassField classField, PortableExecutable executable)
        {
            ClassField = classField;
            _executable = executable;
        }

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) _name = _executable.ReadString(ClassField.name, false);
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
                var monoType = _executable.ReadStruct<MonoType>(ClassField.type, false);
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
                var monoClass = _executable.ReadStruct<MonoClass>(ClassField.parent, false);
                _parent = new MonoClassEx(monoClass, _executable);
                return _parent;
            }
        }

        public MonoObject TryGetFieldAsInstance()
        {
            try
            {
                var pointer = _executable.ReadUInt(Parent.VTable.interface_bitmap + Offset, false);
                return pointer == 0 ? null : new MonoObject(pointer, _executable);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}