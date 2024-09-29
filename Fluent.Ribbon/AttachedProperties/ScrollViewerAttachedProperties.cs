namespace Fluent;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

/// <summary>
/// Attached properties for <see cref="ScrollViewer"/>.
/// </summary>
[StyleTypedProperty(Property = "ScrollBarStyle", StyleTargetType = typeof(ScrollViewer))]
public class ScrollViewerAttachedProperties : DependencyObject
{
    /// <summary>
    /// Defines the <see cref="Style"/> to be used for the <see cref="ScrollBar"/> of an <see cref="ScrollViewer"/>.
    /// </summary>
    public static readonly DependencyProperty ScrollBarStyleProperty = DependencyProperty.RegisterAttached("ScrollBarStyle", typeof(Style), typeof(ScrollViewerAttachedProperties), new PropertyMetadata(default(Style)));

    /// <summary>Helper for setting <see cref="ScrollBarStyleProperty"/> on <paramref name="element"/>.</summary>
    /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ScrollBarStyleProperty"/> on.</param>
    /// <param name="value">ScrollBarStyle property value.</param>
    public static void SetScrollBarStyle(DependencyObject element, Style? value)
    {
        element.SetValue(ScrollBarStyleProperty, value);
    }

    /// <summary>Helper for getting <see cref="ScrollBarStyleProperty"/> from <paramref name="element"/>.</summary>
    /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ScrollBarStyleProperty"/> from.</param>
    /// <returns>ScrollBarStyle property value.</returns>
    [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
    public static Style? GetScrollBarStyle(DependencyObject element)
    {
        return (Style)element.GetValue(ScrollBarStyleProperty);
    }
}