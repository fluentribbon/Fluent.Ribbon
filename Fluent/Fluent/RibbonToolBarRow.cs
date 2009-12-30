using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents size definition for group box
    /// </summary>
    [ContentProperty("Children")]
    public class RibbonToolBarRow : DependencyObject
    {
        #region Fields

        // User defined rows
        ObservableCollection<UIElement> rows = new ObservableCollection<UIElement>();

        #endregion

        #region Properties

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
          "Size",
          typeof(RibbonControlSize),
          typeof(RibbonToolBarLayoutDefinition),
          new FrameworkPropertyMetadata(RibbonControlSize.Large)
        );

        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        #endregion

        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<UIElement> Rows
        {
            get { return rows; }
        }

        #endregion

        internal void Invalidate()
        {
            throw new NotImplementedException();
        }
    }
}
