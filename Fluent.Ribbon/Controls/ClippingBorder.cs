namespace Fluent.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            base.Child = value;

            this.UpdateOpacityMask();
        }
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        // this.opacityBorder?.Width = this.ActualWidth;
        // this.opacityBorder?.Height = this.ActualHeight;
        this.opacityBorder?.Measure(finalSize);
        this.opacityBorder?.Arrange(new(finalSize));

        return base.ArrangeOverride(finalSize);
    }

    private void UpdateOpacityMask()
    {
        this.opacityBorder ??= new()
        {
            Background = Brushes.Magenta,
            CornerRadius = this.CornerRadius
        };

        this.opacityMask = this.IsOpacityMaskRequired()
            ? new VisualBrush(this.opacityBorder)
            : null;

#pragma warning disable WPF0041
        this.Child?.OpacityMask = this.opacityMask;
#pragma warning restore WPF0041
    }

    private bool IsOpacityMaskRequired()
    {
        return this.Child is not null
            && (this.CornerRadius.BottomLeft is not 0
                || this.CornerRadius.BottomRight is not 0
                || this.CornerRadius.TopLeft is not 0
                || this.CornerRadius.TopRight is not 0);
    }
}