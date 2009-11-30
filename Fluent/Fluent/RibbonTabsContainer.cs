using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Fluent
{
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
        public RibbonTabsContainer() : base()
        {

        }

        #endregion

        #region Layout Overridings

        /// <summary>
        /// Measures all of the RibbonGroupBox, and resize them appropriately
        /// to fit within the available room
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (InternalChildren.Count == 0) return base.MeasureOverride(constraint);

            Size desiredSize = MeasureChildrenDesiredSize(constraint);

            
            // Performs steps as described in "2007 MICROSOFT® OFFICE FLUENT™ 
            // USER INTERFACE DESIGN GUIDELINES"

            // Step 1. Gradually remove empty space to the right of the tabs
            // If all tabs already in full size, just return
            if (constraint.Width > desiredSize.Width)
            {
                // Hide separator lines between tabs
                UpdateSeparators(false, false);
                VerifyScrollData(constraint.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 2. Gradually and uniformly remove the padding from both sides 
            // of all the tabs until the minimum padding required for displaying 
            // the tab selection and hover states is reached (regular tabs)
            double overflowWidth = desiredSize.Width - constraint.Width;
            double whitespace = (Children[0] as RibbonTabItem).Whitespace;
            RibbonTabItem[] contextualTabs = Children.Cast<RibbonTabItem>().Where(x => x.IsContextual).ToArray();
            IEnumerable<RibbonTabItem> reversedChildren = Children.Cast<RibbonTabItem>().Reverse();
            double contextualTabsCount = contextualTabs.Length;
            double regularTabsCount = Children.Count - contextualTabsCount;
            if (overflowWidth < regularTabsCount * whitespace * 2)
            {
                double decreaseValue = overflowWidth / (double)regularTabsCount;
                foreach (RibbonTabItem tab in Children) if (!tab.IsContextual) tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - decreaseValue), tab.DesiredSize.Height));// tab.Width = Math.Max(0, tab.ActualWidth - decreaseValue);
                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > constraint.Width) desiredSize.Width = constraint.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, false);
                VerifyScrollData(constraint.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 3. Gradually and uniformly remove the padding from both sides 
            // of all the tabs until the minimum padding required for displaying 
            // the tab selection and hover states is reached (contextual tabs)
            if (overflowWidth < Children.Count * whitespace * 2)
            {
                double regularTabsWhitespace = (double)regularTabsCount * whitespace * 2.0;
                double decreaseValue = (overflowWidth - regularTabsWhitespace) / (double)contextualTabsCount;
                foreach (RibbonTabItem tab in Children)
                {                    
                    if (!tab.IsContextual)
                    {
                        double widthBeforeMeasure = tab.DesiredSize.Width;
                        tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace*2.0), tab.DesiredSize.Height));
                        overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                    }
                }
                foreach (RibbonTabItem tab in reversedChildren)
                {                    
                    if (tab.IsContextual)
                    {
                        double widthBeforeMeasure = tab.DesiredSize.Width;
                        tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - decreaseValue), tab.DesiredSize.Height));

                        // Contextual tabs may overreduce, so check that
                        overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                        if (overflowWidth < 0) break;
                    }
                }
                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > constraint.Width) desiredSize.Width = constraint.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, false);
                VerifyScrollData(constraint.Width, desiredSize.Width);
                return desiredSize;
            }


            // Step 4. Reduce the width of the tab with the longest name by 
            // truncating the text label. Continue reducing the width of the largest 
            // tab (or tabs in the case of ties) until all tabs are the same width. 
            // (Regular tabs)
            foreach (RibbonTabItem tab in Children)
            {
                if (!tab.IsContextual)
                {
                    double widthBeforeMeasure = tab.DesiredSize.Width;
                    tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));
                    overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                }
            }
            foreach (RibbonTabItem tab in reversedChildren)
            {
                if (tab.IsContextual)
                {
                    double widthBeforeMeasure = tab.DesiredSize.Width;
                    tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));

                    // Contextual tabs may overreduce, so check that
                    overflowWidth -= widthBeforeMeasure - tab.DesiredSize.Width;
                    if (overflowWidth < 0)
                    {
                        desiredSize = GetChildrenDesiredSize();
                        if (desiredSize.Width > constraint.Width) desiredSize.Width = constraint.Width;

                        // Add separator lines between 
                        // tabs to assist readability
                        UpdateSeparators(true, false);
                        VerifyScrollData(constraint.Width, desiredSize.Width);
                        return desiredSize;
                    }
                }
            }
         
            // Sort regular tabs by descending
            RibbonTabItem[] sortedRegularTabItems = Children.Cast<RibbonTabItem>()
                .Where(x => !x.IsContextual)
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
                    sortedRegularTabItems[i].Measure(new Size(requiredWidth, constraint.Height));
                }

                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > constraint.Width) desiredSize.Width = constraint.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, true);
                VerifyScrollData(constraint.Width, desiredSize.Width);
                return desiredSize;
            }


            // Step 5. Reduce the width of all regular tabs equally 
            // down to a minimum of about three characters.
            double regularTabsWidth = sortedRegularTabItems.Sum(x => x.DesiredSize.Width);
            double minimumRegularTabsWidth = 30.0 * sortedRegularTabItems.Length;
            if (overflowWidth < regularTabsWidth - minimumRegularTabsWidth)
            {
                double settedWidth = (regularTabsWidth - overflowWidth) / (double)regularTabsCount;
                for (int i = 0; i < reduceCount; i++)
                {
                    sortedRegularTabItems[i].Measure(new Size(settedWidth, constraint.Height));
                }
                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > constraint.Width) desiredSize.Width = constraint.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, true);
                VerifyScrollData(constraint.Width, desiredSize.Width);
                return desiredSize;
            }

            // Step 6. Reduce the width of the tab with the longest name by 
            // truncating the text label. Continue reducing the width of the largest 
            // tab (or tabs in the case of ties) until all tabs are the same width. 
            // (Contextual tabs)
            for (int i = 0; i < reduceCount; i++)
            {
                sortedRegularTabItems[i].Measure(new Size(30, constraint.Height));
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
                    sortedContextualTabItems[i].Measure(new Size(requiredWidth, constraint.Height));
                }

                desiredSize = GetChildrenDesiredSize();
                if (desiredSize.Width > constraint.Width) desiredSize.Width = constraint.Width;

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, true);
                VerifyScrollData(constraint.Width, desiredSize.Width);
                return desiredSize;
            }
            else
            {
                double contextualTabsWidth = sortedContextualTabItems.Sum(x => x.DesiredSize.Width);
                double minimumContextualTabsWidth = 30.0 * sortedContextualTabItems.Length;

                double settedWidth = Math.Max(30, (contextualTabsWidth - overflowWidth) / (double)contextualTabsCount);
                for (int i = 0; i < sortedContextualTabItems.Length; i++)
                {
                    sortedContextualTabItems[i].Measure(new Size(settedWidth, constraint.Height));
                }
                desiredSize = GetChildrenDesiredSize();

                // Add separator lines between 
                // tabs to assist readability
                UpdateSeparators(true, true);
                VerifyScrollData(constraint.Width, desiredSize.Width);
                return desiredSize;
            }            
        }

        Size MeasureChildrenDesiredSize(Size availableSize)
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

        Size GetChildrenDesiredSize()
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
            Rect finalRect = new Rect(finalSize);
            finalRect.X = -HorizontalOffset;
            foreach (UIElement item in InternalChildren)
            {
                finalRect.Width = item.DesiredSize.Width;
                finalRect.Height = Math.Max(finalSize.Height, item.DesiredSize.Height);
                item.Arrange(finalRect);
                finalRect.X += item.DesiredSize.Width;
            }

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                if (InternalChildren[i] is RibbonTabItem)
                {
                    if ((InternalChildren[i] as RibbonTabItem).Group != null)
                    {
                        ((InternalChildren[i] as RibbonTabItem).Group.Parent as RibbonTitleBar).InvalidateMeasure();
                        break;
                    }
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Updates separator visibility
        /// </summary>
        /// <param name="regularTabs">If this parameter true, regular tabs will have separators</param>
        /// <param name="contextualTabs">If this parameter true, contextual tabs will have separators</param>
        void UpdateSeparators(bool regularTabs, bool contextualTabs)
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

        public ScrollViewer ScrollOwner
        {
            get { return ScrollData.ScrollOwner; }
            set { ScrollData.ScrollOwner = value; }
        }

        public void SetHorizontalOffset(double offset)
        {
            double newValue = CoerceOffset(ValidateInputOffset(offset, "HorizontalOffset"),scrollData.ExtentWidth, scrollData.ViewportWidth);
            //if (!DoubleUtil.AreClose(ScrollData.OffsetX, newValue))
            if (ScrollData.OffsetX!= newValue)
            {
                scrollData.OffsetX = newValue;
                InvalidateArrange();
            }
        }

        public double ExtentWidth
        {
            get { return ScrollData.ExtentWidth; }
        }

        public double HorizontalOffset
        {
            get { return ScrollData.OffsetX; }
        }

        public double ViewportWidth
        {
            get { return ScrollData.ViewportWidth; }
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - 16.0);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + 16.0);
        }

        // This is optimized for horizontal scrolling only
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
            bool fAbove = (topChild< topView) && (bottomChild< bottomView);
            bool fBelow = (bottomChild> bottomView) && (topChild> topView);
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

        // Does not support other scrolling than LineLeft/LineRight
        public void MouseWheelDown()
        {
        }

        public void MouseWheelLeft()
        {
        }

        public void MouseWheelRight()
        {
        }

        public void MouseWheelUp()
        {
        }

        public void LineDown()
        {
        }

        public void LineUp()
        {
        }

        public void PageDown()
        {
        }

        public void PageLeft()
        {
        }

        public void PageRight()
        {
        }

        public void PageUp()
        {
        }

        public void SetVerticalOffset(double offset)
        {
        }

        public bool CanVerticallyScroll
        {
            get { return false; }
            set { }
        }

        public bool CanHorizontallyScroll
        {
            get { return true; }
            set { }
        }

        public double ExtentHeight
        {
            get { return 0.0; }
        }

        public double VerticalOffset
        {
            get { return 0.0; }
        }

        public double ViewportHeight
        {
            get { return 0.0; }
        }

        private ScrollData ScrollData
        {
            get
            {
                return scrollData ?? (scrollData = new ScrollData());
            }
        }

        private ScrollData scrollData;

        internal static double ValidateInputOffset(double offset, string parameterName)
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

            isValid &= AreClose(viewportWidth, ScrollData.ViewportWidth);
            isValid &= AreClose(extentWidth, ScrollData.ExtentWidth);
            isValid &= AreClose(ScrollData.OffsetX, offsetX);

            /*isValid &= (viewportWidth == ScrollData.ViewportWidth);
            isValid &= (extentWidth == ScrollData.ExtentWidth);
            isValid &= (ScrollData.OffsetX == offsetX);*/

            
            ScrollData.ViewportWidth = viewportWidth;
            if (AreClose(viewportWidth, extentWidth)) ScrollData.ExtentWidth = viewportWidth;
            else ScrollData.ExtentWidth = extentWidth;
            ScrollData.OffsetX = offsetX;

            if (!isValid)
            {
                

                if (ScrollOwner != null)
                {
                    ScrollOwner.InvalidateScrollInfo();
                }
            }
        }

        bool AreClose(double a, double b)
        {
            return Math.Abs(a - b) < 1.5;
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
