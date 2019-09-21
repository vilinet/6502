using System;
using NES.Interfaces;
using NESInterfaces;
using SDL2;

namespace NES.Display.SDL2
{
    public class SDL2NesGameDisplay: SDL2GeneralDisplay, IController, IDebugDisplay
    {
        private byte _keys;

        public byte GetState()
        {
            return _keys;
        }

        protected override void OnKeyDown(SDL.SDL_Keysym e)
        {
            var code = e.sym;
            switch (code)
            {
                case SDL.SDL_Keycode.SDLK_a:
                    _keys |= 0b0000_0001;
                    break;
                case SDL.SDL_Keycode.SDLK_s:
                    _keys |= 0b0000_0010;
                    break;
                case SDL.SDL_Keycode.SDLK_BACKSPACE:
                    _keys |= 0b0000_0100;
                    break;
                case SDL.SDL_Keycode.SDLK_RETURN:
                    _keys |= 0b0000_1000;
                    break;
                case SDL.SDL_Keycode.SDLK_UP:
                    _keys |= 0b0001_0000;
                    break;
                case SDL.SDL_Keycode.SDLK_DOWN:
                    _keys |= 0b0010_0000;
                    break;
                case SDL.SDL_Keycode.SDLK_LEFT:
                    _keys |= 0b0100_0000;
                    break;
                case SDL.SDL_Keycode.SDLK_RIGHT:
                    _keys |= 0b1000_0000;
                    break;
                case SDL.SDL_Keycode.SDLK_ESCAPE:
                    IsOpen = false;
                    break;
            }
        }

        protected override void OnBeforeRender()
        {
            for (int i = 0; i < InternalHeight; i++)
            {
                DrawPixel(241, i, 0xffffff);
            }
        }

        protected override void OnKeyUp(SDL.SDL_Keysym e)
        {
            var code = e.sym;
            switch (code)
            {
                case SDL.SDL_Keycode.SDLK_a:
                    _keys &= 0b11111110;
                    break;
                case SDL.SDL_Keycode.SDLK_s:
                    _keys &= 0b11111101;
                    break;
                case SDL.SDL_Keycode.SDLK_BACKSPACE:
                    _keys &= 0b11111011;
                    break;
                case SDL.SDL_Keycode.SDLK_RETURN:
                    _keys &= 0b11110111;
                    break;
                case SDL.SDL_Keycode.SDLK_UP:
                    _keys &= 0b11101111;
                    break;
                case SDL.SDL_Keycode.SDLK_DOWN:
                    _keys &= 0b11011111;
                    break;
                case SDL.SDL_Keycode.SDLK_LEFT:
                    _keys &= 0b10111111;
                    break;
                case SDL.SDL_Keycode.SDLK_RIGHT:
                    _keys &= 0b01111111;
                    break;
            }
        }

        public SDL2NesGameDisplay(string title, int width, int height, int internalResWidth, int internalResHeight, int x = 0, int y = 0) : base(title, width, height, internalResWidth, internalResHeight, x, y)
        {
        }
    }
}