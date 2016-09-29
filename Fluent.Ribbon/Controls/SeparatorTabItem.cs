using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents separator to use in the TabControl
    /// </summary>
    public class SeparatorTabItem : TabItem
    {
        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static SeparatorTabItem()
        {
            var type = typeof(SeparatorTabItem);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            IsEnabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, null, CoerceIsEnabledAndTabStop));
            IsTabStopProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, null, CoerceIsEnabledAndTabStop));
            IsSelectedProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnIsSelectedChanged));
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                return;
            }

            var separatorTabItem = (SeparatorTabItem)d;
            var tabControl = separatorTabItem.Parent as TabControl;

            if (tabControl == null
                || tabControl.Items.Count <= 1)
            {
                return;
            }

            tabControl.SelectedIndex = tabControl.SelectedIndex == tabControl.Items.Count - 1
                ? tabControl.SelectedIndex - 1
                : tabControl.SelectedIndex + 1;
        }

        private static object CoerceIsEnabledAndTabStop(DependencyObject d, object basevalue)
        {
            return false;
        }

        #endregion
    }
}
