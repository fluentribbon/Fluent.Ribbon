#region Copyright and License Information

// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license

#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

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
            get { return (string)GetValue(ReduceOrderProperty); }
            set { SetValue(ReduceOrderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReduceOrder.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReduceOrderProperty =
            DependencyProperty.Register("ReduceOrder", typeof(string), typeof(RibbonGroupsContainer), new UIPropertyMetadata(ReduceOrderPropertyChanged));

        // handles ReduseOrder property changed
        static void ReduceOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupsContainer ribbonPanel = (RibbonGroupsContainer)d;
            ribbonPanel.reduceOrder = ((string)e.NewValue).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ribbonPanel.reduceOrderIndex = ribbonPanel.reduceOrder.Length - 1;

            ribbonPanel.InvalidateMeasure();
            ribbonPanel.InvalidateArrange();
        }

        #endregion

        #region Fields

        string[] reduceOrder = new string[0];
        int reduceOrderIndex;

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonGroupsContainer()
            : base()
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
            Size infinitySize = new Size(Double.PositiveInfinity, availableSize.Height);
            Size desiredSize = GetChildrenDesiredSizeIntermediate();
            if (reduceOrder.Length == 0)
            {
                VerifyScrollData(availableSize.Width, desiredSize.Width);
                return desiredSize;
            }

            // If we have more available space - try to expand groups
            while (desiredSize.Width <= availableSize.Width)
            {
                bool hasMoreVariants = reduceOrderIndex < reduceOrder.Length - 1;
                if (!hasMoreVariants) break;

                // Increase size of another item
                reduceOrderIndex++;
                IncreaseGroupBoxSize(reduceOrder[reduceOrderIndex]);

                desiredSize = GetChildrenDesiredSizeIntermediate();
            }

            // If not enough space - go to next variant
            while (desiredSize.Width > availableSize.Width)
            {
                bool hasMoreVariants = reduceOrderIndex >= 0;
                if (!hasMoreVariants) break;

                // Decrease size of another item
                DecreaseGroupBoxSize(reduceOrder[reduceOrderIndex]);
                reduceOrderIndex--;

                desiredSize = GetChildrenDesiredSizeIntermediate();
            }

            // Set find values
            foreach (object item in InternalChildren)
            {
                RibbonGroupBox groupBox = item as RibbonGroupBox;
                if (groupBox == null) continue;

                if ((groupBox.State != groupBox.StateIntermediate) ||
                    (groupBox.Scale != groupBox.ScaleIntermediate))
                {
                    groupBox.SuppressCacheReseting = true;
                    groupBox.State = groupBox.StateIntermediate;
                    groupBox.Scale = groupBox.ScaleIntermediate;
                    groupBox.InvalidateLayout();
                    groupBox.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    groupBox.SuppressCacheReseting = false;
                }

                // Something wrong with cache?
                if (groupBox.DesiredSizeIntermediate != groupBox.DesiredSize)
                {
                    // Reset cache and reinvoke masure
                    groupBox.ClearCache();
                    return MeasureOverride(availableSize);
                }
            }

            VerifyScrollData(availableSize.Width, desiredSize.Width);
            return desiredSize;
        }

        Size GetChildrenDesiredSizeIntermediate()
        {
            double width = 0;
            double height = 0;
            foreach (UIElement child in this.InternalChildren)
            {
                RibbonGroupBox groupBox = child as RibbonGroupBox;
                if (groupBox == null) continue;

                Size desiredSize = groupBox.DesiredSizeIntermediate;
                width += desiredSize.Width;
                height = Math.Max(height, desiredSize.Height);
            }
            return new Size(width, height);
        }



        // Increase size of the item
        void IncreaseGroupBoxSize(string name)
        {
            RibbonGroupBox groupBox = FindGroup(name);
            bool scale = name.StartsWith("(", StringComparison.OrdinalIgnoreCase);
            if (groupBox == null) return;

            if (scale) groupBox.ScaleIntermediate++;
            else groupBox.StateIntermediate = (groupBox.StateIntermediate != RibbonGroupBoxState.Large) ? groupBox.StateIntermediate - 1 : RibbonGroupBoxState.Large;
        }


        // Decrease size of the item
        void DecreaseGroupBoxSize(string name)
        {
            RibbonGroupBox groupBox = FindGroup(name);
            bool scale = name.StartsWith("(", StringComparison.OrdinalIgnoreCase);
            if (groupBox == null) return;

            if (scale) groupBox.ScaleIntermediate--;
            else groupBox.StateIntermediate = (groupBox.StateIntermediate != RibbonGroupBoxState.Collapsed) ? groupBox.StateIntermediate + 1 : groupBox.StateIntermediate;
        }



        RibbonGroupBox FindGroup(string name)
        {
            if (name.StartsWith("(", StringComparison.OrdinalIgnoreCase)) name = name.Substring(1, name.Length - 2);
            foreach (FrameworkElement child in InternalChildren)
            {
                if (child.Name == name) return child as RibbonGroupBox;
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
            if (ScrollData.OffsetX != newValue)
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
                || ReferenceEquals(visual, this)
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
}
