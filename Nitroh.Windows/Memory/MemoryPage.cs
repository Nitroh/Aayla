namespace Nitroh.Windows.Memory
{
    internal class MemoryPage
    {
        internal const long PageSize = 4096;

        internal long BaseAddress { get; }

        internal long EndAddress => BaseAddress + PageSize;

        internal byte[] Data { get; }

        internal MemoryPage(long baseAddress, byte[] data)
        {
            BaseAddress = baseAddress;
            Data = data;
        }
    }
}
