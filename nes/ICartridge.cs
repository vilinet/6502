using emulator6502;

namespace NES
{
    public interface ICartridge : IAddressable
    {
        int ReadPpu(ushort address);
    }
}