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
    }
}
