
using System;

namespace emulator6502
{
    public class OpCodes
    {
        private readonly Opcode[] _opcodes = new Opcode[256];
        private readonly Action<ushort, AddressingMode>[] _opcodeActions = new Action<ushort, AddressingMode>[256];
        private readonly Cpu _cpu;


        /// <summary>
        /// Byte opcode
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public Opcode this[int opcode] { get => _opcodes[opcode]; }

        private void Insert(Opcode opcode, Action<ushort, AddressingMode> action)
        {
            _opcodes[opcode.Code] = opcode;
            _opcodeActions[(int)opcode.Enum] = action;
        }

        public void Execute(ushort param, Opcode opcode)
        {
            _opcodeActions[(int)opcode.Enum](param, opcode.Mode);
        }

        /// <summary>
        /// Try to avoid using this function in every step
        /// It takes much time 
        /// Cache the result
        /// </summary>
        /// <param name="opcodeEnum"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Opcode Get(OpcodeEnum opcodeEnum, AddressingMode mode)
        {
            for (int i = 0; i < 256; i++)
            {
                var op = _opcodes[i];

                if (op != null && op.Enum == opcodeEnum && mode == op.Mode) return op;
            }

            return null;
        }

        /// <summary>
        /// Try to avoid using this function in every step
        /// It takes much time 
        /// Cache the result
        /// </summary>
        /// <param name="opcodeEnum"></param>
        /// <returns></returns>
        public Opcode Get(OpcodeEnum opcodeEnum)
        {
            for (int i = 0; i < 256; i++)
            {
                var op = _opcodes[i];
                if (op?.Enum == opcodeEnum) return op;
            }

            return null;
        }

