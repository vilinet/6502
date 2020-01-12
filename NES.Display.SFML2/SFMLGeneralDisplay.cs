using NES.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace NES.Display.SFML
{
    public class SfmlGeneralDisplay : RenderWindow, IDisplay
    {
        protected readonly byte[] Pixels;
        protected readonly Texture _texture;
        protected readonly Sprite _sprite;
        private readonly Font _font;
        private readonly Text _text;

        protected uint InternalWidth { get; }
        protected uint InternalHeight { get; }

        protected bool FrameFinished { get; set; } = true;

        public SfmlGeneralDisplay(string title, uint width, uint height, uint scale = 1) : base(new VideoMode(width, height), title)
        {
            InternalWidth = width;
            InternalHeight = height;

            Pixels = new byte[width * height * 4];
            _texture = new Texture(width, height);
            _sprite = new Sprite(_texture);

            _font = new Font("default.ttf");
            _text = new Text();
            _text.Font = _font;
            _text.FillColor = Color.White;
            _text.CharacterSize = 10;
        }
        
        private void SetPixel(int x, int y, uint color)
        {
            int p = (int)((x + y * InternalWidth) * 4);
            if (p >= Pixels.Length) return;

            Pixels[p] = (byte)((color & 0xFF0000) >> 16);
            Pixels[p + 1] = (byte)((color & 0xFF00) >> 8);
            Pixels[p + 2] = (byte)(color & 0xFF);
            Pixels[p + 3] = 255;
        }

        public void DrawPixel(int x, int y, uint color)
        {
            if (x >= InternalWidth || y >= InternalHeight) return;
            int xx = x * 2;
            int yy = y * 2;
            SetPixel(xx, yy, color);
            SetPixel(xx + 1, yy, color);
            SetPixel(xx, yy + 1, color);
            SetPixel(xx + 1, yy + 1, color);
        }

        public void SetFrameFinished()
        {
            FrameFinished = true;
        }

        public virtual void Render()
        {
            HandleEvents();
            if (FrameFinished)
            {
                FrameFinished = false;
                _texture.Update(Pixels);
                Draw(_sprite);
                OnPostDraw();
                Display();
            }
        }

        protected virtual void OnPostDraw() { }

        protected virtual void HandleEvents()
        {
            while (PollEvent(out Event ev))
            {
                switch (ev.Type)
                {
                    case EventType.Closed:
                        Close();

                        break;
                    case EventType.KeyPressed:
                        break;

                    case EventType.KeyReleased:
                        OnKeyRelease(ev.Key.Code);
                        break;
                }
            }
        }

        protected virtual void OnKeyRelease(Keyboard.Key key) {}

        public void DrawText(int x, int y, string text, int fontSize = 24, Color color = default)
        {
            if (_text != null)
            {
                _text.DisplayedString = text;
                if (color == default)
                {
                    _text.FillColor = new Color(255, 255, 255, 255);
                }
                else _text.FillColor = color;
                
                _text.CharacterSize = (uint)fontSize;
                _text.Position = new Vector2f(2 * x, 2 * y);
                Draw(_text);
            }
        }

        protected void ClearPixels()
        {
            for (int i = 0; i < Pixels.Length / 4; i++)
            {
                Pixels[i * 4] = 0;
                Pixels[i * 4 + 1] = 0;
                Pixels[i * 4 + 2] = 0;
                Pixels[i * 4 + 3] = 255;
            }
        }

    }
}