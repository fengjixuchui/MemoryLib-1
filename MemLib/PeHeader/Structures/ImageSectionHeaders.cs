using System;
using System.Collections.Generic;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    public class ImageSectionHeaders : RemotePointer {
        private readonly int m_SectionsMax;

        public IEnumerable<ImageSectionHeader> All {
            get {
                for (var i = 0; i < m_SectionsMax; i++)
                    yield return GetSectionHeader(i);
            }
        }

        public ImageSectionHeader this[int index] => GetSectionHeader(index);


        public ImageSectionHeaders(RemoteProcess process, IntPtr address, int numSections) : base(process, address) {
            m_SectionsMax = numSections;
        }

        private ImageSectionHeader GetSectionHeader(int index) {
            var address = BaseAddress + index * 0x28;
            return new ImageSectionHeader(m_Process, address);
        }
    }
}