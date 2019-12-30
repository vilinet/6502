

namespace console
{
    static class Program
    {
        private static void Main(string[] args)
        {
            //new Terminal().Run();  
            var app = new MySfmlNesApp(300, 300, 515, 480, "smb.nes");

            var debugWindow = new DebugWindow("Debug", 1200, 1000, app.Nes);

            while (app.IsOpen && debugWindow.IsOpen)
            {
                debugWindow.Render();
                app.Render();
            }

        }
    }
}