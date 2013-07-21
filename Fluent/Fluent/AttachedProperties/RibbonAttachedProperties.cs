
namespace Fluent.AttachedProperties
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Attached Properties for the Fluent Ribbon library
    /// </summary>
    public class RibbonAttachedProperties
    {
        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.RegisterAttached("TitleBarHeight", typeof(double), typeof(RibbonAttachedProperties), new FrameworkPropertyMetadata(25D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        public static void SetTitleBarHeight(Visual element, double value)
        {
            element.SetValue(TitleBarHeightProperty, value);
        }

        public static double GetTitleBarHeight(Visual element)
        {
            return (double)element.GetValue(TitleBarHeightProperty);
        }
    }
}