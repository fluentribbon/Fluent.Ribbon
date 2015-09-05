﻿using System;
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
        protected override Size MeasureOverride(Size availableSize)
        {
            // Sort children
            this.leftChildren.Clear();
            this.rightChildren.Clear();
            this.otherChildren.Clear();
            for (int i = 0; i < this.InternalChildren.Count; i++)
            {
                FrameworkElement child = this.InternalChildren[i] as FrameworkElement;
                if (child != null)
                {
                    if (child.HorizontalAlignment == HorizontalAlignment.Left)
                        this.leftChildren.Add(child);
                    else if (child.HorizontalAlignment == HorizontalAlignment.Right)
                        this.rightChildren.Add(child);
                    else
                        this.otherChildren.Add(child);
                }
            }

            this.lastRightIndex = this.rightChildren.Count;
            this.lastLeftIndex = this.leftChildren.Count;

            // Measure children
            Size infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Size zero = new Size(0, 0);
            double width = 0;
            double height = 0;
            bool canAdd = true;
            // Right children
            for (int i = 0; i < this.rightChildren.Count; i++)
            {
                if (canAdd)
                {
                    this.rightChildren[i].Measure(infinity);
                    height = Math.Max(this.rightChildren[i].DesiredSize.Height, height);
                    if (width + this.rightChildren[i].DesiredSize.Width <= availableSize.Width)
                    {
                        width += this.rightChildren[i].DesiredSize.Width;
                    }
                    else
                    {
                        canAdd = false;
                        this.rightChildren[i].Measure(zero);
                        this.lastRightIndex = i;
                        this.lastLeftIndex = 0;
                    }
                }
                else
                {
                    this.rightChildren[i].Measure(zero);
                }
            }

            // Left children
            for (int i = 0; i < this.leftChildren.Count; i++)
            {
                if (canAdd)
                {
                    this.leftChildren[i].Measure(infinity);
                    height = Math.Max(this.leftChildren[i].DesiredSize.Height, height);
                    if (width + this.leftChildren[i].DesiredSize.Width <= availableSize.Width)
                    {
                        width += this.leftChildren[i].DesiredSize.Width;
                    }
                    else
                    {
                        canAdd = false;
                        this.leftChildren[i].Measure(zero);
                        this.lastLeftIndex = i;
                        
                    }
                }
                else
                {
                    this.leftChildren[i].Measure(zero);
                }
            }

            // Collapse other children
            for (int i = 0; i < this.otherChildren.Count; i++)
            {
                this.otherChildren[i].Measure(zero);
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
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect zero = new Rect(0, 0, 0, 0);

            // Right shift
            double rightShift = 0;           
            // Arrange right
            for (int i = this.rightChildren.Count - 1; i >= 0; i--)
            {
                if (this.lastRightIndex > i)
                {
                    rightShift += this.rightChildren[i].DesiredSize.Width;
                    this.rightChildren[i].Arrange(new Rect(finalSize.Width - rightShift, 0, this.rightChildren[i].DesiredSize.Width, finalSize.Height));
                }
                else
                    this.rightChildren[i].Arrange(zero);
            }
            
            // Left shift
            double leftShift = 0;
            // Arrange left
            for (int i = 0; i < this.leftChildren.Count; i++)
            {
                if (i < this.lastLeftIndex)
                {
                    this.leftChildren[i].Arrange(new Rect(leftShift, 0, this.leftChildren[i].DesiredSize.Width, finalSize.Height));
                    leftShift += this.leftChildren[i].DesiredSize.Width;
                }
                else
                    this.leftChildren[i].Arrange(zero);
            }

            // Arrange other
            for (int i = 0; i < this.otherChildren.Count; i++)
            {
                this.otherChildren[i].Arrange(zero);
            }
            
            return finalSize;
        }        

        #endregion
    }
}
