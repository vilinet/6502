using System;
using emulator6502;
using NES.Display;
using NES.Registers;
using SFML.Graphics;

namespace NES
{
    public class Ppu : IAddressable
    {
        private bool oddFrame = false;
        public ushort From { get; } = 0x2000;
        public ushort To { get; } = 0x2007;

        public bool FrameFinished { get; internal set; }
        public PpuRegisters PPURegisters { get; } = new PpuRegisters();


        private const int PALETTE_BG = 0x3F00;
        private const int PALETTE_SPRITE = 0x3F10;
        private const int OAM = 0x2000;


        private byte[] _memory = new byte[0x4000];
        private byte[] _oam = new byte[256];
        private int[] _oamOnScanline = new int[8];
        private int _oamOnScanlineCount = 0;
        private ushort _actualPaletteAddress = 0;
        private bool _addressFull = false;
        public short Cycle { get; internal set; }
        public short Scanline { get; internal set; }

        private readonly IDisplay _display;

        private int color = 12;
        private readonly Cpu _cpu;
        private readonly Bus _bus;

        public Ppu(Cpu cpu, Bus bus, IDisplay display)
        {
            _display = display;
            _cpu = cpu;
            _bus = bus;
            LoadPalette();
        }

        private void LoadPalette()
        {
            for (int i = 0; i < PpuColors.Colors.Length; i++)
            {
                uint col = PpuColors.Colors[i];
                _memory[PALETTE_BG + i * 3] = (byte) ((0xFF0000 & col) >> 16);
                _memory[PALETTE_BG + i * 3 + 1] = (byte) ((0x00FF00 & col) >> 8);
                _memory[PALETTE_BG + i * 3 + 2] = (byte) ((0x0000FF & col));
            }
        }

        public void WriteChr(ushort address, byte data)
        {
            _memory[address] = data;
        }

        private byte _currentHighTile, _currentLowTile, _tileShift;

        public void WriteOAM(byte data)
        {
            _oam[PPURegisters.OAMADDR++] = data;
        }

        private byte ReadByte(ushort address)
        {
            return _memory[address];
        }

        private uint[] scanLineColors = new uint[300];


        public void Clock()
        {
            if ((!PPURegisters.PPUMASK.BackgroundEnable && !PPURegisters.PPUMASK.SpriteEnable) ||
                Scanline >= 240 && Scanline <= 260)
            {
                if (Scanline == 241 && Cycle == 1)
                {
                    PPURegisters.PPUSTATUS.VerticalBlank = true;
                    _cpu.Nmi();
                }
            }
            else if (Cycle == 0)
            {
                //iddle
            }
            else if (Cycle <= 256)
            {
                if (Scanline != -1 && Scanline != 261)
                {
                    _display.DrawPixel(Cycle-1, Scanline, scanLineColors[Cycle-1 ]);
                    //Visible tiles
                    //_display.DrawPixel(Cycle - 1, Scanline, PpuColors.Colors[(color++ ) % 64]);
                }
            }
            else if (Cycle <= 320)
            {
                if (Scanline == -1)
                {
                    PPURegisters.OAMADDR = 0;
                }
                else
                {
                }

                //Cycles 257-320
            }
            else if (Cycle <= 336)
            {
                //Cycles 321-336
            }
            else if (Cycle <= 340)
            {
                //Cycles 337-340
            }

            if (Scanline == 304 && Cycle == 280)
            {
                //reload scroll
            }

            if (Cycle >= 341)
            {
                Cycle = 0;
                Scanline++;

                if (Scanline >= 261)
                {
                    Scanline = -1;
                    FrameFinished = true;
                    oddFrame = !oddFrame;
                    _display.FrameFinished();
                }
                else
                {
                    UpdateOamOnScanlines();
                }
            }


            Cycle++;
        }

