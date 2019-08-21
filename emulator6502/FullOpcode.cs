using System;

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

        public override string ToString()
        {
            return $"${Position:X4}: {Format()}";
        }

        private string Format()
        {
            if (Opcode.Mode == BindingMode.Implied)
                return Opcode.Enum.ToString();
            
            var op = Opcode.Enum.ToString() + " ";
            
            switch (Opcode.Mode)
            {
                case BindingMode.Immediate:
                    return op + $"#${Parameter:X2}";
                
                case BindingMode.ZeroPage:
                    return op + $"$({Parameter:X2})";
                
                case BindingMode.ZeroPageX:
                    return op + $"${Parameter:X2}, X";
                
                case BindingMode.ZeroPageY:
                    return op + $"${Parameter:X2}, Y";
                
                case  BindingMode.Absolute:
                    return op +$"${Parameter:X4}";
                
                case  BindingMode.AbsoluteX:
                    return op + $"${Parameter:X4}, X";
                
                case  BindingMode.AbsoluteY:
                    return op + $"${Parameter:X4}, Y";

                case   BindingMode.IndexedIndirect:
                    return $"{op} $({Parameter:X2},X)";
                
                case BindingMode.IndirectIndexed:
                    return $"{op} $({Parameter:X2}),Y";
                
                case BindingMode.Indirect:
                    return op + $"$({Parameter:X4})";
                
                default:
                    throw new Exception("Are you sure?");
            }
        }
    }
}