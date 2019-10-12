using emulator6502;

namespace NES
{
    public enum Mirroring
    {
        Vertical, Horizontal, Both
    }

    public interface ICartridge : IAddressable
    {
        int ReadPpu(ushort address);

        bool WritePpu(ushort address, byte val);

        Mirroring Mirroring { get; }

    }
}