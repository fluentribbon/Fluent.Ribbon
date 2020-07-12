namespace Fluent.Helpers
{
    using System;
    using System.Windows;

    /// <summary>
    /// Helper functions for classes implementing <see cref="ILogicalChildSupport"/>.
    /// </summary>
    public static class LogicalChildSupportHelper
    {
        /// <summary>
        /// Called when <see cref="RibbonControl.IconProperty"/> changes.
        /// </summary>
        public static void OnLogicalChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var logicalChildSupport = d as ILogicalChildSupport ?? throw new ArgumentException("Argument must be of type ILogicalChildSupport.", nameof(d));

            if (e.OldValue is DependencyObject oldValue)
            {
                logicalChildSupport.RemoveLogicalChild(oldValue);
            }

            if (e.NewValue is DependencyObject newValue
                && LogicalTreeHelper.GetParent(newValue) == null)
            {
                logicalChildSupport.AddLogicalChild(newValue);
            }
        }
    }
}