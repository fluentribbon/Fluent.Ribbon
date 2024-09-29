// ReSharper disable once CheckNamespace

namespace Fluent;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Fluent.Internal;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Special wrap panel for <see cref="RibbonGroupBox" />.
/// </summary>
public class RibbonGroupBoxWrapPanel : Panel
{
    /// <summary>
    /// Attached <see cref="DependencyProperty" /> for <c>SharedSizeGroupName</c>.
    /// </summary>
    public static readonly DependencyProperty SharedSizeGroupNameProperty =
        DependencyProperty.RegisterAttached(
            "SharedSizeGroupName",
            typeof(string),
            typeof(RibbonGroupBoxWrapPanel),
            new(default(string)));

    /// <summary>
    /// Attached <see cref="DependencyProperty" /> for <c>SharedSizeGroupName</c>.
    /// </summary>
    public static readonly DependencyProperty ExcludeFromSharedSizeProperty =
        DependencyProperty.RegisterAttached(
            "ExcludeFromSharedSize",
            typeof(bool),
            typeof(RibbonGroupBoxWrapPanel),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>Identifies the <see cref="ItemWidth"/> dependency property.</summary>
    public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register(nameof(ItemWidth),
            typeof(double),
            typeof(RibbonGroupBoxWrapPanel),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
            ValidateItemWidth);

    /// <summary>Identifies the <see cref="ItemHeight"/> dependency property.</summary>
    public static readonly DependencyProperty ItemHeightProperty =
        DependencyProperty.Register(nameof(ItemHeight),
            typeof(double),
            typeof(RibbonGroupBoxWrapPanel),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
            ValidateItemHeight);

    /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
    public static readonly DependencyProperty OrientationProperty =
        StackPanel.OrientationProperty.AddOwner(
            typeof(RibbonGroupBoxWrapPanel),
            new FrameworkPropertyMetadata(Orientation.Vertical,
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnOrientationChanged));

    /// <summary>Identifies the <see cref="IsSimplified"/> dependency property.</summary>
    public static readonly DependencyProperty IsSimplifiedProperty = DependencyProperty.Register(
        nameof(IsSimplified), typeof(bool), typeof(RibbonGroupBoxWrapPanel), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, OnIsSimplifiedChanged));

    /// <summary>
    /// The ItemWidth and ItemHeight properties specify the size of all items in the WrapPanel.
    /// Note that children of
    /// WrapPanel may have their own Width/Height properties set - the ItemWidth/ItemHeight
    /// specifies the size of "layout partition" reserved by WrapPanel for the child.
    /// If this property is not set (or set to "Auto" in markup or Double.NaN in code) - the size of layout
    /// partition is equal to DesiredSize of the child element.
    /// </summary>
    [TypeConverter(typeof(LengthConverter))]
    public double ItemWidth
    {
        get => (double)this.GetValue(ItemWidthProperty);
        set => this.SetValue(ItemWidthProperty, value);
    }

    /// <summary>
    /// The ItemWidth and ItemHeight properties specify the size of all items in the WrapPanel.
    /// Note that children of
    /// WrapPanel may have their own Width/Height properties set - the ItemWidth/ItemHeight
    /// specifies the size of "layout partition" reserved by WrapPanel for the child.
    /// If this property is not set (or set to "Auto" in markup or Double.NaN in code) - the size of layout
    /// partition is equal to DesiredSize of the child element.
    /// </summary>
    [TypeConverter(typeof(LengthConverter))]
    public double ItemHeight
    {
        get => (double)this.GetValue(ItemHeightProperty);
        set => this.SetValue(ItemHeightProperty, value);
    }

    /// <summary>
    /// Specifies dimension of children positioning in absence of wrapping.
    /// Wrapping occurs in orthogonal direction. For example, if Orientation is Horizontal,
    /// the items try to form horizontal rows first and if needed are wrapped and form vertical stack of rows.
    /// If Orientation is Vertical, items first positioned in a vertical column, and if there is
    /// not enough space - wrapping creates additional columns in horizontal dimension.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)this.GetValue(OrientationProperty);
        set => this.SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether or not the ribbon is in simplified mode.
    /// </summary>
    public bool IsSimplified
    {
        get => (bool)this.GetValue(IsSimplifiedProperty);
        set => this.SetValue(IsSimplifiedProperty, value);
    }

