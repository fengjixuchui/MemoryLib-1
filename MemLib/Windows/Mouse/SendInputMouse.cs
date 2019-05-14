using MemLib.Native;

namespace MemLib.Windows.Mouse {
    public sealed class SendInputMouse : BaseMouse {
        internal SendInputMouse(RemoteWindow window) : base(window) { }

        #region Overrides of BaseMouse

        protected override void MoveToAbsolute(int x, int y) {
            var input = CreateInput();
            input.Mouse.DeltaX = CalculateAbsoluteCoordinateX(x);
            input.Mouse.DeltaY = CalculateAbsoluteCoordinateY(y);
            input.Mouse.Flags = MouseFlags.Move | MouseFlags.Absolute;
            input.Mouse.MouseData = 0;
            WindowHelper.SendInput(input);
        }

        public override void PressLeft() {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.LeftDown;
            WindowHelper.SendInput(input);
        }

        public override void PressMiddle() {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.MiddleDown;
            WindowHelper.SendInput(input);
        }

        public override void PressRight() {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.RightDown;
            WindowHelper.SendInput(input);
        }

        public override void ReleaseLeft() {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.LeftUp;
            WindowHelper.SendInput(input);
        }

        public override void ReleaseMiddle() {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.MiddleUp;
            WindowHelper.SendInput(input);
        }

        public override void ReleaseRight() {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.RightUp;
            WindowHelper.SendInput(input);
        }

        public override void ScrollHorizontally(int delta = 120) {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.HWheel;
            input.Mouse.MouseData = delta;
            WindowHelper.SendInput(input);
        }

        public override void ScrollVertically(int delta = 120) {
            var input = CreateInput();
            input.Mouse.Flags = MouseFlags.Wheel;
            input.Mouse.MouseData = delta;
            WindowHelper.SendInput(input);
        }

        #endregion

        private static int CalculateAbsoluteCoordinateX(int x) {
            return x * 65536 / NativeMethods.GetSystemMetrics(SystemMetrics.CxScreen);
        }

        private static int CalculateAbsoluteCoordinateY(int y) {
            return y * 65536 / NativeMethods.GetSystemMetrics(SystemMetrics.CyScreen);
        }

        private static Input CreateInput() {
            return new Input(InputTypes.Mouse);
        }
    }
}