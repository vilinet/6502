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
            var rom = new Rom(0x4001);

            var bus = new Bus();
            var cpu = new Cpu(bus);
            if(File.Exists("c:/tmp/my.log")) File.Delete("c:/tmp/my.log");
            if(File.Exists("c:/tmp/stack.txt")) File.Delete("c:/tmp/stack.txt");
            rom.LoadBinaryProgram( File.ReadAllBytes( @"C:\code\6502\nestest.nes")[0x0010 .. 0x4000], 0x0);
            bus.AddMap(0x0, 0x0800, ram);
            bus.AddMap(0x8000, 0x8000+0x4000-1, rom);
            bus.AddMap(0xC000, (ushort)(0xC000+0x4000-1), rom);

            //Start vector for cpu (first absolute address of rom)
            bus.Write(0xFFFC, 0xC000);

            //cpu.AfterOperationExecuted += Cpu_AfterOperationExecuted;
            
            cpu.Reset();
            cpu.Execute(OpcodeEnum.PHA, BindingMode.Implied);
            cpu.Execute(OpcodeEnum.PHA, BindingMode.Implied);
            cpu.Execute(OpcodeEnum.SEI, BindingMode.Implied);


            cpu.BeforeOperationExecuted += Cpu_BeforeOperationExecuted;

            cpu.Clock();
            cpu.Clock();
            cpu.Clock();
            cpu.Clock();
            cpu.Run();
        }

        private static void Cpu_AfterOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
        }

        private static void Cpu_BeforeOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            var bb = e.Full.ToString(cpu.GetValue(e.Full)).PadRight(40);
            var str = ( $"{bb} A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.Status.Value:X2} SP:{cpu.SP:X2} ");
            Console.WriteLine(str);
            File.AppendAllText("c:/tmp/my.log", str+"\n");
        }
    }
}