    /// <summary>
    /// Sets <see cref="SharedSizeGroupNameProperty" /> for <paramref name="element" />.
    /// </summary>
    public static void SetSharedSizeGroupName(DependencyObject element, string? value)
    {
        element.SetValue(SharedSizeGroupNameProperty, value);
    }

    /// <summary>
    /// Gets <see cref="SharedSizeGroupNameProperty" /> for <paramref name="element" />.
    /// </summary>
    public static string? GetSharedSizeGroupName(DependencyObject element)
    {
        return (string)element.GetValue(SharedSizeGroupNameProperty);
    }

    /// <summary>
    /// Sets <see cref="ExcludeFromSharedSizeProperty" /> for <paramref name="element" />.
    /// </summary>
    public static void SetExcludeFromSharedSize(DependencyObject element, bool value)
    {
        element.SetValue(ExcludeFromSharedSizeProperty, BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Gets <see cref="ExcludeFromSharedSizeProperty" /> for <paramref name="element" />.
    /// </summary>
    public static bool GetExcludeFromSharedSize(DependencyObject element)
    {
        return (bool)element.GetValue(ExcludeFromSharedSizeProperty);
    }

    private static bool ValidateItemWidth(object? value)
    {
        if (value is null)
        {
            return false;
        }

        var v = (double)value;
        return ValidateWidthOrHeight(v);
    }

    private static bool ValidateItemHeight(object? value)
    {
        if (value is null)
        {
            return false;
        }

        var v = (double)value;
        return ValidateWidthOrHeight(v);
    }

    private static bool ValidateWidthOrHeight(double v)
    {
        return double.IsNaN(v) || (v >= 0.0d && !double.IsPositiveInfinity(v));
    }

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (RibbonGroupBoxWrapPanel)d;

        if (control.IsLoaded is false)
        {
            return;
        }

        UIHelper.GetParent<RibbonGroupBox>(control)?.TryClearCacheAndResetStateAndScaleAndNotifyParentRibbonGroupsContainer();
    }

    private static void OnIsSimplifiedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (RibbonGroupBoxWrapPanel)d;

        if (control.IsLoaded is false)
        {
            return;
        }

        UIHelper.GetParent<RibbonGroupBox>(control)?.TryClearCacheAndResetStateAndScaleAndNotifyParentRibbonGroupsContainer();
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        if (this.IsSimplified is false)
        {
            return this.MeasureWrapMode(constraint);
        }

        return this.MeasureStackMode(constraint);
    }

    private Size MeasureWrapMode(Size constraint)
    {
        var curLineSize = new UvSize(this.Orientation);
        var panelSize = new UvSize(this.Orientation);
        var uvConstraint = new UvSize(this.Orientation, constraint.Width, constraint.Height);
        var itemWidth = this.ItemWidth;
        var itemHeight = this.ItemHeight;
        var itemWidthSet = !double.IsNaN(itemWidth);
        var itemHeightSet = !double.IsNaN(itemHeight);

        var childConstraint = new Size(
            itemWidthSet
                ? itemWidth
                : constraint.Width,
            itemHeightSet
                ? itemHeight
                : constraint.Height);

        var children = this.InternalChildren;

        for (int i = 0, count = children.Count; i < count; i++)
        {
            var child = children[i];
            if (child is null)
            {
                continue;
            }

            //Flow passes its own constrint to children
            child.Measure(childConstraint);

            //this is the size of the child in UV space
            var sz = new UvSize(
                this.Orientation,
                itemWidthSet
                    ? itemWidth
                    : child.DesiredSize.Width,
                itemHeightSet
                    ? itemHeight
                    : child.DesiredSize.Height);

            if (DoubleUtil.GreaterThan(curLineSize.U + sz.U, uvConstraint.U)) //need to switch to another line
            {
                panelSize.U = Math.Max(curLineSize.U, panelSize.U);
                panelSize.V += curLineSize.V;
                curLineSize = sz;

                if (DoubleUtil.GreaterThan(sz.U, uvConstraint.U)) //the element is wider then the constraint - give it a separate line
                {
                    panelSize.U = Math.Max(sz.U, panelSize.U);
                    panelSize.V += sz.V;
                    curLineSize = new(this.Orientation);
                }
            }
            else //continue to accumulate a line
            {
                curLineSize.U += sz.U;
                curLineSize.V = Math.Max(sz.V, curLineSize.V);
            }
        }

        //the last line size, if any should be added
        panelSize.U = Math.Max(curLineSize.U, panelSize.U);
        panelSize.V += curLineSize.V;

        //go from UV space to W/H space
        return new(panelSize.Width, panelSize.Height);
    }

