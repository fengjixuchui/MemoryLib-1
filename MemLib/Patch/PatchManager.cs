using System;
using System.Collections.Generic;
using System.Linq;

namespace MemLib.Patch {
    public sealed class PatchManager : IDisposable {
        private readonly RemoteProcess m_Process;
        private readonly HashSet<RemotePatch> m_RemotePatches;

        public List<RemotePatch> RemotePatches => m_RemotePatches.ToList();

        internal PatchManager(RemoteProcess process) {
            m_Process = process;
            m_RemotePatches = new HashSet<RemotePatch>();
        }

        #region CreatePatch

        public RemotePatch CreatePatch(IntPtr address, byte[] patchBytes, bool apply = true, bool mustBeDisposed = true) {
            return CreatePatch(address.ToInt64().ToString("X"), address, patchBytes, apply, mustBeDisposed);
        }

        public RemotePatch CreatePatch(string name, IntPtr address, byte[] patchBytes, bool apply = true, bool mustBeDisposed = true) {
            var patch = new RemotePatch(m_Process, name, address, patchBytes, mustBeDisposed);
            m_RemotePatches.Add(patch);
            if(apply)
                patch.Apply();
            return patch;
        }

        #endregion

        #region RemovePatch

        public void RemovePatch(string patchName) {
            var patch = m_RemotePatches.FirstOrDefault(p => p.Name.Equals(patchName, StringComparison.OrdinalIgnoreCase));
            if(patch != null)
                RemovePatch(patch);
        }

        public void RemovePatch(RemotePatch patch) {
            if(m_RemotePatches.Contains(patch))
                m_RemotePatches.Remove(patch);
            patch.InternalRemove();
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose() {
            foreach (var patch in RemotePatches.Where(p => p.MustBeDisposed).ToList()) {
                patch.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~PatchManager() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}