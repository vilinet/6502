using System;
using System.Collections.Generic;

namespace emulator6502
{
    internal class CpuOperations : Dictionary<OpcodeEnum, Action<ushort, BindingMode>>
    {
        private readonly Cpu _cpu;

        public CpuOperations(Cpu cpu)
        {
            _cpu = cpu;
            this[OpcodeEnum.ADC] = Adc;
            this[OpcodeEnum.AND] = And;
            this[OpcodeEnum.ASL] = Asl;
            this[OpcodeEnum.LSR] = Lsr;
            this[OpcodeEnum.BCC] = Bcc;
            this[OpcodeEnum.BCS] = Bcs;
            this[OpcodeEnum.BEQ] = Beq;
            this[OpcodeEnum.BIT] = Bit;
            this[OpcodeEnum.BMI] = Bmi;
            this[OpcodeEnum.BNE] = Bne;
            this[OpcodeEnum.BPL] = Bpl;
            this[OpcodeEnum.BRK] = Brk;
            this[OpcodeEnum.BVC] = Bvc;
            this[OpcodeEnum.BVS] = Bvs;
            this[OpcodeEnum.CLC] = Clc;
            this[OpcodeEnum.CLD] = Cld;
            this[OpcodeEnum.CLI] = Cli;
            this[OpcodeEnum.CLV] = Clv;
            this[OpcodeEnum.CMP] = Cmp;
            this[OpcodeEnum.CPX] = Cpx;
            this[OpcodeEnum.CPY] = Cpy;
            this[OpcodeEnum.DEC] = Dec;
            this[OpcodeEnum.DEX] = Dex;
            this[OpcodeEnum.DEY] = Dey;
            this[OpcodeEnum.EOR] = Eor;
            this[OpcodeEnum.INC] = Inc;
            this[OpcodeEnum.INX] = Inx;
            this[OpcodeEnum.INY] = Iny;
            this[OpcodeEnum.JMP] = Jmp;
            this[OpcodeEnum.JSR] = Jsr;
            this[OpcodeEnum.LDA] = Lda;
            this[OpcodeEnum.LDX] = Ldx;
            this[OpcodeEnum.LDY] = Ldy;
            this[OpcodeEnum.NOP] = Nop;
            this[OpcodeEnum.ORA] = Ora;
            this[OpcodeEnum.PHA] = Pha;
            this[OpcodeEnum.PHP] = Php;
            this[OpcodeEnum.PLA] = Pla;
            this[OpcodeEnum.PLP] = Plp;
            this[OpcodeEnum.ROL] = Rol;
            this[OpcodeEnum.ROR] = Ror;
            this[OpcodeEnum.RTI] = Rti;
            this[OpcodeEnum.RTS] = Rts;
            this[OpcodeEnum.SBC] = Sbc;
            this[OpcodeEnum.SEC] = Sec;
            this[OpcodeEnum.SED] = Sed;
            this[OpcodeEnum.SEI] = Sei;
            this[OpcodeEnum.STA] = Sta;
            this[OpcodeEnum.STX] = Stx;
            this[OpcodeEnum.STY] = Sty;
            this[OpcodeEnum.TAX] = Tax;
            this[OpcodeEnum.TAY] = Tay;
            this[OpcodeEnum.TSX] = Tsx;
            this[OpcodeEnum.TXA] = Txa;
            this[OpcodeEnum.TXS] = Txs;
            this[OpcodeEnum.TYA] = Tya;
        }

        #region Common Functions

        private ushort ReadWord(ushort address)
        {
            return (ushort)((_cpu.Bus.Read((ushort)(address + 1)) << 8) + _cpu.Bus.Read(address));
        }

        private ushort GetAddress(ushort param, BindingMode mode)
        {
            switch (mode)
            {
                case BindingMode.ZeroPage:
                    return (byte)param;

                case BindingMode.ZeroPageX:
                    return (byte)(param + _cpu.X);

                case BindingMode.ZeroPageY:
                    return (byte)(param + _cpu.Y);

                case BindingMode.Indirect:
                    return ReadWord(param);

                case BindingMode.IndexedIndirect:
                    return ReadWord(_cpu.Bus.Read((ushort)(_cpu.X + param)));

                case BindingMode.IndirectIndexed:
                    return (ushort)(ReadWord(param) + _cpu.Y);

                case BindingMode.Absolute:
                    return param;

                case BindingMode.AbsoluteX:
                    return (ushort)(param + _cpu.X);

                case BindingMode.AbsoluteY:
                    return (ushort)(param + _cpu.Y);

                default:
                    throw new Exception("Not supported mode: " + mode);
            }
        }

