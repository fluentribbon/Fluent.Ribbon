// ReSharper disable once CheckNamespace
namespace Windows.Win32;

#pragma warning disable SA1307, SA1310, SA1401, SA1601, CA1060

using global::System;
using global::System.Runtime.InteropServices;

internal partial class PInvoke
{
    /// <summary>
    /// Creates a Color dialog box that enables the user to select a color.
    /// </summary>
    /// <param name="lpcc">A pointer to a CHOOSECOLOR structure that contains information used to initialize the dialog box. When ChooseColor returns, this structure contains information about the user's color selection.</param>
    /// <returns>If the user clicks the OK button of the dialog box, the return value is nonzero. The rgbResult member of the CHOOSECOLOR structure contains the RGB color value of the color selected by the user.If the user cancels or closes the Color dialog box or an error occurs, the return value is zero. </returns>
    [DllImport("comdlg32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ChooseColor(CHOOSECOLOR lpcc);

    /// <summary>
    /// Contains information the ChooseColor function uses to initialize the Color dialog box. After the user closes the dialog box, the system returns information about the user's selection in this structure. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class CHOOSECOLOR
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
#pragma warning disable CS0649
        public int rgbResult;
#pragma warning restore CS0649

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

    /// <summary>
    /// Causes the dialog box to display all available colors in the set of basic colors.
    /// </summary>
    internal const int CC_ANYCOLOR = 256;
}