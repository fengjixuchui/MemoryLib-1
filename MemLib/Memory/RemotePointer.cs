using System;
using System.Text;
using MemLib.Native;

namespace MemLib.Memory {
    public class RemotePointer : IEquatable<RemotePointer> {
        protected readonly RemoteProcess m_Process;
        public IntPtr BaseAddress { get; protected set; }
        public virtual bool IsValid => m_Process.IsRunning && BaseAddress != IntPtr.Zero;

        public RemotePointer(RemoteProcess process, IntPtr address) {
            m_Process = process;
            BaseAddress = address;
        }

        public override string ToString() => $"0x{BaseAddress.ToInt64():X}";

        public MemoryProtection ChangeProtection(int size, MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true) {
            return new MemoryProtection(m_Process, BaseAddress, size, protection, mustBeDisposed);
        }

        #region ReadMemory

        public T Read<T>(int offset) {
            return m_Process.Read<T>(BaseAddress + offset);
        }

        public T Read<T>() {
            return Read<T>(0);
        }

        public T[] Read<T>(int offset, int count) {
            return m_Process.Read<T>(BaseAddress + offset, count);
        }

        public string ReadString(int offset, Encoding encoding, int maxLength = 512) {
            return m_Process.ReadString(BaseAddress + offset, encoding, maxLength);
        }

        public string ReadString(Encoding encoding, int maxLength = 512) {
            return ReadString(0, encoding, maxLength);
        }

        #endregion

        #region WriteMemory

        public void Write<T>(int offset, T value) {
            m_Process.Write(BaseAddress + offset, value);
        }

        public void Write<T>(T value) {
            Write(0, value);
        }

        public void Write<T>(int offset, T[] array) {
            m_Process.Write(BaseAddress + offset, array);
        }

        public void Write<T>(T[] array) {
            Write(0, array);
        }

        public void WriteString(int offset, string text, Encoding encoding) {
            m_Process.WriteString(BaseAddress + offset, text, encoding);
        }

        public void WriteString(string text, Encoding encoding) {
            WriteString(0, text, encoding);
        }

        public void WriteString(int offset, string text) {
            m_Process.WriteString(BaseAddress + offset, text);
        }

        public void WriteString(string text) {
            WriteString(0, text);
        }

        #endregion

        #region Equality members

        public bool Equals(RemotePointer other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return m_Process.Equals(other.m_Process) && BaseAddress.Equals(other.BaseAddress);
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RemotePointer) obj);
        }

        public override int GetHashCode() {
            return m_Process.GetHashCode() ^ BaseAddress.GetHashCode();
        }

        public static bool operator ==(RemotePointer left, RemotePointer right) {
            return Equals(left, right);
        }

        public static bool operator !=(RemotePointer left, RemotePointer right) {
            return !Equals(left, right);
        }

        #endregion
    }
}