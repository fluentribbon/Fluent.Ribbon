using System;
using System.Windows;

namespace Fluent
{
    /// <summary>
    /// Represents additional context menu service
    /// </summary>
    public static class ContextMenuService
    {
        /// <summary>
        /// Attach needed parameters to control
        /// </summary>
        /// <param name="type"></param>
        public static void Attach(Type type)
        {
            System.Windows.Controls.ContextMenuService.ShowOnDisabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(true));
            FrameworkElement.ContextMenuProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, OnContextMenuChanged, CoerceContextMenu));
        }

        private static void OnContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(FrameworkElement.ContextMenuProperty);
        }

        private static object CoerceContextMenu(DependencyObject d, object basevalue)
        {
            var control = d as IQuickAccessItemProvider;
            if (basevalue == null
                && (control == null || control.CanAddToQuickAccessToolBar))
            {
                return Ribbon.RibbonContextMenu;
            }

            return basevalue;
        }

        /// <summary>
        /// Coerce control context menu
        /// </summary>
        /// <param name="o">Control</param>
        public static void Coerce(DependencyObject o)
        {
            o.CoerceValue(FrameworkElement.ContextMenuProperty);
        }
    }
}