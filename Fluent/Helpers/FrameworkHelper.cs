namespace Fluent
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;

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
            RenderOptions.SetClearTypeHint(d, ClearTypeHint.Enabled);
            d.SetValue(FrameworkElement.UseLayoutRoundingProperty, true);
#endif
        }

#if NET35
        /// <summary>
        /// Checks if an enum has a certain flag
        /// </summary>
        public static bool HasFlag(this Enum thisInstance, Enum flag)
        {
            ulong instanceVal = Convert.ToUInt64(thisInstance);
            ulong flagVal = Convert.ToUInt64(flag);

            return (instanceVal & flagVal) == flagVal;
        }
#endif
    }

#if NET35
    /// <summary>
    /// Forwards to <see cref="System.Windows.Media.Animation.LinearDoubleKeyFrame"/>
    /// </summary>
    public class EasingThicknessKeyFrame : System.Windows.Media.Animation.LinearThicknessKeyFrame
    {
        /// <summary>
        /// Gets or sets EasingFunction
        /// </summary>
        public object EasingFunction { get; set; }
    }

    /// <summary>
    /// Container for EasingMode
    /// </summary>
    public class CubicEase
    {
        /// <summary>
        /// Gets or sets EasingMode
        /// </summary>
        public object EasingMode { get; set; }
    }

    /// <summary>
    /// Forwards to <see cref="System.Windows.Media.Animation.LinearDoubleKeyFrame"/>
    /// </summary>
    public class EasingDoubleKeyFrame : System.Windows.Media.Animation.LinearDoubleKeyFrame
    {
    }
#else  // .NET 4.0 and above
    /// <summary>
    /// Forwards to <see cref="System.Windows.Media.Animation.EasingThicknessKeyFrame"/>
    /// </summary>
    public class EasingThicknessKeyFrame : System.Windows.Media.Animation.EasingThicknessKeyFrame
    {
    }

    /// <summary>
    /// Forwards to <see cref="System.Windows.Media.Animation.CubicEase"/>
    /// </summary>
    public class CubicEase : System.Windows.Media.Animation.CubicEase
    {
    }

    /// <summary>
    /// Forwards to <see cref="System.Windows.Media.Animation.EasingDoubleKeyFrame"/>
    /// </summary>
    public class EasingDoubleKeyFrame : System.Windows.Media.Animation.EasingDoubleKeyFrame
    {
    }
#endif
}