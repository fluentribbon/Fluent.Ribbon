#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Helps to translate Key to Char 
    /// (with keyboard layouts support)
    /// </summary>
    internal static class KeyTranslator
    {
        // Cache with loaded keyboard layouts
        static readonly Dictionary<int, IntPtr> keyboardLayoutHandlers = new Dictionary<int, IntPtr>();
        // State of the keyboard
        static readonly byte[] keyboardState = new byte[255];
        // Layouts existed in the system
        static IntPtr[] existingLayouts;


        /// <summary>
        /// Converts Key to Char (considering keyboard layout defined in the given culture)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cultureInfo">Culture</param>
        /// <returns>Char</returns>
        public static char? KeyToChar(Key key, CultureInfo cultureInfo)
        {
            // Get layouts existed in the system
            if (existingLayouts == null)
            {
                int keyboardsCount = NativeMethods.GetKeyboardLayoutList(0, null);
                existingLayouts = new IntPtr[keyboardsCount];
                NativeMethods.GetKeyboardLayoutList(keyboardsCount, existingLayouts);
            }

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);

            // Try to find keyboard layout in cache
            IntPtr keyboardlayout;
            if (!keyboardLayoutHandlers.TryGetValue(cultureInfo.LCID, out keyboardlayout))
            {
                keyboardlayout = NativeMethods.LoadKeyboardLayout(cultureInfo.LCID.ToString("x8", CultureInfo.InvariantCulture), 0x00000080);
                keyboardLayoutHandlers.Add(cultureInfo.LCID, keyboardlayout);
            }

            // Get keyboard status
            // REMARK: commented due to the bug: Alt + H produce nothing
            // bool bKeyStateStatus = NativeMethods.GetKeyboardState(keyboardState);
            // if (!bKeyStateStatus) return null;
            uint scanCode = NativeMethods.MapVirtualKeyEx((uint)virtualKey, 2, keyboardlayout);

            StringBuilder result = new StringBuilder(10);
            int count = NativeMethods.ToUnicodeEx((uint) virtualKey, scanCode, keyboardState, result, 10, 0, keyboardlayout);

            if (!existingLayouts.Contains(keyboardlayout))
            {
                // We must unload the layout to be sure 
                // that previously non-existed layout remains in the system
                NativeMethods.UnloadKeyboardLayout(keyboardlayout);
                keyboardLayoutHandlers.Remove(cultureInfo.LCID);
            }

            return (result.Length >= 1 && count >= 1) ? new char?(result[0]) : null;
        }
    }
}
