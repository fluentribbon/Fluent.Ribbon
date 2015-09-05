﻿namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// Represents panel for Gallery, InRibbonGallery, ComboBox 
    /// with grouping and filtering capabilities
    /// </summary>
    public class GalleryPanel : VirtualizingStackPanel
    {
        #region Fields

        // Currently used group containers
        private readonly List<GalleryGroupContainer> galleryGroupContainers = new List<GalleryGroupContainer>();

        // Designate that gallery panel must be refreshed its groups
        private bool haveToBeRefreshed;

        // Group name resolver
        private Func<object, string> groupByAdvanced;

        #endregion

        #region Properties

        #region IsGrouped

        /// <summary>
        /// Gets or sets whether gallery panel shows groups 
        /// (Filter property still works as usual)
        /// </summary>
        public bool IsGrouped
        {
            get { return (bool)this.GetValue(IsGroupedProperty); }
            set { this.SetValue(IsGroupedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsGrouped. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsGroupedProperty =
            DependencyProperty.Register("IsGrouped", typeof(bool), typeof(GalleryPanel),
            new UIPropertyMetadata(true, OnIsGroupedChanged));

        private static void OnIsGroupedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryPanel = (GalleryPanel)d;
            galleryPanel.Invalidate();
        }

        #endregion

        #region GroupBy

        /// <summary>
        /// Gets or sets property name to group items
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
        public static readonly DependencyProperty GroupByProperty =
            DependencyProperty.Register("GroupBy", typeof(string), typeof(GalleryPanel), 
            new UIPropertyMetadata(null, OnGroupByChanged));

        private static void OnGroupByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryPanel = (GalleryPanel)d;
            galleryPanel.Invalidate();
        }

        #endregion

        #region GroupByAdvanced

        /// <summary>
        /// Gets or sets custom user method to group items. 
        /// If this property is not null, GroupBy property is ignored
        /// </summary>
        public Func<object, string> GroupByAdvanced
        {
            get { return this.groupByAdvanced; }
            set 
            {
                this.groupByAdvanced = value;
                this.Invalidate();
            }
        }

        #endregion

        #region GroupStyle

        /// <summary>
        /// Gets or sets group style
        /// </summary>
        public Style GroupStyle
        {
            get { return (Style)this.GetValue(GroupStyleProperty); }
            set { this.SetValue(GroupStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupHeaderStyle.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupStyleProperty =
            DependencyProperty.Register("GroupHeaderStyle", typeof(Style), 
            typeof(GalleryPanel), new UIPropertyMetadata(null));
        
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
            typeof(GalleryPanel), new UIPropertyMetadata(double.NaN));

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
            typeof(GalleryPanel), new UIPropertyMetadata(double.NaN));
        
        #endregion

        #region Filter
        
        /// <summary>
        /// Gets or sets groups names separated by comma which must be shown
        /// </summary>
        public string Filter
        {
            get { return (string)this.GetValue(FilterProperty); }
            set { this.SetValue(FilterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Filter. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), 
            typeof(GalleryPanel), new UIPropertyMetadata(null, OnFilterChanged));

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryPanel = (GalleryPanel)d;
            galleryPanel.Invalidate();
        }

        #endregion

        #region MinItemsInRow

        /// <summary>
        /// Gets or sets maximum items quantity in row
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
            typeof(GalleryPanel), new UIPropertyMetadata((int)1));
        
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
            typeof(GalleryPanel), new UIPropertyMetadata(int.MaxValue));

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public GalleryPanel()
        {
            this.visualCollection = new VisualCollection(this);
        }

        #endregion

        #region Visual Tree

        readonly VisualCollection visualCollection;

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount + this.visualCollection.Count;
            }
        }

        /// <summary>
        /// Overrides System.Windows.Media.Visual.GetVisualChild(System.Int32),
        /// and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested 
        /// child element in the collection</param>
        /// <returns>The requested child element. This should not return null; 
        /// if the provided index is out of range, an exception is thrown</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index < base.VisualChildrenCount)
            {
                return base.GetVisualChild(index);
            }

            return this.visualCollection[index - base.VisualChildrenCount];
        }

        #endregion

        #region GetActualMinWidth

        /// <summary>
        /// Updates MinWidth and MaxWidth of the gallery panel (based on MinItemsInRow and MaxItemsInRow)
        /// </summary>
        public void UpdateMinAndMaxWidth()
        {
            // Calculate actual min width
            double actualMinWidth = 0;
            var actualMaxWidth = double.PositiveInfinity;

            foreach (var galleryGroupContainer in this.galleryGroupContainers)
            {
                var backupMinItemsInRow = galleryGroupContainer.MinItemsInRow;
                var backupMaxItemsInRow = galleryGroupContainer.MaxItemsInRow;
                galleryGroupContainer.MinItemsInRow = this.MinItemsInRow;
                galleryGroupContainer.MaxItemsInRow = this.MaxItemsInRow;

                InvalidateMeasureRecursive(galleryGroupContainer);
                galleryGroupContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                galleryGroupContainer.InvalidateMeasure();

                actualMinWidth = Math.Max(actualMinWidth, galleryGroupContainer.MinWidth);
                actualMaxWidth = Math.Min(actualMaxWidth, galleryGroupContainer.MaxWidth);

                galleryGroupContainer.MinItemsInRow = backupMinItemsInRow;
                galleryGroupContainer.MaxItemsInRow = backupMaxItemsInRow;
            }

            this.MinWidth = actualMinWidth;
            this.MaxWidth = actualMaxWidth;
        }

        private static void InvalidateMeasureRecursive(UIElement visual)
        {
            visual.InvalidateMeasure();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                var element = VisualTreeHelper.GetChild(visual, i) as UIElement;
                if (element != null)
                {
                    InvalidateMeasureRecursive(element);
                }
            }
        }

        #endregion

        #region GetItemSize

        /// <summary>
        /// Determinates item's size (return Size.Empty in case of it is not possible)
        /// </summary>
        /// <returns></returns>
        public Size GetItemSize()
        {
            foreach (var galleryGroupContainer in this.galleryGroupContainers)
            {
                var size = galleryGroupContainer.GetItemSize();
                if (size.IsEmpty == false)
                {
                    return size;
                }
            }

            return Size.Empty;
        }

        #endregion

        #region Refresh

        private void Invalidate()
        {
            if (this.haveToBeRefreshed)
            {
                return;
            }

            this.haveToBeRefreshed = true;
            this.Dispatcher.BeginInvoke((Action)this.RefreshDispatchered, DispatcherPriority.Send);
        }

        private void RefreshDispatchered()
        {
            if (this.haveToBeRefreshed == false)
            {
                return;
            }

            this.Refresh();
            this.haveToBeRefreshed = false;
        }

        private void Refresh()
        {
            // Clear currently used group containers 
            // and supply with new generated ones
            foreach (var galleryGroupContainer in this.galleryGroupContainers)
            {
                BindingOperations.ClearAllBindings(galleryGroupContainer);
                //RemoveVisualChild(galleryGroupContainer);
                this.visualCollection.Remove(galleryGroupContainer);
            }

            this.galleryGroupContainers.Clear();
            
            // Gets filters
            var filter = this.Filter == null ? null : this.Filter.Split(',');

            var dictionary = new Dictionary<string, GalleryGroupContainer>();

            foreach(UIElement item in this.InternalChildren)
            {
                if (item == null)
                {
                    continue;
                }

                // Resolve group name
                string propertyValue = null;

                if (this.GroupByAdvanced == null)
                {
                    propertyValue = (this.ItemContainerGenerator == null)
                                        ? this.GetPropertyValueAsString(item)
                                        : this.GetPropertyValueAsString(this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this).ItemFromContainer(item));
                }
                else
                {
                    propertyValue = (this.ItemContainerGenerator == null)
                                        ? this.GroupByAdvanced(item)
                                        : this.GroupByAdvanced(this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this).ItemFromContainer(item));
                }

                if (propertyValue == null)
                {
                    propertyValue = "Undefined";
                }
                
                // Make invisible if it is not in filter (or is not grouped)
                if (this.IsGrouped == false 
                    || (filter != null && filter.Contains(propertyValue) == false))
                {
                    item.Measure(new Size(0,0));
                    item.Arrange(new Rect(0,0,0,0));
                }

                // Skip if it is not in filter
                if (filter != null &&
                    filter.Contains(propertyValue) == false)
                {
                    continue;
                }

                // To put all items in one group in case of IsGrouped = False
                if (this.IsGrouped == false)
                {
                    propertyValue = "Undefined";
                }

                if (dictionary.ContainsKey(propertyValue) == false)
                {
                    var galleryGroupContainer = new GalleryGroupContainer
                                                {
                                                    Header = propertyValue
                                                };
                    RibbonControl.Bind(this, galleryGroupContainer, "GroupStyle", GroupStyleProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "Orientation", GalleryGroupContainer.OrientationProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "ItemWidth", GalleryGroupContainer.ItemWidthProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "ItemHeight", GalleryGroupContainer.ItemHeightProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "MaxItemsInRow", GalleryGroupContainer.MaxItemsInRowProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "MinItemsInRow", GalleryGroupContainer.MinItemsInRowProperty, BindingMode.OneWay);
                    dictionary.Add(propertyValue,galleryGroupContainer);
                    this.galleryGroupContainers.Add(galleryGroupContainer);

                    this.visualCollection.Add(galleryGroupContainer);
                }

                dictionary[propertyValue].Items.Add(new GalleryItemPlaceholder(item));
            }

            if ((this.IsGrouped == false || (this.GroupBy == null && this.GroupByAdvanced == null)) 
                && this.galleryGroupContainers.Count != 0)
            {
                // Make it without headers
                this.galleryGroupContainers[0].IsHeadered = false;
            }

            this.InvalidateMeasure();
        }

        #endregion

        #region Layout Overrides

        /// <summary>
        /// When overridden in a derived class, measures the size in 
        /// layout required for child elements and determines a size 
        /// for the derived class. 
        /// </summary>
        /// <returns>
        /// The size that this element determines it needs during layout, 
        /// based on its calculations of child element sizes.
        /// </returns>
        /// <param name="availableSize">The available size that this element can give 
        /// to child elements. Infinity can be specified as a value to indicate that
        /// the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            var baseSize = base.MeasureOverride(availableSize);

            if (this.galleryGroupContainers.Count == 0)
            {
                return baseSize;
            }

            double width = 0;
            double height = 0;
            foreach (var child in this.galleryGroupContainers)
            {
                child.Measure(availableSize);
                height += child.DesiredSize.Height;
                width = Math.Max(width, child.DesiredSize.Width);
            }

            return new Size(width, height);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements 
        /// and determines a size for a derived class. 
        /// </summary>
        /// <returns> The actual size used. </returns>
        /// <param name="finalSize">The final area within the parent that this 
        /// element should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var baseSize = base.ArrangeOverride(finalSize);

            if (this.galleryGroupContainers.Count == 0)
            {
                return baseSize;
            }

            var finalRect = new Rect(finalSize);

            foreach (var item in this.galleryGroupContainers)
            {
                finalRect.Height = item.DesiredSize.Height;
                finalRect.Width = Math.Max(finalSize.Width, item.DesiredSize.Width);

                // Arrange a container to arrange placeholders
                item.Arrange(finalRect);

                finalRect.Y += item.DesiredSize.Height;

                // Now arrange our actual items using arranged size of placeholders
                foreach (GalleryItemPlaceholder placeholder in item.Items)
                {
                    var leftTop = placeholder.TranslatePoint(new Point(), this);

                    placeholder.Target.Arrange(new Rect(leftTop.X, leftTop.Y,
                        placeholder.ArrangedSize.Width,
                        placeholder.ArrangedSize.Height));
                }
            }

            return finalSize;
        }

        #endregion

        #region Private Methods

        private string GetPropertyValueAsString(object item)
        {
            if (item == null ||
                this.GroupBy == null)
            {
                return "Undefined";
            }

            var property = item.GetType().GetProperty(this.GroupBy, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return "Undefined";
            }

            var result = property.GetValue(item, null);
            if (result == null)
            {
                return "Undefined";
            }
            return result.ToString();
        }

        #endregion

		/// <summary>
		/// Gets an enumerator that can iterate the logical child elements of this <see cref="T:System.Windows.Controls.Panel"/> element. 
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/>. This property has no default value.
		/// </returns>
		protected override IEnumerator LogicalChildren
		{
			get
			{
				var count = this.VisualChildrenCount;

				for (var i = 0; i < count; i++)
				{
					yield return this.GetVisualChild(i);
				}
			}
		}
    }
}