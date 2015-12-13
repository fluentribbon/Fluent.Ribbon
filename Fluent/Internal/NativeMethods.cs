using System;
using System.Runtime.InteropServices;

namespace Fluent
{
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
        public static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

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
                return UnsafeNativeMethods.DwmIsCompositionEnabled();
            }
            catch (DllNotFoundException)
            {
                idDwmDllNotFound = true;
                return false;
            }
        }

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
    }
}