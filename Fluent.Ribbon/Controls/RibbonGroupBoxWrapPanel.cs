// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Special wrap panel for <see cref="RibbonGroupBox"/>.
    /// </summary>
    public class RibbonGroupBoxWrapPanel : Panel
    {
        private const Orientation DefaultOrientation = Orientation.Vertical;

        private Orientation orientation;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonGroupBoxWrapPanel()
        {
            this.orientation = DefaultOrientation;
        }

        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for <c>SharedSizeGroupName</c>.
        /// </summary>
        public static readonly DependencyProperty SharedSizeGroupNameProperty =
            DependencyProperty.RegisterAttached(
                                                "SharedSizeGroupName",
                                                typeof(string),
                                                typeof(RibbonGroupBoxWrapPanel),
                                                new PropertyMetadata(default(string)));

        /// <summary>
        /// Sets <see cref="SharedSizeGroupNameProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static void SetSharedSizeGroupName(DependencyObject element, string value)
        {
            element.SetValue(SharedSizeGroupNameProperty, value);
        }

        /// <summary>
        /// Gets <see cref="SharedSizeGroupNameProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static string GetSharedSizeGroupName(DependencyObject element)
        {
            return (string)element.GetValue(SharedSizeGroupNameProperty);
        }

        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for <c>SharedSizeGroupName</c>.
        /// </summary>
        public static readonly DependencyProperty ExcludeFromSharedSizeProperty =
            DependencyProperty.RegisterAttached(
                                                "ExcludeFromSharedSize",
                                                typeof(bool),
                                                typeof(RibbonGroupBoxWrapPanel),
                                                new FrameworkPropertyMetadata(
                                                                              BooleanBoxes.FalseBox,
                                                                              FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

        /// <summary>
        /// Sets <see cref="ExcludeFromSharedSizeProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static void SetExcludeFromSharedSize(DependencyObject element, bool value)
        {
            element.SetValue(ExcludeFromSharedSizeProperty, value);
        }

        /// <summary>
        /// Gets <see cref="ExcludeFromSharedSizeProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static bool GetExcludeFromSharedSize(DependencyObject element)
        {
            return (bool)element.GetValue(ExcludeFromSharedSizeProperty);
        }

        private static bool ValidateItemWidth(object value)
        {
            var v = (double)value;
            return ValidateWidthOrHeight(v);
        }

        private static bool ValidateItemHeight(object value)
        {
            var v = (double)value;
            return ValidateWidthOrHeight(v);
        }

        private static bool ValidateWidthOrHeight(double v)
        {
            return double.IsNaN(v) || (v >= 0.0d && !double.IsPositiveInfinity(v));
        }

        /// <summary>
        /// DependencyProperty for <see cref="ItemWidth" /> property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth),
                                        typeof(double),
                                        typeof(RibbonGroupBoxWrapPanel),
                                        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
                                        ValidateItemWidth);

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
            get { return (double)this.GetValue(ItemWidthProperty); }
            set { this.SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="ItemHeight" /> property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight),
                                        typeof(double),
                                        typeof(RibbonGroupBoxWrapPanel),
                                        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
                                        ValidateItemHeight);

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
            get { return (double)this.GetValue(ItemHeightProperty); }
            set { this.SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Orientation" /> property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            StackPanel.OrientationProperty.AddOwner(
                                                    typeof(RibbonGroupBoxWrapPanel),
                                                    new FrameworkPropertyMetadata(
                                                                                  Orientation.Horizontal,
                                                                                  FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                  OnOrientationChanged));

        /// <summary>
        /// Specifies dimension of children positioning in absence of wrapping.
        /// Wrapping occurs in orthogonal direction. For example, if Orientation is Horizontal,
        /// the items try to form horizontal rows first and if needed are wrapped and form vertical stack of rows.
        /// If Orientation is Vertical, items first positioned in a vertical column, and if there is
        /// not enough space - wrapping creates additional columns in horizontal dimension.
        /// </summary>
        public Orientation Orientation
        {
            get { return this.orientation; }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// <see cref="PropertyMetadata.PropertyChangedCallback"/>
        /// </summary>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = (RibbonGroupBoxWrapPanel)d;
            p.orientation = (Orientation)e.NewValue;
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
            private Orientation orientation;

            internal double Width
            {
                get { return this.orientation == Orientation.Horizontal ? this.U : this.V; }

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
                get { return this.orientation == Orientation.Horizontal ? this.V : this.U; }

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

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            var curLineSize = new UvSize(this.Orientation);
            var panelSize = new UvSize(this.Orientation);
            var uvConstraint = new UvSize(this.Orientation, constraint.Width, constraint.Height);
            var itemWidth = this.ItemWidth;
            var itemHeight = this.ItemHeight;
            var itemWidthSet = !double.IsNaN(itemWidth);
            var itemHeightSet = !double.IsNaN(itemHeight);

            var childConstraint = new Size(
                                            itemWidthSet ? itemWidth : constraint.Width,
                                            itemHeightSet ? itemHeight : constraint.Height);

            var children = this.InternalChildren;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                if (child == null)
                {
                    continue;
                }

                //Flow passes its own constrint to children
                child.Measure(childConstraint);

                //this is the size of the child in UV space
                var sz = new UvSize(
                                       this.Orientation,
                                       itemWidthSet ? itemWidth : child.DesiredSize.Width,
                                       itemHeightSet ? itemHeight : child.DesiredSize.Height);

                if (DoubleUtil.GreaterThan(curLineSize.U + sz.U, uvConstraint.U)) //need to switch to another line
                {
                    panelSize.U = Math.Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V;
                    curLineSize = sz;

                    if (DoubleUtil.GreaterThan(sz.U, uvConstraint.U)) //the element is wider then the constrint - give it a separate line
                    {
                        panelSize.U = Math.Max(sz.U, panelSize.U);
                        panelSize.V += sz.V;
                        curLineSize = new UvSize(this.Orientation);
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
            return new Size(panelSize.Width, panelSize.Height);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            var parentRibbonGroupBox = UIHelper.GetParent<RibbonGroupBox>(this);

            var isParentRibbonGroupBoxSharedSizeScope = parentRibbonGroupBox != null && Grid.GetIsSharedSizeScope(parentRibbonGroupBox);

            var firstInLine = 0;
            var itemWidth = this.ItemWidth;
            var itemHeight = this.ItemHeight;
            double accumulatedV = 0;
            var itemU = this.Orientation == Orientation.Horizontal ? itemWidth : itemHeight;
            var curLineSize = new UvSize(this.Orientation);
            var uvFinalSize = new UvSize(this.Orientation, finalSize.Width, finalSize.Height);
            var itemWidthSet = !double.IsNaN(itemWidth);
            var itemHeightSet = !double.IsNaN(itemHeight);
            var useItemU = this.Orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet;

            var children = this.InternalChildren;

            var currentColumn = 1;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];

                if (child == null)
                {
                    continue;
                }

                var sz = new UvSize(
                                       this.Orientation,
                                       itemWidthSet ? itemWidth : child.DesiredSize.Width,
                                       itemHeightSet ? itemHeight : child.DesiredSize.Height);

                if (DoubleUtil.GreaterThan(curLineSize.U + sz.U, uvFinalSize.U)) //need to switch to another line
                {
                    this.ArrangeLine(accumulatedV, curLineSize.V, firstInLine, i, useItemU, itemU);

                    accumulatedV += curLineSize.V;
                    curLineSize = sz;

                    if (DoubleUtil.GreaterThan(sz.U, uvFinalSize.U)) //the element is wider then the constraint - give it a separate line
                    {
                        //switch to next line which only contain one element
                        this.ArrangeLine(accumulatedV, sz.V, i, ++i, useItemU, itemU);

                        accumulatedV += sz.V;
                        curLineSize = new UvSize(this.Orientation);
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
                this.ArrangeLine(accumulatedV, curLineSize.V, firstInLine, children.Count, useItemU, itemU);
            }

            return finalSize;
        }

        private void ArrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
        {
            double u = 0;
            var isHorizontal = this.Orientation == Orientation.Horizontal;

            var children = this.InternalChildren;
            for (var i = start; i < end; i++)
            {
                var child = children[i];
                if (child != null)
                {
                    var childSize = new UvSize(this.Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                    var layoutSlotU = useItemU ? itemU : childSize.U;
                    child.Arrange(new Rect(
                                           isHorizontal ? u : v,
                                           isHorizontal ? v : u,
                                           isHorizontal ? layoutSlotU : lineV,
                                           isHorizontal ? lineV : layoutSlotU));
                    u += layoutSlotU;
                }
            }
        }
    }
}