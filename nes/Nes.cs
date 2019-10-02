using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using emulator6502;
using NES.Interfaces;
using NESInterfaces;

namespace NES
{
    public enum NesState
    {
        Paused,
        Running,
        Stopped
    }

    public class Nes
    {
        private readonly Ppu _ppu;
        private readonly IController _controller1, _controller2;
        private readonly Bus _bus;
        private readonly Cartridge _cartridge;

        private int _internalClock;
        private readonly CpuRam _cpuRam;
        private string _filePath;
        private readonly IDisplay _display;

        public NesState State { get; private set; }
        public Cpu Cpu { get; }

        public Nes(IDisplay display, IDebugDisplay debugDisplay,  IController controller1, string filePath, IController controller2 = null)
        {
            _controller1 = controller1;
            _controller2 = controller2;
            _display = display;
            _cpuRam = new CpuRam();
            _cartridge = new Cartridge();
            _bus = new Bus();
            Cpu = new Cpu(_bus);
            _ppu = new Ppu(Cpu, _cartridge, _display, debugDisplay);

            _bus.AddMap(_cpuRam);
            _bus.AddMap(_ppu);
            _bus.AddMap(new ControllerDevice(0x4016, _controller1));
            _bus.AddMap(new ControllerDevice(0x4017, _controller2));
            _bus.AddMap(new OamDma(_ppu, _bus));
            _bus.AddMap(_cartridge);

            _filePath = filePath;
            _cartridge.LoadRom(_filePath);
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
            
            const double frameTime = 1f / 60;
            var elapsedTime = frameTime;
            var prev = DateTime.Now;

            while (State != NesState.Stopped)
            {
                if (elapsedTime >= frameTime)
                {
                    if (State == NesState.Running)
                    {
                        while (!_ppu.FrameFinished)
                        {
                            try
                            {
                                _ppu.Clock();
                         
                            if (_internalClock % 3 == 0)
                            {
                                Cpu.Clock();
                                _internalClock = 0;
                            }
                            }
                            catch (Exception E)
                            {

                            }
                            _internalClock++;
                        }

                        _ppu.FrameFinished = false;
                    }

                    elapsedTime = 0;
                }

                elapsedTime += (DateTime.Now - prev).TotalSeconds;
                prev = DateTime.Now;
            }
        }

        public void RunOnThread()
        {
            Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

     
    }
}