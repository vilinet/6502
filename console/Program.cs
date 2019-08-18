using emulator6502;
using System;

namespace Runner
{
    public class PPU : Addressable
    {
        public PPU(ushort size) : base(size) { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ram = new Ram(0x200);
            var ppu = new PPU(0xFF);
            var rom = new Rom(0x8000);

            var bus = new Bus();
            var cpu = new Cpu(bus);

            bus.AddMap(0,      0x200, ram);
            bus.AddMap(0x200,  0x2FF,  ppu);
            bus.AddMap(0x8000, 0xFFFF, rom);
            
            //Start vector for cpu (first absolute address of rom)
            bus.Write(0xFFFC, 0x8000);

            var codes = new Opcodes();

            //this color should draw 2x4 color "pixels" into the ppu
            rom.LoadBinaryProgram(
                "a9 00 a2 00 9d 00 02 18 69 01 e8 9d 00 02 18 69"+
                "01 e8 9d 00 02 18 69 01 e8 9d 00 02 48 8a 18 69"+
                "20 aa 68 18 69 01 9d 00 02 18 69 01 ca 9d 00 02"+
                "18 69 01 ca 9d 00 02 18 69 01 ca 9d 00 02");

            cpu.BeforeOperationExecuted += Cpu_OnExecutingOperation;
            cpu.AfterOperationExecuted += Cpu_AfterOperationExecuted;

            cpu.Run();

            int width = 32;
            var colors = new ConsoleColor[] { ConsoleColor.Black, ConsoleColor.White, ConsoleColor.Red, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.DarkYellow };

            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                        Console.ForegroundColor = colors[ppu[j * width + i]];
                        Console.Write("M");
                }
                Console.WriteLine();
            }
        }

        private static void Cpu_AfterOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            Console.WriteLine($"A: {cpu.A:X2}, X: {cpu.X:X2}, Y: {cpu.Y:X2}, SP:{cpu.SP:X2}");
            Console.WriteLine();
        }

        private static void Cpu_OnExecutingOperation(object sender, OpcodeEventArgs e)
        {
            Console.WriteLine(e.Opcode.ToString() + " " + ((e.Opcode.Mode != BindingMode.Implied) ? e.Parameter.ToString("X4") : "")); ;
        }
    }
}
