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

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (this.IsEnabled)
            {
                WindowSteeringHelper.HandleMouseLeftButtonDown(e, true, true);
            }
        }

        /// <inheritdoc />
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (this.IsEnabled)
            {
                WindowSteeringHelper.ShowSystemMenu(this, e);
            }
        }
    }
}