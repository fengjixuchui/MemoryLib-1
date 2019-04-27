using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MemLibNative.Keystone;

namespace MemLib.Assembly {
    public sealed class Assembler : IDisposable {
        private readonly KeystoneEngine m_Keystone;
        
        public event EventHandler<string> OnError;

        public string LastError => m_Keystone.GetLastErrorString();
        public bool ThrowOnError { get; set; }
        public AssemblerSyntax Syntax {
            get => (AssemblerSyntax)m_Keystone.Syntax;
            set => m_Keystone.Syntax = (KsSyntax)value;
        }
        
        public Assembler() : this(Environment.Is64BitProcess) { }

        public Assembler(bool use64Bit) {
            m_Keystone = new KeystoneEngine(use64Bit ? KsMode.Mode64 : KsMode.Mode32) {
                Syntax = KsSyntax.Nasm
            };
        }

        #region Assemble

        public bool Assemble(string source, long address, out byte[] data) => Assemble(source, new IntPtr(address), out data);
        public bool Assemble(string source, IntPtr address, out byte[] data) {
            data = Assemble(source, address);
            return data != null;
        }
        
        public bool Assemble(string[] source, long address, out byte[] data) => Assemble(source, new IntPtr(address), out data);
        public bool Assemble(string[] source, IntPtr address, out byte[] data) {
            data = Assemble(source, address);
            return data != null;
        }

        public byte[] Assemble(string source) => Assemble(source, 0);
        public byte[] Assemble(IEnumerable<string> source) => Assemble(source, 0);
        public byte[] Assemble(IEnumerable<string> source, IntPtr address) => Assemble(string.Join("\n", source), address);
        public byte[] Assemble(IEnumerable<string> source, long address) => Assemble(string.Join("\n", source), address);

        [DebuggerStepThrough]
        public byte[] Assemble(string source, IntPtr address) {
            if (m_Keystone.Assemble(source, address, out var data)) return data;
            if(ThrowOnError)
                throw new AssemblerException(LastError);
            OnError?.Invoke(this, LastError);
            return null;
        }

        [DebuggerStepThrough]
        public byte[] Assemble(string source, long address) {
            if (m_Keystone.Assemble(source, address, out var data)) return data;
            if(ThrowOnError)
                throw new AssemblerException(LastError);
            OnError?.Invoke(this, LastError);
            return null;
        }

        #endregion

        #region AssembleFile

        public bool AssembleFile(string path, out byte[] data) {
            return AssembleFile(path, 0, out data);
        }

        public bool AssembleFile(string path, IntPtr address, out byte[] data) {
            data = AssembleFile(path, address);
            return data != null;
        }

        public bool AssembleFile(string path, long address, out byte[] data) {
            data = AssembleFile(path, address);
            return data != null;
        }

        public byte[] AssembleFile(string path) => AssembleFile(path, 0);

        [DebuggerStepThrough]
        public byte[] AssembleFile(string path, IntPtr address) {
            if(!File.Exists(path))
                throw new FileNotFoundException("File not found.", path);
            return Assemble(File.ReadAllText(path), address);
        }

        [DebuggerStepThrough]
        public byte[] AssembleFile(string path, long address) {
            if(!File.Exists(path))
                throw new FileNotFoundException("File not found.", path);
            return Assemble(File.ReadAllText(path), address);
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose() {
            m_Keystone?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~Assembler() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}