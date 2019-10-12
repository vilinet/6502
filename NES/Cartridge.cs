using System;
using System.IO;
using NES.Mapper;

namespace NES
{
    public class Cartridge : ICartridge
    {
        public RomInfo Info { get; private set; }

        public ushort From  => 0x8000;

        public ushort To  => 0xFFFF;

        public Mirroring Mirroring => Info.Mirroring;

        private byte[] _prgRom = new byte[0xFFFF];
        private byte[] _chrRom = new byte[0xFFFF];
        private byte[] _trainerData = new byte[512];

        private IMapper _mapper;

        public Cartridge()
        {
            _mapper = GetMapper(0);
        }

        private RomInfo LoadHeader(BinaryReader reader)
        {
            var info = new RomInfo();
            var bytes = reader.ReadBytes(16);
            
            info.ProgramBanks = bytes[4];
            info.CharBanks = bytes[5];
            
            info.Mirroring = (bytes[6] & 1) == 1 ? Mirroring.Vertical:Mirroring.Horizontal;
            info.HasBattery = (bytes[6] & 2) == 2;
            info.HasTrainerData = (bytes[6] & 3) == 4;
            info.IgnoreMirroringControl = (bytes[6] & 4) == 8;
            info.MapperId = (bytes[6] & 0xF0) >> 4;
            info.Unisystem = (bytes[7] & 1) == 1;
            info.PlayChoice = (bytes[7] & 2) == 2;
            info.NewFormat = (bytes[7] & 6) == 6;
            info.MapperId += (bytes[7] & 0xF0);
            
            return info;
        }

        private IMapper GetMapper(int mapperId)
        {
            if(mapperId == 0) return new Mapper000(Info?.ProgramBanks??0);
            throw new Exception($"Mapper: {mapperId} is not supported!" );
        }
        
        public void LoadRom(string filepath)
        {
            using (var reader = new BinaryReader(new FileStream(filepath, FileMode.Open, FileAccess.Read)))
            {
                Info = LoadHeader(reader);
                
                if(Info.HasTrainerData)  _trainerData = reader.ReadBytes(512);
                _prgRom = reader.ReadBytes(Info.ProgramBanks * 16 * 1024);
                _chrRom = reader.ReadBytes(Info.CharBanks * 8 * 1024);
            }

            _mapper = GetMapper(Info.MapperId);
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
            return _prgRom[_mapper.Read(address)];
        }
    }
}