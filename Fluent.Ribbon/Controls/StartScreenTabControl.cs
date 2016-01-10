namespace Fluent
{
    using System.Windows;

    /// <summary>
    /// Control for representing the left and right side of the start screen.
    /// </summary>
    public class StartScreenTabControl : BackstageTabControl
    {
        /// <summary>
        /// Left side panel content of the startscreen.
        /// </summary>
        public object LeftContent
        {
            get { return (object)this.GetValue(LeftContentProperty); }
            set { this.SetValue(LeftContentProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="LeftContent"/>.
        /// </summary>
        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register(nameof(LeftContent), typeof(object), typeof(StartScreenTabControl));

        /// <summary>
        /// Right side panel content of the startscreen.
        /// </summary>
        public object RightContent
        {
            get { return (object)this.GetValue(RightContentProperty); }
            set { this.SetValue(RightContentProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="RightContent"/>.
        /// </summary>
        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register(nameof(RightContent), typeof(object), typeof(StartScreenTabControl));

        /// <summary>
        /// Static constructor.
        /// </summary>
        static StartScreenTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StartScreenTabControl), new FrameworkPropertyMetadata(typeof(StartScreenTabControl)));
        }
    }
}