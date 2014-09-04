using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Fluent.Metro.Native;

namespace Fluent
{
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
    public class MetroWindow : Window
    {
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_Icon = "PART_Icon";
        private readonly int doubleclick = UnsafeNativeMethods.GetDoubleClickTime();
        private DateTime lastMouseClick;
        private bool _isContextMenuOpen = false;

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty SavePositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        private FrameworkElement iconImage;

        public bool SaveWindowPosition
        {
            get { return (bool)GetValue(SavePositionProperty); }
            set { SetValue(SavePositionProperty, value); }
        }

        static MetroWindow()
        {            
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
            StyleProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(null, OnCoerceStyle));
        }

        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue != null)
            {
                return basevalue;
            }

            var frameworkElement = d as FrameworkElement;
            if (frameworkElement != null)
            {
                basevalue = frameworkElement.TryFindResource(typeof(MetroWindow));
            }

            return basevalue;
        }

        public WindowCommands WindowCommands { get; set; }

        public bool ShowIconOnTitleBar
        {
            get { return (bool)GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        public bool ShowTitleBar
        {
            get { return (bool)GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, value); }
        }

        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, value); }
        }

        public bool ShowMaxRestoreButton
        {
            get { return (bool)GetValue(ShowMaxRestoreButtonProperty); }
            set { SetValue(ShowMaxRestoreButtonProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.iconImage != null)
            {
                this.iconImage.MouseUp -= this.HandleIconMouseUp;
            }

            this.iconImage = GetTemplateChild(PART_Icon) as FrameworkElement;

            if (WindowCommands == null)
            {
                WindowCommands = new WindowCommands();
            }

            if (ShowTitleBar)
            {
                var titleBar = GetTemplateChild(PART_TitleBar) as FrameworkElement;

                if (titleBar != null)
                {
                    titleBar.MouseDown += TitleBarMouseDown;
                    titleBar.MouseUp += TitleBarMouseUp;
                    titleBar.MouseMove += TitleBarMouseMove;
                }                

                if (this.iconImage != null)
                {
                    this.iconImage.MouseUp += this.HandleIconMouseUp;
                }
            }
            else
            {
                MouseDown += TitleBarMouseDown;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowCommands != null)
            {
                WindowCommands.RefreshMaximizeIconState();
            }

            base.OnStateChanged(e);
        }

        protected void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ReferenceEquals(e.OriginalSource, this.iconImage))
            {
                return;
            }

            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

            if (e.ClickCount == 2 && (ResizeMode == ResizeMode.CanResizeWithGrip || ResizeMode == ResizeMode.CanResize))
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            // Do not resize window 
            _isContextMenuOpen = true;
            base.OnContextMenuOpening(e);
        }

        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            // Do not resize window 
            _isContextMenuOpen = false;
            base.OnContextMenuClosing(e);
        }

        protected void TitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!ShowIconOnTitleBar)
            {
                return;
            }

            var mousePosition = GetCorrectPosition(this);

            if (mousePosition.X <= RibbonProperties.GetTitleBarHeight(this)
                && mousePosition.Y <= RibbonProperties.GetTitleBarHeight(this))
            {
                if ((DateTime.Now - lastMouseClick).TotalMilliseconds <= doubleclick)
                {
                    Close();
                    return;
                }
                lastMouseClick = DateTime.Now;

                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(GetCorrectPosition(this)));
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(GetCorrectPosition(this)));
            }
        }

        private void HandleIconMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed
                && e.ClickCount == 1)
            {
                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(GetCorrectPosition(this)));
            }
        }

        private static Point GetCorrectPosition(Visual relativeTo)
        {
            UnsafeNativeMethods.Win32Point w32Mouse;
            UnsafeNativeMethods.GetCursorPos(out w32Mouse);
            return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed
                && e.LeftButton == MouseButtonState.Pressed && WindowState == WindowState.Maximized
                && ResizeMode != ResizeMode.NoResize && !_isContextMenuOpen)
            {
                // Calculating correct left coordinate for multi-screen system.
                Point mouseAbsolute = PointToScreen(Mouse.GetPosition(this));
                double width = RestoreBounds.Width;
                double left = mouseAbsolute.X - width / 2;

                // Aligning window's position to fit the screen.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

                var mousePosition = e.MouseDevice.GetPosition(this);

                // When dragging the window down at the very top of the border,
                // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released
                Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
                Left = left;

                // Restore window to normal state.
                WindowState = WindowState.Normal;

                DragMove();
            }
        }

        internal T GetPart<T>(string name) where T : DependencyObject
        {
            return (T)GetTemplateChild(name);
        }

        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null) return;

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !UnsafeNativeMethods.IsWindow(hwnd))
                return;

            var hmenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);

            var cmd = UnsafeNativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
                UnsafeNativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
        }
    }
}