        public OpCodes(Cpu cpu)
        {
            _cpu = cpu;

            #region LOAD
            Insert(new Opcode(0xA9, OpcodeEnum.LDA, AddressingMode.Immediate, 2), Lda);
            Insert(new Opcode(0xA5, OpcodeEnum.LDA, AddressingMode.ZeroPage, 3), Lda);
            Insert(new Opcode(0xB5, OpcodeEnum.LDA, AddressingMode.ZeroPageX, 4), Lda);
            Insert(new Opcode(0xAD, OpcodeEnum.LDA, AddressingMode.Absolute, 4), Lda);
            Insert(new Opcode(0xBD, OpcodeEnum.LDA, AddressingMode.AbsoluteX, 4), Lda);
            Insert(new Opcode(0xB9, OpcodeEnum.LDA, AddressingMode.AbsoluteY, 4), Lda);
            Insert(new Opcode(0xA1, OpcodeEnum.LDA, AddressingMode.IndexedIndirect, 6), Lda);
            Insert(new Opcode(0xB1, OpcodeEnum.LDA, AddressingMode.IndirectIndexed, 5), Lda);

            Insert(new Opcode(0xA2, OpcodeEnum.LDX, AddressingMode.Immediate, 2), Ldx);
            Insert(new Opcode(0xA6, OpcodeEnum.LDX, AddressingMode.ZeroPage, 3), Ldx);
            Insert(new Opcode(0xB6, OpcodeEnum.LDX, AddressingMode.ZeroPageY, 4), Ldx);
            Insert(new Opcode(0xAE, OpcodeEnum.LDX, AddressingMode.Absolute, 4), Ldx);
            Insert(new Opcode(0xBE, OpcodeEnum.LDX, AddressingMode.AbsoluteY, 4), Ldx);

            Insert(new Opcode(0xA0, OpcodeEnum.LDY, AddressingMode.Immediate, 2), Ldy);
            Insert(new Opcode(0xA4, OpcodeEnum.LDY, AddressingMode.ZeroPage, 3),Ldy);
            Insert(new Opcode(0xB4, OpcodeEnum.LDY, AddressingMode.ZeroPageX, 4), Ldy);
            Insert(new Opcode(0xAC, OpcodeEnum.LDY, AddressingMode.Absolute, 4), Ldy);
            Insert(new Opcode(0xBC, OpcodeEnum.LDY, AddressingMode.AbsoluteX, 4), Ldy);

            #endregion

            #region STORE

            Insert(new Opcode(0x85, OpcodeEnum.STA, AddressingMode.ZeroPage, 3), Sta);
            Insert(new Opcode(0x95, OpcodeEnum.STA, AddressingMode.ZeroPageX, 4), Sta);
            Insert(new Opcode(0x8D, OpcodeEnum.STA, AddressingMode.Absolute, 4), Sta);
            Insert(new Opcode(0x9D, OpcodeEnum.STA, AddressingMode.AbsoluteX, 5), Sta);
            Insert(new Opcode(0x99, OpcodeEnum.STA, AddressingMode.AbsoluteY, 5), Sta);
            Insert(new Opcode(0x81, OpcodeEnum.STA, AddressingMode.IndexedIndirect, 6), Sta);
            Insert(new Opcode(0x91, OpcodeEnum.STA, AddressingMode.IndirectIndexed, 6), Sta);

            Insert(new Opcode(0x84, OpcodeEnum.STY, AddressingMode.ZeroPage, 3), Sty);
            Insert(new Opcode(0x94, OpcodeEnum.STY, AddressingMode.ZeroPageX, 4), Sty);
            Insert(new Opcode(0x8C, OpcodeEnum.STY, AddressingMode.Absolute, 4), Sty);

            Insert(new Opcode(0x86, OpcodeEnum.STX, AddressingMode.ZeroPage, 3),Stx);
            Insert(new Opcode(0x96, OpcodeEnum.STX, AddressingMode.ZeroPageY, 4), Stx);
            Insert(new Opcode(0x8E, OpcodeEnum.STX, AddressingMode.Absolute, 4), Stx);

            #endregion

            #region Flags

            Insert(new Opcode(0x38, OpcodeEnum.SEC, AddressingMode.Implied, 2), Sec);
            Insert(new Opcode(0x78, OpcodeEnum.SEI, AddressingMode.Implied, 2), Sei);
            Insert(new Opcode(0xF8, OpcodeEnum.SED, AddressingMode.Implied, 2), Sed);
            Insert(new Opcode(0x18, OpcodeEnum.CLC, AddressingMode.Implied, 2), Clc);
            Insert(new Opcode(0xD8, OpcodeEnum.CLD, AddressingMode.Implied, 2), Cld);
            Insert(new Opcode(0x58, OpcodeEnum.CLI, AddressingMode.Implied, 2), Cli);
            Insert(new Opcode(0xB8, OpcodeEnum.CLV, AddressingMode.Implied, 2), Clv);

            #endregion

            #region Transfer

            Insert(new Opcode(0xAA, OpcodeEnum.TAX, AddressingMode.Implied, 2), Tax);
            Insert(new Opcode(0xA8, OpcodeEnum.TAY, AddressingMode.Implied, 2), Tay);
            Insert(new Opcode(0xBA, OpcodeEnum.TSX, AddressingMode.Implied, 2), Tsx);
            Insert(new Opcode(0x8A, OpcodeEnum.TXA, AddressingMode.Implied, 2), Txa);
            Insert(new Opcode(0x9A, OpcodeEnum.TXS, AddressingMode.Implied, 2), Txs);
            Insert(new Opcode(0x98, OpcodeEnum.TYA, AddressingMode.Implied, 2), Tya);

            #endregion

            #region CMP             

            Insert(new Opcode(0xC9, OpcodeEnum.CMP, AddressingMode.Immediate, 2), Cmp);
            Insert(new Opcode(0xC5, OpcodeEnum.CMP, AddressingMode.ZeroPage, 3), Cmp);
            Insert(new Opcode(0xD5, OpcodeEnum.CMP, AddressingMode.ZeroPageX, 4), Cmp);
            Insert(new Opcode(0xCD, OpcodeEnum.CMP, AddressingMode.Absolute, 4), Cmp);
            Insert(new Opcode(0xDD, OpcodeEnum.CMP, AddressingMode.AbsoluteX, 4), Cmp);
            Insert(new Opcode(0xD9, OpcodeEnum.CMP, AddressingMode.AbsoluteY, 4), Cmp);
            Insert(new Opcode(0xC1, OpcodeEnum.CMP, AddressingMode.IndexedIndirect, 6), Cmp);
            Insert(new Opcode(0xD1, OpcodeEnum.CMP, AddressingMode.IndirectIndexed, 5), Cmp);

            Insert(new Opcode(0xE0, OpcodeEnum.CPX, AddressingMode.Immediate, 2), Cpx);
            Insert(new Opcode(0xE4, OpcodeEnum.CPX, AddressingMode.ZeroPage, 3), Cpx);
            Insert(new Opcode(0xEC, OpcodeEnum.CPX, AddressingMode.Absolute, 4), Cpx);

            Insert(new Opcode(0xC0, OpcodeEnum.CPY, AddressingMode.Immediate, 2), Cpy);
            Insert(new Opcode(0xC4, OpcodeEnum.CPY, AddressingMode.ZeroPage, 3), Cpy);
            Insert(new Opcode(0xCC, OpcodeEnum.CPY, AddressingMode.Absolute, 4), Cpy);

            #endregion

            #region DEC             
            Insert(new Opcode(0xC6, OpcodeEnum.DEC, AddressingMode.ZeroPage, 5), Dec);
            Insert(new Opcode(0xD6, OpcodeEnum.DEC, AddressingMode.ZeroPageX, 6), Dec);
            Insert(new Opcode(0xCE, OpcodeEnum.DEC, AddressingMode.Absolute, 6), Dec);
            Insert(new Opcode(0xDE, OpcodeEnum.DEC, AddressingMode.AbsoluteX, 7), Dec);

            Insert(new Opcode(0xCA, OpcodeEnum.DEX, AddressingMode.Implied, 2), Dex);
            Insert(new Opcode(0x88, OpcodeEnum.DEY, AddressingMode.Implied, 2), Dey);
            #endregion

            #region INC
            Insert(new Opcode(0xE6, OpcodeEnum.INC, AddressingMode.ZeroPage, 5), Inc);
            Insert(new Opcode(0xF6, OpcodeEnum.INC, AddressingMode.ZeroPageX, 6), Inc);
            Insert(new Opcode(0xEE, OpcodeEnum.INC, AddressingMode.Absolute, 6), Inc);
            Insert(new Opcode(0xFE, OpcodeEnum.INC, AddressingMode.AbsoluteX, 7), Inc);

            Insert(new Opcode(0xE8, OpcodeEnum.INX, AddressingMode.Implied, 2), Inx);
            Insert(new Opcode(0xC8, OpcodeEnum.INY, AddressingMode.Implied, 2), Iny);
            #endregion

            #region Routines       
            Insert(new Opcode(0x00, OpcodeEnum.BRK, AddressingMode.Implied, 7), Brk);
            Insert(new Opcode(0x40, OpcodeEnum.RTI, AddressingMode.Implied, 6), Rti);

            Insert(new Opcode(0xEA, OpcodeEnum.NOP, AddressingMode.Implied, 2), Nop);
            Insert(new Opcode(0x04, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0x0C, OpcodeEnum.NOP, AddressingMode.Absolute, 4), Nop);
            Insert(new Opcode(0x14, OpcodeEnum.NOP, AddressingMode.Immediate, 4), Nop);
            Insert(new Opcode(0x1A, OpcodeEnum.NOP, AddressingMode.Implied, 2), Nop);
            Insert(new Opcode(0x1C, OpcodeEnum.NOP, AddressingMode.Absolute, 4), Nop);
            Insert(new Opcode(0x34, OpcodeEnum.NOP, AddressingMode.Immediate, 4), Nop);

            Insert(new Opcode(0x3A, OpcodeEnum.NOP, AddressingMode.Implied, 2), Nop);
            Insert(new Opcode(0x3C, OpcodeEnum.NOP, AddressingMode.Absolute, 4), Nop);
            Insert(new Opcode(0x44, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0x54, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0x5A, OpcodeEnum.NOP, AddressingMode.Implied, 2), Nop);
            Insert(new Opcode(0x5C, OpcodeEnum.NOP, AddressingMode.Absolute, 4), Nop);
            Insert(new Opcode(0x64, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0x74, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0x7A, OpcodeEnum.NOP, AddressingMode.Implied, 2), Nop);
            Insert(new Opcode(0x7C, OpcodeEnum.NOP, AddressingMode.Absolute, 4), Nop);
            Insert(new Opcode(0x80, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0x82, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0x89, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0xC2, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0xD4, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0xDA, OpcodeEnum.NOP, AddressingMode.Implied, 2), Nop);
            Insert(new Opcode(0xDC, OpcodeEnum.NOP, AddressingMode.Absolute, 4), Nop);
            Insert(new Opcode(0xE2, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0xF4, OpcodeEnum.NOP, AddressingMode.Immediate, 3), Nop);
            Insert(new Opcode(0xFA, OpcodeEnum.NOP, AddressingMode.Implied, 2), Nop);
            Insert(new Opcode(0xFC, OpcodeEnum.NOP, AddressingMode.Absolute, 4), Nop);

            Insert(new Opcode(0x60, OpcodeEnum.RTS, AddressingMode.Implied, 6), Rts);
            Insert(new Opcode(0x20, OpcodeEnum.JSR, AddressingMode.Absolute, 6), Jsr);

            Insert(new Opcode(0x4C, OpcodeEnum.JMP, AddressingMode.Absolute, 3), Jmp);
            Insert(new Opcode(0x6C, OpcodeEnum.JMP, AddressingMode.Indirect, 5), Jmp);

            #endregion

            #region Stack           
            Insert(new Opcode(0x08, OpcodeEnum.PHP, AddressingMode.Implied, 3), Php);
            Insert(new Opcode(0x48, OpcodeEnum.PHA, AddressingMode.Implied, 3), Pha);
            Insert(new Opcode(0x68, OpcodeEnum.PLA, AddressingMode.Implied, 4), Pla);
            Insert(new Opcode(0x28, OpcodeEnum.PLP, AddressingMode.Implied, 4), Plp);
            #endregion

            #region BIT OPERATIONS 
            Insert(new Opcode(0x24, OpcodeEnum.BIT, AddressingMode.ZeroPage, 3), Bit);
            Insert(new Opcode(0x2C, OpcodeEnum.BIT, AddressingMode.Absolute, 4), Bit);

            Insert(new Opcode(0x29, OpcodeEnum.AND, AddressingMode.Immediate, 2), And);
            Insert(new Opcode(0x25, OpcodeEnum.AND, AddressingMode.ZeroPage, 3), And);
            Insert(new Opcode(0x35, OpcodeEnum.AND, AddressingMode.ZeroPageX, 4), And);
            Insert(new Opcode(0x2D, OpcodeEnum.AND, AddressingMode.Absolute, 4), And);
            Insert(new Opcode(0x3D, OpcodeEnum.AND, AddressingMode.AbsoluteX, 4), And);
            Insert(new Opcode(0x39, OpcodeEnum.AND, AddressingMode.AbsoluteY, 4), And);
            Insert(new Opcode(0x21, OpcodeEnum.AND, AddressingMode.IndexedIndirect, 6), And);
            Insert(new Opcode(0x31, OpcodeEnum.AND, AddressingMode.IndirectIndexed, 5), And);

            Insert(new Opcode(0x0A, OpcodeEnum.ASL, AddressingMode.Implied, 2), Asl);
            Insert(new Opcode(0x06, OpcodeEnum.ASL, AddressingMode.ZeroPage, 5), Asl);
            Insert(new Opcode(0x16, OpcodeEnum.ASL, AddressingMode.ZeroPageX, 6), Asl);
            Insert(new Opcode(0x0E, OpcodeEnum.ASL, AddressingMode.Absolute, 6), Asl);
            Insert(new Opcode(0x1E, OpcodeEnum.ASL, AddressingMode.AbsoluteX, 7), Asl);

            Insert(new Opcode(0x4A, OpcodeEnum.LSR, AddressingMode.Implied, 2), Lsr);
            Insert(new Opcode(0x46, OpcodeEnum.LSR, AddressingMode.ZeroPage, 5), Lsr);
            Insert(new Opcode(0x56, OpcodeEnum.LSR, AddressingMode.ZeroPageX, 6), Lsr);
            Insert(new Opcode(0x4E, OpcodeEnum.LSR, AddressingMode.Absolute, 6), Lsr);
            Insert(new Opcode(0x5E, OpcodeEnum.LSR, AddressingMode.AbsoluteX, 7), Lsr);

            Insert(new Opcode(0x09, OpcodeEnum.ORA, AddressingMode.Immediate, 2), Ora);
            Insert(new Opcode(0x05, OpcodeEnum.ORA, AddressingMode.ZeroPage, 3), Ora);
            Insert(new Opcode(0x15, OpcodeEnum.ORA, AddressingMode.ZeroPageX, 4), Ora);
            Insert(new Opcode(0x0D, OpcodeEnum.ORA, AddressingMode.Absolute, 4), Ora);
            Insert(new Opcode(0x1D, OpcodeEnum.ORA, AddressingMode.AbsoluteX, 4), Ora);
            Insert(new Opcode(0x19, OpcodeEnum.ORA, AddressingMode.AbsoluteY, 4), Ora);
            Insert(new Opcode(0x01, OpcodeEnum.ORA, AddressingMode.IndexedIndirect, 6), Ora);
            Insert(new Opcode(0x11, OpcodeEnum.ORA, AddressingMode.IndirectIndexed, 5), Ora);

            Insert(new Opcode(0x49, OpcodeEnum.EOR, AddressingMode.Immediate, 2),Eor);
            Insert(new Opcode(0x45, OpcodeEnum.EOR, AddressingMode.ZeroPage, 3), Eor);
            Insert(new Opcode(0x55, OpcodeEnum.EOR, AddressingMode.ZeroPageX, 4), Eor);
            Insert(new Opcode(0x4D, OpcodeEnum.EOR, AddressingMode.Absolute, 4), Eor);
            Insert(new Opcode(0x5D, OpcodeEnum.EOR, AddressingMode.AbsoluteX, 4), Eor);
            Insert(new Opcode(0x59, OpcodeEnum.EOR, AddressingMode.AbsoluteY, 4), Eor);
            Insert(new Opcode(0x41, OpcodeEnum.EOR, AddressingMode.IndexedIndirect, 6), Eor);
            Insert(new Opcode(0x51, OpcodeEnum.EOR, AddressingMode.IndirectIndexed, 5), Eor);

            Insert(new Opcode(0x2A, OpcodeEnum.ROL, AddressingMode.Implied, 2), Rol);
            Insert(new Opcode(0x26, OpcodeEnum.ROL, AddressingMode.ZeroPage, 5), Rol);
            Insert(new Opcode(0x36, OpcodeEnum.ROL, AddressingMode.ZeroPageX, 6), Rol);
            Insert(new Opcode(0x2E, OpcodeEnum.ROL, AddressingMode.Absolute, 6), Rol);
            Insert(new Opcode(0x3E, OpcodeEnum.ROL, AddressingMode.AbsoluteX, 7), Rol);

            Insert(new Opcode(0x6A, OpcodeEnum.ROR, AddressingMode.Implied, 2), Ror);
            Insert(new Opcode(0x66, OpcodeEnum.ROR, AddressingMode.ZeroPage, 5), Ror);
            Insert(new Opcode(0x76, OpcodeEnum.ROR, AddressingMode.ZeroPageX, 6), Ror);
            Insert(new Opcode(0x6E, OpcodeEnum.ROR, AddressingMode.Absolute, 6), Ror);
            Insert(new Opcode(0x7E, OpcodeEnum.ROR, AddressingMode.AbsoluteX, 7), Ror);

            Insert(new Opcode(0x69, OpcodeEnum.ADC, AddressingMode.Immediate, 2),Adc);
            Insert(new Opcode(0x65, OpcodeEnum.ADC, AddressingMode.ZeroPage, 3), Adc);
            Insert(new Opcode(0x75, OpcodeEnum.ADC, AddressingMode.ZeroPageX, 4), Adc);
            Insert(new Opcode(0x6D, OpcodeEnum.ADC, AddressingMode.Absolute, 4), Adc);
            Insert(new Opcode(0x7D, OpcodeEnum.ADC, AddressingMode.AbsoluteX, 4), Adc);
            Insert(new Opcode(0x79, OpcodeEnum.ADC, AddressingMode.AbsoluteY, 4), Adc);
            Insert(new Opcode(0x61, OpcodeEnum.ADC, AddressingMode.IndexedIndirect, 6), Adc);
            Insert(new Opcode(0x71, OpcodeEnum.ADC, AddressingMode.IndirectIndexed, 5), Adc);

            #endregion
            #region Unoffical
            Insert(new Opcode(0xEB, OpcodeEnum.SBC, AddressingMode.Immediate, 2), Sbc);

            Insert(new Opcode(0xE9, OpcodeEnum.SBC, AddressingMode.Immediate, 2), Sbc);
            Insert(new Opcode(0xE5, OpcodeEnum.SBC, AddressingMode.ZeroPage, 3), Sbc);
            Insert(new Opcode(0xF5, OpcodeEnum.SBC, AddressingMode.ZeroPageX, 4), Sbc);
            Insert(new Opcode(0xED, OpcodeEnum.SBC, AddressingMode.Absolute, 4), Sbc);
            Insert(new Opcode(0xFD, OpcodeEnum.SBC, AddressingMode.AbsoluteX, 4), Sbc);
            Insert(new Opcode(0xF9, OpcodeEnum.SBC, AddressingMode.AbsoluteY, 4), Sbc);
            Insert(new Opcode(0xE1, OpcodeEnum.SBC, AddressingMode.IndexedIndirect, 6), Sbc);
            Insert(new Opcode(0xF1, OpcodeEnum.SBC, AddressingMode.IndirectIndexed, 5), Sbc);
            
            Insert(new Opcode(0xCF, OpcodeEnum.DCP, AddressingMode.Absolute, 6), Dcp);
            Insert(new Opcode(0xDF, OpcodeEnum.DCP, AddressingMode.AbsoluteX, 7), Dcp);
            Insert(new Opcode(0xDB, OpcodeEnum.DCP, AddressingMode.AbsoluteY, 7), Dcp);
            Insert(new Opcode(0xC7, OpcodeEnum.DCP, AddressingMode.ZeroPage, 5), Dcp);
            Insert(new Opcode(0xD7, OpcodeEnum.DCP, AddressingMode.ZeroPageX, 6), Dcp);
            Insert(new Opcode(0xC3, OpcodeEnum.DCP, AddressingMode.IndexedIndirect, 8), Dcp);
            Insert(new Opcode(0xD3, OpcodeEnum.DCP, AddressingMode.IndirectIndexed, 8), Dcp);

            Insert(new Opcode(0xEF, OpcodeEnum.ISB, AddressingMode.Absolute, 6), Isb);
            Insert(new Opcode(0xFF, OpcodeEnum.ISB, AddressingMode.AbsoluteX, 7), Isb);
            Insert(new Opcode(0xFB, OpcodeEnum.ISB, AddressingMode.AbsoluteY, 7), Isb);
            Insert(new Opcode(0xE7, OpcodeEnum.ISB, AddressingMode.ZeroPage, 5), Isb);
            Insert(new Opcode(0xF7, OpcodeEnum.ISB, AddressingMode.ZeroPageX, 6), Isb);
            Insert(new Opcode(0xE3, OpcodeEnum.ISB, AddressingMode.IndexedIndirect, 8), Isb);
            Insert(new Opcode(0xF3, OpcodeEnum.ISB, AddressingMode.IndirectIndexed, 8), Isb);

            Insert(new Opcode(0x0F, OpcodeEnum.SLO, AddressingMode.Absolute, 6), Slo);
            Insert(new Opcode(0x1F, OpcodeEnum.SLO, AddressingMode.AbsoluteX, 7), Slo);
            Insert(new Opcode(0x1B, OpcodeEnum.SLO, AddressingMode.AbsoluteY, 7), Slo);
            Insert(new Opcode(0x07, OpcodeEnum.SLO, AddressingMode.ZeroPage, 5), Slo);
            Insert(new Opcode(0x17, OpcodeEnum.SLO, AddressingMode.ZeroPageX, 6), Slo);
            Insert(new Opcode(0x03, OpcodeEnum.SLO, AddressingMode.IndexedIndirect, 8), Slo);
            Insert(new Opcode(0x13, OpcodeEnum.SLO, AddressingMode.IndirectIndexed, 8), Slo);


            Insert(new Opcode(0x2F, OpcodeEnum.RLA, AddressingMode.Absolute, 6), Rla);
            Insert(new Opcode(0x3F, OpcodeEnum.RLA, AddressingMode.AbsoluteX, 7), Rla);
            Insert(new Opcode(0x3B, OpcodeEnum.RLA, AddressingMode.AbsoluteY, 7), Rla);
            Insert(new Opcode(0x27, OpcodeEnum.RLA, AddressingMode.ZeroPage, 5), Rla);
            Insert(new Opcode(0x37, OpcodeEnum.RLA, AddressingMode.ZeroPageX, 6), Rla);
            Insert(new Opcode(0x23, OpcodeEnum.RLA, AddressingMode.IndexedIndirect, 8), Rla);
            Insert(new Opcode(0x33, OpcodeEnum.RLA, AddressingMode.IndirectIndexed, 8), Rla);

            Insert(new Opcode(0x4F, OpcodeEnum.SRE, AddressingMode.Absolute, 6), Sre);
            Insert(new Opcode(0x5F, OpcodeEnum.SRE, AddressingMode.AbsoluteX, 7), Sre);
            Insert(new Opcode(0x5B, OpcodeEnum.SRE, AddressingMode.AbsoluteY, 7), Sre);
            Insert(new Opcode(0x47, OpcodeEnum.SRE, AddressingMode.ZeroPage, 5), Sre);
            Insert(new Opcode(0x57, OpcodeEnum.SRE, AddressingMode.ZeroPageX, 6), Sre);
            Insert(new Opcode(0x43, OpcodeEnum.SRE, AddressingMode.IndexedIndirect, 8), Sre);
            Insert(new Opcode(0x53, OpcodeEnum.SRE, AddressingMode.IndirectIndexed, 8), Sre);


            Insert(new Opcode(0x6f, OpcodeEnum.RRA, AddressingMode.Absolute, 6), Rra);
            Insert(new Opcode(0x7F, OpcodeEnum.RRA, AddressingMode.AbsoluteX, 7), Rra);
            Insert(new Opcode(0x7B, OpcodeEnum.RRA, AddressingMode.AbsoluteY, 7), Rra);
            Insert(new Opcode(0x67, OpcodeEnum.RRA, AddressingMode.ZeroPage, 5), Rra);
            Insert(new Opcode(0x77, OpcodeEnum.RRA, AddressingMode.ZeroPageX, 6), Rra);
            Insert(new Opcode(0x63, OpcodeEnum.RRA, AddressingMode.IndexedIndirect, 8), Rra);
            Insert(new Opcode(0x73, OpcodeEnum.RRA, AddressingMode.IndirectIndexed, 8), Rra);


            Insert(new Opcode(0xAF, OpcodeEnum.LAX, AddressingMode.Absolute, 4), Lax);
            Insert(new Opcode(0xBF, OpcodeEnum.LAX, AddressingMode.AbsoluteY, 4), Lax);
            Insert(new Opcode(0xA7, OpcodeEnum.LAX, AddressingMode.ZeroPage, 3), Lax);
            Insert(new Opcode(0xB7, OpcodeEnum.LAX, AddressingMode.ZeroPageY, 4), Lax);
            Insert(new Opcode(0xA3, OpcodeEnum.LAX, AddressingMode.IndexedIndirect, 6), Lax);
            Insert(new Opcode(0xB3, OpcodeEnum.LAX, AddressingMode.IndirectIndexed, 5), Lax);

            Insert(new Opcode(0x83, OpcodeEnum.SAX, AddressingMode.IndexedIndirect, 6), Sax);
            Insert(new Opcode(0x87, OpcodeEnum.SAX, AddressingMode.ZeroPage, 3), Sax);
            Insert(new Opcode(0x8F, OpcodeEnum.SAX, AddressingMode.Absolute, 4), Sax);
            Insert(new Opcode(0x97, OpcodeEnum.SAX, AddressingMode.ZeroPageY, 4), Sax);
            #endregion

            #region Branches       
            Insert(new Opcode(0x90, OpcodeEnum.BCC, AddressingMode.Relative, 2), Bcc);
            Insert(new Opcode(0xB0, OpcodeEnum.BCS, AddressingMode.Relative, 2), Bcs);

            Insert(new Opcode(0xF0, OpcodeEnum.BEQ, AddressingMode.Relative, 2), Beq);
            Insert(new Opcode(0xD0, OpcodeEnum.BNE, AddressingMode.Relative, 2), Bne);

            Insert(new Opcode(0x30, OpcodeEnum.BMI, AddressingMode.Relative, 2), Bmi);
            Insert(new Opcode(0x10, OpcodeEnum.BPL, AddressingMode.Relative, 2), Bpl);

            Insert(new Opcode(0x70, OpcodeEnum.BVS, AddressingMode.Relative, 2), Bvs);
            Insert(new Opcode(0x50, OpcodeEnum.BVC, AddressingMode.Relative, 2), Bvc);
            #endregion
        }

