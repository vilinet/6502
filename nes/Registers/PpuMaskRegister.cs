using emulator6502;

namespace NES.Registers
{
    public class PpuMaskRegister : BooleanByteRegister
    {
        /// <summary>
        /// G
        /// Bit 0
        /// </summary>
        public bool Grayscale
        {
            get => Bit0;
            set => Bit0 = value;
        }

        /// <summary>
        ///  m
        /// Bit 1
        /// </summary>
        public bool BackgroundLeftColumnEnable
        {
            get => Bit1;
            set => Bit1 = value;
        }

        /// <summary>
        ///  M
        /// Bit 2
        /// </summary>
        public bool SpriteLeftColumnEnable
        {
            get => Bit2;
            set => Bit2 = value;
        }

        /// <summary>
        /// b
        /// Bit 3
        /// </summary>
        public bool BackgroundEnable
        {
            get => Bit3;
            set => Bit3 = value;
        }

        /// <summary>
        /// s
        /// Bit 4
        /// </summary>
        public bool SpriteEnable
        {
            get => Bit4;
            set => Bit4 = value;
        }

        /// <summary>
        /// R
        /// Bit 5
        /// </summary>
        public bool Red
        {
            get => Bit5;
            set => Bit5 = value;
        }

        /// <summary>
        /// G
        /// Bit 6
        /// </summary>
        public bool Green
        {
            get => Bit6;
            set => Bit6 = value;
        }


        /// <summary>
        /// B
        /// </summary>
        public bool Blue
        {
            get => Bit7;
            set => Bit7 = value;
        }
    }
}