using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents group separator menu item
    /// </summary>
    [ContentProperty("Header")]
    public class GroupSeparatorMenuItem: MenuItem
    {
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static GroupSeparatorMenuItem()
        {
            var type = typeof (GroupSeparatorMenuItem);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            StyleProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            IsEnabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, null, CoerceIsEnabledAndTabStop));
            IsTabStopProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, null, CoerceIsEnabledAndTabStop));
        }

        private static object CoerceIsEnabledAndTabStop(DependencyObject d, object basevalue)
        {
            return false;
        }

        // Coerce object style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(GroupSeparatorMenuItem));
            }

            return basevalue;
        }
    }
}