        #region Common Functions

        private ushort ReadWord(ushort address)
        {
            return (ushort)((_cpu.Bus.Read((ushort)(address + 1)) << 8) + _cpu.Bus.Read(address));
        }

        private ushort ReadIndirectWord(byte address)
        {
            return (ushort)((_cpu.Bus.Read((byte)(address + 1)) << 8) + _cpu.Bus.Read(address));
        }

        public ushort GetAddress(ushort param, AddressingMode mode)
        {
            switch (mode)
            {
                case AddressingMode.ZeroPage:
                    return (byte)param;

                case AddressingMode.ZeroPageX:
                    return (byte)(param + _cpu.X);

                case AddressingMode.ZeroPageY:
                    return (byte)(param + _cpu.Y);

                case AddressingMode.Absolute:
                    return param;

                case AddressingMode.Indirect:
                    return (ushort)(_cpu.Bus.Read(param) | (_cpu.Bus.Read((ushort)((param & 0xFF00) | ((param + 1) & 0x00FF))) << 8));

                case AddressingMode.IndexedIndirect:
                    return ReadIndirectWord((byte)(_cpu.X + param));

                case AddressingMode.IndirectIndexed:
                    return (ushort)(ReadIndirectWord((byte)param) + _cpu.Y);

                case AddressingMode.Relative:
                    if (param < 128) return (ushort)(_cpu.PC + param);
                    else return (ushort)(_cpu.PC - (ushort)((((byte)param) ^ 0xFF) + 1));

                case AddressingMode.AbsoluteX:
                    return (ushort)(param + _cpu.X);

                case AddressingMode.AbsoluteY:
                    return (ushort)(param + _cpu.Y);

                default:
                    return 0xCCCC;
            }
        }

