#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// Represents panel to layout ribbon controls in toolbar style
    /// </summary>
    [ContentProperty("Children")]
    public class RibbonToolBarTray : Panel
    {
        #region Fields

        int[][] condensedOrder;

        #endregion

        #region Properties

        #region Condensed Order

        /// <summary>
        /// Gets or sets order of elements in condensed state
        /// (for example, 0,1,2;3,4;5,6)
        /// </summary>
        public string CondensedOrder
        {
            get { return (string)GetValue(CondensedOrderProperty); }
            set { SetValue(CondensedOrderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CondensedOrder.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CondensedOrderProperty =
            DependencyProperty.Register("CondensedOrder", typeof(string), typeof(RibbonToolBarTray), new FrameworkPropertyMetadata(null, 
                FrameworkPropertyMetadataOptions.AffectsMeasure | 
                FrameworkPropertyMetadataOptions.AffectsArrange | 
                FrameworkPropertyMetadataOptions.AffectsRender, OnCondensedOrderPropertyChanged));

        static void OnCondensedOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonToolBarTray tray = (RibbonToolBarTray)d;
            string[] rows = ((string)e.NewValue).Split(';');
            tray.condensedOrder = new int[3][];
            bool errorOccured = false;
            for (int i = 0; i < 3; i++)
            {
                if (i + 1 > rows.Length) { tray.condensedOrder[i] = new int[0]; continue; }
                tray.condensedOrder[i] = rows[i].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => { 
                        int result = 0; 
                        errorOccured = errorOccured || !Int32.TryParse(x, out result); 
                        return result; }).ToArray();
            }
            System.Diagnostics.Debug.WriteLineIf(errorOccured, "The property CondensedOrder has incorrect formatted value " + e.NewValue);
        }

        #endregion

        #region IsCondensed

        // TODO: add behavior to toolbar tray corresponds current size of the ribbon groupbox

        /// <summary>
        /// Gets or sets whether the tray is condensed state 
        /// (i.e. three lines layout)
        /// </summary>
        public bool IsCondensed
        {
            get { return (bool)GetValue(IsCondensedProperty); }
            set { SetValue(IsCondensedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCondensed. 
        /// This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty IsCondensedProperty =
            DependencyProperty.Register("IsCondensed", typeof(bool), typeof(RibbonToolBarTray), new FrameworkPropertyMetadata(false, 
                FrameworkPropertyMetadataOptions.AffectsMeasure | 
                FrameworkPropertyMetadataOptions.AffectsArrange | 
                FrameworkPropertyMetadataOptions.AffectsRender));
        

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBarTray()
        {
            
        }

        #endregion

        #region Layout

        /// <summary>
        /// When overridden in a derived class, measures the size 
        /// in layout required for child elements and determines a 
        /// size for the System.Windows.FrameworkElement-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements</param>
        /// <returns>The size that this element determines</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureChildren();


            if (IsCondensed && CondensedOrder == null)
            {
                // if condensed order is not present, we make automatic order
                condensedOrder = new int[3][] { new int[0], new int[0], new int[0] };

                double totalWidth = GetChildrenWidth();
                double threshold = totalWidth / 3.0;

                double x = 0;
                bool hasStretchable = false;
                int nextStartIndex = 0;
                int currentRow = 0;
                for (int i = 0; i < Children.Count; i++)
                {
                    Size desiredSize = Children[i].DesiredSize;
                    if (x + desiredSize.Width > threshold)
                    {
                        // Break to next row
                        x = 0;

                        // If the current row has a stretchable control 
                        // we split before the current control to make the next row larger
                        if (hasStretchable)
                        {
                            hasStretchable = false;
                            condensedOrder[currentRow] = MakeArray(nextStartIndex, i - 1);
                            nextStartIndex = i;
                            currentRow++;
                            if (currentRow == 2)
                            {
                                condensedOrder[currentRow] = MakeArray(nextStartIndex, Children.Count - 1);
                                break;
                            }
                        }
                        else
                        {
                            condensedOrder[currentRow] = MakeArray(nextStartIndex, i);
                            nextStartIndex = i + 1;
                            currentRow++;
                            if (currentRow == 2)
                            {
                                condensedOrder[currentRow] = MakeArray(nextStartIndex, Children.Count - 1);
                                break;
                            }
                            continue;
                        }
                    }

                    x += desiredSize.Width;
                    if ((Children[i] as FrameworkElement).HorizontalAlignment == HorizontalAlignment.Stretch)
                        hasStretchable = true;
                }
                
            }

            if (IsCondensed)
            {
                double maxWidth;
                double[] widths;
                CalculateCondensedWidths(condensedOrder, out maxWidth, out widths);
                return new Size(maxWidth, Double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Height);
            }
            else
            {
                int breakIndex;
                double firstPartWidth;
                double maxWidth;
                FindWhereWeCanSplitUncondensed(out breakIndex, out firstPartWidth, out maxWidth);

                return new Size(maxWidth, Double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Height);
            }
        }
                
        // Summ all width of all children
        double GetChildrenWidth()
        {
            double width = 0;
            foreach (UIElement element in InternalChildren) width += element.DesiredSize.Width;
            return width;
        }

        // Measures children 
        void MeasureChildren()
        {
            Size size = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            foreach (FrameworkElement element in Children)
            {
                if (element.HorizontalAlignment == HorizontalAlignment.Stretch)
                    element.Measure(new Size(element.MinWidth, Double.PositiveInfinity));
                else element.Measure(size);
            }
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines
        /// a size for a System.Windows.FrameworkElement derived class
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this 
        /// element should use to arrange itself and its children</param>
        /// <returns>The actual size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0) return finalSize;
            
            return IsCondensed ? 
                ArrangeCondensed(condensedOrder, finalSize) : 
                ArrangeUncondensed(finalSize);
        }

        static int[] MakeArray(int from, int to)
        {
            int[] array = new int[to - from + 1];
            for (int i = from; i <= to; i++)
            {
                array[i - from] = i;
            }
            return array;
        }

        Size ArrangeUncondensed(Size finalSize)
        {
            if (Children.Count == 0) return finalSize;

            double childrenHeight = Children[0].DesiredSize.Height;
            double totalWidth = GetChildrenWidth();
            
            int breakIndex;
            double firstPartWidth;
            double maxWidth;
            FindWhereWeCanSplitUncondensed(out breakIndex, out firstPartWidth, out maxWidth);


            // Arranging            
            double space = (finalSize.Height - (childrenHeight * 2.0)) / 3.0;
            double y = space;
            double x = 0;
            bool stillSeekStretching = true;
            for (int i = 0; i < Children.Count; i++)
            {
                if (i == breakIndex)
                {
                    x = 0;
                    stillSeekStretching = true;
                    y += space + childrenHeight;
                }

                FrameworkElement element = Children[i] as FrameworkElement;
                if (element == null) continue;
                if (stillSeekStretching && element.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    stillSeekStretching = false;
                    double w = maxWidth - (i >= breakIndex ? totalWidth - firstPartWidth : firstPartWidth) + element.DesiredSize.Width;
                    element.Arrange(new Rect(x, y, w, childrenHeight));
                    x += w;
                }
                else
                {
                    element.Arrange(new Rect(x, y, element.DesiredSize.Width, childrenHeight));
                    x += element.DesiredSize.Width;
                }
                
            }

            return finalSize;
        }

        // Calculates where we can split and widths
        void FindWhereWeCanSplitUncondensed(out int breakIndex, out double firstPartWidth, out double maxWidth)
        {
            double totalWidth = GetChildrenWidth();
            double threshold = totalWidth / 2.0;

            // Find where we can split/ break
            double x = 0;
            bool hasStretchable = false;
            breakIndex = 0;
            firstPartWidth = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                Size desiredSize = Children[i].DesiredSize;
                if ((x + desiredSize.Width > threshold) && (i != 0))
                {
                    if (hasStretchable)
                    {
                        breakIndex = i;
                        firstPartWidth = x;
                    }
                    else
                    {
                        breakIndex = i + 1;
                        firstPartWidth = x + desiredSize.Width;
                    }
                    break;
                }

                x += desiredSize.Width;
                if ((Children[i] as FrameworkElement).HorizontalAlignment == HorizontalAlignment.Stretch)
                    hasStretchable = true;
            }
            maxWidth = Math.Max(firstPartWidth, totalWidth - firstPartWidth);
        }

        // Arranges children using the given condensedOrder
        Size ArrangeCondensed(int[][] order, Size finalSize)
        {
            if (Children.Count == 0) return finalSize;
            double childrenHeight = Children[0].DesiredSize.Height;

            double maxWidth;
            double[] widths;
            CalculateCondensedWidths(order, out maxWidth, out widths);

            // Removes skipped items
            if (order[0].Length + order[1].Length + order[2].Length < Children.Count)
            {
                // TODO: fix this dirty way to hide skipped controls in toolbar tray
                Rect empty = new Rect(-10000, -10000, 0.01, 0.01);
                foreach (UIElement item in Children) item.Arrange(empty);
            }

            // Arranging            
            double space = (finalSize.Height - (childrenHeight * 3.0)) / 4.0;
            double y = space;
            for (int i = 0; i < 3; i++)
            {
                double x = 0;
                bool stillSeekStretching = true;
                for (int j = 0; j < order[i].Length; j++)
                {            
                    FrameworkElement element = Children[order[i][j]] as FrameworkElement;
                    if (element == null) continue;
                    if (stillSeekStretching && element.HorizontalAlignment == HorizontalAlignment.Stretch)
                    {
                        // We have found a stretchable item. 
                        // Let the stretchable item take all 
                        // available space in the current row
                        stillSeekStretching = false;
                        double w = maxWidth - widths[i] + element.DesiredSize.Width;
                        element.Arrange(new Rect(x, y, w, childrenHeight));
                        x += w;
                    }
                    else
                    {
                        element.Arrange(new Rect(x, y, element.DesiredSize.Width, childrenHeight));
                        x += element.DesiredSize.Width;
                    }
                }                
                y += childrenHeight + space;
            }

            return new Size(maxWidth, finalSize.Height);
        }

        void CalculateCondensedWidths(int[][] order, out double maxWidth, out double[] widths)
        {
            // Calculate max width
            maxWidth = 0;
            widths = new double[3];
            for (int i = 0; i < 3; i++)
            {
                // Calculate max width
                double width = 0;
                for (int j = 0; j < order[i].Length; j++)
                {
                    width += Children[order[i][j]].DesiredSize.Width;
                }
                widths[i] = width;
                maxWidth = Math.Max(maxWidth, width);
            }
        }

        #endregion
    }
}
