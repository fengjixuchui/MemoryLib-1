using System;
using System.Diagnostics;
using MemLib.Internals;

namespace MemLib.Assembly.CallingConventions {
    public static class CallingConventionSelector {
        [DebuggerStepThrough]
        public static ICallingConvention Get(CallingConvention callingConvention) {
            switch (callingConvention) {
                case CallingConvention.Cdecl:
                    return Singleton<CdeclCallingConvention>.Instance;
                case CallingConvention.StdCall:
                    return Singleton<StdcallCallingConvention>.Instance;
                case CallingConvention.ThisCall:
                    return Singleton<ThiscallCallingConvention>.Instance;
                case CallingConvention.FastCall:
                    return Singleton<FastcallCallingConvention>.Instance;
                case CallingConvention.FastCall64:
                    return Singleton<Fastcall64CallingConvention>.Instance;
                default:
                    throw new ArgumentException("Unsupported Calling Convention");
            }
        }
    }
}
