using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Fluent
{
    [ContentProperty("Text")]
    public class ToggleButton:Button
    {
        #region Properties

        #region IsChecked

        /// <summary>
        /// Get or set ToggleButton checked state
        /// </summary>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleButton), new UIPropertyMetadata(false,OnIsCheckedChanged));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;
            ToggleButton button = d as ToggleButton;
            if(oldValue!=newValue)
            {
                if(newValue)
                {
                    if (button.Checked != null) button.Checked(button, EventArgs.Empty);
                }
                else
                {
                    if (button.Unchecked != null) button.Unchecked(button, EventArgs.Empty);
                }
            }
        }

        #endregion
        
        #endregion

        #region Events

        public event EventHandler Checked;
        public event EventHandler Unchecked;

        #endregion

        #region Constructors

        static ToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton), new FrameworkPropertyMetadata(typeof(ToggleButton)));
        }

        public ToggleButton()
        {
            
        }

        #endregion

        #region Overrides

        protected override void OnClick(RoutedEventArgs e)
        {
            IsChecked = !IsChecked;
            base.OnClick(e);
        }

        #endregion
    }
}
