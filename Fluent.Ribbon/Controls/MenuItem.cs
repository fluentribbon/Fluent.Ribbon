using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents menu item
    /// </summary>
    [ContentProperty(nameof(Items))]
    public class MenuItem : System.Windows.Controls.MenuItem, IQuickAccessItemProvider, IRibbonControl
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
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(MenuItem), new PropertyMetadata(StringBoxes.Empty));

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

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(MenuItem),
            new PropertyMetadata(OnGroupNameChanged));

        // Group name changed
        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleButton = (MenuItem)d;
            var currentGroupName = (string)e.NewValue;
            var previousGroupName = (string)e.OldValue;

            if (previousGroupName != null) RemoveFromGroup(previousGroupName, toggleButton);
            if (currentGroupName != null) AddToGroup(currentGroupName, toggleButton);
        }

        #region Grouped Items Methods

        // Grouped buttons (thread id / group name / weak ref to a control)
        private static readonly Dictionary<int, Dictionary<string, List<WeakReference>>> groupedButtons =
            new Dictionary<int, Dictionary<string, List<WeakReference>>>();

        // Remove from group
        private static void RemoveFromGroup(string groupName, MenuItem button)
        {
            List<WeakReference> buttons;
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!groupedButtons.ContainsKey(threadId)) return;
            if (!groupedButtons[threadId].TryGetValue(groupName, out buttons)) return;

            buttons.RemoveAt(buttons.FindIndex(x => x.IsAlive && ReferenceEquals(x.Target, button)));
        }

        // Remove from group
        private static void AddToGroup(string groupName, MenuItem button)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!groupedButtons.ContainsKey(threadId)) groupedButtons.Add(threadId, new Dictionary<string, List<WeakReference>>());

            List<WeakReference> buttons;
            if (!groupedButtons[threadId].TryGetValue(groupName, out buttons))
            {
                buttons = new List<WeakReference>();
                groupedButtons[threadId].Add(groupName, buttons);
            }

            buttons.Add(new WeakReference(button));
        }

        // Gets all buttons in the given group
        private static IEnumerable<MenuItem> GetItemsInGroup(string groupName)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!groupedButtons.ContainsKey(threadId)) return new List<MenuItem>();

            List<WeakReference> buttons;
            if (!groupedButtons[threadId].TryGetValue(groupName, out buttons)) return new List<MenuItem>();
            return buttons.Where(x => x.IsAlive).Select(x => (MenuItem)x.Target);
        }

        #endregion

        #region IsChecked

        // Coerce IsChecked
        private static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            var toggleButton = (MenuItem)d;
            if (toggleButton.GroupName == null)
            {
                return basevalue;
            }

            var baseIsChecked = (bool)basevalue;
            if (!baseIsChecked)
            {
                // We can not allow that there are no one button checked
                foreach (var item in GetItemsInGroup(toggleButton.GroupName))
                {
                    // It's Ok, atleast one checked button exists
                    if (item.IsChecked)
                    {
                        return false;
                    }
                }

                // This button can not be unchecked
                return true;
            }
            return basevalue;
        }

        // Handles isChecked changed
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool)e.NewValue;
            var button = (MenuItem)d;

            // Uncheck other toggle buttons
            if (newValue
                && button.GroupName != null)
            {
                foreach (var item in GetItemsInGroup(button.GroupName))
                {
                    if (ReferenceEquals(item, button) == false)
                    {
                        item.IsChecked = false;
                    }
                }
            }
        }

    #endregion

    #endregion

        #region DismissPopup

        /// <summary>
        /// Useless property only used in secon level application menu items
        /// </summary>
        public bool DismissPopupOnClick
        {
          get { return (bool)this.GetValue(DismissPopupOnClickProperty); }
          set { this.SetValue(DismissPopupOnClickProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DismissPopupOnClickProperty =
            DependencyProperty.Register(nameof(DismissPopupOnClick), typeof(bool), typeof(MenuItem), new UIPropertyMetadata(true));


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
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static MenuItem()
        {
            var type = typeof(MenuItem);
            ToolTipService.Attach(type);
            //PopupService.Attach(type);            
            ContextMenuService.Attach(type);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            IsCheckedProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnIsCheckedChanged, CoerceIsChecked));
        }

        /// <summary>
        /// Default Constructor
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
        public FrameworkElement CreateQuickAccessItem()
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
                    RibbonControl.Bind(this, button, nameof(this.ItemsSource), ItemsSourceProperty, BindingMode.OneWay);
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnQuickAccessOpened(object sender, EventArgs e)
        {
            var buttonInQuickAccess = (DropDownButton)sender;

            buttonInQuickAccess.DropDownClosed += this.OnQuickAccessMenuClosedOrUnloaded;
            buttonInQuickAccess.Unloaded += this.OnQuickAccessMenuClosedOrUnloaded;

            ItemsControlHelper.MoveItemsToDifferentControl(this,buttonInQuickAccess);
        }

        /// <summary>
        /// Handles quick access button drop down menu closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #endregion

        #region Public

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public virtual void OnKeyTipPressed()
        {
            if (!this.HasItems)
            {
                this.OnClick();
            }
            else
            {
                Keyboard.Focus(this);
                this.IsDropDownOpen = true;
            }
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
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
                    if ((buttonBorder != null) && PopupService.IsMousePhysicallyOver(buttonBorder))
                    {
                        /*if (Command != null)
                        {
                            RoutedCommand command = Command as RoutedCommand;
                            if (command != null) command.Execute(CommandParameter, CommandTarget);
                            else Command.Execute(CommandParameter);
                        }*/
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
                if(DismissPopupOnClick)
                  PopupService.RaiseDismissPopupEventAsync(this, DismissPopupMode.Always);
            }

            base.OnClick();
            if (!DismissPopupOnClick)
              this.Focus();
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
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Debug.WriteLine("MenuItem focus lost - " + this);
            //base.OnPreviewLostKeyboardFocus(e);
            //e.Handled = true;
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
                base.OnKeyDown(e);
            }
        }

        private DependencyObject FindParentDropDownOrMenuItem()
        {
            var parent = this.Parent;
            var pp = System.Windows.Media.VisualTreeHelper.GetParent(this);

            if(parent==null)
              parent= System.Windows.Media.VisualTreeHelper.GetParent(this);
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
                if(parent is Popup)
                {
                  Popup p = (Popup)parent;
                  if (p.TemplatedParent != null)
                    parent = p.TemplatedParent;
                }
                else
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