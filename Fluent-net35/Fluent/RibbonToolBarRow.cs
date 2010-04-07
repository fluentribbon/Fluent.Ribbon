using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents size definition for group box
    /// </summary>
    [ContentProperty("Children")]
    [SuppressMessage("Microsoft.Naming", "CA1702", Justification = "We mean here 'bar row' instead of 'barrow'")]
    public class RibbonToolBarRow : DependencyObject
    {
        #region Fields

        // User defined rows
        readonly ObservableCollection<DependencyObject> children = new ObservableCollection<DependencyObject>();

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<DependencyObject> Children
        {
            get { return children; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBarRow(){}

        #endregion
    }
}
