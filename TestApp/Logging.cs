using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestApp {
    public static class Logging {
        private static Control _logControl;
        private static bool _useCtrl;

        public static void Log(object obj) {
            Log(obj.ToString());
        }

        public static void Log(string msg) {
            if (_useCtrl) {
                msg = msg + Environment.NewLine;
                if (_logControl is RichTextBox ctrl)
                    Invoke(() => {
                        ctrl.SuspendLayout();
                        ctrl.AppendText(msg);
                        ctrl.ScrollToCaret();
                        ctrl.ResumeLayout();
                    });
                else
                    throw new ApplicationException("Logging Error: Log Control is null");
            } else {
                Console.WriteLine(msg);
            }
        }

        public static void Log(string msg, Color color) {
            if (_useCtrl) {
                msg = msg + Environment.NewLine;
                if (_logControl is RichTextBox ctrl)
                    Invoke(() => {
                        ctrl.SuspendLayout();
                        ctrl.AppendText(msg, color);
                        ctrl.ScrollToCaret();
                        ctrl.ResumeLayout();
                    });
                else
                    throw new ApplicationException("Logging Error: Log Control is null");
            } else {
                Console.WriteLine(msg);
            }
        }

        public static void Clear() {
            if (_useCtrl) {
                if (_logControl is RichTextBox ctrl)
                    Invoke(() => ctrl.Clear());
                else
                    throw new ApplicationException("Logging Error: Log Control is null");
            } else {
                Console.Clear();
            }
        }

        public static void SetLogControl(Control ctrl) {
            if (ctrl != null) {
                _logControl = ctrl;
                _useCtrl = true;
            } else {
                _logControl = null;
                _useCtrl = false;
            }
        }

        public static void LogConsole(string msg) {
            Console.WriteLine(msg);
        }

        private static void Invoke(Action method) {
            try {
                if (_logControl.InvokeRequired)
                    _logControl.Invoke(method);
                else
                    method();
            } catch (Exception ex) {
                LogConsole($"[Error] {ex.Message}");
            }
        }

        public static void AppendText(this RichTextBox box, string text, Color color, bool addNewLine = false) {
            box.SuspendLayout();
            box.SelectionColor = color;
            box.AppendText(addNewLine
                ? $"{text}{Environment.NewLine}"
                : text);
            box.ScrollToCaret();
            box.ResumeLayout();
        }

        public static string DumpObject(object obj, bool toLog = true) {
            var output = TypeDescriptor.GetProperties(obj).Cast<PropertyDescriptor>()
                .Aggregate("", (current, descriptor) => current + $"{descriptor.Name} = {descriptor.GetValue(obj)}\n");
            if (toLog) Log(output);
            return output;
        }
    }
}