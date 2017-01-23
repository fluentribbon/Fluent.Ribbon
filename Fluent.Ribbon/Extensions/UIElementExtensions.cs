namespace Fluent.Extensions
{
    using System.Windows;

    internal static class UIElementExtensions
    {
        public static void ForceMeasure(this UIElement element)
        {
            element.InvalidateMeasure();
            element.UpdateLayout();
        }

        public static void ForceMeasureAndArrange(this UIElement element)
        {
            element.InvalidateMeasure();
            element.InvalidateArrange();
            element.UpdateLayout();
        }
    }
}