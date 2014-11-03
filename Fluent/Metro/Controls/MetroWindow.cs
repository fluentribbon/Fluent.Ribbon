using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Fluent.Metro.Native;

namespace Fluent
{
    //public class MetroWindow : Window
    //{
    //    static MetroWindow()
    //    {            
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
    //        StyleProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(null, OnCoerceStyle));
    //    }

    //    private static object OnCoerceStyle(DependencyObject d, object basevalue)
    //    {
    //        if (basevalue != null)
    //        {
    //            return basevalue;
    //        }

    //        var frameworkElement = d as FrameworkElement;
    //        if (frameworkElement != null)
    //        {
    //            basevalue = frameworkElement.TryFindResource(typeof(MetroWindow));
    //        }

    //        return basevalue;
    //    }
    //}
}