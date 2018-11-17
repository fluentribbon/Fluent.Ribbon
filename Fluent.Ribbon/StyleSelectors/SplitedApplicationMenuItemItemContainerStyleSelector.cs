namespace Fluent.StyleSelectors
{
    using System.Windows;
    using System.Windows.Controls;
    using MenuItem = Fluent.MenuItem;

    /// <summary>
    /// <see cref="StyleSelector"/> for <see cref="ItemsControl.ItemContainerStyle"/> in <see cref="MenuItem"/> with style SplitedApplicationMenuItem.
    /// </summary>
    public class SplitedApplicationMenuItemItemContainerStyleSelector : StyleSelector
    {
        /// <summary>
        ///     A singleton instance for <see cref="HeaderApplicationMenuItemItemContainerStyleSelector" />.
        /// </summary>
        public static SplitedApplicationMenuItemItemContainerStyleSelector Instance { get; } = new SplitedApplicationMenuItemItemContainerStyleSelector();

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