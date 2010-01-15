using System;
using System.Linq;
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
    public class RibbonToolBar : RibbonControl
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
        public RibbonToolBar()
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

        #region Size Property Changing

        /// <summary>
        /// Handles size property changing
        /// </summary>
        /// <param name="previous">Previous value</param>
        /// <param name="current">Current value</param>
        protected override void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
            rebuildVisualAndLogicalChildren = true;
            InvalidateMeasure();
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
                    if (child is RibbonToolBarControlGroup)
                    {
                        RibbonToolBarControlGroup controlGroup = (RibbonToolBarControlGroup)child;
                        controlGroup.Items.Clear();
                    }
                    RemoveVisualChild(child);
                    RemoveLogicalChild(child);
                }
                actualChildren.Clear();
                cachedControlGroups.Clear();

                if (layoutDefinition == null)
                {
                    foreach (FrameworkElement child in Children)
                    {
                        actualChildren.Add(child);
                        AddVisualChild(child);
                        AddLogicalChild(child);
                    }
                }
                else
                {
                    // Add controls defined in layout definition in visual & logical children
                   /* foreach (RibbonToolBarRow row in layoutDefinition.Rows)
                    {
                        foreach (DependencyObject obj in row.Children)
                        {
                            if (obj is RibbonToolBarControlDefinition)
                            {
                                string name = ((RibbonToolBarControlDefinition) obj).Target;
                                FrameworkElement control = Children.FirstOrDefault(x => x.Name == name);
                                if (control != null)
                                {
                                    actualChildren.Add(control);
                                    AddVisualChild(control);
                                    AddLogicalChild(control);
                                }
                            }
                            if (obj is RibbonToolBarControlGroupDefinition)
                            {
                                foreach (RibbonToolBarControlDefinition child in ((RibbonToolBarControlGroupDefinition)obj).Children)
                                {
                                    string name = child.Target;
                                    FrameworkElement control = Children.FirstOrDefault(x => x.Name == name);
                                    if (control != null)
                                    {
                                        actualChildren.Add(control);
                                        AddVisualChild(control);
                                        AddLogicalChild(control);
                                    }  
                                }
                            }
                        }
                    }*/
                }
                
            }

            if (layoutDefinition == null)
            {
                return WrapPanelLayuot(availableSize, rebuildVisualAndLogicalChildren);
            }
            else
            {
                return CustomLayout(layoutDefinition, availableSize, true, rebuildVisualAndLogicalChildren);
            }
            
            rebuildVisualAndLogicalChildren = false;
        }

        FrameworkElement GetControl(RibbonToolBarControlDefinition controlDefinition)
        {
            string name = controlDefinition.Target;
            return Children.FirstOrDefault(x => x.Name == name);
        }

        Dictionary<object, RibbonToolBarControlGroup> cachedControlGroups = new Dictionary<object, RibbonToolBarControlGroup>();
        RibbonToolBarControlGroup GetControlGroup(RibbonToolBarControlGroupDefinition controlGroupDefinition)
        {
            RibbonToolBarControlGroup controlGroup = null;
            if (!cachedControlGroups.TryGetValue(controlGroupDefinition, out controlGroup))
            {
                controlGroup = new RibbonToolBarControlGroup();
                // Add items to the group
                foreach (RibbonToolBarControlDefinition child in controlGroupDefinition.Children)
                {
                    controlGroup.Items.Add(GetControl(child));
                }
                cachedControlGroups.Add(controlGroupDefinition, controlGroup);
            }
            return controlGroup;
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
            else
            {
                return CustomLayout(layoutDefinition, finalSize, false, false);
            }
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
        
        #region Custom Layout

        /// <summary>
        /// Layout logic for the given layout definition
        /// </summary>
        /// <param name="availableSize">Available or final size</param>
        /// <param name="measure">Pass true if measure required; pass false if arrange required</param>
        /// <returns>Final size</returns>
        Size CustomLayout(RibbonToolBarLayoutDefinition layoutDefinition, Size availableSize, bool measure, bool addchildren)
        {
            bool arrange = !measure;
            double availableHeight = Double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Height;

            int maxRowCountInColumn = 3;

            // TODO: how to calc row height??
            ////////////////
            //actualChildren[0].Measure(availableSize);
            double rowHeight = 22;// actualChildren[0].DesiredSize.Height;
            ////////////////

            // Calculate whitespace
            int rowCountInColumn = Math.Min(maxRowCountInColumn, layoutDefinition.Rows.Count);
            double whitespace = (availableHeight - ((double)rowCountInColumn * rowHeight)) / (double)(rowCountInColumn + 1);

            double y = 0;
            double x = 0;
            double currentRowBegin = 0;
            double currentMaxX = 0;
            for(int rowIndex = 0; rowIndex < layoutDefinition.Rows.Count; rowIndex++)
            {
                RibbonToolBarRow row = layoutDefinition.Rows[rowIndex];

                if (rowIndex % rowCountInColumn == 0)
                {
                    // Reset vars at new column
                    currentRowBegin = currentMaxX;
                    y = 0;
                }
                y += whitespace;
                x = currentRowBegin;

                // Measure & arrange new row
                for(int i = 0; i < row.Children.Count; i++)
                {
                    if (row.Children[i] is RibbonToolBarControlDefinition)
                    {
                        // Control Definition Case
                        RibbonToolBarControlDefinition controlDefinition =
                            (RibbonToolBarControlDefinition) row.Children[i];
                        FrameworkElement control = GetControl(controlDefinition);
                        if (control == null) continue;

                        if (addchildren)
                        {
                            // Add control in the children
                            AddVisualChild(control);
                            AddLogicalChild(control);
                            actualChildren.Add(control);
                        }

                        if (measure)
                        {
                            // Apply Control Definition Properties
                            if (control is RibbonControl) ((RibbonControl) control).Size = controlDefinition.Size;
                            control.Width = controlDefinition.Width;
                            control.Measure(availableSize);
                        }
                        if (arrange)
                        {
                            control.Arrange(new Rect(x, y, 
                                control.DesiredSize.Width, 
                                control.DesiredSize.Height));
                        }

                        x += control.DesiredSize.Width;
                    }
                    if (row.Children[i] is RibbonToolBarControlGroupDefinition)
                    {
                        // Control Definition Case
                        RibbonToolBarControlGroupDefinition controlGroupDefinition =
                            (RibbonToolBarControlGroupDefinition)row.Children[i];

                        RibbonToolBarControlGroup control = GetControlGroup(controlGroupDefinition);
                        
                        if (addchildren)
                        {
                            // Add control in the children
                            AddVisualChild(control);
                            AddLogicalChild(control);
                            actualChildren.Add(control);
                        }

                        if (measure)
                        {
                            // Apply Control Definition Properties
                            control.IsFirstInRow = (i == 0);
                            control.IsLastInRow = (i == row.Children.Count - 1);
                            control.Measure(availableSize);
                        }
                        if (arrange)
                        {
                            control.Arrange(new Rect(x, y,
                                control.DesiredSize.Width,
                                control.DesiredSize.Height));
                        }

                        x += control.DesiredSize.Width;
                    }
                }

                y += rowHeight;
                if (currentMaxX < x) currentMaxX = x;
            }

            // FIXIT:
            return new Size(currentMaxX, y + whitespace);
        }

        #endregion


        #endregion

        public override UIElement CreateQuickAccessItem()
        {
            throw new NotImplementedException();
        }

        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            throw new NotImplementedException();
        }
    }
}