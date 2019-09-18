using System.Drawing;
using emulator6502;

namespace NES
{
    public class Cartridge : IAddressable
    {
        public ushort From { get; } 
        public ushort To { get; }

        private byte[] _data;

        public Cartridge(ushort from, ushort to)
        {
            From = from;
            To = to;
            _data = new byte[To-From + 1];
        }

        public void Resize(int size)
        {
            _data = new byte[size];
        }
        
        public Cartridge(ushort from, ushort to, Cartridge source)
        {
            From = from;
            To = to;
            _data = source._data;
        }

        public void Write(ushort address, byte value)
        {
            _data[address - From] = value;
        }

        public byte Read(ushort address)
        {
            return _data[address - From];
        }
    }
}