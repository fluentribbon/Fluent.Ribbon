#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Fluent
{
    // TODO: add TemplatePart's in Gallery (!)

    /// <summary>
    /// Represents gallery control. 
    /// Usually a gallery is hosted in context menu
    /// </summary>
    [ContentProperty("Items")]
    public class Gallery : ListBox//, IQuickAccessItemProvider
    {
        #region Fields

        ObservableCollection<GalleryGroupFilter> filters;

        DropDownButton groupsMenuButton;

        #endregion

        #region Properties

        #region MinItemsInRow

        /// <summary>
        /// Min width of the Gallery 
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)GetValue(MinItemsInRowProperty); }
            set { SetValue(MinItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinItemsInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinItemsInRowProperty =
            DependencyProperty.Register("MinItemsInRow", typeof(int),
            typeof(Gallery), new UIPropertyMetadata(1));

        #endregion

        #region MaxItemsInRow

        /// <summary>
        /// Max width of the Gallery 
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
            DependencyProperty.Register("MaxItemsInRow", typeof(int),
            typeof(Gallery), new UIPropertyMetadata(Int32.MaxValue));

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
            DependencyProperty.Register("GroupBy", typeof(string), typeof(Gallery),
            new UIPropertyMetadata(null));

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
        /// Using a DependencyProperty as the backing store for Orientation.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation),
            typeof(Gallery), new UIPropertyMetadata(Orientation.Horizontal));

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
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(Gallery), new UIPropertyMetadata(double.NaN));

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
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(Gallery), new UIPropertyMetadata(double.NaN));

        #endregion

        #region Filters

        /// <summary>
        /// Gets collection of filters
        /// </summary>
        public ObservableCollection<GalleryGroupFilter> Filters
        {
            get
            {
                if (filters == null)
                {
                    filters = new ObservableCollection<GalleryGroupFilter>();
                    filters.CollectionChanged += OnFilterCollectionChanged;
                }
                return filters;
            }
        }

        // Handle toolbar items changes
        void OnFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasFilter = Filters.Count > 0;
            InvalidateProperty(SelectedFilterProperty);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        if (groupsMenuButton != null)
                        {
                            GalleryGroupFilter filter = (GalleryGroupFilter)e.NewItems[i];
                            MenuItem menuItem = new MenuItem();
                            menuItem.Header = filter.Title;
                            menuItem.Tag = filter;
                            if (filter == SelectedFilter) menuItem.IsChecked = true;
                            menuItem.Click += OnFilterMenuItemClick;
                            groupsMenuButton.Items.Insert(e.NewStartingIndex + i, menuItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (groupsMenuButton != null)
                        {
                            groupsMenuButton.Items.Remove(GetFilterMenuItem(item as GalleryGroupFilter));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (groupsMenuButton != null)
                        {
                            groupsMenuButton.Items.Remove(GetFilterMenuItem(item as GalleryGroupFilter));
                        }
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (groupsMenuButton != null)
                        {
                            GalleryGroupFilter filter = (GalleryGroupFilter)item;
                            MenuItem menuItem = new MenuItem();
                            menuItem.Header = filter.Title;
                            menuItem.Tag = filter;
                            if (filter == SelectedFilter) menuItem.IsChecked = true;
                            menuItem.Click += OnFilterMenuItemClick;
                            groupsMenuButton.Items.Add(menuItem);
                        }
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
        /// Using a DependencyProperty as the backing store for SelectedFilter. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(GalleryGroupFilter),
            typeof(Gallery), new UIPropertyMetadata(null, OnFilterChanged, CoerceSelectedFilter));

        // Coerce selected filter
        static object CoerceSelectedFilter(DependencyObject d, object basevalue)
        {
            Gallery gallery = (Gallery)d;
            if ((basevalue == null) && (gallery.Filters.Count > 0)) return gallery.Filters[0];
            return basevalue;
        }

        // Handles filter property changed
        static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery gallery = (Gallery)d;
            GalleryGroupFilter filter = e.NewValue as GalleryGroupFilter;
            if(filter!=null)
            {
                gallery.SelectedFilterTitle = filter.Title;
                gallery.SelectedFilterGroups = filter.Groups;
            }
            else
            {
                gallery.SelectedFilterTitle = "";
                gallery.SelectedFilterGroups = null;
            }
            gallery.UpdateLayout();
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
            DependencyProperty.RegisterReadOnly("SelectedFilterTitle", typeof(string),
            typeof(Gallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilterTitle. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterTitleProperty = SelectedFilterTitlePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets selected filter groups
        /// </summary>
        public string SelectedFilterGroups
        {
            get { return (string)GetValue(SelectedFilterGroupsProperty); }
            private set { SetValue(SelectedFilterGroupsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SelectedFilterGroupsPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedFilterGroups", typeof(string),
            typeof(Gallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilterGroups. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterGroupsProperty = SelectedFilterGroupsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether gallery has selected filter
        /// </summary>       
        public bool HasFilter
        {
            get { return (bool)GetValue(HasFilterProperty); }
            private set { SetValue(HasFilterPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey HasFilterPropertyKey = DependencyProperty.RegisterReadOnly("HasFilter", typeof(bool), typeof(Gallery), new UIPropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasFilter.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasFilterProperty = HasFilterPropertyKey.DependencyProperty;

        void OnFilterMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem senderItem = (MenuItem)sender;
            MenuItem item = GetFilterMenuItem(SelectedFilter);
            item.IsChecked = false;
            senderItem.IsChecked = true;
            SelectedFilter = senderItem.Tag as GalleryGroupFilter;
            groupsMenuButton.IsDropDownOpen = false;
            e.Handled = true;
        }

        MenuItem GetFilterMenuItem(GalleryGroupFilter filter)
        {
            if (filter == null) return null;
            return groupsMenuButton.Items.Cast<MenuItem>().FirstOrDefault(item => (item != null) && (item.Header.ToString() == filter.Title));
            /*foreach (MenuItem item in groupsMenuButton.Items)
            {
                if ((item!=null)&&(item.Header == filter.Title)) return item;
            }
            return null;*/
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
            typeof(Gallery), new UIPropertyMetadata(true, OnSelectableChanged));

        private static void OnSelectableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(SelectedItemProperty);
        }

        #endregion

        #region IsLastItem

        /// <summary>
        /// Gets whether gallery is last item in ItemsControl
        /// </summary>
        public bool IsLastItem
        {
            get { return (bool)GetValue(IsLastItemProperty); }
            private set { SetValue(IsLastItemPropertyKey, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsLastItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyPropertyKey IsLastItemPropertyKey = DependencyProperty.RegisterReadOnly("IsLastItem", typeof(bool), typeof(Gallery), new UIPropertyMetadata(false));
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsLastItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLastItemProperty = IsLastItemPropertyKey.DependencyProperty;
        
        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Gallery()
        {
            Type type = typeof (Gallery);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(typeof(Gallery)));
            SelectedItemProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, CoerceSelectedItem));
            ContextMenuService.Attach(type);
            StyleProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(Gallery));
            }

            return basevalue;
        }

        // Coerce selected item
        private static object CoerceSelectedItem(DependencyObject d, object basevalue)
        {
            Gallery gallery = (Gallery)d;
            if (!gallery.Selectable)
            {
                GalleryItem galleryItem = (GalleryItem)gallery.ItemContainerGenerator.ContainerFromItem(basevalue);
                if (basevalue != null && galleryItem != null) galleryItem.IsSelected = false;
                return null;
            }
            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Gallery()
        {
            ContextMenuService.Coerce(this);
            Loaded += OnLoaded;
            Focusable = false;
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Continue);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ItemsControl parent = Parent as ItemsControl;
            if (parent != null)
            {
                if (parent.Items.IndexOf(this) == parent.Items.Count - 1) IsLastItem = true;
                else IsLastItem = false;
            }
        }

        #endregion

        #region Methods

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GalleryItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is GalleryItem);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (groupsMenuButton != null) groupsMenuButton.Items.Clear();
            groupsMenuButton = GetTemplateChild("PART_DropDownButton") as DropDownButton;
            if (groupsMenuButton != null)
            {
                for (int i = 0; i < Filters.Count; i++)
                {
                    MenuItem item = new MenuItem();
                    item.Header = Filters[i].Title;
                    item.Tag = Filters[i];
                    if (Filters[i] == SelectedFilter) item.IsChecked = true;
                    item.Click += OnFilterMenuItemClick;
                    groupsMenuButton.Items.Add(item);
                }
            }

            base.OnApplyTemplate();
        }

        #endregion

        /*#region QuickAccess

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {            
            DependencyObject parent = LogicalTreeHelper.GetParent(this);
            while (parent != null)
            {
                IRibbonControl control = parent as IRibbonControl;
                if (control != null) return control as FrameworkElement;
                parent = LogicalTreeHelper.GetParent(parent);
            }
            return null;
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
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(Gallery));

        #endregion*/

        //    #region Fields

        //    // TODO: add comments to the fields here (!)

        //    RibbonListBox listBox;

        //    ObservableCollection<GalleryGroupFilter> filters;

        //    ObservableCollection<GalleryGroupIcon> groupIcons;

        //    DropDownButton groupsMenuButton;

        //    double cachedWidthDelta;


        //    ContextMenu parentContextMenu;
        //    DropDownButton quickAccessButton;

        //    #endregion

        //    #region Properties

        //    #region ItemsInRow

        //    /// <summary>
        //    /// Width of the Gallery 
        //    /// </summary>
        //    public int ItemsInRow
        //    {
        //        get { return (int)GetValue(ItemsInRowProperty); }
        //        set { SetValue(ItemsInRowProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for ItemsInRow.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty ItemsInRowProperty =
        //        DependencyProperty.Register("ItemsInRow", typeof(int),
        //        typeof(Gallery), new UIPropertyMetadata(0, OnItemsInRowChanged));

        //    static void OnItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //    {
        //        ((Gallery)d).SetMinWidth();
        //    }

        //    void SetMinWidth()
        //    {
        //        if (listBox == null) return;

        //        Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //        cachedWidthDelta = DesiredSize.Width - listBox.InnerPanelWidth;
        //        InvalidateMeasure();

        //        if (ItemsInRow != 0) /*MinWidth = 0*///;
        //        //else
        //        {
        //            double w = GetItemWidth();
        //            if (!double.IsNaN(w)) MinWidth = ItemsInRow * w + cachedWidthDelta;
        //        }
        //    }

        //    #endregion

        //    #region View

        //    /// <summary>
        //    /// Gets view of items or itemssource
        //    /// </summary>
        //    public CollectionViewSource View
        //    {
        //        get { return (CollectionViewSource)GetValue(ViewProperty); }
        //        private set { SetValue(ViewPropertyKey, value); }
        //    }

        //    private static readonly DependencyPropertyKey ViewPropertyKey =
        //        DependencyProperty.RegisterReadOnly("View", typeof(CollectionViewSource),
        //        typeof(Gallery), new UIPropertyMetadata(null));

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for View.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;

        //    #endregion

        //    #region ScrollBarsVisibility

        //    /// <summary> 
        //    /// HorizonalScollbarVisibility is a Windows.Controls.ScrollBarVisibility that
        //    /// determines if a horizontal scrollbar is shown. 
        //    /// </summary> 
        //    [Bindable(true), Category("Appearance")]
        //    public ScrollBarVisibility HorizontalScrollBarVisibility
        //    {
        //        get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
        //        set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
        //        DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility),
        //        typeof(Gallery), new UIPropertyMetadata(ScrollBarVisibility.Disabled));

        //    /// <summary> 
        //    /// VerticalScrollBarVisibility is a System.Windows.Controls.ScrollBarVisibility that 
        //    /// determines if a vertical scrollbar is shown.
        //    /// </summary> 
        //    [Bindable(true), Category("Appearance")]
        //    public ScrollBarVisibility VerticalScrollBarVisibility
        //    {
        //        get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
        //        set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
        //        DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility),
        //        typeof(Gallery), new UIPropertyMetadata(ScrollBarVisibility.Visible));

        //    #endregion

        //    #region GroupBy

        //    /// <summary>
        //    /// Gets or sets name of property which
        //    /// will use to group items in the Gallery.
        //    /// </summary>
        //    public string GroupBy
        //    {
        //        get { return (string)GetValue(GroupByProperty); }
        //        set { SetValue(GroupByProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for GroupBy.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty GroupByProperty =
        //        DependencyProperty.Register("GroupBy", typeof(string), typeof(Gallery),
        //        new UIPropertyMetadata(null, OnGroupByChanged));

        //    static void OnGroupByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //    {
        //        ((Gallery)d).UpdateGroupBy(e.NewValue as string);
        //    }

        //    void UpdateGroupBy(string groupBy)
        //    {
        //        if (View != null)
        //        {
        //            View.GroupDescriptions.Clear();
        //            if (groupBy != null)
        //            {
        //                View.GroupDescriptions.Add(new PropertyGroupDescription(groupBy));
        //            }
        //        }
        //    }

        //    #endregion

        //    #region Orientation

        //    /// <summary>
        //    /// Gets or sets orientation of gallery
        //    /// </summary>
        //    public Orientation Orientation
        //    {
        //        get { return (Orientation)GetValue(OrientationProperty); }
        //        set { SetValue(OrientationProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for Orientation.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty OrientationProperty =
        //        DependencyProperty.Register("Orientation", typeof(Orientation),
        //        typeof(Gallery), new UIPropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        //    static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //    {
        //        Gallery gallery = (Gallery)d;

        //        if ((Orientation)e.NewValue == Orientation.Horizontal)
        //        {
        //            ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
        //            template.Seal();
        //            gallery.ItemsPanel = template;
        //        }
        //        else
        //        {
        //            ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
        //            template.Seal();
        //            gallery.ItemsPanel = template;
        //        }
        //    }

        //    #endregion

        //    #region ItemWidth

        //    /// <summary>
        //    /// Gets or sets item width
        //    /// </summary>
        //    public double ItemWidth
        //    {
        //        get { return (double)GetValue(ItemWidthProperty); }
        //        set { SetValue(ItemWidthProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty ItemWidthProperty =
        //        DependencyProperty.Register("ItemWidth", typeof(double), typeof(Gallery), new UIPropertyMetadata(double.NaN));

        //    /// <summary>
        //    /// Gets or sets item height
        //    /// </summary>
        //    public double ItemHeight
        //    {
        //        get { return (double)GetValue(ItemHeightProperty); }
        //        set { SetValue(ItemHeightProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty ItemHeightProperty =
        //        DependencyProperty.Register("ItemHeight", typeof(double), typeof(Gallery), new UIPropertyMetadata(double.NaN));

        //    #endregion

        //    #region Filters

        //    /// <summary>
        //    /// Gets collection of filters
        //    /// </summary>
        //    public ObservableCollection<GalleryGroupFilter> Filters
        //    {
        //        get
        //        {
        //            if (filters == null)
        //            {
        //                filters = new ObservableCollection<GalleryGroupFilter>();
        //                filters.CollectionChanged += OnFilterCollectionChanged;
        //            }
        //            return filters;
        //        }
        //    }

        //    // Handle toolbar iitems changes
        //    void OnFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //    {
        //        UpdateGroupBy(GroupBy);
        //        HasFilter = Filters.Count > 0;
        //        InvalidateProperty(SelectedFilterProperty);

        //        switch (e.Action)
        //        {
        //            case NotifyCollectionChangedAction.Add:
        //                foreach (object item in e.NewItems)
        //                {
        //                    if (groupsMenuButton != null)
        //                    {
        //                        GalleryGroupFilter filter = (GalleryGroupFilter)item;
        //                        MenuItem menuItem = new MenuItem();
        //                        menuItem.Header = filter.Title;
        //                        menuItem.Tag = filter;
        //                        if (filter == SelectedFilter) menuItem.IsChecked = true;
        //                        menuItem.Click += OnFilterMenuItemClick;
        //                        groupsMenuButton.Items.Add(menuItem);
        //                    }
        //                }
        //                break;
        //            case NotifyCollectionChangedAction.Remove:
        //                foreach (object item in e.OldItems)
        //                {
        //                    if (groupsMenuButton != null)
        //                    {
        //                        groupsMenuButton.Items.Remove(GetFilterMenuItem(item as GalleryGroupFilter));
        //                    }
        //                }
        //                break;

        //            case NotifyCollectionChangedAction.Replace:
        //                foreach (object item in e.OldItems)
        //                {
        //                    if (groupsMenuButton != null)
        //                    {
        //                        groupsMenuButton.Items.Remove(GetFilterMenuItem(item as GalleryGroupFilter));
        //                    }
        //                }
        //                foreach (object item in e.NewItems)
        //                {
        //                    if (groupsMenuButton != null)
        //                    {
        //                        GalleryGroupFilter filter = (GalleryGroupFilter)item;
        //                        MenuItem menuItem = new MenuItem();
        //                        menuItem.Header = filter.Title;
        //                        menuItem.Tag = filter;
        //                        if (filter == SelectedFilter) menuItem.IsChecked = true;
        //                        menuItem.Click += OnFilterMenuItemClick;
        //                        groupsMenuButton.Items.Add(menuItem);
        //                    }
        //                }
        //                break;
        //        }
        //    }

        //    /// <summary>
        //    /// Gets or sets selected filter
        //    /// </summary>               
        //    public GalleryGroupFilter SelectedFilter
        //    {
        //        get { return (GalleryGroupFilter)GetValue(SelectedFilterProperty); }
        //        set { SetValue(SelectedFilterProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for SelectedFilter. 
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty SelectedFilterProperty =
        //        DependencyProperty.Register("SelectedFilter", typeof(GalleryGroupFilter),
        //        typeof(Gallery), new UIPropertyMetadata(null, OnFilterChanged, CoerceSelectedFilter));

        //    // Coerce selected filter
        //    static object CoerceSelectedFilter(DependencyObject d, object basevalue)
        //    {
        //        Gallery gallery = (Gallery)d;
        //        if ((basevalue == null) && (gallery.Filters.Count > 0)) return gallery.Filters[0];
        //        return basevalue;
        //    }

        //    // Handles filter property changed
        //    static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //    {
        //        Gallery gallery = (Gallery)d;
        //        gallery.SelectedFilterTitle = e.NewValue != null ? ((GalleryGroupFilter)e.NewValue).Title : "";
        //        if (gallery.View.View != null) gallery.View.View.Refresh();
        //    }

        //    /// <summary>
        //    /// Gets selected filter title
        //    /// </summary>
        //    public string SelectedFilterTitle
        //    {
        //        get { return (string)GetValue(SelectedFilterTitleProperty); }
        //        private set { SetValue(SelectedFilterTitlePropertyKey, value); }
        //    }

        //    private static readonly DependencyPropertyKey SelectedFilterTitlePropertyKey =
        //        DependencyProperty.RegisterReadOnly("SelectedFilterTitle", typeof(string),
        //        typeof(Gallery), new UIPropertyMetadata(null));

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for SelectedFilterTitle. 
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty SelectedFilterTitleProperty = SelectedFilterTitlePropertyKey.DependencyProperty;

        //    /// <summary>
        //    /// Gets whether gallery has selected filter
        //    /// </summary>       
        //    public bool HasFilter
        //    {
        //        get { return (bool)GetValue(HasFilterProperty); }
        //        private set { SetValue(HasFilterPropertyKey, value); }
        //    }

        //    private static readonly DependencyPropertyKey HasFilterPropertyKey =
        //DependencyProperty.RegisterReadOnly("HasFilter", typeof(bool),
        //typeof(Gallery), new UIPropertyMetadata(false));

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for HasFilter.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty HasFilterProperty = HasFilterPropertyKey.DependencyProperty;

        //    void OnFiltering(object obj, FilterEventArgs e)
        //    {
        //        if (string.IsNullOrEmpty(GroupBy)) e.Accepted = true;
        //        else if (SelectedFilter == null) e.Accepted = true;
        //        else
        //        {
        //            string[] filterItems = SelectedFilter.Groups.Split(",".ToCharArray());
        //            e.Accepted = filterItems.Contains(GetItemGroupName(e.Item));
        //        }
        //    }

        //    #endregion

        //    #region Selectable

        //    /// <summary>
        //    /// Gets or sets whether gallery items can be selected
        //    /// </summary>
        //    public bool Selectable
        //    {
        //        get { return (bool)GetValue(SelectableProperty); }
        //        set { SetValue(SelectableProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for Selectable.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty SelectableProperty =
        //        DependencyProperty.Register("Selectable", typeof(bool),
        //        typeof(Gallery), new UIPropertyMetadata(true));

        //    #endregion

        //    #region SelectedIndex

        //    /// <summary>
        //    /// Gets or sets index of currently selected item
        //    /// </summary>
        //    public int SelectedIndex
        //    {
        //        get { return (int)GetValue(SelectedIndexProperty); }
        //        set { SetValue(SelectedIndexProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for SelectedIndex.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty SelectedIndexProperty =
        //        DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Gallery), new UIPropertyMetadata(-1, OnSelectedIndexChanged, CoerceSelectedIndex));

        //    static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //    {
        //        Gallery gallery = (Gallery)d;
        //        object item = gallery.GetItem(gallery.SelectedIndex);
        //        if (gallery.SelectedItem != item) gallery.SelectedItem = item;
        //        if (gallery.listBox != null) if (gallery.SelectedIndex != gallery.listBox.SelectedIndex) gallery.listBox.SelectedIndex = gallery.SelectedIndex;
        //    }

        //    static object CoerceSelectedIndex(DependencyObject d, object basevalue)
        //    {
        //        Gallery gallery = (Gallery)d;
        //        if (!gallery.Selectable)
        //        {
        //            gallery.listBox.SelectedIndex = -1;
        //            return -1;
        //        }
        //        return basevalue;
        //    }

        //    #endregion

        //    #region SelectedItem

        //    /// <summary>
        //    /// Gets or sets currently selected item
        //    /// </summary>
        //    public object SelectedItem
        //    {
        //        get { return (object)GetValue(SelectedItemProperty); }
        //        set { SetValue(SelectedItemProperty, value); }
        //    }

        //    /// <summary>
        //    /// Using a DependencyProperty as the backing store for SelectedItem.  
        //    /// This enables animation, styling, binding, etc...
        //    /// </summary>
        //    public static readonly DependencyProperty SelectedItemProperty =
        //        DependencyProperty.Register("SelectedItem", typeof(object),
        //        typeof(Gallery), new UIPropertyMetadata(null, OnSelectedItemChanged, CoerceSelectedItem));

        //    static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //    {
        //        Gallery gallery = (Gallery)d;
        //        int index = gallery.GetItemIndex(gallery.SelectedItem);
        //        if (gallery.SelectedIndex != index) gallery.SelectedIndex = index;
        //        if (gallery.listBox != null) if (gallery.SelectedItem != gallery.listBox.SelectedItem) gallery.listBox.SelectedItem = gallery.SelectedItem;
        //        if (gallery.SelectionChanged != null) gallery.SelectionChanged(gallery, EventArgs.Empty);
        //    }

        //    static object CoerceSelectedItem(DependencyObject d, object basevalue)
        //    {
        //        Gallery gallery = (Gallery)d;
        //        if (!gallery.Selectable)
        //        {
        //            if ((basevalue != null) && (gallery.listBox != null))
        //            {
        //                ListBoxItem item = ((ListBoxItem)gallery.listBox.ItemContainerGenerator.ContainerFromItem(basevalue));
        //                if (item != null) item.IsSelected = false;
        //                gallery.listBox.SelectedItem = null;
        //            }
        //            return null;
        //        }
        //        return basevalue;
        //    }

        //    #endregion

        //    #region GroupIcons

        //    /// <summary>
        //    /// Gets collection of group icons
        //    /// </summary>
        //    public ObservableCollection<GalleryGroupIcon> GroupIcons
        //    {
        //        get
        //        {
        //            if (groupIcons == null)
        //            {
        //                groupIcons = new ObservableCollection<GalleryGroupIcon>();
        //                groupIcons.CollectionChanged += OnGroupIconCollectionChanged;
        //            }
        //            return groupIcons;
        //        }
        //    }

        //    // Handle toolbar iitems changes
        //    void OnGroupIconCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //    {
        //        UpdateGroupBy(GroupBy);
        //    }

        //    #endregion

        //    #endregion

        //    #region Events

        //    /// <summary>
        //    /// Occurs then selection is changed
        //    /// </summary>
        //    public event EventHandler SelectionChanged;

        //    #endregion

        //    #region Constructors

        //    /// <summary>
        //    /// Static constructor
        //    /// </summary>
        //    [SuppressMessage("Microsoft.Performance", "CA1810")]
        //    static Gallery()
        //    {
        //        DefaultStyleKeyProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(typeof(Gallery)));
        //    }

        //    /// <summary>
        //    /// Default constructor
        //    /// </summary>
        //    public Gallery()
        //    {
        //        View = new CollectionViewSource();
        //        View.Filter += OnFiltering;

        //        ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
        //        template.Seal();
        //        ItemsPanel = template;
        //    }

        //    #endregion

        //    #region Overrides

        //    /*/// <summary>
        //    /// Items collection changes hadling 
        //    /// </summary>
        //    /// <param name="e">Event args</param>
        //    protected override void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        //    {
        //        View.Source = Items;
        //        UpdateGroupBy(GroupBy);
        //    }

        //    /// <summary>
        //    /// ItemsSource property change handling
        //    /// </summary>
        //    /// <param name="args">Event args</param>
        //    protected override void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        //    {
        //        View.Source = ItemsSource;
        //        UpdateGroupBy(GroupBy);
        //    }*/

        //    /// <summary>
        //    /// When overridden in a derived class, is invoked whenever 
        //    /// application code or internal processes call ApplyTemplate
        //    /// </summary>
        //    public override void OnApplyTemplate()
        //    {
        //        groupsMenuButton = GetTemplateChild("PART_DropDownButton") as DropDownButton;
        //        if (groupsMenuButton != null)
        //        {
        //            for (int i = 0; i < Filters.Count; i++)
        //            {
        //                MenuItem item = new MenuItem();
        //                item.Header = Filters[i].Title;
        //                item.Tag = Filters[i];
        //                if (Filters[i] == SelectedFilter) item.IsChecked = true;
        //                item.Click += OnFilterMenuItemClick;
        //                groupsMenuButton.Items.Add(item);
        //            }
        //        }

        //        if (listBox != null)
        //        {
        //            listBox.ItemContainerGenerator.StatusChanged -= OnItemsContainerGeneratorStatusChanged;
        //        }

        //        listBox = GetTemplateChild("PART_ListBox") as RibbonListBox;
        //        if (listBox != null)
        //        {
        //            listBox.SelectedIndex = SelectedIndex;
        //            listBox.SelectedItem = SelectedItem;

        //            listBox.SelectionChanged += OnListBoxSelectionChanged;

        //            /*Binding binding = new Binding("SelectedIndex");
        //            binding.Source = listBox;
        //            binding.Mode = BindingMode.TwoWay;
        //            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //            SetBinding(SelectedIndexProperty, binding);

        //            binding = new Binding("SelectedItem");
        //            binding.Source = listBox;
        //            binding.Mode = BindingMode.TwoWay;
        //            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //            SetBinding(SelectedItemProperty, binding);*/

        //            Binding binding = new Binding("HasItems");
        //            binding.Source = listBox;
        //            binding.Mode = BindingMode.OneWay;
        //            SetBinding(HasItemsProperty, binding);

        //            binding = new Binding("IsGrouping");
        //            binding.Source = listBox;
        //            binding.Mode = BindingMode.OneWay;
        //            SetBinding(IsGroupingProperty, binding);

        //            /*binding = new Binding("ItemContainerGenerator");
        //            binding.Source = listBox;
        //            binding.Mode = BindingMode.OneWay;
        //            SetBinding(ItemContainerGeneratorProperty, binding);*/

        //            listBox.ItemContainerGenerator.StatusChanged += OnItemsContainerGeneratorStatusChanged;

        //        }
        //        UpdateGroupBy(GroupBy);
        //        SetMinWidth();
        //    }

        //    private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        //    {
        //        if (SelectedIndex != listBox.SelectedIndex) SelectedIndex = listBox.SelectedIndex;
        //        if (SelectedItem != listBox.SelectedItem) SelectedItem = listBox.SelectedItem;
        //    }

        //    void OnItemsContainerGeneratorStatusChanged(object sender, EventArgs e)
        //    {

        //    }

        //    void OnFilterMenuItemClick(object sender, RoutedEventArgs e)
        //    {
        //        MenuItem senderItem = (MenuItem)sender;
        //        MenuItem item = GetFilterMenuItem(SelectedFilter);
        //        item.IsChecked = false;
        //        senderItem.IsChecked = true;
        //        SelectedFilter = senderItem.Tag as GalleryGroupFilter;
        //        e.Handled = true;
        //    }

        //    MenuItem GetFilterMenuItem(GalleryGroupFilter filter)
        //    {
        //        /*if (filter == null) */return null;
        //       // return groupsMenuButton.Items.First(x => ((MenuItem)x).Header.ToString() == filter.Title) as MenuItem;
        //    }

        //    #endregion

        //    #region Quick Access Item Creating

        //    /// <summary>
        //    /// Gets control which represents shortcut item.
        //    /// This item MUST be syncronized with the original 
        //    /// and send command to original one control.
        //    /// </summary>
        //    /// <returns>Control which represents shortcut item</returns>
        //    public override FrameworkElement CreateQuickAccessItem()
        //    {
        //        IQuickAccessItemProvider provider = FindParentProvider();
        //        if (provider != null) return provider.CreateQuickAccessItem();

        //        DropDownButton button = new DropDownButton();

        //        button.PreviewMouseLeftButtonDown += OnFirstPreviewClick;

        //        //BindQuickAccessItem(button);
        //        return button;
        //    }

        //    private IQuickAccessItemProvider FindParentProvider()
        //    {
        //        DependencyObject parent = LogicalTreeHelper.GetParent(this);
        //        while (parent != null)
        //        {
        //            IQuickAccessItemProvider provider = (parent as IQuickAccessItemProvider);
        //            if (provider != null) return provider;
        //            parent = LogicalTreeHelper.GetParent(parent);
        //        }
        //        return null;
        //    }

        //    private void OnFirstPreviewClick(object sender, MouseButtonEventArgs e)
        //    {
        //        DropDownButton button = (sender as DropDownButton);
        //        button.PreviewMouseLeftButtonDown -= OnFirstPreviewClick;
        //        CreateMenuIfNeeded();
        //        parentContextMenu = FindContextMenu();
        //        if (parentContextMenu != null)
        //        {
        //            button.PreviewMouseLeftButtonDown += OnQuickAccessClick;
        //        }
        //        OnQuickAccessClick(button, e);
        //    }

        //    void CreateMenuIfNeeded()
        //    {
        //        DependencyObject parent = LogicalTreeHelper.GetParent(this);
        //        while (parent != null)
        //        {
        //            DropDownButton button = (parent as DropDownButton);
        //            if (button != null)
        //            {
        //                //button.DoCreateMenu();
        //                return;
        //            }
        //            parent = LogicalTreeHelper.GetParent(parent);
        //        }
        //        return;
        //    }

        //    void OnQuickAccessClick(object sender, MouseButtonEventArgs e)
        //    {
        //       /* DropDownButton button = (DropDownButton)sender;
        //        button.ResizeMode = parentContextMenu.ResizeMode;
        //        for (int i = 0; i < parentContextMenu.Items.Count; i++)
        //        {
        //            UIElement item = parentContextMenu.Items[0];
        //            parentContextMenu.Items.Remove(item);
        //            button.Items.Add(item);
        //            i--;
        //        }
        //        button.Closed += OnQuickAccessMenuClosed;
        //        quickAccessButton = button;*/
        //    }



        //    void OnQuickAccessMenuClosed(object sender, EventArgs e)
        //    {
        //        /*quickAccessButton.Closed -= OnQuickAccessMenuClosed;
        //        for (int i = 0; i < quickAccessButton.Items.Count; i++)
        //        {
        //            UIElement item = quickAccessButton.Items[0];
        //            quickAccessButton.Items.Remove(item);
        //            parentContextMenu.Items.Add(item);
        //            i--;
        //        }*/
        //    }

        //    ContextMenu FindContextMenu()
        //    {
        //        DependencyObject parent = VisualTreeHelper.GetParent(this);
        //        while (parent != null)
        //        {
        //            ContextMenuBar contextMenuBar = (parent as ContextMenuBar);
        //            if (contextMenuBar != null) return contextMenuBar.ParentContextMenu;
        //            parent = VisualTreeHelper.GetParent(parent);
        //        }
        //        return null;
        //    }

        //    #endregion

        //    #region Private Methods

        //    IEnumerable GetItems()
        //    {
        //        IEnumerable items = ItemsSource;
        //        if (items == null) items = Items;
        //        return items;
        //    }

        //    object GetItem(int index)
        //    {
        //        IEnumerable items = GetItems();
        //        if (items == null) return null;
        //        int i = 0;
        //        foreach (var item in items)
        //        {
        //            if (i == index) return item;
        //            i++;
        //        }
        //        return null;
        //    }

        //    int GetItemIndex(object obj)
        //    {
        //        IEnumerable items = GetItems();
        //        if (items == null) return -1;
        //        int i = 0;
        //        foreach (var item in items)
        //        {
        //            if (item.Equals(obj)) return i;
        //            i++;
        //        }
        //        return -1;
        //    }

        //    internal string GetItemGroupName(object obj)
        //    {
        //        if (obj == null) return null;
        //        PropertyInfo property = obj.GetType().GetProperty(GroupBy, BindingFlags.Public | BindingFlags.Instance);
        //        if (property == null) return null;
        //        object result = property.GetValue(obj, null);
        //        if (result == null) return null;
        //        return result.ToString();
        //    }

        //    private double GetItemWidth()
        //    {
        //        if (double.IsNaN(ItemWidth) && (listBox != null) && (listBox.Items.Count > 0))
        //        {
        //            GalleryItem item = (listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[0]) as GalleryItem);
        //            bool useHack = false;
        //            if (item == null)
        //            {
        //                useHack = true;
        //                RemoveLogicalChild(listBox.Items[0]);
        //                item = new GalleryItem();
        //                item.Width = ItemWidth;
        //                item.Height = ItemHeight;
        //                if (ItemContainerStyle != null) item.Style = ItemContainerStyle;
        //                if (ItemTemplate != null)
        //                {
        //                    item.Content = ItemTemplate;
        //                    item.DataContext = listBox.Items[0];
        //                }
        //                else item.Content = listBox.Items[0];
        //            }
        //            item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //            if (useHack)
        //            {
        //                item.Content = null;
        //                AddLogicalChild(listBox.Items[0]);
        //            }
        //            return item.DesiredSize.Width;
        //        }
        //        return ItemWidth;
        //    }

        //    #endregion
    }
}
