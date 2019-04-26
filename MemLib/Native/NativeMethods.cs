using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MemLib.Native {
    public static class NativeMethods {
        #region kernel32

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(SafeMemoryHandle hObject);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(SafeMemoryHandle hProcess, IntPtr lpBaseAddress,[Out] byte[] lpBuffer, long dwSize, out long lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(SafeMemoryHandle hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, long nSize, out long lpNumberOfBytesWritten);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(SafeMemoryHandle hProcess, IntPtr lpAddress, long dwSize, MemoryAllocationFlags flAllocationType, MemoryProtectionFlags flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFreeEx(SafeMemoryHandle hProcess, IntPtr lpAddress, long dwSize, MemoryReleaseFlags dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(SafeMemoryHandle hProcess, IntPtr lpAddress, long dwSize, MemoryProtectionFlags flNewProtect, out MemoryProtectionFlags lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern long VirtualQueryEx(SafeMemoryHandle hProcess, IntPtr lpAddress, out MemoryBasicInformation lpBuffer, int dwLength);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(SafeMemoryHandle hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(SafeMemoryHandle hThread);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle CreateRemoteThread(SafeMemoryHandle hProcess, IntPtr lpThreadAttributes, ulong dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, ThreadCreationFlags dwCreationFlags, out uint lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle OpenThread(ThreadAccessFlags dwDesiredAccess, bool bInheritHandle, int dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(SafeMemoryHandle hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SuspendThread(SafeMemoryHandle hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateThread(SafeMemoryHandle hThread, int dwExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern WaitValues WaitForSingleObject(SafeMemoryHandle hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeThread(SafeMemoryHandle hThread, out IntPtr lpExitCode);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process(SafeMemoryHandle hProcess, out bool wow64Process);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process2(SafeMemoryHandle hProcess, out ImageFileMachine pProcessMachine, out ImageFileMachine pNativeMachine);

        [DllImport("kernel32.dll", SetLastError=false)]
        public static extern void GetNativeSystemInfo(out SystemInfo lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool FlushInstructionCache(SafeMemoryHandle hProcess, IntPtr lpBaseAddress, long dwSize);

        #endregion

        #region psapi

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool EnumProcessModulesEx(SafeMemoryHandle hProcess, [Out] IntPtr lphModule, int cb, out int lpcbNeeded, ListModulesFlags dwFilterFlag);

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetModuleFileNameEx(SafeMemoryHandle hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, int nSize);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetModuleInformation(SafeMemoryHandle hProcess, IntPtr hModule, [Out] out ModuleInfo lpmodinfo, int cb);

        #endregion

        #region ntdll

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(SafeMemoryHandle processHandle, ProcessInformationClass infoclass, ref ProcessBasicInformation processinfo, int length, IntPtr bytesread);

        [DllImport("ntdll.dll")]
        public static extern uint NtQueryInformationThread(SafeMemoryHandle threadHandle, uint infoclass, ref ThreadBasicInformation threadinfo, int length, IntPtr bytesread);

        #endregion

        #region dbghelp

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnDecorateSymbolName(string name, StringBuilder outputString, int maxStringLength, UnDecorateFlags flags);

        #endregion
    }
}