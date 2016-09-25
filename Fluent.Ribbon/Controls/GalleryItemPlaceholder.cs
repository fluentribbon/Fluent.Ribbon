using System.Windows;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    /// <summary>
    /// Represents internal class to use it in 
    /// GalleryPanel as placeholder for GalleryItems
    /// </summary>
    internal class GalleryItemPlaceholder : UIElement
    {
        #region Properties

        /// <summary>
        /// Gets the target of the placeholder
        /// </summary>
        public UIElement Target { get; }

        public Size ArrangedSize { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Target</param>
        public GalleryItemPlaceholder(UIElement target)
        {
            this.Target = target;
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, measures the size in layout 
        /// required for child elements and determines a size for the derived class. 
        /// </summary>
        /// <returns>
        /// The size that this element determines it needs during layout, 
        /// based on its calculations of child element sizes.
        /// </returns>
        /// <param name="availableSize">The available size that this element can 
        /// give to child elements. Infinity can be specified as a value to 
        /// indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureCore(Size availableSize)
        {
            this.Target.Measure(availableSize);
            return this.Target.DesiredSize;
        }

        /// <summary>
        /// Defines the template for WPF core-level arrange layout definition. 
        /// </summary>
        /// <param name="finalRect">
        /// The final area within the parent that element should use to 
        /// arrange itself and its child elements.</param>
        protected override void ArrangeCore(Rect finalRect)
        {
            base.ArrangeCore(finalRect);

            // Remember arranged size to arrange 
            // targets in GalleryPanel lately
            this.ArrangedSize = finalRect.Size;
        }

        #endregion

        #region Debug

        /* FOR DEGUG */
        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 1), new Rect(RenderSize));
        //}

        #endregion
    }
}
