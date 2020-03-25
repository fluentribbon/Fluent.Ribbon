namespace Fluent.Helpers
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal static class ItemsControlHelper
    {
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
            if (source.ItemsSource != null)
            {
                target.ItemsSource = source.ItemsSource;
                source.ItemsSource = null;
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
    }
}