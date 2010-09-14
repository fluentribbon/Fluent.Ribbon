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
    public class InRibbonGallery : Selector, IScalableRibbonControl, IDropDownControl
    {
        #region Fields

        private ObservableCollection<GalleryGroupFilter> filters;

        private ToggleButton expandButton;
        private ToggleButton dropDownButton;

        private MenuPanel menuPanel;

        //
        private int currentItemsInRow;

        private Panel layoutRoot;

        private double cachedWidthDelta;

        // Freezed image (created during snapping)
        Image snappedImage;
        // Visuals which were removed during snapping
        Visual[] snappedVisuals;
        // Is visual currently snapped
        private bool isSnapped;

        // Saved width in for scalable support
        private double savedWidth;

        private Popup popup;

        // Thumb to resize in both directions
        Thumb resizeBothThumb;
        // Thumb to resize vertical
        Thumb resizeVerticalThumb;

        #endregion

        #region Properties

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonControl.SizeProperty.AddOwner(typeof(InRibbonGallery));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonControl.AttachSizeDefinition(typeof(InRibbonGallery));

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
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(InRibbonGallery));

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public object Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(InRibbonGallery));

        #endregion

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
        /*
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
        */
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
        private bool IsSnapped
        {
            get
            {
                return isSnapped;
            }
            set
            {
                if (value == isSnapped) return;


                if ((value) && (((int)ActualWidth > 0) && ((int)ActualHeight > 0)))
                {
                    // Render the freezed image
                    snappedImage = new Image();
                    RenderOptions.SetBitmapScalingMode(snappedImage, BitmapScalingMode.NearestNeighbor);
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ActualWidth,
                                                                                   (int)ActualHeight, 96, 96,
                                                                                   PixelFormats.Pbgra32);
                    renderTargetBitmap.Render((Visual)VisualTreeHelper.GetChild(this, 0));
                    snappedImage.Source = renderTargetBitmap;
                    snappedImage.Width = ActualWidth;
                    snappedImage.Height = ActualHeight;
                    snappedImage.FlowDirection = FlowDirection;
                    // Detach current visual children
                    snappedVisuals = new Visual[VisualTreeHelper.GetChildrenCount(this)];
                    for (int childIndex = 0; childIndex < snappedVisuals.Length; childIndex++)
                    {
                        snappedVisuals[childIndex] = (Visual)VisualTreeHelper.GetChild(this, childIndex);
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

        #region MaxDropDownHeight

        /// <summary>
        /// Get or sets max height of drop down popup
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(InRibbonGallery), new UIPropertyMetadata(350.0));

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
            Type type = typeof(InRibbonGallery);
            ToolTipService.Attach(type);
            PopupService.Attach(type);
            ContextMenuService.Attach(type);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
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

            layoutRoot = GetTemplateChild("PART_LayoutRoot") as Panel;
            popup = GetTemplateChild("PART_Popup") as Popup;

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

            menuPanel = GetTemplateChild("PART_MenuPanel") as MenuPanel;

            // Clear cache then style changed
            cachedWidthDelta = 0;
        }

        void OnDropDownClick(object sender, RoutedEventArgs e)
        {
            dropDownButton.IsChecked = true;
            IsDropDownOpen = true;
            e.Handled = true;
        }

        void OnExpandClick(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = true;
            e.Handled = true;
        }
        /*
        double GetItemWidth()
        {
            if (double.IsNaN(ItemWidth) && (listBox != null) && (listBox.Items.Count > 0))
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
                item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (useHack)
                {
                    item.Content = null;
                    AddLogicalChild(listBox.Items[0]);
                }
                return item.DesiredSize.Width;
            }
            return ItemWidth;
        }
        */

       /* /// <summary>
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
                if (savedWidth != size.Width)
                {
                    savedWidth = size.Width;
                    if (Scaled != null) Scaled(this, EventArgs.Empty);
                }
                return size;
            }
            if (listBox == null) return base.MeasureOverride(constraint);
            if (listBox.Items.Count == 0) return base.MeasureOverride(constraint);
            double itemWidth = GetItemWidth();
            if (cachedWidthDelta == 0)
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
        }*/
        /*
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
        */
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

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(menuPanel.Width)) menuPanel.Width = menuPanel.ActualWidth;
            if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
            menuPanel.Width = Math.Max(menuPanel.MinWidth, menuPanel.Width + e.HorizontalChange);
            menuPanel.Height = Math.Min(Math.Max(menuPanel.MinHeight, menuPanel.Height + e.VerticalChange), MaxDropDownHeight);
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
            menuPanel.Height = Math.Min(Math.Max(menuPanel.MinHeight, menuPanel.Height + e.VerticalChange), MaxDropDownHeight);
        }

        /*
        internal string GetItemGroupName(object obj)
        {
            object result = obj.GetType().GetProperty(GroupBy, BindingFlags.Public | BindingFlags.Instance).GetValue(obj, null);
            if (result == null) return null;
            return result.ToString();
        }
        */
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
        /*

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
        */
        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public FrameworkElement CreateQuickAccessItem()
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
            /*if (ItemsInRow == 0) gallery.MinWidth = Math.Max(ActualWidth, MenuMinWidth);
            else
            {
                gallery.ItemsInRow = ItemsInRow;
            }
            gallery.MinHeight = ActualHeight;
            DropDownButton button = sender as DropDownButton;
            button.ResizeMode = ResizeMode;
            if (!IsCollapsed) IsSnapped = true;

            object selectedItem = SelectedItem;
            if (listBox != null)
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
            /*currentItemsInRow++;

            if ((CanCollapseToButton) && (CurrentItemsInRow >= MinItemsInRow) && (Size == RibbonControlSize.Large)) IsCollapsed = false;

            InvalidateMeasure();*/
        }

        /// <summary>
        /// Reduce control size
        /// </summary>
        public void Reduce()
        {
            /*currentItemsInRow--;
            if ((CanCollapseToButton) && (CurrentItemsInRow < MinItemsInRow)) IsCollapsed = true;

            InvalidateMeasure();*/
        }

        #endregion
    }
}
