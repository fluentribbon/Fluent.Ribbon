using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    /// <summary>
    /// Represents ScrollViewer with modified hit test
    /// </summary>
    public class RibbonScrollViewer : ScrollViewer
    {
        /// <summary>
        /// Performs a hit test to determine whether the specified 
        /// points are within the bounds of this ScrollViewer
        /// </summary>
        /// <returns>The result of the hit test</returns>
        /// <param name="hitTestParameters">The parameters for hit testing within a visual object</param>
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