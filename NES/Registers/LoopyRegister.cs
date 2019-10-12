namespace NES.Registers
{
    internal class LoopyRegister
    {
        public byte CoarseX { get; set; }
        public byte CoarseY { get; set; }
        public byte NametableX { get; set; }
        public byte NametableY { get; set; }
        public byte FineY { get; set; }


        public ushort Value
        {
            get
            {
                return (ushort)(
                        (CoarseX & 31) +
                        ((CoarseY & 31) << 5) +
                        ((NametableX & 1) << 10) +
                        ((NametableY & 1) << 11) +
                        ((FineY & 7) << 12)
                        );
            }
            set
            {
                CoarseX = (byte)(value & 0x1F);
                CoarseY = (byte)((value & 0x3E0) >> 5);
                NametableX = (byte)((value & 0x400) >> 10);
                NametableY = (byte)((value & 0x800) >> 11);
                FineY = (byte)((value & 0x7000) >> 12);
            }
        }
    }
}
