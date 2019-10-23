namespace emulator6502
{
    public enum BindingMode
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
