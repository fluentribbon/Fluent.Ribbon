#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
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
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using Fluent.Extensibility;

    /// <summary>
    /// Represents the In-Ribbon Gallery, a gallery-based control that exposes 
    /// a default subset of items directly in the Ribbon. Any remaining items 
    /// are displayed when a drop-down menu button is clicked
    /// </summary>
    [ContentProperty("Items")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506")]
    public class InRibbonGallery : Selector, IScalableRibbonControl, IDropDownControl, IRibbonControl, IQuickAccessItemProvider, IRibbonSizeChangedSink
    {
        #region Fields

        private ObservableCollection<GalleryGroupFilter> filters;

        private ToggleButton expandButton;
        private ToggleButton dropDownButton;

        private Panel menuPanel;

        // Freezed image (created during snapping)
        Image snappedImage;

        // Is visual currently snapped
        private bool isSnapped;

        private Popup popup;

        // Thumb to resize in both directions
        Thumb resizeBothThumb;
        // Thumb to resize vertical
        Thumb resizeVerticalThumb;

        DropDownButton groupsMenuButton;

        private GalleryPanel galleryPanel;

        private ContentControl controlPresenter;
        private ContentControl popupControlPresenter;

        private ScrollViewer scrollViewer;

        // Needed to prevent drop down reopen 
        private bool canOpenDropDown = true;

        private IInputElement focusedElement;

        private bool isButtonClicked;

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
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(InRibbonGallery));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(InRibbonGallery));

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
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(InRibbonGallery));

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(InRibbonGallery));

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
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(InRibbonGallery));

        #endregion

        #region MinItemsInDropDownRow

        /// <summary>
        /// Min width of the Gallery 
        /// </summary>
        public int MinItemsInDropDownRow
        {
            get { return (int)GetValue(MinItemsInDropDownRowProperty); }
            set { SetValue(MinItemsInDropDownRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinItemsInDropDownRow.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinItemsInDropDownRowProperty =
            DependencyProperty.Register("MinItemsInDropDownRow", typeof(int), typeof(InRibbonGallery), new UIPropertyMetadata(1));

        #endregion

        #region MaxItemsInDropDownRow

        /// <summary>
        /// Max width of the Gallery 
        /// </summary>
        public int MaxItemsInDropDownRow
        {
            get { return (int)GetValue(MaxItemsInDropDownRowProperty); }
            set { SetValue(MaxItemsInDropDownRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxItemsInDropDownRow.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxItemsInDropDownRowProperty =
            DependencyProperty.Register("MaxItemsInDropDownRow", typeof(int), typeof(InRibbonGallery), new UIPropertyMetadata(Int32.MaxValue));

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
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(InRibbonGallery), new UIPropertyMetadata(Orientation.Horizontal));

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
        private void OnFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasFilter = Filters.Count > 0;
            InvalidateProperty(SelectedFilterProperty);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.OfType<GalleryGroupFilter>())
                    {
                        if (groupsMenuButton != null)
                        {
                            GalleryGroupFilter filter = item;
                            MenuItem menuItem = new MenuItem();
                            menuItem.Header = filter.Title;
                            menuItem.Tag = filter;
                            menuItem.IsDefinitive = false;
                            if (filter == SelectedFilter) menuItem.IsChecked = true;
                            menuItem.Click += OnFilterMenuItemClick;
                            groupsMenuButton.Items.Add(menuItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<GalleryGroupFilter>())
                    {
                        if (groupsMenuButton != null)
                        {
                            groupsMenuButton.Items.Remove(GetFilterMenuItem(item));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems.OfType<GalleryGroupFilter>())
                    {
                        if (groupsMenuButton != null)
                        {
                            groupsMenuButton.Items.Remove(GetFilterMenuItem(item));
                        }
                    }
                    foreach (var item in e.NewItems.OfType<GalleryGroupFilter>())
                    {
                        if (groupsMenuButton != null)
                        {
                            GalleryGroupFilter filter = item;
                            MenuItem menuItem = new MenuItem();
                            menuItem.Header = filter.Title;
                            menuItem.Tag = filter;
                            menuItem.IsDefinitive = false;
                            if (filter == SelectedFilter) menuItem.IsChecked = true;
                            menuItem.Click += OnFilterMenuItemClick;
                            groupsMenuButton.Items.Add(menuItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:

                    if (groupsMenuButton != null)
                    {
                        groupsMenuButton.Items.Clear();
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
            typeof(InRibbonGallery), new UIPropertyMetadata(null, OnFilterChanged, CoerceSelectedFilter));

        // Coerce selected filter
        static object CoerceSelectedFilter(DependencyObject d, object basevalue)
        {
            InRibbonGallery gallery = (InRibbonGallery)d;
            if ((basevalue == null) && (gallery.Filters.Count > 0)) return gallery.Filters[0];
            return basevalue;
        }

        // Handles filter property changed
        static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InRibbonGallery gallery = (InRibbonGallery)d;
            GalleryGroupFilter oldFilter = e.OldValue as GalleryGroupFilter;
            if (oldFilter != null)
            {
                System.Windows.Controls.MenuItem menuItem = gallery.GetFilterMenuItem(oldFilter);
                if (menuItem != null) menuItem.IsChecked = false;
            }
            GalleryGroupFilter filter = e.NewValue as GalleryGroupFilter;
            if (filter != null)
            {
                gallery.SelectedFilterTitle = filter.Title;
                gallery.SelectedFilterGroups = filter.Groups;
                System.Windows.Controls.MenuItem menuItem = gallery.GetFilterMenuItem(filter);
                if (menuItem != null) menuItem.IsChecked = true;
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
            typeof(InRibbonGallery), new UIPropertyMetadata(null));

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
            typeof(InRibbonGallery), new UIPropertyMetadata(null));

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

        private static readonly DependencyPropertyKey HasFilterPropertyKey = DependencyProperty.RegisterReadOnly("HasFilter", typeof(bool), typeof(InRibbonGallery), new UIPropertyMetadata(false));

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
            if (groupsMenuButton == null) return null;
            return groupsMenuButton.Items.Cast<MenuItem>().FirstOrDefault(item => (item != null) && (item.Header.ToString() == filter.Title));
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
            typeof(InRibbonGallery), new UIPropertyMetadata(true, OnSelectableChanged));

        private static void OnSelectableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(SelectedItemProperty);
        }

        #endregion

        #region IsDropDownOpen

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
            DependencyProperty.Register("IsDropDownOpen", typeof(bool),
            typeof(InRibbonGallery), new UIPropertyMetadata(false));

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
        public bool IsSnapped
        {
            get
            {
                return isSnapped;
            }
            set
            {
                if (value == isSnapped) return;
                if (IsCollapsed) return;

                if (!IsVisible) return;

                if ((value) && (((int)ActualWidth > 0) && ((int)ActualHeight > 0)))
                {

                    // Render the freezed image
                    RenderOptions.SetBitmapScalingMode(snappedImage, BitmapScalingMode.NearestNeighbor);
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)galleryPanel.ActualWidth,
                                                                                   (int)galleryPanel.ActualHeight, 96, 96,
                                                                                   PixelFormats.Pbgra32);
                    renderTargetBitmap.Render(galleryPanel);
                    snappedImage.Source = renderTargetBitmap;
                    snappedImage.FlowDirection = FlowDirection;
                    snappedImage.Width = galleryPanel.ActualWidth;
                    snappedImage.Height = galleryPanel.ActualHeight;
                    snappedImage.Visibility = Visibility.Visible;
                    isSnapped = value;
                }
                else
                {
                    snappedImage.Visibility = Visibility.Collapsed;
                    isSnapped = value;
                    InvalidateVisual();
                }

                InvalidateVisual();
            }
        }

        #endregion

        #region Menu

        /// <summary>
        /// Gets or sets menu to show in combo box bottom
        /// </summary>
        public RibbonMenu Menu
        {
            get { return (RibbonMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Menu.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register("Menu", typeof(RibbonMenu), typeof(InRibbonGallery), new UIPropertyMetadata(null));

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
                DependencyProperty.Register("MaxItemsInRow", typeof(int), typeof(InRibbonGallery), new UIPropertyMetadata(8, OnMaxItemsInRowChanged));

        /// <summary>
        /// Gets or sets min count of items in row
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)GetValue(MinItemsInRowProperty); }
            set { SetValue(MinItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxItemsInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinItemsInRowProperty =
                DependencyProperty.Register("MinItemsInRow", typeof(int), typeof(InRibbonGallery), new UIPropertyMetadata(1));

        private static void OnMaxItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InRibbonGallery gal = d as InRibbonGallery;
            int minItemsInRow = (int)e.NewValue;
            if (!gal.IsDropDownOpen && (gal.galleryPanel != null) && (gal.galleryPanel.MinItemsInRow < minItemsInRow))
            {
                gal.galleryPanel.MinItemsInRow = minItemsInRow;
                gal.galleryPanel.MaxItemsInRow = minItemsInRow;
            }
        }

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
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(InRibbonGallery), new UIPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0));

        #endregion

        #region MaxDropDownWidth

        /// <summary>
        /// Get or sets max width of drop down popup
        /// </summary>
        public double MaxDropDownWidth
        {
            get { return (double)GetValue(MaxDropDownWidthProperty); }
            set { SetValue(MaxDropDownWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDropDownWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDropDownWidthProperty =
            DependencyProperty.Register("MaxDropDownWidth", typeof(double), typeof(InRibbonGallery), new UIPropertyMetadata(SystemParameters.PrimaryScreenWidth / 3.0));

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
        /// /Using a DependencyProperty as the backing store for DropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register("DropDownHeight", typeof(double), typeof(InRibbonGallery), new UIPropertyMetadata(double.NaN));

        #endregion

        #region DropDownWidth

        /// <summary>
        /// Gets or sets initial dropdown width
        /// </summary>
        public double DropDownWidth
        {
            get { return (double)GetValue(DropDownWidthProperty); }
            set { SetValue(DropDownWidthProperty, value); }
        }

        /// <summary>
        /// /Using a DependencyProperty as the backing store for DropDownWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownWidthProperty =
            DependencyProperty.Register("DropDownWidth", typeof(double), typeof(InRibbonGallery), new UIPropertyMetadata(double.NaN));

        #endregion

        #region ShowPopupOnTop

        /// <summary>
        /// Gets a value indicating whether popup is shown on top;
        /// </summary>
        public bool ShowPopupOnTop
        {
            get { return (bool)GetValue(ShowPopupOnTopProperty); }
            private set { SetValue(ShowPopupOnTopPropertyKey, value); }
        }

        // 
        private static readonly DependencyPropertyKey ShowPopupOnTopPropertyKey = DependencyProperty.RegisterReadOnly("ShowPopupOnTop", typeof(bool), typeof(InRibbonGallery), new UIPropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowPopupOnTop.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowPopupOnTopProperty = ShowPopupOnTopPropertyKey.DependencyProperty;

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when control is scaled
        /// </summary>
        public event EventHandler Scaled;

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
        static InRibbonGallery()
        {
            Type type = typeof(InRibbonGallery);
            ToolTipService.Attach(type);
            PopupService.Attach(type);
            ContextMenuService.Attach(type);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            StyleProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            SelectedItemProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, CoerceSelectedItem));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(InRibbonGallery));
            }

            return basevalue;
        }

        // Coerce selected item
        private static object CoerceSelectedItem(DependencyObject d, object basevalue)
        {
            InRibbonGallery gallery = (InRibbonGallery)d;
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
        public InRibbonGallery()
        {
            ContextMenuService.Coerce(this);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public void OnKeyTipPressed()
        {
            IsDropDownOpen = true;
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public void OnKeyTipBack()
        {
            this.IsDropDownOpen = false;
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                GalleryItem itemContainer = (ItemContainerGenerator.ContainerFromItem(item) as GalleryItem);
                if (itemContainer != null) itemContainer.IsSelected = false;
            }

            foreach (var item in e.AddedItems)
            {
                GalleryItem itemContainer = (ItemContainerGenerator.ContainerFromItem(item) as GalleryItem);
                if (itemContainer != null) itemContainer.IsSelected = true;
            }
            //if (IsDropDownOpen) IsDropDownOpen = false;
            base.OnSelectionChanged(e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application 
        /// code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (expandButton != null) expandButton.Click -= OnExpandClick;
            expandButton = GetTemplateChild("PART_ExpandButton") as ToggleButton;
            if (expandButton != null) expandButton.Click += OnExpandClick;

            if (dropDownButton != null) dropDownButton.Click -= OnDropDownClick;
            dropDownButton = GetTemplateChild("PART_DropDownButton") as ToggleButton;
            if (dropDownButton != null) dropDownButton.Click += OnDropDownClick;

            if (popup != null)
            {
                popup.Opened -= OnDropDownOpened;
                popup.Closed -= OnDropDownClosed;

                popup.PreviewMouseLeftButtonUp -= OnPopupPreviewMouseUp;
                popup.PreviewMouseLeftButtonDown -= OnPopupPreviewMouseDown;
            }

            popup = GetTemplateChild("PART_Popup") as Popup;

            if (popup != null)
            {
                popup.Opened += OnDropDownOpened;
                popup.Closed += OnDropDownClosed;

                popup.PreviewMouseLeftButtonUp += OnPopupPreviewMouseUp;
                popup.PreviewMouseLeftButtonDown += OnPopupPreviewMouseDown;

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

            menuPanel = GetTemplateChild("PART_MenuPanel") as Panel;

            if (groupsMenuButton != null) groupsMenuButton.Items.Clear();
            groupsMenuButton = GetTemplateChild("PART_FilterDropDownButton") as DropDownButton;
            if (groupsMenuButton != null)
            {
                for (int i = 0; i < Filters.Count; i++)
                {
                    MenuItem item = new MenuItem();
                    item.Header = Filters[i].Title;
                    item.Tag = Filters[i];
                    item.IsDefinitive = false;
                    if (Filters[i] == SelectedFilter) item.IsChecked = true;
                    item.Click += OnFilterMenuItemClick;
                    groupsMenuButton.Items.Add(item);
                }
            }

			this.galleryPanel = this.GetTemplateChild("PART_GalleryPanel") as GalleryPanel;

            if (this.galleryPanel != null)
            {
				this.galleryPanel.MinItemsInRow = this.MaxItemsInRow;
				this.galleryPanel.MaxItemsInRow = this.MaxItemsInRow;
            }

            snappedImage = GetTemplateChild("PART_FakeImage") as Image;

            controlPresenter = GetTemplateChild("PART_ContentPresenter") as ContentControl;
            popupControlPresenter = GetTemplateChild("PART_PopupContentPresenter") as ContentControl;

            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
        }

        private void OnPopupPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Ignore mouse up when mouse donw is on expand button
            if (isButtonClicked)
            {
                isButtonClicked = false;
                e.Handled = true;
            }
        }

        private void OnPopupPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isButtonClicked = false;
        }

        private void OnExpandClick(object sender, RoutedEventArgs e)
        {
            isButtonClicked = true;
        }

        private void OnDropDownClick(object sender, RoutedEventArgs e)
        {
            if (canOpenDropDown) IsDropDownOpen = true;
        }

        // Handles drop down opened
        void OnDropDownClosed(object sender, EventArgs e)
        {
            galleryPanel.Width = Double.NaN;
            galleryPanel.IsGrouped = false;
            galleryPanel.MinItemsInRow = this.MinItemsInRow;
            galleryPanel.MaxItemsInRow = this.MaxItemsInRow;
            galleryPanel.UpdateMinAndMaxWidth();
            
            popupControlPresenter.Content = null;
            controlPresenter.Content = galleryPanel;
            Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)(() =>
            {
                if ((quickAccessGallery == null) || ((quickAccessGallery != null) && (!quickAccessGallery.IsDropDownOpen)))
                {
                    IsSnapped = false;
                }
            }));

            //snappedImage.Visibility = Visibility.Collapsed;            
            if (DropDownClosed != null) DropDownClosed(this, e);
            if (Mouse.Captured == this) Mouse.Capture(null);
            Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)(() =>
                                                                               {
                                                                                   GalleryItem selectedContainer = ItemContainerGenerator.ContainerFromItem(SelectedItem) as GalleryItem;
                                                                                   if (selectedContainer != null) selectedContainer.BringIntoView();

                                                                               }));
            dropDownButton.IsChecked = false;
            canOpenDropDown = true;
        }

        // Handles drop down closed
        private void OnDropDownOpened(object sender, EventArgs e)
        {
            this.IsSnapped = true;            

            this.controlPresenter.Content = null;
            this.popupControlPresenter.Content = this.galleryPanel;
            this.galleryPanel.Width = double.NaN;
            this.scrollViewer.Height = double.NaN;

            if (this.DropDownOpened != null)
            {
                this.DropDownOpened(this, e);
            }

            this.galleryPanel.MinItemsInRow = this.MinItemsInDropDownRow;
            this.galleryPanel.MaxItemsInRow = this.MaxItemsInDropDownRow;
            this.galleryPanel.UpdateMinAndMaxWidth();

            this.galleryPanel.IsGrouped = true;
            this.dropDownButton.IsChecked = true;
            this.canOpenDropDown = false;

            Mouse.Capture(this, CaptureMode.SubTree);

            this.focusedElement = Keyboard.FocusedElement;
            Debug.WriteLine("Focused element - " + this.focusedElement);

            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus += this.OnFocusedElementLostKeyboardFocus;
                this.focusedElement.PreviewKeyDown += this.OnFocusedElementPreviewKeyDown;
            }

            //if (ResizeMode != ContextMenuResizeMode.None)
            {
                this.scrollViewer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                var initialHeight = Math.Min(RibbonControl.GetControlWorkArea(this).Height, this.MaxDropDownHeight);                

                if (!double.IsNaN(this.DropDownHeight))
                {
                    initialHeight = Math.Min(this.DropDownHeight, this.MaxDropDownHeight);
                }

                var initialWidth = Math.Min(RibbonControl.GetControlWorkArea(this).Height, this.MaxDropDownWidth);

                if (!double.IsNaN(this.DropDownWidth))
                {
                    initialWidth = Math.Min(this.DropDownWidth, this.MaxDropDownWidth);
                }

                double menuHeight = 0;
                double menuWidth = 0;

                if (this.Menu != null)
                {
                    this.Menu.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    menuHeight = this.Menu.DesiredSize.Height;
                    menuWidth = this.Menu.DesiredSize.Width;
                }

                if (this.scrollViewer.DesiredSize.Height > initialHeight)
                {
                    this.scrollViewer.Height = initialHeight - menuHeight;

                    if (this.scrollViewer.Height < this.galleryPanel.GetItemSize().Height)
                    {
                        this.scrollViewer.Height = this.galleryPanel.GetItemSize().Height;
                    }
                }

                if (this.scrollViewer.DesiredSize.Width > initialWidth)
                {
                    this.scrollViewer.Width = initialWidth - menuWidth;

                    if (this.scrollViewer.Width < this.galleryPanel.GetItemSize().Width)
                    {
                        this.scrollViewer.Width = this.galleryPanel.GetItemSize().Width;
                    }
                }
            }
        }

        /// <summary>
        /// Handles size property changing
        /// </summary>
        /// <param name="previous">Previous value</param>
        /// <param name="current">Current value</param>
        public void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
            if (this.CanCollapseToButton)
            {
                if (current == RibbonControlSize.Large
                    && this.galleryPanel.MinItemsInRow > this.MinItemsInRow)
                {
                    this.IsCollapsed = false;
                }
                else
                {
                    this.IsCollapsed = true;
                }
            }
            else
            {
                this.IsCollapsed = false;
            }
        }

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
            return item is GalleryItem;
        }

        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.IsDropDownOpen = false;
            }

            base.OnKeyDown(e);
        }

        private void OnFocusedElementPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.IsDropDownOpen = false;
            }
        }

        private void OnFocusedElementLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            focusedElement.LostKeyboardFocus -= OnFocusedElementLostKeyboardFocus;
            focusedElement.PreviewKeyDown -= OnFocusedElementPreviewKeyDown;
        }

        #endregion

        #region Private Methods

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(this.scrollViewer.Height))
            {
                this.scrollViewer.Height = this.scrollViewer.ActualHeight;
            }

            this.scrollViewer.Height = Math.Max(0, Math.Min(Math.Max(this.galleryPanel.GetItemSize().Height, this.scrollViewer.Height + e.VerticalChange), this.MaxDropDownHeight));

            this.menuPanel.Width = Double.NaN;

            if (Double.IsNaN(this.galleryPanel.Width))
            {
                this.galleryPanel.Width = this.galleryPanel.ActualWidth;
            }

            this.galleryPanel.Width = Math.Max(0, this.galleryPanel.Width + e.HorizontalChange);

        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(this.scrollViewer.Height))
            {
                this.scrollViewer.Height = this.scrollViewer.ActualHeight;
            }

            this.scrollViewer.Height = Math.Max(0, Math.Min(Math.Max(this.galleryPanel.GetItemSize().Height, this.scrollViewer.Height + e.VerticalChange), this.MaxDropDownHeight));
        }

        #endregion

        #region QuickAccess

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var gallery = new InRibbonGallery();
            RibbonControl.BindQuickAccessItem(this, gallery);
            RibbonControl.Bind(this, gallery, "GroupBy", InRibbonGallery.GroupByProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "ItemHeight", InRibbonGallery.ItemHeightProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "ItemWidth", InRibbonGallery.ItemWidthProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "ResizeMode", InRibbonGallery.ResizeModeProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "MinItemsInDropDownRow", InRibbonGallery.MinItemsInDropDownRowProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "MaxItemsInDropDownRow", InRibbonGallery.MaxItemsInDropDownRowProperty, BindingMode.OneWay);

            RibbonControl.Bind(this, gallery, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "GroupStyleSelector", GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "ItemsPanel", ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "ItemStringFormat", ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "ItemTemplate", ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "SelectedValuePath", SelectedValuePathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "MaxDropDownWidth", MaxDropDownWidthProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.OneWay);

            gallery.DropDownOpened += OnQuickAccessOpened;
            if (DropDownClosed != null) gallery.DropDownClosed += DropDownClosed;
            if (DropDownOpened != null) gallery.DropDownOpened += DropDownOpened;

            RibbonProperties.SetSize(gallery, RibbonControlSize.Small);
            quickAccessGallery = gallery;
            return gallery;
        }


        private object selectedItem;
        private InRibbonGallery quickAccessGallery;
        void OnQuickAccessOpened(object sender, EventArgs e)
        {

            for (int i = 0; i < Filters.Count; i++) quickAccessGallery.Filters.Add(Filters[i]);
            quickAccessGallery.SelectedFilter = SelectedFilter;
            quickAccessGallery.DropDownClosed += OnQuickAccessMenuClosed;
            UpdateLayout();
            Dispatcher.BeginInvoke(DispatcherPriority.Render, ((ThreadStart)(() =>
               {
                   Freeze();
               }
               )));
        }

        void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            quickAccessGallery.DropDownClosed -= OnQuickAccessMenuClosed;
            SelectedFilter = quickAccessGallery.SelectedFilter;
            quickAccessGallery.Filters.Clear();
            Unfreeze();

        }

        private void Freeze()
        {
            IsSnapped = true;
            selectedItem = SelectedItem;
            SelectedItem = null;
            if (ItemsSource != null)
            {
                quickAccessGallery.ItemsSource = ItemsSource;
                ItemsSource = null;
            }
            else
            {

                for (int i = 0; i < Items.Count; i++)
                {
                    object item = Items[0];
                    Items.Remove(item);
                    quickAccessGallery.Items.Add(item);
                    i--;
                }

            }
            quickAccessGallery.SelectedItem = selectedItem;
            quickAccessGallery.Menu = Menu;
            Menu = null;
            //quickAccessGallery.IsSnapped = false;
        }

        private void Unfreeze()
        {
            this.selectedItem = quickAccessGallery.SelectedItem;
            //quickAccessGallery.IsSnapped = true;
            this.quickAccessGallery.SelectedItem = null;

            if (this.quickAccessGallery.ItemsSource != null)
            {
                this.ItemsSource = this.quickAccessGallery.ItemsSource;
                this.quickAccessGallery.ItemsSource = null;
            }
            else
            {
                for (var i = 0; i < this.quickAccessGallery.Items.Count; i++)
                {
                    var item = this.quickAccessGallery.Items[0];
                    this.quickAccessGallery.Items.Remove(item);
                    this.Items.Add(item);
                    i--;
                }
            }

            this.SelectedItem = this.selectedItem;
            this.Menu = this.quickAccessGallery.Menu;
            this.quickAccessGallery.Menu = null;

            if (!this.IsDropDownOpen)
            {
                if (this.controlPresenter != null)
                {
                    this.controlPresenter.Content = null;
                }

                if (this.popupControlPresenter != null)
                {
                    this.popupControlPresenter.Content = this.galleryPanel;
                }

                if (this.galleryPanel != null)
                {
                    this.galleryPanel.IsGrouped = true;
                    this.galleryPanel.IsGrouped = false;
                }

                if (this.popupControlPresenter != null)
                {
                    this.popupControlPresenter.Content = null;
                }

                if (this.controlPresenter != null)
                {
                    this.controlPresenter.Content = this.galleryPanel;
                }
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, ((ThreadStart)(() =>
                                                                                  {
                                                                                      if (!this.IsDropDownOpen)
                                                                                      {
                                                                                          this.IsSnapped = false;
                                                                                      }
                                                                                      var selectedContainer = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as GalleryItem;
                                                                                      if (selectedContainer != null) selectedContainer.BringIntoView();
                                                                                  })));
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
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(InRibbonGallery), new UIPropertyMetadata(true, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        #endregion

        #region Implementation of IScalableRibbonControl

        /// <summary>
        /// Enlarge control size
        /// </summary>
        public void Enlarge()
        {
            /*currentItemsInRow++;

            if ((CanCollapseToButton) && (CurrentItemsInRow >= MinItemsInRow) && (Size == RibbonControlSize.Large)) IsCollapsed = false;

            InvalidateMeasure();*/
            if (IsCollapsed && (RibbonProperties.GetSize(this) == RibbonControlSize.Large)) IsCollapsed = false;
            else if (galleryPanel.MinItemsInRow < MaxItemsInRow)
            {
                galleryPanel.MinItemsInRow++;
                galleryPanel.MaxItemsInRow = galleryPanel.MinItemsInRow;
            }
            else return;
            InvalidateMeasure();
            //UpdateLayout();
            if (Scaled != null) Scaled(this, EventArgs.Empty);
        }

        /// <summary>
        /// Reduce control size
        /// </summary>
        public void Reduce()
        {
            if (galleryPanel.MinItemsInRow > MinItemsInRow)
            {
                galleryPanel.MinItemsInRow--;
                galleryPanel.MaxItemsInRow = galleryPanel.MinItemsInRow;
            }
            else if (CanCollapseToButton && !IsCollapsed) IsCollapsed = true;
            else return;
            InvalidateMeasure();
            if (Scaled != null) Scaled(this, EventArgs.Empty);
            /*currentItemsInRow--;
            if ((CanCollapseToButton) && (CurrentItemsInRow < MinItemsInRow)) IsCollapsed = true;

            InvalidateMeasure();*/
        }

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
				if (this.galleryPanel != null)
				{
					yield return this.galleryPanel;
				}
			}
		}
	}
}