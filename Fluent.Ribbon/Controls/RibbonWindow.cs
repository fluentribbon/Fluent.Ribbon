#pragma warning disable WPF0015

// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ControlzEx;
using ControlzEx.Behaviors;
using Fluent.Extensions;
using Fluent.Helpers;
using Fluent.Internal.KnownBoxes;
using Microsoft.Xaml.Behaviors;
using WindowChrome = ControlzEx.WindowChrome;

/// <summary>
/// Represents basic window for ribbon
/// </summary>
[TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
[TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
[TemplatePart(Name = PART_RibbonTitleBar, Type = typeof(RibbonTitleBar))]
[TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
public class RibbonWindow : WindowChromeWindow, IRibbonWindow
{
    // ReSharper disable InconsistentNaming
#pragma warning disable SA1310 // Field names must not contain underscore
    private const string PART_Icon = "PART_Icon";
    private const string PART_ContentPresenter = "PART_ContentPresenter";
    private const string PART_RibbonTitleBar = "PART_RibbonTitleBar";
    private const string PART_WindowCommands = "PART_WindowCommands";
#pragma warning restore SA1310 // Field names must not contain underscore
    // ReSharper restore InconsistentNaming

    private FrameworkElement? iconImage;

    #region Properties

    #region TitelBar

    /// <inheritdoc />
    public RibbonTitleBar? TitleBar
    {
        get { return (RibbonTitleBar?)this.GetValue(TitleBarProperty); }
        private set { this.SetValue(TitleBarPropertyKey, value); }
    }

    // ReSharper disable once InconsistentNaming
    private static readonly DependencyPropertyKey TitleBarPropertyKey = DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(RibbonWindow), new PropertyMetadata());

    /// <summary>Identifies the <see cref="TitleBar"/> dependency property.</summary>
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

    /// <summary>Identifies the <see cref="TitleBarHeight"/> dependency property.</summary>
    public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register(nameof(TitleBarHeight), typeof(double), typeof(RibbonWindow), new PropertyMetadata(DoubleBoxes.Zero));

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> which is used to render the window title.
    /// </summary>
    public Brush? TitleForeground
    {
        get { return (Brush?)this.GetValue(TitleForegroundProperty); }
        set { this.SetValue(TitleForegroundProperty, value); }
    }

    /// <summary>Identifies the <see cref="TitleForeground"/> dependency property.</summary>
    public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register(nameof(TitleForeground), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata());

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> which is used to render the window title background.
    /// </summary>
    public Brush? TitleBackground
    {
        get { return (Brush?)this.GetValue(TitleBackgroundProperty); }
        set { this.SetValue(TitleBackgroundProperty, value); }
    }

    /// <summary>Identifies the <see cref="TitleBackground"/> dependency property.</summary>
    public static readonly DependencyProperty TitleBackgroundProperty = DependencyProperty.Register(nameof(TitleBackground), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata());

    /// <summary>Identifies the <see cref="WindowCommands"/> dependency property.</summary>
    public static readonly DependencyProperty WindowCommandsProperty = DependencyProperty.Register(nameof(WindowCommands), typeof(WindowCommands), typeof(RibbonWindow), new PropertyMetadata());

    /// <summary>
    /// Gets or sets the window commands
    /// </summary>
    public WindowCommands? WindowCommands
    {
        get { return (WindowCommands?)this.GetValue(WindowCommandsProperty); }
        set { this.SetValue(WindowCommandsProperty, value); }
    }

    #region Window-Border-Properties

    /// <summary>Identifies the <see cref="NonActiveBorderBrush"/> dependency property.</summary>
    public static readonly DependencyProperty NonActiveBorderBrushProperty = DependencyProperty.Register(nameof(NonActiveBorderBrush), typeof(Brush), typeof(RibbonWindow), new PropertyMetadata(default(Brush)));

    /// <summary>
    /// Gets or sets a brush which is used as the border brush when the window is not active.
    /// </summary>
    public Brush? NonActiveBorderBrush
    {
        get { return (Brush?)this.GetValue(NonActiveBorderBrushProperty); }
        set { this.SetValue(NonActiveBorderBrushProperty, value); }
    }

    #endregion

    /// <summary>
    /// Gets or sets whether icon is visible.
    /// </summary>
    public bool IsIconVisible
    {
        get { return (bool)this.GetValue(IsIconVisibleProperty); }
        set { this.SetValue(IsIconVisibleProperty, BooleanBoxes.Box(value)); }
    }

    /// <summary>Identifies the <see cref="IsIconVisible"/> dependency property.</summary>
    public static readonly DependencyProperty IsIconVisibleProperty = DependencyProperty.Register(nameof(IsIconVisible), typeof(bool), typeof(RibbonWindow), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets the vertical alignment of the icon.
    /// </summary>
    public VerticalAlignment VerticalIconAlignment
    {
        get { return (VerticalAlignment)this.GetValue(VerticalIconAlignmentProperty); }
        set { this.SetValue(VerticalIconAlignmentProperty, value); }
    }

    /// <summary>Identifies the <see cref="VerticalIconAlignment"/> dependency property.</summary>
    public static readonly DependencyProperty VerticalIconAlignmentProperty = DependencyProperty.Register(nameof(VerticalIconAlignment), typeof(VerticalAlignment), typeof(RibbonWindow), new PropertyMetadata(VerticalAlignment.Top));

    // todo check if IsCollapsed and IsAutomaticCollapseEnabled should be reduced to one shared property for RibbonWindow and Ribbon

    /// <summary>
    /// Gets whether window is collapsed
    /// </summary>
    public bool IsCollapsed
    {
        get { return (bool)this.GetValue(IsCollapsedProperty); }
        set { this.SetValue(IsCollapsedProperty, BooleanBoxes.Box(value)); }
    }

    /// <summary>Identifies the <see cref="IsCollapsed"/> dependency property.</summary>
    public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(RibbonWindow), new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Defines if the Ribbon should automatically set <see cref="IsCollapsed"/> when the width or height of the owner window drop under <see cref="Ribbon.MinimalVisibleWidth"/> or <see cref="Ribbon.MinimalVisibleHeight"/>
    /// </summary>
    public bool IsAutomaticCollapseEnabled
    {
        get { return (bool)this.GetValue(IsAutomaticCollapseEnabledProperty); }
        set { this.SetValue(IsAutomaticCollapseEnabledProperty, BooleanBoxes.Box(value)); }
    }

    /// <summary>Identifies the <see cref="IsAutomaticCollapseEnabled"/> dependency property.</summary>
    public static readonly DependencyProperty IsAutomaticCollapseEnabledProperty = DependencyProperty.Register(nameof(IsAutomaticCollapseEnabled), typeof(bool), typeof(RibbonWindow), new PropertyMetadata(BooleanBoxes.TrueBox, OnIsAutomaticCollapseEnabledChanged));

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
        this.Loaded += this.OnLoaded;
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
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.GlowColorProperty, new Binding { Path = new PropertyPath(GlowColorProperty), Source = this });

        Interaction.GetBehaviors(this).Add(behavior);
    }

    /// <summary>
    /// Initializes the GlowWindowBehavior which is needed to render the custom resize windows around the current window.
    /// </summary>
    private void InitializeGlowWindowBehavior()
    {
        var behavior = new GlowWindowBehavior();
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.GlowDepthProperty, new Binding { Path = new PropertyPath(GlowDepthProperty), Source = this });
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.GlowColorProperty, new Binding { Path = new PropertyPath(GlowColorProperty), Source = this });
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.NonActiveGlowColorProperty, new Binding { Path = new PropertyPath(NonActiveGlowColorProperty), Source = this });

        Interaction.GetBehaviors(this).Add(behavior);
    }

    #endregion

    // Size change to collapse ribbon
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        this.MaintainIsCollapsed();

        if (this.iconImage is not null
            && this.ActualWidth <= 140D + RibbonProperties.GetLastVisibleWidth(this.iconImage).GetZeroIfInfinityOrNaN() + RibbonProperties.GetLastVisibleWidth(this.WindowCommands?.ItemsControl).GetZeroIfInfinityOrNaN())
        {
            this.SetCurrentValue(IsIconVisibleProperty, BooleanBoxes.FalseBox);
            this.TitleBar?.SetCurrentValue(VisibilityProperty, VisibilityBoxes.Collapsed);
            this.WindowCommands?.SetCurrentValue(WindowCommands.ItemsPanelVisibilityProperty, VisibilityBoxes.Collapsed);
        }
        else
        {
            this.InvalidateProperty(IsIconVisibleProperty);
            this.iconImage?.SetCurrentValue(RibbonProperties.LastVisibleWidthProperty, this.iconImage.ActualWidth);

            this.TitleBar?.InvalidateProperty(VisibilityProperty);

            this.WindowCommands?.InvalidateProperty(WindowCommands.ItemsPanelVisibilityProperty);
            this.WindowCommands?.ItemsControl?.SetCurrentValue(RibbonProperties.LastVisibleWidthProperty, this.WindowCommands.ItemsControl.ActualWidth);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (this.SizeToContent == SizeToContent.Manual)
        {
            return;
        }

        this.RunInDispatcherAsync(() =>
        {
            if (this.TitleBar is null)
            {
                return;
            }

            // Fix for #454 while also keeping #473
            var availableSize = new Size(this.TitleBar.ActualWidth, this.TitleBar.ActualHeight);
            this.TitleBar.Measure(availableSize);
            this.TitleBar.ScheduleForceMeasureAndArrange();
        }, DispatcherPriority.ApplicationIdle);
    }

    private static void OnIsAutomaticCollapseEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RibbonWindow)d).MaintainIsCollapsed();
    }

    private void MaintainIsCollapsed()
    {
        if (this.IsAutomaticCollapseEnabled == false)
        {
            this.ClearValue(IsCollapsedProperty);
            return;
        }

        if (this.ActualWidth < Ribbon.MinimalVisibleWidth
            || this.ActualHeight < Ribbon.MinimalVisibleHeight)
        {
            this.SetCurrentValue(IsCollapsedProperty, BooleanBoxes.TrueBox);
        }
        else
        {
            this.SetCurrentValue(IsCollapsedProperty, BooleanBoxes.FalseBox);
        }
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.TitleBar = this.GetTemplateChild(PART_RibbonTitleBar) as RibbonTitleBar;

        if (this.iconImage is not null)
        {
            this.iconImage.MouseDown -= this.HandleIconMouseDown;
        }

        if (this.WindowCommands is null)
        {
            this.WindowCommands = new WindowCommands();
        }

        this.iconImage = this.GetPart<FrameworkElement>(PART_Icon);

        if (this.iconImage is not null)
        {
            this.iconImage.MouseDown += this.HandleIconMouseDown;
        }

        this.GetPart<UIElement>(PART_Icon)?.SetCurrentValue(WindowChrome.IsHitTestVisibleInChromeProperty, BooleanBoxes.TrueBox);
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

        this.TitleBar?.ScheduleForceMeasureAndArrange();
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
                    ControlzEx.SystemCommands.CloseWindow(this);
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
    internal T? GetPart<T>(string name)
        where T : DependencyObject
    {
        return this.GetTemplateChild(name) as T;
    }
}