using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MemLib;
using MemLib.Assembly;

namespace TestApp {
    public partial class FormMain : Form {
        public FormMain() {
            InitializeComponent();
            Logging.SetLogControl(TextBoxOutput);
        }

        private void FormMain_Load(object sender, EventArgs e) { }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e) { }

        public static string BytesToString(IEnumerable<byte> array, string separator = "") {
            return array == null ? string.Empty : string.Join(separator, array.Select(v => $"{v:X2}"));
        }

        [RemoteFunction("C:\\Test\\TestDLL64.dll", "TestReturnValue")]
        public delegate int TestReturnValue(int arg, int arg2);

        [RemoteFunction("C:\\Test\\TestDLL64.dll", "TestOutParam")]
        public delegate int TestOutParam(ref int arg);

        private void ButtonTest1_Click(object sender, EventArgs e) {
            Logging.Clear();
            var proc = Process.GetCurrentProcess();
            //proc = Process.GetProcessesByName("sekiro").FirstOrDefault();
            //proc = Process.GetProcessesByName("notepad++").FirstOrDefault();
            //proc = Process.GetProcessesByName("ReClass.NET").FirstOrDefault();
            var swTotal = Stopwatch.StartNew();

            using (var mem = new RemoteProcess(proc)) {
                var gen = new RemoteFunctionGenerator(mem);
                var func = gen.GetFunction<TestReturnValue>();
                Logging.Log($"{func?.Invoke(0, 123)}");

                var func2 = gen.GetFunction<TestOutParam>();
                var test = 1;
                func2?.Invoke(ref test);
                Logging.Log($"{test}");
                //for (var i = 1; i < 11; i++) func?.Invoke(0, i);
            }

            swTotal.Stop();
            Logging.Log($"TotalTime: {swTotal.Elapsed.TotalMilliseconds:N1} ms ({swTotal.Elapsed.Ticks:N1} ticks)");
        }
    }
}