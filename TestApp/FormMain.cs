using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MemLib;
using MemLib.Assembly;

namespace TestApp {
    public partial class FormMain : Form {
        public delegate T TestMethod<T>(RemoteProcess proc, IntPtr address, T value);

        [RemoteFunction("C:\\Test\\TestDLL64.dll", "TestOutParam")]
        public delegate int TestOutParam(ref int arg);

        [RemoteFunction("C:\\Test\\TestDLL64.dll", "TestReturnValue")]
        public delegate int TestReturnValue(int arg, int arg2);

        private MethodInfo m_MethodInfo;

        private readonly string TestSource = @"
using System;
using MemLib;

public class TempClass{
public static T TestMethod<T>(RemoteProcess proc, IntPtr address, T value){
    proc.Write(address, value);
    return proc.Read<T>(address);
}
}
";

        public FormMain() {
            InitializeComponent();
            Logging.SetLogControl(TextBoxOutput);
        }

        private void FormMain_Load(object sender, EventArgs e) { }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e) { }

        public static string BytesToString(IEnumerable<byte> array, string separator = "") {
            return array == null ? string.Empty : string.Join(separator, array.Select(v => $"{v:X2}"));
        }

        private void ButtonTest1_Click(object sender, EventArgs e) {
            Logging.Clear();
            var proc = Process.GetCurrentProcess();
            //proc = Process.GetProcessesByName("sekiro").FirstOrDefault();
            //proc = Process.GetProcessesByName("notepad++").FirstOrDefault();
            //proc = Process.GetProcessesByName("ReClass.NET").FirstOrDefault();
            var swTotal = Stopwatch.StartNew();

            using (var mem = new RemoteProcess(proc)) {
                var alloc = mem.Memory.Allocate(sizeof(int));
                var compiler = new RuntimeCompiler();
                compiler.OnError += (o, err) => { Logging.Log(string.Join("\n", err)); };
                if (m_MethodInfo == null) {
                    var asm = compiler.CompileFromSource(TestSource).CompiledAssembly;
                    m_MethodInfo = asm.GetType("TempClass").GetMethod("TestMethod")?.MakeGenericMethod(typeof(int));
                }

                for (var i = 0; i < 10; i++) {
                    var result = m_MethodInfo?.Invoke(null, new object[] {mem, alloc.BaseAddress, 1 + i});
                    Logging.Log($"result = {result ?? -1}");
                }
            }

            swTotal.Stop();
            Logging.Log($"TotalTime: {swTotal.Elapsed.TotalMilliseconds:N1} ms ({swTotal.Elapsed.Ticks:N1} ticks)");
        }
    }
}