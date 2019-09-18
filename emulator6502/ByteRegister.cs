namespace emulator6502
{
    public class ByteRegister
    {
        public byte Value { get; set; }

        protected bool GetBooleanBit(int n)
        {
            return (Value & (1 << n)) != 0;
        }

        protected byte GetBit(int n)
        {
            return (byte)((Value & (1 << n)) >> n);
        }

        protected void SetBoolanBit(int n, bool bit)
        {
            Value ^= (byte)((-(bit ? 1 : 0) ^ Value) & (1 << n));
        }

        protected void SetBit(int n, byte bit)
        {
            Value ^= (byte)((-bit ^ Value) & (1 << n));
        }

        public static implicit operator byte(ByteRegister @class)
        {
            return @class.Value;
        }

        public override string ToString()
        {
            return "7_6_5_4_3_2_1_0\n" + $"{GetBit(7)}|{GetBit(6)}|{GetBit(5)}|{GetBit(4)}|{GetBit(3)}|{GetBit(2)}|{GetBit(1)}|{GetBit(0)}\n";
        }
    }
}