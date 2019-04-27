using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MemLib.Internals;
using MemLib.Native;

namespace MemLib.Threads {
    public sealed class ThreadManager : IDisposable {
        private readonly RemoteProcess m_Process;

        internal IEnumerable<ProcessThread> NativeThreads {
            get {
                m_Process.Native.Refresh();
                return m_Process.Native.Threads.Cast<ProcessThread>();
            }
        }

        public IEnumerable<RemoteThread> RemoteThreads => NativeThreads.Select(t => new RemoteThread(m_Process, t));
        public RemoteThread MainThread => new RemoteThread(m_Process, NativeThreads.Aggregate((current, next) => next.StartTime < current.StartTime ? next : current));

        public RemoteThread this[int threadId] => GetThreadById(threadId);

        internal ThreadManager(RemoteProcess proc) {
            m_Process = proc;
        }

        #region GetThreadById

        public RemoteThread GetThreadById(int id) {
            var native = NativeThreads.FirstOrDefault(t => t.Id == id);
            return native == null ? null : new RemoteThread(m_Process, native);
        }

        #endregion

        #region Create

        public RemoteThread Create(IntPtr address, bool isStarted = true) {
            var tbi = ThreadHelper.NtQueryInformationThread(
                ThreadHelper.CreateRemoteThread(m_Process.Handle, address, IntPtr.Zero, ThreadCreationFlags.Suspended)
            );

            ProcessThread nativeThread;
            do
            {
                nativeThread = m_Process.Threads.NativeThreads.FirstOrDefault(t => t.Id == tbi.ThreadId.ToInt64());
            } while (nativeThread == null);

            var result = new RemoteThread(m_Process, nativeThread);

            if (isStarted)
                result.Resume();
            return result;
        }

        public RemoteThread Create(IntPtr address, dynamic parameter, bool isStarted = true) {
            var marshalledParameter = MarshalValue.Marshal(m_Process, parameter);

            ThreadBasicInformation tbi = ThreadHelper.NtQueryInformationThread(
                ThreadHelper.CreateRemoteThread(m_Process.Handle, address, marshalledParameter.Reference, ThreadCreationFlags.Suspended)
                );

            ProcessThread nativeThread;
            do
            {
                nativeThread = m_Process.Threads.NativeThreads.FirstOrDefault(t => t.Id == tbi.ThreadId.ToInt64());
            } while (nativeThread == null);

            var result = new RemoteThread(m_Process, nativeThread, marshalledParameter);

            if (isStarted)
                result.Resume();
            return result;
        }

        #endregion

        #region CreateAndJoin

        public RemoteThread CreateAndJoin(IntPtr address, dynamic parameter) {
            var ret = Create(address, parameter);
            ret.Join();
            return ret;
        }

        public RemoteThread CreateAndJoin(IntPtr address) {
            var ret = Create(address);
            ret.Join();
            return ret;
        }

        #endregion
        
        #region ResumeAll

        public void ResumeAll() {
            foreach (var thread in RemoteThreads) {
                thread.Resume();
            }
        }

        #endregion

        #region SuspendAll

        public void SuspendAll() {
            foreach (var thread in RemoteThreads) {
                thread.Suspend();
            }
        }

        #endregion
        
        #region IDisposable

        void IDisposable.Dispose() { }

        ~ThreadManager() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}