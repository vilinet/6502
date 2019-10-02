using System;
using System.IO;
using emulator6502;
using NES.Interfaces;
using NES.Registers;

namespace NES
{
    public class Ppu : IAddressable
    {
        public ushort From { get; } = 0x2000;
        public ushort To { get; } = 0x2007;
        public bool FrameFinished { get; internal set; }

        private PpuRegisters _PPURegisters = new PpuRegisters();

        private const int PALETTE_BG = 0x3F00;
        private const int PALETTE_SPRITE = 0x3F10;
        private const int NAMETABLE_TILES = 0X2000;
        private const int NAMETABLE_FULL_LENGTH = 0X400;
        private const int NAMETABLE_ONLY_LENGTH = 0X3C0;
        private const int NAMETABLE_ATTRIBUTES = 0X23C0;

        private byte[] _palettes = new byte[32];
        private byte[,] _patterns = new byte[2, 0x2000];
        private int[] _oamOnScanline = new int[8];
        private byte[,] _nameTables = new byte[2, NAMETABLE_FULL_LENGTH];
        private int _oamOnScanlineCount;
        private ushort _actualAccessAddress;
        private bool _addressFull;
        private Oam[] _oams = new Oam[64];
        private short _cycle { get; set; }
        private short _scanline;
        private readonly IDisplay _display;
        private readonly Cpu _cpu;
        private readonly ICartridge _cartridge;
        private readonly uint[] _scanLineColors = new uint[300];
        private IDebugDisplay _debugDisplay;
        private byte _ppuReadCache;

        public Ppu(Cpu cpu, ICartridge cartridge, IDisplay display, IDebugDisplay debugDisplay)
        {
            _debugDisplay = debugDisplay;
            _display = display;
            _cpu = cpu;
            _cartridge = cartridge;
            for (int i = 0; i < 64; i++) _oams[i] = new Oam();
        }

        public void WriteOAM(byte data)
        {
            int index = _PPURegisters.OAMADDR >> 2;
            int prop = _PPURegisters.OAMADDR - (index << 2);
            switch (prop)
            {
                case 0:
                    _oams[index].Y = data;
                    break;
                case 1:
                    _oams[index].TileIndex = data;
                    break;
                case 2:
                    _oams[index].Attributes = data;
                    break;
                case 3:
                    _oams[index].X = data;
                    break;
            }

            _PPURegisters.OAMADDR++;
        }


        public void Clock()
        {
            if ((!_PPURegisters.PPUMASK.BackgroundEnable && !_PPURegisters.PPUMASK.SpriteEnable) ||
                _scanline >= 240 && _scanline <= 260)
            {
                if (_scanline == 241 && _cycle == 1)
                {
                    _PPURegisters.PPUSTATUS.VerticalBlank = true;
                    _cpu.Nmi();
                }
            }
            else if (_cycle == 0)
            {
                //iddle
            }
            else if (_cycle <= 256)
            {
                if (_scanline != -1 && _scanline != 261)
                {
                    _display.DrawPixel(_cycle - 1, _scanline, _scanLineColors[_cycle - 1]);
                }
            }
            else if (_cycle <= 320)
            {
                if (_scanline == -1)
                {
                    _PPURegisters.OAMADDR = 0;
                }

                //Cycles 257-320
            }
            else if (_cycle <= 336)
            {
                //Cycles 321-336
            }
            else if (_cycle <= 340)
            {
                //Cycles 337-340
            }

            if (_scanline == 304 && _cycle == 280)
            {
                //reload scroll
            }

            if (_cycle >= 341)
            {
                _cycle = 0;
                _scanline++;

                if (_scanline >= 261)
                {
                    _scanline = -1;
                    FrameFinished = true;

                    DebugRender();
                    _display.FrameFinished();
                    _debugDisplay.FrameFinished();
                }
                else
                {
                    UpdateOamOnScanlines();
                }
            }

            _cycle++;
        }

        private void DebugPalettes(int x, int y)
        {
            int ind = 0;
            for (int t = 0; t < 4; t++)
            {
                for (int i = 0; i < 4; i++)
                {
                    DrawRectangle(
                        x + i * 8,
                        t * 8,
                        8, 8, PpuColors.Colors[_palettes[ind]%64]);
                    ind++;
                }
            }
        }

