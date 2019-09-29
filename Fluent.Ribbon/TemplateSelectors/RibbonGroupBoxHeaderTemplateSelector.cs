namespace Fluent.TemplateSelectors
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// <see cref="DataTemplateSelector"/> for the header of <see cref="RibbonGroupBox"/>.
    /// </summary>
    public class RibbonGroupBoxHeaderTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets a static instance of <see cref="RibbonGroupBoxHeaderTemplateSelector"/>.
        /// </summary>
        public static readonly RibbonGroupBoxHeaderTemplateSelector Instance = new RibbonGroupBoxHeaderTemplateSelector();

        /// <inheritdoc />
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = (FrameworkElement)container;

            if (RibbonGroupBox.GetIsCollapsedHeaderContentPresenter(element))
            {
                return (DataTemplate)element.FindResource("Fluent.Ribbon.Templates.RibbonGroupBox.TwoLineHeaderTemplate");
            }

            return (DataTemplate)element.FindResource("Fluent.Ribbon.Templates.RibbonGroupBox.OneLineHeaderTemplate");
        }
    }
}