using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using emulator6502;
using NES.Display;

namespace NES
{
    public enum NesState
    {
        Paused, Running, Stopped
    }
    
    public class Nes
    {
        private readonly Ppu _ppu;
          
        public NesState State { get; private set; }
        public Cpu Cpu { get; }

        private readonly Bus _bus;
        private readonly Cartridge _cartridge;
      
        private long _internalClock;
        private readonly CpuRam _cpuRam;
        private string _filePath;
        private readonly IDisplay _display;

        public Nes(IDisplay display, string filePath)
        {
            _display = display;
            _ppu = new Ppu(_display);
            _cpuRam = new CpuRam();
            _cartridge = new Cartridge(0x8000, 0xBFFF);

            _bus = new Bus();
            _bus.AddMap(_cpuRam);
            _bus.AddMap(_ppu);
            _bus.AddMap(_cartridge);
            _bus.AddMap(new Cartridge(0xC000, 0xFFFF, _cartridge));
            Cpu = new Cpu(_bus);
            
            _filePath = filePath;
            _ppu.PowerOn();
            
            LoadCartridge();
            Cpu.Reset();
        }

        public void Reset()
        {
            LoadCartridge();
            _ppu.Reset();
            Cpu.Reset();
            State = NesState.Running;
        }

        public void Pause()
        {
            State = NesState.Paused;
        }

        public void Resume()
        {
            if (State == NesState.Paused)
                State = NesState.Running;
        }
        

        private void Run()
        {
            if (State != NesState.Paused)
            {
                Reset();
            }
            State = NesState.Running;
            
            const double frameTime = 1f/60f;
            double elapsedTime = frameTime;
            DateTime prev = DateTime.Now;
            
            while (State!=NesState.Stopped)
            {
                if (elapsedTime >= frameTime)
                {
                    if (State == NesState.Running)
                    {
                        while (!_ppu.FrameFinished) Clock();
                        _ppu.FrameFinished = false;
                    }

                    elapsedTime = 0;
                }
                elapsedTime += (DateTime.Now - prev).TotalSeconds;
                prev  = DateTime.Now; 
                Thread.Sleep(5);
            }
        }

        public void RunOnThread()
        {
            Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
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
                    else reader.ReadByte();

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
            _ppu.Clock();
           
            if (_internalClock % 3 == 0)
            {
                Cpu.Clock();
                _internalClock = 0;
            }
            
            _internalClock++;
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