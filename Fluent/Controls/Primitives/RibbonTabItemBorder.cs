using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent.Controls.Primitives
{
    /// <summary>
    /// A custom border element for use in the <see cref="RibbonTabItem"/> control
    /// template.
    /// </summary>
    public class RibbonTabItemBorder : Decorator
    {
        #region "Fields"

        private Pen _cachedInnerBorderPen;
        private Pen _cachedOuterBorderPen;
        private IRenderer _mouserOverRenderer;
        private IRenderer _normalRenderer;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="RibbonTabItemBorder"/> class.
        /// </summary>
        public RibbonTabItemBorder()
        {
        }

        #endregion

        #region "Properties"

        #region CornerRadius

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(double),
            typeof(RibbonTabItemBorder),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                (o, args) =>
                {
                    var border = (RibbonTabItemBorder)o;
                    border.InvalidateGeometries();
                }));

        /// <summary>
        /// Gets or sets the CornerRadius property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        #region RenderInnerBorder

        /// <summary>
        /// Identifies the <see cref="RenderInnerBorder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderInnerBorderProperty = DependencyProperty.Register(
            "RenderInnerBorder",
            typeof(bool),
            typeof(RibbonTabItemBorder),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the RenderInnerBorder property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public bool RenderInnerBorder
        {
            get { return (bool)GetValue(RenderInnerBorderProperty); }
            set { SetValue(RenderInnerBorderProperty, value); }
        }

        #endregion

        #region BorderBrush

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush",
            typeof(Brush),
            typeof(RibbonTabItemBorder),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (o, args) =>
                {
                    var border = (RibbonTabItemBorder)o;
                    border._cachedOuterBorderPen = null;
                }));

        /// <summary>
        /// Gets or sets the BorderBrush property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        #endregion

        #region InnerBorderBrush

        /// <summary>
        /// Identifies the <see cref="InnerBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerBorderBrushProperty = DependencyProperty.Register(
            "InnerBorderBrush",
            typeof(Brush),
            typeof(RibbonTabItemBorder),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (o, args) =>
                {
                    var border = (RibbonTabItemBorder)o;
                    border._cachedInnerBorderPen = null;
                }));

        /// <summary>
        /// Gets or sets the InnerBorderBrush property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public Brush InnerBorderBrush
        {
            get { return (Brush)GetValue(InnerBorderBrushProperty); }
            set { SetValue(InnerBorderBrushProperty, value); }
        }

        #endregion

        #region BorderThickness

        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness",
            typeof(double),
            typeof(RibbonTabItemBorder),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                (o, args) =>
                {
                    var border = (RibbonTabItemBorder)o;
                    border._cachedOuterBorderPen = null;
                    border._cachedInnerBorderPen = null;
                    border.InvalidateGeometries();
                }),
            value =>
            {
                // you can't have a thickness lest than 0
                // we don't coerce we validate.
                var thickness = (double)value;
                return thickness >= 0;
            });

        /// <summary>
        /// Gets or sets the BorderThickness property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        #endregion

        #region Padding

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            "Padding",
            typeof(Thickness),
            typeof(RibbonTabItemBorder),
            new FrameworkPropertyMetadata(
                new Thickness(),
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the Padding property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        #endregion

        #region Background

        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(Brush),
            typeof(RibbonTabItemBorder),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the Background property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion

        #endregion

        #region "Methods"

        protected override Size MeasureOverride(Size constraint)
        {
            var child = this.Child;
            var borderThickness = this.BorderThickness;
            var padding = this.Padding;

            // prepare the child available space
            var reservedWidth = (padding.Left + padding.Right) + (borderThickness * 2);
            var reservedHeight = (padding.Top + padding.Bottom) + (borderThickness * 2);
            if (child == null)
            {
                return new Size(reservedWidth, reservedHeight);
            }

            var availableWidth = Math.Max(0.0, constraint.Width - reservedWidth);
            var availableHeight = Math.Max(0.0, constraint.Height - reservedHeight);

            child.Measure(new Size(availableWidth, availableHeight));
            var desiredSize = child.DesiredSize;
            return new Size(desiredSize.Width + reservedWidth, desiredSize.Height + reservedHeight);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var child = this.Child;

            if (child != null)
            {
                var childRect = new Rect(arrangeSize);
                var borderThickness = this.BorderThickness;
                var padding = this.Padding;
                var reservedWidth = (padding.Left + padding.Right) + (borderThickness * 2);
                var reservedHeight = (padding.Top + padding.Bottom) + (borderThickness * 2);

                childRect.Width = Math.Max(0.0, arrangeSize.Width - reservedWidth);
                childRect.Height = Math.Max(0.0, arrangeSize.Height - reservedHeight);

                childRect.X = padding.Left + borderThickness;
                childRect.Y = padding.Top + borderThickness;

                child.Arrange(childRect);
            }

            this.InvalidateGeometries();

            return arrangeSize;
        }

        private void InvalidateGeometries()
        {
            EnsureRenderers();
            _normalRenderer.InvalidateCache();
            _mouserOverRenderer.InvalidateCache();
        }

        private void EnsureRenderers()
        {
            if (_normalRenderer == null)
            {
                _normalRenderer = new NormalRenderer(this);
            }

            if (_mouserOverRenderer == null)
            {
                _mouserOverRenderer = new MouseOverRenderer(this);
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            IRenderer currentRenderer;
            EnsureOuterBorderPen();

            if (RenderInnerBorder)
            {
                EnsureInnerBorderPen();
                currentRenderer = _mouserOverRenderer;
            }
            else
            {
                currentRenderer = _normalRenderer;
            }

            currentRenderer.Render(dc);
        }

        private void EnsureOuterBorderPen()
        {
            if (_cachedOuterBorderPen == null)
            {
                _cachedOuterBorderPen = new Pen(this.BorderBrush, this.BorderThickness);
            }
        }

        private void EnsureInnerBorderPen()
        {
            if (_cachedInnerBorderPen == null)
            {
                _cachedInnerBorderPen = new Pen(this.InnerBorderBrush, this.BorderThickness);
            }
        }

        #endregion

        #region "Private Types"

        private interface IRenderer
        {
            void Render(DrawingContext dc);
            void InvalidateCache();
        }

        private class MouseOverRenderer : IRenderer
        {
            #region "Fields"

            private readonly RibbonTabItemBorder _border;
            private Geometry _cachedOuterBorderGeometry;
            private Geometry _cachedInnerBorderGeometry;
            private Geometry _cachedBackgroundGeometry;

            private const double BOTTOM_PADDING = 1.0;

            #endregion

            #region "Constructors"

            /// <summary>
            /// Initializes instance members of the <see cref="MouseOverRenderer"/> class.
            /// </summary>
            public MouseOverRenderer(RibbonTabItemBorder border)
            {
                _border = border;
            }

            #endregion

            #region "Methods"

            public void InvalidateCache()
            {
                _cachedBackgroundGeometry = null;
                _cachedInnerBorderGeometry = null;
                _cachedOuterBorderGeometry = null;
            }

            public void Render(DrawingContext dc)
            {
                var cr = _border.CornerRadius;
                var rs = _border.RenderSize;
                var t = _border.BorderThickness;

                // we start by rendering the background
                var background = _border.Background;
                if (background != null)
                {
                    EnsureBackgroundGeometry(rs, t, cr);
                    dc.DrawGeometry(background, null, _cachedBackgroundGeometry);
                }

                // render the inner border
                var innerBorderPen = _border._cachedInnerBorderPen;
                if (innerBorderPen != null)
                {
                    EnsureInnerBorderGeometry(rs, t, cr);
                    dc.DrawGeometry(null, innerBorderPen, _cachedInnerBorderGeometry);
                }

                // render the outer border
                var outerBorderPen = _border._cachedOuterBorderPen;
                if (outerBorderPen != null)
                {
                    EnsureOuterBorderGeometry(rs, t * 0.5, cr);
                    dc.DrawGeometry(null, outerBorderPen, _cachedOuterBorderGeometry);
                }
            }

            private void EnsureOuterBorderGeometry(Size rs, double ht, double cr)
            {
                if (_cachedOuterBorderGeometry != null)
                    return;

                var rect = new Rect(
                    ht + cr,
                    ht,
                    rs.Width - ((ht * 2) + (cr * 2)),
                    rs.Height - ht - BOTTOM_PADDING);
                _cachedOuterBorderGeometry = this.ConstructGeometry(rect, cr, false);
            }

            private void EnsureBackgroundGeometry(Size rs /*render size*/, double t /* thickness*/, double cr /* corner radius */)
            {
                if (_cachedBackgroundGeometry != null)
                    return;

                var ht = t * 0.5;
                // render the background
                var bgRect = new Rect(
                    ht + cr + t + ht,
                    ht + t + ht,
                    Math.Max(0.0, rs.Width - ((ht * 2) + (cr * 2) + ((t + ht) * 2))),
                    Math.Max(0.0, rs.Height - (ht * 2) - t - BOTTOM_PADDING));

                _cachedBackgroundGeometry = this.ConstructGeometry(bgRect, GetInnerBorderRadius(cr, t), true);
            }

            private void EnsureInnerBorderGeometry(Size rs, double t, double cr)
            {
                if (_cachedInnerBorderGeometry != null)
                    return;

                double ht = t * 0.5;
                var innerBorderRect = new Rect(
                    ht + cr + t,
                    ht + t,
                    Math.Max(0.0, rs.Width - ((ht * 2) + (cr * 2) + (t * 2))),
                    Math.Max(0.0, rs.Height - ht - t - BOTTOM_PADDING));

                _cachedInnerBorderGeometry = this.ConstructGeometry(innerBorderRect, cr, true);
            }

            private Geometry ConstructGeometry(Rect rect, double cornerRadius, bool isInnerBorder)
            {
                var geometry = new StreamGeometry();
                const bool isStroked = true;
                const bool isLargeArc = false;
                const bool isSmoothJoin = false;
                var t = _border.BorderThickness;

                using (var ctx = geometry.Open())
                {
                    var p0 = new Point(rect.Left, rect.Bottom);
                    ctx.BeginFigure(p0, true /* is filled */, false);

                    if (cornerRadius > 0)
                    {
                        double radius;

                        if (isInnerBorder)
                        {
                            //var actualOuterCornerRadius = corner radius + (border thickness / 2);
                            radius = Math.Max(0.0, cornerRadius - (t / 2) - (t / 2));

                            var p2 = new Point(rect.Left, rect.Top + radius);
                            ctx.LineTo(p2, isStroked, isSmoothJoin);

                            if (radius > 0.0)
                            {
                                var p3 = new Point(rect.Left + radius, rect.Top);
                                ctx.ArcTo(p3,
                                    new Size(radius, radius),
                                    0,
                                    isLargeArc,
                                    SweepDirection.Clockwise,
                                    isStroked,
                                    isSmoothJoin);
                            }
                        }
                        else
                        {
                            radius = cornerRadius;

                            var p2 = new Point(rect.Left, rect.Top + radius);
                            ctx.LineTo(p2, isStroked, isSmoothJoin);

                            if (radius > 0)
                            {
                                var p3 = new Point(rect.Left + radius, rect.Top);
                                ctx.ArcTo(p3,
                                    new Size(radius, radius),
                                    0,
                                    isLargeArc,
                                    SweepDirection.Clockwise,
                                    isStroked,
                                    isSmoothJoin);
                            }
                        }
                    }
                    else
                    {
                        var p2 = new Point(rect.Left, rect.Top);
                        ctx.LineTo(p2, isStroked, isSmoothJoin);
                    }

                    if (cornerRadius > 0)
                    {
                        double radius;

                        if (isInnerBorder)
                        {
                            radius = Math.Max(0.0, cornerRadius - (t / 2) - (t / 2));

                            var p2 = new Point(rect.Right - radius, rect.Top);
                            ctx.LineTo(p2, isStroked, isSmoothJoin);

                            if (radius > 0)
                            {
                                var p3 = new Point(rect.Right, rect.Top + radius);
                                ctx.ArcTo(p3,
                                    new Size(radius, radius),
                                    0,
                                    isLargeArc,
                                    SweepDirection.Clockwise,
                                    isStroked,
                                    isSmoothJoin);
                            }
                        }
                        else
                        {
                            radius = cornerRadius;

                            var p4 = new Point(rect.Right - radius, rect.Top);
                            ctx.LineTo(p4, isStroked, isSmoothJoin);

                            if (radius > 0)
                            {
                                var p5 = new Point(rect.Right, rect.Top + radius);
                                ctx.ArcTo(p5,
                                    new Size(radius, radius),
                                    0,
                                    isLargeArc,
                                    SweepDirection.Clockwise,
                                    isStroked,
                                    isSmoothJoin);
                            }
                        }
                    }
                    else
                    {
                        var p4 = new Point(rect.Right, rect.Top);
                        ctx.LineTo(p4, isStroked, false);
                    }

                    var p6 = new Point(rect.Right, rect.Bottom);
                    ctx.LineTo(p6, isStroked, false);
                }

                geometry.Freeze();
                return geometry;
            }

            private double GetInnerBorderRadius(double cornerRadius, double borderThickness)
            {
                // very helpful: http://stackoverflow.com/questions/2932146/math-problem-determine-the-corner-radius-of-an-inner-border-based-on-outer-corn
                // as well as the main answer here: https://social.msdn.microsoft.com/Forums/vstudio/en-US/d51311e2-80c1-44cd-a5cf-f463abe444f0/cornerradius-and-border-width
                return Math.Max(0.0, cornerRadius - (borderThickness / 2) - (borderThickness / 2));
            }

            #endregion
        }

        private class NormalRenderer : IRenderer
        {
            #region "Methods"

            private readonly RibbonTabItemBorder _border;
            private Geometry _cachedBackgroundGeometry;
            private Geometry _cachedBorderGeometry;

            #endregion

            #region "Constructors"

            /// <summary>
            /// Initializes instance members of the <see cref="NormalRenderer"/> class.
            /// </summary>
            internal NormalRenderer(RibbonTabItemBorder border)
            {
                _border = border;
            }

            #endregion

            #region "Methods"

            public void InvalidateCache()
            {
                // they will be re-generated on the render pass
                _cachedBackgroundGeometry = null;
                _cachedBorderGeometry = null;
            }

            public void Render(DrawingContext dc)
            {
                var brush = _border.Background;
                if (brush != null)
                {
                    EnsureBackgroundGeometry();
                    dc.DrawGeometry(brush, null, _cachedBackgroundGeometry);
                }

                if (_border.BorderBrush != null)
                {
                    EnsureBorderGeometry();
                    dc.DrawGeometry(null, _border._cachedOuterBorderPen, _cachedBorderGeometry);
                }
            }

            private void EnsureBackgroundGeometry()
            {
                if (_cachedBackgroundGeometry == null)
                {
                    _cachedBackgroundGeometry = GenerateGeometry(_border.RenderSize, _border.CornerRadius, true);
                }
            }

            private void EnsureBorderGeometry()
            {
                if (_cachedBorderGeometry == null)
                {
                    _cachedBorderGeometry = GenerateGeometry(_border.RenderSize, _border.CornerRadius, false);
                }
            }

            private Geometry GenerateGeometry(Size renderSize, double cr /* corner Radius*/, bool isBackground)
            {
                var bt = _border.BorderThickness;
                var hbt = bt * 0.5;
                const bool isLargeArc = false;
                const bool isSmoothJoin = false;
                var rect = new Rect(hbt, hbt, renderSize.Width - bt, renderSize.Height - bt);
                var radiusSize = new Size(cr, cr);
                var geometry = new StreamGeometry();
                const bool isStroked = true;
                var hasCornerRadius = cr > 0;

                using (var ctx = geometry.Open())
                {
                    var p0 = new Point(rect.Left - (hasCornerRadius ? hbt : 0),
                        rect.Bottom + (isBackground || hasCornerRadius ? 0 : hbt));
                    ctx.BeginFigure(p0, true /* is filled */, false);
                    var p1 = new Point(rect.Left + cr, rect.Bottom - cr);

                    if (hasCornerRadius)
                    {
                        var cp1 = new Point(rect.Left + cr, rect.Bottom);
                        ctx.QuadraticBezierTo(cp1, p1, isStroked, false);
                    }

                    var p2 = new Point(rect.Left + cr, rect.Top + cr);
                    ctx.LineTo(p2, isStroked, false);

                    if (hasCornerRadius)
                    {
                        var p3 = new Point(rect.Left + (cr * 2), rect.Top);
                        ctx.ArcTo(p3,
                            radiusSize,
                            0,
                            isLargeArc,
                            SweepDirection.Clockwise,
                            true,
                            isSmoothJoin);
                    }

                    var p4 = new Point(rect.Right - (cr * 2), rect.Top);
                    ctx.LineTo(p4, isStroked, false);

                    if (hasCornerRadius)
                    {
                        var p5 = new Point(rect.Right - cr, rect.Top + cr);
                        ctx.ArcTo(p5,
                            radiusSize,
                            0,
                            isLargeArc,
                            SweepDirection.Clockwise,
                            true,
                            isSmoothJoin);
                    }

                    var p6 = new Point(rect.Right - cr, rect.Bottom - cr + (isBackground || hasCornerRadius ? 0 : hbt));
                    ctx.LineTo(p6, isStroked, false);

                    if (hasCornerRadius)
                    {
                        var p7 = new Point(rect.Right + hbt, rect.Bottom);
                        var cp7 = new Point(rect.Right - cr, rect.Bottom);
                        ctx.QuadraticBezierTo(cp7, p7, isStroked, false);
                    }
                }

                geometry.Freeze();

                // I found it very hard to construct a background geometry
                // that doesn't overlap the border geometry, the issue was
                // always the bottom left/right corner radius.
                // so what I am doing is constructing the background geometry based
                // on the border geometry and stretching to fill the remaining
                // bottom space.
                return isBackground ? ApplyTransform(geometry, bt, renderSize) : geometry;
            }

            private static Geometry ApplyTransform(Geometry geometry, double bt, Size renderSize)
            {
                var pathGeometry = new PathGeometry();
                pathGeometry.AddGeometry(geometry);

                var hbt = bt * 0.5;
                //var scaleY = ((hbt * 100) / renderSize.Height) / 100;
                var scaleY = hbt / renderSize.Height;
                pathGeometry.Transform = new ScaleTransform(1, scaleY + 1, renderSize.Width * 0.5, 0);
                pathGeometry.Freeze();
                return pathGeometry;
            }

            #endregion
        }

        #endregion
    }
}
