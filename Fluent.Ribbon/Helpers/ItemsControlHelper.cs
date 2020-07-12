namespace Fluent.Helpers
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal static class ItemsControlHelper
    {
        public static readonly DependencyProperty IsMovingItemsToDifferentControlProperty = DependencyProperty.RegisterAttached(
            "IsMovingItemsToDifferentControl", typeof(bool), typeof(ItemsControlHelper), new PropertyMetadata(default(bool)));

        public static void SetIsMovingItemsToDifferentControl(DependencyObject element, bool value)
        {
            element.SetValue(IsMovingItemsToDifferentControlProperty, value);
        }

        public static bool GetIsMovingItemsToDifferentControl(DependencyObject element)
        {
            return (bool)element.GetValue(IsMovingItemsToDifferentControlProperty);
        }

        public static ItemsControl ItemsControlFromItemContainer(DependencyObject container)
        {
            if (container is null)
            {
                return null;
            }

            var itemsControl = ItemsControl.ItemsControlFromItemContainer(container);

            if (!(itemsControl is null)
                && itemsControl != DependencyProperty.UnsetValue)
            {
                return itemsControl;
            }

            var visualParent = VisualTreeHelper.GetParent(container);
            if (!(visualParent is null))
            {
                itemsControl = ItemsControl.ItemsControlFromItemContainer(visualParent);
            }

            if (!(itemsControl is null)
                && itemsControl != DependencyProperty.UnsetValue)
            {
                return itemsControl;
            }

            if (container is FrameworkElement frameworkElement
                && frameworkElement.Parent != null)
            {
                itemsControl = ItemsControl.ItemsControlFromItemContainer(frameworkElement.Parent);
            }

            return itemsControl;
        }

        public static void MoveItemsToDifferentControl(ItemsControl source, ItemsControl target)
        {
            try
            {
                SetIsMovingItemsToDifferentControl(source, true);
                SetIsMovingItemsToDifferentControl(target, true);

                var itemsSource = source.ItemsSource;
                if (itemsSource != null)
                {
                    source.ItemsSource = null;
                    target.ItemsSource = itemsSource;
                }
                else
                {
                    while (source.Items.Count > 0)
                    {
                        var item = source.Items[0];
                        source.Items.Remove(item);
                        target.Items.Add(item);
                    }
                }
            }
            finally
            {
                SetIsMovingItemsToDifferentControl(source, false);
                SetIsMovingItemsToDifferentControl(target, false);
            }
        }
    }
}