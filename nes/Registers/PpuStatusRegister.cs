using emulator6502;

namespace NES.Registers
{
    public class PpuStatusRegister : ByteRegister
    {
        /// <summary>
        /// O
        /// </summary>
        public bool SpriteOverflow
        {
            get => GetBooleanBit(5);
            set => SetBoolanBit(5, value);
        }


        /// <summary>
        /// O
        /// </summary>
        public bool SpriteHit
        {
            get => GetBooleanBit(6);
            set => SetBoolanBit(6, value);
        }

        /// <summary>
        /// V
        /// </summary>
        public bool VerticalBlank
        {
            get => GetBooleanBit(7);
            set => SetBoolanBit(7, value);
        }
    }
}