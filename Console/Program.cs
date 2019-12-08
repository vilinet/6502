namespace console
{

    static class Program
    {
        static void Main(string[] args)
        {
            //new Terminal().Run(); 
            var app = new MySfmlNesApp(500,500, 700,700, "first.nes");
            var debugWindow = new DebugWindow("Debug", 1200, 1000, app.Nes);

            while (app.IsOpen && debugWindow.IsOpen)
            {
                debugWindow.Render();
                app.Render();
            }

        }
    }
}