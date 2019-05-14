using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MemLib.Internals;
using MemLib.Native;

namespace MemLib.Threading {
    public sealed class RemoteThread : IDisposable, IEquatable<RemoteThread> {
        private readonly RemoteProcess m_Process;
        private readonly IMarshalledValue m_Parameter;
        private readonly Task m_ParameterCleaner;

        public ProcessThread Native { get; private set; }
        public SafeMemoryHandle Handle { get; }
        public int Id { get; }
        public bool IsMainThread => Equals(m_Process.Threads.MainThread);
        public bool IsAlive => !IsTerminated;
        public bool IsSuspended {
            get {
                Refresh();
                return Native != null && Native.ThreadState == ThreadState.Wait && Native.WaitReason == ThreadWaitReason.Suspended;
            }
        }
        public bool IsTerminated {
            get {
                Refresh();
                return Native == null;
            }
        }

        [DebuggerStepThrough]
        internal RemoteThread(RemoteProcess process, ProcessThread thread) {
            m_Process = process;
            Native = thread ?? throw new ArgumentNullException(nameof(thread));
            Id = thread.Id;
            Handle = ThreadHelper.OpenThread(ThreadAccessFlags.AllAccess, thread.Id);
        }

        [DebuggerStepThrough]
        internal RemoteThread(RemoteProcess process, ProcessThread thread, IMarshalledValue parameter = null) : this(process, thread) {
            m_Parameter = parameter;
            m_ParameterCleaner = new Task(() => {
                Join();
                m_Parameter?.Dispose();
            });
        }
        
        public override string ToString() => $"ThreadId=0x{Id:X} IsAlive={IsAlive} IsMainThread={IsMainThread}";

        public void Refresh() {
            if (Native == null)
                return;
            m_Process.Native.Refresh();
            Native = m_Process.Threads.NativeThreads.FirstOrDefault(t => t.Id == Native.Id);
        }

        public void Join() {
            ThreadHelper.WaitForSingleObject(Handle);
        }

        public WaitValues Join(TimeSpan time) {
            return ThreadHelper.WaitForSingleObject(Handle, time);
        }

        public void Resume() {
            if (!IsAlive) return;
            ThreadHelper.ResumeThread(Handle);
            if(m_Parameter != null && !m_ParameterCleaner.IsCompleted)
                m_ParameterCleaner.Start();
        }

        public FrozenThread Suspend() {
            if (!IsAlive) return null;
            ThreadHelper.SuspendThread(Handle);
            return new FrozenThread(this);
        }

        public void Terminate(int exitCode = 0) {
            if(IsAlive)
                ThreadHelper.TerminateThread(Handle, exitCode);
        }

        public T GetExitCode<T>() {
            var ret = ThreadHelper.GetExitCodeThread(Handle);
            return ret.HasValue ? MarshalType<T>.PtrToObject(m_Process, ret.Value) : default;
        }

        #region IDisposable

        public void Dispose() {
            if (!Handle.IsClosed)
                Handle.Close();
            if (m_Parameter != null && m_Process.IsRunning) {
                m_ParameterCleaner.Dispose();
                m_Parameter.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~RemoteThread() {
            Dispose();
        }

        #endregion

        #region Equality members

        public bool Equals(RemoteThread other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && m_Process.Equals(other.m_Process);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RemoteThread) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (m_Process.GetHashCode() * 397) ^ Id;
            }
        }

        public static bool operator ==(RemoteThread left, RemoteThread right) {
            return Equals(left, right);
        }

        public static bool operator !=(RemoteThread left, RemoteThread right) {
            return !Equals(left, right);
        }

        #endregion
    }

}