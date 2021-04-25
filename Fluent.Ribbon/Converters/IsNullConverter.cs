namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    ///     Converts <c>null</c> to <c>true</c> and not <c>null</c> to <c>false</c>.
    /// </summary>
    public sealed class IsNullConverter : IValueConverter
    {
        /// <summary>
        ///     A singleton instance for <see cref="IsNullConverter" />.
        /// </summary>
        public static readonly IsNullConverter Instance = new IsNullConverter();

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}