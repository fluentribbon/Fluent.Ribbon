// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Represents size definition for group box
    /// </summary>
    [ContentProperty(nameof(Children))]
    [SuppressMessage("Microsoft.Naming", "CA1702", Justification = "We mean here 'bar row' instead of 'barrow'")]
    public class RibbonToolBarRow : DependencyObject
    {
        #region Properties

        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<DependencyObject> Children { get; } = new ObservableCollection<DependencyObject>();

        #endregion

        #region Initialization

        #endregion
    }
}