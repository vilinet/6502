namespace NES.Display
{
    public interface IDisplay
    {
        void DrawPixel(int x, int y, uint color);
        
        void FrameFinished();
    }
}