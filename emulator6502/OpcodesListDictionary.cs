using System.Collections.Generic;

namespace emulator6502
{
    internal class OpcodesListDictionary : SortedDictionary<byte, Opcode>
    {
        private void Insert(Opcode opcode)
        {
            this[opcode.Code] = opcode;
        }

        public OpcodesListDictionary()
        {
            #region LOAD

            Insert(new Opcode(0xA9, OpcodeEnum.LDA, BindingMode.Immediate, 2));
            Insert(new Opcode(0xA5, OpcodeEnum.LDA, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0xB5, OpcodeEnum.LDA, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0xAD, OpcodeEnum.LDA, BindingMode.Absolute, 4));
            Insert(new Opcode(0xBD, OpcodeEnum.LDA, BindingMode.AbsoluteX, 4));
            Insert(new Opcode(0xB9, OpcodeEnum.LDA, BindingMode.AbsoluteY, 4));
            Insert(new Opcode(0xA1, OpcodeEnum.LDA, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0xB1, OpcodeEnum.LDA, BindingMode.IndirectIndexed, 5));

            Insert(new Opcode(0xA2, OpcodeEnum.LDX, BindingMode.Immediate, 2));
            Insert(new Opcode(0xA6, OpcodeEnum.LDX, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0xB6, OpcodeEnum.LDX, BindingMode.ZeroPageY, 4));
            Insert(new Opcode(0xAE, OpcodeEnum.LDX, BindingMode.Absolute, 4));
            Insert(new Opcode(0xBE, OpcodeEnum.LDX, BindingMode.AbsoluteY, 4));

            Insert(new Opcode(0xA0, OpcodeEnum.LDY, BindingMode.Immediate, 2));
            Insert(new Opcode(0xA4, OpcodeEnum.LDY, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0xB4, OpcodeEnum.LDY, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0xAC, OpcodeEnum.LDY, BindingMode.Absolute, 4));
            Insert(new Opcode(0xBC, OpcodeEnum.LDY, BindingMode.AbsoluteX, 4));

            #endregion

            #region STORE

            Insert(new Opcode(0x85, OpcodeEnum.STA, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x95, OpcodeEnum.STA, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0x8D, OpcodeEnum.STA, BindingMode.Absolute, 4));
            Insert(new Opcode(0x9D, OpcodeEnum.STA, BindingMode.AbsoluteX, 5));
            Insert(new Opcode(0x99, OpcodeEnum.STA, BindingMode.AbsoluteY, 5));
            Insert(new Opcode(0x81, OpcodeEnum.STA, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0x91, OpcodeEnum.STA, BindingMode.IndirectIndexed, 6));

            Insert(new Opcode(0x84, OpcodeEnum.STY, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x94, OpcodeEnum.STY, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0x8C, OpcodeEnum.STY, BindingMode.Absolute, 4));

            Insert(new Opcode(0x86, OpcodeEnum.STX, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x96, OpcodeEnum.STX, BindingMode.ZeroPageY, 4));
            Insert(new Opcode(0x8E, OpcodeEnum.STX, BindingMode.Absolute, 4));

            #endregion

            #region Flags

            Insert(new Opcode(0x38, OpcodeEnum.SEC, BindingMode.Implied, 2));
            Insert(new Opcode(0x78, OpcodeEnum.SEI, BindingMode.Implied, 2));
            Insert(new Opcode(0xF8, OpcodeEnum.SED, BindingMode.Implied, 2));

            Insert(new Opcode(0x18, OpcodeEnum.CLC, BindingMode.Implied, 2));
            Insert(new Opcode(0xD8, OpcodeEnum.CLD, BindingMode.Implied, 2));
            Insert(new Opcode(0x58, OpcodeEnum.CLI, BindingMode.Implied, 2));
            Insert(new Opcode(0xB8, OpcodeEnum.CLV, BindingMode.Implied, 2));

            #endregion

            #region Transfer

            Insert(new Opcode(0xAA, OpcodeEnum.TAX, BindingMode.Implied, 2));
            Insert(new Opcode(0xA8, OpcodeEnum.TAY, BindingMode.Implied, 2));
            Insert(new Opcode(0xBA, OpcodeEnum.TSX, BindingMode.Implied, 2));
            Insert(new Opcode(0x8A, OpcodeEnum.TXA, BindingMode.Implied, 2));
            Insert(new Opcode(0x9A, OpcodeEnum.TXS, BindingMode.Implied, 2));
            Insert(new Opcode(0x98, OpcodeEnum.TYA, BindingMode.Implied, 2));

            #endregion

            #region CMP             

            Insert(new Opcode(0xC9, OpcodeEnum.CMP, BindingMode.Immediate, 2));
            Insert(new Opcode(0xC5, OpcodeEnum.CMP, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0xD5, OpcodeEnum.CMP, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0xCD, OpcodeEnum.CMP, BindingMode.Absolute, 4));
            Insert(new Opcode(0xDD, OpcodeEnum.CMP, BindingMode.AbsoluteX, 4));
            Insert(new Opcode(0xD9, OpcodeEnum.CMP, BindingMode.AbsoluteY, 4));
            Insert(new Opcode(0xC1, OpcodeEnum.CMP, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0xD1, OpcodeEnum.CMP, BindingMode.IndirectIndexed, 5));

            Insert(new Opcode(0xE0, OpcodeEnum.CPX, BindingMode.Immediate, 2));
            Insert(new Opcode(0xE4, OpcodeEnum.CPX, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0xEC, OpcodeEnum.CPX, BindingMode.Absolute, 4));

            Insert(new Opcode(0xC0, OpcodeEnum.CPY, BindingMode.Immediate, 2));
            Insert(new Opcode(0xC4, OpcodeEnum.CPY, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0xCC, OpcodeEnum.CPY, BindingMode.Absolute, 4));

            #endregion
            
            #region DEC             
            Insert(new Opcode(0xC6, OpcodeEnum.DEC, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0xD6, OpcodeEnum.DEC, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0xCE, OpcodeEnum.DEC, BindingMode.Absolute, 3));
            Insert(new Opcode(0xDE, OpcodeEnum.DEC, BindingMode.AbsoluteX, 7));

            Insert(new Opcode(0xCA, OpcodeEnum.DEX, BindingMode.Implied, 2));
            Insert(new Opcode(0x88, OpcodeEnum.DEY, BindingMode.Implied, 2));
            #endregion

            #region INC
            Insert(new Opcode(0xE6, OpcodeEnum.INC, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0xF6, OpcodeEnum.INC, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0xEE, OpcodeEnum.INC, BindingMode.Absolute, 6));
            Insert(new Opcode(0xFE, OpcodeEnum.INC, BindingMode.AbsoluteX, 7));

            Insert(new Opcode(0xE8, OpcodeEnum.INX, BindingMode.Implied, 2));
            Insert(new Opcode(0xC8, OpcodeEnum.INY, BindingMode.Implied, 2));
            #endregion

            #region Routines       
            Insert(new Opcode(0x00, OpcodeEnum.BRK, BindingMode.Implied, 7));
            Insert(new Opcode(0x40, OpcodeEnum.RTI, BindingMode.Implied, 6));
            
            Insert(new Opcode(0xEA, OpcodeEnum.NOP, BindingMode.Implied, 2));
            Insert(new Opcode(0x04, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0x0C, OpcodeEnum.NOP, BindingMode.Absolute, 4));
            Insert(new Opcode(0x14, OpcodeEnum.NOP, BindingMode.Immediate, 4));
            Insert(new Opcode(0x1A, OpcodeEnum.NOP, BindingMode.Implied, 2));
            Insert(new Opcode(0x1C, OpcodeEnum.NOP, BindingMode.Absolute, 4));
            Insert(new Opcode(0x34, OpcodeEnum.NOP, BindingMode.Immediate, 4));

            Insert(new Opcode(0x3A, OpcodeEnum.NOP, BindingMode.Implied, 2));
            Insert(new Opcode(0x3C, OpcodeEnum.NOP, BindingMode.Absolute, 4));
            Insert(new Opcode(0x44, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0x54, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0x5A, OpcodeEnum.NOP, BindingMode.Implied, 2));
            Insert(new Opcode(0x5C, OpcodeEnum.NOP, BindingMode.Absolute, 4));
            Insert(new Opcode(0x64, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0x74, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0x7A, OpcodeEnum.NOP, BindingMode.Implied, 2));
            Insert(new Opcode(0x7C, OpcodeEnum.NOP, BindingMode.Absolute, 4));
            Insert(new Opcode(0x80, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0x82, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0x89, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0xC2, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0xD4, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0xDA, OpcodeEnum.NOP, BindingMode.Implied, 2));
            Insert(new Opcode(0xDC, OpcodeEnum.NOP, BindingMode.Absolute, 4));
            Insert(new Opcode(0xE2, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0xF4, OpcodeEnum.NOP, BindingMode.Immediate, 3));
            Insert(new Opcode(0xFA, OpcodeEnum.NOP, BindingMode.Implied, 2));
            Insert(new Opcode(0xFC, OpcodeEnum.NOP, BindingMode.Absolute, 4));
            
            Insert(new Opcode(0x60, OpcodeEnum.RTS, BindingMode.Implied, 6));
            Insert(new Opcode(0x20, OpcodeEnum.JSR, BindingMode.Absolute, 6));

            Insert(new Opcode(0x4C, OpcodeEnum.JMP, BindingMode.Absolute, 3));
            Insert(new Opcode(0x6C, OpcodeEnum.JMP, BindingMode.Indirect, 3));

            #endregion

            #region Stack           
            Insert(new Opcode(0x08, OpcodeEnum.PHP, BindingMode.Implied, 3));
            Insert(new Opcode(0x48, OpcodeEnum.PHA, BindingMode.Implied, 3));
            Insert(new Opcode(0x68, OpcodeEnum.PLA, BindingMode.Implied, 4));
            Insert(new Opcode(0x28, OpcodeEnum.PLP, BindingMode.Implied, 4));
            #endregion

            #region BIT OPERATIONS 
            Insert(new Opcode(0x24, OpcodeEnum.BIT, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x2C, OpcodeEnum.BIT, BindingMode.Absolute, 3));

            Insert(new Opcode(0x29, OpcodeEnum.AND, BindingMode.Immediate, 2));
            Insert(new Opcode(0x25, OpcodeEnum.AND, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x35, OpcodeEnum.AND, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0x2D, OpcodeEnum.AND, BindingMode.Absolute, 4));
            Insert(new Opcode(0x3D, OpcodeEnum.AND, BindingMode.AbsoluteX, 4));
            Insert(new Opcode(0x39, OpcodeEnum.AND, BindingMode.AbsoluteY, 4));
            Insert(new Opcode(0x21, OpcodeEnum.AND, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0x31, OpcodeEnum.AND, BindingMode.IndirectIndexed, 5));

            Insert(new Opcode(0x0A, OpcodeEnum.ASL, BindingMode.Implied, 2));
            Insert(new Opcode(0x06, OpcodeEnum.ASL, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0x16, OpcodeEnum.ASL, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0x0E, OpcodeEnum.ASL, BindingMode.Absolute, 6));
            Insert(new Opcode(0x1E, OpcodeEnum.ASL, BindingMode.AbsoluteX, 7));
            
            


            Insert(new Opcode(0x4A, OpcodeEnum.LSR, BindingMode.Implied, 2));
            Insert(new Opcode(0x46, OpcodeEnum.LSR, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0x56, OpcodeEnum.LSR, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0x4E, OpcodeEnum.LSR, BindingMode.Absolute, 6));
            Insert(new Opcode(0x5E, OpcodeEnum.LSR, BindingMode.AbsoluteX, 7));

            Insert(new Opcode(0x09, OpcodeEnum.ORA, BindingMode.Immediate, 2));
            Insert(new Opcode(0x05, OpcodeEnum.ORA, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x15, OpcodeEnum.ORA, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0x0D, OpcodeEnum.ORA, BindingMode.Absolute, 4));
            Insert(new Opcode(0x1D, OpcodeEnum.ORA, BindingMode.AbsoluteX, 4));
            Insert(new Opcode(0x19, OpcodeEnum.ORA, BindingMode.AbsoluteY, 4));
            Insert(new Opcode(0x01, OpcodeEnum.ORA, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0x11, OpcodeEnum.ORA, BindingMode.IndirectIndexed, 5));

            Insert(new Opcode(0x49, OpcodeEnum.EOR, BindingMode.Immediate, 2));
            Insert(new Opcode(0x45, OpcodeEnum.EOR, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x55, OpcodeEnum.EOR, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0x4D, OpcodeEnum.EOR, BindingMode.Absolute, 4));
            Insert(new Opcode(0x5D, OpcodeEnum.EOR, BindingMode.AbsoluteX, 4));
            Insert(new Opcode(0x59, OpcodeEnum.EOR, BindingMode.AbsoluteY, 4));
            Insert(new Opcode(0x41, OpcodeEnum.EOR, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0x51, OpcodeEnum.EOR, BindingMode.IndirectIndexed, 5));

            Insert(new Opcode(0x2A, OpcodeEnum.ROL, BindingMode.Implied, 2));
            Insert(new Opcode(0x26, OpcodeEnum.ROL, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0x36, OpcodeEnum.ROL, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0x2E, OpcodeEnum.ROL, BindingMode.Absolute, 6));
            Insert(new Opcode(0x3E, OpcodeEnum.ROL, BindingMode.AbsoluteX, 7));

            Insert(new Opcode(0x6A, OpcodeEnum.ROR, BindingMode.Implied, 2));
            Insert(new Opcode(0x66, OpcodeEnum.ROR, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0x76, OpcodeEnum.ROR, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0x6E, OpcodeEnum.ROR, BindingMode.Absolute, 6));
            Insert(new Opcode(0x7E, OpcodeEnum.ROR, BindingMode.AbsoluteX, 7));

            Insert(new Opcode(0x69, OpcodeEnum.ADC, BindingMode.Immediate, 2));
            Insert(new Opcode(0x65, OpcodeEnum.ADC, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x75, OpcodeEnum.ADC, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0x6D, OpcodeEnum.ADC, BindingMode.Absolute, 4));
            Insert(new Opcode(0x7D, OpcodeEnum.ADC, BindingMode.AbsoluteX, 4));
            Insert(new Opcode(0x79, OpcodeEnum.ADC, BindingMode.AbsoluteY, 4));
            Insert(new Opcode(0x61, OpcodeEnum.ADC, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0x71, OpcodeEnum.ADC, BindingMode.IndirectIndexed, 5));

            Insert(new Opcode(0xE9, OpcodeEnum.SBC, BindingMode.Immediate, 2));
            Insert(new Opcode(0xE5, OpcodeEnum.SBC, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0xF5, OpcodeEnum.SBC, BindingMode.ZeroPageX, 4));
            Insert(new Opcode(0xED, OpcodeEnum.SBC, BindingMode.Absolute, 4));
            Insert(new Opcode(0xFD, OpcodeEnum.SBC, BindingMode.AbsoluteX, 4));
            Insert(new Opcode(0xF9, OpcodeEnum.SBC, BindingMode.AbsoluteY, 4));
            Insert(new Opcode(0xE1, OpcodeEnum.SBC, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0xF1, OpcodeEnum.SBC, BindingMode.IndirectIndexed, 5));
            
            Insert(new Opcode(0xEB, OpcodeEnum.SBC, BindingMode.Immediate, 2));
            
            
            Insert(new Opcode(0xCF, OpcodeEnum.DCP, BindingMode.Absolute, 6));
            Insert(new Opcode(0xDF, OpcodeEnum.DCP, BindingMode.AbsoluteX, 7));
            Insert(new Opcode(0xDB, OpcodeEnum.DCP, BindingMode.AbsoluteY, 7));
            Insert(new Opcode(0xC7, OpcodeEnum.DCP, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0xD7, OpcodeEnum.DCP, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0xC3, OpcodeEnum.DCP, BindingMode.IndexedIndirect, 8));
            Insert(new Opcode(0xD3, OpcodeEnum.DCP, BindingMode.IndirectIndexed, 8));
            
            Insert(new Opcode(0xEF, OpcodeEnum.ISB, BindingMode.Absolute, 6));
            Insert(new Opcode(0xFF, OpcodeEnum.ISB, BindingMode.AbsoluteX, 7));
            Insert(new Opcode(0xFB, OpcodeEnum.ISB, BindingMode.AbsoluteY, 7));
            Insert(new Opcode(0xE7, OpcodeEnum.ISB, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0xF7, OpcodeEnum.ISB, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0xE3, OpcodeEnum.ISB, BindingMode.IndexedIndirect, 8));
            Insert(new Opcode(0xF3, OpcodeEnum.ISB, BindingMode.IndirectIndexed, 8));
            
            Insert(new Opcode(0x0F, OpcodeEnum.SLO, BindingMode.Absolute, 6));
            Insert(new Opcode(0x1F, OpcodeEnum.SLO, BindingMode.AbsoluteX, 7));
            Insert(new Opcode(0x1B, OpcodeEnum.SLO, BindingMode.AbsoluteY, 7));
            Insert(new Opcode(0x07, OpcodeEnum.SLO, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0x17, OpcodeEnum.SLO, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0x03, OpcodeEnum.SLO, BindingMode.IndexedIndirect, 8));
            Insert(new Opcode(0x13, OpcodeEnum.SLO, BindingMode.IndirectIndexed, 8));
            
                 
            Insert(new Opcode(0x2F, OpcodeEnum.RLA, BindingMode.Absolute, 6));
            Insert(new Opcode(0x3F, OpcodeEnum.RLA, BindingMode.AbsoluteX, 7));
            Insert(new Opcode(0x3B, OpcodeEnum.RLA, BindingMode.AbsoluteY, 7));
            Insert(new Opcode(0x27, OpcodeEnum.RLA, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0x37, OpcodeEnum.RLA, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0x23, OpcodeEnum.RLA, BindingMode.IndexedIndirect, 8));
            Insert(new Opcode(0x33, OpcodeEnum.RLA, BindingMode.IndirectIndexed, 8));
            
            Insert(new Opcode(0x4F, OpcodeEnum.SRE, BindingMode.Absolute, 6));
            Insert(new Opcode(0x5F, OpcodeEnum.SRE, BindingMode.AbsoluteX, 7));
            Insert(new Opcode(0x5B, OpcodeEnum.SRE, BindingMode.AbsoluteY, 7));
            Insert(new Opcode(0x47, OpcodeEnum.SRE, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0x57, OpcodeEnum.SRE, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0x43, OpcodeEnum.SRE, BindingMode.IndexedIndirect, 8));
            Insert(new Opcode(0x53, OpcodeEnum.SRE, BindingMode.IndirectIndexed, 8));
            
                
            Insert(new Opcode(0x6f, OpcodeEnum.RRA, BindingMode.Absolute, 6));
            Insert(new Opcode(0x7F, OpcodeEnum.RRA, BindingMode.AbsoluteX, 7));
            Insert(new Opcode(0x7B, OpcodeEnum.RRA, BindingMode.AbsoluteY, 7));
            Insert(new Opcode(0x67, OpcodeEnum.RRA, BindingMode.ZeroPage, 5));
            Insert(new Opcode(0x77, OpcodeEnum.RRA, BindingMode.ZeroPageX, 6));
            Insert(new Opcode(0x63, OpcodeEnum.RRA, BindingMode.IndexedIndirect, 8));
            Insert(new Opcode(0x73, OpcodeEnum.RRA, BindingMode.IndirectIndexed, 8));
            
            

            Insert(new Opcode(0xAF, OpcodeEnum.LAX, BindingMode.Absolute, 4));
            Insert(new Opcode(0xBF, OpcodeEnum.LAX, BindingMode.AbsoluteY, 4));
            Insert(new Opcode(0xA7, OpcodeEnum.LAX, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0xB7, OpcodeEnum.LAX, BindingMode.ZeroPageY, 4));
            Insert(new Opcode(0xA3, OpcodeEnum.LAX, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0xB3, OpcodeEnum.LAX, BindingMode.IndirectIndexed, 5));
    
            Insert(new Opcode(0x83, OpcodeEnum.SAX, BindingMode.IndexedIndirect, 6));
            Insert(new Opcode(0x87, OpcodeEnum.SAX, BindingMode.ZeroPage, 3));
            Insert(new Opcode(0x8F, OpcodeEnum.SAX, BindingMode.Absolute, 4));
            Insert(new Opcode(0x97, OpcodeEnum.SAX, BindingMode.ZeroPageY, 4));

            #endregion

            #region Branches       
            Insert(new Opcode(0x90, OpcodeEnum.BCC, BindingMode.Relative, 2));
            Insert(new Opcode(0xB0, OpcodeEnum.BCS, BindingMode.Relative, 2));

            Insert(new Opcode(0xF0, OpcodeEnum.BEQ, BindingMode.Relative, 2));
            Insert(new Opcode(0xD0, OpcodeEnum.BNE, BindingMode.Relative, 2));

            Insert(new Opcode(0x30, OpcodeEnum.BMI, BindingMode.Relative, 2));
            Insert(new Opcode(0x10, OpcodeEnum.BPL, BindingMode.Relative, 2));

            Insert(new Opcode(0x70, OpcodeEnum.BVS, BindingMode.Relative, 2));
            Insert(new Opcode(0x50, OpcodeEnum.BVC, BindingMode.Relative, 2));
            #endregion
        }
    }
}