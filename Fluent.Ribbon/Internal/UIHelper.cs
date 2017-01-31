namespace Fluent.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Class with helper functions for UI related stuff
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal static class UIHelper
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
            foreach (var child in GetVisualChildren(parent))
            {
                var obj = child as T;

                if (obj != null
                    && predicate(obj))
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first visual child of type TChildItem by walking down the visual tree.
        /// </summary>
        /// <typeparam name="TChildItem">The type of visual child to find.</typeparam>
        /// <param name="parent">The parent element whose visual tree shall be walked down.</param>
        /// <returns>The first element of type TChildItem found in the visual tree is returned. If none is found, null is returned.</returns>
        public static TChildItem FindVisualChild<TChildItem>(DependencyObject parent) where TChildItem : DependencyObject
        {
            foreach (var child in GetVisualChildren(parent))
            {
                var item = child as TChildItem;

                if (item != null)
                {
                    return item;
                }

                var childOfChild = FindVisualChild<TChildItem>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all visual children of <paramref name="parent"/>.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject parent)
        {
            var visualChildrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < visualChildrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child != null)
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Finds the parent control of type <typeparamref name="T"/>.
        /// First looks at the visual tree and then at the logical tree to find the parent.
        /// </summary>
        /// <returns>The found visual/logical parent or null.</returns>
        public static T GetParent<T>(DependencyObject element)
            where T : DependencyObject
        {
            var item = element;

            while (item != null
                && item is T == false)
            {
                item = VisualTreeHelper.GetParent(item);
            }

            if (item == null)
            {
                item = element;

                while (item != null &&
                       item is T == false)
                {
                    item = LogicalTreeHelper.GetParent(item);
                }
            }

            return (T)item;
        }

        /// <summary>
        /// First checks if <paramref name="visual"/> is either a <see cref="AdornerDecorator"/> or <see cref="ScrollContentPresenter"/> and if it is returns it's <see cref="AdornerLayer"/>.
        /// If those checks yield no result <see cref="AdornerLayer.GetAdornerLayer"/> is called.
        /// </summary>
        /// <param name="visual">The visual element for which to find an adorner layer.</param>
        /// <returns>An adorner layer for the specified visual, or null if no adorner layer can be found.</returns>
        /// <exception cref="T:System.ArgumentNullException">Raised when visual is null.</exception>
        public static AdornerLayer GetAdornerLayer(Visual visual)
        {
            if (visual == null)
            {
                throw new ArgumentNullException(nameof(visual));
            }

            var decorator = visual as AdornerDecorator;
            if (decorator != null)
            {
                return decorator.AdornerLayer;
            }

            var scrollContentPresenter = visual as ScrollContentPresenter;
            if (scrollContentPresenter != null)
            {
                return scrollContentPresenter.AdornerLayer;
            }

            return AdornerLayer.GetAdornerLayer(visual);
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
          if (parent == null)
            return null;

          for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
          {
            var child = VisualTreeHelper.GetChild(parent, i);
            string controlName = child.GetValue(Control.NameProperty) as string;
            if (controlName == name)
            {
              return child as T;
            }
            else
            {
              T result = FindVisualChildByName<T>(child, name);
              if (result != null)
                return result;
            }
          }
          return null;
        }
  }
}