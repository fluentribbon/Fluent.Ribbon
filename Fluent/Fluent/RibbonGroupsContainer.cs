using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Fluent
{
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
        /// the last to reduce (use Control.Name as group name in the enum)
        /// </summary>
        public string ReduceOrder
        {
            get { return (string)GetValue(ReduceOrderProperty); }
            set { SetValue(ReduceOrderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReduceOrder.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReduceOrderProperty =
            DependencyProperty.Register("ReduceOrder", typeof(string), typeof(RibbonGroupsContainer), new UIPropertyMetadata(ReduceOrderPropertyChanged));


        static void ReduceOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupsContainer ribbonPanel = (RibbonGroupsContainer)d;
            ribbonPanel.cachedConstraint = ribbonPanel.cachedDesiredSize = new Size();
            ribbonPanel.reduceOrder = ((string)e.NewValue).Split(new char[] {',',' '}, StringSplitOptions.RemoveEmptyEntries);
            ribbonPanel.reduceOrderIndex = ribbonPanel.reduceOrder.Length - 1;

            ribbonPanel.InvalidateMeasure();
            ribbonPanel.InvalidateArrange();
        }

        #endregion

        #region Fields

        // A cached copy of the constraint from the previous layout pass.
        Size cachedConstraint;
        // A cached copy of the desired size from the previous layout pass.
        Size cachedDesiredSize;
        string[] reduceOrder = new string[0];
        int reduceOrderIndex = 0;

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonGroupsContainer(): base()
        {
            Focusable = false;
            FocusManager.SetIsFocusScope(this, false);
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
            return new UIElementCollection(this, Parent as FrameworkElement);
        }

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
            Size infinitySize = new Size(Double.PositiveInfinity, constraint.Height);
            Size desiredSize = GetChildrenDesiredSize(infinitySize);

            // If the constraint and desired size are equal to those in the cache, skip
            // this layout measure pass.
            if ((constraint != cachedConstraint) || (desiredSize != cachedDesiredSize))
            {
                cachedConstraint = constraint;
                cachedDesiredSize = desiredSize;

                // If we have more available space - try to expand groups
                while (desiredSize.Width <= constraint.Width)
                {
                    bool hasMoreVariants = reduceOrderIndex < reduceOrder.Length - 1;
                    if (!hasMoreVariants) break;

                    // Increase size of another item
                    reduceOrderIndex++;
                    IncreaseGroupBoxSize(reduceOrder[reduceOrderIndex]);
                    

                    desiredSize = GetChildrenDesiredSize(infinitySize);                    
                }

                // If not enough space - go to next variant
                while (desiredSize.Width > constraint.Width)
                {
                    bool hasMoreVariants = reduceOrderIndex >= 0;
                    if (!hasMoreVariants) break;

                    // Decrease size of another item
                    DecreaseGroupBoxSize(reduceOrder[reduceOrderIndex]);
                    reduceOrderIndex--;

                    desiredSize = GetChildrenDesiredSize(infinitySize);                     
                }                
            }
            VerifyScrollData(constraint.Width, desiredSize.Width);
            return desiredSize;
        }

        /// <summary>
        /// Calculates the total width of all children
        /// </summary>
        /// <returns>Returns the total width</returns>
        double GetChildrenWidth()
        {
            double result = 0;
            foreach (UIElement child in this.InternalChildren)
            {
                result += child.DesiredSize.Width;
            }
            return result;
        }

        Size GetChildrenDesiredSize(Size availableSize)
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

        

        // Increase size of the item
        void IncreaseGroupBoxSize(string name)
        {
            RibbonGroupBox groupBox = FindGroup(name);
            if (groupBox == null) return;
            if(groupBox.State != RibbonGroupBoxState.Large) groupBox.State = groupBox.State - 1;
        }

        // Decrease size of the item
        void DecreaseGroupBoxSize(string name)
        {
            RibbonGroupBox groupBox = FindGroup(name);
            if (groupBox == null) return;
            if(groupBox.State != RibbonGroupBoxState.Collapsed) groupBox.State = groupBox.State + 1;
        }

        private RibbonGroupBox FindGroup(string name)
        {
            foreach (FrameworkElement child in InternalChildren)
            {
                if (child.Name == name) return child as RibbonGroupBox;
            }
            return null;
        }

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
            return finalSize;
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
            double newValue = CoerceOffset(ValidateInputOffset(offset, "HorizontalOffset"), scrollData.ExtentWidth, scrollData.ViewportWidth);
            //if (!DoubleUtil.AreClose(ScrollData.OffsetX, newValue))
            if (ScrollData.OffsetX != newValue)
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

        internal static double ComputeScrollOffsetWithMinimalScroll(
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

            /*isValid &= DoubleUtil.AreClose(viewportWidth, ScrollData.ViewportWidth);
            isValid &= DoubleUtil.AreClose(extentWidth, ScrollData.ExtentWidth);
            isValid &= DoubleUtil.AreClose(ScrollData.OffsetX, offsetX);*/

            isValid &= (viewportWidth == ScrollData.ViewportWidth);
            isValid &= (extentWidth == ScrollData.ExtentWidth);
            isValid &= (ScrollData.OffsetX == offsetX);

            ScrollData.ViewportWidth = viewportWidth;
            ScrollData.ExtentWidth = extentWidth;
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
        // Internal because it is also used by other Avalon ISI implementations (just to avoid code duplication).
        internal static double CoerceOffset(double offset, double extent, double viewport)
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
