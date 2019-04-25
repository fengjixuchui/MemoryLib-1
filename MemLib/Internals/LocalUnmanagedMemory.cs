using System;
using System.Runtime.InteropServices;

namespace MemLib.Internals {
    internal sealed class LocalUnmanagedMemory : IDisposable {
        public IntPtr Address { get; private set; }
        public int Size { get; }

        public LocalUnmanagedMemory(int size) {
            Size = size;
            Address = Marshal.AllocHGlobal(Size);
        }

        public override string ToString() {
            return $"Address={Address.ToInt64():X8} Size={Size:X}";
        }

        #region Read

        public T Read<T>() {
            return (T) Marshal.PtrToStructure(Address, typeof(T));
        }

        public byte[] Read() {
            var bytes = new byte[Size];
            Marshal.Copy(Address, bytes, 0, Size);
            return bytes;
        }

        #endregion

        #region Write

        public void Write(byte[] data, int index = 0) {
            Marshal.Copy(data, index, Address, data.Length);
        }

        public void Write<T>(T data) {
            Marshal.StructureToPtr(data, Address, false);
        }

        #endregion

        #region IDisposable

        public void Dispose() {
            Marshal.FreeHGlobal(Address);
            Address = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        ~LocalUnmanagedMemory() {
            Dispose();
        }

        #endregion
    }
}