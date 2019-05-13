using System;

namespace MemLib.Native {
    #region Flags
    
    [Flags]
    public enum ProcessAccessFlags {
        AllAccess = 0x001F0FFF,
        CreateProcess = 0x0080,
        CreateThread = 0x0002,
        DupHandle = 0x0040,
        QueryInformation = 0x0400,
        QueryLimitedInformation = 0x1000,
        SetInformation = 0x0200,
        SetQuota = 0x0100,
        SuspendResume = 0x0800,
        Terminate = 0x0001,
        VmOperation = 0x0008,
        VmRead = 0x0010,
        VmWrite = 0x0020,
        Synchronize = 0x00100000
    }

    [Flags]
    public enum MemoryAllocationFlags {
        Commit = 0x00001000,
        Reserve = 0x00002000,
        Reset = 0x00080000,
        ResetUndo = 0x1000000,
        LargePages = 0x20000000,
        Physical = 0x00400000,
        TopDown = 0x00100000
    }

    [Flags]
    public enum MemoryProtectionFlags {
        ZeroAccess = 0x0,
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        Guard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400
    }

    [Flags]
    public enum MemoryReleaseFlags {
        Decommit = 0x4000,
        Release = 0x8000
    }

    [Flags]
    public enum MemoryStateFlags {
        Commit = 0x1000,
        Free = 0x10000,
        Reserve = 0x2000
    }
    
    [Flags]
    public enum MemoryTypeFlags {
        None = 0x0,
        Image = 0x1000000,
        Mapped = 0x40000,
        Private = 0x20000
    }

    [Flags]
    public enum ThreadAccessFlags {
        Synchronize = 0x00100000,
        AllAccess = 0x001F0FFF,
        DirectImpersonation = 0x0200,
        GetContext = 0x0008,
        Impersonate = 0x0100,
        QueryInformation = 0x0040,
        QueryLimitedInformation = 0x0800,
        SetContext = 0x0010,
        SetInformation = 0x0020,
        SetLimitedInformation = 0x0400,
        SetThreadToken = 0x0080,
        SuspendResume = 0x0002,
        Terminate = 0x0001
    }

    [Flags]
    public enum ThreadContextFlags {
        Intel386 = 0x10000,
        Intel486 = 0x10000,
        Control = Intel386 | 0x01,
        Integer = Intel386 | 0x02,
        Segments = Intel386 | 0x04,
        FloatingPoint = Intel386 | 0x08,
        DebugRegisters = Intel386 | 0x10,
        ExtendedRegisters = Intel386 | 0x20,
        Full = Control | Integer | Segments,
        All = Control | Integer | Segments | FloatingPoint | DebugRegisters | ExtendedRegisters
    }

    [Flags]
    public enum ThreadCreationFlags {
        Run = 0x0,
        Suspended = 0x04,
        StackSizeParamIsAReservation = 0x10000
    }

    [Flags]
    public enum ListModulesFlags : uint {
        ListModulesDefault = 0x0,
        ListModules32Bit = 0x01,
        ListModules64Bit = 0x02,
        ListModulesAll = ListModules32Bit | ListModules64Bit
    }

    [Flags]
    public enum UnDecorateFlags : uint {
        Complete = 0x0000,
        NoLeadingUnderscores = 0x0001,
        NoMsKeywords = 0x0002,
        NoFunctionReturns = 0x0004,
        NoAllocationModel = 0x0008,
        NoAllocationLanguage = 0x0010,
        NoMsThisType = 0x0020,
        NoCvThisType = 0x0040,
        NoThisType = 0x0060,
        NoAccessSpecifiers = 0x0080,
        NoThrowSignatures = 0x0100,
        NoMemberType = 0x0200,
        NoReturnUdtModel = 0x0400,
        // ReSharper disable once InconsistentNaming
        _32BitDecode = 0x0800,
        NameOnly = 0x1000,
        NoArguments = 0x2000,
        NoSpecialSyms = 0x4000
    }

    #endregion

    #region Enums

    
    public enum WaitValues : uint {
        Abandoned = 0x80,
        Signaled = 0x0,
        Timeout = 0x102,
        Failed = 0xFFFFFFFF
    }

    public enum ProcessInformationClass {
        ProcessBasicInformation = 0x0,
        ProcessDebugPort = 0x7,
        ProcessWow64Information = 0x1A,
        ProcessImageFileName = 0x1B
    }

    public enum ImageFileMachine : ushort {
        Unknown = 0,
        TargetHost = 0x0001,
        I386 = 0x014c,
        R3000 = 0x0162,
        R4000 = 0x0166,
        R10000 = 0x0168,
        Wcemipsv2 = 0x0169,
        Alpha = 0x0184,
        Sh3 = 0x01a2,
        Sh3Dsp = 0x01a3,
        Sh3E = 0x01a4,
        Sh4 = 0x01a6,
        Sh5 = 0x01a8,
        Arm = 0x01c0,
        Thumb = 0x01c2,
        Armnt = 0x01c4,
        Am33 = 0x01d3,
        Powerpc = 0x01F0,
        Powerpcfp = 0x01f1,
        Ia64 = 0x0200,
        Mips16 = 0x0266,
        Alpha64 = 0x0284,
        Mipsfpu = 0x0366,
        Mipsfpu16 = 0x0466,
        Axp64 = Alpha64,
        Tricore = 0x0520,
        Cef = 0x0CEF,
        Ebc = 0x0EBC,
        Amd64 = 0x8664,
        M32R = 0x9041,
        Arm64 = 0xAA64,
        Cee = 0xC0EE
    }

    #endregion
}