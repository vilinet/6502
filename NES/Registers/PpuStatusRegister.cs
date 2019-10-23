using emulator6502;

namespace NES.Registers
{
    public class PpuStatusRegister : BooleanByteRegister
    {
        /// <summary>
        /// O
        /// </summary>
        public bool SpriteOverflow
        {
            get => Bit5;
            set => Bit5 = value;
        }

        /// <summary>
        /// O
        /// </summary>
        public bool SpriteHit
        {
            get => Bit6;
            set => Bit6 = value;
        }

        /// <summary>
        /// V
        /// </summary>
        public bool VerticalBlank
        {
            get => Bit7;
            set => Bit7 = value;
        }
    }
}