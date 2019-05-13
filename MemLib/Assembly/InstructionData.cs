using System;

namespace MemLib.Assembly {
    public class InstructionData {
        public int Size;
        public IntPtr Address;
        public string Mnemonic;
        public string OpString;
        public byte[] Bytes;

        public long AddressLong => Address.ToInt64();
        public string Instruction => $"{Mnemonic} {OpString}";

        public override string ToString() {
            return $"{Address.ToInt64():X8}: {Mnemonic} {OpString}";
        }
    }
}