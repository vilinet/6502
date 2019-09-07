namespace emulator6502
{
    public class Bus : IAddressable
    {
        public ushort From { get; } = 0;
        public ushort To { get; } = 0xFFFF;
        
        private readonly IAddressable[] _entriesArray = new IAddressable[30];
        private int _entriesCount;
        
        public void AddMap(IAddressable addressable)
        {
            _entriesArray[_entriesCount++] = addressable;
        }

        public void Write(ushort address, byte value)
        {
            for (int i = 0; i < _entriesCount; i++)
            {
                var entry = _entriesArray[i];

                if (address >= entry.From && address <= entry.To)
                {
                    entry.Write(address, value);
                }
            }
        }

        public byte Read(ushort address)
        {
            for (int i = 0; i < _entriesCount; i++)
            {
                var entry = _entriesArray[i];
                if (address >= entry.From && address <= entry.To)
                {
                    return entry.Read(address);
                }
            }
            return 0;
        }

        public void Write(ushort address, ushort value)
        {
            Write(address, (byte)(value & 0x00FF) );
            Write((ushort)(address+1), (byte)((value & 0xFF00) >> 8));
        }
    }
}