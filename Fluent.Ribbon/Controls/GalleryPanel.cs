// ReSharper disable once CheckNamespace
namespace Fluent
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
    using Fluent.Extensions;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents panel for Gallery and InRibbonGallery with grouping and filtering capabilities
    /// </summary>
    public class GalleryPanel : StackPanel
    {
        // todo: localization
        private const string Undefined = "Undefined";

        #region Fields

        // Currently used group containers
        private readonly List<GalleryGroupContainer> galleryGroupContainers = new List<GalleryGroupContainer>();

        // Designate that gallery panel must be refreshed its groups
        private bool needsRefresh;

        #endregion

        #region Properties

        /// <summary>
        /// Used to prevent measures which cause the layout to flicker.
        /// This is needed when the gallery panel has switched owners during InRibbonGallery popup open/close.
        /// </summary>
        public bool IgnoreNextMeasureCall { get; set; }

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
            DependencyProperty.Register(nameof(IsGrouped), typeof(bool), typeof(GalleryPanel),
            new PropertyMetadata(BooleanBoxes.TrueBox, OnIsGroupedChanged));

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
        public static readonly DependencyProperty GroupByProperty = DependencyProperty.Register(nameof(GroupBy), typeof(string), typeof(GalleryPanel), new PropertyMetadata(OnGroupByChanged));

        private static void OnGroupByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryPanel = (GalleryPanel)d;
            galleryPanel.Invalidate();
        }

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
        public static readonly DependencyProperty GroupByAdvancedProperty = DependencyProperty.Register(nameof(GroupByAdvanced), typeof(Func<object, string>), typeof(GalleryPanel), new PropertyMetadata(OnGroupByAdvancedChanged));

        private static void OnGroupByAdvancedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryPanel = (GalleryPanel)d;
            galleryPanel.Invalidate();
        }

        #endregion

        #region ItemContainerGenerator

        /// <summary>
        /// Gets or sets ItemContainerGenerator which generates the
        /// user interface (UI) on behalf of its host, such as an  ItemsControl.
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get { return (ItemContainerGenerator)this.GetValue(ItemContainerGeneratorProperty); }
            set { this.SetValue(ItemContainerGeneratorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerGenerator.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerGeneratorProperty =
            DependencyProperty.Register(nameof(ItemContainerGenerator), typeof(ItemContainerGenerator),
            typeof(GalleryPanel), new PropertyMetadata(OnItemContainerGeneratorChanged));

        private static void OnItemContainerGeneratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryPanel = (GalleryPanel)d;
            galleryPanel.Invalidate();
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
            DependencyProperty.Register(nameof(GroupStyle), typeof(Style),
            typeof(GalleryPanel), new PropertyMetadata());

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
            typeof(GalleryPanel), new PropertyMetadata(DoubleBoxes.NaN));

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
            typeof(GalleryPanel), new PropertyMetadata(DoubleBoxes.NaN));

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
            DependencyProperty.Register(nameof(Filter), typeof(string),
            typeof(GalleryPanel), new PropertyMetadata(OnFilterChanged));

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
            DependencyProperty.Register(nameof(MinItemsInRow), typeof(int),
            typeof(GalleryPanel), new PropertyMetadata(1, OnMinItemsInRowChanged));

        private static void OnMinItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnMinOrMaxItemsInRowChanged(d, e);
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
            DependencyProperty.Register(nameof(MaxItemsInRow), typeof(int),
            typeof(GalleryPanel), new PropertyMetadata(int.MaxValue, OnMaxItemsInRowChanged));

        private static void OnMaxItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnMinOrMaxItemsInRowChanged(d, e);
        }

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

        private readonly VisualCollection visualCollection;

        /// <inheritdoc />
        protected override int VisualChildrenCount => base.VisualChildrenCount + this.visualCollection.Count;

        /// <inheritdoc />
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
                if (VisualTreeHelper.GetChild(visual, i) is UIElement element)
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
            if (this.needsRefresh)
            {
                return;
            }

            this.needsRefresh = true;
            this.RunInDispatcherAsync(this.RefreshDispatchered, DispatcherPriority.Send);
        }

        private void RefreshDispatchered()
        {
            if (this.needsRefresh == false)
            {
                return;
            }

            this.Refresh();
            this.needsRefresh = false;
        }

        private void Refresh()
        {
            // Clear currently used group containers
            // and supply with new generated ones
            foreach (var galleryGroupContainer in this.galleryGroupContainers)
            {
                BindingOperations.ClearAllBindings(galleryGroupContainer);
                this.visualCollection.Remove(galleryGroupContainer);
            }

            this.galleryGroupContainers.Clear();

            // Gets filters
            var filter = this.Filter?.Split(',');

            var dictionary = new Dictionary<string, GalleryGroupContainer>();

            foreach (UIElement item in this.InternalChildren)
            {
                if (item == null)
                {
                    continue;
                }

                // Resolve group name
                string propertyValue;

                if (this.GroupByAdvanced == null)
                {
                    propertyValue = this.ItemContainerGenerator == null
                                        ? this.GetPropertyValueAsString(item)
                                        : this.GetPropertyValueAsString(this.ItemContainerGenerator.ItemFromContainer(item));
                }
                else
                {
                    propertyValue = this.ItemContainerGenerator == null
                                        ? this.GroupByAdvanced(item)
                                        : this.GroupByAdvanced(this.ItemContainerGenerator.ItemFromContainer(item));
                }

                if (propertyValue == null)
                {
                    propertyValue = Undefined;
                }

                // Make invisible if it is not in filter (or is not grouped)
                if (this.IsGrouped == false
                    || (filter != null && filter.Contains(propertyValue) == false))
                {
                    item.Measure(new Size(0, 0));
                    item.Arrange(new Rect(0, 0, 0, 0));
                }

                // Skip if it is not in filter
                if (filter != null
                    && filter.Contains(propertyValue) == false)
                {
                    continue;
                }

                // To put all items in one group in case of IsGrouped = False
                if (this.IsGrouped == false)
                {
                    propertyValue = Undefined;
                }

                if (dictionary.ContainsKey(propertyValue) == false)
                {
                    var galleryGroupContainer = new GalleryGroupContainer
                    {
                        Header = propertyValue
                    };
                    RibbonControl.Bind(this, galleryGroupContainer, nameof(this.GroupStyle), GroupStyleProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, nameof(this.Orientation), GalleryGroupContainer.OrientationProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, nameof(this.ItemWidth), GalleryGroupContainer.ItemWidthProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, nameof(this.ItemHeight), GalleryGroupContainer.ItemHeightProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, nameof(this.MaxItemsInRow), GalleryGroupContainer.MaxItemsInRowProperty, BindingMode.OneWay);
                    RibbonControl.Bind(this, galleryGroupContainer, nameof(this.MinItemsInRow), GalleryGroupContainer.MinItemsInRowProperty, BindingMode.OneWay);
                    dictionary.Add(propertyValue, galleryGroupContainer);
                    this.galleryGroupContainers.Add(galleryGroupContainer);

                    this.visualCollection.Add(galleryGroupContainer);
                }

                dictionary[propertyValue].Items.Add(new GalleryItemPlaceholder(item));
            }

            if ((this.IsGrouped == false || (this.GroupBy == null && this.GroupByAdvanced == null))
                && this.galleryGroupContainers.Count != 0)
            {
                // Make it without headers if there is only one group and we are not supposed to group
                this.galleryGroupContainers[0].IsHeadered = false;
            }

            this.UpdateMinAndMaxWidth();
            this.InvalidateMeasure();
        }

        /// <inheritdoc />
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (visualRemoved is GalleryGroupContainer)
            {
                return;
            }

            if (visualAdded is GalleryGroupContainer)
            {
                return;
            }

            this.Invalidate();
        }

        #endregion

        #region Layout Overrides

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.IgnoreNextMeasureCall)
            {
                this.IgnoreNextMeasureCall = false;
                return this.DesiredSize;
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

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
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
                    var leftTop = placeholder.TranslatePoint(default, this);

                    placeholder.Target.Arrange(new Rect(leftTop.X, leftTop.Y, placeholder.ArrangedSize.Width, placeholder.ArrangedSize.Height));
                }
            }

            return finalSize;
        }

        #endregion

        #region Private Methods

        private static void OnMinOrMaxItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var galleryPanel = (GalleryPanel)d;
            galleryPanel.UpdateMinAndMaxWidth();
        }

        private string GetPropertyValueAsString(object item)
        {
            if (item == null ||
                this.GroupBy == null)
            {
                return Undefined;
            }

            var property = item.GetType().GetProperty(this.GroupBy, BindingFlags.Public | BindingFlags.Instance);

            var result = property?.GetValue(item, null);
            if (result == null)
            {
                return Undefined;
            }

            return result.ToString();
        }

        #endregion

        /// <inheritdoc />
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