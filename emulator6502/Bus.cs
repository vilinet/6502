using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace emulator6502
{
    public class Bus : IAddressable
    {
        private class Entry
        {
            public ushort From;
            public ushort To;
            public IAddressable Addressable;

            public override string ToString()
            {
                return From.ToString("X4") + "-" + To.ToString("X4");
            }
        }

        private Entry[] _entriesArray = new Entry[30];
        private int _entriesCount = 0;
        
        public void AddMap(ushort from, Addressable addressable)
        {
            _entriesArray[_entriesCount++] = new Entry { From = from, To = (ushort)(from + addressable.Size - 1), Addressable = addressable };
        }

        public void Write(ushort address, byte value)
        {
            for (int i = 0; i < _entriesCount; i++)
            {
                var entry = _entriesArray[i];

                if (address >= entry.From && address <= entry.To)
                {
                    entry.Addressable.Write((ushort)(address - entry.From), value);
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
                    return entry.Addressable.Read((ushort)(address - entry.From));
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