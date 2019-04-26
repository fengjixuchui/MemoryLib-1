using System;
using System.Collections.Generic;
using System.Text;
using MemLib.Memory;
using MemLib.PeHeader.Structures;

namespace MemLib.PeHeader {
    public class PeHeaderParser : RemotePointer {
        public bool Is64Bit => Read<ushort>((int)DosHeader.e_lfanew + 0x04) == 0x8664;
        public ImageDosHeader DosHeader { get; }
        public ImageNtHeader NtHeader { get; }
        public ImageExportDirectory ExportDirectory { get; }
        public ImageSectionHeaders SectionHeaders { get; }
        public IEnumerable<ExportFunction> ExportFunctions => ReadExports();

        public PeHeaderParser(RemoteProcess process, IntPtr moduleBase) : base(process, moduleBase) {
            DosHeader = new ImageDosHeader(process, moduleBase);
            NtHeader = new ImageNtHeader(process, moduleBase + (int)DosHeader.e_lfanew, Is64Bit);

            var numSections = (int)NtHeader.FileHeader.NumberOfSections;
            var secHeaderStart = DosHeader.e_lfanew + NtHeader.FileHeader.SizeOfOptionalHeader + 0x18;
            SectionHeaders = new ImageSectionHeaders(process, moduleBase + (int)secHeaderStart, numSections);

            var exportDirVa = NtHeader.OptionalHeader.DataDirectory[0].VirtualAddress;
            if (exportDirVa != 0) {
                ExportDirectory = new ImageExportDirectory(process, moduleBase + (int)exportDirVa);
            }
        }
        
        private IEnumerable<ExportFunction> ReadExports() {
            if(ExportDirectory == null || ExportDirectory.NumberOfFunctions == 0)
                yield break;
            var expFuncs = new ExportFunction[ExportDirectory.NumberOfFunctions];

            var funcOffset = (int)ExportDirectory.AddressOfFunctions;
            var ordOffset = (int)ExportDirectory.AddressOfNameOrdinals;
            var nameOffset = (int)ExportDirectory.AddressOfNames;
            var exportBase = (int)ExportDirectory.Base;
            var numberOfNames = (int)ExportDirectory.NumberOfNames;

            for (var i = 0; i < expFuncs.Length; i++) {
                var ordinal = i + exportBase;
                var address = Read<int>(funcOffset + sizeof(uint) * i);
                expFuncs[i] = new ExportFunction(string.Empty, address, ordinal);
            }
            for (var i = 0; i < numberOfNames; i++) {
                var ordinalIndex = Read<ushort>(ordOffset + sizeof(ushort) * i);
                var tmp = expFuncs[ordinalIndex];
                if(tmp.RelativeAddress <= 0) continue;
                var nameAddr = Read<int>(nameOffset + sizeof(uint) * i);
                var name = nameAddr == 0 ? string.Empty : ReadString(nameAddr, Encoding.UTF8);
                
                expFuncs[ordinalIndex] = new ExportFunction(name, tmp.RelativeAddress, tmp.Ordinal);
                if (expFuncs[ordinalIndex].RelativeAddress != 0)
                    yield return expFuncs[ordinalIndex];
            }
        }

    }
}