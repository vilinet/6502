using System;
using System.Runtime.InteropServices;
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

        protected uint[] Pixels { get; set; }
        protected uint[] PixelsText { get; set; }
        protected bool FrameFinished { get; private set; }

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

            Pixels = new uint[internalWidth * internalHeight * 4];
            PixelsText = new uint[internalWidth * internalHeight * 4];

            _renderer = SDL.SDL_CreateRenderer(_window, -1, 0);

            _texture = SDL.SDL_CreateTexture(_renderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, internalWidth, internalHeight);
            SDL.SDL_GL_SetSwapInterval(0);

            unsafe { fixed (uint* t = &Pixels[0]) _bufferPtr = new IntPtr(t); }

            IsOpen = true;
        }

        public DisplayFont LoadFont(string path, int fontSize)
        {
            return new DisplayFont(path, fontSize);
        }

        public void DrawPixel(int x, int y, uint color)
        {
            Pixels[y * InternalWidth + x] = color;
        }

        public void FrameDone()
        {
            FrameFinished = true;
        }

        protected virtual void OnEffectApply(int width, int height, uint[] pixels)
        {

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
            if (FrameFinished)
            {
                OnEffectApply(InternalWidth, InternalHeight, Pixels);
                SDL.SDL_UpdateTexture(_texture, IntPtr.Zero, _bufferPtr, InternalWidth * sizeof(uint));
                SDL.SDL_RenderCopy(_renderer, _texture, IntPtr.Zero, IntPtr.Zero);
                OnBeforeRender();
                SDL.SDL_RenderPresent(_renderer);
            }
        }


        public void DrawText(float x, float y, string text, SDL.SDL_Color color, TextAlignment alignment = TextAlignment.Default, DisplayFont font = null)
        {
            
            var surfacePtr = SDL_ttf.TTF_RenderUNICODE_Solid(font?.Font ?? _font, text, color);
            SDL.SDL_Surface surface = (SDL.SDL_Surface)Marshal.PtrToStructure(surfacePtr, typeof(SDL.SDL_Surface));
            var texture = SDL.SDL_CreateTextureFromSurface(_renderer, surfacePtr);
            int w = surface.w;
            int h = surface.h;
            int xx = (int)(x * ActualWidth);
            int yy = (int)(y * ActualHeight);

            if (alignment.HasFlag(TextAlignment.HorizontalCenter)) xx -= w / 2;
            else if (alignment.HasFlag(TextAlignment.HorizontalRight)) xx -= w;

            if (alignment.HasFlag(TextAlignment.VerticalCenter)) yy -= h / 2;
            else if (alignment.HasFlag(TextAlignment.VerticalBottom)) yy -= h;


            SDL.SDL_FreeSurface(surfacePtr);
            var src = new SDL.SDL_Rect() { x = 0, y = 0, w = w, h = h };
            var target = new SDL.SDL_Rect() { x = xx, y = yy, w = w, h = h };
            SDL.SDL_RenderCopy(_renderer , texture, ref src, ref target);
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

        #region Virtuals

        protected virtual void OnBeforeRender() { }
        protected virtual void OnKeyDown(SDL.SDL_Keysym e) { }
        protected virtual void OnKeyUp(SDL.SDL_Keysym e) { }
        #endregion
    }
}