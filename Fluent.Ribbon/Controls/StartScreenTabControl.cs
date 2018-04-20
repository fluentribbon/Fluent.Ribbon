// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;

    /// <summary>
    /// Control for representing the left and right side of the start screen.
    /// </summary>
    /// <remarks>
    /// To control some aspects of the left handed side of this control please use properties prefixed with "ItemsPanel*".
    /// </remarks>
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
        /// Static constructor.
        /// </summary>
        static StartScreenTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StartScreenTabControl), new FrameworkPropertyMetadata(typeof(StartScreenTabControl)));

            ItemsPanelMinWidthProperty.OverrideMetadata(typeof(StartScreenTabControl), new PropertyMetadata(342d));
        }
    }
}