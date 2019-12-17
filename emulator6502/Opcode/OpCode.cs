namespace emulator6502
{
    public class Opcode
    {
        public OpcodeEnum Enum { get;  }
        public byte Code { get; }
        public AddressingMode Mode { get; }
        public byte Cycles { get; }
        public ushort Length { get; }

        internal Opcode(byte code, OpcodeEnum @enum, AddressingMode mode, byte cycles)
        {
            Code = code;
            Enum = @enum;
            Mode = mode;
            Cycles = cycles;

            if (Mode == AddressingMode.Implied) Length = 0;
            else if (Mode == AddressingMode.Absolute || Mode == AddressingMode.AbsoluteX ||
                     Mode == AddressingMode.AbsoluteY || Mode == AddressingMode.Indirect)
                Length = 2;
            else Length = 1;
        }

        public override string ToString()
        {
            return Enum + "[" + Mode + "]";
        }
    }
}
