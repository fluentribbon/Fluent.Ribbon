// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Fluent.Extensions;
using Fluent.Helpers;
using Fluent.Internal;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Represents menu item
/// </summary>
[ContentProperty(nameof(Items))]
[TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
[TemplatePart(Name = "PART_MenuPanel", Type = typeof(Panel))]
public class MenuItem : System.Windows.Controls.MenuItem, IQuickAccessItemProvider, IRibbonControl, IDropDownControl, IToggleButton
{
    #region Fields

    private Panel? menuPanel;

    private ScrollViewer? scrollViewer;

    #endregion

    #region Properties

    private bool IsItemsControlMenuBase => (ItemsControlHelper.ItemsControlFromItemContainer(this) ?? VisualTreeHelper.GetParent(this)) is MenuBase;

    #region Size

    /// <inheritdoc />
    public RibbonControlSize Size
    {
        get => (RibbonControlSize)this.GetValue(SizeProperty);
        set => this.SetValue(SizeProperty, value);
    }

    /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
    public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(MenuItem));

    #endregion

    #region SizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SizeDefinition
    {
        get => (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty);
        set => this.SetValue(SizeDefinitionProperty, value);
    }

    /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(MenuItem));

    #endregion

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
    public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(MenuItem));

    #endregion

    /// <inheritdoc />
    public Popup? DropDownPopup { get; private set; }

    /// <inheritdoc />
    public bool IsContextMenuOpened { get; set; }

    #region Description

    /// <summary>
    /// Useless property only used in secon level application menu items
    /// </summary>
    public string? Description
    {
        get => (string?)this.GetValue(DescriptionProperty);
        set => this.SetValue(DescriptionProperty, value);
    }

    /// <summary>Identifies the <see cref="Description"/> dependency property.</summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(MenuItem), new PropertyMetadata(default(string)));

    #endregion

    #region IsDropDownOpen

    /// <inheritdoc />
    public bool IsDropDownOpen
    {
        get => this.IsSubmenuOpen;
        set => this.IsSubmenuOpen = value;
    }

    #endregion

    #region IsDefinitive

    /// <summary>
    /// Gets or sets whether ribbon control click must close backstage
    /// </summary>
    public bool IsDefinitive
    {
        get => (bool)this.GetValue(IsDefinitiveProperty);
        set => this.SetValue(IsDefinitiveProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="IsDefinitive"/> dependency property.</summary>
    public static readonly DependencyProperty IsDefinitiveProperty = DependencyProperty.Register(nameof(IsDefinitive), typeof(bool), typeof(MenuItem), new PropertyMetadata(BooleanBoxes.TrueBox));

    #endregion

    #region ResizeMode

    /// <summary>
    /// Gets or sets context menu resize mode
    /// </summary>
    public ContextMenuResizeMode ResizeMode
    {
        get => (ContextMenuResizeMode)this.GetValue(ResizeModeProperty);
        set => this.SetValue(ResizeModeProperty, value);
    }

    /// <summary>Identifies the <see cref="ResizeMode"/> dependency property.</summary>
    public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.Register(nameof(ResizeMode), typeof(ContextMenuResizeMode), typeof(MenuItem), new PropertyMetadata(ContextMenuResizeMode.None));

    #endregion

    #region MaxDropDownHeight

    /// <summary>
    /// Get or sets max height of drop down popup
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)this.GetValue(MaxDropDownHeightProperty);
        set => this.SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>Identifies the <see cref="MaxDropDownHeight"/> dependency property.</summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(MenuItem), new FrameworkPropertyMetadata(double.NaN, null, DropDownHelper.CoerceMaxDropDownHeight));

    #endregion

    #region IsSplit

    /// <summary>
    /// Gets or sets a value indicating whether menu item is split.
    /// </summary>
    public bool IsSplit
    {
        get => (bool)this.GetValue(IsSplitProperty);
        set => this.SetValue(IsSplitProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="IsSplit"/> dependency property.</summary>
    public static readonly DependencyProperty IsSplitProperty = DependencyProperty.Register(nameof(IsSplit), typeof(bool), typeof(MenuItem), new PropertyMetadata(BooleanBoxes.FalseBox));

    #endregion

    #region GroupName

    /// <inheritdoc />
    public string? GroupName
    {
        get => (string?)this.GetValue(GroupNameProperty);
        set => this.SetValue(GroupNameProperty, value);
    }

    /// <inheritdoc />
    bool? IToggleButton.IsChecked
    {
        get => this.IsChecked;
        set => this.IsChecked = value == true;
    }

    /// <summary>Identifies the <see cref="GroupName"/> dependency property.</summary>
    public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(MenuItem), new PropertyMetadata(ToggleButtonHelper.OnGroupNameChanged));

    #endregion

    #endregion

    #region Events

    /// <inheritdoc />
    public event EventHandler? DropDownOpened;

    /// <inheritdoc />
    public event EventHandler? DropDownClosed;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes static members of the <see cref="MenuItem"/> class.
    /// </summary>
    static MenuItem()
    {
        var type = typeof(MenuItem);
        ToolTipService.Attach(type);
        //PopupService.Attach(type);
        ContextMenuService.Attach(type);
        DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
        IsCheckedProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, ToggleButtonHelper.OnIsCheckedChanged));

        IconProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    public MenuItem()
    {
        ContextMenuService.Coerce(this);
    }

    /// <inheritdoc />
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        // Fix to raise MouseWhele event
        (this.Parent as ListBox)?.RaiseEvent(e);
    }

    #endregion

    /// <summary>Identifies the <see cref="RecognizesAccessKey"/> dependency property.</summary>
    public static readonly DependencyProperty RecognizesAccessKeyProperty = DependencyProperty.RegisterAttached(nameof(RecognizesAccessKey), typeof(bool), typeof(MenuItem), new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>Helper for setting <see cref="RecognizesAccessKeyProperty"/> on <paramref name="element"/>.</summary>
    /// <param name="element"><see cref="DependencyObject"/> to set <see cref="RecognizesAccessKeyProperty"/> on.</param>
    /// <param name="value">RecognizesAccessKey property value.</param>
    public static void SetRecognizesAccessKey(DependencyObject element, bool value)
    {
        element.SetValue(RecognizesAccessKeyProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Helper for getting <see cref="RecognizesAccessKeyProperty"/> from <paramref name="element"/>.</summary>
    /// <param name="element"><see cref="DependencyObject"/> to read <see cref="RecognizesAccessKeyProperty"/> from.</param>
    /// <returns>RecognizesAccessKey property value.</returns>
    public static bool GetRecognizesAccessKey(DependencyObject element)
    {
        return (bool)element.GetValue(RecognizesAccessKeyProperty);
    }

    /// <summary>
    /// Defines if access keys should be recognized.
    /// </summary>
    public bool RecognizesAccessKey
    {
        get => (bool)this.GetValue(RecognizesAccessKeyProperty);
        set => this.SetValue(RecognizesAccessKeyProperty, BooleanBoxes.Box(value));
    }

    #region QuickAccess

    /// <inheritdoc />
    public virtual FrameworkElement CreateQuickAccessItem()
    {
        if (this.HasItems)
        {
            if (this.IsSplit)
            {
                var button = new SplitButton
                {
                    CanAddButtonToQuickAccessToolBar = false
                };
                RibbonControl.BindQuickAccessItem(this, button);
                RibbonControl.Bind(this, button, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.DisplayMemberPath), DisplayMemberPathProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.GroupStyleSelector), GroupStyleSelectorProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.ItemContainerStyle), ItemContainerStyleProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.ItemsPanel), ItemsPanelProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.ItemStringFormat), ItemStringFormatProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.ItemTemplate), ItemTemplateProperty, BindingMode.OneWay);
                button.DropDownOpened += this.OnQuickAccessOpened;
                return button;
            }
            else
            {
                var button = new DropDownButton();
                RibbonControl.BindQuickAccessItem(this, button);
                RibbonControl.Bind(this, button, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.DisplayMemberPath), DisplayMemberPathProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.GroupStyleSelector), GroupStyleSelectorProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.ItemContainerStyle), ItemContainerStyleProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.ItemsPanel), ItemsPanelProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.ItemStringFormat), ItemStringFormatProperty, BindingMode.OneWay);
                RibbonControl.Bind(this, button, nameof(this.ItemTemplate), ItemTemplateProperty, BindingMode.OneWay);
                button.DropDownOpened += this.OnQuickAccessOpened;
                return button;
            }
        }
        else
        {
            if (this.IsCheckable)
            {
                var toggleButton = new ToggleButton();
                RibbonControl.Bind(this, toggleButton, nameof(this.IsChecked), System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, BindingMode.TwoWay);
                RibbonControl.BindQuickAccessItem(this, toggleButton);
                return toggleButton;
            }
            else
            {
                var button = new Button();
                RibbonControl.BindQuickAccessItem(this, button);
                return button;
            }
        }
    }

    /// <summary>
    /// Handles quick access button drop down menu opened
    /// </summary>
    protected void OnQuickAccessOpened(object? sender, EventArgs e)
    {
        var buttonInQuickAccess = (DropDownButton?)sender;

        if (buttonInQuickAccess is not null)
        {
            buttonInQuickAccess.DropDownClosed += this.OnQuickAccessMenuClosedOrUnloaded;
            buttonInQuickAccess.Unloaded += this.OnQuickAccessMenuClosedOrUnloaded;

            ItemsControlHelper.MoveItemsToDifferentControl(this, buttonInQuickAccess);
        }
    }

    /// <summary>
    /// Handles quick access button drop down menu closed
    /// </summary>
    protected void OnQuickAccessMenuClosedOrUnloaded(object? sender, EventArgs e)
    {
        var buttonInQuickAccess = (DropDownButton?)sender;

        if (buttonInQuickAccess is not null)
        {
            buttonInQuickAccess.DropDownClosed -= this.OnQuickAccessMenuClosedOrUnloaded;
            buttonInQuickAccess.Unloaded -= this.OnQuickAccessMenuClosedOrUnloaded;

            ItemsControlHelper.MoveItemsToDifferentControl(buttonInQuickAccess, this);
        }
    }

    /// <inheritdoc />
    public bool CanAddToQuickAccessToolBar
    {
        get => (bool)this.GetValue(CanAddToQuickAccessToolBarProperty);
        set => this.SetValue(CanAddToQuickAccessToolBarProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
    public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(MenuItem));

    private bool isContextMenuOpening;

    #endregion

    #region Public

    /// <inheritdoc />
    public virtual KeyTipPressedResult OnKeyTipPressed()
    {
        if (this.HasItems == false)
        {
            this.OnClick();

            return KeyTipPressedResult.Empty;
        }
        else
        {
            Keyboard.Focus(this);
            this.IsDropDownOpen = true;

            return new KeyTipPressedResult(true, true);
        }
    }

    /// <inheritdoc />
    public void OnKeyTipBack()
    {
        this.IsDropDownOpen = false;
    }

    #endregion

    #region Overrides

    private object? currentItem;

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        if (item is ItemsControl or Separator)
        {
            return true;
        }

        if (this.UsesItemContainerTemplate)
        {
            this.currentItem = item;
        }

        return false;
    }

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
    {
        if (this.UsesItemContainerTemplate is false)
        {
            this.currentItem = null;
            return new MenuItem();
        }

        var item = this.currentItem;
        this.currentItem = null;

        var dataTemplate = this.ItemContainerTemplateSelector?.SelectTemplate(item, this)
                           ?? this.ItemTemplate;

        if (dataTemplate is not null)
        {
            return dataTemplate.LoadContent();
        }

        return new MenuItem();
    }

    #region Non MenuBase ItemsControl workarounds

    /// <summary>
    /// Returns logical parent; either Parent or ItemsControlFromItemContainer(this).
    /// </summary>
    /// <remarks>
    /// Copied from <see cref="System.Windows.Controls.MenuItem"/>.
    /// </remarks>
    public object LogicalParent
    {
        get
        {
            if (this.Parent is not null)
            {
                return this.Parent;
            }

            return ItemsControlFromItemContainer(this);
        }
    }

    /// <inheritdoc />
    protected override void OnIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnIsKeyboardFocusedChanged(e);

        if (this.IsItemsControlMenuBase == false)
        {
            this.IsHighlighted = this.IsKeyboardFocused;
        }
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        if (this.IsItemsControlMenuBase == false
            && this.isContextMenuOpening == false)
        {
            if (this.HasItems
                && this.LogicalParent is DropDownButton)
            {
                this.IsSubmenuOpen = true;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        if (this.IsItemsControlMenuBase == false
            && this.isContextMenuOpening == false)
        {
            if (this.HasItems
                && this.LogicalParent is DropDownButton // prevent too slow close on regular DropDown
                && this.LogicalParent is ApplicationMenu == false) // prevent eager close on ApplicationMenu
            {
                this.IsSubmenuOpen = false;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        this.isContextMenuOpening = true;

        // We have to close the sub menu as soon as the context menu gets opened
        // but only if it should be opened on ourself
        if (ReferenceEquals(this, e.Source))
        {
            this.IsSubmenuOpen = false;
        }

        base.OnContextMenuOpening(e);
    }

    /// <inheritdoc />
    protected override void OnContextMenuClosing(ContextMenuEventArgs e)
    {
        this.isContextMenuOpening = false;

        base.OnContextMenuClosing(e);
    }

    #endregion Non MenuBase ItemsControl workarounds

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            if (this.IsSplit)
            {
                if (this.GetTemplateChild("PART_ButtonBorder") is Border buttonBorder
                    && PopupService.IsMousePhysicallyOver(buttonBorder))
                {
                    this.OnClick();
                }
            }
        }

        base.OnMouseLeftButtonUp(e);
    }

    /// <inheritdoc />
    protected override void OnClick()
    {
        // Close popup on click
        if (this.IsDefinitive
            && (!this.HasItems || this.IsSplit))
        {
            PopupService.RaiseDismissPopupEventAsync(this, DismissPopupMode.Always);
        }

        var revertIsChecked = false;

        // Rewriting everthing contained in base.OnClick causes a lot of trouble.
        // In case IsCheckable is true and GroupName is not empty we revert the value for IsChecked back to true to prevent unchecking all items in the group
        if (this.IsCheckable
            && string.IsNullOrEmpty(this.GroupName) == false)
        {
            // If checked revert the IsChecked value back to true after forwarding the click to base
            if (this.IsChecked)
            {
                revertIsChecked = true;
            }
        }

        base.OnClick();

        if (revertIsChecked)
        {
            this.RunInDispatcherAsync(() => this.SetCurrentValue(IsCheckedProperty, BooleanBoxes.TrueBox), DispatcherPriority.Background);
        }
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        if (this.DropDownPopup is not null)
        {
            this.DropDownPopup.Opened -= this.OnDropDownOpened;
            this.DropDownPopup.Closed -= this.OnDropDownClosed;
        }

        this.DropDownPopup = this.GetTemplateChild("PART_Popup") as Popup;

        if (this.DropDownPopup is not null)
        {
            this.DropDownPopup.Opened += this.OnDropDownOpened;
            this.DropDownPopup.Closed += this.OnDropDownClosed;

            KeyboardNavigation.SetControlTabNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetDirectionalNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetTabNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
        }

        this.scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
        this.menuPanel = this.GetTemplateChild("PART_MenuPanel") as Panel;
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (this.IsSubmenuOpen)
            {
                this.IsSubmenuOpen = false;
            }
            else
            {
                this.CloseParentDropDownOrMenuItem();
            }

            e.Handled = true;
        }
        else
        {
            #region Non MenuBase ItemsControl workarounds

            if (this.IsItemsControlMenuBase == false)
            {
                var key = e.Key;

                if (this.FlowDirection == FlowDirection.RightToLeft)
                {
                    if (key == Key.Right)
                    {
                        key = Key.Left;
                    }
                    else if (key == Key.Left)
                    {
                        key = Key.Right;
                    }
                }

                if (key == Key.Right
                    && this.menuPanel is not null)
                {
                    this.IsSubmenuOpen = true;
                    this.menuPanel.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                    e.Handled = true;
                }
                else if (key == Key.Left)
                {
                    if (this.IsSubmenuOpen)
                    {
                        this.IsSubmenuOpen = false;
                    }
                    else
                    {
                        var parentMenuItem = UIHelper.GetParent<System.Windows.Controls.MenuItem>(this);
                        if (parentMenuItem is not null)
                        {
                            parentMenuItem.IsSubmenuOpen = false;
                        }
                    }

                    e.Handled = true;
                }

                if (e.Handled)
                {
                    return;
                }
            }

            #endregion Non MenuBase ItemsControl workarounds

            base.OnKeyDown(e);
        }
    }

    private void CloseParentDropDownOrMenuItem()
    {
        var parent = UIHelper.GetParent<DependencyObject>(this, x => x is IDropDownControl || x is System.Windows.Controls.MenuItem);

        if (parent is null)
        {
            return;
        }

        if (parent is IDropDownControl dropDown)
        {
            dropDown.IsDropDownOpen = false;
        }
        else
        {
            ((System.Windows.Controls.MenuItem)parent).IsSubmenuOpen = false;
        }
    }

    #endregion

    #region Methods

    // Handles drop down opened
    private void OnDropDownClosed(object? sender, EventArgs e)
    {
        this.DropDownClosed?.Invoke(this, e);
    }

    // Handles drop down closed
    private void OnDropDownOpened(object? sender, EventArgs e)
    {
        if (this.scrollViewer is not null
            && this.ResizeMode != ContextMenuResizeMode.None)
        {
            this.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        if (this.menuPanel is not null)
        {
            this.menuPanel.Width = double.NaN;
            this.menuPanel.Height = double.NaN;
        }

        this.DropDownOpened?.Invoke(this, e);
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
        }
    }
}