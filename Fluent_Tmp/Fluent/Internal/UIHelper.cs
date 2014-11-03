namespace Fluent.Internal
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Class with helper functions for UI related stuff
    /// </summary>
    internal class UIHelper
    {
        /// <summary>
        /// Tries to find immediate visual child of type <typeparamref name="T"/> which matches <paramref name="predicate"/>
        /// </summary>
        /// <returns>
        /// The visual child of type <typeparamref name="T"/> that matches <paramref name="predicate"/>. 
        /// Returns <c>null</c> if no child matches.
        /// </returns>
        public static T FindImmediateVisualChild<T>(DependencyObject parent, Predicate<T> predicate)
            where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var obj = VisualTreeHelper.GetChild(parent, i) as T;

                if (obj != null
                    && predicate(obj))
                {
                    return obj;
                }
            }

            return null;
        }
    }
}