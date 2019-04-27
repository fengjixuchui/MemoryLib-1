using System;
using System.Runtime.InteropServices;

namespace MemLib.Assembly {
    [AttributeUsage(AttributeTargets.Delegate)]
    public sealed class RemoteFunctionAttribute : Attribute {
        public RemoteFunctionAttribute() { }

        public RemoteFunctionAttribute(IntPtr remoteAddress) {
            RemoteAddress = remoteAddress;
        }

        public RemoteFunctionAttribute(long remoteAddress) {
            RemoteAddress = new IntPtr(remoteAddress);
        }

        public RemoteFunctionAttribute(string dllName, string entryPoint) {
            DllName = dllName;
            EntryPoint = entryPoint;
        }

        internal string DllName;
        internal string EntryPoint;
        internal IntPtr RemoteAddress;

        public CallingConvention CallingConvention;
        public CharSet CharSet;
    }
}