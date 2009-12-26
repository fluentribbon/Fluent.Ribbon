#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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
        /// Sets a new window style.
        /// </summary>
        public const int GWL_STYLE = (-16);
        /// <summary>
        /// Sets a new extended window style. For more information, see CreateWindowEx. 
        /// </summary>
        public const int GWL_EXSTYLE = (-20);

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
            /*
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="x">Specifies the x-coordinate of the point. </param>
            /// <param name="y">Specifies the y-coordinate of the point.</param>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }*/
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
        /// The GetActiveWindow function retrieves the window handle to the active 
        /// window attached to the calling thread's message queue
        /// </summary>
        /// <returns>The return value is the handle to the active window attached to the calling thread's message queue. 
        /// Otherwise, the return value is NULL. </returns>
        
        [DllImport("user32.dll")]
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
        [DllImport("dwmapi.dll", PreserveSig = false)]
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
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        /// <summary>
        /// Gets low word of dword
        /// </summary>
        /// <param name="value">Dword</param>
        /// <returns>Low word of dword</returns>
        public static int LowWord(int value)
        {
            //return value << 16 >> 16;
            return value & 65535;
        }

        /// <summary>
        /// Gets hi word of dword
        /// </summary>
        /// <param name="value">Dword</param>
        /// <returns>Hi word of dword</returns>
        public static int HiWord(int value)
        {
            return (value >> 16) & 65535;
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
        /// The SetWindowLong function changes an attribute of the specified window. 
        /// The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">Handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">Specifies the zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer.</param>
        /// <param name="dwNewLong">Specifies the replacement value. </param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.
        /// If the function fails, the return value is zero.</returns>
        [DllImport("User32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

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
        [DllImport("user32.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        #endregion
    }
}
