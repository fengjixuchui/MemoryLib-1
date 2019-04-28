using System;

namespace MemLib.Assembly {
    [Flags]
    public enum AssemblerSyntax {
        Intel = 1, // X86 Intel syntax - default on X86
        Att = 2, // X86 ATT asm syntax
        Nasm = 4, // X86 Nasm syntax
        //Masm = 8, // X86 Masm syntax - unsupported yet
        Gas = 16, // X86 GNU GAS syntax
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