        private void UpdateOamOnScanlines()
        {
            _oamOnScanlineCount = 0;
#if DEBUG
            for (int i = 0; i < 8; i++) _oamOnScanline[i] = -1;
#endif
            int y = Scanline;
            for (int i = 0; i < _oam.Length && _oamOnScanlineCount < 8; i += 4)
            {
                if (y >= _oam[i] && y < _oam[i] + 8)
                {
                    _oamOnScanline[_oamOnScanlineCount++] = i;
                }
            }

            for (int i = 0; i < 256; i++)
            {
                scanLineColors[i] = 0;
            }

            for (int j = _oamOnScanlineCount - 1; j >= 0; j--)
            {
                var index = _oamOnScanline[j];
                int actualLine = Scanline - _oam[index];
                int sx = _oam[index + 3];

                int tileIndex = _oam[index + 1] * 16 + actualLine;
                int value1 = _memory[tileIndex];
                int value2 = _memory[tileIndex + 8];

                for (int x = 0; x < 8; x++)
                {
                    int bit = 7 - x;
                    int palette = ((value1 & (1 << bit)) >> bit) + ((value2 & (1 << bit)) >> bit);

                    if (palette > 0)
                    {
                        _display.DrawPixel(sx+x, Scanline, PpuColors.Colors[_memory[PALETTE_SPRITE + palette] % 64]);
                        scanLineColors[sx+x] = PpuColors.Colors[_memory[PALETTE_SPRITE + palette] % 64];
                    }
                }
            }
        }

        public void Write(ushort address, byte value)
        {
            var result = (address - 0x2000) % 8;
            switch (result)
            {
                case 0:
                    break;
                case 1:
                    PPURegisters.PPUMASK.Value = value;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    PPURegisters.PPUADDR = value;
                    if (!_addressFull)
                    {
                        _actualPaletteAddress = value;
                        _addressFull = true;
                    }
                    else
                    {
                        _actualPaletteAddress = (ushort) ((_actualPaletteAddress << 8) + value);
                        _addressFull = false;
                    }

                    break;
                case 7:
                    _memory[_actualPaletteAddress++] = value;
                    break;
            }
        }

        public byte Read(ushort address)
        {
            var result = (address - 0x2000) % 8;
            switch (result)
            {
                case 0:
                    return (byte) PPURegisters.PPUCTRL;
                case 1:
                    return PPURegisters.PPUMASK.Value;
                case 2:
                    var value = (byte) PPURegisters.PPUSTATUS;
                    PPURegisters.PPUSTATUS.VerticalBlank = true;
                    return value;
                case 3:
                    return PPURegisters.OAMADDR;
                case 4:
                    return PPURegisters.OAMDATA;
                case 5:
                    return PPURegisters.PPUSCROLL;
                case 6:
                    return PPURegisters.PPUADDR;
                case 7:
                    return PPURegisters.PPUDATA;
            }

            return 0;
        }

        public void Reset()
        {
            _addressFull = false;
            Cycle = 0;
            Scanline = -1;
            PPURegisters.PPUCTRL.Value = 0;
            PPURegisters.PPUMASK.Value = (byte) (PPURegisters.PPUMASK.Value | 0b1000000);
            PPURegisters.PPUSTATUS.Value &= 0b10111111;
            PPURegisters.OAMADDR = 0;
            PPURegisters.OAMDATA = 0;
            PPURegisters.PPUSCROLL = 0;
            PPURegisters.PPUADDR = 0;
        }

        public void PowerOn()
        {
            _addressFull = false;
            Cycle = 0;
            Scanline = -1;
            PPURegisters.PPUCTRL.Value = 0;
            PPURegisters.PPUMASK.Value = 0;
            PPURegisters.PPUSTATUS.Value &= 0b10111111;
            PPURegisters.OAMADDR = 0;
            PPURegisters.OAMDATA = 0;
            PPURegisters.PPUSCROLL = 0;
            PPURegisters.PPUADDR = 0;
        }
    }
}