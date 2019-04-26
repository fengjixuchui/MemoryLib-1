using System;
using MemLib.Native;

namespace MemLib.Memory {
    public sealed class RemoteAllocation : RemoteRegion, IDisposable {
        public bool IsDisposed { get; private set; }
        public bool MustBeDisposed { get; set; }

        internal RemoteAllocation(RemoteProcess process, int size, MemoryProtectionFlags protect, bool mustBeDisposed) :
            base(process, MemoryHelper.Allocate(process.Handle, size, protect)) {
            MustBeDisposed = mustBeDisposed;
        }

        #region IDisposable

        public void Dispose() {
            if (IsDisposed) return;
            IsDisposed = true;
            Release();
            m_Process.Memory.Deallocate(this);
            BaseAddress = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        ~RemoteAllocation() {
            if (MustBeDisposed)
                Dispose();
        }

        #endregion
    }
}