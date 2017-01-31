// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;
    using System.Windows.Threading;
    using ControlzEx.Behaviors;

    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;

    //using WindowChrome = System.Windows.Shell.WindowChrome;
    using WindowChrome = ControlzEx.Microsoft.Windows.Shell.WindowChrome;

    /// <summary>
    /// Represents basic window for ribbon
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1049")]
    [TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class RibbonWindow : Window
    {
        // ReSharper disable InconsistentNaming
        private const string PART_Icon = "PART_Icon";        
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_WindowCommands = "PART_WindowCommands";
        // ReSharper restore InconsistentNaming

        private FrameworkElement iconImage;

    #region Properties

    #region TitelBar

        /// <summary>
        /// Gets or sets the height which is used to render the window title.
        /// </summary>
        public UIElement TitleBarHeader
        {
          get { return (UIElement)this.GetValue(TitleBarHeaderProperty); }
          set { this.SetValue(TitleBarHeaderProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBarHeader"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarHeaderProperty =
            DependencyProperty.Register(nameof(TitleBarHeader), typeof(UIElement), typeof(RibbonWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        public RibbonTitleBar TitleBar
        {
            get { return (RibbonTitleBar)this.GetValue(TitleBarProperty); }
            private set { this.SetValue(titleBarPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey titleBarPropertyKey = DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(RibbonWindow), new PropertyMetadata());

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = titleBarPropertyKey.DependencyProperty;

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
        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.Register(nameof(TitleBarHeight), typeof(double), typeof(RibbonWindow), new PropertyMetadata(DoubleBoxes.Zero));

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
        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register(nameof(TitleForeground), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata());

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

        /// <summary>
        /// Gets or sets resize border thickness
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)this.GetValue(ResizeBorderThicknessProperty); }
            set { this.SetValue(ResizeBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeBorderTickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty =
            DependencyProperty.Register(nameof(ResizeBorderThickness), typeof(Thickness), typeof(RibbonWindow), new PropertyMetadata(new Thickness(8D)));

        /// <summary>
        /// Gets or sets glass border thickness
        /// </summary>
        public Thickness GlassFrameThickness
        {
            get { return (Thickness)this.GetValue(GlassFrameThicknessProperty); }
            set { this.SetValue(GlassFrameThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GlassFrameThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GlassFrameThicknessProperty =
            DependencyProperty.Register(nameof(GlassFrameThickness), typeof(Thickness), typeof(RibbonWindow), new PropertyMetadata(new Thickness()));

        /// <summary>
        /// Gets or sets corner radius 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(RibbonWindow), new PropertyMetadata(new CornerRadius()));

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
        public static readonly DependencyProperty IsIconVisibleProperty = DependencyProperty.Register(nameof(IsIconVisible), typeof(bool), typeof(RibbonWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

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
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(nameof(IsCollapsed), typeof(bool),
            typeof(RibbonWindow), new PropertyMetadata(BooleanBoxes.FalseBox));

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
        public static readonly DependencyProperty IsAutomaticCollapseEnabledProperty =
            DependencyProperty.Register(nameof(IsAutomaticCollapseEnabled), typeof(bool), typeof(RibbonWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(typeof(RibbonWindow)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonWindow()
        {
            this.SizeChanged += this.OnSizeChanged;

            // WindowChrome initialization has to occur in constructor. Otherwise the load event is fired early.
            this.InitializeWindowChromeBehavior();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Initializes the WindowChromeBehavior which is needed to render the custom WindowChrome
        /// </summary>
        private void InitializeWindowChromeBehavior()
        {
            var behavior = new WindowChromeBehavior();
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.CaptionHeightProperty, new Binding { Path = new PropertyPath(TitleBarHeightProperty), Source = this });
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(ResizeBorderThicknessProperty), Source = this });
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.CornerRadiusProperty, new Binding { Path = new PropertyPath(CornerRadiusProperty), Source = this });
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.GlassFrameThicknessProperty, new Binding { Path = new PropertyPath(GlassFrameThicknessProperty), Source = this });
            behavior.UseAeroCaptionButtons = false;
            Interaction.GetBehaviors(this).Add(behavior);
        }

        #endregion

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

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.TitleBar = this.GetTemplateChild("PART_RibbonTitleBar") as RibbonTitleBar;
            if(TitleBarHeader!=null)
            {
              TitleBar.Header = TitleBarHeader;
            }

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

            this.RunInDispatcherAsync(() =>
                                      {
                                          this.TitleBar?.InvalidateMeasure();
                                          this.TitleBar?.InvalidateArrange();
                                          this.TitleBar?.UpdateLayout();
                                      }, DispatcherPriority.Background);
        }

        private void HandleIconMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    if (e.ClickCount == 1)
                    {
                        e.Handled = true;

                        WindowSteeringHelper.ShowSystemMenuPhysicalCoordinates(this, e, this.PointToScreen(new Point(0, this.TitleBarHeight)));
                    }
                    else if (e.ClickCount == 2)
                    {
                        e.Handled = true;

                        this.Close();
                    }
                    break;

                case MouseButton.Right:
                    e.Handled = true;

                    this.RunInDispatcherAsync(() =>
                                              {
                                                  WindowSteeringHelper.ShowSystemMenuPhysicalCoordinates(this, e);
                                              });
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