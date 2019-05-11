using System;
using MemLib.Memory;

namespace MemLib.Internals {
    public static class MarshalValue {
        public static MarshalledValue<T> Marshal<T>(RemoteProcess proc, T value, bool byRef = false) {
            return new MarshalledValue<T>(proc, value, byRef);
        }
    }

    public sealed class MarshalledValue<T> : IMarshalledValue {
        private readonly RemoteProcess m_Process;
        public T Value { get; }
        public RemoteAllocation Allocated { get; private set; }
        public IntPtr Reference { get; private set; }
        public Type Type { get; }
        public bool IsByRef { get; }

        public MarshalledValue(RemoteProcess process, T value, bool byRef) {
            m_Process = process;
            Value = value;
            Type = value.GetType();
            IsByRef = byRef;
            Marshal();
        }
        
        private void Marshal() {
            if (typeof(T) == typeof(string)) {
                var text = Value.ToString();
                Allocated = m_Process.Memory.Allocate(text.Length + 1);
                Allocated.WriteString(0, text);
                Reference = Allocated.BaseAddress;
            } else {
                var byteArray = MarshalType<T>.ObjectToByteArray(Value);

                if (MarshalType<T>.CanBeStoredInRegisters && !IsByRef) {
                    Reference = MarshalType<IntPtr>.ByteArrayToObject(byteArray);
                } else {
                    Allocated = m_Process.Memory.Allocate(MarshalType<T>.Size);
                    Allocated.Write(0, Value);
                    Reference = Allocated.BaseAddress;
                }
            }
        }

        public T MarshalToManaged() {
            return MarshalType<T>.PtrToRefObject(m_Process, Reference);
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