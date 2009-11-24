using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent
{
    public class RibbonContextualGroupsContainer: Panel
    {
        #region Overrides

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect finalRect = new Rect(finalSize);
            foreach (UIElement item in InternalChildren)
            {
                finalRect.Width = item.DesiredSize.Width;
                finalRect.Height = Math.Max(finalSize.Height, item.DesiredSize.Height);
                item.Arrange(finalRect);
                finalRect.X += item.DesiredSize.Width;
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double x = 0;
            Size infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (RibbonContextualTabGroup child in InternalChildren)
            {
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


                double finalWidth = tabsWidth;
                x += finalWidth;
                if (x > availableSize.Width)
                {
                    finalWidth -= x - availableSize.Width;
                    x = availableSize.Width;
                }
                child.Measure(new Size(finalWidth, availableSize.Height));
            }
            return new Size(x, 25/*availableSize.Height*/);
        }

        #endregion
    }
}
