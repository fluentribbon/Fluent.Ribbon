// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Fluent.Converters;
    using Fluent.Internal;

    /// <summary>
    /// Represents adorner for Backstage
    /// </summary>
    internal class BackstageAdorner : Adorner
    {
        // Content of Backstage
        private readonly UIElement backstageContent;

        // Collection of visual children
        private readonly VisualCollection visualChildren;
        private readonly Rectangle background;
        private readonly BackstageTabControl backstageTabControl;

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
            this.backstageTabControl = this.backstageContent as BackstageTabControl 
                                       ?? UIHelper.FindVisualChild<BackstageTabControl>(this.backstageContent);

            this.background = new Rectangle();

            if (this.backstageTabControl != null)
            {
                BindingOperations.SetBinding(this.background, Shape.FillProperty, new Binding
                                                                                  {
                                                                                      Path = new PropertyPath(Control.BackgroundProperty),
                                                                                      Source = this.backstageTabControl
                                                                                  });

                BindingOperations.SetBinding(this.background, MarginProperty, new Binding
                                                                                  {
                                                                                      Path = new PropertyPath(BackstageTabControl.SelectedContentMarginProperty),
                                                                                      Source = this.backstageTabControl
                                                                                  });
            }
            else
            {
                this.background.SetResourceReference(Shape.FillProperty, "WhiteBrush");
            }

            this.visualChildren = new VisualCollection(this)
                {
                    this.background,
                    this.backstageContent
                };
        }

        /// <summary>
        /// Gets the <see cref="Fluent.Backstage"/>.
        /// </summary>
        public Backstage Backstage { get; }

        public void Clear()
        {
            BindingOperations.ClearAllBindings(this.background);

            this.visualChildren.Clear();
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            // Arrange background and compensate margin used by animation
            this.background.Arrange(new Rect(this.Margin.Left * -1, 0, Math.Max(0, finalSize.Width), Math.Max(0, finalSize.Height)));

            this.backstageContent.Arrange(new Rect(0, 0, Math.Max(0, finalSize.Width), Math.Max(0, finalSize.Height)));

            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            var size = new Size(Math.Max(0, this.AdornedElement.RenderSize.Width), Math.Max(0, this.AdornedElement.RenderSize.Height));

            this.background.Measure(size);
            this.backstageContent.Measure(size);

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