using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// Represents KeyTip control
    /// </summary>
    public class KeyTip : Label
    {
        #region Keys Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeysProperty = DependencyProperty.RegisterAttached(
          "Keys",
          typeof(string),
          typeof(KeyTip),
          new FrameworkPropertyMetadata(null, KeysPropertyChanged)
        );

        static void KeysPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Sets value of attached property Keys for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetKeys(UIElement element, string value)
        {
            element.SetValue(KeysProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Keys of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Keys"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("KeyTips"),
        System.ComponentModel.Description("Key sequence for the given element")]
        public static string GetKeys(UIElement element)
        {
            return (string)element.GetValue(KeysProperty);
        }

        #endregion

        #region Initialization
        
        // Static constructor
        static KeyTip()
        {
            // Override metadata to allow slyling
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyTip), new FrameworkPropertyMetadata(typeof(KeyTip)));
        }
        
        /// <summary>
        /// Default constrctor
        /// </summary>
        public KeyTip()
        {
        }

        #endregion
    }
}
