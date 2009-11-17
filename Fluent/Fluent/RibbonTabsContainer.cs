using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Represent panel with ribbon tab items.
    /// It is automatically adjusting size of tabs
    /// </summary>
    public class RibbonTabsContainer : StackPanel
    {
        #region Fields

        // A cached copy of the constraint from the previous layout pass.
        Size cachedConstraint;
        // A cached copy of the desired size from the previous layout pass.
        Size cachedDesiredSize;
        

        #endregion
                
        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabsContainer() : base()
        {
            Orientation = Orientation.Horizontal;
        }

        #endregion

        #region Layout Overridings

        /// <summary>
        /// Measures all of the RibbonGroupBox, and resize them appropriately
        /// to fit within the available room
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (Children.Count == 0) return constraint;
            Size desiredSize = base.MeasureOverride(constraint);

            // If the constraint and desired size are equal to those in the cache, skip
            // this layout measure pass.
            if ((constraint != cachedConstraint) || (desiredSize != cachedDesiredSize))
            {
                cachedConstraint = constraint;
                cachedDesiredSize = desiredSize;

                // Let tabs get width as much as they want
                foreach (UIElement tab in Children)
                {
                    FrameworkElement element = (FrameworkElement)tab;
                    if (Double.IsNaN(element.Width)) continue;
                    element.Width = Double.NaN;
                }
                desiredSize = base.MeasureOverride(constraint);
                
                // Performs steps as described in "2007 MICROSOFT® OFFICE FLUENT™ 
                // USER INTERFACE DESIGN GUIDELINES"

                // Step 1. Gradually remove empty space to the right of the tabs
                // If all tabs already in full size, just return
                if (constraint.Width > desiredSize.Width)
                {
                    // Hide separator lines between tabs
                    HideSeparators();
                    return desiredSize;
                }

                // Step 2. Gradually and uniformly remove the padding from both sides 
                // of all the tabs until the minimum padding required for displaying 
                // the tab selection and hover states is reached (usial tabs)
                double overflowWidth = desiredSize.Width - constraint.Width;
                double whitespace = (Children[0] as RibbonTabItem).Whitespace;
                double contextualTabsCount = Children.Cast<RibbonTabItem>().Count(x => x.IsContextual);
                double regularTabsCount = Children.Count - contextualTabsCount;
                if (overflowWidth < regularTabsCount * whitespace * 2)
                {
                    double decreaseValue = overflowWidth / (double)regularTabsCount;
                    foreach (RibbonTabItem tab in Children) if (!tab.IsContextual) tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - decreaseValue), tab.DesiredSize.Height));// tab.Width = Math.Max(0, tab.ActualWidth - decreaseValue);
                    desiredSize = new Size(desiredSize.Width - overflowWidth, desiredSize.Height);

                    // Add separator lines between 
                    // tabs to assist readability
                    HideSeparators();
                    return desiredSize;
                }

                // Step 3. Gradually and uniformly remove the padding from both sides 
                // of all the tabs until the minimum padding required for displaying 
                // the tab selection and hover states is reached (contextual tabs)
                if (overflowWidth < Children.Count * whitespace * 2)
                {
                    double regularTabsWhitespace = (double)regularTabsCount * whitespace * 2.0;
                    double decreaseValue = (overflowWidth - regularTabsWhitespace) / (double)contextualTabsCount;
                    foreach (RibbonTabItem tab in Children) 
                        if (tab.IsContextual) tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - decreaseValue), tab.DesiredSize.Height));
                        else tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));
                    desiredSize = new Size(desiredSize.Width - overflowWidth, desiredSize.Height);
                    
                    // Add separator lines between 
                    // tabs to assist readability
                    ShowSeparators();
                    return desiredSize;
                }


                // Step 4. Reduce the width of the tab with the longest name by 
                // truncating the text label. Continue reducing the width of the largest 
                // tab (or tabs in the case of ties) until all tabs are the same width. 
                // (Regular tabs)
                foreach (RibbonTabItem tab in Children)
                    if (tab.IsContextual) tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));
                    else tab.Measure(new Size(Math.Max(0, tab.DesiredSize.Width - whitespace * 2.0), tab.DesiredSize.Height));
                overflowWidth -= (double)Children.Count * whitespace * 2.0;

                // Sort regular tabs by descending
                RibbonTabItem[] sortedRegularTabItems = Children.Cast<RibbonTabItem>()
                    .Where(x => !x.IsContextual)
                    .OrderByDescending(x => x.DesiredSize.Width)
                    .ToArray();

                // Find how many regular tabs we have to reduce
                double reducedLength = 0;
                int reduceCount = 0;
                for (int i = 0; i < sortedRegularTabItems.Length - 1; i++)
                {
                    double temp =
                        sortedRegularTabItems[i].DesiredSize.Width -
                        sortedRegularTabItems[i + 1].DesiredSize.Width;
                    reducedLength += temp * (i + 1);
                    reduceCount = i + 1;
                    if (reducedLength > overflowWidth) break;
                }

                if (reducedLength > overflowWidth)
                {
                    // Reduce regular tabs
                    double requiredWidth = sortedRegularTabItems[reduceCount].DesiredSize.Width;
                    if (reducedLength > overflowWidth) requiredWidth += (reducedLength - overflowWidth) / (double)reduceCount;
                    for(int i = 0; i < reduceCount; i++)
                    {
                        sortedRegularTabItems[i].Measure(new Size(requiredWidth, constraint.Height));
                    }
                
                    desiredSize = new Size(desiredSize.Width - overflowWidth, desiredSize.Height);
                    // Add separator lines between 
                    // tabs to assist readability
                    ShowSeparators();
                    return desiredSize;
                }
                

                // Step 5. Reduce the width of all core tabs equally 
                // down to a minimum of about three characters.
                double regularTabsWidth = sortedRegularTabItems.Sum(x => x.DesiredSize.Width);
                double minimumRegularTabsWidth = 30.0 * sortedRegularTabItems.Length;
                if (overflowWidth < regularTabsWidth - minimumRegularTabsWidth)
                {
                    double settedWidth = (regularTabsWidth - overflowWidth) / (double)regularTabsCount;
                    for (int i = 0; i < reduceCount; i++)
                    {
                        sortedRegularTabItems[i].Measure(new Size(settedWidth, constraint.Height));
                    }
                    desiredSize = new Size(desiredSize.Width - overflowWidth, desiredSize.Height);
                    // Add separator lines between 
                    // tabs to assist readability
                    ShowSeparators();
                    return desiredSize;
                }

                // Step 6. Reduce the width of the tab with the longest name by 
                // truncating the text label. Continue reducing the width of the largest 
                // tab (or tabs in the case of ties) until all tabs are the same width. 
                // (Contextual tabs)
                for (int i = 0; i < reduceCount; i++)
                {
                    sortedRegularTabItems[i].Measure(new Size(30, constraint.Height));
                }
                overflowWidth -= regularTabsWidth - minimumRegularTabsWidth;

                // Sort contextual tabs by descending
                RibbonTabItem[] sortedContextualTabItems = Children.Cast<RibbonTabItem>()
                    .Where(x => x.IsContextual)
                    .OrderByDescending(x => x.DesiredSize.Width)
                    .ToArray();

                // Find how many regular tabs we have to reduce
                reducedLength = 0;
                reduceCount = 0;
                for (int i = 0; i < sortedContextualTabItems.Length - 1; i++)
                {
                    double temp =
                        sortedContextualTabItems[i].DesiredSize.Width -
                        sortedContextualTabItems[i + 1].DesiredSize.Width;
                    reducedLength += temp * (i + 1);
                    reduceCount = i + 1;
                    if (reducedLength > overflowWidth) break;
                }

                if (reducedLength > overflowWidth)
                {
                    // Reduce regular tabs
                    double requiredWidth = sortedContextualTabItems[reduceCount].DesiredSize.Width;
                    if (reducedLength > overflowWidth) requiredWidth += (reducedLength - overflowWidth) / (double)reduceCount;
                    for (int i = 0; i < reduceCount; i++)
                    {
                        sortedContextualTabItems[i].Measure(new Size(requiredWidth, constraint.Height));
                    }

                    desiredSize = new Size(desiredSize.Width - overflowWidth, desiredSize.Height);
                    // Add separator lines between 
                    // tabs to assist readability
                    ShowSeparators();
                    return desiredSize;
                }
                else
                {
                    double contextualTabsWidth = sortedContextualTabItems.Sum(x => x.DesiredSize.Width);
                    double minimumContextualTabsWidth = 30.0 * sortedContextualTabItems.Length;

                    double settedWidth = Math.Max(30, (contextualTabsWidth - overflowWidth) / (double)contextualTabsCount);
                    for (int i = 0; i < reduceCount; i++)
                    {
                        sortedContextualTabItems[i].Measure(new Size(settedWidth, constraint.Height));
                    }
                    desiredSize = new Size(GetChildrenWidth(), desiredSize.Height);

                    // Add separator lines between 
                    // tabs to assist readability
                    ShowSeparators();
                    return desiredSize;
                }

                // Step 7. Display a horizontal scroll button
                // TODO:
            }

            return desiredSize;
        }

        

        /// <summary>
        /// Calculates the total width of all children
        /// </summary>
        /// <returns>Returns the total width</returns>
        double GetChildrenWidth()
        {
            double result = 0;
            foreach (UIElement child in this.Children)
            {
                result += child.DesiredSize.Width;
            }
            return result + 3;
        }

        void ShowSeparators()
        {
            foreach (RibbonTabItem tab in Children) if (!tab.IsSeparatorVisible) tab.IsSeparatorVisible = true;
        }

        void HideSeparators()
        {
            foreach (RibbonTabItem tab in Children) if (tab.IsSeparatorVisible) tab.IsSeparatorVisible = false;
        }

        #endregion
    }
}
