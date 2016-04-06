namespace Fluent.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Fluent.Metro.Native;

    /// <summary>
    /// Class which offers helper methods for steering the window
    /// </summary>
    public static class WindowSteeringHelper
    {
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

            if (handleDragMove && e.ClickCount == 1)
            {
                e.Handled = true;
                window.DragMove();
            }
            else if (handleStateChange &&
                     e.ClickCount == 2 &&
                     window.ResizeMode != ResizeMode.NoResize)
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
                || NativeMethods.IsWindow(hwnd) == false)
            {
                return;
            }

            e.Handled = true;

            var hmenu = NativeMethods.GetSystemMenu(hwnd, false);

            var cmd = NativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
            {                
                NativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);                
            }            
        }
    }
}