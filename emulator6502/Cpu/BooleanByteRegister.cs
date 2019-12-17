namespace emulator6502
{
    public class BooleanByteRegister
    {
        protected bool Bit0, Bit1, Bit2, Bit3, Bit4, Bit5, Bit6, Bit7;
        
        public static implicit operator byte(BooleanByteRegister @class)
        {
            var val = 0;
            val += @class.Bit0 ? 1 : 0;
            val += @class.Bit1 ? 2 : 0;
            val += @class.Bit2 ? 4 : 0;
            val += @class.Bit3 ? 8 : 0;
            val += @class.Bit4 ? 16 : 0;
            val += @class.Bit5 ? 32 : 0;
            val += @class.Bit6 ? 64 : 0;
            val += @class.Bit7 ? 128 : 0;
            return (byte) val;
        }

        public byte Value
        {
            get => this;
            set
            {
                Bit0 = (value & 0b0000_0001)!=0;
                Bit1 = (value & 0b0000_0010)!=0;
                Bit2 = (value & 0b0000_0100)!=0;
                Bit3 = (value & 0b0000_1000)!=0;
                Bit4 = (value & 0b0001_0000)!=0;
                Bit5 = (value & 0b0010_0000)!=0;
                Bit6 = (value & 0b0100_0000)!=0;
                Bit7 = (value & 0b1000_0000)!=0;
            }
        }
    }
}