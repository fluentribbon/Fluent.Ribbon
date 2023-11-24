// ReSharper disable once CheckNamespace
namespace Fluent;

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
    private double minimumLeftRightHeaderPadding = 5;

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
        if (this.InternalChildren.Count is 0)
        {
            return base.MeasureOverride(availableSize);
        }

        var desiredSize = this.MeasureChildrenDesiredSize(availableSize);

        var overflowWidth = desiredSize.Width - availableSize.Width;

        var reduce = overflowWidth > 0;
        var stepSize = reduce
            ? -1.0
            : 1.0;

        var absoluteOverflowWidth = Math.Abs(overflowWidth);

        var visibleTabs = this.InternalChildren.Cast<RibbonTabItem>()
            .Where(x => x.Visibility is not Visibility.Collapsed && x.IsContextual is false)
            .OrderByDescending(x => x.HeaderPadding.Right)
            .ToList();

        var sizeChanges = 0;

        if (reduce
            || DoubleUtil.GreaterThan(absoluteOverflowWidth, 2.0))
        {
            // loop while we got overflow left (still need to reduce more) and all tabs are larger than the minimum size
            while (DoubleUtil.GreaterThan(absoluteOverflowWidth, 0))
            {
                var anyTabChanged = false;

                foreach (var tab in visibleTabs)
                {
                    // ignore tabs that are smaller or equal to the minimum size
                    if (reduce)
                    {
                        if (DoubleUtil.GreaterThan(tab.HeaderPadding.Right, this.minimumLeftRightHeaderPadding) is false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (DoubleUtil.AreClose(tab.HeaderPadding.Right, 9.0)
                            || DoubleUtil.GreaterThan(tab.HeaderPadding.Right, 9.0))
                        {
                            continue;
                        }
                    }

                    var newHeaderPadding = tab.HeaderPadding;

                    if ((reduce && DoubleUtil.GreaterThan(newHeaderPadding.Right, newHeaderPadding.Left))
                        || DoubleUtil.GreaterThan(newHeaderPadding.Left, newHeaderPadding.Right))
                    {
                        newHeaderPadding.Right += stepSize;
                    }
                    else
                    {
                        newHeaderPadding.Left += stepSize;
                    }

                    tab.SetCurrentValue(RibbonTabItem.HeaderPaddingProperty, newHeaderPadding);
                    ++sizeChanges;
                    anyTabChanged = true;

                    absoluteOverflowWidth -= Math.Abs(stepSize);

                    if (DoubleUtil.GreaterThan(absoluteOverflowWidth, 0) is false)
                    {
                        break;
                    }
                }

                // break if no tabs changed their size
                if (anyTabChanged is false)
                {
                    break;
                }
            }
        }

        if (sizeChanges is not 0)
        {
            desiredSize.Width += sizeChanges * stepSize;
            desiredSize = this.MeasureChildrenDesiredSize(desiredSize);
        }

        // Gradually make separators visible between tabs to assist readability
        var separatorOpacity = 0D;
        if (visibleTabs.Any())
        {
            var averageHeaderPadding = visibleTabs.Average(x => x.HeaderPadding.Left + x.HeaderPadding.Right);
            var paddingDiff = averageHeaderPadding - (this.minimumLeftRightHeaderPadding * 2);
            if (DoubleUtil.GreaterThan(paddingDiff, 7) is false)
            {
                separatorOpacity = 1D - (paddingDiff / 8);
            }
        }

        this.UpdateSeparators(separatorOpacity);
        this.VerifyScrollData(availableSize.Width, desiredSize.Width);
        return desiredSize;
    }

    private bool AreAnyTabsAboveMinimumSize(List<RibbonTabItem> tabs)
    {
        return tabs.Any(x => x.Visibility is not Visibility.Collapsed && x.IsContextual is false && DoubleUtil.GreaterThan(x.HeaderPadding.Right, this.minimumLeftRightHeaderPadding));
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

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var finalRect = new Rect(finalSize)
        {
            X = -this.HorizontalOffset
        };

        var orderedChildren = this.InternalChildren.OfType<RibbonTabItem>()
            .OrderBy(x => x.IsContextual);

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
    /// <param name="opacity">If this parameter true, tabs will have separators</param>
    private void UpdateSeparators(double opacity)
    {
        foreach (RibbonTabItem? tab in this.Children)
        {
            if (tab is null)
            {
                continue;
            }

            if (tab.IsContextual)
            {
                tab.SeparatorOpacity = 0D;
            }

            tab.SeparatorOpacity = opacity;
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
    private ScrollData ScrollData => this.scrollData ??= new ScrollData();

    // Scroll data info
    private ScrollData? scrollData;

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
        // if (viewportWidth + 2 < extentWidth)
        // {
        //     this.ScrollData.ExtentWidth = extentWidth;
        // }
        // else
        {
            // Or we show show the srollbar if all tabs are at their minimum width or smaller
            // but do this early (if extent + 2 is equal or larger than the viewport, or they are equal)
            if (extentWidth + 2 >= viewportWidth
                || DoubleUtil.AreClose(extentWidth, viewportWidth))
            {
                var visibleTabs = this.InternalChildren.Cast<RibbonTabItem>()
                    .Where(item => item.Visibility is not Visibility.Collapsed)
                    .ToList();

                var newExtentWidth = viewportWidth;

                if (this.AreAnyTabsAboveMinimumSize(visibleTabs) is false)
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

        if (isValid is false)
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