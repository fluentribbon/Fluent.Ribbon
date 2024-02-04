namespace Fluent.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Used to convert from four double values to <see cref="CornerRadius"/>
/// </summary>
public class CornerRadiusConverter : IMultiValueConverter
{
    #region Implementation of IMultiValueConverter

    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var topLeft = TryConvertSingleValue(values[0]);
        var topRight = TryConvertSingleValue(values[1]);
        var bottomRight = TryConvertSingleValue(values[2]);
        var bottomLeft = TryConvertSingleValue(values[3]);

        return new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
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