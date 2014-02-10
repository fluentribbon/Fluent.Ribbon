using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents size definition for group box
    /// </summary>
    [ContentProperty("Rows")]
    public class RibbonToolBarLayoutDefinition : DependencyObject
    {
        #region Fields

        // User defined rows
        ObservableCollection<RibbonToolBarRow> rows = new ObservableCollection<RibbonToolBarRow>();

        #endregion

        #region Properties

       
        #region Row Count

        /// <summary>
        /// Gets or sets count of rows in the ribbon toolbar
        /// </summary>
        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RowCount.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(RibbonToolBar), new UIPropertyMetadata(3));


        #endregion


        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonToolBarRow> Rows
        {
            get { return rows; }
        }

        #endregion
    }
}
