namespace Fluent.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Class which enables conversion from <see cref="string"/> to <see cref="RibbonGroupBoxStateDefinition"/>
    /// </summary>
    public class RibbonGroupBoxStateDefinitionConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.IsAssignableFrom(typeof(string));
        }

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new RibbonGroupBoxStateDefinition(value as string);
        }
    }
}
