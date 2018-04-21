namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Windows.Data;

    /// <summary>
    /// Converter class which converts from <see cref="string"/> to <see cref="double"/> and back.
    /// </summary>
    public class SpinnerTextToValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converterParam = (Tuple<string, double>)parameter;
            var format = converterParam.Item1;
            var previousValue = converterParam.Item2;

            return this.TextToDouble((string)value, format, previousValue, culture);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.DoubleToText((double)value, (string)parameter, culture);
        }

        /// <summary>
        /// Converts the given <paramref name="text"/> to a <see cref="double"/>.
        /// </summary>
        /// <returns>The <see cref="double"/> value converted from <paramref name="text"/> or <paramref name="previousValue"/> if the conversion fails.</returns>
        public virtual double TextToDouble(string text, string format, double previousValue, CultureInfo culture)
        {
            // Remove all except digits, signs and commas
            var stringBuilder = new StringBuilder();

            foreach (var symbol in text)
            {
                if (char.IsDigit(symbol)
                    || symbol == ','
                    || symbol == '.'
                    || (symbol == '-' && stringBuilder.Length == 0))
                {
                    stringBuilder.Append(symbol);
                }
            }

            text = stringBuilder.ToString();

            if (double.TryParse(text, NumberStyles.Any, culture, out var doubleValue) == false)
            {
                doubleValue = previousValue;
            }

            return doubleValue;
        }

        /// <summary>
        /// Converts <paramref name="value"/> to a formatted text using <paramref name="format"/>.
        /// </summary>
        /// <returns><paramref name="value"/> converted to a <see cref="string"/>.</returns>
        public virtual string DoubleToText(double value, string format, CultureInfo culture)
        {
            return value.ToString(format, culture);
        }
    }
}