using System.Collections.Generic;
using System.Linq;
using Nitroh.Mono.Internals;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoObject
    {
        private readonly uint _objPointer;
        private readonly PortableExecutable _executable;

        private readonly List<MonoObjectField> _fields;

        public MonoObject(uint objPointer, PortableExecutable executable)
        {
            _objPointer = objPointer;
            _executable = executable;
            _fields = Class?.Fields.Select(x => new MonoObjectField(x, executable, objPointer)).ToList() ?? new List<MonoObjectField>();
        }

        private MonoClassEx _class;
        public MonoClassEx Class
        {
            get
            {
                if (_class != null) return _class;
                //TODO: Live data?
                var vtable = _executable.ReadUInt(_objPointer, false);
                var classPointer = _executable.ReadUInt(vtable, false);
                var monoClass = _executable.ReadStruct<MonoClass>(classPointer, false);
                _class = new MonoClassEx(monoClass, _executable);
                return _class;
            }
        }

        public MonoObjectField this[string key] => _fields.FirstOrDefault(x => x.Name == key);
    }
}