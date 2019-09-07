using System;
using System.IO;
using emulator6502;

namespace NES
{
    public class Nes
    {
        private long _deviceClock = 0;
        private readonly Ppu _ppu;
        private readonly Bus _bus;
        private readonly Cartridge _cartridge;
        public Cpu Cpu { get; }
        private readonly CpuRam _cpuRam;
        private string _filePath;

        public Nes(string filePath)
        {
            _ppu = new Ppu();
            _cpuRam = new CpuRam();
            _cartridge = new Cartridge(0x8000, 0xBFFF);
            
            _bus = new Bus();
            _bus.AddMap(_cpuRam);
            _bus.AddMap(_ppu);
            _bus.AddMap(_cartridge);
            _bus.AddMap(new Cartridge(0xC000, 0xFFFF, _cartridge));
            Cpu = new Cpu(_bus);
            
            _filePath = filePath;
            LoadCartridge();
        }

        public void PowerOn()
        {
            _ppu.PowerOn();
            Cpu.Reset();
            Cpu.Execute(OpcodeEnum.LDA, BindingMode.Immediate, 0xC0);
            Cpu.Execute(OpcodeEnum.PHA, BindingMode.Implied);
            Cpu.Execute(OpcodeEnum.LDA, BindingMode.Immediate, 0x00);
            Cpu.Execute(OpcodeEnum.PHA, BindingMode.Implied);
            Cpu.Execute(OpcodeEnum.SEI, BindingMode.Implied);
            LoadCartridge();
        }

        public void Reset()
        {
            _ppu.Reset();
            Cpu.Reset();
        }

        private void LoadCartridge()
        {
            int prgSize = 16 * 1024;
            ushort start = _cartridge.From;
            using (var reader = new BinaryReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read)))
            {
                //Reads header
                for (int i = 0; i < 16; i++)
                {
                    if (i == 4)
                    {
                        prgSize *= reader.ReadByte();
                    }
                    else  {Console.WriteLine($"{i}: " + reader.ReadByte());}
                }

                while (prgSize-- > 0)
                {
                    _cartridge.Write(start++, reader.ReadByte());
                }
                
            }
            //_bus.Write(0xFFFC, 0xC000);
        }

        public void Clock()
        {
            _deviceClock++;
            _ppu.Clock();
           
            if (_deviceClock % 3 == 0)
            {
                Cpu.Clock();
            }
        }


        /*
        public void LoadBinaryProgram(byte[] data, ushort address)
        {
            for (ushort i = 0; i < data.Length; i++)
                Write(address++, data[i]);
        }

        /// <summary>
        /// Awaits for a string of hex formatted binary data
        /// "0F 09 00 AF"
        /// </summary>
        /// <param name="programBytes"></param>
        public void LoadBinaryProgram(string programBytes)
        {
            programBytes = Regex.Replace(programBytes, @"\s+", "").ToUpper(); ;

            for (ushort i = 0; i < programBytes.Length/2; i++)
            {
                Write(i, byte.Parse( programBytes.Substring(i*2,2), System.Globalization.NumberStyles.HexNumber));
            }
        }*/
    }
}