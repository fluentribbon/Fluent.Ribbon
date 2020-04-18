// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// UniformGrid is used to arrange children in a grid with all equal cell sizes.
    /// </summary>
    public class UniformGridWithItemHeight : Panel
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        #endregion Constructors

        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// the start column to arrange children. Leave first 'FirstColumn' 
        /// cells blank.
        /// </summary>
        public int FirstColumn
        {
            get => (int)this.GetValue(FirstColumnProperty);
            set => this.SetValue(FirstColumnProperty, value);
        }

        /// <summary>
        /// FirstColumnProperty
        /// </summary>
        public static readonly DependencyProperty FirstColumnProperty =
            DependencyProperty.Register(
                nameof(FirstColumn),
                typeof(int),
                typeof(UniformGridWithItemHeight),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                ValidateFirstColumn);

        private static bool ValidateFirstColumn(object o)
        {
            return (int)o >= 0;
        }

        /// <summary>
        /// Specifies the number of columns in the grid
        /// A value of 0 indicates that the column count should be dynamically 
        /// computed based on the number of rows (if specified) and the 
        /// number of non-collapsed children in the grid
        /// </summary>
        public int Columns
        {
            get => (int)this.GetValue(ColumnsProperty);
            set => this.SetValue(ColumnsProperty, value);
        }

        /// <summary>
        /// DependencyProperty for <see cref="Columns" /> property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                nameof(Columns),
                typeof(int),
                typeof(UniformGridWithItemHeight),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                ValidateColumns);

        private static bool ValidateColumns(object o)
        {
            return (int)o >= 0;
        }

        /// <summary>
        /// Specifies the number of rows in the grid
        /// A value of 0 indicates that the row count should be dynamically 
        /// computed based on the number of columns (if specified) and the 
        /// number of non-collapsed children in the grid
        /// </summary>
        public int Rows
        {
            get => (int)this.GetValue(RowsProperty);
            set => this.SetValue(RowsProperty, value);
        }

        /// <summary>
        /// DependencyProperty for <see cref="Rows" /> property.
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                nameof(Rows),
                typeof(int),
                typeof(UniformGridWithItemHeight),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                ValidateRows);

        private static bool ValidateRows(object o)
        {
            return (int)o >= 0;
        }

        /// <summary>
        /// DependencyProperty for <see cref="ItemHeight" /> property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight), typeof(double), typeof(UniformGridWithItemHeight), new FrameworkPropertyMetadata(DoubleBoxes.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Specifies the item height.
        /// </summary>
        public double ItemHeight
        {
            get => (double)this.GetValue(ItemHeightProperty);
            set => this.SetValue(ItemHeightProperty, value);
        }

        #endregion Public Properties

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

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
            this.UpdateComputedValues();

            var childConstraint = double.IsNaN(this.ItemHeight)
                ? new Size(constraint.Width / this.columns, constraint.Height / this.rows)
                : new Size(constraint.Width / this.columns, this.ItemHeight);
            var maxChildDesiredWidth = 0.0;
            var maxChildDesiredHeight = 0.0;

            //  Measure each child, keeping track of maximum desired width and height.
            for (int i = 0, count = this.InternalChildren.Count; i < count; ++i)
            {
                var child = this.InternalChildren[i];

                // Measure the child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (maxChildDesiredWidth < childDesiredSize.Width)
                {
                    maxChildDesiredWidth = childDesiredSize.Width;
                }

                if (double.IsNaN(this.ItemHeight))
                {
                    if (maxChildDesiredHeight < childDesiredSize.Height)
                    {
                        maxChildDesiredHeight = childDesiredSize.Height;
                    }
                }
                else
                {
                    maxChildDesiredHeight = this.ItemHeight;
                }
            }

            return new Size(maxChildDesiredWidth * this.columns, maxChildDesiredHeight * this.rows);
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
            var childBounds = new Rect(0, 0, arrangeSize.Width / this.columns, arrangeSize.Height / this.rows);
            var xStep = childBounds.Width;
            var xBound = arrangeSize.Width - 1.0;

            childBounds.X += childBounds.Width * this.FirstColumn;

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

        #endregion Protected Methods

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// If either Rows or Columns are set to 0, then dynamically compute these
        /// values based on the actual number of non-collapsed children.
        ///
        /// In the case when both Rows and Columns are set to 0, then make Rows 
        /// and Columns be equal, thus laying out in a square grid.
        /// </summary>
        private void UpdateComputedValues()
        {
            this.columns = this.Columns;
            this.rows = this.Rows;

            //parameter checking. 
            if (this.FirstColumn >= this.columns)
            {
                //NOTE: maybe we shall throw here. But this is somewhat out of 
                //the MCC itself. We need a whole new panel spec.
                this.FirstColumn = 0;
            }

            if (this.rows == 0
                || this.columns == 0)
            {
                var nonCollapsedCount = 0;

                // First compute the actual # of non-collapsed children to be laid out
                for (int i = 0, count = this.InternalChildren.Count; i < count; ++i)
                {
                    var child = this.InternalChildren[i];
                    if (child.Visibility != Visibility.Collapsed)
                    {
                        nonCollapsedCount++;
                    }
                }

                // to ensure that we have at leat one row & column, make sure
                // that nonCollapsedCount is at least 1
                if (nonCollapsedCount == 0)
                {
                    nonCollapsedCount = 1;
                }

                if (this.rows == 0)
                {
                    if (this.columns > 0)
                    {
                        // take FirstColumn into account, because it should really affect the result
                        this.rows = (nonCollapsedCount + this.FirstColumn + (this.columns - 1)) / this.columns;
                    }
                    else
                    {
                        // both rows and columns are unset -- lay out in a square
                        this.rows = (int)Math.Sqrt(nonCollapsedCount);
                        if (this.rows * this.rows < nonCollapsedCount)
                        {
                            this.rows++;
                        }

                        this.columns = this.rows;
                    }
                }
                else if (this.columns == 0)
                {
                    // guaranteed that _rows is not 0, because we're in the else clause of the check for _rows == 0
                    this.columns = (nonCollapsedCount + (this.rows - 1)) / this.rows;
                }
            }
        }

        #endregion Private Properties

        private int rows;
        private int columns;
    }
}