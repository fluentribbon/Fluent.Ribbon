namespace Fluent.Helpers
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using ControlzEx.Native;
    using Fluent.Extensions;
    using Standard;

    /// <summary>
    /// Class which offers helper methods for steering the window
    /// </summary>
    public static class WindowSteeringHelper
    {
        private static readonly PropertyInfo criticalHandlePropertyInfo = typeof(Window).GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly object[] emptyObjectArray = new object[0];

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="e">The mouse event args.</param>
        /// <param name="handleDragMove">Defines if window dragging should be handled.</param>
        /// <param name="handleStateChange">Defines if window state changes should be handled.</param>
        public static void HandleMouseLeftButtonDown(MouseButtonEventArgs e, bool handleDragMove, bool handleStateChange)
        {
            var dependencyObject = e.Source as DependencyObject;

            if (dependencyObject == null)
            {
                return;
            }

            HandleMouseLeftButtonDown(dependencyObject, e, handleDragMove, handleStateChange);
        }

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="dependencyObject">The object which was the source of the mouse event.</param>
        /// <param name="e">The mouse event args.</param>
        /// <param name="handleDragMove">Defines if window dragging should be handled.</param>
        /// <param name="handleStateChange">Defines if window state changes should be handled.</param>
        public static void HandleMouseLeftButtonDown(DependencyObject dependencyObject, MouseButtonEventArgs e, bool handleDragMove, bool handleStateChange)
        {
            var window = Window.GetWindow(dependencyObject);

            if (window == null)
            {
                return;
            }

            if (handleDragMove 
                && e.ClickCount == 1)
            {
                e.Handled = true;

                // tage from DragMove internal code
                window.VerifyAccess();

#pragma warning disable 618
                // for the touch usage
                UnsafeNativeMethods.ReleaseCapture();

                var criticalHandle = (IntPtr)criticalHandlePropertyInfo.GetValue(window, emptyObjectArray);
                // DragMove works too, but not on maximized windows
                NativeMethods.SendMessage(criticalHandle, WM.SYSCOMMAND, (IntPtr)SC.MOUSEMOVE, IntPtr.Zero);
                NativeMethods.SendMessage(criticalHandle, WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
#pragma warning restore 618
            }
            else if (handleStateChange 
                && e.ClickCount == 2 
                && window.ResizeMode != ResizeMode.NoResize)
            {
                e.Handled = true;
                window.WindowState = window.WindowState == WindowState.Maximized
                                         ? WindowState.Normal
                                         : WindowState.Maximized;
            }
        }

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="dependencyObject">The object which was the source of the mouse event.</param>
        /// <param name="e">The mouse event args.</param>
        public static void ShowSystemMenuPhysicalCoordinates(DependencyObject dependencyObject, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(dependencyObject);

            if (window == null)
            {
                return;
            }

            ShowSystemMenuPhysicalCoordinates(window, e);
        }

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="window">The window for which the system menu should be shown.</param>
        /// <param name="e">The mouse event args.</param>
        public static void ShowSystemMenuPhysicalCoordinates(Window window, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(window);
            var physicalScreenLocation = window.PointToScreen(mousePosition);
            ShowSystemMenuPhysicalCoordinates(window, e, physicalScreenLocation);
        }

        /// <summary>
        /// Shows the system menu at <paramref name="physicalScreenLocation"/>.
        /// </summary>
        /// <param name="window">The window for which the system menu should be shown.</param>
        /// <param name="e">The mouse event args.</param>
        /// <param name="physicalScreenLocation">The location at which the system menu should be shown.</param>
        public static void ShowSystemMenuPhysicalCoordinates(Window window, MouseButtonEventArgs e, Point physicalScreenLocation)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero
#pragma warning disable 618
                || NativeMethods.IsWindow(hwnd) == false)
#pragma warning restore 618
            {
                return;
            }

            e.Handled = true;

            window.RunInDispatcherAsync(() => ShowSystemMenuPhysicalCoordinates(hwnd, physicalScreenLocation), DispatcherPriority.Background);
        }

        private static void ShowSystemMenuPhysicalCoordinates(IntPtr hwnd, Point physicalScreenLocation)
        {
#pragma warning disable 618
            var hmenu = NativeMethods.GetSystemMenu(hwnd, false);

            var cmd = NativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTALIGN | Constants.TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
            {
                NativeMethods.PostMessage(hwnd, WM.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
#pragma warning restore 618
        }
    }
}