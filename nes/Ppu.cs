using System;
using emulator6502;
using NES.Display;
using NES.Registers;
using SFML.Graphics;

namespace NES
{
    public class Ppu : IAddressable
    {
        public ushort From { get; } = 0x2000;
        public ushort To { get; } = 0x2007;

        public bool FrameFinished { get; internal set; }
        public PpuRegisters PPURegisters { get; } = new PpuRegisters();
        
        private const int PALETTE_BG = 0x3F00;
        private const int PALETTE_SPRITE = 0x3F10;
        
        private const int NAMETABLE_TILES = 0X2000;
        private const int NAMETABLE_FULL_LENGTH = 0X400;
        private const int NAMETABLE_ONLY_LENGTH = 0X3C0;
        private const int NAMETABLE_ATTRIBUTES = 0X23C0;

        private byte[] _memory = new byte[0x4000];
        private int[] _oamOnScanline = new int[8];
        private int _oamOnScanlineCount = 0;
        private ushort _actualWriteAddress = 0;
        private bool _addressFull = false;

        private OAM [] Oams = new OAM[64];
        
        public short Cycle { get; internal set; }
        public short Scanline { get; internal set; }

        private readonly IDisplay _display;

        private readonly Cpu _cpu;
        private readonly Bus _bus;

        public Ppu(Cpu cpu, Bus bus, IDisplay display)
        {
            _display = display;
            _cpu = cpu;
            _bus = bus;
            for (int i = 0; i < 64; i++)
            {
                Oams[i] = new OAM();
            }
        }

        public void WriteChr(ushort address, byte data)
        {
            _memory[address] = data;
        }

        private byte _currentHighTile, _currentLowTile, _tileShift;

        
        public void WriteOAM(byte data)
        {
            int index = PPURegisters.OAMADDR >> 2;
            int prop = PPURegisters.OAMADDR - (index << 2);
            switch (prop)
            {
                case 0:
                    Oams[index].Y = data;
                    break;
                case 1:
                    Oams[index].SpriteIndex = data;
                    break;
                case 2:
                    Oams[index].Attributes = data;
                    break;
                case 3:
                    Oams[index].X = data;
                    break;
            }

            PPURegisters.OAMADDR++;
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
                    _display.FrameFinished();
                }
                else
                {
                    UpdateOamOnScanlines();
                }
            }


            Cycle++;
        }

        private class OAM
        {
            public byte Y { get; set; }
            public byte SpriteIndex { get; set; }
            public byte Attributes { get; set; }
            public byte X { get; set; }
        }

        private void UpdateOamOnScanlines()
        {
            if(Scanline>240) return;
            _oamOnScanlineCount = 0;

            int y = Scanline;
            
            for (int i = 0; i < 64 && _oamOnScanlineCount < 8; i += 1)
            {
                if (Oams[i].Y>0 && Oams[i].Y<0xEF &&   y >= Oams[i].Y && y < Oams[i].Y + 8)
                {
                    _oamOnScanline[_oamOnScanlineCount++] = i;
                }
            }

            for (int i = 0; i < 256; i++)  scanLineColors[i] = 0;

            var bgY = y >> 3;
            var index = NAMETABLE_TILES +bgY*32;
            int actualBgY = (Scanline % 8);
            int bgX = 0;
            for (int i = 0; i < 32; i++)
            {
                var spriteIndex =  _memory[index];
                var memIndex = spriteIndex * 16 + actualBgY + 0x1000;

                int value1 = _memory[memIndex];
                int value2 = _memory[memIndex + 8];
                
                for (int x = 0; x < 8; x++)
                {
                    var bit = 7 - x;
                    
                    var palette = ((value2 & (1 << bit))!=0?2:0) + ((value1 & (1 << bit)) >> bit);

                    if (palette > 0)
                    {
                        scanLineColors[bgX] = PpuColors.Colors[_memory[PALETTE_BG + palette] % 64];
                    }
                    bgX++;
                }

                

                index++;
            }


            for (int j = _oamOnScanlineCount - 1; j >= 0; j--)
            {
                var oam = Oams[_oamOnScanline[j]];
                
                int paletteIndex = oam.Attributes & 0x3;
                int paletteBase = paletteIndex * 4 +  PALETTE_SPRITE;
                bool flipHor = (oam.Attributes & 0b01000000) != 0;
                bool flipVer = (oam.Attributes & 0b10000000) != 0;
                
                int actualY = flipVer ? 7 - (Scanline - oam.Y):(Scanline - oam.Y);
                
                int yIndex = oam.SpriteIndex * 16 + actualY ;
                if (PPURegisters.PPUCTRL.SpriteTileSelect) yIndex += 0x1000;
                
                int value1 = _memory[yIndex];
                int value2 = _memory[yIndex + 8];

                for (int x = 0; x < 8; x++)
                {
                    var bit = 7 - x;
                    if (flipHor) bit = x;
                    
                    var palette = ((value2 & (1 << bit))!=0?2:0) + ((value1 & (1 << bit)) >> bit);

                    if (palette > 0)
                    {
                        var p = _memory[paletteBase + palette];
                        if (p == 0x24)
                        {
                            Console.WriteLine("");
                        }
                        scanLineColors[oam.X + x] = PpuColors.Colors[_memory[paletteBase + palette] % 64];
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
                        _actualWriteAddress = value;
                        _addressFull = true;
                    }
                    else
                    {
                        _actualWriteAddress = (ushort) ((_actualWriteAddress << 8) + value);
                        if (_actualWriteAddress == 0x2000)
                        {
                            
                        }
                        _addressFull = false;
                    }

                    break;
                case 7:
                    _memory[_actualWriteAddress++] = value;
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