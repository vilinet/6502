using System;
using System.Collections.Generic;
using System.Text;

namespace NES
{
    [Flags]
    public enum PPUCTRL : byte
    {
        Zero = 0,
        /// <summary>
        /// Name Table address bytes
        /// (0 = $2000; 1 = $2400; 2 = $2800; 3 = $2C00)
        /// N0 Low
        /// N1 High
        /// </summary>
        N0, N2,
        /// <summary>
        /// VRAM address increment per CPU read/write of PPUDATA
        /// </summary>
        I,
        /// <summary>
        /// Sprite pattern table address for 8x8 sprites
        /// (0: $0000; 1: $1000; ignored in 8x16 mode)
        /// </summary>
        S,

        /// <summary>
        /// Sprite size (0: 8x8 pixels; 1: 8x16 pixels)
        /// </summary>
        H,
        /// <summary>
        /// PPU master/slave select
        /// (0: read backdrop from EXT pins; 1: output color on EXT pins)
        /// </summary>
        P,
        /// <summary>
        /// Generate an NMI at the start of the
        /// vertical blanking interval (0: off; 1: on)
        /// </summary>
        V
    }

    [Flags]
    public enum PPUMASK : byte
    {
        Zero = 0,
        /// <summary>
        /// Grayscale
        /// </summary>
        g,
        /// <summary>
        /// Show background in leftmost 8 pixels of screen, 0: Hide
        /// </summary>
        m,
        /// <summary>
        ///  Show sprites in leftmost 8 pixels of screen, 0: Hide
        /// </summary>
        M,
        /// <summary>
        ///  Show background
        /// </summary>
        b,
        s,
        /// <summary>
        ///  Emphasize red
        /// </summary>
        R,
        /// <summary>
        ///  Emphasize green
        /// </summary>
        G,
        /// <summary>
        ///  Emphasize blue
        /// </summary>
        B
    }

    [Flags]
    public enum PPUSTATUS : byte
    {
        Zero = 0,
        /// <summary>
        /// sprite overflow
        /// </summary>
        O = 5,
        /// <summary>
        /// sprite 0 hit 
        /// </summary>
        S = 61,
        /// <summary>
        /// VBLANK
        /// </summary>
        V = 7
    }

    /// <summary>
    /// All properties are in the correct order 0-7
    /// </summary>
    public class PPURegisters
    {
        public PPUCTRL PPUCTRL { get; set; } = new PPUCTRL();
        public PPUMASK PPUMASK { get; set; } = new PPUMASK();
        public PPUSTATUS PPUSTATUS { get; set; } = new PPUSTATUS();
        public byte OEMADDR { get; set; }
        public byte OAMDATA { get; set; }
        public byte PPUSCROLL { get; set; }
        public byte PPUADDR { get; set; }
        public byte PPUDATA { get; set; }
    }
}
