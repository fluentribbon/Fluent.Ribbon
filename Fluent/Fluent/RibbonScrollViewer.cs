using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent
{
    public class RibbonScrollViewer:ScrollViewer
    {
        #region Overrides

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (VisualChildrenCount > 0)
            {
                return VisualTreeHelper.HitTest(GetVisualChild(0), hitTestParameters.HitPoint);
            }
            return base.HitTestCore(hitTestParameters);
        }

        #endregion
    }
}
