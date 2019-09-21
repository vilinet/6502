using System.IO;
using NES.Mapper;

namespace NES
{
    public class RomInfo
    {
        public int ProgramBanks { get; internal set; }

        public int CharBanks { get; internal set; }

        public int MapperId { get; internal set; }
    }

    public class Cartridge : ICartridge
    {

        public RomInfo Info { get; set; }

        public ushort From  => 0x8000;

        public ushort To  => 0xFFFF;

        public Mirroring Mirroring => Mirroring.Vertical;

        private byte[] _prgRom;
        private byte[] _chrRom;

        private IMapper _mapper;

        public void LoadRom(string filepath)
        {
            Info = new RomInfo();

            using (var reader = new BinaryReader(new FileStream(filepath, FileMode.Open, FileAccess.Read)))
            {
                //Reads header
                for (int i = 0; i < 16; i++)
                {
                    if (i == 4)
                    {
                        Info.ProgramBanks = reader.ReadByte();
                        _prgRom = new byte[Info.ProgramBanks * 16 * 1024];
                    }
                    else if (i == 5)
                    {
                        Info.CharBanks = reader.ReadByte();
                        _chrRom = new byte[Info.CharBanks * 8 * 1024];

                    }
                    else
                    {
                        reader.ReadByte();
                    }
                }

                for (int i = 0; i < _prgRom.Length; i++)
                {
                    _prgRom[i] = reader.ReadByte();
                }

                for (int i = 0; i < _chrRom.Length; i++)
                {
                    _chrRom[i] = reader.ReadByte();
                }

                _mapper = new Mapper000(Info.ProgramBanks);
            }
        }

        public int ReadPpu(ushort address)
        {
            int addr = _mapper.ReadPpu(address);
            if (addr == -1) return -1;
            return _chrRom[addr];
        }

        public bool WritePpu(ushort address, byte val)
        {
            int addr = _mapper.WritePpu(address);
            if (addr == -1) return false;
            _chrRom[addr] = val;
            return true;
        }

        public void Write(ushort address, byte value)
        {
            _prgRom[_mapper.Read(address)] = value;
        }

        public byte Read(ushort address)
        {
            return  _prgRom[_mapper.Read(address)];
        }
    }
}