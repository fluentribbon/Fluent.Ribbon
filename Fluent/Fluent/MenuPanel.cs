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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents panel for menu items an over stuff
    /// </summary>
    public class MenuPanel:Panel
    {
        #region Fields
                
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets minimal width for resizing menu panel
        /// </summary>
        public double ResizeMinWidth { get; set; }

        /// <summary>
        /// Gets or sets minimal height for resizing menu panel
        /// </summary>
        public double ResizeMinHeight { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MenuPanel()
        {
            Loaded += OnMenuLoaded;
        }

        void OnMenuLoaded(object sender, RoutedEventArgs e)
        {
            UpdateMenuSizes();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Updates menu sizes
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1800")]
        internal void UpdateMenuSizes()
        {
            if (Children.Count > 0)
            {
                Width = double.NaN;
                //Height = double.NaN;
                double minWidth = 0;
                double minHeight = 0;
                double maxMenuWidth = 0;
                for (int i = 0; i < Children.Count; i++)
                {
                    FrameworkElement element = Children[i] as FrameworkElement;
                    if (element != null)
                    {
                        if ((element is MenuItem) || (element is Separator))
                        {
                            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            if (element is MenuItem)
                            {
                                minWidth = Math.Max(minWidth, element.MinWidth);
                                //maxMenuWidth = Math.Max(maxMenuWidth, element.DesiredSize.Width);
                            }
                            if (!double.IsPositiveInfinity(element.DesiredSize.Width)) maxMenuWidth = Math.Max(maxMenuWidth, element.DesiredSize.Width);
                            minHeight += element.DesiredSize.Height;
                        }
                        else
                        {
                            if (element is MenuPanel) element.Width = double.NaN;
                            minWidth = Math.Max(minWidth, element.MinWidth);
                            minHeight += element.MinHeight;
                        }
                    }
                }
                ResizeMinWidth = Math.Max(0, minWidth);
                ResizeMinHeight = Math.Max(0, minHeight);
                if (ResizeMinHeight!=0) MinHeight = ResizeMinHeight;
                if (ResizeMinWidth != 0) Width = Math.Max(maxMenuWidth, ResizeMinWidth);
                if (ResizeMinWidth < maxMenuWidth) ResizeMinWidth = maxMenuWidth;
                if (VisualTreeHelper.GetParent(this) is MenuPanel) Width = double.NaN;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when the VisualCollection of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The Visual that was added to the collection</param>
        /// <param name="visualRemoved">The Visual that was removed from the collection</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (IsLoaded)
            {
                UpdateMenuSizes();                
                //InvalidateUpdateMenuSizes();
            }
           /* FrameworkElement added = visualAdded as FrameworkElement;
            FrameworkElement removed = visualRemoved as FrameworkElement;
            if (added != null)
            {
                added.SizeChanged += OnItemSizeChanged;
                added.IsVisibleChanged += OnItemVisibilityChanged;
            }
            if (removed != null)
            {
                added.SizeChanged -= OnItemSizeChanged;
                removed.IsVisibleChanged -= OnItemVisibilityChanged;
            }*/
        }

        private void OnItemSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateUpdateMenuSizes();
        }

        void OnItemVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InvalidateUpdateMenuSizes();
            /*InvalidateMeasure();
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate { UpdateMenuSizes(); UpdateLayout(); }));*/
        }

        private bool isInvalidated = false;
        private void InvalidateUpdateMenuSizes()
        {
            if (isInvalidated) return;
            isInvalidated = true;
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,(ThreadStart)(()=>
            {
                isInvalidated = false;
                UpdateMenuSizes();
                UpdateLayout();
            }));
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout 
        /// required for child elements and determines a size for the System.Windows.FrameworkElement-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value 
        /// to indicate that the element will size to whatever content is available.</param>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            double totalHeight = 0;
            double maxWidth = 0;
            if (!double.IsPositiveInfinity(availableSize.Width)) maxWidth = availableSize.Width;
            List<UIElement> nonItemsElements = new List<UIElement>();
            foreach (object child in InternalChildren)
            {
                MenuItem item = child as MenuItem;
                if((item!=null)&&(item.Visibility!=Visibility.Collapsed))
                {
                    item.Measure(availableSize);
                    totalHeight += item.DesiredSize.Height;
                    maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
                }
                else
                {
                    Separator separator = child as Separator;
                    
                    if ((separator!=null)&&(separator.Visibility!=Visibility.Collapsed))
                    {
                        separator.Measure(availableSize);
                        totalHeight += separator.DesiredSize.Height;
                        maxWidth = Math.Max(maxWidth, separator.DesiredSize.Width);
                    }
                    else
                    {
                        UIElement uiElement = child as UIElement;
                        if (uiElement != null)
                        {
                            nonItemsElements.Add(uiElement);
                        }
                    }
                }
            }

            if ((!double.IsPositiveInfinity(availableSize.Height)))
            {
                if (totalHeight < availableSize.Height)
                {
                    double deltaHeight = (availableSize.Height - totalHeight)/nonItemsElements.Count;
                    foreach (FrameworkElement item in nonItemsElements)
                    {
                        if (item.Visibility != Visibility.Collapsed)
                        {
                            item.Measure(new Size(availableSize.Width, deltaHeight));
                            maxWidth = Math.Max(maxWidth, Math.Max(item.DesiredSize.Width, (item).MinWidth));
                            totalHeight += Math.Max(item.DesiredSize.Height, (item).MinHeight);
                        }
                    }
                }
                else
                {
                    foreach (FrameworkElement item in nonItemsElements)
                    {
                        if (item.Visibility != Visibility.Collapsed)
                        {
                            item.Measure(new Size());
                            maxWidth = Math.Max(maxWidth, Math.Max(item.DesiredSize.Width, item.MinWidth));
                            totalHeight += Math.Max(item.DesiredSize.Height, item.MinHeight);
                        }
                    }
                }
            }
            else
            {
                foreach (FrameworkElement item in nonItemsElements)
                {
                    if (item.Visibility != Visibility.Collapsed)
                    {
                        item.Measure(availableSize);
                        maxWidth = Math.Max(maxWidth, Math.Max(item.DesiredSize.Width, item.MinWidth));
                        totalHeight += Math.Max(item.DesiredSize.Height, item.MinHeight);
                    }
                }
            }
            if (maxWidth < ResizeMinWidth) maxWidth = ResizeMinWidth;
            if (maxWidth > availableSize.Width) maxWidth = availableSize.Width;
            if (totalHeight < ResizeMinHeight) totalHeight = ResizeMinHeight;
            return new Size(maxWidth,totalHeight);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and 
        /// determines a size for a System.Windows.FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {            
            double totalHeight = 0;            
            List<UIElement> nonItemsElements = new List<UIElement>();
            foreach (var child in InternalChildren)
            {
                MenuItem item = child as MenuItem;
                if ((item != null)&&(item.Visibility!=Visibility.Collapsed))
                {
                    totalHeight += item.DesiredSize.Height;
                }
                else
                {
                    Separator separator = child as Separator;
                    if ((separator != null) && (separator.Visibility != Visibility.Collapsed))
                    {
                        totalHeight += separator.DesiredSize.Height;
                    }
                    else
                    {
                        /*MenuPanel panel = child as MenuPanel;
                        if(panel!=null)
                        {
                            totalHeight += panel.DesiredSize.Height;
                        }
                        else */nonItemsElements.Add(child as UIElement);
                    }
                }
            }

            double y = 0;
            double deltaHeight = Math.Max(0,(finalSize.Height - totalHeight)/nonItemsElements.Count);
           /* Debug.WriteLine("deltaHeight = " + deltaHeight);
            Debug.WriteLine("finalSize.Height = " + finalSize.Height);
            Debug.WriteLine("totalHeight = " + totalHeight);*/
            foreach (var child in InternalChildren)
            {
                UIElement element=(child as UIElement);
                if ((element != null)&&(element.Visibility!=Visibility.Collapsed))
                {                    
                    double height = deltaHeight;
                    if ((element is MenuItem)||(element is Separator)||(element is MenuPanel)) height = element.DesiredSize.Height;
                    element.Arrange(new Rect(0, y, finalSize.Width, height));
                    y += height;
                }
            }

            return finalSize;            
        }

        #endregion
    }
}
