using System;

namespace MemLib.Assembly.CallingConventions {
    public interface ICallingConvention {
        string FormatCall(IntPtr function, IntPtr[] parameters);
    }
}
