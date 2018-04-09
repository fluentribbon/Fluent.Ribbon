namespace Fluent.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Input;
    using Fluent.Helpers;

    /// <summary>
    /// KeysConverter
    /// </summary>
    public class KeysConverter : TypeConverter
    {
        /// <inheritdoc/>
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc/>
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                KeysCollection keys = new KeysCollection();
                string[] stringList = ((string)value).Split(new char[] { ',' });

                foreach (string keyString in stringList)
                {
                    if (Enum.TryParse<Key>(keyString, out Key key))
                    {
                        keys.Add(key);
                    }
                }

                return keys;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return string.Join(",", (KeysCollection)value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
