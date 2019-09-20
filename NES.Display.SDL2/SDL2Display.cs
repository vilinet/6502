using System;
using NES.Interfaces;
using NESInterfaces;
using SDL2;

namespace NES.Display.SDL2
{
    public class SDL2Display : IDisplay, IController
    {
        private const int NesWidth = 256, NesHeight = 240;
        private IntPtr _bufferPtr, _window, _renderer, _texture;
        private bool _frameFinished;
        private byte _keys;
        private uint[] buffer = new uint[NesHeight * NesWidth * 4];

        public SDL2Display(int width, int height)
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            _window = SDL.SDL_CreateWindow("NES",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                width,
                height, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE
            );

            _renderer = SDL.SDL_CreateRenderer(_window, -1, 0);
            _texture = SDL.SDL_CreateTexture(_renderer, SDL.SDL_PIXELFORMAT_ARGB8888,
                (int) SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, NesWidth, NesHeight);
            SDL.SDL_GL_SetSwapInterval(0);

            unsafe
            {
                fixed (uint* t = &buffer[0])
                {
                    _bufferPtr = new IntPtr(t);
                }
            }

            IsOpen = true;
        }

        public bool IsOpen { get; private set; }

        public byte GetState()
        {
            return _keys;
        }

        public void DrawPixel(int x, int y, uint color)
        {
            buffer[y * NesWidth + x] = color;
        }

        public void FrameFinished()
        {
            _frameFinished = true;
        }

        public void Render()
        {
            if (_frameFinished)
            {
                SDL.SDL_Event e;
                while (SDL.SDL_PollEvent(out e) != 0)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            HandleKeyDown(e.key);
                            break;
                        case SDL.SDL_EventType.SDL_KEYUP:
                            HandleKeyUp(e.key);
                            break;
                        case SDL.SDL_EventType.SDL_QUIT:
                            IsOpen = false;
                            break;
                    }
                }
                
                _frameFinished = false;

                SDL.SDL_RenderClear(_renderer);
                SDL.SDL_UpdateTexture(_texture, IntPtr.Zero, _bufferPtr, NesWidth * sizeof(uint));
                SDL.SDL_RenderCopy(_renderer, _texture, IntPtr.Zero, IntPtr.Zero);
                SDL.SDL_RenderPresent(_renderer);
            }
        }

        private void HandleKeyDown(SDL.SDL_KeyboardEvent e)
        {
            var code = e.keysym.sym;
            if (code == SDL.SDL_Keycode.SDLK_a) _keys |= 0b0000_0001;
            else if (code == SDL.SDL_Keycode.SDLK_s) _keys |= 0b0000_0010;
            else if (code == SDL.SDL_Keycode.SDLK_BACKSPACE) _keys |= 0b0000_0100;
            else if (code == SDL.SDL_Keycode.SDLK_RETURN) _keys |= 0b0000_1000;
            else if (code == SDL.SDL_Keycode.SDLK_UP) _keys |= 0b0001_0000;
            else if (code == SDL.SDL_Keycode.SDLK_DOWN) _keys |= 0b0010_0000;
            else if (code == SDL.SDL_Keycode.SDLK_LEFT) _keys |= 0b0100_0000;
            else if (code == SDL.SDL_Keycode.SDLK_RIGHT) _keys |= 0b1000_0000;
            else if (code == SDL.SDL_Keycode.SDLK_ESCAPE) IsOpen = false;
        }

        private void HandleKeyUp(SDL.SDL_KeyboardEvent e)
        {
            var code = e.keysym.sym;
            if (code == SDL.SDL_Keycode.SDLK_a) _keys &= 0b11111110;
            else if (code == SDL.SDL_Keycode.SDLK_s) _keys &= 0b11111101;
            else if (code == SDL.SDL_Keycode.SDLK_BACKSPACE) _keys &= 0b11111011;
            else if (code == SDL.SDL_Keycode.SDLK_RETURN) _keys &= 0b11110111;
            else if (code == SDL.SDL_Keycode.SDLK_UP) _keys &= 0b11101111;
            else if (code == SDL.SDL_Keycode.SDLK_DOWN) _keys &= 0b11011111;
            else if (code == SDL.SDL_Keycode.SDLK_LEFT) _keys &= 0b10111111;
            else if (code == SDL.SDL_Keycode.SDLK_RIGHT) _keys &= 0b01111111;
            else if (code == SDL.SDL_Keycode.SDLK_ESCAPE) IsOpen = false;
        }
    }
}