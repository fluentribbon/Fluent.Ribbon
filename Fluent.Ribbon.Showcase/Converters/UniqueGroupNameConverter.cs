namespace FluentTest.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter which generates unique group names for example for a group of toggle buttons.
    /// This is done by using the hash code of the value, an underscore and the converter parameter as a string.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class UniqueGroupNameConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value?.GetHashCode()}_{(string)parameter}";
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}