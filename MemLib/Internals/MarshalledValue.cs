using System;
using MemLib.Memory;

namespace MemLib.Internals {
    internal static class MarshalValue {
        public static MarshalledValue<T> Marshal<T>(RemoteProcess proc, T value) {
            return new MarshalledValue<T>(proc, value);
        }
    }

    internal sealed class MarshalledValue<T> : IMarshalledValue {
        private readonly RemoteProcess m_Process;
        public T Value { get; private set; }
        public RemoteAllocation Allocated { get; private set; }
        public IntPtr Reference { get; private set; }
        public Type Type { get; }
        public bool IsByRef { get; private set; }

        public MarshalledValue(RemoteProcess process, T value) {
            m_Process = process;
            Value = value;
            Type = value.GetType();
            Marshal();
        }
        
        private void Marshal() {
            if (typeof(T) == typeof(string)) {
                var text = Value.ToString();
                Allocated = m_Process.Memory.Allocate(text.Length + 1);
                Allocated.WriteString(0, text);
                Reference = Allocated.BaseAddress;
                IsByRef = true;
            } else {
                var byteArray = MarshalType<T>.ObjectToByteArray(Value);

                if (MarshalType<T>.CanBeStoredInRegisters) {
                    Reference = MarshalType<IntPtr>.ByteArrayToObject(byteArray);
                    IsByRef = false;
                } else {
                    Allocated = m_Process.Memory.Allocate(MarshalType<T>.Size);
                    Allocated.Write(0, Value);
                    Reference = Allocated.BaseAddress;
                    IsByRef = true;
                }
            }
        }

        public T MarshalToManaged() {
            if (typeof(T) == typeof(string)) {
                Value = (T) (object) m_Process.ReadString(Reference);
            } else {
                Value = MarshalType<T>.PtrToObject(m_Process, Reference);
            }
            return Value;
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