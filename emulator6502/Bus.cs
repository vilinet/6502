using System;
using System.Collections.Generic;
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
        
        private readonly List<Entry> _entries = new List<Entry>();

        public void AddMap(ushort from, ushort to, Addressable addressable)
        {
            _entries.Add(new Entry { From = from, To = to, Addressable = addressable } );
        }
        
        public void AddMap(ushort from, Addressable addressable)
        {
            _entries.Add(new Entry { From = from, To = (ushort)(from + addressable.Size-1), Addressable = addressable } );
        }

        public void Write(ushort address, byte value)
        {
            var entry = GetEntry(address);
            GetEntry(address).Addressable.Write((ushort)(address - entry.From), value);
        }

        public byte Read(ushort address)
        {
            var entry = GetEntry(address);
            return GetEntry(address).Addressable.Read((ushort)(address - entry.From));
        }

        public void Write(ushort address, ushort value)
        {
            Write(address, (byte)(value & 0x00FF) );
            Write((ushort)(address+1), (byte)((value & 0xFF00) >> 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Entry GetEntry(ushort address)
        {
            foreach (var entry in _entries)
            {
                if(address >= entry.From && address<= entry.To ) return entry;
            }

            throw new Exception("Non configured address");
        }
    }
}