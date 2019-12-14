﻿using emulator6502;
using NES.Interfaces;
using NES.Registers;

namespace NES
{
    public class Ppu : IAddressable
    {
        public ushort From { get; } = 0x2000;
        public ushort To { get; } = 0x2007;
        public bool FrameFinished { get; internal set; }

        public const ushort PALETTE = 0x3F00;
        public const ushort PALETTE_SPRITE = 0x3F10;
        public const ushort NAMETABLE = 0X2000;
        public const ushort NAMETABLE_FULL_LENGTH = 0X400;
        public const ushort NAMETABLE_ONLY_LENGTH = 0X3C0;
        public const ushort NAMETABLE_ATTRIBUTES = 0X23C0;

        private readonly IDisplay _display;
        private readonly Cpu _cpu;
        private readonly ICartridge _cartridge;

        private readonly PpuRegisters _PPURegisters = new PpuRegisters();
        public PpuRegisters PPURegisters => _PPURegisters;

        private readonly byte[] _palettes = new byte[32];
        public byte[] Palettes => _palettes;
        private readonly byte[,] _patterns = new byte[2, 0x2000];
        private readonly byte[,] _nameTables = new byte[2, NAMETABLE_FULL_LENGTH];

        private readonly Oam[] _oam = new Oam[64];
        public Oam[] Oam => _oam;

        private readonly LoopyRegister _vram = new LoopyRegister();
        private readonly LoopyRegister _tram = new LoopyRegister();
        private readonly Oam[] _spriteScanline = new Oam[8];
        private readonly byte[] _spriteShifterPatternLow = new byte[8];
        private readonly byte[] _spriteShifterPatternHigh = new byte[8];

        private bool _addressFull;
        private int _cycle { get; set; }
        private short _scanline;
        private byte _ppuReadCache;
        private byte _fineX;
        private byte _bgNextTileId;
        private byte _bgNextTileAttribute;
        private byte _bgNextTileLow;
        private byte _bgNextTileHigh;
        private ushort _bgShifterPatternLow;
        private ushort _bgShifterPatternHigh;
        private ushort _bgShifterAttributeLow;
        private ushort _bgShifterAttributeHigh;
        private int _spriteCount;
        private bool _spriteZeroHitPossible;
        private bool _spriteZeroBeingRendered;

        public Ppu(Cpu cpu, ICartridge cartridge, IDisplay display )
        {
            _display = display;
            _cpu = cpu;
            _cartridge = cartridge;
            for (int i = 0; i < 64; i++) _oam[i] = new Oam();
        }

        public void WriteOAM(byte data)
        {
            int index = _PPURegisters.OAMADDR >> 2;
            int prop = _PPURegisters.OAMADDR - (index << 2);
            switch (prop)
            {
                case 0:
                    _oam[index].Y = data;
                    break;
                case 1:
                    _oam[index].Id = data;
                    break;
                case 2:
                    _oam[index].Attributes = data;
                    break;
                case 3:
                    _oam[index].X = data;
                    break;
            }

            _PPURegisters.OAMADDR++;
        }

