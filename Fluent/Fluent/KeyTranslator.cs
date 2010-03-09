using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// 
    /// </summary>
    internal class KeyTranslator
    {
        // Cache with loaded keyboard layouts
        static readonly Dictionary<int, IntPtr> keyboardLayoutHandlers = new Dictionary<int, IntPtr>();
        // State of the keyboard
        static readonly byte[] keyboardState = new byte[255];

        /// <summary>
        /// Converts Key to Char (considering keyboard layout defined in the given culture)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cultureInfo">Culture</param>
        /// <returns>Char</returns>
        public static char? KeyToChar(Key key, CultureInfo cultureInfo)
        {
            int virtualKey = KeyInterop.VirtualKeyFromKey(key);

            // Try to find keyboard layout in cache
            IntPtr keyboardlayout;
            if (!keyboardLayoutHandlers.TryGetValue(cultureInfo.LCID, out keyboardlayout))
            {
                keyboardlayout = NativeMethods.LoadKeyboardLayout(cultureInfo.LCID.ToString("x8"), 0x00000080);
                keyboardLayoutHandlers.Add(cultureInfo.LCID, keyboardlayout);
            }

            // Get keyboard status
            bool bKeyStateStatus = NativeMethods.GetKeyboardState(keyboardState);
            if (!bKeyStateStatus) return null;
            uint scanCode = NativeMethods.MapVirtualKeyEx((uint)virtualKey, 2, keyboardlayout);

            StringBuilder result = new StringBuilder(10);
            NativeMethods.ToUnicodeEx((uint) virtualKey, scanCode, keyboardState, result, 10, 0, keyboardlayout);
            return (result.Length >= 1) ? new char?(result[0]) : null;
        }
    }
}
