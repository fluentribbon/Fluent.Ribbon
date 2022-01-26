// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Markup;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents size definition for group box
    /// </summary>
    [ContentProperty(nameof(Rows))]
    public class RibbonToolBarLayoutDefinition : DependencyObject
    {
        #region Size

        /// <summary>
        /// Gets or sets Size for the element.
        /// </summary>
        public RibbonControlSize Size
        {
            get => (RibbonControlSize)this.GetValue(SizeProperty);
            set => this.SetValue(SizeProperty, value);
        }

        /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(RibbonToolBarLayoutDefinition));

        #endregion

        #region SizeDefinition

        /// <summary>
        /// Gets or sets SizeDefinition for element.
        /// </summary>
        public RibbonControlSizeDefinition SizeDefinition
        {
            get => (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty);
            set => this.SetValue(SizeDefinitionProperty, value);
        }

        /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(RibbonToolBarLayoutDefinition));

        #endregion

        #region Row Count

        /// <summary>
        /// Gets or sets count of rows in the ribbon toolbar
        /// </summary>
        public int RowCount
        {
            get => (int)this.GetValue(RowCountProperty);
            set => this.SetValue(RowCountProperty, value);
        }

        /// <summary>Identifies the <see cref="RowCount"/> dependency property.</summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(RibbonToolBarLayoutDefinition), new PropertyMetadata(3));

        #endregion

        /// <summary>Identifies the <see cref="ForSimplified"/> dependency property.</summary>
        public static readonly DependencyProperty ForSimplifiedProperty =
            DependencyProperty.Register(nameof(ForSimplified), typeof(bool), typeof(RibbonToolBarLayoutDefinition), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether the layout definition should be used in simplified state.
        /// </summary>
        public bool ForSimplified
        {
            get => (bool)this.GetValue(ForSimplifiedProperty);
            set => this.SetValue(ForSimplifiedProperty, BooleanBoxes.Box(value));
        }

        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonToolBarRow> Rows { get; } = new();
    }
}