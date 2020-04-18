// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// UniformGrid is used to arrange children in a grid with all equal cell sizes.
    /// </summary>
    public class UniformGridWithItemSize : Panel
    {
        private int rows;
        private int columns;
        private int nonCollapsedCount;
        private int usedColumns;

        /// <summary>
        /// Specifies the number of maximum columns in the grid
        /// </summary>
        public int MinColumns
        {
            get => (int)this.GetValue(MinColumnsProperty);
            set => this.SetValue(MinColumnsProperty, value);
        }

        /// <summary>
        /// DependencyProperty for <see cref="MinColumns" /> property.
        /// </summary>
        public static readonly DependencyProperty MinColumnsProperty =
            DependencyProperty.Register(
                nameof(MinColumns),
                typeof(int),
                typeof(UniformGridWithItemSize),
                new FrameworkPropertyMetadata(
                    IntBoxes.Zero,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                ValidateMinColumns);

        private static bool ValidateMinColumns(object o)
        {
            return (int)o >= 0;
        }

        /// <summary>
        /// Specifies the number of maximum columns in the grid
        /// </summary>
        public int MaxColumns
        {
            get => (int)this.GetValue(MaxColumnsProperty);
            set => this.SetValue(MaxColumnsProperty, value);
        }

        /// <summary>
        /// DependencyProperty for <see cref="MaxColumns" /> property.
        /// </summary>
        public static readonly DependencyProperty MaxColumnsProperty =
            DependencyProperty.Register(
                nameof(MaxColumns),
                typeof(int),
                typeof(UniformGridWithItemSize),
                new FrameworkPropertyMetadata(
                    IntBoxes.Zero,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                ValidateMaxColumns);

        private static bool ValidateMaxColumns(object o)
        {
            return (int)o >= 0;
        }

        /// <summary>
        /// DependencyProperty for <see cref="ItemWidth" /> property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            nameof(ItemWidth), typeof(double), typeof(UniformGridWithItemSize), new FrameworkPropertyMetadata(DoubleBoxes.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Specifies the item width.
        /// </summary>
        public double ItemWidth
        {
            get => (double)this.GetValue(ItemWidthProperty);
            set => this.SetValue(ItemWidthProperty, value);
        }

        /// <summary>
        /// DependencyProperty for <see cref="ItemHeight" /> property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight), typeof(double), typeof(UniformGridWithItemSize), new FrameworkPropertyMetadata(DoubleBoxes.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Specifies the item height.
        /// </summary>
        public double ItemHeight
        {
            get => (double)this.GetValue(ItemHeightProperty);
            set => this.SetValue(ItemHeightProperty, value);
        }

        /// <summary>
        /// Compute the desired size of this UniformGrid by measuring all of the
        /// children with a constraint equal to a cell's portion of the given
        /// constraint (e.g. for a 2 x 4 grid, the child constraint would be
        /// constraint.Width*0.5 x constraint.Height*0.25).  The maximum child
        /// width and maximum child height are tracked, and then the desired size
        /// is computed by multiplying these maximums by the row and column count
        /// (e.g. for a 2 x 4 grid, the desired size for the UniformGrid would be
        /// maxChildDesiredWidth*2 x maxChildDesiredHeight*4).
        /// </summary>
        /// <param name="constraint">Constraint</param>
        /// <returns>Desired size</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Debug.WriteLine($"MeasureOverride: {constraint}");

            this.UpdateComputedValues(this.MaxColumns);

            var useDefinedItemWidth = double.IsNaN(this.ItemWidth) == false && DoubleUtil.AreClose(this.ItemWidth, 0) == false;
            var maxItemWidth = useDefinedItemWidth
                ? this.ItemWidth
                : constraint.Width / this.columns;
            var useDefinedItemHeight = double.IsNaN(this.ItemHeight) == false && DoubleUtil.AreClose(this.ItemHeight, 0) == false;
            var maxItemHeight = useDefinedItemHeight
                ? this.ItemHeight
                : constraint.Height / this.rows;

            var childConstraint = new Size(maxItemWidth, maxItemHeight);
            var maxChildDesiredWidth = 0.0;
            var maxChildDesiredHeight = 0.0;

            //  Measure each child, keeping track of maximum desired width and height.
            for (int i = 0, count = this.InternalChildren.Count; i < count; ++i)
            {
                var child = this.InternalChildren[i];

                // Measure the child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (useDefinedItemWidth)
                {
                    maxChildDesiredWidth = this.ItemWidth;
                }
                else
                {
                    if (maxChildDesiredWidth < childDesiredSize.Width)
                    {
                        maxChildDesiredWidth = childDesiredSize.Width;
                    }
                }

                if (useDefinedItemHeight)
                {
                    maxChildDesiredHeight = this.ItemHeight;
                }
                else
                {
                    if (maxChildDesiredHeight < childDesiredSize.Height)
                    {
                        maxChildDesiredHeight = childDesiredSize.Height;
                    }
                }
            }

            if (double.IsNaN(constraint.Width) == false
                && double.IsPositiveInfinity(constraint.Width) == false)
            {
                var precomputedColumns = this.MaxColumns != 0
                    ? Math.Min(this.MaxColumns, (int)(constraint.Width / maxChildDesiredWidth))
                    : (int)(constraint.Width / maxChildDesiredWidth);
                this.UpdateComputedValues(precomputedColumns);
            }

            return new Size(maxChildDesiredWidth * this.usedColumns, maxChildDesiredHeight * this.rows);
        }

        /// <summary>
        /// Arrange the children of this UniformGrid by distributing space evenly 
        /// among all of the children, making each child the size equal to a cell's
        /// portion of the given arrangeSize (e.g. for a 2 x 4 grid, the child size
        /// would be arrangeSize*0.5 x arrangeSize*0.25)
        /// </summary>
        /// <param name="arrangeSize">Arrange size</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Debug.WriteLine($"ArrangeOverride: {arrangeSize}");

            var childBounds = new Rect(0, 0, arrangeSize.Width / this.usedColumns, arrangeSize.Height / this.rows);
            var xStep = childBounds.Width;
            var xBound = arrangeSize.Width;

            // Arrange and Position each child to the same cell size
            foreach (UIElement child in this.InternalChildren)
            {
                child.Arrange(childBounds);

                // only advance to the next grid cell if the child was not collapsed
                if (child.Visibility != Visibility.Collapsed)
                {
                    childBounds.X += xStep;

                    if (childBounds.X >= xBound)
                    {
                        childBounds.Y += childBounds.Height;
                        childBounds.X = 0;
                    }
                }
            }

            return arrangeSize;
        }

        /// <summary>
        /// If either Rows or Columns are set to 0, then dynamically compute these
        /// values based on the actual number of non-collapsed children.
        ///
        /// In the case when both Rows and Columns are set to 0, then make Rows 
        /// and Columns be equal, thus laying out in a square grid.
        /// </summary>
        private void UpdateComputedValues(int precomputedColumns = 0)
        {
            this.columns = precomputedColumns;
            this.rows = 0;

            this.nonCollapsedCount = 0;

            // First compute the actual # of non-collapsed children to be laid out
            for (int i = 0, count = this.InternalChildren.Count; i < count; ++i)
            {
                var child = this.InternalChildren[i];
                if (child.Visibility != Visibility.Collapsed)
                {
                    this.nonCollapsedCount++;
                }
            }

            // to ensure that we have at leat one row & column, make sure
            // that nonCollapsedCount is at least 1
            if (this.nonCollapsedCount == 0)
            {
                this.nonCollapsedCount = 1;
            }

            if (this.columns == 0)
            {
                // columns are unset -- lay out in a square
                this.columns = (int)Math.Sqrt(this.nonCollapsedCount);
                if (this.columns * this.columns < this.nonCollapsedCount)
                {
                    this.columns++;
                }
            }

            if (this.MinColumns != 0
                && this.columns < this.MinColumns)
            {
                this.columns = this.MinColumns;
            }

            if (this.MaxColumns != 0
                && this.columns > this.MaxColumns)
            {
                this.columns = this.MaxColumns;
            }

            this.columns = Math.Max(1, this.columns);

            // -1 because we ensured that nonCollapsedCount is at least 1
            this.rows = (this.nonCollapsedCount + (this.columns - 1)) / this.columns;
            this.rows = Math.Max(1, this.rows);

            this.usedColumns = Math.Min(this.columns, this.nonCollapsedCount);
        }
    }
}