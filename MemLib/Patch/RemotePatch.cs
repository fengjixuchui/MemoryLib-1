using System;
using System.Linq;
using MemLib.Memory;

namespace MemLib.Patch {
    public sealed class RemotePatch : IDisposable, IEquatable<RemotePatch> {
        private readonly RemoteProcess m_Process;
        public bool IsDisposed { get; private set; }
        public bool MustBeDisposed { get; set; }

        public byte[] NewBytes { get; private set; }
        public byte[] OldBytes { get; private set; }

        public int Size => NewBytes.Length;
        public IntPtr PatchAddress { get; }
        public string Name { get; }

        public bool IsApplied {
            get {
                var remote = m_Process.Read<byte>(PatchAddress, Size);
                return remote.SequenceEqual(NewBytes);
            }
        }

        internal RemotePatch(RemoteProcess process, string name, IntPtr address, byte[] newBytes, bool mustDispose) {
            m_Process = process;
            Name = name;
            NewBytes = newBytes;
            PatchAddress = address;
            MustBeDisposed = mustDispose;
        }

        public void Apply() {
            if (IsApplied) return;
            OldBytes = m_Process.Read<byte>(PatchAddress, NewBytes.Length);
            using (new MemoryProtection(m_Process, PatchAddress, Size)) {
                m_Process.Write(PatchAddress, NewBytes);
            }
        }

        public void Remove() {
            m_Process.Patch.RemovePatch(this);
        }

        internal void InternalRemove() {
            if (!IsApplied) return;
            using (new MemoryProtection(m_Process, PatchAddress, Size)) {
                m_Process.Write(PatchAddress, OldBytes);
            }
        }

        public void ChangePatchBytes(byte[] newBytes) {
            if (IsApplied) {
                using (new MemoryProtection(m_Process, PatchAddress, Size)) {
                    m_Process.Write(PatchAddress, OldBytes);
                    m_Process.Write(PatchAddress, newBytes);
                }
            }
            NewBytes = newBytes;
        }

        #region IDisposable

        public void Dispose() {
            if (IsDisposed) return;
            if (m_Process.IsRunning)
                Remove();
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~RemotePatch() {
            if (MustBeDisposed)
                Dispose();
        }

        #endregion

        #region IEquatable

        public bool Equals(RemotePatch other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return m_Process.Equals(other.m_Process) &&
                   PatchAddress.Equals(other.PatchAddress) &&
                   string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is RemotePatch patch && Equals(patch);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = m_Process.GetHashCode();
                hashCode = (hashCode * 397) ^ PatchAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(RemotePatch left, RemotePatch right) {
            return Equals(left, right);
        }

        public static bool operator !=(RemotePatch left, RemotePatch right) {
            return !Equals(left, right);
        }

        #endregion
        
    }
}