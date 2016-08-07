using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Nitroh.Windows.Memory
{
    internal class MemoryCache
    {
        private readonly IntPtr _processHandle;
        private readonly long _baseAddress;
        private readonly long _headerSize;
        private readonly List<byte> _headerMemory;
        private readonly List<MemoryPage> _pages;

        internal MemoryCache(IntPtr processHandle, long baseAddress, long headerSize)
        {
            _processHandle = processHandle;
            _baseAddress = baseAddress;
            _headerSize = headerSize;
            _headerMemory = new List<byte>();
            _pages = new List<MemoryPage>();
        }

        internal bool GetHeaderMemory(out byte[] buffer)
        {
            if (_headerMemory.Count > 0)
            {
                buffer = _headerMemory.ToArray();
                return true;
            }

            buffer = null;
            var valid = LoadHeaderMemory();
            if (!valid) return false;

            buffer = _headerMemory.ToArray();
            return true;
        }

        private bool LoadHeaderMemory()
        {
            byte[] temp;
            var valid = GetLiveMemory(_baseAddress, _headerSize, out temp);
            if (!valid) return false;

            _headerMemory.Clear();
            _headerMemory.AddRange(temp);
            return true;
        }

        internal bool GetMemory(long baseAddress, long size, out byte[] buffer, bool live)
        {
            return live ? GetLiveMemory(baseAddress, size, out buffer) : GetCachedMemory(baseAddress, size, out buffer);
        }

        private bool GetLiveMemory(long baseAddress, long size, out byte[] buffer)
        {
            buffer = new byte[size];
            var bufferHandle = GCHandle.Alloc(buffer);
            var bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            IntPtr bytesRead;
            var result = Kernel32.ReadProcessMemory(_processHandle, (IntPtr)unchecked((int)baseAddress), bufferPtr, (int)size,
                out bytesRead);
            bufferHandle.Free();
            return result && bytesRead.ToInt64() == size;
        }

        private bool GetCachedMemory(long baseAddress, long size, out byte[] buffer)
        {
            buffer = new byte[size];

            if (baseAddress >= _baseAddress && baseAddress + size <= _baseAddress + _headerSize)
            {
                if (_headerMemory == null || _headerMemory.Count == 0)
                {
                    var valid = LoadHeaderMemory();
                    if (!valid) return false;
                }
                // ReSharper disable once PossibleNullReferenceException
                Array.Copy(_headerMemory.ToArray(), baseAddress - _baseAddress, buffer, 0, size);
                return true;
            }

            var maxAddress = baseAddress + size - 1;
            var maxAddressStartingPage = maxAddress / MemoryPage.PageSize * MemoryPage.PageSize;
            var baseAddressStartingPage = baseAddress / MemoryPage.PageSize * MemoryPage.PageSize;
            var bufferList = new List<byte>();
            for (var index = baseAddressStartingPage; index <= maxAddressStartingPage; index += MemoryPage.PageSize)
            {
                MemoryPage page;
                var valid = LoadPage(index, out page);
                if (!valid) return false;
                bufferList.AddRange(page.Data);
            }

            var startOffset = baseAddress - baseAddressStartingPage;
            Array.Copy(bufferList.ToArray(), startOffset, buffer, 0, size);
            return true;
        }

        private bool LoadPage(long baseAddress, out MemoryPage page)
        {
            page = _pages.FirstOrDefault(x => x.BaseAddress == baseAddress);
            if (page != null) return true;

            byte[] buffer;
            var valid = GetLiveMemory(baseAddress, MemoryPage.PageSize, out buffer);
            if (!valid) return false;

            page = new MemoryPage(baseAddress, buffer);
            _pages.Add(page);
            return true;
        }
    }
}
