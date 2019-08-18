namespace emulator6502
{
    public class Opcode
    {
        public OpcodeEnum Enum { get;  }
        public byte Code { get; }
        public BindingMode Mode { get; }
        public byte Cycles { get; }

        internal Opcode(byte code, OpcodeEnum @enum, BindingMode mode, byte cycles)
        {
            Code = code;
            Enum = @enum;
            Mode = mode;
            Cycles = cycles;
        }

        public override string ToString()
        {
            return Enum + "[" + Mode + "]";
        }
    }
}
