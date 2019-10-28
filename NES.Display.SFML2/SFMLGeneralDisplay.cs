using NES.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Linq.Expressions;

namespace NES.Display.SFML
{
    public class SFMLGeneralDisplay : RenderWindow, IDisplay
    {
        private readonly byte[] Pixels;
        private readonly Texture _texture;
        private readonly Sprite _sprite;
        private readonly Font _font;
        private readonly Text _text;
        private readonly DrawTextImp _drawText;

        protected uint InternalWidth { get; }
        protected uint InternalHeight { get; }

        private bool _frameFinished = true;

        protected Controller Controller1 { get; }

        public SFMLGeneralDisplay(string title, uint width, uint height) : base(new VideoMode(width, height), title)
        {
            InternalWidth = width;
            InternalHeight = height;
            Controller1 = new Controller();

            Pixels = new byte[width * height * 4];
            _texture = new Texture(width, height);
            _sprite = new Sprite(_texture);

            _font = new Font("default.ttf");
            _text = new Text();
            _text.Font = _font;
            _text.FillColor = Color.White;
            _text.CharacterSize = 10;
            _drawText  = new DrawTextImp(this);
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

        public void FrameDone()
        {
            _frameFinished = true;
        }

        public void Render()
        {
            HandleEvents();

            if (_frameFinished)
            {
                _frameFinished = false;
                _texture.Update(Pixels);
                Draw(_sprite);
                OnPostDraw(_drawText);
                Display();
            }
        }

        protected virtual void OnPostDraw(IDrawText drawText)
        {
            
        }

        private void HandleEvents()
        {
            while (PollEvent(out Event ev))
            {
                bool? pressed = null;

                switch (ev.Type)
                {
                    case EventType.Closed:
                        Close();
                        break;
                    case EventType.KeyPressed:
                        pressed = true;
                        break;
                    case EventType.KeyReleased:
                        pressed = false;

                        OnKeyRelease(ev.Key.Code);
                        break;
                }

                if (pressed.HasValue)
                {
                    var c = ev.Key.Code;
                    switch (c)
                    {
                        case Keyboard.Key.Left:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Left, pressed.Value);
                            break;
                        case Keyboard.Key.Right:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Right, pressed.Value);
                            break;
                        case Keyboard.Key.Up:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Up, pressed.Value);
                            break;
                        case Keyboard.Key.Down:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Down, pressed.Value);
                            break;
                        case Keyboard.Key.Backspace:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Select, pressed.Value);
                            break;
                        case Keyboard.Key.Enter:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Start, pressed.Value);
                            break;
                        case Keyboard.Key.A:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.A, pressed.Value);
                            break;
                        case Keyboard.Key.S:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.B, pressed.Value);
                            break;
                        case Keyboard.Key.Escape:
                            Close();
                            break;
                    }
                }
            }
        }

        protected void ClearPixels()
        {
            for (int i = 0; i < InternalWidth * InternalHeight; i++) Pixels[i] = 0;
        }

        protected virtual void OnKeyRelease(Keyboard.Key key) { }
        
        private class DrawTextImp : IDrawText
        {
            private readonly SFMLGeneralDisplay _display;
            public DrawTextImp(SFMLGeneralDisplay display)
            {
                _display = display;
            }

            public void DrawText(int x, int y, string text, int fontSize)
            {
                if (_display._text != null)
                {
                    _display._text.DisplayedString = text;
                    _display._text.Position = new Vector2f(2 * x, 2 * y);
                    _display._text.Draw(_display, RenderStates.Default);
                }
            }
        }
    }
}