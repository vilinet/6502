namespace NES.Registers
{
    public class PpuRegisters
    {
        public PpuCtrlRegister PPUCTRL { get; set; } = new PpuCtrlRegister();
        public PpuMaskRegister PPUMASK { get; set; } = new PpuMaskRegister();
        public PpuStatusRegister PPUSTATUS { get; set; } = new PpuStatusRegister();

        /// <summary>
        /// Relative address in Object memory
        /// </summary>
        public byte OAMADDR { get; set; }

        /// <summary>
        /// 0x[OemDma]00 - 0x[OemDma]FF will be transferred into ppu OamAddr memory
        /// </summary>
        public byte OAMDMA { get; set; }
        /// <summary>
        /// Write OAM data here
        /// </summary>
        public byte OAMDATA { get; set; }
        public byte PPUSCROLL { get; set; }
        public byte PPUADDR { get; set; }
        public byte PPUDATA { get; set; }
    }
}