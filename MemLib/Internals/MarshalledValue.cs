using System;
using MemLib.Memory;

namespace MemLib.Internals {
    internal static class MarshalValue {
        public static MarshalledValue<T> Marshal<T>(RemoteProcess proc, T value, bool byRef = false) {
            return new MarshalledValue<T>(proc, value, byRef);
        }
    }

    internal sealed class MarshalledValue<T> : IMarshalledValue {
        private readonly RemoteProcess m_Process;
        public T Value { get; }
        public RemoteAllocation Allocated { get; private set; }
        public IntPtr Reference { get; private set; }
        public Type Type { get; }

        public MarshalledValue(RemoteProcess process, T value, bool byRef) {
            m_Process = process;
            Value = value;
            Type = typeof(T);
            if(byRef)
                MarshalByRef();
            else Marshal();
        }
        
        private void Marshal() {
            if (typeof(T) == typeof(string)) {
                var text = Value.ToString();
                Allocated = m_Process.Memory.Allocate(text.Length + 1);
                Allocated.WriteString(0, text);
                Reference = Allocated.BaseAddress;
            } else {
                var byteArray = MarshalType<T>.ObjectToByteArray(Value);

                if (MarshalType<T>.CanBeStoredInRegisters) {
                    Reference = MarshalType<IntPtr>.ByteArrayToObject(byteArray);
                } else {
                    Allocated = m_Process.Memory.Allocate(MarshalType<T>.Size);
                    Allocated.Write(0, Value);
                    Reference = Allocated.BaseAddress;
                }
            }
        }

        private void MarshalByRef() {
            if (typeof(T) == typeof(string)) {
                var text = Value.ToString();
                Allocated = m_Process.Memory.Allocate(text.Length + 1);
                Allocated.WriteString(0, text);
                Reference = Allocated.BaseAddress;
            } else {
                Allocated = m_Process.Memory.Allocate(MarshalType<T>.Size);
                Allocated.Write(0, Value);
                Reference = Allocated.BaseAddress;
            }
        }

        #region IDisposable

        public void Dispose() {
            Allocated?.Dispose();
            Reference = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        ~MarshalledValue() {
            Dispose();
        }

        #endregion
    }
}