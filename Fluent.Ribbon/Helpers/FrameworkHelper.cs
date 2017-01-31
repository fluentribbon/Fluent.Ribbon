// ReSharper disable once CheckNamespace
namespace Fluent
{
  using System;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Input;
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
            DependencyProperty.RegisterAttached("UseLayoutRounding", typeof(bool), typeof(FrameworkHelper), new PropertyMetadata(BooleanBoxes.FalseBox, OnUseLayoutRoundingChanged));

        private static void OnUseLayoutRoundingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(UIElement.SnapsToDevicePixelsProperty, true);
            RenderOptions.SetClearTypeHint(d, ClearTypeHint.Enabled);
            d.SetValue(FrameworkElement.UseLayoutRoundingProperty, true);
        }

    public static bool NavigateFocus(this FrameworkElement self, FocusNavigationDirection dircetion)
    {

      try
      {
        var res = self.MoveFocus(new TraversalRequest(dircetion));
        return res;
      }
      catch (ArgumentNullException)
      {
        //Fluent sometimes throws an exception on load when dynamically created items are in the DropDownButton's items collection
      }
      return false;
    }

  }
}