#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
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
        #region Consts

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
        /// The WM_CREATE message is sent when an application requests that a window be created by calling the CreateWindowEx or CreateWindow function. (The message is sent before the function returns.) The window procedure of the new window receives this message after the window is created, but before the window becomes visible
        /// </summary>
        public const int WM_CREATE = 0x0001;

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
        #endregion

        #region Structs

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
        public struct MonitorInfo
        {
            /// <summary>
            /// The size of the structure, in bytes. 
            /// </summary>
            public uint Size;
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

        #endregion

        #region Functions

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
        /// The GetMonitorInfo function retrieves information about a display monitor. 
        /// </summary>
        /// <param name="hMonitor">Handle to the display monitor of interest.</param>
        /// <param name="lpmi">Pointer to a MONITORINFO or MONITORINFOEX structure that receives 
        /// information about the specified display monitor</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

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
        /// Стандартная процедура окна
        /// </summary>
        /// <param name="hwnd">Хэндл окна</param>
        /// <param name="msg">Сообщение</param>
        /// <param name="wParam">Прараметр</param>
        /// <param name="lParam">Параметр</param>
        /// <param name="plResult">Результат</param>
        /// <returns>Результат</returns>
        [DllImport("dwmapi.dll")]
        public static extern int DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);

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

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern Int32 GetWindowLongPtr32(IntPtr hWnd, int nIndex);

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

        [DllImport("user32.dll", EntryPoint="SetWindowLong")]
        static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint="SetWindowLongPtr")]
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
        public static extern bool AdjustWindowRectEx(ref Rect lpRect, int dwStyle,bool bMenu, int dwExStyle);
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
        #endregion

        #region Keyboard Functions

        /// <summary>
        /// The ToUnicodeEx function translates the specified virtual-key 
        /// code and keyboard state to the corresponding 
        /// Unicode character or characters
        /// </summary>
        /// <returns>1, 2 or more if success, otherwise fail</returns>
        [DllImport("user32.dll", SetLastError = false, PreserveSig = true)]
        public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        /// <summary>
        /// The GetKeyboardState function copies the status
        ///  of the 256 virtual keys to the specified buffer. 
        /// </summary>
        /// <returns>If the function fails, the return value is zero</returns>
        [DllImport("user32.dll")]
        [return:MarshalAs(UnmanagedType.Bool)]
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