        private void DrawRectangle(int x, int y, int width, int height, uint color)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    var str = color.ToString("X6");
                    _debugDisplay.DrawPixel(i, j, color);
                }
            }
        }

        private void DebugRender()
        {
            
            DrawSprite(GetSprite(0, 0x10, 1, bg: true), 300, 50);
        }

        private void DebugAttributes()
        {
            int index = 0;
             for(int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    Console.Write(_nameTables[0, NAMETABLE_ONLY_LENGTH+index].ToString("X2") + " ");
                    index += 1;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            System.Console.WriteLine();
        }

        private void DebugNametable(int x, int y)
        {
            var startPos = NAMETABLE_TILES;
            for (int j = 0; j < 30; j++)
            {
                for (int i = 0; i < 32; i++)
                {
                    Console.Write(ReadPpu(startPos++).ToString("X2") + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

        }

        private void DebugOam(int x, int y)
        {
            int index = 0;
            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    var sprite = GetSprite(_oams[index++]);
                    DrawSprite(sprite, x + i * 8, y + j * 8);
                }
            }
        }

        private void DrawSprite(NesSprite sprite, int x, int y)
        {
            int index = 0;
            for (int j = y; j < y + 8; j++)
            {
                for (int i = x; i < x + 8; i++)
                {
                    _debugDisplay.DrawPixel(i, j, sprite.data[index++]);
                }
            }
        } 

        private NesSprite GetSprite(Oam oam)
        {
            bool flipHor = (oam.Attributes & 0b01000000) != 0;
            bool flipVer = (oam.Attributes & 0b10000000) != 0;
            return GetSprite(1, oam.TileIndex, oam.Attributes & 3, flipVer, flipHor);
        }

        private NesSprite GetSprite(int bankIndex, int spriteIndex, int paletteIndex = -1, bool bg = false, bool flipVertical = false, bool flipHorizontal = false)
        {
            if (paletteIndex == -1) paletteIndex = 1;
            var sprite = new NesSprite();

            var paletteBase = !bg ? paletteIndex * 4 + PALETTE_SPRITE : paletteIndex * 4 + PALETTE_BG;

            var addr = spriteIndex * 16 + bankIndex * 0x1000;
            var index = 0;

            for (int j = 0; j < 8; j++)
            {
                int value1 = ReadPpu(addr);
                int value2 = ReadPpu(addr + 8);

                for (int x = 0; x < 8; x++)
                {
                    var palette = ((value2 & 1) << 1) + (value1 & 1);

                    sprite.data[index + (7 - x)] = PpuColors.Colors[ReadPpu(paletteBase + palette) % 64];

                    value1 >>= 1;
                    value2 >>= 1;

                }

                index += 8;
                addr++;
            }

            return sprite;
        }

        private class Oam
        {
            public byte Y { get; set; }
            public byte TileIndex { get; set; }
            public byte Attributes { get; set; }
            public byte X { get; set; }
        }

        private void UpdateOamOnScanlines()
        {
            if (_scanline > 240) return;
            _oamOnScanlineCount = 0;

            var y = _scanline;

            for (var i = 0; i < 64 && _oamOnScanlineCount < 8; i += 1)
            {
                if (_oams[i].Y > 0 && _oams[i].Y < 0xEF && y >= _oams[i].Y && y < _oams[i].Y + 8)
                    _oamOnScanline[_oamOnScanlineCount++] = i;
            }

            for (var i = 0; i < 256; i++) _scanLineColors[i] = 0;

            var bgY = y >> 3;
            var index = NAMETABLE_TILES + bgY * 32;
            var attrIndex = NAMETABLE_TILES + NAMETABLE_ONLY_LENGTH + ((bgY >> 2) << 3);
            var actualBgY = y % 8;
            var top = y % 64 <= 31;
            var bgX = 0;

            for (int i = 0; i < 32; i++)
            {
                var left = (i % 4) <= 1;
                if (i > 0 && i % 4 == 0)
                {
                    attrIndex += 1;
                }
                var attr = ReadPpu(attrIndex);
                var tileIndex = ReadPpu(index);
                int basePalette = PALETTE_BG;

                if (attr > 0)
                {
                    var topleftPaletta = attr & 0x3;
                    var topRightPaletta = (attr & 0b00001100) >> 2;
                    var bottomLeftPaletta = (attr & 0b00110000) >> 4;
                    var bottomRightPaletta = (attr & 0b11000000) >> 6;
                    if (top && left) basePalette += topleftPaletta * 4;
                    else if (top && !left) basePalette += topRightPaletta * 4;
                    else if (left) basePalette += bottomLeftPaletta * 4;
                    else basePalette += bottomRightPaletta * 4;
                }
                
                var memIndex = tileIndex * 16 + actualBgY + 0x1000;

                int value1 = ReadPpu(memIndex);
                int value2 = ReadPpu(memIndex + 8);

                if (_PPURegisters.PPUMASK.BackgroundEnable)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        var palette = (value2 & 1) * 2 + (value1 & 1);
                        value1 >>= 1;
                        value2 >>= 1;

                        _scanLineColors[bgX + (7 - x)] = PpuColors.Colors[ReadPpu(basePalette + palette) % 64];
                    }
                }

                bgX += 8;
                index++;
            }

            for (int j = _oamOnScanlineCount - 1; j >= 0; j--)
            {
                var oam = _oams[_oamOnScanline[j]];

                int paletteIndex = oam.Attributes & 0x3;
                int paletteBase = paletteIndex * 4 + PALETTE_SPRITE;
                bool flipHor = (oam.Attributes & 0b01000000) != 0;
                bool flipVer = (oam.Attributes & 0b10000000) != 0;

                int actualY = flipVer ? 7 - (_scanline - oam.Y) : (_scanline - oam.Y);

                int yIndex = oam.TileIndex * 16 + actualY;
                if (_PPURegisters.PPUCTRL.SpriteBank) yIndex += 0x1000;

                int value1 = ReadPpu(yIndex);
                int value2 = ReadPpu(yIndex + 8);
                if (_PPURegisters.PPUMASK.SpriteEnable)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        var palette = (value2 & 1) * 2 + (value1 & 1);
                        value1 >>= 1;
                        value2 >>= 1;


                        if (palette > 0)
                        {
                            if (flipHor)
                            {
                                _scanLineColors[oam.X + x] = PpuColors.Colors[ReadPpu(paletteBase + palette) % 64];

                            }
                            else
                            {
                                _scanLineColors[oam.X + (7 - x)] = PpuColors.Colors[ReadPpu(paletteBase + palette) % 64];
                            }
                        }
                    }
                }
            }
        }


        public byte Read(ushort address)
        {
            var result = (address - 0x2000) % 8;
            switch (result)
            {
                case 0:
                    return 0;
                case 1:
                    return 0;
                case 2:
                    var value = (byte)_PPURegisters.PPUSTATUS;
                    _PPURegisters.PPUSTATUS.VerticalBlank = true;
                    _actualAccessAddress = 0;
                    _addressFull = false;
                    return value;
                case 3:
                    return 0;
                case 4:
                    return 0;
                case 5:
                    return 0;
                case 6:

                    return 0;
                case 7:
                    var data = _ppuReadCache;
                    _ppuReadCache = ReadPpu(_actualAccessAddress);
                    if (_actualAccessAddress >= 0x3F00) data = _ppuReadCache;
                    
                    _actualAccessAddress += (ushort)(_PPURegisters.PPUCTRL.IncrementMode ? 32 : 1);

                    return data;
            }

            return 0;
        }

        public void Write(ushort address, byte value)
        {
            var result = (address - 0x2000) % 8;
            switch (result)
            {
                case 0:
                    _PPURegisters.PPUCTRL.Value = value;
                    break;
                case 1:
                    _PPURegisters.PPUMASK.Value = value;
                    break;
                case 2:
                    _PPURegisters.PPUSTATUS.Value = value;
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    _PPURegisters.PPUADDR = value;
                    if (!_addressFull)
                    {
                        _actualAccessAddress = value;
                        _addressFull = true;
                    }
                    else
                    {
                        _actualAccessAddress = (ushort)((_actualAccessAddress << 8) + value);
                        _addressFull = false;
                    }

                    break;
                case 7:
                    WritePpu(_actualAccessAddress, value);
                    _actualAccessAddress += (ushort)(_PPURegisters.PPUCTRL.IncrementMode ? 32 : 1);
                    break;
            }
        }

        private byte ReadPpu(int address)
        {
            address &= 0x3FFF;
            int val = _cartridge.ReadPpu((ushort)address);
            if (val != -1) return (byte)val;

            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return _patterns[(address & 0x1000) >> 1, address & 0x0FFF];
            }
            else if (address >= 0x2000 && address <= 0x3EFF)
            {
                address &= 0x0FFF;
                if (_cartridge.Mirroring == Mirroring.Vertical)
                {
                    if (address >= 0x0000 && address <= 0x03FF)
                        return _nameTables[0, address & 0x03FF];
                    if (address >= 0x0400 && address <= 0x07FF)
                        return _nameTables[1, address & 0x03FF];
                    if (address >= 0x0800 && address <= 0x0BFF)
                        return _nameTables[0, address & 0x03FF];
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        return _nameTables[1, address & 0x03FF];
                }
                if (_cartridge.Mirroring == Mirroring.Horizontal)
                {
                    // Horizontal
                    if (address >= 0x0000 && address <= 0x03FF)
                        return _nameTables[0, address & 0x03FF];
                    if (address >= 0x0400 && address <= 0x07FF)
                        return _nameTables[0, address & 0x03FF];
                    if (address >= 0x0800 && address <= 0x0BFF)
                        return _nameTables[1, address & 0x03FF];
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        return _nameTables[1, address & 0x03FF];
                }
            }
            else if (address >= 0x3F00 && address <= 0x3FFF)
            {
                address &= 0x001F;
                if (address == 0x0010) address = 0x0000;
                if (address == 0x0014) address = 0x0004;
                if (address == 0x0018) address = 0x0008;
                if (address == 0x001C) address = 0x000C;
                return (byte)(_palettes[address] & (_PPURegisters.PPUMASK.Grayscale ? 0x30 : 0x3F));
            }
            return 0;
        }

        private void WritePpu(ushort address, byte value)
        {
            address &= 0x3FFF;


            if (_cartridge.WritePpu(address, value))
            {

            }
            else if (address >= 0x0000 && address <= 0x1FFF)
            {
                _patterns[(address & 0x1000) >> 12, address & 0x0FFF] = value;
            }
            else if (address >= 0x2000 && address <= 0x3EFF)
            {
                address &= 0x0FFF;
                if(address == 960 && value == 255)
                {
                    
                    Console.WriteLine(value);
                }
                if (_cartridge.Mirroring == Mirroring.Vertical)
                {
                    if (address >= 0x0000 && address <= 0x03FF)
                        _nameTables[0, address & 0x03FF] = value;
                    if (address >= 0x0400 && address <= 0x07FF)
                        _nameTables[1, address & 0x03FF] = value;
                    if (address >= 0x0800 && address <= 0x0BFF)
                        _nameTables[0, address & 0x03FF] = value;
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        _nameTables[1, address & 0x03FF] = value;
                }
                if (_cartridge.Mirroring == Mirroring.Horizontal)
                {
                    if (address >= 0x0000 && address <= 0x03FF)
                        _nameTables[0, address & 0x03FF] = value;
                    if (address >= 0x0400 && address <= 0x07FF)
                        _nameTables[0, address & 0x03FF] = value;
                    if (address >= 0x0800 && address <= 0x0BFF)
                        _nameTables[1, address & 0x03FF] = value;
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        _nameTables[1, address & 0x03FF] = value;
                }
            }
            else if (address >= 0x3F00)
            {
                address &= 0x001F;
                if (address == 0x0010) address = 0x0000;
                if (address == 0x0014) address = 0x0004;
                if (address == 0x0018) address = 0x0008;
                if (address == 0x001C) address = 0x000C;
                _palettes[address] = value;
            }
        }

        public void Reset()
        {
            _addressFull = false;
            _cycle = 0;
            _scanline = -1;
            _PPURegisters.PPUCTRL.Value = 0;
            _PPURegisters.PPUMASK.Value = (byte)(_PPURegisters.PPUMASK.Value | 0b1000000);
            _PPURegisters.PPUSTATUS.Value &= 0b10111111;
            _PPURegisters.OAMADDR = 0;
            _PPURegisters.OAMDATA = 0;
            _PPURegisters.PPUSCROLL = 0;
            _PPURegisters.PPUADDR = 0;
        }

        public void PowerOn()
        {
            _addressFull = false;
            _cycle = 0;
            _scanline = -1;
            _PPURegisters.PPUCTRL.Value = 0;
            _PPURegisters.PPUMASK.Value = 0;
            _PPURegisters.PPUSTATUS.Value &= 0b10111111;
            _PPURegisters.OAMADDR = 0;
            _PPURegisters.OAMDATA = 0;
            _PPURegisters.PPUSCROLL = 0;
            _PPURegisters.PPUADDR = 0;
        }
    }
}