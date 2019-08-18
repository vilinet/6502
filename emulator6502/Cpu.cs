﻿namespace emulator6502
{
    public delegate void OpCodeEventHandler(Cpu sender, OpcodeEventArgs e);

    public class Cpu
    {
        private readonly Opcodes opcodes;
        private readonly CpuOperations operations;
        internal readonly IAddressable Bus;
        
        public  CpuState State { get; private set; }
        public StatusRegister Status { get; } = new StatusRegister();
        public ushort PC { get; internal set; }
        public byte SP { get; internal set; }
        public byte A { get; internal set; }
        public byte X { get;  internal set; }
        public byte Y { get; internal set; }

        public uint Cycles { get; private set; }

        public event OpCodeEventHandler BeforeOperationExecuted;
        public event OpCodeEventHandler AfterOperationExecuted;

        public Cpu(IAddressable bus)
        {
            opcodes = new Opcodes();
            operations = new CpuOperations(this);
            Bus = bus;
            Reset();
        }

        public void Reset()
        {
            State = CpuState.Reset;
            Cycles = 0;
            Status.Reset();
        }

        public void Run()
        {
            if (State != CpuState.Paused)
            {
                PC = ReadWord(0XFFFC);
            }

            State = CpuState.Running;

            while (State == CpuState.Running)
            {
                ushort prevPC = PC;
                var entry = opcodes[Bus.Read(PC++)];
                if (entry.Enum == OpcodeEnum.BRK) break;

                ushort parameter = 0;

                if (entry.Mode == BindingMode.Absolute || entry.Mode == BindingMode.AbsoluteX || entry.Mode == BindingMode.AbsoluteY)
                {
                    parameter = ReadWord(PC);
                    PC += 2;
                }
                else if (entry.Mode != BindingMode.Implied)
                {
                    parameter = Bus.Read(PC++);
                }
                
                if(!InnerExecute(entry, parameter))
                {
                    PC = prevPC;
                }
            }
        }        

        public void Execute(OpcodeEnum opcodeEnum, BindingMode mode, ushort parameter = 0)
        {
            InnerExecute(opcodes.Get(opcodeEnum, mode), parameter);
        }

        public void Execute(OpcodeEnum opcodeEnum, ushort parameter = 0)
        {
            InnerExecute(opcodes.Get(opcodeEnum), parameter);
        }

        public void Execute(Opcode opcode, ushort parameter = 0)
        {
            InnerExecute(opcode, parameter);
        }

        private bool InnerExecute(Opcode opcode, ushort param)
        {
            OpcodeEventArgs arg = null;

            if (BeforeOperationExecuted!=null)
            {
                arg = new OpcodeEventArgs(opcode, param);
                BeforeOperationExecuted?.Invoke(this, arg);
                
                if (arg.RequestPauseExecution)
                {
                    State = CpuState.Paused;
                    return false;
                }
                
            }
            
            operations[opcode.Enum](param, opcode.Mode);
            Cycles += opcode.Cycles;
            
            if(AfterOperationExecuted != null)
            {
                if (arg == null) arg = new OpcodeEventArgs(opcode, param);
                else arg.RequestPauseExecution = false;

                AfterOperationExecuted?.Invoke(this, arg);
                if (arg.RequestPauseExecution)
                {
                    State = CpuState.Paused;
                }
            }
            
            return true;
        }

        private ushort ReadWord(ushort address)
        {
            return (ushort)((Bus.Read((ushort)(address + 1)) << 8) + Bus.Read(address));
        }
    }
}
