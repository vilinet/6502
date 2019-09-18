using emulator6502;

namespace NES.Registers
{
    public class PpuCtrlRegister : ByteRegister
    {
        /// <summary>
        /// NN
        /// Bit 0-1
        /// </summary>
        public byte NametableSelect
        {
            get => (byte)(Value & 3);
            set
            {
                SetBit(0, (byte)(value & 1));
                SetBit(1, (byte)((value & 2) >> 1));
            }
        }

        /// <summary>
        ///  I
        ///  Bit 2
        /// </summary>
        public bool IncrementMode
        {
            get => GetBooleanBit(2);
            set => SetBoolanBit(2, value);
        }

        /// <summary>
        ///  s
        ///  Bit 3
        /// </summary>
        public bool SpriteTileSelect
        {
            get => GetBooleanBit(3);
            set => SetBoolanBit(3, value);
        }
        /// <summary>
        /// B
        /// Bit 4
        /// </summary>
        public bool BackgroundTileSelect
        {
            get => GetBooleanBit(4);
            set => SetBoolanBit(4, value);
        }

        /// <summary>
        /// S
        /// Sprite size
        /// Bit 5
        /// </summary>
        public bool SpriteHeight
        {
            get => GetBooleanBit(5);
            set => SetBoolanBit(5, value);
        }

        /// <summary>
        /// P
        /// Bit 6
        /// </summary>
        public bool MasterSlave
        {
            get => GetBooleanBit(6);
            set => SetBoolanBit(6, value);
        }

        /// <summary>
        /// V
        /// Bit 7
        /// </summary>
        public bool NmiEnabled
        {
            get => GetBooleanBit(7);
            set => SetBoolanBit(7, value);
        }
    }
}