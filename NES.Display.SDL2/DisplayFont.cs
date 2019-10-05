using System;
using SDL2;

namespace NES.Display.SDL2
{
    public class DisplayFont
    {
        public string Path { get; }
        public int FontSize { get; }

        internal IntPtr Font { get; }

        public DisplayFont(string path, int fontSize)
        {
            Path = path;
            FontSize = fontSize;
            Font = SDL_ttf.TTF_OpenFont(path, fontSize);
        }

        ~DisplayFont()
        {
            SDL_ttf.TTF_CloseFont(Font);
        }

    }
}