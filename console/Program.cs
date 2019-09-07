﻿using emulator6502;
using NES;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NES.Display;

namespace console
{
    
    static class Program
    {
        private static Nes nes;
        
        static void Main(string[] args)
        {
            //   var ram = new Ram(0x0800);
            // var rom = new Rom(0x4000);
            var display = new SFMLDisplay(256,240);
            nes = new Nes(display, "./nestest.nes");


            nes.RunOnThread();
            int counter = 0;
            while (display.Window.IsOpen)
            {
                display.DispatchEvents();
                display.Render();
                counter++;
                if (counter == 1000000)
                {
                    if(nes.State == NesState.Paused) nes.Reset();
                    else nes.Pause();
                    counter = 0;

                }
            }
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