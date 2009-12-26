#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents toggle button
    /// </summary>
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleButton), new UIPropertyMetadata(false,OnIsCheckedChanged));

        // handles isChecked changed
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

        /// <summary>
        /// Occured then the toggle button has been checked
        /// </summary>
        public event EventHandler Checked;
        /// <summary>
        /// Occured then the toggle button has been unchecked
        /// </summary>
        public event EventHandler Unchecked;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ToggleButton()
        {
            StyleProperty.OverrideMetadata(typeof(ToggleButton), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));            
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton), new FrameworkPropertyMetadata(typeof(ToggleButton)));
        }

        // Coerce control style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null) basevalue = ThemesManager.DefaultToggleButtonStyle;
            return basevalue;
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public ToggleButton()
        {
            
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="e">The event data</param>
        protected override void OnClick(RoutedEventArgs e)
        {
            IsChecked = !IsChecked;
            base.OnClick(e);
        }

        #endregion
    }
}
