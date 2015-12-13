namespace Fluent.Internal
{
    using System.Windows.Controls;

    internal static class ItemsControlHelper
    {
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