using System;

namespace emulator6502
{
    public class OpcodeEventArgs : EventArgs
    {
        public FullOpcode Full { get; }
        public int ElapsedCycles { get; }

        public bool RequestPauseExecution { get; set; }

        public OpcodeEventArgs(FullOpcode opcode, int elapsedCycles)
        {
            Full = opcode;
            ElapsedCycles = elapsedCycles;
        }
    }
}
