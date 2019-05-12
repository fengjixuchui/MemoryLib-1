using System;
using MemLibNative.Keystone;
using MemLibNative.Zydis;

namespace MemLib.Assembly {
    [Flags]
    public enum AssemblerSyntax {
        Intel = KsSyntax.Intel,
        Att = KsSyntax.Att,
        Nasm = KsSyntax.Nasm,
        //Masm = KsSyntax.Masm,
        Gas = KsSyntax.Gas,
        Radix16 = KsSyntax.Radix16,
    }

    public enum DisasmSyntax {
        Att = ZydisasmStyle.Att,
        Intel = ZydisasmStyle.Intel,
        Masm = ZydisasmStyle.Masm
    }

    public enum DisasmMode {
        Mode64 = ZydisasmMode.Mode64,
        Mode32 = ZydisasmMode.Mode32,
        Mode16 = ZydisasmMode.Mode16
    }

    public enum DisasmAddressWidth {
        Width16 = ZydisasmAddressWidth.Width16,
        Width32 = ZydisasmAddressWidth.Width32,
        Width64 = ZydisasmAddressWidth.Width64
    }

    public enum CallingConvention {
        Default,
        Cdecl,
        StdCall,
        ThisCall,
        FastCall,
        FastCall64
    }
}