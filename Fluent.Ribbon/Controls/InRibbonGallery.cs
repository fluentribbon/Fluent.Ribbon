// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using Fluent.Extensibility;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;
    using JetBrains.Annotations;

    /// <summary>
    /// Represents the In-Ribbon Gallery, a gallery-based control that exposes
    /// a default subset of items directly in the Ribbon. Any remaining items
    /// are displayed when a drop-down menu button is clicked
    /// </summary>
    [ContentProperty(nameof(Items))]
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ExpandButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_DropDownButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ResizeVerticalThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ResizeBothThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_MenuPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_FilterDropDownButton", Type = typeof(DropDownButton))]
    [TemplatePart(Name = "PART_GalleryPanel", Type = typeof(GalleryPanel))]
    [TemplatePart(Name = "PART_FakeImage", Type = typeof(Image))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_PopupContentPresenter", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]

    [TemplatePart(Name = "PART_PopupMenuPresenter", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_PopupResizeBorder", Type = typeof(FrameworkElement))]
    public class InRibbonGallery : Selector, IScalableRibbonControl, IDropDownControl, IRibbonControl, IQuickAccessItemProvider, IRibbonSizeChangedSink, ILargeIconProvider
    {
        #region Fields

        private ObservableCollection<GalleryGroupFilter> filters;

        private ToggleButton expandButton;
        private ToggleButton dropDownButton;

        private Panel menuPanel;

        // Freezed image (created during snapping)
        private Image snappedImage;

        // Is visual currently snapped
        private bool isSnapped;

        // Thumb to resize in both directions
        private Thumb resizeBothThumb;

        // Thumb to resize vertical
        private Thumb resizeVerticalThumb;

        private DropDownButton groupsMenuButton;

        [CanBeNull]
        private GalleryPanel galleryPanel;

        private ContentControl controlPresenter;
        private ContentControl popupControlPresenter;

        private ScrollViewer scrollViewer;

        private IInputElement focusedElement;

        private bool isButtonClicked;

        private FrameworkElement layoutRoot;

        private FrameworkElement popupMenuPresenter;
        private FrameworkElement popupResizeBorder;

        [CanBeNull]
        internal GalleryPanelState CurrentGalleryPanelState { get; private set; }

        internal PopupState CurrentPopupState { get; } = new PopupState();

        #endregion

        #region Properties

        #region Size

        /// <inheritdoc />
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(InRibbonGallery));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(InRibbonGallery));

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
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(InRibbonGallery));

        #endregion

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
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(InRibbonGallery), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

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
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(InRibbonGallery), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region MinItemsInDropDownRow

        /// <summary>
        /// Min width of the Gallery
        /// </summary>
        public int MinItemsInDropDownRow
        {
            get { return (int)this.GetValue(MinItemsInDropDownRowProperty); }
            set { this.SetValue(MinItemsInDropDownRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinItemsInDropDownRow.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinItemsInDropDownRowProperty =
            DependencyProperty.Register(nameof(MinItemsInDropDownRow), typeof(int), typeof(InRibbonGallery), new PropertyMetadata(IntBoxes.One));

        #endregion

        #region MaxItemsInDropDownRow

        /// <summary>
        /// Max width of the Gallery
        /// </summary>
        public int MaxItemsInDropDownRow
        {
            get { return (int)this.GetValue(MaxItemsInDropDownRowProperty); }
            set { this.SetValue(MaxItemsInDropDownRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxItemsInDropDownRow.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxItemsInDropDownRowProperty = 
            DependencyProperty.Register(nameof(MaxItemsInDropDownRow), typeof(int), typeof(InRibbonGallery), new PropertyMetadata(IntBoxes.Zero));

        #endregion

        #region ItemWidth

        /// <summary>
        /// Gets or sets item width
        /// </summary>
        public double ItemWidth
        {
            get { return (double)this.GetValue(ItemWidthProperty); }
            set { this.SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(InRibbonGallery), new PropertyMetadata(DoubleBoxes.NaN));

        /// <summary>
        /// Gets or sets item height
        /// </summary>
        public double ItemHeight
        {
            get { return (double)this.GetValue(ItemHeightProperty); }
            set { this.SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(InRibbonGallery), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region GroupBy

        /// <summary>
        /// Gets or sets name of property which
        /// will use to group items in the Gallery.
        /// </summary>
        public string GroupBy
        {
            get { return (string)this.GetValue(GroupByProperty); }
            set { this.SetValue(GroupByProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupBy.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupByProperty = DependencyProperty.Register(nameof(GroupBy), typeof(string), typeof(InRibbonGallery), new PropertyMetadata());

        #endregion

        #region GroupByAdvanced

        /// <summary>
        /// Gets or sets name of property which
        /// will use to group items in the Gallery.
        /// </summary>
        public Func<object, string> GroupByAdvanced
        {
            get { return (Func<object, string>)this.GetValue(GroupByAdvancedProperty); }
            set { this.SetValue(GroupByAdvancedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupBy.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupByAdvancedProperty = DependencyProperty.Register(nameof(GroupByAdvanced), typeof(Func<object, string>), typeof(InRibbonGallery), new PropertyMetadata());

        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets orientation of gallery
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(InRibbonGallery), new PropertyMetadata(Orientation.Horizontal));

        #endregion

        #region Filters

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
                    this.filters.CollectionChanged += this.OnFilterCollectionChanged;
                }

                return this.filters;
            }
        }

        // Handle toolbar items changes
        private void OnFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasFilter = this.Filters.Count > 0;
            this.InvalidateProperty(SelectedFilterProperty);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.OfType<GalleryGroupFilter>())
                    {
                        if (this.groupsMenuButton != null)
                        {
                            var filter = item;
                            var menuItem = new MenuItem
                            {
                                Header = filter.Title,
                                Tag = filter,
                                IsDefinitive = false
                            };

                            if (ReferenceEquals(filter, this.SelectedFilter))
                            {
                                menuItem.IsChecked = true;
                            }

                            menuItem.Click += this.OnFilterMenuItemClick;
                            this.groupsMenuButton.Items.Add(menuItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<GalleryGroupFilter>())
                    {
                        this.groupsMenuButton?.Items.Remove(this.GetFilterMenuItem(item));
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems.OfType<GalleryGroupFilter>())
                    {
                        this.groupsMenuButton?.Items.Remove(this.GetFilterMenuItem(item));
                    }

                    foreach (var item in e.NewItems.OfType<GalleryGroupFilter>())
                    {
                        if (this.groupsMenuButton != null)
                        {
                            var filter = item;
                            var menuItem = new MenuItem
                            {
                                Header = filter.Title,
                                Tag = filter,
                                IsDefinitive = false
                            };

                            if (ReferenceEquals(filter, this.SelectedFilter))
                            {
                                menuItem.IsChecked = true;
                            }

                            menuItem.Click += this.OnFilterMenuItemClick;
                            this.groupsMenuButton.Items.Add(menuItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.groupsMenuButton?.Items.Clear();
                    break;
            }
        }

        /// <summary>
        /// Gets or sets selected filter
        /// </summary>
        public GalleryGroupFilter SelectedFilter
        {
            get { return (GalleryGroupFilter)this.GetValue(SelectedFilterProperty); }
            set { this.SetValue(SelectedFilterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilter.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register(nameof(SelectedFilter), typeof(GalleryGroupFilter), typeof(InRibbonGallery), new PropertyMetadata(null, OnSelectedFilterChanged, CoerceSelectedFilter));

        // Coerce selected filter
        private static object CoerceSelectedFilter(DependencyObject d, object basevalue)
        {
            var gallery = (InRibbonGallery)d;
            if (basevalue == null
                && gallery.Filters.Count > 0)
            {
                return gallery.Filters[0];
            }

            return basevalue;
        }

        // Handles filter property changed
        private static void OnSelectedFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gallery = (InRibbonGallery)d;

            if (e.OldValue is GalleryGroupFilter oldFilter)
            {
                var menuItem = gallery.GetFilterMenuItem(oldFilter);

                if (menuItem != null)
                {
                    menuItem.IsChecked = false;
                }
            }

            if (e.NewValue is GalleryGroupFilter newFilter)
            {
                gallery.SelectedFilterTitle = newFilter.Title;
                gallery.SelectedFilterGroups = newFilter.Groups;
                var menuItem = gallery.GetFilterMenuItem(newFilter);

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
            else
            {
                gallery.SelectedFilterTitle = string.Empty;
                gallery.SelectedFilterGroups = null;
            }

            gallery.UpdateLayout();
        }

        /// <summary>
        /// Gets selected filter title
        /// </summary>
        public string SelectedFilterTitle
        {
            get { return (string)this.GetValue(SelectedFilterTitleProperty); }
            private set { this.SetValue(SelectedFilterTitlePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SelectedFilterTitlePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(SelectedFilterTitle), typeof(string), typeof(InRibbonGallery), new PropertyMetadata());

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
            get { return (string)this.GetValue(SelectedFilterGroupsProperty); }
            private set { this.SetValue(SelectedFilterGroupsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SelectedFilterGroupsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(SelectedFilterGroups), typeof(string), typeof(InRibbonGallery), new PropertyMetadata());

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
            get { return (bool)this.GetValue(HasFilterProperty); }
            private set { this.SetValue(HasFilterPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey HasFilterPropertyKey = DependencyProperty.RegisterReadOnly(nameof(HasFilter), typeof(bool), typeof(InRibbonGallery), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasFilter.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasFilterProperty = HasFilterPropertyKey.DependencyProperty;

        private void OnFilterMenuItemClick(object sender, RoutedEventArgs e)
        {
            var senderItem = (MenuItem)sender;
            var item = this.GetFilterMenuItem(this.SelectedFilter);
            item.IsChecked = false;
            senderItem.IsChecked = true;
            this.SelectedFilter = senderItem.Tag as GalleryGroupFilter;
            this.groupsMenuButton.IsDropDownOpen = false;
            e.Handled = true;
        }

        private MenuItem GetFilterMenuItem(GalleryGroupFilter filter)
        {
            if (filter == null)
            {
                return null;
            }

            return this.groupsMenuButton?.Items.Cast<MenuItem>()
                .FirstOrDefault(item => item != null && item.Header.ToString() == filter.Title);
        }

        #endregion

        #region Selectable

        /// <summary>
        /// Gets or sets whether gallery items can be selected
        /// </summary>
        public bool Selectable
        {
            get { return (bool)this.GetValue(SelectableProperty); }
            set { this.SetValue(SelectableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Selectable.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectableProperty =
            DependencyProperty.Register(nameof(Selectable), typeof(bool),
            typeof(InRibbonGallery), new PropertyMetadata(BooleanBoxes.TrueBox, OnSelectableChanged));

        private static void OnSelectableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(SelectedItemProperty);
        }

        #endregion

        #region IsDropDownOpen

        /// <inheritdoc />
        public Popup DropDownPopup { get; private set; }

        /// <inheritdoc />
        public bool IsContextMenuOpened { get; set; }

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
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(InRibbonGallery), new PropertyMetadata(BooleanBoxes.FalseBox));

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
        /// Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register(nameof(ResizeMode), typeof(ContextMenuResizeMode), typeof(InRibbonGallery), new PropertyMetadata(ContextMenuResizeMode.None));

        #endregion

        #region CanCollapseToButton

        /// <summary>
        /// Gets or sets whether InRibbonGallery
        /// </summary>
        public bool CanCollapseToButton
        {
            get { return (bool)this.GetValue(CanCollapseToButtonProperty); }
            set { this.SetValue(CanCollapseToButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCollapseToButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCollapseToButtonProperty =
            DependencyProperty.Register(nameof(CanCollapseToButton), typeof(bool), typeof(InRibbonGallery), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region IsCollapsed

        /// <summary>
        /// Gets whether InRibbonGallery is collapsed to button
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)this.GetValue(IsCollapsedProperty); }
            set { this.SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(InRibbonGallery), new PropertyMetadata(BooleanBoxes.FalseBox));

        #endregion

        #region LargeIcon

        /// <inheritdoc />
        public object LargeIcon
        {
            get { return this.GetValue(LargeIconProperty); }
            set { this.SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty = LargeIconProviderProperties.LargeIconProperty.AddOwner(typeof(InRibbonGallery), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region Snapping

        /// <summary>
        /// Snaps / Unsnaps the Visual
        /// (remove visuals and substitute with freezed image)
        /// </summary>
        public bool IsSnapped
        {
            get => this.isSnapped;

            private set
            {
                if (value == this.isSnapped)
                {
                    return;
                }

                if (this.IsCollapsed)
                {
                    return;
                }

                if (this.IsVisible == false)
                {
                    return;
                }

                if (value
                    && (int)this.ActualWidth > 0
                    && (int)this.ActualHeight > 0
                    && this.galleryPanel.IsNotNull()
                    && (int)this.galleryPanel.ActualWidth > 0
                    && (int)this.galleryPanel.ActualHeight > 0)
                {
                    // Render the freezed image
                    RenderOptions.SetBitmapScalingMode(this.snappedImage, BitmapScalingMode.NearestNeighbor);

                    var renderTargetBitmap = new RenderTargetBitmap(
                        (int)this.galleryPanel.ActualWidth,
                        (int)this.galleryPanel.ActualHeight,
                        96,
                        96,
                        PixelFormats.Pbgra32);

                    renderTargetBitmap.Render(this.galleryPanel);

                    this.snappedImage.Source = renderTargetBitmap;
                    this.snappedImage.FlowDirection = this.FlowDirection;
                    this.snappedImage.Width = this.galleryPanel.ActualWidth;
                    this.snappedImage.Height = this.galleryPanel.ActualHeight;
                }
                else
                {
                    this.snappedImage.Source = null;
                    this.snappedImage.Width = 0;
                    this.snappedImage.Height = 0;
                }

                this.isSnapped = value;
            }
        }

        /// <summary>
        /// Defines whether this item is frozen or not because the copy of this item shown in the <see cref="QuickAccessToolBar"/> has it's dropdown open.
        /// </summary>
        public bool IsFrozen { get; private set; }

        #endregion

        #region Menu

        /// <summary>
        /// Gets or sets menu to show in combo box bottom
        /// </summary>
        public RibbonMenu Menu
        {
            get { return (RibbonMenu)this.GetValue(MenuProperty); }
            set { this.SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Menu.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(RibbonMenu), typeof(InRibbonGallery), new PropertyMetadata());

        #endregion

        #region Min/Max Sizes

        /// <summary>
        /// Gets or sets max count of items in row
        /// </summary>
        public int MaxItemsInRow
        {
            get { return (int)this.GetValue(MaxItemsInRowProperty); }
            set { this.SetValue(MaxItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxItemsInRow.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxItemsInRowProperty =
                DependencyProperty.Register(nameof(MaxItemsInRow), typeof(int), typeof(InRibbonGallery), new PropertyMetadata(8, OnMaxItemsInRowChanged));

        private static void OnMaxItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gal = (InRibbonGallery)d;
            var maxItemsInRow = (int)e.NewValue;

            if (gal.IsDropDownOpen == false
                && gal.galleryPanel != null)
            {
                gal.galleryPanel.MaxItemsInRow = maxItemsInRow;
            }
        }

        /// <summary>
        /// Gets or sets min count of items in row
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)this.GetValue(MinItemsInRowProperty); }
            set { this.SetValue(MinItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxItemsInRow.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinItemsInRowProperty =
                DependencyProperty.Register(nameof(MinItemsInRow), typeof(int), typeof(InRibbonGallery), new PropertyMetadata(1, OnMinItemsInRowChanged));

        private static void OnMinItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gal = (InRibbonGallery)d;
            var minItemsInRow = (int)e.NewValue;

            if (gal.IsDropDownOpen == false
                && gal.galleryPanel != null)
            {
                gal.galleryPanel.MinItemsInRow = minItemsInRow;
            }
        }

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
            DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(InRibbonGallery), new PropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0));

        #endregion

        #region MaxDropDownWidth

        /// <summary>
        /// Get or sets max width of drop down popup
        /// </summary>
        public double MaxDropDownWidth
        {
            get { return (double)this.GetValue(MaxDropDownWidthProperty); }
            set { this.SetValue(MaxDropDownWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDropDownWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDropDownWidthProperty =
            DependencyProperty.Register(nameof(MaxDropDownWidth), typeof(double), typeof(InRibbonGallery), new PropertyMetadata(SystemParameters.PrimaryScreenWidth / 3.0));

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
        /// /Using a DependencyProperty as the backing store for DropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register(nameof(DropDownHeight), typeof(double), typeof(InRibbonGallery), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region DropDownWidth

        /// <summary>
        /// Gets or sets initial dropdown width
        /// </summary>
        public double DropDownWidth
        {
            get { return (double)this.GetValue(DropDownWidthProperty); }
            set { this.SetValue(DropDownWidthProperty, value); }
        }

        /// <summary>
        /// /Using a DependencyProperty as the backing store for DropDownWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownWidthProperty =
            DependencyProperty.Register(nameof(DropDownWidth), typeof(double), typeof(InRibbonGallery), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region GalleryPanelContainerHeight

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="GalleryPanelContainerHeight"/>.
        /// </summary>
        public static readonly DependencyProperty GalleryPanelContainerHeightProperty = DependencyProperty.Register(nameof(GalleryPanelContainerHeight), typeof(double), typeof(InRibbonGallery), new PropertyMetadata(60D));

        /// <summary>
        /// Gets or sets the height of the container which hosts the <see cref="GalleryPanel"/>.
        /// </summary>
        public double GalleryPanelContainerHeight
        {
            get { return (double)this.GetValue(GalleryPanelContainerHeightProperty); }
            set { this.SetValue(GalleryPanelContainerHeightProperty, value); }
        }

        #endregion

        #endregion

        #region Events

        /// <inheritdoc />
        public event EventHandler Scaled;

        /// <inheritdoc />
        public event EventHandler DropDownOpened;

        /// <inheritdoc />
        public event EventHandler DropDownClosed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="InRibbonGallery"/> class.
        /// </summary>
        static InRibbonGallery()
        {
            var type = typeof(InRibbonGallery);

            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            SelectedItemProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, CoerceSelectedItem));

            ToolTipService.Attach(type);
            PopupService.Attach(type);
            ContextMenuService.Attach(type);
        }

        // Coerce selected item
        private static object CoerceSelectedItem(DependencyObject d, object basevalue)
        {
            var gallery = (InRibbonGallery)d;

            if (gallery.Selectable == false)
            {
                var galleryItem = gallery.ItemContainerGenerator.ContainerOrContainerContentFromItem<GalleryItem>(basevalue);
                if (basevalue != null
                    && galleryItem != null)
                {
                    galleryItem.IsSelected = false;
                }

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

        /// <inheritdoc />
        public KeyTipPressedResult OnKeyTipPressed()
        {
            this.IsDropDownOpen = true;

            if (this.DropDownPopup?.Child != null)
            {
                Keyboard.Focus(this.DropDownPopup.Child);
                this.DropDownPopup.Child.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                return new KeyTipPressedResult(false, true);
            }

            return new KeyTipPressedResult(false, true);
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
            this.IsDropDownOpen = false;
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                if (this.ItemContainerGenerator.ContainerOrContainerContentFromItem<GalleryItem>(item) is GalleryItem itemContainer)
                {
                    itemContainer.IsSelected = false;
                }
            }

            foreach (var item in e.AddedItems)
            {
                if (this.ItemContainerGenerator.ContainerOrContainerContentFromItem<GalleryItem>(item) is GalleryItem itemContainer)
                {
                    itemContainer.IsSelected = this.Selectable;
                }
            }

            base.OnSelectionChanged(e);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.layoutRoot = this.GetTemplateChild("PART_LayoutRoot") as FrameworkElement;

            if (this.expandButton != null)
            {
                this.expandButton.Click -= this.OnExpandClick;
            }

            this.expandButton = this.GetTemplateChild("PART_ExpandButton") as ToggleButton;

            if (this.expandButton != null)
            {
                this.expandButton.Click += this.OnExpandClick;
            }

            this.dropDownButton = this.GetTemplateChild("PART_DropDownButton") as ToggleButton;

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened -= this.OnDropDownOpened;
                this.DropDownPopup.Closed -= this.OnDropDownClosed;

                this.DropDownPopup.PreviewMouseLeftButtonUp -= this.OnPopupPreviewMouseUp;
                this.DropDownPopup.PreviewMouseLeftButtonDown -= this.OnPopupPreviewMouseDown;
            }

            this.DropDownPopup = this.GetTemplateChild("PART_Popup") as Popup;

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened += this.OnDropDownOpened;
                this.DropDownPopup.Closed += this.OnDropDownClosed;

                this.DropDownPopup.PreviewMouseLeftButtonUp += this.OnPopupPreviewMouseUp;
                this.DropDownPopup.PreviewMouseLeftButtonDown += this.OnPopupPreviewMouseDown;

                KeyboardNavigation.SetControlTabNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetTabNavigation(this.DropDownPopup, KeyboardNavigationMode.Cycle);
            }

            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta -= this.OnResizeVerticalDelta;
            }

            this.resizeVerticalThumb = this.GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;

            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta += this.OnResizeVerticalDelta;
            }

            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta -= this.OnResizeBothDelta;
            }

            this.resizeBothThumb = this.GetTemplateChild("PART_ResizeBothThumb") as Thumb;

            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta += this.OnResizeBothDelta;
            }

            this.menuPanel = this.GetTemplateChild("PART_MenuPanel") as Panel;

            this.groupsMenuButton?.Items.Clear();

            this.groupsMenuButton = this.GetTemplateChild("PART_FilterDropDownButton") as DropDownButton;

            if (this.groupsMenuButton != null)
            {
                foreach (var currentFilter in this.Filters)
                {
                    var item = new MenuItem
                    {
                        Header = currentFilter.Title,
                        Tag = currentFilter,
                        IsDefinitive = false
                    };

                    if (ReferenceEquals(currentFilter, this.SelectedFilter))
                    {
                        item.IsChecked = true;
                    }

                    item.Click += this.OnFilterMenuItemClick;
                    this.groupsMenuButton.Items.Add(item);
                }
            }

            this.galleryPanel = this.GetTemplateChild("PART_GalleryPanel") as GalleryPanel;
            
            if (this.galleryPanel.IsNotNull())
            {
                using (new ScopeGuard(this.galleryPanel.SuspendUpdates, this.galleryPanel.ResumeUpdates))
                {
                    this.galleryPanel.MinItemsInRow = this.MinItemsInRow;
                    this.galleryPanel.MaxItemsInRow = this.MaxItemsInRow;
                }

                this.CurrentGalleryPanelState = new GalleryPanelState(this.galleryPanel);
            }
            else
            {
                this.CurrentGalleryPanelState = null;
            }

            this.snappedImage = new Image();

            this.controlPresenter = this.GetTemplateChild("PART_ContentPresenter") as ContentControl;

            this.popupControlPresenter = this.GetTemplateChild("PART_PopupContentPresenter") as ContentControl;

            this.scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            this.popupMenuPresenter = this.GetTemplateChild("PART_PopupMenuPresenter") as FrameworkElement;
            this.popupResizeBorder = this.GetTemplateChild("PART_PopupResizeBorder") as FrameworkElement;
        }

        private void OnPopupPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Ignore mouse up when mouse donw is on expand button
            if (this.isButtonClicked)
            {
                this.isButtonClicked = false;
                e.Handled = true;
            }
        }

        private void OnPopupPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isButtonClicked = false;
        }

        private void OnExpandClick(object sender, RoutedEventArgs e)
        {
            this.isButtonClicked = true;
        }

        // Handles drop down closed
        private void OnDropDownClosed(object sender, EventArgs e)
        {
            this.popupControlPresenter.Content = null;

            this.menuPanel.ClearValue(HeightProperty);

            if (this.galleryPanel.IsNotNull())
            {
                using (new ScopeGuard(this.galleryPanel.SuspendUpdates, this.galleryPanel.ResumeUpdatesRefresh))
                {
                    this.CurrentGalleryPanelState.Restore();

                    this.galleryPanel.IsGrouped = false;

                    this.galleryPanel.ClearValue(WidthProperty);
                }
            }

            if (this.IsSnapped
                && this.IsFrozen == false)
            {
                this.IsSnapped = false;
            }

            this.controlPresenter.Content = this.galleryPanel;

            this.DropDownClosed?.Invoke(this, e);

            if (ReferenceEquals(Mouse.Captured, this))
            {
                Mouse.Capture(null);
            }

            this.RunInDispatcherAsync(() =>
                                      {
                                          var selectedContainer = this.ItemContainerGenerator.ContainerOrContainerContentFromItem<GalleryItem>(this.SelectedItem);
                                          selectedContainer?.BringIntoView();
                                      }, DispatcherPriority.SystemIdle);
        }

        // Handles drop down opened
        private void OnDropDownOpened(object sender, EventArgs e)
        {
            this.IsSnapped = true;

            this.controlPresenter.Content = this.snappedImage;

            if (this.galleryPanel.IsNotNull())
            {
                using (new ScopeGuard(this.galleryPanel.SuspendUpdates, this.galleryPanel.ResumeUpdatesRefresh))
                {
                    this.CurrentGalleryPanelState.Save();

                    this.galleryPanel.MinItemsInRow = this.MinItemsInDropDownRow;
                    this.galleryPanel.MaxItemsInRow = this.MaxItemsInDropDownRow;
                    this.galleryPanel.IsGrouped = true;

                    this.CurrentPopupState.Restore(this.galleryPanel, this.menuPanel);
                }
            }

            this.popupControlPresenter.Content = this.galleryPanel;

            this.DropDownOpened?.Invoke(this, e);

            Mouse.Capture(this, CaptureMode.SubTree);

            this.focusedElement = Keyboard.FocusedElement;

            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus += this.OnFocusedElementLostKeyboardFocus;
                this.focusedElement.PreviewKeyDown += this.OnFocusedElementPreviewKeyDown;
            }

            //if (ResizeMode != ContextMenuResizeMode.None)
            {
                this.scrollViewer.Measure(SizeConstants.Infinite);

                var initialHeight = Math.Min(RibbonControl.GetControlWorkArea(this).Height, this.MaxDropDownHeight);

                if (double.IsNaN(this.DropDownHeight) == false)
                {
                    initialHeight = Math.Min(this.DropDownHeight, this.MaxDropDownHeight);
                }

                var initialWidth = Math.Min(RibbonControl.GetControlWorkArea(this).Height, this.MaxDropDownWidth);

                if (double.IsNaN(this.DropDownWidth) == false)
                {
                    initialWidth = Math.Min(this.DropDownWidth, this.MaxDropDownWidth);
                }

                double menuHeight = 0;
                double menuWidth = 0;

                if (this.Menu != null)
                {
                    this.Menu.Measure(SizeConstants.Infinite);
                    menuHeight = this.Menu.DesiredSize.Height;
                    menuWidth = this.Menu.DesiredSize.Width;
                }

                if (this.scrollViewer.DesiredSize.Height > initialHeight)
                {
                    this.scrollViewer.Height = initialHeight - menuHeight;
                }

                if (this.scrollViewer.DesiredSize.Width > initialWidth)
                {
                    this.scrollViewer.Width = initialWidth - menuWidth;
                }
            }
        }

        /// <inheritdoc />
        public void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
            if (this.CanCollapseToButton)
            {
                if (current == RibbonControlSize.Large
                    && this.galleryPanel?.MinItemsInRow > this.MinItemsInRow)
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

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GalleryItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GalleryItem;
        }

        /// <inheritdoc />
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            // We don't want to notify scaling when items are moved to a different control.
            // This prevents excessive cache invalidation.
            if (ItemsControlHelper.GetIsMovingItemsToDifferentControl(this) == false)
            {
                this.Scaled?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
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
            this.focusedElement.LostKeyboardFocus -= this.OnFocusedElementLostKeyboardFocus;
            this.focusedElement.PreviewKeyDown -= this.OnFocusedElementPreviewKeyDown;
        }

        #endregion

        #region Private Methods

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            this.OnResizeVerticalDelta(sender, e);

            if (DoubleUtil.AreClose(e.HorizontalChange, 0))
            {
                return;
            }

            if (this.galleryPanel.IsNotNull())
            {
                if (double.IsNaN(this.galleryPanel.Width))
                {
                    this.galleryPanel.SetCurrentValue(WidthProperty, this.galleryPanel.ActualWidth);
                }

                var minimumWidth = this.snappedImage.ActualWidth;
                this.galleryPanel.SetCurrentValue(WidthProperty, Math.Max(minimumWidth, this.galleryPanel.Width + e.HorizontalChange));
            }

            this.CurrentPopupState.Save(this.galleryPanel, this.menuPanel);
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (DoubleUtil.AreClose(e.VerticalChange, 0))
            {
                return;
            }

            if (double.IsNaN(this.menuPanel.Height))
            {
                this.menuPanel.SetCurrentValue(HeightProperty, this.menuPanel.ActualHeight);
            }

            var minimumHeight = this.layoutRoot.ActualHeight + this.popupMenuPresenter.ActualHeight + this.popupResizeBorder.ActualHeight + 10;
            this.menuPanel.SetCurrentValue(HeightProperty, Math.Max(minimumHeight, Math.Min(this.menuPanel.Height + e.VerticalChange, this.MaxDropDownHeight)));

            this.CurrentPopupState.Save(this.galleryPanel, this.menuPanel);
        }

        #endregion

        #region QuickAccess

        /// <inheritdoc />
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var gallery = new InRibbonGallery();
            RibbonControl.BindQuickAccessItem(this, gallery);
            RibbonControl.Bind(this, gallery, nameof(this.GroupBy), GroupByProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.GroupByAdvanced), GroupByAdvancedProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.ItemHeight), ItemHeightProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.ItemWidth), ItemWidthProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.MinItemsInDropDownRow), MinItemsInDropDownRowProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.MaxItemsInDropDownRow), MaxItemsInDropDownRowProperty, BindingMode.OneWay);

            RibbonControl.Bind(this, gallery, nameof(this.DisplayMemberPath), DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.GroupStyleSelector), GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.ItemContainerStyle), ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.ItemsPanel), ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.ItemStringFormat), ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.ItemTemplate), ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.SelectedValuePath), SelectedValuePathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.MaxDropDownWidth), MaxDropDownWidthProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, gallery, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.OneWay);

            gallery.DropDownOpened += this.OnQuickAccessOpened;

            if (this.DropDownClosed != null)
            {
                gallery.DropDownClosed += this.DropDownClosed;
            }

            if (this.DropDownOpened != null)
            {
                gallery.DropDownOpened += this.DropDownOpened;
            }

            RibbonProperties.SetSize(gallery, RibbonControlSize.Small);
            this.quickAccessGallery = gallery;

            return gallery;
        }

        private InRibbonGallery quickAccessGallery;

        private void OnQuickAccessOpened(object sender, EventArgs e)
        {
            for (var i = 0; i < this.Filters.Count; i++)
            {
                this.quickAccessGallery.Filters.Add(this.Filters[i]);
            }

            this.quickAccessGallery.SelectedFilter = this.SelectedFilter;

            this.quickAccessGallery.DropDownClosed += this.OnQuickAccessMenuClosedOrUnloaded;
            this.quickAccessGallery.Unloaded += this.OnQuickAccessMenuClosedOrUnloaded;

            this.Freeze();
        }

        private void OnQuickAccessMenuClosedOrUnloaded(object sender, EventArgs e)
        {
            this.quickAccessGallery.DropDownClosed -= this.OnQuickAccessMenuClosedOrUnloaded;
            this.quickAccessGallery.Unloaded -= this.OnQuickAccessMenuClosedOrUnloaded;

            this.SelectedFilter = this.quickAccessGallery.SelectedFilter;
            this.quickAccessGallery.Filters.Clear();

            this.Unfreeze();
        }

        private void Freeze()
        {
            this.IsSnapped = true;
            this.IsFrozen = true;

            if (this.controlPresenter != null)
            {
                this.controlPresenter.Content = this.snappedImage;
            }

            // Move items and selected item
            var selectedItem = this.SelectedItem;
            this.SelectedItem = null;

            ItemsControlHelper.MoveItemsToDifferentControl(this, this.quickAccessGallery);

            this.quickAccessGallery.SelectedItem = selectedItem;

            // Move menu
            var menu = this.Menu;
            this.Menu = null;
            this.quickAccessGallery.Menu = menu;
        }

        private void Unfreeze()
        {
            // Move items and selected item
            var selectedItem = this.quickAccessGallery.SelectedItem;
            this.quickAccessGallery.SelectedItem = null;

            ItemsControlHelper.MoveItemsToDifferentControl(this.quickAccessGallery, this);

            this.SelectedItem = selectedItem;

            // Move menu
            var menu = this.quickAccessGallery.Menu;
            this.quickAccessGallery.Menu = null;
            this.Menu = menu;

            if (this.IsDropDownOpen == false)
            {
                if (this.popupControlPresenter != null)
                {
                    this.popupControlPresenter.Content = null;
                }

                if (this.controlPresenter != null)
                {
                    this.controlPresenter.Content = this.galleryPanel;
                }
            }

            this.RunInDispatcherAsync(() =>
                                      {
                                          this.IsFrozen = false;

                                          if (this.IsDropDownOpen == false)
                                          {
                                              this.IsSnapped = false;
                                          }

                                          var selectedContainer = this.ItemContainerGenerator.ContainerOrContainerContentFromItem<GalleryItem>(this.SelectedItem);
                                          selectedContainer?.BringIntoView();
                                      }, DispatcherPriority.SystemIdle);
        }

        /// <inheritdoc />
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(InRibbonGallery), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

        #endregion

        #region Implementation of IScalableRibbonControl

        /// <inheritdoc />
        public void Enlarge()
        {
            if (this.IsCollapsed
                && RibbonProperties.GetSize(this) == RibbonControlSize.Large)
            {
                this.IsCollapsed = false;
            }
            else if (this.galleryPanel.IsNotNull()
                     && this.galleryPanel.MaxItemsInRow < this.MaxItemsInRow)
            {
                this.galleryPanel.MaxItemsInRow = Math.Min(this.galleryPanel.MaxItemsInRow + 1, this.MaxItemsInRow);
            }
            else
            {
                return;
            }

            this.InvalidateMeasure();

            this.Scaled?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void Reduce()
        {
            if (this.galleryPanel.IsNotNull()
                && this.galleryPanel.MaxItemsInRow > this.MinItemsInRow)
            {
                this.galleryPanel.MaxItemsInRow = Math.Max(this.galleryPanel.MaxItemsInRow - 1, 0);
            }
            else if (this.CanCollapseToButton
                     && this.IsCollapsed == false)
            {
                this.IsCollapsed = true;
            }
            else
            {
                return;
            }

            this.InvalidateMeasure();

            this.Scaled?.Invoke(this, EventArgs.Empty);
        }

        #endregion

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

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonInRibbonGalleryAutomationPeer(this);

        internal class GalleryPanelState
        {
            public GalleryPanelState(GalleryPanel galleryPanel)
            {
                this.GalleryPanel = galleryPanel;
                this.Save();
            }

            public GalleryPanel GalleryPanel { get; }

            public int MinItemsInRow { get; private set; }

            public int MaxItemsInRow { get; private set; }

            public void Save()
            {
                this.MinItemsInRow = this.GalleryPanel.MinItemsInRow;
                this.MaxItemsInRow = this.GalleryPanel.MaxItemsInRow;
            }

            public void Restore()
            {
                this.GalleryPanel.MinItemsInRow = this.MinItemsInRow;
                this.GalleryPanel.MaxItemsInRow = this.MaxItemsInRow;
            }
        }

        internal class PopupState
        {
            public double Width { get; private set; } = double.NaN;

            public double Height { get; private set; } = double.NaN;

            public void Save(FrameworkElement widthControl, FrameworkElement heightControl)
            {
                this.Width = widthControl.Width;
                this.Height = heightControl.Height;
            }

            public void Restore(FrameworkElement widthControl, FrameworkElement heightControl)
            {
                if (double.IsNaN(this.Width) == false)
                {
                    widthControl.SetCurrentValue(WidthProperty, this.Width);
                }
                else
                {
                    widthControl.SetCurrentValue(WidthProperty, widthControl.ActualWidth);
                }

                if (double.IsNaN(this.Height) == false)
                {
                    heightControl.SetCurrentValue(HeightProperty, this.Height);
                }
            }
        }
    }
}