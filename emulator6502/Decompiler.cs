using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace emulator6502
{
    public class Decompiler
    {
        private readonly Opcodes _opcodes = new Opcodes();
        
        public List<FullOpcode> Decompile(Stream stream, int start = 0)
        {
            
            var result = new List<FullOpcode>();

            if (stream.CanSeek) stream.Seek(start, SeekOrigin.Begin);
            while (stream.Position < stream.Length)
            {
                var pos = stream.Position;
                
                var code = (byte)stream.ReadByte();
                ushort parameter = 0;
                Opcode opcode = _opcodes[code];

                if (opcode == null)
                {
                    opcode = new Opcode(code, OpcodeEnum.DB, BindingMode.Implied, 0);
                    parameter = code;
                }
                else opcode = _opcodes[code];

                if (opcode.Length == 1)
                {
                    parameter = (ushort) stream.ReadByte();
                } 
                else if (opcode.Length == 2)
                {
                    var f = stream.ReadByte();
                    var s = stream.ReadByte();
                    parameter = (ushort)(f +( s << 8));
                }
                    
                result.Add( new FullOpcode(opcode, parameter, (ushort)pos));
            }

            return result;
        }

        public List<FullOpcode> Decompile(string programBytes)
        {
            programBytes = Regex.Replace(programBytes, @"\s+", "").ToUpper(); ;
            
            var byteArray = new byte[programBytes.Length / 2];
            
            for (ushort i = 0; i < programBytes.Length/2; i++)
            {
                byteArray[i] = byte.Parse(programBytes.Substring(i * 2, 2),  System.Globalization.NumberStyles.HexNumber);
            }

            return Decompile(new MemoryStream(byteArray));
        }
    }
}