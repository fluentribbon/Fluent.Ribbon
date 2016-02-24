namespace Fluent.Internal
{
    using System.Windows.Input;

    internal static class KeyEventUtility
    {
        public static string GetStringFromKey(Key key)
        {
            var keyboardState = new byte[256];
            if (NativeMethods.GetKeyboardState(keyboardState) == false)
            {
                return null;
            }

            var virtualKey = KeyInterop.VirtualKeyFromKey(key);
            var scanCode = NativeMethods.MapVirtualKey((uint)virtualKey, NativeMethods.MapType.MAPVK_VK_TO_VSC);
            var chars = new char[1];

            var result = NativeMethods.ToUnicode((uint)virtualKey, scanCode, keyboardState, chars, chars.Length, 0);
            switch (result)
            {
                case -1:
                case 0:
                    return null;

                case 1:
                    return chars[0].ToString();

                default:
                    return null;
            }
        }
    }
}