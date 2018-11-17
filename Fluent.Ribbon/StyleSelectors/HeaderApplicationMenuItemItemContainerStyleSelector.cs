namespace Fluent.StyleSelectors
{
    using System.Windows;
    using System.Windows.Controls;
    using MenuItem = Fluent.MenuItem;

    /// <summary>
    /// <see cref="StyleSelector"/> for <see cref="ItemsControl.ItemContainerStyle"/> in <see cref="MenuItem"/> with style HeaderApplicationMenuItemTemplate.
    /// </summary>
    public class HeaderApplicationMenuItemItemContainerStyleSelector : StyleSelector
    {
        /// <summary>
        ///     A singleton instance for <see cref="HeaderApplicationMenuItemItemContainerStyleSelector" />.
        /// </summary>
        public static HeaderApplicationMenuItemItemContainerStyleSelector Instance { get; } = new HeaderApplicationMenuItemItemContainerStyleSelector();

        /// <inheritdoc />
        public override Style SelectStyle(object item, DependencyObject container)
        {
            switch (item)
            {
                case MenuItem _:
                    return (container as FrameworkElement)?.TryFindResource("ApplicationMenuSecondLevelStyle") as Style;
            }

            return base.SelectStyle(item, container);
        }
    }
}