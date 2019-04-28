using System;
using System.ComponentModel;
using System.Diagnostics;
using MemLib.Internals;
using MemLib.Native;

namespace MemLib.Threads {
    [DebuggerStepThrough]
    public static class ThreadHelper {
        public static SafeMemoryHandle OpenThread(ThreadAccessFlags accessFlags, int threadId) {
            var handle = NativeMethods.OpenThread(accessFlags, false, threadId);
            if (handle.IsClosed || handle.IsInvalid)
                throw new Win32Exception();
            return handle;
        }

        public static SafeMemoryHandle CreateRemoteThread(SafeMemoryHandle processHandle, IntPtr startAddress, IntPtr parameter, ThreadCreationFlags creationFlags = ThreadCreationFlags.Run) {
            var handle = NativeMethods.CreateRemoteThread(processHandle, IntPtr.Zero, 0, startAddress, parameter, creationFlags, out _);
            if(handle.IsInvalid || handle.IsClosed)
                throw new Win32Exception();
            return handle;
        }

        private static IntPtr STILL_ACTIVE = new IntPtr(259);

        public static IntPtr? GetExitCodeThread(SafeMemoryHandle threadHandle) {
            if (!NativeMethods.GetExitCodeThread(threadHandle, out var exitCode))
                throw new Win32Exception();

            //TODO use something else instead of GetExitCodeThread if possible because of this
            // If the thread is still active
            if (exitCode == STILL_ACTIVE && NativeMethods.WaitForSingleObject(threadHandle, 0) == WaitValues.Timeout)
                return null;

            return exitCode;
        }

        public static ThreadBasicInformation NtQueryInformationThread(SafeMemoryHandle threadHandle) {
            var info = new ThreadBasicInformation();
            var ret = NativeMethods.NtQueryInformationThread(threadHandle, 0, ref info, MarshalType<ThreadBasicInformation>.Size, IntPtr.Zero);
            if (ret == 0)
                return info;
            throw new Win32Exception();
        }

        public static uint ResumeThread(SafeMemoryHandle threadHandle) {
            var ret = NativeMethods.ResumeThread(threadHandle);
            if (ret == uint.MaxValue)
                throw new Win32Exception();
            return ret;
        }

        public static uint SuspendThread(SafeMemoryHandle threadHandle) {
            var ret = NativeMethods.SuspendThread(threadHandle);
            if (ret == uint.MaxValue)
                throw new Win32Exception();
            return ret;
        }

        public static void TerminateThread(SafeMemoryHandle threadHandle, int exitCode) {
            var ret = NativeMethods.TerminateThread(threadHandle, exitCode);
            if (!ret)
                throw new Win32Exception();
        }

        public static WaitValues WaitForSingleObject(SafeMemoryHandle handle, TimeSpan? timeout) {
            var ret = NativeMethods.WaitForSingleObject(handle, timeout.HasValue ? Convert.ToUInt32(timeout.Value.TotalMilliseconds) : 0);
            if (ret == WaitValues.Failed)
                throw new Win32Exception();
            return ret;
        }

        public static WaitValues WaitForSingleObject(SafeMemoryHandle handle) {
            var ret = NativeMethods.WaitForSingleObject(handle, 0xFFFFFFFF);
            if (ret == WaitValues.Failed)
                throw new Win32Exception();
            return ret;
        }

    }
}