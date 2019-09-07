using System;
using emulator6502;
using NES.Display;

namespace NES
{
    public class Ppu : IAddressable
    {
        private bool oddFrame = false;
        public ushort From { get; } = 0x2000;
        public ushort To { get; } = 0x2007;

        public bool FrameFinished { get; internal set; }
        public PPURegisters PPURegisters { get; } = new PPURegisters();

        public short Cycle { get; internal set; }
        public short Scanline { get; internal set; }

        private readonly IDisplay _display;

        private int color = 12;
        private readonly Cpu _cpu;

        public Ppu(Cpu cpu, IDisplay display)
        {
            _display = display;
            _cpu = cpu;
        }

        public void Clock()
        {
            if ((!PPURegisters.PPUMASK.ShowBackground && !PPURegisters.PPUMASK.ShowSprites)||  Scanline >= 240 && Scanline <= 260)
            {
                if (Scanline == 241 && Cycle == 1)
                {
                    PPURegisters.PPUSTATUS = (PPUSTATUS) ((byte) PPURegisters.PPUSTATUS | (byte) PPUSTATUS.V);
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
                    //Visible tiles
                    _display.DrawPixel(Cycle - 1, Scanline, PpuColors.Colors[(color++ ) % 64]);
                }
            }
            else if (Cycle <= 320)
            {
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
            }

            Cycle++;
        }

        public void Write(ushort address, byte value)
        {
            var result = (address - 0x2000) % 8;
            if (result == 1)
            {
                PPURegisters.PPUMASK.Value = value;
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
                    PPURegisters.PPUSTATUS &= (PPUSTATUS) ((byte) PPURegisters.PPUSTATUS & (0xFF & (byte) PPUSTATUS.V));
                    return value;
                case 3:
                    return PPURegisters.OEMADDR;
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
            Cycle = 0;
            Scanline = -1;
            PPURegisters.PPUCTRL = 0;
            PPURegisters.PPUMASK.Value = (byte)(PPURegisters.PPUMASK.Value | 0b1000000);
            PPURegisters.PPUSTATUS = (PPUSTATUS) (0b10111111 & (byte) PPURegisters.PPUSTATUS);
            PPURegisters.OEMADDR = 0;
            PPURegisters.OAMDATA = 0;
            PPURegisters.PPUSCROLL = 0;
            PPURegisters.PPUADDR = 0;
        }

        public void PowerOn()
        {
            Cycle = 0;
            Scanline = -1;
            PPURegisters.PPUCTRL = 0;
            PPURegisters.PPUMASK.Value = 0;
            PPURegisters.PPUSTATUS = (PPUSTATUS) (0b10111111 & (byte) PPURegisters.PPUSTATUS);
            PPURegisters.OEMADDR = 0;
            PPURegisters.OAMDATA = 0;
            PPURegisters.PPUSCROLL = 0;
            PPURegisters.PPUADDR = 0;
        }
    }
}