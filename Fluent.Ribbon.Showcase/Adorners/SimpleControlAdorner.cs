namespace FluentTest.Adorners
{
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class SimpleControlAdorner : Adorner
    {
        private FrameworkElement child;

        public SimpleControlAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "There is only one visual child.");
            }

            return this.child;
        }

        public FrameworkElement Child
        {
            get => this.child;

            set
            {
                if (this.child != null)
                {
                    this.RemoveVisualChild(this.child);
                }

                this.child = value;

                if (this.child != null)
                {
                    this.AddVisualChild(this.child);
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(new Point(0, 0), finalSize));
            return new Size(this.child.ActualWidth, this.child.ActualHeight);
        }
    }
}
