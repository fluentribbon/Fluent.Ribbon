#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

namespace Fluent
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using Fluent.Internal;
    using Fluent.Metro.Native;
#if NET35 || NET40
    using Microsoft.Windows.Shell;
#else
    using System.Windows.Shell;
#endif

    /// <summary>
    /// Represents basic window for ribbon
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1049")]
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_ButtonsPanel, Type = typeof(UIElement))]
    public class RibbonWindow : Window
    {
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_Icon = "PART_Icon";
        private const string PART_ButtonsPanel = "PART_ButtonsPanel";

        private readonly int doubleclick = UnsafeNativeMethods.GetDoubleClickTime();
        private DateTime lastMouseClick;
        private bool isContextMenuOpen;

        private FrameworkElement iconImage;
        private FrameworkElement titleBar;

        #region Properties

        /// <summary>
        /// Using a DependencyProperty as the backing store for WindowCommands.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WindowCommandsProperty = DependencyProperty.Register("WindowCommands", typeof(WindowCommands), typeof(RibbonWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the window commands
        /// </summary>
        public WindowCommands WindowCommands
        {
            get { return (WindowCommands)GetValue(WindowCommandsProperty); }
            set { SetValue(WindowCommandsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SaveWindowPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SavePositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(false));

        /// <summary>
        ///  Gets or sets whether window position will be saved and loaded.
        /// </summary>
        public bool SaveWindowPosition
        {
            get { return (bool)GetValue(SavePositionProperty); }
            set { SetValue(SavePositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets resize border thickness
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeBorderTickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty =
            DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(RibbonWindow), new UIPropertyMetadata(new Thickness(9), OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Gets or sets glass border thickness
        /// </summary>
        public Thickness GlassBorderThickness
        {
            get { return (Thickness)GetValue(GlassBorderThicknessProperty); }
            set { SetValue(GlassBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GlassBorderThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GlassBorderThicknessProperty =
            DependencyProperty.Register("GlassBorderThickness", typeof(Thickness), typeof(RibbonWindow), new UIPropertyMetadata(new Thickness(9, 29, 9, 9), OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Gets or sets caption height
        /// </summary>
        public double CaptionHeight
        {
            get { return (double)GetValue(CaptionHeightProperty); }
            set { SetValue(CaptionHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyProperty.Register("CaptionHeight", typeof(double), typeof(RibbonWindow), new UIPropertyMetadata(20.0, OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Gets or sets corner radius 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonWindow), new UIPropertyMetadata(new CornerRadius(9, 9, 9, 9), OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DontUseDwm.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DontUseDwmProperty =
            DependencyProperty.Register("DontUseDwm", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(false, OnDontUseDwmChanged));

        /// <summary>
        ///  Gets or sets whether DWM should be used.
        /// </summary>
        public bool DontUseDwm
        {
            get { return (bool)GetValue(DontUseDwmProperty); }
            set { SetValue(DontUseDwmProperty, value); }
        }

        /// <summary>
        /// Gets wheter DWM can be used (<see cref="NativeMethods.IsDwmEnabled"/> is true and <see cref="DontUseDwm"/> is false).
        /// </summary>
        public bool CanUseDwm
        {
            get { return (bool)GetValue(CanUseDwmProperty); }
            private set { SetValue(CanUseDwmPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey CanUseDwmPropertyKey = DependencyProperty.RegisterReadOnly("CanUseDwm", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(true));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanUseDwm.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanUseDwmProperty = CanUseDwmPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets whether the WindowChrome will be used. This is needed to disable the WindowChrome for the Office 2013 theme
        /// </summary>
        public bool? UseWindowChrome
        {
            get { return (bool?)GetValue(UseWindowChromeProperty); }
            set { SetValue(UseWindowChromeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UseWindowChrome.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UseWindowChromeProperty =
            DependencyProperty.Register("UseWindowChrome", typeof(bool?), typeof(RibbonWindow), new PropertyMetadata(null, OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Gets or sets whether icon is visible
        /// </summary>
        public bool IsIconVisible
        {
            get { return (bool)GetValue(IsIconVisibleProperty); }
            set { SetValue(IsIconVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether icon is visible
        /// </summary>
        public static readonly DependencyProperty IsIconVisibleProperty = DependencyProperty.Register("IsIconVisible", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(true));

        // todo check if IsCollapsed and IsAutomaticCollapseEnabled should be reduced to one shared property for RibbonWindow and Ribbon
        /// <summary>
        /// Gets whether window is collapsed
        /// </summary>              
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool),
            typeof(RibbonWindow), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Defines if the Ribbon should automatically set <see cref="IsCollapsed"/> when the width or height of the owner window drop under <see cref="Ribbon.MinimalVisibleWidth"/> or <see cref="Ribbon.MinimalVisibleHeight"/>
        /// </summary>
        public bool IsAutomaticCollapseEnabled
        {
            get { return (bool)GetValue(IsAutomaticCollapseEnabledProperty); }
            set { SetValue(IsAutomaticCollapseEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAutomaticCollapseEnabledProperty =
            DependencyProperty.Register("IsAutomaticCollapseEnabled", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(true));

        /// <summary>
        /// Gets whether client window area is activated
        /// </summary>
        public bool IsNonClientAreaActive
        {
            get { return (bool)GetValue(IsNonClientAreaActiveProperty); }
            internal set { SetValue(IsNonClientAreaActivePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsNonClientAreaActivePropertyKey = DependencyProperty.RegisterReadOnly("IsNonClientAreaActive", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(true));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsNcActivated.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsNonClientAreaActiveProperty = IsNonClientAreaActivePropertyKey.DependencyProperty;

        #endregion

        #region Commands

        /// <summary>
        /// Minimize command
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211")]
        public static RoutedCommand MinimizeCommand = new RoutedCommand();

        /// <summary>
        /// Maximize command
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211")]
        public static RoutedCommand MaximizeCommand = new RoutedCommand();

        /// <summary>
        /// Normalize command
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211")]
        public static RoutedCommand NormalizeCommand = new RoutedCommand();

        /// <summary>
        /// Close command
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211")]
        public static RoutedCommand CloseCommand = new RoutedCommand();

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonWindow()
        {
            StyleProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(null, OnCoerceStyle));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(typeof(RibbonWindow)));

            // Register commands
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(CloseCommand, OnCloseCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(MinimizeCommand, OnMinimizeCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(MaximizeCommand, OnMaximizeCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(NormalizeCommand, OnNormalizeCommandExecuted));
        }

        // Coerce object style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue != null)
            {
                return basevalue;
            }

            var frameworkElement = d as FrameworkElement;
            if (frameworkElement != null)
            {
                basevalue = frameworkElement.TryFindResource(typeof(RibbonWindow));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonWindow()
        {
            this.SizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Commands handles

        // Handles Close command
        static void OnCloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: why sender must be RibbonWindow?
            RibbonWindow window = sender as RibbonWindow;
            if (window != null) window.Close();
        }

        // Handles Maximize command
        static void OnMaximizeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = sender as RibbonWindow;
            if (window != null) window.WindowState = WindowState.Maximized;
        }

        // Handles Normalize command
        static void OnNormalizeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = sender as RibbonWindow;
            if (window != null) window.WindowState = WindowState.Normal;
        }

        // Handles Minimize command
        static void OnMinimizeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = sender as RibbonWindow;
            if (window != null) window.WindowState = WindowState.Minimized;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.UpdateCanUseDwm();

            this.UpdateWindowChrome();

            WindowSizing.WindowInitialized(this);
        }

        /// <summary>
        /// Called when the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">A reference to the root of the old content tree.</param><param name="newContent">A reference to the root of the new content tree.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            var content = newContent as IInputElement;

            if (content != null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(content, true);
            }
        }

        #endregion

        private static void OnWindowChromeRelevantPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as RibbonWindow;

            if (window == null)
            {
                return;
            }

            window.UpdateWindowChrome();
        }

        private static void OnDontUseDwmChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as RibbonWindow;

            if (window == null)
            {
                return;
            }

            window.UpdateCanUseDwm();
        }

        // Size change to collapse ribbon
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MaintainIsCollapsed();
        }

        private void MaintainIsCollapsed()
        {
            if (this.IsAutomaticCollapseEnabled == false)
            {
                return;
            }

            if (this.ActualWidth < Ribbon.MinimalVisibleWidth
                || this.ActualHeight < Ribbon.MinimalVisibleHeight)
            {
                this.IsCollapsed = true;
            }
            else
            {
                this.IsCollapsed = false;
            }
        }

        private void UpdateWindowChrome()
        {
            WindowChrome windowChrome = null;

            if (this.UseWindowChrome.GetValueOrDefault())
            {
                windowChrome = new WindowChrome
                {
                    CaptionHeight = this.CaptionHeight,
                    CornerRadius = this.CornerRadius,
                    GlassFrameThickness = this.CanUseDwm
                        ? this.GlassBorderThickness 
                        : default(Thickness),
                    ResizeBorderThickness = this.ResizeBorderThickness,
#if NET45
                    ////NonClientFrameEdges = NonClientFrameEdges.Bottom,
                    UseAeroCaptionButtons = this.CanUseDwm
#endif
                };
            }

            WindowChrome.SetWindowChrome(this, windowChrome);
        }

        private void UpdateCanUseDwm()
        {
            this.CanUseDwm = NativeMethods.IsDwmEnabled()
                          && this.DontUseDwm == false;
        }

        #region Metro

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.UpdateCanUseDwm();

            this.UpdateWindowChrome();

            if (this.iconImage != null)
            {
                this.iconImage.MouseUp -= this.HandleIconMouseUp;
            }

            if (this.titleBar != null)
            {
                this.titleBar.MouseDown -= this.HandleTitleBarMouseDown;
                this.titleBar.MouseUp -= this.HandleTitleBarMouseUp;
                this.titleBar.MouseMove -= this.HandleTitleBarMouseMove;
            }

            if (this.UseWindowChrome.GetValueOrDefault())
            {
                this.WindowCommands = null;

                var buttonsPanel = this.GetTemplateChild(PART_ButtonsPanel) as FrameworkElement;

                if (buttonsPanel != null)
                {
                    WindowChrome.SetIsHitTestVisibleInChrome(buttonsPanel, true);
                }
            }
            else
            {
                this.iconImage = this.GetTemplateChild(PART_Icon) as FrameworkElement;

                if (this.WindowCommands == null)
                {
                    this.WindowCommands = new WindowCommands();
                }

                this.titleBar = this.GetTemplateChild(PART_TitleBar) as FrameworkElement;

                if (this.titleBar != null)
                {
                    this.titleBar.MouseDown += this.HandleTitleBarMouseDown;
                    this.titleBar.MouseUp += this.HandleTitleBarMouseUp;
                    this.titleBar.MouseMove += this.HandleTitleBarMouseMove;
                }

                if (this.iconImage != null)
                {
                    this.iconImage.MouseUp += this.HandleIconMouseUp;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.StateChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowCommands != null)
            {
                this.WindowCommands.RefreshMaximizeIconState();
            }

            base.OnStateChanged(e);
        }

        private void HandleTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ReferenceEquals(e.OriginalSource, this.iconImage))
            {
                return;
            }

            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

            if (e.ClickCount == 2 && (this.ResizeMode == ResizeMode.CanResizeWithGrip || this.ResizeMode == ResizeMode.CanResize))
            {
                this.WindowState = this.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.FrameworkElement.ContextMenuOpening"/> routed event reaches this class in its route. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            // Do not resize window 
            this.isContextMenuOpen = true;
            base.OnContextMenuOpening(e);
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.FrameworkElement.ContextMenuClosing"/> routed event reaches this class in its route. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            // Do not resize window 
            this.isContextMenuOpen = false;
            base.OnContextMenuClosing(e);
        }

        private void HandleTitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsIconVisible)
            {
                return;
            }

            var mousePosition = GetCorrectPosition(this);

            if (mousePosition.X <= RibbonProperties.GetTitleBarHeight(this)
                && mousePosition.Y <= RibbonProperties.GetTitleBarHeight(this))
            {
                if ((DateTime.Now - lastMouseClick).TotalMilliseconds <= this.doubleclick)
                {
                    this.Close();
                    return;
                }
                this.lastMouseClick = DateTime.Now;

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
            POINT w32Mouse;
            UnsafeNativeMethods.GetCursorPos(out w32Mouse);
            return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }

        private void HandleTitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed
                && e.LeftButton == MouseButtonState.Pressed && WindowState == WindowState.Maximized
                && ResizeMode != ResizeMode.NoResize && !this.isContextMenuOpen)
            {
                // Calculating correct left coordinate for multi-screen system.
                Point mouseAbsolute = PointToScreen(Mouse.GetPosition(this));
                double width = this.RestoreBounds.Width;
                double left = mouseAbsolute.X - width / 2;

                // Aligning window's position to fit the screen.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

                var mousePosition = e.MouseDevice.GetPosition(this);

                // When dragging the window down at the very top of the border,
                // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released
                this.Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
                this.Left = left;

                // Restore window to normal state.
                this.WindowState = WindowState.Normal;

                this.DragMove();
            }
        }

        internal T GetPart<T>(string name) where T : DependencyObject
        {
            return (T)this.GetTemplateChild(name);
        }

        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null)
            {
                return;
            }

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !UnsafeNativeMethods.IsWindow(hwnd))
            {
                return;
            }

            var hmenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);

            var cmd = UnsafeNativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
            {
                UnsafeNativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
        }

        #endregion
    }
}