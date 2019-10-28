using SDL2;
using System;

namespace NES.Display.SDL2
{
    [Flags]
    public enum TextAlignment : byte
    {
        /// <summary>
        /// Top/Left
        /// </summary>
        Default = 0,
        HorizontalCenter = 1,
        HorizontalRight = 2,
        VerticalCenter = 4,
        VerticalBottom = 8,
    }
}