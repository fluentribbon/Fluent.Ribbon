// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using ControlzEx.Standard;
    using Fluent.Automation.Peers;
    using Fluent.Collections;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents ribbon tab control
    /// </summary>
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(RibbonTabItem))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_TabsContainer", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_MinimizeButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ToolbarPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_SelectedContentPresenter", Type = typeof(ContentPresenter))]
    public class RibbonTabControl : Selector, IDropDownControl, ILogicalChildSupport
    {
        /// <summary>
        /// Default value for <see cref="ContentGapHeight"/>.
        /// </summary>
        public const double DefaultContentGapHeight = 1;

        /// <summary>
        /// Default value for <see cref="ContentHeight"/>.
        /// </summary>
        public const double DefaultContentHeight = 94;

        /// <summary>
        /// Provides a value needed to add space to the popup to accomodate for overlapping keytips.
        /// </summary>
        public const double AdditionalPopupSpaceForKeyTips = 20;

        /// <summary>
        /// Provides a value needed to add space to the popup to accomodate for overlapping keytips.
        /// </summary>
        public static readonly GridLength AdditionalPopupSpaceForKeyTipsGridLength = new GridLength(AdditionalPopupSpaceForKeyTips);

        #region Fields

        // Collection of toolbar items
        private ObservableCollection<UIElement> toolBarItems;

        // ToolBar panel

        #endregion

        #region Events

        /// <summary>
        /// Event which is fired when the, maybe listening, <see cref="Backstage"/> should be closed
        /// </summary>
        public event EventHandler RequestBackstageClose;

        /// <inheritdoc />
        public event EventHandler DropDownOpened;

        /// <inheritdoc />
        public event EventHandler DropDownClosed;

        #endregion

        #region Properties

        #region Menu

        /// <summary>
        /// Gets or sets file menu control (can be application menu button, backstage button and so on)
        /// </summary>
        public UIElement Menu
        {
            get { return (UIElement)this.GetValue(MenuProperty); }
            set { this.SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Button.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(UIElement),
            typeof(RibbonTabControl), new PropertyMetadata());

        #endregion

        /// <inheritdoc />
        public Popup DropDownPopup { get; private set; }

        /// <summary>
        /// Gets the <see cref="Panel"/> responsible for displaying the selected tabs content.
        /// </summary>
        public Panel TabsContainer { get; private set; }

        internal ButtonBase MinimizeButton { get; private set; }

        /// <summary>
        /// Gets the <see cref="ContentPresenter"/> responsible for displaying the selected tabs content.
        /// </summary>
        public ContentPresenter SelectedContentPresenter { get; private set; }

        /// <inheritdoc />
        public bool IsContextMenuOpened { get; set; }

        /// <summary>
        /// Gets content of selected tab item
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedContent
        {
            get
            {
                return this.GetValue(SelectedContentProperty);
            }

            internal set
            {
                this.SetValue(SelectedContentPropertyKey, value);
            }
        }

        // DependencyProperty key for SelectedContent
        private static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContent), typeof(object), typeof(RibbonTabControl), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for <see cref="SelectedContent"/>.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets whether ribbon is minimized
        /// </summary>
        public bool IsMinimized
        {
            get { return (bool)this.GetValue(IsMinimizedProperty); }
            set { this.SetValue(IsMinimizedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for <see cref="IsMinimized"/>.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register(nameof(IsMinimized), typeof(bool), typeof(RibbonTabControl), new PropertyMetadata(BooleanBoxes.FalseBox, OnIsMinimizedChanged));

        /// <summary>
        /// Gets or sets whether ribbon can be minimized
        /// </summary>
        public bool CanMinimize
        {
            get { return (bool)this.GetValue(CanMinimizeProperty); }
            set { this.SetValue(CanMinimizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for <see cref="CanMinimize"/>.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanMinimizeProperty = DependencyProperty.Register(nameof(CanMinimize), typeof(bool), typeof(RibbonTabControl), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <inheritdoc />
        public bool IsDropDownOpen
        {
            get { return (bool)this.GetValue(IsDropDownOpenProperty); }
            set { this.SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for <see cref="IsDropDownOpen"/>.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(RibbonTabControl), new PropertyMetadata(BooleanBoxes.FalseBox, OnIsDropDownOpenChanged, CoerceIsDropDownOpen));

        private static object CoerceIsDropDownOpen(DependencyObject d, object basevalue)
        {
            var tabControl = d as RibbonTabControl;

            if (tabControl == null)
            {
                return basevalue;
            }

            if (!tabControl.IsMinimized)
            {
                return false;
            }

            return basevalue;
        }

        /// <summary>
        /// Defines if the currently selected item should draw it's highlight/selected borders
        /// </summary>
        public bool HighlightSelectedItem
        {
            get { return (bool)this.GetValue(HighlightSelectedItemProperty); }
            set { this.SetValue(HighlightSelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for <see cref="HighlightSelectedItem"/>.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HighlightSelectedItemProperty =
            DependencyProperty.RegisterAttached(nameof(HighlightSelectedItem), typeof(bool), typeof(RibbonTabControl), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets whether ribbon tabs can scroll
        /// </summary>
        internal bool CanScroll
        {
            get
            {
                if (this.TabsContainer is IScrollInfo scrollInfo)
                {
                    return scrollInfo.ExtentWidth > scrollInfo.ViewportWidth;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets selected tab item
        /// </summary>
        internal RibbonTabItem SelectedTabItem
        {
            get { return (RibbonTabItem)this.GetValue(SelectedTabItemProperty); }
            private set { this.SetValue(SelectedTabItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTabItem.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedTabItemProperty =
            DependencyProperty.Register(nameof(SelectedTabItem), typeof(RibbonTabItem), typeof(RibbonTabControl), new PropertyMetadata());

        /// <summary>
        /// Gets collection of ribbon toolbar items
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<UIElement> ToolBarItems
        {
            get
            {
                if (this.toolBarItems == null)
                {
                    this.toolBarItems = new ObservableCollection<UIElement>();
                    this.toolBarItems.CollectionChanged += this.OnToolbarItemsCollectionChanged;
                }

                return this.toolBarItems;
            }
        }

        internal Panel ToolbarPanel { get; private set; }

        // Handle toolbar iitems changes
        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.ToolbarPanel == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.ToolbarPanel.Children.Insert(e.NewStartingIndex + i, (UIElement)e.NewItems[i]);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var obj3 in e.OldItems.OfType<UIElement>())
                    {
                        this.ToolbarPanel.Children.Remove(obj3);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var obj4 in e.OldItems.OfType<UIElement>())
                    {
                        this.ToolbarPanel.Children.Remove(obj4);
                    }

                    foreach (var obj5 in e.NewItems.OfType<UIElement>())
                    {
                        this.ToolbarPanel.Children.Add(obj5);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.ToolbarPanel.Children.Clear();
                    foreach (var toolBarItem in this.ToolBarItems)
                    {
                        this.ToolbarPanel.Children.Add(toolBarItem);
                    }

                    break;
            }
        }

        /// <summary>
        /// Gets or sets the height of the content area.
        /// </summary>
        public double ContentHeight
        {
            get { return (double)this.GetValue(ContentHeightProperty); }
            set { this.SetValue(ContentHeightProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ContentHeight"/>.
        /// </summary>
        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register(nameof(ContentHeight), typeof(double), typeof(RibbonTabControl), new FrameworkPropertyMetadata(DefaultContentHeight, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the height of the gap between the ribbon and the content
        /// </summary>
        public double ContentGapHeight
        {
            get { return (double)this.GetValue(ContentGapHeightProperty); }
            set { this.SetValue(ContentGapHeightProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="ContentGapHeight"/>
        /// </summary>
        public static readonly DependencyProperty ContentGapHeightProperty =
            DependencyProperty.Register(nameof(ContentGapHeight), typeof(double), typeof(RibbonTabControl), new PropertyMetadata(DefaultContentGapHeight));

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="AreTabHeadersVisible"/>.
        /// </summary>
        public static readonly DependencyProperty AreTabHeadersVisibleProperty = DependencyProperty.Register(nameof(AreTabHeadersVisible), typeof(bool), typeof(RibbonTabControl), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Defines whether tab headers are visible or not.
        /// </summary>
        public bool AreTabHeadersVisible
        {
            get { return (bool)this.GetValue(AreTabHeadersVisibleProperty); }
            set { this.SetValue(AreTabHeadersVisibleProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsToolBarVisible"/>.
        /// </summary>
        public static readonly DependencyProperty IsToolBarVisibleProperty = DependencyProperty.Register(nameof(IsToolBarVisible), typeof(bool), typeof(RibbonTabControl), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Defines whether tab headers are visible or not.
        /// </summary>
        public bool IsToolBarVisible
        {
            get { return (bool)this.GetValue(IsToolBarVisibleProperty); }
            set { this.SetValue(IsToolBarVisibleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="IsMouseWheelScrollingEnabled"/>
        /// </summary>
        public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty = DependencyProperty.Register(nameof(IsMouseWheelScrollingEnabled), typeof(bool), typeof(RibbonTabControl), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Defines whether scrolling by mouse wheel is enabled or not.
        /// </summary>
        public bool IsMouseWheelScrollingEnabled
        {
            get { return (bool)this.GetValue(IsMouseWheelScrollingEnabledProperty); }
            set { this.SetValue(IsMouseWheelScrollingEnabledProperty, value); }
        }

        #endregion

        #region Initializion

        /// <summary>
        /// Initializes static members of the <see cref="RibbonTabControl"/> class.
        /// </summary>
        static RibbonTabControl()
        {
            var type = typeof(RibbonTabControl);

            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(typeof(RibbonTabControl)));
            IsTabStopProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));

            ContextMenuService.Attach(type);
            PopupService.Attach(type);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonTabControl"/> class.
        /// </summary>
        public RibbonTabControl()
        {
            ContextMenuService.Coerce(this);

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.ItemContainerGenerator.StatusChanged += this.OnGeneratorStatusChanged;
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonTabItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RibbonTabItem;
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.TabsContainer = this.GetTemplateChild("PART_TabsContainer") as Panel;

            this.MinimizeButton = this.GetTemplateChild("PART_MinimizeButton") as ButtonBase;

            this.SelectedContentPresenter = this.Template.FindName("PART_SelectedContentPresenter", this) as ContentPresenter;

            this.DropDownPopup = this.Template.FindName("PART_Popup", this) as Popup;

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.CustomPopupPlacementCallback = this.CustomPopupPlacementMethod;
            }

            if (this.ToolbarPanel != null
                && this.toolBarItems != null)
            {
                foreach (var item in this.toolBarItems)
                {
                    this.ToolbarPanel.Children.Remove(item);
                    this.AddLogicalChild(item);
                }
            }

            this.ToolbarPanel = this.Template.FindName("PART_ToolbarPanel", this) as Panel;

            if (this.ToolbarPanel != null
                && this.toolBarItems != null)
            {
                foreach (var item in this.toolBarItems)
                {
                    this.RemoveLogicalChild(item);
                    this.ToolbarPanel.Children.Add(item);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (this.IsMinimized
                && this.IsDropDownOpen == false)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove
                && this.SelectedIndex == -1)
            {
                var startIndex = e.OldStartingIndex + 1;
                if (startIndex > this.Items.Count)
                {
                    startIndex = 0;
                }

                var item = this.FindNextTabItem(startIndex, -1);
                if (item != null)
                {
                    item.IsSelected = true;
                }
                else
                {
                    this.SelectedContent = null;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            this.UpdateSelectedContent();

            if (this.IsKeyboardFocusWithin
                && this.IsMinimized == false)
            {
                // If keyboard focus is within the control, make sure it is going to the correct place
                var item = this.GetSelectedTabItem();
                item?.SetFocus();
            }

            if (e.AddedItems.Count > 0)
            {
                if (this.IsMinimized)
                {
                    this.IsDropDownOpen = true;

                    ((RibbonTabItem)e.AddedItems[0]).IsHitTestVisible = false;
                }
            }
            else
            {
                if (this.IsDropDownOpen)
                {
                    this.IsDropDownOpen = false;
                }
            }

            if (e.RemovedItems.Count > 0)
            {
                ((RibbonTabItem)e.RemovedItems[0]).IsHitTestVisible = true;
            }

            base.OnSelectionChanged(e);
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            //base.OnPreviewMouseWheel(e);

            if (this.IsMouseWheelScrollingEnabled)
            {
                this.ProcessMouseWheel(e);
            }
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.OnKeyUp(e);

            if (e.Handled)
            {
                return;
            }

            // Handle [Ctrl][Shift]Tab, Home and End cases
            // We have special handling here because if focus is inside the TabItem content we cannot
            // cycle through TabItem because the content is not part of the TabItem visual tree
            var direction = 0;
            var startIndex = -1;

            switch (e.Key)
            {
                case Key.Escape:
                    if (this.IsDropDownOpen)
                    {
                        this.IsDropDownOpen = false;
                    }

                    break;

                case Key.Tab:
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        startIndex = this.ItemContainerGenerator.IndexFromContainer(this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem));
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        {
                            direction = -1;
                        }
                        else
                        {
                            direction = 1;
                        }
                    }

                    break;
                case Key.Home:
                    direction = 1;
                    startIndex = -1;
                    break;
                case Key.End:
                    direction = -1;
                    startIndex = this.Items.Count;
                    break;
            }

            var nextTabItem = this.FindNextTabItem(startIndex, direction);

            if (nextTabItem != null
                && ReferenceEquals(nextTabItem, this.SelectedItem) == false)
            {
                e.Handled = nextTabItem.SetFocus();
            }

            if (e.Handled == false)
            {
                base.OnKeyDown(e);
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonTabControlAutomationPeer(this);

        #endregion

        #region Private methods

        // Process mouse wheel event
        internal void ProcessMouseWheel(MouseWheelEventArgs e)
        {
            if (this.IsMinimized
                || this.SelectedItem == null)
            {
                return;
            }

            // Prevent scrolling if
            // - any control inside a RibbonGroupBox has focus
            // - any control outside this RibbonTabControl caused the mouse wheel event
            if ((Keyboard.FocusedElement is DependencyObject focusedElement
                && UIHelper.GetParent<RibbonGroupBox>(focusedElement) != null)
                ||
                (e.OriginalSource is DependencyObject originalSource
                && UIHelper.GetParent<RibbonTabControl>(originalSource) == null))
            {
                return;
            }

            var visualItems = new List<RibbonTabItem>();
            var selectedIndex = -1;

            var tabs = this.ItemContainerGenerator.Items.OfType<RibbonTabItem>()
                .Where(x => x.Visibility == Visibility.Visible && x.IsEnabled && (x.IsContextual == false || (x.IsContextual && x.Group.Visibility == Visibility.Visible)))
                .OrderBy(x => x.IsContextual);

            foreach (var ribbonTabItem in tabs)
            {
                visualItems.Add(ribbonTabItem);

                if (ribbonTabItem.IsSelected)
                {
                    selectedIndex = visualItems.Count - 1;
                }
            }

            // Try to ensure that we have a selection
            if (selectedIndex < 0)
            {
                if (visualItems.Count > 0)
                {
                    visualItems[0].IsSelected = true;
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    if (selectedIndex > 0)
                    {
                        visualItems[selectedIndex].IsSelected = false;
                        selectedIndex--;
                        visualItems[selectedIndex].IsSelected = true;
                    }
                }
                else if (e.Delta < 0)
                {
                    if (selectedIndex < visualItems.Count - 1)
                    {
                        visualItems[selectedIndex].IsSelected = false;
                        selectedIndex++;
                        visualItems[selectedIndex].IsSelected = true;
                    }
                }
            }

            e.Handled = true;
        }

        // Get selected ribbon tab item
        private RibbonTabItem GetSelectedTabItem()
        {
            var selectedItem = this.SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }

            var item = selectedItem as RibbonTabItem
                ?? this.ItemContainerGenerator.ContainerOrContainerContentFromIndex<RibbonTabItem>(this.SelectedIndex);

            return item;
        }

        // Find next tab item
        private RibbonTabItem FindNextTabItem(int startIndex, int direction)
        {
            if (direction != 0)
            {
                var index = startIndex;
                for (var i = 0; i < this.Items.Count; i++)
                {
                    index += direction;

                    if (index >= this.Items.Count)
                    {
                        index = 0;
                    }
                    else if (index < 0)
                    {
                        index = this.Items.Count - 1;
                    }

                    if (this.ItemContainerGenerator.ContainerOrContainerContentFromIndex<RibbonTabItem>(index) is RibbonTabItem nextItem
                        && nextItem.IsEnabled
                        && nextItem.Visibility == Visibility.Visible)
                    {
                        return nextItem;
                    }
                }
            }

            return null;
        }

        // Updates selected content
        private void UpdateSelectedContent()
        {
            if (this.SelectedIndex < 0)
            {
                this.SelectedContent = null;
                this.SelectedTabItem = null;
            }
            else
            {
                var selectedTabItem = this.GetSelectedTabItem();
                if (selectedTabItem != null)
                {
                    this.SelectedContent = selectedTabItem.GroupsContainer;
                    this.SelectedTabItem = selectedTabItem;
                }
            }
        }

        #endregion

        #region Event handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
        }

        // Handles GeneratorStatus changed
        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                this.UpdateSelectedContent();
            }
        }

        /// <summary>
        /// Selects the first tab if <see cref="IsMinimized"/> is <c>false</c>.
        /// </summary>
        public void SelectFirstTab()
        {
            if (this.IsMinimized == false)
            {
                this.SelectedItem = this.GetFirstVisibleAndEnabledItem();

                if (this.SelectedItem == null
                    && this.IsEnabled == false)
                {
                    this.SelectedItem = this.GetFirstVisibleItem();
                }
            }
        }

        // Handles IsMinimized changed
        private static void OnIsMinimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tab = (RibbonTabControl)d;

            if (!tab.IsMinimized)
            {
                tab.IsDropDownOpen = false;
            }

            if ((bool)e.NewValue == false
                && tab.SelectedIndex < 0)
            {
                var item = tab.FindNextTabItem(-1, 1);

                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
        }

        // Handles ribbon popup closing
        private void OnRibbonTabPopupClosing()
        {
            if (this.SelectedItem is RibbonTabItem ribbonTabItem)
            {
                ribbonTabItem.IsHitTestVisible = true;
            }

            if (ReferenceEquals(Mouse.Captured, this))
            {
                Mouse.Capture(null);
            }

            this.DropDownClosed?.Invoke(this, EventArgs.Empty);
        }

        // handles ribbon popup opening
        private void OnRibbonTabPopupOpening()
        {
            if (this.SelectedItem is RibbonTabItem ribbonTabItem)
            {
                ribbonTabItem.IsHitTestVisible = false;
            }

            Mouse.Capture(this, CaptureMode.SubTree);

            this.DropDownOpened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Implements custom placement for ribbon popup
        /// </summary>
        private CustomPopupPlacement[] CustomPopupPlacementMethod(Size popupsize, Size targetsize, Point offset)
        {
            if (this.DropDownPopup == null
                || this.SelectedTabItem == null)
            {
                return null;
            }

            var tabItemDimensionsOnScreen = this.SelectedTabItem.PointToScreen(new Point(this.SelectedTabItem.ActualWidth, this.SelectedTabItem.ActualHeight)) - this.SelectedTabItem.PointToScreen(new Point(0, 0));
            var tabItemActualSizeOnScreen = new Point(Math.Abs(tabItemDimensionsOnScreen.X), Math.Abs(tabItemDimensionsOnScreen.Y));

            var widthFactor = tabItemActualSizeOnScreen.X / this.SelectedTabItem.ActualWidth;
            var heightFactor = tabItemActualSizeOnScreen.Y / this.SelectedTabItem.ActualHeight;

            // Get current workarea
            var tabItemUpperLeftOnScreen = this.SelectedTabItem.PointToScreen(new Point(0, 0));
#pragma warning disable 618
            var tabItemOriginPointOnScreenRect = new RECT
            {
                Left = (int)tabItemUpperLeftOnScreen.X,
                Top = (int)tabItemUpperLeftOnScreen.Y,
                Right = (int)tabItemUpperLeftOnScreen.X + (int)tabItemDimensionsOnScreen.X,
                Bottom = (int)tabItemUpperLeftOnScreen.Y + (int)tabItemDimensionsOnScreen.Y
            };

            var monitor = NativeMethods.MonitorFromRect(ref tabItemOriginPointOnScreenRect, MonitorOptions.MONITOR_DEFAULTTONEAREST);
            if (monitor == IntPtr.Zero)
            {
                return null;
            }

            var monitorInfo = NativeMethods.GetMonitorInfo(monitor);

#pragma warning restore 618

            var tabControlUpperLeftOnScreen = this.PointToScreen(new Point(0, 0));
            var tabControlDimensionsOnScreen = this.PointToScreen(new Point(this.ActualWidth, this.ActualHeight)) - this.PointToScreen(new Point(0, 0));
            var tabControlActualSizeOnScreen = new Point(Math.Abs(tabControlDimensionsOnScreen.X), Math.Abs(tabControlDimensionsOnScreen.Y));

            if (this.FlowDirection == FlowDirection.RightToLeft)
            {
                tabControlUpperLeftOnScreen.X -= tabControlActualSizeOnScreen.X;
            }

            // Calculate the popup width
            // We have to take into account here that, when the window is moved to the side of a monitor and the window is not fully visible 
            // the popup width is reduced to the maximum visible size of the window on the monitor the selected tab item is on.
            // If we don't reduce the popup width wpf tries to be helpful and moves the popup out of the window to satisfy the width.
            {
                var inWindowRibbonWidth = monitorInfo.rcWork.Right - Math.Max(monitorInfo.rcWork.Left, tabControlUpperLeftOnScreen.X);

                var actualWidth = this.ActualWidth;
                if (tabControlUpperLeftOnScreen.X < monitorInfo.rcWork.Left)
                {
                    actualWidth -= (monitorInfo.rcWork.Left - tabControlUpperLeftOnScreen.X) / widthFactor;
                }

                // Set width and prevent negative values
                this.DropDownPopup.Width = Math.Max(0, Math.Min(actualWidth, inWindowRibbonWidth));
            }

            return new[]
            {
                new CustomPopupPlacement(new Point(tabControlUpperLeftOnScreen.X - tabItemUpperLeftOnScreen.X, tabItemActualSizeOnScreen.Y - (AdditionalPopupSpaceForKeyTips * heightFactor)), PopupPrimaryAxis.Vertical),
                new CustomPopupPlacement(new Point(tabControlUpperLeftOnScreen.X - tabItemUpperLeftOnScreen.X, -1 * (tabItemActualSizeOnScreen.Y - (AdditionalPopupSpaceForKeyTips * heightFactor))), PopupPrimaryAxis.Vertical)
            };
        }

        // Handles IsDropDownOpen property changed
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbonTabControl = (RibbonTabControl)d;

            ribbonTabControl.RaiseRequestBackstageClose();

            if (ribbonTabControl.IsDropDownOpen)
            {
                ribbonTabControl.OnRibbonTabPopupOpening();
            }
            else
            {
                ribbonTabControl.OnRibbonTabPopupClosing();
            }

            if (ribbonTabControl.SelectedTabItem != null)
            {
                var peer = UIElementAutomationPeer.CreatePeerForElement(ribbonTabControl.SelectedTabItem) as RibbonTabItemAutomationPeer;
                peer?.RaiseTabExpandCollapseAutomationEvent((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Raises an event causing the Backstage-View to be closed
        /// </summary>
        public void RaiseRequestBackstageClose()
        {
            this.RequestBackstageClose?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// Gets the first visible item
        /// </summary>
        public object GetFirstVisibleItem()
        {
            foreach (var item in this.Items)
            {
                if ((this.ItemContainerGenerator.ContainerOrContainerContentFromItem<RibbonTabItem>(item) ?? item) is RibbonTabItem ribbonTab
                    && ribbonTab.Visibility == Visibility.Visible)
                {
                    return ribbonTab;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first visible and enabled item
        /// </summary>
        public object GetFirstVisibleAndEnabledItem()
        {
            foreach (var item in this.Items)
            {
                if ((this.ItemContainerGenerator.ContainerOrContainerContentFromItem<RibbonTabItem>(item) ?? item) is RibbonTabItem ribbonTab
                    && ribbonTab.Visibility == Visibility.Visible
                    && ribbonTab.IsEnabled)
                {
                    return ribbonTab;
                }
            }

            return null;
        }

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

                if (this.SelectedContent != null)
                {
                    yield return this.SelectedContent;
                }
            }
        }
    }
}