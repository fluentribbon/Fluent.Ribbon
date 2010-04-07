#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents contextual groups container
    /// </summary>
    public class RibbonContextualGroupsContainer: Panel
    {
        #region Fields

        readonly List<Size> sizes = new List<Size>();

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for
        /// a System.Windows.FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should 
        /// use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect finalRect = new Rect(finalSize);
            int index = 0;
            foreach (UIElement item in InternalChildren)
            {
                finalRect.Width = sizes[index].Width;//item.DesiredSize.Width;
                finalRect.Height = Math.Max(finalSize.Height, sizes[index].Height);//Math.Max(finalSize.Height, item.DesiredSize.Height);
                item.Arrange(finalRect);
                finalRect.X += sizes[index].Width;// item.DesiredSize.Width;
                index++;
            }
            return finalSize;
        }
        
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for 
        /// child elements and determines a size for the System.Windows.FrameworkElement-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. 
        /// Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double x = 0;
            sizes.Clear();
            Size infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (RibbonContextualTabGroup child in InternalChildren)
            {
                // Calculate width of tab items of the group
                double tabsWidth = 0;
                for(int i=0;i<child.Items.Count;i++)
                {
                    tabsWidth += child.Items[i].DesiredSize.Width;
                }
                child.Measure(infinity);
                double groupWidth = child.DesiredSize.Width;

                bool tabWasChanged = false;                
                if(groupWidth>tabsWidth)
                {
                    // If tab's width is less than group's width we have to stretch tabs
                    double delta = (groupWidth - tabsWidth) / child.Items.Count;
                    for (int i = 0; i < child.Items.Count; i++)
                    {
                        if (child.Items[i].DesiredWidth==0)
                        {
                            child.Items[i].DesiredWidth=child.Items[i].DesiredSize.Width+delta;
                            child.Items[i].Measure(new Size(child.Items[i].DesiredWidth, child.Items[i].DesiredSize.Height));
                            tabWasChanged = true;
                        }
                    }
                }

                if (tabWasChanged)
                {
                    // If we have changed tabs layout we have 
                    // to invalidate down to RibbonTabsContainer 
                    Visual visual = child.Items[0] as Visual;
                    while (visual != null)
                    {
                        UIElement uiElement = visual as UIElement;
                        if (uiElement != null)
                        {
                            if (uiElement is RibbonTabsContainer)
                            {
                                uiElement.InvalidateMeasure();
                                break;
                            }

                            uiElement.InvalidateMeasure();
                        }

                        visual = VisualTreeHelper.GetParent(visual) as Visual;
                    }
                    tabsWidth = 0;
                    for (int i = 0; i < child.Items.Count; i++)
                    {
                        tabsWidth += child.Items[i].DesiredSize.Width;
                    }
                }

                // Calc final width and measure the group using it 
                double finalWidth = tabsWidth;
                x += finalWidth;
                if (x > availableSize.Width)
                {
                    finalWidth -= x - availableSize.Width;
                    x = availableSize.Width;
                }
                child.Measure(new Size(Math.Max(0, finalWidth), availableSize.Height));
                sizes.Add(new Size(Math.Max(0, finalWidth), availableSize.Height));
            }
            double height = availableSize.Height;
            if (double.IsPositiveInfinity(height)) height = 0;
            return new Size(x, height);
        }
        
        #endregion
    }
}
