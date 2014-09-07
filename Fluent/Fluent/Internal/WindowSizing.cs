using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Fluent.Internal
{
    using System.Windows.Interop;
    using Fluent.Metro.Native;

    public static class WindowSizing
    {
        public static void WindowInitialized(Window window)
        {
            var hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(HwndHook);
            }
        }

        private static IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;

            switch (message)
            {
                case Constants.WM_GETMINMAXINFO:
                    /* http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx */
                    WmGetMinMaxInfo(hWnd, lParam);

                    /* Setting handled to false enables the application to process it's own Min/Max requirements,
                     * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                    handled = false;
                    break;
            }
            return returnval;
        }

        #region WindowSize

        ////private bool IgnoreTaskBar()
        ////{
        ////    //var ignoreTaskBar = this.AssociatedObject.IgnoreTaskbarOnMaximize 
        ////    //    || this.AssociatedObject.WindowStyle == WindowStyle.None;

        ////    var ignoreTaskBar = false;

        ////    return ignoreTaskBar;
        ////}

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            const int MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);

                var ignoreTaskBar = false; //this.IgnoreTaskBar();
                var x = ignoreTaskBar ? monitorInfo.rcMonitor.left : monitorInfo.rcWork.left;
                var y = ignoreTaskBar ? monitorInfo.rcMonitor.top : monitorInfo.rcWork.top;
                mmi.ptMaxSize.X = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.right - x) : Math.Abs(monitorInfo.rcWork.right - x);
                mmi.ptMaxSize.Y = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.bottom - y) : Math.Abs(monitorInfo.rcWork.bottom - y);

                if (!ignoreTaskBar)
                {
                    mmi.ptMaxTrackSize.X = mmi.ptMaxSize.X;
                    mmi.ptMaxTrackSize.Y = mmi.ptMaxSize.Y;
                    mmi = AdjustWorkingAreaForAutoHide(monitor, mmi);
                }
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private static int GetEdge(RECT rc)
        {
            int uEdge;
            if (rc.top == rc.left && rc.bottom > rc.right)
                uEdge = (int)ABEdge.ABE_LEFT;
            else if (rc.top == rc.left && rc.bottom < rc.right)
                uEdge = (int)ABEdge.ABE_TOP;
            else if (rc.top > rc.left)
                uEdge = (int)ABEdge.ABE_BOTTOM;
            else
                uEdge = (int)ABEdge.ABE_RIGHT;
            return uEdge;
        }

        /// <summary>
        /// This method handles the window size if the taskbar is set to auto-hide.
        /// </summary>
        private static MINMAXINFO AdjustWorkingAreaForAutoHide(IntPtr monitorContainingApplication, MINMAXINFO mmi)
        {
            IntPtr hwnd = UnsafeNativeMethods.FindWindow("Shell_TrayWnd", null);
            IntPtr monitorWithTaskbarOnIt = UnsafeNativeMethods.MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (!monitorContainingApplication.Equals(monitorWithTaskbarOnIt))
            {
                return mmi;
            }

            var abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            UnsafeNativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref abd);
            int uEdge = GetEdge(abd.rc);
            bool autoHide = Convert.ToBoolean(UnsafeNativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref abd));

            if (!autoHide)
            {
                mmi.ptMaxSize.X -= 1;
                mmi.ptMaxTrackSize.X -= 1;
                return mmi;
            }

            switch (uEdge)
            {
                case (int)ABEdge.ABE_LEFT:
                    mmi.ptMaxPosition.X += 2;
                    mmi.ptMaxTrackSize.X -= 2;
                    mmi.ptMaxSize.X -= 2;
                    break;
                case (int)ABEdge.ABE_RIGHT:
                    mmi.ptMaxSize.X -= 2;
                    mmi.ptMaxTrackSize.X -= 2;
                    break;
                case (int)ABEdge.ABE_TOP:
                    mmi.ptMaxPosition.Y += 2;
                    mmi.ptMaxTrackSize.Y -= 2;
                    mmi.ptMaxSize.Y -= 2;
                    break;
                case (int)ABEdge.ABE_BOTTOM:
                    mmi.ptMaxSize.Y -= 2;
                    mmi.ptMaxTrackSize.Y -= 2;
                    break;
                default:
                    return mmi;
            }
            return mmi;
        }

        #endregion WindowSize
    }
}