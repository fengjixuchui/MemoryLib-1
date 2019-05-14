using System.Threading;

namespace MemLib.Windows.Mouse {
    public abstract class BaseMouse {
        protected readonly RemoteWindow Window;

        protected BaseMouse(RemoteWindow window) {
            Window = window;
        }

        #region Abstract Methods

        protected abstract void MoveToAbsolute(int x, int y);
        public abstract void PressLeft();
        public abstract void PressMiddle();
        public abstract void PressRight();
        public abstract void ReleaseLeft();
        public abstract void ReleaseMiddle();
        public abstract void ReleaseRight();
        public abstract void ScrollHorizontally(int delta = 120);
        public abstract void ScrollVertically(int delta = 120);

        #endregion

        #region Extended Methods

        public void ClickLeft() {
            PressLeft();
            ReleaseLeft();
        }

        public void ClickLeft(int x, int y) {
            MoveTo(x, y);
            PressLeft();
            ReleaseLeft();
        }

        public void ClickMiddle() {
            PressMiddle();
            ReleaseMiddle();
        }

        public void ClickMiddle(int x, int y) {
            MoveTo(x, y);
            PressMiddle();
            ReleaseMiddle();
        }

        public void ClickRight() {
            PressRight();
            ReleaseRight();
        }

        public void ClickRight(int x, int y) {
            MoveTo(x, y);
            PressRight();
            ReleaseRight();
        }

        public void DoubleClickLeft() {
            ClickLeft();
            Thread.Sleep(10);
            ClickLeft();
        }

        public void DoubleClickLeft(int x, int y) {
            ClickLeft(x, y);
            Thread.Sleep(10);
            ClickLeft();
        }

        public void MoveTo(int x, int y) {
            MoveToAbsolute(Window.X + x, Window.Y + y);
        }

        #endregion
    }
}