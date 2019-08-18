using System.Collections.ObjectModel;
using System.Linq;

namespace emulator6502
{
    public class Opcodes : ReadOnlyDictionary<byte, Opcode>
    {
        /// <summary>
        /// Try to avoid using this function in every step
        /// It takes much time 
        /// Cache the result
        /// </summary>
        /// <param name="opcodeEnum"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Opcode Get(OpcodeEnum opcodeEnum, BindingMode mode)
        {
            return this.First(x => x.Value.Enum == opcodeEnum && x.Value.Mode == mode).Value;
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
            return this.Single(x => x.Value.Enum == opcodeEnum).Value;
        }
        public Opcodes() : base(new OpcodesListDictionary()) {  }
    }
}