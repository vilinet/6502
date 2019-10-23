using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
        private bool _stop;

        public NesState State { get; private set; }

        public Cpu Cpu { get; }

        private float _speed = 1;
        public float Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
                if (value <= 0)
                {
                    _speed = 0;
                    Pause();
                }
                else if(State == NesState.Paused)
                {
                    Resume();
                }
            }
        }
        public int ActualFps { get; private set; }

        public Nes(IDisplay display, IDebugDisplay debugDisplay, IController controller1, IController controller2 = null)
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
                    PpuColors.Colors[colorIndex++] = (uint)((reader.ReadByte() << 16) + (reader.ReadByte() << 8) + reader.ReadByte());
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
            State = NesState.Stopped;
            Thread.Sleep(50);
            _cartridge.LoadRom(_filePath);
            _ppu.Reset();
            Cpu.Reset();
            State = NesState.Running;
        }

        public void Pause()
        {
            State = NesState.Paused;
        }

        public void Stop()
        {
            _stop = true;
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

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!_stop)
            {
                double frameTime = 1 / (Speed * 60)*1000;
                if(Speed <= 0.0001) ActualFps = 0;
                if (!(stopwatch.ElapsedMilliseconds >= frameTime)) continue;
                ActualFps = (int)Math.Round(1000.0 / stopwatch.ElapsedMilliseconds);
                stopwatch.Restart();
                if (State != NesState.Running) continue;

                if (State == NesState.Running)
                {
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
                }
                
                _ppu.FrameFinished = false;
            }
        }

        public void RunOnThread()
        {
            var thread = new Thread(Run) {Priority = ThreadPriority.Highest, IsBackground = true};
            thread.Start();
        }
    }
}