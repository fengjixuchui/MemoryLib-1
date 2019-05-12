using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MemLib;
using MemLibNative.Zydis;

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

        /*
        7FF75DB6BD80 - 33 C0                 - xor eax,eax
        7FF75DB6BD82 - 48 89 41 70           - mov [rcx+70],rax
        7FF75DB6BD86 - 48 89 41 78           - mov [rcx+78],rax
        7FF75DB6BD8A - 88 81 80000000        - mov [rcx+00000080],al
        7FF75DB6BD90 - 48 8B C1              - mov rax,rcx
        7FF75DB6BD93 - C3                    - ret 
         */

        private void ButtonTest1_Click(object sender, EventArgs e) {
            Logging.Clear();
            var proc = Process.GetCurrentProcess();
            //proc = Process.GetProcessesByName("sekiro").FirstOrDefault();
            //proc = Process.GetProcessesByName("notepad++").FirstOrDefault();
            //proc = Process.GetProcessesByName("ReClass.NET").FirstOrDefault();
            var swTotal = Stopwatch.StartNew();
            var address = 0x7FF75DB6BD80;
            var data = new byte[] {
                0x33, 0xC0,
                0x48, 0x89, 0x41, 0x70,
                0x48, 0x89, 0x41, 0x78,
                0x88, 0x81, 0x80, 0x00, 0x00, 0x00,
                0x48, 0x8B, 0xC1,
                0xC3
            };
            using (var mem = new RemoteProcess(proc)) {
            }

            swTotal.Stop();
            Logging.Log($"TotalTime: {swTotal.Elapsed.TotalMilliseconds:N1} ms ({swTotal.Elapsed.Ticks:N1} ticks)");
        }
    }
}