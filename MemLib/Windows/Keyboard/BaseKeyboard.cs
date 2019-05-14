using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MemLib.Native;

namespace MemLib.Windows.Keyboard {
    public abstract class BaseKeyboard {
        protected readonly RemoteWindow Window;
        protected static readonly List<Tuple<IntPtr, Keys>> PressedKeys = new List<Tuple<IntPtr, Keys>>();
        protected BaseKeyboard(RemoteWindow window) {
            Window = window;
        }

        #region Abstract Methods

        public abstract void Press(Keys key);
        public abstract void Write(char character);

        #endregion

        #region Virtual Methods

        public virtual void Release(Keys key) {
            var tuple = Tuple.Create(Window.Handle, key);
            if (PressedKeys.Contains(tuple))
                PressedKeys.Remove(tuple);
        }

        #endregion

        #region Extended Methods

        public void Press(Keys key, TimeSpan interval) {
            var tuple = Tuple.Create(Window.Handle, key);
            if (PressedKeys.Contains(tuple))
                return;
            PressedKeys.Add(tuple);
            
            Task.Run(async () => {
                while (PressedKeys.Contains(tuple)) {
                    Press(key);
                    await Task.Delay(interval);
                }
            });
        }

        public void PressRelease(Keys key) {
            Press(key);
            Thread.Sleep(10);
            Release(key);
        }

        public void Write(string text, params object[] args) {
            foreach (var character in string.Format(text, args)) {
                Write(character);
            }
        }

        #endregion
    }
}