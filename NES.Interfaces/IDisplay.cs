using System;

namespace NES.Interfaces
{
    public interface IDisplay
    {
        void DrawPixel(int x, int y, uint color);
        
        void SetFrameFinished();
    }
}