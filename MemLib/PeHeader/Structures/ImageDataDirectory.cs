using System;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    public class ImageDataDirectory : RemotePointer {
        public uint VirtualAddress => Read<uint>(0x00);
        public uint Size => Read<uint>(0x04);

        public ImageDataDirectory(RemoteProcess process, IntPtr address) : base(process, address) { }
    }
}