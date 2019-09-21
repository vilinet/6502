using emulator6502;
using NES;
using System;
using NES.Display.SDL2;
using NES.Display.SFML;

namespace console
{
    static class Program
    {
        private static Nes nes;
        
        static void Main(string[] args)
        {
            var display = new SDL2NesGameDisplay("NES", 256*4,240*2,256*2,240);
             
            nes = new Nes(display, display , display, "./donkey.nes" );
            nes.LoadPalette("mesen.pal");
            //nes.Cpu.BeforeOperationExecuted += Cpu_BeforeOperationExecuted;
            nes.RunOnThread();
          
            while (display.IsOpen)
            {
                display.Render();
            }
        }

        private static void Cpu_BeforeOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            try
            {
                var bb = e.Full.ToString(cpu.GetValue(e.Full)).PadRight(40);
                var str =
                    ($"{bb} A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.Status.Value:X2} SP:{cpu.SP:X2} Cycles: {e.ElapsedCycles}"
                    );
                //File.AppendAllText("c:/tmp/ki.log", str + "\n");
                Console.WriteLine(str);
            }
            catch{}
        }
    }
}