using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using emulator6502;
using NES.Interfaces;

namespace NES
{
    public class Nes
    {
        private readonly Cartridge _cartridge;
        private string _filePath;
        private int _internalClock;
        private bool _stop;

        public Ppu PPU { get; }
        public Bus Bus { get; }

        public NesState State { get; private set; }

        public Cpu Cpu { get; }

        private float _speed = 1;
        public float Speed
        {
            get => _speed;
            
            set
            {
                _speed = value;

                if (value <= 0)
                {
                    _speed = 0;
                    Pause();
                }
                else if (State == NesState.Paused)
                {
                    Resume();
                }
            }
        }
        public int ActualFps { get; private set; }

        public Nes(IDisplay display, IController controller1, IController controller2 = null)
        {
            _cartridge = new Cartridge();
            Bus = new Bus();
            Cpu = new Cpu(Bus);
            PPU = new Ppu(Cpu, _cartridge, display);

            Bus.AddMap(new CpuRam());
            Bus.AddMap(PPU);
            Bus.AddMap(new ControllerDevice(0x4016, controller1));
            Bus.AddMap(new ControllerDevice(0x4017, controller2));
            Bus.AddMap(new OamDma(PPU, Bus));
            Bus.AddMap(_cartridge);

            PPU.PowerOn();
            Cpu.Reset();
        }

        public void LoadColors(string file)
        {
            var colorIndex = 0;
            using (var reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length && colorIndex<63)
                {
                    PpuColors.Colors[colorIndex++] = (uint)((reader.ReadByte() << 16) + (reader.ReadByte() << 8) + reader.ReadByte());
                    colorIndex++;
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
            PPU.Reset();
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
                double frameTime = 1 / (Speed * 60) * 1000;
                if (Speed <= 0.0001) ActualFps = 0;
                if (!(stopwatch.ElapsedMilliseconds >= frameTime)) continue;
                ActualFps = (int)Math.Round(1000.0 / stopwatch.ElapsedMilliseconds);
                stopwatch.Restart();
                if (State != NesState.Running) continue;

                if (State == NesState.Running)
                {
                    while (!PPU.FrameFinished)
                    {
                        Tick();
                    }
                }

                PPU.FrameFinished = false;
            }
        }

        public void Tick()
        {
            PPU.Clock();

            if (_internalClock % 3 == 0)
            {
                Cpu.Clock();
                _internalClock = 0;
            }
            _internalClock++;
        }

        public void RunOnThread()
        {
            var thread = new Thread(Run) {Priority = ThreadPriority.Highest, IsBackground = true};
            thread.Start();
        }

    }

    public enum NesState
    {
        Paused,
        Running,
        Stopped
    }
}