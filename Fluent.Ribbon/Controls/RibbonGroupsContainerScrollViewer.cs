// ReSharper disable once CheckNamespace

namespace Fluent
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Represents a <see cref="ScrollViewer" /> specific to <see cref="RibbonGroupsContainer" />.
    /// </summary>
    public class RibbonGroupsContainerScrollViewer : ScrollViewer
    {
        static RibbonGroupsContainerScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroupsContainerScrollViewer), new FrameworkPropertyMetadata(typeof(RibbonGroupsContainerScrollViewer)));
        }

        /// <inheritdoc />
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (this.ScrollInfo != null)
            {
                if (e.Delta < 0)
                {
                    this.LineRight();
                }
                else
                {
                    this.LineLeft();
                }

                e.Handled = true;
            }
        }
    }
}