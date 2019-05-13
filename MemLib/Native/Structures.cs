using System;
using System.Runtime.InteropServices;

namespace MemLib.Native {
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryBasicInformation {
        public readonly IntPtr BaseAddress;
        public readonly IntPtr AllocationBase;
        public readonly MemoryProtectionFlags AllocationProtect;
        public readonly IntPtr RegionSize;
        public readonly MemoryStateFlags State;
        public readonly MemoryProtectionFlags Protect;
        public readonly MemoryTypeFlags Type;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ProcessBasicInformation {
        public readonly IntPtr ExitStatus;
        public readonly IntPtr PebBaseAddress;
        public readonly IntPtr AffinityMask;
        public readonly IntPtr BasePriority;
        public readonly UIntPtr UniqueProcessId;
        public readonly IntPtr InheritedFromUniqueProcessId;

        public int Size => Marshal.SizeOf(typeof(ProcessBasicInformation));
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ThreadBasicInformation {
        public readonly uint ExitStatus;
        public readonly IntPtr TebBaseAdress;
        public readonly IntPtr ProcessId;
        public readonly IntPtr ThreadId;
        public readonly IntPtr AffinityMask;
        public readonly uint Priority;
        public readonly uint BasePriority;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ModuleInfo {
        public readonly IntPtr lpBaseOfDll;
        public readonly IntPtr SizeOfImage;
        public readonly IntPtr EntryPoint;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemInfo {
        public ushort ProcessorArchitecture;
        public uint PageSize;
        public readonly IntPtr MinimumApplicationAddress;
        public readonly IntPtr MaximumApplicationAddress;
        public readonly IntPtr ActiveProcessorMask;
        public uint NumberOfProcessors;
        public uint ProcessorType;
        public uint AllocationGranularity;
        public ushort ProcessorLevel;
        public ushort ProcessorRevision;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point {
        public int X;
        public int Y;

        public override string ToString() {
            return $"X = {X} Y = {Y}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public int Height {
            get => Bottom - Top;
            set => Bottom = Top + value;
        }
        public int Width {
            get => Right - Left;
            set => Right = Left + value;
        }

        public override string ToString() {
            return $"Left = {Left} Top = {Top} Height = {Height} Width = {Width}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement {
        public int Length;
        public int Flags;
        public WindowStates ShowCmd;
        public Point MinPosition;
        public Point MaxPosition;
        public Rectangle NormalPosition;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FlashInfo {
        public int Size;
        public IntPtr Hwnd;
        public FlashWindowFlags Flags;
        public uint Count;
        public int Timeout;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Input {
        public Input(InputTypes type) : this() {
            Type = type;
        }

        [FieldOffset(0)]
        public InputTypes Type;
        [FieldOffset(sizeof (int))]
        public MouseInput Mouse;
        [FieldOffset(sizeof(int))]
        public KeyboardInput Keyboard;
        [FieldOffset(sizeof(int))]
        public HardwareInput Hardware;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput {
        public int DeltaX;
        public int DeltaY;
        public int MouseData;
        public MouseFlags Flags;
        public int Time;
        public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInput {
        public Keys VirtualKey;
        public short ScanCode;
        public KeyboardFlags Flags;
        public int Time;
        public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput {
        public int Message;
        public short WParamL;
        public short WParamH;
    }
}