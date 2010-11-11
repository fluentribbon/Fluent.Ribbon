using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents class to determine .NET Framework version difference
    /// </summary>
    public static class FrameworkHelper
    {
        /// <summary>
        /// Version of WPF
        /// </summary>
        public static readonly Version PresentationFrameworkVersion = Assembly.GetAssembly(typeof(Window)).GetName().Version;


        /// <summary>
        /// Gets UseLayoutRounding attached property value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetUseLayoutRounding(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseLayoutRoundingProperty);
        }

        /// <summary>
        /// Gets UseLayoutRounding attached property value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetUseLayoutRounding(DependencyObject obj, bool value)
        {
            obj.SetValue(UseLayoutRoundingProperty, value);
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for UseLayoutRounding.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UseLayoutRoundingProperty =
            DependencyProperty.RegisterAttached("UseLayoutRounding", typeof(bool), typeof(FrameworkHelper), new UIPropertyMetadata(false, OnUseLayoutRoundingChanged));

        private static void OnUseLayoutRoundingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(UIElement.SnapsToDevicePixelsProperty, true);
#if NET35
#else
            d.SetValue(FrameworkElement.UseLayoutRoundingProperty, true);            
            TextOptions.SetTextFormattingMode(d, TextFormattingMode.Display);
            RenderOptions.SetClearTypeHint(d, ClearTypeHint.Enabled);
#endif
        }
    }
}
