using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;

namespace Fluent
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RibbonTabItem))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_TabsContainer", Type = typeof(IScrollInfo))]
    [TemplatePart(Name = "PART_ToolbarPanel", Type = typeof(Panel))]
    public class RibbonTabControl: Selector
    {
        #region Constants
        

        #endregion

        #region Dependency propeties
        
        private static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContent", typeof(object), typeof(RibbonTabControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register("IsMinimized", typeof(bool), typeof(RibbonTabControl), new UIPropertyMetadata(false, OnMinimizedChanged));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(RibbonTabControl), new UIPropertyMetadata(false, OnIsOpenChanged));

        #endregion

        #region Fields

        private Popup popup = null;

        private object oldSelectedItem = null;

        private ObservableCollection<UIElement> toolbarItems = null;

        private Panel toolbarPanel = null;

        #endregion

        #region Свойства

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

        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            set { SetValue(IsMinimizedProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        internal bool CanScroll
        {
            get 
            {
                IScrollInfo scrollInfo = GetTemplateChild("PART_TabsContainer") as IScrollInfo;
                if(scrollInfo!=null)return (scrollInfo.ExtentWidth >scrollInfo.ViewportWidth); 
                else return false;
            }
        }


        internal RibbonTabItem SelectedTabItem
        {
            get { return (RibbonTabItem)GetValue(SelectedTabItemProperty); }
            set { SetValue(SelectedTabItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTabItem.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedTabItemProperty =
            DependencyProperty.Register("SelectedTabItem", typeof(RibbonTabItem), typeof(RibbonTabControl), new UIPropertyMetadata(null));

        public ObservableCollection<UIElement> ToolbarItems
        {
            get
            {
                if (this.toolbarItems == null)
                {
                    this.toolbarItems = new ObservableCollection<UIElement>();
                    this.toolbarItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnToolbarItemsCollectionChanged);
                }
                return this.toolbarItems;
            }
        }

        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (toolbarPanel != null) toolbarPanel.Children.Add(obj2 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (toolbarPanel != null) toolbarPanel.Children.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (toolbarPanel != null) toolbarPanel.Children.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (toolbarPanel != null) toolbarPanel.Children.Add(obj5 as UIElement);
                    }
                    break;
            }

        }

        #endregion

        #region Инициализация

        static RibbonTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTabControl), new FrameworkPropertyMetadata(typeof(RibbonTabControl)));

            /*EventManager.RegisterClassHandler(typeof(RibbonTabControl), Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));
            EventManager.RegisterClassHandler(typeof(RibbonTabControl), Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));*/
        }                

        public RibbonTabControl()
        {
        }

        #endregion

        #region Overrides

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            //base.CanSelectMultiple = false;
            base.ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonTabItem();
        }

        /// <summary>
        /// On new style applying
        /// </summary>
        public override void OnApplyTemplate()
        {
            //if (popup != null) RemoveLogicalChild(popup);            
            popup = GetTemplateChild("PART_Popup") as Popup;
            if (popup != null)
            {
                //if (popup.Parent != null) (popup.Parent as Panel).Children.Remove(popup);
                //AddLogicalChild(popup);
                Binding binding = new Binding("IsOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                popup.SetBinding(Popup.IsOpenProperty, binding);

                popup.CustomPopupPlacementCallback = CustomPopupPlacementMethod;
            }
            if (toolbarPanel != null)
            {
                toolbarPanel.Children.Clear();
            }
            toolbarPanel = GetTemplateChild("PART_ToolbarPanel") as Panel;
            if ((toolbarPanel != null) && (toolbarItems != null))
            {
                for (int i = 0; i < toolbarItems.Count; i++)
                {
                    toolbarPanel.Children.Add(toolbarItems[i]);
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RibbonTabItem);
        }

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
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.AddedItems.Count > 0)
            {
                if (IsMinimized)
                {
                    if (oldSelectedItem == e.AddedItems[0])
                        IsOpen = !IsOpen;
                    else
                    {
                        IsOpen = true;                        
                    }
                    if (e.AddedItems[0] is RibbonTabItem) (e.AddedItems[0] as RibbonTabItem).IsHitTestVisible = false;
                }

                this.UpdateSelectedContent();
            }
            if (e.RemovedItems.Count > 0)
            {
                oldSelectedItem = e.RemovedItems[0];
                if(oldSelectedItem is RibbonTabItem) (oldSelectedItem as RibbonTabItem).IsHitTestVisible = true;
            }
        }
        /*
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (Mouse.Captured != this)
            {
                UIElement selectedTabGroupsPopupChild = popup.Child;
                if (e.OriginalSource == this)
                {
                    // If Ribbon loses capture because something outside popup is clicked - close the popup
                    if (Mouse.Captured == null || !selectedTabGroupsPopupChild.IsAncestorOf(Mouse.Captured as DependencyObject))
                    {
                        this.IsOpen = false;
                    }
                }
                else
                {
                    // If control inside Ribbon loses capture - restore capture to Ribbon
                    if (selectedTabGroupsPopupChild.IsAncestorOf(e.OriginalSource as DependencyObject))
                    {
                        if (this.IsOpen && Mouse.Captured == null)
                        {
                            Mouse.Capture(this, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        this.IsOpen = false;
                    }
                }
            }
            base.OnLostMouseCapture(e);
        }
        */
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            ProcessMouseWheel(e);

            /*if (e.Delta > 0)
                if (SelectedIndex > 0) SelectedIndex--;
            if (e.Delta < 0)
                if (SelectedIndex < Items.Count-1) SelectedIndex++;*/
            e.Handled = true;
        }

        #endregion

        #region Private methods

        internal void ProcessMouseWheel(MouseWheelEventArgs e)
        {
            if (IsMinimized) return;
            List<RibbonTabItem> visualItems = new List<RibbonTabItem>();
            int selectedIndex = -1;
            for (int i = 0; i < Items.Count; i++)
            {
                if ((Items[i] as RibbonTabItem).Visibility == Visibility.Visible)
                {
                    visualItems.Add((Items[i] as RibbonTabItem));
                    if ((Items[i] as RibbonTabItem).IsSelected) selectedIndex = i;
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

        private void UpdateSelectedContent()
        {
            if (base.SelectedIndex < 0)
            {

                this.SelectedContent = null;
                SelectedTabItem = null;
            }
            else
            {
                RibbonTabItem selectedTabItem = this.GetSelectedTabItem();
                if (selectedTabItem != null)
                {
                    FrameworkElement parent = VisualTreeHelper.GetParent(selectedTabItem) as FrameworkElement;
                    this.SelectedContent = selectedTabItem.GroupsContainer;
                    UpdateLayout();
                    SelectedTabItem = selectedTabItem;
                }
            }
        }

        


        #endregion

        #region Event handling

        /*private static void OnToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabControl ribbon = (RibbonTabControl)d;

            UIElement oldToolbar = e.OldValue as UIElement;
            UIElement newToolbar = e.NewValue as UIElement;

            // Remove Logical tree link
            if (oldToolbar != null)
            {
                ribbon.RemoveLogicalChild(oldToolbar);
            }

            // Add Logical tree link
            if (newToolbar != null)
            {
                ribbon.AddLogicalChild(newToolbar);
            }
        }*/

        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                //if (base.HasItems && (base._selectedItems.Count == 0))
                if (base.HasItems && (base.SelectedIndex == -1))
                {
                    base.SelectedIndex = 0;
                }
                this.UpdateSelectedContent();
            }
        }

        private static void OnMinimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabControl tab = d as RibbonTabControl;
            if (!tab.IsMinimized)
            {
                tab.IsOpen = false;
            }
            /*else if (Mouse.Captured == tab)
            {
                Mouse.Capture(null);
            }*/
        }

        private static void OnClickThroughThunk(object sender, MouseButtonEventArgs e)
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
        }

        private void OnRibbonTabPopupClosing()
        {
            /*if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }*/
            if (SelectedItem is RibbonTabItem) (SelectedItem as RibbonTabItem).IsHitTestVisible = true;
         }

        private void OnRibbonTabPopupOpening()
        {
           /*if (IsMinimized)
            {
                Mouse.Capture(this, CaptureMode.SubTree);
            }*/
            if (SelectedItem is RibbonTabItem) (SelectedItem as RibbonTabItem).IsHitTestVisible = false;            
        }

        private CustomPopupPlacement[] CustomPopupPlacementMethod(Size popupsize, Size targetsize, Point offset)
        {
            if ((popup != null) && (SelectedTabItem != null))
            {
                // Get current workarea                
                Point tabItemPos = SelectedTabItem.PointToScreen(new Point(0, 0));
                Rect tabItemRect = new Rect();
                tabItemRect.left = (int)tabItemPos.X;
                tabItemRect.top = (int)tabItemPos.Y;
                tabItemRect.right = (int)tabItemPos.X + (int)SelectedTabItem.ActualWidth;
                tabItemRect.bottom = (int)tabItemPos.Y + (int)SelectedTabItem.ActualHeight;

                uint MONITOR_DEFAULTTONEAREST = 0x00000002;
                System.IntPtr monitor = MonitorFromRect(ref tabItemRect, MONITOR_DEFAULTTONEAREST);
                if (monitor != System.IntPtr.Zero)
                {
                    MonitorInfo monitorInfo = new MonitorInfo();
                    monitorInfo.size = (uint)Marshal.SizeOf(monitorInfo);
                    GetMonitorInfo(monitor, ref monitorInfo);

                    Point startPoint = PointToScreen(new Point(0, 0));
                    double inWindowRibbonWidth = monitorInfo.work.right - Math.Max(monitorInfo.work.left, startPoint.X);

                    double actualWidth = ActualWidth;
                    double startOffset = 0;
                    if (startPoint.X < monitorInfo.work.left)
                    {
                        actualWidth -= monitorInfo.work.left - startPoint.X;
                        //startOffset = monitorInfo.work.left - startPoint.X;
                        startPoint.X = monitorInfo.work.left;
                    }
                    // Set width
                    popup.Width = Math.Min(actualWidth, inWindowRibbonWidth);
                    return new CustomPopupPlacement[]
                               {
                                   new CustomPopupPlacement(new Point(startPoint.X - tabItemPos.X, SelectedTabItem.ActualHeight-(popup.Child as FrameworkElement).Margin.Top), PopupPrimaryAxis.None),
                                   new CustomPopupPlacement(new Point(startPoint.X - tabItemPos.X, -(SelectedContent as RibbonGroupsContainer).ActualHeight-(popup.Child as FrameworkElement).Margin.Bottom), PopupPrimaryAxis.None),
                               };
                }
            }
            return null;
        }

        #region Interop

        [StructLayout(LayoutKind.Sequential)]
        struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MonitorInfo
        {
            public uint size;
            public Rect monitor;
            public Rect work;
            public uint flags;
        }

        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromRect([In] ref Rect lprc, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

        #endregion

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabControl ribbon = (RibbonTabControl)d;

            if (ribbon.IsOpen)
            {
                ribbon.OnRibbonTabPopupOpening();
            }
            else
            {
                ribbon.OnRibbonTabPopupClosing();
            }
        }

        #endregion

        #region RibbonLogicalChildEnumerator Class

        /// <summary>
        ///   An enumerator for the logical children of the Ribbon.
        /// </summary>
        private class RibbonTabControlLogicalChildrenEnumerator : IEnumerator
        {
            #region Fields

            private UIElement toolbar;

            /// <summary>
            ///   The Ribbon's collection of tabs.
            /// </summary>
            private ItemCollection items;

            /// <summary>
            ///   The current position of enumeration.
            /// </summary>
            private Position postition;

            /// <summary>
            ///   The current tab index if we are currently enumerating the Ribbon's tabs.
            /// </summary>
            private int index = 0;

            #endregion

            #region Constructors

            public RibbonTabControlLogicalChildrenEnumerator(UIElement toolbar, ItemCollection items)
            {
                postition = Position.None;
                this.toolbar = toolbar;
                this.items = items;
            }

            #endregion

            #region Position Enum

            /// <summary>
            ///   An enum indicating the current position of enumeration.
            /// </summary>
            private enum Position
            {
                /// <summary>
                ///   Indicates that the enumeration is not currently within the Ribbon's
                ///   logical children.
                /// </summary>
                None,

                /// <summary>
                ///   Indicates enumeration is currently at the ApplicationMenu.
                /// </summary>
                Toolbar,

                /// <summary>
                ///   Indicates enumeration is currently at the QuickAccessToolbar.
                /// </summary>
                Items
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///   Gets the object at the enumerators current position.
            /// </summary>
            public object Current
            {
                get
                {
                    switch (postition)
                    {
                        case Position.Toolbar:
                            return toolbar;
                        case Position.Items:
                            return items[index];
                    }

                    throw new InvalidOperationException();
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///   Advances the enumerator to the next logical child of the Ribbon.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced, false otherwise.</returns>
            public bool MoveNext()
            {
                if (postition == Position.None)
                {
                    postition = Position.Toolbar;
                    return true;
                }
                if (postition == Position.Toolbar)
                {
                    postition = Position.Items;
                    if (items != null)
                    {
                        return true;
                    }
                }

                if (postition == Position.Items)
                {
                    if (index < items.Count - 2)
                    {
                        index++;
                        return true;
                    }
                }

                this.Reset();

                return false;
            }

            /// <summary>
            ///   Resets the RibbonLogicalChildrenEnumerator.
            /// </summary>
            public void Reset()
            {
                postition = Position.None;
                index = 0;
            }

            #endregion
        }

        #endregion
    }
}
