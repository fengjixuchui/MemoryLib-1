using System;
using System.Text;

namespace MemLib.Assembly.CallingConventions {
    internal class FastcallCallingConvention : ICallingConvention {
        public string FormatCall(IntPtr function, IntPtr[] parameters) {
            var asm = new StringBuilder();
            var numArgs = parameters.Length;

            asm.AppendLine("push ebp");
            asm.AppendLine("mov ebp, esp");

            asm.AppendLine($"mov ecx, 0x{(numArgs > 0 ? parameters[0] : IntPtr.Zero).ToInt64():X}");
            asm.AppendLine($"mov edx, 0x{(numArgs > 1 ? parameters[1] : IntPtr.Zero).ToInt64():X}");

            if (numArgs > 2) {
                for (var i = numArgs - 1; i >= 2; i--) {
                    asm.AppendLine($"push 0x{parameters[i].ToInt64():X}");
                }
            }

            asm.AppendLine($"call 0x{function.ToInt64():X}");

            asm.AppendLine("mov esp, ebp");
            asm.AppendLine("pop ebp");

            asm.AppendLine("ret 4");

            return asm.ToString();
        }
    }
}