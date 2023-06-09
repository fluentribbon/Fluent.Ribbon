﻿// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Fluent.Extensions;
using Fluent.Helpers;
using Fluent.Internal;
using Fluent.Internal.KnownBoxes;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

/// <summary>
/// Represent base class for Fluent controls
/// </summary>
public abstract class RibbonControl : Control, ICommandSource, IQuickAccessItemProvider, IRibbonControl
{
    #region KeyTip

    /// <inheritdoc />
    public string? KeyTip
    {
        get { return (string?)this.GetValue(KeyTipProperty); }
        set { this.SetValue(KeyTipProperty, value); }
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for Keys.
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(RibbonControl));

    #endregion

    #region Header

    /// <inheritdoc />
    public object? Header
    {
        get { return this.GetValue(HeaderProperty); }
        set { this.SetValue(HeaderProperty, value); }
    }

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(RibbonControl), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    /// <inheritdoc />
    public DataTemplate? HeaderTemplate
    {
        get { return (DataTemplate?)this.GetValue(HeaderTemplateProperty); }
        set { this.SetValue(HeaderTemplateProperty, value); }
    }

    /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(RibbonControl), new PropertyMetadata());

    /// <inheritdoc />
    public DataTemplateSelector? HeaderTemplateSelector
    {
        get { return (DataTemplateSelector?)this.GetValue(HeaderTemplateSelectorProperty); }
        set { this.SetValue(HeaderTemplateSelectorProperty, value); }
    }

    /// <summary>Identifies the <see cref="HeaderTemplateSelector"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty =
        DependencyProperty.Register(nameof(HeaderTemplateSelector), typeof(DataTemplateSelector), typeof(RibbonControl), new PropertyMetadata());

    #endregion

    #region Icon

    /// <inheritdoc />
    public object? Icon
    {
        get { return this.GetValue(IconProperty); }
        set { this.SetValue(IconProperty, value); }
    }

    /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(RibbonControl), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    #region Command

    private bool currentCanExecute = true;

    /// <inheritdoc />
    [Category("Action")]
    [Localizability(LocalizationCategory.NeverLocalize)]
    [Bindable(true)]
    public ICommand Command
    {
        get
        {
            return (ICommand)this.GetValue(CommandProperty);
        }

        set
        {
            this.SetValue(CommandProperty, value);
        }
    }

    /// <inheritdoc />
    [Bindable(true)]
    [Localizability(LocalizationCategory.NeverLocalize)]
    [Category("Action")]
    public object CommandParameter
    {
        get
        {
            return this.GetValue(CommandParameterProperty);
        }

        set
        {
            this.SetValue(CommandParameterProperty, value);
        }
    }

    /// <inheritdoc />
    [Bindable(true)]
    [Category("Action")]
    public IInputElement CommandTarget
    {
        get
        {
            return (IInputElement)this.GetValue(CommandTargetProperty);
        }

        set
        {
            this.SetValue(CommandTargetProperty, value);
        }
    }

    /// <summary>Identifies the <see cref="CommandParameter"/> dependency property.</summary>
    public static readonly DependencyProperty CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(RibbonControl), new PropertyMetadata());

    /// <summary>Identifies the <see cref="Command"/> dependency property.</summary>
    public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(RibbonControl), new PropertyMetadata(OnCommandChanged));

    /// <summary>Identifies the <see cref="CommandTarget"/> dependency property.</summary>
    public static readonly DependencyProperty CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(RibbonControl), new PropertyMetadata());

    /// <summary>
    /// Handles Command changed
    /// </summary>
    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RibbonControl control)
        {
            return;
        }

        if (e.OldValue is ICommand oldCommand)
        {
            oldCommand.CanExecuteChanged -= control.OnCommandCanExecuteChanged;
        }

        if (e.NewValue is ICommand newCommand)
        {
            newCommand.CanExecuteChanged += control.OnCommandCanExecuteChanged;

            if (e.NewValue is RoutedUICommand routedUiCommand
                && control.Header is null)
            {
                control.Header = routedUiCommand.Text;
            }
        }

        control.UpdateCanExecute();
    }

    /// <summary>
    /// Handles Command CanExecute changed
    /// </summary>
    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        this.UpdateCanExecute();
    }

    private void UpdateCanExecute()
    {
        var canExecute = this.Command is not null
                         && this.CanExecuteCommand();

        if (this.currentCanExecute != canExecute)
        {
            this.currentCanExecute = canExecute;
            this.CoerceValue(IsEnabledProperty);
        }
    }

    #endregion

    #region IsEnabled

    /// <inheritdoc />
    protected override bool IsEnabledCore => base.IsEnabledCore && (this.currentCanExecute || this.Command is null);

    #endregion

    #region Size

    /// <inheritdoc />
    public RibbonControlSize Size
    {
        get { return (RibbonControlSize)this.GetValue(SizeProperty); }
        set { this.SetValue(SizeProperty, value); }
    }

    /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
    public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(RibbonControl));

    #endregion

    #region SizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SizeDefinition
    {
        get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
        set { this.SetValue(SizeDefinitionProperty, value); }
    }

    /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(RibbonControl));

    #endregion

    #region CornerRadius

    /// <summary>
    /// Gets and sets the Corner radius.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
        set { this.SetValue(CornerRadiusProperty, value); }
    }

    /// <summary>Identifies the <see cref="CornerRadius"/> dependency property.</summary>
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(RibbonControl));

    #endregion

    #region Constructors

    /// <summary>
    /// Static constructor
    /// </summary>
    static RibbonControl()
    {
        var type = typeof(RibbonControl);
        ContextMenuService.Attach(type);
        ToolTipService.Attach(type);
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    protected RibbonControl()
    {
        ContextMenuService.Coerce(this);
    }

    #endregion

    #region QuickAccess

    /// <inheritdoc />
    public abstract FrameworkElement CreateQuickAccessItem();

    /// <summary>
    /// Binds default properties of control to quick access element
    /// </summary>
    /// <param name="source">Source item</param>
    /// <param name="element">Toolbar item</param>
    public static void BindQuickAccessItem(FrameworkElement source, FrameworkElement element)
    {
        Bind(source, element, nameof(source.DataContext), DataContextProperty, BindingMode.OneWay);

        if (source is ICommandSource)
        {
            if (source is MenuItem)
            {
                Bind(source, element, nameof(ICommandSource.CommandParameter), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);
                Bind(source, element, nameof(ICommandSource.CommandTarget), System.Windows.Controls.MenuItem.CommandTargetProperty, BindingMode.OneWay);
                Bind(source, element, nameof(ICommandSource.Command), System.Windows.Controls.MenuItem.CommandProperty, BindingMode.OneWay);
            }
            else
            {
                Bind(source, element, nameof(ICommandSource.CommandParameter), ButtonBase.CommandParameterProperty, BindingMode.OneWay);
                Bind(source, element, nameof(ICommandSource.CommandTarget), ButtonBase.CommandTargetProperty, BindingMode.OneWay);
                Bind(source, element, nameof(ICommandSource.Command), ButtonBase.CommandProperty, BindingMode.OneWay);
            }
        }

        Bind(source, element, nameof(FontFamily), FontFamilyProperty, BindingMode.OneWay);
        Bind(source, element, nameof(FontSize), FontSizeProperty, BindingMode.OneWay);
        Bind(source, element, nameof(FontStretch), FontStretchProperty, BindingMode.OneWay);
        Bind(source, element, nameof(FontStyle), FontStyleProperty, BindingMode.OneWay);
        Bind(source, element, nameof(FontWeight), FontWeightProperty, BindingMode.OneWay);

        Bind(source, element, nameof(Foreground), ForegroundProperty, BindingMode.OneWay);
        Bind(source, element, nameof(IsEnabled), IsEnabledProperty, BindingMode.OneWay);
        Bind(source, element, nameof(Opacity), OpacityProperty, BindingMode.OneWay);
        Bind(source, element, nameof(SnapsToDevicePixels), SnapsToDevicePixelsProperty, BindingMode.OneWay);

        Bind(source, element, new PropertyPath(FocusManager.IsFocusScopeProperty), FocusManager.IsFocusScopeProperty, BindingMode.OneWay);

        Bind(source, element, new PropertyPath(InputControlProperties.InputMinWidthProperty), InputControlProperties.InputMinWidthProperty, BindingMode.OneWay);
        Bind(source, element, new PropertyPath(InputControlProperties.InputWidthProperty), InputControlProperties.InputWidthProperty, BindingMode.OneWay);
        Bind(source, element, new PropertyPath(InputControlProperties.InputHeightProperty), InputControlProperties.InputHeightProperty, BindingMode.OneWay);

        if (source is IHeaderedControl headeredControl)
        {
            if (headeredControl is HeaderedItemsControl)
            {
                Bind(source, element, nameof(HeaderedItemsControl.Header), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
                Bind(source, element, nameof(HeaderedItemsControl.HeaderStringFormat), HeaderedItemsControl.HeaderStringFormatProperty, BindingMode.OneWay);
                Bind(source, element, nameof(HeaderedItemsControl.HeaderTemplate), HeaderedItemsControl.HeaderTemplateProperty, BindingMode.OneWay);
                Bind(source, element, nameof(HeaderedItemsControl.HeaderTemplateSelector), HeaderedItemsControl.HeaderTemplateSelectorProperty, BindingMode.OneWay);
            }
            else
            {
                Bind(source, element, nameof(IHeaderedControl.Header), HeaderProperty, BindingMode.OneWay);
            }

            if (source.ToolTip is not null
                || BindingOperations.IsDataBound(source, ToolTipProperty))
            {
                Bind(source, element, nameof(ToolTip), ToolTipProperty, BindingMode.OneWay);
            }
            else
            {
                Bind(source, element, nameof(IHeaderedControl.Header), ToolTipProperty, BindingMode.OneWay);
            }
        }

        var ribbonControl = source as IRibbonControl;
        if (ribbonControl?.Icon is not null)
        {
            if (ribbonControl.Icon is Visual iconVisual)
            {
                var rect = new Rectangle
                {
                    Width = 16,
                    Height = 16,
                    Fill = new VisualBrush(iconVisual)
                };
                ((IRibbonControl)element).Icon = rect;
            }
            else
            {
                Bind(source, element, nameof(IRibbonControl.Icon), IconProperty, BindingMode.OneWay);
            }
        }

        RibbonProperties.SetSize(element, RibbonControlSize.Small);
    }

    /// <inheritdoc />
    public bool CanAddToQuickAccessToolBar
    {
        get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
        set { this.SetValue(CanAddToQuickAccessToolBarProperty, BooleanBoxes.Box(value)); }
    }

    /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
    public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty =
        DependencyProperty.Register(nameof(CanAddToQuickAccessToolBar), typeof(bool), typeof(RibbonControl), new PropertyMetadata(BooleanBoxes.TrueBox, OnCanAddToQuickAccessToolBarChanged));

    /// <summary>
    /// Occurs then CanAddToQuickAccessToolBar property changed
    /// </summary>
    public static void OnCanAddToQuickAccessToolBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.CoerceValue(ContextMenuProperty);
    }

    #endregion

    #region Binding

    internal static void Bind(object source, FrameworkElement target, string path, DependencyProperty property, BindingMode mode)
    {
        Bind(source, target, new PropertyPath(path), property, mode);
    }

    internal static void Bind(object source, FrameworkElement target, string path, DependencyProperty property, BindingMode mode, UpdateSourceTrigger updateSourceTrigger)
    {
        Bind(source, target, new PropertyPath(path), property, mode, updateSourceTrigger);
    }

    internal static void Bind(object source, FrameworkElement target, PropertyPath path, DependencyProperty property, BindingMode mode)
    {
        Bind(source, target, path, property, mode, UpdateSourceTrigger.Default);
    }

    internal static void Bind(object source, FrameworkElement target, PropertyPath path, DependencyProperty property, BindingMode mode, UpdateSourceTrigger updateSourceTrigger)
    {
        var binding = new Binding
        {
            Path = path,
            Source = source,
            Mode = mode,
            UpdateSourceTrigger = updateSourceTrigger
        };
        target.SetBinding(property, binding);
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public virtual KeyTipPressedResult OnKeyTipPressed()
    {
        return KeyTipPressedResult.Empty;
    }

    /// <inheritdoc />
    public virtual void OnKeyTipBack()
    {
    }

    #endregion

    #region StaticMethods

    /// <summary>
    /// Returns screen workarea in witch control is placed
    /// </summary>
    /// <param name="control">Control</param>
    /// <returns>Workarea in witch control is placed</returns>
    public static unsafe Rect GetControlWorkArea(FrameworkElement control)
    {
        if (PresentationSource.FromVisual(control) is null)
        {
            return default;
        }

        var controlPosOnScreen = control.PointToScreen(new Point(0, 0));

        var controlRect = new RECT
        {
            left = (int)controlPosOnScreen.X,
            top = (int)controlPosOnScreen.Y,
            right = (int)controlPosOnScreen.X + (int)control.ActualWidth,
            bottom = (int)controlPosOnScreen.Y + (int)control.ActualHeight
        };
        var monitor = PInvoke.MonitorFromRect(&controlRect, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
            PInvoke.GetMonitorInfo(monitor, &monitorInfo);
            return new Rect(monitorInfo.rcWork.left, monitorInfo.rcWork.top, monitorInfo.rcWork.right - monitorInfo.rcWork.left, monitorInfo.rcWork.bottom - monitorInfo.rcWork.top);
        }

        return default;
    }

    /// <summary>
    /// Returns monitor in witch control is placed
    /// </summary>
    /// <param name="control">Control</param>
    /// <returns>Workarea in witch control is placed</returns>
    public static unsafe Rect GetControlMonitor(FrameworkElement control)
    {
        if (PresentationSource.FromVisual(control) is null)
        {
            return default;
        }

        var controlPosOnScreen = control.PointToScreen(new Point(0, 0));

        var controlRect = new RECT
        {
            left = (int)controlPosOnScreen.X,
            top = (int)controlPosOnScreen.Y,
            right = (int)controlPosOnScreen.X + (int)control.ActualWidth,
            bottom = (int)controlPosOnScreen.Y + (int)control.ActualHeight
        };
        var monitor = PInvoke.MonitorFromRect(&controlRect, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
            PInvoke.GetMonitorInfo(monitor, &monitorInfo);
            return new Rect(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top, monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top);
        }

        return default;
    }

    /// <summary>
    /// Get the parent <see cref="Ribbon"/>.
    /// </summary>
    /// <returns>The found <see cref="Ribbon"/> or <c>null</c> of no parent <see cref="Ribbon"/> could be found.</returns>
    public static Ribbon? GetParentRibbon(DependencyObject obj)
    {
        return UIHelper.GetParent<Ribbon>(obj);
    }

    #endregion

    /// <inheritdoc />
    void ILogicalChildSupport.AddLogicalChild(object child)
    {
        this.AddLogicalChild(child);
    }

    /// <inheritdoc />
    void ILogicalChildSupport.RemoveLogicalChild(object child)
    {
        this.RemoveLogicalChild(child);
    }

    /// <inheritdoc />
    protected override IEnumerator LogicalChildren
    {
        get
        {
            var baseEnumerator = base.LogicalChildren;
            while (baseEnumerator?.MoveNext() == true)
            {
                yield return baseEnumerator.Current;
            }

            if (this.Icon is not null)
            {
                yield return this.Icon;
            }

            if (this.Header is not null)
            {
                yield return this.Header;
            }
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonControlAutomationPeer(this);
}