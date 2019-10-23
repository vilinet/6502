using emulator6502;

namespace NES.Registers
{
    public class PpuCtrlRegister : BooleanByteRegister
    {
        /// <summary>
        /// X
        /// Bit 0
        /// </summary>
        public bool NametableX
        {
            get => Bit0;
            set => Bit0 = value;
        }


        /// <summary>
        /// Y
        /// Bit 1
        public bool NametableY
        {
            get => Bit1;
            set => Bit1 = value;
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
        public bool PatternSprite
        {
            get => Bit3;
            set => Bit3 = value;
        }
        /// <summary>
        /// B
        /// Bit 4
        /// </summary>
        public bool PatternBackground
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