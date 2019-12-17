using System.Collections.Generic;

namespace emulator6502
{
    public partial class Bus : IAddressable
    {

        public ushort From { get; } = 0;
        public ushort To { get; } = 0xFFFF;
        
        private readonly List<IAddressable> _addressables = new List<IAddressable>();
        private IAddressable [] _cache = new IAddressable[0xFFFF + 1];

        public IReadOnlyList<IAddressable> Addressables => _addressables.AsReadOnly();

        public Bus()
        {
            var devNull = new DevNullAddressable();
            for (var i = 0; i < _cache.Length; i++)  _cache[i] = devNull;
        }

        public void AddMap(IAddressable addressable)
        {
            _addressables.Add( addressable);
            for (int i = addressable.From; i <= addressable.To; i++)  _cache[i] = addressable;
        }

        public void Write(ushort address, byte value)
        {
             _cache[address].Write(address, value);
        }

        public byte Read(ushort address)
        {
           return _cache[address].Read(address);
        }
    }
}