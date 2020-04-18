// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents container of grouped gallery items in GalleryPanel or Gallery
    /// </summary>
    public class GalleryGroupContainer : HeaderedItemsControl
    {
        #region Properites

        #region IsHeadered

        /// <summary>
        /// Gets or sets whether the header must be shown.
        /// When the property is false this control uses to show all items without grouping
        /// </summary>
        public bool IsHeadered
        {
            get { return (bool)this.GetValue(IsHeaderedProperty); }
            set { this.SetValue(IsHeaderedProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsHeadered"/>.
        /// </summary>
        public static readonly DependencyProperty IsHeaderedProperty =
            DependencyProperty.Register(nameof(IsHeadered), typeof(bool),
            typeof(GalleryGroupContainer), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets panel orientation
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="Orientation"/>.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation),
            typeof(GalleryGroupContainer), new PropertyMetadata(Orientation.Horizontal));

        #endregion

        #region ItemWidth

        /// <summary>
        /// Gets or sets a value that specifies the width of
        /// all items that are contained within
        /// </summary>
        public double ItemWidth
        {
            get { return (double)this.GetValue(ItemWidthProperty); }
            set { this.SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ItemWidth"/>.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(double),
            typeof(GalleryGroupContainer), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region ItemHeight

        /// <summary>
        /// Gets or sets a value that specifies the height of
        /// all items that are contained within
        /// </summary>
        public double ItemHeight
        {
            get { return (double)this.GetValue(ItemHeightProperty); }
            set { this.SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ItemHeight"/>.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double),
            typeof(GalleryGroupContainer), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region MinItemsInRow

        /// <summary>
        /// Gets or sets minimum items in which should be placed in one row.
        /// </summary>
        public int MinItemsInRow
        {
            get { return (int)this.GetValue(MinItemsInRowProperty); }
            set { this.SetValue(MinItemsInRowProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="MinItemsInRow"/>.
        /// </summary>
        public static readonly DependencyProperty MinItemsInRowProperty =
            DependencyProperty.Register(nameof(MinItemsInRow), typeof(int),
            typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(IntBoxes.Zero, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region MaxItemsInRow

        /// <summary>
        /// Gets or sets maximum items in which should be placed in one row.
        /// </summary>
        public int MaxItemsInRow
        {
            get { return (int)this.GetValue(MaxItemsInRowProperty); }
            set { this.SetValue(MaxItemsInRowProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="MaxItemsInRow"/>.
        /// </summary>
        public static readonly DependencyProperty MaxItemsInRowProperty = DependencyProperty.Register(nameof(MaxItemsInRow), typeof(int), typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        static GalleryGroupContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryGroupContainer), new FrameworkPropertyMetadata(typeof(GalleryGroupContainer)));
        }

        /// <inheritdoc />
        protected override void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel)
        {
            base.OnItemsPanelChanged(oldItemsPanel, newItemsPanel);

            this.InvalidateMeasure();
        }

        #endregion
    }
}