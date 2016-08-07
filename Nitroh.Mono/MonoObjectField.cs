using Nitroh.Windows;

namespace Nitroh.Mono
{
    public class MonoObjectField
    {
        private readonly MonoClassFieldEx _field;
        private readonly PortableExecutable _executable;
        private readonly uint _baseAddress;

        public MonoObjectField(MonoClassFieldEx field, PortableExecutable executable, uint baseAddress)
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
        #endregion

        #region TryGetValue
        public int? TryGetEnum() => Valid ? ReadInt() : null;
        public bool? TryGetBool() => Valid ? ReadBool() : null;
        #endregion
    }
}