using System;
using System.IO;

namespace emulator6502
{
    public delegate void OpCodeEventHandler(Cpu sender, OpcodeEventArgs e);

    public class Cpu
    {
        private readonly OpCodes _opcodes;

        internal IAddressable Bus { get; private set; }

        public CpuState State { get; private set; }
        public StatusRegister Status { get; } = new StatusRegister();
        public ushort PC { get; internal set; }
        public byte SP { get; internal set; }
        public byte A { get; internal set; }
        public byte X { get; internal set; }
        public byte Y { get; internal set; }

        public ulong Cycles { get; internal set; }

        public event OpCodeEventHandler BeforeOperationExecuted;
        public event OpCodeEventHandler AfterOperationExecuted;

        public Cpu(IAddressable bus)
        {
            _opcodes = new OpCodes(this);
            Bus = bus;
        }

        public void Irq()
        {
            _opcodes.Irq();
        }

        public void Nmi()
        {
            _opcodes.Nmi();
        }

        public void Reset()
        {
            PC = ReadWord(0XFFFC);
            State = CpuState.Running;
            Cycles = 8;
            SP = 0xFF;
            Status.Reset();
        }

        public CpuSnapshot GetSnapshot()
        {
            return new CpuSnapshot()
            {
                Cycles = Cycles,
                Status = Status.Value,
                A = A,
                X = X,
                Y = Y,
                PC = PC,
                SP = SP
            };
        }

        public void LoadSnapshot(CpuSnapshot snapshot)
        {
            A = snapshot.A;
            X = snapshot.X;
            Y = snapshot.Y;
            Cycles = snapshot.Cycles;
            PC = snapshot.PC;
            SP = snapshot.SP;
        }

        public bool Clock()
        {
            var prevPC = PC;
            var cycles = Cycles;
            var entry = _opcodes[Bus.Read(PC++)];
            if (entry.Enum == OpcodeEnum.BRK) return false;

            ushort parameter = 0;

            if (entry.Length == 2) parameter = ReadWord(PC);
            else if (entry.Length == 1) parameter = Bus.Read(PC);

            PC += entry.Length;

            if (!InnerExecute(entry, parameter, prevPC, cycles))
                PC = prevPC;
            

            return true;
        }

        public void Run()
        {
            while (State == CpuState.Running)
            {
                if (Clock()) continue;
                State = CpuState.Break;
                break;
            }
        }

        public void Execute(OpcodeEnum opcodeEnum, AddressingMode mode, ushort parameter = 0)
        {
            InnerExecute(_opcodes.Get(opcodeEnum, mode), parameter, 0, 0);
        }

        public void Execute(OpcodeEnum opcodeEnum, ushort parameter = 0)
        {
            InnerExecute(_opcodes.Get(opcodeEnum), parameter, 0, 0);
        }

        public void Execute(Opcode opcode, ushort parameter = 0)
        {
            InnerExecute(opcode, parameter, 0, 0);
        }

        private bool InnerExecute(Opcode opcode, ushort param, ushort pos, ulong cycles)
        {
            OpcodeEventArgs arg = null;
            if (BeforeOperationExecuted != null)
            {
                arg = new OpcodeEventArgs(new FullOpcode(opcode, param, pos), (int)(Cycles- cycles));
                BeforeOperationExecuted?.Invoke(this, arg);

                if (arg.RequestPauseExecution)
                {
                    State = CpuState.Paused;
                    return false;
                }
            }

            _opcodes.Execute(param, opcode);
            Cycles += opcode.Cycles;

            if (AfterOperationExecuted == null) return true;
            if (arg == null) arg = new OpcodeEventArgs(new FullOpcode(opcode, param, pos), (int)(Cycles-cycles));
            else arg.RequestPauseExecution = false;

            AfterOperationExecuted?.Invoke(this, arg);
            if (arg.RequestPauseExecution)
            {
                State = CpuState.Paused;
            }

            return true;
        }

        private ushort ReadWord(ushort address)
        {
            return (ushort)((Bus.Read((ushort)(address + 1)) << 8) + Bus.Read(address));
        }
    }
}
