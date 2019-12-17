namespace emulator6502
{
    public partial class Bus
    {
        private class DevNullAddressable : IAddressable
        {
            public ushort From { get; }
            public ushort To { get; }
            public void Write(ushort address, byte value) { }

            public byte Read(ushort address)  {  return 0;  }
        }
    }
}