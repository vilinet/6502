using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using emulator6502;
using Microsoft.VisualBasic.CompilerServices;

namespace CodeTerminal
{
    public class Terminal
    {
        private readonly Opcodes _opcodes;
        private readonly Cpu _cpu;
        private readonly Bus _bus;

        public Terminal()
        {
            _opcodes = new Opcodes();
            _bus = new Bus();
            _bus.AddMap(new Ram());
            _cpu = new Cpu(_bus);
        }

        private string R(bool val)
        {
            return val ? "1" : "0";
        }
        private void Print()
        {
            Console.SetCursorPosition(0,0);
            Console.WriteLine($"C: {R(_cpu.Status.Carry)} N:{R(_cpu.Status.Negative)} O:{R(_cpu.Status.Overflow)} Z:{R(_cpu.Status.Zero)}                        ");
            Console.WriteLine($"SP: {_cpu.SP}");
            Console.WriteLine($"A: {_cpu.A:X2} X:{_cpu.X:X2} Y:{_cpu.Y:X2}");
            
            for (ushort i = 0; i < 32 ; i++)
            {
                if(i==16) Console.Write("      ");
                if(i%8==0 && i>0) Console.Write("  ");
                Console.Write(_bus.Read(i).ToString("X2") + " ");
                
            }
            Console.SetCursorPosition(0,10);
            Console.Write("                                                   ");
            Console.SetCursorPosition(0,10);
        }

        public void Run()
        {
            Console.WriteLine("Interpreter is running...");

            var reader = Console.In;
            string line = null;
            do
            {
                Print();
                line = reader.ReadLine();
                var command = Parse(line);
                if (command != null)
                {
                    _cpu.Execute(command.Opcode, command.Parameter);
                }
            } while (line != "exit");
        }

        private string[] Tokenize(string line)
        {
            var parts = new List<string>();
            var part = string.Empty;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == ' ' || c == ',')
                {
                    parts.Add(part);
                    part = string.Empty;
                }
                else if (c == '(' || c == ')' )
                {
                    parts.Add(part);
                    parts.Add(c.ToString());
                    part = string.Empty;
                }
                else
                {
                    part += c;
                }
            }

            parts.Add(part);
            return parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        private FullOpcode Parse(string line)
        {
            if (string.IsNullOrEmpty(line)) return null;
            line = line.Trim();
            var parts = Tokenize(line);

            if (parts[0].Length < 0) return null;

            if (Enum.TryParse(parts[0], true, out OpcodeEnum opcode))
            {
                ushort parameter = 0;
                BindingMode mode = BindingMode.Implied;
                if (parts.Length > 1)
                {
                    //All indirect bindings
                    if (parts[1] == "(")
                    {
                        parameter = ParseNumber(parts[2], out mode);

                        if (parts.Length == 5)
                            mode = parts[3].ToUpper() == "X"
                                ? BindingMode.IndexedIndirect
                                : BindingMode.IndirectIndexed;
                        else if (parts.Length == 4)
                            mode = BindingMode.Indirect;
                    }
                    else
                    {
                        parameter = ParseNumber(parts[1], out mode);
                        bool? X = null;
                        if (mode == BindingMode.Implied)
                        {
                            if (parts.Length == 3) X = parts[2].ToUpper() == "X";

                            if (parameter > 255)
                            {
                                if (!X.HasValue) mode = BindingMode.Absolute;
                                else if (X == false) mode = BindingMode.AbsoluteY;
                                else mode = BindingMode.AbsoluteX;
                            }
                            else
                            {
                                if (!X.HasValue) mode = BindingMode.ZeroPage;
                                else if (X == false) mode = BindingMode.ZeroPageY;
                                else mode = BindingMode.ZeroPageX;
                            }
                        }
                    }
                }
                
                return new FullOpcode(_opcodes.Get(opcode, mode), parameter, 0);
            }
            else Console.WriteLine("Invalid opcode");

            return null;
        }

        private ushort ParseNumber(string value, out BindingMode mode)
        {
            mode = value.StartsWith("#") ? BindingMode.Immediate : BindingMode.Implied;
            value = value.Replace("#", "");
            value = value.Replace("$", "");
            return (ushort) int.Parse(value, NumberStyles.HexNumber);
        }
    }
}