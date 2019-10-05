using System.Runtime.InteropServices;
using SDL2;

namespace NES.Display.SDL2
{
    internal class DrawTextImpl : IDrawText
    {
        private SDL2GeneralDisplay _display;

        public DrawTextImpl(SDL2GeneralDisplay sDL2GeneralDisplay)
        {
            _display = sDL2GeneralDisplay;
        }

        public void DrawText(float x, float y, string text, SDL.SDL_Color color, TextAlignment alignment = TextAlignment.Default, DisplayFont font = null)
        {
            var surfacePtr = SDL_ttf.TTF_RenderUNICODE_Solid(font?.Font ?? _display._font, text, color);
            SDL.SDL_Surface surface = (SDL.SDL_Surface)Marshal.PtrToStructure(surfacePtr, typeof(SDL.SDL_Surface));
            var texture = SDL.SDL_CreateTextureFromSurface(_display._renderer, surfacePtr);
            int w = surface.w;
            int h = surface.h;
            int xx = (int)(x * _display.ActualWidth);
            int yy = (int)(y * _display.ActualHeight);

            if (alignment.HasFlag(TextAlignment.HorizontalCenter)) xx -= w / 2;
            else if (alignment.HasFlag(TextAlignment.HorizontalRight)) xx -= w;

            if (alignment.HasFlag(TextAlignment.VerticalCenter)) yy -= h / 2;
            else if (alignment.HasFlag(TextAlignment.VerticalBottom)) yy -= h;



            SDL.SDL_FreeSurface(surfacePtr);
            var src = new SDL.SDL_Rect() { x = 0, y = 0, w = w, h = h };
            var target = new SDL.SDL_Rect() { x = xx, y = yy, w = w, h = h };
            SDL.SDL_RenderCopy(_display._renderer, texture, ref src, ref target);
            SDL.SDL_DestroyTexture(texture);
        }

        public void DrawText(float x, float y, string text, uint color, TextAlignment alignment = TextAlignment.Default, DisplayFont font = null)
        {
            var b = color & 0xFF;
            var g = (color & 0xFF00) >> 8;
            var r = (color & 0xFF0000) >> 16;
            var a = (color & 0xFF000000) >> 24;
            DrawText(x, y, text, new SDL.SDL_Color() { r = (byte)r, g = (byte)g, b = (byte)b, a = (byte)a }, alignment, font);
        }
    }
}