        private void JumpRelativeLocation(bool condition, ushort relative)
        {
            if (condition)
            {
                if (relative < 128) _cpu.PC += relative;
                else _cpu.PC -= (ushort)((((byte)relative) ^ 0xFF) + 1);
            }
        }

        private void PushPc(ushort val)
        {
            Push((byte)(val & 0x00FF));
            Push((byte)((val & 0xFF00) >> 8));
        }

        private byte GetValue(ushort param, BindingMode mode)
        {
            if (mode == BindingMode.Immediate) return (byte)param;
            if (mode == BindingMode.Implied) return _cpu.A;
            return _cpu.Bus.Read(GetAddress(param, mode));
        }

        private void SetNegativeFlag(byte val)
        {
            _cpu.Status.Negative = val > 127;
        }

        private void SetNegativeAndZeroFlag(byte val)
        {
            SetNegativeFlag(val);
            _cpu.Status.Zero = val == 0;
        }

        #endregion

        private void AdcCore(byte value)
        {
            var result = (byte)(((byte)(_cpu.A + value)) + (byte)(_cpu.Status.Carry ? 1 : 0));
            var hasCarry = (_cpu.A + value + (_cpu.Status.Carry ? 1 : 0)) > 255;

            _cpu.Status.Carry = false;

           /* if (_cpu.Status.DecimalMode)
            {
                if (((_cpu.A ^ value ^ result) & 0x10) == 0x10)
                {
                    result += 0x06;
                }
                if ((result & 0xf0) > 0x90)
                {
                    result += 0x60;
                }
                if (result > 99)
                {
                    _cpu.Status.Carry = true;
                }
            }*/

            _cpu.Status.Overflow = ((_cpu.A ^ result) & (value ^ result) & 0x80) == 0x80;
            _cpu.Status.Carry |= hasCarry;
            _cpu.A = (byte)(result & 0xFF);

            SetNegativeAndZeroFlag(result);
        }

        private void Adc(ushort param, BindingMode mode)
        {
            AdcCore(GetValue(param, mode));
        }

        private void Sbc(ushort param, BindingMode mode)
        {
            AdcCore((byte)~GetValue(param, mode));
        }

