using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MemLib.Assembly;
using MemLib.Internals;
using MemLib.Memory;
using MemLib.Modules;
using MemLib.Native;
using MemLib.Patch;
using MemLib.Threading;

namespace MemLib {
    public class RemoteProcess : IDisposable, IEquatable<RemoteProcess> {
        public Process Native { get; }
        public SafeMemoryHandle Handle { get; }
        public IntPtr UnsafeHandle => Handle.DangerousGetHandle();
        public string ProcessName => Native.ProcessName;
        public bool IsRunning => Native != null && !Native.HasExited && !Handle.IsClosed && !Handle.IsInvalid;

        private bool? m_Is64Bit;
        public bool Is64Bit {
            get {
                if (m_Is64Bit.HasValue) return m_Is64Bit.Value;
                if (!Environment.Is64BitOperatingSystem) return false;
                return (bool) (m_Is64Bit = !(NativeMethods.IsWow64Process(Handle, out var wow64) && wow64));
            }
        }

        private MemoryManager m_Memory;
        public MemoryManager Memory => m_Memory ?? (m_Memory = new MemoryManager(this));
        private ModuleManager m_Modules;
        public ModuleManager Modules => m_Modules ?? (m_Modules = new ModuleManager(this));
        private ThreadManager m_Threads;
        public ThreadManager Threads => m_Threads ?? (m_Threads = new ThreadManager(this));
        private AssemblyManager m_Assembly;
        public AssemblyManager Assembly => m_Assembly ?? (m_Assembly = new AssemblyManager(this));
        private PatchManager m_Patch;
        public PatchManager Patch => m_Patch ?? (m_Patch = new PatchManager(this));

        public RemotePointer this[IntPtr address] => new RemotePointer(this, address);
        public RemotePointer this[long address] => new RemotePointer(this, new IntPtr(address));
        public RemoteModule this[string moduleName] => Modules[moduleName];

        public RemoteProcess() : this(Process.GetCurrentProcess()) { }
        public RemoteProcess(string processName) : this(FindProcess(processName)) { }
        public RemoteProcess(int processId) : this(Process.GetProcessById(processId)) { }

        [DebuggerStepThrough]
        public RemoteProcess(Process process) {
            Native = process ?? throw new ArgumentNullException(nameof(process));
            Handle = NativeMethods.OpenProcess(ProcessAccessFlags.AllAccess, false, process.Id);
        }

        #region Read Memory

