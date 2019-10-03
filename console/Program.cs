using emulator6502;
using NES;
using NES.Display.SDL2;
using System;

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
                    ($"{bb} A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.Status.Value:X2} SP:{cpu.SP:X2} Cycles: {e.ElapsedCycles}");
                Console.WriteLine(str);
            }
            catch { }
        }
    }
}