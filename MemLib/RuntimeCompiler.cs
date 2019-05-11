using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MemLib {
    public sealed class RuntimeCompiler : IDisposable {
        private CodeDomProvider m_CodeDom;
        private readonly string[] m_ReferencedAssemblies;
        private CompilerParameters m_Parameters;
        public CompilerParameters Parameters {
            get => m_Parameters ?? (m_Parameters = GetDefaultParameters());
            set => m_Parameters = value;
        }
        public bool DeleteOutputAssembly { get; set; } = true;
        public string Language { get; }

        public event EventHandler<IEnumerable<string>> OnError;
        public event EventHandler<IEnumerable<string>> OnWarning;

        public RuntimeCompiler() {
            if (!CodeDomProvider.IsDefinedLanguage("c#"))
                throw new ApplicationException("No CodeProvider for 'c#'");
            m_CodeDom = CodeDomProvider.CreateProvider("c#");
            var references = System.Reflection.Assembly.GetCallingAssembly().GetReferencedAssemblies();
            m_ReferencedAssemblies = references.Select(r => r.Name + ".dll").ToArray();
            Language = "c#";
        }

        public RuntimeCompiler(string language) {
            if (!CodeDomProvider.IsDefinedLanguage(language))
                throw new ApplicationException($"No CodeProvider for '{language}'");
            m_CodeDom = CodeDomProvider.CreateProvider(language);
            var references = System.Reflection.Assembly.GetCallingAssembly().GetReferencedAssemblies();
            m_ReferencedAssemblies = references.Select(r => r.Name + ".dll").ToArray();
            Language = language;
        }

        public void Reset() {
            Parameters = GetDefaultParameters();
            m_CodeDom?.Dispose();
            m_CodeDom = CodeDomProvider.CreateProvider(Language);
        }

        public CompilerParameters GetDefaultParameters() {
            var param = new CompilerParameters {
                GenerateExecutable = false,
                GenerateInMemory = true
            };
            param.ReferencedAssemblies.AddRange(m_ReferencedAssemblies);
            return param;
        }

        public CompilerResults CompileFromSource(string source) {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException("Source can't be null or empty.", nameof(source));

            if (string.IsNullOrEmpty(Parameters.OutputAssembly)) {
                var match = Regex.Match(source, @"class\W+(?<ClassName>\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (match.Success) {
                    var fileName = match.Groups["ClassName"].Value + (Parameters.GenerateExecutable ? ".exe" : ".dll");
                    Parameters.OutputAssembly = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                } else {
                    Parameters.OutputAssembly = Path.Combine(Directory.GetCurrentDirectory(), "TempAssembly_DeleteMe");
                }
            }

            var result = m_CodeDom.CompileAssemblyFromSource(Parameters, source);
            if (result.Errors.HasErrors)
                OnError?.Invoke(this, result.Errors.Cast<CompilerError>().Select(e => e.ToString()));
            if (result.Errors.HasWarnings)
                OnWarning?.Invoke(this, result.Errors.Cast<CompilerError>().Select(e => e.ToString()));
            if (DeleteOutputAssembly && !Parameters.GenerateExecutable && !string.IsNullOrEmpty(Parameters.OutputAssembly))
                File.Delete(Parameters.OutputAssembly);
            return result;
        }

        public CompilerResults CompileFromFile(string file) {
            if (!File.Exists(file))
                throw new FileNotFoundException("File not found.", file);

            if (string.IsNullOrEmpty(Parameters.OutputAssembly)) {
                var fileName = Path.GetFileNameWithoutExtension(file);
                fileName += Parameters.GenerateExecutable ? ".exe" : ".dll";
                Parameters.OutputAssembly = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            }

            var result = m_CodeDom.CompileAssemblyFromFile(Parameters, file);
            if (result.Errors.HasErrors)
                OnError?.Invoke(this, result.Errors.Cast<CompilerError>().Select(e => e.ToString()));
            if (result.Errors.HasWarnings)
                OnWarning?.Invoke(this, result.Errors.Cast<CompilerError>().Select(e => e.ToString()));
            if (DeleteOutputAssembly && !Parameters.GenerateExecutable && !string.IsNullOrEmpty(Parameters.OutputAssembly))
                File.Delete(Parameters.OutputAssembly);
            return result;
        }

        public static CompilerResults CompileFile(string file, CompilerParameters parameters) {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (!File.Exists(file))
                throw new FileNotFoundException("File not found.", file);
            var ext = Path.GetExtension(file);
            if (string.IsNullOrEmpty(ext))
                throw new ArgumentException("The file needs a file extension.", nameof(file));
            var lang = CodeDomProvider.GetLanguageFromExtension(ext);
            if (!CodeDomProvider.IsDefinedLanguage(lang))
                throw new ArgumentException("The file is not supported.", nameof(file));
            var codeDom = CodeDomProvider.CreateProvider(lang);
            return codeDom.CompileAssemblyFromFile(parameters, file);
        }

        #region IDisposable

        public void Dispose() {
            m_CodeDom?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~RuntimeCompiler() {
            Dispose();
        }

        #endregion
    }
}