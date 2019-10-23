using emulator6502;

namespace NES
{
    /// <summary>
    /// 2048 bytes ram
    /// </summary>
    public class CpuRam : IAddressable
    {
        private readonly byte[] _data = new byte[0x0800];
        public ushort From { get; } = 0x0000;
        public ushort To { get; } = 0x1FFF;
 
        public void Write(ushort address, byte value)
        {
            _data[address & 0x07FF] = value;
        }

        public byte Read(ushort address)
        {
            return _data[address & 0x07FF];
        }
    }
}