using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    using System.Text;
    using Fluent.Metro.Native;

    internal static class NativeMethods
    {
        /// <summary>
        /// Causes the dialog box to display all available colors in the set of basic colors. 
        /// </summary>
        public const int CC_ANYCOLOR = 0x00000100;

        /// <summary>
        /// The MonitorFromRect function retrieves a handle to the display monitor that 
        /// has the largest area of intersection with a specified rectangle.
        /// </summary>
        /// <param name="lprc">Pointer to a RECT structure that specifies the rectangle of interest in 
        /// virtual-screen coordinates</param>
        /// <param name="dwFlags">Determines the function's return value if the rectangle does not intersect 
        /// any display monitor</param>
        /// <returns>
        /// If the rectangle intersects one or more display monitor rectangles, the return value 
        /// is an HMONITOR handle to the display monitor that has the largest area of intersection with the rectangle.
        /// If the rectangle does not intersect a display monitor, the return value depends on the value of dwFlags.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromRect([In] ref RECT lprc, MONITORINFO.MonitorOptions dwFlags);

        /// <summary>
        /// Loads an icon, cursor, animated cursor, or bitmap.
        /// </summary>
        /// <param name="hinst">Handle to the module of either a DLL or executable (.exe) that contains the image to be loaded</param>
        /// <param name="lpszName">Specifies the image to load</param>
        /// <param name="uType">Specifies the type of image to be loaded. </param>
        /// <param name="cxDesired">Specifies the width, in pixels, of the icon or cursor</param>
        /// <param name="cyDesired">Specifies the height, in pixels, of the icon or cursor</param>
        /// <param name="fuLoad">This parameter can be one or more of the following values.</param>
        /// <returns>If the function succeeds, the return value is the requested value.If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hinst, IntPtr lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        /// <summary>
        /// Creates a Color dialog box that enables the user to select a color.
        /// </summary>
        /// <param name="lpcc">A pointer to a CHOOSECOLOR structure that contains information used to initialize the dialog box. When ChooseColor returns, this structure contains information about the user's color selection.</param>
        /// <returns>If the user clicks the OK button of the dialog box, the return value is nonzero. The rgbResult member of the CHOOSECOLOR structure contains the RGB color value of the color selected by the user.If the user cancels or closes the Color dialog box or an error occurs, the return value is zero. </returns>
        [DllImport("comdlg32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChooseColor(CHOOSECOLOR lpcc);

        /// <summary>
        /// Contains information the ChooseColor function uses to initialize the Color dialog box. After the user closes the dialog box, the system returns information about the user's selection in this structure. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class CHOOSECOLOR
        {
            /// <summary>
            /// The length, in bytes, of the structure. 
            /// </summary>
            public int lStructSize = Marshal.SizeOf(typeof(CHOOSECOLOR));
            /// <summary>
            /// A handle to the window that owns the dialog box. This member can be any valid window handle, or it can be NULL if the dialog box has no owner. 
            /// </summary>
            public IntPtr hwndOwner;
            /// <summary>
            /// If the CC_ENABLETEMPLATEHANDLE flag is set in the Flags member, hInstance is a handle to a memory object containing a dialog box template. If the CC_ENABLETEMPLATE flag is set, hInstance is a handle to a module that contains a dialog box template named by the lpTemplateName member. If neither CC_ENABLETEMPLATEHANDLE nor CC_ENABLETEMPLATE is set, this member is ignored. 
            /// </summary>
            public IntPtr hInstance = IntPtr.Zero;
            /// <summary>
            /// If the CC_RGBINIT flag is set, rgbResult specifies the color initially selected when the dialog box is created. If the specified color value is not among the available colors, the system selects the nearest solid color available. If rgbResult is zero or CC_RGBINIT is not set, the initially selected color is black. If the user clicks the OK button, rgbResult specifies the user's color selection. To create a COLORREF color value, use the RGB macro. 
            /// </summary>
            public int rgbResult;
            /// <summary>
            /// A pointer to an array of 16 values that contain red, green, blue (RGB) values for the custom color boxes in the dialog box. If the user modifies these colors, the system updates the array with the new RGB values. To preserve new custom colors between calls to the ChooseColor function, you should allocate static memory for the array. To create a COLORREF color value, use the RGB macro. 
            /// </summary>
            public IntPtr lpCustColors = IntPtr.Zero;
            /// <summary>
            /// A set of bit flags that you can use to initialize the Color dialog box. When the dialog box returns, it sets these flags to indicate the user's input. 
            /// </summary>
            public int Flags;
            /// <summary>
            /// Application-defined data that the system passes to the hook procedure identified by the lpfnHook member. When the system sends the WM_INITDIALOG message to the hook procedure, the message's lParam parameter is a pointer to the CHOOSECOLOR structure specified when the dialog was created. The hook procedure can use this pointer to get the lCustData value. 
            /// </summary>
            public IntPtr lCustData = IntPtr.Zero;
            /// <summary>
            /// A pointer to a CCHookProc hook procedure that can process messages intended for the dialog box. This member is ignored unless the CC_ENABLEHOOK flag is set in the Flags member. 
            /// </summary>
            public IntPtr lpfnHook = IntPtr.Zero;
            /// <summary>
            /// The name of the dialog box template resource in the module identified by the hInstance member. This template is substituted for the standard dialog box template. For numbered dialog box resources, lpTemplateName can be a value returned by the MAKEINTRESOURCE macro. This member is ignored unless the CC_ENABLETEMPLATE flag is set in the Flags member. 
            /// </summary>
            public IntPtr lpTemplateName = IntPtr.Zero;
        }

        // Sets in first IsDwmEnabled call
        private static bool idDwmDllNotFound;

        /// <summary>
        /// Is DWM enabled
        /// </summary>
        /// <returns>Is DWM enabled</returns>
        public static bool IsDwmEnabled()
        {
            if (idDwmDllNotFound) return false;

            if (Environment.OSVersion.Version.Major < 6)
            {
                idDwmDllNotFound = true;
                return false;
            }

            try
            {
                return DwmIsCompositionEnabled();
            }
            catch (DllNotFoundException)
            {
                idDwmDllNotFound = true;
                return false;
            }
        }

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/aa969518%28v=vs.85%29.aspx</devdoc>
        [DllImport("dwmapi", PreserveSig = false, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// Sends a message to the message window and waits until the WndProc method has processed the message. 
        /// </summary>
        /// <param name="hWnd">Handle to destination window </param>
        /// <param name="Msg">Message </param>
        /// <param name="wParam">First message parameter </param>
        /// <param name="lParam">Second message parameter </param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/dd144901%28v=VS.85%29.aspx</devdoc>
        [DllImport("user32", EntryPoint = "GetMonitorInfoW", ExactSpelling = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMonitorInfo([In] IntPtr hMonitor, [Out] MONITORINFO lpmi);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms633528(v=vs.85).aspx</devdoc>
        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool IsWindow([In] [Optional] IntPtr hWnd);

        internal static IntPtr GetClassLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(GetClassLong32(hWnd, nIndex));
            }

            return GetClassLong64(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        private static extern uint GetClassLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        private static extern IntPtr GetClassLong64(IntPtr hWnd, int nIndex);

        internal static void PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
        {
            if (!_PostMessage(hWnd, Msg, wParam, lParam))
            {
                throw new Win32Exception();
            }
        }

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms644944(v=vs.85).aspx</devdoc>
        [DllImport("user32", EntryPoint = "PostMessage", SetLastError = true)]
        private static extern bool _PostMessage([In] [Optional] IntPtr hWnd, [In] uint Msg, [In] IntPtr wParam, [In] IntPtr lParam);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms647985(v=vs.85).aspx</devdoc>
        [DllImport("user32")]
        internal static extern IntPtr GetSystemMenu([In] IntPtr hWnd, [In] bool bRevert);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms648003(v=vs.85).aspx</devdoc>
        [DllImport("user32")]
        internal static extern uint TrackPopupMenuEx([In] IntPtr hmenu, [In] uint fuFlags, [In] int x, [In] int y, [In] IntPtr hwnd, [In] [Optional] IntPtr lptpm);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms684175%28v=vs.85%29.aspx</devdoc>
        [DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryW", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr LoadLibrary([In] [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms683152%28v=vs.85%29.aspx</devdoc>
        [DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary([In] IntPtr hModule);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms647486%28v=vs.85%29.aspx</devdoc>
        [DllImport("user32", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadStringW", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int LoadString([In] [Optional] IntPtr hInstance, [In] uint uID, [Out] StringBuilder lpBuffer, [In] int nBufferMax);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern WS GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, WS dwNewLong);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        /// <summary>Add and remove a native WindowStyle from the HWND.</summary>
        /// <param name="_hwnd">A HWND for a window.</param>
        /// <param name="removeStyle">The styles to be removed.  These can be bitwise combined.</param>
        /// <param name="addStyle">The styles to be added.  These can be bitwise combined.</param>
        /// <returns>Whether the styles of the HWND were modified as a result of this call.</returns>
        /// <SecurityNote>
        ///   Critical : Calls critical methods
        /// </SecurityNote>
        [SecurityCritical]
        public static bool _ModifyStyle(this IntPtr _hwnd, WS removeStyle, WS addStyle)
        {
            var dwStyle = GetWindowLong(_hwnd, GWL.STYLE);

            var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
            if (dwStyle == dwNewStyle)
            {
                return false;
            }

            SetWindowLong(_hwnd, GWL.STYLE, dwNewStyle);
            return true;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int ToUnicode(uint virtualKey, uint scanCode, byte[] keyStates, [MarshalAs(UnmanagedType.LPArray)] [Out] char[] chars, int charMaxCount, uint flags);

        [DllImport("user32.dll")]
        internal static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        // ReSharper disable InconsistentNaming
        internal enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// GetWindowLong values, GWL_*
        /// </summary>
        internal enum GWL : int
        {
            WNDPROC = -4,
            HINSTANCE = -6,
            HWNDPARENT = -8,
            STYLE = -16,
            EXSTYLE = -20,
            USERDATA = -21,
            ID = -12
        }

        /// <summary>
        /// WindowStyle values, WS_*
        /// </summary>
        [Flags]
        internal enum WS : uint
        {
            OVERLAPPED = 0x00000000,
            POPUP = 0x80000000,
            CHILD = 0x40000000,
            MINIMIZE = 0x20000000,
            VISIBLE = 0x10000000,
            DISABLED = 0x08000000,
            CLIPSIBLINGS = 0x04000000,
            CLIPCHILDREN = 0x02000000,
            MAXIMIZE = 0x01000000,
            BORDER = 0x00800000,
            DLGFRAME = 0x00400000,
            VSCROLL = 0x00200000,
            HSCROLL = 0x00100000,
            SYSMENU = 0x00080000,
            THICKFRAME = 0x00040000,
            GROUP = 0x00020000,
            TABSTOP = 0x00010000,

            MINIMIZEBOX = 0x00020000,
            MAXIMIZEBOX = 0x00010000,

            CAPTION = BORDER | DLGFRAME,
            TILED = OVERLAPPED,
            ICONIC = MINIMIZE,
            SIZEBOX = THICKFRAME,
            TILEDWINDOW = OVERLAPPEDWINDOW,

            OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
            POPUPWINDOW = POPUP | BORDER | SYSMENU,
            CHILDWINDOW = CHILD,
        }
    }
}