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
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            {
                if (value is float numericValue)
                {
                    return numericValue * -1;
                }
            }

            {
                if (value is double numericValue)
                {
                    return numericValue * -1;
                }
            }

            {
                if (value is int numericValue)
                {
                    return numericValue * -1;
                }
            }

            {
                if (value is long numericValue)
                {
                    return numericValue * -1;
                }
            }

            return value;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }
}