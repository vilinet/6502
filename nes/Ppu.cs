using emulator6502;

namespace NES
{

    public class Ppu : IAddressable
    {
        public ushort From { get; } = 0x2000;
        public ushort To { get; } = 0x2007;
        
        public bool FrameComplete { get; private  set; }
        private short Cycle { get; set; }
        private short Scanline { get; set; }

        public PPURegisters PPURegisters { get; set; } = new PPURegisters();
   

        public void Clock()
        {
            Cycle++;
            if (Cycle >= 341)
            {
                Cycle = 0;
                Scanline++;
                if (Scanline >= 261)
                {
                    Scanline = -1;
                    FrameComplete = true;
                }
            }
        }

        public void Write(ushort address, byte value)
        {
            
        }

        public byte Read(ushort address)
        {
            var result =  (address-0x2000) % 8;
            switch (result)
            {
                case 0:
                    return (byte)PPURegisters.PPUCTRL;
                case 1:
                    return (byte)PPURegisters.PPUMASK;
                case 2:
                    return (byte)PPURegisters.PPUSTATUS;
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
            PPURegisters.PPUCTRL = 0;
            PPURegisters.PPUMASK = (PPUMASK)((byte)PPURegisters.PPUMASK | 0b1000000);
            PPURegisters.PPUSTATUS = (PPUSTATUS)(0b10111111 & (byte)PPURegisters.PPUSTATUS);
            PPURegisters.OEMADDR = 0;
            PPURegisters.OAMDATA = 0;
            PPURegisters.PPUSCROLL = 0;
            PPURegisters.PPUADDR = 0;
        }

        public void PowerOn()
        {
            PPURegisters.PPUCTRL = 0;
            PPURegisters.PPUMASK = 0;
            PPURegisters.PPUSTATUS = (PPUSTATUS)(0b10111111 & (byte)PPURegisters.PPUSTATUS);
            PPURegisters.OEMADDR = 0;
            PPURegisters.OAMDATA = 0;
            PPURegisters.PPUSCROLL = 0;
            PPURegisters.PPUADDR = 0;
        }
      
    }
}
