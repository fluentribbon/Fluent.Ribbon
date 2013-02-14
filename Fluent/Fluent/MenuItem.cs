#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

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

namespace Fluent
{
    /// <summary>
    /// Represents menu item
    /// </summary>
    [ContentProperty("Items")]
    public class MenuItem : System.Windows.Controls.MenuItem, IQuickAccessItemProvider, IRibbonControl
    {
        #region Fields

        private Popup popup;

        // Thumb to resize in both directions
        Thumb resizeBothThumb;
        // Thumb to resize vertical
        Thumb resizeVerticalThumb;

        private MenuPanel menuPanel;

        private ScrollViewer scrollViewer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup
        {
            get { return popup; }
        }

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
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MenuItem), new UIPropertyMetadata(""));


        #endregion

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonControl.SizeProperty.AddOwner(typeof(MenuItem));

        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        #endregion

        #region SizeDefinition Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonControl.SizeDefinitionProperty.AddOwner(typeof(MenuItem));

        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        public string SizeDefinition
        {
            get { return (string)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        #endregion

        #region IsDropDownOpen

        /// <summary>
        /// Gets or sets whether popup is opened
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return IsSubmenuOpen; }
            set { IsSubmenuOpen = value; }
        }

        #endregion

        #region IsDefinitive

        /// <summary>
        /// Gets or sets whether ribbon control click must close backstage
        /// </summary>
        public bool IsDefinitive
        {
            get { return (bool)GetValue(IsDefinitiveProperty); }
            set { SetValue(IsDefinitiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDefinitive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDefinitiveProperty =
            DependencyProperty.Register("IsDefinitive", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(true));

        #endregion

        #region ResizeMode

        /// <summary>
        /// Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode ResizeMode
        {
            get { return (ContextMenuResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeMode.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode),
            typeof(MenuItem), new UIPropertyMetadata(ContextMenuResizeMode.None));


        #endregion

        #region MaxDropDownHeight

        /// <summary>
        /// Get or sets max height of drop down popup
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(MenuItem), new UIPropertyMetadata(100.0));

        #endregion

        #region IsSplited

        /// <summary>
        /// Gets or sets a value indicating whether menu item is splited
        /// </summary>
        public bool IsSplited
        {
            get { return (bool)GetValue(IsSplitedProperty); }
            set { SetValue(IsSplitedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSplited.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSplitedProperty =
            DependencyProperty.Register("IsSplited", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(false));

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
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(MenuItem),
            new UIPropertyMetadata(null, OnGroupNameChanged));

        // Group name changed
        static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuItem toggleButton = (MenuItem)d;
            string currentGroupName = (string)e.NewValue;
            string previousGroupName = (string)e.OldValue;

            if (previousGroupName != null) RemoveFromGroup(previousGroupName, toggleButton);
            if (currentGroupName != null) AddToGroup(currentGroupName, toggleButton);
        }

        #region Grouped Items Methods

        // Grouped buttons (thread id / group name / weak ref to a control)
        static readonly Dictionary<int, Dictionary<string, List<WeakReference>>> groupedButtons =
            new Dictionary<int, Dictionary<string, List<WeakReference>>>();

        // Remove from group
        static void RemoveFromGroup(string groupName, MenuItem button)
        {
            List<WeakReference> buttons = null;
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!groupedButtons.ContainsKey(threadId)) return;
            if (!groupedButtons[threadId].TryGetValue(groupName, out buttons)) return;

            buttons.RemoveAt(buttons.FindIndex(x => (x.IsAlive && ((MenuItem)x.Target) == button)));
        }

        // Remove from group
        static void AddToGroup(string groupName, MenuItem button)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!groupedButtons.ContainsKey(threadId)) groupedButtons.Add(threadId, new Dictionary<string, List<WeakReference>>());

            List<WeakReference> buttons = null;
            if (!groupedButtons[threadId].TryGetValue(groupName, out buttons))
            {
                buttons = new List<WeakReference>();
                groupedButtons[threadId].Add(groupName, buttons);
            }

            buttons.Add(new WeakReference(button));
        }

        // Gets all buttons in the given group
        static IEnumerable<MenuItem> GetItemsInGroup(string groupName)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!groupedButtons.ContainsKey(threadId)) return new List<MenuItem>();

            List<WeakReference> buttons = null;
            if (!groupedButtons[threadId].TryGetValue(groupName, out buttons)) return new List<MenuItem>();
            return buttons.Where(x => x.IsAlive).Select(x => (MenuItem)x.Target);
        }

        #endregion

        #region IsChecked

        // Coerce IsChecked
        static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            MenuItem toggleButton = (MenuItem)d;
            if (toggleButton.GroupName == null) return basevalue;

            bool baseIsChecked = (bool)basevalue;
            if (!baseIsChecked)
            {
                // We can not allow that there are no one button checked
                foreach (MenuItem item in GetItemsInGroup(toggleButton.GroupName))
                {
                    // It's Ok, atleast one checked button exists
                    if (item.IsChecked) return false;
                }

                // This button can not be unchecked
                return true;
            }
            return basevalue;
        }

        // Handles isChecked changed
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;
            MenuItem button = (MenuItem)d;

            // Uncheck other toggle buttons
            if (newValue && button.GroupName != null)
            {
                foreach (MenuItem item in GetItemsInGroup(button.GroupName))
                    if (item != button) item.IsChecked = false;
            }
        }

        #endregion

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
            Type type = typeof(MenuItem);
            ToolTipService.Attach(type);
            //PopupService.Attach(type);            
            ContextMenuService.Attach(type);
            StyleProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            IsCheckedProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, OnIsCheckedChanged, CoerceIsChecked));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(MenuItem));
            }

            return basevalue;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MenuItem()
        {
            ContextMenuService.Coerce(this);
            ToolTip = new ToolTip();
            (ToolTip as ToolTip).Template = null;
            FocusManager.SetIsFocusScope(this, true);
            this.MouseWheel += OnMenuItemMouseWheel;
        }

        // Fix to raise MouseWhele event
        private void OnMenuItemMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (((MenuItem)sender).Parent as ListBox != null)
            {
                ((ListBox)((MenuItem)sender).Parent).RaiseEvent(e);
            }
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
            if (HasItems)
            {
                if (IsSplited)
                {
                    SplitButton button = new SplitButton();
                    RibbonControl.BindQuickAccessItem(this, button);
                    RibbonControl.Bind(this, button, "ResizeMode", ResizeModeProperty, BindingMode.Default);
                    RibbonControl.Bind(this, button, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.Default);
                    RibbonControl.Bind(this, button, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "GroupStyleSelector", GroupStyleSelectorProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "ItemsPanel", ItemsPanelProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "ItemStringFormat", ItemStringFormatProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "ItemTemplate", ItemTemplateProperty, BindingMode.OneWay);
                    button.DropDownOpened += OnQuickAccessOpened;
                    return button;
                }
                else
                {
                    DropDownButton button = new DropDownButton();
                    RibbonControl.BindQuickAccessItem(this, button);
                    RibbonControl.Bind(this, button, "ResizeMode", ResizeModeProperty, BindingMode.Default);
                    RibbonControl.Bind(this, button, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.Default);
                    RibbonControl.Bind(this, button, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "GroupStyleSelector", GroupStyleSelectorProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "ItemsPanel", ItemsPanelProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "ItemStringFormat", ItemStringFormatProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, button, "ItemTemplate", ItemTemplateProperty, BindingMode.OneWay);
                    button.DropDownOpened += OnQuickAccessOpened;
                    return button;
                }
            }
            else
            {
                Button button = new Button();
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
            DropDownButton button = (DropDownButton)sender;
            if (ItemsSource != null)
            {
                button.ItemsSource = ItemsSource;
                ItemsSource = null;
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    object item = Items[0];
                    Items.Remove(item);
                    button.Items.Add(item);
                    i--;
                }
            }
            button.DropDownClosed += OnQuickAccessMenuClosed;
        }

        /// <summary>
        /// Handles quick access button drop down menu closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            DropDownButton button = (DropDownButton)sender;
            button.DropDownClosed -= OnQuickAccessMenuClosed;
            if (button.ItemsSource != null)
            {
                ItemsSource = button.ItemsSource;
                button.ItemsSource = null;
            }
            else
            {
                for (int i = 0; i < button.Items.Count; i++)
                {
                    object item = button.Items[0];
                    button.Items.Remove(item);
                    Items.Add(item);
                    i--;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)GetValue(CanAddToQuickAccessToolBarProperty); }
            set { SetValue(CanAddToQuickAccessToolBarProperty, value); }
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
            if (!HasItems)
            {
                OnClick();
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
            return (item is FrameworkElement);
        }

        /// <summary>
        /// Called when the left mouse button is released. 
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (IsSplited)
                {
                    Border buttonBorder = GetTemplateChild("PART_ButtonBorder") as Border;
                    if ((buttonBorder != null) && (PopupService.IsMousePhysicallyOver(buttonBorder)))
                    {
                        /*if (Command != null)
                        {
                            RoutedCommand command = Command as RoutedCommand;
                            if (command != null) command.Execute(CommandParameter, CommandTarget);
                            else Command.Execute(CommandParameter);
                        }*/
                        OnClick();
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
            //if ((IsDefinitive)&&(!HasItems)) PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.Always);
            /*if (HasItems && IsSplited)
            {
                Border buttonBorder = GetTemplateChild("PART_ButtonBorder") as Border;
                if ((buttonBorder != null) && (buttonBorder.IsMouseOver))
                {
                    if (IsDefinitive) PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.Always);
                }
            }
            else */
            if ((IsDefinitive) && (!HasItems || IsSplited)) PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.Always);
            base.OnClick();
        }

        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (popup != null)
            {
                popup.Opened -= OnDropDownOpened;
                popup.Closed -= OnDropDownClosed;
            }

            popup = GetTemplateChild("PART_Popup") as Popup;

            if (popup != null)
            {
                popup.Opened += OnDropDownOpened;
                popup.Closed += OnDropDownClosed;

                KeyboardNavigation.SetControlTabNavigation(popup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(popup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetTabNavigation(popup, KeyboardNavigationMode.Cycle);
            }

            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta -= OnResizeVerticalDelta;
            }
            resizeVerticalThumb = GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta += OnResizeVerticalDelta;
            }

            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta -= OnResizeBothDelta;
            }
            resizeBothThumb = GetTemplateChild("PART_ResizeBothThumb") as Thumb;
            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta += OnResizeBothDelta;
            }
            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            menuPanel = GetTemplateChild("PART_MenuPanel") as MenuPanel;
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
                if (IsSubmenuOpen) IsSubmenuOpen = false;
                else
                {
                    DependencyObject parent = FindParentDropDownOrMenuItem();
                    if (parent != null)
                    {
                        IDropDownControl dropDown = parent as IDropDownControl;
                        if (dropDown != null) dropDown.IsDropDownOpen = false;
                        else (parent as System.Windows.Controls.MenuItem).IsSubmenuOpen = false;
                    }
                }
                e.Handled = true;
            }
            else base.OnKeyDown(e);
        }

        private DependencyObject FindParentDropDownOrMenuItem()
        {
            DependencyObject parent = Parent;
            while (parent != null)
            {
                IDropDownControl dropDown = parent as IDropDownControl;
                if (dropDown != null) return parent;
                System.Windows.Controls.MenuItem menuItem = parent as System.Windows.Controls.MenuItem;
                if (menuItem != null) return parent;
                parent = LogicalTreeHelper.GetParent(parent);
            }
            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles size property changing
        /// </summary>
        /// <param name="previous">Previous value</param>
        /// <param name="current">Current value</param>
        protected virtual void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
        }

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (scrollViewer != null) scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            if (double.IsNaN(menuPanel.Width)) menuPanel.Width = menuPanel.ActualWidth;
            if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
            menuPanel.Width = Math.Max(menuPanel.ResizeMinWidth, menuPanel.Width + e.HorizontalChange);
            menuPanel.Height = Math.Min(Math.Max(menuPanel.ResizeMinHeight, menuPanel.Height + e.VerticalChange), MaxDropDownHeight);
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (scrollViewer != null) scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
            menuPanel.Height = Math.Min(Math.Max(menuPanel.ResizeMinHeight, menuPanel.Height + e.VerticalChange), MaxDropDownHeight);
        }

        // Handles drop down opened
        void OnDropDownClosed(object sender, EventArgs e)
        {
            if (DropDownClosed != null) DropDownClosed(this, e);
            //if (Mouse.Captured == this) Mouse.Capture(null);
        }

        // Handles drop down closed
        void OnDropDownOpened(object sender, EventArgs e)
        {
            if (scrollViewer != null && ResizeMode != ContextMenuResizeMode.None) scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            menuPanel.Width = double.NaN;
            menuPanel.Height = double.NaN;

            if (DropDownOpened != null) DropDownOpened(this, e);
            //Mouse.Capture(this, CaptureMode.SubTree);
        }

        #endregion
    }
}
