using System;

namespace MemLib.Threads {
    public sealed class FrozenThread : IDisposable {
        public RemoteThread Thread { get; }

        internal FrozenThread(RemoteThread thread) {
            Thread = thread;
        }

        public override string ToString() {
            return $"FrozenThreadId=0x{Thread.Id:X}";
        }

        #region IDisposable

        public void Dispose() {
            Thread.Resume();
        }

        #endregion
    }
}