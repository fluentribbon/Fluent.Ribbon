#pragma warning disable CS1591
namespace Fluent;

using System.Windows;
using Fluent.Internal.KnownBoxes;
using JetBrains.Annotations;

/// <summary>
/// Attached Properties for controls with an text input part
/// </summary>
[PublicAPI]
public class InputControlProperties : DependencyObject
{
    public static readonly DependencyProperty InputWidthProperty = DependencyProperty.RegisterAttached(
        "InputWidth", typeof(double), typeof(InputControlProperties), new PropertyMetadata(DoubleBoxes.NaN));

    public static void SetInputWidth(DependencyObject element, double value)
    {
        element.SetValue(InputWidthProperty, value);
    }

    public static double GetInputWidth(DependencyObject element)
    {
        return (double)element.GetValue(InputWidthProperty);
    }

    public static readonly DependencyProperty InputMinWidthProperty = DependencyProperty.RegisterAttached(
        "InputMinWidth", typeof(double), typeof(InputControlProperties), new PropertyMetadata(DoubleBoxes.Zero));

    public static void SetInputMinWidth(DependencyObject element, double value)
    {
        element.SetValue(InputMinWidthProperty, value);
    }

    public static double GetInputMinWidth(DependencyObject element)
    {
        return (double)element.GetValue(InputMinWidthProperty);
    }

    public static readonly DependencyProperty InputHeightProperty = DependencyProperty.RegisterAttached(
        "InputHeight", typeof(double), typeof(InputControlProperties), new PropertyMetadata(22D));

    public static void SetInputHeight(DependencyObject element, double value)
    {
        element.SetValue(InputHeightProperty, value);
    }

    public static double GetInputHeight(DependencyObject element)
    {
        return (double)element.GetValue(InputHeightProperty);
    }
}