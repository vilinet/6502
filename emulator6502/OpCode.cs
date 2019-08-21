namespace emulator6502
{
    public class Opcode
    {
        public OpcodeEnum Enum { get;  }
        public byte Code { get; }
        public BindingMode Mode { get; }
        public byte Cycles { get; }

        private ushort _length = 0xFFFF;
        public ushort Length
        {
            get
            {
                if (_length == 0xFFFF)
                {
                    if (Mode == BindingMode.Implied) _length = 0;
                    else if (Mode == BindingMode.Absolute || Mode == BindingMode.AbsoluteX ||
                             Mode == BindingMode.AbsoluteY)
                    {
                        _length = 2;
                    }
                    else _length = 1;
                }

                return _length;
            }
        }

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
