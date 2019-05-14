using System;
using MemLib.Native;

namespace MemLib.Windows.Keyboard {
    public sealed class MessageKeyboard : BaseKeyboard {
        public MessageKeyboard(RemoteWindow window) : base(window) { }

        #region Overrides of BaseKeyboard

        public override void Press(Keys key) {
            Window.PostMessage(WindowsMessages.KeyDown, new UIntPtr((uint)key), MakeKeyParameter(key, false));
        }

        public override void Release(Keys key) {
            base.Release(key);
            Window.PostMessage(WindowsMessages.KeyUp, new UIntPtr((uint)key), MakeKeyParameter(key, true));
        }

        public override void Write(char character) {
            Window.PostMessage(WindowsMessages.Char, new UIntPtr(character), UIntPtr.Zero);
        }

        #endregion

        private static UIntPtr MakeKeyParameter(Keys key, bool keyUp, bool fRepeat, uint cRepeat, bool altDown, bool fExtended) {
            var result = cRepeat;
            result |= NativeMethods.MapVirtualKey((uint) key, TranslationTypes.VirtualKeyToScanCode) << 16;
            if (fExtended) result |= 0x1000000;
            if (altDown) result |= 0x20000000;
            if (fRepeat) result |= 0x40000000;
            if (keyUp) result |= 0x80000000;
            return new UIntPtr(result);
        }

        private static UIntPtr MakeKeyParameter(Keys key, bool keyUp) {
            return MakeKeyParameter(key, keyUp, keyUp, 1, false, false);
        }
    }
}