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

        private byte[] _memory = new byte[0x4000];
        private int[] _oamOnScanline = new int[8];
        private int _oamOnScanlineCount;
        private ushort _actualWriteAddress;
        private bool _addressFull;
        private OAM [] _oams = new OAM[64];
        private short _cycle { get; set; }
        private short _scanline;
        private readonly IDisplay _display;
        private readonly Cpu _cpu;
        private readonly Bus _bus;
        private readonly uint[] _scanLineColors = new uint[300];
        
        public Ppu(Cpu cpu, Bus bus, IDisplay display)
        {
            _display = display;
            _cpu = cpu;
            _bus = bus;
            for (int i = 0; i < 64; i++) _oams[i] = new OAM();
        }

        public void WriteChr(ushort address, byte data)
        {
            _memory[address] = data;
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
                    _oams[index].SpriteIndex = data;
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

        private byte ReadByte(ushort address)
        {
            return _memory[address];
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
                   _display.DrawPixel(_cycle-1, _scanline, _scanLineColors[_cycle-1 ]);
                }
            }
            else if (_cycle <= 320)
            {
                if (_scanline == -1)
                {
                    _PPURegisters.OAMADDR = 0;
                }
                else
                {
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
                    _display.FrameFinished();
                }
                else
                {
                    UpdateOamOnScanlines();
                }
            }


            _cycle++;
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
            if(_scanline>240) return;
            _oamOnScanlineCount = 0;

            var y = _scanline;
            
            for (var i = 0; i < 64 && _oamOnScanlineCount < 8; i += 1)
            {
                if (_oams[i].Y>0 && _oams[i].Y<0xEF &&   y >= _oams[i].Y && y < _oams[i].Y + 8)
                {
                    _oamOnScanline[_oamOnScanlineCount++] = i;
                }
            }

            for (var i = 0; i < 256; i++)  _scanLineColors[i] = 0;

            var bgY = y >> 3;
            var index = NAMETABLE_TILES + bgY *  32;
            var attrIndex = NAMETABLE_TILES + NAMETABLE_ONLY_LENGTH + bgY * 16; 
            var actualBgY = _scanline % 8;
            var bgX = 0;
            var top = _scanline % 16 <= 7;
            for (int i = 0; i < 32; i++)
            {
                var left = (i % 2) == 0;
                
                var attr = _memory[attrIndex];
                int basePalette = PALETTE_BG;
                if (attr > 0)
                {
                    var topleftPaletta = attr& 0x3;
                    var topRightPaletta = (attr & 0b00001100) >> 2;
                    var bottomLeftPaletta = (attr & 0b00110000) >> 4;
                    var bottomRightPaletta = (attr & 0b11000000) >> 6;
                    if (top && left) basePalette += topleftPaletta * 4;
                    else if(top && !left) basePalette += topRightPaletta * 4;
                    else if (left) basePalette += bottomLeftPaletta * 4;
                    else  basePalette += bottomRightPaletta * 4;
                    
                }
                
                attrIndex += i % 2;
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
                        _scanLineColors[bgX] = PpuColors.Colors[_memory[basePalette + palette] % 64];
                    }
                    bgX++;
                }
                index++;
            }


            for (int j = _oamOnScanlineCount - 1; j >= 0; j--)
            {
                var oam = _oams[_oamOnScanline[j]];
                
                int paletteIndex = oam.Attributes & 0x3;
                int paletteBase = paletteIndex * 4 +  PALETTE_SPRITE;
                bool flipHor = (oam.Attributes & 0b01000000) != 0;
                bool flipVer = (oam.Attributes & 0b10000000) != 0;
                
                int actualY = flipVer ? 7 - (_scanline - oam.Y):(_scanline - oam.Y);
                
                int yIndex = oam.SpriteIndex * 16 + actualY ;
                if (_PPURegisters.PPUCTRL.SpriteTileSelect) yIndex += 0x1000;
                
                int value1 = _memory[yIndex];
                int value2 = _memory[yIndex + 8];

                for (int x = 0; x < 8; x++)
                {
                    var bit = 7 - x;
                    if (flipHor) bit = x;
                    
                    var palette = ((value2 & (1 << bit))!=0?2:0) + ((value1 & (1 << bit)) >> bit);

                    if (palette > 0)
                    {
                        _scanLineColors[oam.X + x] = PpuColors.Colors[_memory[paletteBase + palette] % 64];
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
                    _PPURegisters.PPUMASK.Value = value;
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
                    _PPURegisters.PPUADDR = value;
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
                    return (byte) _PPURegisters.PPUCTRL;
                case 1:
                    return _PPURegisters.PPUMASK.Value;
                case 2:
                    var value = (byte) _PPURegisters.PPUSTATUS;
                    _PPURegisters.PPUSTATUS.VerticalBlank = true;
                    return value;
                case 3:
                    return _PPURegisters.OAMADDR;
                case 4:
                    return _PPURegisters.OAMDATA;
                case 5:
                    return _PPURegisters.PPUSCROLL;
                case 6:
                    return _PPURegisters.PPUADDR;
                case 7:
                    return _PPURegisters.PPUDATA;
            }

            return 0;
        }

        public void Reset()
        {
            _addressFull = false;
            _cycle = 0;
            _scanline = -1;
            _PPURegisters.PPUCTRL.Value = 0;
            _PPURegisters.PPUMASK.Value = (byte) (_PPURegisters.PPUMASK.Value | 0b1000000);
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