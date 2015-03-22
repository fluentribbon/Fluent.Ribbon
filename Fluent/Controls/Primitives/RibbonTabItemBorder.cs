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
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

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
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

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
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
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
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

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
            var reservedWidth = (padding.Left + padding.Right) + (borderThickness*2);
            var reservedHeight = (padding.Top + padding.Bottom) + (borderThickness*2);
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
                var reservedWidth = (padding.Left + padding.Right) + (borderThickness*2);
                var reservedHeight = (padding.Top + padding.Bottom) + (borderThickness*2);

                childRect.Width = Math.Max(0.0, arrangeSize.Width - reservedWidth);
                childRect.Height = Math.Max(0.0, arrangeSize.Height - reservedHeight);

                childRect.X = padding.Left + borderThickness;
                childRect.Y = padding.Top + borderThickness;

                child.Arrange(childRect);
            }

            return arrangeSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            var background = this.Background;
            if (background != null)
            {
                var geometry = this.GetBorderGeometry(RenderSize, true);
                dc.DrawGeometry(background, new Pen(null, BorderThickness), geometry);
            }


            var brush = BorderBrush;
            var borderThickness = BorderThickness;

            if (brush != null && borderThickness > 0)
            {
                var borderGeometry = GetBorderGeometry(this.RenderSize, false);
                dc.DrawGeometry(this.Background, new Pen(brush, borderThickness), borderGeometry);
            }
        }

        private Geometry GetBorderGeometry(Size renderSize, bool isFilled)
        {
            var geomtry = new StreamGeometry();
            var borderThickness = BorderThickness*0.5;
            var cornerRadius = CornerRadius;

            bool isCornerRadiusIncluded = cornerRadius > 0;

            using (var ctx = geomtry.Open())
            {
                if (isCornerRadiusIncluded)
                {
                    // we start from the bottom right corner.
                    var p0 = new Point(0,
                        renderSize.Height - (isFilled ? 0 : borderThickness));

                    ctx.BeginFigure(p0, isFilled, false);

                    var p1 = new Point(p0.X + cornerRadius, p0.Y - cornerRadius);
                    var cp1 = new Point(p1.X, renderSize.Height - borderThickness);
                    ctx.QuadraticBezierTo(cp1, p1, true, false);

                    var p2 = new Point(p1.X, cornerRadius + borderThickness);
                    ctx.LineTo(p2, true, true);

                    var p3 = new Point(
                        p2.X + cornerRadius,
                        borderThickness);
                    var cp3 = new Point(p2.X, borderThickness);
                    ctx.QuadraticBezierTo(cp3, p3, true, false);

                    var p4 = new Point(
                        renderSize.Width - (cornerRadius*2) - (isFilled ? 0 : borderThickness),
                        borderThickness);
                    ctx.LineTo(p4, true, true);

                    var p5 = new Point(
                        p4.X + cornerRadius,
                        p4.Y + cornerRadius);
                    var cp5 = new Point(renderSize.Width - borderThickness - cornerRadius,
                        borderThickness);
                    ctx.QuadraticBezierTo(cp5, p5, true, false);

                    var p6 = new Point(
                        p5.X,
                        renderSize.Height - (cornerRadius + (isFilled ? 0 : borderThickness)));
                    ctx.LineTo(p6, true, true);

                    var p7 = new Point(p6.X + cornerRadius, p6.Y + cornerRadius);

                    if (isFilled)
                    {
                        p7.X -= borderThickness;
                    }

                    p7.X += borderThickness;

                    var cp7 = new Point(p6.X, renderSize.Height - borderThickness);
                    ctx.QuadraticBezierTo(cp7, p7, true, false);
                }
                else
                {
                    var renderRect = new Rect(borderThickness,
                        borderThickness,
                        renderSize.Width - (borderThickness*2),
                        renderSize.Height - borderThickness);

                    ctx.BeginFigure(renderRect.BottomLeft, isFilled, false);
                    ctx.LineTo(renderRect.TopLeft, true, true);
                    ctx.LineTo(renderRect.TopRight, true, true);
                    ctx.LineTo(renderRect.BottomRight, true, false);
                }


            }

            geomtry.Freeze();
            return geomtry;
        }

        #endregion
    }
}
