using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Fluent
{
    [ContentProperty("Text")]
    public class CheckBox:ToggleButton
    {
        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static CheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBox), new FrameworkPropertyMetadata(typeof(CheckBox)));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CheckBox()
        {
            
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            CheckBox button = new CheckBox();
            button.VerticalAlignment = VerticalAlignment.Center;
            BindQuickAccessItem(button);
            return button;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            CheckBox button = element as CheckBox;
            Bind(this, element, "IsChecked", IsCheckedProperty, BindingMode.TwoWay);
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
