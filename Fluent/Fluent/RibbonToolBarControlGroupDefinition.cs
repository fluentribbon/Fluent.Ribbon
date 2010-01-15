using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represent logical container for toolbar items
    /// </summary>
    [ContentProperty("Children")]
    public class RibbonToolBarControlGroupDefinition : DependencyObject
    {
        #region Fields

        // User defined rows
        readonly ObservableCollection<RibbonToolBarControlDefinition> children = new ObservableCollection<RibbonToolBarControlDefinition>();

        #endregion

        #region Children Property

        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonToolBarControlDefinition> Children
        {
            get { return children; }
        }

        #endregion

        internal void Invalidate()
        {
            throw new NotImplementedException();
        }
    }
}