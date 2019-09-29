namespace FluentTest.TemplateSelectors
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using FluentTest.Helpers;

    /// <summary>
    /// Provides a means to specify DataTemplates to be selected from within WPF code
    /// </summary>
    public class DynamicTemplateSelector : DataTemplateSelector
    {
        /// <summary> Generic attached property specifying <see cref="DataTemplate"/>s used by the <see cref="DynamicTemplateSelector"/></summary>
        /// <remarks>
        /// This attached property will allow you to set the templates you wish to be available whenever
        /// a control's TemplateSelector is set to an instance of <see cref="DynamicTemplateSelector"/>
        /// </remarks>
        public static readonly DependencyProperty TemplatesCollectionProperty =
            DependencyProperty.RegisterAttached("TemplatesCollection", typeof(TemplateCollection),
                typeof(DynamicTemplateSelector),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));

        /// <summary> Gets the value of the <paramref name="target"/>'s attached <see cref="TemplatesCollectionProperty"/> </summary>
        /// <param name="target">The <see cref="UIElement"/> who's attached template's property you wish to retrieve</param>
        /// <returns>The templates used by the givem <paramref name="target"/> when using the <see cref="DynamicTemplateSelector"/></returns>
        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        public static TemplateCollection GetTemplatesCollection(DependencyObject target)
        {
            return (TemplateCollection)target.GetValue(TemplatesCollectionProperty);
        }

        /// <summary> Sets the value of the <paramref name="target"/>'s attached <see cref="TemplatesCollectionProperty"/> </summary>
        /// <param name="target">The element to set the property on</param>
        /// <param name="value">The collection of <see cref="DataTemplate"/>s to apply to this element</param>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public static void SetTemplatesCollection(DependencyObject target, TemplateCollection value)
        {
            target.SetValue(TemplatesCollectionProperty, value);
        }

        /// <summary>
        /// Overriden base method to allow the selection of the correct DataTemplate
        /// </summary>
        /// <param name="item">The item for which the template should be retrieved</param>
        /// <param name="container">The object containing the current item</param>
        /// <returns>The <see cref="DataTemplate"/> to use when rendering the <paramref name="item"/></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //This should ensure that the item we are getting is in fact capable of holding our property
            //before we attempt to retrieve it.
            if (container != null)
            {
                var templates = GetTemplatesCollection(container);

                if (templates != null
                    && templates.Count != 0)
                {
                    foreach (var template in templates)
                    {
                        //In this case, we are checking whether the type of the item
                        //is the same as the type supported by our DataTemplate
                        var dataType = template.DataType as Type;
                        if (dataType != null
                            && dataType.IsInstanceOfType(item))
                        {
                            //And if it is, then we return that DataTemplate
                            return template;
                        }
                    }
                }
            }

            //If all else fails, then we go back to using the default DataTemplate
            return new DataTemplate();
        }
    }
}