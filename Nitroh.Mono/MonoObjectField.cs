using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nitroh.Mono.Internals;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoObjectField
    {
        private readonly MonoClassFieldEx _field;
        private readonly PortableExecutable _executable;
        private readonly long _baseAddress;

        public MonoObjectField(MonoClassFieldEx field, PortableExecutable executable, long baseAddress)
        {
            _field = field;
            _executable = executable;
            _baseAddress = baseAddress;
        }

        public string Name => _field.Name;

        private bool Valid => _executable.Running && _baseAddress != 0;

        #region Helpers
        private int? ReadInt() => _executable.ReadInt(_baseAddress + _field.Offset, true);
        private bool? ReadBool() => _executable.ReadByte(_baseAddress + _field.Offset, true) != 0;
        private long? ReadPointer() => _executable.ReadUInt(_baseAddress + _field.Offset, true);
        private string ReadString()
        {
            var arrayPointer = ReadPointer();
            if (arrayPointer == null || arrayPointer.Value == 0) return string.Empty;

            var monoString = _executable.ReadStruct<MonoString>(arrayPointer.Value, true);
            if (monoString.length == 0) return string.Empty;

            var stringSize = monoString.length*2; //UTF-16

            var pointerBase = arrayPointer.Value + Marshal.SizeOf(typeof(MonoString));
            byte[] buffer;
            var valid = _executable.GetMemory(pointerBase, stringSize, out buffer, true);
            return valid ? Encoding.Unicode.GetString(buffer) : string.Empty;
        }
        private MonoObject[] ReadArray(bool valueArray)
        {
            var arrayPointer = ReadPointer();
            if (arrayPointer == null || arrayPointer.Value == 0) return null;
            var result = new List<MonoObject>();

            var monoArray = _executable.ReadStruct<MonoArray>(arrayPointer.Value, true);

            var vtable = _executable.ReadStruct<MonoVTable>(monoArray.vtable, true);
            var arrayMonoClass = _executable.ReadStruct<MonoClass>(vtable.klass, true);
            var arrayClass = new MonoClassEx(arrayMonoClass, _executable);

            var pointerBase = arrayPointer.Value + Marshal.SizeOf(typeof(MonoArray));
            for (var index = 0; index < monoArray.max_length; index++)
            {
                var pointer = pointerBase + index * arrayClass.Size;
                if(!valueArray) pointer = _executable.ReadUInt(pointer, true);
                if (pointer == 0) continue;
                var monoObject = new MonoObject(pointer, _executable);
                result.Add(monoObject);
            }

            return result.ToArray();
        }
        private Dictionary<string, MonoObject> ReadObjectArray() => ReadArray(false).ToDictionary(x => x.Class.Name, y => y);
        #endregion

        #region TryGetValue
        public int? TryGetInt() => Valid ? ReadInt() : null;
        public int? TryGetEnum() => Valid ? ReadInt() : null;
        public bool? TryGetBool() => Valid ? ReadBool() : null;
        public long? TryGetPointer() => Valid ? ReadPointer() : null;
        public string TryGetString() => Valid ? ReadString() : string.Empty;
        public MonoObject TryGetMonoObject() => Valid ? new MonoObject(_baseAddress + _field.Offset, _executable) : new MonoObject(0, null);
        public MonoObject TryGetMonoObjectFromPointer() => Valid ? new MonoObject(TryGetPointer() ?? 0, _executable) : null;
        public MonoObject[] TryGetArrayOfValues() => Valid ? ReadArray(true) : null;
        public MonoObject[] TryGetArrayOfPointers() => Valid ? ReadArray(false) : null;
        public Dictionary<string, MonoObject> TryGetArrayOfObjects() => Valid ? ReadObjectArray() : null;
        #endregion
    }
}