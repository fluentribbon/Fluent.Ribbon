namespace Fluent.Controls.Primitives
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A custom panel that will arrange all of its children in the same
    /// space.
    /// </summary>
    public class SingleCellGrid : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var desiredWidth = 0.0;
            var desiredHeight = 0.0;

            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(availableSize);
                var desiredSize = child.DesiredSize;
                if (desiredSize.Width > desiredWidth)
                {
                    desiredWidth = desiredSize.Width;
                }

                if (desiredSize.Height > desiredHeight)
                {
                    desiredHeight = desiredSize.Height;
                }
            }

            return new Size(desiredWidth, desiredHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var rect = new Rect(finalSize);
            foreach (UIElement child in this.InternalChildren)
            {
                child.Arrange(rect);
            }
            return finalSize;
        }
    }
}
