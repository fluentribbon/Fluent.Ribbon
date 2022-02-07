namespace Fluent.Internal
{
    using System.Windows.Input;
    using Windows.Win32;
    using Windows.Win32.Foundation;

    internal static class KeyEventUtility
    {
        public static unsafe string? GetStringFromKey(Key key)
        {
            var keyboardState = new byte[256];

            fixed (byte* pkeyboardState = keyboardState)
            {
                if (PInvoke.GetKeyboardState(pkeyboardState) == false)
                {
                    return null;
                }
            }

            var virtualKey = KeyInterop.VirtualKeyFromKey(key);

            var scanCode = PInvoke.MapVirtualKey((uint)virtualKey, 0x00 /*MAPVK_VK_TO_VSC*/);
            PWSTR chars = default;

            fixed (byte* pkeyboardState = keyboardState)
            {
                var result = PInvoke.ToUnicode((uint)virtualKey, scanCode, pkeyboardState, chars, chars.Length, 1);
                return result switch
                {
                    -1 or 0 => null,
                    1 => new string(chars.Value),
                    _ => null,
                };
            }
        }
    }
}