        public void Clock()
        {
            if (_scanline >= -1 && _scanline < 240)
            {
                if (_scanline == 0 && _cycle == 0)
                {
                    _cycle = 1;
                }
                else if (_scanline == -1 && _cycle == 1)
                {
                    _PPURegisters.PPUSTATUS.VerticalBlank = false;
                    // Clear Shifters
                    for (int i = 0; i < 8; i++)
                    {
                        _spriteShifterPatternLow[i] = 0;
                        _spriteShifterPatternHigh[i] = 0;
                        _PPURegisters.PPUSTATUS.SpriteOverflow = false;
                        _PPURegisters.PPUSTATUS.SpriteHit = false;
                    }
                }
                else if ((_cycle >= 2 && _cycle < 258) || (_cycle >= 321 && _cycle < 338))
                {
                    UpdateShifters();

                    switch ((_cycle - 1) % 8)
                    {
                        case 0:
                            LoadBgShifters();
                            _bgNextTileId = ReadPpu(NAMETABLE | (_vram.Value & 0x0FFF));
                            break;
                        case 2:
                            _bgNextTileAttribute = ReadPpu(NAMETABLE_ATTRIBUTES | (_vram.NametableY << 11)
                                                 | (_vram.NametableX << 10)
                                                 | ((_vram.CoarseY >> 2) << 3)
                                                 | (_vram.CoarseX >> 2));

                            if ((_vram.CoarseY & 0x02) != 0) _bgNextTileAttribute >>= 4;
                            if ((_vram.CoarseX & 0x02) != 0) _bgNextTileAttribute >>= 2;

                            _bgNextTileAttribute &= 0x03;
                            break;
                        case 4:
                            _bgNextTileLow = ReadPpu(((_PPURegisters.PPUCTRL.PatternBackground ? 1 : 0) << 12)
                                           + (_bgNextTileId << 4) + _vram.FineY);
                            break;
                        case 6:
                            _bgNextTileHigh = ReadPpu(((_PPURegisters.PPUCTRL.PatternBackground ? 1 : 0) << 12)
                                                    + (_bgNextTileId << 4) + (_vram.FineY) + 8);
                            break;
                        case 7:
                            IncrementScrollX();
                            break;
                        default:
                            break;
                    }
                }

                if (_cycle == 256)
                {
                    IncrementScrollY();
                }
                else if (_cycle == 257)
                {
                    LoadBgShifters();
                    TransferAddressX();
                }
                else if (_cycle == 338 || _cycle == 340)
                {
                    _bgNextTileId = ReadPpu(NAMETABLE | (_vram.Value & 0x0FFF));
                }
                else if (_scanline == -1 && _cycle >= 280 && _cycle < 305)
                {
                    TransferAddressY();
                }
                if (_cycle == 257 && _scanline >= 0)
                {
                    for (int i = 0; i < 8; i++) _spriteScanline[i] = new Oam();
                    _spriteCount = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        _spriteShifterPatternLow[i] = 0;
                        _spriteShifterPatternHigh[i] = 0;
                    }

                    byte index = 0;

                    _spriteZeroHitPossible = false;

                    while (index < 64 && _spriteCount < 9)
                    {
                        int diff = _scanline - _oam[index].Y;

                        if (diff >= 0 && diff < (_PPURegisters.PPUCTRL.SpriteHeight ? 16 : 8))
                        {
                            if (_spriteCount < 8)
                            {
                                if (index == 0)
                                {
                                    _spriteZeroHitPossible = true;
                                }

                                _spriteScanline[_spriteCount] = _oam[index];
                                _spriteCount++;
                            }
                        }

                        index++;
                    }
                    _PPURegisters.PPUSTATUS.SpriteOverflow = (_spriteCount > 8);
                }
                if (_cycle == 340)
                {
                    for (byte i = 0; i < _spriteCount; i++)
                    {
                        byte sprite_pattern_bits_lo, sprite_pattern_bits_hi;
                        ushort sprite_pattern_addr_lo, sprite_pattern_addr_hi;

                        if (!_PPURegisters.PPUCTRL.SpriteHeight)
                        {
                            if ((_spriteScanline[i].Attributes & 0x80) == 0)
                            {
                                sprite_pattern_addr_lo = (ushort)(
                                    ((_PPURegisters.PPUCTRL.PatternSprite ? 1 : 0) << 12)
                                    | (_spriteScanline[i].Id << 4)
                                    | (_scanline - _spriteScanline[i].Y));

                            }
                            else
                            {
                                sprite_pattern_addr_lo = (ushort)(
                                    ((_PPURegisters.PPUCTRL.PatternSprite ? 1 : 0) << 12)
                                    | (_spriteScanline[i].Id << 4)
                                    | (7 - (_scanline - _spriteScanline[i].Y)));
                            }

                        }
                        else
                        {
                            if ((_spriteScanline[i].Attributes & 0x80) == 0)
                            {
                                if (_scanline - _spriteScanline[i].Y < 8)
                                {
                                    sprite_pattern_addr_lo = (ushort)(
                                        ((_spriteScanline[i].Id & 0x01) << 12)
                                        | ((_spriteScanline[i].Id & 0xFE) << 4)
                                        | ((_scanline - _spriteScanline[i].Y) & 0x07));
                                }
                                else
                                {
                                    sprite_pattern_addr_lo = (ushort)(
                                        ((_spriteScanline[i].Id & 0x01) << 12)
                                        | (((_spriteScanline[i].Id & 0xFE) + 1) << 4)
                                        | ((_scanline - _spriteScanline[i].Y) & 0x07));
                                }
                            }
                            else
                            {
                                if (_scanline - _spriteScanline[i].Y < 8)
                                {
                                    sprite_pattern_addr_lo = (ushort)(
                                        ((_spriteScanline[i].Id & 0x01) << 12)
                                        | (((_spriteScanline[i].Id & 0xFE) + 1) << 4)
                                        | (7 - (_scanline - _spriteScanline[i].Y) & 0x07));
                                }
                                else
                                {
                                    sprite_pattern_addr_lo =
                                        (ushort)(((_spriteScanline[i].Id & 0x01) << 12)
                                        | ((_spriteScanline[i].Id & 0xFE) << 4)
                                        | (7 - (_scanline - _spriteScanline[i].Y) & 0x07));
                                }
                            }
                        }

                        sprite_pattern_addr_hi = (ushort)(sprite_pattern_addr_lo + 8);

                        sprite_pattern_bits_lo = ReadPpu(sprite_pattern_addr_lo);
                        sprite_pattern_bits_hi = ReadPpu(sprite_pattern_addr_hi);

                        if ((_spriteScanline[i].Attributes & 0x40) > 0)
                        {
                            sprite_pattern_bits_lo = FlipByte(sprite_pattern_bits_lo);
                            sprite_pattern_bits_hi = FlipByte(sprite_pattern_bits_hi);
                        }

                        _spriteShifterPatternLow[i] = sprite_pattern_bits_lo;
                        _spriteShifterPatternHigh[i] = sprite_pattern_bits_hi;
                    }
                }

            }
            else if (_scanline == 240)
            {
                //do nothing
            }
            else if (_scanline == 241 && _cycle == 1)
            {
                _PPURegisters.PPUSTATUS.VerticalBlank = true;
                if (_PPURegisters.PPUCTRL.NmiEnabled) _cpu.Nmi();
            }

