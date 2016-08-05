using System;
using System.Collections.Generic;
using System.Linq;
using Nitroh.Mono.Internals;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoObject
    {
        private readonly uint _objPointer;
        private readonly IntPtr _processHandle;

        private readonly List<MonoObjectField> _fields;

        public MonoObject(uint objPointer, IntPtr processHandle)
        {
            _objPointer = objPointer;
            _processHandle = processHandle;
            _fields = Class?.Fields.Select(x => new MonoObjectField(x, processHandle, objPointer)).ToList() ?? new List<MonoObjectField>();
        }

        private MonoClassEx _class;
        public MonoClassEx Class
        {
            get
            {
                if (_class != null) return _class;
                var vtable = WindowsHelper.ReadUInt(_processHandle, _objPointer);
                var classPointer = WindowsHelper.ReadUInt(_processHandle, vtable);
                var monoClass = WindowsHelper.ReadStruct<MonoClass>(_processHandle, classPointer);
                _class = new MonoClassEx(monoClass, _processHandle);
                return _class;
            }
        }

        public MonoObjectField this[string key] => _fields.FirstOrDefault(x => x.Name == key);
    }
}