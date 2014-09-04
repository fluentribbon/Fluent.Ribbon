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
    using System.Windows.Input;
#if NET35 || NET40
    using Microsoft.Windows.Shell;
#else
    using System.Windows.Shell;
#endif

    /// <summary>
    /// Represents basic window for ribbon
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1049")]
    public class RibbonWindow : Window
    {
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

        public static readonly DependencyProperty DontUseDwmProperty =
            DependencyProperty.Register("DontUseDwm", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(false));

        /// <summary>
        ///  Gets or sets whether DWM will be used.
        /// </summary>
        public bool DontUseDwm
        {
            get { return (bool)GetValue(DontUseDwmProperty); }
            set { SetValue(DontUseDwmProperty, value); }
        }

        public bool? UseWindowChrome
        {
            get { return (bool?)GetValue(UseWindowChromeProperty); }
            set { SetValue(UseWindowChromeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseWindowChrome.  This enables animation, styling, binding, etc...
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

            this.UpdateWindowChrome();
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

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var buttonsPanel = this.GetTemplateChild("PART_ButtonsPanel") as FrameworkElement;

            if (buttonsPanel != null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(buttonsPanel, true);
            }

            this.UpdateWindowChrome();
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
                    GlassFrameThickness = this.CanUseDwm() ? this.GlassBorderThickness : default(Thickness),
                    ResizeBorderThickness = this.ResizeBorderThickness,
#if NET45
                    NonClientFrameEdges = NonClientFrameEdges.Bottom,
                    UseAeroCaptionButtons = this.CanUseDwm()
#endif
                };
            }

            WindowChrome.SetWindowChrome(this, windowChrome);
        }

        private bool CanUseDwm()
        {
            return NativeMethods.IsDwmEnabled()
                && !this.DontUseDwm;
        }
    }
}