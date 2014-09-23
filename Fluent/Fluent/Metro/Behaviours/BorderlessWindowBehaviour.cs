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
        /// <summary>
        /// Gets or sets the <see cref="Border"/>
        /// </summary>
        public Border Border { get; set; }

        private HwndSource _mHWNDSource;
        private HandleRef windowHandle;

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
                this.AssociatedObject.SourceInitialized += this.AssociatedObject_SourceInitialized;
            }

            this.AssociatedObject.Loaded += this.HandleAssociatedObjectLoaded;
            if (this.AssociatedObject.IsLoaded)
            {
                this.HandleAssociatedObjectLoaded(this, new RoutedEventArgs());
            }

            base.OnAttached();
        }

        /// <summary>
        /// Called when behavior is being detached
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.SourceInitialized -= this.AssociatedObject_SourceInitialized;
            this.AssociatedObject.Loaded -= this.HandleAssociatedObjectLoaded;

            this.RemoveHwndHook();
            base.OnDetaching();
        }

        private void HandleAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            var ancestors = this.AssociatedObject.GetPart<Border>("PART_Border");
            this.Border = ancestors;
            if (this.ShouldHaveBorder())
            {
                this.AddBorder();
            }
        }

        private void AddHwndHook()
        {
            this._mHWNDSource = PresentationSource.FromVisual(AssociatedObject) as HwndSource;
            if (this._mHWNDSource != null)
            {
                this._mHWNDSource.AddHook(this.HwndHook);
            }

            this.windowHandle = new HandleRef(this.AssociatedObject, new WindowInteropHelper(this.AssociatedObject).Handle);
        }

        private void RemoveHwndHook()
        {
            this.AssociatedObject.SourceInitialized -= this.AssociatedObject_SourceInitialized;
            if (this._mHWNDSource != null)
            {
                this._mHWNDSource.RemoveHook(this.HwndHook);
            }

            this._mHWNDSource = null;
            this.windowHandle = new HandleRef();
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            this.AddHwndHook();
            this.SetDefaultBackgroundColor();
        }

        private bool ShouldHaveBorder()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return true;
            }

            if (!UnsafeNativeMethods.DwmIsCompositionEnabled())
            {
                return true;
            }

            return false;
        }

        readonly SolidColorBrush _borderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));

        private void AddBorder()
        {
            if (this.Border == null)
            {
                return;
            }

            this.Border.BorderThickness = new Thickness(1);
            this.Border.BorderBrush = this._borderColor;
        }

        private void RemoveBorder()
        {
            if (this.Border == null)
            {
                return;
            }

            this.Border.BorderThickness = new Thickness(0);
            this.Border.BorderBrush = null;
        }

        private void SetDefaultBackgroundColor()
        {
            var bgSolidColorBrush = this.AssociatedObject.Background as SolidColorBrush;

            if (bgSolidColorBrush != null)
            {
                var rgb = bgSolidColorBrush.Color.R | (bgSolidColorBrush.Color.G << 8) | (bgSolidColorBrush.Color.B << 16);

                // set the default background color of the window -> this avoids the black stripes when resizing
                var hBrushOld = UnsafeNativeMethods.SetClassLong(this.windowHandle, Constants.GCLP_HBRBACKGROUND, UnsafeNativeMethods.CreateSolidBrush(rgb));

                if (hBrushOld != IntPtr.Zero)
                {
                    UnsafeNativeMethods.DeleteObject(hBrushOld);
                }
            }
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;

            switch (message)
            {
                case Constants.WM_NCCALCSIZE:
                    /* Hides the border */
                    handled = true;
                    break;
                case Constants.WM_NCPAINT:
                    {
                        if (!this.ShouldHaveBorder())
                        {
                            var val = 2;
                            UnsafeNativeMethods.DwmSetWindowAttribute(this.windowHandle.Handle, 2, ref val, 4);
                            var m = new MARGINS { bottomHeight = 1, leftWidth = 1, rightWidth = 1, topHeight = 1 };
                            UnsafeNativeMethods.DwmExtendFrameIntoClientArea(this.windowHandle.Handle, ref m);

                            if (this.Border != null)
                            {
                                this.Border.BorderThickness = new Thickness(0);
                            }
                        }
                        else
                        {
                            this.AddBorder();
                        }
                        //handled = true;
                    }
                    break;
                case Constants.WM_NCACTIVATE:
                    {
                        /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam
                         * "does not repaint the nonclient area to reflect the state change." */
                        returnval = UnsafeNativeMethods.DefWindowProc(hWnd, message, wParam, new IntPtr(-1));

                        if (!this.ShouldHaveBorder())
                        {
                            if (wParam == IntPtr.Zero)
                            {
                                this.AddBorder();
                            }
                            else
                            {
                                this.RemoveBorder();
                            }
                        }

                        handled = true;
                    }
                    break;

                case Constants.WM_NCHITTEST:
                    // don't process the message on windows that are maximized as those don't have a resize border at all
                    if (this.AssociatedObject.WindowState == WindowState.Maximized)
                    {
                        break;
                    }

                    // don't process the message on windows that can't be resized
                    var resizeMode = this.AssociatedObject.ResizeMode;
                    if (resizeMode == ResizeMode.CanMinimize || resizeMode == ResizeMode.NoResize)
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

                case Constants.WM_INITMENU:
                    var window = this.AssociatedObject;

                    if (window != null)
                    {
                        if (window.ResizeMode != ResizeMode.NoResize && window.ResizeMode != ResizeMode.CanMinimize)
                        {
                            UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        }
                        else
                        {
                            if (window.WindowState == WindowState.Maximized)
                            {
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_RESTORE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MOVE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                            }
                            else
                            {
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_RESTORE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MOVE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                            }
                        }

                        if (window.ResizeMode != ResizeMode.NoResize)
                        {
                            UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MINIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        }

                        if (AssociatedObject.ResizeMode == ResizeMode.NoResize || window.WindowState == WindowState.Maximized)
                        {
                            UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_SIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        }
                    }
                    break;
            }
            return returnval;
        }
    }
}