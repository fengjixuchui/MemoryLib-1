using System;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace MemLib {
    public class RemoteProcess : IDisposable, IEquatable<RemoteProcess> {
        public Process Native { get; }
        public SafeProcessHandle SafeHandle => Native.SafeHandle;
        public IntPtr Handle => Native.Handle;

        public RemoteProcess() : this(Process.GetCurrentProcess()) { }
        public RemoteProcess(string processName) : this(Utils.FindProcess(processName)) { }
        public RemoteProcess(Process process) {
            Native = process ?? throw new ArgumentNullException(nameof(process));
        }

        #region IDisposable
        
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        ~RemoteProcess() {
            Dispose();
        }

        #endregion

        #region Equality members

        public bool Equals(RemoteProcess other) {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Native.Id.Equals(other.Native.Id);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RemoteProcess) obj);
        }

        public override int GetHashCode() {
            return Native != null ? Native.Id : 0;
        }

        public static bool operator ==(RemoteProcess left, RemoteProcess right) {
            return Equals(left, right);
        }

        public static bool operator !=(RemoteProcess left, RemoteProcess right) {
            return !Equals(left, right);
        }

        #endregion
    }
}