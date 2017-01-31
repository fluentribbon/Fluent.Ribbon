using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace Fluent
{
  using System.Windows.Automation.Peers;
  using Fluent.Internal;

  /// <summary>
  /// Represent panel with ribbon tab items.
  /// It is automatically adjusting size of tabs
  /// </summary>
  public class RibbonTabsContainer : Panel, IScrollInfo
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabsContainer()
        {
            this.Focusable = false;
        }

        #region Layout Overridings

        /// <summary>
        /// Measures all of the RibbonGroupBox, and resize them appropriately
        /// to fit within the available room
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.InternalChildren.Count == 0) return base.MeasureOverride(availableSize);

            var desiredSize = this.MeasureChildrenDesiredSize(availableSize);

            // Performs steps as described in "2007 MICROSOFT® OFFICE FLUENT™ 
            // USER INTERFACE DESIGN GUIDELINES"

            // Step 1. Gradually remove empty space to the right of the tabs
            // If all tabs already in full size, just return
            if (availableSize.Width > desiredSize.Width)
            {
                // Hide separator lines between tabs
                this.UpdateSeparators(false, false);
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 2. Gradually and uniformly remove the padding from both sides 
            // of all the tabs until the minimum padding required for displaying 
            // the tab selection and hover states is reached (regular tabs)
            var overflowWidth = desiredSize.Width - availableSize.Width;
            var whitespace = ((RibbonTabItem)this.InternalChildren[0]).Indent;
            var contextualTabs = this.InternalChildren.Cast<RibbonTabItem>().Where(x => x.IsContextual && (x.Visibility != Visibility.Collapsed) && (x.Group.Visibility != Visibility.Collapsed))
                .ToList();

            double contextualTabsCount = contextualTabs.Count;
            var regularTabs = this.InternalChildren.Cast<RibbonTabItem>().Where(x => !x.IsContextual && (x.Visibility != Visibility.Collapsed))
                .ToList();

            double regularTabsCount = regularTabs.Count;//InternalChildren.Count - contextualTabsCount;
            var childrenCount = contextualTabsCount + regularTabsCount;

            if (overflowWidth < regularTabsCount * whitespace * 2)
            {
                var decreaseValue = overflowWidth / regularTabsCount;
                foreach (var tab in regularTabs) tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - decreaseValue), tab.DesiredSize.Height));// tab.Width = Math.Max(0, tab.ActualWidth - decreaseValue);
                desiredSize = this.GetChildrenDesiredSize();
                if (desiredSize.Width > availableSize.Width) desiredSize.Width = availableSize.Width;

                // Add separator lines between 
                // tabs to assist readability
                this.UpdateSeparators(false, false);
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 3. Gradually and uniformly remove the padding from both sides 
            // of all the tabs until the minimum padding required for displaying 
            // the tab selection and hover states is reached (contextual tabs)
            if (overflowWidth < childrenCount * whitespace * 2)
            {
                var regularTabsWhitespace = regularTabsCount * whitespace * 2.0;
                var decreaseValue = (overflowWidth - regularTabsWhitespace) / contextualTabsCount;

                foreach (var tab in regularTabs)
                {
                    //if (!tab.IsContextual)
                    {
                        var widthBeforeMeasure = tab.DesiredSize.Width;
                        tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));
                        overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                    }
                }

                foreach (var tab in contextualTabs.Reverse<RibbonTabItem>())
                {
                    //if (tab.IsContextual)
                    {
                        var widthBeforeMeasure = tab.DesiredSize.Width;
                        tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - decreaseValue), tab.DesiredSize.Height));

                        // Contextual tabs may overreduce, so check that
                        overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;

                        if (overflowWidth < 0)
                        {
                            break;
                        }
                    }
                }

                desiredSize = this.GetChildrenDesiredSize();

                if (desiredSize.Width > availableSize.Width)
                {
                    desiredSize.Width = availableSize.Width;
                }

                // Add separator lines between 
                // tabs to assist readability
                this.UpdateSeparators(true, false);
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 4. Reduce the width of the tab with the longest name by 
            // truncating the text label. Continue reducing the width of the largest 
            // tab (or tabs in the case of ties) until all tabs are the same width. 
            // (Regular tabs)
            foreach (var tab in regularTabs)
            {
                //if (!tab.IsContextual)
                {
                    var widthBeforeMeasure = tab.DesiredSize.Width;
                    tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));
                    overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                }
            }

            foreach (var tab in contextualTabs.Reverse<RibbonTabItem>())
            {
                //if (tab.IsContextual)
                {
                    var widthBeforeMeasure = tab.DesiredSize.Width;
                    tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));

                    // Contextual tabs may overreduce, so check that
                    overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;

                    if (overflowWidth < 0)
                    {
                        desiredSize = this.GetChildrenDesiredSize();
                        if (desiredSize.Width > availableSize.Width)
                        {
                            desiredSize.Width = availableSize.Width;
                        }

                        // Add separator lines between 
                        // tabs to assist readability
                        this.UpdateSeparators(true, false);
                        this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                        return desiredSize;
                    }
                }
            }

            // Sort regular tabs by descending
            var sortedRegularTabItems = regularTabs
                .OrderByDescending(x => x.DesiredSize.Width)
                .ToList();

            // Find how many regular tabs we have to reduce
            double reducedLength = 0;
            var reduceCount = 0;

            for (var i = 0; i < sortedRegularTabItems.Count - 1; i++)
            {
                var temp =sortedRegularTabItems[i].DesiredSize.Width - sortedRegularTabItems[i + 1].DesiredSize.Width;
                reducedLength += temp * (i + 1);
                reduceCount = i + 1;

                if (reducedLength > overflowWidth)
                {
                    break;
                }
            }

            if (reducedLength > overflowWidth)
            {
                // Reduce regular tabs
                var requiredWidth = sortedRegularTabItems[reduceCount].DesiredSize.Width;
                if (reducedLength > overflowWidth) requiredWidth += (reducedLength - overflowWidth) / reduceCount;

                for (var i = 0; i < reduceCount; i++)
                {
                    sortedRegularTabItems[i].Measure(new Size(requiredWidth, availableSize.Height));
                }

                desiredSize = this.GetChildrenDesiredSize();

                if (desiredSize.Width > availableSize.Width)
                {
                    desiredSize.Width = availableSize.Width;
                }

                // Add separator lines between 
                // tabs to assist readability
                this.UpdateSeparators(true, true);
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 5. Reduce the width of all regular tabs equally 
            // down to a minimum of about three characters.
            var regularTabsWidth = sortedRegularTabItems.Sum(x => x.DesiredSize.Width);
            var minimumRegularTabsWidth = MinimumRegularTabWidth * sortedRegularTabItems.Count;

            if (overflowWidth < regularTabsWidth - minimumRegularTabsWidth)
            {
                var settedWidth = (regularTabsWidth - overflowWidth) / regularTabsCount;

                for (var i = 0; i < regularTabsCount; i++)
                {
                    sortedRegularTabItems[i].Measure(new Size(settedWidth, availableSize.Height));
                }

                desiredSize = this.GetChildrenDesiredSize();
                //if (desiredSize.Width > availableSize.Width) desiredSize.Width = availableSize.Width;

                // Add separator lines between 
                // tabs to assist readability
                this.UpdateSeparators(true, true);
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 6. Reduce the width of the tab with the longest name by 
            // truncating the text label. Continue reducing the width of the largest 
            // tab (or tabs in the case of ties) until all tabs are the same width. 
            // (Contextual tabs)
            for (var i = 0; i < regularTabsCount; i++)
            {
                sortedRegularTabItems[i].Measure(new Size(MinimumRegularTabWidth, availableSize.Height));
            }

            overflowWidth -= regularTabsWidth - minimumRegularTabsWidth;

            // Sort contextual tabs by descending
            var sortedContextualTabItems = contextualTabs
                .OrderByDescending(x => x.DesiredSize.Width)
                .ToList();

            // Find how many contextual tabs we have to reduce
            reducedLength = 0;
            reduceCount = 0;

            for (var i = 0; i < sortedContextualTabItems.Count - 1; i++)
            {
                var temp = sortedContextualTabItems[i].DesiredSize.Width - sortedContextualTabItems[i + 1].DesiredSize.Width;
                reducedLength += temp * (i + 1);
                reduceCount = i + 1;

                if (reducedLength > overflowWidth)
                {
                    break;
                }
            }

            if (reducedLength > overflowWidth)
            {
                // Reduce regular tabs
                var requiredWidth = sortedContextualTabItems[reduceCount].DesiredSize.Width;
                if (reducedLength > overflowWidth)
                {
                    requiredWidth += (reducedLength - overflowWidth)/reduceCount;
                }

                for (var i = 0; i < reduceCount; i++)
                {
                    sortedContextualTabItems[i].Measure(new Size(requiredWidth, availableSize.Height));
                }

                desiredSize = this.GetChildrenDesiredSize();

                if (desiredSize.Width > availableSize.Width)
                {
                    desiredSize.Width = availableSize.Width;
                }

                // Add separator lines between 
                // tabs to assist readability
                this.UpdateSeparators(true, true);
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }
            else
            {
                var contextualTabsWidth = sortedContextualTabItems.Sum(x => x.DesiredSize.Width);

                var settedWidth = Math.Max(MinimumRegularTabWidth, (contextualTabsWidth - overflowWidth) / contextualTabsCount);

                for (var i = 0; i < sortedContextualTabItems.Count; i++)
                {
                    sortedContextualTabItems[i].Measure(new Size(settedWidth, availableSize.Height));
                }

                desiredSize = this.GetChildrenDesiredSize();

                // Add separator lines between 
                // tabs to assist readability
                this.UpdateSeparators(true, true);
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }
        }

        private Size MeasureChildrenDesiredSize(Size availableSize)
        {
            double width = 0;
            double height = 0;

            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(availableSize);
                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }

            return new Size(width, height);
        }

        private Size GetChildrenDesiredSize()
        {
            double width = 0;
            double height = 0;

            foreach (UIElement child in this.InternalChildren)
            {
                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }

            return new Size(width, height);
        }

        /// <summary>
        /// Positions child elements and determines
        /// a size for the control
        /// </summary>
        /// <param name="finalSize">The final area within the parent 
        /// that this element should use to arrange 
        /// itself and its children</param>
        /// <returns>The actual size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var finalRect = new Rect(finalSize)
            {
                X = -this.HorizontalOffset
            };

            var orderedChildren = this.InternalChildren.OfType<RibbonTabItem>()
                                      .OrderBy(x => x.Group != null);

            foreach (var item in orderedChildren)
            {
                finalRect.Width = item.DesiredSize.Width;
                finalRect.Height = Math.Max(finalSize.Height, item.DesiredSize.Height);
                item.Arrange(finalRect);
                finalRect.X += item.DesiredSize.Width;
            }

            return finalSize;
        }

        /// <summary>
        /// Updates separator visibility
        /// </summary>
        /// <param name="regularTabs">If this parameter true, regular tabs will have separators</param>
        /// <param name="contextualTabs">If this parameter true, contextual tabs will have separators</param>
        private void UpdateSeparators(bool regularTabs, bool contextualTabs)
        {
            foreach (RibbonTabItem tab in this.Children)
            {
                if (tab.IsContextual)
                {
                    if (tab.IsSeparatorVisible != contextualTabs)
                    {
                        tab.IsSeparatorVisible = contextualTabs;
                    }
                }
                else if (tab.IsSeparatorVisible != regularTabs)
                {
                    tab.IsSeparatorVisible = regularTabs;
                }
            }
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
            var newValue = CoerceOffset(ValidateInputOffset(offset, nameof(this.HorizontalOffset)), this.scrollData.ExtentWidth, this.scrollData.ViewportWidth);

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
                || this.IsAncestorOf(visual) == false)
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
        private const double MinimumRegularTabWidth = 30D;

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

            // newExtentWidth is neccessary to fix 20762 (Tab scroll button appears randomly when resizing)
            // To fix 20762 we are manipulating the extentWidth by checking if all regular (non contextual) tabs are at their minimum width.
            // When they are all at their minimum width we have to force the extentWidth to be greater than the viewportWidth.
            // When there are no regular tabs, we MUST NOT apply this fix
            var newExtentWidth = Math.Max(viewportWidth, extentWidth);

            var visibleRegularTabs = this.InternalChildren.Cast<RibbonTabItem>()
                .Where(item => item.IsContextual == false && item.Visibility != Visibility.Collapsed)
                .ToList();

            if (visibleRegularTabs.Any()
                && visibleRegularTabs.All(item => DoubleUtil.AreClose(item.DesiredSize.Width, MinimumRegularTabWidth)))
            {
                if (DoubleUtil.AreClose(newExtentWidth, viewportWidth))
                {
                    newExtentWidth = newExtentWidth + 1;
                }

                this.ScrollData.ExtentWidth = newExtentWidth;
            }
            else
            {
                this.ScrollData.ExtentWidth = this.ScrollData.ViewportWidth;
            }

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

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      RibbonAutomationPeer peer = new RibbonAutomationPeer(this);
      return peer;
    }
  }

    #region ScrollData

    /// <summary>
    /// Helper class to hold scrolling data.
    /// This class exists to reduce working set when SCP is delegating to another implementation of ISI.
    /// Standard "extra pointer always for less data sometimes" cache savings model:
    /// </summary>
    internal class ScrollData
    {
        /// <summary>
        /// Scroll viewer
        /// </summary>
        internal ScrollViewer ScrollOwner;

        /// <summary>
        /// Scroll offset
        /// </summary>
        internal double OffsetX;

        /// <summary>
        /// ViewportSize is computed from our FinalSize, but may be in different units.
        /// </summary>
        internal double ViewportWidth;

        /// <summary>
        /// Extent is the total size of our content.
        /// </summary>
        internal double ExtentWidth;
    }

    #endregion ScrollData
}