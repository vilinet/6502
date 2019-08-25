namespace emulator6502
{
    public class StatusRegister
    {
        public bool Carry { get; internal set; }
        public bool Zero { get; internal  set; }
        public bool InterruptDisable { get; internal  set; }
        public bool DecimalMode { get; internal  set; }
        public bool BreakInterrupt { get; internal  set; }
        public bool NonUsed { get; set; } = true;
        public bool Overflow { get; internal  set; }
        public bool Negative { get; internal set; }

        public void Reset()
        {
            Carry = Zero = InterruptDisable = DecimalMode = BreakInterrupt = Overflow = Negative = false;
        }

        public byte Value
        {
            get => (byte)((Carry?1:0) + ((Zero?1:0) << 1) + ((InterruptDisable ?1:0) << 2) +( (DecimalMode?1:0) << 3) +( (BreakInterrupt?1:0 )<< 4) + ((NonUsed? 1 : 0) << 5) + ((Overflow ? 1 : 0) << 6) + ((Negative?1:0 )<< 7));
        }

        public override string ToString()
        {
            return ("N:" + (Negative ? "1" : "0") + " ") +
                   ("Z:" + (Zero ? "1" : "0") + " ") +
                   ("V:" + (Overflow ? "1" : "0") + " ") +
                   ("C:" + (Carry ? "1" : "0") + "   ") +
                   ("I:" + (InterruptDisable ? "1" : "0") + ", ") +
                   ("B:" + (BreakInterrupt ? "1" : "0") + ", ") +
                   ("D:" + (DecimalMode? "1" : "0") + ", ");
        }
    }
}
