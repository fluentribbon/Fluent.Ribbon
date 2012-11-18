#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Fluent
{
    using Fluent.Internal;

    /// <summary>
    /// Represent panel with ribbon tab items.
    /// It is automatically adjusting size of tabs
    /// </summary>
    public class RibbonTabsContainer : Panel, IScrollInfo
    {
        #region Fields



        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabsContainer()
        {
            Focusable = false;
        }

        #endregion

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
            if (InternalChildren.Count == 0) return base.MeasureOverride(availableSize);

            Size desiredSize = MeasureChildrenDesiredSize(availableSize);

            // Performs steps as described in "2007 MICROSOFT® OFFICE FLUENT™ 
            // USER INTERFACE DESIGN GUIDELINES"

            // Step 1. Gradually remove empty space to the right of the tabs
            // If all tabs already in full size, just return
            if (availableSize.Width > desiredSize.Width)
            {
                // Hide separator lines between tabs
                UpdateSeparators(false, false);
                VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 2. Gradually and uniformly remove the padding from both sides 
            // of all the tabs until the minimum padding required for displaying 
            // the tab selection and hover states is reached (regular tabs)
            double overflowWidth = desiredSize.Width - availableSize.Width;
            double whitespace = (InternalChildren[0] as RibbonTabItem).Indent;
            RibbonTabItem[] contextualTabs = InternalChildren.Cast<RibbonTabItem>().Where(x => (x.IsContextual) && (x.Visibility != Visibility.Collapsed) && (x.Group.Visibility != Visibility.Collapsed)).ToArray();
            double contextualTabsCount = contextualTabs.Length;
            var regularTabs =
                InternalChildren.Cast<RibbonTabItem>().Where(x => (!x.IsContextual) && (x.Visibility != Visibility.Collapsed));
            double regularTabsCount = regularTabs.Count();//InternalChildren.Count - contextualTabsCount;
            double childrenCount = contextualTabsCount + regularTabsCount;
            if (overflowWidth < regularTabsCount * whitespace * 2)
            {
                double decreaseValue = overflowWidth / (double)regularTabsCount;
                foreach (RibbonTabItem tab in regularTabs) tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - decreaseValue), tab.DesiredSize.Height));// tab.Width = Math.Max(0, tab.ActualWidth - decreaseValue);
                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > availableSize.Width) desiredSize.Width = availableSize.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, false);
                VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 3. Gradually and uniformly remove the padding from both sides 
            // of all the tabs until the minimum padding required for displaying 
            // the tab selection and hover states is reached (contextual tabs)
            if (overflowWidth < childrenCount * whitespace * 2)
            {
                double regularTabsWhitespace = (double)regularTabsCount * whitespace * 2.0;
                double decreaseValue = (overflowWidth - regularTabsWhitespace) / (double)contextualTabsCount;
                foreach (RibbonTabItem tab in regularTabs)
                {
                    //if (!tab.IsContextual)
                    {
                        double widthBeforeMeasure = tab.DesiredSize.Width;
                        tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));
                        overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                    }
                }
                foreach (RibbonTabItem tab in contextualTabs.Reverse())
                {
                    //if (tab.IsContextual)
                    {
                        double widthBeforeMeasure = tab.DesiredSize.Width;
                        tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - decreaseValue), tab.DesiredSize.Height));

                        // Contextual tabs may overreduce, so check that
                        overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                        if (overflowWidth < 0) break;
                    }
                }
                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > availableSize.Width) desiredSize.Width = availableSize.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, false);
                VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }


            // Step 4. Reduce the width of the tab with the longest name by 
            // truncating the text label. Continue reducing the width of the largest 
            // tab (or tabs in the case of ties) until all tabs are the same width. 
            // (Regular tabs)
            foreach (RibbonTabItem tab in regularTabs)
            {
                //if (!tab.IsContextual)
                {
                    double widthBeforeMeasure = tab.DesiredSize.Width;
                    tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));
                    overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                }
            }
            foreach (RibbonTabItem tab in contextualTabs.Reverse())
            {
                //if (tab.IsContextual)
                {
                    double widthBeforeMeasure = tab.DesiredSize.Width;
                    tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));

                    // Contextual tabs may overreduce, so check that
                    overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                    if (overflowWidth < 0)
                    {
                        desiredSize = GetChildrenDesiredSize();
                        if (desiredSize.Width > availableSize.Width) desiredSize.Width = availableSize.Width;

                        // Add separator lines between 
                        // tabs to assist readability
                        UpdateSeparators(true, false);
                        VerifyScrollData(availableSize.Width, desiredSize.Width);
                        return desiredSize;
                    }
                }
            }

            // Sort regular tabs by descending
            RibbonTabItem[] sortedRegularTabItems = regularTabs
                .OrderByDescending(x => x.DesiredSize.Width)
                .ToArray();

            // Find how many regular tabs we have to reduce
            double reducedLength = 0;
            int reduceCount = 0;
            for (int i = 0; i < sortedRegularTabItems.Length - 1; i++)
            {
                double temp =
                    sortedRegularTabItems[i].DesiredSize.Width -
                    sortedRegularTabItems[i + 1].DesiredSize.Width;
                reducedLength += temp * (i + 1);
                reduceCount = i + 1;
                if (reducedLength > overflowWidth) break;
            }

            if (reducedLength > overflowWidth)
            {
                // Reduce regular tabs
                double requiredWidth = sortedRegularTabItems[reduceCount].DesiredSize.Width;
                if (reducedLength > overflowWidth) requiredWidth += (reducedLength - overflowWidth) / (double)reduceCount;
                for (int i = 0; i < reduceCount; i++)
                {
                    sortedRegularTabItems[i].Measure(new Size(requiredWidth, availableSize.Height));
                }

                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > availableSize.Width) desiredSize.Width = availableSize.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, true);
                VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }


            // Step 5. Reduce the width of all regular tabs equally 
            // down to a minimum of about three characters.
            double regularTabsWidth = sortedRegularTabItems.Sum(x => x.DesiredSize.Width);
            double minimumRegularTabsWidth = MinimumRegularTabWidth * sortedRegularTabItems.Length;

            if (overflowWidth < regularTabsWidth - minimumRegularTabsWidth)
            {
                double settedWidth = (regularTabsWidth - overflowWidth) / (double)regularTabsCount;
                for (int i = 0; i < regularTabsCount; i++)
                {
                    sortedRegularTabItems[i].Measure(new Size(settedWidth, availableSize.Height));
                }
                desiredSize = GetChildrenDesiredSize();
                //if (desiredSize.Width > availableSize.Width) desiredSize.Width = availableSize.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, true);
                VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 6. Reduce the width of the tab with the longest name by 
            // truncating the text label. Continue reducing the width of the largest 
            // tab (or tabs in the case of ties) until all tabs are the same width. 
            // (Contextual tabs)
            for (int i = 0; i < regularTabsCount; i++)
            {
                sortedRegularTabItems[i].Measure(new Size(MinimumRegularTabWidth, availableSize.Height));
            }
            overflowWidth -= regularTabsWidth - minimumRegularTabsWidth;

            // Sort contextual tabs by descending
            RibbonTabItem[] sortedContextualTabItems = contextualTabs
                .OrderByDescending(x => x.DesiredSize.Width)
                .ToArray();

            // Find how many contextual tabs we have to reduce
            reducedLength = 0;
            reduceCount = 0;
            for (int i = 0; i < sortedContextualTabItems.Length - 1; i++)
            {
                double temp =
                    sortedContextualTabItems[i].DesiredSize.Width -
                    sortedContextualTabItems[i + 1].DesiredSize.Width;
                reducedLength += temp * (i + 1);
                reduceCount = i + 1;
                if (reducedLength > overflowWidth) break;
            }

            if (reducedLength > overflowWidth)
            {
                // Reduce regular tabs
                double requiredWidth = sortedContextualTabItems[reduceCount].DesiredSize.Width;
                if (reducedLength > overflowWidth) requiredWidth += (reducedLength - overflowWidth) / (double)reduceCount;
                for (int i = 0; i < reduceCount; i++)
                {
                    sortedContextualTabItems[i].Measure(new Size(requiredWidth, availableSize.Height));
                }

                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > availableSize.Width) desiredSize.Width = availableSize.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, true);
                VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }
            else
            {
                double contextualTabsWidth = sortedContextualTabItems.Sum(x => x.DesiredSize.Width);

                double settedWidth = Math.Max(MinimumRegularTabWidth, (contextualTabsWidth - overflowWidth) / (double)contextualTabsCount);
                for (int i = 0; i < sortedContextualTabItems.Length; i++)
                {
                    sortedContextualTabItems[i].Measure(new Size(settedWidth, availableSize.Height));
                }
                desiredSize = GetChildrenDesiredSize();

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, true);
                VerifyScrollData(availableSize.Width, desiredSize.Width);
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

            var ribbonTabItemsWithGroups = this.InternalChildren.OfType<RibbonTabItem>()
                                               .Where(item => item.Group != null);

            var ribbonTitleBar = ribbonTabItemsWithGroups.Select(ribbonTabItemsWithGroup => ribbonTabItemsWithGroup.Group.Parent)
                                                         .OfType<RibbonTitleBar>()
                                                         .FirstOrDefault();

            if (ribbonTitleBar != null)
            {
                ribbonTitleBar.InvalidateMeasure();
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
            foreach (RibbonTabItem tab in Children)
            {
                if (tab.IsContextual)
                {
                    if (tab.IsSeparatorVisible != contextualTabs) tab.IsSeparatorVisible = contextualTabs;
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
            get { return ScrollData.ScrollOwner; }
            set { ScrollData.ScrollOwner = value; }
        }

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        public void SetHorizontalOffset(double offset)
        {
            double newValue = CoerceOffset(ValidateInputOffset(offset, "HorizontalOffset"), scrollData.ExtentWidth, scrollData.ViewportWidth);
            if (DoubleUtil.AreClose(ScrollData.OffsetX, newValue) == false)
            {
                scrollData.OffsetX = newValue;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        public double ExtentWidth
        {
            get { return ScrollData.ExtentWidth; }
        }

        /// <summary>
        /// Gets the horizontal offset of the scrolled content.
        /// </summary>
        public double HorizontalOffset
        {
            get { return ScrollData.OffsetX; }
        }

        /// <summary>
        /// Gets the horizontal size of the viewport for this content.
        /// </summary>
        public double ViewportWidth
        {
            get { return ScrollData.ViewportWidth; }
        }

        /// <summary>
        /// Scrolls left within content by one logical unit.
        /// </summary>
        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - 16.0);
        }

        /// <summary>
        /// Scrolls right within content by one logical unit.
        /// </summary>
        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + 16.0);
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
                || visual == (Visual)this
                || !this.IsAncestorOf(visual))
            {
                return Rect.Empty;
            }

            // Compute the child's rect relative to (0,0) in our coordinate space.
            GeneralTransform childTransform = visual.TransformToAncestor(this);

            rectangle = childTransform.TransformBounds(rectangle);

            // Initialize the viewport
            Rect viewport = new Rect(HorizontalOffset, rectangle.Top, ViewportWidth, rectangle.Height);
            rectangle.X += viewport.X;

            // Compute the offsets required to minimally scroll the child maximally into view.
            double minX = ComputeScrollOffsetWithMinimalScroll(viewport.Left, viewport.Right, rectangle.Left, rectangle.Right);

            // We have computed the scrolling offsets; scroll to them.
            SetHorizontalOffset(minX);

            // Compute the visible rectangle of the child relative to the viewport.
            viewport.X = minX;
            rectangle.Intersect(viewport);

            rectangle.X -= viewport.X;

            // Return the rectangle
            return rectangle;
        }

        static double ComputeScrollOffsetWithMinimalScroll(
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
            bool fAbove = (topChild < topView) && (bottomChild < bottomView);
            bool fBelow = (bottomChild > bottomView) && (topChild > topView);
            bool fLarger = (bottomChild - topChild) > (bottomView - topView);

            // Handle Cases:  1 & 4 above
            if ((fAbove && !fLarger)
               || (fBelow && fLarger))
            {
                return topChild;
            }

            // Handle Cases: 2 & 3 above
            else if (fAbove || fBelow)
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
        }/// <summary>
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
                return scrollData ?? (scrollData = new ScrollData());
            }
        }

        // Scroll data info
        private ScrollData scrollData;
        private const double MinimumRegularTabWidth = 30D;

        // Validates input offset
        static double ValidateInputOffset(double offset, string parameterName)
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
            bool isValid = true;

            if (Double.IsInfinity(viewportWidth))
            {
                viewportWidth = extentWidth;
            }

            double offsetX = CoerceOffset(ScrollData.OffsetX, extentWidth, viewportWidth);

            isValid &= DoubleUtil.AreClose(viewportWidth, ScrollData.ViewportWidth);
            isValid &= DoubleUtil.AreClose(extentWidth, ScrollData.ExtentWidth);
            isValid &= DoubleUtil.AreClose(ScrollData.OffsetX, offsetX);

            ScrollData.ViewportWidth = viewportWidth;

            // newExtentWidth is neccessary to fix 20762 (Tab scroll button appears randomly when resizing)
            // To fix 20762 we are manipulating the extentWidth by checking if all regular (non contextual) tabs are at their minimum width.
            // When they are all at their minimum width we have to force the extentWidth to be greater than the viewportWidth.
            // When there are no regular tabs, we MUST NOT apply this fix
            var newExtentWidth = Math.Max(viewportWidth, extentWidth);

            var visibleRegularTabs = this.InternalChildren.Cast<RibbonTabItem>()
                .Where(item => item.IsContextual == false && item.Visibility != Visibility.Collapsed)
                .ToArray();

            if (visibleRegularTabs.Any()
                && visibleRegularTabs.All(item => DoubleUtil.AreClose(item.DesiredSize.Width, MinimumRegularTabWidth)))
            {
                if (DoubleUtil.AreClose(newExtentWidth, viewportWidth))
                {
                    newExtentWidth = newExtentWidth + 1;
                }

                ScrollData.ExtentWidth = newExtentWidth;
            }
            else
            {
                ScrollData.ExtentWidth = ScrollData.ViewportWidth;
            }

            ScrollData.OffsetX = offsetX;

            if (!isValid)
            {
                if (ScrollOwner != null)
                {
                    ScrollOwner.InvalidateScrollInfo();
                }
            }
        }

        // Returns an offset coerced into the [0, Extent - Viewport] range.
        static double CoerceOffset(double offset, double extent, double viewport)
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