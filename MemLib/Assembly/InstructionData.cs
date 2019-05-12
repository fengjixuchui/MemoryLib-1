using System;

namespace MemLib.Assembly {
    public class InstructionData {
        public int Length;
        public IntPtr Address;
        public string Instruction;
        public byte[] Data;

        public override string ToString() {
            return $"{Address.ToInt64():X8}: {Instruction}";
        }
    }
}