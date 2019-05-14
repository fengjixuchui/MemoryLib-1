using System;
using MemLibNative.Keystone;
using MemLibNative.Capstone;

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
        Intel = CapstoneSyntax.Intel,
        Att = CapstoneSyntax.Att,
        Masm = CapstoneSyntax.Masm
    }

    public enum DisasmMode {
        Mode16 = CapstoneMode.Mode16,
        Mode32 = CapstoneMode.Mode32,
        Mode64 = CapstoneMode.Mode64
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