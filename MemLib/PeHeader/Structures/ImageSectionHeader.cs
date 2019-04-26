using System;
using System.Text;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    public class ImageSectionHeader : RemotePointer {
        public string Name => ReadString(0x00, Encoding.UTF8, 8);
        public uint PhysicalAddress => Read<uint>(0x08);
        public uint VirtualSize => PhysicalAddress;
        public uint VirtualAddress => Read<uint>(0x0C);
        public uint SizeOfRawData => Read<uint>(0x10);
        public uint PointerToRawData => Read<uint>(0x14);
        public uint PointerToRelocations => Read<uint>(0x18);
        public uint PointerToLinenumbers => Read<uint>(0x1C);
        public ushort NumberOfRelocations => Read<ushort>(0x20);
        public ushort NumberOfLinenumbers => Read<ushort>(0x22);
        public uint Characteristics => Read<uint>(0x24);

        public ImageSectionHeader(RemoteProcess proc, IntPtr address) : base(proc, address) { }
    }
}