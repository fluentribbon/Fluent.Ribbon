using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    public class GroupSeparator:Separator
    {
        

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(GroupSeparator), new UIPropertyMetadata(""));



        static GroupSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupSeparator),
                                                     new FrameworkPropertyMetadata(typeof(GroupSeparator)));
        }
    }
}
