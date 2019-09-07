using emulator6502;
using NES;
using System;
using System.ComponentModel;
using System.IO;

namespace console
{
    
    static class Program
    {
        static void Main(string[] args)
        {
            //   var ram = new Ram(0x0800);
            // var rom = new Rom(0x4000);

            var nes = new Nes("./nestest.nes");


            nes.PowerOn();
            try
            {
                File.Delete("c:/tmp/ki.log");
            }catch{}

            nes.Cpu.BeforeOperationExecuted += Cpu_BeforeOperationExecuted;
            while (true) nes.Clock();

        }

        private static void Cpu_AfterOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            var bb = e.Full.ToString(cpu.GetValue(e.Full)).PadRight(40);
            var str = ($"{bb} A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.Status.Value:X2} SP:{cpu.SP:X2} Cycles: {e.ElapsedCycles}");
            File.AppendAllText("c:/tmp/ki.log", str + "\n");
            Console.WriteLine(str);
        }

        private static void Cpu_BeforeOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            var bb = e.Full.ToString(cpu.GetValue(e.Full)).PadRight(40);
            var str = ($"{bb} A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.Status.Value:X2} SP:{cpu.SP:X2} Cycles: {e.ElapsedCycles}");
            //File.AppendAllText("c:/tmp/ki.log", str + "\n");
            Console.WriteLine(str);
        }
    }
}