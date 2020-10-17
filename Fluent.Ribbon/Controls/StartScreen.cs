// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents the container for the <see cref="StartScreenTabControl"/>.
    /// </summary>
    public class StartScreen : Backstage
    {
        private bool previousTitleBarIsCollapsed;

        /// <summary>
        /// Indicates whether the <see cref="StartScreen"/> has aleaady been shown or not.
        /// </summary>
        public bool Shown
        {
            get { return (bool)this.GetValue(ShownProperty); }
            set { this.SetValue(ShownProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="Shown"/>.
        /// </summary>
        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register(nameof(Shown), typeof(bool), typeof(StartScreen), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null));

        static StartScreen()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StartScreen), new FrameworkPropertyMetadata(typeof(StartScreen)));

            VisibilityProperty.OverrideMetadata(typeof(StartScreen), new PropertyMetadata(OnVisibilityChanged));
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StartScreen)d).UpdateIsTitleBarCollapsed();
        }

        private void UpdateIsTitleBarCollapsed()
        {
            var parentRibbon = GetParentRibbon(this);

            if (parentRibbon?.TitleBar != null)
            {
                if (this.IsOpen)
                {
                    parentRibbon.TitleBar.IsCollapsed = this.Visibility == Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Shows the <see cref="StartScreen"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="StartScreen"/> was made visible.
        /// <c>false</c> if the <see cref="StartScreen"/> was previously shown and was not made visible during this call.
        /// </returns>
        protected override bool Show()
        {
            if (this.Shown)
            {
                return false;
            }

            var parentRibbon = GetParentRibbon(this);

            if (parentRibbon?.TitleBar != null)
            {
                this.previousTitleBarIsCollapsed = parentRibbon.TitleBar.IsCollapsed;

                this.UpdateIsTitleBarCollapsed();
            }

            return this.Shown = base.Show();
        }

        /// <summary>
        /// Hides the <see cref="StartScreen" />.
        /// </summary>
        protected override void Hide()
        {
            var wasOpen = this.IsOpen;
            base.Hide();

            if (wasOpen)
            {
                var parentRibbon = GetParentRibbon(this);
                if (parentRibbon?.TitleBar != null)
                {
                    parentRibbon.TitleBar.IsCollapsed = this.previousTitleBarIsCollapsed;
                }
            }
        }
    }
}