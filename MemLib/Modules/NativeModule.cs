using System;
using System.IO;
using MemLib.Native;

namespace MemLib.Modules {
    public class NativeModule : IEquatable<NativeModule> {
        public IntPtr BaseAddress { get; }
        public IntPtr EntryPointAddress { get; }
        public string FileName { get; }
        public long ModuleMemorySize { get; }
        public string ModuleName { get; }

        public NativeModule(SafeMemoryHandle hProcess, IntPtr hModule) {
            BaseAddress = hModule;
            FileName = ModuleHelper.GetModuleFileName(hProcess, hModule);
            ModuleName = string.IsNullOrEmpty(FileName) ? string.Empty : Path.GetFileName(FileName);
            if (ModuleHelper.GetModuleInformation(hProcess, hModule, out var info)) {
                EntryPointAddress = info.EntryPoint;
                ModuleMemorySize = (int) info.SizeOfImage.ToInt64();
            }
        }

        #region Equality members

        public bool Equals(NativeModule other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return BaseAddress.Equals(other.BaseAddress) &&
                   ModuleMemorySize == other.ModuleMemorySize &&
                   ModuleName.Equals(other.ModuleName, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((NativeModule) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (BaseAddress.GetHashCode() * 397) ^ ModuleMemorySize.GetHashCode();
            }
        }

        public static bool operator ==(NativeModule left, NativeModule right) {
            return Equals(left, right);
        }

        public static bool operator !=(NativeModule left, NativeModule right) {
            return !Equals(left, right);
        }

        #endregion
    }
}