        private void JumpRelativeLocation(bool condition, ushort relative)
        {
            if (condition)
            {
                ushort newAddress;
                if (relative < 128) newAddress = (ushort)(_cpu.PC + relative);
                else newAddress = (ushort)(_cpu.PC - ((ushort)((((byte)relative) ^ 0xFF) + 1)));

                _cpu.Cycles += 1;

                if ((newAddress & 0xFF00) != (_cpu.PC & 0xFF00))
                {
                    _cpu.Cycles += 2;
                }

                _cpu.PC = newAddress;
            }
        }

        private void PushPc(ushort val)
        {
            Push((byte)((val & 0xFF00) >> 8));
            Push((byte)(val & 0x00FF));
        }

        public byte GetValue(ushort param, AddressingMode mode)
        {
            if (mode == AddressingMode.Immediate) return (byte)param;
            if (mode == AddressingMode.Implied) return _cpu.A;
            return _cpu.Bus.Read(GetAddress(param, mode));
        }

        public byte GetValue(ushort param, AddressingMode mode, out ushort address)
        {
            address = 0;
            if (mode == AddressingMode.Immediate) return (byte)param;
            if (mode == AddressingMode.Implied) return _cpu.A;
            address = GetAddress(param, mode);
            return _cpu.Bus.Read(address);
        }

