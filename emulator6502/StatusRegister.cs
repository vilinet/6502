namespace emulator6502
{
    public class StatusRegister : BooleanByteRegister
    {
        public bool Carry
        {
            get => Bit0;
            set => Bit0 = value;
        }

        public bool Zero
        {
            get => Bit1;
            set => Bit1 = value;
        }

        public bool InterruptDisable
        {
            get => Bit2;
            set => Bit2 = value;
        }

        public bool DecimalMode
        {
            get => Bit3;
            set => Bit3 = value;
        }

        public bool BreakInterrupt
        {
            get => Bit4;
            set => Bit4 = value;
        }

        public bool NonUsed { get; } = true;

        public bool Overflow
        {
            get => Bit6;
            set => Bit6 = value;
        }

        public bool Negative
        {
            get => Bit7;
            set => Bit7 = value;
        }

        public void Reset()
        {
            Carry = Zero = InterruptDisable = DecimalMode = BreakInterrupt = Overflow = Negative = false;
        }

        public override string ToString()
        {
            return ("N:" + (Negative ? "1" : "0") + " ") +
                   ("Z:" + (Zero ? "1" : "0") + " ") +
                   ("V:" + (Overflow ? "1" : "0") + " ") +
                   ("C:" + (Carry ? "1" : "0") + "   ") +
                   ("I:" + (InterruptDisable ? "1" : "0") + ", ") +
                   ("B:" + (BreakInterrupt ? "1" : "0") + ", ") +
                   ("D:" + (DecimalMode ? "1" : "0") + ", ");
        }
    }
}