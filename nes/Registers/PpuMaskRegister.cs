using emulator6502;

namespace NES.Registers
{
    public class PpuMaskRegister : ByteRegister
    {
        /// <summary>
        /// G
        /// Bit 0
        /// </summary>
        public bool Grayscale
        {
            get => GetBooleanBit(0);
            set => SetBoolanBit(0, value);
        }

        /// <summary>
        ///  m
        /// Bit 1
        /// </summary>
        public bool BackgroundLeftColumnEnable
        {
            get => GetBooleanBit(1);
            set => SetBoolanBit(1, value);
        }

        /// <summary>
        ///  M
        /// Bit 2
        /// </summary>
        public bool SpriteLeftColumnEnable
        {
            get => GetBooleanBit(2);
            set => SetBoolanBit(2, value);
        }

        /// <summary>
        /// b
        /// Bit 3
        /// </summary>
        public bool BackgroundEnable
        {
            get => GetBooleanBit(3);
            set => SetBoolanBit(3, value);
        }

        /// <summary>
        /// s
        /// Bit 4
        /// </summary>
        public bool SpriteEnable
        {
            get => GetBooleanBit(4);
            set => SetBoolanBit(4, value);
        }

        /// <summary>
        /// R
        /// Bit 5
        /// </summary>
        public bool Red
        {
            get => GetBooleanBit(5);
            set => SetBoolanBit(5, value);
        }

        /// <summary>
        /// G
        /// Bit 6
        /// </summary>
        public bool Green
        {
            get => GetBooleanBit(6);
            set => SetBoolanBit(6, value);
        }


        /// <summary>
        /// B
        /// </summary>
        public bool Blue
        {
            get => GetBooleanBit(7);
            set => SetBoolanBit(7, value);
        }
    }
}