using System;
using System.Text;

namespace MemLib.Assembly.CallingConventions {
    internal class Fastcall64CallingConvention : ICallingConvention {
        public string FormatCall(IntPtr function, IntPtr[] parameters) {
            var asm = new StringBuilder();
            var numArgs = parameters.Length;

            asm.AppendLine("push rbp");
            asm.AppendLine("mov rbp, rsp");

            asm.AppendLine($"mov rcx, 0x{(numArgs > 0 ? parameters[0] : IntPtr.Zero).ToInt64():X}");
            asm.AppendLine($"mov rdx, 0x{(numArgs > 1 ? parameters[1] : IntPtr.Zero).ToInt64():X}");
            asm.AppendLine($"mov r8, 0x{(numArgs > 2 ? parameters[2] : IntPtr.Zero).ToInt64():X}");
            asm.AppendLine($"mov r9, 0x{(numArgs > 3 ? parameters[3] : IntPtr.Zero).ToInt64():X}");

            if (numArgs > 4) {
                for (var i = numArgs - 1; i >= 4; i--) {
                    asm.AppendLine($"mov rax, 0x{parameters[i].ToInt64():X}");
                    asm.AppendLine("push rax");
                }
            }

            asm.AppendLine("sub rsp, 0x20");
            asm.AppendLine($"mov rax, 0x{function.ToInt64():X}");
            asm.AppendLine("call rax");
            asm.AppendLine("add rsp, 0x20");

            if(numArgs > 4)
                asm.AppendLine($"add rsp, 0x{8 * (numArgs - 4):X}");

            asm.AppendLine("mov rsp, rbp");
            asm.AppendLine("pop rbp");

            asm.AppendLine("ret");

            return asm.ToString();
        }
    }
}