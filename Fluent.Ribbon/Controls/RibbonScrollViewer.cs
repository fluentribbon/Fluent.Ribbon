// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Fluent.Internal;

    /// <summary>
    /// Represents <see cref="ScrollViewer"/> with modified hit test
    /// </summary>
    public class RibbonScrollViewer : ScrollViewer
    {
        /// <inheritdoc />
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (this.VisualChildrenCount > 0
                && this.GetVisualChild(0) is { } firstVisualChild)
            {
                return VisualTreeHelper.HitTest(firstVisualChild, hitTestParameters.HitPoint);
            }

            return base.HitTestCore(hitTestParameters);
        }

        /// <inheritdoc />
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (this.ScrollInfo != null)
            {
                var horizontalOffsetBefore = this.ScrollInfo.HorizontalOffset;
                var verticalOffsetBefore = this.ScrollInfo.VerticalOffset;

                if (e.Delta < 0)
                {
                    this.ScrollInfo.MouseWheelDown();
                }
                else
                {
                    this.ScrollInfo.MouseWheelUp();
                }

                e.Handled = DoubleUtil.AreClose(horizontalOffsetBefore, this.ScrollInfo.HorizontalOffset) == false
                            || DoubleUtil.AreClose(verticalOffsetBefore, this.ScrollInfo.VerticalOffset) == false;
            }
        }
    }
}