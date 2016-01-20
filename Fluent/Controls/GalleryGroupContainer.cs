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

        private Panel previousItemsPanel;
        private int previousItemsCount;

        // Whether MaxWidth of the ItemsPanel needs to be updated
        private bool maxMinWidthNeedsToBeUpdated;

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
            DependencyProperty.Register("IsHeadered", typeof(bool), 
            typeof(GalleryGroupContainer), new UIPropertyMetadata(true));
        
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
            get { return (double)this.GetValue(ItemWidthProperty); }
            set { this.SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemWidth.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double),
            typeof(GalleryGroupContainer), new UIPropertyMetadata(double.NaN));

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
            DependencyProperty.Register("ItemHeight", typeof(double),
            typeof(GalleryGroupContainer), new UIPropertyMetadata(double.NaN));

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
            get { return (int)this.GetValue(MaxItemsInRowProperty); }
            set { this.SetValue(MaxItemsInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsInRow. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxItemsInRowProperty =
            DependencyProperty.Register("MaxItemsInRow", typeof(int),
            typeof(GalleryGroupContainer), new UIPropertyMetadata(int.MaxValue, OnMaxMinItemsInRowChanged));

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
                basevalue = ((FrameworkElement)d).TryFindResource(typeof(GalleryGroupContainer));
            }

            return basevalue;
        }
         
        #endregion

        #region MaxWidth Updating
        
        // Sets MaxWidth of the items panel based of ItemsInRow property
        private void UpdateMinAndMaxWidth()
        {
            this.maxMinWidthNeedsToBeUpdated = false;

            var itemsPanel = FindItemsPanel(this);
            if (itemsPanel == null)
            {
                // Item's panel is not ready now
                if (this.IsLoaded)
                {
                    Debug.WriteLine("Panel with IsItemsHost = true is not found in GalleryGroupContainer (probably the style is not correct or haven't attached yet)");
                }

                this.Dispatcher.BeginInvoke((Action)this.InvalidateMeasure, DispatcherPriority.ContextIdle);
                return;
            }

            if (this.Orientation == Orientation.Vertical)
            {
                // Min/Max is used for Horizontal layout only
                itemsPanel.MinWidth = 0;
                itemsPanel.MaxWidth = double.PositiveInfinity;
                return;
            }

            var itemWidth = this.GetItemWidth();
            if (double.IsNaN(itemWidth))
            {
                // We can't calc item's width now
                return;
            }

            itemsPanel.MinWidth = Math.Min(this.Items.Count, this.MinItemsInRow) * itemWidth + 0.1;
            itemsPanel.MaxWidth = Math.Min(this.Items.Count, this.MaxItemsInRow) * itemWidth + 0.1;
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
            var panel = FindItemsPanel(this);
            if (panel != this.previousItemsPanel 
                || this.previousItemsCount != this.Items.Count 
                || this.maxMinWidthNeedsToBeUpdated)
            {
                // Track ItemsPanel changing
                this.previousItemsPanel = panel;
                this.previousItemsCount = this.Items.Count;
                this.UpdateMinAndMaxWidth();
            }
            return base.MeasureOverride(constraint);
        }
    }
}