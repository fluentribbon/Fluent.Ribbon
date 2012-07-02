#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents drop down button
    /// </summary>
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class DropDownButton : MenuBase, IQuickAccessItemProvider, IRibbonControl, IDropDownControl
    {
        #region Fields

        // Thumb to resize in both directions
        private Thumb resizeBothThumb;

        // Thumb to resize vertical
        private Thumb resizeVerticalThumb;

        private Border buttonBorder;

        private MenuPanel menuPanel;

        private ScrollViewer scrollViewer;

        private bool isFirstTime;

        #endregion

        #region Properties

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup { get; private set; }

        /// <summary>
        /// Gets a value indicating whether context menu is opened
        /// </summary>
        public bool IsContextMenuOpened { get; set; }

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonControl.SizeProperty.AddOwner(typeof(DropDownButton));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonControl.AttachSizeDefinition(typeof(DropDownButton));

        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        public string SizeDefinition
        {
            get { return (string)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        public object Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(DropDownButton));

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(DropDownButton), new UIPropertyMetadata(null, OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton element = d as DropDownButton;
            FrameworkElement oldElement = e.OldValue as FrameworkElement;
            if (oldElement != null) element.RemoveLogicalChild(oldElement);
            FrameworkElement newElement = e.NewValue as FrameworkElement;
            if (newElement != null) element.AddLogicalChild(newElement);
        }

        #endregion

        #region LargeIcon

        /// <summary>
        /// Gets or sets button large icon
        /// </summary>
        public object LargeIcon
        {
            get { return GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(object),
            typeof(DropDownButton), new UIPropertyMetadata(null));

        #endregion

        #region HasTriangle

        /// <summary>
        /// Gets or sets whether button has triangle
        /// </summary>
        public bool HasTriangle
        {
            get { return (bool)GetValue(HasTriangleProperty); }
            set { SetValue(HasTriangleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasTriangle. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasTriangleProperty =
            DependencyProperty.Register(
                "HasTriangle", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(true));

        #endregion

        #region IsDropDownOpen

        /// <summary>
        /// Gets or sets whether popup is opened
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownButton),
            new UIPropertyMetadata(false));

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
            typeof(DropDownButton), new UIPropertyMetadata(ContextMenuResizeMode.None));


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
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(DropDownButton), new UIPropertyMetadata(350.0));

        #endregion

        #region DropDownHeight

        /// <summary>
        /// Gets or sets initial dropdown height
        /// </summary>
        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>
        /// /Using a DependencyProperty as the backing store for InitialDropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register("DropDownHeight", typeof(double), typeof(DropDownButton), new UIPropertyMetadata(double.NaN));

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

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static DropDownButton()
        {
            var type = typeof(DropDownButton);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));

            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));

            ToolTipService.Attach(type);
            PopupService.Attach(type);
            ContextMenuService.Attach(type);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DropDownButton()
        {
            ContextMenuService.Coerce(this);

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeEvents();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            // Always unsubscribe events to ensure we don't subscribe twice
            this.UnSubscribeEvents();

            if (DropDownPopup != null)
            {
                DropDownPopup.Opened += OnDropDownOpened;
                DropDownPopup.Closed += OnDropDownClosed;
            }

            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta += OnResizeVerticalDelta;
            }

            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta += OnResizeBothDelta;
            }
        }

        private void UnSubscribeEvents()
        {
            if (DropDownPopup != null)
            {
                DropDownPopup.Opened -= OnDropDownOpened;
                DropDownPopup.Closed -= OnDropDownClosed;
            }

            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta -= OnResizeVerticalDelta;
            }

            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta -= OnResizeBothDelta;
            }
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
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to add 
        /// class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!buttonBorder.IsMouseOver)
            {
                return;
            }

            if (!IsDropDownOpen)
            {
                if (isFirstTime)
                {
                    DropDownPopup.Opacity = 0;
                }

                if (menuPanel != null)
                {
                    if (scrollViewer != null/* && ResizeMode != ContextMenuResizeMode.None*/)
                    {
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    }

                    menuPanel.Width = double.NaN;
                    menuPanel.Height = double.NaN;// Math.Min(menuPanel.MinHeight, MaxDropDownHeight);                        
                    menuPanel.Loaded += OnMenuPanelLoaded;
                }

                if (!isFirstTime)
                {
                    IsDropDownOpen = true;
                }
            }
            else
            {
                PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.MouseNotOver);
                IsDropDownOpen = false;
            }

            e.Handled = true;

            if (isFirstTime)
            {
                isFirstTime = false;
                //IsDropDownOpen = false;
                Dispatcher.Invoke(DispatcherPriority.Send, (ThreadStart)(() =>
                                                                             {
                                                                                 if (menuPanel != null)
                                                                                 {
                                                                                     if (scrollViewer != null/* && ResizeMode != ContextMenuResizeMode.None*/)
                                                                                         scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                                                                                     menuPanel.Width = double.NaN;
                                                                                     menuPanel.Height = double.NaN;
                                                                                     menuPanel.Loaded += OnMenuPanelLoaded;
                                                                                 }
                                                                                 IsDropDownOpen = true;
                                                                                 OnMenuPanelLoaded(null, null);
                                                                                 DropDownPopup.Opacity = 1;
                                                                             }));
            }
        }

        private void OnMenuPanelLoaded(object sender, RoutedEventArgs e)
        {
            menuPanel.Loaded -= OnMenuPanelLoaded;
            if (ResizeMode != ContextMenuResizeMode.None)
            {
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
                {
                    if (double.IsNaN(menuPanel.Width)) menuPanel.Width = menuPanel.ActualWidth;
                    if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
                    menuPanel.Width = Math.Max(menuPanel.ResizeMinWidth, menuPanel.Width);
                    menuPanel.Height = Math.Min(Math.Max(menuPanel.ResizeMinHeight, menuPanel.Height), Math.Min(DropDownHeight, MaxDropDownHeight));
                    if (scrollViewer != null) scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }));
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.KeyDown"/> routed event that occurs when the user presses a key.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.KeyDown"/> event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            var handled = false;

            switch (e.Key)
            {
                case Key.Down:
                    if (HasItems)
                    {
                        this.IsDropDownOpen = true;

                        var container = ItemContainerGenerator.ContainerFromIndex(0);

                        NavigateToContainer(container);

                        handled = true;
                    }
                    break;

                case Key.Up:
                    if (HasItems)
                    {
                        this.IsDropDownOpen = true;

                        var container = ItemContainerGenerator.ContainerFromIndex(Items.Count - 1);

                        NavigateToContainer(container);

                        handled = true;
                    }
                    break;

                case Key.Escape:
                    IsDropDownOpen = false;
                    handled = true;
                    Mouse.Capture(this);
                    break;

                case Key.Enter:
                case Key.Space:
                    this.IsDropDownOpen = !this.IsDropDownOpen;

                    if (this.IsDropDownOpen == false)
                    {
                        Mouse.Capture(this);
                    }

                    handled = true;
                    break;

                default:
                    handled = false;
                    break;
            }

            if (handled)
            {
                e.Handled = true;
            }
        }

        private static void NavigateToContainer(DependencyObject container)
        {
            var element = container as UIElement;

            if (element == null)
            {
                return;
            }

            if (element.Focusable)
            {
                Keyboard.Focus(element);
            }
            else
            {
                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.UnSubscribeEvents();

            isFirstTime = true;

            DropDownPopup = GetTemplateChild("PART_Popup") as Popup;

            if (DropDownPopup != null)
            {
                KeyboardNavigation.SetControlTabNavigation(DropDownPopup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(DropDownPopup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetTabNavigation(DropDownPopup, KeyboardNavigationMode.Cycle);
            }

            resizeVerticalThumb = GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;

            resizeBothThumb = GetTemplateChild("PART_ResizeBothThumb") as Thumb;

            buttonBorder = GetTemplateChild("PART_ButtonBorder") as Border;

            menuPanel = GetTemplateChild("PART_MenuPanel") as MenuPanel;

            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            base.OnApplyTemplate();

            this.SubscribeEvents();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public virtual void OnKeyTipPressed()
        {
            IsDropDownOpen = true;
        }

        #endregion

        #region Private methods

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
        private void OnDropDownClosed(object sender, EventArgs e)
        {
            if (DropDownClosed != null)
            {
                DropDownClosed(this, e);
            }
        }

        // Handles drop down closed
        private void OnDropDownOpened(object sender, EventArgs e)
        {
            Mouse.Capture(this.DropDownPopup, CaptureMode.SubTree);
            this.DropDownPopup.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            if (DropDownOpened != null)
            {
                DropDownOpened(this, e);
            }
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be synchronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            DropDownButton button = new DropDownButton();
            button.Size = RibbonControlSize.Small;
            BindQuickAccessItem(button);
            RibbonControl.Bind(this, button, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, "GroupStyleSelector", GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, "ItemsPanel", ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, "ItemStringFormat", ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, "ItemTemplate", ItemTemplateProperty, BindingMode.OneWay);

            RibbonControl.Bind(this, button, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.OneWay);

            BindQuickAccessItemDropDownEvents(button);

            button.DropDownOpened += OnQuickAccessOpened;
            return button;
        }

        /// <summary>
        /// Handles quick access button drop down menu opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnQuickAccessOpened(object sender, EventArgs e)
        {
            DropDownButton button = (DropDownButton)sender;
            /* Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)(() =>
                                                                                   {*/
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
            //  }));
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
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)(() =>
                                                                               {
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
                                                                               }));
        }

        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected virtual void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.BindQuickAccessItem(this, element);
            RibbonControl.Bind(this, element, "ResizeMode", ResizeModeProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, "HasTriangle", HasTriangleProperty, BindingMode.Default);
        }

        /// <summary>
        /// Binds the DropDownClosed and DropDownOpened events to the created quick access item
        /// </summary>
        /// <param name="button">Toolbar item</param>
        protected void BindQuickAccessItemDropDownEvents(DropDownButton button)
        {
            if (this.DropDownClosed != null) button.DropDownClosed += this.DropDownClosed;
            if (this.DropDownOpened != null) button.DropDownOpened += this.DropDownOpened;
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
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(DropDownButton), new UIPropertyMetadata(true, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        #endregion
    }
}