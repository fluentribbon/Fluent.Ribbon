using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    /// <summary>
    /// Represents group separator menu item
    /// </summary>
    [ContentProperty(nameof(Header))]
    public class GroupSeparatorMenuItem : MenuItem
    {
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static GroupSeparatorMenuItem()
        {
            var type = typeof(GroupSeparatorMenuItem);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            IsEnabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, null, CoerceIsEnabledAndTabStop));
            IsTabStopProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, null, CoerceIsEnabledAndTabStop));
        }

        private static object CoerceIsEnabledAndTabStop(DependencyObject d, object basevalue)
        {
            return false;
        }
    }
}