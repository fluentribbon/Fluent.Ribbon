#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Represents group separator menu item
    /// </summary>
    public class GroupSeparatorMenuItem:Separator
    {
        /// <summary>
        /// Gets or sets test
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(GroupSeparatorMenuItem), new UIPropertyMetadata(""));

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static GroupSeparatorMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupSeparatorMenuItem), new FrameworkPropertyMetadata(typeof(GroupSeparatorMenuItem)));
            StyleProperty.OverrideMetadata(typeof(GroupSeparatorMenuItem), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(GroupSeparatorMenuItem));
            }

            return basevalue;
        }
    }
}
