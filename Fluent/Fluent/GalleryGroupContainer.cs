using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents container of grouped gallery items in GalleryPanel or Gallery
    /// </summary>
    public class GalleryGroupContainer : HeaderedItemsControl
    {
        #region Fields

        // Whether MaxWidth of the ItemsPanel needs to be updated
        bool maxMinWidthNeedsToBeUpdated;

        #endregion

        #region Properites

        #region IsHeadered

        /// <summary>
        /// Gets or sets whether the header must be shown. 
        /// When the property is false this control uses to show all items without grouping
        /// </summary>
        public bool IsHeadered
        {
            get { return (bool)GetValue(IsHeaderedProperty); }
            set { SetValue(IsHeaderedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsHeadered.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsHeaderedProperty =
            DependencyProperty.Register("IsHeadered", typeof(bool), 
            typeof(GalleryGroupContainer), new UIPropertyMetadata(true));
        
        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets panel orientation
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
            typeof(GalleryGroupContainer), new UIPropertyMetadata(Orientation.Horizontal));

        #endregion
        
        #region ItemWidth

        /// <summary>
        /// Gets or sets a value that specifies the width of 
        /// all items that are contained within
        /// </summary>
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemWidth.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double),
            typeof(GalleryGroupContainer), new UIPropertyMetadata(Double.NaN));

        #endregion

        #region ItemHeight

        /// <summary>
        /// Gets or sets a value that specifies the height of 
        /// all items that are contained within
        /// </summary>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemHeight.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double),
            typeof(GalleryGroupContainer), new UIPropertyMetadata(Double.NaN));

        #endregion
        
        #region MinItemsInRow
        
        /// <summary>
        /// Gets or sets minimum items quantity in row
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)GetValue(MinItemsInRowProperty); }
            set { SetValue(MinItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsInRow. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinItemsInRowProperty =
            DependencyProperty.Register("MinItemsInRow", typeof(int),
            typeof(GalleryGroupContainer), new UIPropertyMetadata(0, OnMaxMinItemsInRowChanged));

        static void OnMaxMinItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroupContainer galleryGroupContainer = (GalleryGroupContainer) d;
            galleryGroupContainer.maxMinWidthNeedsToBeUpdated = true;
        }

        #endregion
        
        #region MaxItemsInRow

        /// <summary>
        /// Gets or sets maximum items quantity in row
        /// </summary>
        public int MaxItemsInRow
        {
            get { return (int)GetValue(MaxItemsInRowProperty); }
            set { SetValue(MaxItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsInRow. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxItemsInRowProperty =
            DependencyProperty.Register("MaxItemsInRow", typeof(int),
            typeof(GalleryGroupContainer), new UIPropertyMetadata(Int32.MaxValue, OnMaxMinItemsInRowChanged));


        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        static GalleryGroupContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(typeof(GalleryGroupContainer)));
            StyleProperty.OverrideMetadata(typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(GalleryGroupContainer));
            }

            return basevalue;
        }
         
        #endregion

        #region MaxWidth Updating
        
        // Sets MaxWidth of the items panel based of ItemsInRow property
        void UpdateMaxWidth()
        {
            maxMinWidthNeedsToBeUpdated = false;
            
            Panel itemsPanel = FindItemsPanel(this);
            if (itemsPanel == null)
            {
                // Item's panel is not ready now
                if (IsLoaded) Debug.WriteLine("Panel with IsItemsHost = true is not found in GalleryGroupContainer (probably the style is not correct or haven't attached yet)");
                Dispatcher.BeginInvoke((Action) InvalidateMeasure, DispatcherPriority.ContextIdle);
                return;
            }

            if (Orientation == Orientation.Vertical)
            {
                // Min/Max is used for Horizontal layout only
                itemsPanel.MinWidth = 0;
                itemsPanel.MaxWidth = Double.PositiveInfinity;
                return;
            }

            double itemWidth = GetItemWidth();
            if (double.IsNaN(itemWidth))
            {
                // We can't calc item's width now
                return;
            }

            itemsPanel.MinWidth = Math.Min(Items.Count, MinItemsInRow) * itemWidth + 0.1;
            itemsPanel.MaxWidth = Math.Min(Items.Count, MaxItemsInRow) * itemWidth + 0.1;
        }
        
        /// <summary>
        /// Determinates item's size (return Size.Empty in case of it is not possible)
        /// </summary>
        /// <returns></returns>
        public Size GetItemSize()
        {
            if (!Double.IsNaN(ItemWidth) && !Double.IsNaN(ItemHeight)) return new Size(ItemWidth, ItemHeight);
            if (Items.Count == 0) return Size.Empty;

            UIElement anItem = this.ItemContainerGenerator.ContainerFromItem(Items[0]) as UIElement;
            if (anItem == null) return Size.Empty;
            anItem.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size result = anItem.DesiredSize;
            anItem.InvalidateMeasure();
            return result;
        }

        // Determinates item's width (return Double.NaN in case of it is not possible)
        double GetItemWidth()
        {
            return GetItemSize().Width;
        }

        // Finds panel with IsItemsHost, or null if such panel is not found
        static Panel FindItemsPanel(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                Panel panel = obj as Panel;
                if (panel != null && panel.IsItemsHost) return panel;
                panel = FindItemsPanel(VisualTreeHelper.GetChild(obj, i));
                if (panel != null) return panel;
            }

            return null;
        }

        #endregion

        Panel previousItemsPanel = null;
        int previousItemsCount = 0;

        /// <summary>
        /// Called to remeasure a control. 
        /// </summary>
        /// <returns>The size of the control, up to the maximum specified by constraint.</returns>
        /// <param name="constraint">The maximum size that the method can return.</param>
        protected override Size MeasureOverride(Size constraint)
        {
            var panel = FindItemsPanel(this);
            if (panel != previousItemsPanel || previousItemsCount != Items.Count || maxMinWidthNeedsToBeUpdated)
            {
                // Track ItemsPanel changing
                previousItemsPanel = panel;
                previousItemsCount = Items.Count;
                UpdateMaxWidth();
            }
            return base.MeasureOverride(constraint);
        }
    }
}
