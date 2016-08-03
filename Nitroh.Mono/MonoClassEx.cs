using System;
using Nitroh.Mono.Internals;
using Nitroh.Windows;

namespace Nitroh.Mono
{
    internal class MonoClassEx
    {
        private readonly IntPtr _processHandle;

        internal MonoClass MonoClass { get; }

        internal MonoClassEx(MonoClass monoClass, IntPtr processHandle)
        {
            MonoClass = monoClass;
            _processHandle = processHandle;
        }

        private string _name;
        internal string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) _name = WindowsHelper.ReadString(_processHandle, MonoClass.name);
                return _name;
            }
        }
    }
}
