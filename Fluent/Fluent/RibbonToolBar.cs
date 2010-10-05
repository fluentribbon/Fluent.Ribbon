using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

        #region Separator Style

        /// <summary>
        /// Gets or sets style for the separator
        /// </summary>
        public Style SeparatorStyle
        {
            get { return (Style)GetValue(SeparatorStyleProperty); }
            set { SetValue(SeparatorStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SeparatorStyle.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SeparatorStyleProperty =
            DependencyProperty.Register("SeparatorStyle", typeof(Style), 
            typeof(RibbonToolBar), new UIPropertyMetadata(null, OnSeparatorStyleChanged));

        static void OnSeparatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonToolBar toolBar = (RibbonToolBar)d;
            toolBar.rebuildVisualAndLogicalChildren = true;
            toolBar.InvalidateMeasure();
        }

        #endregion

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
                if (rebuildVisualAndLogicalChildren)
                {
                    //TODO: Exception during theme changing
                    // UpdateLayout();
                    InvalidateMeasure();
                }
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
            if (rebuildVisualAndLogicalChildren)
            {
                // UpdateLayout();
                InvalidateMeasure();
            }
            return actualChildren[index];
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                return children.GetEnumerator();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonToolBar()
        {
            // Override default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonToolBar), new FrameworkPropertyMetadata(typeof(RibbonToolBar)));
            // Disable QAT for this control
            CanAddToQuickAccessToolBarProperty.OverrideMetadata(typeof(RibbonToolBar), new FrameworkPropertyMetadata(false));
            StyleProperty.OverrideMetadata(typeof(RibbonToolBar), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(RibbonToolBar));
            }

            return basevalue;
        }

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
            rebuildVisualAndLogicalChildren = true;
            InvalidateMeasure();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets current used layout definition (or null if no present definitions)
        /// </summary>
        /// <returns>Layout definition or null</returns>
        internal RibbonToolBarLayoutDefinition GetCurrentLayoutDefinition()
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
                    RibbonToolBarControlGroup controlGroup = child as RibbonToolBarControlGroup;
                    if (controlGroup != null) controlGroup.Items.Clear();
                    RemoveVisualChild(child);
                    RemoveLogicalChild(child);
                }
                actualChildren.Clear();
                cachedControlGroups.Clear();
            }

            if (layoutDefinition == null)
            {
                if (rebuildVisualAndLogicalChildren)
                {
                    // If default layout is used add all children
                    foreach (FrameworkElement child in Children)
                    {
                        actualChildren.Add(child);
                        AddVisualChild(child);
                        AddLogicalChild(child);
                    }
                    rebuildVisualAndLogicalChildren = false;
                }
                return WrapPanelLayuot(availableSize, true);
            }
            else
            {
                Size result = CustomLayout(layoutDefinition, availableSize, true, rebuildVisualAndLogicalChildren);
                rebuildVisualAndLogicalChildren = false;
                return result;
            }


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
            if (layoutDefinition == null) return WrapPanelLayuot(finalSize, false);
            return CustomLayout(layoutDefinition, finalSize, false, false);
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
     
        #region Control and Group Creation from a Definition

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

        #endregion
   
        #region Custom Layout

        // Cached separators (clear & set in Measure pass)
        Dictionary<int, Separator> separatorCache = new Dictionary<int, Separator>();

        /// <summary>
        /// Layout logic for the given layout definition
        /// </summary>
        /// <param name="layoutDefinition">Current layout definition</param>
        /// <param name="availableSize">Available or final size</param>
        /// <param name="measure">Pass true if measure required; pass false if arrange required</param>
        /// <param name="addchildren">Determines whether we have to add children to the logical and visual tree</param>
        /// <returns>Final size</returns>
        Size CustomLayout(RibbonToolBarLayoutDefinition layoutDefinition, Size availableSize, bool measure, bool addchildren)
        {
            bool arrange = !measure;
            double availableHeight = Double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Height;

            // Clear separator cahce
            if (addchildren) separatorCache.Clear();

            // Get the first control and measure, its height accepts as row height
            double rowHeight = GetRowHeight(layoutDefinition);
            

            // Calculate whitespace
            int rowCountInColumn = Math.Min(layoutDefinition.RowCount, layoutDefinition.Rows.Count);
            double whitespace = (availableHeight - ((double)rowCountInColumn * rowHeight)) / (double)(rowCountInColumn + 1);

            double y = 0;
            double x = 0;
            double currentRowBegin = 0;
            double currentMaxX = 0;
            double maxy = 0;
            for(int rowIndex = 0; rowIndex < layoutDefinition.Rows.Count; rowIndex++)
            {
                RibbonToolBarRow row = layoutDefinition.Rows[rowIndex];

                x = currentRowBegin;

                if (rowIndex % rowCountInColumn == 0)
                {
                    // Reset vars at new column
                    x = currentRowBegin = currentMaxX;
                    y = 0;

                    if (rowIndex != 0)
                    {
                        #region Add separator

                        Separator separator = null;
                        if (!separatorCache.TryGetValue(rowIndex, out separator))
                        {
                            separator = new Separator();
                            separator.Style = SeparatorStyle;
                            separatorCache.Add(rowIndex, separator);
                        }
                        if (measure)
                        {
                            separator.Height = availableHeight - separator.Margin.Bottom - separator.Margin.Top;
                            separator.Measure(availableSize);
                        }
                        if (arrange) separator.Arrange(new Rect(x, y,
                                separator.DesiredSize.Width,
                                separator.DesiredSize.Height));
                        x += separator.DesiredSize.Width;

                        if (addchildren)
                        {
                            // Add control in the children
                            AddVisualChild(separator);
                            AddLogicalChild(separator);
                            actualChildren.Add(separator);
                        }

                        #endregion
                    }
                }
                y += whitespace;
                

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
                            IRibbonControl ribbonControl = control as IRibbonControl;
                            if (ribbonControl != null) ribbonControl.Size = controlDefinition.Size;
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
                if (maxy < y) maxy = y;
            }

            return new Size(currentMaxX, maxy + whitespace);
        }

        // Get the first control and measure, its height accepts as row height
        double GetRowHeight(RibbonToolBarLayoutDefinition layoutDefinition)
        {
            const double defaultRowHeight = 0;
            foreach (RibbonToolBarRow row in layoutDefinition.Rows)
            {
                foreach (DependencyObject item in row.Children)
                {
                    RibbonToolBarControlDefinition controlDefinition = item as RibbonToolBarControlDefinition;
                    RibbonToolBarControlGroupDefinition controlGroupDefinition = item as RibbonToolBarControlGroupDefinition;
                    FrameworkElement control = null;
                    if (controlDefinition != null) control = GetControl(controlDefinition);
                    else if (controlGroupDefinition != null)
                        control = GetControlGroup(controlGroupDefinition);

                    if (control == null) return defaultRowHeight;
                    control.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    return control.DesiredSize.Height;
                }
            }
            return defaultRowHeight;
        }

        #endregion
        
        #endregion

        #region QAT Support

        // (!) RibbonToolBar must not to be in QAT

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            return new Control();
        }

        #endregion
    }
}