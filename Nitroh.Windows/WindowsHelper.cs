using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Nitroh.Windows
{
    public static class WindowsHelper
    {
        private const int MaxStringLength = 255;
        private const byte StringSeparator = 0;

        /// <summary>
        /// ASM values Taken from http://ref.x86asm.net/coder32.html.
        /// </summary>
        private const byte MovByte = 0xa1;
        private const byte RtnByte = 0xc3;

        public static uint ReadUInt(byte[] memory, long offset = 0)
        {
            return BitConverter.ToUInt32(memory, (int) offset);
        }

        public static uint ReadUInt(IntPtr handle, long offset)
        {
            byte[] buffer;
            var valid = ReadMemory(handle, offset, 4, out buffer);
            return valid ? ReadUInt(buffer) : 0;
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

        public static string ReadString(IntPtr handle, long offset)
        {
            byte[] buffer;
            var valid = ReadMemory(handle, offset, MaxStringLength, out buffer);
            return valid ? ParseString(buffer) : string.Empty;
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

        public static T ReadStruct<T>(IntPtr handle, long offset) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            byte[] buffer;
            var valid = ReadMemory(handle, offset, size, out buffer);
            return valid ? ParseStruct<T>(buffer) : default(T);
        }

        public static bool ReadMemory(IntPtr handle, long baseAddress, long size, out byte[] buffer)
        {
            buffer = new byte[size];
            var bufferHandle = GCHandle.Alloc(buffer);
            var bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            IntPtr bytesRead;
            var result = Kernel32.ReadProcessMemory(handle, (IntPtr) unchecked((int) baseAddress), bufferPtr, (int) size,
                out bytesRead);
            bufferHandle.Free();
            return result && bytesRead.ToInt64() == size;
        }
    }
}