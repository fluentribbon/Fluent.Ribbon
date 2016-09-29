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

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Extensibility;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represent panel for group box panel
    /// </summary>
    [ContentProperty(nameof(Children))]
    public class RibbonToolBar : RibbonControl, IRibbonSizeChangedSink
    {
        #region Fields

        // User defined children
        // User defined layout definitions

        // Actual children
        private readonly List<FrameworkElement> actualChildren = new List<FrameworkElement>();
        // Designates that rebuilding of visual & logical children is required
        private bool rebuildVisualAndLogicalChildren = true;

        #endregion

        #region Properties

        #region Separator Style

        /// <summary>
        /// Gets or sets style for the separator
        /// </summary>
        public Style SeparatorStyle
        {
            get { return (Style)this.GetValue(SeparatorStyleProperty); }
            set { this.SetValue(SeparatorStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SeparatorStyle.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SeparatorStyleProperty =
            DependencyProperty.Register(nameof(SeparatorStyle), typeof(Style),
            typeof(RibbonToolBar), new PropertyMetadata(OnSeparatorStyleChanged));

        private static void OnSeparatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toolBar = (RibbonToolBar)d;
            toolBar.rebuildVisualAndLogicalChildren = true;
            toolBar.InvalidateMeasure();
        }

        #endregion

        /// <summary>
        /// Gets children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<FrameworkElement> Children { get; } = new ObservableCollection<FrameworkElement>();

        /// <summary>
        /// Gets particular rules  for layout in this group box panel
        /// </summary>
        public ObservableCollection<RibbonToolBarLayoutDefinition> LayoutDefinitions { get; } = new ObservableCollection<RibbonToolBarLayoutDefinition>();

        #endregion

        #region Logical & Visual Tree

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                if (this.LayoutDefinitions.Count == 0)
                {
                    return this.Children.Count;
                }

                if (this.rebuildVisualAndLogicalChildren)
                {
                    //TODO: Exception during theme changing
                    // UpdateLayout();
                    this.InvalidateMeasure();
                }

                return this.actualChildren.Count;
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
            if (this.LayoutDefinitions.Count == 0)
            {
                return this.Children[index];
            }

            if (this.rebuildVisualAndLogicalChildren)
            {
                // UpdateLayout();
                this.InvalidateMeasure();
            }

            return this.actualChildren[index];
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                return this.Children.GetEnumerator();
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
            CanAddToQuickAccessToolBarProperty.OverrideMetadata(typeof(RibbonToolBar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBar()
        {
            this.Children.CollectionChanged += this.OnChildrenCollectionChanged;
            this.LayoutDefinitions.CollectionChanged += this.OnLayoutDefinitionsChanged;
        }

        private void OnLayoutDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.rebuildVisualAndLogicalChildren = true;
            this.InvalidateMeasure();
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Children have changed, reset layouts
            this.rebuildVisualAndLogicalChildren = true;
            this.InvalidateMeasure();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets current used layout definition (or null if no present definitions)
        /// </summary>
        /// <returns>Layout definition or null</returns>
        internal RibbonToolBarLayoutDefinition GetCurrentLayoutDefinition()
        {
            if (this.LayoutDefinitions.Count == 0)
            {
                return null;
            }

            if (this.LayoutDefinitions.Count == 1)
            {
                return this.LayoutDefinitions[0];
            }

            foreach (var definition in this.LayoutDefinitions)
            {
                if (RibbonProperties.GetSize(definition) == RibbonProperties.GetSize(this))
                {
                    return definition;
                }
            }

            // TODO: try to find a better definition
            return this.LayoutDefinitions[0];
        }

        #endregion

        #region Size Property Changing

        /// <summary>
        /// Handles size property changing
        /// </summary>
        /// <param name="previous">Previous value</param>
        /// <param name="current">Current value</param>
        public void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
            foreach (var frameworkElement in this.actualChildren)
            {
                RibbonProperties.SetSize(frameworkElement, current);
            }

            this.rebuildVisualAndLogicalChildren = true;
            this.InvalidateMeasure();
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
            var layoutDefinition = this.GetCurrentLayoutDefinition();

            // Rebuilding actual children (visual & logical)
            if (this.rebuildVisualAndLogicalChildren)
            {
                // Clear previous children
                foreach (var child in this.actualChildren)
                {
                    var controlGroup = child as RibbonToolBarControlGroup;
                    controlGroup?.Items.Clear();

                    this.RemoveVisualChild(child);
                    this.RemoveLogicalChild(child);
                }

                this.actualChildren.Clear();
                this.cachedControlGroups.Clear();
            }

            if (layoutDefinition == null)
            {
                if (this.rebuildVisualAndLogicalChildren)
                {
                    // If default layout is used add all children
                    foreach (var child in this.Children)
                    {
                        this.actualChildren.Add(child);
                        this.AddVisualChild(child);
                        this.AddLogicalChild(child);
                    }

                    this.rebuildVisualAndLogicalChildren = false;
                }

                return this.WrapPanelLayuot(availableSize, true);
            }
            else
            {
                var result = this.CustomLayout(layoutDefinition, availableSize, true, this.rebuildVisualAndLogicalChildren);
                this.rebuildVisualAndLogicalChildren = false;
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
            var layoutDefinition = this.GetCurrentLayoutDefinition();

            if (layoutDefinition == null)
            {
                return this.WrapPanelLayuot(finalSize, false);
            }

            return this.CustomLayout(layoutDefinition, finalSize, false, false);
        }

        #region Wrap Panel Layout

        /// <summary>
        /// Unified method for wrap panel logic
        /// </summary>
        /// <param name="availableSize">Available or final size</param>
        /// <param name="measure">Pass true if measure required; pass false if arrange required</param>
        /// <returns>Final size</returns>
        private Size WrapPanelLayuot(Size availableSize, bool measure)
        {
            var arrange = !measure;
            var availableHeight = double.IsPositiveInfinity(availableSize.Height) 
                ? 0 
                : availableSize.Height;

            double currentheight = 0;
            double columnWidth = 0;

            double resultWidth = 0;
            double resultHeight = 0;

            var infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);

            foreach (var child in this.Children)
            {
                // Measuring
                if (measure)
                {
                    child.Measure(infinity);
                }

                if (currentheight + child.DesiredSize.Height > availableHeight)
                {
                    // Move to the next column
                    resultHeight = Math.Max(resultHeight, currentheight);
                    resultWidth += columnWidth;
                    currentheight = 0;
                    columnWidth = 0;
                }

                // Arranging
                if (arrange)
                {
                    child.Arrange(new Rect(resultWidth, currentheight, child.DesiredSize.Width, child.DesiredSize.Height));
                }

                columnWidth = Math.Max(columnWidth, child.DesiredSize.Width);
                currentheight += child.DesiredSize.Height;
            }

            return new Size(resultWidth + columnWidth, resultHeight);
        }

        #endregion

        #region Control and Group Creation from a Definition

        private FrameworkElement GetControl(RibbonToolBarControlDefinition controlDefinition)
        {
            var name = controlDefinition.Target;
            return this.Children.FirstOrDefault(x => x.Name == name);
        }

        private readonly Dictionary<object, RibbonToolBarControlGroup> cachedControlGroups = new Dictionary<object, RibbonToolBarControlGroup>();

        private RibbonToolBarControlGroup GetControlGroup(RibbonToolBarControlGroupDefinition controlGroupDefinition)
        {
            RibbonToolBarControlGroup controlGroup;

            if (this.cachedControlGroups.TryGetValue(controlGroupDefinition, out controlGroup))
            {
                return controlGroup;
            }

            controlGroup = new RibbonToolBarControlGroup();

            // Add items to the group
            foreach (var child in controlGroupDefinition.Children)
            {
                controlGroup.Items.Add(this.GetControl(child));
            }

            this.cachedControlGroups.Add(controlGroupDefinition, controlGroup);
            return controlGroup;
        }

        #endregion

        #region Custom Layout

        // Cached separators (clear & set in Measure pass)
        private readonly Dictionary<int, Separator> separatorCache = new Dictionary<int, Separator>();

        /// <summary>
        /// Layout logic for the given layout definition
        /// </summary>
        /// <param name="layoutDefinition">Current layout definition</param>
        /// <param name="availableSize">Available or final size</param>
        /// <param name="measure">Pass true if measure required; pass false if arrange required</param>
        /// <param name="addchildren">Determines whether we have to add children to the logical and visual tree</param>
        /// <returns>Final size</returns>
        private Size CustomLayout(RibbonToolBarLayoutDefinition layoutDefinition, Size availableSize, bool measure, bool addchildren)
        {
            var arrange = !measure;
            var availableHeight = double.IsPositiveInfinity(availableSize.Height) 
                ? 0 
                : availableSize.Height;

            // Clear separator cahce
            if (addchildren)
            {
                this.separatorCache.Clear();
            }

            // Get the first control and measure, its height accepts as row height
            var rowHeight = this.GetRowHeight(layoutDefinition);

            // Calculate whitespace
            var rowCountInColumn = Math.Min(layoutDefinition.RowCount, layoutDefinition.Rows.Count);
            var whitespace = (availableHeight - rowCountInColumn * rowHeight) / (rowCountInColumn + 1);

            double y = 0;
            double currentRowBegin = 0;
            double currentMaxX = 0;
            double maxy = 0;

            for (var rowIndex = 0; rowIndex < layoutDefinition.Rows.Count; rowIndex++)
            {
                var row = layoutDefinition.Rows[rowIndex];

                var x = currentRowBegin;

                if (rowIndex % rowCountInColumn == 0)
                {
                    // Reset vars at new column
                    x = currentRowBegin = currentMaxX;
                    y = 0;

                    if (rowIndex != 0)
                    {
                        #region Add separator

                        Separator separator;
                        if (!this.separatorCache.TryGetValue(rowIndex, out separator))
                        {
                            separator = new Separator
                                        {
                                            Style = this.SeparatorStyle
                                        };
                            this.separatorCache.Add(rowIndex, separator);
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
                            this.AddVisualChild(separator);
                            this.AddLogicalChild(separator);
                            this.actualChildren.Add(separator);
                        }

                        #endregion
                    }
                }

                y += whitespace;

                // Measure & arrange new row
                for (var i = 0; i < row.Children.Count; i++)
                {
                    // Control Definition Case
                    var ribbonToolBarControlDefinition = row.Children[i] as RibbonToolBarControlDefinition;
                    if (ribbonToolBarControlDefinition != null)
                    {
                        var control = this.GetControl(ribbonToolBarControlDefinition);

                        if (control == null)
                        {
                            continue;
                        }

                        if (addchildren)
                        {
                            // Add control in the children
                            this.AddVisualChild(control);
                            this.AddLogicalChild(control);
                            this.actualChildren.Add(control);
                        }

                        if (measure)
                        {
                            // Apply Control Definition Properties
                            RibbonProperties.SetSize(control, RibbonProperties.GetSize(ribbonToolBarControlDefinition));
                            control.Width = ribbonToolBarControlDefinition.Width;
                            control.Measure(availableSize);
                        }

                        if (arrange)
                        {
                            control.Arrange(new Rect(x, y, control.DesiredSize.Width, control.DesiredSize.Height));
                        }

                        x += control.DesiredSize.Width;
                    }

                    // Control Group Definition Case
                    var ribbonToolBarControlGroupDefinition = row.Children[i] as RibbonToolBarControlGroupDefinition;
                    if (ribbonToolBarControlGroupDefinition != null)
                    {
                        var control = this.GetControlGroup(ribbonToolBarControlGroupDefinition);

                        if (addchildren)
                        {
                            // Add control in the children
                            this.AddVisualChild(control);
                            this.AddLogicalChild(control);
                            this.actualChildren.Add(control);
                        }

                        if (measure)
                        {
                            // Apply Control Definition Properties
                            control.IsFirstInRow = i == 0;
                            control.IsLastInRow = i == row.Children.Count - 1;
                            control.Measure(availableSize);
                        }

                        if (arrange)
                        {
                            control.Arrange(new Rect(x, y, control.DesiredSize.Width, control.DesiredSize.Height));
                        }

                        x += control.DesiredSize.Width;
                    }
                }

                y += rowHeight;

                if (currentMaxX < x)
                {
                    currentMaxX = x;
                }

                if (maxy < y)
                {
                    maxy = y;
                }
            }

            return new Size(currentMaxX, maxy + whitespace);
        }

        // Get the first control and measure, its height accepts as row height
        private double GetRowHeight(RibbonToolBarLayoutDefinition layoutDefinition)
        {
            const double defaultRowHeight = 0;

            foreach (var row in layoutDefinition.Rows)
            {
                foreach (var item in row.Children)
                {
                    var controlDefinition = item as RibbonToolBarControlDefinition;
                    var controlGroupDefinition = item as RibbonToolBarControlGroupDefinition;
                    FrameworkElement control = null;

                    if (controlDefinition != null)
                    {
                        control = this.GetControl(controlDefinition);
                    }
                    else if (controlGroupDefinition != null)
                    {
                        control = this.GetControlGroup(controlGroupDefinition);
                    }

                    if (control == null)
                    {
                        return defaultRowHeight;
                    }

                    control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

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