            if (_cycle > 0 && _scanline >= 0 && _cycle - 1 < 256 && _scanline < 240)
            {

                byte bg_pixel = 0x00;
                byte bg_palette = 0x00;

                if (_PPURegisters.PPUMASK.BackgroundEnable)
                {
                    var bit_mux = (ushort)(0x8000 >> _fineX);

                    var p0_pixel = (_bgShifterPatternLow & bit_mux) != 0 ? 1 : 0;
                    var p1_pixel = (_bgShifterPatternHigh & bit_mux) != 0 ? 1 : 0;

                    bg_pixel = (byte)((p1_pixel << 1) | p0_pixel);

                    var bg_pal0 = (_bgShifterAttributeLow & bit_mux) != 0 ? 1 : 0;
                    var bg_pal1 = (_bgShifterAttributeHigh & bit_mux) != 0 ? 1 : 0;
                    bg_palette = (byte)((bg_pal1 << 1) | bg_pal0);
                }

                byte fg_pixel = 0x00;
                byte fg_palette = 0x00;
                byte fg_priority = 0x00;


                if (_PPURegisters.PPUMASK.SpriteEnable)
                {
                    _spriteZeroBeingRendered = false;

                    for (byte i = 0; i < _spriteCount; i++)
                    {
                        if (_spriteScanline[i].X == 0)
                        {
                            byte fg_pixel_lo = (byte)((_spriteShifterPatternLow[i] & 0x80) > 0 ? 1 : 0);
                            byte fg_pixel_hi = (byte)((_spriteShifterPatternHigh[i] & 0x80) > 0 ? 1 : 0);
                            fg_pixel = (byte)((fg_pixel_hi << 1) | fg_pixel_lo);
                            fg_palette = (byte)((_spriteScanline[i].Attributes & 0x03) + 0x04);
                            fg_priority = (byte)((_spriteScanline[i].Attributes & 0x20) == 0 ? 1 : 0);

                            if (fg_pixel != 0)
                            {
                                if (i == 0) _spriteZeroBeingRendered = true;
                                break;
                            }
                        }
                    }
                }

                byte pixel = 0x00;
                byte palette = 0x00;

                if (bg_pixel == 0 && fg_pixel == 0)
                {
                    pixel = 0x00;
                    palette = 0x00;
                }
                else if (bg_pixel == 0 && fg_pixel > 0)
                {
                    pixel = fg_pixel;
                    palette = fg_palette;
                }
                else if (bg_pixel > 0 && fg_pixel == 0)
                {
                    pixel = bg_pixel;
                    palette = bg_palette;
                }
                else if (bg_pixel > 0 && fg_pixel > 0)
                {
                    if (fg_priority == 1)
                    {
                        pixel = fg_pixel;
                        palette = fg_palette;
                    }
                    else
                    {
                        pixel = bg_pixel;
                        palette = bg_palette;
                    }

                    if (_spriteZeroHitPossible && _spriteZeroBeingRendered)
                    {
                        if (_PPURegisters.PPUMASK.SpriteEnable && _PPURegisters.PPUMASK.BackgroundEnable)
                        {
                            if (!(_PPURegisters.PPUMASK.BackgroundLeftColumnEnable || _PPURegisters.PPUMASK.SpriteLeftColumnEnable))
                            {
                                if (_cycle >= 9 && _cycle < 258) _PPURegisters.PPUSTATUS.SpriteHit = true;
                            }
                            else
                            {
                                if (_cycle >= 1 && _cycle < 258) _PPURegisters.PPUSTATUS.SpriteHit = true;
                            }
                        }
                    }
                }

                _display.DrawPixel(_cycle - 1, _scanline, GetColorFromPalette(palette, pixel));
            }

