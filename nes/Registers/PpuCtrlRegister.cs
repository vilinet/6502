using emulator6502;

namespace NES.Registers
{
    public class PpuCtrlRegister : BooleanByteRegister
    {
        /// <summary>
        /// NN
        /// Bit 0-1
        /// </summary>
        public byte NametableSelect
        {
            get
            {
                int sum = 0;
                sum+= Bit0 ? 1:0;
                sum+= Bit1 ? 2:0;
                return (byte)sum;
            }
            set
            {
                Bit0 = (value & 1)!=0;
                Bit1 = (value & 2) != 0;
            }
        }

        /// <summary>
        ///  I
        ///  Bit 2
        /// </summary>
        public bool IncrementMode
        {
            get => Bit2;
            set => Bit2 = value;
        }

        /// <summary>
        ///  s
        ///  Bit 3
        /// </summary>
        public bool SpriteBank
        {
            get => Bit3;
            set => Bit3 = value;
        }
        /// <summary>
        /// B
        /// Bit 4
        /// </summary>
        public bool BackgroundBank
        {
            get => Bit4;
            set => Bit4 = value;
        }

        /// <summary>
        /// S
        /// Sprite size
        /// Bit 5
        /// </summary>
        public bool SpriteHeight
        {
            get => Bit5;
            set => Bit5 = value;
        }

        /// <summary>
        /// P
        /// Bit 6
        /// </summary>
        public bool MasterSlave
        {
            get => Bit6;
            set => Bit6 = value;
        }

        /// <summary>
        /// V
        /// Bit 7
        /// </summary>
        public bool NmiEnabled
        {
            get => Bit7;
            set => Bit7 = value;
        }
    }
}