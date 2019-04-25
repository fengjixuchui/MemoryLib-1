using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MemLib.Internals;
using MemLib.Native;

namespace MemLib {
    public class RemoteProcess : IDisposable, IEquatable<RemoteProcess> {
        public Process Native { get; }
        public IntPtr Handle => Native.Handle;

        private bool? m_Is64Bit;
        public bool Is64Bit {
            get {
                if (!Environment.Is64BitOperatingSystem) return false;
                if (m_Is64Bit.HasValue) return m_Is64Bit.Value;
                return (bool) (m_Is64Bit = !(NativeMethods.IsWow64Process(Handle, out var wow64) && wow64));
            }
        }

        public RemoteProcess() : this(Process.GetCurrentProcess()) { }
        public RemoteProcess(string processName) : this(Utils.FindProcess(processName)) { }
        public RemoteProcess(int processId) : this(Process.GetProcessById(processId)) { }

        public RemoteProcess(Process process) {
            Native = process ?? throw new ArgumentNullException(nameof(process));
        }

        #region Read Memory

        [DebuggerStepThrough]
        public byte[] ReadBytes(IntPtr address, long count) {
            var buffer = new byte[count];
            if (!NativeMethods.ReadProcessMemory(Handle, address, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return buffer;
        }

        public bool ReadBytes(IntPtr address, ref byte[] buffer) {
            return buffer != null && NativeMethods.ReadProcessMemory(Handle, address, buffer, buffer.Length, out _);
        }

        public bool ReadBytes(IntPtr address, out byte[] buffer, long count) {
            buffer = count <= 0 ? null : new byte[count];
            return buffer != null && NativeMethods.ReadProcessMemory(Handle, address, buffer, buffer.Length, out _);
        }

        public T Read<T>(IntPtr address) {
            return MarshalType<T>.ByteArrayToObject(ReadBytes(address, MarshalType<T>.Size));
        }

        public bool Read<T>(IntPtr address, out T value) {
            if (ReadBytes(address, out var buffer, MarshalType<T>.Size)) {
                value = MarshalType<T>.ByteArrayToObject(buffer);
                return true;
            }

            value = default;
            return false;
        }

        public bool Read<T>(IntPtr address, out T[] value, int count) {
            if (count <= 0) {
                value = null;
                return false;
            }

            if (ReadBytes(address, out var buffer, MarshalType<T>.Size * count)) {
                value = new T[count];
                if (typeof(T) != typeof(byte))
                    for (var i = 0; i < count; i++)
                        value[i] = MarshalType<T>.ByteArrayToObject(buffer, MarshalType<T>.Size * i);
                else
                    Buffer.BlockCopy(buffer, 0, value, 0, count);

                return true;
            }

            value = null;
            return false;
        }

        public T[] Read<T>(IntPtr address, int count) {
            var data = ReadBytes(address, MarshalType<T>.Size * count);

            var result = new T[count];
            if (typeof(T) != typeof(byte))
                for (var i = 0; i < count; i++)
                    result[i] = MarshalType<T>.ByteArrayToObject(data, MarshalType<T>.Size * i);
            else
                Buffer.BlockCopy(data, 0, result, 0, count);

            return result;
        }

        public string ReadString(IntPtr address, Encoding encoding, int maxLength = 512) {
            var data = encoding.GetString(ReadBytes(address, maxLength));
            var eosPos = data.IndexOf('\0');
            return eosPos == -1 ? data : data.Substring(0, eosPos);
        }

        public string ReadString(IntPtr address, int maxLength = 512) {
            return ReadString(address, Encoding.UTF8, maxLength);
        }

        public IntPtr ReadPointer(IntPtr address, params int[] offsets) {
            try {
                if (offsets == null || offsets.Length == 0)
                    return Read<IntPtr>(address);
                return offsets.Aggregate(address, (current, offset) => Read<IntPtr>(current) + offset);
            } catch {
                return IntPtr.Zero;
            }
        }

        #endregion

        #region Write Memory

        public bool WriteBytes(IntPtr address, byte[] buffer) {
            return NativeMethods.WriteProcessMemory(Handle, address, buffer, buffer.Length, out _);
        }

        public bool Write<T>(IntPtr address, T value) {
            return WriteBytes(address, MarshalType<T>.ObjectToByteArray(value));
        }

        public bool Write<T>(IntPtr address, T[] array) {
            var size = MarshalType<T>.Size;
            var buffer = new byte[size * array.Length];
            for (var i = 0; i < array.Length; i++)
                Buffer.BlockCopy(MarshalType<T>.ObjectToByteArray(array[i]), 0, buffer, size * i, size);
            return WriteBytes(address, buffer);
        }

        public bool WriteString(IntPtr address, string text, Encoding encoding) {
            return WriteBytes(address, encoding.GetBytes(text + '\0'));
        }

        public bool WriteString(IntPtr address, string text) {
            return WriteString(address, text, Encoding.UTF8);
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

        #region IDisposable

        protected bool IsDisposed { get; set;}

        public virtual void Dispose() {
            if (!IsDisposed) {
                IsDisposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~RemoteProcess() {
            Dispose();
        }

        #endregion
    }
}