using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemLib.Assembly.CallingConventions;
using MemLib.Internals;
using MemLib.Memory;

namespace MemLib.Assembly {
    public sealed class AssemblyManager : IDisposable {
        private readonly RemoteProcess m_Process;
        private Assembler m_Assembler;
        public Assembler Assembler => m_Assembler ?? (m_Assembler = new Assembler(m_Process.Is64Bit));

        internal AssemblyManager(RemoteProcess process) {
            m_Process = process;
        }
        
        #region BeginTransaction

        public AssemblyTransaction BeginTransaction(IntPtr address, bool autoExecute = true) {
            return new AssemblyTransaction(m_Process, address, autoExecute);
        }

        public AssemblyTransaction BeginTransaction(bool autoExecute = true) {
            return new AssemblyTransaction(m_Process, autoExecute);
        }

        #endregion

        #region Execute

        public T Execute<T>(IntPtr address) {
            var thread = m_Process.Threads.CreateAndJoin(address);
            return thread.GetExitCode<T>();
        }
        
        public IntPtr Execute(IntPtr address) {
            return Execute<IntPtr>(address);
        }
        
        public IntPtr Execute(IntPtr address, params dynamic[] parameters) {
            return Execute<IntPtr>(address, CallingConvention.Default, parameters);
        }

        public T Execute<T>(IntPtr address, params dynamic[] parameters) {
            return Execute<T>(address, CallingConvention.Default, parameters);
        }

        public T Execute<T>(IntPtr address, CallingConvention callingConvention, params dynamic[] parameters) {
            var marshalledParameters = parameters.Select(p => MarshalValue.Marshal(m_Process, p)).Cast<IMarshalledValue>().ToArray();

            var calling = callingConvention == CallingConvention.Default ?
                CallingConventionSelector.Get(m_Process.Is64Bit ? CallingConvention.FastCall64 : CallingConvention.StdCall) :
                CallingConventionSelector.Get(m_Process.Is64Bit ? CallingConvention.FastCall64 : callingConvention);

            AssemblyTransaction t;
            using (t = BeginTransaction()) {
                t.Append(calling.FormatCall(address, marshalledParameters.Select(p => p.Reference).ToArray()));
            }
            
            foreach (var parameter in marshalledParameters) {
                parameter.Dispose();
            }

            return t.GetExitCode<T>();
        }

        public IntPtr Execute(IntPtr address, CallingConvention callingConvention, params dynamic[] parameters) {
            return Execute<IntPtr>(address, callingConvention, parameters);
        }
        
        #endregion

        #region ExecuteAsync

        public Task<T> ExecuteAsync<T>(IntPtr address) {
            return Task.Run(() => Execute<T>(address));
        }
        
        public Task<IntPtr> ExecuteAsync(IntPtr address) {
            return ExecuteAsync<IntPtr>(address);
        }
        
        public Task<T> ExecuteAsync<T>(IntPtr address, params dynamic[] parameters) {
            return Task.Run(() => Execute<T>(address, CallingConvention.Default, parameters));
        }

        public Task<T> ExecuteAsync<T>(IntPtr address, CallingConvention callingConvention, params dynamic[] parameters) {
            return Task.Run(() => Execute<T>(address, callingConvention, parameters));
        }

        public Task<IntPtr> ExecuteAsync(IntPtr address, params dynamic[] parameters) {
            return ExecuteAsync<IntPtr>(address, CallingConvention.Default, parameters);
        }

        public Task<IntPtr> ExecuteAsync(IntPtr address, CallingConvention callingConvention, params dynamic[] parameters) {
            return ExecuteAsync<IntPtr>(address, callingConvention, parameters);
        }

        #endregion

        #region Inject

        public void Inject(string asm, IntPtr address) {
            var data = Assembler.Assemble(asm, address);
            if(data == null)
                throw new InvalidOperationException("The assembler returned nothing");
            m_Process.Write(address, data);
        }
        
        public void Inject(IEnumerable<string> asm, IntPtr address) {
            Inject(string.Join("\n", asm), address);
        }
        
        public RemoteAllocation Inject(string asm) {
            var code = Assembler.Assemble(asm);
            if (code == null)
                throw new InvalidOperationException("The assembler returned nothing");
            var memory = m_Process.Memory.Allocate(code.Length);
            Inject(asm, memory.BaseAddress);

            return memory;
        }
        
        public RemoteAllocation Inject(IEnumerable<string> asm) {
            return Inject(string.Join("\n", asm));
        }

        #endregion

        #region InjectAndExecute

        public T InjectAndExecute<T>(string asm, IntPtr address) {
            Inject(asm, address);
            return Execute<T>(address);
        }
        
        public IntPtr InjectAndExecute(string asm, IntPtr address) {
            return InjectAndExecute<IntPtr>(asm, address);
        }
        
        public T InjectAndExecute<T>(IEnumerable<string> asm, IntPtr address) {
            return InjectAndExecute<T>(string.Join("\n", asm), address);
        }
        
        public IntPtr InjectAndExecute(IEnumerable<string> asm, IntPtr address) {
            return InjectAndExecute<IntPtr>(asm, address);
        }
        
        public T InjectAndExecute<T>(string asm) {
            using (var memory = Inject(asm)) {
                return Execute<T>(memory.BaseAddress);
            }
        }
        
        public IntPtr InjectAndExecute(string asm) {
            return InjectAndExecute<IntPtr>(asm);
        }
        
        public T InjectAndExecute<T>(IEnumerable<string> asm) {
            return InjectAndExecute<T>(string.Join("\n", asm));
        }

        public IntPtr InjectAndExecute(IEnumerable<string> asm) {
            return InjectAndExecute<IntPtr>(asm);
        }

        #endregion

        #region InjectAndExecuteAsync

        public Task<T> InjectAndExecuteAsync<T>(string asm, IntPtr address) {
            return Task.Run(() => InjectAndExecute<T>(asm, address));
        }


        public Task<IntPtr> InjectAndExecuteAsync(string asm, IntPtr address) {
            return InjectAndExecuteAsync<IntPtr>(asm, address);
        }


        public Task<T> InjectAndExecuteAsync<T>(IEnumerable<string> asm, IntPtr address) {
            return Task.Run(() => InjectAndExecute<T>(asm, address));
        }


        public Task<IntPtr> InjectAndExecuteAsync(IEnumerable<string> asm, IntPtr address) {
            return InjectAndExecuteAsync<IntPtr>(asm, address);
        }


        public Task<T> InjectAndExecuteAsync<T>(string asm) {
            return Task.Run(() => InjectAndExecute<T>(asm));
        }


        public Task<IntPtr> InjectAndExecuteAsync(string asm) {
            return InjectAndExecuteAsync<IntPtr>(asm);
        }


        public Task<T> InjectAndExecuteAsync<T>(IEnumerable<string> asm) {
            return Task.Run(() => InjectAndExecute<T>(asm));
        }


        public Task<IntPtr> InjectAndExecuteAsync(IEnumerable<string> asm) {
            return InjectAndExecuteAsync<IntPtr>(asm);
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