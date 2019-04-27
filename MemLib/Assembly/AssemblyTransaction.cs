using System;
using System.Collections.Generic;
using System.Text;
using MemLib.Internals;

namespace MemLib.Assembly {
    public sealed class AssemblyTransaction : IDisposable {
        private readonly RemoteProcess m_Process;
        private readonly StringBuilder m_Code;
        private IntPtr m_ExitCode;

        public IntPtr BaseAddress { get; }
        public bool IsAutoExecuted { get; set; }

        public AssemblyTransaction(RemoteProcess process, bool autoExecute) : this(process, IntPtr.Zero, autoExecute){}
        public AssemblyTransaction(RemoteProcess process, IntPtr address, bool autoExecute) {
            m_Process = process;
            m_Code = new StringBuilder();
            IsAutoExecuted = autoExecute;
            BaseAddress = address;
        }

        public override string ToString() => m_Code.ToString();

        #region GetExitCode

        public T GetExitCode<T>() {
            return MarshalType<T>.PtrToObject(m_Process, m_ExitCode);
        }

        #endregion

        #region Assemble

        public bool Assemble(out byte[] data) {
            if (m_Code.Length > 0) {
                return m_Process.Assembly.Assembler.Assemble(m_Code.ToString(), BaseAddress, out data);
            }
            data = null;
            return false;
        }

        public byte[] Assemble() {
            return Assemble(out var data) ? data : null;
        }

        #endregion

        #region Clear

        public void Clear() {
            m_Code.Clear();
        }

        #endregion

        #region Append

        public void Append(string asm) {
            m_Code.Append(asm);
        }

        public void AppendLine(string asm, params object[] args) {
            m_Code.AppendLine(string.Format(asm, args));
        }

        public void AppendLines(IEnumerable<string> asmStrings) {
            if(asmStrings == null) return;
            foreach (var asmString in asmStrings) {
                AppendLine(asmString);
            }
        }

        #endregion

        #region Prepend

        public void Prepend(string asm, params object[] args) {
            Insert(0, asm, args);
        }

        public void PrependLine(string asm, params object[] args) {
            InsertLine(0, asm, args);
        }

        #endregion

        #region Insert

        public void InsertLine(int index, string asm, params object[] args) {
            if (!asm.EndsWith("\n")) asm += "\n";
            m_Code.Insert(index, string.Format(asm, args));
        }

        public void Insert(int index, string asm, params object[] args) {
            m_Code.Insert(index, string.Format(asm, args));
        }

        #endregion

        #region IDisposable

        public void Dispose() {
            if (BaseAddress != IntPtr.Zero) {
                if (IsAutoExecuted) {
                    m_ExitCode = m_Process.Assembly.InjectAndExecute<IntPtr>(m_Code.ToString(), BaseAddress);
                } else {
                    m_Process.Assembly.Inject(m_Code.ToString(), BaseAddress);
                }
            }
            if (BaseAddress == IntPtr.Zero && IsAutoExecuted) {
                m_ExitCode = m_Process.Assembly.InjectAndExecute<IntPtr>(m_Code.ToString());
            }
        }

        #endregion
    }
}