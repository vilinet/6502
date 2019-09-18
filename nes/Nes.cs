using System;
using System.Collections.Generic;
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
            _cpuRam = new CpuRam();
            _cartridge = new Cartridge(0x8000, 0xFFFF);

            _bus = new Bus();
            Cpu = new Cpu(_bus);
            _ppu = new Ppu(Cpu, _bus, _display);
            
            _bus.AddMap(_cpuRam);
            _bus.AddMap(_ppu);
            _bus.AddMap(_cartridge);
            _bus.AddMap(new OamDma(_ppu, _bus));

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
            int chrSize = 8 * 1024;
            ushort start = _cartridge.From;
            using (var reader = new BinaryReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read)))
            {
                //Reads header
                for (int i = 0; i < 16; i++)
                {
                    if (i == 4)
                    {
                        var size = reader.ReadByte();
                        prgSize *= size;
                    }
                    else if (i == 5)
                    {
                        var size = reader.ReadByte();
                        chrSize *= size;
                    }
                    else reader.ReadByte();
                }

                while (prgSize-- > 0)
                {
                    _cartridge.Write(start++, reader.ReadByte());
                }
                var tiles = new List<SpritePattern>();
                
                while (chrSize-- > 0)
                {
                    _ppu.WriteChr(start++, reader.ReadByte());
                }
            }
        }

        private void Clock()
        {
            _ppu.Clock();
           
            if (_internalClock % 3 == 0)
            {
                Cpu.Clock();
                _internalClock = 0;
            }
            
            _internalClock++;
        }

        private List<SpritePattern> LoadPatternsFromAddress(int kbytes)
        {
            return null;
        }
    }
}