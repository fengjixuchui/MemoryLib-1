using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using MemLib;

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
            var proc = Process.GetCurrentProcess();
            //proc = Process.GetProcessesByName("sekiro").FirstOrDefault();
            //proc = Process.GetProcessesByName("notepad++").FirstOrDefault();
            //proc = Process.GetProcessesByName("ReClass.NET").FirstOrDefault();
            var swTotal = Stopwatch.StartNew();

            using (var mem = new RemoteProcess(proc)) {
            }

            swTotal.Stop();
            Logging.Log($"TotalTime: {swTotal.Elapsed.TotalMilliseconds:N1} ms ({swTotal.Elapsed.Ticks:N1} ticks)");
        }

        public T GetFunction<T>() where T : class {
            return (T)(object)CreateMethod(typeof(T));
        }
        
        public delegate int TestDelegate1(int arg, int arg2);
        public delegate int TestDelegate2(string arg, out string arg1);

        public int TestMethod(int arg, int arg2) => arg * arg2;
        public int TestMethod(string arg, out string arg2) {
            arg2 = "123" + arg;
            return 0;
        }
        
        public Delegate CreateMethod(Type del) {
            var fn = del.GetMethod("Invoke");
            if (fn == null) return null;

            var parameterTypes = fn.GetParameters().Select(p => p.ParameterType).ToArray();

            var methodInfo = GetType().GetMethod("TestMethod", parameterTypes);
            if (methodInfo == null)
                return null;
            
            var result = Expression.Variable(fn.ReturnType);
            var paramVars = parameterTypes.Select(Expression.Parameter).ToList();

            var block = Expression.Block(
                fn.ReturnType,
                new[] {result},
                Expression.Call(Expression.Constant(this), methodInfo, paramVars)
                );
            return Expression.Lambda(del, block, paramVars).Compile();
        }
    }
}