        private void SetNegativeFlag(byte val)
        {
            _cpu.Status.Negative = (val & 0x80) == 0x80;
        }

        private void SetNegativeAndZeroFlag(byte val)
        {
            _cpu.Status.Negative = (val & 0x80) == 0x80;
            _cpu.Status.Zero = val == 0;
        }

        #endregion

        private void AdcCore(byte value)
        {
            var result = (byte)(((byte)(_cpu.A + value)) + (byte)(_cpu.Status.Carry ? 1 : 0));
            var hasCarry = (_cpu.A + value + (_cpu.Status.Carry ? 1 : 0)) > 255;

            _cpu.Status.Carry = false;
            _cpu.Status.Overflow = ((_cpu.A ^ result) & (value ^ result) & 0x80) == 0x80;
            _cpu.Status.Carry |= hasCarry;
            _cpu.A = (byte)(result & 0xFF);

            SetNegativeAndZeroFlag(result);
        }

        private void Dcp(ushort param, AddressingMode mode)
        {
            ushort address = GetAddress(param, mode);
            var val = (byte)(_cpu.Bus.Read(address) - 1);
            _cpu.Bus.Write(address, val);
            Cmp(val, AddressingMode.Immediate);
        }

        private void Isb(ushort param, AddressingMode mode)
        {
            ushort address = GetAddress(param, mode);
            var val = (byte)(_cpu.Bus.Read(address) + 1);
            _cpu.Bus.Write(address, (byte)(val));
            Sbc(val, AddressingMode.Immediate);
        }


