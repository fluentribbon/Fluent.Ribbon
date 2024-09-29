namespace Fluent.Helpers;

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

/// <summary>
/// Helper class for drop downs.
/// </summary>
public static class DropDownHelper
{
    /// <summary>
    /// Coerces the maximum drop down height.
    /// </summary>
    public static object? CoerceMaxDropDownHeight(DependencyObject d, object? baseValue)
    {
        return baseValue is not double value
            ? baseValue
            : GetMaxDropDownHeight(d, value);
    }

    /// <summary>
    /// Gets the maximum drop down height.
    /// </summary>
    public static double GetMaxDropDownHeight(DependencyObject d, double baseValue)
    {
        if (double.IsNaN(baseValue) is false)
        {
            return baseValue;
        }

        if (Window.GetWindow(d) is not { } window)
        {
            return double.NaN;
        }

        var workingAreaHeight = Screen.FromHandle(new WindowInteropHelper(window).Handle).WorkingArea.Height;
        return Math.Floor(workingAreaHeight / 3D);
    }
}