using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace NES.Display
{
    public class SFMLDisplay : IDisplay
    {
        private const int NesWidth = 256, NesHeight = 240; 
        public RenderWindow Window { get; }
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
            
            _displayBuffer = new byte[NesWidth * NesHeight * 4];
            _displayTexture = new Texture(NesWidth,  NesHeight);
            _displaySprite = new Sprite(_displayTexture);
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
    }
}