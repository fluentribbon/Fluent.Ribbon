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
            get { return this.GetValue(LeftContentProperty); }
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
            get { return this.GetValue(RightContentProperty); }
            set { this.SetValue(RightContentProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="RightContent"/>.
        /// </summary>
        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register(nameof(RightContent), typeof(object), typeof(StartScreenTabControl));

        /// <summary>
        /// Defines the margin for <see cref="LeftContent"/>
        /// </summary>
        public Thickness LeftContentMargin
        {
            get { return (Thickness)this.GetValue(LeftContentMarginProperty); }
            set { this.SetValue(LeftContentMarginProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="LeftContentMargin"/>.
        /// </summary>
        public static readonly DependencyProperty LeftContentMarginProperty =
            DependencyProperty.Register(nameof(LeftContentMargin), typeof(Thickness), typeof(StartScreenTabControl), new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// Defines if the <see cref="WindowSteeringHelperControl"/> is enabled in this control
        /// </summary>
        public bool IsWindowSteeringHelperEnabled
        {
            get { return (bool)this.GetValue(IsWindowSteeringHelperEnabledProperty); }
            set { this.SetValue(IsWindowSteeringHelperEnabledProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsWindowSteeringHelperEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsWindowSteeringHelperEnabledProperty =
            DependencyProperty.Register(nameof(IsWindowSteeringHelperEnabled), typeof(bool), typeof(StartScreenTabControl), new PropertyMetadata(true));

        /// <summary>
        /// Static constructor.
        /// </summary>
        static StartScreenTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StartScreenTabControl), new FrameworkPropertyMetadata(typeof(StartScreenTabControl)));
        }
    }
}