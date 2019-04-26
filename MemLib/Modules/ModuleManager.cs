using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MemLib.Native;

namespace MemLib.Modules {
    public sealed class ModuleManager : IDisposable {
        private readonly RemoteProcess m_Process;
        private readonly HashSet<InjectedModule> m_InjectedModules;
        internal IEnumerable<NativeModule> NativeModules => InternalEnumProcessModules();

        public RemoteModule MainModule { get; }
        public IEnumerable<RemoteModule> RemoteModules => NativeModules.Select(FetchModule);

        public RemoteModule this[string moduleName] => FetchModule(moduleName);

        internal ModuleManager(RemoteProcess process) {
            m_Process = process;
            m_InjectedModules = new HashSet<InjectedModule>();
            MainModule = FetchModule(process.Native.MainModule.ModuleName);
        }

        private RemoteModule FetchModule(string moduleName) {
            if (!Path.HasExtension(moduleName))
                moduleName += ".dll";
            var nativeMod = NativeModules.FirstOrDefault(m => m.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            return nativeMod == null ? null : new RemoteModule(m_Process, nativeMod);
        }

        private RemoteModule FetchModule(NativeModule module) {
            return new RemoteModule(m_Process, module);
        }
        
        [DebuggerStepThrough]
        public InjectedModule Inject(string moduleFile, bool mustBeDisposed = true) {
            if (!File.Exists(moduleFile)) {
                moduleFile = FindFullPath(moduleFile);
                if(!File.Exists(moduleFile))
                    throw new FileNotFoundException("File not found.", moduleFile);
            }
            var module = InternalInject(moduleFile, mustBeDisposed);
            if (module != null && !m_InjectedModules.Contains(module)) 
                m_InjectedModules.Add(module);
            return module;
        }

        public void Eject(RemoteModule module) {
            if (!module.IsValid) return;
            
            var injected = m_InjectedModules.FirstOrDefault(m => m.Equals(module));
            if (injected != null)
                m_InjectedModules.Remove(injected);

            InternalEject(module);
        }

        public void Eject(string moduleName) {
            var module = RemoteModules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            if (module != null)
                InternalEject(module);
        }

        private static string FindFullPath(string fileName) {
            fileName = Environment.ExpandEnvironmentVariables(fileName);
            if (File.Exists(fileName)) return Path.GetFullPath(fileName);
            if (Path.GetDirectoryName(fileName) == string.Empty) {
                foreach (var test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';')) {
                    var path = test.Trim();
                    if (!string.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, fileName)))
                        return Path.GetFullPath(path);
                }
            }
            return null;
        }

        internal IEnumerable<NativeModule> InternalEnumProcessModules() {
            var flags = m_Process.Is64Bit ? ListModulesFlags.ListModules64Bit : ListModulesFlags.ListModules32Bit;
            if (!ModuleHelper.EnumProcessModules(m_Process.Handle, out var modHandles, flags))
                yield break;
            foreach (var handle in modHandles)
                yield return new NativeModule(m_Process.Handle, handle);
        }

        private InjectedModule InternalInject(string path, bool mustBeDisposed) {
            var thread = m_Process.Threads.CreateAndJoin(m_Process["kernel32"]["LoadLibraryA"].BaseAddress, path);
            var exitCode = thread.GetExitCode<int>();
            if (exitCode == 0) return null;
            var moduleName = Path.GetFileName(path);
            var nativeMod = m_Process.Modules.NativeModules.First(m => m.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            return new InjectedModule(m_Process, nativeMod, mustBeDisposed);
        }

        private void InternalEject(RemoteModule module) {
            m_Process.Threads.CreateAndJoin(m_Process["kernel32"]["FreeLibrary"].BaseAddress, module.BaseAddress);
        }

        #region IDisposable

        void IDisposable.Dispose() {
            foreach (var module in m_InjectedModules.Where(m => m.MustBeDisposed).ToList()) {
                module.Dispose();
            }
            foreach (var cachedFunction in RemoteModule.CachedFunctions.ToArray()) {
                if (cachedFunction.Key.Item2 == m_Process.Handle)
                    RemoteModule.CachedFunctions.Remove(cachedFunction);
            }
            GC.SuppressFinalize(this);
        }

        ~ModuleManager() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }

}