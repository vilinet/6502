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

    public interface IDrawText
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Percentage of x coordinate, 0-1 range</param>
        /// <param name="y">Percentage of x coordinate, 0-1 range</param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="font"></param>
        void DrawText(float x, float y, string text, SDL.SDL_Color color, TextAlignment alignment = TextAlignment.Default, DisplayFont font = null);
        void DrawText(float x, float y, string text, uint color, TextAlignment alignment = TextAlignment.Default, DisplayFont font = null);
    }
}