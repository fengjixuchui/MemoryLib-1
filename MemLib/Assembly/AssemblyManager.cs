using System;
using System.Collections.Generic;
using MemLib.Memory;

namespace MemLib.Assembly {
    public sealed class AssemblyManager : IDisposable {
        private readonly RemoteProcess m_Process;
        private Assembler m_Assembler;
        public Assembler Assembler => m_Assembler ?? (m_Assembler = new Assembler(m_Process.Is64Bit));

        internal AssemblyManager(RemoteProcess process) {
            m_Process = process;
        }
        

        #region Inject

        public void Inject(string asm, IntPtr address) {
            var data = Assembler.Assemble(asm, address);
            if(data == null)
                throw new AssemblerException(Assembler.LastError);
            m_Process.Write(address, data);
        }

        public void Inject(IEnumerable<string> asm, IntPtr address) {
            Inject(string.Join("\n", asm), address);
        }
        
        public RemoteAllocation Inject(string asm) {
            var code = Assembler.Assemble(asm);
            if (code == null)
                throw new AssemblerException(Assembler.LastError);
            var memory = m_Process.Memory.Allocate(code.Length);
            Inject(asm, memory.BaseAddress);

            return memory;
        }
        
        public RemoteAllocation Inject(IEnumerable<string> asm) {
            return Inject(string.Join("\n", asm));
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose() {
            ((IDisposable)m_Assembler)?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~AssemblyManager() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}