namespace emulator6502
{
    public class CpuSnapshot
    {
        public ulong Cycles { get; set; }
        public byte Status { get; set; }
        public ushort PC { get; set; }
        public byte SP { get; set; }
        public byte A { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
    }
}