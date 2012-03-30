#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents ribbon tab control
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RibbonTabItem))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_TabsContainer", Type = typeof(IScrollInfo))]
    [TemplatePart(Name = "PART_ToolbarPanel", Type = typeof(Panel))]
    public class RibbonTabControl : Selector, IDropDownControl
    {
        #region Fields

        // Popup
        Popup popup;
        // Old selected item
        WeakReference oldSelectedItem;

        // Collection of toolbar items
        ObservableCollection<UIElement> toolBarItems;
        // ToolBar panel
        Panel toolbarPanel;

        #endregion

        #region Properties

        #region Menu

        /// <summary>
        /// Gets or sets file menu control (can be application menu button, backstage button and so on)
        /// </summary>
        public UIElement Menu
        {
            get { return (UIElement)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Button. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register("Menu", typeof(UIElement),
            typeof(RibbonTabControl), new UIPropertyMetadata(null));

        #endregion

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

        /// <summary>
        /// Gets content of selected tab item
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedContent
        {
            get
            {
                return base.GetValue(SelectedContentProperty);
            }
            internal set
            {
                base.SetValue(SelectedContentPropertyKey, value);
            }
        }

        // DependencyProperty key for SelectedContent
        static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContent", typeof(object), typeof(RibbonTabControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets whether ribbon is minimized
        /// </summary>
        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            set { SetValue(IsMinimizedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMinimized.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register("IsMinimized", typeof(bool), typeof(RibbonTabControl), new UIPropertyMetadata(false, OnMinimizedChanged));

        /// <summary>
        /// Gets or sets whether ribbon popup is opened
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RibbonTabControl), new UIPropertyMetadata(false, OnIsOpenChanged, CoerceIsDropDownOpen));

        private static object CoerceIsDropDownOpen(DependencyObject d, object basevalue)
        {
            RibbonTabControl tabControl = d as RibbonTabControl;
            if (!tabControl.IsMinimized) return false;
            return basevalue;
        }

        /// <summary>
        /// Gets whether ribbon tabs can scroll
        /// </summary>
        internal bool CanScroll
        {
            get
            {
                IScrollInfo scrollInfo = GetTemplateChild("PART_TabsContainer") as IScrollInfo;
                if (scrollInfo != null) return (scrollInfo.ExtentWidth > scrollInfo.ViewportWidth);
                else return false;
            }
        }

        /// <summary>
        /// Gets or sets selected tab item
        /// </summary>
        internal RibbonTabItem SelectedTabItem
        {
            get { return (RibbonTabItem)GetValue(SelectedTabItemProperty); }
            set { SetValue(SelectedTabItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTabItem.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedTabItemProperty =
            DependencyProperty.Register("SelectedTabItem", typeof(RibbonTabItem), typeof(RibbonTabControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets collection of ribbon toolbar items
        /// </summary>
        public ObservableCollection<UIElement> ToolBarItems
        {
            get
            {
                if (this.toolBarItems == null)
                {
                    this.toolBarItems = new ObservableCollection<UIElement>();
                    this.toolBarItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnToolbarItemsCollectionChanged);
                }
                return this.toolBarItems;
            }
        }

        internal Panel ToolbarPanel
        {
            get { return toolbarPanel; }
        }

        // Handle toolbar iitems changes
        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        if (ToolbarPanel != null) ToolbarPanel.Children.Insert(e.NewStartingIndex + i, (UIElement)e.NewItems[i]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (ToolbarPanel != null) ToolbarPanel.Children.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (ToolbarPanel != null) ToolbarPanel.Children.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (ToolbarPanel != null) ToolbarPanel.Children.Add(obj5 as UIElement);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets or sets the height of the gap between the ribbon and the content
        /// </summary>
        public double ContentGapHeight
        {
            get { return (double)GetValue(ContentGapHeightProperty); }
            set { SetValue(ContentGapHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentGapHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentGapHeightProperty =
            DependencyProperty.Register("ContentGapHeight", typeof(double), typeof(RibbonTabControl), new UIPropertyMetadata(5D));

        #endregion

        #region Initializion

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonTabControl()
        {
            Type type = typeof(RibbonTabControl);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(typeof(RibbonTabControl)));
            ContextMenuService.Attach(type);
            PopupService.Attach(type);
            StyleProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(RibbonTabControl));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabControl()
        {
            ContextMenuService.Coerce(this);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. 
        /// This method is invoked whenever System.Windows.
        /// FrameworkElement.IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            base.ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonTabItem();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            popup = GetTemplateChild("PART_Popup") as Popup;
            if (popup != null)
            {
                /*Binding binding = new Binding("IsOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                popup.SetBinding(Popup.IsOpenProperty, binding);
                */
                popup.CustomPopupPlacementCallback = CustomPopupPlacementMethod;
            }
            if ((ToolbarPanel != null) && (toolBarItems != null))
            {
                for (int i = 0; i < toolBarItems.Count; i++)
                {
                    ToolbarPanel.Children.Remove(toolBarItems[i]);
                }
            }
            toolbarPanel = GetTemplateChild("PART_ToolbarPanel") as Panel;
            if ((ToolbarPanel != null) && (toolBarItems != null))
            {
                for (int i = 0; i < toolBarItems.Count; i++)
                {
                    ToolbarPanel.Children.Add(toolBarItems[i]);
                }
            }
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RibbonTabItem);
        }

        /// <summary>
        /// Updates the current selection when an item in the System.Windows.Controls.Primitives.Selector has changed
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if ((e.Action == NotifyCollectionChangedAction.Remove) && (base.SelectedIndex == -1))
            {
                int startIndex = e.OldStartingIndex + 1;
                if (startIndex > base.Items.Count)
                {
                    startIndex = 0;
                }
                RibbonTabItem item = this.FindNextTabItem(startIndex, -1);
                if (item != null)
                {
                    item.IsSelected = true;
                }
                else SelectedContent = null;
            }
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Backstage backstage = Menu as Backstage;
                if (IsMinimized && (backstage == null || !backstage.IsOpen))
                {
                    if (oldSelectedItem != null
                        && oldSelectedItem.IsAlive
                        && oldSelectedItem.Target == e.AddedItems[0])
                    {
                        IsDropDownOpen = !IsDropDownOpen;
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new ThreadStart(delegate { IsDropDownOpen = true; }));
                    }
                    ((RibbonTabItem)e.AddedItems[0]).IsHitTestVisible = false;
                }

                UpdateSelectedContent();
            }
            else
            {
                IsDropDownOpen = false;
            }

            if (e.RemovedItems.Count > 0)
            {
                oldSelectedItem = new WeakReference(e.RemovedItems[0]);
                ((RibbonTabItem)e.RemovedItems[0]).IsHitTestVisible = true;
            }

            base.OnSelectionChanged(e);
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.PreviewMouseWheel 
        /// attached event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseWheelEventArgs that contains the event data.</param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            //base.OnPreviewMouseWheel(e);
            ProcessMouseWheel(e);
        }

        #endregion

        #region Private methods

        // Find parent ribbon
        private Ribbon FindParentRibbon()
        {
            DependencyObject element = this;
            while (LogicalTreeHelper.GetParent(element) != null)
            {
                element = LogicalTreeHelper.GetParent(element);
                Ribbon ribbon = element as Ribbon;
                if (ribbon != null) return ribbon;
            }
            return null;
        }

        private bool IsRibbonAncestorOf(DependencyObject element)
        {
            while (element != null)
            {
                if (element is Ribbon) return true;
                DependencyObject parent = LogicalTreeHelper.GetParent(element);
                if (parent == null) parent = VisualTreeHelper.GetParent(element);
                element = parent;
            }
            return false;
        }

        // Process mouse wheel event
        internal void ProcessMouseWheel(MouseWheelEventArgs e)
        {
            if (IsMinimized) return;
            if (SelectedItem == null) return;
            DependencyObject focusedElement = Keyboard.FocusedElement as DependencyObject;
            if (focusedElement != null)
            {
                if (IsRibbonAncestorOf(focusedElement)) return;
            }
            List<RibbonTabItem> visualItems = new List<RibbonTabItem>();
            int selectedIndex = -1;
            for (int i = 0; i < Items.Count; i++)
            {
                if ((Items[i] as RibbonTabItem).Visibility == Visibility.Visible)
                {
                    visualItems.Add((Items[i] as RibbonTabItem));
                    if ((Items[i] as RibbonTabItem).IsSelected) selectedIndex = visualItems.Count - 1;
                }
            }
            if (e.Delta > 0)
            {
                if (selectedIndex > 0)
                {
                    visualItems[selectedIndex].IsSelected = false;
                    selectedIndex--;
                    visualItems[selectedIndex].IsSelected = true;
                }
            }
            if (e.Delta < 0)
            {
                if (selectedIndex < visualItems.Count - 1)
                {
                    visualItems[selectedIndex].IsSelected = false;
                    selectedIndex++;
                    visualItems[selectedIndex].IsSelected = true;
                }
            }
            e.Handled = true;
        }

        // Get selected ribbon tab item
        private RibbonTabItem GetSelectedTabItem()
        {
            object selectedItem = base.SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }
            RibbonTabItem item = selectedItem as RibbonTabItem;
            if (item == null)
            {
                item = base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex) as RibbonTabItem;
            }
            return item;
        }

        // Find next tab item
        private RibbonTabItem FindNextTabItem(int startIndex, int direction)
        {
            if (direction != 0)
            {
                int index = startIndex;
                for (int i = 0; i < base.Items.Count; i++)
                {
                    index += direction;
                    if (index >= base.Items.Count)
                    {
                        index = 0;
                    }
                    else if (index < 0)
                    {
                        index = base.Items.Count - 1;
                    }
                    RibbonTabItem item2 = base.ItemContainerGenerator.ContainerFromIndex(index) as RibbonTabItem;
                    if (((item2 != null) && item2.IsEnabled) && (item2.Visibility == Visibility.Visible))
                    {
                        return item2;
                    }
                }
            }
            return null;
        }

        // Updates selected content
        void UpdateSelectedContent()
        {
            if (SelectedIndex < 0)
            {

                SelectedContent = null;
                SelectedTabItem = null;
            }
            else
            {
                RibbonTabItem selectedTabItem = GetSelectedTabItem();
                if (selectedTabItem != null)
                {
                    SelectedContent = selectedTabItem.GroupsContainer;
                    UpdateLayout();
                    SelectedTabItem = selectedTabItem;
                }
            }
        }

        #endregion

        #region Event handling

        // Handles GeneratorStatus changed
        void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                UpdateSelectedContent();
            }
        }

        // Handles IsMinimized changed
        static void OnMinimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabControl tab = (RibbonTabControl)d;
            if (!tab.IsMinimized)
            {
                tab.IsDropDownOpen = false;
            }
        }

        // Handles ribbon popup closing
        void OnRibbonTabPopupClosing()
        {
            if (SelectedItem is RibbonTabItem)
            {
                (SelectedItem as RibbonTabItem).IsHitTestVisible = true;
            }
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }
        }

        // handles ribbon popup opening
        void OnRibbonTabPopupOpening()
        {
            if (SelectedItem is RibbonTabItem)
            {
                (SelectedItem as RibbonTabItem).IsHitTestVisible = false;
            }
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        /// <summary>
        /// Implements custom placement for ribbon popup
        /// </summary>
        /// <param name="popupsize"></param>
        /// <param name="targetsize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        CustomPopupPlacement[] CustomPopupPlacementMethod(Size popupsize, Size targetsize, Point offset)
        {
            if ((popup != null) && (SelectedTabItem != null))
            {
                // Get current workarea                
                Point tabItemPos = SelectedTabItem.PointToScreen(new Point(0, 0));
                NativeMethods.Rect tabItemRect = new NativeMethods.Rect();
                tabItemRect.Left = (int)tabItemPos.X;
                tabItemRect.Top = (int)tabItemPos.Y;
                tabItemRect.Right = (int)tabItemPos.X + (int)SelectedTabItem.ActualWidth;
                tabItemRect.Bottom = (int)tabItemPos.Y + (int)SelectedTabItem.ActualHeight;

                uint MONITOR_DEFAULTTONEAREST = 0x00000002;
                System.IntPtr monitor = NativeMethods.MonitorFromRect(ref tabItemRect, MONITOR_DEFAULTTONEAREST);
                if (monitor != System.IntPtr.Zero)
                {
                    NativeMethods.MonitorInfo monitorInfo = new NativeMethods.MonitorInfo();
                    monitorInfo.Size = Marshal.SizeOf(monitorInfo);
                    NativeMethods.GetMonitorInfo(monitor, monitorInfo);

                    Point startPoint = PointToScreen(new Point(0, 0));
                    if (FlowDirection == FlowDirection.RightToLeft) startPoint.X -= ActualWidth;
                    double inWindowRibbonWidth = monitorInfo.Work.Right - Math.Max(monitorInfo.Work.Left, startPoint.X);

                    double actualWidth = ActualWidth;
                    if (startPoint.X < monitorInfo.Work.Left)
                    {
                        actualWidth -= monitorInfo.Work.Left - startPoint.X;
                        startPoint.X = monitorInfo.Work.Left;
                    }
                    // Set width
                    popup.Width = Math.Min(actualWidth, inWindowRibbonWidth);
                    return new CustomPopupPlacement[]
                               {
                                   new CustomPopupPlacement(new Point(startPoint.X - tabItemPos.X, SelectedTabItem.ActualHeight-(popup.Child as FrameworkElement).Margin.Top), PopupPrimaryAxis.None),
                                   new CustomPopupPlacement(new Point(startPoint.X - tabItemPos.X, -(SelectedContent as ScrollViewer).ActualHeight-(popup.Child as FrameworkElement).Margin.Bottom), PopupPrimaryAxis.None),
                               };
                }
            }
            return null;
        }

        // Handles IsOpen property changed
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabControl ribbon = (RibbonTabControl)d;

            if (ribbon.IsDropDownOpen)
            {
                ribbon.OnRibbonTabPopupOpening();
            }
            else
            {
                ribbon.OnRibbonTabPopupClosing();
            }
        }

        /*private static void OnClickThroughThunk(object sender, MouseButtonEventArgs e)
        {
            RibbonTabControl ribbon = (RibbonTabControl)sender;
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
            {
                if (Mouse.Captured == ribbon)
                {
                    ribbon.IsOpen = false;
                    Mouse.Capture(null);
                }
            }
        }*/

        #endregion
    }
}
