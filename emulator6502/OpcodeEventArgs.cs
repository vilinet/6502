using System;

namespace emulator6502
{
    public class OpcodeEventArgs : EventArgs
    {
        public Opcode Opcode { get; private set; }
        public ushort Parameter { get; private set; }
        public bool RequestPauseExecution { get; set; }

        public OpcodeEventArgs(Opcode opcode, ushort parameter)
        {
            Opcode = opcode;
            Parameter = parameter;
        }
    }
}
