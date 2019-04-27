using System;
using System.Text;

namespace MemLib.Assembly.CallingConventions {
    internal class CdeclCallingConvention : ICallingConvention {
        public string FormatCall(IntPtr function, IntPtr[] parameters) {
            var asm = new StringBuilder();
            var numArgs = parameters.Length;

            asm.AppendLine("push ebp");
            asm.AppendLine("mov ebp, esp");
            
            for (var i = numArgs - 1; i >= 0; i--) {
                asm.AppendLine($"push 0x{parameters[i].ToInt64():X}");
            }

            asm.AppendLine($"call 0x{function.ToInt64():X}");

            asm.AppendLine($"add esp, 0x{parameters.Length * 4:X}");

            asm.AppendLine("mov esp, ebp");
            asm.AppendLine("pop ebp");

            asm.AppendLine("ret 4");

            return asm.ToString();
        }
    }
}