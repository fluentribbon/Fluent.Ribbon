namespace Fluent.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// Draws a border, background, or both around another element and applies a clipping to the child.
/// </summary>
public class ClippingBorder : VisualClippingBorder
{
}

/// <summary>
/// Draws a border, background, or both around another element and applies a clipping to the child.
/// </summary>
public class VisualClippingBorder : Border
{
    private Border? opacityBorder;
    private Brush? opacityMask;

    /// <inheritdoc />
    public override UIElement? Child
    {
        get => base.Child;
        set
        {
            base.Child = value;

            this.opacityBorder = this.CreateOpacityBorder();
            this.opacityMask = new VisualBrush(this.opacityBorder);
            value?.OpacityMask = this.opacityMask;
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

    /// <summary>
    /// Creates the opacity border.
    /// </summary>
    /// <returns></returns>
    protected virtual Border CreateOpacityBorder()
    {
        return new Border
        {
            Background = Brushes.Magenta,
            CornerRadius = this.CornerRadius
        };
    }
}