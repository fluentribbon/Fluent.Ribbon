// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Represents ScrollViewer with modified hit test
    /// </summary>
    public class RibbonScrollViewer : ScrollViewer
    {
        /// <inheritdoc />
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (this.VisualChildrenCount > 0)
            {
                return VisualTreeHelper.HitTest(this.GetVisualChild(0), hitTestParameters.HitPoint);
            }

            return base.HitTestCore(hitTestParameters);
        }
    }
}