    private Size MeasureStackMode(Size constraint)
    {
        var stackDesiredSize = default(Size);
        var children = this.InternalChildren;
        var layoutSlotSize = constraint;
        var fHorizontal = this.Orientation is Orientation.Horizontal;

        // Initialize child sizing and iterator data
        // Allow children as much size as they want along the stack.
        if (fHorizontal)
        {
            layoutSlotSize.Width = double.PositiveInfinity;
        }
        else
        {
            layoutSlotSize.Height = double.PositiveInfinity;
        }

        //  Iterate through children.
        //  While we still supported virtualization, this was hidden in a child iterator (see source history).
        for (int i = 0, count = children.Count; i < count; ++i)
        {
            // Get next child.
            var child = children[i];

            if (child == null)
            {
                continue;
            }

            // Measure the child.
            child.Measure(layoutSlotSize);
            var childDesiredSize = child.DesiredSize;

            // Accumulate child size.
            if (fHorizontal)
            {
                stackDesiredSize.Width += childDesiredSize.Width;
                stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
            }
            else
            {
                stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                stackDesiredSize.Height += childDesiredSize.Height;
            }
        }

        return stackDesiredSize;
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (this.IsSimplified is false)
        {
            return this.ArrangeWrapMode(finalSize);
        }

        return this.ArrangeStackMode(finalSize);
    }

    private Size ArrangeWrapMode(Size finalSize)
    {
        var parentRibbonGroupBox = UIHelper.GetParent<RibbonGroupBox>(this);

        var isParentRibbonGroupBoxSharedSizeScope = parentRibbonGroupBox is not null && Grid.GetIsSharedSizeScope(parentRibbonGroupBox);

        var firstInLine = 0;
        var itemWidth = this.ItemWidth;
        var itemHeight = this.ItemHeight;
        double accumulatedV = 0;
        var itemU = this.Orientation == Orientation.Horizontal
            ? itemWidth
            : itemHeight;
        var curLineSize = new UvSize(this.Orientation);
        var uvFinalSize = new UvSize(this.Orientation, finalSize.Width, finalSize.Height);
        var itemWidthSet = !double.IsNaN(itemWidth);
        var itemHeightSet = !double.IsNaN(itemHeight);
        var useItemU = this.Orientation == Orientation.Horizontal
            ? itemWidthSet
            : itemHeightSet;

        var children = this.InternalChildren;

        var currentColumn = 1;

        for (int i = 0, count = children.Count; i < count; i++)
        {
            var child = children[i];

            if (child is null)
            {
                continue;
            }

            var sz = new UvSize(
                this.Orientation,
                itemWidthSet
                    ? itemWidth
                    : child.DesiredSize.Width,
                itemHeightSet
                    ? itemHeight
                    : child.DesiredSize.Height);

            if (DoubleUtil.GreaterThan(curLineSize.U + sz.U, uvFinalSize.U)) //need to switch to another line
            {
                this.ArrangeLineWrapMode(accumulatedV, curLineSize.V, firstInLine, i, useItemU, itemU);
                accumulatedV += curLineSize.V;
                curLineSize = sz;

                if (DoubleUtil.GreaterThan(sz.U, uvFinalSize.U)) //the element is wider then the constraint - give it a separate line
                {
                    //switch to next line which only contain one element
                    this.ArrangeLineWrapMode(accumulatedV, sz.V, i, ++i, useItemU, itemU);

                    accumulatedV += sz.V;
                    curLineSize = new(this.Orientation);
                }

                firstInLine = i;
                ++currentColumn;
            }
            else //continue to accumulate a line
            {
                curLineSize.U += sz.U;
                curLineSize.V = Math.Max(sz.V, curLineSize.V);
            }

            if (isParentRibbonGroupBoxSharedSizeScope
                && GetExcludeFromSharedSize(child) == false)
            {
                SetSharedSizeGroupName(child, $"SharedSizeGroup_Column_{currentColumn}");
            }
            else
            {
                SetSharedSizeGroupName(child, null);
            }
        }

        //arrange the last line, if any
        if (firstInLine < children.Count)
        {
            this.ArrangeLineWrapMode(accumulatedV, curLineSize.V, firstInLine, children.Count, useItemU, itemU);
        }

        return finalSize;
    }

