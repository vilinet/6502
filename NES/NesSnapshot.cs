using emulator6502;

namespace NES
{
    public class NesSnapshot
    {
        public string RomPath { get; private set; }
        public CpuSnapshot CpuSnapshot { get; set; }
        public byte[] memory;
    }
}
