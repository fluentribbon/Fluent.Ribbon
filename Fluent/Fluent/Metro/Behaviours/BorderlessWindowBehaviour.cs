using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using Fluent.Metro.Native;

namespace Fluent.Metro.Behaviours
{
    /// <summary>
    /// Behavior for borderless windows (used for Office 2013 theme)
    /// </summary>
    public class BorderlessWindowBehavior : Behavior<RibbonWindow>
    {
        private HwndSource hwndSource;

        /// <summary>
        /// Called when behavior is being attached
        /// </summary>
        protected override void OnAttached()
        {
            if (PresentationSource.FromVisual(AssociatedObject) != null)
            {
                this.AddHwndHook();
            }
            else
            {
                this.AssociatedObject.SourceInitialized += this.HandleAssociatedObject_SourceInitialized;
            }

            base.OnAttached();
        }

        /// <summary>
        /// Called when behavior is being detached
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.SourceInitialized -= this.HandleAssociatedObject_SourceInitialized;

            this.RemoveHwndHook();
            base.OnDetaching();
        }

        private void AddHwndHook()
        {
            this.hwndSource = PresentationSource.FromVisual(AssociatedObject) as HwndSource;
            if (this.hwndSource != null)
            {
                this.hwndSource.AddHook(this.HwndHook);
            }
        }

        private void RemoveHwndHook()
        {
            this.AssociatedObject.SourceInitialized -= this.HandleAssociatedObject_SourceInitialized;
            if (this.hwndSource != null)
            {
                this.hwndSource.RemoveHook(this.HwndHook);
            }

            this.hwndSource = null;
        }

        private void HandleAssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            this.AddHwndHook();
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;

            switch (message)
            {
                case Constants.WM_NCHITTEST:
                    // don't process the message on windows that are maximized as those don't have a resize border at all
                    if (this.AssociatedObject.WindowState == WindowState.Maximized)
                    {
                        break;
                    }

                    // don't process the message on windows that can't be resized
                    var resizeMode = this.AssociatedObject.ResizeMode;
                    if (resizeMode == ResizeMode.CanMinimize 
                        || resizeMode == ResizeMode.NoResize)
                    {
                        break;
                    }

                    // get X & Y out of the message                   
                    var screenPoint = new Point(UnsafeNativeMethods.GET_X_LPARAM(lParam), UnsafeNativeMethods.GET_Y_LPARAM(lParam));

                    // convert to window coordinates
                    var windowPoint = this.AssociatedObject.PointFromScreen(screenPoint);
                    var windowSize = this.AssociatedObject.RenderSize;
                    var windowRect = new Rect(windowSize);
                    windowRect.Inflate(-6, -6);

                    // don't process the message if the mouse is outside the 6px resize border
                    if (windowRect.Contains(windowPoint))
                    {
                        break;
                    }

                    var windowHeight = (int)windowSize.Height;
                    var windowWidth = (int)windowSize.Width;

                    // create the rectangles where resize arrows are shown
                    var topLeft = new Rect(0, 0, 6, 6);
                    var top = new Rect(6, 0, windowWidth - 12, 6);
                    var topRight = new Rect(windowWidth - 6, 0, 6, 6);

                    var left = new Rect(0, 6, 6, windowHeight - 12);
                    var right = new Rect(windowWidth - 6, 6, 6, windowHeight - 12);

                    var bottomLeft = new Rect(0, windowHeight - 6, 6, 6);
                    var bottom = new Rect(6, windowHeight - 6, windowWidth - 12, 6);
                    var bottomRight = new Rect(windowWidth - 6, windowHeight - 6, 6, 6);

                    // check if the mouse is within one of the rectangles
                    if (topLeft.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOPLEFT;
                    else if (top.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOP;
                    else if (topRight.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOPRIGHT;
                    else if (left.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTLEFT;
                    else if (right.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTRIGHT;
                    else if (bottomLeft.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOMLEFT;
                    else if (bottom.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOM;
                    else if (bottomRight.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOMRIGHT;

                    if (returnval != IntPtr.Zero)
                    {
                        handled = true;
                    }

                    break;
            }
            return returnval;
        }
    }
}