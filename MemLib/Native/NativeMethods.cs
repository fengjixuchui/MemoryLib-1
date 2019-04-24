using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MemLib.Native {
    public static class NativeMethods {
        #region kernel32

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,[MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,[Out] byte[] lpBuffer, long dwSize, out long lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, long nSize, out long lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, long dwSize, MemoryAllocationFlags flAllocationType, MemoryProtectionFlags flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, long dwSize, MemoryReleaseFlags dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, long dwSize, MemoryProtectionFlags flNewProtect, out MemoryProtectionFlags lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MemoryBasicInformation lpBuffer, long dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, long dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, ThreadCreationFlags dwCreationFlags, out int lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateThread(IntPtr hThread, int dwExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern WaitValues WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeThread(IntPtr hThread, out IntPtr lpExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process2(IntPtr hProcess, out ImageFileMachine pProcessMachine, out ImageFileMachine pNativeMachine);

        [DllImport("kernel32.dll", SetLastError=false)]
        public static extern void GetNativeSystemInfo(out SystemInfo lpSystemInfo);

        #endregion

        #region psapi

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, [Out] IntPtr lphModule, IntPtr cb, out IntPtr lpcbNeeded, ListModulesFlags dwFilterFlag);

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, int nSize);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, [Out] out ModuleInfo lpmodinfo, int cb);

        #endregion

        #region ntdll

        #endregion

        #region dbghelp

        [DllImport("dbghelp.dll", SetLastError=true, PreserveSig=true)]
        public static extern int UnDecorateSymbolName(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string name,
            [Out] StringBuilder outputString,
            [In] [MarshalAs(UnmanagedType.U4)] int maxStringLength,
            [In] [MarshalAs(UnmanagedType.U4)] UnDecorateFlags flags);

        #endregion
    }
}