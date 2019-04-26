using System;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    public class ImageFileHeader : RemotePointer {
        public FileHeaderMachine Machine => (FileHeaderMachine)Read<ushort>(0x00);
        public ushort NumberOfSections => Read<ushort>(0x02);
        public uint TimeDateStamp => Read<uint>(0x04);
        public uint NumberOfSymbols => Read<uint>(0x0C);
        public ushort SizeOfOptionalHeader => Read<ushort>(0x10);
        public FileHeaderCharacteristics Characteristics => (FileHeaderCharacteristics)Read<ushort>(0x12);

        public ImageFileHeader(RemoteProcess process, IntPtr address) : base(process, address) { }
    }
}