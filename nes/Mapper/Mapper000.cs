using System;
using System.Collections.Generic;
using System.Text;

namespace NES.Mapper
{
    public class Mapper000 : IMapper
    {
        private int _programBanks;
        public Mapper000(int programBanks)
        {
            _programBanks = programBanks;
        }
        public int Read(ushort address)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                return address & (_programBanks > 1 ? 0x7FFF : 0x3FFF);
            }

            return -1;
        }

        public int ReadPpu(ushort address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return address;
            }
            return -1;
        }

        public int WritePpu(ushort address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return address;
            }
            return -1;
        }

        public int Write(ushort address)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                return address & (_programBanks > 1 ? 0x7FFF : 0x3FFF);
            }

            return -1;
        }
    }
}
