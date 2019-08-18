namespace emulator6502
{
    public interface IAddressable
    {
        void Write(ushort address, byte value);
        byte Read(ushort address);
    }
}