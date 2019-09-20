using NES.Interfaces;
using NESInterfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace NES.Display.SFML
{
    public class SFMLDisplay : IDisplay, IController
    {
        private const int NesWidth = 256, NesHeight = 240; 
        public RenderWindow Window { get; }

        private byte _keys;
        
        //native nes display size
        private readonly Texture _displayTexture;
        private readonly byte[] _displayBuffer;
        private readonly Sprite _displaySprite;
        private bool _frameFinished;

        public SFMLDisplay(int width, int height)
        {
            Window = new RenderWindow(new VideoMode((uint)width,(uint) height), "Nes")
            {
                Size = new Vector2u((uint) width, (uint) height),
                Position = new Vector2i(505,500)
            };

            Window.KeyPressed += KeyPressed;
            Window.KeyReleased += KeyReleased;
            
            _displayBuffer = new byte[NesWidth * NesHeight * 4];
            _displayTexture = new Texture(NesWidth,  NesHeight);
            _displaySprite = new Sprite(_displayTexture);
        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.A) _keys               &= 0b11111110;
            else if (e.Code == Keyboard.Key.S) _keys          &= 0b11111101;
            else if (e.Code == Keyboard.Key.Backspace) _keys    &= 0b11111011;       
            else if (e.Code == Keyboard.Key.Enter) _keys       &= 0b11110111;   
            else if (e.Code == Keyboard.Key.Up) _keys           &=  0b11101111;    
            else if (e.Code == Keyboard.Key.Down) _keys        &=  0b11011111;       
            else if (e.Code == Keyboard.Key.Left) _keys        &= 0b10111111;     
            else if (e.Code == Keyboard.Key.Right) _keys       &=  0b01111111;    
        }

     
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.A) _keys |= 0b0000_0001;
            else if (e.Code == Keyboard.Key.S) _keys |= 0b0000_0010;
            else if (e.Code == Keyboard.Key.Backspace) _keys |= 0b0000_0100;
            else if (e.Code == Keyboard.Key.Enter) _keys |= 0b0000_1000;
            else if (e.Code == Keyboard.Key.Up) _keys |= 0b0001_0000;
            else if (e.Code == Keyboard.Key.Down) _keys |= 0b0010_0000;
            else if (e.Code == Keyboard.Key.Left) _keys |= 0b0100_0000;
            else if (e.Code == Keyboard.Key.Right) _keys |= 0b1000_0000;
        }

        public void DrawPixel(int x, int y, uint color)
        {
            if (x >= NesWidth || y >= NesHeight || x < 0 || y < 0) return;
            int pos = (y * NesWidth + x)*4;
            _displayBuffer[pos] = (byte) ((color &  0x00FF0000) >> 16) ;
            _displayBuffer[pos + 1] = (byte)((color & 0x0000FF00) >> 8) ;
            _displayBuffer[pos + 2] = (byte)  (color &  0x000000FF) ;
            _displayBuffer[pos+3] = 255;
        }

        public void FrameFinished()
        {
            _frameFinished = true;
        }

        public void Render()
        {
            if (_frameFinished)
            {
                _frameFinished = false;
                _displayTexture.Update(_displayBuffer);
                Window.Draw(_displaySprite);
                Window.Display();
            }
        }

        public void DispatchEvents()
        {
            Window.DispatchEvents();
        }

        public byte GetState()
        {
            return _keys;
        }
    }
}