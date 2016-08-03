using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nitroh.Windows
{
    public class PortableExecutable
    {
        private readonly string _processName;
        private readonly string _moduleName;
        private byte[] _headerMemory;

        public Process Process { get; private set; }

        public long BaseAddress { get; private set; }

        private List<Tuple<string, long>> _functionPointers;
        public IEnumerable<Tuple<string, long>> FunctionPointers => _functionPointers;


        public PortableExecutable(string processName, string moduleName)
        {
            _processName = processName;
            _moduleName = moduleName;
            Reload();
        }

        public void Reload()
        {
            LoadHeaderMemory();
            LoadHeader();
        }

        private void Reset()
        {
            Process = null;
            BaseAddress = 0;
            _functionPointers = new List<Tuple<string, long>>();
            _headerMemory = null;
        }

        private void LoadHeaderMemory()
        {
            Reset();
            Process = Process.GetProcessesByName(_processName).FirstOrDefault();
            var module = Process?.Modules.OfType<ProcessModule>().FirstOrDefault(x => x.ModuleName == _moduleName);
            if (module == null)
            {
                Reset();
                return;
            }
            BaseAddress = module.BaseAddress.ToInt64();
            WindowsHelper.ReadMemory(Process.Handle, BaseAddress, module.ModuleMemorySize, out _headerMemory);
        }

        /// <summary>
        /// Logic taken from http://www.csn.ul.ie/~caolan/pub/winresdump/winresdump/doc/pefile.html.
        /// </summary>
        private void LoadHeader()
        {
            if (_headerMemory == null)
            {
                Reset();
                return;
            }

            var offset = 0;
            var dosHeader = WindowsHelper.ParseStruct<Kernel32.IMAGE_DOS_HEADER>(_headerMemory, offset);

            offset = dosHeader.e_lfanew;
            var signature = BitConverter.ToUInt32(_headerMemory, offset);
            if (signature != Kernel32.IMAGE_NT_SIGNATURE) return;

            offset += sizeof(uint);
            var fileHeader = WindowsHelper.ParseStruct<Kernel32.IMAGE_FILE_HEADER>(_headerMemory, offset);
            if (fileHeader.Machine != Kernel32.IMAGE_FILE_MACHINE_I386) return;

            offset += Kernel32.IMAGE_SIZEOF_FILE_HEADER;
            var optHeader = WindowsHelper.ParseStruct<Kernel32.IMAGE_OPTIONAL_HEADER32>(_headerMemory, offset);

            offset = (int)optHeader.ExportTable.VirtualAddress;
            var exportDir = WindowsHelper.ParseStruct<Kernel32.IMAGE_EXPORT_DIRECTORY>(_headerMemory, offset);

            var count = (int)exportDir.NumberOfFunctions;
            for (var index = 0; index < count; index++)
            {
                var nameAddr = exportDir.AddressOfNames + index * 4;
                var namePtr = BitConverter.ToUInt32(_headerMemory, (int)nameAddr);
                var name = WindowsHelper.ParseString(_headerMemory, namePtr);

                var funcAddr = exportDir.AddressOfFunctions + index * 4;
                var funcPtr = BitConverter.ToUInt32(_headerMemory, (int)funcAddr);
                _functionPointers.Add(new Tuple<string, long>(name, funcPtr));
            }
        }
    }
}