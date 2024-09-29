namespace Fluent.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Used to convert from four double values to <see cref="CornerRadius"/>
/// </summary>
[ValueConversion(typeof(CornerRadius), typeof(CornerRadius))]
[ValueConversion(typeof(IConvertible[]), typeof(CornerRadius))]
public class CornerRadiusConverter : IValueConverter, IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var valuesToExtract = parameter is CornerRadiusPart cornerRadiusValue
            ? cornerRadiusValue
            : CornerRadiusPart.All;

        var source = (CornerRadius)value!;

        var topLeft = valuesToExtract.HasFlag(CornerRadiusPart.TopLeft) ? source.TopLeft : 0;
        var topRight = valuesToExtract.HasFlag(CornerRadiusPart.TopRight) ? source.TopRight : 0;
        var bottomRight = valuesToExtract.HasFlag(CornerRadiusPart.BottomRight) ? source.BottomRight : 0;
        var bottomLeft = valuesToExtract.HasFlag(CornerRadiusPart.BottomLeft) ? source.BottomLeft : 0;

        return new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #region Implementation of IMultiValueConverter

    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var valuesToExtract = parameter is CornerRadiusPart cornerRadiusValue
            ? cornerRadiusValue
            : CornerRadiusPart.All;

        var topLeft = valuesToExtract.HasFlag(CornerRadiusPart.TopLeft) ? TryConvertSingleValue(values[0]) : 0;
        var topRight = valuesToExtract.HasFlag(CornerRadiusPart.TopRight) ? TryConvertSingleValue(values[1]) : 0;
        var bottomRight = valuesToExtract.HasFlag(CornerRadiusPart.BottomRight) ? TryConvertSingleValue(values[2]) : 0;
        var bottomLeft = valuesToExtract.HasFlag(CornerRadiusPart.BottomLeft) ? TryConvertSingleValue(values[3]) : 0;

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

/// <summary>
/// Defines parts of a <see cref="CornerRadius"/>.
/// </summary>
[Flags]
public enum CornerRadiusPart
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// Top left.
    /// </summary>
    TopLeft = 1 << 1,

    /// <summary>
    /// Top right.
    /// </summary>
    TopRight = 1 << 2,

    /// <summary>
    /// Bottom right.
    /// </summary>
    BottomRight = 1 << 3,

    /// <summary>
    /// Bottom left.
    /// </summary>
    BottomLeft = 1 << 4,

    /// <summary>
    /// All parts.
    /// </summary>
    All = TopLeft | TopRight | BottomRight | BottomLeft
}