        private void Lax(ushort param, AddressingMode mode)
        {
            Lda(param, mode);
            Ldx(param, mode);
        }

        private void Sax(ushort param, AddressingMode mode)
        {
            Sta(param, mode);
            Stx(param, mode);
            _cpu.Bus.Write(GetAddress(param, mode), (byte)(_cpu.A & _cpu.X));
        }

        private void Adc(ushort param, AddressingMode mode)
        {
            AdcCore(GetValue(param, mode, out ushort address));
            BoundaryCyles(address, mode);
        }

        private void Sbc(ushort param, AddressingMode mode)
        {
            AdcCore((byte)~GetValue(param, mode, out ushort address));
            BoundaryCyles(address, mode);
        }

        private void Ldx(ushort param, AddressingMode mode)
        {
            _cpu.X = GetValue(param, mode, out ushort address);
            SetNegativeAndZeroFlag(_cpu.X);
            BoundaryCyles(address, mode);
        }

        private void Ora(ushort param, AddressingMode mode)
        {
            _cpu.A |= GetValue(param, mode, out ushort address);
            SetNegativeAndZeroFlag(_cpu.A);
            BoundaryCyles(address, mode);
        }

        private void Eor(ushort param, AddressingMode mode)
        {
            _cpu.A ^= GetValue(param, mode, out ushort address);
            SetNegativeAndZeroFlag(_cpu.A);
            BoundaryCyles(address, mode);
        }

