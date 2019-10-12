namespace emulator6502
{
    public interface IAddressable
    {
        /// <summary>
        /// First mapped byte address
        /// </summary>
        ushort From { get; }
        
        /// <summary>
        /// Last mapped byte address
        /// </summary>
        ushort To { get; }
        
        void Write(ushort address, byte value);
        
        byte Read(ushort address);
    }
}