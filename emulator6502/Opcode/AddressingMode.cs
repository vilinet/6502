namespace emulator6502
{
    public enum AddressingMode
    {
        Implied,
        Immediate,
        ZeroPage,
        ZeroPageX,
        ZeroPageY,
        Absolute,
        AbsoluteX,
        AbsoluteY,
        Indirect,
        /// <summary>
        /// Indirect X
        /// </summary>
        IndexedIndirect,
        /// <summary>
        /// Indirect Y
        /// </summary>
        IndirectIndexed,
        Relative
    }
}
