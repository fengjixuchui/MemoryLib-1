using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MemLib {
    public static class Utils {
        public static Process FindProcess(string processName) {
            if (Path.HasExtension(processName))
                processName = Path.GetFileNameWithoutExtension(processName);
            return Process.GetProcessesByName(processName).FirstOrDefault();
        }
    }
}