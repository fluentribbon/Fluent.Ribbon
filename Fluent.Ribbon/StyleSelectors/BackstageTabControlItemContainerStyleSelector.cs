namespace Fluent.StyleSelectors
{
    using System.Windows;
    using System.Windows.Controls;
    using Button = Fluent.Button;

    /// <summary>
    /// <see cref="StyleSelector"/> for <see cref="ItemsControl.ItemContainerStyle"/> in <see cref="BackstageTabControl"/>.
    /// </summary>
    public class BackstageTabControlItemContainerStyleSelector : StyleSelector
    {
        /// <summary>
        ///     A singleton instance for <see cref="BackstageTabControlItemContainerStyleSelector" />.
        /// </summary>
        public static BackstageTabControlItemContainerStyleSelector Instance { get; } = new BackstageTabControlItemContainerStyleSelector();

        /// <inheritdoc />
        public override Style? SelectStyle(object item, DependencyObject container)
        {
            switch (item)
            {
                case Button _:
                    return (container as FrameworkElement)?.TryFindResource("Fluent.Ribbon.Styles.BackstageTabControl.Button") as Style;

                case SeparatorTabItem _:
                    return (container as FrameworkElement)?.TryFindResource("Fluent.Ribbon.Styles.BackstageTabControl.SeparatorTabItem") as Style;
            }

            return base.SelectStyle(item, container);
        }
    }
}