namespace Fluent.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Input;
    using Fluent.Helpers;

    /// <summary>
    /// Type converter used to convert a string to a list of keys.
    /// </summary>
    public class KeysConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from. </param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture. </param><param name="value">The <see cref="T:System.Object"/> to convert. </param><exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                KeyTipKeysCollection keys = new KeyTipKeysCollection();
                string[] stringList = ((string)value).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in stringList)
                {
                    if (Enum.TryParse<Key>(s.Trim(), out Key key))
                    {
                        keys.Add(key);
                    }
                }

                return keys;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
