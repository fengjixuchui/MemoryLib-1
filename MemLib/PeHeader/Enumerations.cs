using System;

namespace MemLib.PeHeader {
    [Flags]
    public enum FileHeaderCharacteristics : ushort {
        ImageFileRelocsStripped = 0x01,
        ImageFileExecutableImage = 0x02,
        ImageFileLineNumsStripped = 0x04,
        ImageFileLocalSymsStripped = 0x08,
        ImageFileAggresiveWsTrim = 0x10,
        ImageFileLargeAddressAware = 0x20,
        ImageFileBytesReversedLo = 0x80,
        ImageFile32BitMachine = 0x100,
        ImageFileDebugStripped = 0x200,
        ImageFileRemovableRunFromSwap = 0x400,
        ImageFileNetRunFromSwap = 0x800,
        ImageFileSystem = 0x1000,
        ImageFileDll = 0x2000,
        ImageFileUpSystemOnly = 0x4000,
        ImageFileBytesReversedHi = 0x8000
    }

    [Flags]
    public enum FileHeaderMachine : ushort {
        ImageFileMachineUnknown = 0x0,
        ImageFileMachineI386 = 0x14C,
        ImageFileMachineI860 = 0x14D,
        ImageFileMachineR3000 = 0x162,
        ImageFileMachineR4000 = 0x166,
        ImageFileMachineR10000 = 0x168,
        ImageFileMachineWcemipsv2 = 0x169,
        ImageFileMachineOldalpha = 0x183,
        ImageFileMachineAlpha = 0x184,
        ImageFileMachineSh3 = 0x1A2,
        ImageFileMachineSh3Dsp = 0x1A3,
        ImageFileMachineSh3E = 0x1A4,
        ImageFileMachineSh4 = 0x1A6,
        ImageFileMachineSh5 = 0x1A8,
        ImageFileMachineArm = 0x1C0,
        ImageFileMachineThumb = 0x1C2,
        ImageFileMachineAm33 = 0x1D3,
        ImageFileMachinePowerpc = 0x1F0,
        ImageFileMachinePowerpcfp = 0x1F1,
        ImageFileMachineIa64 = 0x200,
        ImageFileMachineMips16 = 0x266,
        ImageFileMachineM68K = 0x268,
        ImageFileMachineAlpha64 = 0x284,
        ImageFileMachineMipsfpu = 0x366,
        ImageFileMachineMipsfpu16 = 0x466,
        ImageFileMachineAxp64 = ImageFileMachineAlpha64,
        ImageFileMachineTricore = 0x520,
        ImageFileMachineCef = 0xCEF,
        ImageFileMachineEbc = 0xEBC,
        ImageFileMachineAmd64 = 0x8664,
        ImageFileMachineM32R = 0x9041,
        ImageFileMachineCee = 0xC0EE
    }

    [Flags]
    public enum OptionalHeaderDllCharacteristics : ushort {
        ImageDllcharacteristicsDynamicBase = 0x40,
        ImageDllcharacteristicsForceIntegrity = 0x80,
        ImageDllcharacteristicsNxCompat = 0x100,
        ImageDllcharacteristicsNoIsolation = 0x200,
        ImageDllcharacteristicsNoSeh = 0x400,
        ImageDllcharacteristicsNoBind = 0x401,
        ImageDllcharacteristicsWdmDriver = 0x2000,
        ImageDllcharacteristicsTerminalServerAware = 0x8000
    }

    [Flags]
    public enum OptionalHeaderMagic : ushort {
        ImageNtOptionalHdr32Magic = 0x10B,
        ImageNtOptionalHdr64Magic = 0x20B,
        ImageRomOptionalHdrMagic = 0x107
    }

    [Flags]
    public enum OptionalHeaderSubsystem : ushort {
        ImageSubsystemNative = 1,
        ImageSubsystemWindowsGui = 2,
        ImageSubsystemWindowsCui = 3,
        ImageSubsystemOs2Cui = 5,
        ImageSubsystemPosixCui = 7,
        ImageSubsystemWindowsCeGui = 9,
        ImageSubsystemEfiApplication = 10,
        ImageSubsystemEfiBootServiceDriver = 11,
        ImageSubsystemEfiRuntimeDriver = 12,
        ImageSubsystemEfiRom = 13,
        ImageSubsystemXbox = 14,
        ImageSubsystemWindowsBootApplication = 16
    }
}