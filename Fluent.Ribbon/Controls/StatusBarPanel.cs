// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Fluent.Internal;

    /// <summary>
    /// Represents panel for status bar
    /// </summary>
    public class StatusBarPanel : Panel
    {
        #region Attributes

        private readonly List<UIElement> leftChildren = new List<UIElement>();
        private readonly List<UIElement> rightChildren = new List<UIElement>();
        private readonly List<UIElement> otherChildren = new List<UIElement>();

        private int lastRightIndex;
        private int lastLeftIndex;

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            // Sort children
            this.leftChildren.Clear();
            this.rightChildren.Clear();
            this.otherChildren.Clear();

            for (var i = 0; i < this.InternalChildren.Count; i++)
            {
                if (this.InternalChildren[i] is FrameworkElement child)
                {
                    if (child.HorizontalAlignment == HorizontalAlignment.Left)
                    {
                        this.leftChildren.Add(child);
                    }
                    else if (child.HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        this.rightChildren.Add(child);
                    }
                    else
                    {
                        this.otherChildren.Add(child);
                    }
                }
            }

            this.lastRightIndex = this.rightChildren.Count;
            this.lastLeftIndex = this.leftChildren.Count;

            // Measure children
            var zero = new Size(0, 0);
            double width = 0;
            double height = 0;
            var canAdd = true;

            // Right children
            for (var i = 0; i < this.rightChildren.Count; i++)
            {
                if (canAdd)
                {
                    this.rightChildren[i].Measure(SizeConstants.Infinite);
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
            for (var i = 0; i < this.leftChildren.Count; i++)
            {
                if (canAdd)
                {
                    this.leftChildren[i].Measure(SizeConstants.Infinite);
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
            foreach (var otherChild in this.otherChildren)
            {
                otherChild.Measure(zero);
            }

            return new Size(width, height);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            var zero = new Rect(0, 0, 0, 0);

            // Right shift
            double rightShift = 0;

            // Arrange right
            for (var i = this.rightChildren.Count - 1; i >= 0; i--)
            {
                if (this.lastRightIndex > i)
                {
                    rightShift += this.rightChildren[i].DesiredSize.Width;
                    this.rightChildren[i].Arrange(new Rect(finalSize.Width - rightShift, 0, this.rightChildren[i].DesiredSize.Width, finalSize.Height));
                }
                else
                {
                    this.rightChildren[i].Arrange(zero);
                }
            }

            // Left shift
            double leftShift = 0;

            // Arrange left
            for (var i = 0; i < this.leftChildren.Count; i++)
            {
                if (i < this.lastLeftIndex)
                {
                    this.leftChildren[i].Arrange(new Rect(leftShift, 0, this.leftChildren[i].DesiredSize.Width, finalSize.Height));
                    leftShift += this.leftChildren[i].DesiredSize.Width;
                }
                else
                {
                    this.leftChildren[i].Arrange(zero);
                }
            }

            // Arrange other
            foreach (var otherChild in this.otherChildren)
            {
                otherChild.Arrange(zero);
            }

            return finalSize;
        }

        #endregion
    }
}