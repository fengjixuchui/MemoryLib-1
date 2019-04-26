using System;
using MemLib.Memory;
using MemLib.Native;

namespace MemLib.Modules {
    public class RemoteFunction : RemotePointer {
        public string Name { get; }
        public bool IsMangled => Name != UndecoratedName;
        public string UndecoratedName => ModuleHelper.UnDecorateSymbolName(Name, UnDecorateFlags.NameOnly);

        public RemoteFunction(RemoteProcess process, IntPtr address, string name) : base(process, address) {
            Name = name;
        }

        public override string ToString() => $"Address = 0x{BaseAddress.ToInt64():X8} Name = {Name}";
    }
}