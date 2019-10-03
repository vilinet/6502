using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using emulator6502;
using NES.Interfaces;
using NESInterfaces;

namespace NES
{
    public class Nes
    {
        private readonly Ppu _ppu;
        private readonly Bus _bus;
        private readonly Cartridge _cartridge;
        private readonly CpuRam _cpuRam;
        private readonly IDisplay _display;
        private string _filePath;
        private int _internalClock;

        public NesState State { get; private set; }
        
        public Cpu Cpu { get; }

        public Nes(IDisplay display, IDebugDisplay debugDisplay,  IController controller1, IController controller2 = null)
        {
            _display = display;
            _cpuRam = new CpuRam();
            _cartridge = new Cartridge();
            _bus = new Bus();
            Cpu = new Cpu(_bus);
            _ppu = new Ppu(Cpu, _cartridge, _display, debugDisplay);

            _bus.AddMap(_cpuRam);
            _bus.AddMap(_ppu);
            _bus.AddMap(new ControllerDevice(0x4016, controller1));
            _bus.AddMap(new ControllerDevice(0x4017, controller2));
            _bus.AddMap(new OamDma(_ppu, _bus));
            _bus.AddMap(_cartridge);
            
            _ppu.PowerOn();
            Cpu.Reset();
        }

        public void LoadPalette(string file)
        {
            var colorIndex = 0;
            using (var reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    PpuColors.Colors[colorIndex++] = (uint) ((reader.ReadByte() << 16) + (reader.ReadByte() << 8) + reader.ReadByte());
                }
            }
        }

        public void LoadRom(string filePath)
        {
            _filePath = filePath;
            Reset();
        }

        public void Reset()
        {
            _cartridge.LoadRom(_filePath);
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
            
            const double frameTime = (1f / 60) * 1000;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int fps = 0;
            while (State != NesState.Stopped)
            {
                if (!(stopwatch.ElapsedMilliseconds >= frameTime)) continue;
                if (State != NesState.Running) continue;
                
                stopwatch.Restart();
                
                while (!_ppu.FrameFinished)
                {
                    _ppu.Clock();
                         
                    if (_internalClock % 3 == 0)
                    {
                        Cpu.Clock();
                        _internalClock = 0;
                    }
                    _internalClock++;
                }

                fps++;
                _ppu.FrameFinished = false;
            }
        }

        public void RunOnThread()
        {
            Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }
    }
}