            _cycle++;
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
            }
        }

        private byte FlipByte(byte b)
        {
            b = (byte)((b & 0xF0) >> 4 | (b & 0x0F) << 4);
            b = (byte)((b & 0xCC) >> 2 | (b & 0x33) << 2);
            b = (byte)((b & 0xAA) >> 1 | (b & 0x55) << 1);
            return b;
        }

        private uint GetColorFromPalette(int palette, int pixel)
        {
            return PpuColors.Colors[ReadPpu(PALETTE + (palette << 2) + pixel) & 0x3F];
        }

        private void TransferAddressX()
        {
            if (_PPURegisters.PPUMASK.BackgroundEnable || _PPURegisters.PPUMASK.SpriteEnable)
            {
                _vram.NametableX = _tram.NametableX;
                _vram.CoarseX = _tram.CoarseX;
            }
        }
        private void TransferAddressY()
        {
            if (_PPURegisters.PPUMASK.BackgroundEnable || _PPURegisters.PPUMASK.SpriteEnable)
            {
                _vram.NametableY = _tram.NametableY;
                _vram.CoarseY = _tram.CoarseY;
                _vram.FineY = _tram.FineY;
            }
        }

        private void IncrementScrollX()
        {
            if (_PPURegisters.PPUMASK.BackgroundEnable || _PPURegisters.PPUMASK.SpriteEnable)
            {
                if (_vram.CoarseX == 31)
                {
                    _vram.CoarseX = 0;
                    _vram.NametableX = (byte)(~_vram.NametableX & 1);
                }
                else
                {
                    _vram.CoarseX++;
                }
            }
        }

        void IncrementScrollY()
        {
            if (_PPURegisters.PPUMASK.BackgroundEnable || _PPURegisters.PPUMASK.SpriteEnable)
            {
                if (_vram.FineY < 7)
                {
                    _vram.FineY++;
                }
                else
                {
                    _vram.FineY = 0;
                    if (_vram.CoarseY == 29)
                    {
                        _vram.CoarseY = 0;
                        _vram.NametableY = (byte)(~_vram.NametableY & 1);
                    }
                    else if (_vram.CoarseY == 31)
                    {
                        _vram.CoarseY = 0;
                    }
                    else
                    {
                        _vram.CoarseY++;
                    }
                }
            }
        }

        void UpdateShifters()
        {
            if (_PPURegisters.PPUMASK.BackgroundEnable)
            {
                _bgShifterAttributeHigh <<= 1;
                _bgShifterAttributeLow <<= 1;

                _bgShifterPatternHigh <<= 1;
                _bgShifterPatternLow <<= 1;
            }
            if (_PPURegisters.PPUMASK.SpriteEnable && _cycle >= 1 && _cycle < 258)
            {
                for (int i = 0; i < _spriteCount; i++)
                {
                    if (_spriteScanline[i].X > 0)
                    {
                        _spriteScanline[i].X--;
                    }
                    else
                    {
                        _spriteShifterPatternLow[i] <<= 1;
                        _spriteShifterPatternHigh[i] <<= 1;
                    }
                }
            }
        }

        private void LoadBgShifters()
        {
            _bgShifterPatternLow = (ushort)((_bgShifterPatternLow & 0xFF00) | _bgNextTileLow);
            _bgShifterPatternHigh = (ushort)((_bgShifterPatternHigh & 0xFF00) | _bgNextTileHigh);
            _bgShifterAttributeLow = (ushort)((_bgShifterAttributeLow & 0xFF00) | (((_bgNextTileAttribute & 0b01) != 0) ? 0xFF : 0x00));
            _bgShifterAttributeHigh = (ushort)((_bgShifterAttributeHigh & 0xFF00) | (((_bgNextTileAttribute & 0b10) != 0) ? 0xFF : 0x00));
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
                    var value = (_PPURegisters.PPUSTATUS & 0xE0) | (_ppuReadCache & 0x1F);
                    _PPURegisters.PPUSTATUS.VerticalBlank = true;
                    _addressFull = false;
                    return (byte)value;
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
                    _ppuReadCache = ReadPpu(_vram.Value);
                    if (_vram.Value >= PALETTE) data = _ppuReadCache;
                    _vram.Value += (ushort)(_PPURegisters.PPUCTRL.IncrementMode ? 32 : 1);
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
                    _tram.NametableX = (byte)(_PPURegisters.PPUCTRL.NametableX ? 1 : 0);
                    _tram.NametableY = (byte)(_PPURegisters.PPUCTRL.NametableY ? 1 : 0);
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
                    if (!_addressFull)
                    {
                        _fineX = (byte)(value & 7);
                        _tram.CoarseX = (byte)(value >> 3);
                        _addressFull = true;
                    }
                    else
                    {
                        _tram.FineY = (byte)(value & 7);
                        _tram.CoarseY = (byte)(value >> 3);
                        _addressFull = false;
                    }
                    break;
                case 6:
                    if (!_addressFull)
                    {
                        _tram.Value = (ushort)(((value & 0x3F) << 8) | (_tram.Value & 0x00FF));
                        _addressFull = true;
                    }
                    else
                    {
                        _tram.Value = (ushort)((_tram.Value & 0xFF00) | value);

                        _vram.Value = _tram.Value;
                        _addressFull = false;
                    }

                    break;
                case 7:
                    WritePpu(_vram.Value, value);
                    _vram.Value += (ushort)(_PPURegisters.PPUCTRL.IncrementMode ? 32 : 1);
                    break;
            }
        }

        public byte ReadPpu(int address)
        {
            address &= 0x3FFF;
            int val = _cartridge.ReadPpu((ushort)address);
            if (val != -1) return (byte)val;

            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return _patterns[(address & 0x1000) >> 1, address & 0x0FFF];
            }
            else if (address >= NAMETABLE && address <= 0x3EFF)
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
            else if (address >= PALETTE && address <= 0x3FFF)
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
            else if (address <= 0x1FFF)
            {
                _patterns[(address & 0x1000) >> 12, address & 0x0FFF] = value;
            }
            else if (address >= NAMETABLE && address <= 0x3EFF)
            {
                address &= 0x0FFF;
                if (_cartridge.Mirroring == Mirroring.Vertical)
                {
                    if (address <= 0x03FF)
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
                    if (address <= 0x03FF)
                        _nameTables[0, address & 0x03FF] = value;
                    if (address >= 0x0400 && address <= 0x07FF)
                        _nameTables[0, address & 0x03FF] = value;
                    if (address >= 0x0800 && address <= 0x0BFF)
                        _nameTables[1, address & 0x03FF] = value;
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        _nameTables[1, address & 0x03FF] = value;
                }
            }
            else if (address >= PALETTE)
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
            _bgNextTileId = _bgNextTileAttribute = _bgNextTileLow = _bgNextTileHigh = 0;
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

    public class NesSprite
    {
        public uint[] data = new uint[8 * 8];
    }
}