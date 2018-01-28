// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;
    using Fluent.Internal.KnownBoxes;

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
        /// <returns></returns>
        public static bool GetUseLayoutRounding(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseLayoutRoundingProperty);
        }

        /// <summary>
        /// Gets UseLayoutRounding attached property value
        /// </summary>
        public static void SetUseLayoutRounding(DependencyObject obj, bool value)
        {
            obj.SetValue(UseLayoutRoundingProperty, value);
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for UseLayoutRounding.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UseLayoutRoundingProperty =
            DependencyProperty.RegisterAttached("UseLayoutRounding", typeof(bool), typeof(FrameworkHelper), new PropertyMetadata(BooleanBoxes.FalseBox, OnUseLayoutRoundingChanged));

        private static void OnUseLayoutRoundingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(UIElement.SnapsToDevicePixelsProperty, true);
            RenderOptions.SetClearTypeHint(d, ClearTypeHint.Enabled);
            d.SetValue(FrameworkElement.UseLayoutRoundingProperty, true);
        }
    }
}