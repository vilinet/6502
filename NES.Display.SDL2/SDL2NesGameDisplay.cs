using System;
using NES.Controllers;
using NES.Interfaces;
using NESInterfaces;
using SDL2;

namespace NES.Display.SDL2
{
    public class SDL2NesGameDisplay: SDL2GeneralDisplay, IDebugDisplay
    {
        protected IController Controller1 { get; }


        private void HandleController(SDL.SDL_Keycode code, bool pressed)
        {
            switch (code)
            {
                case SDL.SDL_Keycode.SDLK_a:
                    Controller1.SetButtonState(ControllerButton.A, pressed);
                    break;
                case SDL.SDL_Keycode.SDLK_s:
                    Controller1.SetButtonState(ControllerButton.B, pressed);
                    break;
                case SDL.SDL_Keycode.SDLK_BACKSPACE:
                    Controller1.SetButtonState(ControllerButton.Select, pressed);
                    break;
                case SDL.SDL_Keycode.SDLK_RETURN:
                    Controller1.SetButtonState(ControllerButton.Start, pressed);
                    break;
                case SDL.SDL_Keycode.SDLK_UP:
                    Controller1.SetButtonState(ControllerButton.Up, pressed);
                    break;
                case SDL.SDL_Keycode.SDLK_DOWN:
                    Controller1.SetButtonState(ControllerButton.Down, pressed);
                    break;
                case SDL.SDL_Keycode.SDLK_LEFT:
                    Controller1.SetButtonState(ControllerButton.Left, pressed);
                    break;
                case SDL.SDL_Keycode.SDLK_RIGHT:
                    Controller1.SetButtonState(ControllerButton.Right, pressed);
                    break;
            }
        }
        protected override void OnKeyUp(SDL.SDL_Keysym e)
        {
            HandleController(e.sym, false);
        }

        protected override void OnKeyDown(SDL.SDL_Keysym e)
        {
            HandleController(e.sym, true);
        }

        protected override void OnBeforeRender()
        {
            for (int i = 0; i < InternalHeight; i++)
            {
                DrawPixel(256, i, 0xffffff);
            }
        }

        public SDL2NesGameDisplay(string title, int width, int height, int internalResWidth, int internalResHeight, int x = 0, int y = 0,  string fontFile = null, int? fontSize = null):
            base(title, width, height, internalResWidth, internalResHeight, x, y, fontFile, fontSize) {
            Controller1 = new Controller();
        }
    }
}