#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents basic window for ribbon
    /// </summary>
    public class RibbonWindow : Window
    {
        #region Constants

        /// <summary>
        /// Epsilon - more or less random, more or less small number.
        /// </summary>
        private const double Epsilon = 0.00000153;

        /// <summary>
        /// Version of WPF
        /// </summary>
        private static readonly Version PresentationFrameworkVersion = Assembly.GetAssembly(typeof(Window)).GetName().Version;

        private const int SwpFlags = NativeMethods.SWP_FRAMECHANGED | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOOWNERZORDER | NativeMethods.SWP_NOACTIVATE;

        /// <summary>
        /// Matrix of the hit test values to return when responding to NC window messages.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
        private static readonly int[,] HitTestBorders = new[,]
        {
            { NativeMethods.HTTOPLEFT,    NativeMethods.HTTOP,     NativeMethods.HTTOPRIGHT    },
            { NativeMethods.HTLEFT,       NativeMethods.HTCLIENT,  NativeMethods.HTRIGHT       },
            { NativeMethods.HTBOTTOMLEFT, NativeMethods.HTBOTTOM,  NativeMethods.HTBOTTOMRIGHT },
        };

        #endregion

        #region Fields

        // Window native data
        private IntPtr handle;
        private HwndSource hwndSource;
        private bool isHooked;

        // These fields are for tracking workarounds for WPF 3.5SP1 behaviors.
        private bool isFixedUp = false;
        private bool isUserResizing = false;
        private bool hasUserMovedWindow = false;
        private Point windowPosAtStartOfUserMove = default(Point);

        // Field to track attempts to force off Device Bitmaps on Win7.
        private int blackGlassFixupAttemptCount;

        // Keep track of this so we can detect when we need to apply changes.  Tracking these separately
        // as I've seen using just one cause things to get enough out of sync that occasionally the caption will redraw.
        private WindowState lastRoundingState;
        private WindowState lastMenuState;

        private Grid mainGrid;

        #endregion

        #region Properties

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
            DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(RibbonWindow), new UIPropertyMetadata(new Thickness(9)));

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
            DependencyProperty.Register("GlassBorderThickness", typeof(Thickness), typeof(RibbonWindow), new UIPropertyMetadata(new Thickness(9, 29, 9, 9), OnGlassBorderThicknessChanged));

        private static void OnGlassBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RibbonWindow).UpdateFrameState(false);
        }

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
            DependencyProperty.Register("CaptionHeight", typeof(double), typeof(RibbonWindow), new UIPropertyMetadata(20.0));

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
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonWindow), new UIPropertyMetadata(new CornerRadius(9, 9, 9, 9)));



        /// <summary>
        /// Is DWM Enabled
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public bool IsDwmEnabled
        {
            get { return (bool)GetValue(IsDwmEnabledProperty); }
            private set { SetValue(IsDwmEnabledPropertyKey, value); }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704")]
        private static readonly DependencyPropertyKey IsDwmEnabledPropertyKey = DependencyProperty.RegisterReadOnly("IsDwmEnabled", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Is DWM Enabled Dependency property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static readonly DependencyProperty IsDwmEnabledProperty = IsDwmEnabledPropertyKey.DependencyProperty;

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
            typeof(RibbonWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets whether client window area is activated
        /// </summary>
        public bool IsNonClientAreaActive
        {
            get { return (bool)GetValue(IsNonClientAreaActiveProperty); }
            private set { SetValue(IsNonClientAreaActivePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsNonClientAreaActivePropertyKey =
    DependencyProperty.RegisterReadOnly("IsNonClientAreaActive", typeof(bool),
    typeof(RibbonWindow), new UIPropertyMetadata(true));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsNcActivated.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsNonClientAreaActiveProperty =
            IsNonClientAreaActivePropertyKey.DependencyProperty;

        // Windows tries hard to hide this state from applications.
        // Generally you can tell that the window is in a docked position because the restore bounds from GetWindowPlacement
        // don't match the current window location and it's not in a maximized or minimized state.
        // Because this isn't docked or supported, it's also not incredibly consistent.  Sometimes some things get updated in
        // different orders, so this isn't absolutely reliable.
        private bool IsWindowDocked
        {
            get
            {
                if (WindowState != WindowState.Normal)
                {
                    return false;
                }

                NativeMethods.Rect adjustedOffset = GetAdjustedWindowRect(new NativeMethods.Rect() { Bottom = 100, Right = 100 });
                Point windowTopLeft = new Point(Left, Top);
                //windowTopLeft -= /*(Vector)DpiHelper.DevicePixelsToLogical(*/new Point(adjustedOffset.Left, adjustedOffset.Top)/*)*/;
                windowTopLeft.X -= adjustedOffset.Left;
                windowTopLeft.Y -= adjustedOffset.Top;

                return RestoreBounds.Location != windowTopLeft;
            }
        }

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
            StyleProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(typeof(RibbonWindow)));
            if (PresentationFrameworkVersion < new Version("4.0"))
            {
                // On older versions of the framework the client size of the window is incorrectly calculated.
                TemplateProperty.AddOwner(typeof(RibbonWindow), new FrameworkPropertyMetadata(OnWindowPropertyChangedThatRequiresTemplateFixup));
                FlowDirectionProperty.AddOwner(typeof(RibbonWindow), new FrameworkPropertyMetadata(OnWindowPropertyChangedThatRequiresTemplateFixup));
            }

            // Register commands
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(RibbonWindow.CloseCommand, OnCloseCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(RibbonWindow.MinimizeCommand, OnMinimizeCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(RibbonWindow.MaximizeCommand, OnMaximizeCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(RibbonWindow.NormalizeCommand, OnNormalizeCommandExecuted));
        }

        private static void OnWindowPropertyChangedThatRequiresTemplateFixup(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow window = d as RibbonWindow;
            if (window.handle != null)
            {
                // Assume that when the template changes it's going to be applied.
                // We don't have a good way to externally hook into the template
                // actually being applied, so we asynchronously post the fixup operation
                // at Loaded priority, so it's expected that the visual tree will be
                // updated before _FixupFrameworkIssues is called.
                window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)window.FixFrameworkIssues);
            }
        }

        // Coerce control style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = ((FrameworkElement)d).Resources["RibbonWindowStyle"] as Style ??
                              Application.Current.Resources["RibbonWindowStyle"] as Style;
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonWindow()
        {
            SizeChanged += OnSizeChanged;
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.NewSize.Width < /*Ribbon.MinimalVisibleWidth*/300) || (e.NewSize.Height < /*Ribbon.MinimalVisibleHeight*/200)) IsCollapsed = true;
            else IsCollapsed = false;
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
            handle = new WindowInteropHelper(this).Handle;
            hwndSource = HwndSource.FromHwnd(handle);

            IsDwmEnabled = NativeMethods.IsDwmEnabled();

            ApplyCustomChrome();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainGrid = GetTemplateChild("PART_MainGrid") as Grid;

            // Icon
            Image iconImage = GetTemplateChild("PART_IconImage") as Image;
            if (iconImage != null)
            {
                iconImage.MouseUp += (o, e) =>
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        if (e.ClickCount == 1)
                        {
                            System.Windows.Point pos = iconImage.PointToScreen(new System.Windows.Point(0, 0));
                            if (FlowDirection == FlowDirection.RightToLeft) pos.X += 16;
                            ShowSystemMenu(new System.Windows.Point(pos.X, pos.Y + 16));
                        }
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                    {
                        System.Windows.Point pos = iconImage.PointToScreen(Mouse.GetPosition(iconImage));
                        ShowSystemMenu(pos);
                    }
                };
                iconImage.MouseDown += (o, e) =>
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        if (e.ClickCount == 2)
                        {
                            Close();
                        }
                    }
                };
            }
        }

        #endregion

        #region Methods

        #endregion

        #region Private Methods

        private void ApplyCustomChrome()
        {

            if (!isHooked)
            {
                hwndSource.AddHook(WndProc);
                isHooked = true;
            }

            FixFrameworkIssues();

            // Force this the first time.
            UpdateSystemMenu(WindowState);
            UpdateFrameState(true);

            NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SwpFlags);
        }

        private void FixFrameworkIssues()
        {
            // This margin is only necessary if the client rectangle is going to be calculated incorrectly by WPF version less then 4.0
            if (PresentationFrameworkVersion >= new Version("4.0"))
            {
                return;
            }

            if (Template == null)
            {
                // Nothing to fixup yet.  This will get called again when a template does get set.
                return;
            }

            // Guard against the visual tree being empty.
            if (VisualTreeHelper.GetChildrenCount(this) == 0)
            {
                // The template isn't null, but we don't have a visual tree.
                // Hope that ApplyTemplate is in the queue and repost this, because there's not much we can do right now.
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)FixFrameworkIssues);
                return;
            }

            var rootElement = (FrameworkElement)VisualTreeHelper.GetChild(this, 0);

            NativeMethods.Rect rcWindow = new NativeMethods.Rect();
            NativeMethods.GetWindowRect(handle, ref rcWindow);
            NativeMethods.Rect rcAdjustedClient = GetAdjustedWindowRect(rcWindow);

            Rect rcLogicalWindow = /*DpiHelper.DeviceRectToLogical*/(new Rect(rcWindow.Left, rcWindow.Top, rcWindow.Right - rcWindow.Left, rcWindow.Bottom - rcWindow.Top));
            Rect rcLogicalClient = /*rcDpiHelper.DeviceRectToLogical*/(new Rect(rcAdjustedClient.Left, rcAdjustedClient.Top, rcAdjustedClient.Right - rcAdjustedClient.Left, rcAdjustedClient.Bottom - rcAdjustedClient.Top));

            Thickness nonClientThickness = new Thickness(
               rcLogicalWindow.Left - rcLogicalClient.Left,
               rcLogicalWindow.Top - rcLogicalClient.Top,
               rcLogicalClient.Right - rcLogicalWindow.Right,
               rcLogicalClient.Bottom - rcLogicalWindow.Bottom);

            rootElement.Margin = new Thickness(
                0,
                0,
                -(nonClientThickness.Left + nonClientThickness.Right),
                -(nonClientThickness.Top + nonClientThickness.Bottom));

            // The negative thickness on the margin doesn't properly get applied in RTL layouts.
            // The width is right, but there is a black bar on the right.
            // To fix this we just add an additional RenderTransform to the root element.
            // This works fine, but if the window is dynamically changing its FlowDirection then this can have really bizarre side effects.
            // This will mostly work if the FlowDirection is dynamically changed, but there aren't many real scenarios that would call for
            // that so I'm not addressing the rest of the quirkiness.
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                rootElement.RenderTransform = new MatrixTransform(1, 0, 0, 1, -(nonClientThickness.Left + nonClientThickness.Right), 0);
            }
            else
            {
                rootElement.RenderTransform = null;
            }

            if (!isFixedUp)
            {
                hasUserMovedWindow = false;
                StateChanged += FixRestoreBounds;

                isFixedUp = true;
            }
        }

        // There was a regression in DWM in Windows 7 with regard to handling WM_NCCALCSIZE to effect custom chrome.
        // When windows with glass are maximized on a multimonitor setup the glass frame tends to turn black.
        // Also when windows are resized they tend to flicker black, sometimes staying that way until resized again.
        //
        // This appears to be a bug in DWM related to device bitmap optimizations.  At least on RTM Win7 we can
        // evoke a legacy code path that bypasses the bug by calling an esoteric DWM function.  This doesn't affect
        // the system, just the application.
        // WPF also tends to call this function anyways during animations, so we're just forcing the issue
        // consistently and a bit earlier.
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void FixWindows7Issues()
        {
            if (blackGlassFixupAttemptCount > 5)
            {
                // Don't keep trying if there's an endemic problem with this.
                return;
            }

            if (IsDwmEnabled)
            {
                ++blackGlassFixupAttemptCount;

                bool success = false;
                try
                {
                    NativeMethods.DWM_TIMING_INFO? dti = NativeMethods.DwmGetCompositionTimingInfo(handle);
                    success = dti != null;
                }
                catch (Exception)
                {
                    // We aren't sure of all the reasons this could fail.
                    // If we find new ones we should consider making the NativeMethod swallow them as well.
                    // Since we have a limited number of retries and this method isn't actually critical, just repost.

                    // Disabling this for the published code to reduce debug noise.  This will get compiled away for retail binaries anyways.
                    //Assert.Fail(e.Message);
                }

                // NativeMethods.DwmGetCompositionTimingInfo swallows E_PENDING.
                // If the call wasn't successful, try again later.
                if (!success)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)FixWindows7Issues);
                }
                else
                {
                    // Reset this.  We will want to force this again if DWM composition changes.
                    blackGlassFixupAttemptCount = 0;
                }
            }
        }

        private void FixRestoreBounds(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized || WindowState == WindowState.Minimized)
            {
                // Old versions of WPF sometimes force their incorrect idea of the Window's location
                // on the Win32 restore bounds.  If we have reason to think this is the case, then
                // try to undo what WPF did after it has done its thing.
                if (hasUserMovedWindow)
                {
                    hasUserMovedWindow = false;
                    NativeMethods.WINDOWPLACEMENT wp = new NativeMethods.WINDOWPLACEMENT();
                    NativeMethods.GetWindowPlacement(handle, wp);

                    NativeMethods.Rect adjustedDeviceRc = /*GetAdjustedWindowRect(*/new NativeMethods.Rect { Bottom = 100, Right = 100 }/*)*/;
                    Point adjustedTopLeft = /*DpiHelper.DevicePixelsToLogical(*/
                        new Point(
                            wp.rcNormalPosition.Left - adjustedDeviceRc.Left,
                            wp.rcNormalPosition.Top - adjustedDeviceRc.Top)/*)*/;

                    Top = adjustedTopLeft.Y;
                    Left = adjustedTopLeft.X;
                }
            }
        }

        private NativeMethods.Rect GetAdjustedWindowRect(NativeMethods.Rect rcWindow)
        {
            var style = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWL_STYLE);
            var exstyle = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWL_EXSTYLE);

            NativeMethods.AdjustWindowRectEx(ref rcWindow, (int)style, false, (int)exstyle);
            return rcWindow;
        }

        /// <summary>Add and remove a native WindowStyle from the HWND.</summary>
        /// <param name="removeStyle">The styles to be removed.  These can be bitwise combined.</param>
        /// <param name="addStyle">The styles to be added.  These can be bitwise combined.</param>
        /// <returns>Whether the styles of the HWND were modified as a result of this call.</returns>
        private bool ModifyStyle(int removeStyle, int addStyle)
        {
            var dwStyle = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWL_STYLE).ToInt32();
            var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
            if (dwStyle == dwNewStyle)
            {
                return false;
            }

            NativeMethods.SetWindowLongPtr(handle, NativeMethods.GWL_STYLE, new IntPtr((int)dwNewStyle));
            return true;
        }

        private int HitTestNonClientArea(Rect windowPosition, Point mousePosition)
        {
            // Determine if hit test is for resizing, default middle (1,1).
            int uRow = 1;
            int uCol = 1;
            bool onResizeBorder = false;

            // Determine if the point is at the top or bottom of the window.
            if (mousePosition.Y >= windowPosition.Top && mousePosition.Y < windowPosition.Top + ResizeBorderThickness.Top + CaptionHeight)
            {
                onResizeBorder = (mousePosition.Y < (windowPosition.Top + ResizeBorderThickness.Top));
                uRow = 0; // top (caption or resize border)
            }
            else if (mousePosition.Y < windowPosition.Bottom && mousePosition.Y >= windowPosition.Bottom - (int)ResizeBorderThickness.Bottom)
            {
                uRow = 2; // bottom
            }

            // Determine if the point is at the left or right of the window.
            if (mousePosition.X >= windowPosition.Left && mousePosition.X < windowPosition.Left + (int)ResizeBorderThickness.Left)
            {
                uCol = 0; // left side
            }
            else if (mousePosition.X < windowPosition.Right && mousePosition.X >= windowPosition.Right - ResizeBorderThickness.Right)
            {
                uCol = 2; // right side
            }

            // If the cursor is in one of the top edges by the caption bar, but below the top resize border,
            // then resize left-right rather than diagonally.
            if (uRow == 0 && uCol != 1 && !onResizeBorder)
            {
                uRow = 1;
            }

            int ht = HitTestBorders[uRow, uCol];

            if (ht == NativeMethods.HTTOP && !onResizeBorder)
            {
                ht = NativeMethods.HTCAPTION;
            }

            return ht;
        }

        /// <summary>Display the system menu at a specified location.</summary>
        /// <param name="screenLocation">The location to display the system menu, in logical screen coordinates.</param>
        public void ShowSystemMenu(Point screenLocation)
        {
            ShowSystemMenuPhysicalCoordinates(screenLocation);
        }

        private void ShowSystemMenuPhysicalCoordinates(Point physicalScreenLocation)
        {
            const uint TPM_RETURNCMD = 0x0100;
            const uint TPM_LEFTBUTTON = 0x0;

            if (handle == IntPtr.Zero)
            {
                return;
            }

            IntPtr hmenu = NativeMethods.GetSystemMenu(handle, false);

            uint cmd = NativeMethods.TrackPopupMenuEx(hmenu, TPM_LEFTBUTTON | TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, handle, IntPtr.Zero);
            if (0 != cmd)
            {
                NativeMethods.PostMessage(handle, NativeMethods.WM_SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
        }

        /// <summary>
        /// Get the WindowState as the native HWND knows it to be.  This isn't necessarily the same as what Window thinks.
        /// </summary>
        private WindowState GetHwndState()
        {
            NativeMethods.WINDOWPLACEMENT wpl = new NativeMethods.WINDOWPLACEMENT();
            NativeMethods.GetWindowPlacement(handle, wpl);
            switch (wpl.showCmd)
            {
                case NativeMethods.SW_SHOWMINIMIZED: return WindowState.Minimized;
                case NativeMethods.SW_SHOWMAXIMIZED: return WindowState.Maximized;
            }
            return WindowState.Normal;
        }

        // Check flag set
        private static bool IsFlagSet(int value, int mask)
        {
            return 0 != (value & mask);
        }

        /// <summary>
        /// Update the items in the system menu based on the current, or assumed, WindowState.
        /// </summary>
        /// <param name="assumeState">
        /// The state to assume that the Window is in.  This can be null to query the Window's state.
        /// </param>
        /// <remarks>
        /// We want to update the menu while we have some control over whether the caption will be repainted.
        /// </remarks>
        private void UpdateSystemMenu(WindowState? assumeState)
        {
            const uint mfEnabled = NativeMethods.MF_ENABLED | NativeMethods.MF_BYCOMMAND;
            const uint mfDisabled = NativeMethods.MF_GRAYED | NativeMethods.MF_DISABLED | NativeMethods.MF_BYCOMMAND;

            WindowState state = assumeState ?? GetHwndState();

            if (null != assumeState || lastMenuState != state)
            {
                lastMenuState = state;

                bool modified = ModifyStyle(NativeMethods.WS_VISIBLE, 0);
                IntPtr hmenu = NativeMethods.GetSystemMenu(handle, false);
                if (IntPtr.Zero != hmenu)
                {
                    var dwStyle = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWL_STYLE).ToInt32();

                    bool canMinimize = IsFlagSet((int)dwStyle, (int)NativeMethods.WS_MINIMIZEBOX);
                    bool canMaximize = IsFlagSet((int)dwStyle, (int)NativeMethods.WS_MAXIMIZEBOX);
                    bool canSize = IsFlagSet((int)dwStyle, (int)NativeMethods.WS_THICKFRAME);

                    switch (state)
                    {
                        case WindowState.Maximized:
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_RESTORE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MOVE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_SIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MINIMIZE, canMinimize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MAXIMIZE, mfDisabled);
                            break;
                        case WindowState.Minimized:
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_RESTORE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MOVE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_SIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MINIMIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MAXIMIZE, canMaximize ? mfEnabled : mfDisabled);
                            break;
                        default:
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_RESTORE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MOVE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_SIZE, canSize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MINIMIZE, canMinimize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MAXIMIZE, canMaximize ? mfEnabled : mfDisabled);
                            break;
                    }
                }

                if (modified)
                {
                    ModifyStyle(0, NativeMethods.WS_VISIBLE);
                }
            }
        }

        private void UpdateFrameState(bool force)
        {
            if (IntPtr.Zero == handle)
            {
                return;
            }

            if (!IsDwmEnabled)
            {
                SetRoundingRegion(null);
            }
            else
            {
                ClearRoundingRegion();
                ExtendGlassFrame();

                FixWindows7Issues();
            }

            NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SwpFlags);

        }

        private void ClearRoundingRegion()
        {
            NativeMethods.SetWindowRgn(handle, IntPtr.Zero, NativeMethods.IsWindowVisible(handle));
        }

        private void SetRoundingRegion(NativeMethods.WINDOWPOS? wp)
        {
            const int MONITOR_DEFAULTTONEAREST = 0x00000002;

            // We're early - WPF hasn't necessarily updated the state of the window.
            // Need to query it ourselves.
            NativeMethods.WINDOWPLACEMENT wpl = new NativeMethods.WINDOWPLACEMENT();
            NativeMethods.GetWindowPlacement(handle, wpl);

            if (wpl.showCmd == NativeMethods.SW_SHOWMAXIMIZED)
            {
                int left;
                int top;

                if (wp.HasValue)
                {
                    left = wp.Value.x;
                    top = wp.Value.y;
                }
                else
                {
                    NativeMethods.Rect r = new NativeMethods.Rect();
                    NativeMethods.GetWindowRect(handle, ref r);
                    left = r.Left;
                    top = r.Top;
                }

                IntPtr hMon = NativeMethods.MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);

                NativeMethods.MonitorInfo mi = new NativeMethods.MonitorInfo();
                NativeMethods.GetMonitorInfo(hMon, mi);
                NativeMethods.Rect rcMax = mi.Work;
                // The location of maximized window takes into account the border that Windows was
                // going to remove, so we also need to consider it.
                rcMax.Left -= left; rcMax.Right -= left;
                rcMax.Top -= top; rcMax.Bottom -= top;

                IntPtr hrgn = IntPtr.Zero;
                try
                {
                    hrgn = NativeMethods.CreateRectRgnIndirect(ref rcMax);
                    NativeMethods.SetWindowRgn(handle, hrgn, NativeMethods.IsWindowVisible(handle));
                    hrgn = IntPtr.Zero;
                }
                finally
                {
                    NativeMethods.DeleteObject(hrgn);
                }
            }
            else
            {
                Size windowSize;

                // Use the size if it's specified.
                if (null != wp && !IsFlagSet(wp.Value.flags, NativeMethods.SWP_NOSIZE))
                {
                    windowSize = new Size((double)wp.Value.cx, (double)wp.Value.cy);
                }
                else if (null != wp && (lastRoundingState == WindowState))
                {
                    return;
                }
                else
                {
                    NativeMethods.Rect r = new NativeMethods.Rect();
                    NativeMethods.GetWindowRect(handle, ref r);
                    Rect rect = new Rect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
                    windowSize = rect.Size;
                }

                lastRoundingState = WindowState;

                IntPtr hrgn = IntPtr.Zero;
                try
                {
                    double shortestDimension = Math.Min(windowSize.Width, windowSize.Height);

                    double topLeftRadius = /*DpiHelper.LogicalPixelsToDevice*/(new Point(CornerRadius.TopLeft, 0)).X;
                    topLeftRadius = Math.Min(topLeftRadius, shortestDimension / 2);

                    if (IsUniform(CornerRadius))
                    {
                        // RoundedRect HRGNs require an additional pixel of padding.
                        hrgn = CreateRoundRectRgn(new Rect(windowSize), topLeftRadius);
                    }
                    else
                    {
                        // We need to combine HRGNs for each of the corners.
                        // Create one for each quadrant, but let it overlap into the two adjacent ones
                        // by the radius amount to ensure that there aren't corners etched into the middle
                        // of the window.
                        hrgn = CreateRoundRectRgn(new Rect(0, 0, windowSize.Width / 2 + topLeftRadius, windowSize.Height / 2 + topLeftRadius), topLeftRadius);

                        double topRightRadius = /*DpiHelper.LogicalPixelsToDevice*/(new Point(CornerRadius.TopRight, 0)).X;
                        topRightRadius = Math.Min(topRightRadius, shortestDimension / 2);
                        Rect topRightRegionRect = new Rect(0, 0, windowSize.Width / 2 + topRightRadius, windowSize.Height / 2 + topRightRadius);
                        topRightRegionRect.Offset(windowSize.Width / 2 - topRightRadius, 0);

                        CreateAndCombineRoundRectRgn(hrgn, topRightRegionRect, topRightRadius);

                        double bottomLeftRadius = /*DpiHelper.LogicalPixelsToDevice*/(new Point(CornerRadius.BottomLeft, 0)).X;
                        bottomLeftRadius = Math.Min(bottomLeftRadius, shortestDimension / 2);
                        Rect bottomLeftRegionRect = new Rect(0, 0, windowSize.Width / 2 + bottomLeftRadius, windowSize.Height / 2 + bottomLeftRadius);
                        bottomLeftRegionRect.Offset(0, windowSize.Height / 2 - bottomLeftRadius);

                        CreateAndCombineRoundRectRgn(hrgn, bottomLeftRegionRect, bottomLeftRadius);

                        double bottomRightRadius = /*DpiHelper.LogicalPixelsToDevice*/(new Point(CornerRadius.BottomRight, 0)).X;
                        bottomRightRadius = Math.Min(bottomRightRadius, shortestDimension / 2);
                        Rect bottomRightRegionRect = new Rect(0, 0, windowSize.Width / 2 + bottomRightRadius, windowSize.Height / 2 + bottomRightRadius);
                        bottomRightRegionRect.Offset(windowSize.Width / 2 - bottomRightRadius, windowSize.Height / 2 - bottomRightRadius);

                        CreateAndCombineRoundRectRgn(hrgn, bottomRightRegionRect, bottomRightRadius);
                    }

                    NativeMethods.SetWindowRgn(handle, hrgn, NativeMethods.IsWindowVisible(handle));
                    hrgn = IntPtr.Zero;
                }
                finally
                {
                    // Free the memory associated with the HRGN if it wasn't assigned to the HWND.
                    NativeMethods.DeleteObject(hrgn);
                }
            }
        }

        /// <summary>
        /// AreClose returns whether or not two doubles are "close".  That is, whether or 
        /// not they are within epsilon of each other.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false. 
        /// </summary>
        /// <param name="value1">The first double to compare.</param>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>The result of the AreClose comparision.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            double delta = value1 - value2;
            return (delta < Epsilon) && (delta > -Epsilon);
        }

        private static IntPtr CreateRoundRectRgn(Rect region, double radius)
        {
            // Round outwards.

            if (AreClose(0, radius))
            {
                return NativeMethods.CreateRectRgn(
                    (int)Math.Floor(region.Left),
                    (int)Math.Floor(region.Top),
                    (int)Math.Ceiling(region.Right),
                    (int)Math.Ceiling(region.Bottom));
            }

            // RoundedRect HRGNs require an additional pixel of padding on the bottom right to look correct.
            return NativeMethods.CreateRoundRectRgn(
                (int)Math.Floor(region.Left),
                (int)Math.Floor(region.Top),
                (int)Math.Ceiling(region.Right) + 1,
                (int)Math.Ceiling(region.Bottom) + 1,
                (int)Math.Ceiling(radius),
                (int)Math.Ceiling(radius));
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HRGNs")]
        private static void CreateAndCombineRoundRectRgn(IntPtr hrgnSource, Rect region, double radius)
        {
            IntPtr hrgn = IntPtr.Zero;
            try
            {
                hrgn = CreateRoundRectRgn(region, radius);
                int result = NativeMethods.CombineRgn(hrgnSource, hrgnSource, hrgn, NativeMethods.RGN_OR);
                if (result == 0)
                {
                    throw new InvalidOperationException("Unable to combine two HRGNs.");
                }
            }
            catch
            {
                NativeMethods.DeleteObject(hrgn);
                throw;
            }
        }

        private static bool IsUniform(CornerRadius cornerRadius)
        {
            if (!AreClose(cornerRadius.BottomLeft, cornerRadius.BottomRight))
            {
                return false;
            }

            if (!AreClose(cornerRadius.TopLeft, cornerRadius.TopRight))
            {
                return false;
            }

            if (!AreClose(cornerRadius.BottomLeft, cornerRadius.TopRight))
            {
                return false;
            }

            return true;
        }

        private void ExtendGlassFrame()
        {

            if (IntPtr.Zero == handle)
            {
                // Can't do anything with this call until the Window has been shown.
                return;
            }

            // Ensure standard HWND background painting when DWM isn't enabled.
            if (!IsDwmEnabled)
            {
                hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
            }
            else
            {
                // This makes the glass visible at a Win32 level so long as nothing else is covering it.
                // The Window's Background needs to be changed independent of this.

                // Apply the transparent background to the HWND
                hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;

                // Thickness is going to be DIPs, need to convert to system coordinates.
                Point deviceTopLeft = /*DpiHelper.LogicalPixelsToDevice*/(new Point(GlassBorderThickness.Left, GlassBorderThickness.Top));
                Point deviceBottomRight = /*DpiHelper.LogicalPixelsToDevice*/(new Point(GlassBorderThickness.Right, GlassBorderThickness.Bottom));

                var dwmMargin = new NativeMethods.MARGINS((int)Math.Ceiling(deviceTopLeft.X), (int)Math.Ceiling(deviceTopLeft.Y), (int)Math.Ceiling(deviceBottomRight.X), (int)Math.Ceiling(deviceBottomRight.Y));
                NativeMethods.DwmExtendFrameIntoClientArea(handle, dwmMargin);
            }
        }

        #endregion

        #region Window Function

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_SETTEXT:
                case NativeMethods.WM_SETICON:
                    {
                        bool modified = ModifyStyle(NativeMethods.WS_VISIBLE, 0);

                        // Setting the caption text and icon cause Windows to redraw the caption.
                        // Letting the default WndProc handle the message without the WS_VISIBLE
                        // style applied bypasses the redraw.
                        IntPtr lRet = NativeMethods.DefWindowProc(handle, msg, wParam, lParam);

                        // Put back the style we removed.
                        if (modified)
                        {
                            ModifyStyle(0, NativeMethods.WS_VISIBLE);
                        }
                        handled = true;
                        return lRet;
                    }
                case NativeMethods.WM_NCACTIVATE:
                    {
                        // Despite MSDN's documentation of lParam not being used,
                        // calling DefWindowProc with lParam set to -1 causes Windows not to draw over the caption.

                        // Directly call DefWindowProc with a custom parameter
                        // which bypasses any other handling of the message.
                        IntPtr lRet = NativeMethods.DefWindowProc(handle, NativeMethods.WM_NCACTIVATE, wParam, new IntPtr(-1));
                        IsNonClientAreaActive = (wParam != IntPtr.Zero);
                        handled = true;
                        return lRet;
                    }
                case NativeMethods.WM_NCCALCSIZE:
                    {
                        handled = true;
                        return new IntPtr((int)NativeMethods.WVR_REDRAW);
                    }
                case NativeMethods.WM_NCHITTEST:
                    {
                        IntPtr lRet = IntPtr.Zero;
                        handled = false;

                        // Give DWM a chance at this first.
                        if (IsDwmEnabled)
                        {
                            // If we're on Vista, give the DWM a chance to handle the message first.
                            handled = NativeMethods.DwmDefWindowProc(handle, msg, wParam, lParam, ref lRet);
                        }

                        // Handle letting the system know if we consider the mouse to be in our effective non-client area.
                        // If DWM already handled this by way of DwmDefWindowProc, then respect their call.
                        if (IntPtr.Zero == lRet)
                        {
                            var mousePosScreen = new Point(NativeMethods.LowWord(lParam), NativeMethods.HiWord(lParam));
                            NativeMethods.Rect wndPosition = new NativeMethods.Rect();
                            NativeMethods.GetWindowRect(handle, ref wndPosition);
                            Rect windowPosition = new Rect(wndPosition.Left, wndPosition.Top, wndPosition.Right - wndPosition.Left, wndPosition.Bottom - wndPosition.Top);

                            int ht = HitTestNonClientArea(
                                /*DpiHelper.DeviceRectToLogical(*/windowPosition/*)*/,
                                /*DpiHelper.DevicePixelsToLogical(*/mousePosScreen)/*)*/;

                            // Don't blindly respect HTCAPTION.
                            // We want UIElements in the caption area to be actionable so run through a hittest first.
                            if ((ht != NativeMethods.HTCLIENT) && (mainGrid != null))
                            {
                                Point mousePosWindow = mousePosScreen;
                                mousePosWindow.Offset(-windowPosition.X, -windowPosition.Y);
                                //mousePosWindow = /*DpiHelper.DevicePixelsToLogical(*/mousePosWindow/*)*/;
                                IInputElement inputElement = mainGrid.InputHitTest(mousePosWindow);
                                if (inputElement != null)
                                {
                                    if ((inputElement as FrameworkElement).Name == "PART_TitleBar") ht = NativeMethods.HTCAPTION;
                                    else if (inputElement != mainGrid) ht = NativeMethods.HTCLIENT;
                                }
                            }
                            handled = true;
                            lRet = new IntPtr((int)ht);
                        }
                        return lRet;
                    }
                case NativeMethods.WM_NCRBUTTONUP:
                    {
                        // Emulate the system behavior of clicking the right mouse button over the caption area
                        // to bring up the system menu.
                        if (NativeMethods.HTCAPTION == wParam.ToInt32())
                        {
                            ShowSystemMenuPhysicalCoordinates(new Point(NativeMethods.LowWord(lParam), NativeMethods.HiWord(lParam)));
                        }
                        handled = false;
                        return IntPtr.Zero;
                    }
                case NativeMethods.WM_SIZE:
                    {
                        const int SIZE_MAXIMIZED = 2;

                        // Force when maximized.
                        // We can tell what's happening right now, but the Window doesn't yet know it's
                        // maximized.  Not forcing this update will eventually cause the
                        // default caption to be drawn.
                        WindowState? state = null;
                        if (wParam.ToInt32() == SIZE_MAXIMIZED)
                        {
                            state = WindowState.Maximized;
                        }
                        UpdateSystemMenu(state);

                        // Still let the default WndProc handle this.
                        handled = false;
                        return IntPtr.Zero;
                    }
                case NativeMethods.WM_WINDOWPOSCHANGED:
                    {

                        if (!IsDwmEnabled)
                        {
                            var wp = (NativeMethods.WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(NativeMethods.WINDOWPOS));
                            SetRoundingRegion(wp);
                        }

                        // Still want to pass this to DefWndProc
                        handled = false;
                        return IntPtr.Zero;
                    }
                case NativeMethods.WM_DWMCOMPOSITIONCHANGED:
                    {
                        IsDwmEnabled = NativeMethods.IsDwmEnabled();

                        UpdateFrameState(false);

                        handled = false;
                        return IntPtr.Zero;
                    }
            }

            if (PresentationFrameworkVersion < new Version("4.0"))
            {
                switch (msg)
                {
                    case NativeMethods.WM_SETTINGCHANGE:
                        {
                            FixFrameworkIssues();

                            handled = false;
                            return IntPtr.Zero;
                        }
                    case NativeMethods.WM_ENTERSIZEMOVE:
                        {
                            isUserResizing = true;

                            if (WindowState != WindowState.Maximized)
                            {
                                // Check for the docked window case.  The window can still be restored when it's in this position so 
                                // try to account for that and not update the start position.
                                if (!IsWindowDocked)
                                {
                                    windowPosAtStartOfUserMove = new Point(Left, Top);
                                }
                                // Realistically we also don't want to update the start position when moving from one docked state to another (or to and from maximized),
                                // but it's tricky to detect and this is already a workaround for a bug that's fixed in newer versions of the framework.
                                // Not going to try to handle all cases.
                            }

                            handled = false;
                            return IntPtr.Zero;
                        }
                    case NativeMethods.WM_EXITSIZEMOVE:
                        {
                            isUserResizing = false;

                            // On Win7 the user can change the Window's state by dragging the window to the top of the monitor.
                            // If they did that, then we need to try to update the restore bounds or else WPF will put the window at the maximized location (e.g. (-8,-8)).
                            if (WindowState == WindowState.Maximized)
                            {
                                Top = windowPosAtStartOfUserMove.Y;
                                Left = windowPosAtStartOfUserMove.X;
                            }

                            handled = false;
                            return IntPtr.Zero;
                        }
                    case NativeMethods.WM_MOVE:
                        {
                            if (isUserResizing)
                            {
                                hasUserMovedWindow = true;
                            }

                            handled = false;
                            return IntPtr.Zero;
                        }
                }
            }

            return IntPtr.Zero;
        }

        #endregion
    }
}
