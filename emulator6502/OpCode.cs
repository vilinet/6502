namespace emulator6502
{
    public class Opcode
    {
        public OpcodeEnum Enum { get;  }
        public byte Code { get; }
        public BindingMode Mode { get; }
        public byte Cycles { get; }
        public ushort Length { get; }

        internal Opcode(byte code, OpcodeEnum @enum, BindingMode mode, byte cycles)
        {
            Code = code;
            Enum = @enum;
            Mode = mode;
            Cycles = cycles;

            if (Mode == BindingMode.Implied) Length = 0;
            else if (Mode == BindingMode.Absolute || Mode == BindingMode.AbsoluteX ||
                     Mode == BindingMode.AbsoluteY || Mode == BindingMode.Indirect)
                Length = 2;
            else Length = 1;
        }

        public override string ToString()
        {
            return Enum + "[" + Mode + "]";
        }
    }
}
