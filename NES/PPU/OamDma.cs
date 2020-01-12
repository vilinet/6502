using emulator6502;

namespace NES
{
    internal class OamDma : IAddressable
    {
        private IAddressable _bus;
        private PPU _ppu;

        public OamDma(PPU ppu, IAddressable bus)
        {
            _ppu = ppu;
            _bus = bus;
        }

        public ushort From { get; } = 0x4014;
        public ushort To { get; } = 0x4014;
        public void Write(ushort address, byte value)
        {
            ushort actualAddress = (ushort)(value << 8);
            for (int i = 0; i < 256; i++)
            {
                _ppu.WriteOAM(_bus.Read(actualAddress));
                actualAddress++;
            }
        }

        public byte Read(ushort address)
        {
            return 0;
        }
    }
}