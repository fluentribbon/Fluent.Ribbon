namespace Fluent.Converters;

using System;
using System.Globalization;
using System.Windows.Data;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Converter used to invert a boolean value.
/// </summary>
#pragma warning disable WPF0072
[ValueConversion(typeof(bool), typeof(bool))]
#pragma warning restore WPF0072
public class InverseBoolConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return BooleanBoxes.Box(!(bool)value);
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return BooleanBoxes.Box(!(bool)value);
    }
}