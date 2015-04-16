using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
	using System.Collections;

	/// <summary>
    /// Represents panel for Gallery, InRibbonGallery, ComboBox 
    /// with grouping and filtering capabilities
    /// </summary>
    public class GalleryPanel : Panel
    {
        #region Fields

        // Currently used group containers
        readonly List<GalleryGroupContainer> galleryGroupContainers = new List<GalleryGroupContainer>();
        // Designate that gallery panel must be refreshed its groups
        bool haveToBeRefreshed;
        // Group name resolver
        Func<object, string> groupByAdvanced;

        #endregion

        #region Properties

        #region IsGrouped

        /// <summary>
        /// Gets or sets whether gallery panel shows groups 
        /// (Filter property still works as usual)
        /// </summary>
        public bool IsGrouped
        {
            get { return (bool)GetValue(IsGroupedProperty); }
            set { SetValue(IsGroupedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsGrouped. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsGroupedProperty =
            DependencyProperty.Register("IsGrouped", typeof(bool), typeof(GalleryPanel),
            new UIPropertyMetadata(true, OnIsGroupedChanged));

        static void OnIsGroupedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryPanel galleryPanel = (GalleryPanel)d;
            galleryPanel.Invalidate();
        }

        #endregion

        #region GroupBy

        /// <summary>
        /// Gets or sets property name to group items
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
            DependencyProperty.Register("GroupBy", typeof(string), typeof(GalleryPanel), 
            new UIPropertyMetadata(null, OnGroupByChanged));

        static void OnGroupByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryPanel galleryPanel = (GalleryPanel)d;
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
            get { return groupByAdvanced; }
            set 
            { 
                groupByAdvanced = value;
                Invalidate();
            }
        }

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
            typeof(GalleryPanel), new UIPropertyMetadata(Orientation.Horizontal));
                
        #endregion

        #region ItemContainerGenerator

        /// <summary>
        /// Gets or sets ItemContainerGenerator which generates the 
        /// user interface (UI) on behalf of its host, such as an  ItemsControl. 
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get { return (ItemContainerGenerator)GetValue(ItemContainerGeneratorProperty); }
            set { SetValue(ItemContainerGeneratorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerGenerator.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerGeneratorProperty =
            DependencyProperty.Register("ItemContainerGenerator", typeof(ItemContainerGenerator), 
            typeof(GalleryPanel), new UIPropertyMetadata(null, OnItemContainerGeneratorChanged));

        static void OnItemContainerGeneratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryPanel galleryPanel = (GalleryPanel) d;
            galleryPanel.Invalidate();
        }

        #endregion

        #region GroupStyle

        /// <summary>
        /// Gets or sets group style
        /// </summary>
        public Style GroupStyle
        {
            get { return (Style)GetValue(GroupStyleProperty); }
            set { SetValue(GroupStyleProperty, value); }
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
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemWidth.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), 
            typeof(GalleryPanel), new UIPropertyMetadata(Double.NaN));

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
            typeof(GalleryPanel), new UIPropertyMetadata(Double.NaN));
        
        #endregion

        #region Filter
        
        /// <summary>
        /// Gets or sets groups names separated by comma which must be shown
        /// </summary>
        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Filter. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), 
            typeof(GalleryPanel), new UIPropertyMetadata(null, OnFilterChanged));

        static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryPanel galleryPanel = (GalleryPanel)d;
            galleryPanel.Invalidate();
        }

        #endregion

        #region MinItemsInRow

        /// <summary>
        /// Gets or sets maximum items quantity in row
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
            typeof(GalleryPanel), new UIPropertyMetadata((int)1));
        
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
            typeof(GalleryPanel), new UIPropertyMetadata(Int32.MaxValue));

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public GalleryPanel()
        {
            visualCollection = new VisualCollection(this);
        }

        #endregion

        #region Visual Tree

        readonly VisualCollection visualCollection = null;

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount + visualCollection.Count;
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
            if (index < base.VisualChildrenCount) return base.GetVisualChild(index);
            return visualCollection[index - base.VisualChildrenCount];
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
            double actualMaxWidth = double.PositiveInfinity;

            foreach (var galleryGroupContainer in galleryGroupContainers)
            {
                var backupMinItemsInRow = galleryGroupContainer.MinItemsInRow;
                var backupMaxItemsInRow = galleryGroupContainer.MaxItemsInRow;
                galleryGroupContainer.MinItemsInRow = this.MinItemsInRow;
                galleryGroupContainer.MaxItemsInRow = this.MaxItemsInRow;

                InvalidateMeasureRecursive(galleryGroupContainer);
                galleryGroupContainer.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                galleryGroupContainer.InvalidateMeasure();

                actualMinWidth = Math.Max(actualMinWidth, galleryGroupContainer.MinWidth);
                actualMaxWidth = Math.Min(actualMaxWidth, galleryGroupContainer.MaxWidth);

                galleryGroupContainer.MinItemsInRow = backupMinItemsInRow;
                galleryGroupContainer.MaxItemsInRow = backupMaxItemsInRow;
            }

            this.MinWidth = actualMinWidth;
            this.MaxWidth = actualMaxWidth;
        }

        static void InvalidateMeasureRecursive(UIElement visual)
        {
            visual.InvalidateMeasure();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                UIElement element = VisualTreeHelper.GetChild(visual, i) as UIElement;
                if (element != null) InvalidateMeasureRecursive(element);
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
            foreach (GalleryGroupContainer galleryGroupContainer in galleryGroupContainers)
            {
                Size size = galleryGroupContainer.GetItemSize();
                if (!size.IsEmpty) return size;
            }
            return Size.Empty;
        }

        #endregion

        #region Refresh

        void Invalidate()
        {
            if (haveToBeRefreshed) return;

            haveToBeRefreshed = true;
            Dispatcher.BeginInvoke((Action) RefreshDispatchered, DispatcherPriority.Send);
        }

        void RefreshDispatchered()
        {
            if (!haveToBeRefreshed) return;
            Refresh();
            haveToBeRefreshed = false;
        }

        void Refresh()
        {
            // Clear currently used group containers 
            // and supply with new generated ones
            foreach (GalleryGroupContainer galleryGroupContainer in galleryGroupContainers)
            {
                BindingOperations.ClearAllBindings(galleryGroupContainer);
                //RemoveVisualChild(galleryGroupContainer);
                visualCollection.Remove(galleryGroupContainer);
            }
            galleryGroupContainers.Clear();
            
            // Gets filters
            string[] filter = Filter == null ? null : Filter.Split(',');

            Dictionary<string, GalleryGroupContainer> dictionary = new Dictionary<string, GalleryGroupContainer>();
            foreach(UIElement item in InternalChildren)
            {
                if (item == null) continue;

                // Resolve group name
                string propertyValue;
                if (GroupByAdvanced == null)
                {
                    propertyValue = (ItemContainerGenerator == null)
                                        ? GetPropertyValueAsString(item)
                                        : GetPropertyValueAsString(ItemContainerGenerator.ItemFromContainer(item));
                }
                else
                {
                    propertyValue = (ItemContainerGenerator == null)
                                        ? GroupByAdvanced(item)
                                        : GroupByAdvanced(ItemContainerGenerator.ItemFromContainer(item));
                }
                if (propertyValue == null) propertyValue = "Undefined";

                // Make invisible if it is not in filter (or is not grouped)
                if ((!IsGrouped) || (filter != null && !filter.Contains(propertyValue)))
                {
                    item.Measure(new Size(0,0));
                    item.Arrange(new Rect(0,0,0,0));
                }
                // Skip if it is not in filter
                if (filter != null && !filter.Contains(propertyValue)) continue;

                // To put all items in one group in case of IsGrouped = False
                if (!IsGrouped) propertyValue = "Undefined";
                
                if (!dictionary.ContainsKey(propertyValue))
                {
                    GalleryGroupContainer galleryGroupContainer = new GalleryGroupContainer();
                    galleryGroupContainer.Header = propertyValue;
                    RibbonControl.Bind(this, galleryGroupContainer, "GroupStyle", GroupStyleProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "Orientation", GalleryGroupContainer.OrientationProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "ItemWidth", GalleryGroupContainer.ItemWidthProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "ItemHeight", GalleryGroupContainer.ItemHeightProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "MaxItemsInRow", GalleryGroupContainer.MaxItemsInRowProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, "MinItemsInRow", GalleryGroupContainer.MinItemsInRowProperty, BindingMode.OneWay);
                    dictionary.Add(propertyValue,galleryGroupContainer);
                    galleryGroupContainers.Add(galleryGroupContainer);
                   
                    visualCollection.Add(galleryGroupContainer);
                }
                dictionary[propertyValue].Items.Add(new GalleryItemPlaceholder(item));
            }

            if (((!IsGrouped) || (GroupBy == null && GroupByAdvanced == null)) && galleryGroupContainers.Count != 0)
            {
                // Make it without headers
                galleryGroupContainers[0].IsHeadered = false;
            }
            
            InvalidateMeasure();
        }

        /// <summary>
        /// Invoked when the VisualCollection of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The Visual that was added to the collection.</param>
        /// <param name="visualRemoved">The Visual that was removed from the collection.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (visualRemoved is GalleryGroupContainer) return;
            if (visualAdded is GalleryGroupContainer) return;
            Invalidate();
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
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            double width = 0;
            double height = 0;
            foreach (GalleryGroupContainer child in galleryGroupContainers)
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
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {           
            Rect finalRect = new Rect(finalSize);

            foreach (GalleryGroupContainer item in galleryGroupContainers)
            {
                finalRect.Height = item.DesiredSize.Height;
                finalRect.Width = Math.Max(finalSize.Width, item.DesiredSize.Width);

                // Arrange a container to arrange placeholders
                item.Arrange(finalRect);
                
                finalRect.Y += item.DesiredSize.Height;

                // Now arrange our actual items using arranged size of placeholders
                foreach (GalleryItemPlaceholder placeholder in item.Items)
                {
                    Point leftTop = placeholder.TranslatePoint(new Point(), this);

                    placeholder.Target.Arrange(new Rect(leftTop.X, leftTop.Y,
                        placeholder.ArrangedSize.Width,
                        placeholder.ArrangedSize.Height));
                }
            }

            

            return finalSize;
        }

        #endregion

        #region Private Methods

        string GetPropertyValueAsString(object item)
        {
            if (item == null || GroupBy == null) return "Undefined";
            PropertyInfo property = item.GetType().GetProperty(GroupBy, BindingFlags.Public | BindingFlags.Instance);
            if (property == null) return "Undefined";
            object result = property.GetValue(item, null);
            if (result == null) return "Undefined";
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