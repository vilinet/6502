namespace NES.Interfaces
{
    public interface IDebugDisplay : IDisplay
    {
        void DrawText(int x, int y, string text);
    }
}