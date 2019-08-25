using System;
using System.Text.RegularExpressions;

namespace emulator6502
{
    public class Rom : Addressable
    {
        public Rom(ushort size) : base(size)
        {
        }

        public void LoadBinaryProgram(byte[] data, ushort address)
        {
            for (ushort i = 0; i < data.Length; i++)
                Write(address++, data[i]);
            Console.WriteLine("Max address:" + (address-1).ToString());
        }

        /// <summary>
        /// Awaits for a string of hex formatted binary data
        /// "0F 09 00 AF"
        /// </summary>
        /// <param name="programBytes"></param>
        public void LoadBinaryProgram(string programBytes)
        {
            programBytes = Regex.Replace(programBytes, @"\s+", "").ToUpper(); ;

            for (ushort i = 0; i < programBytes.Length/2; i++)
            {
                Write(i, byte.Parse( programBytes.Substring(i*2,2), System.Globalization.NumberStyles.HexNumber));
            }
        }
    }
}
