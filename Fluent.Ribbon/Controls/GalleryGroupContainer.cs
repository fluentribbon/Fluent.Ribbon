// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents container of grouped gallery items in GalleryPanel or Gallery
    /// </summary>
    public class GalleryGroupContainer : HeaderedItemsControl
    {
        #region Fields

        private int previousItemsCount;

        // Whether MinWidth/MaxWidth of the ItemsPanel needs to be updated
        private bool minMaxWidthNeedsToBeUpdated = true;
        private Panel itemsPanel;

        #endregion

        #region Properites

        #region IsHeadered

        /// <summary>
        /// Gets or sets whether the header must be shown.
        /// When the property is false this control uses to show all items without grouping
        /// </summary>
        public bool IsHeadered
        {
            get { return (bool)this.GetValue(IsHeaderedProperty); }
            set { this.SetValue(IsHeaderedProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsHeadered"/>.
        /// </summary>
        public static readonly DependencyProperty IsHeaderedProperty =
            DependencyProperty.Register(nameof(IsHeadered), typeof(bool),
            typeof(GalleryGroupContainer), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets panel orientation
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="Orientation"/>.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation),
            typeof(GalleryGroupContainer), new PropertyMetadata(Orientation.Horizontal));

        #endregion

        #region ItemWidth

        /// <summary>
        /// Gets or sets a value that specifies the width of
        /// all items that are contained within
        /// </summary>
        public double ItemWidth
        {
            get { return (double)this.GetValue(ItemWidthProperty); }
            set { this.SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ItemWidth"/>.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(double),
            typeof(GalleryGroupContainer), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region ItemHeight

        /// <summary>
        /// Gets or sets a value that specifies the height of
        /// all items that are contained within
        /// </summary>
        public double ItemHeight
        {
            get { return (double)this.GetValue(ItemHeightProperty); }
            set { this.SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ItemHeight"/>.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double),
            typeof(GalleryGroupContainer), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region MinItemsInRow

        /// <summary>
        /// Gets or sets minimum items in which should be placed in one row.
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)this.GetValue(MinItemsInRowProperty); }
            set { this.SetValue(MinItemsInRowProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="MinItemsInRow"/>.
        /// </summary>
        public static readonly DependencyProperty MinItemsInRowProperty =
            DependencyProperty.Register(nameof(MinItemsInRow), typeof(int),
            typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(IntBoxes.Zero, FrameworkPropertyMetadataOptions.AffectsMeasure, OnMinItemsInRowChanged));

        private static void OnMinItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnMaxOrMinItemsInRowChanged(d, e);
        }

        #endregion

        #region MaxItemsInRow

        /// <summary>
        /// Gets or sets maximum items in which should be placed in one row.
        /// </summary>
        public int MaxItemsInRow
        {
            get { return (int)this.GetValue(MaxItemsInRowProperty); }
            set { this.SetValue(MaxItemsInRowProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="MaxItemsInRow"/>.
        /// </summary>
        public static readonly DependencyProperty MaxItemsInRowProperty =
            DependencyProperty.Register(nameof(MaxItemsInRow), typeof(int),
            typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.AffectsMeasure, OnMaxItemsInRowChanged));

        private static void OnMaxItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnMaxOrMinItemsInRowChanged(d, e);
        }

        #endregion

        private Panel RealItemsPanel => this.itemsPanel ?? (this.itemsPanel = FindItemsPanel(this));

        private static void OnMaxOrMinItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryGroupContainer = (GalleryGroupContainer)d;
            galleryGroupContainer.minMaxWidthNeedsToBeUpdated = true;
            galleryGroupContainer.UpdateMinAndMaxWidth();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new instance of <see cref="GalleryGroupContainer"/>.
        /// </summary>
        public GalleryGroupContainer()
        {
            this.Unloaded += this.HandleUnloaded;
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static GalleryGroupContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(typeof(GalleryGroupContainer)));
        }

        /// <inheritdoc />
        protected override void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel)
        {
            base.OnItemsPanelChanged(oldItemsPanel, newItemsPanel);

            this.itemsPanel = null;
            this.minMaxWidthNeedsToBeUpdated = true;
            this.InvalidateMeasure();
        }

        #endregion

        #region MaxWidth Updating

        // Sets MaxWidth of the items panel based of ItemsInRow property
        private void UpdateMinAndMaxWidth()
        {
            if (this.minMaxWidthNeedsToBeUpdated == false)
            {
                return;
            }

            if (this.RealItemsPanel == null)
            {
                // Item's panel is not ready now
                if (this.IsLoaded)
                {
                    Debug.WriteLine("Panel with IsItemsHost = true is not found in GalleryGroupContainer (probably the style is not correct or haven't attached yet)");
                }
                else
                {
                    // Prevent duplicate registration
                    this.Loaded -= this.HandleLoaded;
                    this.Loaded += this.HandleLoaded;
                }

                return;
            }

            this.minMaxWidthNeedsToBeUpdated = false;

            // Issue references:
            // - #542 + commit https://github.com/fluentribbon/Fluent.Ribbon/commit/8b458b1cfc5e440f54778c808142fffa67a23978
            // - #666
            // We need to check if we are inside a closed InRibbonGallery.
            // - If we are inside an closed InRibbonGallery we need to restrict the size of "this"
            // - If we are inside an opened InRibbonGallery or not inside an InRibbonGallery we need to restrict the size of "RealItemsPanel"
            var inRibbonGallery = UIHelper.GetParent<InRibbonGallery>(this);
            var isInsideClosedInRibbonGallery = inRibbonGallery != null && inRibbonGallery.IsDropDownOpen == false;
            var targetForSizeConstraints = isInsideClosedInRibbonGallery ? (FrameworkElement)this : this.RealItemsPanel;

            var nonTargetForSizeConstraints = isInsideClosedInRibbonGallery ? (FrameworkElement)this.RealItemsPanel : this;

            nonTargetForSizeConstraints.MinWidth = 0;
            nonTargetForSizeConstraints.MaxWidth = double.PositiveInfinity;

            if (this.Orientation == Orientation.Vertical)
            {
                // Min/Max is used for Horizontal layout only
                targetForSizeConstraints.MinWidth = 0;
                targetForSizeConstraints.MaxWidth = double.PositiveInfinity;
                return;
            }

            var itemWidth = this.GetItemWidth();
            if (double.IsNaN(itemWidth))
            {
                // We can't calc item's width now
                return;
            }

            targetForSizeConstraints.MinWidth = (Math.Min(this.Items.Count, this.MinItemsInRow) * itemWidth) + 0.1;
            targetForSizeConstraints.MaxWidth = (Math.Min(this.Items.Count, this.MaxItemsInRow) * itemWidth) + 0.1;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.HandleLoaded;

            if (this.minMaxWidthNeedsToBeUpdated == false)
            {
                return;
            }

            this.InvalidateMeasure();
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            this.itemsPanel = null;

            this.minMaxWidthNeedsToBeUpdated = true;
        }

        /// <summary>
        /// Determinates item's size (return Size.Empty in case of it is not possible)
        /// </summary>
        /// <returns></returns>
        public Size GetItemSize()
        {
            if (!double.IsNaN(this.ItemWidth)
                && !double.IsNaN(this.ItemHeight))
            {
                return new Size(this.ItemWidth, this.ItemHeight);
            }

            if (this.Items.Count == 0)
            {
                return Size.Empty;
            }

            var anItem = this.ItemContainerGenerator.ContainerFromItem(this.Items[0]) as UIElement;
            if (anItem == null)
            {
                return Size.Empty;
            }

            anItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var result = anItem.DesiredSize;
            anItem.InvalidateMeasure();
            return result;
        }

        // Determinates item's width (return Double.NaN in case of it is not possible)
        private double GetItemWidth()
        {
            return this.GetItemSize().Width;
        }

        // Finds panel with IsItemsHost, or null if such panel is not found
        private static Panel FindItemsPanel(DependencyObject obj)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                if (obj is Panel panel &&
                    panel.IsItemsHost)
                {
                    return panel;
                }

                panel = FindItemsPanel(VisualTreeHelper.GetChild(obj, i));
                if (panel != null)
                {
                    return panel;
                }
            }

            return null;
        }

        #endregion

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.previousItemsCount != this.Items.Count
                || this.minMaxWidthNeedsToBeUpdated)
            {
                // Track ItemsPanel changing
                this.previousItemsCount = this.Items.Count;
                this.UpdateMinAndMaxWidth();
            }

            return base.MeasureOverride(constraint);
        }
    }
}