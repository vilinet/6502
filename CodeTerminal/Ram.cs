using emulator6502;

namespace CodeTerminal
{
    public class Ram : IAddressable
    {
        private readonly byte[] _data = new byte[0xFFFF+1];
        public ushort From { get; } = 0x0000;
        public ushort To { get; } = 0xFFFF;
 
        public void Write(ushort address, byte value)
        {
            _data[address] = value;
        }

        public byte Read(ushort address)
        {
            return _data[address];
        }
    }
}