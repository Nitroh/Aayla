using System;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoObjectField
    {
        private readonly MonoClassFieldEx _field;
        private readonly IntPtr _processHandle;
        private readonly uint _baseAddress;

        public MonoObjectField(MonoClassFieldEx field, IntPtr processHandle, uint baseAddress)
        {
            _field = field;
            _processHandle = processHandle;
            _baseAddress = baseAddress;
        }

        public string Name => _field.Name;

        private bool Valid => _processHandle.ToInt64() != 0 && _baseAddress != 0;

        #region WindowsHelper
        private int? ReadInt() => WindowsHelper.ReadInt(_processHandle, _baseAddress + _field.Offset);
        private bool? ReadBool() => WindowsHelper.ReadByte(_processHandle, _baseAddress + _field.Offset) != 0;
        #endregion

        #region TryGetValue
        public int? TryGetEnum() => Valid ? ReadInt() : null;
        public bool? TryGetBool() => Valid ? ReadBool() : null;
        #endregion
    }
}