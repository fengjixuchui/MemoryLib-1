using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MemLib.Native;
using MemLib.Threading;
using MemLib.Windows.Keyboard;
using MemLib.Windows.Mouse;

namespace MemLib.Windows {
    public sealed class RemoteWindow : IEquatable<RemoteWindow>{
        private readonly RemoteProcess m_Process;
        private IEnumerable<IntPtr> ChildHandles => WindowHelper.EnumChildWindows(Handle);
        internal WindowPlacement Placement {
            get => WindowHelper.GetWindowPlacement(Handle);
            set => WindowHelper.SetWindowPlacement(Handle, value);
        }

        public IntPtr Handle { get; }
        public SendInputMouse Mouse { get; }
        public MessageKeyboard Keyboard { get; }
        public bool IsMainWindow => m_Process.Windows.MainWindow == this;
        public bool IsActivated => NativeMethods.GetForegroundWindow() == Handle;
        public string ClassName => WindowHelper.GetClassName(Handle);
        public RemoteThread Thread => m_Process.Threads.GetThreadById(NativeMethods.GetWindowThreadProcessId(Handle, out _));
        public IEnumerable<RemoteWindow> Children => ChildHandles.Select(handle => new RemoteWindow(m_Process, handle));
        public WindowStates State {
            get => Placement.ShowCmd;
            set => NativeMethods.ShowWindow(Handle, value);
        }
        public string Title {
            get => WindowHelper.GetWindowText(Handle);
            set => NativeMethods.SetWindowText(Handle, value);
        }
        public int Width {
            get => Placement.NormalPosition.Width;
            set {
                var p = Placement;
                p.NormalPosition.Width = value;
                Placement = p;
            }
        }
        public int Height {
            get => Placement.NormalPosition.Height;
            set {
                var p = Placement;
                p.NormalPosition.Height = value;
                Placement = p;
            }
        }
        public int X {
            get => Placement.NormalPosition.Left;
            set {
                var p = Placement;
                p.NormalPosition.Right = value + p.NormalPosition.Width;
                p.NormalPosition.Left = value;
                Placement = p;
            }
        }
        public int Y {
            get => Placement.NormalPosition.Top;
            set {
                var p = Placement;
                p.NormalPosition.Bottom = value + p.NormalPosition.Height;
                p.NormalPosition.Top = value;
                Placement = p;
            }
        }

        internal RemoteWindow(RemoteProcess process, IntPtr handle) {
            m_Process = process;
            Handle = handle;
            Mouse = new SendInputMouse(this);
            Keyboard = new MessageKeyboard(this);
        }

        public bool Activate() {
            return WindowHelper.SetForegroundWindow(Handle);
        }

        public bool Close() {
            return PostMessage(WindowsMessages.Close, UIntPtr.Zero, UIntPtr.Zero);
        }

        public void Flash() {
            NativeMethods.FlashWindow(Handle, true);
        }

        public void Flash(uint count, FlashWindowFlags flags = FlashWindowFlags.All) {
            Flash(count, TimeSpan.FromMilliseconds(0), flags);
        }

        public void Flash(uint count, TimeSpan timeout, FlashWindowFlags flags = FlashWindowFlags.All) {
            var flashInfo = new FlashInfo {
                Size = Marshal.SizeOf(typeof(FlashInfo)),
                Hwnd = Handle,
                Flags = flags,
                Count = count,
                Timeout = Convert.ToInt32(timeout.TotalMilliseconds)
            };
            NativeMethods.FlashWindowEx(ref flashInfo);
        }

        public bool PostMessage(WindowsMessages message, UIntPtr wParam, UIntPtr lParam) {
            return NativeMethods.PostMessage(Handle, (uint)message, wParam, lParam);
        }

        public bool PostMessage(uint message, UIntPtr wParam, UIntPtr lParam) {
            return NativeMethods.PostMessage(Handle, message, wParam, lParam);
        }

        public IntPtr SendMessage(WindowsMessages message, UIntPtr wParam, IntPtr lParam) {
            return NativeMethods.SendMessage(Handle, (uint)message, wParam, lParam);
        }

        public IntPtr SendMessage(uint message, UIntPtr wParam, IntPtr lParam) {
            return NativeMethods.SendMessage(Handle, message, wParam, lParam);
        }

        public override string ToString() => $"Title = [{Title}] ClassName = [{ClassName}]";

        #region Equality members

        public bool Equals(RemoteWindow other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(m_Process, other.m_Process) && Handle.Equals(other.Handle);
        }

        public override bool Equals(object obj) {
            return ReferenceEquals(this, obj) || obj is RemoteWindow other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return ((m_Process != null ? m_Process.GetHashCode() : 0) * 397) ^ Handle.GetHashCode();
            }
        }

        public static bool operator ==(RemoteWindow left, RemoteWindow right) {
            return Equals(left, right);
        }

        public static bool operator !=(RemoteWindow left, RemoteWindow right) {
            return !Equals(left, right);
        }

        #endregion
    }
}