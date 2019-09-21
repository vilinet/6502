using System;
using NES.Interfaces;
using SDL2;

namespace NES.Display.SDL2
{
    public class SDL2GeneralDisplay : IDisplay
    {
        private IntPtr _bufferPtr, _window, _renderer, _texture;
        private bool _frameFinished;
        private readonly uint[] _buffer;
        protected int InternalWidth { get; }
        protected int InternalHeight { get; }

        public SDL2GeneralDisplay(string title, int width, int height, int internalWidth, int internalHeight,  int x = 0, int y = 0)
        {
            InternalWidth = internalWidth;
            InternalHeight = internalHeight;
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            _window = SDL.SDL_CreateWindow(title,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                width,
                height, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE
            );

            if (x > 0 && y > 0) 
                SDL.SDL_SetWindowPosition(_window, x, y);
            
            _buffer = new uint[internalWidth * internalHeight * 4];

            _renderer = SDL.SDL_CreateRenderer(_window, -1, 0);
            _texture = SDL.SDL_CreateTexture(_renderer, SDL.SDL_PIXELFORMAT_ARGB8888,  (int) SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, internalWidth, internalHeight);
            
            SDL.SDL_GL_SetSwapInterval(0);

            unsafe { fixed (uint* t = &_buffer[0]) _bufferPtr = new IntPtr(t); }
            
            IsOpen = true;
        }

        protected virtual void OnBeforeRender(){}

        public bool IsOpen { get; protected set; }
        

        public void DrawPixel(int x, int y, uint color)
        {
            _buffer[y * InternalWidth + x] = color;
        }

        public void FrameFinished()
        {
            _frameFinished = true;
        }

        protected virtual void OnKeyDown(SDL.SDL_Keysym e)  { }

        protected virtual void OnKeyUp(SDL.SDL_Keysym e)  { }
        
        public void Render()
        {
            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        OnKeyDown(e.key.keysym);
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        OnKeyUp(e.key.keysym);
                        break;
                    case SDL.SDL_EventType.SDL_QUIT:
                        IsOpen = false;
                        break;
                }
            }
            if (_frameFinished)
            {
                _frameFinished = false;
                OnBeforeRender();

                SDL.SDL_RenderClear(_renderer);
                SDL.SDL_UpdateTexture(_texture, IntPtr.Zero, _bufferPtr, InternalWidth * sizeof(uint));
                SDL.SDL_RenderCopy(_renderer, _texture, IntPtr.Zero, IntPtr.Zero);
                SDL.SDL_RenderPresent(_renderer);
            }
        }

    
    
    }
}