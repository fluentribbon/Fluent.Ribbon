﻿// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents drop down button
    /// </summary>
    [ContentProperty(nameof(Items))]
    [TemplatePart(Name = "PART_ResizeVerticalThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ResizeBothThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ButtonBorder", Type = typeof(UIElement))]
    public class DropDownButton : ItemsControl, IQuickAccessItemProvider, IRibbonControl, IDropDownControl, ILargeIconProvider
    {
        #region Fields

        // Thumb to resize in both directions
        private Thumb resizeBothThumb;

        // Thumb to resize vertical
        private Thumb resizeVerticalThumb;

        private ScrollViewer scrollViewer;

        private UIElement buttonBorder;

        private readonly Stack<WeakReference> openMenuItems = new Stack<WeakReference>();

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
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(DropDownButton));

        #endregion

        #region SizeDefinition

        /// <inheritdoc />
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
            set { this.SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(DropDownButton));

        #endregion

        #region KeyTip

        /// <inheritdoc />
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(DropDownButton));

        #endregion

        /// <inheritdoc />
        public Popup DropDownPopup { get; private set; }

        /// <inheritdoc />
        public bool IsContextMenuOpened { get; set; }

        #region Header

        /// <inheritdoc />
        public object Header
        {
            get { return this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(DropDownButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region Icon

        /// <inheritdoc />
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(DropDownButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region LargeIcon

        /// <inheritdoc />
        public object LargeIcon
        {
            get { return this.GetValue(LargeIconProperty); }
            set { this.SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty = LargeIconProviderProperties.LargeIconProperty.AddOwner(typeof(DropDownButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region HasTriangle

        /// <summary>
        /// Gets or sets whether button has triangle
        /// </summary>
        public bool HasTriangle
        {
            get { return (bool)this.GetValue(HasTriangleProperty); }
            set { this.SetValue(HasTriangleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasTriangle.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasTriangleProperty =
            DependencyProperty.Register(nameof(HasTriangle), typeof(bool), typeof(DropDownButton), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region IsDropDownOpen

        /// <inheritdoc />
        public bool IsDropDownOpen
        {
            get { return (bool)this.GetValue(IsDropDownOpenProperty); }
            set { this.SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(DropDownButton),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsDropDownOpenChanged));

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
            typeof(DropDownButton), new PropertyMetadata(ContextMenuResizeMode.None));

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
            DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(DropDownButton), new PropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0));

        #endregion

        #region DropDownHeight

        /// <summary>
        /// Gets or sets initial dropdown height
        /// </summary>
        public double DropDownHeight
        {
            get { return (double)this.GetValue(DropDownHeightProperty); }
            set { this.SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InitialDropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register(nameof(DropDownHeight), typeof(double), typeof(DropDownButton), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region ClosePopupOnMouseDown

        /// <summary>
        /// Gets or sets whether the popup of this drop down button should automatically be closed on mouse down.
        /// </summary>
        public bool ClosePopupOnMouseDown
        {
            get { return (bool)this.GetValue(ClosePopupOnMouseDownProperty); }
            set { this.SetValue(ClosePopupOnMouseDownProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ClosePopupOnMouseDown.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ClosePopupOnMouseDownProperty =
            DependencyProperty.Register(nameof(ClosePopupOnMouseDown), typeof(bool), typeof(DropDownButton), new PropertyMetadata(BooleanBoxes.FalseBox));

        #endregion

        #region ClosePopupOnMouseDownDelay

        /// <summary>
        /// Gets or sets the delay in milliseconds to close the popup on mouse down.
        /// </summary>
        public int ClosePopupOnMouseDownDelay
        {
            get { return (int)this.GetValue(ClosePopupOnMouseDownDelayProperty); }
            set { this.SetValue(ClosePopupOnMouseDownDelayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ClosePopupOnMouseDownDelay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ClosePopupOnMouseDownDelayProperty =
            DependencyProperty.Register(nameof(ClosePopupOnMouseDownDelay), typeof(int), typeof(DropDownButton), new PropertyMetadata(150));

        #endregion

        #endregion

        #region Events

        /// <inheritdoc />
        public event EventHandler DropDownOpened;

        /// <inheritdoc />
        public event EventHandler DropDownClosed;

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
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

            this.AddHandler(System.Windows.Controls.MenuItem.SubmenuOpenedEvent, new RoutedEventHandler(this.OnSubmenuOpened));
            this.AddHandler(System.Windows.Controls.MenuItem.SubmenuClosedEvent, new RoutedEventHandler(this.OnSubmenuClosed));
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

        /// <inheritdoc />
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

            this.scrollViewer = this.Template.FindName("PART_ScrollViewer", this) as ScrollViewer;

            this.buttonBorder = this.Template.FindName("PART_ButtonBorder", this) as UIElement;

            base.OnApplyTemplate();

            this.SubscribeEvents();
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <inheritdoc />
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
                // Note: get outside thread to prevent exceptions (it's a dependency property after all)
                var timespan = this.ClosePopupOnMouseDownDelay;

                // Ugly workaround, but use a timer to allow routed event to continue
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(timespan);

                    this.RunInDispatcherAsync(() => this.IsDropDownOpen = false);
                });
            }
        }

        private void HandleButtonBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            this.Focus();
            this.IsDropDownOpen = !this.IsDropDownOpen;
        }

        /// <inheritdoc />
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

                        var container = this.ItemContainerGenerator.ContainerFromIndex(0);

                        NavigateToContainer(container, FocusNavigationDirection.Down);

                        handled = true;
                    }

                    break;

                case Key.Up:
                    if (this.HasItems
                        && this.IsDropDownOpen == false) // Only handle this for initial navigation. Further navigation is handled by the dropdown itself
                    {
                        this.IsDropDownOpen = true;

                        var container = this.ItemContainerGenerator.ContainerFromIndex(this.Items.Count - 1);

                        NavigateToContainer(container, FocusNavigationDirection.Up);

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

            base.OnKeyDown(e);
        }

        private static void NavigateToContainer(DependencyObject container, FocusNavigationDirection focusNavigationDirection = FocusNavigationDirection.Down)
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
                element.MoveFocus(new TraversalRequest(focusNavigationDirection));
            }
        }

        private static object CoerceToolTipIsEnabled(DependencyObject d, object basevalue)
        {
            var control = (DropDownButton)d;

            return !control.IsDropDownOpen;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public virtual KeyTipPressedResult OnKeyTipPressed()
        {
            this.IsDropDownOpen = true;

            return new KeyTipPressedResult(true, true);
        }

        /// <inheritdoc />
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

            var oldValue = (bool)e.OldValue;
            var newValue = (bool)e.NewValue;

            control.OnIsDropDownOpenChanged(newValue);

            (UIElementAutomationPeer.FromElement(control) as Fluent.Automation.Peers.RibbonDropDownButtonAutomationPeer)?.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
        }

        private void OnIsDropDownOpenChanged(bool newValue)
        {
            this.SetValue(System.Windows.Controls.ToolTipService.IsEnabledProperty, !newValue);

            Debug.WriteLine($"{this.Header} IsDropDownOpen: {newValue}");

            if (newValue)
            {
                Mouse.Capture(this, CaptureMode.SubTree);

                Keyboard.Focus(this.DropDownPopup);

                this.RunInDispatcherAsync(
                                             () =>
                                             {
                                                 var container = this.ItemContainerGenerator.ContainerFromIndex(0);

                                                 NavigateToContainer(container);

                                                 // Edge case: Whole dropdown content is disabled
                                                 if (this.IsKeyboardFocusWithin == false)
                                                 {
                                                     Keyboard.Focus(this.DropDownPopup);
                                                 }
                                             });

                this.OnDropDownOpened();
            }
            else
            {
                // If focus is within the subtree, make sure we have the focus so that focus isn't in the disposed hwnd
                if (this.IsKeyboardFocusWithin)
                {
                    // make sure the control has focus
                    this.Focus();
                }

                Mouse.Capture(null);

                this.OnDropDownClosed();
            }
        }

        /// <summary>
        /// Called when drop down opened.
        /// </summary>
        protected virtual void OnDropDownOpened()
        {
            this.DropDownOpened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when drop down closed.
        /// </summary>
        protected virtual void OnDropDownClosed()
        {
            foreach (var openMenuItem in this.openMenuItems.ToArray())
            {
                if (openMenuItem.IsAlive == false)
                {
                    continue;
                }

                var menuItem = (System.Windows.Controls.MenuItem)openMenuItem.Target;
                if (menuItem.IsSubmenuOpen)
                {
                    menuItem.IsSubmenuOpen = false;
                }
            }

            this.openMenuItems.Clear();

            this.DropDownClosed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Quick Access Item Creating

        /// <inheritdoc />
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var button = new DropDownButton
            {
                Size = RibbonControlSize.Small
            };

            this.BindQuickAccessItem(button);
            RibbonControl.Bind(this, button, nameof(this.DisplayMemberPath), DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, nameof(this.GroupStyleSelector), GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, nameof(this.ItemContainerStyle), ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, nameof(this.ItemsPanel), ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, nameof(this.ItemStringFormat), ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, button, nameof(this.ItemTemplate), ItemTemplateProperty, BindingMode.OneWay);

            RibbonControl.Bind(this, button, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.OneWay);

            this.BindQuickAccessItemDropDownEvents(button);

            button.DropDownOpened += this.OnQuickAccessOpened;
            return button;
        }

        /// <summary>
        /// Handles quick access button drop down menu opened
        /// </summary>
        protected void OnQuickAccessOpened(object sender, EventArgs e)
        {
            var buttonInQuickAccess = (DropDownButton)sender;

            buttonInQuickAccess.DropDownClosed += this.OnQuickAccessMenuClosedOrUnloaded;
            buttonInQuickAccess.Unloaded += this.OnQuickAccessMenuClosedOrUnloaded;

            ItemsControlHelper.MoveItemsToDifferentControl(this, buttonInQuickAccess);
        }

        /// <summary>
        /// Handles quick access button drop down menu closed
        /// </summary>
        protected void OnQuickAccessMenuClosedOrUnloaded(object sender, EventArgs e)
        {
            var buttonInQuickAccess = (DropDownButton)sender;
            buttonInQuickAccess.DropDownClosed -= this.OnQuickAccessMenuClosedOrUnloaded;
            buttonInQuickAccess.Unloaded -= this.OnQuickAccessMenuClosedOrUnloaded;
            this.RunInDispatcherAsync(() =>
                                      {
                                          ItemsControlHelper.MoveItemsToDifferentControl(buttonInQuickAccess, this);
                                      }, DispatcherPriority.Loaded);
        }

        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected virtual void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.BindQuickAccessItem(this, element);
            RibbonControl.Bind(this, element, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.HasTriangle), HasTriangleProperty, BindingMode.Default);
        }

        /// <summary>
        /// Binds the DropDownClosed and DropDownOpened events to the created quick access item
        /// </summary>
        /// <param name="button">Toolbar item</param>
        protected void BindQuickAccessItemDropDownEvents(DropDownButton button)
        {
            if (this.DropDownClosed != null)
            {
                button.DropDownClosed += this.DropDownClosed;
            }

            if (this.DropDownOpened != null)
            {
                button.DropDownOpened += this.DropDownOpened;
            }
        }

        /// <inheritdoc />
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(DropDownButton), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

        #endregion

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonDropDownButtonAutomationPeer(this);

        #region MenuItem workarounds

        private void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            if (menuItem != null)
            {
                this.openMenuItems.Push(new WeakReference(menuItem));
            }
        }

        private void OnSubmenuClosed(object sender, RoutedEventArgs e)
        {
            if (this.openMenuItems.Count > 0)
            {
                this.openMenuItems.Pop();
            }
        }

        #endregion MenuItem workarounds

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

                if (this.Icon != null)
                {
                    yield return this.Icon;
                }

                if (this.LargeIcon != null)
                {
                    yield return this.LargeIcon;
                }

                if (this.Header != null)
                {
                    yield return this.Header;
                }
            }
        }
    }
}