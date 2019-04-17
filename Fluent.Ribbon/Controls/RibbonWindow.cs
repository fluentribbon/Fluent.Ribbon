// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using ControlzEx.Behaviors;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;
    using Microsoft.Xaml.Behaviors;
    using WindowChrome = ControlzEx.Windows.Shell.WindowChrome;

    /// <summary>
    /// Represents basic window for ribbon
    /// </summary>
    [TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_RibbonTitleBar, Type = typeof(RibbonTitleBar))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class RibbonWindow : Window, IRibbonWindow
    {
        // ReSharper disable InconsistentNaming
#pragma warning disable SA1310 // Field names must not contain underscore
        private const string PART_Icon = "PART_Icon";
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_RibbonTitleBar = "PART_RibbonTitleBar";
        private const string PART_WindowCommands = "PART_WindowCommands";
#pragma warning restore SA1310 // Field names must not contain underscore
        // ReSharper restore InconsistentNaming

        private FrameworkElement iconImage;

        #region Properties

        #region TitelBar

        /// <inheritdoc />
        public RibbonTitleBar TitleBar
        {
            get { return (RibbonTitleBar)this.GetValue(TitleBarProperty); }
            private set { this.SetValue(TitleBarPropertyKey, value); }
        }

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey TitleBarPropertyKey = DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(RibbonWindow), new PropertyMetadata());

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = TitleBarPropertyKey.DependencyProperty;

        #endregion

        /// <summary>
        /// Gets or sets the height which is used to render the window title.
        /// </summary>
        public double TitleBarHeight
        {
            get { return (double)this.GetValue(TitleBarHeightProperty); }
            set { this.SetValue(TitleBarHeightProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBarHeight"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register(nameof(TitleBarHeight), typeof(double), typeof(RibbonWindow), new PropertyMetadata(DoubleBoxes.Zero));

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> which is used to render the window title.
        /// </summary>
        public Brush TitleForeground
        {
            get { return (Brush)this.GetValue(TitleForegroundProperty); }
            set { this.SetValue(TitleForegroundProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleForeground"/>.
        /// </summary>
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register(nameof(TitleForeground), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata());

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> which is used to render the window title background.
        /// </summary>
        public Brush TitleBackground
        {
            get { return (Brush)this.GetValue(TitleBackgroundProperty); }
            set { this.SetValue(TitleBackgroundProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBackground"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBackgroundProperty = DependencyProperty.Register(nameof(TitleBackground), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata());

        /// <summary>
        /// Using a DependencyProperty as the backing store for WindowCommands.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WindowCommandsProperty = DependencyProperty.Register(nameof(WindowCommands), typeof(WindowCommands), typeof(RibbonWindow), new PropertyMetadata());

        /// <summary>
        /// Gets or sets the window commands
        /// </summary>
        public WindowCommands WindowCommands
        {
            get { return (WindowCommands)this.GetValue(WindowCommandsProperty); }
            set { this.SetValue(WindowCommandsProperty, value); }
        }

        #region Window-Border-Properties

        /// <summary>
        /// Gets or sets resize border thickness.
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)this.GetValue(ResizeBorderThicknessProperty); }
            set { this.SetValue(ResizeBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeBorderTickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty = DependencyProperty.Register(nameof(ResizeBorderThickness), typeof(Thickness), typeof(RibbonWindow), new PropertyMetadata(new Thickness(6D)));

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="GlowBrush"/>.
        /// </summary>
        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register(nameof(GlowBrush), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets or sets a brush which is used as the glow when the window is active.
        /// </summary>
        public Brush GlowBrush
        {
            get { return (Brush)this.GetValue(GlowBrushProperty); }
            set { this.SetValue(GlowBrushProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="NonActiveGlowBrush"/>.
        /// </summary>
        public static readonly DependencyProperty NonActiveGlowBrushProperty = DependencyProperty.Register(nameof(NonActiveGlowBrush), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets or sets a brush which is used as the glow when the window is not active.
        /// </summary>
        public Brush NonActiveGlowBrush
        {
            get { return (Brush)this.GetValue(NonActiveGlowBrushProperty); }
            set { this.SetValue(NonActiveGlowBrushProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="NonActiveBorderBrush"/>.
        /// </summary>
        public static readonly DependencyProperty NonActiveBorderBrushProperty = DependencyProperty.Register(nameof(NonActiveBorderBrush), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets or sets a brush which is used as the border brush when the window is not active.
        /// </summary>
        public Brush NonActiveBorderBrush
        {
            get { return (Brush)this.GetValue(NonActiveBorderBrushProperty); }
            set { this.SetValue(NonActiveBorderBrushProperty, value); }
        }

        #endregion

        /// <summary>
        /// Gets or sets whether icon is visible
        /// </summary>
        public bool IsIconVisible
        {
            get { return (bool)this.GetValue(IsIconVisibleProperty); }
            set { this.SetValue(IsIconVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether icon is visible
        /// </summary>
        public static readonly DependencyProperty IsIconVisibleProperty = DependencyProperty.Register(nameof(IsIconVisible), typeof(bool), typeof(RibbonWindow), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        // todo check if IsCollapsed and IsAutomaticCollapseEnabled should be reduced to one shared property for RibbonWindow and Ribbon

        /// <summary>
        /// Gets whether window is collapsed
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)this.GetValue(IsCollapsedProperty); }
            set { this.SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(RibbonWindow), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Defines if the Ribbon should automatically set <see cref="IsCollapsed"/> when the width or height of the owner window drop under <see cref="Ribbon.MinimalVisibleWidth"/> or <see cref="Ribbon.MinimalVisibleHeight"/>
        /// </summary>
        public bool IsAutomaticCollapseEnabled
        {
            get { return (bool)this.GetValue(IsAutomaticCollapseEnabledProperty); }
            set { this.SetValue(IsAutomaticCollapseEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAutomaticCollapseEnabledProperty = DependencyProperty.Register(nameof(IsAutomaticCollapseEnabled), typeof(bool), typeof(RibbonWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Defines if the taskbar should be ignored and hidden while the window is maximized.
        /// </summary>
        public bool IgnoreTaskbarOnMaximize
        {
            get { return (bool)this.GetValue(IgnoreTaskbarOnMaximizeProperty); }
            set { this.SetValue(IgnoreTaskbarOnMaximizeProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IgnoreTaskbarOnMaximize"/>.
        /// </summary>
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register(nameof(IgnoreTaskbarOnMaximize), typeof(bool), typeof(RibbonWindow), new PropertyMetadata(default(bool)));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(typeof(RibbonWindow)));

            BorderThicknessProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(new Thickness(1)));
            WindowStyleProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(WindowStyle.None));

            AllowsTransparencyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonWindow()
        {
            this.SizeChanged += this.OnSizeChanged;
            this.Loaded += this.OnLoaded;
            this.ContentRendered += this.OnContentRendered;

            // WindowChromeBehavior initialization has to occur in constructor. Otherwise the load event is fired early and performance of the window is degraded.
            this.InitializeWindowChromeBehavior();
        }

        #endregion

        #region Behaviors

        /// <summary>
        /// Initializes the WindowChromeBehavior which is needed to render the custom WindowChrome.
        /// </summary>
        private void InitializeWindowChromeBehavior()
        {
            var behavior = new WindowChromeBehavior();
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(ResizeBorderThicknessProperty), Source = this });
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.IgnoreTaskbarOnMaximizeProperty, new Binding { Path = new PropertyPath(IgnoreTaskbarOnMaximizeProperty), Source = this });
            BindingOperations.SetBinding(behavior, GlowWindowBehavior.GlowBrushProperty, new Binding { Path = new PropertyPath(GlowBrushProperty), Source = this });

            Interaction.GetBehaviors(this).Add(behavior);
        }

        /// <summary>
        /// Initializes the GlowWindowBehavior which is needed to render the custom resize windows around the current window.
        /// </summary>
        private void InitializeGlowWindowBehavior()
        {
            var behavior = new GlowWindowBehavior();
            BindingOperations.SetBinding(behavior, GlowWindowBehavior.ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(ResizeBorderThicknessProperty), Source = this });
            BindingOperations.SetBinding(behavior, GlowWindowBehavior.GlowBrushProperty, new Binding { Path = new PropertyPath(GlowBrushProperty), Source = this });
            BindingOperations.SetBinding(behavior, GlowWindowBehavior.NonActiveGlowBrushProperty, new Binding { Path = new PropertyPath(NonActiveGlowBrushProperty), Source = this });

            Interaction.GetBehaviors(this).Add(behavior);
        }

        #endregion

        // Size change to collapse ribbon
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MaintainIsCollapsed();
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            this.ContentRendered -= this.OnContentRendered;

            this.InitializeGlowWindowBehavior();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.SizeToContent == SizeToContent.Manual)
            {
                return;
            }

            this.RunInDispatcherAsync(() =>
                                      {
                                          // Fix for #454 while also keeping #473
                                          var availableSize = new Size(this.TitleBar.ActualWidth, this.TitleBar.ActualHeight);
                                          this.TitleBar.Measure(availableSize);
                                          this.TitleBar.ForceMeasureAndArrange();
                                      }, DispatcherPriority.ApplicationIdle);
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

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.TitleBar = this.GetTemplateChild(PART_RibbonTitleBar) as RibbonTitleBar;

            if (this.iconImage != null)
            {
                this.iconImage.MouseDown -= this.HandleIconMouseDown;
            }

            if (this.WindowCommands == null)
            {
                this.WindowCommands = new WindowCommands();
            }

            this.iconImage = this.GetPart<FrameworkElement>(PART_Icon);

            if (this.iconImage != null)
            {
                this.iconImage.MouseDown += this.HandleIconMouseDown;
            }

            this.GetPart<UIElement>(PART_Icon)?.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            this.GetPart<UIElement>(PART_WindowCommands)?.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
        }

        /// <inheritdoc />
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            // todo: remove fix if we update to ControlzEx 4.0
            if (this.WindowState == WindowState.Maximized
                && this.SizeToContent != SizeToContent.Manual)
            {
                this.SizeToContent = SizeToContent.Manual;
            }

            this.RunInDispatcherAsync(() => this.TitleBar?.ForceMeasureAndArrange(), DispatcherPriority.Background);
        }

        private void HandleIconMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    if (e.ClickCount == 1)
                    {
                        e.Handled = true;

                        WindowSteeringHelper.ShowSystemMenu(this, this.PointToScreen(new Point(0, this.TitleBarHeight)));
                    }
                    else if (e.ClickCount == 2)
                    {
                        e.Handled = true;

#pragma warning disable 618
                        ControlzEx.Windows.Shell.SystemCommands.CloseWindow(this);
#pragma warning restore 618
                    }

                    break;

                case MouseButton.Right:
                    e.Handled = true;

                    WindowSteeringHelper.ShowSystemMenu(this, e);
                    break;
            }
        }

        /// <summary>
        /// Gets the template child with the given name.
        /// </summary>
        /// <typeparam name="T">The interface type inheirted from DependencyObject.</typeparam>
        /// <param name="name">The name of the template child.</param>
        internal T GetPart<T>(string name)
            where T : DependencyObject
        {
            return this.GetTemplateChild(name) as T;
        }
    }
}