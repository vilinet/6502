namespace emulator6502
{
    public class Addressable : IAddressable
    {
        public ushort Size => (ushort)Data.Length;

        public byte this[int index] => Data[index];

        private byte[] Data { get; set; }

        protected Addressable(int size)
        {
            Data = new byte[size];
        }

        public virtual void Write(ushort address, byte value)
        {
            Data[address] = value;
        }

        public virtual byte Read(ushort address)
        {
            return Data[address];
        }
    }
}
