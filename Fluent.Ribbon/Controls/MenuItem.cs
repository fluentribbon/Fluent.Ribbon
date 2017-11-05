// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Fluent.Extensions;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents menu item
    /// </summary>
    [ContentProperty(nameof(Items))]
    public class MenuItem : System.Windows.Controls.MenuItem, IQuickAccessItemProvider, IRibbonControl, IDropDownControl, IToggleButton
    {
        #region Fields

        // Thumb to resize in both directions
        private Thumb resizeBothThumb;
        // Thumb to resize vertical
        private Thumb resizeVerticalThumb;

        private Panel menuPanel;

        private ScrollViewer scrollViewer;

        #endregion

        #region Properties

        private bool IsItemsControlMenuBase => (ItemsControlFromItemContainer(this) ?? VisualTreeHelper.GetParent(this)) is MenuBase;

        #region Size

        /// <summary>
        /// Gets or sets Size for the element.
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(MenuItem));

        #endregion

        #region SizeDefinition

        /// <summary>
        /// Gets or sets SizeDefinition for element.
        /// </summary>
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
            set { this.SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(MenuItem));

        #endregion

        #region KeyTip

        /// <summary>
        /// Gets or sets KeyTip for element.
        /// </summary>
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(MenuItem));

        #endregion

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup { get; private set; }

        /// <summary>
        /// Gets a value indicating whether context menu is opened
        /// </summary>
        public bool IsContextMenuOpened { get; set; }

        #region Description

        /// <summary>
        /// Useless property only used in secon level application menu items
        /// </summary>
        public string Description
        {
            get { return (string)this.GetValue(DescriptionProperty); }
            set { this.SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(MenuItem), new PropertyMetadata(default(string)));

        #endregion

        #region IsDropDownOpen

        /// <summary>
        /// Gets or sets whether popup is opened
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return this.IsSubmenuOpen; }
            set { this.IsSubmenuOpen = value; }
        }

        #endregion

        #region IsDefinitive

        /// <summary>
        /// Gets or sets whether ribbon control click must close backstage
        /// </summary>
        public bool IsDefinitive
        {
            get { return (bool)this.GetValue(IsDefinitiveProperty); }
            set { this.SetValue(IsDefinitiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDefinitive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDefinitiveProperty =
            DependencyProperty.Register(nameof(IsDefinitive), typeof(bool), typeof(MenuItem), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region ResizeMode

        /// <summary>
        /// Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode ResizeMode
        {
            get { return (ContextMenuResizeMode)this.GetValue(ResizeModeProperty); }
            set { this.SetValue(ResizeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeMode.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register(nameof(ResizeMode), typeof(ContextMenuResizeMode),
            typeof(MenuItem), new PropertyMetadata(ContextMenuResizeMode.None));

        #endregion

        #region MaxDropDownHeight

        /// <summary>
        /// Get or sets max height of drop down popup
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)this.GetValue(MaxDropDownHeightProperty); }
            set { this.SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(MenuItem), new PropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0));

        #endregion

        #region IsSplited

        /// <summary>
        /// Gets or sets a value indicating whether menu item is splited
        /// </summary>
        public bool IsSplited
        {
            get { return (bool)this.GetValue(IsSplitedProperty); }
            set { this.SetValue(IsSplitedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSplited.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSplitedProperty =
            DependencyProperty.Register(nameof(IsSplited), typeof(bool), typeof(MenuItem), new PropertyMetadata(BooleanBoxes.FalseBox));

        #endregion

        #region GroupName

        /// <summary>
        /// Gets or sets the name of the group that the toggle button belongs to.
        /// Use the GroupName property to specify a grouping of toggle buttons to
        /// create a mutually exclusive set of controls. You can use the GroupName
        /// property when only one selection is possible from a list of available
        /// options. When this property is set, only one ToggleButton in the specified
        /// group can be selected at a time.
        /// </summary>
        public string GroupName
        {
            get { return (string)this.GetValue(GroupNameProperty); }
            set { this.SetValue(GroupNameProperty, value); }
        }

        /// <inheritdoc />
        bool? IToggleButton.IsChecked
        {
            get { return this.IsChecked; }
            set { this.IsChecked = value == true; }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(MenuItem), new PropertyMetadata(ToggleButtonHelper.OnGroupNameChanged));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when context menu is opened
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Occurs when context menu is closed
        /// </summary>
        public event EventHandler DropDownClosed;

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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        public MenuItem()
        {
            ContextMenuService.Coerce(this);

            this.MouseWheel += this.OnMenuItemMouseWheel;
        }

        // Fix to raise MouseWhele event
        private void OnMenuItemMouseWheel(object sender, MouseWheelEventArgs e)
        {
            (((MenuItem)sender).Parent as ListBox)?.RaiseEvent(e);
        }

        #endregion

        #region QuickAccess

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be synchronized with the original
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            if (this.HasItems)
            {
                if (this.IsSplited)
                {
                    var button = new SplitButton();
                    RibbonControl.BindQuickAccessItem(this, button);
                    RibbonControl.Bind(this, button, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.Default);
                    RibbonControl.Bind(this, button, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.Default);
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
                    RibbonControl.Bind(this, button, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.Default);
                    RibbonControl.Bind(this, button, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.Default);
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
                var button = new Button();
                RibbonControl.BindQuickAccessItem(this, button);
                return button;
            }
        }

        /// <summary>
        /// Handles quick access button drop down menu opened
        /// </summary>
        protected void OnQuickAccessOpened(object sender, EventArgs e)
        {
            var buttonInQuickAccess = (DropDownButton)sender;

            buttonInQuickAccess.DropDownClosed += this.OnQuickAccessMenuClosedOrUnloaded;
            buttonInQuickAccess.Unloaded += this.OnQuickAccessMenuClosedOrUnloaded;

            ItemsControlHelper.MoveItemsToDifferentControl(buttonInQuickAccess, this);
        }

        /// <summary>
        /// Handles quick access button drop down menu closed
        /// </summary>
        protected void OnQuickAccessMenuClosedOrUnloaded(object sender, EventArgs e)
        {
            var buttonInQuickAccess = (DropDownButton)sender;

            buttonInQuickAccess.DropDownClosed -= this.OnQuickAccessMenuClosedOrUnloaded;
            buttonInQuickAccess.Unloaded -= this.OnQuickAccessMenuClosedOrUnloaded;

            ItemsControlHelper.MoveItemsToDifferentControl(buttonInQuickAccess, this);
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
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

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrameworkElement;
        }

        #region Non MenuBase ItemsControl workarounds

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

            if (this.IsItemsControlMenuBase == false)
            {
                if (this.HasItems)
                {
                    this.IsSubmenuOpen = true;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (this.isContextMenuOpening)
            {
                return;
            }

            base.OnMouseLeave(e);
        }

        /// <inheritdoc />
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            this.isContextMenuOpening = true;
            this.IsContextMenuOpened = true;

            base.OnContextMenuOpening(e);
        }

        /// <inheritdoc />
        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            this.isContextMenuOpening = false;
            this.IsContextMenuOpened = false;

            base.OnContextMenuClosing(e);

            if (this.IsMouseOver == false)
            {
                this.OnMouseLeave(new MouseEventArgs(Mouse.PrimaryDevice, 0));
            }
        }

        #endregion Non MenuBase ItemsControl workarounds

        /// <summary>
        /// Called when the left mouse button is released.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (this.IsSplited)
                {
                    var buttonBorder = this.GetTemplateChild("PART_ButtonBorder") as Border;
                    if (buttonBorder != null
                        && PopupService.IsMousePhysicallyOver(buttonBorder))
                    {
                        this.OnClick();
                    }
                }
            }

            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Called when a <see cref="T:System.Windows.Controls.Button"/> is clicked.
        /// </summary>
        protected override void OnClick()
        {
            // Close popup on click
            if (this.IsDefinitive
                && (!this.HasItems || this.IsSplited))
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

        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened -= this.OnDropDownOpened;
                this.DropDownPopup.Closed -= this.OnDropDownClosed;
            }

            this.DropDownPopup = this.GetTemplateChild("PART_Popup") as Popup;

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened += this.OnDropDownOpened;
                this.DropDownPopup.Closed += this.OnDropDownClosed;

                KeyboardNavigation.SetControlTabNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetTabNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
            }

            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta -= this.OnResizeVerticalDelta;
            }

            this.resizeVerticalThumb = this.GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta += this.OnResizeVerticalDelta;
            }

            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta -= this.OnResizeBothDelta;
            }

            this.resizeBothThumb = this.GetTemplateChild("PART_ResizeBothThumb") as Thumb;
            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta += this.OnResizeBothDelta;
            }

            this.scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            this.menuPanel = this.GetTemplateChild("PART_MenuPanel") as Panel;
        }

        /// <summary>
        /// Responds to the <see cref="E:System.Windows.UIElement.KeyDown"/> event.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.KeyDown"/> event.</param>
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
                    var parent = this.FindParentDropDownOrMenuItem();
                    if (parent != null)
                    {
                        var dropDown = parent as IDropDownControl;
                        if (dropDown != null)
                        {
                            dropDown.IsDropDownOpen = false;
                        }
                        else
                        {
                            ((System.Windows.Controls.MenuItem)parent).IsSubmenuOpen = false;
                        }
                    }
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

                    if (key == Key.Right)
                    {
                        this.IsSubmenuOpen = true;
                        this.menuPanel.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                        e.Handled = true;
                    }
                    else if (key == Key.Left)
                    {
                        this.IsSubmenuOpen = false;
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

        private DependencyObject FindParentDropDownOrMenuItem()
        {
            var parent = this.Parent;
            while (parent != null)
            {
                var dropDown = parent as IDropDownControl;
                if (dropDown != null)
                {
                    return parent;
                }

                var menuItem = parent as System.Windows.Controls.MenuItem;
                if (menuItem != null)
                {
                    return parent;
                }

                parent = LogicalTreeHelper.GetParent(parent);
            }

            return null;
        }

        #endregion

        #region Methods

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.scrollViewer != null)
            {
                this.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }

            if (this.menuPanel != null)
            {
                if (double.IsNaN(this.menuPanel.Width))
                {
                    this.menuPanel.Width = this.menuPanel.ActualWidth;
                }

                if (double.IsNaN(this.menuPanel.Height))
                {
                    this.menuPanel.Height = this.menuPanel.ActualHeight;
                }

                this.menuPanel.Width = Math.Max(this.menuPanel.MinWidth, this.menuPanel.Width + e.HorizontalChange);
                this.menuPanel.Height = Math.Min(Math.Max(this.menuPanel.MinHeight, this.menuPanel.Height + e.VerticalChange), this.MaxDropDownHeight);
            }
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.scrollViewer != null)
            {
                this.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }

            if (this.menuPanel != null)
            {
                if (double.IsNaN(this.menuPanel.Height))
                {
                    this.menuPanel.Height = this.menuPanel.ActualHeight;
                }

                this.menuPanel.Height = Math.Min(Math.Max(this.menuPanel.MinHeight, this.menuPanel.Height + e.VerticalChange), this.MaxDropDownHeight);
            }
        }

        // Handles drop down opened
        private void OnDropDownClosed(object sender, EventArgs e)
        {
            this.DropDownClosed?.Invoke(this, e);
        }

        // Handles drop down closed
        private void OnDropDownOpened(object sender, EventArgs e)
        {
            if (this.scrollViewer != null
                && this.ResizeMode != ContextMenuResizeMode.None)
            {
                this.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }

            if (this.menuPanel != null)
            {
                this.menuPanel.Width = double.NaN;
                this.menuPanel.Height = double.NaN;
            }

            this.DropDownOpened?.Invoke(this, e);
        }

        #endregion
    }
}