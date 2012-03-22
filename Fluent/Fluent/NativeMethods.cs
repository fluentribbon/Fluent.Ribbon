#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Fluent
{
    /// <summary>
    /// Native methods
    /// </summary>
    static internal class NativeMethods
    {
        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEHOOKSTRUCT
        {
            public NativeMethods.POINT pt;
            public IntPtr hwnd;
            public uint wHitTestCode;
            public IntPtr dwExtraInfo;
        } ;

        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam,
           IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn,
        IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_NCRENDERING_ENABLED = 1,      // [get] Is non-client rendering enabled/disabled
            DWMWA_NCRENDERING_POLICY,           // [set] Non-client rendering policy
            DWMWA_TRANSITIONS_FORCEDISABLED,    // [set] Potentially enable/forcibly disable transitions
            DWMWA_ALLOW_NCPAINT,                // [set] Allow contents rendered in the non-client area to be visible on the DWM-drawn frame.
            DWMWA_CAPTION_BUTTON_BOUNDS,        // [get] Bounds of the caption button area in window-relative space.
            DWMWA_NONCLIENT_RTL_LAYOUT,         // [set] Is non-client content RTL mirrored
            DWMWA_FORCE_ICONIC_REPRESENTATION,  // [set] Force this window to display iconic thumbnails.
            DWMWA_FLIP3D_POLICY,                // [set] Designates how Flip3D will treat the window.
            DWMWA_EXTENDED_FRAME_BOUNDS,        // [get] Gets the extended frame bounds rectangle in screen space
            DWMWA_HAS_ICONIC_BITMAP,            // [set] Indicates an available bitmap when there is no better thumbnail representation.
            DWMWA_DISALLOW_PEEK,                // [set] Don't invoke Peek on the window.
            DWMWA_EXCLUDED_FROM_PEEK,           // [set] LivePreview exclusion information
            DWMWA_LAST
        };

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, IntPtr pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref Rect pvAttribute, int cbAttribute);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        #region Consts

        /// <summary>
        /// Causes the dialog box to display all available colors in the set of basic colors. 
        /// </summary>
        public const int CC_ANYCOLOR = 0x00000100;
        /// <summary>
        /// Enables the hook procedure specified in the lpfnHook member of this structure. This flag is used only to initialize the dialog box.
        /// </summary>
        public const int CC_ENABLEHOOK = 0x00000010;
        /// <summary>
        /// The hInstance and lpTemplateName members specify a dialog box template to use in place of the default template. This flag is used only to initialize the dialog box.
        /// </summary>
        public const int CC_ENABLETEMPLATE = 0x00000020;
        /// <summary>
        /// The hInstance member identifies a data block that contains a preloaded dialog box template. The system ignores the lpTemplateName member if this flag is specified. This flag is used only to initialize the dialog box.
        /// </summary>
        public const int CC_ENABLETEMPLATEHANDLE = 0x00000040;
        /// <summary>
        /// Causes the dialog box to display the additional controls that allow the user to create custom colors. If this flag is not set, the user must click the Define Custom Color button to display the custom color controls.
        /// </summary>
        public const int CC_FULLOPEN = 0x00000002;
        /// <summary>
        /// Disables the Define Custom Color button.
        /// </summary>
        public const int CC_PREVENTFULLOPEN = 0x00000004;
        /// <summary>
        /// Causes the dialog box to use the color specified in the rgbResult member as the initial color selection.
        /// </summary>
        public const int CC_RGBINIT = 0x00000001;
        /// <summary>
        /// Causes the dialog box to display the Help button. The hwndOwner member must specify the window to receive the HELPMSGSTRING registered messages that the dialog box sends when the user clicks the Help button.
        /// </summary>
        public const int CC_SHOWHELP = 0x00000008;
        /// <summary>
        /// Causes the dialog box to display only solid colors in the set of basic colors. 
        /// </summary>
        public const int CC_SOLIDCOLOR = 0x00000008;


        /// <summary>
        /// A window receives this message when the user chooses a 
        /// command from the Window menu (formerly known as the system 
        /// or control menu) or when the user chooses the maximize button, 
        /// minimize button, restore button, or close button.
        /// </summary>
        public const int WM_SYSCOMMAND = 0x0112;

        /// <summary>
        /// 'Famous' undocumented WM_GETSYSMENU message to display own context menu
        /// </summary>
        public const int WM_GETSYSMENU = 0x313;

        /// <summary>
        /// Sent to all top-level windows when Desktop Window Manager (DWM) composition has been enabled or disabled. 
        /// </summary>
        public const int WM_DWMCOMPOSITIONCHANGED = 0X031E;

        /// <summary>
        /// The WM_NCRBUTTONUP message is posted when the user releases 
        /// the right mouse button while the cursor is within the nonclient area of 
        /// a window. This message is posted to the window that contains the cursor. 
        /// If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCRBUTTONUP = 0x00A5;

        /// <summary>
        /// The WM_NCACTIVATE message is sent to a window when its nonclient area needs to be changed to indicate an active or inactive state
        /// </summary>
        public const int WM_NCACTIVATE = 0x0086;

        /// <summary>
        /// The WM_PAINT message is sent when the system or another application makes a request to paint a portion of an application's window.
        /// </summary>
        public const int WM_PAINT = 0x000F;

        /// <summary>
        /// The WM_ERASEBKGND message is sent when the window background must be erased (for example, when a window is resized). The message is sent to prepare an invalidated portion of a window for painting. 
        /// </summary>
        public const int WM_ERASEBKGND = 0x0014;

        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that window to be redrawn or to prevent changes in that window from being redrawn. 
        /// </summary>
        public const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// The WM_CREATE message is sent when an application requests that a window be created by calling the CreateWindowEx or CreateWindow function. (The message is sent before the function returns.) The window procedure of the new window receives this message after the window is created, but before the window becomes visible
        /// </summary>
        public const int WM_CREATE = 0x0001;

        /// <summary>
        /// An application sends a WM_SETTEXT message to set the text of a window. 
        /// </summary>
        public const int WM_SETTEXT = 0x000C;
        /// <summary>
        /// An application sends the WM_SETICON message to associate a new large or small icon with a window. The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption. 
        /// </summary>
        public const int WM_SETICON = 0x0080;
        /// <summary>
        /// The WM_WINDOWPOSCHANGED message is sent to a window whose size, position, or place in the Z order has changed as a result of a call to the SetWindowPos function or another window-management function. 
        /// </summary>
        public const int WM_WINDOWPOSCHANGED = 0x0047;

        /// <summary>
        /// A message that is sent to all top-level windows when the SystemParametersInfo function changes a system-wide setting or when policy settings have changed. 
        /// </summary>
        public const int WM_SETTINGCHANGE = 0x001A;
        /// <summary>
        /// The WM_ENTERSIZEMOVE message is sent one time to a window after it enters the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the err! bad pv reference "SC_MOVE" [type 'winnotify' not supported.] or err! bad pv reference "SC_SIZE" [type 'winnotify' not supported.] value. The operation is complete when DefWindowProc returns. 
        /// </summary>
        public const int WM_ENTERSIZEMOVE = 0x0231;
        /// <summary>
        /// The WM_EXITSIZEMOVE message is sent one time to a window, after it has exited the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the err! bad pv reference "SC_MOV" [type 'winnotify' not supported.]E or err! bad pv reference "SC_SIZE" [type 'winnotify' not supported.] value. The operation is complete when DefWindowProc returns. 
        /// </summary>
        public const int WM_EXITSIZEMOVE = 0x0232;

        /// <summary>
        /// Used in combination with any other values, except WVR_VALIDRECTS, causes the window to be completely redrawn if the client rectangle changes size horizontally. This value is similar to CS_HREDRAW  class style
        /// </summary>
        public const int WVR_HREDRAW = 0x0100;
        /// <summary>
        /// Used in combination with any other values, except WVR_VALIDRECTS, causes the window to be completely redrawn if the client rectangle changes size vertically. This value is similar to CS_VREDRAW  class style
        /// </summary>
        public const int WVR_VREDRAW = 0x0200;
        /// <summary>
        /// This value causes the entire window to be redrawn. It is a combination of WVR_HREDRAW and WVR_VREDRAW values.
        /// </summary>
        public const int WVR_REDRAW = WVR_HREDRAW | WVR_VREDRAW;

        /// <summary>
        /// Enables the menu item so that it can be selected and restores it from its grayed state. 
        /// </summary>
        public const uint MF_ENABLED = 0;
        /// <summary>
        /// Indicates that the uPosition parameter specifies the identifier of the menu item (the default). 
        /// </summary>
        public const uint MF_BYCOMMAND = 0;
        /// <summary>
        /// Disables the menu item and grays it so that it cannot be selected. 
        /// </summary>
        public const uint MF_GRAYED = 1;
        /// <summary>
        /// Disables the menu item so that it cannot be selected, but this flag does not gray it. 
        /// </summary>
        public const uint MF_DISABLED = 2;

        /// <summary>
        /// Creates the intersection of the two combined regions.
        /// </summary>
        public const int RGN_AND = 1;

        /// <summary>
        /// Creates the union of two combined regions.
        /// </summary>
        public const int RGN_OR = 2;
        /// <summary>
        /// Creates the union of two combined regions except for any overlapping areas.
        /// </summary>
        public const int RGN_XOR = 3;
        /// <summary>
        /// Combines the parts of hrgnSrc1 that are not part of hrgnSrc2.
        /// </summary>
        public const int RGN_DIFF = 4;
        /// <summary>
        /// Creates a copy of the region identified by hrgnSrc1.
        /// </summary>
        public const int RGN_COPY = 5;

        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        public const int SW_HIDE = 0;
        /// <summary>
        /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
        /// </summary>
        public const int SW_SHOWNORMAL = 1;
        /// <summary>
        /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
        /// </summary>
        public const int SW_NORMAL = 1;
        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        public const int SW_SHOWMINIMIZED = 2;
        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>
        public const int SW_SHOWMAXIMIZED = 3;
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        public const int SW_MAXIMIZE = 3;
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
        /// </summary>
        public const int SW_SHOWNOACTIVATE = 4;
        /// <summary>
        /// Activates the window and displays it in its current size and position. 
        /// </summary>
        public const int SW_SHOW = 5;
        /// <summary>
        /// Minimizes the specified window and activates the next top-level window in the Z order.
        /// </summary>
        public const int SW_MINIMIZE = 6;
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
        /// </summary>
        public const int SW_SHOWMINNOACTIVE = 7;
        /// <summary>
        /// Displays the window in its current size and position. This value is similar to SW_SHOW, except the window is not activated.
        /// </summary>
        public const int SW_SHOWNA = 8;
        /// <summary>
        /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
        /// </summary>
        public const int SW_RESTORE = 9;
        /// <summary>
        /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application. 
        /// </summary>
        public const int SW_SHOWDEFAULT = 10;
        /// <summary>
        /// Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread. 
        /// </summary>
        public const int SW_FORCEMINIMIZE = 11;

        /// <summary>
        /// Sizes the window.
        /// </summary>
        public const int SC_SIZE = 0xF000;
        /// <summary>
        /// Moves the window.
        /// </summary>
        public const int SC_MOVE = 0xF010;
        /// <summary>
        /// Minimizes the window.
        /// </summary>
        public const int SC_MINIMIZE = 0xF020;
        /// <summary>
        /// Maximizes the window.
        /// </summary>
        public const int SC_MAXIMIZE = 0xF030;
        /// <summary>
        /// Moves to the next window.
        /// </summary>
        public const int SC_NEXTWINDOW = 0xF040;
        /// <summary>
        /// Moves to the previous window.
        /// </summary>
        public const int SC_PREVWINDOW = 0xF050;
        /// <summary>
        /// Closes the window.
        /// </summary>
        public const int SC_CLOSE = 0xF060;
        /// <summary>
        /// Scrolls vertically.
        /// </summary>
        public const int SC_VSCROLL = 0xF070;
        /// <summary>
        /// Scrolls horizontally.
        /// </summary>
        public const int SC_HSCROLL = 0xF080;
        /// <summary>
        /// Retrieves the window menu as a result of a mouse click.
        /// </summary>
        public const int SC_MOUSEMENU = 0xF090;
        /// <summary>
        /// Retrieves the window menu as a result of a keystroke.
        /// </summary>
        public const int SC_KEYMENU = 0xF100;
        /// <summary>
        /// 
        /// </summary>
        public const int SC_ARRANGE = 0xF110;
        /// <summary>
        /// Restores the window to its normal position and size.
        /// </summary>
        public const int SC_RESTORE = 0xF120;
        /// <summary>
        /// Activates the Start menu. 
        /// </summary>
        public const int SC_TASKLIST = 0xF130;
        /// <summary>
        /// Executes the screen saver application specified in the [boot] section of the System.ini file.
        /// </summary>
        public const int SC_SCREENSAVE = 0xF140;
        /// <summary>
        /// Activates the window associated with the application-specified hot key. The lParam parameter identifies the window to activate. 
        /// </summary>
        public const int SC_HOTKEY = 0xF150;
        /// <summary>
        /// Selects the default item; the user double-clicked the window menu.
        /// </summary>
        public const int SC_DEFAULT = 0xF160;
        /// <summary>
        /// Sets the state of the display. This command supports devices that have power-saving features, such as a battery-powered personal computer. 
        /// The lParam parameter can have the following values: 
        /// -1 - the display is powering on
        /// 1 - the display is going to low power
        /// 2 - the display is being shut off
        /// </summary>
        public const int SC_MONITORPOWER = 0xF170;
        /// <summary>
        /// Changes the cursor to a question mark with a pointer. If the user then clicks a control in the dialog box, the control receives a WM_HELP message. 
        /// </summary>
        public const int SC_CONTEXTHELP = 0xF180;

        /// <summary>
        /// On the screen background or on a dividing line between windows.
        /// </summary>
        public const int HTNOWHERE = 0;
        /// <summary>
        /// In a client area.
        /// </summary>
        public const int HTCLIENT = 1;
        /// <summary>
        /// In a title bar
        /// </summary>
        public const int HTCAPTION = 2;
        /// <summary>
        /// In a system menu
        /// </summary>
        public const int HTSYSMENU = 3;
        /// <summary>
        /// In the left border of a resizable window (the user can click the mouse to resize the window horizontally).
        /// </summary>
        public const int HTLEFT = 10;
        /// <summary>
        /// In the right border of a resizable window (the user can click the mouse to resize the window horizontally).
        /// </summary>
        public const int HTRIGHT = 11;
        /// <summary>
        /// In the upper-horizontal border of a window.
        /// </summary>
        public const int HTTOP = 12;
        /// <summary>
        /// In the upper-left corner of a window border.
        /// </summary>
        public const int HTTOPLEFT = 13;
        /// <summary>
        /// In the upper-right corner of a window border.
        /// </summary>
        public const int HTTOPRIGHT = 14;
        /// <summary>
        /// In the lower-horizontal border of a resizable window (the user can click the mouse 
        /// to resize the window vertically).
        /// </summary>
        public const int HTBOTTOM = 15;
        /// <summary>
        /// In the lower-left corner of a border of a resizable window 
        /// (the user can click the mouse to resize the window diagonally).
        /// </summary>
        public const int HTBOTTOMLEFT = 16;
        /// <summary>
        /// In the lower-right corner of a border of a resizable window 
        /// (the user can click the mouse to resize the window diagonally).
        /// </summary>
        public const int HTBOTTOMRIGHT = 17;


        /// <summary>
        /// The WM_NCHITTEST message is sent to a window when the cursor 
        /// moves, or when a mouse button is pressed or released. If the 
        /// mouse is not captured, the message is sent to the window beneath 
        /// the cursor. Otherwise, the message is sent to the window 
        /// that has captured the mouse.
        /// </summary>
        public const int WM_NCHITTEST = 0x0084;

        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted. 
        /// </summary>
        public const int WM_NCPAINT = 0x0085;

        /// <summary>
        /// The WM_NCCREATE message is sent prior to the WM_CREATE message when a window is first created
        /// </summary>
        public const int WM_NCCREATE = 0x0081;
        /// <summary>
        /// The WM_NCDESTROY message informs a window that its nonclient area is being destroyed. 
        /// The DestroyWindow function sends the WM_NCDESTROY message to the window following the 
        /// WM_DESTROY message. WM_DESTROY is used to free the allocated memory object associated 
        /// with the window. The WM_NCDESTROY message is sent after the child windows 
        /// have been destroyed. In contrast, WM_DESTROY is sent before the child windows are destroyed.
        /// </summary>
        public const int WM_NCDESTROY = 0x0082;

        /// <summary>
        /// The WM_NCCALCSIZE message is sent when the size and position of a window's client 
        /// area must be calculated. By processing this message, an application can control the 
        /// content of the window's client area when the size or position of the window changes.
        /// </summary>
        public const int WM_NCCALCSIZE = 0x0083;

        /// <summary>
        /// The WM_GETMINMAXINFO message is sent to a window when the size or position of 
        /// the window is about to change. An application can use this message to override 
        /// the window's default maximized size and position, or its default minimum or maximum tracking size. 
        /// </summary>
        public const int WM_GETMINMAXINFO = 0x0024;

        /// <summary>
        /// The WM_SIZE message is sent to a window after its size has changed.
        /// </summary>
        public const int WM_SIZE = 0x0005;

        /// <summary>
        /// The WM_MOVE message is sent after a window has been moved
        /// </summary>
        public const int WM_MOVE = 0x0003;

        /// <summary>
        /// The window has been resized, but neither the SIZE_MINIMIZED nor SIZE_MAXIMIZED value applies.
        /// </summary>
        public const int SIZE_RESTORED = 0;
        /// <summary>
        /// The window has been minimized.
        /// </summary>
        public const int SIZE_MINIMIZED = 1;
        /// <summary>
        /// The window has been maximized.
        /// </summary>
        public const int SIZE_MAXIMIZED = 2;

        /// <summary>
        /// Creates a pop-up window. This style cannot be used with the WS_CHILD style
        /// </summary>
        public const int WS_POPUP = -2147483648;//0x80000000;
        /// <summary>
        /// Creates a window that is initially visible.
        /// </summary>
        public const int WS_VISIBLE = 0x10000000;
        /// <summary>
        /// Clips child windows relative to each other; that is, 
        /// when a particular child window receives a WM_PAINT message, 
        /// the WS_CLIPSIBLINGS style clips all other overlapping child windows 
        /// out of the region of the child window to be updated. 
        /// If WS_CLIPSIBLINGS is not specified and child windows overlap, 
        /// it is possible, when drawing within the client area of a child window, 
        /// to draw within the client area of a neighboring child window.
        /// </summary>
        public const int WS_CLIPSIBLINGS = 0x04000000;
        /// <summary>
        /// Excludes the area occupied by child windows when drawing occurs within the parent window. 
        /// This style is used when creating the parent window.
        /// </summary>
        public const int WS_CLIPCHILDREN = 0x02000000;
        /// <summary>
        /// Creates a window that has a title bar (includes the WS_BORDER style).
        /// </summary>
        public const int WS_CAPTION = 0x00C00000;
        /// <summary>
        /// Creates a window that has a sizing border. Same as the WS_SIZEBOX style.
        /// </summary>
        public const int WS_THICKFRAME = 0x00040000;
        /// <summary>
        /// Creates a window that has a thin-line border.
        /// </summary>
        public const int WS_BORDER = 0x00800000;
        /// <summary>
        /// Creates a window that has a window menu on its title bar. The WS_CAPTION style must also be specified.
        /// </summary>
        public const int WS_SYSMENU = 0x00080000;
        /// <summary>
        /// Creates a window that has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified. 
        /// </summary>
        public const long WS_MINIMIZEBOX = 0x00020000L;
        /// <summary>
        /// Creates a window that has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified. 
        /// </summary>
        public const long WS_MAXIMIZEBOX = 0x00010000L;
        /// <summary>
        /// Creates an overlapped window. An overlapped window has a title bar and a border. Same as the WS_TILED style.
        /// </summary>
        public const long WS_OVERLAPPED = 0x00000000L;

        /// <summary>
        /// Creates a window that is initially maximized.
        /// </summary>
        public const long WS_MAXIMIZE = 0x01000000L;
        /// <summary>
        /// Creates a window that is initially minimized. Same as the WS_ICONIC style.
        /// </summary>
        public const long WS_MINIMIZE = 0x20000000L;


        /// <summary>
        /// Gives window generic left-aligned properties. This is the default.
        /// </summary>
        public const int WS_EX_LEFT = 0x00000000;
        /// <summary>
        /// Displays the window text using left-to-right reading order properties. This is the default. 
        /// </summary>
        public const int WS_EX_LTRREADING = 0x00000000;
        /// <summary>
        /// Places a vertical scroll bar (if present) to the right of the client area. This is the default. 
        /// </summary>
        public const int WS_EX_RIGHTSCROLLBAR = 0x00000000;
        /// <summary>
        /// Specifies that a window has a border with a raised edge.
        /// </summary>
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        /// <summary>
        /// Forces a top-level window onto the taskbar when the window is visible
        /// </summary>
        public const int WS_EX_APPWINDOW = 0x00040000;

        /// <summary>
        /// Sets a new window style.
        /// </summary>
        public const int GWL_STYLE = (-16);
        /// <summary>
        /// Sets a new extended window style. For more information, see CreateWindowEx. 
        /// </summary>
        public const int GWL_EXSTYLE = (-20);

        /// <summary>
        /// Retrieves a handle to the parent window, if any
        /// </summary>
        public const int GWL_HWNDPARENT = (-8);

        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        public const int HWND_TOP = 0;

        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). 
        /// This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        public const int HWND_NOTOPMOST = -2;

        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        public const int SWP_NOSIZE = 0x0001;
        /// <summary>
        /// Retains the current position (ignores X and Y parameters).
        /// </summary>
        public const int SWP_NOMOVE = 0x0002;
        /// <summary>
        /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        /// </summary>
        public const int SWP_NOACTIVATE = 0x0010;
        /// <summary>
        /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request. 
        /// </summary>
        public const int SWP_ASYNCWINDOWPOS = 0x4000;
        /// <summary>
        /// Prevents generation of the WM_SYNCPAINT message. 
        /// </summary>
        public const int SWP_DEFERERASE = 0x2000;
        /// <summary>
        /// Draws a frame (defined in the window's class description) around the window.
        /// </summary>
        public const int SWP_DRAWFRAME = 0x0020;
        /// <summary>
        /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed. 
        /// </summary>
        public const int SWP_FRAMECHANGED = 0x0020;
        /// <summary>
        /// Hides the window.
        /// </summary>
        public const int SWP_HIDEWINDOW = 0x0080;
        /// <summary>
        /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
        /// </summary>
        public const int SWP_NOCOPYBITS = 0x0100;
        /// <summary>
        /// Does not change the owner window's position in the Z order.
        /// </summary>
        public const int SWP_NOOWNERZORDER = 0x0200;
        /// <summary>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        /// </summary>
        public const int SWP_NOREDRAW = 0x0008;
        /// <summary>
        /// Same as the SWP_NOOWNERZORDER flag.
        /// </summary>
        public const int SWP_NOREPOSITION = 0x0200;
        /// <summary>
        /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message. 
        /// </summary>
        public const int SWP_NOSENDCHANGING = 0x0400;
        /// <summary>
        /// Retains the current Z order (ignores the hWndInsertAfter parameter). 
        /// </summary>
        public const int SWP_NOZORDER = 0x0004;
        /// <summary>
        /// Displays the window.
        /// </summary>
        public const int SWP_SHOWWINDOW = 0x0040;

        #endregion

        #region Structs

        /// <summary>
        /// Contains information the ChooseColor function uses to initialize the Color dialog box. After the user closes the dialog box, the system returns information about the user's selection in this structure. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class CHOOSECOLOR        
        {
            /// <summary>
            /// The length, in bytes, of the structure. 
            /// </summary>
            public int lStructSize = Marshal.SizeOf(typeof(NativeMethods.CHOOSECOLOR));
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
        } ;

        /// <summary>
        /// The NCCALCSIZE_PARAMS structure contains information that an application can use while processing the WM_NCCALCSIZE message to calculate the size, position, and valid contents of the client area of a window. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NCCALCSIZE_PARAMS
        {
            /// <summary>
            /// Specifies an array of rectangles. The meaning of the array of rectangles changes during the processing of the WM_NCALCSIZE message
            /// </summary>
            public Rect rect0;
            /// <summary>
            /// Specifies an array of rectangles. The meaning of the array of rectangles changes during the processing of the WM_NCALCSIZE message
            /// </summary>
            public Rect rect1;
            /// <summary>
            /// Specifies an array of rectangles. The meaning of the array of rectangles changes during the processing of the WM_NCALCSIZE message
            /// </summary>
            public Rect rect2;
            /// <summary>
            /// Pointer to a WINDOWPOS structure that contains the size and position values specified in the operation that moved or resized the window. 
            /// </summary>
            public IntPtr lppos;
        }

        /// <summary>
        /// The WINDOWPOS structure contains information about the size and position of a window. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            /// <summary>
            /// Identifies the window. 
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// Identifies the window behind which this window is placed. 
            /// </summary>
            public IntPtr hwndInsertAfter;
            /// <summary>
            /// Specifies the position of the left edge of the window. 
            /// </summary>
            public int x;
            /// <summary>
            /// Specifies the position of the right edge of the window. 
            /// </summary>
            public int y;
            /// <summary>
            /// Specifies the window width, in pixels. 
            /// </summary>
            public int cx;
            /// <summary>
            /// Specifies the window height, in pixels. 
            /// </summary>
            public int cy;
            /// <summary>
            /// Specifies window-positioning options. 
            /// </summary>
            public int flags;
        }

        /// <summary>
        /// The RECT structure defines a rectangle by the coordinates of its upper-left and lower-right corners.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            /// <summary>
            /// Specifies the x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Left;
            /// <summary>
            /// Specifies the y-coordinate of the upper-left corner of the rectangle. 
            /// </summary>
            public int Top;
            /// <summary>
            /// Specifies the x-coordinate of the lower-right corner of the rectangle. 
            /// </summary>
            public int Right;
            /// <summary>
            /// Specifies the y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Bottom;
        }

        /// <summary>
        /// The MONITORINFO structure contains information about a display monitor.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MonitorInfo
        {
            /// <summary>
            /// The size of the structure, in bytes. 
            /// </summary>
            public int Size = Marshal.SizeOf(typeof(MonitorInfo));
            /// <summary>
            /// A RECT structure that specifies the display monitor rectangle, expressed 
            /// in virtual-screen coordinates. 
            /// Note that if the monitor is not the primary display monitor, 
            /// some of the rectangle's coordinates may be negative values. 
            /// </summary>
            public Rect Monitor;
            /// <summary>
            /// A RECT structure that specifies the work area rectangle of the display monitor, 
            /// expressed in virtual-screen coordinates. Note that if the monitor is not the primary 
            /// display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            public Rect Work;
            /// <summary>
            /// A set of flags that represent attributes of the display monitor. 
            /// </summary>
            public uint Flags;
        }

        /// <summary>
        /// Returned by the GetThemeMargins function to define the margins of windows that have visual styles applied. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MARGINS
        {
            /// <summary>
            /// Width of the left border that retains its size.
            /// </summary>
            public int cxLeftWidth;
            /// <summary>
            /// Width of the right border that retains its size.
            /// </summary>
            public int cxRightWidth;
            /// <summary>
            /// Height of the top border that retains its size.
            /// </summary>
            public int cyTopHeight;
            /// <summary>
            /// Height of the bottom border that retains its size.
            /// </summary>
            public int cyBottomHeight;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="left"> Width of the left border that retains its size.</param>
            /// <param name="top">Height of the top border that retains its size.</param>
            /// <param name="right">Width of the right border that retains its size.</param>
            /// <param name="bottom">Height of the bottom border that retains its size.</param>
            public MARGINS(int left, int top, int right, int bottom)
            {
                cxLeftWidth = left; cyTopHeight = top;
                cxRightWidth = right; cyBottomHeight = bottom;
            }
        }

        /// <summary>
        /// The POINT structure defines the x- and y- coordinates of a point. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// Specifies the x-coordinate of the point. 
            /// </summary>
            public int x;

            /// <summary>
            /// Specifies the y-coordinate of the point. 
            /// </summary>
            public int y;
        }

        /// <summary>
        /// The MINMAXINFO structure contains information about a window's maximized size 
        /// and position and its minimum and maximum tracking size. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            /// <summary>
            /// Reserved; do not use.
            /// </summary>
            public POINT ptReserved;
            /// <summary>
            /// Specifies the maximized width (POINT.x) and the maximized height (POINT.y) of the window. 
            /// For top-level windows, this value is based on the width of the primary monitor.
            /// </summary>
            public POINT ptMaxSize;
            /// <summary>
            /// Specifies the position of the left side of the maximized window (POINT.x) and the
            /// position of the top of the maximized window (POINT.y). For top-level windows, this 
            /// value is based on the position of the primary monitor.
            /// </summary>
            public POINT ptMaxPosition;
            /// <summary>
            /// Specifies the minimum tracking width (POINT.x) and the minimum tracking height 
            /// (POINT.y) of the window. This value can be obtained programmatically from the 
            /// system metrics SM_CXMINTRACK and SM_CYMINTRACK.
            /// </summary>
            public POINT ptMinTrackSize;
            /// <summary>
            /// Specifies the maximum tracking width (POINT.x) and the maximum tracking 
            /// height (POINT.y) of the window. This value is based on the size of the virtual 
            /// screen and can be obtained programmatically from the system 
            /// metrics SM_CXMAXTRACK and SM_CYMAXTRACK.
            /// </summary>
            public POINT ptMaxTrackSize;
        } ;

        /// <summary>
        /// The WINDOWINFO structure contains window information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            /// <summary>
            /// The size of the structure, in bytes. The caller must set this to sizeof(WINDOWINFO). 
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// Pointer to a RECT structure that specifies the coordinates of the window. 
            /// </summary>
            public Rect rcWindow;
            /// <summary>
            /// Pointer to a RECT structure that specifies the coordinates of the client area
            /// </summary>
            public Rect rcClient;
            /// <summary>
            /// The window styles. For a table of window styles, see CreateWindowEx.
            /// </summary>
            public uint dwStyle;
            /// <summary>
            /// The extended window styles. For a table of extended window styles, see CreateWindowEx
            /// </summary>
            public uint dwExStyle;
            /// <summary>
            /// The window status. If this member is WS_ACTIVECAPTION, the window is active. Otherwise, this member is zero
            /// </summary>
            public uint dwWindowStatus;
            /// <summary>
            /// The width of the window border, in pixels.
            /// </summary>
            public uint cxWindowBorders;
            /// <summary>
            /// The height of the window border, in pixels. 
            /// </summary>
            public uint cyWindowBorders;
            /// <summary>
            /// The window class atom (see RegisterClass). 
            /// </summary>
            public ushort atomWindowType;
            /// <summary>
            /// The Microsoft Windows version of the application that created the window. 
            /// </summary>
            public ushort wCreatorVersion;
        }

        /// <summary>
        /// A ratio used with the Desktop Window Manager (DWM) timing API.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct UNSIGNED_RATIO
        {
            /// <summary>
            /// The ratio numerator.
            /// </summary>
            public uint uiNumerator;
            /// <summary>
            /// The ratio denominator
            /// </summary>
            public uint uiDenominator;
        }

        /// <summary>
        /// Contains Desktop Window Manager (DWM) composition timing information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DWM_TIMING_INFO
        {
            /// <summary>
            /// The size of this DWM_TIMING_INFO structure. 
            /// </summary>
            public int cbSize;
            /// <summary>
            /// The monitor refresh rate
            /// </summary>
            public UNSIGNED_RATIO rateRefresh;
            /// <summary>
            /// The monitor refresh rate.
            /// </summary>
            public ulong qpcRefreshPeriod;
            /// <summary>
            /// The composition rate.
            /// </summary>
            public UNSIGNED_RATIO rateCompose;
            /// <summary>
            /// The query performance counter value before the vertical blank.
            /// </summary>
            public ulong qpcVBlank;
            /// <summary>
            /// The DWM refresh counter.
            /// </summary>
            public ulong cRefresh;
            /// <summary>
            /// The Microsoft DirectX refresh counter.
            /// </summary>
            public uint cDXRefresh;
            /// <summary>
            /// The query performance counter value for a frame composition.
            /// </summary>
            public ulong qpcCompose;
            /// <summary>
            /// The frame number that was composed at qpcCompose. 
            /// </summary>
            public ulong cFrame;
            /// <summary>
            /// The DirectX present number used to identify rendering frames.
            /// </summary>
            public uint cDXPresent;
            /// <summary>
            /// The refresh count of the frame that was composed at qpcCompose. 
            /// </summary>
            public ulong cRefreshFrame;
            /// <summary>
            /// The DWM frame number that was last submitted.
            /// </summary>
            public ulong cFrameSubmitted;
            /// <summary>
            /// The DirectX present number that was last submitted.
            /// </summary>
            public uint cDXPresentSubmitted;
            /// <summary>
            /// The DWM frame number that was last confirmed as presented.
            /// </summary>
            public ulong cFrameConfirmed;
            /// <summary>
            /// The DirectX present number that was last confirmed as presented.
            /// </summary>
            public uint cDXPresentConfirmed;
            /// <summary>
            /// The target refresh count of the last frame confirmed completed by the graphics processing unit (GPU).
            /// </summary>
            public ulong cRefreshConfirmed;
            /// <summary>
            /// The DirectX refresh count when the frame was confirmed as presented.
            /// </summary>
            public uint cDXRefreshConfirmed;
            /// <summary>
            /// The number of frames the DWM presented late.
            /// </summary>
            public ulong cFramesLate;
            /// <summary>
            /// The number of composition frames that have been issued but have not been confirmed as completed.
            /// </summary>
            public uint cFramesOutstanding;
            /// <summary>
            /// The last frame displayed.
            /// </summary>
            public ulong cFrameDisplayed;
            /// <summary>
            /// The query performance counter (QPC) time of the composition pass when the frame was displayed.
            /// </summary>
            public ulong qpcFrameDisplayed;
            /// <summary>
            /// The vertical refresh count when the frame should have become visible.
            /// </summary>
            public ulong cRefreshFrameDisplayed;
            /// <summary>
            /// The ID of the last frame marked complete.
            /// </summary>
            public ulong cFrameComplete;
            /// <summary>
            /// /The QPC time when the last frame was marked as completed.
            /// </summary>
            public ulong qpcFrameComplete;
            /// <summary>
            /// The ID of the last frame marked as pending.
            /// </summary>
            public ulong cFramePending;
            /// <summary>
            /// The QPC time when the last frame was marked pending.
            /// </summary>
            public ulong qpcFramePending;
            /// <summary>
            /// The number of unique frames displayed. This value is valid only after a second call to DwmGetCompositionTimingInfo. 
            /// </summary>
            public ulong cFramesDisplayed;
            /// <summary>
            /// The number of new completed frames that have been received.
            /// </summary>
            public ulong cFramesComplete;
            /// <summary>
            /// The number of new frames submitted to DirectX but not yet completed.
            /// </summary>
            public ulong cFramesPending;
            /// <summary>
            /// The number of frames available but not displayed, used, or dropped. This value is valid only after a second call to DwmGetCompositionTimingInfo. 
            /// </summary>
            public ulong cFramesAvailable;
            /// <summary>
            /// The number of rendered frames that were never displayed because composition occurred too late. This value is valid only after a second call to DwmGetCompositionTimingInfo. 
            /// </summary>
            public ulong cFramesDropped;
            /// <summary>
            /// The number of times an old frame was composed when a new frame should have been used but was not available.
            /// </summary>
            public ulong cFramesMissed;
            /// <summary>
            /// The frame count at which the next frame is scheduled to be displayed.
            /// </summary>
            public ulong cRefreshNextDisplayed;
            /// <summary>
            /// The frame count at which the next DirectX present is scheduled to be displayed.
            /// </summary>
            public ulong cRefreshNextPresented;
            /// <summary>
            /// The total number of refreshes that have been displayed for the application since DwmSetPresentParameters was last called. 
            /// </summary>
            public ulong cRefreshesDisplayed;
            /// <summary>
            /// The total number of refreshes that have been presented by the application since DwmSetPresentParameters was last called. 
            /// </summary>
            public ulong cRefreshesPresented;
            /// <summary>
            /// The refresh number when content for this window started to be displayed.
            /// </summary>
            public ulong cRefreshStarted;
            /// <summary>
            /// The total number of pixels DirectX redirected to the DWM.
            /// </summary>
            public ulong cPixelsReceived;
            /// <summary>
            /// The number of pixels drawn.
            /// </summary>
            public ulong cPixelsDrawn;
            /// <summary>
            /// The number of empty buffers in the flip chain.
            /// </summary>
            public ulong cBuffersEmpty;
        }

        /// <summary>
        /// The WINDOWPLACEMENT structure contains information about the placement of a window on the screen. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class WINDOWPLACEMENT
        {
            /// <summary>
            /// Specifies the length, in bytes, of the structure. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof( WINDOWPLACEMENT). 
            /// </summary>
            public int length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            /// <summary>
            /// Specifies flags that control the position of the minimized window and the method by which the window is restored. This member can be one or more of the following values. 
            /// </summary>
            public int flags;
            /// <summary>
            /// Specifies the current show state of the window. This member can be one of the following values. 
            /// </summary>
            public int showCmd;
            /// <summary>
            /// Specifies the coordinates of the window's upper-left corner when the window is minimized. 
            /// </summary>
            public POINT ptMinPosition;
            /// <summary>
            /// Specifies the coordinates of the window's upper-left corner when the window is maximized. 
            /// </summary>
            public POINT ptMaxPosition;
            /// <summary>
            /// Specifies the window's coordinates when the window is in the restored position. 
            /// </summary>
            public Rect rcNormalPosition;
        }

        #endregion

        #region Functions

        /// <summary>
        /// The DefWindowProc function calls the default window procedure to provide default processing for any window messages that an application does not process. This function ensures that every message is processed. DefWindowProc is called with the same parameters received by the window procedure. 
        /// </summary>
        /// <param name="hWnd">Handle to the window procedure that received the message. </param>
        /// <param name="Msg">Specifies the message. </param>
        /// <param name="wParam">Specifies additional message information. The content of this parameter depends on the value of the Msg parameter. </param>
        /// <param name="lParam">Specifies additional message information. The content of this parameter depends on the value of the Msg parameter. </param>
        /// <returns>The return value is the result of the message processing and depends on the message.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

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
        public static extern IntPtr MonitorFromRect([In] ref Rect lprc, uint dwFlags);

        /// <summary>
        /// The MonitorFromWindow function retrieves a handle to the display monitor that has the largest area of intersection with the bounding rectangle of a specified window. 
        /// </summary>
        /// <param name="hwnd">A handle to the window of interest.</param>
        /// <param name="dwFlags">Determines the function's return value if the window does not intersect any display monitor.</param>
        /// <returns>If the window intersects one or more display monitor rectangles, the return value is an HMONITOR handle to the display monitor that has the largest area of intersection with the window. 
        /// If the window does not intersect a display monitor, the return value depends on the value of dwFlags.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        /// <summary>
        /// The CreateRectRgnIndirect function creates a rectangular region. 
        /// </summary>
        /// <param name="lprc">Pointer to a RECT structure that contains the coordinates of the upper-left and lower-right corners of the rectangle that defines the region in logical units. </param>
        /// <returns>If the function succeeds, the return value is the handle to the region.If the function fails, the return value is NULL.</returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateRectRgnIndirect", SetLastError = true)]
        public static extern IntPtr CreateRectRgnIndirect([In] ref Rect lprc);

        /// <summary>
        /// The CombineRgn function combines two regions and stores the result in a third region. The two regions are combined according to the specified mode. 
        /// </summary>
        /// <param name="hrgnDest">A handle to a new region with dimensions defined by combining two other regions. (This region must exist before CombineRgn is called.) </param>
        /// <param name="hrgnSrc1">A handle to the first of two regions to be combined.</param>
        /// <param name="hrgnSrc2">A handle to the second of two regions to be combined.</param>
        /// <param name="fnCombineMode">A mode indicating how the two regions will be combined. </param>
        /// <returns>The return value specifies the type of the resulting region. </returns>
        [DllImport("gdi32.dll")]
        public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);

        /// <summary>
        /// The GetMonitorInfo function retrieves information about a display monitor. 
        /// </summary>
        /// <param name="hMonitor">Handle to the display monitor of interest.</param>
        /// <param name="lpmi">Pointer to a MONITORINFO or MONITORINFOEX structure that receives 
        /// information about the specified display monitor</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out] MonitorInfo lpmi);

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

        /// <summary>
        /// This function makes the specified top-level window associated with the thread 
        /// calling this function the active window. 
        /// </summary>
        /// <param name="hWnd">Handle to the top-level window to be activated. </param>
        /// <returns>The handle to the window that was previously active indicates success. 
        /// NULL indicates failure.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        /// <summary>
        /// The SetForegroundWindow function puts the thread that created the specified window into 
        /// the foreground and activates the window. Keyboard input is directed to the window, 
        /// and various visual cues are changed for the user. The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads. 
        /// </summary>
        /// <param name="hWnd">Handle to the window that should be activated and brought to the foreground. </param>
        /// <returns>TIf the window was brought to the foreground, the return value is nonzero. 
        /// If the window was not brought to the foreground, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        [SuppressMessage("Microsoft.Performance", "CA1811")]
        [SuppressMessage("Microsoft.Portability", "CA1901")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// The GetActiveWindow function retrieves the window handle to the active 
        /// window attached to the calling thread's message queue
        /// </summary>
        /// <returns>The return value is the handle to the active window attached to the calling thread's message queue. 
        /// Otherwise, the return value is NULL. </returns>
        [DllImport("user32.dll")]
        [SuppressMessage("Microsoft.Performance", "CA1811")]
        public static extern IntPtr GetActiveWindow();

        /// <summary>
        /// Default window procedure for Desktop Window Manager (DWM) hit testing within the non-client area.
        /// </summary>
        /// <param name="hwnd">Handle to the window procedure that received the message.</param>
        /// <param name="msg">Specifies the message.</param>
        /// <param name="wParam">Specifies additional message information. The content of this parameter depends on the value of the msg parameter. </param>
        /// <param name="lParam">Specifies additional message information. The content of this parameter depends on the value of the msg parameter. </param>
        /// <param name="plResult">Pointer to an LRESULT value that, when this method returns, receives the result of the hit test.</param>
        /// <returns>TRUE if DwmDefWindowProc handled the message; otherwise, FALSE. </returns>
        [DllImport("dwmapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);

        //Retrieves the current composition timing information.
        [DllImport("dwmapi.dll", EntryPoint = "DwmGetCompositionTimingInfo")]
        private static extern IntPtr DwmGetCompositionTimingInfo(IntPtr hwnd, ref DWM_TIMING_INFO pTimingInfo);

        /// <summary>
        /// Retrieves the current composition timing information.
        /// </summary>
        /// <param name="hwnd">The handle to the window for which the composition timing information should be retrieved.</param>
        /// <returns>A pointer to a DWM_TIMING_INFO structure that, when this function returns successfully, receives the current composition timing information for the window. </returns>
        public static DWM_TIMING_INFO? DwmGetCompositionTimingInfo(IntPtr hwnd)
        {
            if (Environment.OSVersion.Version < new Version("6.0"))
            {
                // API was new to Vista.
                return null;
            }

            var dti = new DWM_TIMING_INFO { cbSize = Marshal.SizeOf(typeof(DWM_TIMING_INFO)) };
            DwmGetCompositionTimingInfo(hwnd, ref dti);

            return dti;
        }

        /// <summary>
        /// Extends the window frame behind the client area.
        /// </summary>
        /// <param name="hWnd">The handle to the window for which the frame is extended into the client area</param>
        /// <param name="pMargins">The pointer to a MARGINS Structure structure 
        /// that describes the margins to use when extending the frame into the client area.</param>
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, MARGINS pMargins);

        /// <summary>
        /// Is DWM enabled
        /// </summary>
        /// <returns>Is DWM enabled</returns>
        [DllImport("dwmapi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmIsCompositionEnabled();

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

        /// <summary>
        /// The IsWindowVisible function retrieves the visibility state of the specified window. 
        /// </summary>
        /// <param name="hwnd">Handle to the window to test. </param>
        /// <returns>If the specified window, its parent window, its parent's parent window, and so forth, have the WS_VISIBLE style, the return value is nonzero. Otherwise, the return value is zero. </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hwnd);


        /// <summary>
        /// The EnableMenuItem function enables, disables, or grays the specified menu item. 
        /// </summary>
        /// <param name="hMenu">Handle to the menu.</param>
        /// <param name="uIDEnableItem">Specifies the menu item to be enabled, disabled, or grayed, as determined by the uEnable parameter. This parameter specifies an item in a menu bar, menu, or submenu. </param>
        /// <param name="uEnable">Controls the interpretation of the uIDEnableItem parameter and indicate whether the menu item is enabled, disabled, or grayed. </param>
        /// <returns>The return value specifies the previous state of the menu item (it is either MF_DISABLED, MF_ENABLED, or MF_GRAYED). If the menu item does not exist, the return value is -1.</returns>
        [DllImport("user32.dll", EntryPoint = "EnableMenuItem")]
        public static extern int EnableMenuItem(IntPtr hMenu, int uIDEnableItem, uint uEnable);

        /// <summary>
        /// The TrackPopupMenuEx function displays a shortcut menu at the specified location and 
        /// tracks the selection of items on the shortcut menu. The shortcut menu can appear anywhere on the screen.
        /// </summary>
        /// <param name="hmenu">Handle to the shortcut menu to be displayed. This handle can be 
        /// obtained by calling the CreatePopupMenu function to create a new shortcut menu or 
        /// by calling the GetSubMenu function to retrieve a handle to a submenu associated with an existing menu item.</param>
        /// <param name="fuFlags">Specifies function options</param>
        /// <param name="x">Horizontal location of the shortcut menu, in screen coordinates.</param>
        /// <param name="y">Vertical location of the shortcut menu, in screen coordinates</param>
        /// <param name="hwnd">Handle to the window that owns the shortcut menu. This window receives 
        /// all messages from the menu. The window does not receive a WM_COMMAND message from the menu 
        /// until the function returns. If you specify TPM_NONOTIFY in the fuFlags parameter, the 
        /// function does not send messages to the window identified by hwnd. However, you must still
        ///  pass a window handle in hwnd. It can be any window handle from your application.</param>
        /// <param name="lptpm">Pointer to a TPMPARAMS structure that specifies an area of the screen 
        /// the menu should not overlap. This parameter can be NULL.</param>
        /// <returns>If you specify TPM_RETURNCMD in the fuFlags parameter, the return value is the 
        /// menu-item identifier of the item that the user selected. If the user cancels the menu 
        /// without making a selection, or if an error occurs, then the return value is zero.</returns>
        [DllImport("User32.dll")]
        public static extern uint TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        /// <summary>
        /// The GetSystemMenu function allows the application to access the window menu 
        /// (also known as the system menu or the control menu) for copying and modifying. 
        /// </summary>
        /// <param name="hWnd">Handle to the window that will own a copy of the window menu.</param>
        /// <param name="bRevert">Specifies the action to be taken. If this parameter is FALSE, 
        /// GetSystemMenu returns a handle to the copy of the window menu currently in use. 
        /// The copy is initially identical to the window menu, but it can be modified. 
        /// If this parameter is TRUE, GetSystemMenu resets the window menu back to the default state. 
        /// The previous window menu, if any, is destroyed.</param>
        /// <returns>If the bRevert parameter is FALSE, the return value is a handle to a copy of 
        /// the window menu. If the bRevert parameter is TRUE, the return value is NULL. </returns>
        [DllImport("User32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)]bool bRevert);

        /// <summary>
        /// The PostMessage function places (posts) a message in the message queue 
        /// associated with the thread that created the specified window and returns 
        /// without waiting for the thread to process the message. 
        /// </summary>
        /// <param name="hWnd">Handle to the window whose window procedure is to receive the message.
        /// The following values have special meanings.</param>
        /// <param name="Msg">Specifies the message to be posted.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// The GetWindowPlacement function retrieves the show state and the restored, minimized, and maximized positions of the specified window. 
        /// </summary>
        /// <param name="hwnd">Handle to the window. </param>
        /// <param name="lpwndpl">Pointer to the WINDOWPLACEMENT structure that receives the show state and position information. Before calling GetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to sizeof( WINDOWPLACEMENT). GetWindowPlacement fails if lpwndpl-> length is not set correctly. </param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// Gets low word of dword
        /// </summary>
        /// <param name="value">Dword</param>
        /// <returns>Low word of dword</returns>
        public static int LowWord(IntPtr value)
        {
            //return value << 16 >> 16;
            //return value & 65535;
            return (short)(value.ToInt64() & 0xffffL);
        }

        /// <summary>
        /// Gets hi word of dword
        /// </summary>
        /// <param name="value">Dword</param>
        /// <returns>Hi word of dword</returns>
        public static int HiWord(IntPtr value)
        {
            //return (value >> 16) & 65535;
            return (short)((value.ToInt64() >> 16) & 0xffffL);
        }

        /// <summary>
        /// Created DWORD from two WORDs
        /// </summary>
        /// <param name="lo">Low word</param>
        /// <param name="hi">Hi word</param>
        /// <returns>Result DWORD</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811")]
        public static IntPtr MakeDWord(int lo, int hi)
        {
            //return (lo & 65535) + (hi & 65535) << 16;
            return (IntPtr)((((short)hi) << 16) | (lo & 0xffff));
        }

        /// <summary>
        /// The SetWindowRgn function sets the window region of a window. 
        /// The window region determines the area within the window where the system permits drawing. 
        /// The system does not display any portion of a window that lies outside of the window region 
        /// </summary>
        /// <param name="hWnd">Handle to window</param>
        /// <param name="hRgn">Handle to region</param>
        /// <param name="bRedraw">Window redraw option</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)]bool bRedraw);

        /// <summary>
        /// The CreateRoundRectRgn function creates a rectangular region with rounded corners
        /// </summary>
        /// <param name="nLeftRect">Specifies the x-coordinate of the upper-left corner of the region in device units.</param>
        /// <param name="nTopRect">Specifies the y-coordinate of the upper-left corner of the region in device units.</param>
        /// <param name="nRightRect">Specifies the x-coordinate of the lower-right corner of the region in device units.</param>
        /// <param name="nBottomRect">Specifies the y-coordinate of the lower-right corner of the region in device units</param>
        /// <param name="nWidthEllipse">Specifies the width of the ellipse used to create the rounded corners in device units.</param>
        /// <param name="nHeightEllipse">Specifies the height of the ellipse used to create the rounded corners in device units.</param>
        /// <returns></returns>
        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        /// <summary>
        /// The CreateRectRgn function creates a rectangular region.
        /// </summary>
        /// <param name="nLeftRect"> x-coordinate of upper-left corner</param>
        /// <param name="nTopRect">y-coordinate of upper-left corner</param>
        /// <param name="nRightRect">x-coordinate of lower-right corner</param>
        /// <param name="nBottomRect">y-coordinate of lower-right corner</param>
        /// <returns></returns>
        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        /// <summary>
        /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, 
        /// or palette, freeing all system resources associated with the object. After the object 
        /// is deleted, the specified handle is no longer valid. 
        /// </summary>
        /// <param name="hObject">Handle to a logical pen, brush, font, bitmap, region, or palette. </param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the specified handle is not valid or is currently selected into a DC, the return value is zero.</returns>
        [DllImport("Gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);


        /// <summary>
        /// The GetWindowLong function retrieves information about the specified window. The function also retrieves the 32-bit (long) value at the specified offset into the extra window memory.If you are retrieving a pointer or a handle, this function has been superseded by the GetWindowLongPtr function. (Pointers and handles are 32 bits on 32-bit Microsoft Windows and 64 bits on 64-bit Windows.) To write code that is compatible with both 32-bit and 64-bit versions of Windows, use GetWindowLongPtr.
        /// </summary>
        /// <param name="hWnd">Handle to the window and, indirectly, the class to which the window belongs</param>
        /// <param name="nIndex">Specifies the zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer</param>
        /// <returns>If the function succeeds, the return value is the requested 32-bit value. If the function fails, the return value is zero.</returns>
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            return new IntPtr(IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex));
        }
        // The GetWindowLong function retrieves information about the specified window. The function also retrieves the 32-bit (long) value at the specified offset into the extra window memory.If you are retrieving a pointer or a handle, this function has been superseded by the GetWindowLongPtr function. (Pointers and handles are 32 bits on 32-bit Microsoft Windows and 64 bits on 64-bit Windows.) To write code that is compatible with both 32-bit and 64-bit versions of Windows, use GetWindowLongPtr.
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern Int32 GetWindowLongPtr32(IntPtr hWnd, int nIndex);
        // The GetWindowLong function retrieves information about the specified window. The function also retrieves the 32-bit (long) value at the specified offset into the extra window memory.If you are retrieving a pointer or a handle, this function has been superseded by the GetWindowLongPtr function. (Pointers and handles are 32 bits on 32-bit Microsoft Windows and 64 bits on 64-bit Windows.) To write code that is compatible with both 32-bit and 64-bit versions of Windows, use GetWindowLongPtr.
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        [SuppressMessage("Microsoft.Interoperability", "CA1400")]
        static extern Int64 GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        /// <summary>
        /// The SetWindowLong function changes an attribute of the specified window. 
        /// The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">Handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">Specifies the zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer.</param>
        /// <param name="dwNewLong">Specifies the replacement value. </param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.
        /// If the function fails, the return value is zero.</returns>
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }
        // The SetWindowLong function changes an attribute of the specified window. 
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
        // The SetWindowLong function changes an attribute of the specified window. 
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        [SuppressMessage("Microsoft.Interoperability", "CA1400")]
        static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. 
        /// These windows are ordered according to their appearance on the screen. 
        /// The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window</param>
        /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order</param>
        /// <param name="X">Specifies the new position of the left side of the window, in client coordinates</param>
        /// <param name="Y">Specifies the new position of the top of the window, in client coordinates</param>
        /// <param name="cx">Specifies the new width of the window, in pixels</param>
        /// <param name="cy">Specifies the new height of the window, in pixels</param>
        /// <param name="uFlags">Specifies the window sizing and positioning flags.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        /// <summary>
        /// The EnumChildProc function is an application-defined callback function used with the EnumChildWindows function. It receives the child window handles. The WNDENUMPROC type defines a pointer to this callback function. EnumChildProc is a placeholder for the application-defined function name. 
        /// </summary>
        /// <param name="hwnd">Handle to a child window of the parent window specified in EnumChildWindows</param>
        /// <param name="lParam">Specifies the application-defined value given in EnumChildWindows</param>
        /// <returns>To continue enumeration, the callback function must return TRUE; to stop enumeration, it must return FALSE</returns>
        public delegate bool EnumChildProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// The EnumChildWindows function enumerates the child windows that belong to the specified parent window by passing the handle to each child window, in turn, to an application-defined callback function. EnumChildWindows continues until the last child window is enumerated or the callback function returns FALSE.
        /// </summary>
        /// <param name="hWndParent">Handle to the parent window whose child windows are to be enumerated. If this parameter is NULL, this function is equivalent to EnumWindows.</param>
        /// <param name="lpEnumFunc">Pointer to an application-defined callback function</param>
        /// <param name="lParam">Specifies an application-defined value to be passed to the callback function</param>
        /// <returns>Not used</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Performance", "CA1811")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// The SetParent function changes the parent window of the specified child window. 
        /// </summary>
        /// <param name="hWndChild">Handle to the child window</param>
        /// <param name="hWndNewParent">Handle to the new parent window. If this parameter is NULL, the desktop window becomes the new parent window. Windows 2000/XP: If this parameter is HWND_MESSAGE, the child window becomes a message-only window. </param>
        /// <returns> the function succeeds, the return value is a handle to the previous parent window. If the function fails, the return value is NULL. </returns>
        [DllImport("user32.dll", PreserveSig = false)]
        [SuppressMessage("Microsoft.Performance", "CA1811")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// The GetWindowInfo function retrieves information about the specified window.
        /// </summary>
        /// <param name="hwnd">Handle to the window whose information is to be retrieved. </param>
        /// <param name="pwi">Pointer to a WINDOWINFO structure to receive the information. Note that you must set WINDOWINFO.cbSize to sizeof(WINDOWINFO) before calling this function. </param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        /// <summary>
        /// The GetClassLongPtr function retrieves the specified value from the WNDCLASSEX structure associated with the specified window.
        /// If you are retrieving a pointer or a handle, this function supersedes the GetClassLong function. (Pointers and handles are 32 bits on 32-bit Microsoft Windows and 64 bits on 64-bit Windows.) To write code that is compatible with both 32-bit and 64-bit versions of Windows, use GetClassLongPtr.
        /// </summary>
        /// <param name="hWnd">Handle to the window and, indirectly, the class to which the window belongs. </param>
        /// <param name="nIndex">Specifies the value to retrieve. To retrieve a value from the extra class memory, specify the positive, zero-based byte offset of the value to be retrieved. Valid values are in the range zero through the number of bytes of extra class memory, minus eight; for example, if you specified 24 or more bytes of extra class memory, a value of 16 would be an index to the third integer. To retrieve any other value from the WNDCLASSEX structure, specify one of the following values. </param>
        /// <returns>If the function succeeds, the return value is the requested value.If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetClassLongPtr64(hWnd, nIndex);
            }
            return new IntPtr((long)GetClassLongPtr32(hWnd, nIndex));
        }

        // GetClassLong (x86 version)
        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        private static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);
        // GetClassLong (x64 version)
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        [SuppressMessage("Microsoft.Interoperability", "CA1400")]
        private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

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
        /// The GetWindowRect function retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen. 
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="lpRect">Pointer to a structure that receives the screen coordinates of the upper-left and lower-right corners of the window</param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);

        /// <summary>
        /// The GetClientRect function retrieves the coordinates of a window's client area. The client coordinates specify the upper-left and lower-right corners of the client area. Because client coordinates are relative to the upper-left corner of a window's client area, the coordinates of the upper-left corner are (0,0). 
        /// </summary>
        /// <param name="hWnd"> Handle to the window whose client coordinates are to be retrieved. </param>
        /// <param name="lpRect">Pointer to a RECT structure that receives the client coordinates. The left and top members are zero. The right and bottom members contain the width and height of the window. </param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Performance", "CA1811")]
        public static extern bool GetClientRect(IntPtr hWnd, ref Rect lpRect);

        /// <summary>
        /// The AdjustWindowRectEx function calculates the required size of the window rectangle, based on the desired size of the client rectangle. The window rectangle can then be passed to the CreateWindowEx function to create a window whose client area is the desired size. 
        /// </summary>
        /// <param name="lpRect">Pointer to a RECT structure that contains the coordinates of the top-left and bottom-right corners of the desired client area. When the function returns, the structure contains the coordinates of the top-left and bottom-right corners of the window to accommodate the desired client area. </param>
        /// <param name="dwStyle">Specifies the window style of the window whose required size is to be calculated. Note that you cannot specify the WS_OVERLAPPED style.</param>
        /// <param name="bMenu">Specifies whether the window has a menu. </param>
        /// <param name="dwExStyle">Specifies the extended window style of the window whose required size is to be calculated. For more information, see CreateWindowEx.</param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustWindowRectEx(ref Rect lpRect, int dwStyle, [MarshalAs(UnmanagedType.Bool)]bool bMenu, int dwExStyle);
        /*
        /// <summary>
        /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. 
        /// To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function.
        /// </summary>
        /// <param name="hWnd">Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows</param>
        /// <param name="Msg">Specifies the message to be sent</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        */

        /// <summary>
        /// Creates a Color dialog box that enables the user to select a color.
        /// </summary>
        /// <param name="lpcc">A pointer to a CHOOSECOLOR structure that contains information used to initialize the dialog box. When ChooseColor returns, this structure contains information about the user's color selection.</param>
        /// <returns>If the user clicks the OK button of the dialog box, the return value is nonzero. The rgbResult member of the CHOOSECOLOR structure contains the RGB color value of the color selected by the user.If the user cancels or closes the Color dialog box or an error occurs, the return value is zero. </returns>
        [DllImport("comdlg32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChooseColor(CHOOSECOLOR lpcc);

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public Rect rc;
            public bool lParam;
        }
        #endregion

        #region Keyboard Functions

        /// <summary>
        /// The ToUnicodeEx function translates the specified virtual-key 
        /// code and keyboard state to the corresponding 
        /// Unicode character or characters
        /// </summary>
        /// <returns>1, 2 or more if success, otherwise fail</returns>
        [DllImport("user32.dll", SetLastError = false)]
        public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        /// <summary>
        /// The GetKeyboardState function copies the status
        ///  of the 256 virtual keys to the specified buffer. 
        /// </summary>
        /// <returns>If the function fails, the return value is zero</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Portability", "CA1901")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        /// <summary>
        /// The MapVirtualKeyEx function translates (maps) a virtual-key 
        /// code into a scan code or character value, or translates a scan 
        /// code into a virtual-key code. The function translates the codes 
        /// using the input language and an input locale identifier
        /// </summary>
        /// <returns>The return value is either a scan code, 
        /// a virtual-key code, or a character value, depending 
        /// on the value of uCode and uMapType. If there is no translation,
        /// the return value is zero</returns>
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        /// <summary>
        /// The LoadKeyboardLayout function loads a new input locale identifier 
        /// (formerly called the keyboard layout) into the system. Several input
        /// locale identifiers can be loaded at a time, but only one per process 
        /// is active at a time. Loading multiple input locale identifiers makes 
        /// it possible to rapidly switch between them
        /// </summary>
        /// <returns>If the function succeeds, the return value is the input 
        /// locale identifier to the locale matched with the requested name. 
        /// If no matching locale is available, the return value is NULL</returns>
        [DllImport("user32.dll")]
        [SuppressMessage("Microsoft.Globalization", "CA2101")]
        public static extern IntPtr LoadKeyboardLayout(string cultureId, uint flags);

        /// <summary>
        /// The UnloadKeyboardLayout function unloads an input locale identifier 
        /// (formerly called a keyboard layout). 
        /// </summary>
        /// <param name="hkl">Input locale identifier to unload</param>
        /// <returns>If the function succeeds, the return value is nonzero</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnloadKeyboardLayout(IntPtr hkl);

        /// <summary>
        /// The GetKeyboardLayoutList function retrieves the input locale identifiers 
        /// (formerly called keyboard layout handles) corresponding to the current set 
        /// of input locales in the system. The function copies the 
        /// identifiers to the specified buffer.
        /// </summary>
        /// <param name="nBuff">Specifies the maximum number of handles that the buffer can hold</param>
        /// <param name="lpList">Pointer to the buffer that receives the array of input locale identifiers</param>
        /// <returns>If the function succeeds, the return value is the number of input locale 
        /// identifiers copied to the buffer or, if nBuff is zero, the return value is the size, 
        /// in array elements, of the buffer needed to receive all current input locale identifiers
        /// If the function fails, the return value is zero</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int GetKeyboardLayoutList(int nBuff, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] lpList);


        #endregion
    }
}
