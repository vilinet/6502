using System;
using NES.Interfaces;
using SDL2;

namespace NES.Display.SDL2
{
    public partial class SDL2GeneralDisplay : IDisplay
    {
        internal readonly IntPtr _renderer;
        internal IntPtr _font;
        private readonly IntPtr _bufferPtr;
        private readonly IntPtr _window;
        private readonly IntPtr _texture;
        private readonly IDrawText _drawText;
        private readonly uint[] _buffer;
        private bool _frameFinished;

        protected int InternalWidth { get; }
        protected int InternalHeight { get; }
        public int ActualWidth { get; private set; }
        public int ActualHeight { get; private set; }
        public bool IsOpen { get; protected set; }

        public SDL2GeneralDisplay(string title, int width, int height, int internalWidth, int internalHeight, int x = 0, int y = 0, string fontFile = null, int? fontSize = null)
        {
            SDL_ttf.TTF_Init();
            _font = SDL_ttf.TTF_OpenFont(fontFile ?? "./default.ttf", fontSize ?? 20);

            InternalWidth = internalWidth;
            InternalHeight = internalHeight;

            ActualWidth = width;
            ActualHeight = height;

            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            try
            {
                _window = SDL.SDL_CreateWindow(title,
                    SDL.SDL_WINDOWPOS_CENTERED,
                    SDL.SDL_WINDOWPOS_CENTERED,
                    width,
                    height,
                    SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN
                );
            }
            catch { }
            finally
            {
                if (_window == IntPtr.Zero)
                {
                    _window = SDL.SDL_CreateWindow(title,
                   SDL.SDL_WINDOWPOS_CENTERED,
                   SDL.SDL_WINDOWPOS_CENTERED,
                   width,
                   height,
                   SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                }
            }


            if (x > 0 && y > 0)
                SDL.SDL_SetWindowPosition(_window, x, y);

            _buffer = new uint[internalWidth * internalHeight * 4];

            _renderer = SDL.SDL_CreateRenderer(_window, -1, 0);
            _texture = SDL.SDL_CreateTexture(_renderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, internalWidth, internalHeight);
            _drawText = new DrawTextImpl(this);
            SDL.SDL_GL_SetSwapInterval(0);

            unsafe { fixed (uint* t = &_buffer[0]) _bufferPtr = new IntPtr(t); }

            IsOpen = true;
        }

        public DisplayFont LoadFont(string path, int fontSize)
        {
            return new DisplayFont(path, fontSize);
        }

        public void DrawPixel(int x, int y, uint color)
        {
            _buffer[y * InternalWidth + x] = color;
        }

        public void FrameFinished()
        {
            _frameFinished = true;
        }

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

                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                        {
                            SDL.SDL_GetWindowSize(_window, out int w, out int h);
                            ActualWidth = w;
                            ActualHeight = h;
                        }
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
                OnRenderText(_drawText);
                SDL.SDL_RenderPresent(_renderer);
            }
        }

        #region Virtuals
        public virtual void OnRenderText(IDrawText renderer) { }
        protected virtual void OnBeforeRender() { }
        protected virtual void OnKeyDown(SDL.SDL_Keysym e) { }
        protected virtual void OnKeyUp(SDL.SDL_Keysym e) { }
        #endregion
    }
}