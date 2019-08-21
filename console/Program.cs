using emulator6502;
using System;

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
            var ram = new Ram(0x0200);
            var ppu = new PPU(0x00FF);
            var rom = new Rom(0x8000);

            var bus = new Bus();
            var cpu = new Cpu(bus);

            bus.AddMap(0,      0x200, ram);
            bus.AddMap(0x200,  0x2FF,  ppu);
            bus.AddMap(0x8000, 0xFFFF, rom);
            
            //Start vector for cpu (first absolute address of rom)
            bus.Write(0xFFFC, 0x8000);
            var code = "a9 00 a2 00 9d 00 02 18 69 01 e8 9d 00 02 18 69" +
                       "01 e8 9d 00 02 18 69 01 e8 9d 00 02 48 8a 18 69" +
                       "20 aa 68 18 69 01 9d 00 02 18 69 01 ca 9d 00 02" +
                       "18 69 01 ca 9d 00 02 18 69 01 ca 9d 00 02";
            
            Decompiler decompiler = new Decompiler();
            foreach (var line in decompiler.Decompile(code))
            {
                Console.WriteLine(line);
            }
            return;
            //this color should draw 2x4 color "pixels" into the ppu
            rom.LoadBinaryProgram(code);
            
            cpu.BeforeOperationExecuted += Cpu_BeforeOperationExecuted;
            cpu.AfterOperationExecuted += Cpu_AfterOperationExecuted;
            cpu.Execute(OpcodeEnum.LDA, BindingMode.Immediate, 0x0F );
            cpu.Execute(OpcodeEnum.AND, BindingMode.Immediate, 0x0F );
            
            cpu.Run();
            
            var width = 32;
            var colors = new[] { ConsoleColor.Black, ConsoleColor.White, ConsoleColor.Red, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.DarkYellow };
            
            for (var j = 0; j < 2; j++)
            {
                for (var i = 0; i < 4; i++)
                {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = colors[ppu[j * width + i]];
                        Console.Write("M");
                } 
                Console.WriteLine();
            }

            Console.ReadLine();
        }

        private static void Cpu_AfterOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            Console.WriteLine($"A: {cpu.A:X2}, X: {cpu.X:X2}, Y: {cpu.Y:X2}, SP:{cpu.SP:X2}");
            Console.WriteLine();
        }

        private static void Cpu_BeforeOperationExecuted(object sender, OpcodeEventArgs e)
        {
            Console.WriteLine(e.Opcode.ToString() + " " + ((e.Opcode.Mode != BindingMode.Implied) ? e.Parameter.ToString("X4") : "")); ;
        }
    }
}