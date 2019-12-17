using System;
using System.Collections.Generic;

namespace emulator6502
{
    public class FullOpcode
    {
        public Opcode Opcode { get; }
        public ushort Parameter { get; }
        public ushort Position { get; }

        public FullOpcode(Opcode opcode, ushort parameter, ushort position)
        {
            Opcode = opcode;
            Parameter = parameter;
            Position = position;
        }

        private static List<OpcodeEnum> enumsToWriteValue = new List<OpcodeEnum> { OpcodeEnum.BIT, OpcodeEnum.LDA, OpcodeEnum.STA,  OpcodeEnum.STX,  OpcodeEnum.STY, OpcodeEnum.LDY, OpcodeEnum.LDX  };
        public string ToString(ushort outsideParameter)
        {
            string bytes;
            string valChange = "";

            if (Opcode.Enum == OpcodeEnum.DB)
            {
                bytes = Parameter.ToString("X2");
            }
            else
            {
                bytes = Opcode.Code.ToString("X2");
                if (Opcode.Length == 1)
                {
                    bytes += " " +  Parameter.ToString("X2");
                }
                else if (Opcode.Length == 2)
                {
                    bytes += " " +  (Parameter & 0xFF).ToString("X2") + " " + ((Parameter & 0xFF00)>>8).ToString("X2") ;
                }
            }
            
            if (enumsToWriteValue.Contains(Opcode.Enum) && Opcode.Mode!= AddressingMode.Immediate)
            {
                valChange = "= " + outsideParameter.ToString("X2");
            }
            return $"{Position:X4}  {bytes.PadRight(10)}  {Format(outsideParameter)} {valChange}";
        }

        private string Format(ushort outsideParameter)
        {
            if (Opcode.Enum == OpcodeEnum.DB)
            {
                return $".DB ${Parameter:X2}";
            }
            if (Opcode.Mode == AddressingMode.Implied)
                return Opcode.Enum.ToString();


            if (Opcode.Enum == OpcodeEnum.BNE || Opcode.Enum == OpcodeEnum.BCC || Opcode.Enum == OpcodeEnum.BCS ||
                Opcode.Enum == OpcodeEnum.BMI || Opcode.Enum == OpcodeEnum.BEQ || Opcode.Enum == OpcodeEnum.BPL|| Opcode.Enum == OpcodeEnum.BVC ||  Opcode.Enum == OpcodeEnum.BVS)
            {
                return $"{Opcode.Enum} ${outsideParameter:X4}";
            }
            
            var op = Opcode.Enum.ToString() + " ";
            
            switch (Opcode.Mode)
            {
                case AddressingMode.Immediate:
                    return op + $"#${Parameter:X2}";
                
                case AddressingMode.ZeroPage:
                    return op + $"${Parameter:X2}";
                
                case AddressingMode.ZeroPageX:
                    return op + $"${Parameter:X2},X";
                
                case AddressingMode.ZeroPageY:
                    return op + $"${Parameter:X2},Y";
                
                case  AddressingMode.Absolute:
                    return op +$"${Parameter:X4}";
                
                case  AddressingMode.AbsoluteX:
                    return op + $"${Parameter:X4},X";
                
                case  AddressingMode.AbsoluteY:
                    return op + $"${Parameter:X4},Y";

                case   AddressingMode.IndexedIndirect:
                    return $"{op} (${Parameter:X2},X)";
                
                case AddressingMode.IndirectIndexed:
                    return $"{op} (${Parameter:X2}),Y";
                
                case AddressingMode.Indirect:
                    return op + $"(${Parameter:X4})";
                
                case AddressingMode.Relative:
                    return op + $"${Parameter:X2}";
                
                default:
                    throw new Exception("Are you sure?");
            }
        }
    }
}