        private void Slo(ushort param, AddressingMode mode)
        {
            Asl(param, mode);
            Ora(param, mode);
        }
        private void Rla(ushort param, AddressingMode mode)
        {
            Rol(param, mode);
            And(param, mode);
        }

        private void Sre(ushort param, AddressingMode mode)
        {
            Lsr(param, mode);
            Eor(param, mode);
        }

        private void Rra(ushort param, AddressingMode mode)
        {
            Ror(param, mode);
            Adc(param, mode);
        }


        private void Asl(ushort param, AddressingMode mode)
        {
            var val = GetValue(param, mode);

            _cpu.Status.Carry = (val & 0x80) == 0x80;
            val = (byte)(val << 1);

            if (mode == AddressingMode.Implied) _cpu.A = val;
            else _cpu.Bus.Write(GetAddress(param, mode), val);

            SetNegativeAndZeroFlag(val);
        }

        private void Lsr(ushort param, AddressingMode mode)
        {
            var val = GetValue(param, mode);

            _cpu.Status.Carry = (val & 1) == 1;
            val = (byte)(val >> 1);

            if (mode == AddressingMode.Implied) _cpu.A = val;
            else _cpu.Bus.Write(GetAddress(param, mode), val);

            SetNegativeAndZeroFlag(val);
        }

        private void Rol(ushort param, AddressingMode mode)
        {
            var val = GetValue(param, mode);

            var carry = (byte)(_cpu.Status.Carry ? 1 : 0);
            _cpu.Status.Carry = (val & 0b10000000) > 0;

            val = (byte)((val << 1) + carry);

            if (mode == AddressingMode.Implied) _cpu.A = val;
            else _cpu.Bus.Write(GetAddress(param, mode), val);

            SetNegativeAndZeroFlag(val);
        }

        private void Ror(ushort param, AddressingMode mode)
        {
            var val = GetValue(param, mode);
            var carry = (byte)(_cpu.Status.Carry ? 128 : 0);

            _cpu.Status.Carry = (val & 0b00000001) > 0;
            val = (byte)((val >> 1) + carry);

            if (mode == AddressingMode.Implied) _cpu.A = val;
            else _cpu.Bus.Write(GetAddress(param, mode), val);

            SetNegativeAndZeroFlag(val);
        }

        private void Bit(ushort param, AddressingMode mode)
        {
            var val = GetValue(param, mode);
            var temp = (byte)(_cpu.A & val);
            _cpu.Status.Zero = temp == 0;
            SetNegativeFlag(val);
            _cpu.Status.Overflow = (val & (1 << 6)) != 0;
        }

        private void Ldy(ushort val, AddressingMode mode)
        {
            _cpu.Y = GetValue(val, mode, out ushort address);
            SetNegativeAndZeroFlag(_cpu.Y);
            BoundaryCyles(address, mode);
        }

        private void Lda(ushort val, AddressingMode mode)
        {
            _cpu.A = GetValue(val, mode, out ushort address);
            SetNegativeAndZeroFlag(_cpu.A);
            BoundaryCyles(address, mode);
        }

        private void Sta(ushort val, AddressingMode mode)
        {
            _cpu.Bus.Write(GetAddress(val, mode), _cpu.A);
        }

        private void Sty(ushort val, AddressingMode mode)
        {
            _cpu.Bus.Write(GetAddress(val, mode), _cpu.Y);
        }

        private void Stx(ushort val, AddressingMode mode)
        {
            _cpu.Bus.Write(GetAddress(val, mode), _cpu.X);
        }

        private void Brk(ushort param, AddressingMode mode)
        {
            PushPc((ushort)(_cpu.PC + 1));
            Php(0, AddressingMode.Immediate);

            _cpu.Status.BreakInterrupt = true;
            _cpu.PC = ReadWord(0xFFFE);
        }

        private void Rti(ushort param, AddressingMode mode)
        {
            Plp(0, AddressingMode.Implied);
            _cpu.PC = PopWord();
        }

        private ushort PopWord()
        {
            byte f = Pop();
            byte s = Pop();
            return (ushort)((s << 8) + f);
        }

        private void Jmp(ushort param, AddressingMode mode)
        {
            _cpu.PC = GetAddress(param, mode);
        }

        private void Inc(ushort param, AddressingMode mode)
        {
            var addr = GetAddress(param, mode);
            var val = (byte)(_cpu.Bus.Read(addr) + 1);
            _cpu.Bus.Write(addr, val);
            SetNegativeAndZeroFlag(val);
        }

