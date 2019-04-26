using System;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    public class ImageOptionalHeader : RemotePointer {
        private readonly bool _is64Bit;

        public OptionalHeaderMagic Magic => (OptionalHeaderMagic)Read<ushort>(0x00);
        public byte MajorLinkerVersion => Read<byte>(0x02);
        public byte MinorLinkerVersion => Read<byte>(0x03);
        public uint SizeOfCode => Read<uint>(0x04);
        public uint SizeOfInitializedData => Read<uint>(0x08);
        public uint SizeOfUninitializedData => Read<uint>(0x0C);
        public uint AddressOfEntryPoint => Read<uint>(0x10);
        public uint BaseOfCode => Read<uint>(0x14);
        public uint BaseOfData => _is64Bit ? 0 : Read<uint>(0x18);
        public ulong ImageBase => _is64Bit ? Read<ulong>(0x18) : Read<uint>(0x1C);
        public uint SectionAlignment => Read<uint>(0x20);
        public uint FileAlignment => Read<uint>(0x24);
        public ushort MajorOperatingSystemVersion => Read<ushort>(0x28);
        public ushort MinorOperatingSystemVersion => Read<ushort>(0x2A);
        public ushort MajorImageVersion => Read<ushort>(0x2C);
        public ushort MinorImageVersion => Read<ushort>(0x2E);
        public ushort MajorSubsystemVersion => Read<ushort>(0x30);
        public ushort MinorSubsystemVersion => Read<ushort>(0x32);
        public uint Win32VersionValue => Read<uint>(0x34);
        public uint SizeOfImage => Read<uint>(0x38);
        public uint SizeOfHeaders => Read<uint>(0x3C);
        public uint CheckSum => Read<uint>(0x40);
        public OptionalHeaderSubsystem Subsystem => (OptionalHeaderSubsystem)Read<ushort>(0x44);
        public OptionalHeaderDllCharacteristics DllCharacteristics => (OptionalHeaderDllCharacteristics)Read<ushort>(0x46);
        public ulong SizeOfStackReserve => _is64Bit ? Read<ulong>(0x48) : Read<uint>(0x48);
        public ulong SizeOfStackCommit => _is64Bit ? Read<ulong>(0x50) : Read<uint>(0x4C);
        public ulong SizeOfHeapReserve => _is64Bit ? Read<ulong>(0x58) : Read<uint>(0x50);
        public ulong SizeOfHeapCommit => _is64Bit ? Read<ulong>(0x60) : Read<uint>(0x54);
        public uint LoaderFlags => _is64Bit ? Read<uint>(0x68) : Read<uint>(0x58);
        public uint NumberOfRvaAndSizes => _is64Bit ? Read<uint>(0x6C) : Read<uint>(0x5C);

        public readonly ImageDataDirectories DataDirectory;

        public ImageOptionalHeader(RemoteProcess proc, IntPtr address, bool is64Bit) : base(proc, address) {
            _is64Bit = is64Bit;
            DataDirectory = new ImageDataDirectories(proc, address + (_is64Bit ? 0x70 : 0x60));
        }
    }
}