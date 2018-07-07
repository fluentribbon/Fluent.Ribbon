namespace Fluent.Extensions
{
    using System.Windows;

    /// <summary>
    /// Class with extension methods for <see cref="FrameworkElement"/>.
    /// </summary>
    internal static class FrameworkElementExtensions
    {
        public static void ForceMeasure(this FrameworkElement element)
        {
            // Calling anything on not loaded elements makes no sense
            if (element.IsLoaded == false)
            {
                return;
            }

            element.InvalidateMeasure();

            element.UpdateLayout();
        }

        public static void ForceMeasureAndArrange(this FrameworkElement element)
        {
            // Calling anything on not loaded elements makes no sense
            if (element.IsLoaded == false)
            {
                return;
            }

            element.InvalidateMeasure();
            element.InvalidateArrange();

            element.UpdateLayout();
        }
    }
}