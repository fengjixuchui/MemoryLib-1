using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using MemLib.Internals;
using MemLib.Native;

namespace MemLib.Modules {
    [DebuggerStepThrough]
    internal static class ModuleHelper {
        public static string GetModuleFileName(SafeMemoryHandle hProcess, IntPtr hModule) {
            var sb = new StringBuilder(260);
            if (NativeMethods.GetModuleFileNameEx(hProcess, hModule, sb, sb.Capacity) != 0)
                return sb.ToString();
            return string.Empty;
        }

        public static bool EnumProcessModules(SafeMemoryHandle hProcess, out IntPtr[] modules, ListModulesFlags flags = ListModulesFlags.ListModulesAll) {
            modules = new IntPtr[512];
            var numberOfModules = 0;
            var cb = IntPtr.Size * modules.Length;
            var lphModule = GCHandle.Alloc(modules, GCHandleType.Pinned);
            if (NativeMethods.EnumProcessModulesEx(hProcess, lphModule.AddrOfPinnedObject(), cb, out var cbNeeded, flags))
                numberOfModules = cbNeeded / IntPtr.Size;
            lphModule.Free();
            Array.Resize(ref modules, numberOfModules);
            return numberOfModules != 0;
        }

        public static bool GetModuleInformation(SafeMemoryHandle hProcess, IntPtr hModule, out ModuleInfo info) {
            var size = MarshalType<ModuleInfo>.Size;
            if (NativeMethods.GetModuleInformation(hProcess, hModule, out info, size))
                return true;
            return false;
        }

        public static string UnDecorateSymbolName(string name, UnDecorateFlags flags = UnDecorateFlags.NameOnly) {
            var sb = new StringBuilder(260);
            return NativeMethods.UnDecorateSymbolName(name, sb, sb.Capacity, flags) ? sb.ToString() : name;
        }
    }
}