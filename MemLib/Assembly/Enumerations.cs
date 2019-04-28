using System;

namespace MemLib.Assembly {
    [Flags]
    public enum AssemblerSyntax {
        Intel = 1, // X86 Intel syntax - default on X86 (KS_OPT_SYNTAX).
        Att = 2, // X86 ATT asm syntax (KS_OPT_SYNTAX).
        Nasm = 4, // X86 Nasm syntax (KS_OPT_SYNTAX).
        //Masm = 8, // X86 Masm syntax (KS_OPT_SYNTAX) - unsupported yet.
        Gas = 16, // X86 GNU GAS syntax (KS_OPT_SYNTAX).
        Radix16 = 32, // All immediates are in hex format (i.e 12 is 0x12)
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