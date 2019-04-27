namespace MemLib.Internals {
    internal static class Singleton<T> where T : new() {
        public static readonly T Instance = new T();
    }
}
