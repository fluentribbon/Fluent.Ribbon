// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Fluent.Collections;
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
        get => (string?)this.GetValue(KeyTipProperty);
        set => this.SetValue(KeyTipProperty, value);
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
        get => this.GetValue(HeaderProperty);
        set => this.SetValue(HeaderProperty, value);
    }

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(RibbonControl), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    /// <inheritdoc />
    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?)this.GetValue(HeaderTemplateProperty);
        set => this.SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(RibbonControl), new PropertyMetadata());

    /// <inheritdoc />
    public DataTemplateSelector? HeaderTemplateSelector
    {
        get => (DataTemplateSelector?)this.GetValue(HeaderTemplateSelectorProperty);
        set => this.SetValue(HeaderTemplateSelectorProperty, value);
    }

    /// <summary>Identifies the <see cref="HeaderTemplateSelector"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty =
        DependencyProperty.Register(nameof(HeaderTemplateSelector), typeof(DataTemplateSelector), typeof(RibbonControl), new PropertyMetadata());

    #endregion

    #region Icon

    /// <inheritdoc />
    [Localizability(LocalizationCategory.NeverLocalize)]
    [Localizable(false)]
    public object? Icon
    {
        get => this.GetValue(IconProperty);
        set => this.SetValue(IconProperty, value);
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
    public ICommand? Command
    {
        get => (ICommand?)this.GetValue(CommandProperty);

        set => this.SetValue(CommandProperty, value);
    }

    /// <inheritdoc />
    [Bindable(true)]
    [Localizability(LocalizationCategory.NeverLocalize)]
    [Category("Action")]
    public object CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);

        set => this.SetValue(CommandParameterProperty, value);
    }

    /// <inheritdoc />
    [Bindable(true)]
    [Category("Action")]
    public IInputElement CommandTarget
    {
        get => (IInputElement)this.GetValue(CommandTargetProperty);

        set => this.SetValue(CommandTargetProperty, value);
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
            CanExecuteChangedEventManager.RemoveHandler(oldCommand, control.OnCommandCanExecuteChanged);
        }

        if (e.NewValue is ICommand newCommand)
        {
            CanExecuteChangedEventManager.AddHandler(newCommand, control.OnCommandCanExecuteChanged);

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
        get => (RibbonControlSize)this.GetValue(SizeProperty);
        set => this.SetValue(SizeProperty, value);
    }

    /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
    public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(RibbonControl));

    #endregion

    #region SizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SizeDefinition
    {
        get => (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty);
        set => this.SetValue(SizeDefinitionProperty, value);
    }

    /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(RibbonControl));

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

    // Keeps the GroupStyle sync helpers alive exactly as long as their quick access copy (the key), without the source referencing the copy.
    private static readonly ConditionalWeakTable<ItemsControl, List<CollectionSyncHelper<GroupStyle>>> GroupStyleSyncHelpers = new();

    /// <summary>
    /// Binds default properties of control to quick access element
    /// </summary>
    /// <param name="source">Source item</param>
    /// <param name="target">Toolbar item</param>
    public static void BindQuickAccessItem(FrameworkElement source, FrameworkElement target)
    {
        Bind(source, target, nameof(source.DataContext), DataContextProperty, BindingMode.OneWay);

        Bind(source, target, nameof(FontFamily), FontFamilyProperty, BindingMode.OneWay);
        Bind(source, target, nameof(FontSize), FontSizeProperty, BindingMode.OneWay);
        Bind(source, target, nameof(FontStretch), FontStretchProperty, BindingMode.OneWay);
        Bind(source, target, nameof(FontStyle), FontStyleProperty, BindingMode.OneWay);
        Bind(source, target, nameof(FontWeight), FontWeightProperty, BindingMode.OneWay);

        Bind(source, target, nameof(Foreground), ForegroundProperty, BindingMode.OneWay);
        Bind(source, target, nameof(IsEnabled), IsEnabledProperty, BindingMode.OneWay);
        Bind(source, target, nameof(Opacity), OpacityProperty, BindingMode.OneWay);
        Bind(source, target, nameof(SnapsToDevicePixels), SnapsToDevicePixelsProperty, BindingMode.OneWay);

        Bind(source, target, FocusManager.IsFocusScopeProperty, BindingMode.OneWay);

        // The Quick Access Toolbar copy is a separate element with its own automation peer, so screen readers
        // would otherwise announce it without the accessible name/help text the app author gave the original.
        // AutomationId is intentionally NOT copied: it must be unique, and a duplicate would make UI tests
        // that search by AutomationId find the copy first instead of the original control.
        Bind(source, target, System.Windows.Automation.AutomationProperties.NameProperty, BindingMode.OneWay);
        Bind(source, target, System.Windows.Automation.AutomationProperties.HelpTextProperty, BindingMode.OneWay);

        Bind(source, target, InputControlProperties.InputMinWidthProperty, BindingMode.OneWay);
        Bind(source, target, InputControlProperties.InputWidthProperty, BindingMode.OneWay);
        Bind(source, target, InputControlProperties.InputHeightProperty, BindingMode.OneWay);

        if (source is ICommandSource)
        {
            if (source is MenuItem)
            {
                Bind(source, target, nameof(ICommandSource.CommandParameter), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);
                Bind(source, target, nameof(ICommandSource.CommandTarget), System.Windows.Controls.MenuItem.CommandTargetProperty, BindingMode.OneWay);
                Bind(source, target, nameof(ICommandSource.Command), System.Windows.Controls.MenuItem.CommandProperty, BindingMode.OneWay);
            }
            else
            {
                Bind(source, target, nameof(ICommandSource.CommandParameter), ButtonBase.CommandParameterProperty, BindingMode.OneWay);
                Bind(source, target, nameof(ICommandSource.CommandTarget), ButtonBase.CommandTargetProperty, BindingMode.OneWay);
                Bind(source, target, nameof(ICommandSource.Command), ButtonBase.CommandProperty, BindingMode.OneWay);
            }
        }

        if (source is System.Windows.Controls.Primitives.ToggleButton or System.Windows.Controls.MenuItem
            && target is System.Windows.Controls.Primitives.ToggleButton)
        {
            var sourceIsCheckedProperty = source is System.Windows.Controls.MenuItem
                ? System.Windows.Controls.MenuItem.IsCheckedProperty
                : System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty;

            BindIsChecked(source, sourceIsCheckedProperty, target, System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty);
        }

        if (source is ItemsControl sourceItemsControl
            && target is ItemsControl targetItemsControl)
        {
            Bind(source, target, nameof(ItemsControl.AlternationCount), ItemsControl.AlternationCountProperty, BindingMode.OneWay);

            Bind(source, target, nameof(ItemsControl.DisplayMemberPath), ItemsControl.DisplayMemberPathProperty, BindingMode.OneWay);

            Bind(source, target, nameof(ItemsControl.ItemBindingGroup), ItemsControl.ItemBindingGroupProperty, BindingMode.OneWay);
            Bind(source, target, nameof(ItemsControl.ItemStringFormat), ItemsControl.ItemStringFormatProperty, BindingMode.OneWay);

            Bind(source, target, nameof(ItemsControl.ItemTemplate), ItemsControl.ItemTemplateProperty, BindingMode.OneWay);
            Bind(source, target, nameof(ItemsControl.ItemTemplateSelector), ItemsControl.ItemTemplateSelectorProperty, BindingMode.OneWay);

            Bind(source, target, nameof(ItemsControl.ItemsPanel), ItemsControl.ItemsPanelProperty, BindingMode.OneWay);

            Bind(source, target, nameof(ItemsControl.ItemContainerStyle), ItemsControl.ItemContainerStyleProperty, BindingMode.OneWay);
            Bind(source, target, nameof(ItemsControl.ItemContainerStyleSelector), ItemsControl.ItemContainerStyleSelectorProperty, BindingMode.OneWay);

            Bind(source, target, nameof(ItemsControl.GroupStyleSelector), ItemsControl.GroupStyleSelectorProperty, BindingMode.OneWay);

            // cannot "bind" to GroupStyle observable collection property, but can at least keep them in sync.
            // The helper listens weakly and is kept alive by the target only. A strong subscription would let the source
            // (which usually lives as long as the window) keep every quick access copy ever created alive, and the toolbar
            // re-creates its copies whenever items are added/removed, its state is loaded or the ribbon is re-templated.
            var groupStyleSync = new CollectionSyncHelper<GroupStyle>(sourceItemsControl.GroupStyle, targetItemsControl.GroupStyle, listenWeakly: true);
            GroupStyleSyncHelpers.GetOrCreateValue(targetItemsControl).Add(groupStyleSync);

            if (source is Selector
                && target is Selector)
            {
                Bind(source, target, nameof(Selector.SelectedValuePath), Selector.SelectedValuePathProperty, BindingMode.OneWay);

                Bind(source, target, nameof(Selector.IsSynchronizedWithCurrentItem), Selector.IsSynchronizedWithCurrentItemProperty, BindingMode.OneWay);
            }
        }

        if (source is IHeaderedControl
            && target is IHeaderedControl)
        {
            Bind(source, target, nameof(IHeaderedControl.Header), HeaderProperty, BindingMode.OneWay);

            // The header properties of HeaderedItemsControl are different from the ones of RibbonControl (and its derived/AddOwner types),
            // so we can only use them if both, source and target, are a HeaderedItemsControl.
            if (source is HeaderedItemsControl
                && target is HeaderedItemsControl)
            {
                Bind(source, target, nameof(HeaderedItemsControl.Header), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
                Bind(source, target, nameof(HeaderedItemsControl.HeaderStringFormat), HeaderedItemsControl.HeaderStringFormatProperty, BindingMode.OneWay);
                Bind(source, target, nameof(HeaderedItemsControl.HeaderTemplate), HeaderedItemsControl.HeaderTemplateProperty, BindingMode.OneWay);
                Bind(source, target, nameof(HeaderedItemsControl.HeaderTemplateSelector), HeaderedItemsControl.HeaderTemplateSelectorProperty, BindingMode.OneWay);
            }

            if (source.ToolTip is not null
                || BindingOperations.IsDataBound(source, ToolTipProperty))
            {
                Bind(source, target, nameof(ToolTip), ToolTipProperty, BindingMode.OneWay);
            }
            else
            {
                Bind(source, target, nameof(IHeaderedControl.Header), ToolTipProperty, BindingMode.OneWay);
            }
        }

        Bind(source, target, RibbonProperties.CustomIconSizeProperty, BindingMode.OneWay);

        if (source is IRibbonControl sourceRibbonControl)
        {
            if (sourceRibbonControl.Icon is Visual iconVisual)
            {
                var rect = new Rectangle
                {
                    Width = 16,
                    Height = 16,
                    Fill = new VisualBrush(iconVisual)
                };
                target.SetValue(IconProperty, rect);
            }
            else
            {
                Bind(source, target, nameof(IRibbonControl.Icon), IconProperty, BindingMode.OneWay);
            }
        }

        if (source is IMediumIconProvider sourceMediummIconProvider)
        {
            if (sourceMediummIconProvider.MediumIcon is Visual iconVisual)
            {
                var rect = new Rectangle
                {
                    Width = 24,
                    Height = 24,
                    Fill = new VisualBrush(iconVisual)
                };
                target.SetValue(MediumIconProviderProperties.MediumIconProperty, rect);
            }
            else
            {
                Bind(source, target, nameof(IMediumIconProvider.MediumIcon), MediumIconProviderProperties.MediumIconProperty, BindingMode.OneWay);
            }
        }

        if (source is ILargeIconProvider sourceLargeIconProvider)
        {
            if (sourceLargeIconProvider.LargeIcon is Visual iconVisual)
            {
                var rect = new Rectangle
                {
                    Width = 32,
                    Height = 32,
                    Fill = new VisualBrush(iconVisual)
                };
                target.SetValue(LargeIconProviderProperties.LargeIconProperty, rect);
            }
            else
            {
                Bind(source, target, nameof(ILargeIconProvider.LargeIcon), LargeIconProviderProperties.LargeIconProperty, BindingMode.OneWay);
            }
        }

        Bind(source, target, new PropertyPath(RibbonProperties.QATIconSizeProperty), RibbonProperties.IconSizeProperty, BindingMode.OneWay);

        RibbonProperties.SetSize(target, RibbonControlSize.Small);
    }

    /// <inheritdoc />
    public bool CanAddToQuickAccessToolBar
    {
        get => (bool)this.GetValue(CanAddToQuickAccessToolBarProperty);
        set => this.SetValue(CanAddToQuickAccessToolBarProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
    public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty =
        DependencyProperty.Register(nameof(CanAddToQuickAccessToolBar), typeof(bool), typeof(RibbonControl), new PropertyMetadata(BooleanBoxes.TrueBox, OnCanAddToQuickAccessToolBarChanged));

    /// <summary>
    /// Handles changes of <see cref="CanAddToQuickAccessToolBar"/> by re-coercing the context menu
    /// </summary>
    public static void OnCanAddToQuickAccessToolBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.CoerceValue(ContextMenuProperty);
    }

    #endregion

    #region Binding

    internal static void Bind(object source, FrameworkElement target, DependencyProperty property, BindingMode mode)
    {
        Bind(source, target, new PropertyPath(property), property, mode);
    }

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

    /// <summary>
    /// Keeps the IsChecked property of <paramref name="target"/> (a quick access copy) in sync with the IsChecked property of <paramref name="source"/>.
    /// </summary>
    /// <remarks>
    /// A TwoWay binding would write <paramref name="source"/> with SetValue when the copy is clicked.
    /// That replaces a OneWay binding the app has on the original (state from the view model, change through a command) with a local value,
    /// so the original would no longer follow the view model. So we bind OneWay and write back with SetCurrentValue, which keeps such bindings
    /// (and still updates TwoWay bindings).
    /// </remarks>
    internal static void BindIsChecked(DependencyObject source, DependencyProperty sourceIsCheckedProperty, FrameworkElement target, DependencyProperty targetIsCheckedProperty)
    {
        Bind(source, target, new PropertyPath(sourceIsCheckedProperty), targetIsCheckedProperty, BindingMode.OneWay);

        var handler = new RoutedEventHandler((_, e) =>
        {
            // Checked etc. bubble, so ignore the ones raised by elements inside the copy (for example the inner button of a SplitButton).
            if (ReferenceEquals(e.OriginalSource, target))
            {
                WriteIsCheckedBack(target, targetIsCheckedProperty, source, sourceIsCheckedProperty);
            }
        });

        target.AddHandler(System.Windows.Controls.Primitives.ToggleButton.CheckedEvent, handler);
        target.AddHandler(System.Windows.Controls.Primitives.ToggleButton.UncheckedEvent, handler);
        target.AddHandler(System.Windows.Controls.Primitives.ToggleButton.IndeterminateEvent, handler);
    }

    /// <summary>
    /// Writes the IsChecked value of <paramref name="target"/> to <paramref name="source"/> with SetCurrentValue, so bindings on <paramref name="source"/> are kept.
    /// If <paramref name="source"/> doesn't take the value (coercion), <paramref name="target"/> is reset to the value of <paramref name="source"/>.
    /// </summary>
    internal static void WriteIsCheckedBack(DependencyObject target, DependencyProperty targetIsCheckedProperty, DependencyObject source, DependencyProperty sourceIsCheckedProperty)
    {
        var value = (bool?)target.GetValue(targetIsCheckedProperty);

        // MenuItem.IsChecked is a bool, not a bool?
        var sourceValue = sourceIsCheckedProperty.PropertyType == typeof(bool)
            ? BooleanBoxes.Box(value == true)
            : BooleanBoxes.Box(value);

        if (Equals(source.GetValue(sourceIsCheckedProperty), sourceValue) == false)
        {
            source.SetCurrentValue(sourceIsCheckedProperty, sourceValue);
        }

        var actualValue = (bool?)source.GetValue(sourceIsCheckedProperty);
        if (actualValue != value)
        {
            target.SetCurrentValue(targetIsCheckedProperty, BooleanBoxes.Box(actualValue));
        }
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
    /// <returns>Bounds of the whole monitor on which the control is placed (including the taskbar)</returns>
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