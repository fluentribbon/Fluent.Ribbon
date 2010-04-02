#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
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
    public class RibbonWindow:Window
    {        
        #region Fields

        // Window handle
        private IntPtr handle = IntPtr.Zero;

        // Main grid
        private Grid mainGrid;
        // Size border
        private Border sizeBorder;
        // Icon image
        private Image iconImage;
        // Image resize grip
        private Image imageResizeGrip;
        // Title bar
        private Grid titleBar;

        // Sizes
        private Thickness sizers;

        #endregion

        #region Properties 

        /// <summary>
        /// Is Dwm Enabled
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public bool IsDwmEnabled
        {
            get { return (bool)GetValue(IsDwmEnabledProperty); }
            private set { SetValue(IsDwmEnabledProperty, value); }
        }
        /// <summary>
        /// Is Dwm Enabled Dependency property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static readonly DependencyProperty IsDwmEnabledProperty = DependencyProperty.Register("IsDwmEnabled", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Отображается ли иконка
        /// </summary>
        public bool IsIconVisible
        {
            get { return (bool)GetValue(IsIconVisibleProperty); }
            set { SetValue(IsIconVisibleProperty, value); }
        }
        /// <summary>
        /// Отображается ли иконка
        /// </summary>
        public static readonly DependencyProperty IsIconVisibleProperty = DependencyProperty.Register("IsIconVisible", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(true));

        /// <summary>
        /// Глассовые бордюры
        /// </summary>
        public Thickness GlassBorders
        {
            get { return (Thickness)GetValue(GlassBordersProperty); }
            set
            {
                value = new Thickness(value.Left > 0 ? value.Left : 0, value.Top > 0 ? value.Top : 0, value.Right > 0 ? value.Right : 0, value.Bottom > 0 ? value.Bottom : 0);
                SetValue(GlassBordersProperty, value);
            }
        }
        /// <summary>
        /// Глассовые бордюры
        /// </summary>
        public static readonly DependencyProperty GlassBordersProperty = DependencyProperty.Register("GlassBorders", typeof(Thickness), typeof(RibbonWindow), new UIPropertyMetadata(new Thickness(0, 20, 0, 0), OnGlassBordersChanged));

        // Handles GlassBorder property changes
        private static void OnGlassBordersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow window = (RibbonWindow)d;
            if (window.IsDwmEnabled) window.DwmInit();
        }

        /// <summary>
        /// Высота заголовка окна(за него можно таксать окно и на нем появляется системное меню)
        /// </summary>
        public double TitleBarHeight
        {
            get { return (double)GetValue(TitleBarHeightProperty); }
            set { SetValue(TitleBarHeightProperty, value); }
        }
        /// <summary>
        /// Высота заголовка окна(за него можно таксать окно и на нем появляется системное меню)
        /// </summary>
        public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register("TitleBarHeight", typeof(double), typeof(RibbonWindow), new UIPropertyMetadata(20.0));

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
            private set { SetValue(IsNonClientAreaActiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsNcActivated.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsNonClientAreaActiveProperty =
            DependencyProperty.Register("IsNonClientAreaActive", typeof(bool), 
            typeof(RibbonWindow), new UIPropertyMetadata(false));


        #endregion

        #region Commands

        // TODO: why are these ribbon window's commands static?

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
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonWindow()
        {
            StyleProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(typeof(RibbonWindow)));
            // Register commands
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(RibbonWindow.CloseCommand, OnCloseCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(RibbonWindow.MinimizeCommand, OnMinimizeCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(RibbonWindow.MaximizeCommand, OnMaximizeCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(RibbonWindow.NormalizeCommand, OnNormalizeCommandExecuted));
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
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        public RibbonWindow()
        {            
            IsDwmEnabled = NativeMethods.IsDwmEnabled();
            Loaded += OnLoaded;
            SourceInitialized += OnSourceInitialized;
            SizeChanged += OnSizeChanged;
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.NewSize.Width < Ribbon.MinimalVisibleWidth)||(e.NewSize.Height < Ribbon.MinimalVisibleHeight)) IsCollapsed = true;
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

        #region Public methods

        /// <summary>
        /// Display window system menu
        /// </summary>
        /// <param name="point">Point to display</param>
        /// <returns>True if menu displayed, else false</returns>
        public bool ShowSystemMenu(Point point)
        {
            bool flag = false;
            uint num = NativeMethods.TrackPopupMenuEx(NativeMethods.GetSystemMenu(handle, false), 0x100, (int)point.X, (int)point.Y, handle, IntPtr.Zero);
            if (num != 0)
            {
                NativeMethods.PostMessage(handle, NativeMethods.WM_SYSCOMMAND, new IntPtr(num), IntPtr.Zero);
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// Gets window border width
        /// </summary>
        /// <returns>Window border width</returns>
        public static double BorderWidth
        {
            get { return SystemParameters.ResizeFrameVerticalBorderWidth; }
        }

        /// <summary>
        /// Get window border height
        /// </summary>
        /// <returns>Window border height</returns>
        public static double BorderHeight
        {
            get { return SystemParameters.ResizeFrameHorizontalBorderHeight; }
        }

        #endregion

        #region Event handling

        // Handle window loaded
        void OnLoaded(object sender, EventArgs e)
        {
            IsDwmEnabled = NativeMethods.IsDwmEnabled();
            if (IsDwmEnabled)
            {
                DwmInit();
                Activate();
                UpdateWindowStyle();
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Titlebar
            titleBar = GetTemplateChild("PART_TitleBar") as Grid;
            // Icon
            iconImage = GetTemplateChild("PART_IconImage") as Image;
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

            // Resize grip
            imageResizeGrip = GetTemplateChild("PART_ImageResizeGrip") as Image;

            // Main window grid
            mainGrid = GetTemplateChild("PART_MainGrid") as Grid;

            // Size change border
            sizeBorder = GetTemplateChild("PART_SizeBorder") as Border;

            // Set resize margins
            if (mainGrid != null) sizers = mainGrid.Margin;

            if (IsDwmEnabled) DwmInit();
            else NonDwmInit();
            UpdateLayout();
        }

        // Handles source initialize
        private void OnSourceInitialized(object sender, EventArgs e)
        {
            handle = (new WindowInteropHelper(this)).Handle;
            // Ставим хук на оконную функцию
            HwndSource.FromHwnd(handle).AddHook(WindowProc);
            UpdateWindowStyle();
        }
        
        #endregion

        #region Window functions

        // Main window fucntion
        private IntPtr WindowProc(
                   IntPtr hwnd,
                   int msg,
                   IntPtr wParam,
                   IntPtr lParam,
                   ref bool handled)
        {
            switch (msg)
            {
                // Handles DWM composition changes
                case NativeMethods.WM_DWMCOMPOSITIONCHANGED:
                    {
                        WindowState state = WindowState;
                        if (WindowState == WindowState.Maximized) WindowState = WindowState.Minimized;
                        IsDwmEnabled = NativeMethods.IsDwmEnabled();
                        UpdateWindowStyle();
                        if (IsDwmEnabled)
                        {                            
                            DwmInit();
                            NativeMethods.SetWindowRgn(handle, IntPtr.Zero, true);
                        }
                        else
                        {
                            NonDwmInit();
                        }
                        UpdateWindowStyle();
                        WindowState = state;
                        Activate();
                        break;
                    }
                // Handles right mouse up on nonclient area
                case NativeMethods.WM_NCRBUTTONUP:
                    {
                        IntPtr ncHitTest = DoNcHitTest(msg, wParam, lParam);
                        if (ncHitTest.ToInt32() == NativeMethods.HTCAPTION) NativeMethods.SendMessage(hwnd, NativeMethods.WM_GETSYSMENU, IntPtr.Zero, lParam);
                        break;
                    }
                // Handles hittest
                case NativeMethods.WM_NCHITTEST:
                    {
                        IntPtr ncHitTest = DoNcHitTest(msg, wParam, lParam);
                        if (ncHitTest != IntPtr.Zero)
                        {
                            handled = true;
                            return ncHitTest;
                        }
                        break;
                    }
                case NativeMethods.WM_NCCREATE:
                case NativeMethods.WM_NCDESTROY:
                case NativeMethods.WM_NCPAINT: return IntPtr.Zero;
            }
            if (IsDwmEnabled) return DwmWindowProc(hwnd, msg, wParam, lParam, ref handled);
            else return NonDwmWindowProc(hwnd, msg, wParam, lParam, ref handled);
        }

        // Main window function on enabled DWN
        [SuppressMessage("Microsoft.Usage", "CA1801")]
        IntPtr DwmWindowProc(
               IntPtr hwnd,
               int msg,
               IntPtr wParam,
               IntPtr lParam,
               ref bool handled)
        {
            switch (msg)
            {
                // Handles nonclient ares size changed
                case NativeMethods.WM_NCCALCSIZE:
                    {
                        if (wParam.ToInt32() == 1)
                        {
                            handled = true;
                            return IntPtr.Zero;
                        }

                        break;
                    }
                // Handles window size changed
                case NativeMethods.WM_SIZE:
                    {
                        int lP = wParam.ToInt32();
                        if (lP == NativeMethods.SIZE_RESTORED)
                        {
                            DwmInit();
                        }
                        else if (lP == NativeMethods.SIZE_MAXIMIZED)
                        {                            
                            DwmInit();
                        }
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        // Main window function on disabled DWN
        [SuppressMessage("Microsoft.Usage", "CA1801")]
        IntPtr NonDwmWindowProc(
               IntPtr hwnd,
               int msg,
               IntPtr wParam,
               IntPtr lParam,
               ref bool handled)
        {
            switch (msg)
            {
                // Handles window min max size changed
                case NativeMethods.WM_GETMINMAXINFO:
                    NonDwmGetMinMaxInfo(lParam);
                    handled = true;
                    break;
                case NativeMethods.WM_NCACTIVATE:
                    IsNonClientAreaActive = (wParam != IntPtr.Zero);
                    break;
                // Handles window size changed
                case NativeMethods.WM_SIZE:
                    {
                        int lP = wParam.ToInt32();
                        if (lP == NativeMethods.SIZE_RESTORED)
                        {
                            mainGrid.Margin = sizers;
                         
                            int w = NativeMethods.LowWord(lParam.ToInt32());
                            int h = NativeMethods.HiWord(lParam.ToInt32());
                            mainGrid.Margin = sizers;
                            
                            if ((!Double.IsNaN(Width)) && ((Double.IsNaN(Height))) && ((ResizeMode != ResizeMode.CanResize) && (ResizeMode != ResizeMode.CanResizeWithGrip)))
                            {
                                SetNonDwmRgn(w + 2 * (int)BorderWidth - 2,
                                             h + 2 * (int)BorderHeight);
                            }
                            else SetNonDwmRgn(w + 2 * (int)BorderWidth - 2,
                                             h + 2 * (int)BorderHeight - 2);
                            
                        }
                        else if (lP == NativeMethods.SIZE_MAXIMIZED)
                        {
                            mainGrid.Margin = new Thickness(0, 1, 0, 0);
                            
                            //
                            int borderWidth = (int)BorderWidth;
                            int borderHeight = (int)BorderHeight;
                            Rect rect = GetCurrentWorkarea();
                            IntPtr hRgn = NativeMethods.CreateRectRgn(
                                borderWidth,
                                borderHeight,
                                (int)rect.Width - borderWidth,
                                (int)rect.Height - borderHeight);
                            NativeMethods.SetWindowRgn(handle, hRgn, true);
                            NativeMethods.DeleteObject(hRgn);
                        }

                        break;
                    }
            }
            return IntPtr.Zero;
        }

        #endregion

        #region Private methods

        // Do hittest
        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        IntPtr DoNcHitTest(int msg, IntPtr wParam, IntPtr lParam)
        {
            int mp = lParam.ToInt32();
            if(!mainGrid.IsVisible) return IntPtr.Zero;
            Point ptMouse = new Point((short)(mp & 0x0000FFFF), (short)((mp >> 16) & 0x0000FFFF));
            ptMouse = mainGrid.PointFromScreen(ptMouse);
            IInputElement hitTested = mainGrid.InputHitTest(ptMouse);
            if ((hitTested != null) && (hitTested != mainGrid))
            {
                if (hitTested == imageResizeGrip) 
                {
                    if (FlowDirection == FlowDirection.LeftToRight) return new IntPtr(NativeMethods.HTBOTTOMRIGHT);
                    else return new IntPtr(NativeMethods.HTBOTTOMLEFT);
                }
                if (hitTested == titleBar)
                {
                    if (IsDwmEnabled)
                    {
                        IntPtr result = IntPtr.Zero;
                        NativeMethods.DwmDefWindowProc(handle, msg, wParam, lParam, ref result);
                        if (result != IntPtr.Zero)
                        {
                            return result;
                        }
                    }
                    return new IntPtr(NativeMethods.HTCAPTION);
                }
                if (hitTested == iconImage) return new IntPtr(NativeMethods.HTCLIENT);
                return IntPtr.Zero;
            }
            if (IsDwmEnabled)
            {
                IntPtr result = IntPtr.Zero;
                NativeMethods.DwmDefWindowProc(handle, msg, wParam, lParam, ref result);
                if (result != IntPtr.Zero)
                {
                    return result;
                }
            }

            int uRow = 1;
            int uCol = 1;
            bool fOnResizeBorder = false;

            int borderWidth = (int)BorderWidth;
            int borderHeight = (int)BorderHeight;
                       
            Thickness borderSize = IsDwmEnabled ? sizers : new Thickness(sizers.Left + borderWidth, sizers.Top + borderHeight, sizers.Right + borderWidth, sizers.Bottom + borderHeight);


            if (ptMouse.Y >= -borderSize.Top && ptMouse.Y < 20)
            {
                fOnResizeBorder = (ptMouse.Y < 0);
                uRow = 0;
            }
            else if (ptMouse.Y < mainGrid.ActualHeight + borderSize.Bottom && ptMouse.Y >= mainGrid.ActualHeight)
            {
                uRow = 2;
                fOnResizeBorder = true;
            }

            if (ptMouse.X >= -borderSize.Left && ptMouse.X < 0)
            {
                uCol = 0;
                fOnResizeBorder = true;
            }
            else if (ptMouse.X < mainGrid.ActualWidth + borderSize.Right && ptMouse.X >= mainGrid.ActualWidth)
            {
                uCol = 2;
                fOnResizeBorder = true;
            }

            int[][] hitTests;
            if (FlowDirection == FlowDirection.LeftToRight) hitTests = new int[][]
                {
                    new int[] {fOnResizeBorder ? NativeMethods.HTTOPLEFT:NativeMethods.HTLEFT, fOnResizeBorder ? NativeMethods.HTTOP : NativeMethods.HTCAPTION, fOnResizeBorder ? NativeMethods.HTTOPRIGHT:NativeMethods.HTRIGHT},
                    new int[] {NativeMethods.HTLEFT, NativeMethods.HTNOWHERE, NativeMethods.HTRIGHT},
                    new int[] {NativeMethods.HTBOTTOMLEFT, NativeMethods.HTBOTTOM, NativeMethods.HTBOTTOMRIGHT},
                };
            else
                hitTests = new int[][]
                {
                    new int[] {fOnResizeBorder ? NativeMethods.HTTOPRIGHT:NativeMethods.HTRIGHT, fOnResizeBorder ? NativeMethods.HTTOP : NativeMethods.HTCAPTION, fOnResizeBorder ? NativeMethods.HTTOPLEFT:NativeMethods.HTLEFT},
                    new int[] {NativeMethods.HTRIGHT, NativeMethods.HTNOWHERE, NativeMethods.HTLEFT},
                    new int[] {NativeMethods.HTBOTTOMRIGHT, NativeMethods.HTBOTTOM, NativeMethods.HTBOTTOMLEFT},
                };

            return new IntPtr(hitTests[uRow][uCol]);
        }

        // Initialize wind when DWM is on
        private void DwmInit()
        {
            if (handle == IntPtr.Zero) return;
            
            HwndSource mainWindowSrc = HwndSource.FromHwnd(handle);
            if (mainWindowSrc == null) return;
            if (mainWindowSrc.CompositionTarget.BackgroundColor != Colors.Transparent) mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;
            NativeMethods.MARGINS margins = new NativeMethods.MARGINS(
                        (int)sizers.Left + (int)GlassBorders.Left,
                        (int)sizers.Top + (int)GlassBorders.Top,
                        (int)sizers.Right + (int)GlassBorders.Right,
                        (int)sizers.Bottom + (int)GlassBorders.Bottom);
            NativeMethods.DwmExtendFrameIntoClientArea(handle, margins);
            if (mainGrid == null) return;

            int borderWidth = (int)BorderWidth;
            int borderHeight = (int)BorderHeight;

            if (FlowDirection == FlowDirection.LeftToRight) mainGrid.Margin = new Thickness(sizers.Left,
                                            sizers.Top,
                                            -borderWidth * 2 + sizers.Left,
                                            -SystemParameters.CaptionHeight - 1 - borderHeight * 2 + sizers.Top);
            else mainGrid.Margin = new Thickness(-borderWidth * 2 + sizers.Left, 
                                            sizers.Top,
                                            sizers.Left,
                                            -SystemParameters.CaptionHeight - 1 - borderHeight * 2 + sizers.Top);
        }

        // Initialize wind when DWM is off        
        private void NonDwmInit()
        {
            if(mainGrid==null) return;
            mainGrid.Margin = sizers;
            SetNonDwmRgn(ActualWidth, ActualHeight);
            UpdateLayout();
        }

        // Calc window minmax information
        private void NonDwmGetMinMaxInfo(IntPtr lParam)
        {
            NativeMethods.MINMAXINFO mmi = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MINMAXINFO));
            mmi.ptMinTrackSize.x = (int)MinWidth;
            mmi.ptMinTrackSize.y = (int)MinHeight;

            int borderWidth = (int)BorderWidth;
            int borderHeight = (int)BorderHeight;

            Rect rect = GetCurrentWorkarea();
            mmi.ptMaxPosition.x = (int)SystemParameters.WorkArea.Left - borderWidth;
            mmi.ptMaxPosition.y = (int)SystemParameters.WorkArea.Top - borderHeight;
            mmi.ptMaxSize.x = (int)rect.Width;
            mmi.ptMaxSize.y = (int)rect.Height;

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        // Gets current workspace
        static Rect GetCurrentWorkarea()
        {
            int borderWidth = (int)BorderWidth;
            int borderHeight = (int)BorderHeight;

            Rect rect = SystemParameters.WorkArea;
            return new Rect(rect.Left - borderWidth, rect.Top - borderHeight, rect.Width + borderWidth * 2, rect.Height + borderHeight * 2);
        }

        // Set window region
        private void SetNonDwmRgn(double newWidth, double newHeight)
        {
            if (sizeBorder == null) return;
            
            int borderWidth = (int)BorderWidth;
            int borderHeight = (int)BorderHeight;

            int topSide = borderHeight - 1;
            int bottomSide = borderHeight - 1;
            int leftSide = borderWidth - 1;

            int width = (int)newWidth + 1;
            int height = (int)newHeight + 1;

            IntPtr hRgn = NativeMethods.CreateRoundRectRgn(leftSide, topSide, width - leftSide, height - bottomSide, 15, 15);
            NativeMethods.SetWindowRgn(handle, hRgn, true);
            NativeMethods.DeleteObject(hRgn);                            
        }

        // Update window style
        private void UpdateWindowStyle()
        {            
            long style = NativeMethods.WS_POPUP | NativeMethods.WS_VISIBLE | NativeMethods.WS_CLIPSIBLINGS | NativeMethods.WS_CLIPCHILDREN;
            long exStyle = NativeMethods.WS_EX_WINDOWEDGE | NativeMethods.WS_EX_APPWINDOW;
            if (IsDwmEnabled)
            {
                style |= NativeMethods.WS_CAPTION;
                style |= NativeMethods.WS_THICKFRAME;
            }
            else
            {
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                    style |= NativeMethods.WS_THICKFRAME;
                else
                    style |= NativeMethods.WS_BORDER;
            }

            style |= NativeMethods.WS_SYSMENU | NativeMethods.WS_MINIMIZEBOX | NativeMethods.WS_MAXIMIZEBOX;            

            // Устанавливаем стиль окна
            NativeMethods.SetWindowLongPtr(handle, NativeMethods.GWL_STYLE, new IntPtr(style));
            //NativeMethods.SetWindowLongPtr(handle, NativeMethods.GWL_EXSTYLE, new IntPtr(exStyle));            
            NativeMethods.SetWindowPos(handle, new IntPtr(NativeMethods.HWND_TOP), 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);            
            UpdateLayout();
            //Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(delegate{NativeMethods.SetForegroundWindow(handle);}));            
        }

        #endregion
    }
}
