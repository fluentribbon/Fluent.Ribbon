#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

namespace Fluent
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Threading;

    /// <summary>
    /// Represents drop down button
    /// </summary>
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_ResizeVerticalThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ResizeBothThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_MenuPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ButtonBorder", Type = typeof(UIElement))]
    public class DropDownButton : MenuBase, IQuickAccessItemProvider, IRibbonControl, IDropDownControl
    {
        #region Fields

        // Thumb to resize in both directions
        private Thumb resizeBothThumb;

        // Thumb to resize vertical
        private Thumb resizeVerticalThumb;

        private Panel menuPanel;

        private ScrollViewer scrollViewer;

        private UIElement buttonBorder;

        #endregion

        #region Properties

        #region Size

        /// <summary>
        /// Gets or sets Size for the element.
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(DropDownButton));

        #endregion

        #region SizeDefinition

        /// <summary>
        /// Gets or sets SizeDefinition for element.
        /// </summary>
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(DropDownButton));

        #endregion

        #region KeyTip

        /// <summary>
        /// Gets or sets KeyTip for element.
        /// </summary>
        public string KeyTip
        {
            get { return (string)GetValue(KeyTipProperty); }
            set { SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(DropDownButton));

        #endregion

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup { get; private set; }

        /// <summary>
        /// Gets a value indicating whether context menu is opened
        /// </summary>
        public bool IsContextMenuOpened { get; set; }

        private bool HasCapture
        {
            get
            {
                return ReferenceEquals(Mouse.Captured, this);
            }
        }

        #region Header

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        public object Header
        {
            get { return this.GetValue(HeaderProperty); }
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
            var element = (DropDownButton)d;

            var oldElement = e.OldValue as FrameworkElement;

            if (oldElement != null)
            {
                element.RemoveLogicalChild(oldElement);
            }

            var newElement = e.NewValue as FrameworkElement;

            if (newElement != null
                && LogicalTreeHelper.GetParent(newElement) == null)
            {
                element.AddLogicalChild(newElement);
            }
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
            new UIPropertyMetadata(false, OnIsDropDownOpenChanged));

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
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(DropDownButton), new UIPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0));

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
        /// Using a DependencyProperty as the backing store for InitialDropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register("DropDownHeight", typeof(double), typeof(DropDownButton), new UIPropertyMetadata(double.NaN));

        #endregion

        #region ClosePopupOnMouseDown

        /// <summary>
        /// Gets or sets whether the popup of this drop down button should automatically be closed on mouse down.
        /// </summary>
        public bool ClosePopupOnMouseDown
        {
            get { return (bool)GetValue(ClosePopupOnMouseDownProperty); }
            set { SetValue(ClosePopupOnMouseDownProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ClosePopupOnMouseDown.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ClosePopupOnMouseDownProperty =
            DependencyProperty.Register("ClosePopupOnMouseDown", typeof(bool), typeof(DropDownButton), new PropertyMetadata(false));

        #endregion

        #region ClosePopupOnMouseDownDelay

        /// <summary>
        /// Gets or sets the delay in milliseconds to close the popup on mouse down.
        /// </summary>
        public int ClosePopupOnMouseDownDelay
        {
            get { return (int)GetValue(ClosePopupOnMouseDownDelayProperty); }
            set { SetValue(ClosePopupOnMouseDownDelayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ClosePopupOnMouseDownDelay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ClosePopupOnMouseDownDelayProperty =
            DependencyProperty.Register("ClosePopupOnMouseDownDelay", typeof(int), typeof(DropDownButton), new PropertyMetadata(150));

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

            System.Windows.Controls.ToolTipService.IsEnabledProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(null, CoerceToolTipIsEnabled));

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

            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta += this.OnResizeVerticalDelta;
            }

            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta += this.OnResizeBothDelta;
            }

            if (this.buttonBorder != null)
            {
                this.buttonBorder.MouseLeftButtonDown += this.HandleButtonBorderMouseLeftButtonDown;
            }

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.KeyDown += this.OnDropDownPopupKeyDown;
                this.DropDownPopup.AddHandler(MouseDownEvent, new RoutedEventHandler(this.OnDropDownPopupMouseDown), true);
            }
        }

        private void UnSubscribeEvents()
        {
            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta -= this.OnResizeVerticalDelta;
            }

            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta -= this.OnResizeBothDelta;
            }

            if (this.buttonBorder != null)
            {
                this.buttonBorder.MouseLeftButtonDown -= this.HandleButtonBorderMouseLeftButtonDown;
            }

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.KeyDown -= this.OnDropDownPopupKeyDown;
                this.DropDownPopup.RemoveHandler(MouseDownEvent, new RoutedEventHandler(this.OnDropDownPopupMouseDown));
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.UnSubscribeEvents();

            this.DropDownPopup = this.Template.FindName("PART_Popup", this) as Popup;

            if (this.DropDownPopup != null)
            {
                KeyboardNavigation.SetDirectionalNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetTabNavigation(this.DropDownPopup, KeyboardNavigationMode.Continue);
            }

            this.resizeVerticalThumb = this.Template.FindName("PART_ResizeVerticalThumb", this) as Thumb;

            this.resizeBothThumb = this.Template.FindName("PART_ResizeBothThumb", this) as Thumb;

            this.menuPanel = this.Template.FindName("PART_MenuPanel", this) as Panel;

            this.scrollViewer = this.Template.FindName("PART_ScrollViewer", this) as ScrollViewer;

            this.buttonBorder = this.Template.FindName("PART_ButtonBorder", this) as UIElement;

            base.OnApplyTemplate();

            this.SubscribeEvents();
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

        private void OnDropDownPopupKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            var handled = false;

            switch (e.Key)
            {
                case Key.Escape:
                    this.IsDropDownOpen = false;
                    handled = true;
                    break;
            }

            if (handled)
            {
                e.Handled = true;
            }
        }

        private void OnDropDownPopupMouseDown(object sender, RoutedEventArgs e)
        {
            if (this.ClosePopupOnMouseDown
                && this.resizeBothThumb.IsMouseOver == false
                && this.resizeVerticalThumb.IsMouseOver == false)
            {
                e.Handled = false;

                // Note: get outside thread to prevent exceptions (it's a dependency property after all)
                var closePopupOnMouseDownDelay = this.ClosePopupOnMouseDownDelay;

                // Ugly workaround, but use a timer to allow routed event to continue
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(closePopupOnMouseDownDelay);

                    this.Dispatcher.BeginInvoke(new Action(() => this.IsDropDownOpen = false));
                });
            }
        }

        private void HandleButtonBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            this.Focus();
            this.IsDropDownOpen = !this.IsDropDownOpen;
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.KeyDown"/> routed event that occurs when the user presses a key.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.KeyDown"/> event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            var handled = false;

            switch (e.Key)
            {
                case Key.Down:
                    if (this.HasItems
                        && this.IsDropDownOpen == false) // Only handle this for initial navigation. Further navigation is handled by the dropdown itself
                    {
                        this.IsDropDownOpen = true;

                        var container = ItemContainerGenerator.ContainerFromIndex(0);

                        NavigateToContainer(container);

                        handled = true;
                    }
                    break;

                case Key.Up:
                    if (this.HasItems
                        && this.IsDropDownOpen == false) // Only handle this for initial navigation. Further navigation is handled by the dropdown itself
                    {
                        this.IsDropDownOpen = true;

                        var container = ItemContainerGenerator.ContainerFromIndex(this.Items.Count - 1);

                        NavigateToContainer(container);

                        handled = true;
                    }
                    break;

                case Key.Escape:
                    if (this.IsDropDownOpen)
                    {
                        this.IsDropDownOpen = false;
                        handled = true;
                    }
                    break;

                case Key.Enter:
                case Key.Space:
                    this.IsDropDownOpen = !this.IsDropDownOpen;
                    handled = true;
                    break;
            }

            if (handled)
            {
                e.Handled = true;
            }
        }

        private static void NavigateToContainer(DependencyObject container)
        {
            var element = container as FrameworkElement;

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
                var predicted = element.PredictFocus(FocusNavigationDirection.Down);

                if (predicted is MenuBase == false)
                {
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
            }
        }

        private static object CoerceToolTipIsEnabled(DependencyObject d, object basevalue)
        {
            var control = (DropDownButton)d;

            return !control.IsDropDownOpen;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public virtual void OnKeyTipPressed()
        {
            this.IsDropDownOpen = true;

            if (this.DropDownPopup != null
                && this.DropDownPopup.Child != null)
            {
                Keyboard.Focus(this.DropDownPopup.Child);
                this.DropDownPopup.Child.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
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

        #region Private methods

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.scrollViewer == null)
            {
                return;
            }

            if (double.IsNaN(this.scrollViewer.Width))
            {
                this.scrollViewer.Width = this.scrollViewer.ActualWidth;
            }

            if (double.IsNaN(this.scrollViewer.Height))
            {
                this.scrollViewer.Height = this.scrollViewer.ActualHeight;
            }

            this.scrollViewer.Width = Math.Max(this.ActualWidth, this.scrollViewer.Width + e.HorizontalChange);
            this.scrollViewer.Height = Math.Min(Math.Max(this.ActualHeight, this.scrollViewer.Height + e.VerticalChange), this.MaxDropDownHeight);
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.scrollViewer == null)
            {
                return;
            }

            if (double.IsNaN(this.scrollViewer.Height))
            {
                this.scrollViewer.Height = this.scrollViewer.ActualHeight;
            }

            this.scrollViewer.Height = Math.Min(Math.Max(this.ActualHeight, this.scrollViewer.Height + e.VerticalChange), this.MaxDropDownHeight);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DropDownButton)d;

            var newValue = (bool)e.NewValue;

            control.SetValue(System.Windows.Controls.ToolTipService.IsEnabledProperty, !newValue);

            Debug.WriteLine(string.Format("{0} IsDropDownOpen: {1}", control.Header, newValue));

            if (newValue)
            {
                Mouse.Capture(control, CaptureMode.SubTree);

                Keyboard.Focus(control.DropDownPopup);

                control.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object arg)
                    {
                        var ctrl = (DropDownButton)arg;

                        var container = ctrl.ItemContainerGenerator.ContainerFromIndex(0);

                        NavigateToContainer(container);

                        // Edge case: Whole dropdown content is disabled
                        if (ctrl.IsKeyboardFocusWithin == false)
                        {
                            Keyboard.Focus(ctrl.DropDownPopup);
                        }

                        return null;
                    },
                    control);

                control.OnDropDownOpened();
            }
            else
            {
                // If focus is within the subtree, make sure we have the focus so that focus isn't in the disposed hwnd 
                if (control.IsKeyboardFocusWithin)
                {
                    // make sure the control has focus 
                    control.Focus();
                }

                Mouse.Capture(null);

                control.OnDropDownClosed();
            }
        }

        // Handles drop down closed
        private void OnDropDownClosed()
        {
            if (this.DropDownClosed != null)
            {
                this.DropDownClosed(this, EventArgs.Empty);
            }
        }

        // Handles drop down opened
        private void OnDropDownOpened()
        {
            if (this.DropDownOpened != null)
            {
                this.DropDownOpened(this, EventArgs.Empty);
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
            var button = new DropDownButton
                {
                    Size = RibbonControlSize.Small
                };

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
            var button = (DropDownButton)sender;

            if (this.ItemsSource != null)
            {
                button.ItemsSource = this.ItemsSource;
                this.ItemsSource = null;
            }
            else
            {
                for (var i = 0; i < this.Items.Count; i++)
                {
                    var item = this.Items[0];
                    this.Items.Remove(item);
                    button.Items.Add(item);
                    i--;
                }
            }

            button.DropDownClosed += this.OnQuickAccessMenuClosed;
        }

        /// <summary>
        /// Handles quick access button drop down menu closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            var button = (DropDownButton)sender;
            button.DropDownClosed -= this.OnQuickAccessMenuClosed;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)(() =>
                                                                               {
                                                                                   if (button.ItemsSource != null)
                                                                                   {
                                                                                       this.ItemsSource = button.ItemsSource;
                                                                                       button.ItemsSource = null;
                                                                                   }
                                                                                   else
                                                                                   {
                                                                                       for (var i = 0; i < button.Items.Count; i++)
                                                                                       {
                                                                                           var item = button.Items[0];
                                                                                           button.Items.Remove(item);
                                                                                           this.Items.Add(item);
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

        /// <summary>
        /// Gets an enumerator for the logical child objects of the <see cref="T:System.Windows.Controls.ItemsControl"/> object.
        /// </summary>
        /// <returns>
        /// An enumerator for the logical child objects of the <see cref="T:System.Windows.Controls.ItemsControl"/> object. The default is null.
        /// </returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Icon != null)
                {
                    yield return this.Icon;
                }

                foreach (var item in this.Items)
                {
                    yield return item;
                }
            }
        }
    }
}