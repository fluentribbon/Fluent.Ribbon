using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace Fluent
{
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
        /// Using a DependencyProperty as the backing store for IsHeadered.  
        /// This enables animation, styling, binding, etc...
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
        /// Using a DependencyProperty as the backing store for Orientation.  
        /// This enables animation, styling, binding, etc...
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
        /// Using a DependencyProperty as the backing store for ItemWidth.  
        /// This enables animation, styling, binding, etc...
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
        /// Using a DependencyProperty as the backing store for ItemHeight.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double),
            typeof(GalleryGroupContainer), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region MinItemsInRow

        /// <summary>
        /// Gets or sets minimum items quantity in row
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)this.GetValue(MinItemsInRowProperty); }
            set { this.SetValue(MinItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsInRow. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinItemsInRowProperty =
            DependencyProperty.Register(nameof(MinItemsInRow), typeof(int),
            typeof(GalleryGroupContainer), new PropertyMetadata(IntBoxes.Zero, OnMaxMinItemsInRowChanged));

        #endregion

        #region MaxItemsInRow

        /// <summary>
        /// Gets or sets maximum items quantity in row
        /// </summary>
        public int MaxItemsInRow
        {
            get { return (int)this.GetValue(MaxItemsInRowProperty); }
            set { this.SetValue(MaxItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsInRow. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxItemsInRowProperty =
            DependencyProperty.Register(nameof(MaxItemsInRow), typeof(int),
            typeof(GalleryGroupContainer), new PropertyMetadata(int.MaxValue, OnMaxMinItemsInRowChanged));

        #endregion

        private Panel RealItemsPanel
        {
            get
            {
                return this.itemsPanel ?? (this.itemsPanel = FindItemsPanel(this));
            }
        }

        private static void OnMaxMinItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryGroupContainer = (GalleryGroupContainer)d;
            galleryGroupContainer.minMaxWidthNeedsToBeUpdated = true;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        static GalleryGroupContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(typeof(GalleryGroupContainer)));
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.ItemsPanel"/> property changes.
        /// </summary>
        /// <param name="oldItemsPanel">Old value of the <see cref="P:System.Windows.Controls.ItemsControl.ItemsPanel"/> property.</param><param name="newItemsPanel">New value of the <see cref="P:System.Windows.Controls.ItemsControl.ItemsPanel"/> property.</param>
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

            if (this.Orientation == Orientation.Vertical)
            {
                // Min/Max is used for Horizontal layout only
                this.RealItemsPanel.MinWidth = 0;
                this.RealItemsPanel.MaxWidth = double.PositiveInfinity;
                return;
            }

            var itemWidth = this.GetItemWidth();
            if (double.IsNaN(itemWidth))
            {
                // We can't calc item's width now
                return;
            }

            this.RealItemsPanel.MinWidth = Math.Min(this.Items.Count, this.MinItemsInRow) * itemWidth + 0.1;
            this.RealItemsPanel.MaxWidth = Math.Min(this.Items.Count, this.MaxItemsInRow) * itemWidth + 0.1;
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
                var panel = obj as Panel;
                if (panel != null &&
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

        /// <summary>
        /// Called to remeasure a control. 
        /// </summary>
        /// <returns>The size of the control, up to the maximum specified by constraint.</returns>
        /// <param name="constraint">The maximum size that the method can return.</param>
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