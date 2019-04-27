using System;

namespace MemLib.Assembly {
    internal interface ICallingConvention {
        string FormatCall(IntPtr function, IntPtr[] parameters);
    }
}
