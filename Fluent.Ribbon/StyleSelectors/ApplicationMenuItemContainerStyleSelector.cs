namespace Fluent.StyleSelectors
{
    using System.Windows;
    using System.Windows.Controls;
    using MenuItem = Fluent.MenuItem;

    /// <summary>
    /// <see cref="StyleSelector"/> for <see cref="ItemsControl.ItemContainerStyle"/> in <see cref="ApplicationMenu"/>.
    /// </summary>
    public class ApplicationMenuItemContainerStyleSelector : StyleSelector
    {
        /// <summary>
        ///     A singleton instance for <see cref="ApplicationMenuItemContainerStyleSelector" />.
        /// </summary>
        public static ApplicationMenuItemContainerStyleSelector Instance { get; } = new ApplicationMenuItemContainerStyleSelector();

        /// <inheritdoc />
        public override Style SelectStyle(object item, DependencyObject container)
        {
            switch (item)
            {
                case MenuItem _:
                    return (container as FrameworkElement)?.TryFindResource("ApplicationMenuStyle") as Style;
            }

            return base.SelectStyle(item, container);
        }
    }
}