namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Windows.Data;

    public class SpinnerTextToValueConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.TextToDouble((string)value, (double)parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.DoubleToText((double)value, (string)parameter, culture);
        }

        public virtual double TextToDouble(string text, double previousValue, CultureInfo culture)
        {
            // Remove all except digits, signs and commas
            var stringBuilder = new StringBuilder();

            foreach (var symbol in text)
            {
                if (char.IsDigit(symbol) ||
                    symbol == ',' ||
                    symbol == '.' ||
                    (symbol == '-' && stringBuilder.Length == 0))
                {
                    stringBuilder.Append(symbol);
                }
            }

            text = stringBuilder.ToString();

            double doubleValue;

            if (double.TryParse(text, NumberStyles.Any, culture, out doubleValue) == false)
            {
                doubleValue = previousValue;
            }

            return doubleValue;
        }

        public virtual string DoubleToText(double value, string format, CultureInfo culture)
        {
            return value.ToString(format, culture);
        }
    }
}