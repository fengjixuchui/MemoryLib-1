using System;

namespace MemLib.Assembly.CallingConventions {
    internal interface ICallingConvention {
        string FormatCall(IntPtr function, IntPtr[] parameters);
    }
}
