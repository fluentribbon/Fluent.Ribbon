#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents ScrollViewer with modified hit test
    /// </summary>
    public class RibbonScrollViewer : ScrollViewer
    {
        #region Overrides

        /// <summary>
        /// Performs a hit test to determine whether the specified 
        /// points are within the bounds of this ScrollViewer
        /// </summary>
        /// <returns>The result of the hit test</returns>
        /// <param name="hitTestParameters">The parameters for hit testing within a visual object</param>
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
