namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Checks equality of value and the converter parameter.
    /// Returns <see cref="Visibility.Visible"/> if they are equal.
    /// Returns <see cref="Visibility.Collapsed"/> if they are NOT equal.
    /// </summary>
    public class EqualsToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <inheritdoc />
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value == parameter
                || (value is not null && value.Equals(parameter)))
            {
                return VisibilityBoxes.Visible;
            }

            return VisibilityBoxes.Collapsed;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}