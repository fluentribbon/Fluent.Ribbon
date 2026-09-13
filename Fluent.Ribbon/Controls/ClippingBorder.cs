// ReSharper disable once CheckNamespace
namespace Fluent;

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Fluent.Extensions;
using JetBrains.Annotations;

/// <summary>
/// Draws a border, background, or both around another element and applies a clipping to the child.
/// </summary>
[PublicAPI]
public class ClippingBorder : Border
{
    private Border? opacityBorder;
    private Brush? opacityMask;

    static ClippingBorder()
    {
        CornerRadiusProperty.OverrideMetadata(typeof(ClippingBorder), new FrameworkPropertyMetadata(OnCornerRadiusChanged));

        return;

        void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ClippingBorder)d).UpdateOpacityMask();
        }
    }

    /// <inheritdoc />
    public override UIElement? Child
    {
        get => base.Child;
        set
        {
#pragma warning disable WPF0041
            base.Child?.OpacityMask = null;
#pragma warning restore WPF0041

            base.Child = value;

            this.UpdateOpacityMask();
        }
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        // Skip measure/arrange if opacity mask is not required
        if (this.IsOpacityMaskRequired())
        {
            this.EnsureOpacityBorder();

            this.opacityBorder!.SetCurrentValue(WidthProperty, finalSize.Width);
            this.opacityBorder.SetCurrentValue(HeightProperty, finalSize.Height);
            this.opacityBorder.Measure(finalSize);
            this.opacityBorder.Arrange(new(finalSize));
        }

        return base.ArrangeOverride(finalSize);
    }

    private void UpdateOpacityMask()
    {
        if (this.IsOpacityMaskRequired() is false)
        {
            this.opacityBorder = null;
            this.opacityMask = null;
            return;
        }

        this.EnsureOpacityBorder();

        this.opacityBorder!.SetCurrentValue(CornerRadiusProperty, this.CornerRadius);

        this.opacityMask = new VisualBrush(this.opacityBorder);

        this.Child!.OpacityMask = this.opacityMask;
    }

    private void EnsureOpacityBorder()
    {
        this.opacityBorder ??= new()
        {
            Background = Brushes.Magenta
        };
    }

#if netcore
    [MemberNotNullWhen(true, nameof(Child))]
    [MemberNotNullWhen(true, nameof(opacityBorder))]
#endif
    private bool IsOpacityMaskRequired()
    {
        return this.Child is not null
               && IsAllZero(this.CornerRadius) is false;

        static bool IsAllZero(CornerRadius cr) => cr.TopLeft.IsZero() && cr.TopRight.IsZero() && cr.BottomLeft.IsZero() && cr.BottomRight.IsZero();
    }
}