using System;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    public class ImageExportDirectory : RemotePointer {
        public uint Characteristics => Read<uint>(0x00);
        public uint TimeDateStamp => Read<uint>(0x04);
        public ushort MajorVersion => Read<ushort>(0x08);
        public ushort MinorVersion => Read<ushort>(0x0A);
        public uint Name => Read<uint>(0x0C);
        public uint Base => Read<uint>(0x10);
        public uint NumberOfFunctions => Read<uint>(0x14);
        public uint NumberOfNames => Read<uint>(0x18);
        public uint AddressOfFunctions => Read<uint>(0x1C);
        public uint AddressOfNames => Read<uint>(0x20);
        public uint AddressOfNameOrdinals => Read<uint>(0x24);

        public ImageExportDirectory(RemoteProcess process, IntPtr address) : base(process, address) { }
    }
}