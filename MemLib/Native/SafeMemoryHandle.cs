using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace MemLib.Native {
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public sealed class SafeMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid {

        internal static SafeMemoryHandle InvalidHandle = new SafeMemoryHandle(IntPtr.Zero);

        internal SafeMemoryHandle() : base(true) { }

        internal SafeMemoryHandle(IntPtr handle) : base(true) {
            SetHandle(handle);
        }

        public SafeMemoryHandle(IntPtr existingHandle, bool ownsHandle) : base(ownsHandle) {
            SetHandle(existingHandle);
        }

        internal void InitialSetHandle(IntPtr h){
            Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
            base.handle = h;
        }
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle() {
            return CloseHandle(handle);
        }
    }
}