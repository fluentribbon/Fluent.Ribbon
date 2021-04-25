// ReSharper disable once CheckNamespace

namespace Fluent
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Fluent.Internal;

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
                var horizontalOffsetBefore = this.ScrollInfo.HorizontalOffset;
                var verticalOffsetBefore = this.ScrollInfo.VerticalOffset;

                if (e.Delta < 0)
                {
                    this.ScrollInfo.MouseWheelDown();
                }
                else
                {
                    this.ScrollInfo.MouseWheelUp();
                }

                e.Handled = DoubleUtil.AreClose(horizontalOffsetBefore, this.ScrollInfo.HorizontalOffset) == false
                            || DoubleUtil.AreClose(verticalOffsetBefore, this.ScrollInfo.VerticalOffset) == false;
            }
        }
    }
}