    private Size ArrangeStackMode(Size finalSize)
    {
        var children = this.InternalChildren;
        var fHorizontal = this.Orientation is Orientation.Horizontal;
        var rcChild = new Rect(finalSize);
        var previousChildSize = 0.0;

        // Arrange and Position Children.
        for (int i = 0, count = children.Count; i < count; ++i)
        {
            var child = children[i];

            if (child == null)
            {
                continue;
            }

            if (fHorizontal)
            {
                rcChild.X += previousChildSize;
                previousChildSize = child.DesiredSize.Width;
                rcChild.Width = previousChildSize;
                rcChild.Height = Math.Max(finalSize.Height, child.DesiredSize.Height);
            }
            else
            {
                rcChild.Y += previousChildSize;
                previousChildSize = child.DesiredSize.Height;
                rcChild.Height = previousChildSize;
                rcChild.Width = Math.Max(finalSize.Width, child.DesiredSize.Width);
            }

            child.Arrange(rcChild);
        }

        return finalSize;
    }

    private void ArrangeLineWrapMode(double v, double lineV, int start, int end, bool useItemU, double itemU)
    {
        double u = 0;
        var isHorizontal = this.Orientation == Orientation.Horizontal;

        var children = this.InternalChildren;
        for (var i = start; i < end; i++)
        {
            var child = children[i];
            if (child is not null)
            {
                var childSize = new UvSize(this.Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                var layoutSlotU = useItemU
                    ? itemU
                    : childSize.U;
                child.Arrange(new(
                    isHorizontal
                        ? u
                        : v,
                    isHorizontal
                        ? v
                        : u,
                    isHorizontal
                        ? layoutSlotU
                        : lineV,
                    isHorizontal
                        ? lineV
                        : layoutSlotU));
                u += layoutSlotU;
            }
        }
    }

    private struct UvSize
    {
        internal UvSize(Orientation orientation, double width, double height)
        {
            this.U = this.V = 0d;
            this.orientation = orientation;
            this.Width = width;
            this.Height = height;
        }

        internal UvSize(Orientation orientation)
        {
            this.U = this.V = 0d;
            this.orientation = orientation;
        }

        internal double U;
        internal double V;
        private readonly Orientation orientation;

        internal double Width
        {
            get => this.orientation == Orientation.Horizontal
                ? this.U
                : this.V;

            set
            {
                if (this.orientation == Orientation.Horizontal)
                {
                    this.U = value;
                }
                else
                {
                    this.V = value;
                }
            }
        }

        internal double Height
        {
            get => this.orientation == Orientation.Horizontal
                ? this.V
                : this.U;

            set
            {
                if (this.orientation == Orientation.Horizontal)
                {
                    this.V = value;
                }
                else
                {
                    this.U = value;
                }
            }
        }
    }
}