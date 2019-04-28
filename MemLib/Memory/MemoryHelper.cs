using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using MemLib.Internals;
using MemLib.Native;

namespace MemLib.Memory {
    [DebuggerStepThrough]
    internal static class MemoryHelper {
        public static IntPtr Allocate(SafeMemoryHandle handle, long size,
            MemoryProtectionFlags protectionFlags = MemoryProtectionFlags.ExecuteReadWrite,
            MemoryAllocationFlags allocationFlags = MemoryAllocationFlags.Commit) {
            return NativeMethods.VirtualAllocEx(handle, IntPtr.Zero, size, allocationFlags, protectionFlags);
        }

        public static bool Free(SafeMemoryHandle handle, IntPtr address) {
            return NativeMethods.VirtualFreeEx(handle, address, 0, MemoryReleaseFlags.Release);
        }

        public static MemoryBasicInformation Query(SafeMemoryHandle handle, IntPtr baseAddress) {
            if (NativeMethods.VirtualQueryEx(handle, baseAddress, out var mbi, MarshalType<MemoryBasicInformation>.Size) == 0)
                throw new Win32Exception();
            return mbi;
        }
        
        public static IEnumerable<MemoryBasicInformation> Query(SafeMemoryHandle handle, IntPtr start, IntPtr end) {
            var address = start.ToInt64();
            var limit = end.ToInt64();

            if (address >= limit)
                throw new ArgumentOutOfRangeException(nameof(start));

            long ret;
            var mbiSize = MarshalType<MemoryBasicInformation>.Size;
            do {
                ret = NativeMethods.VirtualQueryEx(handle, new IntPtr(address), out var mbi, mbiSize);
                address += mbi.RegionSize.ToInt64();
                if (mbi.State != MemoryStateFlags.Free && ret != 0)
                    yield return mbi;
            } while (address < limit && ret != 0);
        }
    }
}