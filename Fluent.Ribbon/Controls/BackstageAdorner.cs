// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents adorner for Backstage
    /// </summary>
    internal class BackstageAdorner : Adorner
    {
        // Content of Backstage
        private readonly UIElement backstageContent;

        // Collection of visual children
        private readonly VisualCollection visualChildren;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackstageAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">Adorned element</param>
        /// <param name="backstage">Backstage</param>
        public BackstageAdorner(FrameworkElement adornedElement, Backstage backstage)
            : base(adornedElement)
        {
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);

            this.Backstage = backstage;
            this.backstageContent = this.Backstage.Content;

            this.visualChildren = new VisualCollection(this)
                {
                    this.backstageContent
                };

            // TODO: fix it! (below ugly workaround) in measureoverride we cannot get RenderSize, we must use DesiredSize
            // Syncronize with visual size
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        /// <summary>
        /// Gets the <see cref="Fluent.Backstage"/>.
        /// </summary>
        public Backstage Backstage { get; }

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

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.backstageContent.Arrange(new Rect(0, 0, finalSize.Width, Math.Max(0, finalSize.Height)));
            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            // TODO: fix it! (below ugly workaround) in measureoverride we cannot get RenderSize, we must use DesiredSize
            this.backstageContent.Measure(new Size(this.AdornedElement.RenderSize.Width, Math.Max(0, this.AdornedElement.RenderSize.Height)));
            return this.AdornedElement.RenderSize;
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount => this.visualChildren.Count;

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            return this.visualChildren[index];
        }
    }
}