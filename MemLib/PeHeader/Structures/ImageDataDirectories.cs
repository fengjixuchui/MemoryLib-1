using System;
using System.Collections.Generic;
using MemLib.Memory;

namespace MemLib.PeHeader.Structures {
    public class ImageDataDirectories : RemotePointer {

        public IEnumerable<ImageDataDirectory> All {
            get {
                for (var i = 0; i < 16; i++)
                    yield return GetDataDirectory(i);
            }
        }

        public ImageDataDirectory this[int index] => GetDataDirectory(index);

        public ImageDataDirectories(RemoteProcess process, IntPtr address) :
            base(process, address) {}

        private ImageDataDirectory GetDataDirectory(int index) {
            var address = BaseAddress + index * 8;
            return new ImageDataDirectory(m_Process, address);
        }
    }
}