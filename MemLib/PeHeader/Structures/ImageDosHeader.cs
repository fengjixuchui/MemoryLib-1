using System;
using System.Diagnostics.CodeAnalysis;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ImageDosHeader : RemotePointer {
        public ushort e_magic => Read<ushort>(0x00);
        public ushort e_cblp => Read<ushort>(0x02);
        public ushort e_cp => Read<ushort>(0x04);
        public ushort e_crlc => Read<ushort>(0x06);
        public ushort e_cparhdr => Read<ushort>(0x08);
        public ushort e_minalloc => Read<ushort>(0x0A);
        public ushort e_maxalloc => Read<ushort>(0x0C);
        public ushort e_ss => Read<ushort>(0x0E);
        public ushort e_sp => Read<ushort>(0x10);
        public ushort e_csum => Read<ushort>(0x12);
        public ushort e_ip => Read<ushort>(0x14);
        public ushort e_cs => Read<ushort>(0x16);
        public ushort e_lfarlc => Read<ushort>(0x18);
        public ushort e_ovno => Read<ushort>(0x1A);
        public ushort[] e_res => Read<ushort>(0x1C, 4);
        public ushort e_oemid => Read<ushort>(0x24);
        public ushort e_oeminfo => Read<ushort>(0x26);
        public ushort[] e_res2 => Read<ushort>(0x28, 10);
        public uint e_lfanew => Read<uint>(0x3C);

        public ImageDosHeader(RemoteProcess process, IntPtr address) : base(process, address) { }
    }
}