#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents adorner for Backstage
    /// </summary>
    internal class BackstageAdorner : Adorner
    {
        #region Fields

        // Backstage
        private readonly Backstage backstage;

        // Content of Backstage
        private readonly UIElement backstageContent;

        // Collection of visual children
        private readonly VisualCollection visualChildren;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adornedElement">Adorned element</param>
        /// <param name="backstage">Backstage</param>
        public BackstageAdorner(FrameworkElement adornedElement, Backstage backstage)
            : base(adornedElement)
        {
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);

            this.backstage = backstage;
            this.backstageContent = this.backstage.Content;

            this.visualChildren = new VisualCollection(this) 
                {
                    this.backstageContent
                };

            // TODO: fix it! (below ugly workaround) in measureoverride we cannot get RenderSize, we must use DesiredSize
            // Syncronize with visual size
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += this.CompositionTargetRendering;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= this.CompositionTargetRendering;
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (this.RenderSize != this.AdornedElement.RenderSize)
            {
                this.InvalidateMeasure();
            }
        }

        public void Clear()
        {
            this.visualChildren.Clear();
        }

        #endregion

        // Adorner offset from top of window
        private double GetTopOffset()
        {
            var topOffset = this.backstage.TranslatePoint(new Point(0, this.backstage.ActualHeight), this.AdornedElement).Y;
            
            return topOffset;
        }

        #region Layout & Visual Children

        /// <summary>
        /// Positions child elements and determines
        /// a size for the control
        /// </summary>
        /// <param name="finalSize">The final area within the parent 
        /// that this element should use to arrange 
        /// itself and its children</param>
        /// <returns>The actual size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var topOffset = this.GetTopOffset();

            this.backstageContent.Arrange(new Rect(0, topOffset, finalSize.Width, Math.Max(0, finalSize.Height - topOffset)));
            return finalSize;
        }

        /// <summary>
        /// Measures KeyTips
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var topOffset = this.GetTopOffset();

            // TODO: fix it! (below ugly workaround) in measureoverride we cannot get RenderSize, we must use DesiredSize
            this.backstageContent.Measure(new Size(this.AdornedElement.RenderSize.Width, Math.Max(0, this.AdornedElement.RenderSize.Height - topOffset)));
            return this.AdornedElement.RenderSize;
        }

        /// <summary>
        /// Gets visual children count
        /// </summary>
        protected override int VisualChildrenCount { get { return this.visualChildren.Count; } }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        protected override Visual GetVisualChild(int index) { return this.visualChildren[index]; }

        #endregion
    }
}