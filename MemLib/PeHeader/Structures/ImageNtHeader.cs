using System;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    public class ImageNtHeader : RemotePointer {
        public uint Signature => Read<uint>(0x00);
        public readonly ImageFileHeader FileHeader;
        public readonly ImageOptionalHeader OptionalHeader;

        public ImageNtHeader(RemoteProcess process, IntPtr address, bool is64Bit) : base(process, address) {
            FileHeader = new ImageFileHeader(process, address + 0x04);
            OptionalHeader = new ImageOptionalHeader(process, address + 0x18, is64Bit);
        }
    }
}