        private void Ldx(ushort param, BindingMode mode)
        {
            _cpu.X = GetValue(param, mode);
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Ora(ushort param, BindingMode mode)
        {
            _cpu.A |= GetValue(param, mode);
            SetNegativeAndZeroFlag(_cpu.A);
        }

        private void Eor(ushort param, BindingMode mode)
        {
            _cpu.A ^= GetValue(param, mode);
            SetNegativeAndZeroFlag(_cpu.A);
        }

        private void Asl(ushort param, BindingMode mode)
        {
            var val = GetValue(param, mode);

            _cpu.Status.Carry = (val & 0x80) == 0x80;
            val = (byte)(val << 1);

            if (mode == BindingMode.Implied) _cpu.A = val;
            else _cpu.Bus.Write(GetAddress(param, mode), val);

            SetNegativeAndZeroFlag(val);
        }

        private void Lsr(ushort param, BindingMode mode)
        {
            var val = GetValue(param, mode);

            _cpu.Status.Carry = (val & 1) == 1;
            val = (byte)(val >> 1);

            if (mode == BindingMode.Implied) _cpu.A = val;
            else _cpu.Bus.Write(GetAddress(param, mode), val);

            SetNegativeAndZeroFlag(val);
        }

        private void Rol(ushort param, BindingMode mode)
        {
            var val = GetValue(param, mode);

            var carry = (byte)(_cpu.Status.Carry ? 1 : 0);
            _cpu.Status.Carry = (val & 0b10000000) > 0;

            val = (byte)((val << 1) + carry);

            if (mode == BindingMode.Implied) _cpu.A = val;
            else _cpu.Bus.Write(GetAddress(param, mode), val);

            SetNegativeAndZeroFlag(val);
        }

        private void Ror(ushort param, BindingMode mode)
        {
            var val = GetValue(param, mode);
            var carry = (byte)(_cpu.Status.Carry ? 128 : 0);

            _cpu.Status.Carry = (val & 0b00000001) > 0;
            val = (byte)((val >> 1) + carry);

            if (mode == BindingMode.Implied) _cpu.A = val;
            else _cpu.Bus.Write(GetAddress(param, mode), val);

            SetNegativeAndZeroFlag(val);
        }

        private void Bit(ushort param, BindingMode mode)
        {
            var val = GetValue(param, mode);
            SetNegativeAndZeroFlag(val);
            _cpu.Status.Overflow = (val & 0b01000000) > 0;
        }

        private void Ldy(ushort val, BindingMode mode)
        {
            _cpu.Y = GetValue(val, mode);
            SetNegativeAndZeroFlag(_cpu.Y);
        }

        private void Lda(ushort val, BindingMode mode)
        {
            _cpu.A = GetValue(val, mode);
            SetNegativeAndZeroFlag(_cpu.A);
        }

        private void Sta(ushort val, BindingMode mode)
        {
            _cpu.Bus.Write(GetAddress(val, mode), _cpu.A);
        }

        private void Sty(ushort val, BindingMode mode)
        {
            _cpu.Bus.Write(GetAddress(val, mode), _cpu.Y);
        }

        private void Stx(ushort val, BindingMode mode)
        {
            _cpu.Bus.Write(GetAddress(val, mode), _cpu.X);
        }

        private void Brk(ushort param, BindingMode mode)
        {
            PushPc((ushort)(_cpu.PC + 1));
            Php(0, BindingMode.Immediate);

            _cpu.Status.BreakInterrupt = true;
            _cpu.PC = ReadWord(0xFFFE);
        }

        private void Rti(ushort param, BindingMode mode)
        {
            Plp(0, BindingMode.Implied);
            PopPc();
        }

        private void PopPc()
        {
            byte f = Pop();
            byte s = Pop();
            ushort VAL = (ushort)((f<<8) + s);
            _cpu.PC = VAL;
        }

        private void Jmp(ushort param, BindingMode mode)
        {
            _cpu.PC = GetAddress(param, mode);
        }

        private void Inc(ushort param, BindingMode mode)
        {
            var addr = GetAddress(param, mode);
            var val = (byte)(_cpu.Bus.Read(addr) + 1);
            _cpu.Bus.Write(addr, val);
            SetNegativeAndZeroFlag(val);
        }

        private void Inx(ushort param, BindingMode mode)
        {
            _cpu.X += 1;
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Iny(ushort param, BindingMode mode)
        {
            _cpu.Y += 1;

            SetNegativeAndZeroFlag(_cpu.Y);
        }

        private void Dec(ushort param, BindingMode mode)
        {
            var addr = GetAddress(param, mode);
            var val = (byte)(_cpu.Bus.Read(addr) - 1);
            _cpu.Bus.Write(addr, val);

            SetNegativeAndZeroFlag(val);
        }

        private byte Pop()
        {
            return _cpu.Bus.Read((ushort)(0x0100 + (++_cpu.SP)));
        }

        private void Cpx(ushort value, BindingMode mode)
        {
            Cmp(_cpu.X, GetValue(value, mode));
        }

        private void Cpy(ushort value, BindingMode mode)
        {
            Cmp(_cpu.Y, GetValue(value, mode));
        }

        private void Cmp(ushort value, BindingMode mode)
        {
            Cmp(_cpu.A, GetValue(value, mode));
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
            _cpu.Bus.Write((ushort)(0x0100 + (_cpu.SP--)), value);
        }

        private void Php(ushort param, BindingMode mode)
        {
            Push((byte)(_cpu.Status.Value | (byte)16));
            _cpu.Status.BreakInterrupt = false;
        }

        private void Plp(ushort param, BindingMode mode)
        {
            var val = Pop();

            _cpu.Status.Carry = (val & 0b00000001) > 0;
            _cpu.Status.Zero = (val & 0b00000010) > 0;
            _cpu.Status.InterruptDisable = (val & 0b00000100) > 0;
            _cpu.Status.DecimalMode = (val & 0b00001000) > 0;
            _cpu.Status.BreakInterrupt =  (val & 0b00010000) > 0;
            
            _cpu.Status.Overflow = (val & 0b01000000) > 0;
            _cpu.Status.Negative = (val & 0b10000000) > 0;
        }

        private void Pla(ushort param, BindingMode mode)
        {
            _cpu.A = Pop();
            SetNegativeAndZeroFlag(_cpu.A);
        }

        private void And(ushort param, BindingMode mode)
        {
            _cpu.A &= GetValue(param, mode);
            SetNegativeAndZeroFlag(_cpu.A);
        }

        private void Dex(ushort param, BindingMode mode)
        {
            _cpu.X -= 1;
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Dey(ushort param, BindingMode mode)
        {
            _cpu.Y -= 1;
            SetNegativeAndZeroFlag(_cpu.Y);
        }

  
        private void Bcc(ushort param, BindingMode mode)
        {
            JumpRelativeLocation(!_cpu.Status.Carry, param);
        }

        private void Bcs(ushort param, BindingMode mode)
        {
            JumpRelativeLocation(_cpu.Status.Carry, param);
        }


        private void Beq(ushort param, BindingMode mode)
        {
            JumpRelativeLocation(_cpu.Status.Zero, param);
        }

        private void Bne(ushort param, BindingMode mode)
        {
            JumpRelativeLocation(!_cpu.Status.Zero, param);
        }


        private void Bmi(ushort param, BindingMode mode)
        {
            JumpRelativeLocation(_cpu.Status.Negative, param);
        }

        private void Bpl(ushort param, BindingMode mode)
        {
            JumpRelativeLocation(!_cpu.Status.Negative, param);
        }

        private void Bvs(ushort param, BindingMode mode)
        {
            JumpRelativeLocation(_cpu.Status.Overflow, param);
        }

        private void Bvc(ushort param, BindingMode mode)
        {
            JumpRelativeLocation(!_cpu.Status.Overflow, param);
        }

        private void Pha(ushort param, BindingMode mode)
        {
            Push(_cpu.A);
        }

        private void Nop(ushort param, BindingMode mode)
        {

        }

        private void Rts(ushort param, BindingMode mode)
        {
            PopPc();
        }

        private void Jsr(ushort param, BindingMode mode)
        {
            PushPc((ushort)(_cpu.PC));
            _cpu.PC = param;
        }


        private void Sec(ushort param, BindingMode mode)
        {
            _cpu.Status.Carry = true;
        }

        private void Sei(ushort param, BindingMode mode)
        {
            _cpu.Status.InterruptDisable = true;
        }

        private void Sed(ushort param, BindingMode mode)
        {
            _cpu.Status.DecimalMode = true;
        }

        private void Clc(ushort param, BindingMode mode)
        {
            _cpu.Status.Carry = false;
        }

        private void Cld(ushort param, BindingMode mode)
        {
            _cpu.Status.DecimalMode = false;
        }

        private void Clv(ushort param, BindingMode mode)
        {
            _cpu.Status.Overflow = false;
        }

        private void Cli(ushort param, BindingMode mode)
        {
            _cpu.Status.InterruptDisable = false;
        }

        private void Tax(ushort param, BindingMode mode)
        {
            _cpu.X = _cpu.A;
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Tay(ushort param, BindingMode mode)
        {
            _cpu.Y = _cpu.A;
            SetNegativeAndZeroFlag(_cpu.Y);
        }

        private void Tsx(ushort param, BindingMode mode)
        {
            _cpu.X = _cpu.SP;
            SetNegativeAndZeroFlag(_cpu.X);
        }

        private void Txa(ushort param, BindingMode mode)
        {
            _cpu.A = _cpu.X;
            SetNegativeAndZeroFlag(_cpu.A);
        }

        private void Txs(ushort param, BindingMode mode)
        {
            _cpu.SP = _cpu.X;
        }

        private void Tya(ushort param, BindingMode mode)
        {
            _cpu.A = _cpu.Y;
            SetNegativeAndZeroFlag(_cpu.A);
        }
    }
}

