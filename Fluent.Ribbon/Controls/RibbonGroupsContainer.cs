using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal;

    /// <summary>
    /// Represent panel with ribbon group.
    /// It is automatically adjusting size of controls
    /// </summary>
    public class RibbonGroupsContainer : Panel, IScrollInfo
    {
        #region Reduce Order

        /// <summary>
        /// Gets or sets reduce order of group in the ribbon panel.
        /// It must be enumerated with comma from the first to reduce to 
        /// the last to reduce (use Control.Name as group name in the enum). 
        /// Enclose in parentheses as (Control.Name) to reduce/enlarge 
        /// scalable elements in the given group
        /// </summary>
        public string ReduceOrder
        {
            get { return (string)this.GetValue(ReduceOrderProperty); }
            set { this.SetValue(ReduceOrderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReduceOrder.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReduceOrderProperty =
            DependencyProperty.Register(nameof(ReduceOrder), typeof(string), typeof(RibbonGroupsContainer), new PropertyMetadata(ReduceOrderPropertyChanged));

        // handles ReduseOrder property changed
        private static void ReduceOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbonPanel = (RibbonGroupsContainer)d;
            ribbonPanel.reduceOrder = ((string)e.NewValue).Split(new [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ribbonPanel.reduceOrderIndex = ribbonPanel.reduceOrder.Length - 1;

            ribbonPanel.InvalidateMeasure();
            ribbonPanel.InvalidateArrange();
        }

        #endregion

        #region Fields

        private string[] reduceOrder = new string[0];
        private int reduceOrderIndex;

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonGroupsContainer()
        {
            this.Focusable = false;
        }

        #endregion

        #region Layout Overridings

        /// <summary>
        ///   Returns a collection of the panel's UIElements.
        /// </summary>
        /// <param name="logicalParent">The logical parent of the collection to be created.</param>
        /// <returns>Returns an ordered collection of elements that have the specified logical parent.</returns>
        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return new UIElementCollection(this, /*Parent as FrameworkElement*/this);
        }

        /// <summary>
        /// Measures all of the RibbonGroupBox, and resize them appropriately
        /// to fit within the available room
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var desiredSize = this.GetChildrenDesiredSizeIntermediate();

            if (this.reduceOrder.Length == 0)
            {
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // If we have more available space - try to expand groups
            while (desiredSize.Width <= availableSize.Width)
            {
                var hasMoreVariants = this.reduceOrderIndex < this.reduceOrder.Length - 1;
                if (hasMoreVariants == false)
                {
                    break;
                }

                // Increase size of another item
                this.reduceOrderIndex++;
                this.IncreaseGroupBoxSize(this.reduceOrder[this.reduceOrderIndex]);

                desiredSize = this.GetChildrenDesiredSizeIntermediate();
            }

            // If not enough space - go to next variant
            while (desiredSize.Width > availableSize.Width)
            {
                var hasMoreVariants = this.reduceOrderIndex >= 0;
                if (hasMoreVariants == false)
                {
                    break;
                }

                // Decrease size of another item
                this.DecreaseGroupBoxSize(this.reduceOrder[this.reduceOrderIndex]);
                this.reduceOrderIndex--;

                desiredSize = this.GetChildrenDesiredSizeIntermediate();
            }

            // Set find values
            foreach (var item in this.InternalChildren)
            {
                var groupBox = item as RibbonGroupBox;
                if (groupBox == null)
                {
                    continue;
                }

                if (groupBox.State != groupBox.StateIntermediate 
                    || groupBox.Scale != groupBox.ScaleIntermediate)
                {
                    groupBox.SuppressCacheReseting = true;
                    groupBox.State = groupBox.StateIntermediate;
                    groupBox.Scale = groupBox.ScaleIntermediate;
                    groupBox.InvalidateLayout();
                    groupBox.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                    groupBox.SuppressCacheReseting = false;
                }

                // Something wrong with cache?
                if (groupBox.DesiredSizeIntermediate != groupBox.DesiredSize)
                {
                    // Reset cache and reinvoke masure
                    groupBox.ClearCache();
                    return this.MeasureOverride(availableSize);
                }
            }

            this.VerifyScrollData(availableSize.Width, desiredSize.Width);
            return desiredSize;
        }

        private Size GetChildrenDesiredSizeIntermediate()
        {
            double width = 0;
            double height = 0;

            foreach (UIElement child in this.InternalChildren)
            {
                var groupBox = child as RibbonGroupBox;
                if (groupBox == null)
                {
                    continue;
                }

                var desiredSize = groupBox.DesiredSizeIntermediate;
                width += desiredSize.Width;
                height = Math.Max(height, desiredSize.Height);
            }

            return new Size(width, height);
        }

        // Increase size of the item
        private void IncreaseGroupBoxSize(string name)
        {
            var groupBox = this.FindGroup(name);
            var scale = name.StartsWith("(", StringComparison.OrdinalIgnoreCase);

            if (groupBox == null)
            {
                return;
            }

            if (scale)
            {
                groupBox.ScaleIntermediate++;
            }
            else
            {
                groupBox.StateIntermediate = groupBox.StateIntermediate != RibbonGroupBoxState.Large
                    ? groupBox.StateIntermediate - 1
                    : RibbonGroupBoxState.Large;
            }
        }

        // Decrease size of the item
        private void DecreaseGroupBoxSize(string name)
        {
            var groupBox = this.FindGroup(name);
            var scale = name.StartsWith("(", StringComparison.OrdinalIgnoreCase);

            if (groupBox == null)
            {
                return;
            }

            if (scale)
            {
                groupBox.ScaleIntermediate--;
            }
            else
            {
                groupBox.StateIntermediate = groupBox.StateIntermediate != RibbonGroupBoxState.Collapsed
                    ? groupBox.StateIntermediate + 1
                    : groupBox.StateIntermediate;
            }
        }

        private RibbonGroupBox FindGroup(string name)
        {
            if (name.StartsWith("(", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(1, name.Length - 2);
            }

            foreach (FrameworkElement child in this.InternalChildren)
            {
                if (child.Name == name)
                {
                    return child as RibbonGroupBox;
                }
            }

            return null;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines 
        /// a size for a System.Windows.FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var finalRect = new Rect(finalSize)
            {
                X = -this.HorizontalOffset
            };

            foreach (UIElement item in this.InternalChildren)
            {
                finalRect.Width = item.DesiredSize.Width;
                finalRect.Height = Math.Max(finalSize.Height, item.DesiredSize.Height);
                item.Arrange(finalRect);
                finalRect.X += item.DesiredSize.Width;
            }

            return finalSize;
        }

        #endregion

        #region IScrollInfo Members

        /// <summary>
        /// Gets or sets a System.Windows.Controls.ScrollViewer element that controls scrolling behavior.
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get { return this.ScrollData.ScrollOwner; }
            set { this.ScrollData.ScrollOwner = value; }
        }

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        public void SetHorizontalOffset(double offset)
        {
            var newValue = CoerceOffset(ValidateInputOffset(offset, "HorizontalOffset"), this.scrollData.ExtentWidth, this.scrollData.ViewportWidth);

            if (DoubleUtil.AreClose(this.ScrollData.OffsetX, newValue) == false)
            {
                this.scrollData.OffsetX = newValue;
                this.InvalidateArrange();
            }
        }
        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        public double ExtentWidth
        {
            get { return this.ScrollData.ExtentWidth; }
        }

        /// <summary>
        /// Gets the horizontal offset of the scrolled content.
        /// </summary>
        public double HorizontalOffset
        {
            get { return this.ScrollData.OffsetX; }
        }

        /// <summary>
        /// Gets the horizontal size of the viewport for this content.
        /// </summary>
        public double ViewportWidth
        {
            get { return this.ScrollData.ViewportWidth; }
        }

        /// <summary>
        /// Scrolls left within content by one logical unit.
        /// </summary>
        public void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - 16.0);
        }

        /// <summary>
        /// Scrolls right within content by one logical unit.
        /// </summary>
        public void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + 16.0);
        }

        /// <summary>
        /// Forces content to scroll until the coordinate space of a System.Windows.Media.Visual object is visible.
        /// This is optimized for horizontal scrolling only
        /// </summary>
        /// <param name="visual">A System.Windows.Media.Visual that becomes visible.</param>
        /// <param name="rectangle">A bounding rectangle that identifies the coordinate space to make visible.</param>
        /// <returns>A System.Windows.Rect that is visible.</returns>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            // We can only work on visuals that are us or children.
            // An empty rect has no size or position.  We can't meaningfully use it.
            if (rectangle.IsEmpty
                || visual == null
                || ReferenceEquals(visual, this)
                || !this.IsAncestorOf(visual))
            {
                return Rect.Empty;
            }

            // Compute the child's rect relative to (0,0) in our coordinate space.
            var childTransform = visual.TransformToAncestor(this);

            rectangle = childTransform.TransformBounds(rectangle);

            // Initialize the viewport
            var viewport = new Rect(this.HorizontalOffset, rectangle.Top, this.ViewportWidth, rectangle.Height);
            rectangle.X += viewport.X;

            // Compute the offsets required to minimally scroll the child maximally into view.
            var minX = ComputeScrollOffsetWithMinimalScroll(viewport.Left, viewport.Right, rectangle.Left, rectangle.Right);

            // We have computed the scrolling offsets; scroll to them.
            this.SetHorizontalOffset(minX);

            // Compute the visible rectangle of the child relative to the viewport.
            viewport.X = minX;
            rectangle.Intersect(viewport);

            rectangle.X -= viewport.X;

            // Return the rectangle
            return rectangle;
        }

        private static double ComputeScrollOffsetWithMinimalScroll(
            double topView,
            double bottomView,
            double topChild,
            double bottomChild)
        {
            // # CHILD POSITION       CHILD SIZE      SCROLL      REMEDY
            // 1 Above viewport       <= viewport     Down        Align top edge of child & viewport
            // 2 Above viewport       > viewport      Down        Align bottom edge of child & viewport
            // 3 Below viewport       <= viewport     Up          Align bottom edge of child & viewport
            // 4 Below viewport       > viewport      Up          Align top edge of child & viewport
            // 5 Entirely within viewport             NA          No scroll.
            // 6 Spanning viewport                    NA          No scroll.
            //
            // Note: "Above viewport" = childTop above viewportTop, childBottom above viewportBottom
            //       "Below viewport" = childTop below viewportTop, childBottom below viewportBottom
            // These child thus may overlap with the viewport, but will scroll the same direction
            /*bool fAbove = DoubleUtil.LessThan(topChild, topView) && DoubleUtil.LessThan(bottomChild, bottomView);
            bool fBelow = DoubleUtil.GreaterThan(bottomChild, bottomView) && DoubleUtil.GreaterThan(topChild, topView);*/
            var fAbove = (topChild < topView) && (bottomChild < bottomView);
            var fBelow = (bottomChild > bottomView) && (topChild > topView);
            var fLarger = bottomChild - topChild > bottomView - topView;

            // Handle Cases:  1 & 4 above
            if ((fAbove && !fLarger)
               || (fBelow && fLarger))
            {
                return topChild;
            }

            // Handle Cases: 2 & 3 above
            if (fAbove || fBelow)
            {
                return bottomChild - (bottomView - topView);
            }

            // Handle cases: 5 & 6 above.
            return topView;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void MouseWheelDown()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void MouseWheelLeft()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void MouseWheelRight()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void MouseWheelUp()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void LineDown()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void LineUp()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void PageDown()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void PageLeft()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void PageRight()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void PageUp()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="offset"></param>
        public void SetVerticalOffset(double offset)
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the vertical axis is possible.
        /// </summary>
        public bool CanVerticallyScroll
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis is possible.
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get { return true; }
            set { }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public double ExtentHeight
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public double VerticalOffset
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public double ViewportHeight
        {
            get { return 0.0; }
        }

        // Gets scroll data info
        private ScrollData ScrollData
        {
            get
            {
                return this.scrollData ?? (this.scrollData = new ScrollData());
            }
        }

        // Scroll data info
        private ScrollData scrollData;

        // Validates input offset
        private static double ValidateInputOffset(double offset, string parameterName)
        {
            if (double.IsNaN(offset))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }

            return Math.Max(0.0, offset);
        }

        // Verifies scrolling data using the passed viewport and extent as newly computed values.
        // Checks the X/Y offset and coerces them into the range [0, Extent - ViewportSize]
        // If extent, viewport, or the newly coerced offsets are different than the existing offset,
        //   cachces are updated and InvalidateScrollInfo() is called.
        private void VerifyScrollData(double viewportWidth, double extentWidth)
        {
            var isValid = true;

            if (double.IsInfinity(viewportWidth))
            {
                viewportWidth = extentWidth;
            }

            var offsetX = CoerceOffset(this.ScrollData.OffsetX, extentWidth, viewportWidth);

            isValid &= DoubleUtil.AreClose(viewportWidth, this.ScrollData.ViewportWidth);
            isValid &= DoubleUtil.AreClose(extentWidth, this.ScrollData.ExtentWidth);
            isValid &= DoubleUtil.AreClose(this.ScrollData.OffsetX, offsetX);

            this.ScrollData.ViewportWidth = viewportWidth;
            this.ScrollData.ExtentWidth = extentWidth;
            this.ScrollData.OffsetX = offsetX;

            if (isValid == false)
            {
                this.ScrollOwner?.InvalidateScrollInfo();
            }
        }

        // Returns an offset coerced into the [0, Extent - Viewport] range.
        private static double CoerceOffset(double offset, double extent, double viewport)
        {
            if (offset > extent - viewport)
            {
                offset = extent - viewport;
            }

            if (offset < 0)
            {
                offset = 0;
            }

            return offset;
        }

        #endregion
    }
}