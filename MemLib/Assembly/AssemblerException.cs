using System;

namespace MemLib.Assembly {
    [Serializable]
    public sealed class AssemblerException : Exception {
        public string Error { get; }

        public AssemblerException(string message) : base(message) { }

        public AssemblerException(string message, string error) : base(message) {
            Error = error;
        }

        #region Overrides of Exception

        public override string ToString() {
            return string.IsNullOrEmpty(Error) ? $"{Message}" : $"{Message}: {Error}";
        }

        #endregion
    }
}