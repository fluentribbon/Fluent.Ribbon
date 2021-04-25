// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using Fluent.Internal;

    /// <summary>
    /// Represent panel with ribbon tab items.
    /// It is automatically adjusting size of tabs
    /// </summary>
    public class RibbonTabsContainer : Panel, IScrollInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonTabsContainer"/> class.
        /// </summary>
        public RibbonTabsContainer()
        {
            this.Focusable = false;
        }

        static RibbonTabsContainer()
        {
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(RibbonTabsContainer), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(RibbonTabsContainer), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        #region Layout Overridings

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.InternalChildren.Count == 0)
            {
                return base.MeasureOverride(availableSize);
            }

            var desiredSize = this.MeasureChildrenDesiredSize(availableSize);

            // Step 1. If all tabs already fit, just return.
            if (availableSize.Width >= desiredSize.Width
                || DoubleUtil.AreClose(availableSize.Width, desiredSize.Width))
            {
                // Hide separator lines between tabs
                this.UpdateSeparators(false, false);
                this.VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Size reduction:
            // - calculate the overflow width
            // - get all visible tabs ordered by:
            //   - non context tabs first
            //   - largest tabs first
            // - then loop over all tabs and reduce their size in steps
            //   - during each tabs reduction check if it's still the largest tab
            //   - if it's still the largest tab reduce it's size further till there is no larger tab left
            var overflowWidth = desiredSize.Width - availableSize.Width;

            var visibleTabs = this.InternalChildren.Cast<RibbonTabItem>()
                                  .Where(x => x.Visibility != Visibility.Collapsed)
                                  .OrderBy(x => x.IsContextual)
                                  .ToList();

            // did we change the size of any contextual tabs?
            var contextualTabsSizeChanged = false;

            // step size for reducing the size of tabs
            const int sizeChangeStepSize = 4;

            // loop while we got overflow left (still need to reduce more) and all tabs are larger than the minimum size
            while (overflowWidth > 0
                   && AreAnyTabsAboveMinimumSize(visibleTabs))
            {
                var tabsChangedInSize = 0;
                foreach (var tab in visibleTabs.OrderByDescending(x => x.DesiredSize.Width))
                {
                    var widthBeforeMeasure = tab.DesiredSize.Width;

                    // ignore tabs that are smaller or equal to the minimum size
                    if (widthBeforeMeasure < MinimumRegularTabWidth
                        || DoubleUtil.AreClose(widthBeforeMeasure, MinimumRegularTabWidth))
                    {
                        continue;
                    }

                    var wasLargestTab = IsLargestTab(visibleTabs, tab.DesiredSize.Width, tab.IsContextual);

                    // measure with reduced size, but at least the minimum size
                    tab.Measure(new Size(Math.Max(MinimumRegularTabWidth, tab.DesiredSize.Width - sizeChangeStepSize), tab.DesiredSize.Height));

                    // calculate diff of measure before and after possible reduction
                    var widthDifference = widthBeforeMeasure - tab.DesiredSize.Width;
                    var didWidthChange = widthDifference > 0;

                    // count as changed if diff is greater than zero
                    tabsChangedInSize += didWidthChange
                                       ? 1
                                       : 0;

                    // was it a changed contextual tab?
                    if (tab.IsContextual
                        && didWidthChange)
                    {
                        contextualTabsSizeChanged = true;
                    }

                    // reduce remaining overflow width
                    overflowWidth -= widthDifference;

                    // break if no overflow width is left
                    if (overflowWidth <= 0)
                    {
                        break;
                    }

                    // if the current tab was the largest tab break to reduce it's size further
                    if (wasLargestTab
                        && didWidthChange)
                    {
                        break;
                    }
                }

                // break if no tabs changed their size
                if (tabsChangedInSize == 0)
                {
                    break;
                }
            }

            desiredSize = this.GetChildrenDesiredSize();

            // Add separator lines between
            // tabs to assist readability
            this.UpdateSeparators(true, contextualTabsSizeChanged || AreAnyTabsAboveMinimumSize(visibleTabs) == false);
            this.VerifyScrollData(availableSize.Width, desiredSize.Width);
            return desiredSize;
        }

        private static bool AreAnyTabsAboveMinimumSize(List<RibbonTabItem> tabs)
        {
            return tabs.Any(item => item.DesiredSize.Width > MinimumRegularTabWidth);
        }

        private static bool IsLargestTab(List<RibbonTabItem> tabs, double width, bool isContextual)
        {
            return tabs.Count > 1 && tabs.Any(x => x.IsContextual == isContextual && x.DesiredSize.Width > width) == false;
        }

        private Size MeasureChildrenDesiredSize(Size availableSize)
        {
            double width = 0;
            double height = 0;

            foreach (UIElement? child in this.InternalChildren)
            {
                if (child is null)
                {
                    continue;
                }

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

            foreach (UIElement? child in this.InternalChildren)
            {
                if (child is null)
                {
                    continue;
                }

                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }

            return new Size(width, height);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            var finalRect = new Rect(finalSize)
            {
                X = -this.HorizontalOffset
            };

            var orderedChildren = this.InternalChildren.OfType<RibbonTabItem>()
                                      .OrderBy(x => x.Group is not null);

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
            foreach (RibbonTabItem? tab in this.Children)
            {
                if (tab is null)
                {
                    continue;
                }

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

        /// <inheritdoc />
        public ScrollViewer? ScrollOwner
        {
            get { return this.ScrollData.ScrollOwner; }
            set { this.ScrollData.ScrollOwner = value; }
        }

        /// <inheritdoc />
        public void SetHorizontalOffset(double offset)
        {
            var newValue = CoerceOffset(ValidateInputOffset(offset, nameof(this.HorizontalOffset)), this.ScrollData.ExtentWidth, this.ScrollData.ViewportWidth);

            if (DoubleUtil.AreClose(this.ScrollData.OffsetX, newValue) == false)
            {
                this.ScrollData.OffsetX = newValue;
                this.InvalidateMeasure();
                this.ScrollOwner?.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc />
        public double ExtentWidth => this.ScrollData.ExtentWidth;

        /// <inheritdoc />
        public double HorizontalOffset => this.ScrollData.OffsetX;

        /// <inheritdoc />
        public double ViewportWidth => this.ScrollData.ViewportWidth;

        /// <inheritdoc />
        public void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - 16.0);
        }

        /// <inheritdoc />
        public void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + 16.0);
        }

        /// <inheritdoc />
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            // We can only work on visuals that are us or children.
            // An empty rect has no size or position.  We can't meaningfully use it.
            if (rectangle.IsEmpty
                || visual is null
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

        /// <inheritdoc />
        public void MouseWheelLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - 16);
        }

        /// <inheritdoc />
        public void MouseWheelRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + 16);
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

        /// <inheritdoc />
        public void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
        }

        /// <inheritdoc />
        public void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
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
        public void SetVerticalOffset(double offset)
        {
        }

        /// <inheritdoc />
        public bool CanVerticallyScroll
        {
            get => false;
            set { }
        }

        /// <inheritdoc />
        public bool CanHorizontallyScroll
        {
            get => true;
            set { }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public double ExtentHeight => 0.0;

        /// <summary>
        /// Not implemented
        /// </summary>
        public double VerticalOffset => 0.0;

        /// <summary>
        /// Not implemented
        /// </summary>
        public double ViewportHeight => 0.0;

        // Gets scroll data info
        private ScrollData ScrollData => this.scrollData ?? (this.scrollData = new ScrollData());

        // Scroll data info
        private ScrollData? scrollData;
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
        //   caches are updated and InvalidateScrollInfo() is called.
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

            // Prevent flickering by only using extentWidth if it's at least 2 larger than viewportWidth
            if (viewportWidth + 2 < extentWidth)
            {
                this.ScrollData.ExtentWidth = extentWidth;
            }
            else
            {
                // Or we show show the srollbar if all tabs are at their minimum width or smaller
                // but do this early (if extent + 2 is equal or larger than the viewport, or they are equal)
                if (extentWidth + 2 >= viewportWidth
                    || DoubleUtil.AreClose(extentWidth, viewportWidth))
                {
                    var visibleTabs = this.InternalChildren.Cast<RibbonTabItem>().Where(item => item.Visibility != Visibility.Collapsed).ToList();

                    var newExtentWidth = viewportWidth;

                    if (visibleTabs.Any()
                        && visibleTabs.All(item => DoubleUtil.AreClose(item.DesiredSize.Width, MinimumRegularTabWidth) || item.DesiredSize.Width < MinimumRegularTabWidth))
                    {
                        if (DoubleUtil.AreClose(newExtentWidth, viewportWidth))
                        {
                            newExtentWidth += 1;
                        }

                        this.ScrollData.ExtentWidth = newExtentWidth;
                    }
                    else
                    {
                        this.ScrollData.ExtentWidth = viewportWidth;
                    }
                }
                else
                {
                    this.ScrollData.ExtentWidth = viewportWidth;
                }
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
        internal ScrollViewer? ScrollOwner { get; set; }

        /// <summary>
        /// Scroll offset
        /// </summary>
        internal double OffsetX { get; set; }

        /// <summary>
        /// ViewportSize is computed from our FinalSize, but may be in different units.
        /// </summary>
        internal double ViewportWidth { get; set; }

        /// <summary>
        /// Extent is the total size of our content.
        /// </summary>
        internal double ExtentWidth { get; set; }
    }

    #endregion ScrollData
}