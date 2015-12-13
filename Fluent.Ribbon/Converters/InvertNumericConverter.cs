namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Used to invert numbers
    /// </summary>
    public class InvertNumericConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            {
                var numericValue = value as float?;

                if (numericValue != null)
                {
                    return numericValue * -1;
                }
            }

            {
                var numericValue = value as double?;

                if (numericValue != null)
                {
                    return numericValue * -1;
                }
            }

            {
                var numericValue = value as int?;

                if (numericValue != null)
                {
                    return numericValue * -1;
                }
            }

            {
                var numericValue = value as long?;

                if (numericValue != null)
                {
                    return numericValue * -1;
                }
            }

            return value;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }

        #endregion
    }
}