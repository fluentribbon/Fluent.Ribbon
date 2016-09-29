// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Helper control which enables easy embedding of window steering functions.
    /// </summary>
    public class WindowSteeringHelperControl : Border
    {
        /// <summary>
        /// Static constructor
        /// </summary>
        static WindowSteeringHelperControl()
        {
            BackgroundProperty.OverrideMetadata(typeof(WindowSteeringHelperControl), new FrameworkPropertyMetadata(Brushes.Transparent));
            IsHitTestVisibleProperty.OverrideMetadata(typeof(WindowSteeringHelperControl), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(WindowSteeringHelperControl), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
            VerticalAlignmentProperty.OverrideMetadata(typeof(WindowSteeringHelperControl), new FrameworkPropertyMetadata(VerticalAlignment.Stretch));
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (this.IsEnabled)
            {
                WindowSteeringHelper.HandleMouseLeftButtonDown(e, true, true);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (this.IsEnabled)
            {
                WindowSteeringHelper.ShowSystemMenuPhysicalCoordinates(this, e);
            }
        }
    }
}