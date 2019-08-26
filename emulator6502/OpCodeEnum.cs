﻿namespace emulator6502
{
    public enum OpcodeEnum
    {
        BRK,
        AND,
        PHP,
        PHA,
        PLA,
        PLP,
        RTS,
        RTI,
        JSR,
        STA,
        LDA,
        LDX,
        LDY,
        STY,
        STX,
        DEX,
        DEY,
        JMP,
        CMP,
        CPX,
        CPY,
        DEC,
        INC,
        INX,
        INY,
        NOP,
        SEC,
        SEI,
        SED,
        CLI,
        CLC,
        CLD,
        CLV,
        TAX,
        TAY,
        TSX,
        TXA,
        TXS,
        BIT,
        ASL,
        ORA,
        EOR,
        TYA,
        ROL,
        ROR,
        ADC,
        SBC,
        BCS,
        BCC,
        BEQ,
        BNE,
        BMI,
        BPL,
        BVS,
        BVC,
        //Non used in assembler, every other stuff
        DB,
        LSR,
        LAX,
        SAX
    }
}
