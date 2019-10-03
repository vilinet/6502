using NES;
using NES.Display.SDL2;

namespace console
{
    static class Program
    {
        
        static void Main(string[] args)
        {
            var display = new SDL2NesGameDisplay("NES", 256*4,240*2,256*2,240);
            var nes = new Nes(display, display , display);
            nes.LoadPalette("mesen.pal");
            nes.LoadRom("./donkey.nes" );
            nes.RunOnThread();
          
            while (display.IsOpen)
            {
               display.Render();
            }
        }
    }
}