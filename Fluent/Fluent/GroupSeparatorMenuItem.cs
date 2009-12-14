using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupSeparatorMenuItem),
                                                     new FrameworkPropertyMetadata(typeof(GroupSeparatorMenuItem)));
        }
    }
}
