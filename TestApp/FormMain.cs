using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

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

        private void ButtonTest1_Click(object sender, EventArgs e) {
            Logging.Clear();
            //var proc = Process.GetCurrentProcess();
            //proc = Process.GetProcessesByName("sekiro").FirstOrDefault();
            //proc = Process.GetProcessesByName("notepad++").FirstOrDefault();
            var swTotal = Stopwatch.StartNew();
            using (var asm = new MemLibNative.AsmTk.AsmTkWrapper()) {
                var asmtxt = "mov rax,05\n" +
                             "xor rcx,rcx\n" +
                             "testlabel:\n" +
                             "inc rcx\n" +
                             "cmp rax,rcx\n" +
                             "jne testlabel\n" +
                             "ret";

                if (asm.Test(asmtxt, out var bytes)) {
                    Logging.Log($"asmtk: {BytesToString(bytes, " ")}");
                } else Logging.Log($"asmtk nope");
            }

            using (var zdis = new MemLibNative.Zydis.ZydisWrapper()) {
                var bytes = new byte[] { 0xB8, 0x05, 0x00, 0x00, 0x00, 0x48, 0x33, 0xC9, 0x48, 0xFF, 0xC1, 0x48, 0x3B, 0xC1, 0x75, 0xF8, 0xC3 };
                if (zdis.Test(bytes, out var text)) {
                    Logging.Log($"zydis: {text}");
                } else Logging.Log("zydis nope");
            }

            swTotal.Stop();
            Logging.Log($"TotalTime: {swTotal.Elapsed.TotalMilliseconds:N1} ms ({swTotal.Elapsed.Ticks:N1} ticks)");
        }
    }
}