        [DebuggerStepThrough]
        private byte[] ReadBytes(IntPtr address, long count) {
            var buffer = new byte[count < 0 ? 0 : count];
            if (!NativeMethods.ReadProcessMemory(Handle, address, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return buffer;
        }

        private bool ReadBytes(IntPtr address, out byte[] buffer, long count) {
            buffer = count <= 0 ? null : new byte[count];
            return buffer != null && NativeMethods.ReadProcessMemory(Handle, address, buffer, buffer.Length, out _);
        }

        public T Read<T>(IntPtr address) {
            return MarshalType<T>.ByteArrayToObject(ReadBytes(address, MarshalType<T>.Size));
        }

        public T Read<T>(Enum address) {
            return Read<T>(new IntPtr(Convert.ToInt64(address)));
        }

        public bool Read<T>(IntPtr address, out T value) {
            if (ReadBytes(address, out var buffer, MarshalType<T>.Size)) {
                value = MarshalType<T>.ByteArrayToObject(buffer);
                return true;
            }

            value = default;
            return false;
        }

        public bool Read<T>(Enum address, out T value) {
            return Read(new IntPtr(Convert.ToInt64(address)), out value);
        }

        public bool Read<T>(IntPtr address, out T[] value, int count) {
            if (count <= 0) {
                value = null;
                return false;
            }

            if (ReadBytes(address, out var buffer, MarshalType<T>.Size * count)) {
                value = new T[count];
                if (MarshalType<T>.TypeCode != TypeCode.Byte)
                    for (var i = 0; i < count; i++)
                        value[i] = MarshalType<T>.ByteArrayToObject(buffer, MarshalType<T>.Size * i);
                else
                    Buffer.BlockCopy(buffer, 0, value, 0, count);

                return true;
            }

            value = null;
            return false;
        }

        public bool Read<T>(Enum address, out T[] value, int count) {
            return Read(new IntPtr(Convert.ToInt64(address)), out value, count);
        }

        public T[] Read<T>(IntPtr address, int count) {
            var data = ReadBytes(address, MarshalType<T>.Size * count);

            var result = new T[count];
            if (MarshalType<T>.TypeCode != TypeCode.Byte)
                for (var i = 0; i < count; i++)
                    result[i] = MarshalType<T>.ByteArrayToObject(data, MarshalType<T>.Size * i);
            else
                Buffer.BlockCopy(data, 0, result, 0, count);

            return result;
        }

        public T[] Read<T>(Enum address, int count) {
            return Read<T>(new IntPtr(Convert.ToInt64(address)), count);
        }

        public string ReadString(IntPtr address, Encoding encoding, int maxLength = 512) {
            var data = encoding.GetString(ReadBytes(address, maxLength));
            var eosPos = data.IndexOf('\0');
            return eosPos == -1 ? data : data.Substring(0, eosPos);
        }

        public string ReadString(Enum address, Encoding encoding, int maxLength = 512) {
            return ReadString(new IntPtr(Convert.ToInt64(address)), encoding, maxLength);
        }

        public string ReadString(IntPtr address, int maxLength = 512) {
            return ReadString(address, Encoding.UTF8, maxLength);
        }

        public string ReadString(Enum address, int maxLength = 512) {
            return ReadString(new IntPtr(Convert.ToInt64(address)), Encoding.UTF8, maxLength);
        }

        public IntPtr ReadPointer(IntPtr address, params int[] offsets) {
            try {
                if (offsets == null || offsets.Length == 0)
                    return Is64Bit ? Read<IntPtr>(address) : (IntPtr)Read<int>(address);
                if(Is64Bit)
                    return offsets.Aggregate(address, (current, offset) => Read<IntPtr>(current) + offset);
                return offsets.Aggregate(address, (current, offset) => (IntPtr) Read<int>(current) + offset);
            } catch {
                return IntPtr.Zero;
            }
        }

        #endregion

        #region Write Memory

        private bool WriteBytes(IntPtr address, byte[] buffer) {
            bool result;
            using (new MemoryProtection(this, address, buffer.LongLength)) {
                result = NativeMethods.WriteProcessMemory(Handle, address, buffer, buffer.Length, out _);
            }
            return result;
        }

        public bool Write<T>(IntPtr address, T value) {
            return WriteBytes(address, MarshalType<T>.ObjectToByteArray(value));
        }

        public bool Write<T>(Enum address, T value) {
            return Write(new IntPtr(Convert.ToInt64(address)), value);
        }

        public bool Write<T>(IntPtr address, T[] array) {
            var size = MarshalType<T>.Size;
            var buffer = new byte[size * array.Length];
            for (var i = 0; i < array.Length; i++)
                Buffer.BlockCopy(MarshalType<T>.ObjectToByteArray(array[i]), 0, buffer, size * i, size);
            return WriteBytes(address, buffer);
        }

        public bool Write<T>(Enum address, T[] array) {
            return Write(new IntPtr(Convert.ToInt64(address)), array);
        }

        public bool WriteString(IntPtr address, string text, Encoding encoding) {
            return WriteBytes(address, encoding.GetBytes(text + '\0'));
        }

        public bool WriteString(IntPtr address, string text) {
            return WriteString(address, text, Encoding.UTF8);
        }

        public bool WriteString(Enum address, string text, Encoding encoding) {
            return WriteString(new IntPtr(Convert.ToInt64(address)), text, encoding);
        }

        public bool WriteString(Enum address, string text) {
            return WriteString(new IntPtr(Convert.ToInt64(address)), text, Encoding.UTF8);
        }

        #endregion

        public static Process FindProcess(string processName) {
            if (Path.HasExtension(processName))
                processName = Path.GetFileNameWithoutExtension(processName);
            return Process.GetProcessesByName(processName).FirstOrDefault();
        }

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

        public virtual void Dispose() {
            ((IDisposable)m_Memory)?.Dispose();
            ((IDisposable)m_Modules)?.Dispose();
            ((IDisposable)m_Threads)?.Dispose();
            ((IDisposable)m_Assembly)?.Dispose();
            ((IDisposable)m_Patch)?.Dispose();
            if(Handle != null && !Handle.IsClosed)
                Handle.Close();
            GC.SuppressFinalize(this);
        }

        ~RemoteProcess() {
            Dispose();
        }

        #endregion
    }
}