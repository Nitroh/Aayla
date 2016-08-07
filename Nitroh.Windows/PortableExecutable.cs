using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nitroh.Windows.Memory;

namespace Nitroh.Windows
{
    public class PortableExecutable
    {
        #region variables
        private const int ProcessMaxIdleTime = 5000;
        private const int MaxStringLength = 100;
        private const byte StringSeparator = 0;

        /// <summary>
        /// ASM values Taken from http://ref.x86asm.net/coder32.html.
        /// </summary>
        private const byte MovByte = 0xa1;
        private const byte RtnByte = 0xc3;

        private readonly string _processName;
        private readonly string _moduleName;
        private MemoryCache _memoryCache;

        public Process Process { get; private set; }

        public long BaseAddress { get; private set; }

        public bool Running { get; private set; }

        private List<Tuple<string, long>> _functionPointers;
        public IEnumerable<Tuple<string, long>> FunctionPointers => _functionPointers;
        #endregion

        public PortableExecutable(string processName, string moduleName)
        {
            _processName = processName;
            _moduleName = moduleName;
            Running = false;
            LoadHeader();
        }
        
        private void Reset()
        {
            BaseAddress = 0;
            _functionPointers = new List<Tuple<string, long>>();
            _memoryCache = null;
            Process = null;
            Running = false;
        }

        public void Update()
        {
            var running = Process.GetProcessesByName(_processName).FirstOrDefault() != null;
            if (!Running && running) Refresh();
            if (running) return;
            Running = false;
            Reset();
        }

        protected virtual void Refresh()
        {
            LoadHeader();
        }

        /// <summary>
        /// Logic taken from http://www.csn.ul.ie/~caolan/pub/winresdump/winresdump/doc/pefile.html.
        /// </summary>
        private void LoadHeader()
        {
            Reset();
            Process = Process.GetProcessesByName(_processName).FirstOrDefault();
            if (Process == null)
            {
                Reset();
                return;
            }
            
            var loaded = Process.WaitForInputIdle(ProcessMaxIdleTime);
            if (!loaded)
            {
                Reset();
                return;
            }

            var module = Process.Modules.OfType<ProcessModule>().FirstOrDefault(x => x.ModuleName == _moduleName);
            if (module == null)
            {
                Reset();
                return;
            }

            Running = true;
            BaseAddress = module.BaseAddress.ToInt64();
            _memoryCache = new MemoryCache(Process.Handle, BaseAddress, module.ModuleMemorySize);

            byte[] headerMemory;
            var valid = _memoryCache.GetHeaderMemory(out headerMemory);
            if(!valid)
            {
                Reset();
                return;
            }

            var offset = 0;
            var dosHeader = ParseStruct<Kernel32.IMAGE_DOS_HEADER>(headerMemory, offset);

            offset = dosHeader.e_lfanew;
            var signature = BitConverter.ToUInt32(headerMemory, offset);
            if (signature != Kernel32.IMAGE_NT_SIGNATURE) return;

            offset += sizeof(uint);
            var fileHeader = ParseStruct<Kernel32.IMAGE_FILE_HEADER>(headerMemory, offset);
            if (fileHeader.Machine != Kernel32.IMAGE_FILE_MACHINE_I386) return;

            offset += Kernel32.IMAGE_SIZEOF_FILE_HEADER;
            var optHeader = ParseStruct<Kernel32.IMAGE_OPTIONAL_HEADER32>(headerMemory, offset);

            offset = (int)optHeader.ExportTable.VirtualAddress;
            var exportDir = ParseStruct<Kernel32.IMAGE_EXPORT_DIRECTORY>(headerMemory, offset);

            var count = (int)exportDir.NumberOfFunctions;
            for (var index = 0; index < count; index++)
            {
                var nameAddr = exportDir.AddressOfNames + index * 4;
                var namePtr = BitConverter.ToUInt32(headerMemory, (int)nameAddr);
                var name = ParseString(headerMemory, namePtr);

                var funcAddr = exportDir.AddressOfFunctions + index * 4;
                var funcPtr = BitConverter.ToUInt32(headerMemory, (int)funcAddr);
                _functionPointers.Add(new Tuple<string, long>(name, funcPtr));
            }
        }

        #region Read
        public byte ReadByte(long offset, bool live)
        {
            byte[] buffer;
            var valid = _memoryCache.GetMemory(offset, 1, out buffer, live);
            return valid ? ParseByte(buffer) : default(byte);
        }

        public uint ReadUInt(long offset, bool live)
        {
            byte[] buffer;
            var valid = _memoryCache.GetMemory(offset, 4, out buffer, live);
            return valid ? ParseUInt(buffer) : 0;
        }

        public int ReadInt(long offset, bool live)
        {
            byte[] buffer;
            var valid = _memoryCache.GetMemory(offset, 4, out buffer, live);
            return valid ? ParseInt(buffer) : 0;
        }

        public string ReadString(long offset, bool live)
        {
            byte[] buffer;
            var valid = _memoryCache.GetMemory(offset, MaxStringLength, out buffer, live);
            return valid ? ParseString(buffer) : string.Empty;
        }

        public T ReadStruct<T>(long offset, bool live) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            byte[] buffer;
            var valid = _memoryCache.GetMemory(offset, size, out buffer, live);
            return valid ? ParseStruct<T>(buffer) : default(T);
        }

        public bool GetMemory(long offset, long size, out byte[] buffer, bool live) => _memoryCache.GetMemory(offset, size, out buffer, live);
        #endregion

        #region Parse
        public static byte ParseByte(byte[] memory, long offset = 0)
        {
            return memory[offset];
        }

        public static uint ParseUInt(byte[] memory, long offset = 0)
        {
            return BitConverter.ToUInt32(memory, (int)offset);
        }

        public static int ParseInt(byte[] memory, long offset = 0)
        {
            return BitConverter.ToInt32(memory, (int)offset);
        }

        public static string ParseString(byte[] memory, long offset = 0)
        {
            var buffer = new List<byte>();
            for (var index = 0; index < MaxStringLength; index++)
            {
                if (memory[index + offset] == StringSeparator) break;
                buffer.Add(memory[index + offset]);
            }
            return Encoding.ASCII.GetString(buffer.ToArray());
        }

        public static T ParseStruct<T>(byte[] memory, long offset = 0) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var buffer = new byte[size];
            Array.Copy(memory, offset, buffer, 0, buffer.Length);
            var bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var result = (T)Marshal.PtrToStructure(bufferHandle.AddrOfPinnedObject(), typeof(T));
            bufferHandle.Free();
            return result;
        }

        public static byte[] ParseFunctionCode(byte[] memory)
        {
            var buffer = new byte[memory.Length - 2];
            if (memory[0] == MovByte && memory[memory.Length - 1] == RtnByte)
            {
                Array.Copy(memory, 1, buffer, 0, buffer.Length);
            }
            return buffer;
        }
        #endregion
    }
}