        private void Inx(ushort param, AddressingMode mode)
        {
            _cpu.X += 1;
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Iny(ushort param, AddressingMode mode)
        {
            _cpu.Y += 1;

            SetNegativeAndZeroFlag(_cpu.Y);
        }

        private void Dec(ushort param, AddressingMode mode)
        {
            var addr = GetAddress(param, mode);
            var val = (byte)(_cpu.Bus.Read(addr) - 1);
            _cpu.Bus.Write(addr, val);

            SetNegativeAndZeroFlag(val);
        }

        private byte Pop()
        {
            _cpu.SP += 1;
            return _cpu.Bus.Read((ushort)(0x0100 + _cpu.SP));
        }

        private void Cpx(ushort value, AddressingMode mode)
        {
            Cmp(_cpu.X, GetValue(value, mode));
        }

        private void Cpy(ushort value, AddressingMode mode)
        {
            Cmp(_cpu.Y, GetValue(value, mode));
        }

        private void Cmp(ushort value, AddressingMode mode)
        {
            Cmp(_cpu.A, GetValue(value, mode, out ushort address));
            BoundaryCyles(address, mode);
        }

        private void Cmp(byte regValue, byte value)
        {
            _cpu.Status.Carry = false;
            _cpu.Status.Zero = false;
            _cpu.Status.Negative = false;

            if (regValue == value) _cpu.Status.Zero = _cpu.Status.Carry = true;
            else _cpu.Status.Carry = regValue >= value;

            SetNegativeFlag((byte)(regValue - value));
        }


        private void Push(byte value)
        {
            _cpu.Bus.Write((ushort)(0x0100 + _cpu.SP), value);
            _cpu.SP--;
        }

        private void Php(ushort param, AddressingMode mode)
        {
            Push((byte)(_cpu.Status.Value | 16 | 32));
            _cpu.Status.BreakInterrupt = false;
            _cpu.Status.NonUsed = false;
        }

        private void Plp(ushort param, AddressingMode mode)
        {
            var val = Pop();

            _cpu.Status.Carry = (val & 0b00000001) > 0;
            _cpu.Status.Zero = (val & 0b00000010) > 0;
            _cpu.Status.InterruptDisable = (val & 0b00000100) > 0;
            _cpu.Status.DecimalMode = (val & 0b00001000) > 0;
            _cpu.Status.BreakInterrupt = false;

            _cpu.Status.Overflow = (val & 0b01000000) > 0;
            _cpu.Status.Negative = (val & 0b10000000) > 0;
        }

        private void Pla(ushort param, AddressingMode mode)
        {
            _cpu.A = Pop();
            SetNegativeAndZeroFlag(_cpu.A);
        }

        private void And(ushort param, AddressingMode mode)
        {
            _cpu.A &= GetValue(param, mode, out ushort address);
            SetNegativeAndZeroFlag(_cpu.A);
            BoundaryCyles(address, mode);
        }

        private void Dex(ushort param, AddressingMode mode)
        {
            _cpu.X -= 1;
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Dey(ushort param, AddressingMode mode)
        {
            _cpu.Y -= 1;
            SetNegativeAndZeroFlag(_cpu.Y);
        }

        private void Bcc(ushort param, AddressingMode mode)
        {
            JumpRelativeLocation(!_cpu.Status.Carry, param);
        }

        private void Bcs(ushort param, AddressingMode mode)
        {
            JumpRelativeLocation(_cpu.Status.Carry, param);
        }


        private void Beq(ushort param, AddressingMode mode)
        {
            JumpRelativeLocation(_cpu.Status.Zero, param);
        }

        private void Bne(ushort param, AddressingMode mode)
        {
            JumpRelativeLocation(!_cpu.Status.Zero, param);
        }


        private void Bmi(ushort param, AddressingMode mode)
        {
            JumpRelativeLocation(_cpu.Status.Negative, param);
        }

        private void Bpl(ushort param, AddressingMode mode)
        {
            JumpRelativeLocation(!_cpu.Status.Negative, param);
        }

        private void Bvs(ushort param, AddressingMode mode)
        {
            JumpRelativeLocation(_cpu.Status.Overflow, param);
        }

        private void Bvc(ushort param, AddressingMode mode)
        {
            JumpRelativeLocation(!_cpu.Status.Overflow, param);
        }

        private void Pha(ushort param, AddressingMode mode)
        {
            Push(_cpu.A);
        }

        private void Nop(ushort param, AddressingMode mode)
        {

        }

        private void Rts(ushort param, AddressingMode mode)
        {
            _cpu.PC = (ushort)(PopWord() + 1);
        }

        private void Jsr(ushort param, AddressingMode mode)
        {
            PushPc((ushort)(_cpu.PC - 1));
            _cpu.PC = param;
        }


        private void Sec(ushort param, AddressingMode mode)
        {
            _cpu.Status.Carry = true;
        }

        private void Sei(ushort param, AddressingMode mode)
        {
            _cpu.Status.InterruptDisable = true;
        }

        private void Sed(ushort param, AddressingMode mode)
        {
            _cpu.Status.DecimalMode = true;
        }

        private void Clc(ushort param, AddressingMode mode)
        {
            _cpu.Status.Carry = false;
        }

        private void Cld(ushort param, AddressingMode mode)
        {
            _cpu.Status.DecimalMode = false;
        }

        private void Clv(ushort param, AddressingMode mode)
        {
            _cpu.Status.Overflow = false;
        }

        private void Cli(ushort param, AddressingMode mode)
        {
            _cpu.Status.InterruptDisable = false;
        }

        private void Tax(ushort param, AddressingMode mode)
        {
            _cpu.X = _cpu.A;
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Tay(ushort param, AddressingMode mode)
        {
            _cpu.Y = _cpu.A;
            SetNegativeAndZeroFlag(_cpu.Y);
        }

        private void Tsx(ushort param, AddressingMode mode)
        {
            _cpu.X = _cpu.SP;
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Txa(ushort param, AddressingMode mode)
        {
            _cpu.A = _cpu.X;
            SetNegativeAndZeroFlag(_cpu.A);
        }

        private void Txs(ushort param, AddressingMode mode)
        {
            _cpu.SP = _cpu.X;
        }

        private void Tya(ushort param, AddressingMode mode)
        {
            _cpu.A = _cpu.Y;
            SetNegativeAndZeroFlag(_cpu.A);
        }


        private void BoundaryCyles(ushort address, AddressingMode mode)
        {
            if (mode == AddressingMode.AbsoluteX || mode == AddressingMode.AbsoluteY || mode == AddressingMode.IndirectIndexed)
            {
                if ((0xFF00 & _cpu.PC) != (address & 0xFF00))
                {
                    _cpu.Cycles++;
                }
            }
        }

        internal void Irq()
        {
            if (!_cpu.Status.InterruptDisable)
            {
                PushPc(_cpu.PC);
                _cpu.Status.BreakInterrupt = false;
                _cpu.Status.InterruptDisable = true;
                Push(_cpu.Status.Value);
                _cpu.PC = ReadWord(0xFFFE);
                _cpu.Cycles += 7;
            }
        }

        internal void Nmi()
        {
            PushPc(_cpu.PC);
            _cpu.Status.BreakInterrupt = false;
            _cpu.Status.InterruptDisable = true;
            Push(_cpu.Status.Value);
            _cpu.PC = ReadWord(0xFFFA);
            _cpu.Cycles += 7;
        }

    }
}