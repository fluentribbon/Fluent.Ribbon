using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Represents panel for status bar
    /// </summary>
    public class StatusBarPanel: Panel
    {
        #region Attributes
        
        private List<UIElement> leftChildren = new List<UIElement>();
        private List<UIElement> rightChildren = new List<UIElement>();
        private List<UIElement> otherChildren = new List<UIElement>();

        private int lastRightIndex = 0;
        private int lastLeftIndex = 0;

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class. 
        /// </summary>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            // Sort children
            leftChildren.Clear();
            rightChildren.Clear();
            otherChildren.Clear();
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                FrameworkElement child = InternalChildren[i] as FrameworkElement;
                if (child != null)
                {
                    if (child.HorizontalAlignment == HorizontalAlignment.Left) leftChildren.Add(child);
                    else if (child.HorizontalAlignment == HorizontalAlignment.Right) rightChildren.Add(child);
                    else otherChildren.Add(child);
                }
            }

            lastRightIndex = rightChildren.Count;
            lastLeftIndex = leftChildren.Count;

            // Measure children
            Size infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Size zero = new Size(0, 0);
            double width = 0;
            double height = 0;
            bool canAdd = true;
            // Right children
            for (int i = 0; i < rightChildren.Count; i++)
            {
                if (canAdd)
                {
                    rightChildren[i].Measure(infinity);
                    height = Math.Max(rightChildren[i].DesiredSize.Height, height);
                    if (width + rightChildren[i].DesiredSize.Width <= availableSize.Width)
                    {
                        width += rightChildren[i].DesiredSize.Width;
                    }
                    else
                    {
                        canAdd = false;
                        rightChildren[i].Measure(zero);
                        lastRightIndex = i;
                        lastLeftIndex = 0;
                    }
                }
                else
                {
                    rightChildren[i].Measure(zero);
                }
            }

            // Left children
            for (int i = 0; i < leftChildren.Count; i++)
            {
                if (canAdd)
                {
                    leftChildren[i].Measure(infinity);
                    height = Math.Max(leftChildren[i].DesiredSize.Height, height);
                    if (width + leftChildren[i].DesiredSize.Width <= availableSize.Width)
                    {
                        width += leftChildren[i].DesiredSize.Width;
                    }
                    else
                    {
                        canAdd = false;
                        leftChildren[i].Measure(zero);
                        lastLeftIndex = i;
                        
                    }
                }
                else
                {
                    leftChildren[i].Measure(zero);
                }
            }

            // Collapse other children
            for (int i = 0; i < otherChildren.Count; i++)
            {
                otherChildren[i].Measure(zero);
            }

            return new Size(width, height);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class. 
        /// </summary>
        /// <returns>
        /// The actual size used.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            Rect zero = new Rect(0, 0, 0, 0);

            // Right shift
            double rightShift = 0;           
            // Arrange right
            for (int i = rightChildren.Count - 1; i >= 0; i--)
            {
                if (lastRightIndex > i)
                {
                    rightShift += rightChildren[i].DesiredSize.Width;
                    rightChildren[i].Arrange(new Rect(finalSize.Width - rightShift, 0, rightChildren[i].DesiredSize.Width, finalSize.Height));
                }
                else rightChildren[i].Arrange(zero);
            }
            
            // Left shift
            double leftShift = 0;
            // Arrange left
            for (int i = 0; i < leftChildren.Count; i++)
            {
                if (i < lastLeftIndex)
                {
                    leftChildren[i].Arrange(new Rect(leftShift, 0, leftChildren[i].DesiredSize.Width, finalSize.Height));
                    leftShift += leftChildren[i].DesiredSize.Width;
                }
                else leftChildren[i].Arrange(zero);
            }

            // Arrange other
            for (int i = 0; i < otherChildren.Count; i++)
            {
                otherChildren[i].Arrange(zero);
            }
            
            return finalSize;
        }        

        #endregion
    }
}
