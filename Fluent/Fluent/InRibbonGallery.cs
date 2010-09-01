#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fluent
{
    /// <summary>
    /// Represents the In-Ribbon Gallery, a gallery-based control that exposes 
    /// a default subset of items directly in the Ribbon. Any remaining items 
    /// are displayed when a drop-down menu button is clicked
    /// </summary>
    [ContentProperty("Items")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506")]
    public class InRibbonGallery : RibbonItemsControl,IScalableRibbonControl
    {
        #region Fields

        private ObservableCollection<GalleryGroupFilter> filters;

        private ObservableCollection<GalleryGroupIcon> groupIcons;

        private ListBox listBox;

        private ContextMenu contextMenu;

        private Gallery gallery = new Gallery();
        private MenuPanel menuBar = new MenuPanel();
        private ToggleButton expandButton;
        private ToggleButton dropDownButton;

        // Collection of toolbar items
        private ObservableCollection<UIElement> menuItems;

        //
        private int currentItemsInRow;

        private Panel layoutRoot;

        private double cachedWidthDelta;

        // Freezed image (created during snapping)
        Image snappedImage;
        // Visuals which were removed diring snapping
        Visual[] snappedVisuals;
        // Is visual currently snapped
        private bool isSnapped;

        private bool isInitializing;

        private DropDownButton quickAccessButton;

        // Saved width in for scalable support
        private double savedWidth;

        #endregion

        #region Properties

        #region ItemsInRow

        /// <summary>
        /// Width of the Gallery 
        /// </summary>
        public int ItemsInRow
        {
            get { return (int)GetValue(ItemsInRowProperty); }
            set { SetValue(ItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsInRow.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsInRowProperty =
            DependencyProperty.Register("ItemsInRow", typeof(int), typeof(InRibbonGallery), new UIPropertyMetadata(0));

        #endregion

        #region ItemWidth

        /// <summary>
        /// Gets or sets item width
        /// </summary>
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(InRibbonGallery), new UIPropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets item height
        /// </summary>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(InRibbonGallery), new UIPropertyMetadata(double.NaN));

        #endregion

        #region GalleryView

        /*/// <summary>
        /// Gets view of items or itemssource of dropdown gallery
        /// </summary>
        public CollectionViewSource GalleryView
        {
            get { return gallery.View; }
        }*/

        #endregion

        #region View

        /// <summary>
        /// Gets view of items or itemssource
        /// </summary>
        public CollectionViewSource View
        {
            get { return (CollectionViewSource)GetValue(ViewProperty); }
            private set { SetValue(ViewPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ViewPropertyKey =
            DependencyProperty.RegisterReadOnly("View", typeof(CollectionViewSource), typeof(InRibbonGallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for View.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;

        #endregion

        /// <summary>
        /// Gets current items in row
        /// </summary>
        int CurrentItemsInRow
        {
            get
            {
                return Math.Max(CanCollapseToButton ? MinItemsInRow - 1 : MinItemsInRow, Math.Min(MaxItemsInRow, MaxItemsInRow + currentItemsInRow));
            }
        }

        #region ScrollBarsVisibility

        /// <summary> 
        /// HorizonalScollbarVisibility is a Windows.Controls.ScrollBarVisibility that
        /// determines if a horizontal scrollbar is shown. 
        /// </summary> 
        [Bindable(true), Category("Appearance")]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(InRibbonGallery), new UIPropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary> 
        /// VerticalScrollBarVisibility is a System.Windows.Controls.ScrollBarVisibility that 
        /// determines if a vertical scrollbar is shown.
        /// </summary> 
        [Bindable(true), Category("Appearance")]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(InRibbonGallery), new UIPropertyMetadata(ScrollBarVisibility.Visible));

        #endregion

        #region GroupBy

        /// <summary>
        /// Gets or sets name of property which
        /// will use to group items in the Gallery.
        /// </summary>
        public string GroupBy
        {
            get { return (string)GetValue(GroupByProperty); }
            set { SetValue(GroupByProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupBy.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupByProperty =
            DependencyProperty.Register("GroupBy", typeof(string), 
            typeof(InRibbonGallery), new UIPropertyMetadata(null));        

        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets orientation of gallery
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(InRibbonGallery), new UIPropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InRibbonGallery inRibbonGallery = (InRibbonGallery) d;
            if (inRibbonGallery.gallery != null)
            {
                if ((Orientation)e.NewValue == Orientation.Horizontal)
                {
                    ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof (WrapPanel)));
                    template.Seal();
                    inRibbonGallery.ItemsPanel = template;
                }
                else
                {
                    ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
                    template.Seal();
                    inRibbonGallery.ItemsPanel = template;
                }
            }
        }

        #endregion

        #region Filters

        /// <summary>
        /// Gets collection of filters
        /// </summary>
        /// <summary>
        /// Gets collection of filters
        /// </summary>
        public ObservableCollection<GalleryGroupFilter> Filters
        {
            get
            {
                if (this.filters == null)
                {
                    this.filters = new ObservableCollection<GalleryGroupFilter>();
                    this.filters.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnFilterCollectionChanged);
                }
                return this.filters;
            }
        }


        // Handle toolbar iitems changes
        private void OnFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Filters.Count > 0) HasFilter = true;
            else HasFilter = false;
            InvalidateProperty(SelectedFilterProperty);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        gallery.Filters.Add(obj2 as GalleryGroupFilter);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        gallery.Filters.Remove(obj3 as GalleryGroupFilter);
                        
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        gallery.Filters.Remove(obj4 as GalleryGroupFilter);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        gallery.Filters.Add(obj5 as GalleryGroupFilter);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets or sets selected filter
        /// </summary>               
        public GalleryGroupFilter SelectedFilter
        {
            get { return (GalleryGroupFilter)GetValue(SelectedFilterProperty); }
            set { SetValue(SelectedFilterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(GalleryGroupFilter), typeof(InRibbonGallery), new UIPropertyMetadata(null, OnFilterChanged, CoerceSelectedFilter));

        // Coerce selected filter
        private static object CoerceSelectedFilter(DependencyObject d, object basevalue)
        {
            InRibbonGallery inRibbonGallery = (InRibbonGallery)d;
            if ((basevalue == null) && (inRibbonGallery.Filters.Count > 0)) return inRibbonGallery.Filters[0];
            return basevalue;
        }

        // Handles filter property changed
        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InRibbonGallery inRibbonGallery = (InRibbonGallery)d;
            if (e.NewValue != null) inRibbonGallery.SelectedFilterTitle = ((GalleryGroupFilter)e.NewValue).Title;
            else inRibbonGallery.SelectedFilterTitle = "";
            if (inRibbonGallery.View.View != null) inRibbonGallery.View.View.Refresh();
        }

        /// <summary>
        /// Gets selected filter title
        /// </summary>
        public string SelectedFilterTitle
        {
            get { return (string)GetValue(SelectedFilterTitleProperty); }
            private set { SetValue(SelectedFilterTitlePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SelectedFilterTitlePropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedFilterTitle", typeof(string), typeof(InRibbonGallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilterTitle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterTitleProperty = SelectedFilterTitlePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether gallery has selected filter
        /// </summary>       
        public bool HasFilter
        {
            get { return (bool)GetValue(HasFilterProperty); }
            private set { SetValue(HasFilterPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey HasFilterPropertyKey =
            DependencyProperty.RegisterReadOnly("HasFilter", typeof(bool), typeof(InRibbonGallery), new UIPropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasFilter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasFilterProperty = HasFilterPropertyKey.DependencyProperty;

        void OnFiltering(object obj, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(GroupBy)) e.Accepted = true;
            else if (SelectedFilter == null) e.Accepted = true;
            else if (e.Item!=null)
            {
                string[] filterItems = SelectedFilter.Groups.Split(",".ToCharArray());
                e.Accepted = filterItems.Contains(GetItemGroupName(e.Item));
            }
        }        

        #endregion

        #region Selectable

        /// <summary>
        /// Gets or sets whether gallery items can be selected
        /// </summary>
        public bool Selectable
        {
            get { return (bool)GetValue(SelectableProperty); }
            set { SetValue(SelectableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Selectable.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectableProperty =
            DependencyProperty.Register("Selectable", typeof(bool), 
            typeof(InRibbonGallery), new UIPropertyMetadata(true));

        #endregion

        #region SelectedIndex

        /// <summary>
        /// Gets or sets index of currently selected item
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedIndex. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int),
            typeof(InRibbonGallery), new UIPropertyMetadata(-1, OnSelectedIndexChanged, CoerceSelectedIndex));

        static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InRibbonGallery inRibbonGallery = (InRibbonGallery) d;
            if (inRibbonGallery.listBox != null)
            {
                inRibbonGallery.listBox.SelectedIndex = (int)e.NewValue;
            }
        }

        static object CoerceSelectedIndex(DependencyObject d, object basevalue)
        {
            InRibbonGallery inRibbonGallery = (InRibbonGallery)d;
            if (!inRibbonGallery.Selectable)
            {
                inRibbonGallery.listBox.SelectedIndex = -1;
                return -1;
            }
            return basevalue;
        }

        #endregion

        #region SelectedItem

        /// <summary>
        /// Gets or sets selected item
        /// </summary>
        [Bindable(true)]
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), 
            typeof(InRibbonGallery), new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChange, CoerceSelectedItem));

        static void OnSelectedItemChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InRibbonGallery inRibbonGallery = (InRibbonGallery)d;
            if (inRibbonGallery.listBox != null)
            {
                inRibbonGallery.listBox.SelectedItem = e.NewValue;
            }
            if (inRibbonGallery.SelectionChanged != null) inRibbonGallery.SelectionChanged(inRibbonGallery, EventArgs.Empty);
        }

        static object CoerceSelectedItem(DependencyObject d, object basevalue)
        {
            InRibbonGallery inRibbonGallery = (InRibbonGallery)d;
            if (!inRibbonGallery.Selectable)
            {
                if ((basevalue != null) && (inRibbonGallery.listBox!=null))
                {
                    ListBoxItem item = ((ListBoxItem)inRibbonGallery.listBox.ItemContainerGenerator.ContainerFromItem(basevalue));
                    if (item != null) item.IsSelected = false;
                    inRibbonGallery.listBox.SelectedItem = null;
                }
                return null;
            }
            return basevalue;
        }

        #endregion

        #region GroupIcons

        /// <summary>
        /// Gets collection of group icons
        /// </summary>
        public ObservableCollection<GalleryGroupIcon> GroupIcons
        {
            get
            {
                if (this.groupIcons == null)
                {
                    this.groupIcons = new ObservableCollection<GalleryGroupIcon>();
                    this.groupIcons.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupIconCollectionChanged);
                }
                return this.groupIcons;
            }
        }


        // Handle toolbar iitems changes
        private void OnGroupIconCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           /* switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        gallery.GroupIcons.Add(obj2 as GalleryGroupIcon);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        gallery.GroupIcons.Remove(obj3 as GalleryGroupIcon);

                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        gallery.GroupIcons.Remove(obj4 as GalleryGroupIcon);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        gallery.GroupIcons.Add(obj5 as GalleryGroupIcon);
                    }
                    break;
            }*/
        }

        #endregion

        #region IsOpen

        /// <summary>
        /// Gets or sets whether popup is opened
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), 
            typeof(InRibbonGallery), new UIPropertyMetadata(false, OnIsOpenChanged, CoerceIsOpen));

        // Coerce IsOpen
        private static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            if (((InRibbonGallery)d).isInitializing) return true;
            return basevalue;
        }

        // IsOpen Change Handling
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InRibbonGallery inRibbonGallery = ((InRibbonGallery)d);
            if ((bool)e.NewValue)
            {
                if (inRibbonGallery.contextMenu == null) inRibbonGallery.CreateMenu();
                else inRibbonGallery.contextMenu.IsOpen = true;
                if (inRibbonGallery.IsCollapsed) inRibbonGallery.dropDownButton.IsChecked = true;
            }
            else
            {
                if (inRibbonGallery.contextMenu != null) inRibbonGallery.contextMenu.IsOpen = false;
                if (inRibbonGallery.IsCollapsed) inRibbonGallery.dropDownButton.IsChecked = false;
            }
        }

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
        /// Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode), typeof(InRibbonGallery), new UIPropertyMetadata(ContextMenuResizeMode.None));

        #endregion

        #region MenuItems

        /// <summary>
        /// Gets collection of menu items
        /// </summary>
        public ObservableCollection<UIElement> MenuItems
        {
            get
            {
                if (menuItems == null)
                {
                    menuItems = new ObservableCollection<UIElement>();
                    menuItems.CollectionChanged += OnMenuItemsCollectionChanged;
                }
                return this.menuItems;
            }
        }

        /// <summary>
        /// handles colection of menu items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnMenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        if (menuBar != null) menuBar.Children.Add((UIElement)item);
                        else AddLogicalChild(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (menuBar != null) menuBar.Children.Remove((UIElement)item);
                        else RemoveLogicalChild(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (menuBar != null) menuBar.Children.Remove((UIElement)item);
                        else RemoveLogicalChild(item);
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (menuBar != null) menuBar.Children.Add((UIElement)item);
                        else AddLogicalChild(item);
                    }
                    break;
            }

        }

        #endregion

        #region CanCollapseToButton

        /// <summary>
        /// Gets or sets whether InRibbonGallery
        /// </summary>
        public bool CanCollapseToButton
        {
            get { return (bool)GetValue(CanCollapseToButtonProperty); }
            set { SetValue(CanCollapseToButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCollapseToButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCollapseToButtonProperty =
            DependencyProperty.Register("CanCollapseToButton", typeof(bool), typeof(InRibbonGallery), new UIPropertyMetadata(true));

        #endregion

        #region IsCollapsed

        /// <summary>
        /// Gets whether InRibbonGallery is collapsed to button
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(InRibbonGallery), new UIPropertyMetadata(false));

        #endregion

        #region LargeIcon

        /// <summary>
        /// Button large icon
        /// </summary>
        public ImageSource LargeIcon
        {
            get { return (ImageSource)GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(InRibbonGallery), new UIPropertyMetadata(null));

        #endregion

        #region Snapping

        /// <summary>
        /// Snaps / Unsnaps the Visual 
        /// (remove visuals and substitute with freezed image)
        /// </summary>
        private bool IsSnapped
        {
            get
            {
                return isSnapped;
            }
            set
            {
                if (value == isSnapped) return;

                
                    if ((value)&&(((int)ActualWidth > 0) && ((int)ActualHeight > 0)))
                    {
                        // Render the freezed image
                        snappedImage = new Image();
                        RenderOptions.SetBitmapScalingMode(snappedImage, BitmapScalingMode.NearestNeighbor);
                        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int) ActualWidth,
                                                                                       (int) ActualHeight, 96, 96,
                                                                                       PixelFormats.Pbgra32);
                        renderTargetBitmap.Render((Visual) VisualTreeHelper.GetChild(this, 0));
                        snappedImage.Source = renderTargetBitmap;
                        snappedImage.Width = ActualWidth;
                        snappedImage.Height = ActualHeight;
                        snappedImage.FlowDirection = FlowDirection;
                        // Detach current visual children
                        snappedVisuals = new Visual[VisualTreeHelper.GetChildrenCount(this)];
                        for (int childIndex = 0; childIndex < snappedVisuals.Length; childIndex++)
                        {
                            snappedVisuals[childIndex] = (Visual) VisualTreeHelper.GetChild(this, childIndex);
                            RemoveVisualChild(snappedVisuals[childIndex]);
                        }

                        // Attach freezed image
                        AddVisualChild(snappedImage);
                        isSnapped = value;
                        /*
                                            PngBitmapEncoder enc = new PngBitmapEncoder();
                                            enc.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                                            string path = Path.GetTempFileName() + ".png";
                                            using (FileStream f = new FileStream(path, FileMode.Create))
                                            {
                                                enc.Save(f);
                        
                                            }
                                            Process.Start(path);*/
                    }
                    else if ((snappedVisuals != null) && (snappedImage != null))
                    {                        
                        RemoveVisualChild(snappedImage);
                        for (int childIndex = 0; childIndex < snappedVisuals.Length; childIndex++)
                        {
                            AddVisualChild(snappedVisuals[childIndex]);
                        }

                        // Clean up
                        snappedImage = null;
                        snappedVisuals = null;
                        isSnapped = value;
                    }
                    
                
                
                InvalidateVisual();
                //UpdateLayout();
            }
        }

        /// <summary>
        /// Gets visual children count
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                if (isSnapped && IsVisible) return 1;
                return base.VisualChildrenCount;
            }
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (isSnapped && IsVisible) return snappedImage;
            return base.GetVisualChild(index);
        }

        #endregion

        #region Min/Max Sizes

        /// <summary>
        /// Gets or sets max count of items in row
        /// </summary>
        public int MaxItemsInRow
        {
            get { return (int)GetValue(MaxItemsInRowProperty); }
            set { SetValue(MaxItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxItemsInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxItemsInRowProperty =
                DependencyProperty.Register("MaxItemsInRow", typeof(int), typeof(InRibbonGallery), new UIPropertyMetadata(8));

        /// <summary>
        /// Gets or sets min count of items in row
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)GetValue(MinSizeProperty); }
            set { SetValue(MinSizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxItemsInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinSizeProperty =
                DependencyProperty.Register("MinItemsInRow", typeof(int), typeof(InRibbonGallery), new UIPropertyMetadata(1));

        #endregion

        #region LogicalChildren

        /// <summary>
        /// Gets an enumerator for logical child elements of this element. 
        /// </summary>
        /// <returns>
        /// An enumerator for logical child elements of this element.
        /// </returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if (listBox != null) list.AddRange(listBox.Items);
                list.AddRange(MenuItems);
                return list.GetEnumerator();
            }
        }

        #endregion

        #region MenuMinWidth

        /// <summary>
        /// Gets or sets minimal width of dropdown menu
        /// </summary>
        public double MenuMinWidth
        {
            get { return (double)GetValue(MenuMinWidthProperty); }
            set { SetValue(MenuMinWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MenuMinWidth. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuMinWidthProperty =
            DependencyProperty.Register("MenuMinWidth", typeof(double),
            typeof(InRibbonGallery), new UIPropertyMetadata(0.0));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when menu is opened
        /// </summary>
        public event EventHandler MenuOpened;
        /// <summary>
        /// Occurs when menu is closed
        /// </summary>
        public event EventHandler MenuClosed;

        /// <summary>
        /// Occurs when control is scaled
        /// </summary>
        public event EventHandler Scaled;

        /// <summary>
        /// Occurs then selection is changed
        /// </summary>
        public event EventHandler SelectionChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static InRibbonGallery()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InRibbonGallery), new FrameworkPropertyMetadata(typeof(InRibbonGallery)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public InRibbonGallery()
        {
            View = new CollectionViewSource();
            View.Filter += OnFiltering;

            //Loaded += delegate { if(View.View!=null)View.View.Refresh();};
            Loaded +=
                    delegate
                    {
                        object selectedItem = this.SelectedItem;
                        if (View.View != null)
                        {
                            if (View.View.CurrentItem != selectedItem)
                                View.View.MoveCurrentTo(selectedItem);
                            View.View.Refresh();
                        }
                    }; 
            Binding binding = new Binding("DisplayMemberPath");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.DisplayMemberPathProperty, binding);
            binding = new Binding("ItemBindingGroup");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemBindingGroupProperty, binding);
            binding = new Binding("ItemContainerStyle");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemContainerStyleProperty, binding);
            binding = new Binding("ItemContainerStyleSelector");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemContainerStyleSelectorProperty, binding);
            binding = new Binding("ItemsPanel");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemsPanelProperty, binding);
            binding = new Binding("ItemStringFormat");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemStringFormatProperty, binding);
            binding = new Binding("ItemTemplate");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemTemplateProperty, binding);
            binding = new Binding("ItemTemplateSelector");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemTemplateSelectorProperty, binding);
            binding = new Binding("ItemWidth");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemWidthProperty, binding);
            binding = new Binding("ItemHeight");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemHeightProperty, binding);
            binding = new Binding("IsTextSearchEnabled");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.IsTextSearchEnabledProperty, binding);

           /* binding = new Binding("VerticalScrollBarVisibility");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.VerticalScrollBarVisibilityProperty, binding);
            binding = new Binding("HorizontalScrollBarVisibility");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.HorizontalScrollBarVisibilityProperty, binding);*/

            binding = new Binding("GroupBy");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.GroupByProperty, binding);

            /*binding = new Binding("Orientation");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.OrientationProperty, binding);*/
            gallery.Orientation = Orientation;

            binding = new Binding("SelectedFilter");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.SelectedFilterProperty, binding);

            binding = new Binding("SelectedIndex");
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            gallery.SetBinding(SelectedIndexProperty, binding);

            binding = new Binding("SelectedItem");
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            gallery.SetBinding(SelectedItemProperty, binding);   

            

            AddLogicalChild(gallery);
            AddLogicalChild(menuBar);
        }

        #endregion  
      
        #region Overrides

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public override void OnKeyTipPressed()
        {
            IsOpen = true;
            base.OnKeyTipPressed();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application 
        /// code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (listBox != null)
            {
                listBox.SelectionChanged -= OnListBoxSelectionChanged;
                listBox.ItemContainerGenerator.StatusChanged -= OnItemsContainerGeneratorStatusChanged;           
                listBox.ItemsSource = null;                
            }
            listBox = GetTemplateChild("PART_ListBox") as ListBox;
            if (listBox != null)
            {
                RibbonControl.Bind(this,listBox,"View.View",ListBox.ItemsSourceProperty,BindingMode.OneWay);
                
                listBox.SelectedItem = SelectedItem;
                if (SelectedIndex!=-1) listBox.SelectedIndex = SelectedIndex;
                
                listBox.SelectionChanged += OnListBoxSelectionChanged;
                listBox.ItemContainerGenerator.StatusChanged += OnItemsContainerGeneratorStatusChanged;           
            }
            if (expandButton != null) expandButton.Click -= OnExpandClick;
            expandButton = GetTemplateChild("PART_ExpandButton") as ToggleButton;
            if (expandButton != null) expandButton.Click += OnExpandClick;

            if (dropDownButton != null) dropDownButton.Click -= OnDropDownClick;
            dropDownButton = GetTemplateChild("PART_DropDownButton") as ToggleButton;
            if (dropDownButton != null) dropDownButton.Click += OnDropDownClick;

            layoutRoot = GetTemplateChild("PART_LayoutRoot") as Panel;

            // Clear cache then style changed
            cachedWidthDelta = 0;
        }

        void OnItemsContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (Scaled != null) Scaled(this, EventArgs.Empty);
        }

        void OnDropDownClick(object sender, RoutedEventArgs e)
        {            
            dropDownButton.IsChecked = true;
            IsOpen = true;
            e.Handled = true;            
        }

        void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.ItemsSource != null)
            {
                SelectedIndex = listBox.SelectedIndex;
                SelectedItem = listBox.SelectedItem;
            }
        }

        void OnExpandClick(object sender, RoutedEventArgs e)
        {                        
            IsOpen = true;            
            e.Handled = true;
        }
        
        double GetItemWidth()
        {
            if(double.IsNaN(ItemWidth)&&(listBox!=null)&&(listBox.Items.Count>0))
            {
                GalleryItem item = (listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[0]) as GalleryItem);
                bool useHack = false;
                if (item == null)
                {
                    useHack = true;
                    RemoveLogicalChild(listBox.Items[0]);
                    item = new GalleryItem();                    
                    item.Width = ItemWidth;
                    item.Height = ItemHeight;
                    if (ItemContainerStyle != null) item.Style = ItemContainerStyle;
                    if (ItemTemplate != null)
                    {
                        item.Content = ItemTemplate;
                        item.DataContext = listBox.Items[0];
                    }
                    else item.Content = listBox.Items[0];
                }
                item.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
                if (useHack)
                {
                    item.Content = null;
                    AddLogicalChild(listBox.Items[0]);
                }
                return item.DesiredSize.Width;
            }
            return ItemWidth;
        }

        /// <summary>
        /// Called to remeasure a control. 
        /// </summary>
        /// <returns>The size of the control, up to the maximum specified by constraint</returns>
        /// <param name="constraint">The maximum size that the method can return</param>
        protected override Size MeasureOverride(Size constraint)
        {
            if (isSnapped)
            {
                if (snappedImage != null) return new Size(snappedImage.ActualWidth, snappedImage.ActualHeight);
                else return new Size(ActualWidth, ActualHeight);
            }
            if (IsCollapsed)
            {
                Size size = base.MeasureOverride(constraint);
                if(savedWidth != size.Width)
                {
                    savedWidth = size.Width;
                    if (Scaled != null) Scaled(this, EventArgs.Empty);
                }
                return size;
            }
            if (listBox == null) return base.MeasureOverride(constraint);
            if (listBox.Items.Count == 0) return base.MeasureOverride(constraint);
            double itemWidth = GetItemWidth();
            if(cachedWidthDelta==0)
            {
                base.MeasureOverride(constraint);
                //cachedWidthDelta = layoutRoot.DesiredSize.Width - listBox.InnerPanelWidth;
            }
            base.MeasureOverride(new Size(CurrentItemsInRow * itemWidth + cachedWidthDelta, constraint.Height));
            if (layoutRoot.DesiredSize.Width != savedWidth)
            {
                savedWidth = layoutRoot.DesiredSize.Width;
                if (Scaled != null) Scaled(this, EventArgs.Empty);
            }
            return layoutRoot.DesiredSize;
        }

        /// <summary>
        /// Handles size property changing
        /// </summary>
        /// <param name="previous">Previous value</param>
        /// <param name="current">Current value</param>
        protected override void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
            if (CanCollapseToButton)
            {
                if ((current == RibbonControlSize.Large) && ((CurrentItemsInRow > MinItemsInRow))) IsCollapsed = false;
                else IsCollapsed = true;
            }
            else IsCollapsed = false;
            base.OnSizePropertyChanged(previous, current);
        }

        /*/// <summary>
        /// Items collection changes hadling 
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsCollectionChanged(e);
            View.Source = Items;
        }

        /// <summary>
        /// ItemsSource property change handling
        /// </summary>
        /// <param name="args"></param>
        protected override void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            base.OnItemsSourceChanged(args);
            View.Source = ItemsSource;
        }*/

        #endregion

        #region Private Methods

        internal string GetItemGroupName(object obj)
        {
            object result = obj.GetType().GetProperty(GroupBy, BindingFlags.Public | BindingFlags.Instance).GetValue(obj, null);
            if(result==null) return null;
            return result.ToString();
        }

        private void CreateMenu()
        {
            /*if (ItemsInRow == 0) gallery.MinWidth = Math.Max(ActualWidth, MenuMinWidth);
            else
            {
                gallery.ItemsInRow = ItemsInRow;
            }
            gallery.MinHeight = ActualHeight;
            isInitializing = true;
            contextMenu = new ContextMenu();
            contextMenu.Owner = this;
            AddLogicalChild(contextMenu.RibbonPopup);                        
            contextMenu.IsOpen = true;            
            if (listBox != null)
            {
                if (!IsCollapsed) IsSnapped = true;
                object selectedItem = listBox.SelectedItem;
                int selectedIndex = listBox.SelectedIndex;
                listBox.ItemsSource = null;
                if (ItemsSource == null) gallery.ItemsSource = Items;
                else gallery.ItemsSource = ItemsSource;
                gallery.SelectedItem = selectedItem;
                gallery.SelectedIndex = selectedIndex;
                SelectedItem = selectedItem;
                SelectedIndex = selectedIndex;
                expandButton.IsChecked = true;
            }  
            contextMenu.RibbonPopup.Opened += OnMenuOpened;
            contextMenu.RibbonPopup.Closed += OnMenuClosed;            

            Binding binding = new Binding("ResizeMode");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            contextMenu.SetBinding(Fluent.ContextMenu.ResizeModeProperty, binding);

            contextMenu.PlacementTarget = this;
            if(IsCollapsed)contextMenu.Placement = PlacementMode.Bottom;
            else contextMenu.Placement = PlacementMode.Relative;

            RemoveLogicalChild(gallery);
            RemoveLogicalChild(menuBar);
            contextMenu.Items.Add(gallery);
            contextMenu.Items.Add(menuBar);
            
            isInitializing = false;
            Mouse.Capture(null);
            IsOpen = true;
            contextMenu.IsOpen = true;*/
        }


        private void OnMenuClosed(object sender, EventArgs e)
        {
            if (gallery.ItemsSource != null)
            {
                object selectedItem = gallery.SelectedItem;
                gallery.ItemsSource = null;
                if (listBox != null)
                {
                    listBox.ItemsSource = View.View;
                    listBox.SelectedItem = selectedItem;
                    SelectedItem = selectedItem;
                    SelectedIndex = listBox.SelectedIndex;
                    if (MenuClosed != null) MenuClosed(this, e);
                    if (!IsCollapsed) IsSnapped = false;
                    expandButton.IsChecked = false;
                    expandButton.InvalidateVisual();
                }
            }
            IsOpen = false;
        }

        private void OnMenuOpened(object sender, EventArgs e)
        {
            if (listBox.ItemsSource != null)
            {
                if (ItemsInRow == 0) gallery.MinWidth = Math.Max(ActualWidth, MenuMinWidth);
                else
                {
                    gallery.ItemsInRow = ItemsInRow;
                }
                gallery.MinHeight = ActualHeight;
                if (!IsCollapsed) IsSnapped = true;
                if (IsCollapsed) contextMenu.Placement = PlacementMode.Bottom;
                else contextMenu.Placement = PlacementMode.Relative;
                object selectedItem = listBox.SelectedItem;
                listBox.ItemsSource = null;
                if (ItemsSource == null) gallery.ItemsSource = Items;
                else gallery.ItemsSource = ItemsSource;
                gallery.SelectedItem = selectedItem;
                SelectedItem = selectedItem;
                SelectedIndex = gallery.SelectedIndex;
                if (MenuOpened != null) MenuOpened(this, e);
                //InvalidateVisual();
                //UpdateLayout();
                expandButton.IsChecked = true;
            }
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            DropDownButton button = new DropDownButton();
            BindQuickAccessItem(button);
            /*if (contextMenu == null)
            {
                CreateMenu();
                IsOpen = false;
            }*/            
            return button;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected void BindQuickAccessItem(FrameworkElement element)
        {
            DropDownButton button = element as DropDownButton;            
            //base.BindQuickAccessItem(element);
           // button.Opened += OnQuickAccessMenuOpened;
        }

        private void OnQuickAccessMenuOpened(object sender, EventArgs e)
        {
            if (ItemsInRow == 0) gallery.MinWidth = Math.Max(ActualWidth, MenuMinWidth);
            else
            {
                gallery.ItemsInRow = ItemsInRow;
            }
            gallery.MinHeight = ActualHeight;
            DropDownButton button = sender as DropDownButton;
            button.ResizeMode = ResizeMode;
            if (!IsCollapsed) IsSnapped = true;

            object selectedItem = SelectedItem;
            if (listBox!=null)
            {
                selectedItem = listBox.SelectedItem;
                listBox.ItemsSource = null;
            }
            if (ItemsSource == null) gallery.ItemsSource = Items;
            else gallery.ItemsSource = ItemsSource;
            gallery.SelectedItem = selectedItem;
            SelectedItem = selectedItem;
            SelectedIndex = gallery.SelectedIndex;

           /* if (contextMenu != null)
            {
                for (int i = 0; i < contextMenu.Items.Count; i++)
                {
                    UIElement item = contextMenu.Items[0];
                    contextMenu.Items.Remove(item);
                    button.Items.Add(item);
                    i--;
                }
            }
            else
            {                
                RemoveLogicalChild(gallery);
                RemoveLogicalChild(menuBar);
                button.Items.Add(gallery);
                button.Items.Add(menuBar);
            }*/
            /*button.Closed += OnQuickAccessMenuClosed;
            quickAccessButton = button;*/
        }

        private void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            /*quickAccessButton.Closed -= OnQuickAccessMenuClosed;
            if (contextMenu != null)
            {
                for (int i = 0; i < quickAccessButton.Items.Count; i++)
                {
                    UIElement item = quickAccessButton.Items[0];
                    quickAccessButton.Items.Remove(item);
                    contextMenu.Items.Add(item);
                    i--;
                }
            }
            else
            {
                quickAccessButton.Items.Remove(gallery);
                quickAccessButton.Items.Remove(menuBar);
                AddLogicalChild(gallery);
                AddLogicalChild(menuBar);
            }

            object selectedItem = gallery.SelectedItem;
            gallery.ItemsSource = null;
            if (listBox != null)
            {
                if (ItemsSource == null) listBox.ItemsSource = Items;
                else listBox.ItemsSource = ItemsSource;
                listBox.SelectedItem = selectedItem;
                SelectedIndex = listBox.SelectedIndex;
            }
            SelectedItem = selectedItem;            
            if (!IsCollapsed) IsSnapped = false;*/
        }

        #endregion

        #region Implementation of IScalableRibbonControl

        /// <summary>
        /// Enlarge control size
        /// </summary>
        public void Enlarge()
        {
            currentItemsInRow++;

            if ((CanCollapseToButton) && (CurrentItemsInRow >= MinItemsInRow) && (Size == RibbonControlSize.Large)) IsCollapsed = false;
           
            InvalidateMeasure();
        }

        /// <summary>
        /// Reduce control size
        /// </summary>
        public void Reduce()
        {            
            currentItemsInRow--;
            if ((CanCollapseToButton) && (CurrentItemsInRow < MinItemsInRow)) IsCollapsed = true;
           
            InvalidateMeasure();
        }

        #endregion
    }
}
