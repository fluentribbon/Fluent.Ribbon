namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Used to convert from four double values to <see cref="Thickness"/>
    /// </summary>
    public class ThicknessConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var left = TryConvertSingleValue(values[0]);
            var top = TryConvertSingleValue(values[1]);
            var right = TryConvertSingleValue(values[2]);
            var bottom = TryConvertSingleValue(values[3]);

            return new Thickness(left, top, right, bottom);
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static double TryConvertSingleValue(object value)
        {
            try
            {
                return (value as IConvertible)?.ToDouble(null) ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}