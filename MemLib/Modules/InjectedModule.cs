using System;

namespace MemLib.Modules {
    public sealed class InjectedModule : RemoteModule, IDisposable {
        public bool IsDisposed { get; private set; }
        public bool MustBeDisposed { get; set; }

        internal InjectedModule(RemoteProcess process, NativeModule module, bool mustBeDisposed = true) : base(process, module) {
            MustBeDisposed = mustBeDisposed;
        }

        public override string ToString() => $"{base.ToString()} MustBeDisposed={MustBeDisposed}";

        #region IDisposable

        public void Dispose() {
            if (!IsDisposed) {
                IsDisposed = true;
                m_Process.Modules.Eject(this);
                BaseAddress = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }

        ~InjectedModule() {
            if (MustBeDisposed)
                Dispose();
        }

        #endregion
    }
}