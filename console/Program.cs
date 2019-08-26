using emulator6502;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace console
{
    public class PPU : Addressable
    {
        public PPU(ushort size) : base(size) { }
    }
    
    static class Program
    {
        static void Main(string[] args)
        {
            var ram = new Ram(0x0800);
            var rom = new Rom(0x4000);

            var bus = new Bus();
            var cpu = new Cpu(bus);

            rom.LoadBinaryProgram( File.ReadAllBytes( @"C:\code\6502\nestest.nes")[0x0010 .. 0x4000], 0x0);
            bus.AddMap(0x0, ram);
            bus.AddMap(0x4000,  new Ram(0x4000) );
            bus.AddMap(0x8000,  rom);
            bus.AddMap(0xC000,  rom);

            //Start vector for cpu (first absolute address of rom)
            bus.Write(0xFFFC, 0xC000);

            //cpu.AfterOperationExecuted += Cpu_AfterOperationExecuted;


            /*cpu.Execute(OpcodeEnum.PHA, BindingMode.Implied);
            cpu.Execute(OpcodeEnum.PHA, BindingMode.Implied);
            cpu.Execute(OpcodeEnum.SEI, BindingMode.Implied);*/


            // cpu.BeforeOperationExecuted += Cpu_BeforeOperationExecuted;

            var start = DateTime.Now;
            int runCount = 1000;
            int i = 0;
            long cycles = 0;
            while (i++ < runCount)
            {
                cpu.Reset();
                cpu.Run();
                cycles += cpu.Cycles;
            }
            Console.WriteLine($"Finished: {runCount} runs,  {(DateTime.Now - start).TotalMilliseconds} milliseconds, cycles: {cycles}");

        }

        private static void Cpu_AfterOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
        }

        private static void Cpu_BeforeOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            var bb = e.Full.ToString(cpu.GetValue(e.Full)).PadRight(40);
            var str = ( $"{bb} A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.Status.Value:X2} SP:{cpu.SP:X2} ");
           // Console.WriteLine(str);
        }
    }
}