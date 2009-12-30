using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represent panel for group box panel
    /// </summary>
    [ContentProperty("Children")]
    public class RibbonToolBarControlGroup : RibbonControl
    {
        #region Fields

        // User defined children
        readonly ObservableCollection<FrameworkElement> children = new ObservableCollection<FrameworkElement>();
        // User defined layout definitions
        readonly ObservableCollection<RibbonToolBarLayoutDefinition> layoutDefinitions =
            new ObservableCollection<RibbonToolBarLayoutDefinition>();

        // Actual children
        readonly List<FrameworkElement> actualChildren = new List<FrameworkElement>();
        // Designates that rebuilding of visual & logical children is required
        bool rebuildVisualAndLogicalChildren = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<FrameworkElement> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets particular rules  for layout in this group box panel
        /// </summary>
        public ObservableCollection<RibbonToolBarLayoutDefinition> LayoutDefinitions
        {
            get { return layoutDefinitions; }
        }

        #endregion

        #region Logical & Visual Tree

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                if (layoutDefinitions.Count == 0) return children.Count;
                if (rebuildVisualAndLogicalChildren) UpdateLayout();
                return actualChildren.Count;
            }
        }

        /// <summary>
        /// Overrides System.Windows.Media.Visual.GetVisualChild(System.Int32),
        /// and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested 
        /// child element in the collection</param>
        /// <returns>The requested child element. This should not return null; 
        /// if the provided index is out of range, an exception is thrown</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (layoutDefinitions.Count == 0) return children[index];
            if (rebuildVisualAndLogicalChildren) UpdateLayout();
            return actualChildren[index];
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (layoutDefinitions.Count == 0) return children.GetEnumerator();
                if (rebuildVisualAndLogicalChildren) UpdateLayout();
                return actualChildren.GetEnumerator();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBarControlGroup()
        {
            children.CollectionChanged += OnChildrenCollectionChanged;
            layoutDefinitions.CollectionChanged += OnLayoutDefinitionsChanged;
        }

        void OnLayoutDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            rebuildVisualAndLogicalChildren = true;
            InvalidateMeasure();
        }

        void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Children have changed, reset layouts
            foreach (RibbonToolBarLayoutDefinition definition in layoutDefinitions)
            {
                definition.Invalidate();
            }
            rebuildVisualAndLogicalChildren = true;
            InvalidateMeasure();
        }

        #endregion

        #region Methods

        RibbonToolBarLayoutDefinition GetCurrentLayoutDefinition()
        {
            if (layoutDefinitions.Count == 0) return null;
            if (layoutDefinitions.Count == 1) return layoutDefinitions[0];

            foreach (RibbonToolBarLayoutDefinition definition in layoutDefinitions)
            {
                if (definition.Size == Size) return definition;
            }

            // TODO: try to find a better definition
            return layoutDefinitions[0];
        }

        #endregion

        #region Layout Overriding

        /// <summary>
        /// Measures all of the RibbonGroupBox, and resize them appropriately
        /// to fit within the available room
        /// </summary>
        /// <param name="availableSize">The available size that 
        /// this element can give to child elements.</param>
        /// <returns>The size that the panel determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            RibbonToolBarLayoutDefinition layoutDefinition = GetCurrentLayoutDefinition();
            
            // Rebuilding actual children (visual & logical)
            if (rebuildVisualAndLogicalChildren)
            {
                // Clear previous children
                foreach (FrameworkElement child in actualChildren)
                {
                    RemoveVisualChild(child);
                    RemoveLogicalChild(child);
                }
                actualChildren.Clear();

                if (layoutDefinition == null)
                {
                    foreach (FrameworkElement child in Children)
                    {
                        actualChildren.Add(child);
                        AddVisualChild(child);
                        AddLogicalChild(child);
                    }
                }
                rebuildVisualAndLogicalChildren = false;
            }

            if (layoutDefinition == null)
            {
                return WrapPanelLayuot(availableSize, true);
            }


            return availableSize;
        }

        

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines 
        /// a size for a System.Windows.FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this 
        /// element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            RibbonToolBarLayoutDefinition layoutDefinition = GetCurrentLayoutDefinition();
            if (layoutDefinition == null)
            {
                return WrapPanelLayuot(finalSize, false);
            }

            return finalSize;
        }


        #region Wrap Panel Layout

        /// <summary>
        /// Unified method for wrap panel logic
        /// </summary>
        /// <param name="availableSize">Available or final size</param>
        /// <param name="measure">Pass true if measure required; pass false if arrange required</param>
        /// <returns>Final size</returns>
        Size WrapPanelLayuot(Size availableSize, bool measure)
        {
            bool arrange = !measure;
            double availableHeight = Double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Height;

            double currentheight = 0;
            double columnWidth = 0;

            double resultWidth = 0;
            double resultHeight = 0;

            Size infinity = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            foreach (FrameworkElement child in children)
            {
                // Measuring
                if (measure) child.Measure(infinity);

                if (currentheight + child.DesiredSize.Height > availableHeight)
                {
                    // Move to the next column
                    resultHeight = Math.Max(resultHeight, currentheight);
                    resultWidth += columnWidth;
                    currentheight = 0;
                    columnWidth = 0;
                }

                // Arranging
                if (arrange) child.Arrange(new Rect(
                    resultWidth,
                    currentheight,
                    child.DesiredSize.Width,
                    child.DesiredSize.Height));

                columnWidth = Math.Max(columnWidth, child.DesiredSize.Width);
                currentheight += child.DesiredSize.Height;
            }

            return new Size(resultWidth + columnWidth, resultHeight);
        }

        #endregion

        #endregion
    }
}